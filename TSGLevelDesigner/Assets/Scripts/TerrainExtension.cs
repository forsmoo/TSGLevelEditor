//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lirp
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainExtension : MonoBehaviour
    {

        public TerrainHeightSettings HeightSettings;
        public TerrainAutoSettings AutoSettings;
        public Color TreeColor = new Color(226, 248, 247, 0);
        public Texture2D Lightmap;
        public bool BakeMeshesIntoNormals;
#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D HeightMap = null;
        public Texture2D GetHeightMap() { return HeightMap; }
#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D NormalMap = null;
        public Texture2D GetNormalMap() { return NormalMap; }
#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D AOBounceMap = null;
        public Texture2D GetAOBounceMap() { return AOBounceMap; }

#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D ShiftMap = null;
        public Texture2D GetShiftMap() { return ShiftMap; }

        public bool NormalsInTangent = false;
#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D CastShadows = null;
        public Texture2D GetCastShadows() { return CastShadows; }
#if UNITY_EDITOR
        [SerializeField]
#endif
        Texture2D SplatMap = null;
        public Texture2D GetSplatMap() { return SplatMap; }
        public float TerrainBaseMapDistance = 3000;
        public float TreeDistance = 8000;
        public TerrainLightmapSettings TerrainLMSettings = new TerrainLightmapSettings();
        public TerrainMapSettings MapSettings;

#if UNITY_EDITOR
        [SerializeField]
#endif
        TerrainDeformSettings DeformSettings = null;
        public TerrainDeformSettings GetDeformSettings() { return DeformSettings; }


        //Hashtable htDependencies = null;
        public List<Material> terrainMaterials;
        public GameObject TreeInstances;
        public bool FlipSplatMapY = false;
        public bool FlipSplatMapX = true;
        public Vector2 SnowCoverNormalThreshold = new Vector2(0.8f, 0.9f);
        public bool OnlyAddSnowToRocks = false;
        public bool BigTerrainMode = false;
        public float DirtHeightStart = 400;
        public float DirtHeightEnd = 450;
        public float DirtHeightNormal = 0.5f;
        public Texture2D SplatGenerationMask;

        public bool SetupMaterialLightmaps = true;
        public float DirtNormalMultiplier = 1;
        public ShiftMapSettings ShiftSettings;

        public int BakeTimeIndex = 0;
        public int BakeAllTimeIndicesUpTo = 3;

        [System.Serializable()]
        public class ShiftMapSettings
        {
            public float SampleDistance = 0.1f;
            public float MaxDistance = 0.1f;
            public int Channel = 3;
            public int MaxSamples = 1;
            public int NumIterations = 5;
            public float Falloff = 0.7f;
            public float NormalAlignMin = 0.5f;
            public float NormalAlignMax = 0.9f;
        }

        public static float DecodeFloatRGBA(Vector4 enc)
        {
            Vector4 kDecodeDot = new Vector4(1.0f, 1 / 255.0f, 1 / 65025.0f, 1 / 160581375.0f);
            return Vector4.Dot(enc, kDecodeDot);
        }

        internal void SetHeightFromTexture2D(Texture2D tex)
        {
            Terrain t = GetComponent<Terrain>();

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
                    
                    var encodedHeight = tex.GetPixelBilinear(tz, tx);
                    float height = DecodeFloatRGBA(encodedHeight);
                    if (x == 0 || y == 0 || x == heightMapWidth - 1 || y == heightMapWidth - 1)
                        height = 0;
                    masterData[x, y] = Mathf.Clamp01(height);

                }
            }
            t.terrainData.SetHeights(0, 0, masterData);
        }

        public void SetSplatFromMap(Texture2D splat)
        {
            Terrain t = GetComponent<Terrain>();
            int splatMapWidth = t.terrainData.alphamapWidth;
            //int asize = t.terrainData.alphamapHeight;
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
                    if (FlipSplatMapX)
                        etx = 1 - tx;
                    float ety = ty;
                    if (FlipSplatMapY)
                        ety = 1 - ty;
                    Color c = splat.GetPixelBilinear(ety, etx);
                    float[] ts = new float[4];
                    float total = 1;
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
    }
    [System.Serializable()]
    public class TerrainLightmapSettings
    {
        public int TreeShadowSamples = 5;
        public float TreeShadowStrength = 1;
        public float TreeHeightSearch = 20;
        public float TreeRadiusSearch = 5;
        public bool DebugTreeLM = false;

        public float MinShadowSampleThickness = 1;
        public float ShadowSampleSunDistane = 1000;
        public int NumTerrainSquareShadowSamples = 3;
        public int NumTerrainSquareAOSamples = 3;
        public int NumHorizontalAOSamples = 5;
        public int NumVerticalAOSamples = 3;
        public bool DebugSamples = false;
        public float ShadowSampleOffset = 0.001f;
    }

    [System.Serializable()]
    public class TerrainAutoSettings
    {
        public float RockSlope = 0.25f;
        public float RockSlopeFalloff = 0.26f;
        public float RoughSnowSlope;
        public float RoughSnowSlopeFalloff;
        public float PushEdgeToLevel = 0;
        public float EdgeFadeLength = 5;
        public float[] SmoothBlendPerSplat = { 1, 0, 1, 1 };
        public float SmoothBlend = 1;
    }

    [System.Serializable()]
    public class TerrainMapSettings
    {
        public float TempShadowResolution = 1024;
        public int ShadowSmoothPixels = 2;
        public float ShadowFalloff = 10;
        public float SampleFilterWidth = 1f;
        public bool ShadowSampleBilinear = true;
        public float ShadowIntensity = 1;
    }

    [System.Serializable()]
    public class NoiseMapSettings
    {
        public Texture2D NoiseMap;
        public Texture2D TerrainFilterMap;
        public bool UseFilter = true;
        public float Strength = 1;
        public int Channel = 0;
        public Vector2 UVScale = new Vector2(0.1f, 0.1f);
        public float TrailDistance = 1;
        public bool enabled = false;
    }

    [System.Serializable()]
    public class TerrainDeformSettings
    {
        public List<NoiseMapSettings> channelSettings;
    }


    [System.Serializable()]
    public class TerrainHeightSettings
    {
        public Texture2D HeightBase;
        public Texture2D AdditiveHeightmap;
        public HeightDifferenceSettings diffSettings;
    }

    [System.Serializable()]
    public class HeightDifferenceSettings
    {
        public Texture2D From;
        public Texture2D To;
        public bool Additive = true;
    }

    public class HeightMapSetter
    {
        public static bool Setup(Terrain t, TerrainHeightSettings settings)
        {
            var t1 = settings.diffSettings.From;
            var t2 = settings.diffSettings.To;

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
                    tz = 1 - (float)y / (float)(heightMapWidth - 1);

                    float diff = t2.GetPixelBilinear(tx, tz).r - t1.GetPixelBilinear(tx, tz).r;
                    if (settings.diffSettings.Additive)
                        masterData[x, y] = masterData[x, y] + diff;
                    else
                        masterData[x, y] = masterData[x, y];
                }
            }

            t.terrainData.SetHeights(0, 0, masterData);
            return true;
        }

        public static bool AddHeight(Terrain t, TerrainHeightSettings settings)
        {
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
                    tz = 1 - (float)y / (float)(heightMapWidth - 1);
                }
            }

            t.terrainData.SetHeights(0, 0, masterData);
            return true;
        }
    }
}
