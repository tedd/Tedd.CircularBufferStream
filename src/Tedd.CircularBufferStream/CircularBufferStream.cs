using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace Tedd;

public sealed class CircularBufferStream : Stream
{
    private readonly Pipe _pipe;
    private readonly Stream _readStream;
    private readonly Stream _writeStream;

    public CircularBufferStream()
    {
        _pipe = new Pipe(new PipeOptions(null, null, null, 0, 0, 4096, false));
        //_pipe = new Pipe(new PipeOptions(null, PipeScheduler.ThreadPool, PipeScheduler.ThreadPool, 0, 0, 1024, false));
        _readStream = _pipe.Reader.AsStream();
        _writeStream = _pipe.Writer.AsStream();
    }

    public CircularBufferStream(Pipe pipe)
    {
        _pipe = pipe;
        _readStream = _pipe.Reader.AsStream();
        _writeStream = _pipe.Writer.AsStream();
    }

    public CircularBufferStream(PipeOptions pipeOptions)
    {
        _pipe = new Pipe(pipeOptions);
        _readStream = _pipe.Reader.AsStream();
        _writeStream = _pipe.Writer.AsStream();
    }

    public Pipe Pipe { get => _pipe; }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => _readStream.Length;

    public override void Flush() => _writeStream.Flush();
    public override long Position { get => _readStream.Position; set => _readStream.Position = value; }
    public override long Seek(long offset, SeekOrigin origin) => _readStream.Seek(offset, origin);
    public override void SetLength(long value) => _readStream.SetLength(value);



#if !NETSTANDARD
    public ValueTask<int> ReadAsync(Memory<byte> buffer)
        => _readStream.ReadAsync(buffer);

#endif

    public override int Read(byte[] buffer, int offset, int count)
        => _readStream.Read(buffer, offset, count);

    public new Task<int> ReadAsync(byte[] buffer, int offset, int count)
        => _readStream.ReadAsync(buffer, offset, count);

    /// <summary>Cancels the pending ReadAsync() operation without causing it to throw. If there is no pending operation, this cancels the next operation.</summary>
    public void CancelPendingRead() => _pipe.Reader.CancelPendingRead();

    /// <summary>Asynchronously reads the bytes from the stream and writes them to the specified stream, using a specified cancellation token.</summary>
    /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="System.Threading.CancellationToken.None" />.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    public Task CopyToAsync(Stream stream, CancellationToken cancellationToken = default) => _pipe.Reader.CopyToAsync(stream, cancellationToken);

#if !NETSTANDARD
    public ValueTask WriteAsync(Memory<byte> buffer) => _readStream.WriteAsync(buffer);
    
#endif

    public override void Write(byte[] buffer, int offset, int count) => _writeStream.Write(buffer, offset, count);
    

    public new Task WriteAsync(byte[] buffer, int offset, int count) => _writeStream.WriteAsync(buffer, offset, count);
    
    
    public new void Dispose()
    {
        _readStream.Dispose();
        _writeStream.Dispose();
        base.Dispose();
    }
}