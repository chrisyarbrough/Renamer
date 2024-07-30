namespace Xarbrough.Renamer
{
	using UnityEngine;

	/// <summary>
	/// Adds a sequence of numbers to the string.
	/// </summary>
	/// <example>
	///	MyString_01, MyString_02, 
	/// </example>
	internal class NumberingOperation : StringOperation
	{
		[Tooltip("Pick where to add the numbers, e.g. the start or end of the name.")]
		[SerializeField]
		private Position position = Position.Suffix;

		[Tooltip("The total number of zero-padded digits; e.g. a value of two will result in numbers 00, 01, 02.")]
		[SerializeField]
		private int padding = 2;

		[Tooltip("The character to insert between the existing name and the added number.")]
		[SerializeField]
		private string delimiter = "_";

		[Tooltip("Whether the numbering should be in standard ascending order or reverse.")]
		[SerializeField]
		private SequenceType sequenceType;

		[Tooltip("The number to start counting at.")]
		[SerializeField]
		private int startNumber;

		private void OnValidate()
		{
			if (padding < 1)
				padding = 1;

			if (startNumber < 0)
				startNumber = 0;
		}

		public override GUIContent DisplayContent
			=> new GUIContent("Numbering", "Add consecutive numbers to the list of objects.");

		public override Color DisplayColor => FromHTML("#488C03");
		public override int DefaultOrder => 10;

		public override bool Rename(RenameInput input, out string output)
		{
			if (padding < 0)
			{
				output = input.Text;
				return false;
			}

			int index = IndexToSequenceNumber(input.ObjectIndex, input.ObjectCount);

			switch (position)
			{
				case Position.Prefix:
					output = ApplyPadding(index) + delimiter + input.Text;
					return true;

				case Position.Suffix:
					output = input.Text + delimiter + ApplyPadding(index);
					return true;

				default:
					output = input.Text;
					return false;
			}
		}

		/// <summary>
		/// Converts the linear index to the number within the output sequence.
		/// </summary>
		private int IndexToSequenceNumber(int index, int objectCount)
		{
			return sequenceType switch
			{
				// The selection index will always start counting at zero.
				// To start enumerating the display names, we can simply apply an offset.
				SequenceType.Ascending => startNumber + index,
				SequenceType.Descending => objectCount - 1 + startNumber - index,
				_ => throw new System.NotImplementedException($"Unknown sequence type '{sequenceType}'."),
			};
		}

		private string ApplyPadding(int number)
		{
			return number.ToString().PadLeft(padding, '0');
		}

		private enum Position
		{
			Prefix,
			Suffix,
		}

		private enum SequenceType
		{
			Ascending,
			Descending,
		}
	}
}