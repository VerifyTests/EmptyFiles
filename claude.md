# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

All commands run from the repo root. There is no .sln file; use the `src` directory directly.

```bash
dotnet build src --configuration Release
dotnet test src --configuration Release --no-build --no-restore
dotnet publish src/EmptyFiles.AotTests --configuration Release
```

Run a single test:
```bash
dotnet test src/Tests --configuration Release --filter "FullyQualifiedName~Tests.TestClassName.MethodName"
```

## Project Structure

- **src/EmptyFiles/** — Main library (NuGet: `EmptyFiles`). Targets net462/net472/net48 + net6.0 through net11.0.
- **src/EmptyFiles.Tool/** — CLI global tool (`emptyfile` command). Targets net10.0.
- **src/Tests/** — NUnit tests with Verify snapshot testing. Targets net48 + net8.0 through net11.0.
- **src/EmptyFiles.AotTests/** — AOT/trimming compatibility tests (net9.0, PublishAot).
- **files/** — The actual empty template files organized by category: `archive/`, `document/`, `image/`, `sheet/`, `slide/`, `binary/`.
- **buildTransitive/EmptyFiles.targets** — MSBuild targets included in the NuGet package; copies empty files to output directory on build.

## Architecture

Single namespace `EmptyFiles` with four public static API classes:

- **AllFiles** — Main facade. Discovers and loads empty template files from the `files/` directory at static init. Provides file creation (`CreateFile`/`TryCreateFile`), lookup (`GetPathFor`/`TryGetPathFor`), validation (`IsEmptyFile`), and registration (`UseFile`). Files are stored in `FrozenDictionary<string, EmptyFile>` per `Category` enum (Archive, Document, Image, Sheet, Slide, Binary).
- **FileExtensions** — Text file detection. Maintains a set of ~443 known text extensions and supports custom conventions via `AddTextFileConvention(Func<CharSpan, bool>)`.
- **ContentTypes** — MIME type mapping between extensions and media types (200+ mappings).
- **EmptyFile** — Data class holding `Path`, `LastWriteTime`, and `Category` for each template file.

## Key Conventions

- **C# LangVersion: preview** with `TreatWarningsAsErrors` and `EnforceCodeStyleInBuild` enabled.
- **Global using alias:** `CharSpan` = `ReadOnlySpan<char>` (defined in `Directory.Build.props`). Many APIs have both `string` and `CharSpan` overloads.
- **Immutable collections:** Uses `FrozenDictionary` and `FrozenSet` for thread-safe, post-initialization data.
- **Try pattern:** Non-throwing variants (`TryCreateFile`, `TryGetPathFor`) alongside throwing versions.
- **Central package management:** Versions in `src/Directory.Packages.props`, shared config in `src/Directory.Build.props`.
- **CLS Compliant:** Assembly-level attribute enforced.
- **Snapshot testing:** Tests use Verify for snapshot assertions — expect `.verified.` files alongside tests.
- **.editorconfig:** Strict ReSharper rules enforced.
