using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryDetection : MonoBehaviour
{
    private SpriteRenderer attackRenderer;
    private CapsuleCollider2D attackCollider;
    public GameObject body;
    public bool parrySuccess;

    void Start()
    {
        parrySuccess = false;
        attackCollider = gameObject.GetComponent<CapsuleCollider2D>();
        attackRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Parry"))
        {
            parrySuccess = true;
            StartCoroutine("parrySuccessDuration");
            attackCollider.enabled = false;
            attackRenderer.enabled = false;
            body.SetActive(false);
            //print("parried");

        } else if(other.CompareTag("Player"))
        {
            if(!parrySuccess)
            {
                other.gameObject.SetActive(false);
            }
            
            //print("dead");
        }
    }
    private IEnumerator parrySuccessDuration()
    {
        yield return new WaitForSeconds(0.2f);
        parrySuccess = false;
    }
}
