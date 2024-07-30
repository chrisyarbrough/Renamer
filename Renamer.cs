namespace Xarbrough.Renamer
{
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;

	[Serializable]
	public class Renamer
	{
		public bool HasOperations => operations.Count > 0;

		public IEnumerable<(StringOperation operation, int id)> Operations
		{
			get
			{
				for (int i = 0; i < operations.Count; i++)
					yield return (operations[i], i);
			}
		}

		[SerializeField]
		private List<StringOperation> operations;

		public void Initialize()
		{
			if (operations == null || operations.Any(x => x == null))
			{
				operations = new List<StringOperation>();

				var types = typeof(RenameWindow).Assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					if (!types[i].IsAbstract && typeof(StringOperation).IsAssignableFrom(types[i]))
					{
						var instance = StringOperation.Create(types[i]);
						operations.Add(instance);
					}
				}
				operations.Sort(SortByCustomOrder);
			}
		}

		private static int SortByCustomOrder(StringOperation x, StringOperation y)
		{
			return x.DefaultOrder.CompareTo(y.DefaultOrder);
		}

		public string CalculateRename(string name, int index, int objectCount)
		{
			if (name == null)
				return null;

			for (int i = 0; i < operations.Count; i++)
			{
				StringOperation operation = operations[i];

				if (operation.Active)
				{
					if (operations[i].Rename(new RenameInput(name, index, objectCount), out string output))
						name = output;
				}
			}

			return name;
		}

		public void Clear()
		{
			if (operations != null)
			{
				for (int i = 0; i < operations.Count; i++)
					Object.DestroyImmediate(operations[i]);

				operations.Clear();
			}
		}

		public void MoveUp(int id)
		{
			if (id > 0)
			{
				(operations[id - 1], operations[id]) = (operations[id], operations[id - 1]);
			}
		}

		public void MoveDown(int id)
		{
			if (id < operations.Count - 1)
			{
				(operations[id + 1], operations[id]) = (operations[id], operations[id + 1]);
			}
		}

		public void ResetAllOperationSettings()
		{
			for (int i = 0; i < operations.Count; i++)
			{
				operations[i].Active = false;
				ResetOperationSettings(i);
			}
		}

		public void ResetOperationSettings(int id)
		{
			Type type = operations[id].GetType();
			Object.DestroyImmediate(operations[id]);
			operations[id] = StringOperation.Create(type);
		}

		public void AddOperationHeaderMenu(GenericMenu menu, int targetID, Action onResetCalled)
		{
			if (targetID > 0)
				menu.AddItem(new GUIContent("Move Up"), false, () => MoveUp(targetID));
			else
				menu.AddDisabledItem(new GUIContent("Move Up"));

			if (targetID < operations.Count - 1)
				menu.AddItem(new GUIContent("Move Down"), false, () => MoveDown(targetID));
			else
				menu.AddDisabledItem(new GUIContent("Move Down"));

			menu.AddSeparator(string.Empty);
			menu.AddItem(new GUIContent("Reset Settings"), false, () =>
			{
				ResetOperationSettings(targetID);
				onResetCalled();
			});
		}

		public void AddWindowOptions(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Reset All"), false, ResetAllOperationSettings);
		}
	}
}