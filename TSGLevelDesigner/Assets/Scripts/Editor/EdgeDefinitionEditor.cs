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
			GUILayout.Label("This will create a physics edge when in exported. This is used on half/quarter pipes in standard game, to get better board ground detections");

			GUILayout.Label("Place each node with Z axis (blue) along the edge");
			GUILayout.Label("Place each node with Y axis (greem) top surface");
			GUILayout.Label("This will give a hint of how the edge is created. It will autodetect the edge when the game is started, so make sure these are placed properly or there might be funny physics bugs");

			if (GUILayout.Button("Update view"))
			{
				Gizmos.color = Color.black;
				(target as EdgeDefinition).UpdateDefinition();
			}
		}
	}
}
