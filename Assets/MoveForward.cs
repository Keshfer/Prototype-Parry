using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 enemydirection;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        enemydirection = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = (enemydirection * speed * Time.deltaTime);
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }

}
