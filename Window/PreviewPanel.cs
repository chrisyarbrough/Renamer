namespace Xarbrough.Renamer
{
	using System;
	using System.Text;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	[Serializable]
	internal class PreviewPanel
	{
		public Pref<bool> showDiff;

		private Vector2 scroll;
		private Styles styles;

		public void OnEnable()
		{
			showDiff = new BoolPref("ShowDiff", false);
		}

		public void InitializeStyles()
		{
			styles ??= new Styles();
		}

		private class Styles
		{
			public readonly GUIStyle headerLabel;
			public readonly GUIStyle previewLabel = EditorStyles.boldLabel;

			public readonly GUIStyle previewLabelDiff = new GUIStyle(EditorStyles.boldLabel)
			{
				richText = true,
			};

			public Styles()
			{
				headerLabel = new GUIStyle(EditorStyles.miniLabel);
				headerLabel.normal.textColor = Color.gray;
				headerLabel.contentOffset -= Vector2.up;
			}
		}

		public void OnGUI(Object[] targets, Renamer renamer)
		{
			InitializeStyles();
			DrawPreviewHeader();

			if (targets == null || targets.Length == 0)
			{
				EditorGUILayout.HelpBox("Select GameObjects from the Hierarchy or assets " +
				                        "in the Project window to rename them.", MessageType.Info);
				return;
			}

			scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(false));
			for (int i = 0; i < targets.Length; i++)
			{
				Object target = targets[i];
				string original = target != null ? target.name : string.Empty;
				string preview = renamer.CalculateRename(original, i, targets.Length);
				bool oddRow = targets.Length > 3 && i % 2 != 0;
				DrawPreview(target, original, preview, oddRow);
			}
			EditorGUILayout.EndScrollView();
		}

		private void DrawPreviewHeader()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			EditorGUILayout.LabelField("Preview", styles.headerLabel, GUILayout.Width(50));

			EditorGUILayout.Space();
			showDiff.Value = GUILayout.Toggle(showDiff, "Show Diff", EditorStyles.toolbarButton, GUILayout.Width(70));

			EditorGUILayout.EndHorizontal();
		}

		private void DrawPreview(Object target, string original, string preview, bool oddRow)
		{
			// For some reason, broken prefabs don't report the prefab icon
			// and also we need to apply the red text style manually.
			Texture2D icon;
			GUIStyle style = new(EditorStyles.label);
			if (target != null && PrefabUtility.IsPrefabAssetMissing(target))
			{
				icon = (Texture2D)EditorGUIUtility.Load("d_Prefab Icon");
				style = "PR BrokenPrefabLabel";
			}
			else
			{
				icon = AssetPreview.GetMiniThumbnail(target);
			}

			Rect rect = EditorGUILayout.GetControlRect();

			Rect background = rect;
			background.yMin -= 2;
			background.xMin = 0;
			background.xMax = EditorGUIUtility.currentViewWidth;
			Color color = Color.black * 0.15f;

			if (oddRow)
				color = Color.black * 0.05f;

			EditorGUI.DrawRect(background, color);

			DrawRow(rect, original, preview, icon, style);
		}

		private void DrawRow(Rect rect, string original, string preview, Texture2D icon, GUIStyle nameStyle)
		{
			rect.width /= 2f;
			rect.y -= 2;

			var s = new GUIStyle(EditorStyles.label);
			s.fixedHeight = 18;
			s.normal.textColor = nameStyle.normal.textColor;

			Rect iconRect = rect;
			iconRect.width = 20;
			EditorGUI.LabelField(iconRect, new GUIContent(icon), s);

			rect.x = iconRect.xMax;
			rect.width -= iconRect.width;

			string originalDiffView = original;
			if (showDiff)
			{
				s = new GUIStyle(EditorStyles.label);
				s.fontStyle = FontStyle.Normal;
				s.richText = true;
				var c = Diff.LongestCommonSubstring(original, preview);
				var sb = new StringBuilder(preview.Length * 23);
				// The old unchanged text with diff removals.
				Diff.CreateRichOriginalDiffString(sb, c, original, preview, original.Length, preview.Length);
				originalDiffView = sb.ToString();
			}

			EditorGUI.LabelField(rect, new GUIContent(originalDiffView), s);

			rect.x = rect.xMax;

			GUIStyle style;
			if (showDiff)
			{
				style = styles.previewLabelDiff;

				// The new changed text with diff additions.
				preview = Diff.GetRichDiffString(original, preview);
			}
			else
			{
				style = styles.previewLabel;
			}

			EditorGUI.LabelField(rect, preview, style);
		}

		private static bool CaseInsensitiveCompare(char a, char b)
		{
			return char.ToUpperInvariant(a) == char.ToUpperInvariant(b);
		}
	}
}