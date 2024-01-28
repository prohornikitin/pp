using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace AvaloniaUi.Services;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public async Task<IStorageFile?> OpenFilesAsync(string title)
    {
        var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = title,
            AllowMultiple = true,
        });

        return files.Count > 0 ? files[0] : null;
    }

    public async Task<IStorageFile?> SaveFileAsync(string title)
    {
        if(Debugger.IsAttached && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //For some reason file save do not work properly with Debugger;
            return new MockStorageFile();
        }
        return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = title
        });
    }
}