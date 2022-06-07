# Tedd.CircularBufferStream
High performance circular buffer Stream using System.IO.Pipelines.

This functions similarly as a MemoryStream, except as a circular memory buffer.

Memory will dynamically increase as data is written to the stream.
Memory will dynamically decrease as data is read from the stream.

Reading with no data in the stream will block until data is written to the stream. It will however not necessarilly wait until enough data is written so read buffer is full upon return.
Writing without reading will eventually block the writer until reading frees up buffer space. See PipeOptions https://docs.microsoft.com/en-us/dotnet/api/system.io.pipelines.pipeoptions?view=dotnet-plat-ext-6.0 for info on PauseWriterThreshold and ResumeWriterThreshold. Default PauseWriterThreshold is 65536 bytes.


