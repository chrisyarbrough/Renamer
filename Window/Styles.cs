namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;

	internal static class Styles
	{
		private static readonly Color splitterDark;
		private static readonly Color splitterLight;

		/// <summary>
		/// Color of UI splitters.
		/// </summary>
		public static Color splitterColor => EditorGUIUtility.isProSkin ? splitterDark : splitterLight;

		private static readonly Color headerBackgroundDark;
		private static readonly Color headerBackgroundLight;

		/// <summary>
		/// Color of effect header backgrounds.
		/// </summary>
		public static Color headerBackground => EditorGUIUtility.isProSkin ? headerBackgroundDark : headerBackgroundLight;

		/// <summary>
		/// Style for a checkbox.
		/// </summary>
		public static readonly GUIStyle smallToggle;

		private static readonly Texture2D paneOptionsIconDark;
		private static readonly Texture2D paneOptionsIconLight;

		/// <summary>
		/// Option icon used in string operation headers.
		/// </summary>
		public static Texture2D paneOptionsIcon => EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;

		static Styles()
		{
			splitterDark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
			splitterLight = new Color(0.6f, 0.6f, 0.6f, 1.333f);

			headerBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
			headerBackgroundLight = new Color(1f, 1f, 1f, 0.2f);

			smallToggle = new GUIStyle("ShurikenToggle");

			paneOptionsIconDark =
				(Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
			paneOptionsIconLight =
				(Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
		}
	}
}
