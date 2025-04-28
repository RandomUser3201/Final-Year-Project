# Heart Rate-Driven Gameplay in a 3D Stealth Game: Exploring Dynamic Difficulty Mechanisms

This artifact presents a 3D stealth-based game where the player must navigate through a level while avoiding detection by the enemy. The enemy AI uses sound-based mechanics, which allow it to hear, patrol, and chase the player depending on auditory cues and the player's heart rate.

## Summary

The game features a dynamic AI system where guards patrol, chase, and attack the player based on sight, sound, and heart rate values. The player's heart rate plays a critical role in the detection system, affecting how easily the enemy can detect them. By manipulating the heart rate (through gameplay mechanics like stress or tension), the enemy's behavior is influenced, making the game more challenging as the player’s heart rate increases.

Due to a combination of overestimating my abilities, underestimating the complexity of certain tasks, and inadequate project management, several planned features could not be implemented within the given timeframe. These include the addition of multiple NPCs, variety of sound and player data. However, the core gameplay mechanics, such as AI behavior, stealth, and dynamic detection, have been successfully integrated which is crucial for the research project. Furthermore, the last minute changes were made due to lack of immersion recieved by participants during the intitial test, for better results, new elements were introduced.

### Controls
- W, A, S, D -> Basic movement controls for the player.
- C -> Activate sneak mode, which reduces the player's sound emission but also reduces their movement speed by 3.
- Space -> Display information about the current game state, including enemy position and detection status.

### Key Features
- Pathfinding: The AI uses Unity’s NavMeshAgent to navigate and dynamically follow the player based on sight and sound.

- Sound Mechanics: Sound plays a pivotal role in triggering guard behaviors. The enemy will react to the player's noise, making stealth essential for success.

- Dynamic AI: The enemy AI exhibits intelligent behavior, including patrolling, chasing, and attacking based on player detection (sight, sound, and heart rate).

- Stealth Mechanic: The player can use the "Sneak" mechanic (left shift key) to reduce their sound emissions, making it harder for the enemy to detect them, though this reduces movement speed.

- Heart Rate Data: The player's heart rate, based on age, directly impacts the detection behavior of the enemy. Higher heart rates increase the likelihood of detection by the enemy, whereas lower heart rates keep the player more concealed.

To play the game open the "bin" folder and open the .exe". 

### Heart Rate Zones and Enemy AI Behavior:
The heart rate system is driven by the player's age and heart rate zones. The game calculates a player's target heart rate zone based on their age, and this heart rate data influences enemy detection:

1. Heart Rate Zones: The player's heart rate zone is classified as the target heart rate (THR), which varies by age:

- The player’s heart rate will fluctuate depending on stress or other in-game actions.
- If the player's heart rate exceeds certain thresholds, the enemy will become more alert and will detect the player more easily.
- Conversely, lower heart rates make it harder for the enemy to detect the player, simulating a more relaxed and less detectable state.

2. Age and Heart Rate Calculation: The player's heart rate zone is dynamically calculated based on the selected age. 
For example:
- A 20-year-old will have a target heart rate zone between 120-160 bpm.
- A 40-year-old will have a target heart rate zone between 108-153 bpm.

3. Impact on Enemy Detection: As the player's heart rate increases (due to stress or proximity to the enemy), the enemy AI becomes more sensitive to the player. This simulates the stress of the player increasing their likelihood of being detected. On the other hand, a relaxed player with a lower heart rate is harder to detect, offering a stealth advantage.

### Heart Rate Data Calculation:
The heart rate data is managed by the HeartRateData class. The player's heart rate data is stored in a table based on their age, which determines the minimum and maximum heart rate ranges for that age. The heart rate will be tracked and dynamically adjusted during gameplay.

