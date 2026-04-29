Prompt Forge Lane Install Session Charter

Purpose

This session is primarily focused on installing and refining lanes in Prompt Forge with a high standard of precision, restraint, and implementation safety.

Why This Matters

Prompt Forge is not just a private local tool. Work done here feeds back into the broader OpenAI developer ecosystem through public-facing community discussion and showcases. The quality bar should reflect that reality.

This project also serves a larger product philosophy:
- reduce unnecessary LLM burden
- improve token economics
- help users arrive with stronger authored prompts instead of forcing models to invent and infer everything at runtime

Standing Session Priorities

- prioritize lane installation and lane-authoring work
- preserve architecture boundaries
- prefer bounded, implementation-safe changes over speculative redesign
- keep semantic authority clean and intentional
- maintain a standard worthy of public technical scrutiny

Working Environment Constraints

The working environment is a Windows 11 machine running Prompt Forge locally.

Operationally important constraints:
- local shell is Windows PowerShell
- Codex is operating with sandbox restrictions
- some system inspection may be permission-limited
- local app/build behavior should be verified with the actual Windows/.NET environment in mind
- avoid assuming unrestricted filesystem or machine-level access

Lane Work Standard

When performing lane installs or lane updates:
- classify the lane or change surface first
- keep the pass isolated to the approved install surface
- preserve unresolved decisions as unresolved unless explicitly approved
- surface holes clearly when they matter
- keep advisory suggestions separate from implementation-safe spec
- do not silently widen a partial lane pass into a full install
- respect the distinction between metadata, phrase authority, prompt assembly, and UI/state wiring

Architecture Reminder

- `LaneRegistry` owns lane shape, selectors, defaults, metadata, and default nudges
- `SliderLanguageCatalog` files own phrase authority
- `PromptBuilderService` owns prompt assembly and route order
- `MainWindowViewModel` owns binding, capture/apply/reset flow, and visible UI state
- implementation should follow the lane insertion checklist in `docs/LANE_INSERTION_CHECKLIST.md`

Quality Bar

Earlier models working on this project established a high standard. Continue to meet that standard:
- precise handoffs
- disciplined scope control
- clean prompt semantics
- careful implementation
- no casual degradation of the lane system

Default Posture

If there is tension between speed and quality, prefer the version that protects Prompt Forge's long-term integrity and public-facing quality.
