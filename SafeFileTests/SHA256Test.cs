using System.Security.Cryptography;

namespace SafeFileTests
{
	[TestClass]
	public class SHA256Test
	{
		[TestMethod]
		public void AlwaysReturnsTheSameHash()
		{
			byte[] testInput = [0];
			byte[] expectedHash = [
				110, 52, 11, 156, 255, 179, 122, 152,
				156, 165, 68, 230, 187, 120, 10, 44,
				120, 144, 29, 63, 179, 55, 56, 118,
				133, 17, 163, 6, 23, 175, 160, 29
			];

			byte[] actualHash = SHA256.HashData(testInput);

			AssertHashEquality(expectedHash, actualHash);
		}


		private static void AssertHashEquality(byte[] e, byte[] a)
		{
			Assert.AreEqual(e.Length, a.Length);

			foreach (int i in e.Length)
			{
				Assert.AreEqual(e[i], a[i]);
			}
		}
	}
}