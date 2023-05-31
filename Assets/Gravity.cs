using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float fallAccel;
    private Rigidbody2D rb;
    private float velocity;
    public Vector2 fallValue = new Vector2(0,0);
    private bool isFalling;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        velocity = 0;
        isFalling = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            fallValue = fallValue + new Vector2(0, velocity + (fallAccel * Time.deltaTime));
        }
        
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground")) {
            fallValue = new Vector2(0,0);
            isFalling = false;
        }
    }
    public void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            isFalling = true;
        }
    }
}
