# Repository Guidelines

## Project Structure & Module Organization
- `App.xaml` and `App.xaml.cs` bootstrap the WPF application shell.
- `MainWindow.xaml(.cs)` hosts layout logic for rendering `.txt` shortcuts sourced from `C:\TextDockFiles`.
- `Controls/` contains reusable UI elements such as `FileIconControl`.
- `Models/FileItem.cs` defines the view-model backing each icon and is the right place to extend metadata or selection state.

## Build, Test, and Development Commands
- `dotnet restore` ensures dependencies for `TextDock.csproj` are available.
- `dotnet build TextDock.csproj -c Debug` compiles the app; use `-c Release` when packaging.
- `dotnet run --project TextDock.csproj` launches the desktop app for local verification.
- `dotnet publish TextDock.csproj -c Release -r win-x64 --self-contained false` creates a deployable Windows build without bundling the runtime.

## Coding Style & Naming Conventions
- Follow standard C# conventions: 4-space indentation, file-scoped namespaces, PascalCase for types, and camelCase for locals.
- Keep constants in `UPPER_CAMEL` as seen in `MainWindow.xaml.cs`, and prefer expression-bodied members for simple property accessors.
- Keep XAML names descriptive (`IconCanvas`, `WindowBorder`) and co-locate templates with their controls in `Controls/`.
- Run `dotnet format` prior to commits to normalize whitespace and ordering.

## Testing Guidelines
- No automated tests exist yet; introduce them in a `Tests/` directory using xUnit (`dotnet new xunit -n TextDock.Tests`).
- Name test classes after the type under test (e.g., `FileItemTests`) and test methods in `MethodName_ShouldExpectedBehavior` form.
- Target key behaviors: file discovery in `MainWindow.LoadFiles`, icon arrangement, and click/double-click interactions.
- Execute `dotnet test` from the repository root once tests are in place.

## Commit & Pull Request Guidelines
- Use short, imperative commit subjects under ~50 characters (e.g., `Add grid layout spacing guard`), and add detail in the body when rationale is non-trivial.
- Group related changes per commit; avoid mixing feature updates, refactors, and formatting in one change.
- PRs should include: purpose summary, testing notes (`dotnet build`, `dotnet run`, `dotnet test`), and screenshots or GIFs when altering UI layout.
- Reference related issues with `Fixes #123` syntax so automation links the work.

## Environment Notes
- The app watches `C:\TextDockFiles`; keep this path configurable through future contributions and document any changes.
- For feature work that manipulates the file system, use lightweight `.txt` fixtures like `test1.txt`–`test3.txt` already in the repo.
