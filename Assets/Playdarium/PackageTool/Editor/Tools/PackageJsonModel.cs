using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;

namespace Playdarium.PackageTool.Tools
{
	public class PackageJsonModel
	{
		[JsonProperty("name")] public string Name;
		[JsonProperty("version")] public string Version;
		[JsonProperty("displayName")] public string DisplayName;
		[JsonProperty("description")] public string Description;
		[JsonProperty("category")] public string Category;
		[JsonProperty("unity")] public string Unity;
		[JsonProperty("homepage")] public string Homepage;
		[JsonProperty("documentationUrl")] public string DocumentationUrl;
		[JsonProperty("changelogUrl")] public string ChangelogUrl;
		[JsonProperty("licensesUrl")] public string LicensesUrl;
		[JsonProperty("keywords")] public string[] Keywords;
		[JsonProperty("dependencies")] public Dictionary<string, string> Dependencies;
		[JsonProperty("samples")] public Sample[] Samples;
		[JsonProperty("author")] public Author Author;
		[JsonProperty("repository")] public Repository Repository;
		[JsonProperty("bugs")] public Bugs Bugs;


		public static PackageJsonModel CreateFromManifest(PackageManifestConfig manifest)
		{
			var model = new PackageJsonModel
			{
				Name = manifest.packageName,
				Version = manifest.packageVersion,
				DisplayName = manifest.displayName,
				Description = manifest.description,
				Category = manifest.category,
				Unity = manifest.unityVersion,
				Homepage = manifest.homepage,
			};
			
			model.Keywords = manifest.keywords != null && manifest.keywords.Length > 0
				? manifest.keywords
					.Where(k => !string.IsNullOrEmpty(k))
					.Distinct()
					.ToArray()
				: null;

			model.Author = !string.IsNullOrEmpty(manifest.author.name)
				? new Author(manifest.author.name, manifest.author.email, manifest.author.url)
				: null;

			model.Dependencies = manifest.dependencies != null && manifest.dependencies.Length > 0
				? manifest.dependencies
					.Where(d => !d.IsEmpty())
					.ToDictionary(d => d.packageName, d => d.packageVersion)
				: null;

			model.Samples = manifest.samples != null && manifest.samples.Length > 0
				? manifest.samples
					.Where(s => !s.IsEmpty())
					.Select(s => new Sample(s.displayName, s.description, s.path))
					.ToArray()
				: null;

			if (!string.IsNullOrEmpty(manifest.homepage))
			{
				var homepage = manifest.homepage;
				if (homepage.EndsWith("/"))
					homepage = homepage.Remove(homepage.Length - 2, 1);

				model.Homepage = homepage;
				model.DocumentationUrl = $"{homepage}/README.md";
				model.ChangelogUrl = $"{homepage}/CHANGELOG.md";
				model.LicensesUrl = $"{homepage}/LICENSE";
				model.Repository = new Repository("git", $"{homepage}.git");
				model.Bugs = new Bugs($"{homepage}/issues");
			}

			return model;
		}
	}

	public class Author
	{
		[JsonProperty("name")] public string Name;
		[JsonProperty("email")] public string Email;
		[JsonProperty("url")] public string URL;

		public Author()
		{
		}

		public Author(string name, string email, string url)
		{
			Name = name;
			Email = email;
			URL = url;
		}
	}

	public class Repository
	{
		[JsonProperty("type")] public string Type;
		[JsonProperty("url")] public string URL;

		public Repository()
		{
		}

		public Repository(string type, string url)
		{
			Type = type;
			URL = url;
		}
	}

	public class Bugs
	{
		[JsonProperty("url")] public string URL;

		public Bugs()
		{
		}

		public Bugs(string url)
		{
			URL = url;
		}
	}

	public class Sample
	{
		[JsonProperty("displayName")] public string DisplayName;
		[JsonProperty("description")] public string Description;
		[JsonProperty("path")] public string Path;

		public Sample()
		{
		}

		public Sample(string displayName, string description, string path)
		{
			DisplayName = displayName;
			Description = description;
			Path = path;
		}
	}
}