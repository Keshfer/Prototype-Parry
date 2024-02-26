using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float power;
    private Rigidbody2D rb;
    private bool isParried;
    private Vector2 reflectVector;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        isParried = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isParried)
        {
            rb.velocity = reflectVector * power * 4 * Time.fixedDeltaTime;
        }
        else
        {
            rb.velocity = transform.right * power * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Parry"))
        {
            Debug.Log("Parry");
            isParried = true;
            Vector2 contactPoint = collision.ClosestPoint(gameObject.transform.position);
            Vector2 newDirection = (new Vector2(transform.position.x, transform.position.y)) - contactPoint;
            reflectVector = newDirection.normalized;

            
            
            
        } else if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
