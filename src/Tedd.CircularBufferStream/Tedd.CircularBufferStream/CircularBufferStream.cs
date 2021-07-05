using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Tedd
{
    public sealed class CircularBufferStream : Stream, IDisposable
    {
        private readonly Pipe _pipe;
        private readonly Stream _readStream;
        private readonly Stream _writeStream;

        public CircularBufferStream()
        {
            _pipe = new Pipe(new PipeOptions(null, PipeScheduler.ThreadPool, PipeScheduler.ThreadPool, 0, 0, 1024, false));
            _readStream = _pipe.Reader.AsStream();
            _writeStream = _pipe.Writer.AsStream();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _readStream.Length;

        public override void Flush() => _writeStream.Flush(); 
        public override long Position { get => _readStream.Position; set => _readStream.Position = value; }
        public override long Seek(long offset, SeekOrigin origin) => _readStream.Seek(offset,origin);
        public override void SetLength(long value) => _readStream.SetLength(value);

#if !NETSTANDARD        
        public ValueTask<int> ReadAsync(Memory<byte> buffer)
            => _readStream.ReadAsync(buffer);

#endif
        public override int Read(byte[] buffer, int offset, int count)
            => _readStream.Read(buffer, offset, count);

        public new Task<int> ReadAsync(byte[] buffer, int offset, int count)
            => _readStream.ReadAsync(buffer, offset, count);


#if !NETSTANDARD
        public ValueTask WriteAsync(Memory<byte> buffer)
            => _readStream.WriteAsync(buffer);

#endif
        public override void Write(byte[] buffer, int offset, int count)
            => _writeStream.Write(buffer, offset, count);

        public new Task WriteAsync(byte[] buffer, int offset, int count)
            => _writeStream.WriteAsync(buffer, offset, count);

        public new void Dispose()
        {
            _readStream.Dispose();
            _writeStream.Dispose();
            base.Dispose();
        }
    }
}
