# Roadmap: PARANOID:PROTOTYPE [Ecliptica] - MVP & Graduation Demo

**Status:** Draft v0.3 based on GDD v0.6
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

## 📅 Phase 2: Gameplay Mechanics & Rules (IN PROGRESS)
**Goal:** ใส่กติกาและความลึกของเกม (Combos, Field, Boss AI)

- [x] **Combo System**
    - [x] Basic 3-Hit Combos (AAA, DDD, NNN, etc.) logic
    - [x] Combo Resolver to modify command values
- [ ] **Field Resonance**
    - [x] FieldManager logic (Logos/Illogic/Nihil dominance counters)
    - [x] Advanced Effects (Logos: +Def/-Crit, Illogic: -Def/+Crit, Nihil: Reset)
    - [ ] Visual Feedback for Field State
- [ ] **Boss AI & Patterns**
    - [ ] Create Boss Unit with multi-health bars (Phases)
    - [ ] Implement Telegraph System (Show intended action)
    - [ ] Script Basic Boss AI (Random/Pattern mix)
- [ ] **Action Execution System**
    - [x] **Reactive Timeline:** (Basic implementation done - Commands execute sequentially by Speed)
    - [ ] **Interrupt System:** (Logic added in Timeline, needs Trigger implementation via cards/skills)

## 📅 Phase 3: UI/UX & Visual Feedback
**Goal:** ทำให้ผู้เล่นเข้าใจสิ่งที่เกิดขึ้น (Communication)

- [ ] **Battle UI Overhaul**
    - [ ] Party Member Select / Status HUD (HP, Sanity Bar)
    - [ ] **Command Queue UI:** Show 3 slots per character clearly
    - [ ] **Timeline UI:** Show turn order strip
- [ ] **Card UI**
    - [ ] Card Visuals (Cost, Type, Owner indicator)
    - [ ] Drag & Drop / Click to Select interaction
    - [ ] Lock Toggle UI
- [ ] **Feedback**
    - [ ] Damage Numbers (Popups)
    - [ ] Sanity Break Visual Effects (Glitch/Screen shake)

## 📅 Phase 4: Content & Tuning (Pre-Demo)
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
