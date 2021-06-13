using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Color_Panel : MonoBehaviour
	{
		private Color m_color;

		void Start()
		{
			m_color = Color.Black;
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					switch (hit.transform.gameObject.name)
					{
						case ("Red"):
							{
								m_color = Color.Red;
								break;
							}
						case ("Green"):
							{
								m_color = Color.Green;
								break;
							}
						case ("Blue"):
							{
								m_color = Color.Blue;
								break;
							}
						case ("Yellow"):
							{
								m_color = Color.Yellow;
								break;
							}
						default:
							break;
					}
				}
			}
		}

		public IEnumerator SetColor(GameObject card)
		{
			yield return new WaitWhile(() => m_color == Color.Black);

			card.GetComponent<Choose_Color>().ChangeColor(m_color);
			m_color = Color.Black;
		}

		/*public Color GetColor()
		{
			m_color = Color.Black;

			while (m_color == Color.Black)
			{

			}

			return m_color;
		}*/
	}
}
