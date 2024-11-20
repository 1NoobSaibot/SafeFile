using System.Security.Cryptography;

namespace SafeFile
{
	public class SignedBinaryReader : BinaryReader
	{
		private readonly ContentStream _wrapedStream;
		public override Stream BaseStream => _wrapedStream;


		public SignedBinaryReader(Stream input)
			: base(input)
		{
			ValidateSignature();
			base.BaseStream.Seek(0, SeekOrigin.Begin);
			_wrapedStream = new(input);
		}


		private void ValidateSignature()
		{
			if (base.BaseStream.Length < SHA256.HashSizeInBytes)
			{
				throw new FileCorruptedException();
			}

			base.BaseStream.Seek(-SHA256.HashSizeInBytes, SeekOrigin.End);
			var storedHash = ReadBytes(SHA256.HashSizeInBytes);

			base.BaseStream.Seek(0, SeekOrigin.Begin);
			var contentLength = base.BaseStream.Length - SHA256.HashSizeInBytes;
			var buffer = new byte[contentLength];
			base.BaseStream.Read(buffer, 0, buffer.Length);
			var computedHash = SHA256.HashData(buffer);

			if (AreEqual(storedHash, computedHash) == false)
			{
				throw new FileCorruptedException();
			}
		}


		public override int Read(byte[] buffer, int index, int count)
		{
			long contentLength = BaseStream.Length - SHA256.HashSizeInBytes;
			var remainLength = (int)(contentLength - BaseStream.Position);
			count = Math.Min(count, remainLength);
			return base.Read(buffer, index, count);
		}


		private static bool AreEqual(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}

			return true;
		}


		public static bool IsValid(string fileName)
		{
			try
			{
				using FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				using SignedBinaryReader sbr = new SignedBinaryReader(fs);
				return true;
			}
			catch (FileCorruptedException)
			{
				return false;
			}
		}


		private class ContentStream : Stream
		{
			private readonly Stream _baseStream;
			public override bool CanRead => true;
			public override bool CanSeek => false;
			public override bool CanWrite => false;
			public override long Length => _baseStream.Length - SHA256.HashSizeInBytes;

			public override long Position
			{
				get => _baseStream.Position;
				set => throw new InvalidOperationException();
			}


			public ContentStream(Stream stream)
			{
				_baseStream = stream;
			}


			public override void Flush() => _baseStream.Flush();

			public override int Read(byte[] buffer, int offset, int count)
			{
				count = (int)Math.Min(count, Length - Position);
				return _baseStream.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotImplementedException();
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}
		}
	}
}
