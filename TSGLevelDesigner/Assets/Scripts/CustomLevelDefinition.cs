//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lirp
{
    [System.Serializable()]
    public class CustomLevelDefinition
    {
        public string LevelName;
        public List<LevelPrefabInstance> Placeables;
        public List<LevelRail> Rails;
        public List<LevelEdge> Edges;
        public List<Spawn> Spawns;
        public Vector3 LobbyPosition;
        public Quaternion LobbyOrientation;
        public Vector3 LobbyCameraPosition;
        public Quaternion LobbyCameraRotation;
        public float TerrainWidth;
        public float TerrainHeight;
        public Vector3 TerrainPosition;
        public float TreeDensity;
        public float GrassDensity;
        public float PlantDensity;
        public bool ManualTrees;
        public List<TreeExportInstance> Trees = new List<TreeExportInstance>();
    }

    [System.Serializable()]
    public class TreeExportInstance
    {
        public int PrefabIndex;
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }
    [System.Serializable()]
    public class Spawn
    {
        public int SpawnType;
        public Vector3 Position;
        public Vector3 Direction;
        public float Velocity;
    }

    [System.Serializable()]
    public class LevelPrefabInstance
    {
        public Vector3 Position;
        public Vector3 LocalScale;
        public Quaternion Rotation;
        public string PrefabID;
    }


    [System.Serializable()]
    public class LevelRail
    {
        public List<RailComponent> railParts = new List<RailComponent>();
    }

    [System.Serializable()]
    public class RailComponent
    {
        public string Name;
        public Vector3 Position;
        public Vector3 LocalScale;
        public Quaternion Rotation;
        public float radius;
        public float length;
    }


    [System.Serializable()]
    public class LevelEdge
    {
        public List<EdgeCompoennt> edgeParts;
    }


    [System.Serializable()]
    public class EdgeCompoennt
    {
        public string Name;
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 F1;
        public Vector3 N1;
        public Vector3 F2;
        public Vector3 N2;
        public int Material;
    }
}
