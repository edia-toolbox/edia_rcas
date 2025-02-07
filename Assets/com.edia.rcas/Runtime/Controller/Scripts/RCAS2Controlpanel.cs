using Edia;
using Edia.Utilities;
using Edia.Rcas;
using UnityEngine;

/// <summary> In project version of the connector to a remote interface </summary>
public class RCAS2Controlpanel : MonoBehaviour {

	#region TO EXECUTER >>

	private void Awake() {
		// * TO EXECUTER >>
		
		// Settings
		EventManager.StartListening(Edia.Events.Settings.EvRequestSystemSettings, NwEvRequestSystemSettings); 

		// State machine
		EventManager.StartListening(Edia.Events.StateMachine.EvStartExperiment, NwEvStartExperiment);
		EventManager.StartListening(Edia.Events.StateMachine.EvPauseExperiment, NwEvPauseExperiment);
		EventManager.StartListening(Edia.Events.StateMachine.EvProceed, NwEvProceed);

		// Features
		EventManager.StartListening(Edia.Events.Casting.EvToggleCasting, NwEvToggleCasting);

		// Configs
		EventManager.StartListening(Edia.Events.Config.EvSetSessionInfo, NwEvSetSessionInfo);
		EventManager.StartListening(Edia.Events.Config.EvSetXBlockSequence, NwEvSetXBlockSequence);
		EventManager.StartListening(Edia.Events.Config.EvSetXBlockDefinitions, NwEvSetXBlockDefinitions);
		EventManager.StartListening(Edia.Events.Config.EvSetTaskDefinitions, NwEvSetTaskDefinitions);

		// Control panel
		EventManager.StartListening(Edia.Events.ControlPanel.EvNextMessagePanelMsg, NwEvNextMessagePanelMsg);
		
	}



	// * TO EXECUTER >>
	private void NwEvUpdateSystemSettings(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvUpdateSystemSettings, obj.GetString());
	}
	
	private void NwEvRequestSystemSettings(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvRequestSystemSettings);
	}

	private void NwEvNextMessagePanelMsg(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvNextMessagepanelMsg);
	}

	private void NwEvStartExperiment(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvStartExperiment);
	}

	private void NwEvPauseExperiment(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvPauseExperiment);
	}

	private void NwEvProceed(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvProceed);
	}

	private void NwEvToggleCasting(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvToggleCasting);
	}

	private void NwEvSetSessionInfo(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvSetSessionInfo, obj.GetStrings());
	}

	private void NwEvSetXBlockSequence(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvSetXBlockSequence, obj.GetString());
	}

	private void NwEvSetXBlockDefinitions(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvSetXBlockDefinitions, obj.GetStrings());
	}

	private void NwEvSetTaskDefinitions(eParam obj) {
		RCAS_Peer.Instance.TriggerRemoteEvent(Edia.Events.Network.NwEvSetTaskDefinitions, obj.GetStrings());
	}

	#endregion // -------------------------------------------------------------------------------------------------------------------------------
	#region FROM EXECUTER <<

	// Settings
	
	[RCAS_RemoteEvent(Edia.Events.Network.NwEvProvideSystemSettings)]
	static void NwEvProvideSystemSettings(string args) {
		Debug.Log("Received NwEvProvideSystemSettings");
		EventManager.TriggerEvent(Edia.Events.Settings.EvProvideSystemSettings, new eParam(args));
	}
	
	// Configs

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvReadyToGo)]
	static void NwEvReadyToGo() {
		Debug.Log("Received NwEvReadyToGo");
		EventManager.TriggerEvent(Edia.Events.Config.EvReadyToGo);
	}


	// ControlPanel

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvEnableButton)]
	static void NwEvEnableButton(string[] args) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvEnableButton, new eParam(args));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvUpdateStepProgress)]
	static void NwEvUpdateStepProgress(string[] args) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvUpdateStepProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvUpdateTrialProgress)]
	static void NwEvUpdateTrialProgress(string[] args) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvUpdateTrialProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvUpdateBlockProgress)]
	static void NwEvUpdateBlockProgress(string[] args) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvUpdateBlockProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvUpdateSessionSummary)]
	static void NwEvUpdateSessionSummary(string[] args) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvUpdateSessionSummary, new eParam(args));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvUpdateProgressInfo)]
	static void NwEvUpdateProgressInfo(string arg) {
		//EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateProgressInfo, new eParam(arg));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvSessionEnded)]
	static void NwEvSessionEnded(string arg) {
		EventManager.TriggerEvent(Edia.Events.StateMachine.EvSessionEnded, new eParam(arg));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvEnableEyeCalibrationTrigger)]
	static void NwEvEnableEyeCalibrationTrigger(string arg) {
		EventManager.TriggerEvent(Edia.Events.Eye.EvEnableEyeCalibrationTrigger, new eParam(bool.Parse(arg)));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvStartTimer)]
	static void NwEvStartTimer(string arg) {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvStartTimer, new eParam(float.Parse(arg)));
	}

	[RCAS_RemoteEvent(Edia.Events.Network.NwEvStopTimer)]
	static void NwEvStopTimer() {
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvStopTimer);
	}

	#endregion // -------------------------------------------------------------------------------------------------------------------------------
}

