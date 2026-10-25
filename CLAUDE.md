# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

KeepAChangelog.IO is a zero-dependency .NET library (NuGet package) for parsing, writing and formatting changelogs in the https://keepachangelog.com/ format. The library targets `netstandard2.0;net8.0;net10.0` with `LangVersion` latest and nullable enabled, so APIs that only exist on newer frameworks (e.g. `DateOnly`) need `#if NET8_0_OR_GREATER` guards (see `ReleaseDate.cs`). Keep the library free of runtime package dependencies.

## Commands

The build is driven by NUKE (`build/Build.cs`); CI runs the same targets.

- Full pipeline (clean, restore, compile, validate CHANGELOG.md, test): `./build.cmd Test` (or `./build.ps1` / `./build.sh`). `Test` is the default target.
- Build only: `dotnet build KeepAChangelog.IO.sln`
- Run tests: `dotnet test KeepAChangelog.IO.sln`
- Single test: `dotnet test --filter "FullyQualifiedName~ChangelogTests.ToString_ValidChangelog_IsSameAsOriginal"`
- Reformat the repo's own CHANGELOG.md with the library: `./build.cmd FormatChangelog`

## Architecture

- `Changelog.Parse` is a line-by-line state machine over `ParsingContext` (Title → Description → ReleaseSection → EntryCategory → Entry → VersionLink). Line types are recognized by markdown prefix constants defined on each model class (`Release.Symbol = "## "`, `Category.Symbol = "### "`, `Entry.Symbol`, `VersionLink.Symbol`). Continuation lines inside an entry are appended to the previous entry's text with a newline.
- Serialization is done via `ToString()` overrides on each model (`Changelog`, `Release`, `Category`, `Entry`, `VersionLink`), joined with `Changelog.DoubleNewLine`. Writing **normalizes order**: releases sort descending via `Release.CompareTo` (Unreleased first, then by date, then version string), categories sort by `CategoryType` enum order, version links sort descending. `CategoryType` enum order therefore defines output order.
- A release with `ReleaseDate == null` is "Unreleased". `ReleaseDate` is a custom type (not `DateTime`) with conversion helpers.
- `ChangelogExtensions.ToFile` and `Changelog.FormatFile` (parse + rewrite in place) are the file-writing entry points; `Changelog.Create()` produces the default template.

## Testing

Tests use xUnit v3 with Verify snapshot testing, run through Microsoft.Testing.Platform (enabled in `global.json`, required on the .NET 10 SDK). Verify needs the `Verify_Sponsorship*` properties in the test csproj or the build fails with SC021; the OpenSource exemption is time-bounded and must be renewed before `Verify_SponsorshipExemptionUntil` passes (the build fails with SC049 after that month). `.gitattributes` forces `*.verified.txt` to LF; snapshots must match byte for byte, including no trailing newline. Test inputs live in `KeepAChangelog.IO.Tests/Data/`; `*.verified.txt` files are both inputs and expected outputs for round-trip tests (parse → `ToString()` must reproduce the file exactly). When output changes intentionally, update the `.verified.txt` snapshots (Verify writes `.received.txt` files on mismatch).

## Repository conventions

- The repo's own `CHANGELOG.md` must round-trip exactly through the library — the `ValidateChangelog` build target fails otherwise. Add user-facing changes under `## [Unreleased]` in the same style as existing entries (e.g. "Added `X` method to `Y`").
- Releases are published by pushing a `vX.Y.Z` tag; the `release` workflow runs `./build.cmd Publish`, which takes the package version from the tag and pushes to NuGet and GitHub Packages and creates a GitHub release from the latest changelog release.
