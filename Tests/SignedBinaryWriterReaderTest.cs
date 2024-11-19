using SafeFile;

namespace Tests
{
	[TestClass]
	public class SignedBinaryWriterReaderTest
	{
		private const string FILE_NAME = "test_file.bin";


		[TestMethod]
		public void WritesAndReadsWell()
		{
			Write(double.Pi);
			Assert.AreEqual(double.Pi, Read());
		}


		private static double Read()
		{
			using FileStream fileStream = File.Open(FILE_NAME, FileMode.Open, FileAccess.Read);
			using SignedBinaryReader reader = new(fileStream);
			double actualValue = reader.ReadDouble();
			return actualValue;
		}


		private static void Write(double value)
		{
			if (File.Exists(FILE_NAME))
			{
				File.Delete(FILE_NAME);
			}

			using FileStream fileStream = File.Open(FILE_NAME, FileMode.Create);
			using SignedBinaryWriter writer = new(fileStream);
			writer.Write(value);
		}
	}
}
