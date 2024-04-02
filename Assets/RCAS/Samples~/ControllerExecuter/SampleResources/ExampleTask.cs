using Edia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class ExampleTask : XBlock
{
	float UserResponseTime = 0f;
	float StepStartTime = 0;
	public List<string> taskColors = new();

	[Header("Refs")]
	public GameObject Cube;
	public GameObject Sphere;


	private void Awake() {

		/*
			Each trial exists out of a sequence of steps. 
			In order to use them, we need to add the methods of this task to the trial sequence.

			An common approach of these steps within a trial are as follows:
		*/

		AddToTrialSequence(TaskStep1); // In many cases setting up the environment for the task, i.e. Generating Stimuli
		AddToTrialSequence(TaskStep2); // Input from the user
		AddToTrialSequence(TaskStep3); // Check input and log
		AddToTrialSequence(TaskStep4); // Clean up
	}

	// -------------------------------------------------------------------------------------------------------------------------------
	#region TASK STEPS


	/// <summary>Present Cube</summary>
	void TaskStep1() {

		switch(Session.instance.CurrentTrial.settings.GetString("active_object")) {
			case "cube":
				Cube.SetActive(true);
				break;
			case "sphere":
				Sphere.SetActive(true);
				break;

		}

		// Tell the system to wait on proceed
		Experiment.Instance.WaitOnProceed();
		// Tell the system to proceed with delay
		Experiment.Instance.ProceedWithDelay(Session.instance.CurrentBlock.settings.GetFloat("timer_showcube"));
	}

	/// <summary>Move cube, wait on user input</summary>
	void TaskStep2() {

		StepStartTime = Time.time;

		// Show message to user and allow proceeding to NextStep by pressing the button.
		MessagePanelInVR.Instance.ShowMessage("Click the object", 2f);

		// Enable interaction from the user. The system will automaticly enable the Ray Interaction for the active hands set in the settings.
		XRManager.Instance.EnableXRRayInteraction(true);
		
		// Tell the system to wait on proceed
		Experiment.Instance.WaitOnProceed();
	}

	/// <summary>User clicked button</summary>
	void TaskStep3() {

		// Change color
		Cube.GetComponent<Renderer>().material.color = PickColor();

		// calculate response time
		UserResponseTime = Time.time - StepStartTime;

		// Add result to log
		Experiment.Instance.AddToTrialResults("UserResponseTime", UserResponseTime.ToString());

		Experiment.Instance.WaitOnProceed();
		Experiment.Instance.ShowMessageToUser("Wait on experimenter button click.", false);
	}

	/// <summary>Clean up</summary>
	void TaskStep4() {

		HideStimuli();

		Experiment.Instance.WaitOnProceed();
		Experiment.Instance.Proceed();
	}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region TASK HELPERS

	Color PickColor() {
		string pickedColorString = taskColors[Random.Range(0,taskColors.Count)];
		ColorUtility.TryParseHtmlString(pickedColorString, out Color color);
		return color;
	}
	
	public void UserClicked () {
		Debug.Log("UserClicked ");
		XRManager.Instance.EnableXRRayInteraction(false);
		Experiment.Instance.Proceed();
	}

	void HideStimuli() {
		// Hide stimuli
		Cube.SetActive(false);
		Sphere.SetActive(false);
	}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region STATEMACHINE OVERRIDES

	public override void OnBlockStart() {
		HideStimuli();

		// Get the colors for this xblock
		taskColors = Session.instance.CurrentBlock.settings.GetStringList("task_colors");
	}

	public override void OnStartTrial() {
		// Disable XR interaction from the user
		XRManager.Instance.EnableXRRayInteraction(false);

		HideStimuli();
	}


	public override void OnEndTrial() {
	}

	public override void OnBetweenSteps() {
	}

	public override void OnBlockOutro() {
	}

	public override void OnBlockEnd() {
	}


	#endregion // -------------------------------------------------------------------------------------------------------------------------------
}

