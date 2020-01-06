//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lirp
{
    public class TerrainManager : MonoBehaviour
    {
        [NonSerialized()]
        public float SnowHeight = 0.15f;

        public static Terrain GetTerrain(Vector3 worldPos)
        {
            return FindObjectOfType<Terrain>();
        }

        public static RaycastHit? GetTerrainHitOnly(Vector3 pos, Vector3 dir)
        {
            Ray ray = new Ray();
            RaycastHit hit;
            ray.origin = pos;
            ray.direction = dir.normalized;
            if (Physics.Raycast(ray, out hit, 150.0f, Masks.TerrainMask))
            {
                return hit;
            }

            return null;
        }

        public static RaycastHit? GetGroundHit(Vector3 pos, Vector3 dir)
        {
            Ray ray = new Ray();
            RaycastHit hit;
            int GroundMask;
            GroundMask = Masks.GroundMask;
            ray.origin = pos;
            ray.direction = dir;
            if (Physics.Raycast(ray, out hit, 40000.0f, GroundMask))
            {
                return hit;
            }
            return null;
        }

        public static RaycastHit? GetGroundHit(Vector3 pos)
        {
            Ray ray = new Ray();
            RaycastHit hit;
            int GroundMask;
            GroundMask = Masks.GroundMask;
            ray.origin = pos + Vector3.up * 2000;
            ray.direction = Vector3.down;
            if (Physics.Raycast(ray, out hit, 40000.0f, GroundMask))
            {
                return hit;
            }
            return null;
        }

        public static Vector3? GetGroundPoint(Vector3 pos)
        {
            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = pos + Vector3.up * 2000;
            ray.direction = Vector3.down;

            if (Physics.Raycast(ray, out hit, 40000.0f, Masks.GroundMask))
            {
                return hit.point;
            }
            return null;
        }

        public static TerrainManager FindInstance()
        {
            return FindObjectOfType<TerrainManager>();
        }

        public static float GetHeightInTerrainSpace(float height, Terrain terrain)
        {
            return (height - terrain.transform.position.y) / terrain.terrainData.size.y;
        }
    }

    public class Masks
    {
        public static int GroundMask = 1 << 8 | 1 << 12 | 1 << 20 | 1 << 23 | 1 << 24 | 1 << 28;
        public static int TerrainMask = 1 << 8;
    }
    public class Layers
    {
        public static int Terrain = 8;
        public static int Deep = 20;
        public static int Groomed = 24;
    }
}
