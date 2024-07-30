using System.Text;

namespace Xarbrough.Renamer
{
	internal static class Diff
	{
		public static int[,] LongestCommonSubstring(string s1, string s2)
		{
			// Stores the length of the common substring at different points in the strings.
			int[,] c = new int[s1.Length + 1, s2.Length + 1];

			for (int i = 1; i <= s1.Length; i++)
			for (int j = 1; j <= s2.Length; j++)
			{
				if (s1[i - 1] == s2[j - 1])
					c[i, j] = c[i - 1, j - 1] + 1; // Extend common streak.
				else
					c[i, j] = c[i - 1, j] > c[i, j - 1] ? c[i - 1, j] : c[i, j - 1];
			}
			return c;
		}

		public static string GetRichDiffString(string original, string preview)
		{
			var c = LongestCommonSubstring(original, preview);
			var sb = new StringBuilder(preview.Length * 23);
			CreateRichPreviewDiffString(sb, c, original, preview, original.Length, preview.Length);
			return sb.ToString();
		}

		private static void CreateRichPreviewDiffString(StringBuilder diff,
			int[,] c,
			string s1,
			string s2,
			int i,
			int j)
		{
			if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
			{
				// The same
				CreateRichPreviewDiffString(diff, c, s1, s2, i - 1, j - 1);
				diff.Append(s1[i - 1]);
			}
			else if (j > 0 && (i == 0 || (c[i, j - 1] > c[i - 1, j])))
			{
				// Added
				CreateRichPreviewDiffString(diff, c, s1, s2, i, j - 1);
				RichDiff_Add(diff, s2[j - 1]);
			}
			else if (i > 0 && (j == 0 || (c[i, j - 1] <= c[i - 1, j])))
			{
				// Removed (ignore)
				CreateRichPreviewDiffString(diff, c, s1, s2, i - 1, j);
			}
		}

		public static void CreateRichOriginalDiffString(StringBuilder diff,
			int[,] c,
			string s1,
			string s2,
			int i,
			int j)
		{
			if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
			{
				// The same
				CreateRichOriginalDiffString(diff, c, s1, s2, i - 1, j - 1);
				diff.Append(s1[i - 1]);
			}
			else if (j > 0 && (i == 0 || (c[i, j - 1] > c[i - 1, j])))
			{
				// Added
				CreateRichOriginalDiffString(diff, c, s1, s2, i, j - 1);
			}
			else if (i > 0 && (j == 0 || (c[i, j - 1] <= c[i - 1, j])))
			{
				// Removed (ignore)
				CreateRichOriginalDiffString(diff, c, s1, s2, i - 1, j);
				RichDiff_Remove(diff, s1[i - 1]);
			}
		}

		public static void RichDiff_Add(StringBuilder sb, char s)
		{
			// Green
			sb.Append("<color=#22AA66>").Append(s).Append("</color>");
		}

		public static void RichDiff_Remove(StringBuilder sb, char s)
		{
			// Red
			sb.Append("<color=#CC3344>").Append(s).Append("</color>");
		}
	}
}