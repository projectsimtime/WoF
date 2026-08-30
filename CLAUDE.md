# Claude Project Instructions

Before working in this repository, read `AGENTS.md` and `MEMORY.md` completely and treat them as the canonical project guidance and current architectural memory.

In particular:

- Inspect neighboring code before proposing or implementing a pattern.
- Follow the existing Unity `OnValidate()` auto-wiring convention.
- Preserve the event direction and zone architecture recorded in `MEMORY.md`.
- Do not add interfaces, state machines, builders, caches, or pooling systems without a concrete need.
- Keep UI work compliant with the brief-derived hierarchy, TextMeshPro, anchoring, raycast, masking, sliced-sprite, and aspect-ratio rules in `AGENTS.md`.
- Preserve unrelated working-tree and staging changes.
- Update `MEMORY.md` when the user settles an architectural decision so a later session does not reopen it.

If this file and a current user instruction conflict, follow the current user instruction and update the repository memory when appropriate.
