using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using WoF.Zone;

namespace WoF.Tests
{
	public class ZoneResolverTests
	{
		private const string ZoneCatalogPath = "Assets/_WoF/Data/Zone/zoneCatalog.asset";

		private ZoneResolver _zoneResolver;

		[SetUp]
		public void SetUp()
		{
			ZoneCatalog zoneCatalog = AssetDatabase.LoadAssetAtPath<ZoneCatalog>(ZoneCatalogPath);
			Assert.That(zoneCatalog, Is.Not.Null, $"Missing zone catalog at {ZoneCatalogPath}.");

			_zoneResolver = new ZoneResolver(zoneCatalog.ZoneTypes);
		}

		[TestCase(1, "zoneType_Safe")]
		[TestCase(2, "zoneType_Normal")]
		[TestCase(5, "zoneType_Safe")]
		[TestCase(29, "zoneType_Normal")]
		[TestCase(30, "zoneType_Super")]
		[TestCase(60, "zoneType_Super")]
		public void Resolve_ReturnsHighestPriorityMatchingZone(int zoneIndex, string expectedZoneName)
		{
			Assert.That(_zoneResolver.Resolve(zoneIndex).name, Is.EqualTo(expectedZoneName));
		}

		[Test]
		public void GetUpcomingOccurrences_SkipsZonesOverriddenByHigherPriorityDefinitions()
		{
			IReadOnlyList<ZoneOccurrence> occurrences = _zoneResolver.GetUpcomingOccurrences(29, 60);

			Assert.That(occurrences, Has.Count.EqualTo(2));
			Assert.That(occurrences[0].ZoneType.name, Is.EqualTo("zoneType_Super"));
			Assert.That(occurrences[0].ZoneIndex, Is.EqualTo(30));
			Assert.That(occurrences[1].ZoneType.name, Is.EqualTo("zoneType_Safe"));
			Assert.That(occurrences[1].ZoneIndex, Is.EqualTo(35));
		}

		[Test]
		public void GetUpcomingOccurrences_PreservesTheFirstSafeZoneIndicator()
		{
			IReadOnlyList<ZoneOccurrence> occurrences = _zoneResolver.GetUpcomingOccurrences(1, 60);

			Assert.That(occurrences, Has.Count.EqualTo(2));
			Assert.That(occurrences[0].ZoneType.name, Is.EqualTo("zoneType_Super"));
			Assert.That(occurrences[0].ZoneIndex, Is.EqualTo(30));
			Assert.That(occurrences[1].ZoneType.name, Is.EqualTo("zoneType_Safe"));
			Assert.That(occurrences[1].ZoneIndex, Is.EqualTo(1));
		}

		[Test]
		public void Resolve_ProvidesExitCapabilityWithoutCheckingZoneNames()
		{
			Assert.That(_zoneResolver.Resolve(1).CanExit, Is.True);
			Assert.That(_zoneResolver.Resolve(2).CanExit, Is.False);
			Assert.That(_zoneResolver.Resolve(30).CanExit, Is.True);
		}
	}
}
