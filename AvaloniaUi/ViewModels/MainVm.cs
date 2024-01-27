using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaUi.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUi.ViewModels;

public partial class MainVm : VmBase
{
    [ObservableProperty]
    public ObservableCollection<string> errorMessages = new ObservableCollection<string>();

    [RelayCommand]
    private void OpenGenerator()
    {
        var generatorWindow = new MatrixGeneratorWindow() {
            DataContext = new MatrixGeneratorVm(),
        };
        generatorWindow.Show();
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new Exception("Is not a desktop application");
        }
        generatorWindow.Closing += (sender, e) => desktop?.MainWindow?.Show();
        desktop?.MainWindow?.Hide();
        generatorWindow.Show();
    }


    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try {
            var file = await DoOpenFilePickerAsync();
            if (file == null)
            {
                return;
            }
            
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                throw new Exception("Is not a desktop application");
            }
            
            var viewerWindow = new MatrixViewerWindow("matrix.bin");
            desktop?.MainWindow?.Hide();
            viewerWindow.Show();
            viewerWindow.Closing += (sender, e) => desktop?.MainWindow?.Show();
        } 
        catch(Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Matrix File",
            AllowMultiple = false,
        });

        return files?.Count == 1 ? files[0] : null;
    }
}