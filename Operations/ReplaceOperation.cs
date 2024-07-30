namespace Xarbrough.Renamer
{
	using UnityEngine;

	internal class ReplaceOperation : StringOperation
	{
		[Tooltip("The characters to find in the existing name.")]
		[SerializeField]
		private string replace = string.Empty;

		[Tooltip("The characters to replace the found string with.")]
		[SerializeField]
		private string with = string.Empty;

		public override GUIContent DisplayContent => new GUIContent("Replace");
		public override Color DisplayColor => FromHTML("#0388A6EE");
		public override int DefaultOrder => 5;

		public override bool Rename(RenameInput input, out string output)
		{
			if (replace == string.Empty)
			{
				output = input.Text;
				return false;
			}

			output = input.Text.Replace(replace, with);
			return true;
		}
	}
}