# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.1.0-preview.1] - 2019-06-05
- Adding support for ARKit 3 functionality: multiple face tracking and tracking a face (with front camera) while in world tracking (with rear camera).

## [1.0.0-preview.6] - 2019-05-31
### Fixes
- Fix documentation links.

## [1.0.0-preview.4] - 2019-05-06

### This is the first release of *ARKit Face Tracking*.

Provides runtime support for Face Tracking on ARKit. This is a separate package from com.unity.xr.arkit due to security concerns: apps that contain certain face-tracking related symbols in their compiled binaries will fail App Store validation unless additional documentation explaning the uses of face tracking are documented. This allows face tracking support to be added separately.
