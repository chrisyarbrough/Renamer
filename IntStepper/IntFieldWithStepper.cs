namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// An EditorGUI.IntField with increment/decrement buttons next to it.
	/// </summary>
	internal static class IntFieldWithStepper
	{
		private static readonly GUIContent plusIcon;
		private static readonly GUIContent minusIcon;
		private static GUIStyle style;

		static IntFieldWithStepper()
		{
			// Load from builtin resources.
			plusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
			minusIcon = EditorGUIUtility.IconContent("Toolbar Minus");

			// If Unity renames/removes these icons, fall back to text.
			if (plusIcon.image == null || minusIcon.image == null)
			{
				plusIcon.image = null;
				minusIcon.image = null;
				plusIcon.text = "+";
				minusIcon.text = "-";
			}
		}

		public static int DrawLayout(int value, string label)
		{
			Rect rect = EditorGUILayout.GetControlRect(hasLabel: !string.IsNullOrEmpty(label));
			return Draw(rect, value, label);
		}

		public static int Draw(Rect rect, int value, string label)
		{
			rect.xMax -= 34;
			value = EditorGUI.IntField(rect, label, value);

			rect.x = rect.xMax + 2;
			rect.width = 16;
			rect.yMin += 1;
			rect.yMax -= 1;

			style ??= new GUIStyle(EditorStyles.miniButton)
			{
				margin = new RectOffset(),
				padding = new RectOffset(),
			};

			if (RepeatButton.Draw(rect, plusIcon, style))
			{
				value++;
			}

			rect.x = rect.xMax;

			if (RepeatButton.Draw(rect, minusIcon, style))
			{
				value--;
			}
			return value;
		}
	}
}