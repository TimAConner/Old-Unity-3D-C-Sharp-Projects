# Unity-C-Old-Projects
This repository is made up of Unity 3D C# scripts written by me before I had any formal coding training for personal and collaborative projects from 2013 - 2016.  I have not cleaned them up to show an example of the code I wrote before attending [Nashville Software School](https://github.com/nashville-software-school)

# Projects
## Cut! The Visual Novel Designer for Unity (2013 - 2014)
A runtime-compiled language to develop visual novels in Unity.  I wanted to see if I could build my own "programming languages", and determined Unity was not an environment in which client could easily design visual novel game levels.  In order to give client the most control with the least amount of programming, I developed a language written in C# within Unity.

## World of WarCraft Camera & Movement Clone (2016)
These scripts were made to replicate the World of WarCraft player camera, including mouse orbit, arrow key movement, zooming in and out, and collision detection to push the camera closer to the player.

## Territories - Settlers of Catan Style Game (2015 - 16)
Territories was a multiplayer turn based 3D game where players explore the map, mine resources, advance their technology, and encounter other players that become trusted allies or despicable foes.  The "server" is not a server, but a script that reads a file on the players computer. 

### File Description
1. Territories - Settlers of Catan Style Game (Folder)  
    1. Classes (Folder) 
        1. AlliesClass.cs  
        1. GridPoint.cs
            * Class containing and x and y grid coordinate.
        1. HexGridClass.cs
            * Class containing a hex on the map and it stats.
        1. MessageClass.cs  
            * Class containing message type, text, and sender information.
        1. PlayerClasas.cs
            * Class containing player resources, allies, upgrades, messages, and other stats.
        1. ResourcesClass.cs  
            * Class containing a list of resources and a possible hex to be used in the messege class if resources and/or a hex is being traded between players.
        1. SettingsClass.cs  
            
        1. SoldierClasss.cs  
            * Class contianng soldier type and wounds variables.
        1. StringIntClass.cs 
            * Class enables you to store a string that is associatec with an integer. 
        1. TwoIntegerList.cs  
            * Class holding two integers under the same variable name.
        1. UnitClass.cs  
            * Class containing information about a unit moving around the game board, including the owner's information, its resources, the soldiers that make it up, and what waves the soldiers are in.
        1. UpgradeClass.cs   
            * Class containing the information that defines the effects of an upgrad and its cost.
    1. Saving Data Scripts (Folder) 
        1. FirstTestScriptPHPPost.css          
        1. SaveData.cs          
        1. server.cs          
        1. VersionDeserializationBinder.cs  
    1. BoatWave.cs  
    1. CameraMovement.cs  
    1. CamerScrollMovment.js  
    1. GrassWave.cs  
    1. HexHoverScript.cs  
    1. MainGame.cs  
