using Playdarium.PackageTool.Utils.PackageInitialize;
using UnityEditor;

namespace Playdarium.PackageTool
{
	internal static class MenuItems
	{
		[MenuItem("Tools/PackageTools/Init Package")]
		internal static void OpenAboutModalDialog() => PackageInitializeWindow.Open();
	}
}