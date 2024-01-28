using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace AvaloniaUi.Services;

public interface IFilesService
{
    public Task<IStorageFile?> OpenFilesAsync(string title);
    public Task<IStorageFile?> OpenFilesAsync()
    {
        return OpenFilesAsync("Open File");
    }

    public Task<IStorageFile?> SaveFileAsync(string title);
    public Task<IStorageFile?> SaveFileAsync()
    {
        return SaveFileAsync("Save File");
    }
}