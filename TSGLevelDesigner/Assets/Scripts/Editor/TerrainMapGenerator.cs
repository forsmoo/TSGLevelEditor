//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;


namespace Lirp
{
	public class TerrainMapGenerator
	{
	    public static Color PackNormal(Vector3 normal, float atten)
	    {
	        normal = normal * 0.5f;
	        normal += new Vector3(0.5f, 0.5f, 0.5f);
	        return new Color(normal.x, normal.y, normal.z, atten);
	    }
	
	    public static Color PackNormal2(Vector3 normal)
	    {
	        normal = normal * 0.5f;
	        normal += new Vector3(0.5f, 0.5f, 0.5f);
	        return new Color(normal.x, normal.z, 0, 0);
	    }
	
	    public static Vector2 EncodeFloatRG(float v)
	    {
	        Vector2 kEncodeMul = new Vector2(1.0f, 255.0f);
	        float kEncodeBit = 1.0f / 255.0f;
	        Vector2 enc = kEncodeMul * v;
	        enc.x = enc.x - Mathf.Floor(enc.x);
	        enc.y = enc.y - Mathf.Floor(enc.y);
	        enc.x -= enc.y * kEncodeBit;
	        return enc;
	    }
        

        static float[] EncodeFloatRGBA(float val)
        {
            float[] kEncodeMul = new float[] { 1.0f, 255.0f, 65025.0f, 160581375.0f };
            float kEncodeBit = 1.0f / 255.0f;
            for (int i = 0; i < kEncodeMul.Length; ++i)
            {
                kEncodeMul[i] *= val;

                // Frac
                kEncodeMul[i] = (float)(kEncodeMul[i] - Math.Truncate(kEncodeMul[i]));
            }

            // enc -= enc.yzww * kEncodeBit;
            float[] yzww = new float[] { kEncodeMul[1], kEncodeMul[2], kEncodeMul[3], kEncodeMul[3] };
            for (int i = 0; i < kEncodeMul.Length; ++i)
            {
                kEncodeMul[i] -= yzww[i] * kEncodeBit;
            }

            return kEncodeMul;
        }

        public static float DecodeFloatRGBA(Vector4 enc)
        {
            Vector4 kDecodeDot = new Vector4(1.0f, 1 / 255.0f, 1 / 65025.0f, 1 / 160581375.0f);
            return Vector4.Dot(enc, kDecodeDot);
        }

        public static Color PackFloat(float f)
        {
            if (f > 1.0f)
                Debug.LogError("Can not encode that");

            Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
            float kEncodeBit = 1.0f / 255.0f;
            Vector4 enc = kEncodeMul * f;
            enc.x = enc.x - Mathf.Floor(enc.x);
            enc.y = enc.y - Mathf.Floor(enc.y);
            enc.z = enc.y - Mathf.Floor(enc.z);
            enc.w = enc.z - Mathf.Floor(enc.w);

            enc.x -= enc.y * kEncodeBit;
            enc.y -= enc.z * kEncodeBit;
            enc.z -= enc.w * kEncodeBit;
            enc.w -= enc.w * kEncodeBit;

            //enc -= enc.yzww * kEncodeBit;
            return enc;
        }


        public static void SaveSplatMapFromTerrain(TerrainExtension ter,string path)
	    {
	        Terrain t = ter.GetComponent<Terrain>();
	
	        int splatMapWidth = t.terrainData.alphamapWidth;
	        float[,,] splatmapData = t.terrainData.GetAlphamaps(0, 0, splatMapWidth, splatMapWidth);
            var targetSplatmap = ter.GetSplatMap();
            
            Color [] colors = new Color[splatMapWidth * splatMapWidth];
            if (splatmapData != null && t.terrainData.splatPrototypes.Length > 1)
            {
                for (int j = 0; j < targetSplatmap.height; j++)
                {
                    for (int i = 0; i < targetSplatmap.width; i++)
                    {
                        float tu = i / ((float)targetSplatmap.width);
                        float tv = j / ((float)targetSplatmap.height);

                        Color splatC = Color.red;

                        int sxi = Mathf.Clamp(Mathf.RoundToInt((float)splatMapWidth * tv), 0, splatMapWidth - 1);
                        int syi = Mathf.Clamp(Mathf.RoundToInt((float)splatMapWidth * tu), 0, splatMapWidth - 1);
                        splatC.r = splatmapData[sxi, syi, 3];
                        splatC.g = splatmapData[sxi, syi, 1];
                        splatC.b = splatmapData[sxi, syi, 2];
                        splatC.a = 1;

                        colors[j * targetSplatmap.height + i] = splatC;
                        //targetSplatmap.SetPixel(i, j, splatC);
                        
                    }
                }
            }

            targetSplatmap.SetPixels(colors);

            byte[] bytes = targetSplatmap.EncodeToPNG();
	
	        //string path = AssetDatabase.GetAssetOrScenePath(ter.GetSplatMap());
	        Debug.Log("Writing to path" + path);
	        if (!string.IsNullOrEmpty(path))
	            File.WriteAllBytes(path, bytes);
	    }

