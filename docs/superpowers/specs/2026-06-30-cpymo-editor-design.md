# CPyMO Graphical Editor Design

Date: 2026-06-30

## Goal

Build a graphical, accessible .NET editor for CPyMO/PyMO projects on Windows and Android.

The editor uses native platform UI through .NET MAUI. It treats YukimiScript (`.ykm`) as the primary authoring format, while still supporting native PyMO `.txt` scripts. The editor is not a game previewer. Previewing and testing are done through CPyMO itself.

## Confirmed Scope

- Platforms: Windows and Android.
- UI framework: .NET MAUI single-project app, using native controls on each platform.
- Platform parity: Windows and Android must expose the same product capabilities.
- Main authoring workflow: graphical event-list editing, not code-first editing.
- Main script source format: YukimiScript (`.ykm`).
- Secondary script format: native PyMO `.txt`.
- Built-in preview: out of scope.
- CPyMO launch/testing: supported through an explicit command.
- Mobile YKM compilation: required.
- Mobile tooling: required for bundled CPyMO/PyMO tool workflows, including image conversion.
- Accessibility: required design constraint, not a final polish pass.

## Non-Goals

- Do not implement a CPyMO renderer inside the editor.
- Do not promise lossless recovery of YKM source from compiled PyMO output.
- Do not treat compiled YKM distribution scripts as editable PyMO projects.
- Do not force users to edit raw code for normal workflows.
- Do not rewrite CPyMO or YukimiScript business logic unless platform adaptation requires it.

## Repository Layout

```text
src/CpymoEditor/              .NET MAUI app
src/CpymoEditor.Core/         Project model, event model, compiler and tool abstractions
src/CpymoEditor.Tests/        Core parsing, mapping, and service tests
external/CPyMO/               Git submodule, initialized recursively
external/YukimiScript/        Git submodule
patches/cpymo/                CPyMO and cpymo-tool adaptation patches
patches/yukimiscript/         YukimiScript adaptation patches
tools/bootstrap.ps1           Initialize submodules and apply patches
tools/refresh-patches.ps1     Regenerate local patch files
docs/                         Product and developer documentation
```

CPyMO must be cloned as a recursive submodule because it contains submodules such as `stb`, `endianness.h`, SDL, and Android FFmpeg build tooling.

## Upstream And Tool Sources

Primary upstream sources:

- CPyMO: `https://github.com/Strrationalism/CPyMO`
- YukimiScript: `https://github.com/Strrationalism/YukimiScript`

Local reference materials:

- `PYMO教程.md`
- `YukimiScript教程.md`
- `pymo_v1_2_0_tools/`

Tool priority:

1. Prefer CPyMO's `cpymo-tool` implementation for overlapping functionality.
2. Use `pymo_v1_2_0_tools` as legacy PyMO reference, template source, or fallback for functions not covered by CPyMO tooling.
3. Do not ship legacy Windows-only binaries as the mobile implementation path.

CPyMO `cpymo-tool` currently covers asset analysis, asset filtering, conversion, image processing, packaging, spritesheet packing, resizing, stripping, FFmpeg integration hooks, and `gameconfig` handling. The legacy local tools include Android/S60 templates, `pymo_pack.exe`, `pymo_unpack.exe`, `image_converter_v1.0`, GoldWave, and winmenc.

## UI Model

The editor is tab-based to avoid overcrowding small Android screens.

Windows and Android must provide the same editor features. Layout may differ to match platform conventions and screen size, but the available tabs, supported event types, compiler targets, validation checks, asset operations, and tool workflows must remain equivalent unless a capability is explicitly marked unavailable for a documented platform limitation.

Core tabs:

- Events: paged linear event list for the currently opened script.
- Add: event picker and parameter form for inserting new events.
- Assets: categorized asset library.
- Config: graphical editor for `gameconfig.txt`.
- Problems: compiler errors, resource warnings, validation results.
- Tools: conversion, checking, packaging, and CPYMO/PyMO tool workflows.
- Source: advanced raw source view for unsupported script fragments.

Android uses a bottom tab bar or scrollable top tabs. Each screen focuses on one task. Windows may use top tabs or a side navigation layout. Windows can show an event details pane beside the list, but the tab still owns the task.

