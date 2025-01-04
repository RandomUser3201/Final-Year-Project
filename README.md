# 3D Prison Game

This project is a stealth-based 3D prison game that challenges player to navigate to the goal while avoiding the Enemy. The AI uses sound-based mechanics, enabling NPCs to hear, patrol, and chase the player based on auditory cues.

## Summary:

Due to a combination of overestimating my abilities, underestimating the complexity of certain tasks and inadequate project management, I was unable to implement all the planned features within the given timeframe. These features relate to multiple NPCs, player animations and assets, and better camera control.

### Implemented:
- A Pathfinding*: Ensures efficient and dynamic movement for NPCs.
- Sound Mechanics: Sound plays a key role in triggering guard actions.
- Dynamic AI: Guards with intelligent behaviour to Chase, Patrol, Attack and detect sound.
- Stealth Mechanic: Using the left shift key to sneak, the player can navigate around the enemy.

To play the game open the "bin" folder and open the .exe". 

### Controls:
- W, A, S, D -> Basic Movement 
- Left-Shift -> Sneak 
- Space -> Display Information

### Gameplay

- The Player must avoid the Enemy and reach the goalPoint position before the timer reaches 0. 
- Sneaking will provide benefit by reducing the volume if within the sound range, or keep it at 0 if pressed before moving. However, it will reduce the Player's speed by 3.
- If the Player is within the sound range and the volume exceeds the detectionthreshold of 0.7, the Enemy will begin chasing.

- The goalPoint position is located at the gate opposite the starting point, behind the green cube.

- Enemy will return to Patrol if the Player is not detected by sight or sound.
- The Player health is set to 3, when the Player is within the attack range the Enemy will reduce the Player's health by 1.
- Use the Space key to assist you with key information. 

