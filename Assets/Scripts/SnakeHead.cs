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
    public GameObject upgradeTextPrefab;

    public Vector2 lastPlayerDirectionInput;

    public uint bodySegmentsToSpawn;
    public GameObject bodySegmentPrefab;

    public static string KILLS_PLAYER_TAG = "KillsPlayer";

	public float graceTime = 0.5f;

	public float stTime;

    public bool dead = false;

    private Rigidbody2D body2d;

    private List<SnakeBody> _bodySegments = new List<SnakeBody>();

    public List<SnakeBody> bodySegments
    {
        get
        {
            return _bodySegments;
        }
    }

    public bool headHasLeftStartingPosition = false;
    private bool defaultSegmentsInitialized = false;
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

        //cause the new segment text to be shown
        defaultSegmentsInitialized = true;
    }

    void Update()
    {
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            //touch
            if(Input.touches.Length > 0)
            {
                Touch touch = Input.touches[0];
                if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    //move the orgin to the center of the screen
                    PolarVec2 touchCoords = PolarVec2.FromCartesian(touch.position.x - Screen.width / 2, touch.position.y - Screen.height / 2);

                    if (touchCoords.A >= 45 && touchCoords.A < 135)
                    {
                        lastPlayerDirectionInput = Vector2.up;
                    }
                    else if (touchCoords.A >= 135 && touchCoords.A < 225)
                    {
                        lastPlayerDirectionInput = -Vector2.right;
                    }
                    else if (touchCoords.A >= 225 && touchCoords.A < 315)
                    {
                        lastPlayerDirectionInput = -Vector2.up;
                    }
                    else // touchCoords.A >= 315 || touchCoords.A < 45
                    {
                        lastPlayerDirectionInput = Vector2.right;
                    }
                }
            }
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
            KillSegmentsFrom(_bodySegments[0]);
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

        Debug.Log("distanceFromPrevSegment: " + distanceFromPrevSegment);

        //add extra offset becuase the head is bigger than the body segments
        uint extraOffset = segmentPositions.maxSize == 0 ? headExtraOffset : 0;

        newSegmentScript.indexInQueue = (uint)(((int)segmentPositions.maxSize) - 1 + distanceFromPrevSegment) + extraOffset;
        newSegmentScript.head = this;
	

        //set linked-list references

        if (_bodySegments.Count > 0)
        {
            SnakeBody prevSegment = _bodySegments[_bodySegments.Count - 1];
		
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

        if(_bodySegments.Count <= 1) //the second segment can hit the head when turning, so add it to the ignore list.
        {
            newSegmentScript.ignoreCollisionSegments.Add(gameObject);
        }

        uint oldPosQueueSize = segmentPositions.size;
        segmentPositions.Resize(segmentPositions.maxSize + distanceFromPrevSegment + extraOffset);

        Pair<Vector3, Quaternion> lastSegmentOrientation;

        if(_bodySegments.Count < 1) //first segment
        {
            lastSegmentOrientation = new Pair<Vector3, Quaternion>(transform.position, transform.rotation);
        }
        else
        {
            
            SnakeBody oldLastSegment = _bodySegments[_bodySegments.Count - 1];

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

        _bodySegments.Add(newSegmentScript);

        GameManager.instance.OnAddSegment();

        if(defaultSegmentsInitialized)
        {
            Vector3 messagePosition = newSegment.transform.position;
            messagePosition.z = -1;

            GameObject newSegmentMessage = (GameObject)Instantiate(upgradeTextPrefab, messagePosition, Quaternion.identity);
            newSegmentMessage.GetComponent<UpgradeText>().text = string.Format("Pepper {0}", _bodySegments.Count);
        }
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
        if(gameObject != null && otherObject == gameObject && headHasLeftStartingPosition)
        {
            if(headHasLeftStartingPosition == true)
            {
                KillSegmentsFrom(_bodySegments[0]);

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
            dead = true;
            animator.SetTrigger("Die");
            firstDeadSegment.DieRecursive(0);

            GameManager.instance.OnPlayerDeath();
        }
        else
        {
            //remove the dead segments from the snake
            segmentPositions.Resize(newLastSegment.indexInQueue + 1);
            firstDeadSegment.DieRecursive(0);
            newLastSegment.segmentAfter = null;

            int firstDeadSegmentIndex = _bodySegments.IndexOf(firstDeadSegment);

            _bodySegments.RemoveRange(firstDeadSegmentIndex, _bodySegments.Count - firstDeadSegmentIndex);
        }


    }
}
