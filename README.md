# TSGLevelEditor
Instructions on how to create custom levels for The Snowboard Game

Download and install Unity3D (It's free)
https://store.unity.com/#plans-individual

Download or clone this project to your computer and open it in Unity

Open the scene Assets/Scene/Example
Find the object CustomLevel and in the inspector change these fields
Output Directory: [Steam]/steamapps/common/thesnowboardgame/CustomLevels
Level Name: [Level name]

Design your level.(More instructions below)
LCZ's awesome tutorial: https://www.youtube.com/watch?time_continue=2&v=YiHTZXl1FNM&feature=emb_title
Video tutorial: https://www.youtube.com/watch?v=rJwxMCG8Cpo

To create/save the level so it shows up in the game, you need to go to the menu item "The Snowboard Game/Export All"
"The Snowboard Game/Export All" (This will export the terrain and all objects)
"The Snowboard Game/Export Placeables" (This will export everything you placed in the level, excluding the terrain, this is alot faster)

Now you should be able to see it in game in the Travel menu
If you already loaded the level you can reload it instantly with the hotkeys 
[Ctrl]+[R] to reload everything
[Ctrl]+[D] to reload placeables

Design Process
Unpack and open the level editor project

1. Find Custom Level object
 - Fix Output Directory
 - Level Name

2. Setup required objects
 - Spawns
 - Lobby 
 - Camera

3. Project structure
 - Scenes: Contains example.scene with a leve, and prefab.scene with all prefabsl
 - Prefabs: All prefabs you can use when build the level 
 - Materials: Special materials that may convert once imported to the game

4. Terrain painting (Terrain layers)
	1. Deep snow
	2. Packed snow trails (Specific care, brush strength (not there))
	3. Snow rubble
	4. Rocks

5. Terrain Tools
Use TerrainExtention object on the Terrain to generate splatmap from terrain and smooth it out.

6. Placeables
 - GroundAlign: With this component you can place and rotate objects on the ground 
 - Scaleable: If this is set, the object can be scaled

7. Trees
- All the prefabs with a TreeDefinition component will be exported as a tree

8. Your own stuff
- You can export custom meshes, textures and materials if you place them in GLTFRoot in the scene.
- If you want to be able to ride your custom meshes they need a collider. You have three optioons. BoxCollider, SphereCollider or MeshCollider. Add this component to the custom game object, then you need to set the Layer in the top right corner of the Inspector to 
Default, Terrain, DeepPhysicsSnow or GroomedPhysicsSnow


9. Travel to the level in game and use these shortcuts to reload
 - Reload level [Ctrl]+[R]
 - Reload placeables [Ctrl]+[D]

Useful keyboard shortcuts
[W] - Move tool
[E] - Rotation tool
[R] - Scale tool
[F] - Focus object
[ctrl]+[shift] - To ground objects the fast way, select an object, and then hold [ctrl]+[shift] grab the small rectangle and it will automatically place it on the ground

Terrain layers
	1. Deep snow
	2. Packed snow trails (Specific care, brush strength (not there))
	3. Snow rubble
	4. Rocks

Limitations
- Can't change position of the terrain
- Can't add layers to terrains.

## Update
To update from the first version
1. Make a backup of the old project
2. In the project view, make sure you name your scene something other than Example.
3. Right click your scene and export it as a package. For dependencies only include your terrain and any custom prefabs you made
4. Open the new level design project and import the package. Don't override anything, just tick everything that will be added.
5. Now you should have the Example scene and your previous scene in the project. 
6. To export correctly you need to make sure that the CustomLevel object is setup correctly. Easiest way is to replace the Spawns and CustomLevel from the new scene to your old scene, and setup path again.
