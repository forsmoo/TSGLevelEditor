//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Lirp
{
    public class CustomLevelSaver : MonoBehaviour
    {
        public string OutputDirectory;
        public string LevelName;
        public List<Transform> Spawns;
        public Transform Lobby;
        public Transform LobbyCamera;

        public Transform[] GLTFRoot;

        public float TreeDensity = 0.15f;
        public float GrassDensity = 0.3f;
        public float PlantDensity = 0.3f;

        public bool ValidateLevel()
        {
            bool isValid = true;
            var terrain = FindObjectOfType<Terrain>();

            if ( string.IsNullOrEmpty(LevelName) )
            {
                Debug.LogError("Must give this level a name in field LevelName");
                return false;
            }

            if (!Mathf.Approximately(terrain.transform.position.sqrMagnitude, 0))
                Debug.LogWarning("Terrain must be placed at 0,0,0");

            if (terrain)
            {
                bool validTerrain = true;// Mathf.Approximately(terrain.terrainData.size.x, 2000) && Mathf.Approximately(terrain.terrainData.size.z, 2000) && Mathf.Approximately(terrain.terrainData.size.x, 600);
                bool validSplat = true;// terrain.terrainData.alphamapHeight == 2048;
                bool validHM = true;// terrain.terrainData.heightmapWidth == 2049;

                if (!validSplat)
                    Debug.LogError("Terrain alpha map must be of the size (2048,2048)");

                if ( !validHM)
                    Debug.LogError("Terrain heightmap must be of the size (2049,2049)");

                if (!validTerrain)
                    Debug.LogError("Terrain must be of the size (2000, 600, 2000)");

                isValid &= validTerrain && validSplat &&  validHM;
            }
            else
            {
                return false;
            }

            return isValid;
        }


        
    }
}
