<h1 align="center">
  <img src="https://github.com/sandrofigo/KeepAChangelog.IO/blob/main/.github/assets/KeepAChangelog.IO.png?raw=true" alt="KeepAChangelog.IO">
  <br>
  KeepAChangelog.IO
  <br>
</h1>

A .NET library for reading and writing files in the https://keepachangelog.com/ format.

---

## Features

- 📖 **Parse** changelog files into a strongly-typed model  
- ✍️ **Write** new changelogs or update existing ones  
- ✅ **Validate** changelog formatting and structure

# Usage

## Read from a file

```csharp
using KeepAChangelog.IO;

var changelog = Changelog.From("CHANGELOG.md");
```

## Write to a file
```csharp
using KeepAChangelog.IO;

changelog.ToFile("CHANGELOG.md");
```

## Create a changelog from scratch

```csharp
using KeepAChangelog.IO;

var changelog = Changelog.Create();
```

# Contributing

Support this project with a ⭐️, report an issue or if you feel adventurous and would like to extend the functionality open a pull request.

# License

[MIT](https://github.com/sandrofigo/KeepAChangelog.IO/blob/main/LICENSE)