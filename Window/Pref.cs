namespace Xarbrough.Renamer
{
	using UnityEditor;
	using System;

	internal abstract class Pref<T>
	{
		public T Value
		{
			get
			{
				if (!isCachePrimed)
				{
					cachedValue = GetEditorPrefsValue();
					isCachePrimed = true;
				}

				return cachedValue;
			}
			set
			{
				if (!isCachePrimed)
				{
					cachedValue = Value;
					isCachePrimed = true;
				}

				if (!value.Equals(cachedValue))
				{
					SetEditorPrefsValue(value);
					cachedValue = value;
				}
			}
		}

		protected readonly string key;
		protected readonly T defaultValue;

		protected T cachedValue;
		private bool isCachePrimed;

		protected Pref(string name, T defaultValue)
		{
			key = "Xarbrough/Renamer/" + name;
			this.defaultValue = defaultValue;
		}

		protected abstract T GetEditorPrefsValue();
		protected abstract void SetEditorPrefsValue(T value);

		public static implicit operator T(Pref<T> pref) => pref.Value;
	}

	internal sealed class BoolPref : Pref<bool>
	{
		public BoolPref(string name, bool defaultValue) : base(name, defaultValue)
		{
		}

		protected override bool GetEditorPrefsValue() => EditorPrefs.GetBool(key, defaultValue);

		protected override void SetEditorPrefsValue(bool value) => EditorPrefs.SetBool(key, value);
	}

	internal sealed class EnumPref<T> : Pref<T> where T : Enum
	{
		public EnumPref(string name, T defaultValue) : base(name, defaultValue)
		{
		}

		protected override T GetEditorPrefsValue() => (T)(object)EditorPrefs.GetInt(key, GetEnumValue(defaultValue));

		protected override void SetEditorPrefsValue(T value) => EditorPrefs.SetInt(key, GetEnumValue(value));

		// The boxing here is faster than using (value as IConvertible).ToInt32(CultureInfo.InvariantCulture).
		private static int GetEnumValue(T value) => (int)(object)value;
	}
}