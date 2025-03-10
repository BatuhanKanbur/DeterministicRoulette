# DeterministicRoulette
 
Game Rules and Steps:
Chip Selection:

Players select 25, 250, 500, 1000 and 5000 dollar chips to bet on.
The chips are placed by dragging them to the betting areas on the playing field.
Chip Placement on Betting Areas:

Players drag chips to specific betting areas.
Bets can be placed on various betting options (for example, odd numbers, colors, 1-18, 19-36, etc.).
Deleting Bets:

To delete placed bets, click on the bet fields on the playing field.
This removes the chips from the betting area.
Multipliers:

Bet multipliers are set according to standard European roulette rules.
The multipliers vary according to the type of bet the player chooses when placing a bet (for example, for odd number bets the multiplier can be 35:1).
Previous Numbers:

The table displays the numbers and results from previous games.
This section shows past results that players can refer to when strategizing.
The previous numbers are listed in a small area in one corner of the table.
Game Start:

After placing their bets, players click on the Start button.
This starts the game and the ball starts rolling.
Cheat Menu:

In the Cheat menu, numbers can be manually manipulated to change the outcome of the game.
Players can manually enter a specific number to determine where the ball will land.
Data Clearing:

Players can press a button to delete saved data.
This will clear previous game results and betting data.
Game Results:

The result of the game determines which number the ball lands on.
Players receive winnings or losses based on the results of their bets.
Restart:

After the game has finished, players can start a new game by clicking the Start button again to place new bets.

Design Patterns and SOLID Principles
Hook Attribute:

Description: The Hook Attribute is a structure that listens to changes in variables and automatically calls specific methods when these variables change. It provides a way to track changes and trigger necessary actions whenever a change occurs.
Usage: This is particularly useful for triggering events or actions when data changes. It follows an event-driven approach, making it easier to manage dynamic data changes in the game.
Dependency Injection (DI):

Description: Dependency Injection is a design principle where dependencies for a component are provided externally rather than hardcoded. Since frameworks like DI cannot be used in your case, you've created a simple Dependency Injection system that also supports additive scene loading, making it extensible and adaptable.
Usage: DI makes it easier to manage dependencies, improving testability, modularity, and flexibility. Popular DI frameworks such as Zencejt and VContainer could be used to solve such dependency management issues, though they are not used in your case.
AssetManager:

Description: The AssetManager provides a generic framework for handling Addressables and asset management. It helps in efficient asset loading and memory management.
Usage: The AssetManager allows dynamic loading and management of assets, making memory usage more efficient and reducing load times. It is a great solution for handling assets in large-scale games, where constant loading and unloading of resources are required.
ObjectManager:

Description: The ObjectManager implements an advanced object pool system that works without the need for a warm-up phase. It allows for the efficient reuse of objects.
Usage: Object pooling improves performance by reusing frequently used objects instead of creating new ones each time. This is particularly useful for games with dynamically spawning objects, as it reduces the overhead of object instantiation and garbage collection.
SOLID Principles:

Description: SOLID is a set of five design principles that help create more maintainable, scalable, and testable software. These principles ensure that the software is easy to modify, extend, and maintain.
Single Responsibility Principle (SRP): A class should have only one responsibility.
Open/Closed Principle (OCP): Classes should be open for extension but closed for modification.
Liskov Substitution Principle (LSP): Derived classes should be able to replace base classes without altering the correctness of the program.
Interface Segregation Principle (ISP): Clients should not be forced to depend on interfaces they do not use.
Dependency Inversion Principle (DIP): High-level modules should not depend on low-level modules. Both should depend on abstractions.
Usage: These principles are applied throughout your codebase, including scripts like Bet and Chip, making the software more modular, flexible, and easier to maintain.
TweenManager:

Description: Since frameworks like DOTween cannot be used, you developed a simple tweening system with support for extending methods. This system can be further extended in the future using UniTask to improve performance and flexibility.
Usage: This system provides a flexible solution for managing animations and transitions. It can be expanded using UniTask to enhance performance and make tweening operations more efficient.
General Approach:
Modular and Flexible Systems: The design focuses on modularity and flexibility, which allows for easy extension and maintenance. This makes it easy to add new features or modify existing systems without breaking the overall structure.
Performance and Efficiency: With features like object pooling and asset management, your system is optimized for performance, reducing load times and memory usage in resource-heavy games.