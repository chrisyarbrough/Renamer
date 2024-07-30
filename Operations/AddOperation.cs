namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;

	internal class AddOperation : StringOperation
	{
		[Tooltip("The characters to add to the front of the name.")]
		[SerializeField]
		private string prefix = string.Empty;

		[Tooltip("The characters to add to the end of the name.")]
		[SerializeField]
		private string suffix = string.Empty;

		[Tooltip("The characters to insert at a specified position.")]
		[SerializeField]
		private string insert = string.Empty;

		[Tooltip("The position at which to insert the new characters.")]
		[SerializeField]
		private int atPosition;

		private SerializedProperty insertProp;

		private void OnValidate()
		{
			if (atPosition < 0)
				atPosition = 0;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			insertProp = FindProperty(nameof(insert));
		}

		public override GUIContent DisplayContent => new("Add", "Characters at the front, middle or end of the name.");
		public override Color DisplayColor => FromHTML("#445F7F");
		public override int DefaultOrder => 8;

		public override bool Rename(RenameInput input, out string output)
		{
			output = prefix + ApplyInsert(input.Text) + suffix;
			return true;
		}

		private string ApplyInsert(string input)
		{
			if (insertProp.isExpanded == false)
				return input;

			int position = Mathf.Clamp(atPosition, 0, input.Length);
			return input.Insert(position, insert);
		}

		protected override void OnInspectorGUI()
		{
			DrawProperty(nameof(prefix));
			DrawProperty(nameof(suffix));

			insertProp.isExpanded = EditorGUILayout.Toggle("Insert", insertProp.isExpanded);
			if (insertProp.isExpanded)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(insertProp, new GUIContent("Characters"));
				DrawProperty(nameof(atPosition));
				EditorGUI.indentLevel--;
			}
		}
	}
}