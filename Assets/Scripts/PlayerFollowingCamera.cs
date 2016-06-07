using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
//when the player moves near the edges of the screen, this camera pans to follow it.
public class PlayerFollowingCamera : MonoBehaviour {

    public uint deadzonePercentage = 50;

    private Camera cameraToOperate;

    void Awake()
    {
        cameraToOperate = GetComponent<Camera>();
    }

	void Update ()
    {
        if(GameManager.instance.snakeHeadInstance != null)
        {
            Vector2 distance = GameManager.instance.snakeHeadInstance.transform.position - transform.position;
            Vector2 acceptableDistanceFromCam = new Vector2();

            acceptableDistanceFromCam.y = cameraToOperate.orthographicSize * deadzonePercentage / 100f;
            acceptableDistanceFromCam.x = cameraToOperate.orthographicSize * cameraToOperate.aspect * deadzonePercentage / 100f;

            Vector3 camPos = gameObject.transform.position;

            if (Mathf.Abs(distance.x) > acceptableDistanceFromCam.x)
            {
                camPos.x += Mathf.Sign(distance.x) * (Mathf.Abs(distance.x) - acceptableDistanceFromCam.x);
            }
            if (Mathf.Abs(distance.y) > acceptableDistanceFromCam.y)
            {
                camPos.y += Mathf.Sign(distance.y) * (Mathf.Abs(distance.y) - acceptableDistanceFromCam.y);
            }
            transform.position = camPos;

        }
    }
}
