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

        public override long Length => throw new NotSupportedException();

        public override void Flush() { }
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

#if !NETSTANDARD        
        public ValueTask<int> ReadAsync(Memory<byte> buffer)
            => _readStream.ReadAsync(buffer);
        
#endif
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _readStream.Read(buffer, offset, count);
            //return ReadAsync(buffer.AsMemory().Slice(offset, count)).Result;
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return _readStream.ReadAsync(buffer, offset, count);
        }
        //public async Task<int> ReadAsync(Memory<byte> target)
        //{
        //    if (target.Length == 0)
        //        return 0;
        //    ReadResult result = await _pipe.Reader.ReadAsync();

        //    var buffer = result.Buffer;
        //    var len = Math.Min(target.Length, result.Buffer.Length);
        //    // Source
        //    buffer = buffer.Slice(0, len);

        //    if (buffer.IsSingleSegment)
        //    {
        //        // Short-circuit if there is only one
        //        buffer.First.Span.CopyTo(target.Span);
        //    }
        //    else
        //    {
        //        // Copy each element and slice down target
        //        foreach (var segment in buffer)
        //        {
        //            segment.Span.CopyTo(target.Span);
        //            target = target.Slice(segment.Length);
        //        }
        //    }
        //    _pipe.Reader.AdvanceTo(buffer.End);
        //    return (int)len;
        //}


#if !NETSTANDARD
        public ValueTask WriteAsync(Memory<byte> buffer)
            => _readStream.WriteAsync(buffer);

#endif
        public override void Write(byte[] buffer, int offset, int count)
            => _writeStream.Write(buffer, offset, count);

        public Task WriteAsync(byte[] buffer, int offset, int count)
            => _writeStream.WriteAsync(buffer, offset, count);

        //if (count == 0)
        //    return;
        //var wt = WriteAsync(buffer.AsMemory(offset, count));
        //wt.GetAwaiter().GetResult();
    //}

    //public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    //{
    //    // Allocate memory and copy data to it
    //    var memory = _pipe.Writer.GetMemory(buffer.Length);
    //    buffer.CopyTo(memory);
    //    // Tell the PipeWriter how much was read
    //    _pipe.Writer.Advance(buffer.Length);
    //    // Make the data available to the PipeReader
    //    FlushResult result = await _pipe.Writer.FlushAsync();        
    //}

    public new void Dispose()
        {
            _readStream.Dispose();
            _writeStream.Dispose();
            base.Dispose();
        }
    }
}
