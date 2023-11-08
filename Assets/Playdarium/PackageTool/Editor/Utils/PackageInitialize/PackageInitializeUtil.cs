using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playdarium.PackageTool.Utils.PackageInitialize
{
	public static class PackageInitializeUtil
	{
		public const string ASSETS_FOLDER_NAME = "Assets";
		public const string RELEASE_FOLDER_NAME = "Release";

		private const string DOCUMENTATION_FOLDER_NAME = "Documentation~";

		private const string PACKAGE_MANIFEST_FOLDER_NAME = "PackageManifest";
		private const string PACKAGE_MANIFEST_CONFIG_FILE_NAME = "PackageManifestConfig.asset";

		private const string README_FILE_NAME = "README.md";
		private const string CHANGELOG_FILE_NAME = "CHANGELOG.md";
		private const string LICENSE_FILE_NAME = "LICENSE";

		private const string GITHUB_CI_FILE_PATH = ".github/workflows";
		private const string GITLAB_CI_FILE_NAME = "release-upm.yml";

		private static readonly string AssetPath = Application.dataPath;

		private static readonly string ProjectPath = Application.dataPath
			.Replace(ASSETS_FOLDER_NAME, string.Empty)
			.TrimEnd('\\', '/');

		public static void Init(string scope, PackageManifestConfig config)
		{
			config.sourcePath = Path.Combine(ASSETS_FOLDER_NAME, config.author.name);

			CreateFolderInProject(config.sourcePath);
			CreateFolderInProject(DOCUMENTATION_FOLDER_NAME);
			CreateFolderInProject(config.packageDestinationPath);

			CreatePackageManifestConfig(config);
			CreateFileAtPath(
				ProjectPath,
				README_FILE_NAME,
				PackageInitializeTemplates.CreateReadme(config.author.name, scope, config.packageName,
					config.displayName, config.homepage)
			);
			CreateFileAtPath(ProjectPath, CHANGELOG_FILE_NAME,
				PackageInitializeTemplates.CreateChangelog(config.homepage));
			CreateFileAtPath(ProjectPath, LICENSE_FILE_NAME,
				PackageInitializeTemplates.CreateLicense(config.author.name));
			CreateFileAtPath(Path.Combine(ProjectPath, GITHUB_CI_FILE_PATH), GITLAB_CI_FILE_NAME,
				PackageInitializeTemplates.GitHubActionScript);

			AssetDatabase.Refresh();
		}

		private static void CreatePackageManifestConfig(PackageManifestConfig config)
		{
			var packageConfigPath = Path.Combine(Application.dataPath, PACKAGE_MANIFEST_FOLDER_NAME);
			if (!Directory.Exists(packageConfigPath))
				Directory.CreateDirectory(packageConfigPath);

			var configPath = Path.Combine("Assets", PACKAGE_MANIFEST_FOLDER_NAME, PACKAGE_MANIFEST_CONFIG_FILE_NAME);
			var asset = AssetDatabase.LoadAssetAtPath<PackageManifestConfig>(configPath);
			if (asset != null)
			{
				Debug.Log(
					$"[{nameof(PackageInitializeUtil)}] {nameof(PackageManifestConfig)} already exist at path '{configPath}'");
				return;
			}

			AssetDatabase.CreateAsset(config, configPath);
			AssetDatabase.SaveAssets();
		}

		public static void CreateFileAtPath(
			string path,
			string fileName,
			string content
		)
		{
			var filePath = Path.Combine(path, fileName);
			if (File.Exists(filePath))
			{
				Debug.Log($"[{nameof(PackageInitializeUtil)}] File already exist '{filePath}'");
				return;
			}

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			File.WriteAllText(filePath, content);
		}

		public static void CreateFolderInProject(string directoryPath)
		{
			var dir = Path.Combine(ProjectPath, directoryPath);
			if (Directory.Exists(dir))
				return;

			Directory.CreateDirectory(dir);
		}

		public static void CreateFolderInAssets(string directoryPath)
		{
			var dir = Path.Combine(AssetPath, directoryPath);
			if (Directory.Exists(dir))
				return;

			Directory.CreateDirectory(dir);
		}
	}
}