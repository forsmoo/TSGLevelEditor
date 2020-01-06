//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
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
	[CustomEditor(typeof(GroundAlign))]
	[CanEditMultipleObjects]
	public class GroundAlignEditor : Editor
	{
	    //static string message;
		//GroundAlign gaRoot;
	    EditorUndoManager undoManager = new EditorUndoManager();
	    public override void OnInspectorGUI()
	    {
	        base.OnInspectorGUI();
	
	        GroundAlign ga = target as GroundAlign;
			//gaRoot = ga;
	        EditorGUILayout.Separator();
	        EditorGUILayout.BeginVertical();
	        
			if( ga.ObjectToAlignTo != null )
			{
				if (GUILayout.Button("AlignToObject"))
		        {
		            GroundAlign.AlignToObject(ga);
		        }
			}
			
		    if (GUILayout.Button("Align"))
		    {
	            GroundAlign.AlignToGround(ga, undoManager);
		    }
				
			if (GUILayout.Button("Align rotation"))
		    {
	            GroundAlign.AlignToGroundNormal(ga, undoManager);
		    }
			
			
	        //if (!string.IsNullOrEmpty(message))
	          //  GUILayout.Label(message);
	
	        EditorGUILayout.EndVertical();
	        
	    }
		
	    /*
		void AligneChildrenRotation(Transform t,GroundAlign parentGA)
		{
			for( int i=0;i<t.childCount;i++ )
			{
				Transform child = t.GetChild(i);
				GroundAlign ga = child.GetComponent<GroundAlign>();
				if( ga != null && !ga.IgnoreGlobalAlign)
				{
					if( ga.IsVirtual )
					{
						if( ga.AlignAsVirtual )
							AlignToGroundNormal(ga);
						
						AligneChildrenRotation(child,ga);	
					}
					else
					{
						AlignToGroundNormal(ga);
					}
				}
			}
		}
		
		void AligneChildren(Transform t,GroundAlign parentGA)
		{
			for( int i=0;i<t.childCount;i++ )
			{
				Transform child = t.GetChild(i);
				GroundAlign ga = child.GetComponent<GroundAlign>();
				if( ga != null )
				{
					if( !ga.IgnoreGlobalAlign )
					{
						if( ga.IsVirtual )
						{
							if( ga.AlignAsVirtual )
								SetGroundOffset(child);
							
							AligneChildren(child,ga);	
						}
						else if( !ga.IgnoreGlobalAlign )
						{
							AlignToGround(ga);
							//SetGroundOffset(child);
						}
						//AlignToGround(ga);
					}
				}
				else
				{
					if( !parentGA.IgnoreGlobalAlign ) 
						SetGroundOffset(child);
					
				}
			}
		}*/
	}
	
}