## Event List Editor

CPyMO/PyMO scripts execute linearly, so the editor presents a long, paged, top-to-bottom event list.

Event list behavior:

- Opening a script file shows its event list by default.
- The list is paginated and supports previous page, next page, jump to label, search, and page jump.
- Event operations include insert, duplicate, delete, move up, and move down.
- Multi-line structures such as `#sel` are represented as a single compound event.
- Unknown content is represented as a raw event and preserved unless the user explicitly edits it.

Event examples:

- Dialogue: speaker plus text.
- Background: asset, transition, duration, optional crop coordinates.
- Character: character ID, file, position, layer, duration.
- BGM, SE, voice, video.
- Variable set/add/sub/random.
- Label, goto, conditional goto, call, change, return.
- Selection menus.
- Wait, wait key, wait SE.
- System actions such as load, album, music, date.

## Add Event Workflow

The Add tab groups events by function:

- Text
- Image
- Sound
- Variable and jump
- Selection
- System
- YKM macro or extension

Selecting an event opens a graphical parameter form. File parameters are selected from the asset library. Coordinates, numbers, booleans, colors, and enums use platform-native controls such as numeric input, sliders or steppers, switches, color pickers, and picker menus.

The user should not need to type code for ordinary PyMO/YKM authoring.

## Asset Library

The asset library scans and manages standard project directories:

- `bg/`
- `bgm/`
- `chara/`
- `script/`
- `se/`
- `system/`
- `video/`
- `voice/`

Assets are grouped by type. The library supports import, rename, delete, conversion, reference checking, and missing-reference reporting. Deleting or renaming an asset that is referenced by events requires explicit confirmation.

Android import uses the system file picker and storage access APIs. The app must not depend on arbitrary absolute filesystem paths on Android.

## Script Modes

The editor supports two editable project modes.

### YKM Source Mode

YKM source mode is enabled only when real `.ykm` source files are present.

- Graphical events save back to `.ykm`.
- Compilation generates PyMO `.txt`.
- `libpymo.ykm` is loaded from the CPyMO submodule or from a project override.
- Compiler diagnostics map back to YKM files and lines where possible.
- The generated PyMO output directory must be distinct from the source directory.

### Native PyMO Script Mode

Native PyMO mode is for original `.txt` scripts.

- Graphical events save back to `.txt`.
- PyMO instructions such as `#say`, `#bg`, `#chara`, `#sel`, `#if`, `#goto`, `#bgm`, `#se`, `#vo`, and `#wait` map to event types.
- Unknown PyMO instructions and comments are preserved as raw events.
- No YKM source recovery is attempted.

## YKM Compiled Product Rejection

Compiled YKM output is materially different from YKM source. Macro expansion, scene ordering, generated labels, debug comments, and low-level PyMO instructions make it non-reversible.

If the editor detects that an opened game is a YKM-compiled distribution product, it refuses to open it for editing.

Detection signals include:

- Dense `;YKMDBG;` debug metadata in PyMO scripts.
- YukimiScript CodeGen-specific output patterns.
- Generated script layout known to differ from hand-authored PyMO scripts.
- Future metadata markers added by this editor's compiler output.

Refusal behavior:

- Show a clear message that the project appears to be generated from YKM and cannot be safely edited as source.
- Do not offer "open as PyMO anyway".
- Do not offer "recover original YKM project".
- Suggest opening the original `.ykm` source project instead.

This rejection does not apply to native PyMO scripts that were not generated from YKM.

## Compiler Integration

Android cannot rely on a global `ykmc` command-line tool. The editor embeds the compiler as libraries.

`IYkmCompilerService`:

- Input: source file or project, library paths, output directory, target format.
- Output: generated PyMO files, structured errors, warnings, logs.
- Required target: PyMO.

Implementation:

- Reference or adapt `YukimiScript.Parser`.
- Reference or adapt `YukimiScript.CodeGen`.
- Keep patches minimal and stored under `patches/yukimiscript/`.
- Validate F# and MAUI Android compatibility, especially trimming and AOT behavior.

## CPyMO Tool Integration

