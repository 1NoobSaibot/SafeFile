using System.Security.Cryptography;

namespace SafeFile
{
	public class SignedBinaryReader : BinaryReader
	{
		public SignedBinaryReader(Stream input)
			: base(input)
		{
			ValidateSignature();
			BaseStream.Seek(0, SeekOrigin.Begin);
		}


		private void ValidateSignature()
		{
			if (BaseStream.Length < SHA256.HashSizeInBytes)
			{
				throw new FileCorruptedException();
			}

			BaseStream.Seek(-SHA256.HashSizeInBytes, SeekOrigin.End);
			var storedHash = ReadBytes(SHA256.HashSizeInBytes);

			BaseStream.Seek(0, SeekOrigin.Begin);
			var contentLength = BaseStream.Length - SHA256.HashSizeInBytes;
			var buffer = new byte[contentLength];
			BaseStream.Read(buffer, 0, buffer.Length);
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
	}
}
