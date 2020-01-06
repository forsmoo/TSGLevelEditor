//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lirp
{
    public class CustomPrefab : MonoBehaviour
    {
        public bool Scalable = true;
        public string PrefabID;

        private void Update()
        {
            if (!Scalable && !Mathf.Approximately(transform.localScale.sqrMagnitude, 1))
            {
                transform.localScale = Vector3.one;
            }
        }
    }
}