`IToolService` abstracts tool operations from UI.

Feature parity requirement:

- Every required tool category must have both a Windows implementation and an Android implementation.
- If a platform-specific implementation is temporarily incomplete, the feature must be marked experimental or unavailable on both platforms until parity is restored.
- Different implementation mechanisms are allowed, such as Windows command execution and Android native library calls, but user-visible behavior, inputs, outputs, validation, progress, cancellation, and error reporting must match.

Required tool categories:

- Image conversion and mask/alpha processing.
- Asset analysis and reference checking.
- Asset filtering.
- Packaging and unpacking where supported.
- Spritesheet and resize operations where useful.
- Gameconfig validation.

Windows implementation:

- Build and invoke CPyMO `cpymo-tool` where practical.
- Wrap outputs into structured logs and problem items.

Android implementation:

- Build portable C tool functionality through Android NDK as native libraries or isolated executables.
- Expose through MAUI platform services.
- Use cancellable operations with progress reporting.
- Use app-private storage and Android storage access APIs.

FFmpeg-related functionality:

- Treat as optional or separately packaged if size, licensing, or performance is a problem.
- Prefer explicit user action and clear status over background conversion.

## CPyMO Launch And Testing

The editor does not preview gameplay.

Windows:

- Let the user configure a CPyMO executable path or use a bundled build.
- Run the current output project through an explicit "Run with CPyMO" command.

Android:

- Launch an installed or bundled CPyMO runtime if available.
- Pass or copy the selected output project through a documented platform path.

Launch is never triggered by focus changes, file selection, or tab switches.

## Accessibility Requirements

Accessibility is a product constraint.

Required behavior:

- Every focusable or actionable control has a concise accessible name.
- Tab order follows the task flow.
- Every feature is usable without mouse or touch-only gestures.
- State changes, validation errors, compiler results, and tool completion are perceivable.
- Focus is restored predictably after dialogs, delete confirmations, compile results, and tool runs.
- Selection changes do not compile, convert, write files, launch CPyMO, or perform destructive actions.
- Destructive or overwriting operations require confirmation.
- Color, icon shape, animation, or position are never the only source of meaning.
- Android touch targets follow platform size expectations.
- Windows menus and shortcuts provide full keyboard access.

## Error Handling

- Parser errors appear in the Problems tab and can navigate to the source event or raw source location.
- YKM compiler errors point to YKM file and line when available.
- PyMO parser errors point to `.txt` file and line.
- Tool failures show command, input/output paths, exit status if applicable, and readable error text.
- Long operations support cancellation where feasible.
- Output overwrite conflicts are listed before the user confirms.

## Testing Strategy

Core tests:

- Parse YKM event files.
- Parse native PyMO scripts.
- Detect and reject YKM-compiled output.
- Preserve unknown YKM macros and unknown PyMO instructions.
- Round-trip open/save without changing untouched raw content.
- Map multi-line `#sel` into one event and back.
- Compile sample YKM into PyMO.
- Validate missing assets and broken references.

Tool tests:

- Exercise image conversion.
- Exercise asset analysis/filtering.
- Exercise packaging where available.
- Validate `gameconfig.txt` parsing and writing.

Accessibility tests:

- Keyboard-only workflow on Windows.
- Screen reader names for tabs, event rows, forms, problem entries, and tool actions.
- Focus restoration after dialogs and operations.
- High zoom and large text behavior.

Platform tests:

- Windows desktop layout.
- Android small-screen tab navigation.
- Android file import via system picker.
- Android compiler and native tool execution.
- Cross-platform parity tests that assert the same project can be opened, edited, validated, compiled, converted, and packaged on both Windows and Android with equivalent results.

## Open Implementation Risks

- YukimiScript is F#. MAUI Android compatibility, trimming, and AOT behavior must be verified early.
- CPyMO C tools may need Android-specific file and path adaptation.
- FFmpeg integration may be too large for the base mobile package.
- Graphical editing of arbitrary user-defined YKM macros is not always possible. Unknown macro calls must remain raw or macro-call events.
- YKM compiled output detection must be strict enough to prevent accidental editing but not reject ordinary hand-authored PyMO scripts.
