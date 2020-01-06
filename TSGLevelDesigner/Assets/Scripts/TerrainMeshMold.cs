//-----------------------------------------------------------------------
// <copyright file="TerrainMeshMold.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Lirp
{
	public class TerrainMeshMold {
		
		public static string MoldToMesh(Terrain terrain,MeshCollider collider,bool MoldAdditive,float saftyMargin,float offset, bool strengthFromColor, bool doNotAddHeight=false, bool invertStrength=false)
		{
			if( collider == null )
				return "error no collider";
	
			Transform parent = collider.transform.parent;
			if( parent != null )
				collider.transform.parent = null;
	
			if (!terrain.GetComponent<Collider>().bounds.Intersects(collider.bounds))
				return ".";
		
	
			Vector3 terrainPos = terrain.transform.position;
			Vector3 terrainSize = terrain.terrainData.size;
			Vector2 tmin = Vector2.zero;
			Vector2 tmax = Vector2.zero;
			tmin.x = (collider.bounds.min.x-terrainPos.x)/terrainSize.x;
			tmin.y = (collider.bounds.min.z-terrainPos.z)/terrainSize.z;
			tmax.x = (collider.bounds.max.x-terrainPos.x)/terrainSize.x;
			tmax.y = (collider.bounds.max.z-terrainPos.z)/terrainSize.z;
	
			int minIndexX = Mathf.FloorToInt(tmin.x*terrain.terrainData.heightmapWidth-saftyMargin);
			int minIndexY = Mathf.FloorToInt(tmin.y*terrain.terrainData.heightmapHeight-saftyMargin);
			int maxIndexX = Mathf.CeilToInt(tmax.x*terrain.terrainData.heightmapWidth+saftyMargin);
			int maxIndexY = Mathf.CeilToInt(tmax.y*terrain.terrainData.heightmapHeight+saftyMargin);
	
			//Make sure rectangle is inside terrain
			minIndexX = Mathf.Clamp(minIndexX,0,terrain.terrainData.heightmapWidth-1);
			minIndexY = Mathf.Clamp(minIndexY,0,terrain.terrainData.heightmapHeight-1);
			maxIndexX = Mathf.Clamp(maxIndexX,0,terrain.terrainData.heightmapWidth-1);
			maxIndexY = Mathf.Clamp(maxIndexY,0,terrain.terrainData.heightmapHeight-1);
			
			int mapWidth = maxIndexX - minIndexX;
			int mapHeight = maxIndexY - minIndexY;
			
			float[,] heights = terrain.terrainData.GetHeights(minIndexX, minIndexY, mapWidth, mapHeight);
			Vector3 worldPos;
			
			for( int iz=0;iz<mapHeight;iz++)
			{
				for( int ix=0;ix<mapWidth;ix++)
				{
					float originalHeight = heights[iz,ix];
					float height = terrainPos.y+originalHeight*terrain.terrainData.size.y;
					worldPos = terrainPos;
	
					worldPos.x += terrain.terrainData.size.x * ((float)(minIndexX + ix)) / ((float)(terrain.terrainData.heightmapWidth-1));
					worldPos.y += height;
					worldPos.z += terrain.terrainData.size.z * ((float)(minIndexY + iz)) / ((float)(terrain.terrainData.heightmapHeight-1));
					
					
					float strength = 1;
					float newHeight = height;
	
					Vector3 rayOrigin = worldPos;
					rayOrigin.y = terrain.transform.position.y + terrain.terrainData.size.y;
					Ray ray = new Ray(rayOrigin,Vector3.down);
					RaycastHit hit;
					
					if( collider.Raycast(ray,out hit,terrain.terrainData.size.y*10) ) 
					{
                        bool hasError = false;
                        try
                        {
                            
                            if (strengthFromColor && collider.sharedMesh.colors != null && collider.sharedMesh.triangles.Length > hit.triangleIndex * 3 + 3)
                            {
                                int[] triangles = collider.sharedMesh.triangles;
                                float[] barys = new float[3];
                                barys[0] = hit.barycentricCoordinate.x;
                                barys[1] = hit.barycentricCoordinate.y;
                                barys[2] = hit.barycentricCoordinate.z;

                                float tsmooth = 0;
                                for (int i = 0; i < 3; i++)
                                {
                                    tsmooth += collider.sharedMesh.colors[triangles[hit.triangleIndex * 3 + i]].a * barys[i];
                                }

                                strength = tsmooth;
                            }
                            else
                            {
                                strength = 1;
                            }
                        }
                        catch
                        {
                            hasError = true;
                        }

                        if (hasError)
                            strength = 1;

	                    if (invertStrength)
	                        strength = 1 - strength;
						newHeight = hit.point.y-offset*strength;
					}
					else
					{
						//newHeight = 1;
					}
	
					if( MoldAdditive )
					{
						newHeight = Mathf.Max(height,newHeight);
					}
	                float targetHeight = TerrainManager.GetHeightInTerrainSpace(newHeight, terrain);
	
	                if (doNotAddHeight)
	                    targetHeight = Mathf.Min(originalHeight, targetHeight);
	                heights[iz,ix] = Mathf.Lerp(originalHeight,targetHeight,strength);
				}
			}
	
			terrain.terrainData.SetHeights(minIndexX,minIndexY,heights);
			collider.transform.parent = parent;
			return "ok";
		}
	
	    static Vector3 GetTerrainPos(Terrain terrain, int ix, int iz,float ht)
	    {
	        Vector3 worldPos = terrain.GetPosition();
	        worldPos.x += terrain.terrainData.size.x * ((float)(ix)) / ((float)(terrain.terrainData.heightmapWidth - 1));
	        worldPos.y += ht* terrain.terrainData.size.y;
	        worldPos.z += terrain.terrainData.size.z * ((float)(iz)) / ((float)(terrain.terrainData.heightmapHeight - 1));
	        return worldPos;
	    }
	
	    public static string _MoldToMeshByTerrainPolygon(Terrain terrain, MeshCollider collider, bool MoldAdditive, float saftyMargin, float offset, bool doNotAddHeight = false)
	    {
	        if (collider == null)
	            return "error no collider";
	
	        Transform parent = collider.transform.parent;
	        if (parent != null)
	            collider.transform.parent = null;
	
	        if (!terrain.GetComponent<Collider>().bounds.Intersects(collider.bounds))
	            return ".";
	
	
	        Vector3 terrainPos = terrain.transform.position;
	        Vector3 terrainSize = terrain.terrainData.size;
	        Vector2 tmin = Vector2.zero;
	        Vector2 tmax = Vector2.zero;
	        tmin.x = (collider.bounds.min.x - terrainPos.x) / terrainSize.x;
	        tmin.y = (collider.bounds.min.z - terrainPos.z) / terrainSize.z;
	        tmax.x = (collider.bounds.max.x - terrainPos.x) / terrainSize.x;
	        tmax.y = (collider.bounds.max.z - terrainPos.z) / terrainSize.z;
	
	        int minIndexX = Mathf.FloorToInt(tmin.x * terrain.terrainData.heightmapWidth - saftyMargin);
	        int minIndexY = Mathf.FloorToInt(tmin.y * terrain.terrainData.heightmapHeight - saftyMargin);
	        int maxIndexX = Mathf.CeilToInt(tmax.x * terrain.terrainData.heightmapWidth + saftyMargin);
	        int maxIndexY = Mathf.CeilToInt(tmax.y * terrain.terrainData.heightmapHeight + saftyMargin);
	
	        //Make sure rectangle is inside terrain
	        minIndexX = Mathf.Clamp(minIndexX, 0, terrain.terrainData.heightmapWidth - 1);
	        minIndexY = Mathf.Clamp(minIndexY, 0, terrain.terrainData.heightmapHeight - 1);
	        maxIndexX = Mathf.Clamp(maxIndexX, 0, terrain.terrainData.heightmapWidth - 1);
	        maxIndexY = Mathf.Clamp(maxIndexY, 0, terrain.terrainData.heightmapHeight - 1);
	
	        int mapWidth = maxIndexX - minIndexX;
	        int mapHeight = maxIndexY - minIndexY;
	
	        float[,] heights = terrain.terrainData.GetHeights(minIndexX, minIndexY, mapWidth, mapHeight);

	        for (int iz = 0; iz < mapHeight-1; iz++)
	        {
	            for (int ix = 0; ix < mapWidth-1; ix++)
	            {
	                float[] originalHeight = new float[4];
	                float[] targetHeight = new float[4];
	                float strength = 1;
	                float[] newHeight = new float[4];
	                Vector3[] terrainPositions = new Vector3[4];
	
	                originalHeight[0] = heights[iz, ix];
	                originalHeight[1] = heights[iz, ix + 1];
	                originalHeight[2] = heights[iz + 1, ix + 1];
	                originalHeight[3] = heights[iz + 1, ix];
	
	                terrainPositions[0] = GetTerrainPos(terrain, minIndexX + ix, minIndexY + iz, originalHeight[0]);
	                terrainPositions[1] = GetTerrainPos(terrain, minIndexX + ix+1, minIndexY + iz, originalHeight[1]);
	                terrainPositions[2] = GetTerrainPos(terrain, minIndexX + ix+1, minIndexY + iz+1, originalHeight[2]);
	                terrainPositions[3] = GetTerrainPos(terrain, minIndexX + ix, minIndexY + iz+1, originalHeight[3]);
	
	                Vector3 rayOrigin = terrainPositions[0];
	                rayOrigin.y = terrain.transform.position.y + terrain.terrainData.size.y;
	                Ray ray = new Ray(rayOrigin, Vector3.down);
	                RaycastHit hit;
	
	                for (int i = 0; i < 4; i++)
	                {
	                    if (collider.Raycast(ray, out hit, terrain.terrainData.size.y * 10))
	                    {
	                        
	                        int[] triangles = collider.sharedMesh.triangles;
	                        float[] barys = new float[3];
	                        barys[0] = hit.barycentricCoordinate.x;
	                        barys[1] = hit.barycentricCoordinate.y;
	                        barys[2] = hit.barycentricCoordinate.z;
	
	                        if (collider.sharedMesh.colors != null)
	                        {
	                            float tsmooth = 0;
	                            for (int j = 0; j < 3; j++)
	                            {
	                                tsmooth += collider.sharedMesh.colors[triangles[hit.triangleIndex * 3 + j]].a * barys[j];
	                            }
	
	                            strength = tsmooth;
	                        }
	
	                        newHeight[i] = hit.point.y - offset;
	                    }
	                    else
	                    {
	                        //newHeight = 1;
	                    }
	                }
	
	                if (MoldAdditive)
	                {
	                    for (int i = 0; i < 4; i++)
	                        newHeight[i] = Mathf.Max(terrainPositions[i].y, newHeight[i]);
	                }
	
	                for (int i = 0; i < 4; i++)
	                    targetHeight[i] = TerrainManager.GetHeightInTerrainSpace(newHeight[0], terrain);
	
	                if (doNotAddHeight)
	                {
	                    for (int i = 0; i < 4; i++)
	                        targetHeight[i] = Mathf.Min(originalHeight[i], targetHeight[i]);
	                }
	
	                heights[iz, ix] = Mathf.Lerp(originalHeight[0], targetHeight[0], strength);
	                heights[iz, ix + 1] = Mathf.Lerp(originalHeight[1], targetHeight[1], strength);
	                heights[iz+1, ix+1] = Mathf.Lerp(originalHeight[2], targetHeight[2], strength);
	                heights[iz+1, ix] = Mathf.Lerp(originalHeight[3], targetHeight[3], strength);
	            }
	        }
	
	        terrain.terrainData.SetHeights(minIndexX, minIndexY, heights);
	        collider.transform.parent = parent;
	        return "ok";
	    }
	}
}
