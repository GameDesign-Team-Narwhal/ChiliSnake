using UnityEngine;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour {

    public Vector2 nextPos;

    public SnakeHead head;
    public uint indexInQueue;

    private Rigidbody2D body2d;

    // Use this for initialization
    void Awake ()
    {
        body2d = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate ()
    {
        //Debug.Log("New position: " + head.segmentPositions[indexInQueue].ToString());
        if (head.segmentPositions.lastIndex >= indexInQueue)
        {

            transform.position = head.segmentPositions[indexInQueue];
        }
    }
}
