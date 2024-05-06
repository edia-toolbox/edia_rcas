using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Edia;
<<<<<<< Updated upstream
using Edia.Rcas;
=======
>>>>>>> Stashed changes
using Edia.Utilities;

// Communication manager interface. Translates internal commands into network packages and viseversa
// ==============================================================================================================================================
<<<<<<< Updated upstream
=======
namespace Edia.RCAS {

	/// <summary>
	/// Communication manager interface. Translates internal commands into network packages and viseversa
	/// </summary>
	public class RCAS2Experiment : MonoBehaviour {

		private void Awake() {
			StartForwarder();
		}

		// ==============================================================================================================================================

		// * FROM MANAGER <<

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetSessionInfo)]
		static void NwEvSetSessionInfo(string[] sessionInfoJSONstrings) {
			// We are sending a array of data
			AddToLog("NwEvSetSessionInfo:" + sessionInfoJSONstrings[0]);
			EventManager.TriggerEvent(Edia.Events.Config.EvSetSessionInfo, new eParam(sessionInfoJSONstrings));
		}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetXBlockSequence)]
		static void NwEvSetXBlockSequence(string blockSequenceJSONstring) {
			AddToLog("NwEvSetEBlockSequence" + blockSequenceJSONstring);
			EventManager.TriggerEvent(Edia.Events.Config.EvSetXBlockSequence, new eParam(blockSequenceJSONstring));
		}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetXBlockDefinitions)]
		static void NwEvSetXBlockDefinitions(string[] blockDefintionsJSONstrings) {
			AddToLog("NwEvSetEBlockDefinitions" + blockDefintionsJSONstrings.Length);
			EventManager.TriggerEvent(Edia.Events.Config.EvSetXBlockDefinitions, new eParam(blockDefintionsJSONstrings));
		}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetTaskDefinitions)]
		static void NwEvSetTaskDefinitions(string[] taskDefinitionsJSONstrings) {
			AddToLog("NwEvSetTaskDefinitions" + taskDefinitionsJSONstrings.Length);
			EventManager.TriggerEvent(Edia.Events.Config.EvSetTaskDefinitions, new eParam(taskDefinitionsJSONstrings));
		}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvStartExperiment)]
		static void NwEvStartExperiment() {
			AddToLog("NwEvStartExperiment");
			EventManager.TriggerEvent(Edia.Events.StateMachine.EvStartExperiment, null);
		}

		//[RCAS_RemoteEvent(Edia.Events.Network.NwEvFinalizeSession)]
		//static void NwEvFinaliseExperiment() {
		//	AddToLog("NwEvFinaliseExperiment");
		//	EventManager.TriggerEvent(Edia.Events.StateMachine.EvFinalizeSession, null);
		//}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvProceed)]
		static void NwEvProceed() {
			AddToLog("NwEvProceed");
			EventManager.TriggerEvent(Edia.Events.StateMachine.EvProceed, null);
		}

		[RCAS_RemoteEvent(Edia.Events.Network.NwEvToggleCasting)]
		static void NwEvToggleCasting() {
			AddToLog("NwEvToggleCasting");
			EventManager.TriggerEvent(Edia.Events.Casting.EvToggleCasting, null);
		}

		// Left over methods from development

		[RCAS_RemoteEvent("poke")]
		static void Poke() {
			Debug.Log("You got poked!");
		}

		[RCAS_RemoteEvent("SetConfig")]
		static void SetConfig(string message) {
			Debug.Log("string length: " + message.Length);
			Debug.Log("Someone whispers us a message: " + message);

			// Controller.SetConfig(message);
		}

		[RCAS_RemoteEvent("set_params")]
		static void SetParams(string[] args) {
			Debug.Log($"Parameters received: {args[0]}, {args[1]}, {args[2]}");
		}
>>>>>>> Stashed changes


/// <summary>
/// Communication manager interface. Translates internal commands into network packages and viseversa
/// </summary>
public class RCAS2Experiment : MonoBehaviour {

	private void Awake() {
		StartForwarder();
	}

	// ==============================================================================================================================================

