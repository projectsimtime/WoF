# Wheel of Fortune Project Memory

This file records settled decisions and current context for future sessions. It is not a substitute for reading the relevant code. Update it when the user accepts or rejects an architectural direction.

## Project goal

This is a revision of a Unity game-developer job application demo. The revision must improve visual quality, match the reference layout, fix death/spin/reward flow, and make movement and transitions feel smooth. Final evidence must include videos captured from an Android or iOS device and uploaded to Google Drive.

The original review identified these risks:

- `GameSession` had become a god class combining zone generation, spin flow, reward accounting, economy, revive pricing, exit rules, hint flow, and many panels.
- Zone and rarity selection contained hard-coded type chains that were closed to extension.
- Some base classes were fake or too broad.
- Runtime scene searches and concrete UI dependencies reversed the desired dependency direction.
- The review mentioned a low interface count, but the user explicitly does not want interfaces added without a genuine substitution or test boundary.

## Settled architectural decisions

- No state-machine refactor for this demo.
- Events communicate game state changes to presentation. Public queries remain public when they are the simplest correct read boundary.
- `GameSession` owns one runtime `ZoneSchedule`, the current zone index, and the `ZoneEntered(int zoneIndex, ZoneDefinition zoneDefinition)` event. It publishes the event after orchestrating entry into a zone.
- `ZoneProgressBarController` depends directly on `ZoneSchedule` for surrounding-zone lookup and receives current-zone updates through `SetCurrentZone`; it does not depend on `GameSession`.
- `GameHudController` injects `GameSession.ZoneSchedule` into the bar and relays `GameSession.ZoneEntered` through `SetCurrentZone`.
- Do not introduce `ZoneProgression` or `IZoneProgressSource` merely to move an integer, event, and schedule lookup behind another type. Add a progression abstraction later only if it gains meaningful independent behavior. Do not turn the schedule into static/global state.
- `ZoneProgressEntry` is a small presentation component. It receives an index, color, visibility, and preferred width; it does not query game state.
- Zone 1 being configured as safe is intentional.

## Zone pattern and schedule

- `ZoneDefinition` is the complete per-type gameplay and presentation asset.
- `ZonePattern` contains the default definition and special rules.
- `ZoneSchedule` does not brute-force or cache a definition for every zone. It stores a sparse `SortedDictionary<int, ZoneDefinition>` for special indices and falls back to the default definition.
- `SpecialZoneDefinitions` contains unique special definitions only; the default definition is intentionally excluded.
- Special definitions are collected once for upcoming-zone notifications.
- The wheel's per-definition reserved-reward bookkeeping should initialize lazily when encountered; it should not depend on all zone definitions.
- Rules are sorted in place by ascending priority. Applying later entries through the dictionary indexer makes the higher priority win.
- Recurring indices and explicit overrides remain separate passes. Explicit overrides run second and therefore beat recurring rules regardless of priority. Higher priority wins when multiple explicit overrides target the same index.
- `_overrideZoneIndices` is the agreed field name.
- Special-rule frequency has `[Min(2)]`; the default fills all unclaimed indices and is conceptually frequency 1.
- Invalid asset configuration logs a warning rather than throwing.

## Zone progress bar

Current implementation direction:

- The top bar shows 13 visible entries by default and keeps the current marker centered.
- It creates one extra entry for the incoming value. The hierarchy remains fixed; after the slide, all entries receive the next displayed range before the content offset resets to zero.
- It completes an active tween before processing another zone event.
- It rebuilds immediately for the first zone or a non-sequential jump.
- Out-of-range indices are hidden because `ZoneSchedule.GetZoneDefinition` returns null outside the configured run.
- Past labels use `zoneDefinition.ThemeColor * Color.gray`.
- The current frame uses the current theme. The current number picks black or white for contrast.
- The arrow is a fixed black prefab visual and is not recolored in code.
- The bar is not hidden by game flow.
- No `LayoutRebuilder.ForceRebuildLayoutImmediate` unless a reproducible layout timing bug proves it necessary.
- The fixed entries are not a pooling use case. Existing pooling candidates are the actual `Instantiate`/`Destroy` loops in reward presentation, and those should be handled separately.

## Working preferences established with the user

- Use explicit variable names, especially `_currentZoneIndex` and `zoneDefinition`.
- Inspect and mimic existing implementations before adding code. For component references, this particularly means editor-time `OnValidate()` assignment with `GetComponent` or `GetComponentInChildren` when the hierarchy makes the target unambiguous.
- Do not overengineer. Explain the responsibility and dependency direction before creating a new architectural type.
- Do not treat SOLID as a request to maximize interface or class count.
- Do not compile Unity for routine edits unless the user asks.
- Keep zone refactor work separate from zone-progress work when preparing commits, and preserve the user's staged changes.

## Brief checklist still relevant to the revision

- Match the reference layout and use unstretched supplied visuals.
- Canvas Scaler: Expand.
- TextMeshPro for UI text.
- Runtime-changing UI hierarchy names end in `_value`; names run from general to specific.
- Disable unnecessary image raycasts and masking.
- Use sliced sprites for scalable frames.
- Keep UI animation off the root transform.
- Validate 20:9, 16:9, and 4:3.
- Validate exit lock during wheel spin and reward reveal.
- Validate bomb, give-up, ad revive, currency revive, restart, safe-zone, super-zone, and end-game flows.
- Record final video on a physical Android or iOS device and upload it to Google Drive.
- Publish the APK through a GitHub Release and provide the requested repository and release links.
- The brief says AI should be limited to research and learning. The user must understand and be able to explain every core design and implementation decision in the submission.

## Immediate follow-up

- Review the zone progress bar in Unity at all three target aspect ratios.
- Verify rapid sequential `ZoneEntered` events complete and settle the slide correctly.
- Check that the current marker remains centered after resize and that the last configured zone receives its correct theme.
- Continue the broader revision audit against the brief after the top bar is accepted.
