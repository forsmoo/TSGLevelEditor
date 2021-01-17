using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDefinition : MonoBehaviour
{
    public List<Transform> Nodes;
    public bool Draw = true;
    public Color Color = Color.black;
    public Lirp.MaterialEnum MaterialType;
    public Lirp.Edge edge;
	public float SearchOffsetZ = 0.1f;
    public void UpdateDefinition()
    {
        if (Nodes == null)
            Nodes = new List<Transform>();

        Nodes.Clear();
        foreach (Transform t in transform)
            Nodes.Add(t);

        if (Nodes.Count > 1)
        {
            for (int i = 1; i < Nodes.Count; i++)
                Nodes[i - 1].rotation = Quaternion.LookRotation(Nodes[i].position - Nodes[i - 1].position, Nodes[i-1].up);

            Nodes[Nodes.Count-1].rotation = Quaternion.LookRotation(Nodes[Nodes.Count - 2].forward, Nodes[Nodes.Count - 1].up);
            edge = new Lirp.Edge();
            //edge.Setup(Nodes[0].position, Nodes[1].position, Nodes[0].up, Nodes[1].up);
			UpdateEdge();
		}
        else
        {
            Debug.LogError("Need two nodes");
        }
        
    }

    public void Initialize()
    {
		var start = new GameObject("Start");
		var end = new GameObject("End");
		start.transform.SetParent(this.transform);
		end.transform.SetParent(this.transform);
		ResetTransform(start.transform);
		ResetTransform(end.transform);
		Nodes.Add(start.transform);
		Nodes.Add(end.transform);
	}

	void ResetTransform(Transform t)
    {
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
	}
    public bool UpdateEdge()
	{
		var ManualStart = Nodes[0];
		var ManualEnd = Nodes[1];

		if (ManualEnd && ManualStart)
		{
			edge.Setup(ManualStart.position, ManualEnd.position, ManualStart.up, ManualEnd.up);
			
			ManualEnd.SetParent(null);
			ManualStart.SetParent(null);
			ManualStart.LookAt(ManualEnd.position,ManualStart.up);
			ManualEnd.LookAt(ManualEnd.position + (edge.RimDir),ManualEnd.up);

			Vector3 N1=Vector3.up;
			Vector3 N2=Vector3.up;
			RaycastHit hit;
			Vector3 ro = ManualStart.position + edge.Normal1 + edge.Dir1 * (SearchOffsetZ*2);
			ro += edge.RimDir * SearchOffsetZ;
			if (Physics.Raycast(ro, -edge.Normal1, out hit, 2))
			{
				N1 = hit.normal;
			}
			else
            {
				Debug.LogError("No collider found start");
				ManualStart.parent = transform;
				ManualEnd.parent = transform;
				return false;
			}
			ro = ManualEnd.position + edge.Normal2 + edge.Dir2 * (SearchOffsetZ*2);
			ro -= edge.RimDir * SearchOffsetZ;
			if (Physics.Raycast(ro, -edge.Normal2, out hit, 2))
			{
				N2 = hit.normal;
			}
			else
			{
				Debug.LogError("No collider found end");
				ManualStart.parent = transform;
				ManualEnd.parent = transform;
				return false;
			}
			transform.position = ManualStart.position + (ManualEnd.position - ManualStart.position) * 0.5f;
			transform.LookAt(ManualEnd);

			ManualStart.parent = transform;
			ManualEnd.parent = transform;

			edge.Setup(ManualStart.position, ManualEnd.position, N1, N2);
			
			return true;
		}
		return false;
	}

	private void OnDrawGizmos()
    {
        if (Draw)
        {
			if( edge != null )
            {
				edge.DrawDebug();
            }

            if (Nodes.Count == 2 && Nodes[0] != null  && Nodes[1]!=null)
            {
                var previousColor = Gizmos.color;
                Gizmos.color = Color;
                for (int i = 1; i < Nodes.Count; i++)
                    Gizmos.DrawLine(Nodes[i - 1].position, Nodes[i].position);
                Gizmos.color = previousColor;
            }
        }
    }
}


