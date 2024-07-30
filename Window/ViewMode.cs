namespace Xarbrough.Renamer
{
	[System.Flags]
	public enum ViewMode
	{
		Default = 1,
		Advanced = 2,
	}

	internal static class ViewModeExtensions
	{
		public static bool HasFlagCustom(this ViewMode value, ViewMode flag)
		{
			// Performance optimization/simplification for Enum.HasFlag.
			return (value & flag) == flag;
		}
	}
}