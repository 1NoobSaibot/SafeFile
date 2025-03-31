namespace SafeFile.Atomic
{
	public class AtomicFileStorage
	{
		protected virtual string TempExtension => "temp";
		public readonly string FileName;


		public AtomicFileStorage(string fileName)
		{
			FileName = fileName;
			TryFinishTransaction();
		}


		private void TryFinishTransaction()
		{
			string tempFileName = TempFileName();
			if (File.Exists(tempFileName))
			{
				if (SignedBinaryReader.IsValid(tempFileName))
				{
					File.Delete(FileName);
					File.Move(tempFileName, FileName);
				}
				File.Delete(tempFileName);
			}
		}


		public bool TryRead(Action<SignedBinaryReader> action)
		{
			if (File.Exists(FileName))
			{
				using FileStream fs = new(FileName, FileMode.Open, FileAccess.Read);
				using SignedBinaryReader sbr = new(fs);
				action(sbr);
				return true;
			}
			return false;
		}


		public bool TryRead<T>(Func<SignedBinaryReader, T> readFn, out T? result)
		{
			if (File.Exists(FileName))
			{
				using FileStream fs = new(FileName, FileMode.Open, FileAccess.Read);
				using SignedBinaryReader sbr = new(fs);
				result = readFn(sbr);
				return true;
			}
			result = default;
			return false;
		}


		public void WriteAndSave(Action<SignedBinaryWriter> writeFn)
		{
			string tempFileName = TempFileName();
			using (FileStream fs = new(tempFileName, FileMode.Create, FileAccess.ReadWrite))
			{
				using SignedBinaryWriter writer = new(fs);
				writeFn(writer);
			}

			if (File.Exists(FileName))
			{
				File.Delete(FileName);
			}

			File.Move(tempFileName, FileName);
			File.Delete(tempFileName);
		}


		private string TempFileName()
		{
			return FileName + "." + TempExtension;
		}
	}
}
