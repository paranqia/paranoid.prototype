# PARANOID:PROTOTYPE[Ecliptica]

---

**GDD Skeleton v.05 (for MVP + Prototype)**

Owner: Team of 3 (Design / Code / Art+Audio) | Engine: Unity | Status: **v0.5 (Updated)**

---

## **0) Game Design Document Status**

| **Field**     | **Value**                                                                                                                         |
| ------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Version       | **v0.5 — 08 Nov 2025** **[อัปเดต v0.5]**                                                                                          |
| Scope         | Graduation demo; gameplay-first MVP                                                                                               |
| Audience      | Internal team + potential contributors                                                                                            |
| Out of scope  | Gacha, meta-prog, full narrative, multi-boss fights                                                                               |
| **Changelog** | **v0.5: อัปเดต Core Loop (Section 2), Art Direction (Section 16.5), UX/UI (Section 15), และ Technical Architecture (Section 16)** |

---

## **1) Vision & High Concept**

**High Concept.** เกมต่อสู้แบบผลัดกันเล่น _Turn-based Party vs Boss_ มีระบบการ์ดเป็นตัวเสริม ผู้เล่นวางแผนล่วงหน้า 3 คำสั่งต่อตัวละคร/รอบ (Attack / Defend / Analysis + Cards ใช้ค่าสติ Sanity) หัวใจของเกมคือ _จับจังหวะ อ่านแพทเทิร์น และประสานงานในปาร์ตี้_ เพื่อข้ามเฟสบอสที่กดดันขึ้นเรื่อยๆ。

**Player Experience Goals.**

- _อ่านเกมเก่งขึ้นทุกครั้งที่สู้ซ้ำ_ (learning boss patterns)
- _รู้สึกฉลาด_ เมื่อจัดคอมโบ 3-ช่องและการ์ดซินเนอร์จีได้พอดี
- กดดันแต่ยุติธรรม ด้วย telegraph และช่องทางตอบโต้ที่ชัดเจน

**เน้นย้ำ:** ตัดฟีเจอร์ที่ไม่จำเป็นทั้งหมดออกไป เพื่อให้สามารถปล่อยเกมเวอร์ชันที่ใช้งานได้จริงได้เร็วที่สุด.

**Target.** PC · Single Player · 1 boss encounter polished.

---

## 2) Core Loop & Core Pillars

### Planning Phase

- เมื่อเริ่มเกมจะได้รับค่า **Sanity** เริ่มต้นที่ 100 หน่วยต่อหนึ่งตัวละคร
- **(เหมือนเดิม)** ผู้เล่นสามารถ ล็อก (Lock) การ์ดบนมือ 0-N ใบ
- Card cost:** baseCost 10–30 (ขึ้นกับชนิดการ์ด) → **effective ต่อยูนิต** ≈ 8–40 (ดู §5.1)
- **[อัปเดต UI/UX]** ผู้เล่นจะ "โฟกัส" (Focus) การสั่งการทีละตัวละคร (ดู §15)
- สำหรับพันธมิตรแต่ละคน (ใน MVP คือ 2-3 ตัวละคร) ให้จัดเรียง (Queue) คำสั่งล่วงหน้าได้สูงสุด 3 คำสั่ง โดยเลือกจาก:
    - Attack / Defend / Analysis
    - Card (หากมี Sanity เพียงพอ)
- ผู้เล่นสามารถ ล็อก (Lock) การ์ดบนมือ 0-N ใบ เพื่อเก็บไว้ใช้ใน Round ถัดไปได้

### Action Phase

