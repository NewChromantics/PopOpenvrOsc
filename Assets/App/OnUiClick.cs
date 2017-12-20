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



	void OnMousePosition(PointerEventData eventData)
	{
		var ScreenPosition = eventData.position;
		Vector2 uv;
		var recttransform = GetComponent<RectTransform> ();
		Camera cam = eventData.pressEventCamera;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (recttransform, ScreenPosition, cam, out uv)) {
			OnMouseUp.Invoke ();
			return;
		}
		uv.y = 1 - uv.y;
		Debug.Log (uv);

		OnMouseDown.Invoke (uv);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		OnMousePosition(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnMousePosition(eventData);
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
		OnMousePosition(eventData);
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

