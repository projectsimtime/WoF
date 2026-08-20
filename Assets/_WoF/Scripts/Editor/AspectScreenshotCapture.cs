using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WoF.Editor
{
	/// <summary>
	/// Steps the Game view through a list of resolutions and writes one png per resolution.
	/// Runs in play mode only, so panels are captured in a real game state.
	/// </summary>
	public class AspectScreenshotCapture
	{
		private const int SettleFrameCount = 12;
		private const int WriteFrameCount = 12;
		private const string DefaultPrefix = "wof";

		private enum EStep
		{
			Idle,
			Settling,
			Writing
		}

		private readonly GameViewSizeController _sizeController = new GameViewSizeController();
		private readonly List<AspectResolution> _pending = new List<AspectResolution>();
		private readonly List<string> _writtenFiles = new List<string>();

		private string _outputDirectory;
		private string _filePrefix;
		private int _restoreSizeIndex;
		private bool _restoreRunInBackground;
		private int _currentIndex;
		private int _frameCounter;
		private EStep _step = EStep.Idle;

		public event Action<IReadOnlyList<string>> Finished;

		public bool IsRunning => _step != EStep.Idle;

		public string CurrentLabel =>
			IsRunning && _currentIndex < _pending.Count ? _pending[_currentIndex].Label : string.Empty;

		public bool CanRun(out string reason)
		{
			if (!EditorApplication.isPlaying)
			{
				reason = "Enter play mode and set up the shot, then capture.";
				return false;
			}

			if (!_sizeController.IsAvailable)
			{
				reason = "Unity's internal Game view API could not be resolved on this version.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		public void Run(IReadOnlyList<AspectResolution> resolutions, string outputDirectory, string filePrefix)
		{
			if (IsRunning || resolutions == null || resolutions.Count == 0)
			{
				return;
			}

			_pending.Clear();
			_writtenFiles.Clear();

			for (int i = 0; i < resolutions.Count; ++i)
			{
				_pending.Add(resolutions[i]);
			}

			_outputDirectory = outputDirectory;
			_filePrefix = string.IsNullOrEmpty(filePrefix) ? DefaultPrefix : filePrefix;

			Directory.CreateDirectory(_outputDirectory);

			_restoreSizeIndex = _sizeController.SelectedSizeIndex;
			_restoreRunInBackground = Application.runInBackground;
			Application.runInBackground = true;

			_currentIndex = 0;
			ApplyCurrentResolution();

			EditorApplication.update += Tick;
		}

		private void Tick()
		{
			if (!EditorApplication.isPlaying)
			{
				Stop();
				return;
			}

			++_frameCounter;

			switch (_step)
			{
				case EStep.Settling:
					if (_frameCounter >= SettleFrameCount)
					{
						CaptureCurrent();
					}

					break;

				case EStep.Writing:
					if (_frameCounter >= WriteFrameCount)
					{
						MoveNext();
					}

					break;
			}
		}

		private void ApplyCurrentResolution()
		{
			AspectResolution resolution = _pending[_currentIndex];

			_sizeController.Select(
				_sizeController.FindOrCreateFixedResolution(resolution.Width, resolution.Height));

			_frameCounter = 0;
			_step = EStep.Settling;
		}

		private void CaptureCurrent()
		{
			AspectResolution resolution = _pending[_currentIndex];
			Vector2 renderSize = _sizeController.TargetRenderSize;

			int width = renderSize.x >= 1f ? (int)renderSize.x : resolution.Width;
			int height = renderSize.y >= 1f ? (int)renderSize.y : resolution.Height;

			string path = Path.Combine(_outputDirectory,
				$"{_filePrefix}_{resolution.FileLabel}_{width}x{height}.png");

			ScreenCapture.CaptureScreenshot(path);
			_writtenFiles.Add(path);

			_frameCounter = 0;
			_step = EStep.Writing;
		}

		private void MoveNext()
		{
			++_currentIndex;

			if (_currentIndex >= _pending.Count)
			{
				Stop();
				return;
			}

			ApplyCurrentResolution();
		}

		private void Stop()
		{
			EditorApplication.update -= Tick;
			_step = EStep.Idle;

			Application.runInBackground = _restoreRunInBackground;
			_sizeController.Select(_restoreSizeIndex);

			Finished?.Invoke(_writtenFiles);
		}
	}
}
