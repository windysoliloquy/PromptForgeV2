# Prompt Forge V5 Licensing Plan

## Goal

Upgrade Prompt Forge from a single portable offline unlock into a dual-mode offline licensing system:

- `MachineBound`
  Normal purchase flow. Unlock works only on the requesting machine.
- `Temporary`
  Trusted-user portable flow. Unlock remains signature-validated but is not tied to one machine.

Both modes should continue to use the existing Prompt Forge private/public keypair model. V5 should extend the signed payload rather than replace the cryptographic foundation.

## Current Baseline

Current signed license payload:

- `ProductName`
- `PurchaserEmail`
- `LicenseId`
- `IssuedUtc`
- `ValidationToken`

Current behavior:

- unlock files are portable
- unlock persistence is plain JSON under `%LocalAppData%`
- demo copy limits are plain JSON under `%LocalAppData%`
- app validates signatures
- app supports `MachineBound` and `Temporary` license modes
- app enforces machine-token matching for `MachineBound` licenses
- signed `AllowedLanes` participate in actual lane access gating
- unlock persistence is still plain JSON and not yet DPAPI-hardened

## V5 Target Schema

Add these fields to `PromptForgeLicense` and the canonical signed payload:

- `LicenseMode`
  Expected values: `MachineBound`, `Temporary`
- `MachineToken`
  Optional string. Required for `MachineBound`, empty for `Temporary`
- `EntitlementProfile`
  Short named bundle such as `Full`, `Commercial`, `Standard`
- `AllowedLanes`
  Optional explicit lane ids for finer-grained control

Add matching persisted fields to `UnlockState` so the app can enforce the granted rights after import.

## Machine-Bound Flow

1. App generates a friendly activation request code from a stable machine token.
2. User emails that request code.
3. Seller enters email plus request code into the generator.
4. Generator emits a signed unlock file with:
   - `LicenseMode=MachineBound`
   - `MachineToken=<signed token>`
   - entitlement data
5. App validates:
   - schema integrity
   - signature
   - mode rules
   - local machine token match

If the token does not match, import fails cleanly.

## Trusted-User Portable Flow

Trusted-user unlocks should use:

- `LicenseMode=Temporary`
- empty `MachineToken`
- signed entitlement data

App validation rules:

- valid signature is still required
- `Temporary` explicitly bypasses machine matching
- `Temporary` must be an explicit signed mode, never an accidental fallback caused by a missing token

## Lane Gating

Lane access should be data-driven from the signed license, not inferred from UI state.

Current implemented rule:

- base/free lane
- OR local durable lane unlock
- OR signed `AllowedLanes` contains the intent name

Current limitation:

- `EntitlementProfile` is persisted and visible, but current lane access is enforced through explicit `AllowedLanes` rather than profile-name inference alone

Recommended first entitlement profiles:

- `Full`
  All standard lanes
- `Commercial`
  Standard lanes plus commercial lanes
- `Temporary`
  Same lane coverage as the granted profile, but portable

Optional future refinement:

- use `AllowedLanes` for one-off grants or per-customer custom access

Do not encode lane access by mutating random slider defaults or checkbox state in the license itself unless that behavior is deliberately modeled as a signed lane policy.

## Compatibility Strategy

Treat current unlock files as legacy portable licenses during the transition.

Migration rule:

- old licenses with the legacy five-field payload remain valid for now as legacy portable unlocks
- new V5 licenses require the richer payload and explicit mode handling

This avoids stranding previously issued unlock files while V5 rolls out.

## Storage Hardening

After mode and entitlement support is working, harden local persistence:

1. Encrypt persisted unlock state with DPAPI.
2. Optionally encrypt or integrity-protect demo state as a second pass.
3. Keep the imported unlock-file deletion as best-effort only; do not treat it as security.

## Implementation Order

1. Extend `PromptForgeLicense`.
2. Extend `UnlockState`.
3. Update `PromptForgeLicenseCodec` canonical payload and validation helpers.
4. Add machine-token generation helper in the app.
5. Update `LicenseService` import rules for `MachineBound` and `Temporary`.
6. Add entitlement evaluation path for commercial lanes.
7. Update `PromptForge.LicenseTool` CLI for new fields and modes.
8. Update the GUI launcher script to issue both modes.
9. Add migration handling for legacy unlocks.
10. Add DPAPI hardening after functional verification passes.

## Generator UX

The v5 generator should collect:

- purchaser email
- license mode
- entitlement profile
- optional request code
- optional custom lane list later

Expected UX rule:

- if mode is `MachineBound`, request code is required
- if mode is `Temporary`, request code is ignored

## Verification Checklist

Before shipping V5 licensing:

1. Generate a `Temporary` unlock and verify it imports on multiple machines.
2. Generate a `MachineBound` unlock and verify it imports only on the target machine.
3. Verify commercial lanes stay locked without the right entitlement.
4. Verify legacy unlock files still import as intended during migration.
5. Verify corrupted request codes fail cleanly.
6. Verify copied local state alone cannot bypass machine mismatch once DPAPI hardening lands.

## Current Status

As of this plan:

- the cryptographic base is already present
- V5 schema fields are implemented in the app and generator
- signed `Temporary` and `MachineBound` modes are implemented
- machine-token validation is implemented for `MachineBound`
- signed `AllowedLanes` are part of current lane-access enforcement
- remaining hardening work is mainly persistence protection and any future profile-to-lane expansion beyond explicit `AllowedLanes`
