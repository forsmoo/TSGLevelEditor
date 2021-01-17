//-----------------------------------------------------------------------
// <copyright file="EdgeDefinitionEditor.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Lirp
{
	[CustomEditor(typeof(EdgeDefinition))]
	[CanEditMultipleObjects]
	public class EdgeDefinitionEditor : Editor
	{
		EditorUndoManager undoManager = new EditorUndoManager();
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if ((target as EdgeDefinition).Nodes.Count < 2)
			{
				if (GUILayout.Button("Initialize"))
				{
					(target as EdgeDefinition).Initialize();
				}
			}
			else
			{


				GUILayout.Label("This will create a physics edge when in exported. This is used on half/quarter pipes in standard game, to get better ground detections");

				GUILayout.Label("Place start node with Z axis (blue) along the edge and Y on the top surface");
				GUILayout.Label("Place end node with Y axis (green) out from the second surface");
				GUILayout.Label("Press update view to get the actual edge values");

				GUILayout.Label("Tip: while placing a transform, hold [V] to snap to vertices");

				if (GUILayout.Button("Update"))
				{
					Gizmos.color = Color.black;
					(target as EdgeDefinition).UpdateDefinition();
				}
			}
		}
	}
}
