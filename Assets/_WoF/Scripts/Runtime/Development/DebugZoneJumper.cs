#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using WoF.Session;

namespace WoF.Development
{
	/// <summary>
	/// Keypad shortcuts for reaching the late zones without playing sixty spins.
	/// </summary>
	public class DebugZoneJumper : MonoBehaviour
	{
		private const int FirstJumpZone = 29;
		private const int SecondJumpZone = 59;

		private IDebugRunCommands _debugRunCommands;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Create()
		{
			new GameObject(nameof(DebugZoneJumper)).AddComponent<DebugZoneJumper>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				JumpToZone(FirstJumpZone);
			}

			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				JumpToZone(SecondJumpZone);
			}
		}

		private void JumpToZone(int zoneIndex)
		{
			if (_debugRunCommands == null && DebugServices.Current != null)
			{
				DebugServices.Current.TryResolve(out _debugRunCommands);
			}

			_debugRunCommands?.JumpToZone(zoneIndex);
		}
	}
}
#endif
