# MMORPG Vision and Architecture Notes

## Goal

Build an MMORPG with a visual style inspired by Ragnarok Online and Octopath Traveler: a stylized 2.5D presentation that mixes strong art direction, depth, lighting, and readable RPG gameplay.

This document captures the current working ideas so they can evolve as the project becomes more concrete.

## Product Direction

### Visual target

- Ragnarok Online style strengths:
  - readable world structure
  - map-based MMO design
  - social town and field gameplay
  - clear class/combat presentation
- Octopath Traveler style strengths:
  - dramatic lighting
  - depth and atmosphere
  - layered 2.5D presentation
  - stylized environmental rendering

### Intended hybrid

The target is not pure retro 2D and not full modern realistic 3D.

The intended look is:
- a 2.5D RPG/MMO presentation
- camera-constrained world composition
- stylized depth, lighting, fog, and atmosphere
- either sprite-like characters in a 3D world or low-poly 3D characters with pixel-art-inspired materials

## Main Technical Objective

The project now has two different technical objectives, one for the current phase and one for the later MMO target.

### Current objective

Build a client-side gameplay prototype that can answer:
- does movement feel good?
- does combat feel good?
- does the world presentation feel right?
- does progression feel satisfying?
- is the game loop fun enough to justify a larger MMO effort?

### Future objective

If the prototype proves the concept, choose a stack that is also good for the hard parts of an MMORPG:
- authoritative multiplayer
- persistent progression
- content tooling
- economy and inventory consistency
- live operations
- long-term maintainability

## Current Phase

The project is currently focused on a local, code-first gameplay prototype.

This phase should optimize for:
- fast iteration
- Linux-native development
- mocked/local data
- mechanics and game-feel validation
- minimal editor dependency

This phase should not optimize for:
- real networking
- backend services
- production persistence
- live ops
- MMO-scale architecture

## Current Stack Recommendation

### Recommended default for the prototype phase

- Client/game prototype: MonoGame
- Language: C#
- Workflow: Linux-native development
- Data: mocked local JSON/static data
- Durable backend/datastores: not required for the first prototype phase

### Why this is the current recommendation

- The current goal is to validate gameplay feel, mechanics, readability, and progression before taking on MMO infrastructure burden.
- MonoGame is a strong fit for a code-first workflow with minimal editor dependency.
- C# matches existing experience and keeps iteration comfortable.
- Linux-native development is an explicit project preference for the client prototype.
- Mocked local data is enough to test the game loop without backend complexity.

### Current prototype philosophy

The prototype should optimize for:
- fast iteration
- code-driven implementation
- mocked/local data
- validating whether the game is fun

The prototype should not yet optimize for:
- real networking
- backend services
- persistent MMO infrastructure
- production-grade live operations

### Alternative path for the prototype

- Client/game prototype: Godot 4
- Language: C#

This remains a valid option if the project later needs stronger built-in scene tooling or a true engine-based rendering workflow, but it is no longer the current default.

### Alternative path for the future MMO-capable client

- Client/game engine: Unity
- Backend/services: Go
- Datastores: PostgreSQL + Redis

This path may be better later if the project benefits from Unity's broader ecosystem, tools, or asset marketplace.

### Not the current first choice

- Unreal Engine

Reason: likely heavier than needed for this specific visual target and for early MMO iteration.

## Prototype Client Architecture

The current prototype should be a local, code-first client application built in MonoGame.

### Prototype principles

- everything local
- everything mocked
- no networking
- no server authority yet
- no production persistence requirements
- clean enough structure to evolve, but optimized for speed

### Suggested prototype modules

- Core
  - game bootstrap
  - scene/screen flow
  - timing/input
  - data loading
- World
  - map rendering
  - player controller
  - NPC/enemy spawning
- Combat
  - targeting
  - attacks/skills
  - damage and death
- Progression
  - stats
  - experience
  - level up
  - inventory
- UI
  - HUD
  - dialogue
  - inventory
  - quest tracker
- Data
  - characters
  - enemies
  - items
  - skills
  - quests
  - maps

## Future MMO Architecture Direction

The sections below describe the longer-term MMO architecture direction if the prototype proves the game is fun.

They are not the current implementation target for the prototype phase.

## Core Architecture Principles

### 1. Server authoritative model

The server should own the truth for:
- movement validation
- combat resolution
- item drops
- inventory state changes
- quest progression
- trading/economy actions

The client should be responsible for presentation, responsiveness aids, and local prediction where appropriate, but not for trusted game state.

### 2. Zone-based world architecture

The most natural initial world model is map-based, similar to Ragnarok-style progression.

