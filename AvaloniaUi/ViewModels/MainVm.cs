using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaUi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaUi.ViewModels;

public partial class MainVm : VmBase
{
    public ObservableCollection<TabVm> Tabs { get; } = new ObservableCollection<TabVm>();

    public MainVm() {
        OpenNewTab(TabVm.CreateGeneratorTab());
    }

    private void OpenNewTab(TabVm vm) {
        vm.OnClose += (sender) => {
            Tabs.Remove(sender);
        };
        Tabs.Add(vm);
    }
    
    [ObservableProperty]
    public ObservableCollection<string> errorMessages = new ObservableCollection<string>();

    [RelayCommand]
    private void OpenGenerator()
    {
        OpenNewTab(TabVm.CreateGeneratorTab());
    }


    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService == null)
            {
                throw new NullReferenceException("Missing File Service instance.");
            }

            var file = await filesService.OpenFilesAsync();
            if (file == null)
            {
                return;
            }
            OpenNewTab(TabVm.CreateViewerTab(file.Path.LocalPath));
        } 
        catch(Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    // private async Task<IStorageFile?> DoOpenFilePickerAsync()
    // {
    //     if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
    //         desktop.MainWindow?.StorageProvider is not { } provider)
    //         throw new NullReferenceException("Missing StorageProvider instance.");

    //     var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
    //     {
    //         Title = "Open Matrix File",
    //         AllowMultiple = false,
    //     });

    //     return files?.Count == 1 ? files[0] : null;
    // }
}