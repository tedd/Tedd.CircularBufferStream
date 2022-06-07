using System;
using Xunit;
using Tedd;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Tedd.CircularBufferStreamTest;

    public class CBStreamTest
    {
        [Fact]
        public async Task TestThousand()
        {
            var rnd = new Random();
            for (var i = 1; i < 1_000; i++)
            {
                var inBuffer = new byte[i];
                var outBuffer = new byte[i ];
                rnd.NextBytes(inBuffer);
                var stream = new CircularBufferStream();
                await stream.WriteAsync(inBuffer,0,inBuffer.Length);
                var sw = stream.ReadAsync(outBuffer, 0, outBuffer.Length);
                var len = await sw;
                
                Assert.Equal(len, inBuffer.Length);
                Assert.Equal(inBuffer, outBuffer);
                
            }
        }

        [Fact]
        public async Task TestRandom()
        {
            var rnd = new Random();
            for (var i = 0; i < 100; i++)
            {
                var size = rnd.Next(1, 1_000_000);
                var inBuffer = new byte[size];
                var outBuffer = new byte[size];
                rnd.NextBytes(inBuffer);
                var stream = new CircularBufferStream();
                await stream.WriteAsync(inBuffer, 0, inBuffer.Length);
                var sw = stream.ReadAsync(outBuffer, 0, outBuffer.Length);
                var len = await sw;

                Assert.Equal(len, inBuffer.Length);
                Assert.Equal(inBuffer, outBuffer);

            }
        }
        
        [Fact]
        public async Task TestBlockingRead()
        {
                var inBuffer = new byte[1000];
                var outBuffer = new byte[100];

                var stream = new CircularBufferStream();
                var sw = stream.ReadAsync(outBuffer, 0, outBuffer.Length);
                Thread.Sleep(1000);
                Assert.False(sw.IsCompleted);
                await stream.WriteAsync(inBuffer, 0, inBuffer.Length);
                Thread.Sleep(1000);
                Assert.True(sw.IsCompleted);
            
        }

        [Fact]
        public async Task TestBlockingRead_CancelPendingRead()
        {
            var inBuffer = new byte[1000];
            var outBuffer = new byte[100];

            var stream = new CircularBufferStream();
            var sw = stream.ReadAsync(outBuffer, 0, outBuffer.Length);
            Thread.Sleep(1000);
            Assert.False(sw.IsCompleted);
            stream.CancelPendingRead();
            Thread.Sleep(1000);
            Assert.True(sw.IsCompleted);

        }  
        
}