- **[กฎใหม่ v0.5]** `TurnManager` จะ "ไม่" เรียงลำดับ 9 Actions
- **[กฎใหม่ v0.5]** ลำดับเทิร์น (Turn Order) จะเรียงตาม **"ตัวละคร" (Actor)** (Player 1, 2, 3 และ Boss) โดยอิงค่าความว่องไว (Agility/SPD)
- **[กฎใหม่ v0.5]** เมื่อถึงตาของ Actor คนนั้นๆ (เช่น Player 1) Actor จะดำเนินการ 3 Actions (A1/A2/A3) ที่วางแผนไว้ **"รวดเดียว" (Execute Sequentially at Once)**
- **[กฎใหม่ v0.5]** (สำหรับบอส) Boss/Enemy จะมี Pattern และจะดำเนินการ 3 Actions (A1/A2/A3) "รวดเดียว" เมื่อถึงตาของบอส
- ช่วงจบรอบ (Round End) - แสดงผลเอฟเฟกต์สิ้นสุดรอบ, จั่วการ์ดจนเต็มมือ, จัดการสถานะการ์ดที่ล็อกไว้, และเปลี่ยนเฟสของบอสหากเข้าเงื่อนไข

---

## 3) Encounter Format

- **Party Size**: 2–3 ตัว (MVP ตั้งเป้าที่ 3 ตัวละคร หากเป็นไปได้; แผนสำรองคือ 2 ตัวละคร)
- **Enemy:** Single **Boss** กับหลอด HP 2–3 หลอด”Phase”
- **Boss Mechanics:**
    - Phase Thresholds (HP bars changes)
    - Debuffs / Statuses
    - Boost SPD/Agility on Phase 1 - 2
    - มีรูปแบบการโจมตีที่แสดงท่าทีล่วงหน้า (Telegraph) และอาจมีรูปแบบพิเศษปะปนมา
    - Ultimate

---

## 4) Actions & Combos (MVP)

### 4.1 Actions

- โจมตี (Attack): สร้างความเสียหาย (อิงตามค่า **Power**) อาจมีการผนวกธาตุตามชุดสกิลของตัวละครหรือการ์ด
- ป้องกัน (Defend): ได้รับเกราะ (อิงตามค่า **Durability**), ล้างดีบัฟเล็กน้อยเมื่อทำคอมโบถึงจำนวนที่กำหนด, อาจใช้เซ็ตอัปการสวนกลับ (Counter) ได้ (เป็นช่องสำหรับพัฒนาระบบคลาสในอนาคต)
- วิเคราะห์ (Analysis): ฟื้นฟู **Sanity** (ทรัพยากรสำหรับใช้การ์ด) และจะติดสถานะ สแกน (Scan) ชั่วคราว (เพิ่มค่าความแม่นยำ/คริติคอลเล็กน้อย; รอปรับสมดุล)
- การ์ด (Card): เล่นจากบนมือ; ใช้ค่าสติ; เอฟเฟกต์ขึ้นอยู่กับประเภทการ์ดและข้อความสกิลเฉพาะของตัวละคร

### 4.2 3-Moves Combos (per char)

(per char) ให้ช่องคำสั่งทั้ง 3 เป็น A1→A2→A3 แนวคิดพื้นฐานคือตัวคูณที่เรียบง่ายและเข้าใจได้ (ปรับเปลี่ยนในอนาคต)

- AAA (โจมตีล้วน): การโจมตีครั้งสุดท้ายแรงขึ้น +18%
- AAD / ADA / DAA (รุกพร้อมรับ): เกราะที่ได้จากคำสั่งป้องกันในชุดคอมโบนี้เพิ่มขึ้น +8%
- AAN / ANA / NAA (ผสานจิต): คำสั่งวิเคราะห์ในชุดคอมโบนี้จะฟื้นฟูค่าสติเพิ่มขึ้น +24%
- DDD (ตั้งรับเต็มที่): ได้รับความเสียหายลดลง 30% จนกว่าจะถึงตารอบถัดไปของตัวเอง
- NNN (วิเคราะห์ลึก): การวิเคราะห์ครั้งที่ 3 ฟื้นฟูค่าสติ +20% (ครั้งก่อนหน้าจะได้น้อยลง)
- ADN / DAN / NAD (สมดุล): ได้โบนัสเล็กน้อย +4% ความเสียหาย, +4% เกราะ และ +8% ค่าสติ
- การ์ดที่ใส่ในช่องจะนับเป็น C (Card); ตารางคอมโบพื้นฐานจะถือว่า C เป็นกลาง ยกเว้นการ์ดใบนั้นจะมีเงื่อนไขระบุว่า "หากใช้ตามหลังคำสั่งโจมตี..." เป็นต้น (สำหรับ MVP ให้ตารางคอมโบมีขนาดเล็กไว้ก่อน แล้วค่อยขยายตามคลาสย่อยในอนาคต)