        public static void GenerateHeightMap(TerrainExtension ter,string path)
        {
            Texture2D heightMap = ter.GetHeightMap();

            if (heightMap.format != TextureFormat.RGBA32)
            {
                Debug.LogError("Format must be RGBA32 it is now " + heightMap.format);
                return;
            }
            Terrain t = ter.GetComponent<Terrain>();
            float pixelWidthT = 1 / (float)heightMap.width;
            float pixelWidthT2 = pixelWidthT * 0.5f;

            int splatMapWidth = t.terrainData.alphamapWidth;
            float[,,] splatmapData = t.terrainData.GetAlphamaps(0, 0, splatMapWidth, splatMapWidth);

            float sh = TerrainManager.FindInstance().SnowHeight;
            for (int j = 0; j < heightMap.height; j++)
            {
                for (int i = 0; i < heightMap.width; i++)
                {
                    float tu = i / ((float)heightMap.width) + pixelWidthT2;
                    float tv = j / ((float)heightMap.height) + pixelWidthT2;

                    int sxi = Mathf.Clamp(Mathf.RoundToInt((float)splatMapWidth * tv), 0, splatMapWidth - 1);
                    int syi = Mathf.Clamp(Mathf.RoundToInt((float)splatMapWidth * tu), 0, splatMapWidth - 1);
                    float snowDepth = sh * (splatmapData[sxi, syi, 0]+splatmapData[sxi, syi, 2]);
                    
                    Vector3 normal = t.terrainData.GetInterpolatedNormal(tu, tv);
                    normal = ter.transform.InverseTransformDirection(normal);

                    Vector3 pos = t.GetPosition();
                    pos.x += tu * t.terrainData.size.x;//+pixelWidth*0.5f;
                    pos.z += tv * t.terrainData.size.z;//+pixelWidth*0.5f;
                    
                    float intHeight = t.terrainData.GetInterpolatedHeight(tu, tv)+ snowDepth;
                    //pos.y += intHeight;
                    //pos.y += snowDepth;

                    float h = Mathf.Clamp01((intHeight) / t.terrainData.size.y);
                    float[] encs = EncodeFloatRGBA(h);
                    Color c = new Color(encs[0], encs[1], encs[2], encs[3]);
                    heightMap.SetPixel(i, j, c);
                }
            }
            heightMap.Apply();

            byte[] bytes = heightMap.EncodeToPNG();

            //string path = AssetDatabase.GetAssetOrScenePath(heightMap);
            Debug.Log("Writing to path" + path);
            if (!string.IsNullOrEmpty(path))
                File.WriteAllBytes(path, bytes);
        }

