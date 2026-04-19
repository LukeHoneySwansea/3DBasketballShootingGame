# 🏀 Basketball Shoot 3D

<div align="center">

Fast-paced 3D basketball game built in Unity  
**Score as many baskets as possible before time runs out**

</div>

---


https://github.com/user-attachments/assets/6da84bca-e1ff-46f6-96a3-e7b3a83da2c7




---

## 🎮 Gameplay

- Pick up a basketball  
- Charge your shot  
- Aim for the hoop  
- Score within **30-second rounds**

Each round:
- ⏱ Time limit
- 🎯 Target score to progress
- 📈 Increasing difficulty

---

## 🕹 Controls

| Action        | Input               |
|--------------|--------------------|
| Pick up ball | `Left Click`       |
| Charge shot  | `Hold Left Click`  |
| Shoot        | `Release Click`    |
| Aim          | Mouse movement     |

---

## 🔥 Features

- 🎯 Arc-based shooting system  
- 🧠 Aim assist using trajectory calculation  
- 🏀 Continuous ball spawning system  
- ⏱ Round-based gameplay loop  
- 📈 Progressive difficulty scaling  
- 🎯 Moving hoop with increasing challenge  
- ✨ Particle effects (score + despawn)  
- 🔊 Audio feedback (bounce + scoring)  
- 🧹 Clean round reset system  

---

## 🧠 Systems Overview

### 🎮 Player System
Handled in `PlayerGrab`
- Pickup / throw logic
- Charge mechanic
- Arc-based shooting

### 🏀 Ball System
Handled in `BallController`
- One-use balls (prevents reuse exploits)
- Bounce physics + audio
- Despawn with VFX

### 🔁 Spawning System
Handled in `BallSpawner`
- Ensures at least one ball exists
- Limits max balls in scene
- Prevents softlocks

### 🎯 Scoring System
Handled in `ScoreZone`
- Detects successful shots
- Prevents duplicate scoring
- Triggers:
  - Score
  - Sound
  - Particles

### ⏱ Round System
Handled in `GameRoundController`
- 30-second rounds
- Target score progression
- Game over condition

### 🎯 Hoop Movement
Handled in `HoopMover`
- Static in Round 1
- Moves from Round 2+
- Difficulty scaling:
  - Speed increases
  - Movement range expands gradually

---

## 🖼 UI

- Round indicator  
- Timer countdown  
- Score display (`current / target`)  
- Round start popup  
- Game over screen  

---