---

## 5) Card System (MVP)

- **Deck Size:** 6 card 5 type per char
- **Achertype:** 5 type
    - Assault — Deal Damage / Massive Damage / DPS(Damage per second)
    - Aegis — Defend / Shield / Target
    - Benediction — Supporter / Buff / Healing
    - Manipulation — Crowd Control / Debuff
    - Eldritch — Sanity Mechanic / True Damage(Ignore DEF)
- **Draw:** เริ่มเกมด้วยการ์ด 5 ใบบนมือ; จั่วการ์ดให้เต็ม 5 ใบทุกต้นรอบ
- **Lock:** ผู้เล่นสามารถล็อกการ์ดที่เลือกบนมือเพื่อเก็บไว้ใช้รอบถัดไป (การ์ดที่ล็อกไว้จะไม่ถูกแทนที่ด้วยการ์ดใหม่)
- **Sanity**: การ์ดทุกใบมีค่าร่ายเป็นค่าสติ; สมารถใช้คำสั่ง Analysis เพื่อฟื้นฟู Sanity
- **Character-specific skill text:** การ์ดประเภทเดียวกันอาจทำงานต่างกันในแต่ละตัวละคร (มีเงื่อนไข/ความสามารถเฉพาะตัว)
- **Synergy:** การ์ดบางใบอ้างอิงถึงบริบทของคำสั่ง ("หากใช้การ์ดนี้ตามหลังคำสั่งป้องกัน จะเกิดผล X") หรือแท็กของปาร์ตี้ ("หากพันธมิตรมีธาตุเดียวกัน จะเกิดผล Y")

### 5.1 Card System — Infinite Draw Model

- **Skill Pool (Per Character):** ตัวละครแต่ละตัวมี Skill Set = 6 ใบ (Archetype Skills) ซึ่งถือเป็นการ์ดถาวรประจำตัวละคร - การ์ดอาจซ้ำ Archetype กันได้ (เช่น Assault 3 ใบ, Aegis 2 ใบ ฯลฯ) - แต่เมื่อสุ่มขึ้นมาใน **Shared Hand** จะไม่ปรากฏการ์ดชนิดเดียวกันซ้ำภายในมือเดียวกัน
- **Shared Hand (Per Team):**
    - ในการต่อสู้ ผู้เล่นจะมี **มือกลาง (Shared Hand) ขนาด 5 ใบ**
    - การ์ดในมือถูกสุ่มมาจาก _Skill Pool ของทุกตัวละครในทีม_
    - เมื่อใช้การ์ดเสร็จหรือจบรอบ จะทำการสุ่มขึ้นมาใหม่จนเต็ม 5 ใบ
    - ไม่มีการ “หมดเด็ค” → การสุ่มเป็น **Infinite Draw** จาก Skill Pool ที่ยึดติดกับตัวละคร
- **Randomization & Weighting:**
    - โดยพื้นฐาน ทุกการ์ดมีโอกาสถูกสุ่มแบบเท่ากัน
    - อนาคตอาจมี **Modifier** เช่น เพิ่มโอกาสการ์ด Archetype บางประเภทปรากฏตามค่า **Anomaly Parameter** หรือสถานะของปาร์ตี้
- **Sanity Cost:**
    - การ์ดทุกใบมีค่าร่ายเป็น **Sanity ของตัวละครเจ้าของการ์ด**
    - ค่าร่ายถูกกำหนดจาก **ตัวละคร + ประเภทการ์ด + Anomaly Parameter**
    - หาก Sanity ไม่เพียงพอ จะไม่สามารถใช้การ์ดได้
