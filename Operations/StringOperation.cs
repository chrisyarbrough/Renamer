namespace Xarbrough.Renamer
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public abstract class StringOperation : ScriptableObject
	{
		public static StringOperation Create(Type type)
		{
			var instance = (StringOperation)CreateInstance(type);
			instance.hideFlags = HideFlags.DontSave;
			instance.name = instance.DisplayContent.text;
			return instance;
		}

		/// <summary>
		/// Indicates whether this operation will be applied or ignored
		/// when executing the stack of rename operations.
		/// </summary>
		public bool Active;

		/// <summary>
		/// Performs the implemented operation on the provided input.
		/// Operations may be reordered and therefore the input may have been
		/// processed by other other operations already.
		/// </summary>
		public abstract bool Rename(RenameInput input, out string output);

		public abstract GUIContent DisplayContent { get; }

		/// <summary>
		/// Determines the order this operation appears in a list of operations when first opening the window.
		/// Lower values are displayed at the top, higher values at the bottom.
		/// </summary>
		public virtual int DefaultOrder => 100;

		/// <summary>
		///	Indicates the modes (flags) in which this operation should be shown.
		/// </summary>
		/// <example>
		///	An operation of type <see cref="ViewMode.Advanced"/> will only be
		///	shown in the advanced view mode. An operation of type
		///	<see cref="ViewMode.Default"/> | <see cref="ViewMode.Advanced"/>
		///	will be shown in both the default and the advanced view.
		/// </example>
		protected virtual ViewMode ViewMode => ViewMode.Default | ViewMode.Advanced;

		/// <summary>
		/// Returns true if this operation should be displayed in the provided mode argument.
		/// </summary>
		/// <param name="currentViewMode">The mode to check against.</param>
		public bool DisplayInMode(ViewMode currentViewMode) => ViewMode.HasFlagCustom(currentViewMode);

		public virtual Color DisplayColor => Color.clear;

		protected static Color FromHTML(string html)
		{
			ColorUtility.TryParseHtmlString(html, out Color color);
			return color;
		}

		protected SerializedObject SerializedObject { get; private set; }

		public SerializedProperty ActiveProp { get; private set; }

		protected virtual void OnEnable()
		{
			SerializedObject = new SerializedObject(this);
			ActiveProp = SerializedObject.FindProperty(nameof(Active));
		}

		protected SerializedProperty FindProperty(string name) => SerializedObject.FindProperty(name);

		protected virtual void OnDisable()
		{
			if (SerializedObject != null)
			{
				SerializedObject.Dispose();
				SerializedObject = null;
			}
		}

		public void Draw()
		{
			SerializedObject.Update();
			GUILayout.Space(3f);
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth -= 20;
			OnInspectorGUI();
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUILayout.Space();
			SerializedObject.ApplyModifiedProperties();
		}

		protected virtual void OnInspectorGUI()
		{
			SerializedProperty iterator = SerializedObject.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				enterChildren = false;
				if (!propertiesToExclude.Contains(iterator.name))
				{
					if (iterator.propertyType == SerializedPropertyType.Integer)
					{
						iterator.intValue = IntFieldWithStepper.DrawLayout(iterator.intValue, iterator.displayName);
					}
					else
					{
						EditorGUILayout.PropertyField(iterator, true);
					}
				}
			}
		}

		protected void DrawProperty(string name)
		{
			EditorGUILayout.PropertyField(SerializedObject.FindProperty(name));
		}

		protected static readonly HashSet<string> propertiesToExclude = new HashSet<string>()
		{
			"m_Script",
			"Active",
		};
	}
}