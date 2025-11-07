using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;
using System;


public class DebugExperimentDummy : MonoBehaviour
	{
		private void Awake() {

			//EventManager.StartListening(eDIA.Events.Config.EvSetExperimentConfig, OnSetExpConfig);
			//EventManager.StartListening(eDIA.Events.Config.EvSetTaskConfig, OnSetExpConfig);
		}

	private void OnSetExpConfig(eParam obj)
	{
		Debug.Log("trigger EvReadyToGo");
		EventManager.TriggerEvent(Edia.Events.Config.EvReadyToGo, null);
	}
}
	