	// * FROM MANAGER <<

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetSessionInfo)]
	static void NwEvSetSessionInfo(string[] sessionInfoJSONstrings) {
		// We are sending a array of data
		AddToLog("NwEvSetSessionInfo:" + sessionInfoJSONstrings[0]);
		EventManager.TriggerEvent(Edia.Events.Config.EvSetSessionInfo, new eParam(sessionInfoJSONstrings));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetXBlockSequence)]
	static void NwEvSetXBlockSequence(string blockSequenceJSONstring) {
		AddToLog("NwEvSetEBlockSequence" + blockSequenceJSONstring);
		EventManager.TriggerEvent(Edia.Events.Config.EvSetXBlockSequence, new eParam(blockSequenceJSONstring));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetXBlockDefinitions)]
	static void NwEvSetXBlockDefinitions(string[] blockDefintionsJSONstrings) {
		AddToLog("NwEvSetEBlockDefinitions" + blockDefintionsJSONstrings.Length);
		EventManager.TriggerEvent(Edia.Events.Config.EvSetXBlockDefinitions, new eParam(blockDefintionsJSONstrings));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvSetTaskDefinitions)]
	static void NwEvSetTaskDefinitions(string[] taskDefinitionsJSONstrings) {
		AddToLog("NwEvSetTaskDefinitions" + taskDefinitionsJSONstrings.Length);
		EventManager.TriggerEvent(Edia.Events.Config.EvSetTaskDefinitions, new eParam(taskDefinitionsJSONstrings));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvStartExperiment)]
	static void NwEvStartExperiment() {
		AddToLog("NwEvStartExperiment");
		EventManager.TriggerEvent(Edia.Events.StateMachine.EvStartExperiment, null);
	}

	//[RCAS_RemoteEvent(Edia.Events.Network.NwEvFinalizeSession)]
	//static void NwEvFinaliseExperiment() {
	//	AddToLog("NwEvFinaliseExperiment");
	//	EventManager.TriggerEvent(Edia.Events.StateMachine.EvFinalizeSession, null);
	//}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvProceed)]
	static void NwEvProceed() {
		AddToLog("NwEvProceed");
		EventManager.TriggerEvent(Edia.Events.StateMachine.EvProceed, null);
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvToggleCasting)]
	static void NwEvToggleCasting() {
		AddToLog("NwEvToggleCasting");
		EventManager.TriggerEvent(Edia.Events.Casting.EvToggleCasting, null);
	}

	// Left over methods from development

	[RCAS_RemoteEvent("poke")]
	static void Poke() {
		Debug.Log("You got poked!");
	}

	[RCAS_RemoteEvent("SetConfig")]
	static void SetConfig(string message) {
		Debug.Log("string length: " + message.Length);
		Debug.Log("Someone whispers us a message: " + message);

		// Controller.SetConfig(message);
	}

	[RCAS_RemoteEvent("set_params")]
	static void SetParams(string[] args) {
		Debug.Log($"Parameters received: {args[0]}, {args[1]}, {args[2]}");
	}


	private static void AddToLog(string _msg) {
		Edia.LogUtilities.AddToLog(_msg, "EXP", Color.cyan);
	}


	// ==============================================================================================================================================

	// * TO MANAGER >>

	private void StartForwarder() {

		// Configs
		EventManager.StartListening(Edia.Events.Config.EvReadyToGo, NwEvReadyToGo);

		// Control panel
		EventManager.StartListening(Edia.Events.ControlPanel.EvEnableButton, NwEvEnableButton);
		EventManager.StartListening(Edia.Events.ControlPanel.EvUpdateStepProgress, NwEvUpdateStepProgress);
		EventManager.StartListening(Edia.Events.ControlPanel.EvUpdateTrialProgress, NwEvUpdateTrialProgress);
		EventManager.StartListening(Edia.Events.ControlPanel.EvUpdateBlockProgress, NwEvUpdateBlockProgress);
		EventManager.StartListening(Edia.Events.ControlPanel.EvUpdateSessionSummary, NwEvUpdateSessionSummary);
		EventManager.StartListening(Edia.Events.ControlPanel.EvStartTimer, NwEvStartTimer);
		EventManager.StartListening(Edia.Events.ControlPanel.EvStopTimer, NwEvStopTimer);
		EventManager.StartListening(Edia.Events.StateMachine.EvSessionEnded, NwEvSessionEnded);

		// Eye
		EventManager.StartListening(Edia.Events.Eye.EvEnableEyeCalibrationTrigger, NwEvEnableEyeCalibrationTrigger);

	}


	// Configs


	private void NwEvReadyToGo(eParam obj) {
		Debug.Log("Sending NwEvReadyToGo");
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvReadyToGo);
	}


	// Control panel

	private void NwEvEnableButton(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvEnableButton, obj.GetStrings());
	}

	private void NwEvUpdateStepProgress(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateStepProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
	}

	private void NwEvUpdateTrialProgress(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateTrialProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
	}

	private void NwEvUpdateBlockProgress(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateBlockProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
	}

	private void NwEvUpdateSessionSummary(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateSessionSummary, obj.GetStrings());
	}

	private void NwEvUpdateProgressInfo(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateProgressInfo, obj.GetString());
	}

	private void NwEvStartTimer(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvStartTimer, obj.GetFloat().ToString());
	}

	private void NwEvStopTimer(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvStopTimer);
	}

	private void NwEvSessionEnded(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvSessionEnded);
	}

	// Eye

	private void NwEvEnableEyeCalibrationTrigger(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvEnableEyeCalibrationTrigger, obj.GetBool().ToString());
	}

}
