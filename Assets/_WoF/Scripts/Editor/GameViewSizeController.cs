using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WoF.Editor
{
	/// <summary>
	/// Reflection wrapper over Unity's internal Game view size list, so a capture can switch
	/// resolution without the user touching the aspect dropdown by hand.
	/// </summary>
	public class GameViewSizeController
	{
		private const BindingFlags AnyInstance =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private const string FixedResolutionName = "FixedResolution";
		private const string GameViewTitle = "Game";

		private readonly Type _gameViewType;
		private readonly Type _gameViewSizeType;
		private readonly Type _sizeTypeEnum;
		private readonly object _sizeGroup;

		public GameViewSizeController()
		{
			// GameView derives from EditorWindow, so it always lives in the same assembly.
			Assembly editorAssembly = typeof(EditorWindow).Assembly;

			_gameViewType = editorAssembly.GetType("UnityEditor.GameView");
			_gameViewSizeType = editorAssembly.GetType("UnityEditor.GameViewSize");
			_sizeTypeEnum = editorAssembly.GetType("UnityEditor.GameViewSizeType");
			_sizeGroup = ResolveCurrentGroup(editorAssembly);
		}

		public bool IsAvailable =>
			_gameViewType != null && _gameViewSizeType != null &&
			_sizeTypeEnum != null && _sizeGroup != null;

		/// <summary>
		/// Index of the size currently picked in the Game view dropdown, kept so it can be restored.
		/// </summary>
		public int SelectedSizeIndex =>
			(int)_gameViewType.GetProperty("selectedSizeIndex", AnyInstance).GetValue(GetGameView());

		/// <summary>
		/// Pixel size the Game view actually renders at, which is what ends up in the png.
		/// </summary>
		public Vector2 TargetRenderSize =>
			(Vector2)_gameViewType.GetProperty("targetRenderSize", AnyInstance).GetValue(GetGameView());

		public void Select(int sizeIndex)
		{
			EditorWindow gameView = GetGameView();

			_gameViewType.GetMethod("SizeSelectionCallback", AnyInstance)
				.Invoke(gameView, new object[] { sizeIndex, null });

			gameView.Repaint();
		}

		/// <summary>
		/// Returns the index of a fixed resolution entry matching the given size, adding one
		/// to the current build target's size group when it is missing.
		/// </summary>
		public int FindOrCreateFixedResolution(int width, int height)
		{
			Type groupType = _sizeGroup.GetType();
			MethodInfo getSize = groupType.GetMethod("GetGameViewSize");
			int totalCount = (int)groupType.GetMethod("GetTotalCount").Invoke(_sizeGroup, null);

			for (int i = 0; i < totalCount; ++i)
			{
				object size = getSize.Invoke(_sizeGroup, new object[] { i });

				if (Matches(size, width, height))
				{
					return i;
				}
			}

			object created = Activator.CreateInstance(_gameViewSizeType, new object[]
			{
				Enum.Parse(_sizeTypeEnum, FixedResolutionName),
				width,
				height,
				$"{width}x{height}"
			});

			groupType.GetMethod("AddCustomSize").Invoke(_sizeGroup, new object[] { created });

			return totalCount;
		}

		private bool Matches(object size, int width, int height)
		{
			object sizeType = _gameViewSizeType.GetProperty("sizeType", AnyInstance).GetValue(size);

			if (Convert.ToInt32(sizeType) != Convert.ToInt32(Enum.Parse(_sizeTypeEnum, FixedResolutionName)))
			{
				return false;
			}

			int sizeWidth = (int)_gameViewSizeType.GetProperty("width", AnyInstance).GetValue(size);
			int sizeHeight = (int)_gameViewSizeType.GetProperty("height", AnyInstance).GetValue(size);

			return sizeWidth == width && sizeHeight == height;
		}

		private EditorWindow GetGameView()
		{
			return EditorWindow.GetWindow(_gameViewType, false, GameViewTitle, false);
		}

		private static object ResolveCurrentGroup(Assembly editorAssembly)
		{
			Type sizesType = editorAssembly.GetType("UnityEditor.GameViewSizes");

			if (sizesType == null)
			{
				return null;
			}

			Type singletonType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
			object sizes = singletonType
				.GetProperty("instance", BindingFlags.Static | BindingFlags.Public)
				.GetValue(null);

			return sizesType.GetProperty("currentGroup", AnyInstance).GetValue(sizes);
		}
	}
}
