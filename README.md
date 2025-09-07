<h1 align="center">
  <img src="https://github.com/sandrofigo/KeepAChangelog.IO/blob/main/.github/assets/KeepAChangelog.IO.png?raw=true" alt="KeepAChangelog.IO">
  <br>
  KeepAChangelog.IO
  <br>
</h1>

<h4 align="center">A .NET library for reading and writing files in the <a href="https://keepachangelog.com/" target="_blank">https://keepachangelog.com/</a> format.</h4>

<p align="center">
  <a href="https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml">
    <img src="https://github.com/sandrofigo/KeepAChangelog.IO/actions/workflows/test.yml/badge.svg" alt="tests">
  </a>
</p>

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