using System;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace WoF.Editor
{
	/// <summary>
	/// Captures the Game View at every aspect ratio the demo has to ship
	/// (20:9, 16:9 and 4:3) from a single keypress while in play mode.
	/// Default key: Ctrl+Alt+S, rebindable from Edit > Shortcuts > WoF.
	/// </summary>
	public static class AspectScreenshotCapture
	{
		private readonly struct AspectSpec
		{
			public AspectSpec(string label, int width, int height)
			{
				Label = label;
				Width = width;
				Height = height;
			}

			public string Label { get; }
			public int Width { get; }
			public int Height { get; }

			public string SizeName => $"WoF {Label}";
			public string FileName => $"screenshot_{Label.Replace(':', '-')}.png";
		}

		private const string OutputFolder = "Docs";
		private const int WarmUpTicks = 12;
		private const int WriteTimeoutTicks = 600;

		private static readonly AspectSpec[] Specs =
		{
			new AspectSpec("20:9", 2400, 1080),
			new AspectSpec("16:9", 1920, 1080),
			new AspectSpec("4:3", 1440, 1080)
		};

		private static readonly int[] CustomSizeIndices = new int[Specs.Length];

		private static bool _isCapturing;
		private static int _specIndex;
		private static int _ticks;
		private static bool _isWaitingForFile;
		private static string _pendingPath;
		private static int _originalSizeIndex;
		private static int _registeredSizeCount;
		private static EditorWindow _gameView;

		[MenuItem("WoF/Capture Aspect Screenshots")]
		[Shortcut("WoF/Capture Aspect Screenshots", KeyCode.S, ShortcutModifiers.Action | ShortcutModifiers.Alt)]
		public static void Capture()
		{
			if (_isCapturing)
			{
				return;
			}

			if (!EditorApplication.isPlaying)
			{
				Debug.LogWarning("Aspect screenshots: enter play mode first, the Game View has nothing to capture.");
				return;
			}

			_gameView = GameViewReflection.GetOpenGameView();

			if (_gameView == null)
			{
				Debug.LogWarning("Aspect screenshots: no Game View window is open.");
				return;
			}

			Directory.CreateDirectory(OutputFolder);

			_isCapturing = true;
			_specIndex = 0;
			_ticks = 0;
			_isWaitingForFile = false;
			_pendingPath = null;
			_originalSizeIndex = GameViewReflection.GetSelectedSizeIndex(_gameView);

			RegisterSizes();
			SelectCurrentSpec();
			EditorApplication.update += Tick;
		}

		/// <summary>
		/// Registers every size up front so nothing is removed mid-run: custom sizes are
		/// appended, so removing one would shift the indices of the ones after it.
		/// </summary>
		private static void RegisterSizes()
		{
			for (int i = 0; i < Specs.Length; ++i)
			{
				AspectSpec spec = Specs[i];
				CustomSizeIndices[i] = GameViewReflection.AddCustomSize(spec.SizeName, spec.Width, spec.Height);
				_registeredSizeCount = i + 1;
			}
		}

		private static void SelectCurrentSpec()
		{
			GameViewReflection.SetSelectedSizeIndex(_gameView, CustomSizeIndices[_specIndex]);
			_gameView.Repaint();
		}

		private static void Tick()
		{
			try
			{
				if (_isWaitingForFile)
				{
					WaitForFile();
					return;
				}

				++_ticks;
				_gameView.Repaint();

				if (_ticks < WarmUpTicks)
				{
					return;
				}

				_pendingPath = Path.Combine(OutputFolder, Specs[_specIndex].FileName);

				if (File.Exists(_pendingPath))
				{
					File.Delete(_pendingPath);
				}

				ScreenCapture.CaptureScreenshot(_pendingPath);
				_isWaitingForFile = true;
				_ticks = 0;
			}
			catch (Exception exception)
			{
				Debug.LogError($"Aspect screenshots: {exception}");
				Finish();
			}
		}

		private static void WaitForFile()
		{
			++_ticks;
			_gameView.Repaint();

			if (!File.Exists(_pendingPath))
			{
				if (_ticks < WriteTimeoutTicks)
				{
					return;
				}

				Debug.LogError($"Aspect screenshots: timed out waiting for {_pendingPath}.");
				Finish();
				return;
			}

			LogCaptured(_pendingPath, Specs[_specIndex]);

			_isWaitingForFile = false;
			_ticks = 0;
			++_specIndex;

			if (_specIndex >= Specs.Length)
			{
				Debug.Log($"Aspect screenshots: done, {Specs.Length} files in {OutputFolder}.");
				Finish();
				return;
			}

			SelectCurrentSpec();
		}

		private static void LogCaptured(string path, AspectSpec spec)
		{
			if (!TryReadPngSize(path, out int width, out int height))
			{
				Debug.Log($"Aspect screenshots: wrote {path}.");
				return;
			}

			if (width == spec.Width && height == spec.Height)
			{
				Debug.Log($"Aspect screenshots: wrote {path} at {width}x{height}.");
				return;
			}

			Debug.LogWarning(
				$"Aspect screenshots: {path} came out {width}x{height}, expected {spec.Width}x{spec.Height}. " +
				"Set the Game View scale slider to 1x and give the window enough room.");
		}

		private static bool TryReadPngSize(string path, out int width, out int height)
		{
			width = 0;
			height = 0;

			byte[] header = new byte[24];

			using (FileStream stream = File.OpenRead(path))
			{
				if (stream.Read(header, 0, header.Length) != header.Length)
				{
					return false;
				}
			}

			width = (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
			height = (header[20] << 24) | (header[21] << 16) | (header[22] << 8) | header[23];

			return width > 0 && height > 0;
		}

		private static void Finish()
		{
			EditorApplication.update -= Tick;

			if (_gameView != null)
			{
				GameViewReflection.SetSelectedSizeIndex(_gameView, _originalSizeIndex);
				_gameView.Repaint();
			}

			// Tail first, so each removal leaves the remaining indices untouched.
			for (int i = _registeredSizeCount - 1; i >= 0; --i)
			{
				GameViewReflection.RemoveCustomSize(CustomSizeIndices[i]);
			}

			_registeredSizeCount = 0;
			_isCapturing = false;
			_isWaitingForFile = false;
			_pendingPath = null;
			_gameView = null;
		}
	}
}
