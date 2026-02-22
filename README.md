# TextDock

A lightweight Windows desktop dock that displays your `.txt` files as clickable icons, built with WPF and .NET.

## What it does

TextDock watches a single folder (`C:\TextDockFiles`) and renders every `.txt` file inside it as a labelled icon in a floating, borderless window. Changes to the folder (files added, removed, or renamed) are reflected automatically without restarting the app.

- **Single-click** an icon to select it (highlighted in amber).
- **Double-click** an icon to open the file in the system's default text editor.
- **Drag** the window by clicking its background to reposition it anywhere on screen.
- Icons reflow into a grid automatically when the window is resized.

## Requirements

- Windows 10 / 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or the matching runtime for a published build)

## Getting started

1. Clone the repository.
2. Create the watched folder if it does not already exist:
   ```
   mkdir C:\TextDockFiles
   ```
3. Drop any `.txt` files you want to appear into that folder.
4. Run the application:
   ```
   dotnet run --project TextDock.csproj
   ```

## Building

```
dotnet build TextDock.csproj -c Release
```

For a self-contained Windows executable:

```
dotnet publish TextDock.csproj -c Release -r win-x64 --self-contained false
```

## Project structure

| Path | Purpose |
|---|---|
| `App.xaml / App.xaml.cs` | Application entry point and shell |
| `MainWindow.xaml / .cs` | Top-level window and event routing |
| `Controls/FileIconControl.xaml / FileIconControl.xaml.cs` | Reusable icon tile (file badge + label) |
| `Models/FileItem.cs` | View-model backing each icon |
| `ViewModels/MainViewModel.cs` | File loading, `FileSystemWatcher`, grid layout |

## Development notes

- `MainViewModel` implements `IDisposable`; the watcher and debounce timer are cleaned up when the window closes.
- The watched path is defined as `WatchedDirectory` in `MainViewModel.cs`. Update that constant to change the source folder.
- Run `dotnet format` before committing to normalise whitespace and member ordering.
- No automated tests exist yet. When adding them, place them in a `Tests/` directory using xUnit (`dotnet new xunit -n TextDock.Tests`) and run with `dotnet test`.
