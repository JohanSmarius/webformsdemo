# 01-prerequisites: Validate upgrade prerequisites

Verify the development environment can support .NET 10 and validate any global.json SDK constraints. This includes checking that the .NET 10 SDK is installed and that no global.json files in the solution directory or parent directories pin to an incompatible SDK version.

**Done when**: .NET 10 SDK is confirmed installed, no global.json conflicts exist, and the environment is ready for the upgrade.
