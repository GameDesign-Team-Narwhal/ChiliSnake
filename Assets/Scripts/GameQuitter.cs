using UnityEngine;
using System.Collections;

//helper script to exit the game from an event
public class GameQuitter : MonoBehaviour {

	public void QuitGame()
    {
        Application.Quit();
    }
}
