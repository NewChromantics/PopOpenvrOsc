using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCalibration : MonoBehaviour {

	[Range(-180,180)]
	public float			YawRotation = 0;

	[Range(-10,10)]
	public float			Width = 1;

	[Range(-10,10)]
	public float			Depth = 1;

	[Range(-10,10)]
	public Vector2			CenterX = 0;
	[Range(-10,10)]
	public Vector2			CenterZ = 0;
	public Vector2			Center		{	get{	return new Vector2 (CenterX, CenterZ);	}}


	public Matrix4x4		GetTransformPositionMatrix()
	{
		var Rotation = Quaternion.Euler (new Vector3 (0, YawRotation, 0));
		var Transform = Matrix4x4.Rotate( Rotation );
		return Transform;
	}

	public Vector2[]		GetBounds()
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
	
		var Bounds2 = new Vector2[4];
		for (int i = 0;	i < Bounds.Length;	i++)
			Bounds2 [i] = new Vector2 (Bounds [i].x, Bounds [i].z);

		return Bounds2;
	}
}
