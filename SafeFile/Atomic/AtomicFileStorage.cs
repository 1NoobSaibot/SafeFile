namespace SafeFile.Atomic
{
	public class AtomicFileStorage
	{
		protected virtual string TempExtension => "temp";
		public readonly string FileName;
		public readonly string TempFileName;


		public AtomicFileStorage(string fileName)
		{
			FileName = fileName;
			TempFileName = GetTempFileName();
			TryFinishTransaction();
		}


		public bool Exists()
		{
			return SignedBinaryReader.ExistsAndIsValid(FileName);
		}


		private void TryFinishTransaction()
		{
			if (File.Exists(TempFileName))
			{
				if (SignedBinaryReader.IsValid(TempFileName))
				{
					File.Delete(FileName);
					File.Move(TempFileName, FileName);
				}
				File.Delete(TempFileName);
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
			using (FileStream fs = new(TempFileName, FileMode.Create, FileAccess.ReadWrite))
			{
				using SignedBinaryWriter writer = new(fs);
				writeFn(writer);
			}

			if (File.Exists(FileName))
			{
				File.Delete(FileName);
			}

			File.Move(TempFileName, FileName);
			File.Delete(TempFileName);
		}


		private string GetTempFileName()
		{
			return FileName + "." + TempExtension;
		}
	}
}
