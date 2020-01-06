//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Lirp
{
	public class EditorUndoManager : IUndoManager
	{
	    public void RecordObject(UnityEngine.Object objectToUndo, string name)
	    {
	        Undo.RecordObject(objectToUndo, name);
	    }
	
	    public void RecordTransform(string name, Transform t)
	    {
	        Undo.RecordObject(t, name);
	    }
	}
}