- **Lock Mechanic:**
    - ผู้เล่นสามารถ **ล็อกการ์ดได้สูงสุด 5 ใบ**
    - การ์ดที่ล็อกจะคงอยู่ในมือและกิน Slot → ทำให้รอบนั้นมีการ์ดเล่นน้อยลง
    - การ์ดที่ไม่ถูกล็อกจะถูกแทนที่ด้วยการสุ่มใบใหม่ตามปกติ (_แนวคิดทางเลือก:_ การ์ดที่ล็อกอาจเสียสิทธิ์เล่นในเทิร์นนั้น แต่เก็บไว้ใช้รอบถัดไป)
- **Synergy & Contextual Effects:**
    - การ์ดบางใบมีเอฟเฟกต์พิเศษขึ้นอยู่กับบริบท เช่น
    - “หากใช้ตามหลัง Defend → ได้ผล X”
    - “หากในปาร์ตี้มีธาตุเดียวกัน → ได้ผล Y”
    - การ์ดถือเป็น **กลาง (Neutral C) ใน 3-slot Combo** เว้นแต่จะระบุเงื่อนไขเฉพาะ
- **Key Difference:** ระบบนี้ไม่ใช่ Deck-builder (แบบการ์ดหมดและ reshuffle) แต่เป็น **Dynamic Skill Randomizer** ที่เน้นการจัดการ “หน้าต่างโอกาส (5 ใบ)” และการตัดสินใจใช้ทรัพยากร (Sanity)

**Note:**

1. น้ำหนักการสุ่ม (Weighting): จะใช้สัดส่วนเท่ากันจริง ๆ หรือมี Archetype บางประเภทที่สุ่มง่ายกว่าเสมอ
2. UI/UX: ต้องแสดงให้ผู้เล่นรู้ว่า “การ์ดนี้เป็นของตัวละครไหน” โดยไม่ทำให้รก
3. Synergy Layer: จะเชื่อม Combo 3-Slot Queue เข้ากับ Card Contextual Effect แบบไหน
4. Lock Mechanic Final: จะเลือกแบบ “เสียสิทธิ์เล่นทันที” หรือ “ล็อกฟรีแต่เสี่ยงเสีย Slot”

---

## **6) Resources & Status**

- **HP / Sheild:** แบบมาตรฐาน ค่า Durability ส่งผลต่อปริมาณเกราะและอัตราการลดทอนความเสียหาย
- **Sanity (0–100 per char):** สกุลเงินสำหรับใช้การ์ด; ฟื้นฟูได้ด้วยคำสั่งวิเคราะห์และเอฟเฟกต์ต่างๆ
- **Debuff / Status:** เลือดออก, พิษ, อ่อนแอ, เปราะบาง, เชื่องช้า ฯลฯ ใน MVP จะมีรายการสถานะที่คัดมาแล้วจำนวนไม่มาก
- **Element Adventage:** ระบบสามเส้า Nihil / Logos / Anomaly ส่งผลต่อตัวคูณความเสียหาย

---

## 7) Parameters (Stats)

### 7.1 รายการ Parameters

|**Parameters**|**Description**|
|---|---|
|Power|ค่าพื้นฐานสำหรับคำนวณพลังโจมตี|
|Durability|ค่าพื้นฐานสำหรับคำนวณพลังป้องกันและเกราะ|
|Agility|กำหนดลำดับการเล่น และการตอบสนองต่อเหตุการณ์แทรกในไทม์ไลน์|
|Technique|ความแม่นยำ / โอกาสติดสถานะ|
|Anomaly|ประสิทธิภาพการใช้ค่าสติ / การขยายผลของการ์ดพลังผิดปกติ|
|Luck|โอกาสคริติคอล / ความแรงคริติคอล, โอกาสเกิดเหตุการณ์หายาก, โอกาสติดเอฟเฟกต์พิเศษของการ์ด|

### 7.2 สเกลค่าพลังพื้นฐาน (เปลี่ยนแปลงในอนาคต):

- Agility (SPD):
    - ทั่วไป: 95–130
    - ขีดจำกัดโดยประมาณ (Soft cap): 150 (ขีดจำกัดสูงสุดที่ 160+)
    - ลำดับเทิร์นเรียงตาม SPD; ค่าที่ต่างกันมีผลชัดเจน (เช่น SPD +20 จะทำให้ได้ออกตัวก่อนบอสที่มี SPD 110 แน่นอน)
