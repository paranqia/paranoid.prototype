# Roadmap: PARANOID:PROTOTYPE [Ecliptica] - MVP & Graduation Demo

**Status:** Draft v0.1 based on GDD v0.6
**Target:** Gameplay-First MVP (Single Boss Encounter)

---

## 📅 Phase 1: Core Systems Foundation (Current Focus)
**Goal:** ทำให้ระบบพื้นฐานทำงานได้ครบวงจร (Core Loop Playable)

- [x] **Project Structure & Architecture**
    - [x] Folder Structure (Core, Gameplay, Managers, UI)
    - [x] Event Bus System
    - [x] Base Command Pattern (Attack, Defend, Analyze)
- [x] **Character & Unit Foundation**
    - [x] Unit Class (HP, Sanity, Stats)
    - [x] ScriptableObjects for CharacterData
    - [ ] **[TODO]** Implement Sanity States (Lucid/Strained/Fractured) effects
- [x] **Card System (Infinite Draw)**
    - [x] CardData Structure (ScriptableObject)
    - [x] DeckManager Refactor (Shared Hand from Party Pool)
    - [ ] **[TODO]** Implement Card Lock mechanic
- [ ] **Battle Flow (Turn-Based)**
    - [x] BattleManager State Machine (Setup -> Player -> Execution -> Resolution)
    - [ ] **[TODO]** Implement Turn Order logic (TimelineManager based on Agility)
    - [ ] **[TODO]** Support Multi-Unit Party control in PlayerTurnState

## 📅 Phase 2: Gameplay Mechanics & Rules (Next Step)
**Goal:** ใส่กติกาและความลึกของเกม (Combos, Field, Boss AI)

- [ ] **Action Execution System**
    - [ ] **Reactive Timeline:** ทำให้ Action ทำงานทีละตัวตามคิว ไม่ใช่รวดเดียวจบ
    - [ ] **Interrupt System:** สร้างเงื่อนไขการขัดจังหวะ (Stun/Cancel)
- [ ] **Combo System**
    - [ ] Basic 3-Hit Combos (AAA, DDD, NNN, etc.) logic
    - [ ] Combo Resolver to modify command values
- [ ] **Field Resonance**
    - [ ] FieldManager logic (Logos/Illogic/Nihil dominance counters)
    - [ ] Visual Feedback for Field State
- [ ] **Boss AI & Patterns**
    - [ ] Create Boss Unit with multi-health bars (Phases)
    - [ ] Implement Telegraph System (Show intended action)
    - [ ] Script Basic Boss AI (Random/Pattern mix)

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

---

## 📝 Immediate Next Tasks (Priority)

1. **Party Control Logic:** Update `PlayerTurnState` to handle input for multiple characters (switch focus).
2. **Timeline Implementation:** Build the queue sorting logic based on Agility.
3. **Card Locking:** Add the ability to keep cards for next turn.

