using System;

namespace Playdarium.PackageTool.Utils.PackageInitialize
{
	public static class PackageInitializeTemplates
	{
		public static string CreateChangelog(string homepage) => @$"# Changelog

---

## [v0.0.0]({homepage}/releases/tag/v0.0.0)

### Added

- For new features.

### Changed

- For changes in existing functionality.

### Deprecated

- For soon-to-be removed features.

### Removed

- For now removed features.

### Fixed

- For any bug fixes.

### Security

- In case of vulnerabilities.
";

		public static string CreateReadme(
			string authorName,
			string scope,
			string packageName,
			string packageTitle,
			string homepage
		)
		{
			return $@"# {packageTitle}

[![openupm](https://img.shields.io/npm/v/{packageName}?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/{packageName}/)
<img alt=""GitHub"" src=""https://img.shields.io/github/license/{homepage.Replace("https://github.com/", string.Empty)}"">

## Installing

Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your
`manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below
on how to reference it.

### Install via OpenUPM

The package is available on the [openupm](https://openupm.com/packages/{packageName}/)
registry.

#### Add registry scope

```
{{
  ""dependencies"": {{
    ...
  }},
  ""scopedRegistries"": [
    {{
      ""name"": ""{authorName}"",
      ""url"": ""https://package.openupm.com"",
      ""scopes"": [
        ""{scope}""
      ]
    }}
  ]
}}
```

#### Add package in PackageManager

Open `Window -> Package Manager` choose `Packages: My Regestries` and install package

### Install via GIT URL

```
""{packageName}"": ""{homepage}.git#upm""
```
";
		}

		public static string CreateLicense(string authorName) => @$"MIT License

Copyright (c) {DateTimeOffset.Now.Year} {authorName}

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.";

		public static string GitHubActionScript => @"
##############################################################
#                                                            #
#   This file is auto generated by Playdarium.PackageTool.   #
#                                                            #
##############################################################

name: Create Tag on Main Branch Update

on:
  push:
    branches:
      - main
    paths:
      - 'Release/package.json'
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout branch main
        uses: actions/checkout@v4
        with:
          ref: 'main'
          path: './repository'

      - name: Checkout branch upm
        uses: actions/checkout@v4
        with:
          ref: 'upm'
          path: './upm-package'

      - name: Get version from package.json
        id: version
        run: |
          echo ""version=$(grep -o '""version"": ""[^""]*""' ./repository/Release/package.json | awk -F'""' '{print $4}')"" >> $GITHUB_OUTPUT
        shell: bash

      - name: Copy package files from Release folder
        run: |
          rm -rf ./upm-package/*
          cp -r ./repository/Release/* ./upm-package
        shell: bash
      
      # Check in Unity Package on Release branch
      - name: Add & Commit Release Changes
        uses: EndBug/add-and-commit@v9
        with:
          author_name: Github Action Bot
          message: 'Auto-updated package contents'
          cwd: './upm-package'
          new_branch: 'upm'
          tag: v${{ steps.version.outputs.version }}
          push: true

";
	}
}