using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RCAS;
using eDIA.Utilities;

// Communication manager interface. Translates internal commands into network packages and viseversa
// ==============================================================================================================================================
namespace eDIA {

      /// <summary>
      /// Communication manager interface. Translates internal commands into network packages and viseversa
      /// </summary>
      public class RCAS2Experiment : MonoBehaviour {

            private void Awake() {
                  StartForwarder();
            }

            // ==============================================================================================================================================

            // * FROM MANAGER <<

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvSetSessionInfo)]
            static void NwEvSetSessionInfo(string[] sessionInfoJSONstrings) {
                  // We are sending a array of data
                  AddToLog("NwEvSetSessionInfo:" + sessionInfoJSONstrings[0]);
                  EventManager.TriggerEvent(eDIA.Events.Config.EvSetSessionInfo, new eParam(sessionInfoJSONstrings));
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvSetXBlockSequence)]
            static void NwEvSetEBlockSequence(string blockSequenceJSONstring) {
                  AddToLog("NwEvSetEBlockSequence" + blockSequenceJSONstring);
                  EventManager.TriggerEvent(eDIA.Events.Config.EvSetXBlockSequence, new eParam(blockSequenceJSONstring));
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvSetXBlockDefinitions)]
            static void NwEvSetEBlockDefinitions(string[] blockDefintionsJSONstrings) {
                  AddToLog("NwEvSetEBlockDefinitions" + blockDefintionsJSONstrings.Length);
                  EventManager.TriggerEvent(eDIA.Events.Config.EvSetXBlockDefinitions, new eParam(blockDefintionsJSONstrings));
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvSetTaskDefinitions)]
            static void NwEvSetTaskDefinitions(string[] taskDefinitionsJSONstrings) {
                  AddToLog("NwEvSetTaskDefinitions" + taskDefinitionsJSONstrings.Length);
                  EventManager.TriggerEvent(eDIA.Events.Config.EvSetTaskDefinitions, new eParam(taskDefinitionsJSONstrings));
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvStartExperiment)]
            static void NwEvStartExperiment() {
                  AddToLog("NwEvStartExperiment");
                  EventManager.TriggerEvent(eDIA.Events.StateMachine.EvStartExperiment, null);
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvProceed)]
            static void NwEvProceed() {
                  AddToLog("NwEvProceed");
                  EventManager.TriggerEvent(eDIA.Events.StateMachine.EvProceed, null);
            }

            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvToggleCasting)]
            static void NwEvToggleCasting() {
                  AddToLog("NwEvToggleCasting");
                  EventManager.TriggerEvent(eDIA.Events.Casting.EvToggleCasting, null);
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
                  eDIA.LogUtilities.AddToLog(_msg, "EXP", Color.cyan);
            }


            // ==============================================================================================================================================

            // * TO MANAGER >>

            private void StartForwarder() {

                  // Configs
                  EventManager.StartListening(eDIA.Events.Config.EvReadyToGo, NwEvReadyToGo);

                  // Control panel
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvEnableButton, NwEvEnableButton);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvUpdateStepProgress, NwEvUpdateStepProgress);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvUpdateTrialProgress, NwEvUpdateTrialProgress);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvUpdateBlockProgress, NwEvUpdateBlockProgress);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvUpdateSessionSummary, NwEvUpdateSessionSummary);
                  //EventManager.StartListening(eDIA.Events.ControlPanel.EvUpdateProgressInfo, NwEvUpdateProgressInfo);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvStartTimer, NwEvStartTimer);
                  EventManager.StartListening(eDIA.Events.ControlPanel.EvStopTimer, NwEvStopTimer);

                  // Eye
                  EventManager.StartListening(eDIA.Events.Eye.EvEnableEyeCalibrationTrigger, NwEvEnableEyeCalibrationTrigger);

            }


            // Configs


            private void NwEvReadyToGo(eParam obj) {
                  Debug.Log("Sending NwEvReadyToGo");
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvReadyToGo);
            }


            // Control panel

            private void NwEvEnableButton(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvEnableButton, obj.GetStrings());
            }

            private void NwEvUpdateStepProgress(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvUpdateStepProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
            }

            private void NwEvUpdateTrialProgress(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvUpdateTrialProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
            }

            private void NwEvUpdateBlockProgress(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvUpdateBlockProgress, ArrayTools.ConvertIntsToStrings(obj.GetInts()));
            }

            private void NwEvUpdateSessionSummary(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvUpdateSessionSummary, obj.GetStrings());
            }

            private void NwEvUpdateProgressInfo(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvUpdateProgressInfo, obj.GetString());
            }

            private void NwEvStartTimer(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvStartTimer, obj.GetFloat().ToString());
            }

            private void NwEvStopTimer(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvStopTimer);
            }


            // Eye

            private void NwEvEnableEyeCalibrationTrigger(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvEnableEyeCalibrationTrigger, obj.GetBool().ToString());
            }

      }
}