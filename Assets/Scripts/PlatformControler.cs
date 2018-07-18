using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformControler : RaycastController {
    public LayerMask passangerMask;


    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;
    
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passenegerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    // Use this for initialization
    public override void Start () {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();
        CalculatePassengersMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    float Ease(float x)
    {
        float a = easeAmount +1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex],globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passenegerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity,passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengersMovement(Vector3  velocity)
    {
        HashSet<Transform> movedPassanger = new HashSet<Transform>();
        passenegerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Platforma vertical
        if (velocity.y != 0)
        {
            float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, passangerMask);

                if (hit && hit.distance!=0){
                    if (!movedPassanger.Contains(hit.transform))
                    {
                        movedPassanger.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passenegerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        //pltaform oreizontal
        if (velocity.x != 0)
        {
            float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, passangerMask);
                if (hit && hit.distance != 0)
                {
                    if (!movedPassanger.Contains(hit.transform))
                    {
                        movedPassanger.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX; ;
                        float pushY = -skinWidth;

                        passenegerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // peste o platforma care pica in jos sau se misca orizontal
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLenght = skinWidth*2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLenght, passangerMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassanger.Contains(hit.transform))
                    {
                        movedPassanger.Add(hit.transform);
                        float pushX =velocity.x;
                        float pushY = velocity.y;

                        passenegerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }
    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size=.3f;

            for(int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i]: localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
