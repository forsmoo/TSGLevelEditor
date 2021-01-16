//-----------------------------------------------------------------------
// <copyright file="Edge.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// <date>den 29 mars 2018 17:42:06</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Lirp
{
	public class Edge
	{
		public void Setup(Vector3 v1, Vector3 v2, Vector3 n1, Vector3 n2)
		{
			Normal1 = n1;
			Normal2 = n2;
			Start = v1;
			End = v2;
			Vector3 diff = End - Start;
			RimDir = diff.normalized;
			Length = diff.magnitude;
			Dir1 = Vector3.Cross(n1, RimDir);
			if (Vector3.Dot(Dir1, n2) > 0)
				Dir1 = -Dir1;
			Dir2 = Vector3.Cross(RimDir, n2);
			if (Vector3.Dot(Dir2, n1) > 0)
				Dir2 = -Dir2;
		}

		public void DrawDebug()
		{
			Debug.DrawLine(Start, End, Color.magenta);
			Vector3 mid = (End - Start) * 0.5f + Start;
			Debug.DrawLine(mid, mid + Normal1, Color.cyan);
			Debug.DrawLine(mid, mid + Dir1, Color.cyan);

			Debug.DrawLine(mid, mid + Normal2, Color.magenta);
			Debug.DrawLine(mid, mid + Dir2, Color.magenta);
		}

		public Vector3 Normal1;
		public Vector3 Normal2;
		public Vector3 Start;
		public Vector3 End;
		public Vector3 Dir1;
		public Vector3 Dir2;
		public Vector3 RimDir;
		public float Length;
		public MaterialEnum mat;
	}
}
