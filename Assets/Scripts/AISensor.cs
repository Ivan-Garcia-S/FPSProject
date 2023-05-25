using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AISensor : MonoBehaviour
{
    public float distance = 10f;
    public float angle = 30f;
    public float height = 1f;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public Color meshColor = Color.red;
    public List<GameObject> Objects = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;

    private void Start() 
    {
        scanInterval = 1.0f / scanFrequency;
    }

    private void Update() 
    {
        scanTimer -= Time.deltaTime;
        if(scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        Objects.Clear();
        for(int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if(IsInSight(obj))
            {
                Objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if(direction.y < 0 || direction.y > height) return false;
        
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if(deltaAngle > angle) return false;

        origin.y += height / 2;
        dest.y = origin.y;
        if(Physics.Linecast(origin, dest, occlusionLayers)) return false;

        return true;
    }

    Mesh CreateWedgeMesh(){
        Mesh mesh = new Mesh();
        
        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        Vector3 offset = Vector3.down * height/2;

        
        Vector3 bottomCenter = Vector3.zero + offset;
        Vector3 bottomLeft = Quaternion.Euler(0,-angle,0) * Vector3.forward * distance + offset;
        Vector3 bottomRight = Quaternion.Euler(0,angle,0) * Vector3.forward * distance + offset;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        //Left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //Right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;
        
        //Far side
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = topLeft;
        vertices[vert++] = bottomLeft;
 
        //Top
        vertices[vert++] = topCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;
        
        //Bottom
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomLeft;

        for(int i = 0; i < numVertices; ++i){
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate() 
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if(mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh,transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for(int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 1.5f);
        }

        Gizmos.color = Color.green;
        foreach(var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 1.5f);
        }
    }
}