That means:
- each map or zone is simulated by a dedicated zone process
- changing maps triggers a handoff between zone contexts
- dungeons can be modeled as instances

This is simpler and safer for a first MMO architecture than building a seamless open world.

### 3. Modular backend, not microservices everywhere on day one

Initial recommendation:
- one platform backend deployable
- one realtime gateway deployable
- one zone server deployable type
- one worker/background deployable

Internally, these should be organized into domains/modules rather than split into many independent services too early.

### 4. Separate simulation state from persistent state

Persistent state examples:
- account data
- characters
- inventory
- equipment
- quests
- currency

Simulation state examples:
- current position
- aggro tables
- temporary buffs/debuffs
- current NPC combat state
- short-lived zone event state

### 5. Keep content data-driven

Externalize and version gameplay/content data where possible:
- items
- monsters
- NPC definitions
- skills
- quests
- spawn tables
- maps and warp links

For an MMORPG, content authoring speed becomes a major constraint very early.

## High-Level System Graph

```text
                                +----------------------+
                                |   Web / Launcher     |
                                | patcher, news, auth  |
                                +----------+-----------+
                                           |
                                           v
+----------------+              +----------------------+
| Game Client    |<-----------> | API Gateway / Edge   |
| MonoGame/Unity |  HTTPS/WS    | auth, routing, rate  |
| 2.5D renderer  |              +----+------------+----+
| input, UI, FX  |                   |            |
+-------+--------+                   |            |
        |                            |            |
        | realtime                    |            |
        v                            v            v
+-------------------+         +-----------+   +----------------+
| Realtime Gateway  |<------->| Auth Svc  |   | Account Svc    |
| session mgmt      |         | login JWT |   | profile, bans  |
| connection hub    |         +-----------+   +----------------+
+---------+---------+
          |
          | routes player to zone/map
          v
+--------------------------------------------------------------+
|                  World / Zone Server Cluster                 |
| +------------------+  +------------------+  +--------------+ |
| | Zone Server A    |  | Zone Server B    |  | Dungeon Inst | |
| | map simulation   |  | map simulation   |  | party runs   | |
| | movement/combat  |  | AI/NPC/events    |  | boss logic   | |
| +--------+---------+  +--------+---------+  +------+-------+ |
|          |                     |                     |         |
+----------+---------------------+---------------------+---------+
           |                     |                     |
           v                     v                     v
  +----------------+   +-------------------+   +------------------+
  | Chat / Social  |   | Party/Guild Svc   |   | Matchmaking Svc  |
  | chat channels  |   | groups, guilds    |   | queues/instances |
  +--------+-------+   +---------+---------+   +---------+--------+
           |                     |                       |
           +---------------------+-----------------------+
                                 |
                                 v
                    +-------------------------------+
                    | Game Platform Services        |
                    | inventory, items, quests,     |
                    | economy, mail, trading, logs  |
                    +---------+-------------+-------+
                              |             |
                              v             v
                     +---------------+   +----------------+
                     | PostgreSQL    |   | Redis          |
                     | source of     |   | cache, session,|
                     | truth         |   | transient data |
                     +-------+-------+   +--------+-------+
                             |                    |
                             v                    v
                     +------------------------------------+
                     | Event Bus / Async Workers          |
                     | analytics, notifications, saves,   |
                     | fraud checks, audit/event replay   |
                     +----------------+-------------------+
                                      |
                                      v
                     +------------------------------------+
                     | Observability / Admin / Live Ops   |
                     | metrics, tracing, GM tools,        |
                     | dashboards, support tooling        |
                     +------------------------------------+
```

## Runtime Layers

### Client layer

Responsibilities:
- rendering
- input handling
- UI
- animation and effects
- local interpolation/prediction
- networking client logic

### Edge layer

Responsibilities:
- authentication entrypoint
- API routing
- connection/session entrypoint
- patch/version checks
- rate limiting and edge protections

### Simulation layer

Responsibilities:
- zone/map simulation
- monster AI
- movement resolution
- combat resolution
- event triggers
- dungeon instances
- player visibility/interest management

### Platform layer

Responsibilities:
- account and character management
- inventory/equipment systems
- quests
- party and guild features
- trade/mail/economy systems
- chat and social systems
- moderation/admin actions

### Data and async layer

Responsibilities:
- persistence
- cache/session management
- analytics pipelines
- background processing
- audit/event handling

## Proposed Backend Modules

These are logical modules first. They can remain in a modular monolith for a long time.

### Identity and access

- auth
- account profile
- bans/suspensions
- session management

### Character domain

- character creation
- character selection
- base stats/progression
- saved position
- class/job state

