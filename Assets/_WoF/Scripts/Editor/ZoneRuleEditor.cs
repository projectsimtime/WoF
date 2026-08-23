using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using WoF.Reward;
using WoF.Zone;

namespace WoF.Editor
{
	[CustomEditor(typeof(ZoneRules))]
	public class ZoneRulesEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			ZoneRules rules = (ZoneRules)target;
			GameSettings settings = rules.GameSettings;

			if (settings == null)
			{
				EditorGUILayout.HelpBox("Assign Game Settings to validate.", MessageType.Warning);
				return;
			}

			HashSet<int> seenZones = new HashSet<int>();

			EditorGUILayout.Space();

			for (int i = 0; i < rules.ZoneOverrides.Length; ++i)
			{
				ZoneRules.ZoneOverride zoneOverride = rules.ZoneOverrides[i];
				int zone = zoneOverride.ZoneNumber;

				if (zone < 1 || zone > settings.EndGameZoneIndex)
				{
					EditorGUILayout.HelpBox(
						$"Element {i}: zone {zone} is outside 1-{settings.EndGameZoneIndex}.",
						MessageType.Error);
				}

				if (!seenZones.Add(zone))
				{
					EditorGUILayout.HelpBox($"Element {i}: zone {zone} is overridden twice.",
						MessageType.Error);
				}

				RewardDefinition[] rewards = zoneOverride.Rewards;
				int rewardCount = rewards == null ? 0 : rewards.Length;

				if (rewardCount != settings.SlotCount)
				{
					EditorGUILayout.HelpBox(
						$"Zone {zone}: {rewardCount} rewards, expected {settings.SlotCount}.",
						MessageType.Error);
					continue;
				}

				int emptyCount = rewards.Count(reward => reward == null);

				if (emptyCount > 0)
				{
					EditorGUILayout.HelpBox($"Zone {zone}: {emptyCount} empty slots.",
						MessageType.Error);
				}

			}
		}
	}
}
