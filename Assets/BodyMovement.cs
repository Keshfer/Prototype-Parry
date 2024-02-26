using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    public GameObject body;
    private Rigidbody2D bodyRB;
    // Start is called before the first frame update
    void Start()
    {
        bodyRB = body.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
