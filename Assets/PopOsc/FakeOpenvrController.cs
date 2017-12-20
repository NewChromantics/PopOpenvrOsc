using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeOpenvrController : MonoBehaviour {

	OpenvrControllerManager	OpenvrManager	{	get	{ return GameObject.FindObjectOfType<OpenvrControllerManager> (); }}

	[Range(0,10)]
	public int		FakeControllerIndex = 9;

	[Range(-1,3)]
	public float	FakeY = 0;

	[Range(1,100)]
	public float	DisplayWidth = 40;
	public float	DisplayHeight	{	get { return DisplayWidth / 2.0f; } }

	void SendFrame(OpenvrControllerFrame Frame)
	{
		var Frames = new List<OpenvrControllerFrame> ();
		for ( int i=0;	i<FakeControllerIndex;	i++ )
			Frames.Add(null);
		Frames.Add (Frame);
		OpenvrManager.OnUpdateAll.Invoke (Frames);
	}

	public void TrackPosition(Vector2 uv)
	{
		uv.x /= DisplayWidth / 2.0f;
		uv.y /= DisplayHeight / 2.0f;

		var Frame = new OpenvrControllerFrame ();
		Frame.Position.x = uv.x;
		Frame.Position.y = FakeY;
		Frame.Position.z = uv.y;
		Frame.Attached = true;
		Frame.Tracking = true;
		SendFrame (Frame);
	}

	public void UnTrack()
	{
		var Frame = new OpenvrControllerFrame ();
		Frame.Attached = true;
		Frame.Tracking = false;
		SendFrame (Frame);
	}
}
