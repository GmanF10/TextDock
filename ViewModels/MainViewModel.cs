using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using TextDock.Models;

namespace TextDock.ViewModels;

public class MainViewModel : IDisposable
{
    private const string WatchedDirectory = @"C:\TextDockFiles";
    private const double IconWidth = 64;
    private const double IconHeight = 80;
    private const double IconSpacing = 16;

    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _debounceTimer;
    private FileSystemWatcher? _watcher;
    private double _lastAvailableWidth;
    private bool _disposed;

    public ObservableCollection<FileItem> FileItems { get; } = new();

    public MainViewModel()
    {
        _dispatcher = Application.Current.Dispatcher;
        _debounceTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        _debounceTimer.Tick += DebounceTimer_Tick;
    }

    public void Initialize()
    {
        LoadFiles();
        InitializeFileWatcher();
    }

    public void ArrangeFileItems(double availableWidth)
    {
        _lastAvailableWidth = availableWidth;

        if (FileItems.Count == 0)
        {
            return;
        }

        availableWidth = Math.Max(IconWidth, availableWidth);
        var columns = Math.Max(1, (int)Math.Floor((availableWidth + IconSpacing) / (IconWidth + IconSpacing)));

        for (var index = 0; index < FileItems.Count; index++)
        {
            var column = index % columns;
            var row = index / columns;
            FileItems[index].X = column * (IconWidth + IconSpacing);
            FileItems[index].Y = row * (IconHeight + IconSpacing);
        }
    }

    public void SelectFile(FileItem target)
    {
        foreach (var item in FileItems)
        {
            item.IsSelected = item == target;
        }
    }

    public void OpenFile(FileItem fileItem)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fileItem.FullPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to open file:\n{ex.Message}",
                "TextDock",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void LoadFiles()
    {
        FileItems.Clear();

        if (!Directory.Exists(WatchedDirectory))
        {
            return;
        }

        foreach (var filePath in Directory.EnumerateFiles(WatchedDirectory, "*.txt"))
        {
            FileItems.Add(new FileItem
            {
                FullPath = filePath,
                FileName = Path.GetFileName(filePath)
            });
        }
    }

    private void InitializeFileWatcher()
    {
        if (!Directory.Exists(WatchedDirectory))
        {
            return;
        }

        _watcher = new FileSystemWatcher(WatchedDirectory, "*.txt")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _watcher.Created += OnFileSystemChanged;
        _watcher.Deleted += OnFileSystemChanged;
        _watcher.Renamed += OnFileSystemRenamed;
        _watcher.Error += OnWatcherError;
    }

    private void OnFileSystemChanged(object sender, FileSystemEventArgs e) =>
        ScheduleReload();

    private void OnFileSystemRenamed(object sender, RenamedEventArgs e) =>
        ScheduleReload();

    private void ScheduleReload()
    {
        _dispatcher.InvokeAsync(() =>
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        });
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        _dispatcher.InvokeAsync(() =>
        {
            DisposeFileWatcher();
            InitializeFileWatcher();
        });
    }

    private void DebounceTimer_Tick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        LoadFiles();
        ArrangeFileItems(_lastAvailableWidth);
    }

    private void DisposeFileWatcher()
    {
        if (_watcher is null)
        {
            return;
        }

        _watcher.EnableRaisingEvents = false;
        _watcher.Created -= OnFileSystemChanged;
        _watcher.Deleted -= OnFileSystemChanged;
        _watcher.Renamed -= OnFileSystemRenamed;
        _watcher.Error -= OnWatcherError;
        _watcher.Dispose();
        _watcher = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _debounceTimer.Stop();
        _debounceTimer.Tick -= DebounceTimer_Tick;
        DisposeFileWatcher();
        GC.SuppressFinalize(this);
    }
}
