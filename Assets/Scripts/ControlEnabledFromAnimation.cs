using UnityEngine;
using System.Collections;

//allos animations to set their game objects to be active or inactive
public class ControlEnabledFromAnimation : MonoBehaviour {

	public void SetActiveFromAnim(bool active)
    {
        gameObject.SetActive(enabled);
    }
}
