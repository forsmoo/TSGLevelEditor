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
	public class GroundAlign : MonoBehaviour {
		
		public GameObject ObjectToAlignTo;
	    public Vector3 Offset;
	    public AlignAxis FromAxis = AlignAxis.Z;
	    public bool InvertAxis = false;
	    public bool IgnoreBatchAlign = false;
		public bool OnlyAlignToTerrain = true;
	    
		public void Align()
		{
	        AlignToGround(this);
			AlignToGroundNormal(this);
		}
	
		public static void AlignToGround(GroundAlign ga,IUndoManager undo = null)
		{
	        if (undo != null)
	            undo.RecordTransform("Align",ga.transform);
	
	        bool resetLayer = false;
			if( LayerMask.LayerToName(ga.gameObject.layer) == "Environment" )
			{
				ga.gameObject.layer = LayerMask.NameToLayer("Default");
				resetLayer = true;
			}
			
			RaycastHit? hit;
			if( ga.OnlyAlignToTerrain )
				hit = TerrainManager.GetTerrainHitOnly(ga.transform.position+Vector3.up*100,Vector3.down);
			else
				hit = TerrainManager.GetGroundHit(ga.transform.position);
	
			if (hit != null)
				ga.transform.position = hit.Value.point + ga.Offset;
			
			if( resetLayer )
				ga.gameObject.layer = LayerMask.NameToLayer("Environment");
		}
		
		public static void AlignToGroundNormal(GroundAlign ga, IUndoManager undo = null)
		{
	        if( undo != null )
	            undo.RecordTransform("Align",ga.transform);
	
			bool resetLayer = false;
			if( LayerMask.LayerToName(ga.gameObject.layer) == "Environment" )
			{
				ga.gameObject.layer = LayerMask.NameToLayer("Default");
				resetLayer = true;
			}
			
			RaycastHit? hit;
			if( ga.OnlyAlignToTerrain )
				hit = TerrainManager.GetTerrainHitOnly(ga.transform.position+Vector3.up*100,Vector3.down);
			else
				hit = TerrainManager.GetGroundHit(ga.transform.position);
			
	
			if (hit.HasValue)
			{
	
				Vector3 fromv = GetTransfomAxis(ga.transform,ga.FromAxis,ga.InvertAxis);
				
				ga.transform.rotation = Quaternion.FromToRotation(fromv,hit.Value.normal) * ga.transform.rotation;
			}
			else
			{
	
			}
			
			if( resetLayer )
				ga.gameObject.layer = LayerMask.NameToLayer("Environment");
		}
	
		public static Vector3 GetTransfomAxis(Transform t,AlignAxis axis, bool invert)
		{
			float mod = 1;
			if( invert )
				mod = -1;
			
			if( axis == AlignAxis.X )
				return t.right*mod;
			else if(axis == AlignAxis.Y )
				return t.up*mod;
			else if( axis == AlignAxis.Z )
				return t.forward*mod;
			
			return Vector3.up;
		}
	
	    public static void AlignToObject(GroundAlign ga)
	    {
	        ga.transform.position = ga.ObjectToAlignTo.transform.position; ;
	    }
	
	    public static void SetGroundOffset(Transform t)
	    {
	        Vector3? pos = TerrainManager.GetGroundPoint(t.position);
	        if (pos.HasValue)
	            t.position = pos.Value;
	    }
	}
	
	
}
