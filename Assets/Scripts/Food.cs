using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {
	public GameObject Player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		GameObject.Destroy (gameObject);
	}
}
