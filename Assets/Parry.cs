using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Parry : MonoBehaviour
{
    private bool isParry;
    public GameObject player;
    public GameObject parryColliderObject;
    public GameObject parryColliderObjectUp;
    public GameObject parryColliderObjectDown;
    public PlayerMovement PlayerMovementScript;
    private CapsuleCollider2D parryCollider;
    private CapsuleCollider2D parryColliderUp;
    private CapsuleCollider2D parryColliderDown;

    // Start is called before the first frame update
    void Start()
    {
        isParry = false;
        parryCollider = parryColliderObject.GetComponent<CapsuleCollider2D>();
        parryColliderUp = parryColliderObjectUp.GetComponent<CapsuleCollider2D>();
        parryColliderDown = parryColliderObjectDown.GetComponent<CapsuleCollider2D>();
        parryCollider.enabled = false;
        parryColliderUp.enabled = false;
        parryColliderDown.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isParry = true;
            //Debug.Log("Parry activated");
            if(PlayerMovementScript.direction.y > 0)
            {
                parryColliderUp.enabled = true;

            } else if(PlayerMovementScript.direction.y < 0)
            {
                parryColliderDown.enabled = true;

            } else
            {
                parryCollider.enabled = true;
            }
            
            StartCoroutine("ParryDuration");
        }
        
    }
    private IEnumerator ParryDuration()
    {
        yield return new WaitForSeconds(0.2f);
        isParry = false;
        parryCollider.enabled = false;
        parryColliderUp.enabled = false;
        parryColliderDown.enabled = false;
        //Debug.Log("Parry disabled");
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy Projectile"))
        {
            if(isParry)
            {
                Vector2 direction = (other.gameObject.transform.position - player.transform.position).normalized;
                MoveForward projectile = other.gameObject.GetComponent<MoveForward>();
                Rigidbody2D projectileRb = other.gameObject.GetComponent<Rigidbody2D>();
                projectile.speed = projectile.speed * 2;
                projectile.enemydirection = direction;
                //Debug.Log("Deflected!");
            }
        }
        /*
        if(other.gameObject.CompareTag("Enemy Attack"))
        {
            if(isParry)
            {
                SpriteRenderer attackRenderer = other.gameObject.GetComponent<SpriteRenderer>();
                CapsuleCollider2D attackCollider = other.gameObject.GetComponent<CapsuleCollider2D>();
                attackRenderer.enabled = false;
                attackCollider.enabled = false;

            }
        }
        */
    }
}
