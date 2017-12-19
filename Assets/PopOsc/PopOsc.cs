using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnityEvent_OscMessage : UnityEngine.Events.UnityEvent <OscMessage> {}



public class OscData
{
	protected bool			Dirty = false;
	float[]					_Values;
	public float[]			Values {
		get	{ return _Values; }
		set {
			this._Values = value;
			this.Dirty = true;
		}
	}

	public float[]	GetValues ()
	{
		return _Values;
	}

	public float[]	PopDirtyValues ()
	{
		if (!Dirty)
			return null;
		Dirty = false;
		return _Values;
	}
}
	
/*
 * Interface to sending OSC data
 */
public class PopOsc : MonoBehaviour {

	public UnityEvent_OscMessage		SendOscMessage;

	[Header("Only send data that's changed, or send everything every time")]
	public bool							OnlySendDirtyData = true;
	[Range(1,60)]
	public float						MaxSendPerSecond = 30;
	float								LastSendTime = 0;
	float								SendDelaySecs	{	get{	return 1.0f / MaxSendPerSecond;}}
	float								SecsSinceLastSend{	get{ return Time.time - LastSendTime; }}

	public Dictionary<string,OscData>	Datas;

	OscData			GetData(string Name)
	{
		if ( Datas == null)
			Datas = new Dictionary<string,OscData>();

		if ( !Datas.ContainsKey(Name) )
			Datas.Add(Name, new OscData() );

		var Data = Datas [Name];

		return Data;
	}

	public void		Push(string Name,float Value)
	{
		var Data = GetData(Name);
		Data.Values = new float[1]{Value};
	}

	public void		Push(string Name,Vector2 Value)
	{
		var Data = GetData(Name);
		Data.Values = new float[2]{Value.x,Value.y};
	}

	public void		Push(string Name,Vector3 Value)
	{
		var Data = GetData(Name);
		Data.Values = new float[3]{Value.x,Value.y,Value.z};
	}

	public void		GetDirtyDatas(System.Action<string,float[]> Enumerate)
	{
		if (Datas == null)
			return;

		foreach (var NameAndData in Datas) {
			var Name = NameAndData.Key;
			var Data = NameAndData.Value;
			var DirtyDatas = Data.PopDirtyValues ();
			if (DirtyDatas == null)
				continue;

			Enumerate.Invoke (Name, DirtyDatas);
		}
	}

	public void		GetAllDatas(System.Action<string,float[]> Enumerate)
	{
		if (Datas == null)
			return;

		foreach (var NameAndData in Datas) {
			var Name = NameAndData.Key;
			var Data = NameAndData.Value;
			var Values = Data.GetValues ();

			Enumerate.Invoke (Name, Values);
		}
	}

	void SendData()
	{
		System.Action<string,float[]> SendMessage = (Name, Values) => {

			var Message =  new OscMessage();
			Message.address = "/" + Name;
			Message.values.AddRange(Values);
			
			SendOscMessage.Invoke(Message);
		};

		if (OnlySendDirtyData) {
			GetDirtyDatas (SendMessage);
		} else {
			GetAllDatas (SendMessage);
		}
	}

	void Update()
	{
		//	is it time to send data?
		var TimeSinceSend = SecsSinceLastSend;
		if (TimeSinceSend >= SendDelaySecs) {
			SendData ();
			LastSendTime = Time.time;
		}
	}
}
