using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Controller2D))]
public class PlayerPhysics : MonoBehaviour
{
   

    Teleporter tel;
    public static Animator anim;

    public float maxJumpHeight = .2f;
    public float minJumpHeight = .1f;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .05f;
    public float moveSpeed = 1.5f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 2.5f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    public float coolDown = 1;
    private float CoolDownTimer;

    public bool isTeleporting;
    public bool isInFrontOfTheButton;

    Controller2D controller;

    public Vector3 respawnPoint;
    public LevelManager gameLevelManager;


    void Start()
    {
        respawnPoint = transform.position;

        tel = FindObjectOfType<Teleporter>();

        anim = GetComponent<Animator>();

        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        // print("Gravity" + gravity + " Jump Velocity" + maxJumpVelocity);
        gameLevelManager = FindObjectOfType<LevelManager>();
    } 
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, 1, 1));
    }
    void Update()
    {
        if (isTeleporting == false)
        {

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            int wallDirX = (controller.collisions.left) ? -1 : 1;

            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

            bool wallSliding = false;

            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                anim.SetBool("IsSticked", true);

                wallSliding = true;

                if (velocity.y < -wallSlideSpeedMax)
                {

                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {

                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (input.x != wallDirX && input.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                       GetComponent<SpriteRenderer>().flipX = true;
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
                if (controller.collisions.left)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            else
            {
                anim.SetBool("IsSticked", false);
            }



            if (controller.collisions.below)
            {
                anim.SetBool("IsJumping", false);
            }

            if (input.x == -1 && velocity.y == 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetBool("IsMoving", true);
                
             

            }
            else if (input.x == 1 && velocity.y == 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetBool("IsMoving", true);
            }
            else
            {
                anim.SetBool("IsMoving", false);
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (wallSliding)
                {
                    if (wallDirX == input.x)
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else if (input.x == 0)
                    {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    else
                    {
                        velocity.x = -wallDirX * wallLeap.x;
                        velocity.y = wallLeap.y;
                    }
                }
                if (controller.collisions.below)
                {
                    velocity.y = maxJumpVelocity;
                    anim.SetBool("IsJumping", true);
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }


            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime, input);


            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;

            }
        }

        if (CoolDownTimer > 0)
        {
            CoolDownTimer -= Time.deltaTime;
        }

        if (CoolDownTimer < 0)
        {
            CoolDownTimer = 0;
        }


        if (Input.GetMouseButton(0))
        {
          
            tel.GetComponent<SpriteRenderer>().enabled = true;
        }else
        {
            tel.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (Input.GetMouseButtonUp(0)&&CoolDownTimer==0)
          {
            
            if (tel.cantTeleportX==false && tel.cantTeleportY == false)
            {

                isTeleporting = true;
                CoolDownTimer = coolDown;
                anim.SetBool("IsDissapearing", true);
                Invoke("Teleporting", 0.2f);

            }
            
            tel.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }

    void Teleporting()
    {
        anim.SetBool("IsDissapearing", false);
        isTeleporting = false;
        if (tel.cantTeleportY == false && tel.cantTeleportX == false)
        {            
            Vector2 pos = GameObject.Find("TeleportLocation").transform.position;
            transform.position = (pos);    
        }
        else Debug.Log("Cant teleport there");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "DeathBox")
        {
            gameLevelManager.Respawn();
        }
        
        if (col.tag == "Checkpoint")
        {
            respawnPoint = col.transform.position;

        }
        if (col.tag == "Button")
        {
            isInFrontOfTheButton = true;
        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            isInFrontOfTheButton = false;
        }
    }
}
