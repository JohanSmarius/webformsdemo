# .NET Version Upgrade Tasks

## Project Hierarchy
- **BasketballScores.csproj** (Web Forms, net48 → SDK-style → side-by-side Blazor migration)

---

## Task Status

### 01-prerequisites
**Description**: Validate upgrade prerequisites
**Status**: completed
**Depends on**: —

---

### 02-sdk-conversion
**Description**: Convert project to SDK-style format
**Status**: pending
**Depends on**: 01-prerequisites

---

### 03-data-layer-migration
**Description**: Migrate data access from ADO.NET to EF Core
**Status**: pending
**Depends on**: 02-sdk-conversion

---

### 04-scaffold-basketballscores-blazor
**Description**: Scaffold new Blazor Server project with YARP proxy
**Status**: pending
**Depends on**: 03-data-layer-migration

---

### 05-migrate-basketballscores-ui
**Description**: Migrate Web Forms pages to Blazor components
**Status**: pending
**Depends on**: 04-scaffold-basketballscores-blazor

---

### 06-final-validation
**Description**: Validate complete migration
**Status**: pending
**Depends on**: 05-migrate-basketballscores-ui
