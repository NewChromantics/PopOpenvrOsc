using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class StringToUiLink
{
	public string				Value;
	public InputField			Input;
	public UnityEvent_String	OnChanged;

	public static implicit operator StringToUiLink(string InitialValue)
	{
		var stoui = new StringToUiLink();
		stoui.Value = InitialValue;
		return stoui;
	}


	public void			OnValueChanged(string NewValue)
	{
		this.Value = NewValue;
		OnChanged.Invoke (Value);
	}

	public void			Init()
	{
		//	initialise ui
		Input.text = Value;
		Input.onValueChanged.AddListener (OnValueChanged);
	}

};


public class StringToUILinkComponent : MonoBehaviour {

	public StringToUiLink		Link;

	void Awake()
	{
		Link.Init ();

		//	make this link the master value and do an initial "reset"
		Link.OnValueChanged (Link.Value);
	}

}
