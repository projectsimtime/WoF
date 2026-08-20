using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WoF.Editor
{
	/// <summary>
	/// Editor window that dumps one screenshot per demo aspect ratio to disk.
	/// </summary>
	public class AspectScreenshotWindow : EditorWindow
	{
		private static readonly AspectResolution[] Aspects =
		{
			new AspectResolution("20:9", 2400, 1080),
			new AspectResolution("16:9", 1920, 1080),
			new AspectResolution("4:3", 1440, 1080)
		};

		private readonly AspectScreenshotCapture _capture = new AspectScreenshotCapture();

		[SerializeField] private string _outputDirectory;
		[SerializeField] private string _filePrefix = "wof";
		[SerializeField] private bool _portrait;

		[MenuItem("Tools/WoF/Aspect Screenshots")]
		private static void Open()
		{
			GetWindow<AspectScreenshotWindow>("Aspect Screenshots").minSize = new Vector2(380f, 260f);
		}

		private void OnEnable()
		{
			if (string.IsNullOrEmpty(_outputDirectory))
			{
				_outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Screenshots"));
			}

			_capture.Finished += OnCaptureFinished;
		}

		private void OnDisable()
		{
			_capture.Finished -= OnCaptureFinished;
		}

		private void OnGUI()
		{
			EditorGUILayout.HelpBox(
				"Enter play mode, set up the shot, then capture. The Game view resolution is " +
				"switched for each aspect and restored when the run finishes.",
				MessageType.None);

			_filePrefix = EditorGUILayout.TextField("File Prefix", _filePrefix);
			_portrait = EditorGUILayout.Toggle("Portrait", _portrait);

			DrawOutputDirectory();
			DrawResolutionList();

			EditorGUILayout.Space();

			if (_capture.IsRunning)
			{
				EditorGUILayout.LabelField("Capturing", _capture.CurrentLabel);
				Repaint();
				return;
			}

			if (!_capture.CanRun(out string reason))
			{
				EditorGUILayout.HelpBox(reason, MessageType.Warning);
				return;
			}

			if (GUILayout.Button("Capture All Aspects", GUILayout.Height(30f)))
			{
				_capture.Run(BuildResolutions(), _outputDirectory, _filePrefix);
			}
		}

		private void DrawOutputDirectory()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel("Output");
				EditorGUILayout.SelectableLabel(_outputDirectory,
					EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

				if (GUILayout.Button("...", GUILayout.Width(30f)))
				{
					string picked = EditorUtility.OpenFolderPanel(
						"Screenshot Output", _outputDirectory, string.Empty);

					if (!string.IsNullOrEmpty(picked))
					{
						_outputDirectory = picked;
					}
				}
			}
		}

		private void DrawResolutionList()
		{
			EditorGUILayout.Space();

			foreach (AspectResolution resolution in BuildResolutions())
			{
				EditorGUILayout.LabelField(resolution.Label, $"{resolution.Width} x {resolution.Height}");
			}
		}

		private IReadOnlyList<AspectResolution> BuildResolutions()
		{
			AspectResolution[] resolutions = new AspectResolution[Aspects.Length];

			for (int i = 0; i < Aspects.Length; ++i)
			{
				resolutions[i] = _portrait ? Aspects[i].Rotated : Aspects[i];
			}

			return resolutions;
		}

		private void OnCaptureFinished(IReadOnlyList<string> files)
		{
			for (int i = 0; i < files.Count; ++i)
			{
				Debug.Log($"Screenshot written: {files[i]}");
			}

			if (files.Count > 0)
			{
				EditorUtility.RevealInFinder(files[0]);
			}

			Repaint();
		}
	}
}
