using SafeFile.Atomic;

namespace Tests.Atomic
{
	[TestClass]
	public class AtomicFileTest
	{
		const string FILE_NAME = $"{nameof(AtomicFileTest)}.bin";


		[TestMethod]
		public void WorksWell()
		{
			double randomValue = Random.Shared.NextDouble();
			AtomicFileStorage file = new(FILE_NAME);

			file.WriteAndSave((writer) =>
			{
				writer.Write(randomValue);
			});

			double readValue = -randomValue;
			bool wasRead = file.TryRead((reader) =>
			{
				readValue = reader.ReadDouble();
			});

			Assert.IsTrue(wasRead);
			Assert.AreEqual(randomValue, readValue);
		}
	}
}
