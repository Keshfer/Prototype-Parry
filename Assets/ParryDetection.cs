using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryDetection : MonoBehaviour
{
    private SpriteRenderer thrustRenderer;
    private CapsuleCollider2D thrustCollider;
    public GameObject body;

    void Start()
    {
        thrustCollider = gameObject.GetComponent<CapsuleCollider2D>();
        thrustRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Parry"))
        {
            thrustCollider.enabled = false;
            thrustRenderer.enabled = false;
            body.SetActive(false);
            //print("parried");

        } else if(other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            //print("dead");
        }
    }
}
