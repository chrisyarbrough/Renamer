namespace Xarbrough.Renamer
{
	using NUnit.Framework;

	public class DiffTests
	{
		[Test]
		public void LongestCommonSubstring_SanityCheck()
		{
			int[,] result = Diff.LongestCommonSubstring("abc", "abc");
			Assert.AreEqual(3, result[3, 3]);
		}

		[Test]
		public void LongestCommonSubstring_EmptyStrings()
		{
			int[,] result = Diff.LongestCommonSubstring("", "");
			Assert.AreEqual(0, result[0, 0]);
		}

		[Test]
		public void LongestCommonSubstring_NoCommonSubstring()
		{
			int[,] result = Diff.LongestCommonSubstring("abc", "xyz");
			Assert.AreEqual(0, result[3, 3]);
		}
	}
}