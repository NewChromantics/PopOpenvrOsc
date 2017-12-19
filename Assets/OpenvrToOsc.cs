using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenvrToOsc : MonoBehaviour {

	public PopOsc	OscOutput 	{	get{	return GameObject.FindObjectOfType<PopOsc>();}}

	public bool		SuffixJoystickIndex = true;

	[Header("Set these strings to empty to stop them sending")]
	public string	PositionName = "Position";
	public string	RotationName = "Position";
	public string	TriggerName = "Trigger";
	public string	TouchpadAxisName = "TouchpadAxis";
	public string	IsTrackingName = "IsTracking";

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

		for (int Joystick = 0;	Joystick < ControllerFrames.Count;	Joystick++) {
			var Frame = ControllerFrames [Joystick];
			var IsTracking = Frame.Tracking;

			Osc.Push (GetOscName (IsTrackingName, Joystick), IsTracking ? 1 : 0);

			if (!IsTracking)
				continue;

			//	rotation as eulars
			var Rotation = Frame.Rotation.eulerAngles;

			Osc.Push (GetOscName (PositionName, Joystick), Frame.Position);	
			Osc.Push (GetOscName (RotationName, Joystick), Rotation);
			Osc.Push (GetOscName (TriggerName, Joystick), Frame.TriggerAxis);	
			Osc.Push (GetOscName (TouchpadAxisName, Joystick), Frame.TouchpadAxis);	
		}
	}


}
