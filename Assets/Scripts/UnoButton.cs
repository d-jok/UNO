using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class UnoButton : MonoBehaviour
	{
		// Public
		[HideInInspector]
		public bool isPressed;
		[HideInInspector]
		public bool isDone;

		// Private
		private bool m_canPress;

		//-----------------------------------------------------------------------------

		private void Start()
		{
			isPressed = false;
			isDone = false;
		}

		public void ResetObject()
		{
			isPressed = false;
			isDone = false;
			m_canPress = true;
		}

		public IEnumerator ButtonPress()
		{
			yield return new WaitForSeconds(3);
			m_canPress = false;
			isDone = true;
		}

		private void OnMouseUp()
		{
			if (m_canPress)
			{
				isPressed = true;
				m_canPress = false;
				isDone = true;
			}
		}
	}
}
