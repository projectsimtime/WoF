# WoF Demo — Revision Notes

> Everything from the review session, in one place. Working notes — gitignored, not for the reviewer.
> Task checklist lives separately in `TASKS.md`.

---

## 1. The feedback, decoded

Two FAIL, three WARN. The two FAILs (SRP + DIP) are **the same bug wearing two hats** — fix one and the other mostly dissolves.

What they praised, which matters: *"Hesaplayıcıların çıkarılmış olması doğru bir refleks"* and *"SO tabanlı çark ve zone içeriği ile AdRewardBase polimorfizmi gerçek uzantı noktaları."* This is a "close, come back" rejection, not a "you can't code" rejection.

### The root cause: dependency arrows

```
SpinButton ──┐
ExitButton ──┤
ReviveButton─┼──► GameSession ──► LosePanelController
SeeAdButton ─┤        │      ──► ExitPanelController
GiveUpButton─┘        │      ──► RewardPanelController
                      │      ──► CurrencyPanelController
                      │      ──► ...9 panels total
                      └──────► ZoneCalculator, RarityCalculator,
                               SpinAngleCalculator, ReviveCostCalculator
```

Every arrow points **at** `GameSession`, and `GameSession` points back **at** every concrete view. That hub shape is what "god class" means — not "541 lines."

**The metric to fix is arrows, not lines.** Splitting `GameSession.cs` into five 110-line files that still reference each other and still get found by `FindObjectOfType` would fail again.

### Scoreboard

| Metric | Now | Target |
|---|---|---|
| `GameSession.cs` lines | 541 | ~110 |
| `FindObjectOfType` calls | 12 | 1 |
| Panel fields in GameSession | 9 | 0 |
| Public methods on GameSession | 15 | <5 |
| Interfaces in project | 1 | 4 |
| Button subclasses | 13 | 3 |
| Tests | 0 | ~12 |

The **15 public methods** number is the one to watch hardest — it means fifteen things can reach in and command the core.

```bash
cd /c/dev/WoF/Assets/_WoF/Scripts/Runtime
wc -l GameSession.cs
grep -rn "FindObjectOfType" . | wc -l
grep -rn "interface " . | wc -l
find . -name "*Button.cs" | wc -l
```

---

## 2. SOLID reference

SOLID is really **two ideas**: separate things that change for different reasons (S, I), and depend on shapes not specifics (O, L, D).

### S — Single Responsibility

> One reason to change — meaning one *person* who asks for changes to it.

```csharp
class Order {
    void  AddItem(Item i)   { ... }   // business rules  → product owner
    void  SaveToDatabase()  { ... }   // persistence     → backend dev
    string ToHtml()         { ... }   // presentation    → designer
}
```

Three people can each force a change to this file. Split: `Order`, `OrderRepository`, `OrderView`.

**Spot it without reading methods:** look at the `using` block. `GameSession.cs:1-16` imports Currency, EndGame, ExitPanel, HintPanel, LosePanel, Reward, RewardSummary, Wheel, Zone, ZoneView, DOTween. Eleven unrelated worlds in one file.

**Self-check:** describe the class in one sentence. If you need "and," split it.

### O — Open/Closed

> Open for extension, closed for modification. **"Closed" is the good half** — it means you don't have to edit this file to extend it.

```csharp
// ❌ violates OCP — a new shape forces you to MODIFY this method
float Area(Shape s) {
    if (s.Kind == Circle) return 3.14f * s.R * s.R;
    if (s.Kind == Square) return s.Side * s.Side;
}

// ✅ satisfies OCP — open to new shapes, closed to modification
abstract class Shape { public abstract float Area(); }
class Circle : Shape { public override float Area() => 3.14f * _r * _r; }
```

The good version is **both** open and closed. The bad version is neither.

**Two tools — picking the right one is the actual skill:**

| Tool | Use when |
|---|---|
| Polymorphism (subclass) | the **behaviour** differs |
| Lookup table (dict/array) | only the **data** differs |

`AdRewardBase` is polymorphism done right — `AdPanelController.cs:37` calls one virtual method and never branches on reward type.

Your two WARN spots are both data, not behaviour. Use tables, not subclasses.

**The sharper test — duplication:**

> If the same branch structure appears in **more than one place**, you have an OCP problem.
> If it appears in exactly **one** place, that place can legitimately be your factory.

Somewhere, something must know the concrete list. You cannot reach zero conditionals. The goal is **exactly one** place that knows, shaped as data so a designer can edit it.

Applied:
- `GameSession.cs:237-252` — chain appears once. Mild. Fix it so adding a zone type is a *data* change.
- `RarityCalculator.cs:50-65` — adding "Mythic" means editing the enum **and** `GameSettings`' three chance fields **and** the chain. Three places. Real violation, and the one the reviewer named.

Not every `if` is a violation. Guard clauses, null checks, boundary tests — fine.

### L — Liskov Substitution

> A subclass must work anywhere its base works, without the caller knowing which one it got.

