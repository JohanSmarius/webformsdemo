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

## Research Findings

### Projects Affected
- **BasketballScores.csproj** (C:\src\webformsdemomodernization\BasketballScores.csproj)
  - Current format: Legacy non-SDK-style (ToolsVersion=15.0)
  - Current TFM: net48 (TargetFrameworkVersion v4.8)
  - Project type: ASP.NET Web Application (ProjectTypeGuids: {349c5851-65df-11da-9384-00065b846f21})
  - OutputType: Library

### Topological Order
1. BasketballScores.csproj (single project, no dependencies)

### Project Structure
- **Current format details**:
  - Legacy PropertyGroup with multiple explicit properties
  - GAC assembly references (System, System.Data, System.Web, System.Drawing, etc.)
  - IIS Express settings configured
  - Web Forms content files (.aspx, .master, .config)

### Packages to Update
- No packages.config file detected
- No NuGet packages currently referenced
- Post-conversion: will need to verify no missing packages

### Conversion Strategy
- Use convert_project_to_sdk_style tool (single project conversion)
- Stay on net48 (no TFM changes in this task)
- Verify build after conversion using msbuild.exe (not dotnet build, since net48 + Web project)
- Check for excluded items from globbing post-conversion

### Dependencies & Risks
- Web project with System.Web dependency (SDK conversion keeps net48, so should be compatible)
- No packages to migrate (low risk)
- Single project (no cross-project impact)

### Decisions Made
- Use SDK conversion tool (not manual rewrite)
- Build with msbuild.exe after conversion (net48 Web project requires full MSBuild)
- Check for excluded item warnings post-build
