# 🎰 Deterministic Roulette
[Oyunumu oynayın](https://itch.io/embed/3386026)

## 📜 Game Rules and Steps

### 🎟️ Chip Selection
- Players can select chips of **25, 250, 500, 1000,** and **5000** dollars.
- Chips are placed by dragging them to the betting areas on the playing field.

### 🎯 Chip Placement on Betting Areas
- Players drag chips to specific betting areas.
- Bets can be placed on various options, such as:
  - **Odd/Even numbers**
  - **Red/Black colors**
  - **1-18 or 19-36 ranges**
  - **Straight, Split, Street, Corner, Line, and other European Roulette bets**

### ❌ Deleting Bets
- Click on the betting area to remove placed chips.

### 🎲 Multipliers
- Bet multipliers follow **standard European roulette rules**.
- Example: **Straight-up bets (single number) have a 35:1 payout**.

### 🔢 Previous Numbers
- Displays past game results in a small section of the table.
- Helps players analyze and strategize based on previous outcomes.

### ▶️ Game Start
- Players place bets and click the **Start** button.
- The ball starts rolling, and the game outcome is determined.

### 🛠️ Cheat Menu
- Allows manual manipulation of the outcome.
- Players can **enter a specific number** to set the final result.

### 🧹 Data Clearing
- A **reset button** clears all previous game results and betting data.

### 🎯 Game Results
- The game determines where the ball lands.
- Players receive winnings or losses based on their bets.

### 🔄 Restart
- After a game ends, players can start a new round by clicking the **Start** button again.

---

## 🏗️ Design Patterns and SOLID Principles

### 🏷️ Hook Attribute
- **Description**: Listens to variable changes and automatically triggers methods.
- **Usage**: Useful for handling **event-driven updates** in the game.

### 🔗 Dependency Injection (DI)
- **Description**: External dependencies are injected instead of being hardcoded.
- **Custom Implementation**: Since DI frameworks (e.g., Zencejt, VContainer) are not used, a **custom DI system** was built with support for **additive scene loading**.
- **Benefits**:
  - Improves **testability** and **modularity**.
  - Makes the system more **extensible and flexible**.

### 📦 AssetManager
- **Description**: Manages **Addressables and asset loading** efficiently.
- **Usage**:
  - Handles **dynamic asset loading**.
  - Optimizes **memory usage** and reduces **load times**.

### 🔄 ObjectManager
- **Description**: Implements an **advanced object pooling system** without a warm-up phase.
- **Usage**:
  - Reuses objects instead of creating new ones, reducing overhead.
  - Improves performance in **dynamically spawning elements**.

### 📏 SOLID Principles
- **S**ingle Responsibility Principle (**SRP**): Each class handles only **one responsibility**.
- **O**pen/Closed Principle (**OCP**): Classes can be **extended without modification**.
- **L**iskov Substitution Principle (**LSP**): Derived classes can **replace base classes** seamlessly.
- **I**nterface Segregation Principle (**ISP**): Clients **only depend on required interfaces**.
- **D**ependency Inversion Principle (**DIP**): High-level modules rely on **abstractions** instead of low-level details.
- **Usage**:
  - Applied throughout the codebase, including **Bet** and **Chip** scripts.
  - Makes the software **modular, maintainable, and scalable**.

### 🎭 TweenManager
- **Description**: A custom tweening system (since **DOTween** is not used).
- **Usage**:
  - Supports **extending methods**.
  - Can be improved further using **UniTask** for better performance.
  - Manages animations and transitions smoothly.

---

## 🎯 General Approach
✅ **Modular and Flexible Systems**
- Easy to **extend and maintain**.
- New features can be added **without breaking existing functionality**.

🚀 **Performance and Efficiency**
- **Object pooling** and **asset management** optimize **memory usage and performance**.
- Reduces **load times** in resource-heavy scenarios.

🎮 **Scalable Architecture**
- Designed to **adapt** to new game mechanics and updates effortlessly.

---

### 📌 Notes
- This project is built with **Unity**.
- Uses **Addressables, Object Pooling, and Dependency Injection** for efficient game management.
- Custom **TweenManager** replaces **third-party tweening libraries**.

👨‍💻 **Happy Coding! 🎲**

