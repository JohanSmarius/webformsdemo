# 01-prerequisites: Validate upgrade prerequisites

Verify the development environment can support .NET 10 and validate any global.json SDK constraints. This includes checking that the .NET 10 SDK is installed and that no global.json files in the solution directory or parent directories pin to an incompatible SDK version.

**Done when**: .NET 10 SDK is confirmed installed, no global.json conflicts exist, and the environment is ready for the upgrade.

## Research Findings

### Environment Status
- ✅ .NET 10 SDK: Compatible SDK found and installed
- ✅ global.json: No global.json configuration found in solution or parent directories
- ✅ Solution: BasketballScores.sln at C:\src\webformsdemomodernization\

### Projects Affected
- BasketballScores.csproj — Web Forms project currently on net48, will upgrade to net10.0

### SDK Requirements
- Target Framework: net10.0
- Current SDK: Compatible with .NET 10

### Dependencies & Risks
- No global.json constraints blocking the upgrade
- No SDK version conflicts detected
- Environment is ready for SDK-style conversion and framework upgrade

### Decisions Made
- Prerequisites validation complete — no blockers detected
- Ready to proceed to task 02-sdk-conversion
