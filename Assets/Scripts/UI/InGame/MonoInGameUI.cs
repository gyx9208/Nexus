using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nexus.UI.InGame
{
	public class MonoInGameUI : MonoBehaviour
	{
		internal const string PATH = "UI/InGame/InGameUI";

		// Use this for initialization
		void Start()
		{
			_MoveButtonCenter = MoveButton.TransformPoint(new Vector3(MoveButton.rect.width * (0.5f - MoveButton.pivot.x), MoveButton.rect.height * (0.5f - MoveButton.pivot.y), 0));
			Debug.Log(_MoveButtonCenter);
		}

		// Update is called once per frame
		void Update()
		{

		}

		#region Move Button
		public RectTransform MoveButton;
		private Vector3 _MoveButtonCenter;


		public void OnMoveButtonBeginDrag(BaseEventData data)
		{
			var p = (PointerEventData)data;
			HandleMoveTouch(p.position);
		}

		public void OnMoveButtonDrag(BaseEventData data)
		{
			var p = (PointerEventData)data;
			HandleMoveTouch(p.position);
		}

		public void OnMoveButtonEndDrag(BaseEventData data)
		{
			var p = (PointerEventData)data;
			HandleMoveTouch(p.position);
		}

		private void HandleMoveTouch(Vector2 position)
		{
			
		}

		#endregion
	}
}