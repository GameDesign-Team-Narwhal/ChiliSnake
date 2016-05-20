using UnityEngine;


public class MovementStep
{
    //normalized vector in the direction of motion
    public PolarVec2 direction;

    public float time;

    public float speed;

    public MovementStep(PolarVec2 direction, float time, float speed)
    {
        Debug.Log("Constructor Speed: " + speed);

        this.direction = direction;
        this.time = time;
        this.speed = speed;
    }
}