- Power (ATK):
    - พื้นฐานทั่วไป: 800–1,200
    - สายโจมตีระดับสูง S: 1,300–1,600
- Durability (DEF):
    - พื้นฐานทั่วไป: 400–700
    - สายแทงก์ระดับสูง S: 800–1,000
- Technique (ACC/Status):
    - ความแม่นยำพื้นฐาน 95%; ค่า Technique จะบวกเพิ่มให้กับ Hit% และ Ailment% (เช่น Technique +10 → Hit +3%, Ailment +4%; ค่าคงที่จะปรับภายหลัง)
- Anomaly (ประสิทธิภาพค่าสติ):
    - ค่าสติที่ได้จากการวิเคราะห์พื้นฐาน = 20
    - ค่าที่ได้จริง = 20 × (1 + สัมประสิทธิ์ Anomaly); เริ่มต้นด้วย สัมประสิทธิ์ Anomaly Anomaly / 100 (เช่น 50 Anomaly → ได้รับเพิ่ม +50%)
    - การขยายผลของการ์ดประเภทอีเธอร์ริก: 1 + (Anomaly × 0.4%) มีเพดานจำกัดใน MVP
- Luck (Crit / Rare): โอกาสคริติคอลพื้นฐาน 5%, ความแรงคริติคอล 150% - ทุกๆ 10 Luck → +2% โอกาสคริติคอล และ +4% ความแรงคริติคอล (ค่าพื้นฐานใน MVP) - ค่า Luck จะถูกบวกเพิ่มเข้าไปในโอกาสการเกิดเหตุการณ์หายาก (เช่น 1-5%)

### 7.3 ระดับขั้น (Tier Labels)E/D/C/B/A/S/EX

คือช่วงค่าสถานะเริ่มต้น ไม่ใช่ขีดจำกัดถาวร

- ตัวอย่างการเทียบค่าใน MVP (ไม่ขึ้นกับเลเวล):
    - Power: E=600, D=700, C=800, B=900, A=1,050, S=1,300, EX=1,550 (±5–10%)
    - Agility: E=90, D=95, C=100, B=110, A=120, S=130, EX=145
- ใช้บันไดค่าพลังที่คล้ายกันสำหรับ Durability และค่าอื่นๆ โดยปรับตามบทบาทของตัวละคร

---

## **8) Element**

### **Element Triangle:**

- **Nihil** beat **Logos**
- **Logos** beat **Illogic**
- **Illogic** beat **Nihil**

### **Multipilers:**

- ได้เปรียบ: ×1.20
- เสียเปรียบ: ×0.80
- ปกติ: ×1.00

### **Flavor Anchor:**

- **Nihil (ว่างเปล่า):**
    - **Nihilism** (ภาวะอนัตตา): การปฏิเสธความหมาย, คุณค่า, และความจริงสูงสุดของชีวิตและการมีอยู่. ทุกสิ่งไร้ความหมายและจะกลับสู่ความว่างเปล่าในที่สุด.
- **Logos (กฏ):**
    - **Rationalism** (เหตุผลนิยม) และ **Order:** การเชื่อในเหตุผล, ตรรกะ, ความสมเหตุสมผล, และโครงสร้างที่แน่นอน.
- **Illogic (อตรรกะ):**
    - **Surrealism** และ **Absurdism:** การปฏิเสธเหตุผลและตรรกะทั่วไป. การยอมรับความไม่สมเหตุสมผล.

---

## 9) Class

### Core Class:

- Warlord
    - Warlord เป็น Class ที่สร้างความเสียหายทางกายภาพ (Physical Damage) ในระดับสูง โดยอาศัยพลังกายภาพ เน้นการสร้างดาเมจแบบ "ยืนหยัด" มากกว่าดาเมจฉาบฉวย.
- Magister
    - Magister บทบาทหลักคือการสนับสนุนทีม Buff / Sanity Boost / Sanity Reduce / Debuffer / Crowd Control(CC)
