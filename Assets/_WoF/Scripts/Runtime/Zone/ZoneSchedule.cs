using System.Collections.Generic;
using UnityEngine;

namespace WoF.Zone
{
	public class ZoneSchedule
	{
		private ZoneDefinition _defaultZoneDefinition;
		private SortedDictionary<int, ZoneDefinition> _specialZoneDefinitionsByIndex = new();
		private List<ZoneDefinition> _specialZoneDefinitions = new();
		private int _lastZoneIndex;

		public List<ZoneDefinition> SpecialZoneDefinitions => _specialZoneDefinitions;

		public ZoneSchedule(ZonePattern zonePattern, int lastZoneIndex)
		{
			if (!zonePattern)
			{
				Debug.LogWarning("Zone pattern is null!");
				return;
			}

			if (!zonePattern.DefaultZoneDefinition)
			{
				Debug.LogWarning("No default zone is specified!");
				return;
			}

			_defaultZoneDefinition = zonePattern.DefaultZoneDefinition;
			_lastZoneIndex = lastZoneIndex;

			List<ZonePatternRule> zonePatternRulesByPriority = GetValidZonePatternRules(zonePattern.ZonePatternRules);
			zonePatternRulesByPriority.Sort((firstZonePatternRule, secondZonePatternRule) =>
				firstZonePatternRule.Priority.CompareTo(secondZonePatternRule.Priority));
			CollectSpecialZoneDefinitions(zonePatternRulesByPriority);

			AddRecurringZones(zonePatternRulesByPriority);
			ApplyZoneOverrides(zonePatternRulesByPriority);
		}

		public ZoneDefinition GetZoneDefinition(int zoneIndex)
		{
			if (zoneIndex < 1 || zoneIndex > _lastZoneIndex)
			{
				return null;
			}

			return _specialZoneDefinitionsByIndex.GetValueOrDefault(zoneIndex, _defaultZoneDefinition);
		}

		public int GetNextZoneIndex(ZoneDefinition zoneDefinition, int currentZoneIndex)
		{
			if (!zoneDefinition)
			{
				Debug.LogWarning("Zone definition is null!");
				return int.MaxValue;
			}

			if (zoneDefinition == _defaultZoneDefinition)
			{
				for (int zoneIndex = currentZoneIndex + 1; zoneIndex <= _lastZoneIndex; ++zoneIndex)
				{
					if (!_specialZoneDefinitionsByIndex.ContainsKey(zoneIndex))
					{
						return zoneIndex;
					}
				}

				return int.MaxValue;
			}

			foreach (var specialZoneDefinitionByIndex in _specialZoneDefinitionsByIndex)
			{
				if (specialZoneDefinitionByIndex.Key > currentZoneIndex && specialZoneDefinitionByIndex.Value == zoneDefinition)
				{
					return specialZoneDefinitionByIndex.Key;
				}
			}

			return int.MaxValue;
		}

		private List<ZonePatternRule> GetValidZonePatternRules(ZonePatternRule[] zonePatternRules)
		{
			List<ZonePatternRule> validZonePatternRules = new();

			foreach (ZonePatternRule zonePatternRule in zonePatternRules)
			{
				if (!zonePatternRule.ZoneDefinition)
				{
					Debug.LogWarning("Zone pattern contains a rule without a zone definition.");
					continue;
				}

				if (zonePatternRule.ZoneDefinition == _defaultZoneDefinition)
				{
					Debug.LogWarning($"Default zone '{zonePatternRule.ZoneDefinition.name}' should not also have a pattern rule.");
					continue;
				}

				validZonePatternRules.Add(zonePatternRule);
			}

			return validZonePatternRules;
		}

		private void CollectSpecialZoneDefinitions(List<ZonePatternRule> zonePatternRules)
		{
			foreach (ZonePatternRule zonePatternRule in zonePatternRules)
			{
				if (!_specialZoneDefinitions.Contains(zonePatternRule.ZoneDefinition))
				{
					_specialZoneDefinitions.Add(zonePatternRule.ZoneDefinition);
				}
			}
		}

		private void AddRecurringZones(List<ZonePatternRule> zonePatternRulesByPriority)
		{
			foreach (ZonePatternRule zonePatternRule in zonePatternRulesByPriority)
			{
				for (int zoneIndex = zonePatternRule.Frequency; zoneIndex <= _lastZoneIndex; zoneIndex += zonePatternRule.Frequency)
				{
					_specialZoneDefinitionsByIndex[zoneIndex] = zonePatternRule.ZoneDefinition;
				}
			}
		}

		private void ApplyZoneOverrides(List<ZonePatternRule> zonePatternRulesByPriority)
		{
			foreach (ZonePatternRule zonePatternRule in zonePatternRulesByPriority)
			{
				foreach (int zoneIndex in zonePatternRule.OverrideZoneIndices)
				{
					if (zoneIndex < 1 || zoneIndex > _lastZoneIndex)
					{
						Debug.LogWarning($"'{zonePatternRule.ZoneDefinition}' zone override index {zoneIndex} is out of range!");
						continue;
					}

					_specialZoneDefinitionsByIndex[zoneIndex] = zonePatternRule.ZoneDefinition;
				}
			}
		}
	}
}
