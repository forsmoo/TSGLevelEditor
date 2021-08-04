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
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGLTF;

namespace Lirp
{
    public class SpawnName : MonoBehaviour
    {
        public string Caption = "Spawn";
        public int SortOrder = 1;
        public bool IsVisible = true;
    }
}
