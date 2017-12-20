using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ManualCalibration : MonoBehaviour {

	[Range(-180,180)]
	public float			YawRotation = 0;

	[Range(0,30)]
	public float			Width = 1;

	[Range(0,30)]
	public float			Depth = 1;

	[Range(-10,10)]
	public float			CenterX = 0;
	[Range(-10,10)]
	public float			CenterZ = 0;
	public Vector2			Center2		{	get{	return new Vector2 (CenterX, CenterZ);	}}
	public Vector3			Center3		{	get{	return new Vector3 (CenterX, 0, CenterZ);	}}

	public Material			DisplayMaterial;

	Vector4[]				LastPositions = new Vector4[10];	//	w = enabled

	void ResetPositions()
	{
		LastPositions = new Vector4[10];
		
		for (int i = 0;	i < LastPositions.Length;	i++)
			LastPositions [i] = new Vector4 (i, 0, i, 0);
	}

	public void				SetPosition(int Player,Vector3? Position)
	{
		if (Position.HasValue) {
			var Pos = Position.Value;
			LastPositions [Player].x = Pos.x;
			LastPositions [Player].y = Pos.y;
			LastPositions [Player].z = Pos.z;
		}
		LastPositions [Player].w = Position.HasValue ? 1 : 0.5f;
	}

	public Matrix4x4		GetTransformPositionMatrix()
	{
		var Rotation = Quaternion.Euler (new Vector3 (0, YawRotation, 0));
		var Transform = Matrix4x4.Rotate( Rotation );
		var Translate = Matrix4x4.Translate (Center3);
		Transform = Translate * Transform;
		return Transform;
	}

	public Vector3 		GetCalibratedPosition(Vector3 OriginalPosition)
	{
		var Transform = GetTransformPositionMatrix ().inverse;
		var NormalisedPosition = Transform.MultiplyPoint (OriginalPosition);

		//	gr: this should be in the matrix
		//	convert from min/max to 0..1
		NormalisedPosition.x /= Width;
		NormalisedPosition.z /= Depth;
		NormalisedPosition.x += 0.5f;
		NormalisedPosition.z += 0.5f;

		return NormalisedPosition;
	}

	public Vector3[]		GetBounds()
	{
		var Transform = GetTransformPositionMatrix ();
		var Bounds = new Vector3[4];
		var HalfWidth = Width / 2f;
		var HalfDepth = Depth / 2f;
		Bounds [0] = new Vector3 (-HalfWidth, 0, -HalfDepth);
		Bounds [1] = new Vector3 ( HalfWidth, 0, -HalfDepth);
		Bounds [2] = new Vector3 ( HalfWidth, 0, HalfDepth);
		Bounds [3] = new Vector3 (-HalfWidth, 0, HalfDepth);

		for (int i = 0;	i < Bounds.Length;	i++)
			Bounds [i] = Transform.MultiplyPoint (Bounds [i]);
	
		return Bounds;
	}

	void OnEnabled()
	{
		ResetPositions ();
	}

	void Update()
	{
		UpdateMaterial ();
	}

	void UpdateMaterial()
	{
		if ( DisplayMaterial == null )
			return;

		var Bounds = GetBounds ();
		DisplayMaterial.SetVector ("BoundsTopLeft", Bounds [0]);
		DisplayMaterial.SetVector ("BoundsTopRight", Bounds [1]);
		DisplayMaterial.SetVector ("BoundsBottomRight", Bounds [2]);
		DisplayMaterial.SetVector ("BoundsBottomLeft", Bounds [3]);

		if ( LastPositions != null && LastPositions.Length > 0 )
			DisplayMaterial.SetVectorArray ("PlayerPositions", LastPositions);
	}


	public void		OnControllerUpdate(List<OpenvrControllerFrame> ControllerFrames)
	{
		for (int Joystick = 0;	Joystick < ControllerFrames.Count;	Joystick++) {
			var Frame = ControllerFrames [Joystick];
			if (Frame == null)
				continue;
			var IsTracking = Frame.Tracking;

			Vector3? Pos = null;
			if (IsTracking)
				Pos = Frame.Position;
			SetPosition (Joystick, Pos);
		}
	}
}
