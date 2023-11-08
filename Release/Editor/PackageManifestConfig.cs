﻿/*
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

using System;
using Playdarium.PackageTool.Tools;
using UnityEngine;

namespace Playdarium.PackageTool
{
	/// <summary>
	/// <see cref="PackageManifestConfig"/> is an asset config representing a Unity Package. It allows for
	/// defining the data used on the package json file, the locations of source files/assets that make up the
	/// package, and a destination for th package source to be distributed at.
	/// </summary>
	[CreateAssetMenu(fileName = "PackageManifestConfig", menuName = "JCMG/PackageTools/PackageManifestConfig")]
	public sealed class PackageManifestConfig : ScriptableObject
	{
		/// <summary>
		/// Describes a dependency that this package requires.
		/// </summary>
		[Serializable]
		public sealed class Dependency
		{
			/// <summary>
			/// The name of the dependent package.
			/// </summary>
			public string packageName;

			/// <summary>
			/// The semantic version of the dependent package in MAJOR.MINOR.PATCH format.
			/// </summary>
			public string packageVersion;

			public bool IsEmpty() => string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(packageVersion);
		}

		/// <summary>
		/// Describes the author of this package.
		/// </summary>
		[Serializable]
		public sealed class Author
		{
			public string name = "";
			public string email = "";
			public string url = "";
		}

		/// <summary>
		/// Describes the samples of this package.
		/// </summary>
		[Serializable]
		public sealed class Sample
		{
			public string displayName;
			public string description;
			public string path;

			public bool IsEmpty() => string.IsNullOrEmpty(displayName)
			                         || string.IsNullOrEmpty(description)
			                         || string.IsNullOrEmpty(path);
		}

		/// <summary>
		/// A unique id for this <see cref="PackageManifestConfig"/> instance.
		/// </summary>
		public string Id => _id;

		public string homepage = "https://github.com/";

		/// <summary>
		/// A collection of paths to folders and files for the source for the package.
		/// </summary>
		// public string[] packageSourcePaths;
		public string sourcePath;

		public string documentationPath = "Documentation~";
		public string readmePath = "README.md";
		public string changelogPath = "CHANGELOG.md";
		public string licensePath = "LICENSE";

		/// <summary>
		/// A collection of file/folder paths to exclude from the package.
		/// </summary>
		public string[] packageIgnorePaths;

		/// <summary>
		/// A path to the package source distribution contents.
		/// </summary>
		public string packageDestinationPath;

		/// <summary>
		/// The relative path to the folder where the legacy package will be exported to.
		/// </summary>
		public string legacyPackageDestinationPath;

		/// <summary>
		/// The fully-qualified package name.
		/// </summary>
		public string packageName = "";

		/// <summary>
		/// The package name as it appears in the Package Manager window.
		/// </summary>
		public string displayName;

		/// <summary>
		/// The semantic version of the package in MAJOR.MINOR.PATCH format.
		/// </summary>
		public string packageVersion = "1.0.0";

		/// <summary>
		/// The version of Unity in semantic version format like 2018.1.
		/// </summary>
		public string unityVersion = "2021.3";

		/// <summary>
		/// A description of the package.
		/// </summary>
		public string description;

		/// <summary>
		/// The category the package belongs in.
		/// </summary>
		public string category = "unity";

		/// <summary>
		/// The author of this package.
		/// </summary>
		public Author author;

		/// <summary>
		/// A collection of keywords that describe the package.
		/// </summary>
		public string[] keywords;

		/// <summary>
		/// A collection of packages that this package depends on.
		/// </summary>
		public Dependency[] dependencies;

		public Sample[] samples;

		/// <summary>
		/// A path to the where the VersionConstants.cs file should be created/updated
		/// </summary>
		public string versionConstantsPath;

		/// <summary>
		/// The namespace the generated VersionConstants class will be in. If left blank, this will be
		/// the global namespace.
		/// </summary>
		public string versionConstantsNamespace;

		[SerializeField] private string _id;

		/// <summary>
		/// Returns a Json <see cref="string"/> representation.
		/// </summary>
		/// <returns></returns>
		public string GenerateJson()
		{
			return PackageManifestTools.GenerateJson(this);
		}

		private void Reset()
		{
			packageVersion = EditorConstants.DEFAULT_PACKAGE_VERSION;
			unityVersion = EditorConstants.DEFAULT_UNITY_VERSION;
			_id = Guid.NewGuid().ToString();
		}
	}
}