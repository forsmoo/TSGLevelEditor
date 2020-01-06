//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Lirp
{
	[CustomEditor(typeof(MeshTerrainMolder))]
	public class MeshTerrainMolderEditor : Editor{
	    public override void OnInspectorGUI()
	    {
	        base.OnInspectorGUI();
	        var molder = target as MeshTerrainMolder;
	
	        if ( GUILayout.Button("Mold") )
	        {
	            var undo = new EditorUndoManager();
	            molder.Mold(undo);
	        }
	    }
	}
}
