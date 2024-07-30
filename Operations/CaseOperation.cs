namespace Xarbrough.Renamer
{
	using System.Globalization;
	using UnityEngine;

	internal class CaseOperation : StringOperation
	{
		[SerializeField]
		private Case @case = Case.TitleCase;

		public override GUIContent DisplayContent =>
			new("Case", "Change the case of the name from, e.g. lower to uppercase.");

		public override Color DisplayColor => FromHTML("#2328A6BE");
		public override int DefaultOrder => 5;
		protected override ViewMode ViewMode => ViewMode.Advanced;

		public override bool Rename(RenameInput input, out string output)
		{
			switch (@case)
			{
				case Case.ToLower:
					output = input.Text.ToLower();
					return true;

				case Case.ToUpper:
					output = input.Text.ToUpper();
					return true;

				case Case.TitleCase:
					output = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Text);
					return true;

				default:
					output = input.Text;
					return false;
			}
		}

		public enum Case
		{
			ToLower,
			ToUpper,
			TitleCase,
		}
	}
}