using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    private bool isJumping;
    private Vector2 jumpValue;
    private Rigidbody2D rb;
    public Vector2 direction;
    private bool isGrounded;
    public Gravity gravityScript;
    private Vector2 totalMovement;
    private Vector2 horizontal;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        isGrounded = false;
        isJumping = false;
        jumpValue = new Vector2(0, 0);



    }

    // Update is called once per frame
    void Update()
    {
        
        totalMovement = (((horizontal * speed * Time.deltaTime) + gravityScript.fallValue + jumpValue));
        
        rb.velocity = totalMovement;
        //Debug.Log(rb.velocity);
    }

    public void OnMove(InputAction.CallbackContext context)  
    {
        
        if(context.performed)
        {
            direction = context.ReadValue<Vector2>();
            if(direction.x > 0)
            {
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            } else
            {
                gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            horizontal = new Vector2(direction.x, 0);
            //Debug.Log(direction);
            
            
        }
        if(context.canceled)
        {
            direction = context.ReadValue<Vector2>();
            horizontal = new Vector2(direction.x, 0);

        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        
        if(context.performed)
        {
            if(isGrounded)
            {
                jumpValue.y = jumpPower;
                isJumping = true;
            }
            
        }
        if(context.canceled)
        {
            isJumping = false;
            jumpValue.y = 0;
        }
        
        

    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(isJumping)
            {
                jumpValue.y = 0;
            }
            isGrounded = true;
            
        }
        if(other.gameObject.CompareTag("Enemy Projectile"))
        {
            gameObject.SetActive(false);
        }
    }
    public void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
