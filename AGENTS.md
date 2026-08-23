# WoF Project Working Rules

## Primary Goal

This is a time-limited Vertigo Games revision test. The goal is to submit a stable, understandable demo that directly addresses the team's feedback. Passing the test is more important than demonstrating extra patterns or building a reusable framework.

## Decision Rules

- Treat the Vertigo feedback and demo briefing as the scope. Every material change must have a clear connection to one of those requirements or to a confirmed bug.
- Preserve existing gameplay behavior during refactors. In particular, do not silently change spin behavior, zone selection, reward randomization, death/revive flow, economy, or panel flow.
- Refactor incrementally. Do not rewrite working systems or expand the scope while fixing one feedback item.
- Do not introduce an interface, base class, installer, event bus, service, model/view/controller split, or design pattern merely to appear SOLID.
- Add an abstraction only when the current code has a concrete dependency problem and the abstraction makes the requested change simpler to understand.
- Prefer the smallest production-quality solution that can be completed and tested within the remaining two-day revision window.
- Before making a material architecture or behavior change, explain in plain Turkish what will change, why it is necessary, and which feedback item it solves. Wait for explicit user approval.
- If a proposed refactor changes existing behavior, call out that behavior change before editing. Never hide it as an implementation detail.

## Existing Code Style

- Follow the project's existing naming, formatting, field, and method style.
- Preserve the user's variable names where practical.
- Do not add `sealed`, `[RequireComponent]`, excessive guard clauses, defensive null checks, or other stylistic constructs that are not already characteristic of this demo unless they fix a real problem.
- Avoid large `if` chains, but do not replace a simple condition with unnecessary polymorphism.
- Do not create empty overrides, fake `Base` classes, fat base classes, or interfaces with only speculative value.
- Keep class and abstraction counts low. Each new class must own a clear responsibility currently present in the project.

## Unity Safety

- Preserve serialized scene, prefab, and ScriptableObject references when moving fields or renaming scripts.
- Do not edit scenes, prefabs, UI layout, buttons, or assets unless they are explicitly in scope or a serialized-reference migration is required for the approved refactor.
- When an asset migration is required, state exactly which references will move before editing.
- A successful C# build is not proof that Unity runtime behavior is correct. Also identify the exact Play Mode flows that need smoke testing.
- Never claim that Play Mode or device behavior was tested unless it was actually run.

## Communication

- Speak Turkish unless the user requests another language.
- Explain concepts at the user's current level with one concrete project example.
- Do not write code when the user asks to discuss or learn. Write code only after the user explicitly asks for implementation.
- Lead with the decision and its effect on the Vertigo feedback. Avoid long theoretical explanations.
- If uncertain, stop and discuss the tradeoff instead of adding architecture speculatively.
- Own mistakes directly and correct them without defending the previous choice.

## Scope Protection

- Do not modify `TASKS.md` or `NOTES.md` unless the user explicitly asks.
- Preserve unrelated user changes in the working tree.
- Do not commit, discard, reset, or delete user work unless explicitly requested.

