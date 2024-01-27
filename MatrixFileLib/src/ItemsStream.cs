namespace MatrixFile;
public abstract class ItemsStream : Stream {
    protected static int itemSize = sizeof(int);
    private byte[] itemBuffer = new byte[itemSize];

    public int ReadItem()
    {
        Read(itemBuffer, 0, itemSize);
        return BitConverter.ToInt32(itemBuffer);
    }

    public void WriteItem(int item)
    {
        Write(BitConverter.GetBytes(item), 0, itemSize);
    }

    public async Task<int> ReadItemAsync()
    {
        await ReadAsync(itemBuffer, 0, itemSize);
        return BitConverter.ToInt32(itemBuffer);
    }

    public async Task WriteItemAsync(int item)
    {
        await WriteAsync(BitConverter.GetBytes(item), 0, itemSize);
    }

    public void SeekItem(long offset, SeekOrigin origin)
    {
        Seek(offset * itemSize, origin);
    }
}