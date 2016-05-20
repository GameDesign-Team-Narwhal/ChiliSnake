using UnityEngine;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour {

    //height and width of head and body textures
    public int segmentSize;

    //pixels / sec
    public float headSpeed = 30;

    public Vector2 currentDirection;

    public Vector2 lastPlayerDirectionInput;

    public SnakeBody firstBodySegment;

    public uint bodySegmentsToSpawn;
    public GameObject bodySegmentPrefab;

    //position of the head the last time that movement started
    private Vector3 lastStartPos;
    private float lastStartTime;

    private Rigidbody2D body2d;

    private List<SnakeBody> bodySegments = new List<SnakeBody>();

	void Awake () {
        lastPlayerDirectionInput = Vector2.up;

        body2d = GetComponent<Rigidbody2D>();

        currentDirection = lastPlayerDirectionInput;
        body2d.velocity = currentDirection * headSpeed;
    }

    void Start()
    {
        for(uint counter = 0; counter < bodySegmentsToSpawn; ++counter)
        {
            SpawnBodySegment();
        }
    }

    void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.UpArrow))
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

        //check if the current movement has finished
        //we only move in one dimension at a time, so this should be OK
        if(lastPlayerDirectionInput != currentDirection)
        {
            //the current movement is done

            Vector2 distanceTraveled = transform.position - lastStartPos;

            MovementStep completedStep = new MovementStep(PolarVec2.FromCartesian(distanceTraveled).normalized, headSpeed, Time.time - lastStartTime);
            if (firstBodySegment != null)
            {
                firstBodySegment.QueueStep(completedStep);
            }

            currentDirection = lastPlayerDirectionInput;
            body2d.velocity = currentDirection * headSpeed;
            lastStartPos = transform.position;
            lastStartTime = Time.time;
        }

    }

    void SpawnBodySegment()
    {
        GameObject newSegment = GameObject.Instantiate(bodySegmentPrefab);
        SnakeBody newSegmentScript = newSegment.GetComponent<SnakeBody>();

        Vector3 lastSegmentPosition;

        if(bodySegments.Count < 1) //first segment
        {
            lastSegmentPosition = transform.position;

            firstBodySegment = newSegmentScript;
        }
        else
        {
            SnakeBody oldLastSegment = bodySegments[bodySegments.Count - 1];

            lastSegmentPosition = oldLastSegment.transform.position;

            oldLastSegment.nextPart = newSegmentScript;
        }

        newSegment.transform.position = lastSegmentPosition + (lastSegmentPosition - lastStartPos).normalized * segmentSize;

        bodySegments.Add(newSegmentScript); 
    }
}
