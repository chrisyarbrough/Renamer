namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Object = UnityEngine.Object;
	using System;
	using System.Linq;

	internal class RenameWindow : EditorWindow, IHasCustomMenu
	{
		[MenuItem("Xarbrough/Renamer")]
		public static void ShowWindow()
		{
			GetWindow<RenameWindow>();
		}

		[SerializeField]
		private WindowIcon windowIcon;

		[SerializeField]
		private Renamer renamer = new();

		[SerializeField]
		private OperationsPanel operationsPanel = new();

		private Object[] targets;

		private readonly WindowLayout windowLayout = new();

		private EnumPref<ViewMode> viewMode = new("ViewMode", ViewMode.Default);

		private PreviewPanel previewTable;

		private void OnEnable()
		{
			if (windowIcon != null)
				windowIcon.Apply(this);

			windowLayout.Load();

			previewTable ??= new PreviewPanel();
			previewTable.OnEnable();

			viewMode = new("ViewMode", ViewMode.Default);

			renamer.Initialize();
			operationsPanel.Initialize(renamer, new SerializedObject(this).FindProperty("renamer.operations"));

			RefreshTargets();

			EditorSceneManager.sceneOpened += OnSceneOpened;
		}

		private void OnDisable()
		{
			EditorSceneManager.sceneOpened -= OnSceneOpened;
		}

		private void OnSceneOpened(Scene scene, OpenSceneMode mode) => RefreshTargets();

		private void OnDestroy() => renamer.Clear();

		private void OnSelectionChange() => RefreshTargets();

		private void OnHierarchyChange() => Repaint();

		private void RefreshTargets()
		{
			targets = Selection.objects;

			if (targets.FirstOrDefault() is GameObject)
				Array.Sort(targets, HierarchySorting.Sort);

			Repaint();
		}

		public void OnGUI()
		{
			windowLayout.SplitRect(position, out Rect operationsPanelRect, out Rect previewPanelRect);

			GUILayout.BeginArea(operationsPanelRect);
			operationsPanel.Draw(viewMode.Value);
			GUILayout.EndArea();

			GUILayout.BeginArea(previewPanelRect);
			DrawPreviewPanel();
			GUILayout.EndArea();
		}

		private void DrawPreviewPanel()
		{
			previewTable.OnGUI(targets, renamer);
			DrawRenameButton();
			EditorGUILayout.Space();
		}

		private void DrawRenameButton()
		{
			EditorGUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
			EditorGUILayout.Space();
			GUI.enabled = targets is { Length: > 0 };
			if (GUILayout.Button("Rename", GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f)))
			{
				ApplyRenameOperationsToTargets();
			}

			GUI.enabled = true;
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
		}

		private void ApplyRenameOperationsToTargets()
		{
			for (int i = 0; i < targets.Length; i++)
			{
				Object target = targets[i];
				Undo.RecordObject(target, "Perform Rename");

				string newName = renamer.CalculateRename(target.name, i, targets.Length);

				if (EditorUtility.IsPersistent(target))
				{
					string assetPath = AssetDatabase.GetAssetPath(target);
					AssetDatabase.RenameAsset(assetPath, newName);
				}
				else
				{
					target.name = newName;
				}
			}
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			windowLayout.AddWindowOptions(menu);
			menu.AddItem(new GUIContent("Show Advanced Operations"), viewMode.Value == ViewMode.Advanced, ToggleViewMode);
			renamer.AddWindowOptions(menu);
		}

		private void ToggleViewMode()
		{
			viewMode.Value = viewMode.Value == ViewMode.Default ? ViewMode.Advanced : ViewMode.Default;
			foreach ((StringOperation operation, int _) in renamer.Operations)
			{
				if (operation.DisplayInMode(viewMode) == false)
					operation.Active = false;
			}
		}
	}
}