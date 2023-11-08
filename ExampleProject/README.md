# Example Project

[![openupm](https://img.shields.io/npm/v/com.example.package?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.example.package/)
<img alt="GitHub" src="https://img.shields.io/github/license/playdarium/package-tool">

## Installing

Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your
`manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below
on how to reference it.

### Install via OpenUPM

The package is available on the [openupm](https://openupm.com/packages/com.example.package/)
registry.

#### Add registry scope

```
{
  "scopedRegistries": [
    {
      "name": "Playdarium",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.example"
      ]
    }
  ]
}
```

#### Add package in PackageManager

Open `Window -> Package Manager` choose `Packages: My Regestries` and install package

### Install via GIT URL

```
"com.example.package": "https://github.com/playdarium/package-tool.git#upm"
```
