using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement:MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private float wallJumpCoolDown;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;
    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;
    
    private void Awake()
    {
        body=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        boxCollider=GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        horizontalInput=Input.GetAxis("Horizontal");
        
        //Flip the Player when moving Left-Right
        if (horizontalInput > 0.01f)
        {
            transform.localScale=Vector3.one;
        }
        else if(horizontalInput<-0.01f)
        {
            transform.localScale=new Vector3(-1,1,1);
        }
        //Set Animator Parameters
        anim.SetBool("run",horizontalInput!=0);
        anim.SetBool("grounded",isGrounded());
        //wall jump logic
        // if(wallJumpCoolDown>0.2f)
        // {
        //     body.linearVelocity=new Vector2(horizontalInput*speed,body.linearVelocity.y);
        //     if(onWall() && !isGrounded())
        //     {
        //         body.gravityScale=0;
        //         body.linearVelocity=Vector2.zero;
        //     }
        //     else
        //     {
        //         body.gravityScale=7;
        //     }
        //     if (Input.GetKey(KeyCode.Space))
        //     {
        //         Jump();
        //         if(Input.GetKeyDown(KeyCode.Space)&& isGrounded())
        //         {
        //             SoundManager.instance.PlaySound(jumpSound);
        //         }
        //     }
        // }
        // else
        // {
        //     wallJumpCoolDown+=Time.deltaTime;
        // }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity=new Vector2(body.linearVelocity.x,body.linearVelocity.y/2);
        }
        if (onWall())
        {
            body.gravityScale=0;
            body.linearVelocity=Vector2.zero;
        }
        else
        {
            body.gravityScale=7;
            body.linearVelocity=new Vector2(horizontalInput*speed,body.linearVelocity.y);
            if (isGrounded())
            {
                coyoteCounter=coyoteTime;
                jumpCounter=extraJumps;
            }
            else
            {
                coyoteCounter-=Time.deltaTime;
            }
        }
    }
    private void Jump()
    {
        if(coyoteCounter<0 && !onWall() && jumpCounter<=0)return;
        SoundManager.instance.PlaySound(jumpSound);

        // if(isGrounded())
        // {
        //     body.linearVelocity=new Vector2(body.linearVelocity.x,jumpPower);
        //     // anim.SetTrigger("jump");
        //     SoundManager.instance.PlaySound(jumpSound);
        // }
        // else if(onWall() && !isGrounded())
        // {
        //     if (horizontalInput == 0)
        //     {
        //         body.linearVelocity=new Vector2(-Math.Sign(transform.localScale.x)*10,0);
        //         transform.localScale=new Vector3(-Mathf.Sign(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        //     }
        //     else
        //     {
        //         body.linearVelocity=new Vector2(-Mathf.Sign(transform.localScale.x)*3,6);
        //     }
        //     wallJumpCoolDown=0;

        // }
        if (onWall())
        {
            WallJump();
        }
        else
        {
            if (isGrounded())
            {
                body.linearVelocity=new Vector2(body.linearVelocity.x,jumpPower);
            }
            else
            {
                if (coyoteCounter > 0)
                {
                    body.linearVelocity=new Vector2(body.linearVelocity.x,jumpPower);
                }
                else
                {
                    if (jumpCounter > 0)
                    {
                        body.linearVelocity=new Vector2(body.linearVelocity.x,jumpPower);
                        jumpCounter--;
                    }
                }
            }
            coyoteCounter=0;
        }       
      
    }
    private void WallJump()
    {
        
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit=Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size,0,Vector2.down,0.1f,groundLayer);
        return raycastHit.collider!=null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit=Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size,0,new Vector2(transform.localScale.x,0),0.1f,wallLayer);
        return raycastHit.collider!=null;
    }

    public bool canAttack()
    {
        return horizontalInput==0 && isGrounded() && !onWall();
    }

}

