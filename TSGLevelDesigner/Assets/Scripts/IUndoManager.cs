//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Lirp
{
	public interface IUndoManager
	{
	    void RecordTransform(string name,Transform t);
	    void RecordObject(UnityEngine.Object objectToUndo,string name);
	}
}