- Aberrant
    - Aberrant ทำความเสียหาย True Damage แบบ Ignore DEF, Concept ความเสียหายมาจากพลังงาน **nullified** เป็นเหมือนเวทมนตร์ในรูปแบบที่ไม่บริสุทธิ์

### Subclass:

- **Formula:** Subclass = Class + Element + Parameters
    - Juggernaut = Warlord + Logos + Power
    - Sentinel = Warlord + Logos + Technique
    - Void Reaver = Warlord + Illogic+ Anomaly
    - ซับคลาสอื่นๆ

---

## 10) การคำนวณความเสียหายและการป้องกัน (ฉบับ MVP)

### 10.1 สูตรคำนวณความเสียหายหลัก (v2)

- Final Damage = [(Base ATK × SkillMult) - (Target DEF / (Target DEF + K))] × ElementMult × CritMult
- ATK มาจากค่า Power (ดูตารางใน 7.2)
- K คือค่าคงที่สำหรับปรับสมดุล (ใน MVP เริ่มที่ K=500)
- ElementMult ตามที่ระบุใน §8
- Crit: โอกาส/ความแรงมาจากค่า Luck (7.2)

### 10.2 ปฏิสัมพันธ์กับการ์ด

- CardMult จะถูกนำไปรวมกับ SkillMult ของคำสั่งนั้น
- Anomaly จะเพิ่มการฟื้นฟูหรือ Reduce Cost ค่าสติและผลของการ์ดอีเธอร์ริก; จะไม่นำไปหารกับค่าความเสียหาย (เพื่อไม่ให้เป็นการลงโทษบิลด์ที่เน้น Anomaly)
- หากต้องการใช้แนวคิดเดิมที่ให้ Power / Anomaly มีผลกับการ์ด Overclock:
    - ให้ใช้สูตรแบบมีเงื่อนไข เช่น OverclockMult = 1 + (Power / max(Anomaly, 25)) × 0.05 โดยมีเพดานจำกัด เพื่อป้องกันค่าที่สูงเกินไป

### 10.3 การป้องกัน / เกราะ

- ShieldGain = Base × (DurabilityScale) × (1 + โบนัสจากคอมโบ/ประเภท)
- การลดทอนความเสียหายใช้สูตร DEF เดียวกัน; เกราะจะลดก่อน HP

---

## 11) Boss Fight :

อยู่ระหว่างการเลือก
- Tempus Paradoxum (Shirogane Naoto) :
    - Void Destruction / Magister + Aberrant / Illogic
- The Singurlarity (Kiragasa Ren) :
    - [Placeholder] / Aberrant / Nihil
- Katastrophē Anthrōpou (Kirasagi Ryoma) :
    - Imperator / Warlord / Illogic + Nihil
- The Maestro (???) :
    - [Placeholder] / Magister / Nihil
- Kuroi Taiyō Lysendros (Lysendros Heliodor) :
    - Solar Eater / Warlord / Logos + Nihil

---

## 12) ตัวละครที่เล่นได้ใน MVP

### Main Character — User

- **[???] ─ The Horizon**

### Playable Character — 3 char

อยู่ระหว่างการเลือก

### Chapter: Void’s Destruction

- Shirogane Naoto ─ Humanity’s Paradox
    - Class : Magister
    - Subclass : The Inquisitor
    - Element : Logos
- Tsukishiro Genma — Ozymandias
    - Class : Warlord
    - Subclass : Juggernaut
    - Element : Logos
- Kirasagi Ryoma ─ Katastrophē Anthrōpou
    - Class : Warlord
    - Subclass : Imperator
    - Element : Illogic

### Chapter: Blacksun Chronicles

- Lysendros Heliodor — The Sunbringer
    - Class : Warlord
    - Subclass : The Sentinel
    - Element : Logos
- Kuroi Taiyō Lysendros — Lysendros Heliodor{UTSURO}
    - Class : Warlord
    - Subclass : False Saviour
    - Element : Nihil
- Silvia Slythene ─ Silver Edios
    - Class : Aberrant
    - Subclass : Etherialist
    - Element : Illogic
