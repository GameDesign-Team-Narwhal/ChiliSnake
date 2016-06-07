using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour {

    public Vector2 nextPos;

    public SnakeHead head;

    public uint indexInQueue;
   
    //segments before and after this one which will not trigger a collision if this segment touches them.
    public GameObject segmentBefore, segmentAfter;

    private Animator animator;

    //list of segments to ignore collisions with
    public HashSet<GameObject> ignoreCollisionSegments = new HashSet<GameObject>();

    //delay between the death animations of adjacent segments starting
    public float segmentDeathDelay = .2f;

    //set to false when the segment dies
    private bool attachedToSnake = true;

    // Use this for initialization
    void Awake ()
    {
        animator = GetComponent<Animator>();
	}
	
	void FixedUpdate ()
    {
        //Debug.Log("New position: " + head.segmentPositions[indexInQueue].ToString());
        if (attachedToSnake && head.segmentPositions.lastIndex >= indexInQueue)
        {
            Pair<Vector3, Quaternion> newPos = head.segmentPositions[indexInQueue];

            if(newPos != null)
            {
                transform.position = newPos.left;
                transform.rotation = newPos.right;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (!ignoreCollisionSegments.Contains(other.gameObject))
        {

            Debug.Log("Snake collision detected.  Other object: " + other.gameObject.ToString());

            Debug.Log("Snake collision detected. This segment: #" + head.bodySegments.IndexOf(this) + " Other object: " + other.gameObject.ToString());

            if(head != null)
            {
                head.OnCollision(this, other.gameObject);
            }
        }
    }

    //kill this segment and all segments after it
    public void DieRecursive(float builtupDelay)
    {
        StartCoroutine(DieAfterDelay(builtupDelay));
        attachedToSnake = false;

        if (segmentAfter != null)
        {
            segmentAfter.GetComponent<SnakeBody>().DieRecursive(builtupDelay + segmentDeathDelay);
        }

    }

    public IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Die");

        yield break;
    }
}
