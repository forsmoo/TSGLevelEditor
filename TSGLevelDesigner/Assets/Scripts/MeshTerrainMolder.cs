//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Lirp
{
	public class MeshTerrainMolder : MonoBehaviour {
	
	    public float SaftyMargin = 10;
	    public float OffsetY = 0;
	    public bool Additive = false;
	    public bool DoNotAddHeight = false;
	    public bool InvertStrength = false;
	    public bool EnableUndo = true;
        public bool StrengthFromColor;

        public void Mold(IUndoManager undo)
	    {
	        MeshCollider mc = GetComponent<MeshCollider>();
	
	        if( mc != null && mc.sharedMesh != null )
	        {
	            Terrain t = TerrainManager.GetTerrain(transform.position);
	            if (t != null)
	            {
	                if(EnableUndo)
	                    undo.RecordObject(t.terrainData, "Molded " + t.terrainData.name);
	                Debug.Log(TerrainMeshMold.MoldToMesh(t, mc, Additive, SaftyMargin, OffsetY, StrengthFromColor,DoNotAddHeight, InvertStrength));
	            }
	            else
	            {
	                Debug.Log("Couldn't find terrain");
	            }
	        }
	        else
	        {
	            Debug.LogError("Needs meshcollider");
	        }
	    }
	}
}
