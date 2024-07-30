using System;

namespace Xarbrough.Renamer
{
	using UnityEditor;
	using UnityEngine;

	internal class WindowLayout
	{
		private EnumPref<SplitDirection> selectedDirection;

		public void Load()
		{
			selectedDirection = new EnumPref<SplitDirection>("WindowLayout", SplitDirection.Auto);
		}

		private void SelectDirection(object layoutID)
		{
			selectedDirection.Value = (SplitDirection)(int)layoutID;
		}

		public void SplitRect(Rect windowRect, out Rect a, out Rect b)
		{
			SplitRect(windowRect, DetermineSplitDirection(windowRect), out a, out b);
		}

		private SplitDirection DetermineSplitDirection(Rect windowPosition)
		{
			if (selectedDirection == SplitDirection.Auto)
			{
				Vector2 size = windowPosition.size;
				if (size.x > size.y)
					return SplitDirection.Horizontal;
				else
					return SplitDirection.Vertical;
			}
			else
			{
				return selectedDirection;
			}
		}

		private void SplitRect(Rect windowRect, SplitDirection direction, out Rect a, out Rect b)
		{
			windowRect = new(Vector2.zero, windowRect.size);

			// When dividing an uneven height, e.g. 5, make the upper
			// rect half size 2, but the bottom half size 3 for pixel perfection.
			if (direction == SplitDirection.Horizontal)
			{
				float halfWidth = windowRect.width / 2f;
				windowRect.width = Mathf.FloorToInt(halfWidth);
				b = new Rect(windowRect.xMax, windowRect.y, Mathf.CeilToInt(halfWidth), windowRect.height);
			}
			else // Vertical
			{
				float halfHeight = windowRect.height / 2f;
				windowRect.height = Mathf.FloorToInt(halfHeight);
				b = new Rect(windowRect.x, windowRect.yMax, windowRect.width, Mathf.CeilToInt(halfHeight));
			}
			a = windowRect;
		}

		public void AddWindowOptions(GenericMenu menu)
		{
			foreach (SplitDirection direction in Enum.GetValues(typeof(SplitDirection)))
			{
				menu.AddItem(
					new GUIContent($"Window Layout/{direction}"),
					on: selectedDirection == direction,
					func: SelectDirection,
					userData: (int)direction);
			}
		}

		private enum SplitDirection
		{
			Auto = 0,
			Vertical = 1,
			Horizontal = 2,
		}
	}
}