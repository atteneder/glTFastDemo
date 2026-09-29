# Release Process

## Changelog

- Change the `## [Unreleased]` section in [CHANGELOG.md] to have the format `## [major.minor.patch] - YYYY-MM-DD`
  - `major.minor.patch` is a semantic version. Bump the numbers according to category of changes (only fixes: patch, additions: minor, breaking change: major)

## Unity Player Settings

- Update the *Version* in the player settings.
- Bump the Android Bundle Version Code by one.

## Build

- Web
- Android

## Post-Release

- Add a new `## [Unreleased]` section on top of [CHANGELOG.md].

[CHANGELOG.md]: ../CHANGELOG.md
