//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lirp
{
    [CustomEditor(typeof(TerrainExtension))]
    public class TerrainExtensionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainExtension ter = (TerrainExtension)target as TerrainExtension;

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Editor for terrain extension");
            EditorGUILayout.EndHorizontal();

            /*if (ter.Lightmap != null)
            {
                if (GUILayout.Button("Generate NormalMap"))
                {
                    if (ter.BakeAllTimeIndicesUpTo > 0)
                    {
                        for (int i = 0; i < ter.BakeAllTimeIndicesUpTo; i++)
                        {
                            ter.BakeTimeIndex = i;
                            TerrainMapGenerator.GenerateNormalMap(ter, AssetDatabase.GetAssetOrScenePath(ter.GetNormalMap()));
                        }
                    }
                    else
                    {
                        TerrainMapGenerator.GenerateNormalMap(ter, AssetDatabase.GetAssetOrScenePath(ter.GetNormalMap()));
                    }
                }

                if (GUILayout.Button("Generate HeightMap"))
                {
                    TerrainMapGenerator.GenerateHeightMap(ter, AssetDatabase.GetAssetOrScenePath(ter.GetHeightMap()));
                }

            }
            else
            {
                GUILayout.Label("Must create a png map (with correct settings)\n and associate it as Normal map on both the material\n and on the terrain extension");
            }*/


            if (ter.GetSplatMap() != null)
            {
                if (GUILayout.Button("Set splat from map"))
                {
                    SetSplatFromMap(ter);
                }

                if (GUILayout.Button("Save splatmap"))
                {
                    TerrainMapGenerator.SaveSplatMapFromTerrain(ter, AssetDatabase.GetAssetOrScenePath(ter.GetSplatMap()));
                }
            }

            if (GUILayout.Button("Generate splatmap from terrain"))
            {
                TerrainMapGenerator.GenerateSplatMap(ter);
            }

            if (GUILayout.Button("Smooth terrain"))
            {
                SmoothTerrain(ter);
            }

            /*if (GUILayout.Button("AddRockNoise"))
            {
                TerrainMapGenerator.GenerateRockNoise(ter);
            }*/
        }

        public static void SetSplatFromMap(TerrainExtension ter)
        {
            Terrain t = ter.GetComponent<Terrain>();
            int splatMapWidth = t.terrainData.alphamapWidth;
            float[,,] splatmapData = new float[splatMapWidth, splatMapWidth, t.terrainData.alphamapLayers];

            float tx = 0;
            float ty = 0;
            for (int y = 0; y < splatMapWidth; y++)
            {
                ty = (float)(y) / (float)splatMapWidth;
                for (int x = 0; x < splatMapWidth; x++)
                {
                    tx = (float)(x) / (float)splatMapWidth;

                    float etx = tx;
                    if (ter.FlipSplatMapX)
                        etx = 1 - tx;
                    float ety = ty;
                    if (ter.FlipSplatMapY)
                        ety = 1 - ty;
                    Color c = ter.GetSplatMap().GetPixelBilinear(ety, etx);
                    float[] ts = new float[4];
                    float total = 1;//c.g+c.b+c.a;
                                    //Normalize splats
                    ts[1] = c.g;
                    total -= ts[1];
                    ts[2] = Mathf.Clamp01(c.b * total);
                    total -= ts[2];
                    ts[3] = Mathf.Clamp01(c.r * total);
                    total -= ts[3];
                    
                    ts[0] = Mathf.Clamp01(total);
                    for (int j = 0; j < t.terrainData.alphamapLayers; j++)
                    {
                        splatmapData[x, y, j] = ts[j];
                    }
                }
            }
            t.terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        public static void SmoothTerrain(TerrainExtension ext)
        {
            Terrain t = ext.GetComponent<Terrain>();
            if (t == null)
                return;

            int heightMapWidth = t.terrainData.heightmapWidth;
            float[,] masterData = t.terrainData.GetHeights(0, 0, heightMapWidth, heightMapWidth);
            int asize = t.terrainData.alphamapWidth;
            float[,,] alphaData = t.terrainData.GetAlphamaps(0, 0, asize, asize);

            float tx = 0;
            float tz = 0;

            Vector3 pos = Vector3.zero;
            for (int x = 0; x < heightMapWidth; x++)
            {
                tx = (float)x / (float)(heightMapWidth - 1);
                for (int y = 0; y < heightMapWidth; y++)
                {
                    tz = (float)y / (float)(heightMapWidth - 1);
                    float blend = 0;
                    if (t.terrainData.alphamapLayers >= ext.AutoSettings.SmoothBlendPerSplat.Length)
                    {
                        int atx = Mathf.FloorToInt(tx * (asize - 1));
                        int aty = Mathf.FloorToInt(tz * (asize - 1));

                        for (int ic = 0; ic < ext.AutoSettings.SmoothBlendPerSplat.Length; ic++)
                        {
                            blend += alphaData[atx, aty, ic] * ext.AutoSettings.SmoothBlendPerSplat[ic];
                        }
                    }
                    else
                    {
                        blend = ext.AutoSettings.SmoothBlend;
                        //Debug.Log("Mismatch in layers");
                    }

                    List<float> accumulated = new List<float>();

                    List<int> xshifts = new List<int>();
                    List<int> yshifts = new List<int>();
                    if (x > 0)
                        xshifts.Add(-1);
                    xshifts.Add(0);
                    if (x < heightMapWidth - 1)
                        xshifts.Add(1);

                    if (y > 0)
                        yshifts.Add(-1);
                    yshifts.Add(0);
                    if (y < heightMapWidth - 1)
                        yshifts.Add(1);

                    float accumHeight = 0;
                    float numSamples = 0;
                    for (int ii = 0; ii < xshifts.Count; ii++)
                    {
                        for (int jj = 0; jj < yshifts.Count; jj++)
                        {
                            float h = masterData[x + xshifts[ii], y + yshifts[jj]];
                            accumulated.Add(h);
                            accumHeight += h;
                            numSamples += 1;
                        }
                    }

                    masterData[x, y] = Mathf.Lerp(masterData[x, y], accumHeight / numSamples, blend);
                }
            }

            t.terrainData.SetHeights(0, 0, masterData);
        }
    }

}