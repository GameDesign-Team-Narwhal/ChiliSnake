using UnityEngine;
using System.Collections;


//use this script to destroy the GameObject from an animation event 
public class DestroyFromAnimation: MonoBehaviour {

    public void DestroyObject()
    {
        GameObject.Destroy(gameObject);
    }
}
