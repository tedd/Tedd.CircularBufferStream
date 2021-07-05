using System;
using Xunit;
using Tedd;
using System.Threading.Tasks;
using System.IO;

namespace Tedd.CircularBufferStreamTest
{
    public class UnitTest1
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
    }
}
