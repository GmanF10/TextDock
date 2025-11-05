using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TextDock.Models;

namespace TextDock;

public partial class MainWindow : Window
{
    private const string WatchedDirectory = @"C:\TextDockFiles";
    private const double IconWidth = 64;
    private const double IconHeight = 80;
    private const double IconSpacing = 16;

    public ObservableCollection<FileItem> FileItems { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += MainWindow_Loaded;
        SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadFiles();
        ArrangeFileItems();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ArrangeFileItems();
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
            var fileItem = new FileItem
            {
                FullPath = filePath,
                FileName = Path.GetFileName(filePath)
            };

            FileItems.Add(fileItem);
        }
    }

    private void ArrangeFileItems()
    {
        if (FileItems.Count == 0)
        {
            return;
        }

        var availableWidth = IconCanvas.ActualWidth;

        if (availableWidth <= 0)
        {
            availableWidth = Math.Max(0, ActualWidth - WindowBorder.BorderThickness.Left - WindowBorder.BorderThickness.Right - IconCanvas.Margin.Left - IconCanvas.Margin.Right);
        }

        availableWidth = Math.Max(IconWidth, availableWidth);

        var columns = Math.Max(1, (int)Math.Floor((availableWidth + IconSpacing) / (IconWidth + IconSpacing)));

        for (var index = 0; index < FileItems.Count; index++)
        {
            var column = index % columns;
            var row = index / columns;

            var x = column * (IconWidth + IconSpacing);
            var y = row * (IconHeight + IconSpacing);

            FileItems[index].X = x;
            FileItems[index].Y = y;
        }
    }

    private void FileIconControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not FileItem fileItem)
        {
            return;
        }

        foreach (var item in FileItems)
        {
            item.IsSelected = item == fileItem;
        }

        e.Handled = true;
    }

    private void FileIconControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not FileItem fileItem)
        {
            return;
        }

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
            MessageBox.Show(this, $"Unable to open file:\n{ex.Message}", "TextDock", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        e.Handled = true;
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