```csharp
class Bird    { public virtual void Fly() { ... } }
class Penguin : Bird { public override void Fly() => throw new NotSupportedException(); }

foreach (Bird b in birds) b.Fly();   // 💥 subclass broke the base's promise
```

Your variant is quieter — **a base that promises nothing**:

```csharp
protected virtual  void OnButtonClicked() {}   // "you may ignore this"  ← ButtonController.cs:37
protected abstract void OnButtonClicked();     // "you must answer this"
```

With `virtual {}`, a button that forgets to override compiles and silently does nothing. With `abstract`, the compiler catches it. You're not adding safety — you're **moving a bug from runtime to compile time**.

Also: `ZoneViewBase` has no subclasses. "Base" promises a hierarchy that doesn't exist. Rename to `ZoneView`.

**Self-check:** any override that throws, or is empty? Any caller that type-checks before calling?

### I — Interface Segregation

> Don't force a client to depend on members it doesn't use.

```csharp
interface IWorker { void Work(); void Eat(); void Sleep(); }
class Robot : IWorker {
    public void Work()  { ... }
    public void Eat()   => throw new NotSupportedException();  // robots don't eat
    public void Sleep() => throw new NotSupportedException();
}
```

Split into `IWorkable` / `IFeedable`.

**Applies to base classes too, which is your case.** `ButtonController` forces `_button`, `_buttonText`, *and* `_gameSession` onto every button. `SkipButton` has no text and doesn't need the game session — it carries both.

**Rule of thumb:** a one-method interface is usually right. A five-method interface is usually two interfaces.

### D — Dependency Inversion

> Depend on abstractions, not concretions. The *detail* depends on the *abstraction*, never the reverse.

```csharp
// ❌ the class decides what it uses
class OrderService {
    private MySqlDatabase _db = new MySqlDatabase();
}

// ✅ whoever BUILDS it decides
class OrderService {
    private readonly IOrderStore _store;
    public OrderService(IOrderStore store) => _store = store;
}
```

The real shift isn't the interface — it's **who decides**. The constructor-caller is your composition root (`GameInstaller`).

`FindObjectOfType` is Unity's `new MySqlDatabase()`, only worse: a *global search* at a time you don't control. `LosePanelSeeAdButton.cs:21` — `new AdRewardRevive(_gameSession)` — is the textbook form exactly.

**Two-part self-check:**
1. Does this class **go and get** its collaborators, or are they **handed to it**? Getting = violation.
2. Could I hand it a fake in a unit test? No = violation.

That second question is why tests are proof rather than decoration.

### Cheat sheet

| | One line | Your smell |
|---|---|---|
| **S** | One reason to change | 11 `using` statements in one file |
| **O** | Add cases without editing | if-chain on `EZoneType`, if/else on rarity |
| **L** | Subclass keeps the base's promise | `virtual {}` that promises nothing |
| **I** | Don't force unused members | fat `ButtonController` base |
| **D** | Be handed things, don't go get them | 11 × `FindObjectOfType`, `new AdRewardRevive(...)` |

---

## 3. Extension points — what the OCP praise meant

> *"SO tabanlı çark ve zone içeriği ile AdRewardBase polimorfizmi gerçek uzantı noktaları."*

**Extension point** = a place where someone adds new behaviour **without editing existing code**.

**The one-sentence test:** to add a new one of these, do I **create** something, or **edit** something? Create = extension point. Edit = OCP violation.

**✓ ScriptableObject wheel/zone content.** To add a Diamond wheel: right-click → Create → new asset → drag it in. Zero C#, no recompile. `WheelSpin.ApplyStyle` just does `_image.sprite = style.WheelSprite` — it has no idea Gold exists.

The teaching moment: `WheelType.cs:8` has `enum EWheelType { Bronze, Silver, Gold }` — **declared and never referenced anywhere**. It's the fossil of the *closed* version of this design. You replaced it with `.asset` files and the enum became dead weight. Same feature, two designs. Delete it.

**✓ `AdRewardBase` polymorphism.** `AdPanelController.Play(AdRewardBase reward, ...)` then `_pendingReward?.GiveAdReward()`. No `if (type == Revive) ... else if (type == Hint)`. When you added `AdRewardZoneHint` you touched `AdPanelController` zero times. That's polymorphism replacing a conditional.

**✗ Where the same idea failed.** `GameSession.cs:237-252` — your `ZoneTypeData` asset is a perfect extension point, but nothing can *reach* it without editing `GameSession`. **A feature can be extensible in its data and non-extensible in its wiring.**

**Note:** `AdRewardBase` was praised under OCP **and** criticised under DIP. Not a contradiction — it's a genuine extension point that takes a `GameSession` in its constructor. The principles are independent axes, not one score.

---

## 4. The brief — requirements the feedback didn't repeat

Source: *Vertigo Games Game Developer Demo.pdf*

