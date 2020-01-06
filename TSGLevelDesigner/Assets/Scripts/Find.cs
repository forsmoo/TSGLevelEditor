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
	public class Find {
	    public static Transform Recursive(Transform parent, string name)
	    {
	        Transform match = parent.Find(name);
	        if (match != null)
	            return match;
	        for (int i = 0; i < parent.childCount; i++)
	        {
	            match = Recursive(parent.GetChild(i), name);
	            if (match != null)
	                return match;
	        }
	        return match;
	    }
	
	    public static T RecursiveParent<T>(Transform t) where T : Component
	    {
	        if (t == null)
	            return null;
	
	        T ti = t.gameObject.GetComponent<T>();
	        if (ti != null)
	        {
	            return ti;
	        }
	        else if (t.parent != null)
	        {
	            return RecursiveParent<T>(t.parent);
	        }
	        else
	        {
	            return null;
	        }
	    }
	
	    public static T FindByName<T>(string name) where T : Component
	    {
	        foreach (T t in GameObject.FindObjectsOfType<T>())
	        {
	            if (t.name == name)
	            {
	                return t;
	            }
	        }
	        return null;
	    }
	
		public static List<T> RecursiveTypes<T>(Transform parent) where T : Component
	    {
			List<T> components = new List<T>();
			Recursive<T>(parent,ref components);
			return components;
		}
		
		public static void Recursive<T>(Transform parent,ref List<T> components) where T : Component
	    {
			for (int i = 0; i < parent.childCount; i++)
	        {
				Transform child = parent.GetChild(i);
				T component = child.GetComponent<T>();
				if( component != null )
				   components.Add(component);
	            Recursive<T>(child, ref components);
	        }
		}
		
		public static T Recursive<T>(Transform parent, string name) where T : Component
	    {
			Transform go = Recursive(parent,name);
			if( go != null )
			{
				return go.GetComponent<T>();
			}
			return null;
		}
	    public static GameObject RecursiveType(Transform parent, System.Type type)
	    {
	        Component match = parent.GetComponent(type);
	        if (match != null)
	            return match.gameObject;
	        for (int i = 0; i < parent.childCount; i++)
	        {
	            GameObject go = RecursiveType(parent.GetChild(i), type);
	            if (go != null)
	                return go;
	        }
	        return null;
	    }
		
		public static string FindNextUniqueName(Transform parent,string namebase,bool squeeze)
		{
			if( parent == null )
			{
				return "";
			}
			else
			{
				Transform t = FindNameInParent(parent,namebase);
				if( t== null )
					return namebase;
				int order = GetOrder(namebase);
				
				if( order > 0 && order < 98) //If ordering exists, squeeze in next name
				{
					string nextname = SetOrder(namebase,order+1);
					Transform nextt = FindNameInParent(parent,nextname);
					
					if( nextt != null ) //Ok, next ordered name was busy
					{
						if( squeeze ) //Rename all postsequent transforms
						{
							if( SqueezeInName(parent,nextname) )
								return nextname;
							else
								return "";
						}
						else
						{
							FindNextUniqueName(parent,nextname,false);
						}
					}
					else
					{
						return nextname;
					}
				}
				else
				{
					return namebase + "_01";
				}
			}
			return "";
		}
		
		public static bool SqueezeInName(Transform parent,string currentName)
		{
			int startOrder = GetOrder(currentName);
			int order = startOrder;
			if( order < 0 )
				return false;
			Transform t = null;
			bool done = false;
			//Find last order
			while( !done && order < 98 )
			{
				order++;
				string nextName = SetOrder(currentName,order);
				t = FindNameInParent(parent,nextName);
				if( t == null )
					done = true;
			}
			
			done = false;
			while( !done && order > startOrder )
			{
				order--;
				string nextName = SetOrder(currentName,order);
				t = FindNameInParent(parent,nextName);
				if( t == null )
				{
					done = true;
					return false;
				}
				else
				{
					string rename = SetOrder(currentName,order+1);
					if( string.IsNullOrEmpty(rename ) )
					{
						done = true;
						return false;
					}
					else
					{
						t.name = rename;
					}
				}
			}
			
			return true;
		}
		
		public static string SetOrder(string name,int order)
		{
			if( name.Length > 3 )
			{
				string postfix = "_";
				if( order < 10 )
					postfix += "0";
				postfix += order.ToString();
				return name.Substring(0,name.Length-3) + postfix;
			}
			return "";
		}
		
		public static int GetOrder(string name)
		{
			if( name.Length <= 3 )
				return -1;
			
			string ending = name.Substring(name.Length-2,2);
			string separator = name.Substring(name.Length-3,1);
			if( separator == "_" )
			{
				int order = -1;
				if( int.TryParse(ending,out order) )
				{
					return order;
				}
			}
			return -1;
		}
		
		public static Transform FindNameInParent(Transform parent,string namebase)
		{
			for(int i=0;i<parent.childCount;i++ )
			{
				Transform child = parent.GetChild(i);
				if( child.name == namebase )
					return child;
			}
			return null;
		}
		
		public static Transform RenameChildren(Transform parent,string namebase)
		{
			for(int i=0;i<parent.childCount;i++ )
			{
				Transform child = parent.GetChild(i);

				child.name = NameUtility.GetNumberedName(namebase,i);
			}
			return null;
		}
	}
}
