# .NET Version Upgrade: Basketball Scores to .NET 10

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet10-blazor
- **Commit Strategy**: after-each-task
- **Branch Sync**: auto-merge

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Project Approach: Side-by-side (Web Forms → Blazor)

### Compatibility
- Unsupported API Handling: Fix Inline

## Modernization Phases
This upgrade is Phase 1 of a three-phase modernization:
1. **Phase 1** (current): Upgrade .NET Framework 4.8 → .NET 10
2. **Phase 2** (next): Migrate Web Forms → Blazor with EF Core
3. **Phase 3** (final): Add Aspire orchestration

## Execution Constraints

### All-at-Once Strategy
- Single project (BasketballScores.csproj) treated as atomic upgrade unit
- No tier ordering or phased rollout
- All project file changes, package updates, and fixes happen together

### Side-by-Side Web Migration Constraints
- Scaffold task (04-scaffold-basketballscores-blazor) must complete and validate (builds, stub 200 response) before migrate starts
- Old Framework project remains live and deployable throughout entire migrate phase
- Migrate task (05-migrate-basketballscores-ui) will be broken into subtasks at execution time — load migrating-aspnet-framework-to-core skill
- Data layer migration (task 03) handles dependencies before web layer migration begins
- YARP proxy enables incremental page migration while keeping both apps functional
- Reference cleanup and final routing switch are part of migrate task, not separate tasks
- Old Web Forms project is NOT deleted by the agent — documented as post-upgrade step for user

### Build and Test Requirements
- Zero warnings policy: all warnings in modified projects must be fixed before task completion
- Tests must pass after each major task (SDK conversion, data layer migration, page migrations)
- Solution must remain buildable after each task (no broken intermediate states)
