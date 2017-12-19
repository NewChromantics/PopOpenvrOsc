using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


[System.Serializable]
public class UnityEvent_Integer : UnityEvent <int> {}


[System.Serializable]
public class IntegerToSliderLink
{
	public int					Value;
	public Slider				InputSlider;
	public UnityEvent_Integer	OnChanged;

	public static implicit operator IntegerToSliderLink(int InitialValue)
	{
		var stoui = new IntegerToSliderLink();
		stoui.Value = InitialValue;
		return stoui;
	}


	public void			OnValueChanged(float NewValue)
	{
		this.Value = (int)NewValue;
		OnChanged.Invoke (Value);
	}

	public void			Init()
	{
		//	initialise ui
		InputSlider.value = Value;
		InputSlider.onValueChanged.AddListener (OnValueChanged);
	}

};


public class IntegerToUILinkComponent : MonoBehaviour {

	public IntegerToSliderLink		Link;

	void Awake()
	{
		Link.Init ();

		//	make this link the master value and do an initial "reset"
		Link.OnValueChanged (Link.Value);
	}

}