        public static void GenerateNormalMap(TerrainExtension ter,string path)
	    {
	        Texture2D normalMap = ter.GetNormalMap();
	
	        if (normalMap.format != TextureFormat.RGBA32)
	        {
	            Debug.LogError("Format must be RGBA32 it is now " + normalMap.format);
	            return;
	        }
	        Terrain t = ter.GetComponent<Terrain>();
	        float pixelWidthT = 1 / (float)normalMap.width;
	        float pixelWidthT2 = pixelWidthT * 0.5f;
	        for (int j = 0; j < normalMap.height; j++)
	        {
	            for (int i = 0; i < normalMap.width; i++)
	            {
	                float tu = i / ((float)normalMap.width) + pixelWidthT2;
	                float tv = j / ((float)normalMap.height) + pixelWidthT2;
	                Vector3 normal = t.terrainData.GetInterpolatedNormal(tu, tv);
	                normal = ter.transform.InverseTransformDirection(normal);
	
	                Vector3 pos = t.GetPosition();
	                pos.x += tu * t.terrainData.size.x;//+pixelWidth*0.5f;
	                pos.z += tv * t.terrainData.size.z;//+pixelWidth*0.5f;
	                pos.y += t.terrainData.GetInterpolatedHeight(tu, tv);

                    if (ter.BakeMeshesIntoNormals)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(pos + Vector3.up * 100, Vector3.down, out hit, 1000, Masks.GroundMask))
                        {
                            MeshCollider meshCollider = hit.collider as MeshCollider;
                            float diff = hit.point.y - pos.y;
                            float blendStrength = Mathf.Clamp01((diff - 0.2f) / 0.8f);

                            if (meshCollider != null && meshCollider.sharedMesh != null)
                            {
                                //var modifier = Find.RecursiveParent<MeshModifier>(meshCollider.gameObject.transform);

                                Mesh mesh = meshCollider.sharedMesh;

                                Vector3[] normals = mesh.normals;
                                int[] triangles = mesh.triangles;

                                Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
                                Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
                                Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

                                Vector3 baryCenter = hit.barycentricCoordinate;
                               
                                var newNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
                                //if (modifier == null || !modifier.Settings.OutputWorldNormalTangent)
                                //    newNormal = hit.collider.transform.TransformDirection(newNormal);
                                normal = Vector3.Lerp(normal, ter.transform.InverseTransformDirection(newNormal), blendStrength).normalized;
                            }
                        }
                    }
	
	                Color shadowC = normalMap.GetPixel(i, j);
	                Color c = PackNormal(normal.normalized, 1);
	                normalMap.SetPixel(i, j, c);
	            }
	        }
	        normalMap.Apply();
	
	        byte[] bytes = normalMap.EncodeToPNG();
	
