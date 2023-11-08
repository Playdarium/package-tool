# <img src="./Documentation~/PackageManifestConfigIcon.png" alt="" width="35" height="35"/> Package Tool

[![openupm](https://img.shields.io/npm/v/com.playdarium.package-tool?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.playdarium.package-tool/)
<img alt="GitHub" src="https://img.shields.io/github/license/playdarium/package-tool">

## Installing

Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your
`manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below
on how to reference it.

### Install via OpenUPM

The package is available on the [openupm](https://openupm.com/packages/com.playdarium.package-tool/)
registry.

#### Add registry scope

```
{
  "dependencies": {
    ...
  },
  "scopedRegistries": [
    {
      "name": "Playdarium",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.playdarium"
      ]
    }
  ]
}
```

#### Add package in PackageManager

Open `Window -> Package Manager` choose `Packages: My Regestries` and install package

### Install via GIT URL

```
"com.playdarium.package-tool": "https://github.com/playdarium/package-tool.git#upm"
```

## Setting up your first package

Creating your first package is a simple process that can be made more complex because information available about Unity
Package manager is inconsistent right now. This is largely due to it not being widely and officially supported for
end-users of Unity yet, but starting with 2019.1 will gain more official support.

The first step is to create a `PackageManifestConfig` asset that we can use to define the properties and contents of our
package. This can be done by right-clicking in the `Project` window and
selecting `Create -> JCMG -> Package Tools -> Package
Manifest Config`. This will create an asset named `PackageManifestConfig` in that folder.

<img src="./Documentation~/Inspector.png" width="500">

A `PackageManifestConfig` is broken up into two sections:

* **Package Json:** This section contains fields relating to properties of the `package.json` file that we will need to
  have in our package root folder. It describes the minimum Unity version that it is compatible with and the version of
  the package itself [Semantic versioning](https://docs.unity3d.com/Manual/upm-semver.html), a
  description of the package, and any dependencies it
  has on other packages. This last part I have the least information about, but is supposed to gain more widespread
  support in 2019.1.
* **Package Content and Export:** This section focuses on defining the folders/files that make up the content (including
  code and assets) that make up the package itself and a location that the content will be published to when clicking on
  the button labeled `Export Package Source`.

For convenience sake, I have added file and folder pickers as buttons to the right of these fields that make it easier
to add relative file/folder paths from the Assets folder for package source and export location.

There are also options for excluding specific files and/or folders from the package source that might otherwise be
included. This can be useful for example, if there are unit tests or example content located within your package source
path that you would not want to include directly or indirectly. Folders and files can be excluded by adding them to
the **ExcludePaths** list.

Recently I have found myself manually creating a `VersionConstants` static class containing versioning information about
the package itself. This can be useful for displaying to a user or checking for updates outside of the native Unity
PackageManager window.. Since updating this file by hand can be error-prone or easy to forget, this can now be
configured on a `PackageManifestConfig` and generated using a new action `Generate VersionConstants.cs` on the config's
inspector.

**Example VersionConstants.cs used in JCMG.PackageTools**

```csharp
namespace JCMG.PackageTools.Editor
{
	/// <summary>
	/// Version info for this library.
	/// </summary>
	internal static class VersionConstants
	{
		/// <summary>
		/// The semantic version
		/// </summary>
		public const string VERSION = "1.3.0";

		/// <summary>
		/// The branch of GIT this package was published from.
		/// </summary>
		public const string GIT_BRANCH = "develop";

		/// <summary>
		/// The current GIT commit hash this package was published on.
		/// </summary>
		public const string GIT_COMMIT = "57aec574ed19746de42ffa5032358562fb041ebf";

		/// <summary>
		/// The UTC human-readable date this package was published at.
		/// </summary>
		public const string PUBLISH_DATE = "Friday, May 1, 2020";

		/// <summary>
		/// The UTC time this package was published at.
		/// </summary>
		public const string PUBLISH_TIME = "05/01/2020 08:26:31";
	}
}

```

The contents of this file can be configured based on the use of a text template. A default one is provided
at `JCMG\PackageTools\Scripts\Editor\Templates\VersionConstants.txt` and a custom template can be used instead by
providing it's meta GUID on the `PackageManifestConfig` `versionTemplateGuid` field. There is an example of this in the
development project

Example Template

```csharp
namespace JCMG.PackageTools.Editor
{
	/// <summary>
	/// Version info for this library.
	/// </summary>
	internal static class VersionConstants
	{
		/// <summary>
		/// The semantic version
		/// </summary>
		public const string VERSION = "${version}";

		/// <summary>
		/// The branch of GIT this package was published from.
		/// </summary>
		public const string GIT_BRANCH = "${git_branch}";

		/// <summary>
		/// The current GIT commit hash this package was published on.
		/// </summary>
		public const string GIT_COMMIT = "${git_commit}";

		/// <summary>
		/// The UTC human-readable date this package was published at.
		/// </summary>
		public const string PUBLISH_DATE = "${publish_date}";

		/// <summary>
		/// The UTC time this package was published at.
		/// </summary>
		public const string PUBLISH_TIME = "${publish_utc_time}";
	}
}

```

## Generating packages from command-line

It can be desirable at times to generate legacy Unity packages and/or package source from the command-line so that they
can be created and distributed as part of a continuous integration (CI) process. For example, you may want to trigger a
process every time the repository is updated and source files changed so that a new package is created and made
available to users. This can be done by launching the Unity Editor from the command line and supplying the appropriate
arguments.

**Example Command-Line Call**

`"D:\Program Files\2019.4.11f1\Editor\Unity.exe" -quit -batchmode -executeMethod JCMG.PackageTools.Editor.PackageToolsCI.Generate version=1.0.1 GenerateVersionConstants=true`

In this case, the `-executeMethod JCMG.PackageTools.Editor.PackageToolsCI.Generate` is the CLI argument that will
trigger the PackageTools CI process to execute. By default, this will find all `PackageManifestConfig` instances in the
project and if they have an appropriate output path will generate the legacy Unity package and/or package source.
Progress will be output to the Unity Editor player log.

### Arguments

#### ID

An optional `ID` CLI argument (case-insensitive) that can be supplied with one or more `ID` values to limit the package
generation to a subset of `PackageManifestConfig` instances in the project if so desired. These `ID` values should
correspond to the `ID` displayed on the `PackageManifestConfig` inspector for that instance. If not supplied, all

**Example ID Argument Usage (for three ID values passed)**

`id=cb87cda2-0bfa-44dc-b583-ae61ff81efcb,e1725f4a-f9f3-42c0-ac15-756f017c90ed,6a406891-59d5-430a-815d-c252deae5d5b`

#### Version

An optional `Version` CLI argument (case-insensitive) can be supplied which will both override and set the value of the
package version used on the `PackageManifestConfig` `packageVersion` field. This can be useful where the semantic
package version value should be coming from an external source to the Unity project.

#### GenerateVersionConstants

An optional `GenerateVersionConstants` CLI argument (case-insensitive) can be supplied which will force
the `VersionConstants` file to be written out prior to generating the package (if a path is specified for it on
the `PackageManifestConfig`). This can be useful where this file is created as a result of a CI build and has it's
version number set by the `version` argument.