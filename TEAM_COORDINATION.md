# Project Coordination Plan: PARANOID:PROTOTYPE [Ecliptica]

เอกสารนี้ใช้สำหรับประสานงานกับทีม (Design, Art, Tech) เพื่อให้เห็นภาพรวมว่าใครต้องส่งมอบอะไรในช่วงต่อไปของการพัฒนา MVP

---

## 🟢 Current Status (Tech)
**ระบบที่เสร็จแล้ว (Backend Logic Ready):**
1.  **Core Battle Loop:** Turn-based, Party System (3 ตัวละคร), Phase-based Execution.
2.  **Resources:** HP, Sanity (Lucid/Strained/Fractured effects), Shield.
3.  **Cards:** Infinite Draw, Shared Hand, Card Locking.
4.  **Mechanics:**
    *   **Combo:** 3-Hit chains (AAA, DDD, NNN) มีผลโบนัสแล้ว
    *   **Field Resonance:** Logos/Illogic/Nihil เปลี่ยน Stat และ Damage ได้จริง
    *   **Boss:** รองรับ Multi-HP Bars (Phases)

---

## 🚧 สิ่งที่ต้องการจากทีม (Dependencies Request)

### 1. Game Design (GD)
*   **Boss Patterns:** ขอตารางท่าบอส (Boss Actions) แยกตาม Phase
    *   *Format:* Phase 1 ใช้ท่า A, B (สุ่ม); Phase 2 เพิ่มท่า C (Telegraph); Phase 3 ท่าไม้ตาย
*   **Card & Skill Data:**
    *   ขอรายชื่อ Skill Card ครบ 6 ใบ ของตัวละครทั้ง 3 ตัว (ชื่อ, Cost, Effect, Stat Scaling) เพื่อนำไปกรอกลง ScriptableObject
    *   ขอค่า Stats เริ่มต้น (HP, Agility, Power, Durability) ที่ "น่าจะสมดุล" สำหรับเริ่ม Test
*   **Balance Formulas:** ยืนยันสูตร Anomaly Parameter (ผลต่อ Sanity Cost) และ Luck (Crit Chance)

### 2. Art (2D/UI)
*   **UI Assets (Priority สูง):**
    *   **Party HUD:** กรอบภาพตัวละคร, หลอด HP, หลอด Sanity (แนวนอน/วงกลม?)
    *   **Command Slots:** ช่องว่าง 3 ช่อง สำหรับแสดงท่าที่เลือก (Attack/Defend/Analyze)
    *   **Timeline Strip:** ไอคอนแสดงลำดับเทิร์น (ใครได้เล่นก่อน/หลัง)
    *   **Card Frame:** กรอบการ์ดที่แยกสีตาม Type (Assault, Aegis, etc.) และพื้นที่ใส่ภาพประกอบ
*   **Character Sprites:**
    *   ต้องการภาพ `Idle`, `Attack`, `Defend`, `Hit/Damaged` ของตัวละคร 3 ตัว (สำหรับทำ Prototype)
*   **FX References:**
    *   ขอ Reference mood ของเอฟเฟกต์ "Sanity Break" (Glitch/Distortion)
    *   สี/ธีม ของ Field ทั้ง 3 แบบ (Logos=ทอง/ขาว?, Illogic=ม่วง/ชมพู?, Nihil=ดำ/เทา?)

### 3. Audio
*   **SFX:**
    *   UI Clicks (Select, Cancel, Confirm)
    *   Combat Hits (Slash, Blunt, Magic Hit)
    *   Sanity Glass Break (เสียงกระจกแตก/เสียงวิ้ง)
*   **BGM:**
    *   เพลง Battle Phase 1 (Normal)
    *   เพลง Battle Phase 2/3 (Intense/Desperation)

---

## 📅 Next Steps Timeline

| Phase | Focus | Who | Actions |
| :--- | :--- | :--- | :--- |
| **Phase 2.5** | **Boss AI & Logic** | **Code (Me)** | เขียน AI บอสให้เปลี่ยนท่าตาม Phase และรองรับ Telegraph |
| | **Data Entry** | **GD + Code** | กรอกข้อมูล Skill และ Boss Stats ลง Unity Inspector |
| **Phase 3** | **Battle UI** | **Code + Art** | เชื่อมต่อ Logic เข้ากับ UI (Party Select, Command Queue) |
| | **Visual Feedback** | **Code + Art** | ใส่ Damage Text, Field Change Effect |
| **Phase 4** | **Content & Polish** | **All** | Playtest, ปรับตัวเลข, ใส่เสียง |

---

**สรุปสำหรับทีม:**
*   **Code** พร้อมทำ UI ต่อแล้ว -> รอ Asset UI เบื้องต้น (หรือจะใช้ Placeholder ไปก่อน)
*   **Code** พร้อมทำ Boss AI -> รอตาราง Pattern จาก GD

