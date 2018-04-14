# Unity 3D Old C# Projects
This repository is made up of Unity 3D C# scripts written, before I had formal coding training, for personal and collaborative projects from 2013 - 2016.  They were not written using GitHub.  I have not cleaned them up to show an example of the code written before attending [Nashville Software School](https://github.com/nashville-software-school).  

# To Use
All files have to be run in the [Unity 3D Game Engine](https://unity3d.com/) version 3.x.

# Projects
1. Cut! The Visual Novel Designer for Unity (2013 - 2014)
1. World of WarCraft Camera & Movement Clone (2016)
1. Territories - Settlers of Catan Style Game (2015 - 2016)


---
## Cut! The Visual Novel Designer for Unity (2013 - 2014)
A runtime-compiled language to develop visual novels in Unity.  I wanted to see if I could build my own "programming language."  After talking wtih a friend about a visual novel project they wanted to build, I determined Unity was not an environment in which they could easily design visual novel game levels.  In order to give them the most control with the least amount of programming, I developed a language written in C# within Unity.

It was originally built without regex.  About half way through, I decided to use regex to parse CUT code.

### File Descriptions

1. Cut! The Visual Novel Designer for Unity _Folder_
    1. #### CUTCodingReference - Version 1.1405.docx  
        `A quick coding guide written in word for the CUT language.`
    1. #### C# Scripts _Folder_
        1. #### Classes _Folder_
            1. AnimationoClass.cs  
                `Contains name of animation and the string containing the actual animation.`
                
            1. AudioClass.cs  
                `Contains the name of the audio and the file path of the sound file.`
            1. ButtonClass.cs  
                `Contains the name of the button, the command to be executed when clicked, the x and y coordinates, and the texture.`
            1. ButtonClassDefault.cs  
                `Contains the default values for the button class.  I don't know why I chose to put the defaults in an alternate class to ButtonClass.cs  `
            1. ChannelClass.cs  
                `Contains information about the current channel that audio is being played over.  Contains the id of the channel, the max vovlumen, the time played for, and the information on when to fade in / fade out.`
            1. Character.cs  
                `Contains character name, the variable associated with the character, and their text color.`
            1. CurrentImage.cs  
                `Contains information of the image currently being displayed on the screen.  Values include is name, x position, y position, height, width, alpha, and the animation currently on it.`
            1. MenuClass.cs  
                `Contains name of menu and the lines that make up the menu.`  
            1. MenuControls.cs  
                `Contains information on the functionality of the menu.  Including the type of menu object, x and y location, width and height, text displayed, the command, whether it fades in, the current state of its fade in, and the texture and hover texture.`
            1. MovieClass.cs  
                `Contains information on a movie.  The name of the variable, the file name, and the actual data off the movie.`
            1. SaveData.cs  
                `Contains class that contains slots for all information that should be saved when the game is saved.`
            1. SceneClass.cs  
                `Contains the name of a scene (a block of cut code) and at what line number that occurs if it should be jumped to.`
            1. TextureVariable.cs  
                `Contains name and texture data of a texture to be loaded.`
            1. VariablesClass.cs  
                `Contains list of variables that the main game file uses.  The variables are abstracted away to this file to make the main file cleaner.`
            1. VersionDeserializationBinder.cs  
                `Turns C#/Unity objects into text to be stored in a file.` 

        1. CutVersion11400.cs  
            `Main controller of CUT.  Contains primary logic and parses the code.`
    1. #### Example CUT Game Files _Folder_
        1. Game00.cut  
            `An example game.`

        1. Game01.cut  
            `An example game.`
        1. GitMDemo.cut  
            `An example game.`
        1. Settings.cut  
            `An example settings file.`
    1. #### Example Save Files _Folder_
        1. QuickSave.cut  
            `An example quick save file.`

        1. Save.cut  
            `An example save file.`
        1. Save3.cut  
            `An example save file.`
        1. Save4.cut  
            `An example save file.`

<!-- Use to hide file structure to make this look better -->
<!-- <details>
<summary>Test</summary>
Content
</details> -->

---
## World of WarCraft Camera & Movement Clone (2016)
These scripts were made to replicate the World of WarCraft player camera, including mouse orbit, arrow key movement, zooming in and out, and collision detection to push the camera closer to the player.

The animations and models used in this project are not provided.  They were made by a third party.

### File Descriptions
1. World of WarCraft Camera & Movement Clone _Folder_
    1. CameraMovement20161020.cs    
        `Controls the camera's movement.  It provides functionality to smoothly follow the player as they move, rotate around the player, zoom in in when objects are between the camera and the player, and zoom in with the scroll wheel.
        This would be attached to the camera.`
    1. PlayerMovement20161004.cs    
        `Controls player's movement.  Animations used in the example are not provided.  This would be attached to the player.`

---
## Territories - Settlers of Catan Style Game (2015 - 2016)
Territories was a multiplayer turn based 3D game where players explore the map, mine resources, advance their technology, and encounter other players that become trusted allies or despicable foes.  The "server" is not a server, but a script that reads a file on the players computer. 

### File Descriptions
1. Territories - Settlers of Catan Style Game _Folder_  
    1. #### Classes _Folder_
        1. AlliesClass.cs  
        `Class containing the name of an ally.`

        1. GridPoint.cs  
        `Class containing and x and y grid coordinate.`
        1. HexGridClass.cs  
            `Class containing a hex on the map and it stats.`
        1. MessageClass.cs     
            `Class containing message type, text, and sender information.`
        1. PlayerClass.cs    
            `Class containing player resources, allies, upgrades, messages, and other stats.`
        1. ResourcesClass.cs     
            `Class containing a list of resources and a possible hex to be used in the messege class if resources and/or a hex is being traded between players.`
        1. SettingsClass.cs    
            `Holds two strings, the key and the value.  Used to hold settings that the player may have.` 
        1. SoldierClasss.cs     
            `Class contianng soldier type and wounds variables.`
        1. StringIntClass.cs    
            `Class enables you to store a string that is associated with an integer.` 
        1. TwoIntegerList.cs     
            `Class holding two integers under the same variable name.`
        1. UnitClass.cs     
            `Class containing information about a unit moving around the game board, including the owner's information, its resources, the soldiers that make it up, and what waves the soldiers are in.`
        1. UpgradeClass.cs      
            `Class containing the information that defines the effects of an upgrad and its cost.`
    1. #### Saving Data Scripts _Folder_ 
        1. FirstTestScriptPHPPost.cs  s          
            `Testing of posting data created by VersionDeserializationBinder to a website.`

        1. SaveData.cs     
            `Turns C#/Unity objects into text to be stored in a file.`
        1. server.cs    
            `Allows a user to create a map and save the map locally.`     
        1. VersionDeserializationBinder.cs     
            `Turns C#/Unity objects into text to be stored in a file. `
    1. BoatWave.cs    
        `Makes a 3d boat object rock back and forth.  It would be attached to the object.` 
        
    1. CameraMovement.cs     
        `Controls the camera's movement.  It would be attached to the camera object.`
    1. CamerScrollMovment.js  
        `Controls camera scrolling across the map.  It would be attached to the camera objectd.`
    1. GrassWave.cs    
        `This script makes grass wave as if it was in the wind.  It would be attached to the grass object. `
    1. HexHoverScript.cs     
        `Changes the color a hex when the cursor is hovering over it.`
    1. MainGame.cs     
        `Contains the main logic of the game.`
