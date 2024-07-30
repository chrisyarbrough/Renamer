namespace Xarbrough.Renamer
{
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;

	internal class RegexReplaceOperation : StringOperation
	{
		[SerializeField]
		private string matchPattern = @"\ \(\d\)";

		[SerializeField]
		private string replacement = string.Empty;

		[SerializeField]
		private RegexOptions regexOptions = RegexOptions.Compiled;

		public override GUIContent DisplayContent => new GUIContent("Regex Replace");
		public override Color DisplayColor => FromHTML("#9FBF2CEE");
		public override int DefaultOrder => 6;
		protected override ViewMode ViewMode => ViewMode.Advanced;

		public override bool Rename(RenameInput input, out string output)
		{
			try
			{
				output = Regex.Replace(input.Text, matchPattern, replacement, regexOptions);
				return true;
			}
			catch
			{
				output = input.Text;
				return false;
			}
		}

		private SerializedProperty matchPatternProp;
		private SerializedProperty replacementProp;
		private SerializedProperty regexOptionsProp;

		protected override void OnEnable()
		{
			base.OnEnable();
			matchPatternProp = SerializedObject.FindProperty(nameof(matchPattern));
			replacementProp = SerializedObject.FindProperty(nameof(replacement));
			regexOptionsProp = SerializedObject.FindProperty(nameof(regexOptions));
		}

		protected override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(matchPatternProp);
			EditorGUILayout.PropertyField(replacementProp);
			RegexOptionsField();
		}

		private void RegexOptionsField()
		{
			EditorGUI.BeginChangeCheck();
			regexOptions = (RegexOptions)EditorGUILayout.EnumFlagsField(
				regexOptionsProp.displayName,
				regexOptions);
			if (EditorGUI.EndChangeCheck())
				regexOptionsProp.enumValueIndex = (int)regexOptions;
		}
	}
}