![image](https://github.com/user-attachments/assets/6cfb0cdf-2b53-404c-9be0-e833270d5bf3)
![image](https://github.com/user-attachments/assets/eef1ad72-3b47-4027-abdf-cb6c2af7b722)
![image](https://github.com/user-attachments/assets/fa5c0062-6e20-4edc-b1bf-563876bd0d4f)
![image](https://github.com/user-attachments/assets/f57a804c-f9e0-43be-ae85-fa533e096de2)

## Project Timeline & Description:
| Task Name                                    | Start Date  | End Date    | Duration (Days) | Description                                      |
|----------------------------------------------|-------------|-------------|------------------|----------------------------------------------------|
| **1. Project Setup**                         | 14/10/2024  | 27/10/2024  | 14               | - Set up Unity project. <br>- Initial environment design. <br>- Basic player movement. |
| **1.1 Design basic level layout**            | 14/10/2024  | 20/10/2024  | 7                | - Create the overall layout of the game level.    |
| **1.2 Implement basic player controls**      | 21/10/2024  | 27/10/2024  | 7                | - Develop basic movement (and interaction) mechanics. |
| **2. Basic Guard AI & Pathfinding**       | 28/10/2024  | 10/11/2024  | 14               | - Implement basic guard AI patrol. <br>- Setup A* pathfinding (grid creation). |
| **2.1 Design guard patrol routes**           | 28/10/2024  | 03/11/2024  | 7                | - Define the paths along which guards will patrol. |
| **2.2 Implement A* algorithm**               | 04/11/2024  | 10/11/2024  | 7                | - Code the A* pathfinding algorithm functionality. |
| **3. Enhanced Guard AI**                     | 11/11/2024  | 24/11/2024  | 14               | - Implement investigating and chasing behaviors for guards. |
| **3.1 Define states in FSM**                 | 11/11/2024  | 15/11/2024  | 5                | - Establish finite state machine for guard AI.     |
| **3.2 Code chasing & investigate behavior**| 16/11/2024  | 24/11/2024  | 9                | - Implement behavior for guards to chase and investigate. |
| **4. Player Interaction & Sound Detection**| 25/11/2024  | 08/12/2024  | 14               | - Implement player mechanics for sneaking, hiding, and creating distractions. <br>- Introduce sound detection to guard AI. |
| **4.1 Implement guard sound detection mechanics** | 25/11/2024| 02/12/2024  | 8                | - Code how guards detect sounds made by the player. |
| **4.2 Develop stealth mechanics (sneak & crouch)** | 03/12/2024| 08/12/2024  | 6                | - Implement mechanics for sneaking and crouching.  |
| **5. Behavior Trees & Testing**            | 09/12/2024  | 22/12/2024  | 14               | - Implement behavior trees for complex guard behaviors. <br>- Begin playtesting to identify issues & improve gameplay. |
| **5.1 Design behavior tree structure**       | 09/12/2024  | 13/12/2024  | 5                | - Create the structure for guard behavior trees.    |
| **5.2 Integrate behavior tree into guard AI**| 14/12/2024  | 18/12/2024  | 5                | - Implement behavior tree functionality into guard AI. |
| **5.3 Conduct initial playtests & collect feedback** | 19/12/2024| 22/12/2024  | 4                | - Playtest the game and gather player feedback.     |
| **6. Finalization & Submission**           | 23/12/2024  | 02/01/2025  | 11               | - Finalize gameplay mechanics. <br>- Conduct playtesting & debugging. |
| **6.1 Act on tutor feedback**                | 23/12/2024  | 27/12/2024  | 5                | - Review and implement feedback from tutor.     |
| **6.2 Optimize performance**                  | 28/12/2024  | 30/12/2024  | 3                | - Improve game performance and efficiency.          |
| **6.3 Prepare Documentation for Submission** | 31/12/2024  | 02/01/2025  | 3                | - Prepare final documentation and presentation. |

## Development Log

### October: Initial Setup and Foundations
The project began with importing the necessary assets and setting up the Unity environment. A basic scene prototype was created to test player movement mechanics, which were successfully implemented. Early attempts at implementing A* pathfinding were made, laying the groundwork for future AI game development. While progress was lacking, the focus was on establishing a solid foundation for the game.

### November: Basic Pathfinding and Urgency for AI States
This month saw the successful implementation of basic pathfinding for the enemy AI. Despite taking longer than expected, the system now enables the enemy to track the player's location and avoid obstacles effectively. However, it was clear that enhancements were needed, including path visualization and improving the map for better AI interaction. There was an increasing urgency to implement enemy states, which became the next priority. Overall, the focus shifted toward refining core AI functionality to make the gameplay dynamic and interactive.

### December: Recovery, Enhancements, and Core Features
December was a highly productive but challenging month. After losing previous work due to issues with the package manager and forced pushes, efforts were directed at rebuilding the map and recovering lost code. Enemy patrol logic was introduced, allowing enemies to move within a radius, locate points randomly, and transition into a chase state upon spotting the player.

Enhancements included adding gizmos for debugging, improving obstacle avoidance, and refining A* pathfinding to activate only when the player was in sight. This made enemy behavior more realistic and efficient. Audio detection and player sound mechanics were also integrated: footsteps sounds were added for the player, and sneaking (using the L-Shift key) reduced noise and player speed.

Code quality was a focus this month. A superlinter workflow was set up to identify and fix whitespace errors, and detailed code comments were added for clarity. Additional improvements included fixing a bug that broke all enemy states, reworking enemy AI states, and adding an end-game timer. The game now supports a cohesive enemy state system, consisting of patrol, chase, and attack behaviors based on player noise and visibility. These changes brought the project much closer to its intended vision, setting up a strong foundation for further enhancements.

## Gantt Chart:
![gant chart for ai](https://github.com/user-attachments/assets/34566559-924f-4869-942b-d91dcc65a06c)

## Initial Finite State Machine Diagram:
![FSM Diagram](https://github.com/user-attachments/assets/66bcd05b-88c8-47d1-9d66-924e9627520a)

## Final FlowCharts:

The following images depict a summary for the main logic used in the game.

### Pathfinding

### ![flowchart pathfinding](https://github.com/user-attachments/assets/9bba1ed2-c218-47fe-9c07-ebfd90cb9342)

### Enemy AI

![flowchart enemy state](https://github.com/user-attachments/assets/aa93c01c-9c2c-4493-8658-775d785afbe0)

## References:

### [Pathfinding]
Sebastian League (2014). A* Pathfinding (E03: algorithm implementation). [online] YouTube. Available at: https://www.youtube.com/watch?v=mZfyt03LDH4&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3 [Accessed 3 Jan. 2025].

Aron G (2017). Get Started Tutorial - A* Pathfinding Project (4.1+). [online] YouTube. Available at: https://www.youtube.com/watch?v=5QT5Czfe0YE&list=PLKwzRvX1eGwHXz5D0nZUFx3fjHZBdyA6s [Accessed 3 Jan. 2025].

Aron G (2017). Get Started Tutorial - A* Pathfinding Project (4.0 and earlier). [online] YouTube. Available at: https://www.youtube.com/watch?v=OpgUcYzRpwM&list=PLKwzRvX1eGwHXz5D0nZUFx3fjHZBdyA6s&index=2 [Accessed 3 Jan. 2025].

Code Monkey (2021). How to use Unity NavMesh Pathfinding! (Unity Tutorial). [online] Available at: https://www.youtube.com/watch?v=atCOd4o7tG4 [Accessed 3 Jan 2025].

Sebastian League (2014). A* Pathfinding Tutorial (Unity) - YouTube. [online] Available at: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW. [Accessed 3 Jan 2025].

### [Enemy AI & State Machines]
Code Monkey (2021). Simple Enemy AI in Unity (State Machine, Find Target, Chase, Attack). [online] Available at: https://www.youtube.com/watch?v=db0KWYaWfeM [Accessed 3 Jan 2025].

Dave / Game Development (2020). FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial. [online] Available at: https://youtu.be/UjkSFoLxesw. [Accessed 3 Jan 2025].

git-amend (2023). EASY Unity Enemy AI using a State Machine. [online] YouTube. Available at: https://www.youtube.com/watch?v=eR-AGr5nKEU [Accessed 3 Jan. 2025].

This is GameDev (2023). How to code SMARTER A.I. enemies | Unity Tutorial. [online] YouTube. Available at: https://www.youtube.com/watch?v=rs7xUi9BqjE [Accessed 3 Jan 2025].

### [Camera]
All Things Game Dev (2022). How To Make An FPS Player In Under A Minute - Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=qQLvcS9FxnY. [Accessed 3 Jan 2025].

### [Assets]
Ink Phantom (2018) Polylised - Medieval Desert City | 3D Historic | Unity Asset Store. [online] Available at: https://assetstore.unity.com/packages/3d/environments/historic/polylised-medieval-desert-city-94557 [Accessed 3 Jan 2025].

Effecism (2023). Sound Effects - Footsteps. [online] Available at: https://www.youtube.com/watch?v=mPgGg4MJKKc.[Accessed 3 Jan 2025].
