using System;
using System.IO;
using System.Linq;
using System.Text;
using Playdarium.PackageTool.Tools;
using UnityEditor;
using UnityEngine;

namespace Playdarium.PackageTool
{
	/// <summary>
	/// Continuous-integration API for package tools.
	/// </summary>
	public static class CIUtils
	{
		// Command-line arguments
		private const string ID_ARG_KEY = "id";
		private const string VERSION_ARG_KEY = "version";
		private const string PREVIEW_ARG_KEY = "preview";
		private const string GENERATE_VERSION_CONSTANTS_ARG_KEY = "generateversionconstants";

		// Logs
		private const string LOG_PREFIX = "[Package Tools] ";
		private const string CI_STARTING = LOG_PREFIX + "Starting Package Tools CI...";
		private const string CI_COMPLETE = LOG_PREFIX + "Package Tools CI has completed!";
		private const string CI_COMMAND_LINE_ARGS_PARSED_FORMAT = LOG_PREFIX + "Parsed CI Command Line Args.\n\n{0}";

		private const string CI_USING_ONLY_CONFIGS_FROM_ARG =
			LOG_PREFIX + "Using configs matching IDs passed in 'ID' argument";

		private const string CI_FOUND_CONFIGS = LOG_PREFIX + "Found [{0}] configs in project.";
		private const string CI_USING_ALL_CONFIGS = LOG_PREFIX + "Using all configs in project.";
		private const string CI_GENERATION_STARTING = LOG_PREFIX + "Starting to generate packages...";

		private const string CI_PACKAGE_NOT_FOUND_FORMAT =
			LOG_PREFIX + "Could not find a package for ID: [{0}], skipping it.";

		private const string CI_PACKAGE_FOUND_FORMAT = LOG_PREFIX + "Package [{0}] found for ID: [{1}]";

		private const string CI_GENERATING_LEGACY_PACKAGE_FORMAT =
			LOG_PREFIX + "Generating Legacy Package for Config [{0}] with ID: [{1}]";

		private const string CI_SKIPPING_LEGACY_PACKAGE_FORMAT = LOG_PREFIX +
		                                                         "Skipping Legacy Package for Config [{0}] with ID: [{1}] as no output path is present in config.";

		private const string CI_GENERATING_PACKAGE_SOURCE_FORMAT =
			LOG_PREFIX + "Generating Package Source for Config [{0}] with ID: [{1}].";

		private const string CI_SKIPPING_PACKAGE_SOURCE_FORMAT = LOG_PREFIX +
		                                                         "Skipping Package Source for Config [{0}] with ID: [{1}] as no output path is present in config.";

		private static readonly StringBuilder StringBuilder;

		private static bool IsRunningOnCI => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

		static CIUtils()
		{
			StringBuilder = new StringBuilder(8192);
		}

