using UnityEngine;
using System.Collections;

// Helper script for loading anew level from an event
public class LevelLoader : MonoBehaviour {

	public void LoadLevel(int index)
    {
        Application.LoadLevel(index);
    }
}
