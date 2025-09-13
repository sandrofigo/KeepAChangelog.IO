# KeepAChangelog.IO

[![nuget](https://img.shields.io/nuget/v/KeepAChangelog.IO)](https://www.nuget.org/packages/KeepAChangelog.IO/)
[![downloads](https://img.shields.io/nuget/dt/KeepAChangelog.IO?color=blue)](https://www.nuget.org/packages/KeepAChangelog.IO/)
[![tests](https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml/badge.svg)](https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml)

[![logo](https://raw.githubusercontent.com/sandrofigo/KeepAChangelog.IO/refs/heads/main/.github/assets/KeepAChangelog.IO.png)](https://github.com/sandrofigo/KeepAChangelog.IO)

A .NET library for reading and writing files in the https://keepachangelog.com/ format.

## Features

- 📖 **Parse** changelog files into a strongly-typed model  
- ✍️ **Write** new changelogs or update existing ones  
- ✅ **Validate** changelog formatting and structure

## Usage

### Read from a file

```csharp
using KeepAChangelog.IO;

var changelog = Changelog.From("CHANGELOG.md");
```

### Write to a file
```csharp
changelog.ToFile("CHANGELOG.md");
```

### Create a changelog from scratch

```csharp
var changelog = Changelog.Create();
```

### Access different parts of the changelog

```csharp
using System.Linq;
using KeepAChangelog.IO;

var latestRelease = changelog.Releases[0];
var latestFixes = latestRelease.Categories.First(c => c.Type == CategoryType.Fixed).Entries;
// ...
```

## Contributing

Support this project with a ⭐️, open an issue or if you feel adventurous and would like to extend the functionality open a pull request.
