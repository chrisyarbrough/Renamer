namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;

	internal class RemoveOperation : StringOperation
	{
		[Tooltip("Remove the first n characters.")]
		[SerializeField]
		private int firstN;

		[Tooltip("Remove the last n characters.")]
		[SerializeField]
		private int lastN = 4;

		[Tooltip("Remove characters starting from index.")]
		[SerializeField]
		private int from;

		[Tooltip("Remove characters until index.")]
		[SerializeField]
		private int to;

		[Tooltip("Removes any whitespace at the start and end of the name.")]
		[SerializeField]
		private bool trim = true;

		private void OnValidate()
		{
			if (firstN < 0)
				firstN = 0;

			if (lastN < 0)
				lastN = 0;

			if (from < 0)
				from = 0;

			if (to < 0)
				to = 0;
		}

		public override GUIContent DisplayContent => new GUIContent("Remove", "Remove a fixed number of characters.");
		public override Color DisplayColor => FromHTML("#F5CA72AA");
		public override int DefaultOrder => 1;

		public override bool Rename(RenameInput input, out string output)
		{
			string text = input.Text;

			if (trim)
				text = text.Trim();

			int firstNClamped = Mathf.Min(firstN, text.Length);
			int lastNClamped = Mathf.Min(lastN, text.Length);

			int length = Mathf.Clamp(text.Length - lastNClamped - firstNClamped, 0, text.Length);
			output = text.Substring(firstNClamped, length);

			if (output.Length > 0 && from < output.Length)
			{
				int toClamped = Mathf.Min(to, output.Length);
				int count = Mathf.Clamp(toClamped - from, 0, output.Length);
				output = output.Remove(from, count);
			}

			return true;
		}

		protected override void OnInspectorGUI()
		{
			float originalWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 48;

			GUILayout.Space(2);

			Rect rect = EditorGUILayout.BeginHorizontal();
			rect.xMin -= 5;
			rect.xMax += 4;
			rect.yMin -= 3;
			rect.yMax += 3;
			GUI.Box(rect, GUIContent.none);
			DrawIntFieldWithStepper(SerializedObject.FindProperty(nameof(firstN)));

			GUILayout.Space(20);
			DrawIntFieldWithStepper(SerializedObject.FindProperty((nameof(lastN))));
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(7);

			Rect rect2 = EditorGUILayout.BeginHorizontal();
			rect.y = rect2.y - 3;
			GUI.Box(rect, GUIContent.none);
			EditorGUI.BeginChangeCheck();
			DrawIntFieldWithStepper(SerializedObject.FindProperty(nameof(from)));
			if (EditorGUI.EndChangeCheck())
			{
				if (from >= to)
					to = from + 1;
			}
			GUILayout.Space(20);
			EditorGUI.BeginChangeCheck();
			DrawIntFieldWithStepper(SerializedObject.FindProperty(nameof(to)));
			if (EditorGUI.EndChangeCheck())
			{
				if (to <= from)
					from = Mathf.Max(0, to - 1);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = originalWidth;
		}

		private void DrawIntFieldWithStepper(SerializedProperty property)
		{
			property.intValue = IntFieldWithStepper.DrawLayout(property.intValue, property.displayName);
		}
	}
}