- Lilith — Silvia Slythene{UTSURO}
    - Class : Aberrant
    - Subclass : The Veil Witch
    - Element : Nihil + Illogic
- Corvin Grayson — The Ebon Aegis
    - Class : Warlord
    - Subclass : Juggernaut
    - Element : Logos
- The Void Aegis — Corvin Grayson{UTSURO}
    - Class : Warlord
    - Subclass : Slasher
    - Element : Nihil

---

## 13) Card Archetype (MVP)

- **คงประเภทการ์ดไว้ที่ 5 ประเภท ตัวอย่างเช่น:**
    - **Assault:** สร้างความเสียหายโดยตรง, ผนวกธาตุ, มีผลต่อเนื่องหากใช้ตามหลังคำสั่งโจมตี
    - **Aegis:** สร้างเกราะ, ล้างดีบัฟเล็กน้อย, สวนกลับเบาๆ หากใช้ตามหลังคำสั่งป้องกัน
    - **Benediction:** บัฟ ATK/DEF/crit/spd, Healing
    - **Manipulation:** สร้างสถานะ อ่อนแอ/เปราะบาง/เชื่องช้า/สตั้น; ตรวจสอบด้วยค่า Technique
    - **Eldritch:** เกี่ยวข้องกับค่าสติ, มีปฏิสัมพันธ์กับค่า Anomaly, เอฟเฟกต์พิเศษที่ต้องแลกมา
- ตัวละครแต่ละตัวจะมีข้อความสกิลที่แตกต่างกันในการ์ดประเภทเดียวกัน (เช่น การ์ดโจมตีของ Striker อาจโจมตีเป็นหมู่, แต่ของ Caster อาจสร้างสถานะผิดปกติ)

---

## 14) กฎการจั่ว/ล็อกการ์ด

- เริ่มการต่อสู้: จั่ว 5 ใบ
- เริ่มรอบ: จั่วจนเต็มมือ 5 ใบ (การ์ดที่ล็อกไว้จะคงอยู่และนับเป็นส่วนหนึ่งของมือ)
- การ์ดบนมือสูงสุด: 5 ใบ; เมื่อเด็คหมดจะนำกองทิ้งมาสับใหม่
- กองทิ้ง: การ์ดที่ใช้แล้วจะไปที่กองทิ้ง ยกเว้นการ์ดที่ระบุว่าให้ "เก็บไว้บนมือ"

---

## 15) UX / UI (Draft)

- **[v0.5] Style:** `Minimalist` (เรียบง่าย)
- **[v0.5] Core Mechanic (Contextual UI):**
    - "แผงวางแผน 3-Slot" และ "แถบการ์ดบนมือ 5 ใบ" จะถูก **"ซ่อน" (Hidden)** เป็นค่าเริ่มต้น
    - UI นี้จะ "ปรากฏ" (Expand) ก็ต่อเมื่อผู้เล่น "กด" (Press) ที่ `2D Bust-up Art` (ภาพครึ่งตัว) ของตัวละคร (ที่มุมขวาล่าง)

- **[v0.5] Focus System (การแสดงผล):**
    
    - **Planning Phase:** หน้าจอจะ "โชว์เดี่ยวๆ" (Focus) โดยแสดง `Sprite` ตัวละคร ที่ Active เพียงคนเดียว
    - ผู้เล่น "สลับ" (Switch) ตัวละครผ่าน `Icon` (มุมซ้ายล่าง) ทำให้ `Sprite` "สไลด์เข้า/ออก" (Slide In/Out)
    - **Action Phase:** ใช้ "ภาษา" (Language) เดียวกัน `Sprite` ของ Actor (Player หรือ Boss) จะ "สไลด์เข้า" -> "เล่น 3 Action รวดเดียว" -> "สไลด์ออก"

- **[v0.5] Glitch Effect:**
    - `Glitch` จะถูกใช้แบบ "แฝงไว้" (Subtle) (ไม่ใช่ธีมหลัก)
    - จะ "ผูก" (Trigger) กับ "เหตุการณ์" ในเกม เช่น Boss `Telegraph` หรือการใช้การ์ด `Eldritch`
