using System;
using System.Windows;
using System.Windows.Input;
using TextDock.Models;
using TextDock.ViewModels;

namespace TextDock;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += MainWindow_Loaded;
        SizeChanged += MainWindow_SizeChanged;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Initialize();
        _viewModel.ArrangeFileItems(GetAvailableWidth());
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _viewModel.ArrangeFileItems(GetAvailableWidth());
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _viewModel.Dispose();
    }

    private double GetAvailableWidth()
    {
        var width = IconCanvas.ActualWidth;

        if (width <= 0)
        {
            width = Math.Max(0, ActualWidth
                - WindowBorder.BorderThickness.Left - WindowBorder.BorderThickness.Right
                - IconCanvas.Margin.Left - IconCanvas.Margin.Right);
        }

        return width;
    }

    private void FileIconControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FileItem fileItem })
        {
            _viewModel.SelectFile(fileItem);
            e.Handled = true;
        }
    }

    private void FileIconControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FileItem fileItem })
        {
            _viewModel.OpenFile(fileItem);
            e.Handled = true;
        }
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
