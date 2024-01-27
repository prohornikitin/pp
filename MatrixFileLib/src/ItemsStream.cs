namespace MatrixFile;
public abstract class ItemsStream : Stream {
    protected static int itemSize = sizeof(int);
    private byte[] oneItemBuffer = new byte[itemSize];
    public int ReadItem()
    {
        Read(oneItemBuffer, 0, itemSize);
        return BitConverter.ToInt32(oneItemBuffer);
    }

    public void WriteItem(int item)
    {
        Write(BitConverter.GetBytes(item), 0, itemSize);
    }

    public async Task<int> ReadItemAsync(CancellationToken cancel = new CancellationToken())
    {
        await ReadAsync(oneItemBuffer, 0, itemSize, cancel);
        return BitConverter.ToInt32(oneItemBuffer);
    }

    public async Task WriteItemAsync(int item, CancellationToken cancel = new CancellationToken())
    {
        await WriteAsync(BitConverter.GetBytes(item), 0, itemSize, cancel);
    }

    public void SeekItem(long offset, SeekOrigin origin)
    {
        Seek(offset * itemSize, origin);
    }

    public override void Flush()
    {
        Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(buffer, offset, count);
    }
}