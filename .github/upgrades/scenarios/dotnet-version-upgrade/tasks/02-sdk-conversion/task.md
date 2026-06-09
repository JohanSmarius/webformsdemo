# 02-sdk-conversion: Convert project to SDK-style format

Convert BasketballScores.csproj from the legacy .NET Framework project format to the modern SDK-style format while staying on net48. This is a structural change that simplifies the project file and enables modern .NET features. The conversion will automatically migrate packages.config to PackageReference format.

**Scope**: 
- Convert BasketballScores.csproj (legacy format → SDK-style)
- Migrate packages.config to PackageReference (handled by conversion tooling)
- Stay on net48 during this task (TFM upgrade comes later)

**Key concerns from assessment**:
- Project is currently non-SDK-style (Wap, Sdk Style = False)
- No NuGet packages currently referenced (minimal PackageReference migration)

**Done when**: Project file is SDK-style format, builds successfully on net48, all packages are PackageReference-based, and no functionality has regressed.
