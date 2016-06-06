using UnityEngine;
using System.Collections.Generic;
using System;

public class SnakeHead : MonoBehaviour {

    //height and width of head and body textures
    public int segmentSize;

    //offset in px between segments
    public float segmentOffset = 1;
    public uint headExtraOffset = 5;
    //pixels / sec
    public float speed = 30;

    public FixedCircularQueue<Pair<Vector3, Quaternion>> segmentPositions = new FixedCircularQueue<Pair<Vector3, Quaternion>>(0);

    public Vector2 lastPlayerDirectionInput;

    public uint bodySegmentsToSpawn;
    public GameObject bodySegmentPrefab;

    public static string KILLS_PLAYER_TAG = "KillsPlayer";

	public float graceTime = 0.5f;

	public float stTime;

    public bool dead = false;

    private Rigidbody2D body2d;

    private List<SnakeBody> bodySegments = new List<SnakeBody>();

    public bool headHasLeftStartingPosition = false;
    private Vector2 hitboxTopRight, hitboxBottomLeft; //local space

    private Animator animator;

	void Awake () {
        body2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        hitboxTopRight = collider.offset /2  + collider.size/2 ;
        hitboxBottomLeft = collider.offset/2  - collider.size/2 ;
		stTime = Time.time;
    }

    void Start()
    {
        for(uint counter = 0; counter < bodySegmentsToSpawn; ++counter)
        {
            SpawnBodySegment();
        }
    }

    void Update()
    {
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            //touch
            if(Input.touches.)
        }
        else //keyboard
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                lastPlayerDirectionInput = Vector2.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                lastPlayerDirectionInput = -Vector2.up;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                lastPlayerDirectionInput = -Vector2.right;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                lastPlayerDirectionInput = Vector2.right;
            }
        }

    }

    void FixedUpdate ()
    {

        if(dead)
        {
            body2d.velocity = Vector2.zero;
        }
        else
        {
            body2d.velocity = lastPlayerDirectionInput * speed;

            body2d.rotation = PolarVec2.FromCartesian(lastPlayerDirectionInput).A;

            segmentPositions.Enqueue(new Pair<Vector3, Quaternion>(transform.position, transform.rotation));
			if(graceTime < Time.time - stTime)
			{
            headHasLeftStartingPosition = true;
			}
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag.Equals(KILLS_PLAYER_TAG) && headHasLeftStartingPosition) //check if e've hit an obsctacle with the head
        {
            KillSegmentsFrom(bodySegments[0]);
        }
        if (other.tag == "Food")
        {
            Debug.Log("Spawning");
            SpawnBodySegment();
            GameObject.Destroy(other.gameObject);
        }
    }

	public void SpawnBodySegment()
    {
        GameObject newSegment = GameObject.Instantiate(bodySegmentPrefab);
        SnakeBody newSegmentScript = newSegment.GetComponent<SnakeBody>();

        // the 30 comes from FixedUpdate being called 30 times per second
        uint distanceFromPrevSegment /* in updates */ = (uint)Mathf.RoundToInt((segmentSize + segmentOffset) * 30f / speed);

        //Debug.Log("distanceFromPrevSegment: " + distanceFromPrevSegment);

        //add extra offset becuase the head is bigger than the body segments
        uint extraOffset = segmentPositions.maxSize == 0 ? headExtraOffset : 0;

        newSegmentScript.indexInQueue = (uint)(((int)segmentPositions.maxSize) - 1 + distanceFromPrevSegment) + extraOffset;
        newSegmentScript.head = this;
	

        //set linked-list references

        if (bodySegments.Count > 0)
        {
            SnakeBody prevSegment = bodySegments[bodySegments.Count - 1];
		
			if(prevSegment != null)
			{
				prevSegment.segmentAfter = newSegment;

            	newSegmentScript.segmentBefore = prevSegment.gameObject;

				newSegmentScript.ignoreCollisionSegments.Add(prevSegment.gameObject);
			}
        }
        else
        {
            newSegmentScript.segmentBefore = gameObject;
        }

        if(bodySegments.Count <= 1) //the second segment can hit the head when turning, so add it to the ignore list.
        {
            newSegmentScript.ignoreCollisionSegments.Add(gameObject);
        }

        uint oldPosQueueSize = segmentPositions.size;
        segmentPositions.Resize(segmentPositions.maxSize + distanceFromPrevSegment + extraOffset);

        Pair<Vector3, Quaternion> lastSegmentOrientation;

        if(bodySegments.Count < 1) //first segment
        {
            lastSegmentOrientation = new Pair<Vector3, Quaternion>(transform.position, transform.rotation);
        }
        else
        {
            
            SnakeBody oldLastSegment = bodySegments[bodySegments.Count - 1];

			if(oldLastSegment == null)
			{
				lastSegmentOrientation = new Pair<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
			}
			else
			{
            	lastSegmentOrientation = new Pair<Vector3, Quaternion>(oldLastSegment.transform.position, oldLastSegment.transform.rotation);
			}
        }

        newSegment.transform.position = lastSegmentOrientation.left;

        //fill the new space in the position queue
        for (uint counter = 0; counter < distanceFromPrevSegment; ++counter)
        {
            segmentPositions[oldPosQueueSize + counter] = lastSegmentOrientation;
        }

        bodySegments.Add(newSegmentScript); 
    }

    public void OnCollision(SnakeBody segment, GameObject otherObject)
    {

        //if (otherObject.GetComponent<SnakeBody>() != null)
        //{

        //    //body to body collision
        //    SnakeBody otherSegment = otherObject.GetComponent<SnakeBody>();

        //    SnakeBody lowestIndexSegment = otherSegment.indexInQueue < segment.indexInQueue ? otherSegment : segment;

        //    //may have already killed this part
        //    if(segmentPositions.size >= lowestIndexSegment.indexInQueue)
        //    {
        //        KillSegmentsFrom(lowestIndexSegment);
        //    }
        //}
        if (otherObject == gameObject && headHasLeftStartingPosition)
        {
            if(headHasLeftStartingPosition == true)
            {
                KillSegmentsFrom(bodySegments[0]);

                //GameObject.Destroy(gameObject);
            }
        }
        else if (otherObject.tag == KILLS_PLAYER_TAG)
        {
            KillSegmentsFrom(segment);
        }

    }

    //removes the provided segment and all segments after it from the snake, and plays their death animation
    private void KillSegmentsFrom(SnakeBody firstDeadSegment)
    {
        SnakeBody newLastSegment = firstDeadSegment.segmentBefore.GetComponent<SnakeBody>();

        if (newLastSegment == null)
        {
            //segment next to the head

            //TODO: kill player
            dead = true;
            animator.SetTrigger("Die");

            Debug.LogError("Player died");
            firstDeadSegment.DieRecursive(0);

        }
        else
        {
            //remove the dead segments from the snake
            segmentPositions.Resize(newLastSegment.indexInQueue + 1);
            firstDeadSegment.DieRecursive(0);
            newLastSegment.segmentAfter = null;

            int firstDeadSegmentIndex = bodySegments.IndexOf(firstDeadSegment);

            bodySegments.RemoveRange(firstDeadSegmentIndex, bodySegments.Count - firstDeadSegmentIndex);
        }


    }
}
