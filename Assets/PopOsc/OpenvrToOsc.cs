using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenvrToOsc : MonoBehaviour {

	public PopOsc				OscOutput 	{	get{	return GameObject.FindObjectOfType<PopOsc>();}}
	public ManualCalibration	Calibration {	get{	return GameObject.FindObjectOfType<ManualCalibration>();}}

	public bool		SuffixJoystickIndex = true;

	[Header("Set these strings to empty to stop them sending")]
	public StringToUiLink	PositionName = "Position";
	public StringToUiLink	RotationName = "Position";
	public StringToUiLink	TriggerName = "Trigger";
	public StringToUiLink	TouchpadAxisName = "TouchpadAxis";
	public StringToUiLink	IsTrackingName = "IsTracking";


	void Awake()
	{
		PositionName.Init();
		RotationName.Init();
		TriggerName.Init();
		TouchpadAxisName.Init();
		IsTrackingName.Init();
	}

	string			GetOscName(string Name,int JoystickIndex)
	{
		if (string.IsNullOrEmpty (Name))
			return null;
		
		var OscName = Name;
		if ( SuffixJoystickIndex )
			OscName += JoystickIndex;

		return OscName;
	}

	public void		OnControllerUpdate(List<OpenvrControllerFrame> ControllerFrames)
	{
		var Osc = OscOutput;
		if (Osc == null)
			return;

		for (int Joystick = 0;	Joystick < ControllerFrames.Count;	Joystick++) {
			var Frame = ControllerFrames [Joystick];
			if (Frame == null)
				continue;
			var IsTracking = Frame.Tracking;

			Osc.Push (GetOscName (IsTrackingName.Value, Joystick), IsTracking ? 1 : 0);

			if (!IsTracking)
				continue;

			//	rotation as eulars
			var Rotation = Frame.Rotation.eulerAngles;
			var Position = Calibration.GetCalibratedPosition (Frame.Position);

			Osc.Push (GetOscName (PositionName.Value, Joystick), Position);	
			Osc.Push (GetOscName (RotationName.Value, Joystick), Rotation);
			Osc.Push (GetOscName (TriggerName.Value, Joystick), Frame.TriggerAxis);	
			Osc.Push (GetOscName (TouchpadAxisName.Value, Joystick), Frame.TouchpadAxis);	
		}
	}


}
