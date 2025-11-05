using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextDock.Models;

public class FileItem : INotifyPropertyChanged
{
    private string _fullPath = string.Empty;
    private string _fileName = string.Empty;
    private double _x;
    private double _y;
    private bool _isSelected;

    public string FullPath
    {
        get => _fullPath;
        set => SetField(ref _fullPath, value);
    }

    public string FileName
    {
        get => _fileName;
        set => SetField(ref _fileName, value);
    }

    public double X
    {
        get => _x;
        set => SetField(ref _x, value);
    }

    public double Y
    {
        get => _y;
        set => SetField(ref _y, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value!;
        OnPropertyChanged(propertyName);
    }
}
