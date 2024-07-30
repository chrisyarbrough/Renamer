namespace Xarbrough.Renamer
{
	using System;

	/// <summary>
	/// A single instruction for the next operation in the sequence.
	/// </summary>
	public struct RenameInput
	{
		/// <summary>
		/// The string to rename. Guaranteed to never be null.
		/// </summary>
		public readonly string Text;

		/// <summary>
		/// The index within a sequence of the objects to be renamed.
		/// </summary>
		public readonly int ObjectIndex;

		/// <summary>
		/// The total number of objects when multiple objects are being renamed.
		/// </summary>
		public readonly int ObjectCount;

		public RenameInput(string text, int objectIndex, int objectCount)
		{
			Text = text ?? throw new ArgumentNullException(nameof(text));
			ObjectIndex = objectIndex;
			ObjectCount = objectCount;
		}
	}
}