```csharp
private Dictionary<int, (int min, int max, int maxHR)> heartRateTable = new Dictionary<int, (int, int, int)>
{
    { 20, (120, 160, 200) },
    { 25, (117, 166, 195) },
    { 30, (114, 162, 190) },
    { 35, (111, 157, 185) },
    { 40, (108, 153, 180) },
    { 45, (105, 149, 175) },
    { 50, (102, 145, 170) },
    { 55, (99, 140, 165) },
    { 60, (96, 136, 160) },
    { 65, (93, 132, 155) },
    { 70, (90, 128, 150) }
};
```

### Enemy AI State Machine:
The enemy's behavior is controlled by a simple state machine with three main states:

- Patrol: The enemy randomly moves around the environment, looking for the player. If it detects the player (via sight, sound, or heart rate), it transitions to the chase state.
- Chase: The enemy actively pursues the player if they are within sight range or making noise. If the player is within attack range, the enemy transitions to the attack state.
- Attack: The enemy stops moving and attacks the player, decreasing their health. After attacking, the enemy returns to the chase or patrol state.

### Gameplay

- Objective: The player must avoid detection by the enemy and reach the goal point before the timer runs out. The goal point is located at the gate opposite the starting point, behind a green cube.

- Stealth Mechanic: Sneaking (using the "C" key) will reduce the volume emitted by the player, which helps avoid detection if the player is within the sound detection range. Sneaking also reduces the player's speed by 3 units.

- Sound Detection: If the player is within the enemy's sound detection range and their emitted volume exceeds the detection threshold (0.7), the enemy will begin chasing the player.

- Player Health: The player starts with 3 health points. If the enemy attacks the player (when they are within attack range), the player's health will decrease by 1.

- Timer: The player must reach the goal point before the timer reaches zero. If time runs out, the player loses.

- Heart Rate: The player’s heart rate, as selected in the age dropdown, will determine how easily the enemy can detect them based on the player’s stress level. 

Enemy AI Behavior:  
The enemy will patrol the area, following a random path defined by a NavMeshAgent.

- If the player is in sight range or making noise, the enemy will transition into a chase state and pursue the player.

- If the player is within attack range, the enemy will stop moving and attack, reducing the player's health by 1 each time.

- If the player is not detected by sight or sound, the enemy will return to patrol.

