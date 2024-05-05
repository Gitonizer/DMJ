Welcome to Magico Random game! This game was made so I could show some features usually present in game development while teaching classes.

Implemented features:
- Proc gen;
- Combat:
  - Spell based combat;
  - Type effectiveness;
  - You can swap your own type on the fly;
- Save feature:
  - Game keeps track of the level number you were;
  - Game keeps track of the level configuration if it's not the beginning level.
- Dialog feature:
  - It's part of the objectives feature, you will talk with an NPC.
- Objectives feature:
  - Single objective implemented, meant to cycle through random objectives;
  - By talking and interacting with an NPC, you may complete part of an objective;
  - By defeating all of the enemies, you may complete part of an objective.
- Items/Inventory feature:
  - You can interact with items:
    - Pick up;
    - Use;
    - Throw away (unless it's a key item);
    - All these can be managed through inventory.

- Levels are proceduraly generated where the following aspects are always different:
  - Map configuration;
  - Map theme;
  - Placement and number of enemies;
  - Placement of quest objectives.

- Known issues:
  - Map may generate corridors that are too thin and without walls;
  - Enemy counter may be set up with the incorrect number of total enemies.
