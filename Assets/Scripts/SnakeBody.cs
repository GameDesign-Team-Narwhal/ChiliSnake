using UnityEngine;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour {

    public Vector2 nextPos;
    public SnakeBody nextPart;

    private Queue<MovementStep> movesToExecute = new Queue<MovementStep>();
    private Rigidbody2D body2d;

    private MovementStep currentStep;
    private float stepStartTime;

    public void QueueStep(MovementStep step)
    {
        movesToExecute.Enqueue(step);
    }

    // Use this for initialization
    void Awake ()
    {
        body2d = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        //check if we need to start a new step
        if (currentStep == null || (stepStartTime + currentStep.time) > Time.time)     
        {
            if (currentStep != null && nextPart != null)
            {
                //pass the step down
                nextPart.QueueStep(currentStep);
            }

            if(movesToExecute.Count > 0)
            {
                currentStep = movesToExecute.Dequeue();

                //execute the new step
                body2d.velocity = currentStep.direction.Cartesian2D * currentStep.speed;
                Debug.Log("Body speed: " + currentStep.speed);
                stepStartTime = Time.time;
            }
            else
            {
                //set this so that we will check for a new step next time
                currentStep = null;
            }


        }
	}
}
