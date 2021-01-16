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
            edge.Setup(Nodes[0].position, Nodes[1].position, Nodes[0].up, Nodes[1].up);
			UpdateDebugInfo();

		}
        else
        {
            Debug.LogError("Need two nodes");
        }
        
    }

	public void UpdateDebugInfo()
	{
		var ManualStart = Nodes[0];
		var ManualEnd = Nodes[1];

		if (ManualEnd && ManualStart)
		{
			edge.Setup(ManualStart.position, ManualEnd.position, ManualStart.up, ManualEnd.up);
			
			ManualEnd.parent = null;
			ManualStart.parent = null;
			ManualStart.LookAt(ManualEnd.position);
			ManualEnd.LookAt(ManualEnd.position + (edge.RimDir));
			RaycastHit hit;
			Vector3 ro = ManualStart.position + edge.Normal1 - edge.Dir1 * 0.2f;
			ro += edge.RimDir * 0.1f;
			if (Physics.Raycast(ro, -edge.Normal1, out hit, 2))
			{
				ManualStart.rotation = Quaternion.FromToRotation(ManualStart.up, hit.normal) * ManualStart.rotation;
			}
			ro = ManualEnd.position + edge.Normal2 - edge.Dir2 * 0.2f;
			ro -= edge.RimDir * 0.1f;
			if (Physics.Raycast(ro, -edge.Normal2, out hit, 2))
			{
				ManualEnd.rotation = Quaternion.FromToRotation(ManualEnd.up, hit.normal) * ManualEnd.rotation;
			}
			transform.position = ManualStart.position + (ManualEnd.position - ManualStart.position) * 0.5f;
			transform.LookAt(ManualEnd);

			ManualStart.parent = transform;
			ManualEnd.parent = transform;


			BoxCollider bc = GetComponent<Collider>() as BoxCollider;
			
			if (bc == null)
				bc = gameObject.AddComponent<BoxCollider>();
			Bounds b = new Bounds(ManualStart.position, Vector3.zero);
			b.Encapsulate(ManualEnd.position);
			bc.isTrigger = true;
			bc.center = Vector3.zero;
			bc.size = new Vector3(0.5f, 0.5f, edge.Length);

			edge.Setup(ManualStart.position, ManualEnd.position, ManualStart.up, ManualEnd.up);
			/*if (edgeDefinition == null)
				edgeDefinition = new EdgeDefinition();
			edgeDefinition.Start = edge.Start;
			edgeDefinition.End = edge.End;
			edgeDefinition.Direction = edge.Dir1;
			edgeDefinition.Direction2 = edge.Dir2;
			edgeDefinition.Normal1 = edge.Normal1;
			edgeDefinition.Normal2 = edge.Normal2;*/
		}
	}

	private void OnDrawGizmos()
    {
        if (Draw)
        {
			if( edge != null )
            {
				edge.DrawDebug();
            }

            if (Nodes.Count > 1)
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

