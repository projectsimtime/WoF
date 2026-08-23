using System;
using System.Reflection;
using UnityEditor;

namespace WoF.Editor
{
	/// <summary>
	/// Thin wrapper over UnityEditor's internal Game View sizing API so tools can
	/// drive the Game View resolution from code.
	/// </summary>
	internal static class GameViewReflection
	{
		private const BindingFlags Members =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Assembly EditorAssembly = typeof(UnityEditor.Editor).Assembly;
		private static readonly Type GameViewType = EditorAssembly.GetType("UnityEditor.GameView");
		private static readonly Type SizesType = EditorAssembly.GetType("UnityEditor.GameViewSizes");
		private static readonly Type SizeType = EditorAssembly.GetType("UnityEditor.GameViewSize");
		private static readonly Type SizeKindType = EditorAssembly.GetType("UnityEditor.GameViewSizeType");

		public static EditorWindow GetOpenGameView()
		{
			UnityEngine.Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(GameViewType);

			return windows.Length == 0 ? null : (EditorWindow)windows[0];
		}

		public static int GetSelectedSizeIndex(EditorWindow gameView)
		{
			return (int)GetSelectedSizeIndexProperty().GetValue(gameView);
		}

		public static void SetSelectedSizeIndex(EditorWindow gameView, int index)
		{
			GetSelectedSizeIndexProperty().SetValue(gameView, index);
		}

		/// <returns>The index of the added size within the current group.</returns>
		public static int AddCustomSize(string name, int width, int height)
		{
			object group = GetCurrentGroup();
			object size = SizeType
				.GetConstructor(new[] { SizeKindType, typeof(int), typeof(int), typeof(string) })
				.Invoke(new[] { Enum.Parse(SizeKindType, "FixedResolution"), width, height, name });

			group.GetType().GetMethod("AddCustomSize", Members).Invoke(group, new[] { size });

			return GetTotalCount(group) - 1;
		}

		public static void RemoveCustomSize(int index)
		{
			object group = GetCurrentGroup();

			if (index < 0 || index >= GetTotalCount(group))
			{
				return;
			}

			group.GetType()
				.GetMethod("RemoveCustomSize", Members)
				.Invoke(group, new object[] { index });
		}

		private static PropertyInfo GetSelectedSizeIndexProperty()
		{
			return GameViewType.GetProperty("selectedSizeIndex", Members);
		}

		private static object GetCurrentGroup()
		{
			object sizes = typeof(ScriptableSingleton<>)
				.MakeGenericType(SizesType)
				.GetProperty("instance", BindingFlags.Static | BindingFlags.Public)
				.GetValue(null);

			return SizesType.GetProperty("currentGroup", Members).GetValue(sizes);
		}

		private static int GetTotalCount(object group)
		{
			return (int)group.GetType().GetMethod("GetTotalCount", Members).Invoke(group, null);
		}
	}
}
