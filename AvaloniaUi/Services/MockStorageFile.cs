
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

public class MockStorageFile : IStorageFile
{
    public string Name => "null";

    public Uri Path => new Uri("/dev/null");

    public bool CanBookmark => false;

    public Task DeleteAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        
    }

    public Task<StorageItemProperties> GetBasicPropertiesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IStorageFolder?> GetParentAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IStorageItem?> MoveAsync(IStorageFolder destination)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> OpenReadAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Stream> OpenWriteAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string?> SaveBookmarkAsync()
    {
        throw new NotImplementedException();
    }
}