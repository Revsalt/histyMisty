using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class meshGenerator : MonoBehaviour
{
    Mesh mesh;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape(transform.position);
        UpdateShape();
    }

    int i = 0;
    private void Update()
    {
        if  (Input.GetKeyDown(KeyCode.UpArrow))
        {
            i++;
            CreateShape(transform.position + Vector3.right * i);
            UpdateShape();
        }
    }

    private void CreateShape(Vector3 pos)
    {
        if (vertices.Count == 0)
        {
            vertices.Add(pos + new Vector3(0, 0, 0));
            vertices.Add(pos + new Vector3(0, 0, 1));
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(1, 0, 1));
        } else
        {
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(1, 0, 1));
        }

        if (triangles.Count == 0)
        {
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);

            triangles.Add(1);
            triangles.Add(3);
            triangles.Add(2);
        } 
        else
        {
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 1); 
            triangles.Add(vertices.Count - 2);

            triangles.Add(vertices.Count - 4);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var item in vertices)
        {
            Gizmos.DrawWireSphere(item, 0.1f);
        }
    }

    private void UpdateShape()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }
}