- **(เหมือนเดิม)** ไทม์ไลน์: แสดงลำดับเทิร์น (ตาม Actor); มีสัญลักษณ์เตือนท่าของบอส

---

## 16) ด้านเทคนิคและข้อมูล (Unity)

### ScriptableObjects

- CharacterData: ระดับขั้นพื้นฐาน (E–EX), ธาตุ, คลาส, พาสซีฟเฉพาะตัว, ค่าสถานะ
- CardData: ประเภท, ค่าร่าย, อ้างอิงสคริปต์เอฟเฟกต์, แท็ก (สำหรับคอมโบ), ระดับความหายาก (ตัวสำรอง)
- SkillData: ตัวคูณ, กฎการกำหนดเป้าหมาย, แท็ก
- StatusEffectData: จำนวนซ้อนทับ, ระยะเวลา, เงื่อนไข
- BossPatternData: เฟส, ท่าโจมตี, เงื่อนไขเปลี่ยนเฟส

### Managers

- BattleState, TurnManager (จัดการคิวตาม SPD), DeckManager (จัดการจั่ว/ล็อก/ทิ้ง), EffectResolver, UIController

### Technical Architecture (กฎที่เพิ่มเข้ามา)
- **Event System:** สคริปต์ `Manager` ทั้งหมด (`BattleManager`, `UIManager`, ฯลฯ) "ต้อง" (Must) สื่อสารกันผ่าน **`Event System`** (ระบบสื่อสาร) เท่านั้น (เช่น `OnGameStateChanged`, `OnSanityChanged`)
- **Clean Code:** ห้ามมิให้ `Manager` "อ้างอิง" (Reference) ถึง `Manager` ตัวอื่นโดยตรงเด็ดขาด (No Tight Coupling) เพื่อให้ "โค้ดมีประสิทธิภาพ" และง่ายต่อการ "คัดกรอง"

---

## 16.5) Art Direction

- **Style:** `2D Anime / Cel Shaded
- **Actors (ในสนาม):** `2D Animated Sprite` (หันหลังสำหรับผู้เล่น, หันหน้าสำหรับบอส)
- **Animation Technique:**
    - `2D Rigging` (เช่น Spine/Live2D) จะถูกใช้เป็น "ฐาน" (Base) สำหรับอนิเมชั่นส่วนใหญ่ (Idle, Defend, Slide In/Out) เพื่อ "ประหยัดเวลา" (Scope)
    - `Frame-by-Frame (FBF)` อาจถูกใช้ "ผสม" (Hybrid) สำหรับ "ท่าที่ต้องการความสวยงาม" (Impact) (เช่น ท่าโจมตีหลัก, ท่าการ์ด `Assault`)
- **UI (Command):** `2D Bust-up` (ภาพนิ่งครึ่งตัว, ไม่ Rigged) ใช้เป็นปุ่มกด (มุมขวาล่าง)

---

## 17) Tuning Knobs (initial)

- **Element:** 1.20 / 0.80
- **SPD Baselines:** ผู้เล่น 100–120, บอส 110–125
- **Analysis:** +20 Sanity พื้นฐาน; เพิ่มขึ้นตามค่า Anomaly (ดู 7.2)
- **Card costs:** 12–35 Sanity (โจมตีถูก; อีเธอร์ริกปานกลาง/สูง)
- **Combo Multipliers:** ±5–20% (น้อย, เข้าใจง่าย, ไม่ซับซ้อน)
- **K (DEF Curves):** 500 เป็นค่าพื้นฐาน
- **Crit:** 5% / 150% พื้นฐาน; เพิ่มขึ้นตามค่า Luck (ดู 7.2)

---

## 18) Out of Scope

- Gacha System, Inventory, Equipment System
- พาสซีฟของคลาสย่อยขั้นสูง และระบบ Counter Attack ที่ซับซ้อน
- แผนที่โลก/การดำเนินเรื่อง, เส้นเรื่องยาว
- การต่อสู้กับบอสหลายตัว, กีกี้, กลไกแบบเรด

---