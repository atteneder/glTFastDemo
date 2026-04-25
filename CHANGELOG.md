# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- Updates
  - Unity 6000.3.14f1
  - glTFast 6.18.0
  - KTX for Unity 3.6.3
  - Draco for Unity 5.4.3
  - meshoptimizer mesh compression for Unity 0.2.0-exp.1

### Removed
- `CustomLoadDemo`
- Generic file drop support on Windows.

### Fixed
- (Web) Errors about missing scripts and difference in serialization size.
- (Web) Removed IMGUI artifacts by disabling IMGUI in Web builds.

## [0.9.3] - 2026-04-23

### Changed
- (Performance) Enabled Native C/C++ Multithreading on Web
  - Target WebAssembly 2023
  - Enabled explicitly thrown exceptions for Web
- Updates
  - Unity 6000.0.63f1
  - glTFast 6.15.0
  - KTX for Unity 3.5.1
  - Draco for Unity 5.4.1

## [0.9.2] - 2025-10-12

### Changed
- Updates
  - Unity 6000.0.59f2
  - glTFast 6.14.1
  - KTX for Unity 3.5.0
  - Draco for Unity 5.2.0

### Fixed
- Supports [16kB page size on Android](https://developer.android.com/guide/practices/page-sizes).

## [0.9.1] - 2025-04-06

### Changed
- Update glTFast to 6.12.0
- Update Unity to 6000.0.45f1

## [0.9.0] - 2024-03-29

First tagged release.