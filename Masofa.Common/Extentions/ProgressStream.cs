namespace Masofa.Common.Extentions
{
    public class ProgressStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly Action<long> _onRead;
        private long _totalRead = 0;
        private long _totalWrite = 0;

        public ProgressStream(Stream innerStream, Action<long> onRead)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _onRead = onRead ?? throw new ArgumentNullException(nameof(onRead));
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => _innerStream.FlushAsync(cancellationToken);

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = _innerStream.Read(buffer, offset, count);
            if (bytesRead > 0)
            {
                _totalRead += bytesRead;
                _onRead(_totalRead);
            }
            return bytesRead;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var bytesRead = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
            if (bytesRead > 0)
            {
                _totalRead += bytesRead;
                _onRead(_totalRead);
            }
            return bytesRead;
        }
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count <= 0) return;

            _innerStream.Write(buffer, offset, count);

            _totalWrite += count; // ← именно count, а не buffer.Length!
            _onRead(_totalWrite);
        }
    }
}
