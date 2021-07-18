using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Choose_Color : MonoBehaviour
	{
		public bool IsColorChanged;

		private bool mIsPlayed;
		public bool mIsInHand;
		private Card mCard;
		private Vector3 mOldPos;

		void Start()
		{
			IsColorChanged = false;

			mIsPlayed = false;
			mIsInHand = false;
			mCard = this.GetComponent<Card>();
			mOldPos = new Vector3(-20f, 0f, -6f);
		}

		public void ChangeColor(Color color)
		{
			IsColorChanged = false;
			string path = "";

			//PROBLEM HERE!!!!

			if (this.GetComponent<Card>().value == Constants.CHANGE_COLOR_PLUS_4_VALUE)
			{
				path = "Materials/Cards/ChangeColor_Plus_4_" + color;
			}
			else
			{
				path = "Materials/Cards/ColorChange_" + color;
			}

			Material change_color_material = Resources.Load<Material>(path);
			this.transform.Find("Front").GetComponent<MeshRenderer>().material = change_color_material;
			mCard.color = color;
			mIsInHand = false;
			mIsPlayed = true;
			IsColorChanged = true;
		}

		private void OnMouseUp()
		{
			if (mIsPlayed == false && mIsInHand == true)
			{
				mIsPlayed = true;
			}
		}
	}
}