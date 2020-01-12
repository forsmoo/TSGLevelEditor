﻿//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGLTF;

namespace Lirp
{
    
    public class CustomLevelEditorWindow
    {
        static CustomLevelSaver saver;
        static string heightmapFilename = "heightmap.png";
        static string splatmapFilename = "splatmap.png";
        static string lightmapFilename = "lightmap.png";
        static string definitionFilename = "definition.json";

        [MenuItem("The Snowboard Game/Export all")]
        public static void ExportAll()
        {
            saver = GameObject.FindObjectOfType<CustomLevelSaver>();
            if (saver == null)
                Debug.LogError("Save definition could not be found");
            else
                _ExportAll();
        }

        [MenuItem("The Snowboard Game/Export placeables")]
        public static void ExportPlaceables()
        {
            saver = GameObject.FindObjectOfType<CustomLevelSaver>();
            if (saver == null)
                Debug.LogError("Save definition could not be found");
            else
                _ExportPlaceables();
        }


        static string GetNamePostfix(GameObject go)
        {
            var postFix = "";

            if (go.GetComponent<SphereCollider>() != null)
            {
                postFix = "_SCOL";
            }
            if (go.GetComponent<BoxCollider>() != null)
            {
                postFix = "_BCOL";
            }
            if (go.GetComponent<MeshCollider>() != null)
            {
                postFix = "_COL";
            }

            if (go.layer == Layers.Deep)
                postFix += "_D";
            else if (go.layer == Layers.Groomed)
                postFix += "_G";

            return postFix;
        }
        static void TraverseT(Transform t, bool setup)
        {
            var layer = t.gameObject.layer;
            var postFix = GetNamePostfix(t.gameObject);
           
            if( setup && !t.gameObject.name.Contains(postFix) )
            {
                t.gameObject.name += postFix;
            }
            
            if( !setup && t.gameObject.name.Contains(postFix))
            {
                if( !string.IsNullOrEmpty(postFix) )
                    t.gameObject.name.Replace(postFix,"");
            }
            
            for (int i =0;i<t.childCount;i++)
            {
                var child = t.GetChild(i);
                TraverseT(child, setup);
            }
        }

        [MenuItem("The Snowboard Game/Export GLTF")]
        public static void ExportGLTF()
        {
            saver = GameObject.FindObjectOfType<CustomLevelSaver>();
            
            var exportOptions = new ExportOptions { TexturePathRetriever = RetrieveTexturePath };

            //foreach(var t in saver.GLTFRoot)
              //  TraverseT(t, true);
            var exporter = new GLTFSceneExporter(saver.GLTFRoot, exportOptions);

            //var path = EditorUtility.OpenFolderPanel("glTF Export Path", "", "");

            var path = GetPath("");
            if (!string.IsNullOrEmpty(path))
            {
                exporter.SaveGLTFandBin(path, saver.LevelName);
            }

            //foreach (Transform t in saver.GLTFRoot)
              //  TraverseT(t, false);
        }


        static string GetPath(string filename)
        {
            if( saver == null )
                saver = GameObject.FindObjectOfType<CustomLevelSaver>();
            return saver.OutputDirectory + "/" + saver.LevelName + "/" + filename;
        }


        static bool SetupSave()
        {
            string basePath = GetPath("");
            try
            {
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }


        public static void _ExportAll()
        {
            ExportTerrain();
            ExportPlaceables();
            ExportGLTF();
        }

        public static void _ExportPlaceables()
        {
            if (!SetupSave())
                return;

            
            var levelDefinition = new CustomLevelDefinition();
            levelDefinition.LevelName = saver.LevelName;
            levelDefinition.Spawns = new List<Spawn>();
            levelDefinition.LobbyCameraPosition = saver.LobbyCamera.position;
            levelDefinition.LobbyCameraRotation = saver.LobbyCamera.rotation;
            levelDefinition.LobbyPosition = saver.Lobby.position;
            levelDefinition.LobbyOrientation = saver.Lobby.rotation;

            foreach (var spawn in saver.Spawns)
            {
                var newSpawn = new Spawn();
                newSpawn.Direction = spawn.transform.forward;
                newSpawn.Position = spawn.transform.position;
                levelDefinition.Spawns.Add(newSpawn);
            }

            var customPrefabs = GameObject.FindObjectsOfType<CustomPrefab>();
            var placeables = new List<LevelPrefabInstance>();
            foreach (var obj in customPrefabs)
            {
                var instance = new LevelPrefabInstance();
                instance.Position = obj.transform.position;
                instance.Rotation = obj.transform.rotation;
                if (obj.Scalable)
                    instance.LocalScale = obj.transform.localScale;
                else
                    instance.LocalScale = Vector3.one;

                bool throwAway = false;
                if (!string.IsNullOrEmpty(obj.PrefabID))
                {
                    instance.PrefabID = obj.PrefabID;
                }
                else
                {
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj.gameObject);
                    if (prefab)
                        instance.PrefabID = prefab.name;
                    else
                        throwAway = true;
                }

                if( !throwAway)
                    placeables.Add(instance);
            }

            levelDefinition.Placeables = placeables;

            var terrain = GameObject.FindObjectOfType<Terrain>();
            levelDefinition.TerrainWidth = terrain.terrainData.size.x;
            levelDefinition.TerrainHeight = terrain.terrainData.size.y;
            levelDefinition.TerrainPosition = terrain.GetPosition();

            var trees = GameObject.FindObjectsOfType<TreeDefinition>();
            levelDefinition.TreeDensity = saver.TreeDensity;
            levelDefinition.GrassDensity = saver.GrassDensity;
            levelDefinition.PlantDensity = saver.PlantDensity;
            levelDefinition.Trees = new List<TreeExportInstance>();
            foreach(var tree in trees)
            {
                var newTree = new TreeExportInstance();
                newTree.position = tree.transform.position;
                newTree.rotation = tree.transform.rotation;
                newTree.scale = tree.transform.localScale;
                newTree.PrefabIndex = tree.TreePrefabIndex;
                levelDefinition.Trees.Add(newTree);
            }

            string json = JsonUtility.ToJson(levelDefinition);
            string filename = GetPath(definitionFilename);
            if (File.Exists(filename))
                File.Delete(filename);

            File.WriteAllText(filename, json);
        }

        


        public static void ExportAssetBundles()
        {
            var path = GetPath("unityAssetBundle");
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.NoTarget);
        }


        public static void ExportTerrain()
        {
            if (!SetupSave())
                return;

            if(GameObject.FindObjectsOfType<Terrain>().Length > 1 )
            {
                Debug.LogError("More than 1 terrain in scene");
                return;
            }

            var terrain = GameObject.FindObjectOfType<Terrain>();
            var terrainExtention = terrain.gameObject.GetComponent<TerrainExtension>();
            if( terrainExtention == null )
            {
                Debug.LogError("Couldn't find terrain extention");
                return;
            }
            TerrainMapGenerator.GenerateHeightMap(terrainExtention, GetPath(heightmapFilename));
            TerrainMapGenerator.GenerateNormalMap(terrainExtention, GetPath(lightmapFilename));
            TerrainMapGenerator.SaveSplatMapFromTerrain(terrainExtention, GetPath(splatmapFilename));
        }

        public static string RetrieveTexturePath(UnityEngine.Texture texture)
        {
            return AssetDatabase.GetAssetPath(texture);
        }
    }
}
