# Upgrade Options — Basketball Scores

Assessment: 1 project (net48), High complexity, 544 API issues (57% System.Web), single Web Forms application

## Strategy

### Upgrade Strategy
Single Web Forms project with no dependencies — straightforward atomic migration.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade the project in a single atomic pass — simplest approach for single-project solutions |

## Project Structure

### Project Approach
Web Forms project with heavy System.Web usage (312 issues) migrating to modern .NET — this requires an ASP.NET Core rewrite.

| Value | Description |
|-------|-------------|
| **Side-by-side** (selected) | Create a new Blazor project alongside the existing Web Forms project, migrate pages incrementally using YARP proxy routing, keep old project live during migration |
| In-place rewrite | Replace the Framework web project entirely in one pass — higher risk, but acceptable for small projects |

**Note**: Given your stated goal to "modernize this application to Blazor," Side-by-side is recommended. This approach:
- Allows incremental migration from Web Forms to Blazor components
- Uses YARP reverse proxy to route requests between old and new apps
- Keeps the existing Web Forms app running until migration completes
- Provides a safe rollback path

## Compatibility

### Unsupported API Handling
Breaking changes detected: 544 API issues (302 binary incompatible, 242 source incompatible), primarily System.Web APIs.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change during migration — no deferred work or stubs to clean up later |
| Defer Complex Changes | Apply simple replacements immediately, stub complex changes for later resolution |
