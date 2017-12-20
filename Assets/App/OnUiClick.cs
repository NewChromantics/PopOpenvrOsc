using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class UnityEvent_uv : UnityEvent <Vector2> {}



[RequireComponent(typeof(GraphicRaycaster))]
public class OnUiClick : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public UnityEvent_uv	OnMouseDown;
	public UnityEvent		OnMouseUp;

	Vector2? NormalisePosition(Vector2 Position)
	{
		Vector2 uv;
		Camera cam = null;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (GetComponent<RectTransform> (), Position, cam, out uv))
			return null;
		uv.y = 1 - uv.y;
		Debug.Log (uv);
		return uv;
	}

	void OnMousePosition(Vector2 ScreenPosition)
	{
		var uv = NormalisePosition (ScreenPosition);
		if (!uv.HasValue)
			OnMouseUp.Invoke ();
		else
			OnMouseDown.Invoke (uv.Value);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		OnMousePosition(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnMousePosition(eventData.position);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		OnMouseUp.Invoke ();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name + this.name);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnMousePosition(eventData.position);
		//Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name + this.name);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Mouse Enter" + this.name);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Mouse Exit" + this.name);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnMouseUp.Invoke ();
		//Debug.Log("Mouse Up" + this.name);
	}
}

