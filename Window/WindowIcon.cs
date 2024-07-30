using UnityEditor;
using UnityEngine;

namespace Xarbrough.Renamer
{
	[CreateAssetMenu]
	public class WindowIcon : ScriptableObject
	{
		[SerializeField]
		private Texture2D windowIconLightSkin;

		[SerializeField]
		private Texture2D windowIconDarkSkin;

		public void Apply(EditorWindow window)
		{
			window.titleContent = new GUIContent(
				"Renamer",
				EditorGUIUtility.isProSkin ? windowIconDarkSkin : windowIconLightSkin);
		}
	}
}