using UnityEngine;
using System.Collections;
// commenturile de aici sunt lucruri care trebuie starese/adaugate ca sa schimb wall slideul intre required directional key to stick to wall or doesnt require

public class Controller2D : RaycastController
{
    float maxClimbAngle = 80;
    float maxDescendingAngle = 75;
    
    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;//dis

    }

    public void Move(Vector2 moveAmount,bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount,Vector2 input,bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)//dis
        {//dis
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);//dis
        }//dis
        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        //if(moveAmount.x!=0){ 
            HorizontalCollisions(ref moveAmount);
      // }
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }
        transform.Translate(moveAmount);
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;//=Mathf.Sign(moveAmount.x);
        float rayLenght = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)//dis
        {//dis
            rayLenght = 2 * skinWidth;//dis
        }//dis

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {

                if (hit.distance == 0)
                {
                    continue;
                }
                
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle<=maxClimbAngle )
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }

                    float distanceToSlopeStart = 0;

                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climingSlope || slopeAngle > maxClimbAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLenght = hit.distance;

                    if (collisions.climingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLenght = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, collisionMask);

            Debug.DrawRay(rayOrigin,Vector2.up*directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance==0)
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

                moveAmount.y = (hit.distance-skinWidth) * directionY;
                rayLenght = hit.distance;

                if (collisions.climingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        if (collisions.climingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLenght = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1)? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght,collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle!= collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDescendingAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

  

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
  
        public bool above, below;
        public bool left, right;

        public bool climingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 moveAmountOld;
        public int faceDir;//dis
        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        
        }
    }

}
