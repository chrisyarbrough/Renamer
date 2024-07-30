namespace Xarbrough.Renamer
{
	using UnityEngine;

	internal static class HierarchySorting
	{
		/// <summary>
		/// Sorts GameObjects in the order the Unity Hierarchy displays them.
		/// </summary>
		public static int Sort(Object lhs, Object rhs)
		{
			return GetHierarchySorting(lhs).CompareTo(GetHierarchySorting(rhs));
		}

		private static float GetHierarchySorting(Object obj)
		{
			Transform transform = ((GameObject)obj).transform;
			if (transform.parent == null)
			{
				return transform.GetSiblingIndex();
			}
			else
			{
				float weightFromSiblingIndex = GetSiblingWeight(transform) * (transform.GetSiblingIndex() + 1);
				return GetHierarchySorting(transform.parent.gameObject) + weightFromSiblingIndex;
			}
		}

		private static float GetSiblingWeight(Transform t)
		{
			Transform parent = t.parent;

			if (parent != null)
			{
				// SiblingWeight is the product of the weight of the parent divided amongst their children.
				// An element with 5 siblings would have a sibling weight of .2.
				return GetSiblingWeight(parent) / (parent.childCount + 1);
			}
			else
			{
				return 1;
			}
		}
	}
}