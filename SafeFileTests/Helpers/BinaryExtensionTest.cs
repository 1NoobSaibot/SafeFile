using SafeFile.Helpers;

namespace SafeFileTests.Helpers
{
	[TestClass]
	public class BinaryExtensionTest
	{
		[TestMethod]
		public void ReadingNullablePrimitivesWorksWell()
		{
			int?[] samples = [null, 7];

			foreach (int? expected in samples)
			{
				int? actual = TestWriteAndReadTheSame(
					expected,
					(w, i) => w.WriteNullable(i),
					(r) => r.ReadNullableInt32()
				);

				Assert.AreEqual(expected, actual);
			}
		}


		[TestMethod]
		public void ReadingNullableStructsWorksWell()
		{
			TestStruct?[] samples = [null, new(3, 7)];

			foreach (TestStruct? expected in samples)
			{
				TestStruct? actual = TestWriteAndReadTheSame(
					expected,
					(w, nv) =>
					{
						w.WriteNullableStruct<TestStruct>(nv, (w, v) =>
						{
							w.Write(v!.Value1);
							w.Write(v!.Value2);
						});
					},
					(r) => r.ReadNullableStruct<TestStruct>(
						(r) =>
						{
							int v1 = r.ReadInt32();
							int v2 = r.ReadInt32();
							return new(v1, v2);
						}
					)
				);

				AssertTestStructs(expected, actual);
			}
		}


		[TestMethod]
		public void ReadingNullableClassesWorksWell()
		{
			TestClass?[] samples = [null, new(3, 7)];

			foreach (TestClass? expected in samples)
			{
				TestClass? actual = TestWriteAndReadTheSame(
					expected,
					(w, nv) =>
					{
						w.WriteNullable(nv, (w, v) =>
						{
							w.Write(v.Value1);
							w.Write(v.Value2);
						});
					},
					(r) => r.ReadNullable<TestClass>(
						(r) =>
						{
							int v1 = r.ReadInt32();
							int v2 = r.ReadInt32();
							return new(v1, v2);
						}
					)
				);

				AssertTestClasses(expected, actual);
			}
		}


		private static T TestWriteAndReadTheSame<T>(
			T inputValue,
			Action<BinaryWriter, T> writeFn,
			Func<BinaryReader, T> readFn
		)
		{
			var data = Write(w => writeFn(w, inputValue));
			return Read(data, readFn);
		}


		private static byte[] Write(Action<BinaryWriter> writeFn)
		{
			using var stream = new MemoryStream();
			using BinaryWriter writer = new(stream);
			writeFn(writer);
			return stream.ToArray();
		}


		private static T Read<T>(byte[] data, Func<BinaryReader, T> readFn)
		{
			using var readStream = new MemoryStream(data);
			using BinaryReader reader = new(readStream);
			return readFn(reader);
		}


		private static void AssertTestClasses(TestClass? expected, TestClass? actual)
		{
			if (expected is null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.Value1, actual.Value1);
			Assert.AreEqual(expected.Value2, actual.Value2);
		}


		private static void AssertTestStructs(TestStruct? expected, TestStruct? actual)
		{
			if (expected is null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.Value.Value1, actual.Value.Value1);
			Assert.AreEqual(expected.Value.Value2, actual.Value.Value2);
		}


		private class TestClass
		{
			public readonly int Value1;
			public readonly int Value2;


			public TestClass(int value1, int value2)
			{
				Value1 = value1;
				Value2 = value2;
			}
		}


		private readonly struct TestStruct
		{
			public readonly int Value1;
			public readonly int Value2;


			public TestStruct(int value1, int value2)
			{
				Value1 = value1;
				Value2 = value2;
			}
		}
	}
}
