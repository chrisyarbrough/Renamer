namespace Xarbrough.Renamer
{
	using UnityEngine;

	/// <summary>
	/// A button which returns true in repeated intervals when held down.
	/// </summary>
	internal static class RepeatButton
	{
		private static float nextRepeatTime;

		public static bool Draw(Rect rect, GUIContent content, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			switch (Event.current.GetTypeForControl(controlID))
			{
				case EventType.MouseDown:
					return OnMouseDown(rect, controlID);

				case EventType.Repaint:
					return OnRepaint(rect, content, style, controlID);

				case EventType.MouseUp:
					OnMouseUp(controlID);
					break;
			}
			return false;
		}

		private static bool OnMouseDown(Rect rect, int controlID)
		{
			if (GUIUtility.hotControl == 0 && rect.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlID;
				Event.current.Use();

				// When number fields are focused and currently editing text
				// they won't update the displayed value until unfocused.
				GUI.FocusControl(null);

				// The first input is instant and has a slightly longer delay than later repetitions.
				nextRepeatTime = Time.realtimeSinceStartup + 0.5f;
				return true;
			}
			return false;
		}

		private static bool OnRepaint(Rect rect, GUIContent content, GUIStyle style, int controlID)
		{
			style.Draw(rect, content, controlID);

			if (GUIUtility.hotControl == controlID)
			{
				// Trigger a repaint event to keep updating the loop while the mouse is held down.
				GUI.changed = true;

				if (Time.realtimeSinceStartup >= nextRepeatTime)
				{
					nextRepeatTime = Time.realtimeSinceStartup + 0.1f;
					return true;
				}
			}
			return false;
		}

		private static void OnMouseUp(int controlID)
		{
			if (GUIUtility.hotControl == controlID)
			{
				GUIUtility.hotControl = 0;
			}
		}
	}
}