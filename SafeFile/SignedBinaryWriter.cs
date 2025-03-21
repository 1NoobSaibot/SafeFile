using System.Security.Cryptography;

namespace SafeFile
{
	public class SignedBinaryWriter : BinaryWriter
	{
		private bool _wasSigned = false;


		public SignedBinaryWriter(Stream output)
			: base(output)
		{ }


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Sign();
			}
			base.Dispose(disposing);
		}


		public override ValueTask DisposeAsync()
		{
			GC.SuppressFinalize(this);
			Sign();
			return base.DisposeAsync();
		}


		private void Sign()
		{
			if (_wasSigned)
			{
				throw new InvalidOperationException("Must not sign second time");
			}

			BaseStream.Seek(0, SeekOrigin.Begin);
			var contentLength = BaseStream.Length;
			var buffer = new byte[contentLength];
			BaseStream.ReadExactly(buffer);
			var hash = SHA256.HashData(buffer);

			BaseStream.Seek(0, SeekOrigin.End);
			Write(hash);

			_wasSigned = true;
		}
	}
}
