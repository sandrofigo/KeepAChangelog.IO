# KeepAChangelog.IO

[![nuget](https://img.shields.io/nuget/v/KeepAChangelog.IO)](https://www.nuget.org/packages/KeepAChangelog.IO/)
[![downloads](https://img.shields.io/nuget/dt/KeepAChangelog.IO?color=blue)](https://www.nuget.org/packages/KeepAChangelog.IO/)
[![tests](https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml/badge.svg)](https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml)

[![logo](https://raw.githubusercontent.com/sandrofigo/KeepAChangelog.IO/refs/heads/main/.github/assets/KeepAChangelog.IO.png)](https://github.com/sandrofigo/KeepAChangelog.IO)

A .NET library for reading and writing files in the https://keepachangelog.com/ format.

## Features

- Parse changelog files into a strongly-typed model
- Write new changelogs or update existing ones
- Validate changelog formatting and structure
- Format changelogs into a consistent, normalized layout
- Zero dependencies

## Installation

```
dotnet add package KeepAChangelog.IO
```

Supported target frameworks: `netstandard2.0`, `net8.0` and `net10.0`.

## Usage

### Read a changelog

```csharp
using KeepAChangelog.IO;

var changelog = Changelog.FromFile("CHANGELOG.md");

// or from a string
var changelog = Changelog.Parse(content);
```

### Write to a file

```csharp
changelog.ToFile("CHANGELOG.md");

// or get the content as a string
string content = changelog.ToString();
```

### Create a changelog from scratch

```csharp
var changelog = Changelog.Create();
```

This creates a changelog with the default title and description from https://keepachangelog.com/ and an empty `Unreleased` section.

### Access different parts of the changelog

```csharp
using System.Linq;
using KeepAChangelog.IO;

Release? unreleased = changelog.GetUnreleased();
Release? latestRelease = changelog.GetLatestRelease();

var latestFixes = latestRelease?
    .Categories.FirstOrDefault(c => c.Type == CategoryType.Fixed)?
    .Entries ?? [];
```

### Add an entry

```csharp
Release unreleased = changelog.GetUnreleased() ?? throw new InvalidOperationException("No unreleased section");

Category? added = unreleased.Categories.FirstOrDefault(c => c.Type == CategoryType.Added);
if (added is null)
{
    added = new Category { Type = CategoryType.Added };
    unreleased.Categories.Add(added);
}

added.Entries.Add(new Entry { Text = "Added `Foo` method to `Bar`" });

changelog.ToFile("CHANGELOG.md");
```

Entry text can span multiple lines by using newline characters.

### Publish a release

```csharp
Release unreleased = changelog.GetUnreleased() ?? throw new InvalidOperationException("No unreleased section");

unreleased.Version = "1.3.0";
unreleased.ReleaseDate = ReleaseDate.FromDateTime(DateTime.Today);

// Start a new, empty unreleased section
changelog.Releases.Add(new Release());

changelog.VersionLinks.Add(new VersionLink
{
    Version = "1.3.0",
    Url = "https://github.com/owner/repo/compare/v1.2.0...v1.3.0"
});

changelog.ToFile("CHANGELOG.md");
```

A release can be marked as yanked with `release.IsYanked = true`, which appends `[YANKED]` to its heading.

On `net8.0` and above, `ReleaseDate.FromDateOnly` and `ToDateOnly` are available in addition to `FromDateTime` and `ToDateTime`.

### Validate a changelog

```csharp
// Can the changelog be parsed?
bool isValid = Changelog.IsValidFile("CHANGELOG.md");

// Is it valid and would formatting it leave it unchanged?
bool isFormatted = Changelog.IsFormattedFile("CHANGELOG.md");
```

Both methods also have overloads accepting a string or lines (`IsValid`, `IsFormatted`). This is useful as a CI check, e.g. to fail a build when the changelog is not formatted.

When parsing fails, a `ChangelogParseException` is thrown with the line number of the problem:

```csharp
try
{
    var changelog = Changelog.Parse(content);
}
catch (ChangelogParseException e)
{
    // e.g. "Line 5: Unknown category 'Foo'. Valid categories are: ..."
    Console.WriteLine(e.Message);

    int line = e.LineNumber;
}
```

### Format a changelog

```csharp
Changelog.FormatFile("CHANGELOG.md");
```

This parses the file and writes it back in place.

## Formatting behavior

Writing a changelog normalizes it:

- Releases are sorted descending: `Unreleased` first, then by release date, then by version (using semantic versioning precedence for semantic versions)
- Categories are sorted in the order `Added`, `Changed`, `Deprecated`, `Removed`, `Fixed`, `Security`
- Version links are sorted descending by version
- Lines are separated with `Environment.NewLine` and the output has no trailing newline

Parsing has a few rules to be aware of:

- Only the six categories above are allowed (case-insensitive). Unknown categories throw a `ChangelogParseException`, and category names are written back in their canonical casing.
- Entries must be placed under a category heading.
- Text inside a release that is not part of a category or entry is not preserved.

## Upgrading to 2.0

- `Changelog.Format` was renamed to `Changelog.FormatFile`
- `Changelog.Parse` now throws `ChangelogParseException` for unknown categories and entries without a category heading

See the [changelog](https://github.com/sandrofigo/KeepAChangelog.IO/blob/main/CHANGELOG.md) for all changes.

## Contributing

Support this project with a ⭐️, open an issue or if you feel adventurous and would like to extend the functionality open a pull request.

To build and run the tests:

```
./build.cmd Test
```

The repository's own `CHANGELOG.md` is validated by the build and must be formatted exactly as the library would write it. Run `./build.cmd FormatChangelog` to format it.

## License

[MIT](https://github.com/sandrofigo/KeepAChangelog.IO/blob/main/LICENSE)