### World simulation

- map/zone lifecycle
- player presence in zone
- movement
- visibility/interest management
- NPC/monster simulation
- zone events

### Combat domain

- skills
- damage/healing resolution
- buffs/debuffs
- cooldowns
- target validation
- death/respawn flow

### Inventory and item domain

- bag inventory
- equipment slots
- stackable items
- loot generation
- item consumption
- transactional item changes

### Quest and NPC domain

- NPC dialogue
- quest definitions
- quest progress state
- rewards
- event triggers

### Social systems

- chat
- parties
- guilds
- friends/presence

### Economy systems

- trade
- mail
- player marketplace or auction features
- gold/currency tracking
- anti-duplication safeguards

### Operations and support

- GM/admin commands
- player support actions
- event toggles
- item grants
- moderation actions

## Data Storage Direction

### PostgreSQL

Use as the primary durable source of truth for:
- accounts
- characters
- inventory
- equipment
- quests
- guilds
- parties metadata when persistence is needed
- mail
- market listings
- audit logs or important event records

### Redis

Use for:
- sessions
- presence
- hot caches
- temporary queues
- rate limiting
- ephemeral coordination data

### Event/worker model

Use background workers for non-realtime side effects such as:
- analytics
- notifications
- audit processing
- asynchronous save pipelines when appropriate
- fraud detection or rule checks

## Networking Direction

### HTTP/HTTPS

Use for:
- login flows
- patch metadata
- account APIs
- launcher/web APIs
- admin/support APIs

### WebSocket or TCP

Use for:
- realtime player movement
- combat events
- chat
- map state updates

Current direction: WebSocket/TCP is a very reasonable starting point for a Ragnarok-like MMO pace.

### UDP

Not the first recommendation.

It should only be introduced if later combat/movement requirements prove that the simpler transport is insufficient.

## Visual/Content Pipeline Direction

The style target is as important as the backend target, but the current prototype should approach it pragmatically.

### Rendering direction for the prototype

Initial rendering direction:
- 2D or pseudo-2.5D presentation
- layered map composition
- readable RPG combat presentation
- code-driven rendering and scene behavior
- stylized atmosphere explored incrementally

The prototype does not need to fully solve the final rendering pipeline yet.
Its job is to validate gameplay and presentation direction with the least friction.

### Longer-term rendering direction

Possible future approach:
- 3D environment/world structure
- camera-constrained presentation
- sprite-based or sprite-like characters
- stylized lighting and fog
- selective bloom/depth effects
- highly readable combat silhouettes

### Content pipeline tools to evaluate

- Aseprite for sprite work
- Blender for environment and asset work
- Tiled or custom map tooling depending on world representation

### Data-driven content assets

Version structured content for:
- items
- NPCs
- monsters
- skills
- quests
- maps
- spawn points
- warp links

## Operational Direction

### Local development

- Docker Compose for local multi-service development

### Early deployment

- simple cloud VM or managed container approach
- avoid overcommitting to Kubernetes too early

### Observability

Recommended baseline:
- Prometheus
- Grafana
- OpenTelemetry
- central logs

## First Playable Vertical Slice

The first slice should be fully local and use mocked data.

### V1 slice target

- one town area
- one field area
- one player character
- login
- movement
- one monster type
- one basic attack
- one skill
- loot drop and pickup
- inventory screen
- exp gain and level up
- NPC interaction
- quest mock
- area transition

If this works and feels good, the project will have validated the core gameplay direction before MMO architecture is introduced.

## Major Risks

### 1. Content production load

The biggest long-term challenge may be creating content fast enough, not just building engine/server code.

### 2. State consistency

Inventory, loot, combat, trade, and economy systems need careful transactional rules.

### 3. Zone handoff complexity

Map transitions and player transfer between simulation contexts must be designed carefully.

### 4. Live ops burden

MMOs require operational tooling earlier than many games do.

### 5. Anti-cheat and trust boundaries

The client should never be treated as a trusted authority for core game state.

## Open Decisions

These questions are still open and will shape the next version of the architecture:

1. Is the world strictly map-based like Ragnarok, or should it feel more seamless?
2. Is combat tab-target/soft-target, or more action-oriented?
3. Is the first target platform desktop only, or desktop plus mobile?
4. Are characters sprite-based in a 3D world, or low-poly 3D with pixel-art-inspired materials?
5. How much of the project should be optimized for solo development versus future team scaling?

## Next Discussion Topics

When continuing this document, the most useful next steps are:

1. choose the world model
2. choose the combat model
3. choose the character rendering approach
4. refine the content pipeline
5. define the first milestone architecture in more concrete technical detail
