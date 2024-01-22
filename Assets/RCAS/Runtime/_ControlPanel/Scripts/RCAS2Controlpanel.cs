using System;
using System.Collections;
using System.Collections.Generic;
using eDIA;
using eDIA.Utilities;
using RCAS;
using UnityEngine;
using UnityEngine.UI;

namespace eDIA.Manager
{
	/// <summary> In project version of the connector to a remote interface </summary>
	public class RCAS2Controlpanel : MonoBehaviour
	{

#region TO APP >>

		private void Awake()
		{
			// * TO APP >>

			// State machine
			EventManager.StartListening(eDIA.Events.StateMachine.EvStartExperiment, NwEvStartExperiment);
			EventManager.StartListening(eDIA.Events.StateMachine.EvPauseExperiment, NwEvPauseExperiment);
			EventManager.StartListening(eDIA.Events.StateMachine.EvProceed, 		NwEvProceed);

			// Features
			EventManager.StartListening(eDIA.Events.Casting.EvToggleCasting, 		NwEvToggleCasting);

			// Configs
			EventManager.StartListening(eDIA.Events.Config.EvSetSessionInfo,		NwEvSetSessionInfo);
			EventManager.StartListening(eDIA.Events.Config.EvSetEBlockSequence,	NwEvSetEBlockSequence);
                  EventManager.StartListening(eDIA.Events.Config.EvSetEBlockDefinitions,	NwEvSetBlockDefinitions);
                  EventManager.StartListening(eDIA.Events.Config.EvSetTaskDefinitions,	NwEvSetTaskDefinitions);

            }


            // * TO APP >>


            private void NwEvStartExperiment(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvStartExperiment);
		}

		private void NwEvPauseExperiment(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvPauseExperiment);
		}

		private void NwEvProceed(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvProceed);
		}

		private void NwEvToggleCasting(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvToggleCasting);
		}

		private void NwEvSetSessionInfo(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvSetSessionInfo, obj.GetString());
		}

		private void NwEvSetEBlockSequence(eParam obj)
		{
			RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvSetEBlockSequence, obj.GetString());
		}

            private void NwEvSetBlockDefinitions(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvSetEBlockDefinitions, obj.GetString());
            }

            private void NwEvSetTaskDefinitions(eParam obj) {
                  RCAS_Peer.Instance.TriggerRemoteEvent(eDIA.Events.Network.NwEvSetTaskDefinitions, obj.GetString());
            }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region FROM APP <<

            // * FROM APP <<

            // Configs


            [RCAS_RemoteEvent(eDIA.Events.Network.NwEvReadyToGo)]
		static void NwEvReadyToGo()
		{
			Debug.Log("Received NwEvReadyToGo");
			EventManager.TriggerEvent(eDIA.Events.Config.EvReadyToGo);
		}


		// ControlPanel

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvEnableButton)]
		static void NwEvEnableButton(string[] args)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvEnableButton, new eParam(args));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvUpdateStepProgress)]
		static void NwEvUpdateStepProgress(string[] args)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateStepProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvUpdateTrialProgress)]
		static void NwEvUpdateTrialProgress(string[] args)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateTrialProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvUpdateBlockProgress)]
		static void NwEvUpdateBlockProgress(string[] args)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateBlockProgress, new eParam(ArrayTools.ConvertStringsIntoInts(args)));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvUpdateSessionSummary)]
		static void NwEvUpdateSessionSummary(string[] args)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateSessionSummary, new eParam(args));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvUpdateProgressInfo)]
		static void NwEvUpdateProgressInfo(string arg)
		{
			//EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvUpdateProgressInfo, new eParam(arg));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvEnableEyeCalibrationTrigger)]
		static void NwEvEnableEyeCalibrationTrigger(string arg)
		{
			EventManager.TriggerEvent(eDIA.Events.Eye.EvEnableEyeCalibrationTrigger, new eParam(bool.Parse(arg)));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvStartTimer)]
		static void NwEvStartTimer(string arg)
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvStartTimer, new eParam(float.Parse(arg)));
		}

		[RCAS_RemoteEvent(eDIA.Events.Network.NwEvStopTimer)]
		static void NwEvStopTimer()
		{
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvStopTimer);
		}


#endregion // -------------------------------------------------------------------------------------------------------------------------------
	}
}
