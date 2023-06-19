using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Vector2 totalMove;
    public Gravity gravityScript;
    public StateManager stateManagerScript;
    private State currentState;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        gravityScript = gameObject.GetComponent<Gravity>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = stateManagerScript.currentState;
        if (gravityScript != null)
        {
            totalMove = gravityScript.fallValue + this.currentState.velocity;
        } else
        {
            totalMove = this.currentState.velocity;
        }
        rb.velocity = totalMove;
    }
}
