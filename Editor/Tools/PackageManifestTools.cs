/*
MIT License

Copyright (c) 2020 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;

namespace Playdarium.PackageTool.Tools
{
	/// <summary>
	/// Helper methods for the Package Manifest Tools
	/// </summary>
	internal static class PackageManifestTools
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore
		};

		/// <summary>
		/// Returns a Json <see cref="string"/> representation of the <see cref="PackageManifestConfig"/>
		/// <paramref name="manifest"/>.
		/// </summary>
		/// <param name="manifest"></param>
		/// <returns></returns>
		public static string GenerateJson(PackageManifestConfig manifest)
		{
			var packageJsonModel = PackageJsonModel.CreateFromManifest(manifest);
			return JsonConvert.SerializeObject(packageJsonModel, Formatting.Indented, Settings);
		}

		/// <summary>
		/// Retrieves all <see cref="PackageManifestConfig"/> instances in the project.
		/// </summary>
		public static PackageManifestConfig[] GetAllConfigs()
		{
			var assetList = new List<PackageManifestConfig>();

			const string TYPE_FILTER = "t:PackageManifestConfig";

			var configGuids = AssetDatabase.FindAssets(TYPE_FILTER);
			foreach (var configGuid in configGuids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(configGuid);
				var config = AssetDatabase.LoadAssetAtPath<PackageManifestConfig>(assetPath);
				if (config != null)
				{
					assetList.Add(config);
				}
			}

			return assetList.ToArray();
		}
	}
}