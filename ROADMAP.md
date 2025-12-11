# Roadmap: PARANOID:PROTOTYPE [Ecliptica] - MVP & Graduation Demo

**Status:** Draft v0.5 (Phase 2.5 Logic & Phase 3 Core UI DONE)
**Target:** Gameplay-First MVP (Single Boss Encounter)

---

## 📅 Phase 1: Core Systems Foundation (COMPLETE)
**Goal:** ทำให้ระบบพื้นฐานทำงานได้ครบวงจร (Core Loop Playable)

- [x] **Project Structure & Architecture**
    - [x] Folder Structure (Core, Gameplay, Managers, UI)
    - [x] Event Bus System
    - [x] Base Command Pattern (Attack, Defend, Analyze)
- [x] **Character & Unit Foundation**
    - [x] Unit Class (HP, Sanity, Stats)
    - [x] ScriptableObjects for CharacterData
    - [x] Sanity States Effects (Lucid/Strained/Fractured stats calculation)
- [x] **Card System (Infinite Draw)**
    - [x] CardData Structure (ScriptableObject)
    - [x] DeckManager Refactor (Shared Hand from Party Pool)
    - [x] Card Lock mechanic
- [x] **Battle Flow (Turn-Based)**
    - [x] BattleManager State Machine (Setup -> Player -> Execution -> Resolution)
    - [x] Timeline Sorting logic (Phase-based by Agility)
    - [x] Party Control in PlayerTurnState (Cycle units)

## 📅 Phase 2: Gameplay Mechanics & Rules (COMPLETE)
**Goal:** ใส่กติกาและความลึกของเกม (Combos, Field, Boss AI)

- [x] **Combo System**
    - [x] Basic 3-Hit Combos (AAA, DDD, NNN, etc.) logic
    - [x] Combo Resolver to modify command values
- [x] **Field Resonance**
    - [x] FieldManager logic (Logos/Illogic/Nihil dominance counters)
    - [x] Advanced Effects (Logos: +Def/-Crit, Illogic: -Def/+Crit, Nihil: Reset)
- [x] **Boss AI & Patterns**
    - [x] Create Boss Unit with multi-health bars (Phases)
    - [x] Boss AI Controller (Selects pattern by Phase)
    - [x] Telegraph System (Broadcasts Intent Event)
- [x] **Action Execution System**
    - [x] **Reactive Timeline:** (Commands execute sequentially by Speed)
    - [x] **Interrupt System:** (Logic added, Boss Intent declared at start of turn)

## 📅 Phase 3: UI/UX & Visual Feedback (CORE DONE)
**Goal:** ทำให้ผู้เล่นเข้าใจสิ่งที่เกิดขึ้น (Communication)

- [x] **Battle UI Overhaul**
    - [x] Party Member Select / Status HUD (HP, Sanity Bar)
    - [x] **Command Queue UI:** Shows 3 slots per character
    - [ ] **Timeline UI:** Show turn order strip (Low Priority for MVP)
- [x] **Card UI**
    - [x] Card Visuals (Cost, Type, Owner indicator)
    - [x] **Hand UI:** Interaction Logic (Click to play, Lock toggle)
- [x] **Telegraph UI**
    - [x] Icon Display for Boss Intents (Attack, Defend, Ultimate)
- [ ] **Feedback**
    - [ ] Damage Numbers (Popups)
    - [ ] Sanity Break Visual Effects (Glitch/Screen shake)

## 📅 Phase 4: Content & Tuning (NEXT STEP)
**Goal:** เติมเนื้อหาและปรับสมดุล

- [ ] **Content Implementation**
    - [ ] Create Data for 3 Playable Characters (Stats + Skill Cards)
    - [ ] Create 1 Boss Encounter (Stats + Phase Logic)
- [ ] **Balancing**
    - [ ] Tune Damage/Sanity Costs
    - [ ] Adjust Sanity Thresholds bonuses
- [ ] **Polishing**
    - [ ] Sound Effects (SFX) integration
    - [ ] Background Music (BGM) implementation
    - [ ] Placeholder Art replacement with Final Assets
