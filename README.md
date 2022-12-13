# Griphoton
*by Pia Schroeter*

**The game started out as an university project for the course Games Programming (2. Semester 2022)**

The game was published on the Google Play Store: https://play.google.com/store/apps/details?id=com.hexagonpuzzles.griphoton  
   
## Premise
*In this game, the player ends up in a parallel world called Griphoton and has to fight monsters in dungeon levels to return to the player's world. The player has to solve puzzles given by ghosts living in Griphoton to get stronger for the dungeon. The game is a 2D mobile game, and the player walks around the world to find houses with ghosts and their puzzles.*

## Griphoton - The World  
   
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-13_14-34-55.jpg" width=50% height=50%/>
<p/>

<p align="center">
   The world Griphoton consists of 31 houses spread out on a wide plane.
<p/>



### The Houses 
The position of every house except for the dungeon is generated randomly during the first tutorial. This layout is saved and unique for every player so they can explore it in their own time. If the player should decide to reset the game, then Griphoton is generated anew. Every house has an owner. These owners are ghosts who are stuck on a puzzle, and the player can help them with their puzzle to get stronger for the dungeon. If a puzzle is solved, the house and the owner will vanish.

### The Dungeon House 
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-47.jpg" width=65% height=65%/>
<p/>

The dungeon is a house with a black roof, and it always has the same position in the centre of Griphoton. The player can always return to the dungeon house by walking back or clicking on the "Dungeon" button in the settings. The player can enter the dungeon by entering the house.

### The Player 
When the player enters Griphoton for the first time, they will stand in front of the dungeon house. The player can explore Griphoton by moving their finger along the touchscreen in the direction they want to move to. The player can enter every house by just tapping on it. 

### The Pathways 
Every house is connected to another random house via a path, and the player can use the paths to find new houses.


## The Map 
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_10-42-47.jpg" width=65% height=65%/>
<p/>
The player can use a map to orientate themself. The map will initially be empty except for the dungeon house and the player. Whenever the player enters a house for the first time, the house will (magically) appear on the map. The house will be crossed out on the map when the player solves a puzzle.

## The Puzzles
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-51.jpg" width=33% height=33%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-55.jpg" width=33% height=33%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-13_17-00-42.jpg" width=33% height=33%/>
<p/>

There are 15 different puzzle types. Except for two, every puzzle type appears twice. The zebra puzzle only appears once, and the replacement puzzle appears three times. Most of the puzzles came from [Erich Friedman's Website](https://erich-friedman.github.io/puzzle/). The others are either personal implementations of known puzzles (e.g. river crossing puzzles) or came from other websites (check out the credits in the game for more information).

## The Dungeon
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-13_17-00-27.jpg" width=65% height=65%/>
<p/>

### The Levels
There are a total of 20 levels. Every level consists of a room with at least one monster and sometimes some spikes which are coming from the ground. Once the player has defeated all the monsters, they can enter the next room. The last room contains all the monsters and a portal through which the player can return home. When the player reaches the portal, they can end the game and return home. If the player still needs to solve some puzzles, they can choose to stay and solve them first.

### The Monsters
<p align="center">
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-13.jpg" width=49% height=49%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-22.jpg" width=49% height=49%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-37.jpg" width=49% height=49%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-41.jpg" width=49% height=49%>
   <img src="https://github.com/pia-sr/Griphoton/blob/main/Pictures_Git/photo_2022-12-08_11-15-44.jpg" width=55% height=55%/>
<p/>
There are five different types of monsters, and every monster has its own pattern. The "Reaper of the Unsolvable" is only in the last level and protects the portal. The other four monsters appear multiple times in the dungeons, depending on the level's difficulty.

 
