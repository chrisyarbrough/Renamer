namespace Xarbrough.Renamer
{
	using UnityEngine;

	internal class BaseNameOperation : StringOperation
	{
		[Tooltip("The previous name will be entirely overriden by this new name.")]
		[SerializeField]
		private string newName = string.Empty;

		public override GUIContent DisplayContent => new GUIContent("Base Name", "Set a new name to start from.");
		public override Color DisplayColor => FromHTML("#0266B6");
		public override int DefaultOrder => 0;

		public override bool Rename(RenameInput _, out string output)
		{
			output = newName;
			return true;
		}
	}
}