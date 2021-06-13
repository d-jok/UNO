using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Choose_Color : MonoBehaviour
	{
		public bool IsColorChanged;

		//private GameObject ColorPanel;

		private bool mIsPlayed;
		public bool mIsInHand;
		private Card mCard;
		private Vector3 mOldPos;

		void Start()
		{
			IsColorChanged = false;
			//ColorPanel = GameObject.Find("Color_Choose");

			mIsPlayed = false;
			mIsInHand = false;
			mCard = this.GetComponent<Card>();
			mOldPos = new Vector3(-20f, 0f, -6f);
		}

		void Update()
		{
			/*if (Input.GetMouseButtonUp(0) && mIsInHand == true)
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					switch (hit.transform.gameObject.name)
					{
						case ("Red"):
							{
								ChangeColor(Color.Red);
								break;
							}
						case ("Green"):
							{
								ChangeColor(Color.Green);
								break;
							}
						case ("Blue"):
							{
								ChangeColor(Color.Blue);
								break;
							}
						case ("Yellow"):
							{
								ChangeColor(Color.Yellow);
								break;
							}
						default:
							break;
					}
				}
			}*/
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
			//ColorPanel.transform.position = mOldPos;
			mIsInHand = false;
			mIsPlayed = true;
			IsColorChanged = true;
		}

		private void OnMouseUp()
		{
			if (mIsPlayed == false && mIsInHand == true)
			{
				//ColorPanel.transform.position = new Vector3(0f, 0f, -6f);
				mIsPlayed = true;
			}
		}
	}
}