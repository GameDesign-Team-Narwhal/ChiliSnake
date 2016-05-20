using UnityEngine;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour {

    //height and width of head and body textures
    public int segmentSize;

    //offset in px between segments
    public int segmentOffset = 1;

    //pixels / sec
    public float speed = 30;

    public FixedCircularQueue<Vector3> segmentPositions = new FixedCircularQueue<Vector3>(0);

    public Vector2 lastPlayerDirectionInput;

    public uint bodySegmentsToSpawn;
    public GameObject bodySegmentPrefab;

    private Rigidbody2D body2d;

    private List<SnakeBody> bodySegments = new List<SnakeBody>();

	void Awake () {
        body2d = GetComponent<Rigidbody2D>();

    }

    void Start()
    {
        for(uint counter = 0; counter < bodySegmentsToSpawn; ++counter)
        {
            SpawnBodySegment();
        }
    }

    void FixedUpdate ()
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

        body2d.velocity = lastPlayerDirectionInput * speed;

        segmentPositions.Enqueue(transform.position);
    }

    void SpawnBodySegment()
    {
        GameObject newSegment = GameObject.Instantiate(bodySegmentPrefab);
        SnakeBody newSegmentScript = newSegment.GetComponent<SnakeBody>();

        // the 30 comes from FixedUpdate being called 30 times per second
        uint distanceFromPrevSegment /* in updates */ = (uint)Mathf.RoundToInt((segmentSize + segmentOffset) * 30f / speed);


        newSegmentScript.indexInQueue = (uint)(((int)segmentPositions.maxSize) - 1 + distanceFromPrevSegment);
        newSegmentScript.head = this;

        segmentPositions.Resize(segmentPositions.maxSize + distanceFromPrevSegment);

        Vector3 lastSegmentPosition;

        if(bodySegments.Count < 1) //first segment
        {
            lastSegmentPosition = transform.position;

        }
        else
        {
            
            SnakeBody oldLastSegment = bodySegments[bodySegments.Count - 1];

            lastSegmentPosition = oldLastSegment.transform.position;
        }

        newSegment.transform.position = lastSegmentPosition;

        bodySegments.Add(newSegmentScript); 
    }
}