![image](https://github.com/user-attachments/assets/6cfb0cdf-2b53-404c-9be0-e833270d5bf3)
![image](https://github.com/user-attachments/assets/eef1ad72-3b47-4027-abdf-cb6c2af7b722)
![image](https://github.com/user-attachments/assets/fa5c0062-6e20-4edc-b1bf-563876bd0d4f)
![image](https://github.com/user-attachments/assets/f57a804c-f9e0-43be-ae85-fa533e096de2)

![Image](https://github.com/user-attachments/assets/26132cf6-17aa-4681-9243-c13ec8b6df45)
![Image](https://github.com/user-attachments/assets/b938e6ba-5e9f-4d4b-ac36-c04f43e89282)


## Project Timeline & Description
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

### January: Enemy AI Overhaul, Stealth Mechanics & Game Polish
January focused on refining enemy AI and implementing sound-based detection. Enemies now respond to player-generated noise, with louder movement triggering a chase. Players can sneak (L-Shift) or stay still to avoid detection.
AI states were reworked into Patrol, Chase, and Attack, based on sight and sound. Key bugs related to LayerMasks and A* Pathfinding were fixed, ensuring stable enemy behavior.
Gameplay was polished with a win condition (reaching the finish before time runs out), improved camera movement, and a GUI update with control instructions. Code was also cleaned and commented for clarity.

### February: Heart Rate Integration, Visibility Toggle & Difficulty Mechanics

During this month the Pulse Rate Manager and Arduino integration to track heart rate were introduced. The goal was to toggle visibility based on heart rate, but this feature is still in progress.
Detection radius was successfully adjusted according to the player's heart rate, allowing the enemy to react more dynamically. A bug in script referencing was fixed, taking 3 hours but resolving the issue.
A fog effect was added, along with a difficulty mechanic: if the player’s heart rate exceeds 120 for more than 6 seconds, enemy speed increases to 100 (temporarily).

### March: Heart Rate Zones, Accuracy Improvements & Performance Enhancements
This month the focus was on refining the integration of heart rate data with the gameplay and optimizing performance. 
The Arduino project was successfully added to the Unity project folder, consolidating everything in one place for easier management.
To improve performance, heart rate updates were modified from registering every second to a longer interval, addressing earlier performance issues and reducing lag.

A key feature this month was the implementation of a GUI dropdown menu, allowing players to select their age group. Based on the selected group, the game dynamically adjusts the heart rate zones through a dictionary, modifying gameplay mechanics accordingly.

Accuracy improvements were made by adding timers and adjusting the heart rate threshold for more precise detection. This change ensures smoother and more accurate transitions between states.
Additionally, heart rate logs are now generated in a .CSV format, providing a detailed log with each entry containing Timestamp, HeartRate, and EnemyState. The logs can be found in AppData/LocalRow/.

### April: Enhanced Player Controls, Fear Factor & FMOD Integration
The focus this month was on improving player experience and enhancing the horror elements of the game. The player capsule was replaced with a more refined model to provide better controls, offering a smoother and more responsive gameplay experience.
The enemy character was also replaced with a scarier model, accompanied by animations that enhance the fear factor. These animations are still a work in progress but are already contributing to a more immersive atmosphere.

Key technical improvements were made, including fixing the enemy rotation to properly face both the WalkPoint and the player, ensuring better AI behavior.
To deepen the horror experience, lighting was introduced with two spotlights:
A forward-facing flashlight that follows the player, enhancing visibility.
A spotlight above the player, aiding navigation in dark areas.

In terms of audio, FMOD integration was implemented for added realism, introducing an enemy growl and a player heartbeat sound effect to amplify tension. Additionally, issues with player volume and enemy sound range were fixed, improving sound accuracy.

Code cleanup was performed, with efforts to adhere to Unity C# conventions and resolve the issue where PlayerSound.cs would uncheck itself upon play.

Lastly, adjustments were made to the pulse rate logic, filtering out inaccurate data (like a heart rate of 0 or values outside the expected age group zones), ensuring the heart rate system functions correctly.


## Gantt Chart
![Image](https://github.com/user-attachments/assets/6a7650af-6097-4e77-9d67-b42cb1a084bd)

## Initial Finite State Machine Diagram
![FSM Diagram](https://github.com/user-attachments/assets/66bcd05b-88c8-47d1-9d66-924e9627520a)

## Final FlowCharts

The following images depict a summary for the main logic used in the game.

### Pathfinding:

### ![flowchart pathfinding](https://github.com/user-attachments/assets/9bba1ed2-c218-47fe-9c07-ebfd90cb9342)

### Enemy AI:

![flowchart enemy state](https://github.com/user-attachments/assets/aa93c01c-9c2c-4493-8658-775d785afbe0)

## References

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

### [Assets] / [Sound] 
Ink Phantom (2018) Polylised - Medieval Desert City | 3D Historic | Unity Asset Store. [online] Available at: https://assetstore.unity.com/packages/3d/environments/historic/polylised-medieval-desert-city-94557 [Accessed 3 Jan 2025].

Effecism (2023). Sound Effects - Footsteps. [online] Available at: https://www.youtube.com/watch?v=mPgGg4MJKKc.[Accessed 3 Jan 2025].

THE_bizniss (2007) beast breathing [online] Available at: https://freesound.org/people/THE_bizniss/sounds/37822/ [Accessed 25 Apr 2025].

Epidemic Sound (2025) Clicky, 83 BPM [online] Available at: https://www.epidemicsound.com/sound-effects/tracks/e375cb2f-dd1a-4201-9053-73af24c7b7d7/ [Accessed 25 Apr 2025].

Mixamo (2025) Parasite L Starkie [online] Available at: https://www.mixamo.com/#/?page=2&query=&type=Character [Accessed 25 Apr 2025].


