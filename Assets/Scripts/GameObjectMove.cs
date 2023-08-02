using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectMove : MonoBehaviour
{

    public float zMove;
    public float yMove;
    public float xMove;
    public float zRotation; 
    public float yRotation;
    public float xRotation;
    // Start is called before the first frame update
    void Start()
    {
        xMove = transform.position.x;
        yMove = transform.position.y;
        zMove = transform.position.z;

        xRotation = transform.rotation.x;
        yRotation = transform.rotation.y;
        zRotation = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(xMove,yMove,zMove);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
    }
}