	        Debug.Log("Writing to path" + path);
	        if (!string.IsNullOrEmpty(path))
	            File.WriteAllBytes(path, bytes);
	    }
	
	    void SetMatrixColumn(int index, Matrix4x4 matrix, Vector3 vec)
	    {
	        matrix[0, index] = vec.x;
	        matrix[1, index] = vec.y;
	        matrix[2, index] = vec.z;
	    }
	
	    void SetMatrixRow(int index, Matrix4x4 matrix, Vector3 vec)
	    {
	        matrix[index, 0] = vec.x;
	        matrix[index, 1] = vec.y;
	        matrix[index, 2] = vec.z;
	    }
	
	    public static void GenerateSplatMap(TerrainExtension ter)
	    {
	
	        Terrain t = ter.GetComponent<Terrain>();
	
	        int splatMapWidth = t.terrainData.alphamapWidth;
	
	        float[,,] splatmapData = new float[splatMapWidth, splatMapWidth, t.terrainData.alphamapLayers];
	        float pixelWidthT = 1 / (float)splatMapWidth;
	        float pixelWidthT2 = pixelWidthT * 0.5f;
	
	        float tx = 0;
	        float ty = 0;
	        for (int y = 0; y < splatMapWidth; y++)
	        {
	            ty = (float)(y) / (float)splatMapWidth;
	            for (int x = 0; x < splatMapWidth; x++)
	            {
	                tx = (float)(x) / (float)splatMapWidth;
	
	                float tu = y / ((float)splatMapWidth) + pixelWidthT2;
	                float tv = x / ((float)splatMapWidth) + pixelWidthT2;
	                Vector3 normal = t.terrainData.GetInterpolatedNormal(tu, tv);
	                normal = ter.transform.InverseTransformDirection(normal);
	
	                Vector3 pos = t.GetPosition();
	                pos.x += tu * t.terrainData.size.x;
	                pos.z += tv * t.terrainData.size.z;
	                pos.y += t.terrainData.GetInterpolatedHeight(tu, tv);
	
	                float[] ts = new float[4];
	                float total = 1;
	                float rockStrength = splatmapData[x, y, 3];
	
	                Color mask = Color.white;
	                mask.a = 1;
	                if (ter.SplatGenerationMask != null)
	                {
	                    mask = ter.SplatGenerationMask.GetPixelBilinear(tu, tv);
	                }
	
	                float cov = 1 - Mathf.Clamp01((normal.y - ter.SnowCoverNormalThreshold.x) / (ter.SnowCoverNormalThreshold.y - ter.SnowCoverNormalThreshold.x));
	
	                float dirtFade = 1 - (pos.y - ter.DirtHeightStart) / (ter.DirtHeightEnd - ter.DirtHeightStart);
	                float grassStrength = Mathf.Clamp01(dirtFade + normal.y * normal.y * normal.y * ter.DirtNormalMultiplier);
	
	                if (ter.BigTerrainMode)
	                {
	                    ts[1] = cov * grassStrength;
	                    ts[3] = cov * (1 - grassStrength);
	                    total = 1 - (ts[1] + ts[3]);
	                    ts[2] = total * grassStrength;
	                    ts[0] = total * (1 - grassStrength);
	                }
	                else
	                {
	                    ts[1] = splatmapData[x, y, 1];
	                    total -= ts[1];
	                    ts[2] = splatmapData[x, y, 2];
	                    total -= ts[2];
	                    ts[3] = 0;
	
	                    if ((rockStrength > 0 && ter.OnlyAddSnowToRocks) || !ter.OnlyAddSnowToRocks)
	                    {
	                        ts[3] = Mathf.Clamp01(cov * total);
	                    }
	                    total -= ts[3];
	                    ts[0] = Mathf.Clamp01(total);
	                }
	                
	                splatmapData[x, y, 0] = Mathf.Lerp(splatmapData[x, y, 0], ts[0], mask.r);
	                splatmapData[x, y, 1] = Mathf.Lerp(splatmapData[x, y, 1], ts[1], mask.g);
	                splatmapData[x, y, 2] = Mathf.Lerp(splatmapData[x, y, 2], ts[2], mask.b);
	                splatmapData[x, y, 3] = Mathf.Lerp(splatmapData[x, y, 3], ts[3], mask.a);
	
	            }
	        }
	        t.terrainData.SetAlphamaps(0, 0, splatmapData);
	    }
	
	    internal static void GenerateRockNoise(TerrainExtension ter)
	    {
	        Terrain t = ter.GetComponent<Terrain>();
	
	        if (t == null)
	            return;
	
	        int heightMapWidth = t.terrainData.heightmapWidth;
	        float[,] masterData = t.terrainData.GetHeights(0, 0, heightMapWidth, heightMapWidth);
	        int asize = t.terrainData.alphamapWidth;
	        float[,,] alphaData = t.terrainData.GetAlphamaps(0, 0, asize, asize);
	
	        float tx = 0;
	        float tz = 0;
	
	        float addHeightScale = 1 / t.terrainData.size.y;
	        Vector3 pos = Vector3.zero;
	        for (int x = 0; x < heightMapWidth; x++)
	        {
	            tx = (float)x / (float)(heightMapWidth - 1);
	            for (int y = 0; y < heightMapWidth; y++)
	            {
	                tz = (float)y / (float)(heightMapWidth - 1);
	
	                int atx = Mathf.FloorToInt(tx * (asize - 1));
	                int aty = Mathf.FloorToInt(tz * (asize - 1));
	
	                pos = t.transform.position;
	                pos.z += t.terrainData.size.x * (((float)x) / ((float)heightMapWidth - 1.0f));
	                pos.x += t.terrainData.size.z * (((float)y) / ((float)heightMapWidth - 1.0f));
	                float heightAdd = 0;
	                for (int i = 0; i < ter.GetDeformSettings().channelSettings.Count; i++)
	                {
	                    var settings = ter.GetDeformSettings().channelSettings[i];
	                    if (settings.enabled)
	                    {
	                        float typeStrength = alphaData[atx, aty, settings.Channel];
	                        float noise = settings.NoiseMap.GetPixelBilinear(pos.x * settings.UVScale.x, pos.z * settings.UVScale.y).r;
	                        if (settings.UseFilter && settings.TerrainFilterMap != null)
	                        {
	                            float filter = settings.TerrainFilterMap.GetPixelBilinear(tz,tx).r;
	                            typeStrength = filter;
	                        }
	                        heightAdd += typeStrength * settings.Strength * addHeightScale * noise;
	                    }
	                }
	                masterData[x, y] = Mathf.Clamp01(masterData[x, y] + heightAdd);
	            }
	        }
	
	        t.terrainData.SetHeights(0, 0, masterData);
	    }
	
    }
	
}
