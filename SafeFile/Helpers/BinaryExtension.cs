using System.Runtime.CompilerServices;

namespace SafeFile.Helpers
{
	public static class BinaryExtension
	{
		public static T[] ReadArray<T>(
			this BinaryReader reader,
			Func<BinaryReader, T> readItemFunc
		)
		{
			int length = reader.ReadInt32();
			T[] res = new T[length];
			for (int i = 0; i < length; i++)
			{
				res[i] = readItemFunc(reader);
			}
			return res;
		}


		public static BinaryWriter Write<T>(
			this BinaryWriter writer,
			T[] array,
			Action<BinaryWriter, T> writeItemAction
		)
		{
			writer.Write(array.Length);
			foreach (T item in array)
			{
				writeItemAction(writer, item);
			}
			return writer;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string[] ReadStringArray(this BinaryReader reader)
		{
			return reader.ReadArray<string>((reader) => reader.ReadString());
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryWriter Write(this BinaryWriter writer, string[] strings)
		{
			return writer.Write(strings, (writer, str) => writer.Write(str));
		}


		public static void WriteNullable<T>(
			this BinaryWriter writer,
			T? nullableValue,
			Action<BinaryWriter, T> writeNotNull
		)
			where T : class
		{
			if (nullableValue is T notNullValue)
			{
				writer.Write(true);
				writeNotNull(writer, notNullValue);
				return;
			}

			writer.Write(false);
		}


		public static void WriteNullableStruct<TStruct>(
			this BinaryWriter writer,
			Nullable<TStruct> nullableValue,
			Action<BinaryWriter, TStruct> writeNotNull
		)
			where TStruct : struct
		{
			if (nullableValue.HasValue)
			{
				writer.Write(true);
				writeNotNull(writer, nullableValue.Value);
				return;
			}

			writer.Write(false);
		}


		public static T? ReadNullable<T>(
			this BinaryReader reader,
			Func<BinaryReader, T> readAsNotNull
		)
			where T : class
		{
			if (reader.ReadBoolean())
			{
				return readAsNotNull(reader);
			}

			return null;
		}


		public static Nullable<T> ReadNullableStruct<T>(
			this BinaryReader reader,
			Func<BinaryReader, T> readAsNotNull
		)
			where T : struct
		{
			if (reader.ReadBoolean())
			{
				return readAsNotNull(reader);
			}

			return null;
		}


		public static void WriteNullable(this BinaryWriter writer, int? nullableInt)
		{
			writer.WriteNullableStruct<int>(nullableInt, (w, v) => writer.Write(v));
		}


		public static int? ReadNullableInt32(this BinaryReader reader)
		{
			return reader.ReadNullableStruct<int>((r) => reader.ReadInt32());
		}
	}
}
