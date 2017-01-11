using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemChecker : MonoBehaviour
{
    void Awake() {
        SceneManager.sceneLoaded += delegate {
            TheLevelWasLoaded();
        };
    }

	//OnLevelWasLoaded is called after a new scene has finished loading
	void TheLevelWasLoaded ()
	{
		//If there is no EventSystem (needed for UI interactivity) present
		if(!FindObjectOfType<EventSystem>())
		{
			//The following code instantiates a new object called EventSystem
			GameObject obj = new GameObject("EventSystem");

			//And adds the required components
			obj.AddComponent<EventSystem>();
			obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
		}
	}
}
