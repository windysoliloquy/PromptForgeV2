# Prompt Forge - Artist Influence vs. Imitation Doctrine

## Purpose

This document defines Prompt Forge's posture toward artist influence, artist pairing, and style blending.

Prompt Forge may support artist influence, but the intended use is **visual-principle transfer**, not direct imitation. The system should help users apply transferable aspects of an artist's visual thinking to a new subject, inside the selected Prompt Forge lane, without drifting into copied compositions, signature-object recall, famous-work reconstruction, or recognizable protected-expression pull.

This doctrine exists to keep future Prompt Forge work:

- cleaner
- smarter
- more commercially defensible
- more legally cautious
- less dependent on default model behavior
- less vulnerable to cliche artist-name recall

This is a project doctrine note, not legal advice. It should guide future prompt wording, artist-pair behavior, UI copy, documentation, and sales language.

---

## Core Distinction

Prompt Forge distinguishes between **artist influence** and **artist imitation**.

### Artist influence

Artist influence means borrowing transferable visual principles such as:

- lighting behavior
- composition logic
- shape treatment
- color discipline
- texture handling
- symbolic pressure
- edge behavior
- staging tendencies
- realism vs. abstraction balance
- emotional gravity
- subject transformation logic
- surface finish
- spatial organization

These are generalizable visual behaviors. They can be applied to new subjects without attempting to reproduce a specific protected work.

### Artist imitation

Artist imitation means trying to recreate, approximate, or evoke a specific artist's recognizable expression too directly.

Prompt Forge should not encourage:

- copied compositions
- famous-work reconstruction
- signature-object recall
- recognizable scene layouts from known works
- artist-name shorthand that summons famous props
- protected-looking expression pull
- "make it exactly like [artist]" behavior
- default cliche recall attached to a known artist

Prompt Forge's goal is **influence without copycat pull**.

---

## What Prompt Forge Means by Artist Influence

Prompt Forge should treat artist influence as something that is **decomposed before it is operationalized**.

That means an artist reference should not behave like a magic token that pulls a cluster of famous objects from model memory. It should be translated into usable visual principles.

For example, an artist influence may contribute:

- how light falls across the subject
- how forms are simplified or exaggerated
- how the scene is staged
- how symbolic tension is built
- how color is restrained or intensified
- how surfaces are rendered
- how depth is compressed or expanded
- how the subject is transformed without losing identity

The important phrase is:

> **influence through treatment, not substitution**

The artist influence should change how the requested subject is handled. It should not replace the requested subject with artist-associated memorabilia.

---

## What Prompt Forge Is Not Trying to Do

Prompt Forge is not trying to become a copycat engine.

It is not trying to help users recreate a living or historical artist's protected expression. It is not trying to summon the most famous visual artifacts associated with an artist. It is not trying to make the model lazily default to the most recognizable objects from the training cluster.

Prompt Forge should avoid language that encourages:

- direct imitation
- exact style replication
- recognizable work reconstruction
- "in the exact style of" framing
- famous-object insertion
- artist-associated prop dumping
- signature-composition mimicry

A model that sees an artist name and inserts the artist's most famous objects is not demonstrating strong style understanding. It is demonstrating shallow retrieval.

Prompt Forge should be better than that.

---

## Subject, Lane, and Artist Role Priority

Prompt Forge should preserve a clear hierarchy:

1. **Subject integrity**
2. **Lane grammar**
3. **Artist influence through visual principles**
4. **Modifier and blend nuance**

### 1. Subject integrity

The user's subject remains dominant.

If the user asks for a frog, a forge, a product bottle, a courtroom, a spaceship, a dashboard, or an infographic, that subject must remain the thing being depicted.

Artist influence should not hijack the subject.

### 2. Lane grammar

The selected Prompt Forge lane controls the visual grammar of the image.

A Comic Book lane should still behave like comic-book illustration. A Pixel Art lane should still behave like pixel art. A Photography lane should still behave like photography. A Watercolor lane should still behave like watercolor.

Artist influence must pass through the lane, not erase it.

### 3. Artist influence through principles

Artist influence modifies treatment.

It may affect lighting, symbolic structure, mood, staging, surface logic, shape handling, or transformation behavior, but it should not force the image into recognizable artist artifacts.

### 4. Modifier and blend nuance

Modifiers and pairings may refine the output, but they should not override subject integrity or lane grammar.

The system should not become a pile of competing influence tags. Prompt Forge should keep the image legible, directed, and subject-centered.

---

## Style / Medium Lane vs. Artist Influence vs. Subject

Prompt Forge should keep these three surfaces separate.

### Style or medium lane

The style or medium lane answers:

> How should the image behave visually?

It controls things like:

- rendering grammar
- medium behavior
- edge treatment
- texture logic
- composition tendencies
- finish
- genre conventions
- camera or staging assumptions

### Artist influence

Artist influence answers:

