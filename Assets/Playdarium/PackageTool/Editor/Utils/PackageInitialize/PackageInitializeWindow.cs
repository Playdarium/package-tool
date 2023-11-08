using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playdarium.PackageTool.Utils.PackageInitialize
{
	public class PackageInitializeWindow : EditorWindow
	{
		private const string DISPLAY_NAME_FIELD = "Display Name";
		private const string PACKAGE_NAME_FIELD = "Package Name";
		private const string SCOPE_FIELD = "Scope";
		private const string AUTHOR_FIELD = "Author";
		private const string GIT_URL_FIELD = "Git URL";

		private string _displayName;
		private string _packageName = "com.example.package";
		private string _scope = "com.example";
		private string _author;
		private string _gitURL;

		public static void Open()
		{
			var window = GetWindowWithRect<PackageInitializeWindow>(new Rect(0, 0, 300, 130), false, "Init Package");
			window.ShowPopup();
		}

		private void OnGUI()
		{
			DrawTextField(DISPLAY_NAME_FIELD, ref _displayName);
			DrawTextField(PACKAGE_NAME_FIELD, ref _packageName);
			DrawTextField(SCOPE_FIELD, ref _scope);
			DrawTextField(AUTHOR_FIELD, ref _author);
			DrawTextField(GIT_URL_FIELD, ref _gitURL);

			if (GUILayout.Button("Create"))
				CreatePackageFiles();
		}

		private void CreatePackageFiles()
		{
			var valid = ValidateField(DISPLAY_NAME_FIELD, _displayName);
			valid &= ValidateField(PACKAGE_NAME_FIELD, _packageName);
			valid &= ValidateField(SCOPE_FIELD, _scope);
			valid &= ValidateField(AUTHOR_FIELD, _author);
			valid &= ValidateField(GIT_URL_FIELD, _gitURL);

			if (!valid)
			{
				EditorUtility.DisplayDialog("Init Package", "All fields must be completed", "Ok");
				return;
			}

			var config = CreateInstance<PackageManifestConfig>();
			config.displayName = _displayName;
			config.packageName = _packageName;
			config.homepage = _gitURL;
			config.author = new PackageManifestConfig.Author
			{
				name = _author
			};
			config.packageDestinationPath = PackageInitializeUtil.RELEASE_FOLDER_NAME;
			config.sourcePath = Path.Combine(
				PackageInitializeUtil.ASSETS_FOLDER_NAME,
				config.author.name,
				config.displayName.Replace(" ", string.Empty)
			);

			PackageInitializeUtil.Init(_scope, config);

			if (EditorUtility.DisplayDialog("Init Package", "Successful created", "Ok"))
				Close();
		}

		private static bool ValidateField(string name, string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
				return true;

			Debug.LogError($"[{nameof(PackageInitializeWindow)}] Field with name '{name}' cannot be empty");
			return false;
		}

		private static void DrawTextField(string label, ref string value)
			=> value = EditorGUILayout.TextField(label, value);
	}
}