		/// <summary>
		/// Attempts to use zero or more <see cref="PackageManifestConfig"/> assets to generate legacy Unity packages
		/// and Unity source.
		/// </summary>
		public static void Generate()
		{
			Debug.Log(CI_STARTING);

			PrepareDll();

			try
			{
				EditorApplication.LockReloadAssemblies();
				AssetDatabase.StartAssetEditing();

				// Get command line args and log them
				var commandLineArgs = CommandLineTools.GetKVPCommandLineArguments();

				StringBuilder.Clear();
				const string cliArgFormat = "{0} => {1}";
				foreach (var commandLineArg in commandLineArgs)
				{
					StringBuilder.AppendFormat(cliArgFormat, commandLineArg.Key, commandLineArg.Value);
					StringBuilder.AppendLine();
				}

				Debug.LogFormat(CI_COMMAND_LINE_ARGS_PARSED_FORMAT, StringBuilder);

				// Attempt to parse CLI-passed version, if present.
				string version = null;
				if (commandLineArgs.TryGetValue(VERSION_ARG_KEY, out var versionRawValue))
					version = (string)versionRawValue;

				// Attempt to parse CLI-passed version, if present.
				bool? isPreview = null;
				if (commandLineArgs.TryGetValue(PREVIEW_ARG_KEY, out var previewRawValue))
					isPreview = bool.Parse((string)previewRawValue);

				// See if we should generate version constants
				var generateVersionConstants = false;
				if (commandLineArgs.TryGetValue(
					    GENERATE_VERSION_CONSTANTS_ARG_KEY,
					    out var generateVersionConstantsRawValue))
					generateVersionConstants = bool.Parse(generateVersionConstantsRawValue.ToString());

				// Get all package manifests in project
				var allPackageManifestConfigs = PackageManifestTools.GetAllConfigs();

				Debug.LogFormat(CI_FOUND_CONFIGS, allPackageManifestConfigs.Length);

				// Check to see if any IDs have been passed for specific configs
				string[] configIds;
				if (commandLineArgs.TryGetValue(ID_ARG_KEY, out var arg))
				{
					Debug.Log(CI_USING_ONLY_CONFIGS_FROM_ARG);

					const char commaChar = ',';
					var idArgValue = arg.ToString();
					configIds = idArgValue.Split(commaChar);
				}
				// Otherwise generate all package manifest configs in project.
				else
				{
					Debug.Log(CI_USING_ALL_CONFIGS);

					configIds = allPackageManifestConfigs.Select(x => x.Id).ToArray();
				}

				// For each matching config ID, find the matching package manifest config and generate any relevant packages.
				Debug.Log(CI_GENERATION_STARTING);
				foreach (var configId in configIds)
				{
					var matchingConfig = allPackageManifestConfigs.FirstOrDefault(x =>
						string.Compare(x.Id, configId, StringComparison.OrdinalIgnoreCase) == 0);

					// If a config cannot be found matching config id, skip it and continue.
					if (matchingConfig == null)
					{
						Debug.LogWarningFormat(CI_PACKAGE_NOT_FOUND_FORMAT, configId);

						continue;
					}

					var configName = matchingConfig.name;

					Debug.LogFormat(CI_PACKAGE_FOUND_FORMAT, configName, configId);

					// Set the CLI passed version if present, otherwise default to checked in version number.
					if (!string.IsNullOrEmpty(version))
					{
						matchingConfig.packageVersion = version;
						EditorUtility.SetDirty(matchingConfig);
					}

					// If set to generate version constants, do so.
					if (generateVersionConstants) CodeGenTools.GenerateVersionConstants(matchingConfig);

					// Otherwise generate the corresponding legacy unity package and package source if their output paths
					// have been defined
					if (!string.IsNullOrEmpty(matchingConfig.legacyPackageDestinationPath))
					{
						Debug.LogFormat(CI_GENERATING_LEGACY_PACKAGE_FORMAT, configName, configId);

						UnityFileTools.CompileLegacyPackage(matchingConfig);
					}
					else
					{
						Debug.LogFormat(CI_SKIPPING_LEGACY_PACKAGE_FORMAT, configName, configId);
					}

					if (!string.IsNullOrEmpty(matchingConfig.packageDestinationPath))
					{
						Debug.LogFormat(CI_GENERATING_PACKAGE_SOURCE_FORMAT, configName, configId);

						FileTools.CreateOrUpdatePackageSource(matchingConfig);
					}
					else
					{
						Debug.LogFormat(CI_SKIPPING_PACKAGE_SOURCE_FORMAT, configName, configId);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("An unexpected error occured during package generation:\n\n{0}", e);
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				EditorApplication.UnlockReloadAssemblies();
			}

			Debug.Log(CI_COMPLETE);
		}

		[MenuItem("Tools/PackageTools/PrepareDll")]
		public static void PrepareDll()
		{
			try
			{
				Debug.Log($"[{nameof(CIUtils)}] PrepareDll");

				var packageManifestConfig = PackageManifestTools.GetAllConfigs().Single();
				var sourcePath = packageManifestConfig.sourcePath;

				Debug.Log($"[{nameof(CIUtils)}] Find assets in path: {sourcePath}");

				var assets = AssetDatabase.GetAllAssetPaths().Where(s => s.Contains(sourcePath));
				foreach (var asset in assets)
				{
					if (Path.GetExtension(asset) != ".dll")
						continue;

					var isEditor = Path.GetFileNameWithoutExtension(asset).Contains("Editor")
					               || asset.Contains("/Editor/");
					var pluginImporter = (PluginImporter)AssetImporter.GetAtPath(asset);
					if (isEditor)
					{
						pluginImporter.SetCompatibleWithAnyPlatform(false);
						pluginImporter.SetCompatibleWithEditor(true);
						Debug.Log($"[{nameof(CIUtils)}] SetCompatibleWithEditor: {asset}");
					}
					else
					{
						pluginImporter.SetCompatibleWithAnyPlatform(true);
						pluginImporter.SetExcludeEditorFromAnyPlatform(false);
						Debug.Log($"[{nameof(CIUtils)}] SetCompatibleWithAnyPlatform and ExcludeEditor: {asset}");
					}

					pluginImporter.SaveAndReimport();
				}

				Debug.Log($"[{nameof(CIUtils)}] Complete dll import");
			}
			catch (Exception e)
			{
				Debug.LogException(e);

				if (IsRunningOnCI)
					EditorApplication.Exit(1);
			}

			if (IsRunningOnCI)
				EditorApplication.Exit(0);
		}
	}
}