> What transferable visual principles should shape the image?

It should contribute principles, not props.

### Subject

The subject answers:

> What is actually being depicted?

This is the anchor. The subject should not be displaced by the artist reference, the lane, or the blend.

Short form:

> **style = how it looks**  
> **artist = how it thinks visually**  
> **subject = what it is**

---

## Artist Pairing / Blend Rule

Artist blending should not mean mixing famous objects from Artist A with famous objects from Artist B.

That produces shallow collage behavior and increases the chance of protected-expression drift.

Instead, artist pairing should assign principle roles.

Example structure:

- Artist A contributes lighting behavior
- Artist B contributes symbolic transformation logic
- the selected lane controls rendering grammar
- the subject remains dominant

Another example:

- Artist A contributes composition and figure staging
- Artist B contributes color discipline and surface texture
- the lane determines the medium behavior
- the output depicts the requested subject, not either artist's famous works

A good artist blend should feel like a governed transfer of visual intelligence.

A bad artist blend feels like artifact soup.

Prompt Forge should prefer governed transfer.

---

## Anti-Drift Terms

Future Prompt Forge docs, prompts, and implementation notes may use these terms when useful.

### Artifact recall

The model pulls famous objects associated with an artist instead of applying visual principles.

Example failure pattern:

- the prompt asks for a new subject
- the model inserts famous artist-associated props
- the subject becomes secondary

### Signature-object drift

The image starts collecting recognizable objects or motifs strongly associated with an artist, even when the user did not ask for them.

### Subject substitution

The requested subject is replaced or overwhelmed by artist-associated material.

### Protected-expression pull

The output begins to resemble a specific known work, composition, or protected-looking expression too closely.

### Principle transfer

The desired behavior: extracting general visual principles and applying them to the user's subject inside the selected lane.

---

## Operational Guidance for Future Prompt Work

When writing prompts, helper text, artist-pair logic, or user-facing explanations, prefer language that describes visual principles.

Prefer:

- dramatic chiaroscuro
- compressed theatrical staging
- delicate botanical surface handling
- symbolic spatial distortion
- crisp illusionistic rendering
- restrained tonal architecture
- luminous focal hierarchy
- graphic contour discipline
- dreamlike object transformation applied to the subject
- warm shadow mass
- angular compositional pressure
- intimate figure emphasis

Avoid relying on:

- exact artist-style commands
- famous object names
- known-work references
- recognizable composition descriptions
- artist-associated prop clusters
- instructions that tell the model to imitate a work

If an artist reference is used, it should be interpreted as:

> Translate the artist's visual principles into treatment instructions for the requested subject.

Not:

> Retrieve the artist's most recognizable images and imitate them.

---

## Prompt Wording Pattern

When needed, Prompt Forge can use a protective wording pattern like:

```text
Apply artist influence through transferable visual principles only: composition, light, form, texture, color, symbolic structure, and subject treatment. Preserve the user's subject and selected lane. Do not summon famous objects, copied compositions, recognizable works, branded characters, or direct imitation artifacts.
```

For artist pairs:

```text
Artist A contributes [principle role]. Artist B contributes [principle role]. The selected lane controls rendering grammar. The subject remains dominant. Avoid signature-object recall, famous-work reconstruction, and copied composition.
```

This structure keeps the artist influence useful without letting it become the main subject.

---

## Short Disclaimer Block

Prompt Forge may support artist influence as a way to guide visual principles such as composition, lighting, form, color, texture, symbolic treatment, and staging. Artist influence is intended to shape how a new subject is handled, not to recreate specific works, copy recognizable compositions, summon signature objects, or imitate protected expression. The selected lane and the user's subject remain the primary anchors.

---

## Public-Facing Short Version

Prompt Forge treats artist influence as visual-principle transfer, not copycat prompting. The goal is to steer lighting, composition, form, color, texture, and symbolic treatment while preserving the user's subject and selected lane.

---

## Internal Short Version

Do not let artist names behave like artifact magnets.

Decompose influence into visual principles, apply those principles to the requested subject, preserve the lane, and suppress signature-object drift.

---

## Non-Goals

This doctrine does not add enforcement code by itself.

It does not change:

- runtime prompt assembly
- lane registry structure
- artist-pair UI behavior
- compression logic
- selector behavior
- modifier behavior
- licensing behavior
- legal terms

It is a doctrine note for future work.

Implementation should only happen through separately approved, bounded passes.

---

## Future Implementation Note (Non-Binding)

Future Prompt Forge work may benefit from an artist-influence translation layer that maps artist references into principle roles such as lighting, composition, form handling, surface treatment, symbolic logic, or transformation behavior.

That should be designed as a separate pass. It should not be added casually to existing prompt assembly or lane logic.

---

## Short Rule

**Artist influence should change the treatment of the subject, not replace the subject with the artist's artifacts.**

Or more bluntly:

**Influence is not imitation.**
