# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added `ToDateOnly` method to `ReleaseDate`
- Added `FromDateOnly` method to `ReleaseDate`
- Added `ChangelogParseException`
- Added `IsValid` method to `Changelog`
- Added `IsValidFile` method to `Changelog`
- Added `IsFormatted` method to `Changelog`
- Added `IsFormattedFile` method to `Changelog`
- Added `GetUnreleased` method to `Changelog`
- Added `GetLatestRelease` method to `Changelog`
- Added `Parse` overload accepting a string to `Changelog`
- Added `IsValid` overload accepting a string to `Changelog`
- Added `IsFormatted` overload accepting a string to `Changelog`

### Changed

- `Changelog.Parse` now throws `ChangelogParseException` for unknown category names instead of ignoring them or throwing `ArgumentOutOfRangeException`
- `Changelog.Parse` now throws `ChangelogParseException` for entries without a category heading instead of throwing `ArgumentOutOfRangeException`
- Renamed `Format` method on `Changelog` to `FormatFile`

### Fixed

- Fixed releases without categories swallowing the following release or version links when parsing
- Fixed version sorting to compare numbers numerically (e.g. `1.10.0` > `1.9.0`), using semantic versioning precedence for semantic versions

## [1.2.0] - 2025-12-04

### Added

- Added `FromDateTime` method to `ReleaseDate`
- Added `Format` method to `Changelog`

## [1.1.0] - 2025-09-14

### Added

- Added `ToDateTime` method for `ReleaseDate`

### Removed

- Removed `readonly` on the `Releases` property
- Removed `readonly` on the `VersionLinks` property

## [1.0.2] - 2025-09-13

### Changed

- Updated image URL in README

## [1.0.1] - 2025-09-13

### Changed

- Updated README

### Fixed

- Fixed README rendering on nuget.org

## [1.0.0] - 2025-09-13

### Added

- Initial release