# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [BasketballScores.csproj](#basketballscorescsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | All require upgrade |
| Total NuGet Packages | 0 | All compatible |
| Total Code Files | 11 |  |
| Total Code Files with Incidents | 11 |  |
| Total Lines of Code | 1059 |  |
| Total Number of Issues | 546 |  |
| Estimated LOC to modify | 544+ | at least 51.4% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [BasketballScores.csproj](#basketballscorescsproj) | net48 | 🔴 High | 0 | 544 | 0 | 544+ | Wap, Sdk Style = False |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 0 | 0.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***0*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 302 | High - Require code changes |
| 🟡 Source Incompatible | 242 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 940 |  |
| ***Total APIs Analyzed*** | ***1484*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| ASP.NET Framework (System.Web) | 312 | 57.4% | Legacy ASP.NET Framework APIs for web applications (System.Web.*) that don't exist in ASP.NET Core due to architectural differences. ASP.NET Core represents a complete redesign of the web framework. Migrate to ASP.NET Core equivalents or consider System.Web.Adapters package for compatibility. |
| Legacy Configuration System | 6 | 1.1% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Web.UI.WebControls.TextBox | 54 | 9.9% | Binary Incompatible |
| P:System.Web.UI.WebControls.TextBox.Text | 45 | 8.3% | Binary Incompatible |
| T:System.Data.SqlClient.SqlParameterCollection | 32 | 5.9% | Source Incompatible |
| P:System.Data.SqlClient.SqlCommand.Parameters | 32 | 5.9% | Source Incompatible |
| T:System.Data.SqlClient.SqlParameter | 32 | 5.9% | Source Incompatible |
| M:System.Data.SqlClient.SqlParameterCollection.AddWithValue(System.String,System.Object) | 32 | 5.9% | Source Incompatible |
| T:System.Web.UI.WebControls.Literal | 23 | 4.2% | Binary Incompatible |
| T:System.Web.UI.WebControls.Label | 22 | 4.0% | Binary Incompatible |
| T:System.Web.UI.WebControls.Repeater | 16 | 2.9% | Binary Incompatible |
| T:System.Data.SqlClient.SqlConnection | 15 | 2.8% | Source Incompatible |
| P:System.Web.UI.Control.Visible | 13 | 2.4% | Binary Incompatible |
| P:System.Web.UI.WebControls.Literal.Text | 13 | 2.4% | Binary Incompatible |
| T:System.Data.SqlClient.SqlCommand | 12 | 2.2% | Source Incompatible |
| M:System.Data.SqlClient.SqlConnection.Open | 12 | 2.2% | Source Incompatible |
| M:System.Data.SqlClient.SqlCommand.#ctor(System.String,System.Data.SqlClient.SqlConnection) | 11 | 2.0% | Source Incompatible |
| T:System.Web.UI.WebControls.DropDownList | 11 | 2.0% | Binary Incompatible |
| T:System.Web.UI.WebControls.HiddenField | 11 | 2.0% | Binary Incompatible |
| T:System.Web.UI.WebControls.Button | 10 | 1.8% | Binary Incompatible |
| P:System.Web.UI.WebControls.HiddenField.Value | 9 | 1.7% | Binary Incompatible |
| M:System.Data.SqlClient.SqlCommand.ExecuteNonQuery | 7 | 1.3% | Source Incompatible |
| M:System.Data.SqlClient.SqlDataReader.Read | 6 | 1.1% | Source Incompatible |
| T:System.Data.SqlClient.SqlDataReader | 6 | 1.1% | Source Incompatible |
| M:System.Data.SqlClient.SqlCommand.ExecuteReader | 6 | 1.1% | Source Incompatible |
| P:System.Data.SqlClient.SqlCommand.CommandText | 6 | 1.1% | Source Incompatible |
| T:System.Web.UI.Control | 6 | 1.1% | Binary Incompatible |
| M:System.Web.UI.Control.FindControl(System.String) | 6 | 1.1% | Binary Incompatible |
| P:System.Web.UI.WebControls.ListControl.SelectedValue | 6 | 1.1% | Binary Incompatible |
| T:System.Web.UI.Page | 6 | 1.1% | Binary Incompatible |
| M:System.Data.SqlClient.SqlDataReader.GetInt32(System.Int32) | 5 | 0.9% | Source Incompatible |
| M:System.Web.UI.WebControls.Repeater.DataBind | 5 | 0.9% | Binary Incompatible |
| P:System.Web.UI.WebControls.Repeater.DataSource | 5 | 0.9% | Binary Incompatible |
| T:System.Web.UI.WebControls.Panel | 4 | 0.7% | Binary Incompatible |
| P:System.Web.UI.Page.IsPostBack | 4 | 0.7% | Binary Incompatible |
| M:System.Web.UI.Page.#ctor | 4 | 0.7% | Binary Incompatible |
| P:System.Web.UI.WebControls.CommandEventArgs.CommandName | 4 | 0.7% | Binary Incompatible |
| M:System.Data.SqlClient.SqlDataReader.GetOrdinal(System.String) | 3 | 0.6% | Source Incompatible |
| P:System.Web.UI.WebControls.WebControl.CssClass | 3 | 0.6% | Binary Incompatible |
| P:System.Web.UI.WebControls.Label.Text | 3 | 0.6% | Binary Incompatible |
| T:System.Web.HttpResponse | 3 | 0.6% | Source Incompatible |
| P:System.Web.UI.Page.Response | 3 | 0.6% | Binary Incompatible |
| M:System.Web.HttpResponse.Redirect(System.String) | 3 | 0.6% | Source Incompatible |
| M:System.Data.SqlClient.SqlDataReader.GetString(System.Int32) | 2 | 0.4% | Source Incompatible |
| T:System.Web.HttpRequest | 2 | 0.4% | Source Incompatible |
| P:System.Web.UI.Page.Request | 2 | 0.4% | Binary Incompatible |
| P:System.Web.HttpRequest.QueryString | 2 | 0.4% | Source Incompatible |
| P:System.Web.UI.WebControls.DropDownList.SelectedIndex | 2 | 0.4% | Binary Incompatible |
| T:System.Web.UI.WebControls.RepeaterCommandEventArgs | 2 | 0.4% | Binary Incompatible |
| P:System.Web.UI.WebControls.CommandEventArgs.CommandArgument | 2 | 0.4% | Binary Incompatible |
| P:System.Web.UI.Control.Page | 2 | 0.4% | Binary Incompatible |
| P:System.Web.UI.Page.IsValid | 2 | 0.4% | Binary Incompatible |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>⚙️&nbsp;BasketballScores.csproj</b><br/><small>net48</small>"]
    click P1 "#basketballscorescsproj"

```

## Project Details

<a id="basketballscorescsproj"></a>
### BasketballScores.csproj

#### Project Info

- **Current Target Framework:** net48
- **Proposed Target Framework:** net10.0
- **SDK-style**: False
- **Project Kind:** Wap
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 20
- **Number of Files with Incidents**: 11
- **Lines of Code**: 1059
- **Estimated LOC to modify**: 544+ (at least 51.4% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["BasketballScores.csproj"]
        MAIN["<b>⚙️&nbsp;BasketballScores.csproj</b><br/><small>net48</small>"]
        click MAIN "#basketballscorescsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 302 | High - Require code changes |
| 🟡 Source Incompatible | 242 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 940 |  |
| ***Total APIs Analyzed*** | ***1484*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 6 | 1.1% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |
| ASP.NET Framework (System.Web) | 312 | 57.4% | Legacy ASP.NET Framework APIs for web applications (System.Web.*) that don't exist in ASP.NET Core due to architectural differences. ASP.NET Core represents a complete redesign of the web framework. Migrate to ASP.NET Core equivalents or consider System.Web.Adapters package for compatibility. |