| Requirement | Status |
|---|---|
| Unity 2021 LTS | ✅ 2021.3.45f2 |
| Canvas mode Expand | ✅ `m_ScreenMatchMode: 1` |
| TextMeshPro | ✅ |
| `_value` suffix on changeable elements | ✅ `ui_text_currency_value`, `ui_image_zone_icon_value` |
| Root-general-to-specific naming | ✅ across ~100 objects |
| Wheel prefab naming | ❌ `WheelSpin`/`WheelParent`/`WheelSlot`/`WheelIndicator` have Image components, break the convention |
| RaycastTarget off where unneeded | ⚠️ 19 enabled in prefabs — audit |
| Screen orientation | ❌ `defaultScreenOrientation: 4` (AutoRotation), all four rotations allowed |
| APK as GitHub **Release** | ⚠️ `v1.0` tag exists — verify the APK is actually attached |
| Sliced sprites | verify |
| No animation on root transform | verify `CurrentZonePanelValue.cs:29` `DOPunchScale` |

### CRITICAL — do not delete all OnValidate

The brief **requires** it: *"Button references should be automatically set from OnValidate Editor codes."*

| Keep | Delete |
|---|---|
| `GetComponent<Button>()` | `FindObjectOfType<GameSession>()` |
| `GetComponentInChildren<TextMeshProUGUI>()` | `FindObjectOfType<AdPanelController>()` (×2) |
| `GetComponentInChildren<LosePanelReviveCostText>()` | the 9 `FindObjectOfType<...PanelController>()` in GameSession |

**Rule:** looking **inside my own hierarchy** = required by the brief. Scanning **the whole scene for a stranger** = the DIP violation. Only 11 of the 12 calls are the bad kind.

Also: *"Do not use unity OnClick or event references from Editor."* So `Bind(Action)` in code isn't optional, it's **required**. Dragging *object* references into an installer's `[SerializeField]` is still fine — that prohibition is about UnityEvent OnClick lists.

Also: *"Reusable, maintainable, scalable and easy testable codebase"* is a listed review criterion — so the tests map directly to the brief.

### AI usage

The brief asks for minimal AI use; research and learning are explicitly fine. Core implementation must be yours, and you should be able to defend every line.

---

## 5. Layout diagnosis

Canvas Scaler is Expand at 1920×1080 reference:

| Aspect | Resolution | Scale | Canvas units |
|---|---|---|---|
| 20:9 | 2400×1080 | 1.00 | 2400×1080 |
| 4:3 | 1440×1080 | **0.75** | 1920×1440 |

Everything shrinks 25% at 4:3 and the canvas gains 360 units of height. Elements are pinned to corners at fixed sizes, so **all that extra space becomes gaps**. The 4:3 and 20:9 screenshots are the same layout with more black added — nothing reflows.

**Four changes:**

1. **Group the top bar.** Zone badge `1`, SUPER ZONE, SAFE ZONE and the `500` currency are four separate floating things. One horizontal group anchored top-stretch. Currency currently sits alone mid-right for no reason.
2. **Fix the reward rail.** A tall empty frame eating the entire left edge. Horizontal strip along the bottom, or a compact panel.
3. **Wheel anchored center, sized as a % of canvas height** — not fixed pixels floating in dead space.
4. **Turn on the background.** `ui_group_game_background` exists in prefabs but every screenshot is pure black.

Then re-shoot 20:9, 16:9, 4:3.

### Smoothness

- `UI/PanelController.cs:7-15` — `Show`/`Hide` are raw `SetActive`. **Every panel in the game pops.** This is most of the "akışkanlık" complaint. Tween scale/fade once, applies everywhere.
- Wheel: `Ease.OutCubic` over 3.1–3.5s is flat and long. Reverse kick before launch, `OutQuart`, ~2.5s.
- Death flow: bomb → flash/shake → short pause → panel slides in. Currently instant, reads as a bug.

---

## 6. Out of scope

Not in the feedback, cut for time: domain asmdef sealing (keep the test asmdef, skip the sealing), rarity-as-ScriptableObject, per-wheel odds, zone placement rules, content validation tooling, odds preview inspector, explicit state machine classes.

---

## 7. Reading (from the brief's required list)

- https://refactoring.guru/smells/large-class — the SRP verdict
- https://refactoring.guru/extract-class — technique for splitting GameSession
- https://refactoring.guru/replace-conditional-with-polymorphism — the OCP verdict
- https://refactoring.guru/design-patterns/observer — the panel events inversion

This is the reviewer's vocabulary — the brief put refactoring.guru on the required list. Name these techniques in `ARCHITECTURE.md`.

---

## 8. Things worth surfacing in the submission

- `Editor/ZoneRuleEditor.cs` is genuine designer validation tooling — zone range, duplicate overrides, slot-count mismatch, empty slots, bomb-count vs special zone. The reviewer never mentioned it, which means the submission didn't surface it.
- The UI naming convention is followed across ~100 objects and it's a listed criterion. Point at it.
- `WheelSpinningChanged` and `ExitAvailabilityChanged` show you already had the event-inversion instinct — you just stopped after two. Say so.
- Delete the defensive comments (`RarityCalculator.cs:49`, `GameSession.cs:484`). Reasoning belongs in `ARCHITECTURE.md`, not arguing with a reviewer inline.
