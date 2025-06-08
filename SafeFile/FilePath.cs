namespace SafeFile
{
	public readonly struct FilePath
	{
		public readonly string Value;


		public FilePath(string value)
		{
			Value = value;
		}


		public static FilePath operator /(FilePath left, FilePath right)
		{
			return new(Path.Combine(left.Value, right.Value));
		}


		public static implicit operator FilePath(string value)
		{
			return new(value);
		}


		public static implicit operator string(FilePath left)
		{
			return left.Value;
		}
	}
}
