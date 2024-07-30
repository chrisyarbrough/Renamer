namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;
	using System;

	[Serializable]
	internal class OperationsPanel
	{
		[SerializeField]
		private Vector2 scrollPosition;

		// When resetting operations, we need to remove focus for all text fields during the next GUI call.
		private bool resetWasCalled;

		private Renamer renamer;
		private SerializedProperty operationsProp;

		public void Initialize(Renamer renamer, SerializedProperty operationsProp)
		{
			this.renamer = renamer;
			this.operationsProp = operationsProp;
		}

		public void Draw(ViewMode viewMode)
		{
			DrawOperationsPanelHeader();

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(false));
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(5);

			EditorGUILayout.BeginVertical();
			DrawOperations(viewMode);
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();

			if (renamer.HasOperations)
				DrawVerticalSplitter();
		}

		private void DrawOperationsPanelHeader()
		{
			Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			var style = new GUIStyle(EditorStyles.miniLabel);
			style.normal.textColor = Color.gray;
			style.contentOffset -= Vector2.up;
			EditorGUILayout.LabelField("Operations", style);
			EditorGUILayout.EndHorizontal();

			rect.xMin = rect.xMax - 1;
			EditorGUI.DrawRect(rect, Styles.splitterColor * 0.27f);
		}

		private void DrawOperations(ViewMode viewMode)
		{
			if (resetWasCalled)
			{
				// Removing focus will only work during the GUI update.
				GUI.FocusControl(null);
				resetWasCalled = false;
			}

			foreach ((StringOperation operation, int id) in renamer.Operations)
			{
				if (!operation.DisplayInMode(viewMode))
					continue;

				if (operation == null)
				{
					EditorGUILayout.HelpBox("Failed to draw string operation.", MessageType.Error);
					continue;
				}

				if (id > 0)
					DrawVerticalSplitter();

				bool displayContent = DrawOperationHeader(
					operation,
					target: operationsProp.GetArrayElementAtIndex(id),
					activeField: operation.ActiveProp,
					id);

				Rect headerRect = GUILayoutUtility.GetLastRect();

				if (displayContent)
				{
					Rect start = GUILayoutUtility.GetLastRect();

					using (new EditorGUI.DisabledGroupScope(!operation.Active))
					{
						operation.Draw();
					}

					Rect colorRect = GUILayoutUtility.GetLastRect();
					colorRect.yMin = start.yMax;
					colorRect.width = 4;
					colorRect.x = 0;
					EditorGUI.DrawRect(colorRect, operation.DisplayColor);

					GUI.color = Color.white;
					headerRect.yMax = colorRect.yMax;
				}

				// Horizontal splitter on the right side.
				if (Event.current.type == EventType.Repaint)
				{
					headerRect.xMin = headerRect.xMax - 1;
					EditorGUI.DrawRect(headerRect, Styles.splitterColor);
				}
			}
		}

		private bool DrawOperationHeader(
			StringOperation operation,
			SerializedProperty target,
			SerializedProperty activeField,
			int targetID)
		{
			Rect backgroundRect = GUILayoutUtility.GetRect(1f, 21f);
			backgroundRect.width = Mathf.Min(backgroundRect.width, EditorGUIUtility.currentViewWidth - 6);
			backgroundRect.xMin += 4;
			float xMaxOriginal = backgroundRect.xMax;
			backgroundRect.xMax -= 8;

			Rect labelRect = backgroundRect;
			labelRect.xMin += 34f;
			labelRect.xMax -= 20f;
			labelRect.y -= 1;

			Rect foldoutRect = backgroundRect;
			foldoutRect.y += 3f;
			foldoutRect.width = 13f;
			foldoutRect.height = 13f;

			Rect toggleRect = backgroundRect;
			toggleRect.x += 16f;
			toggleRect.y += 3f;
			toggleRect.width = 13f;
			toggleRect.height = 13f;

			Texture2D menuIcon = Styles.paneOptionsIcon;
			Rect menuRect = new(labelRect.xMax + 4f, labelRect.y + 3f, menuIcon.width, menuIcon.height);

			// Background rect should be full-width
			backgroundRect.xMin = 0f;
			backgroundRect.xMax = xMaxOriginal;
			backgroundRect.width += 4f;

			// Background
			EditorGUI.DrawRect(backgroundRect, Styles.headerBackground);

			Rect colorRect = backgroundRect;
			colorRect.x = 0;
			colorRect.width = 4f;
			EditorGUI.DrawRect(colorRect, operation.DisplayColor);

			// Title
			using (new EditorGUI.DisabledScope(!activeField.boolValue))
			{
				EditorGUI.LabelField(labelRect, operation.DisplayContent, EditorStyles.boldLabel);
			}

			// Foldout
			target.serializedObject.Update();
			target.isExpanded = GUI.Toggle(foldoutRect, target.isExpanded, GUIContent.none, EditorStyles.foldout);
			target.serializedObject.ApplyModifiedProperties();

			// Active checkbox
			activeField.serializedObject.Update();
			activeField.boolValue = GUI.Toggle(toggleRect, activeField.boolValue, GUIContent.none, Styles.smallToggle);
			activeField.serializedObject.ApplyModifiedProperties();

			// Dropdown menu icon
			GUI.DrawTexture(menuRect, menuIcon);

			// Handle events
			Event e = Event.current;

			if (e.type == EventType.MouseDown)
			{
				if (menuRect.Contains(e.mousePosition))
				{
					ShowHeaderContextMenu(new Vector2(menuRect.x, menuRect.yMax), targetID);
					e.Use();
				}
				else if (labelRect.Contains(e.mousePosition))
				{
					if (e.button == 0)
						target.isExpanded = !target.isExpanded;
					else
						ShowHeaderContextMenu(e.mousePosition, targetID);

					e.Use();
				}
			}

			return target.isExpanded;
		}

		private void ShowHeaderContextMenu(Vector2 position, int targetID)
		{
			var menu = new GenericMenu();
			renamer.AddOperationHeaderMenu(menu, targetID, onResetCalled: () => resetWasCalled = true);
			menu.DropDown(new Rect(position, Vector2.zero));
		}

		private void DrawVerticalSplitter()
		{
			Rect rect = GUILayoutUtility.GetRect(1f, 1f);

			// Splitter rect should be full-width
			rect.xMin = 0f;
			rect.width += 4f;

			if (Event.current.type != EventType.Repaint)
				return;

			EditorGUI.DrawRect(rect, Styles.splitterColor);
		}
	}
}