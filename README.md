# Wheel of Fortune

---

## Video

[**Watch the gameplay video**](https://drive.google.com/file/d/1F6EvGE_bxK7qtbxdDJT4ts6WxUYiWMkX/view?usp=sharing)

The video has text on it for the zone override and the next zone hint.

---

## Screenshots

**20:9**

![20:9](Docs/screenshot_20-9.png)

**16:9**

![16:9](Docs/screenshot_16-9.png)

**4:3**

![4:3](Docs/screenshot_4-3.png)

---

## Zone override

Rewards on the wheel are random, but any zone can be set by hand instead.
Zone 3 is done this way — all cash. You can see it in the video.

![Zone 3 override](Docs/zone_override.png)

---

## Next zone hint (my own idea)

This is not in the brief. I added it myself.

The player can watch an ad to see the best reward waiting in the next zone.
I wanted to give the player a reason to keep going instead of leaving.
You can see it in the video.

![Next zone hint](Docs/next_zone_hint.png)

---

## Continue with currency (bonus part)

The brief says this one is a bonus, so I did it.

When the bomb explodes the player can give up, pay currency to revive, or watch an ad to revive for free.
Every revive costs more than the last one.

---

## Architecture

`GameBootstrap` is the composition root and the only class that knows concrete types.
On `Awake` it walks the scene once, registers every component that publishes an
`ISceneService` contract, builds the services on top of those contracts, and hands each
scene component the single interface it asked for. There is no `FindObjectOfType` in the
game code and no button holds a reference to the session.

| Layer | Classes |
| --- | --- |
| Composition root | `GameBootstrap`, `ServiceContainer`, `SceneComponentScanner` |
| Flow | `GameFlow`, `ZoneRunner`, `SpinService`, `ExitService`, `HintService`, `ReviveService` |
| State and rules | `RunState`, `ZoneCalculator`, `ZoneWheelFactory`, `MilestoneRewardPlanner`, `RarityCalculator`, `SpinAngleCalculator`, `ReviveCostCalculator`, `EconomyService`, `RewardService` |
| Contracts | `ISpinCommands`, `IExitCommands`, `IReviveCommands`, `IHintCommands`, `IRunCommands`, `IWheelView`, `ILoseView`, `IExitView`, `IHintView`, `IEndGameView`, `ICurrencyView`, `IRewardTrackView`, `IRewardSummaryView`, `IZoneHudView`, `IZoneIndicatorView`, `IAdPlayer`, `IAdPlaybackControls` |
| Views | The `*PanelController` and `*View` MonoBehaviours, which only implement their contract |

Zone flavours, rarity odds and ad lengths are data. A new zone type is a new `ZoneTypeData`
asset added to the `zoneCatalog`, with its own frequency, priority and wheel; a new rarity is
a row in the rarity table on `gameSettings`. Neither requires a code change.
