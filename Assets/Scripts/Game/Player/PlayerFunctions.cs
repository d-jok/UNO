using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerFunctions : MonoBehaviour
	{
		public bool IsYourTurn;
		public Player mPlayer;

		private bool m_isCardChoosed;
		private int OldCardCount;
		private float mIndent;
		private AnimationScript mAnim;
		private GameObject mGameProcess;
		private GameObject m_Card;
		private GameObject m_colorPanel;

		private void Awake()
		{
			IsYourTurn = false;
			mPlayer = new Player();


			m_isCardChoosed = false;
			OldCardCount = 0;
			mIndent = 1.12f;
			mAnim = new AnimationScript();
			mGameProcess = GameObject.Find("GameController");
			m_colorPanel = GameObject.Find("Color_Choose");
		}

		void Start()
		{
			//mPlayer = new Player();
			//Debug.Log(mPlayer.playerName);
		}

		public IEnumerator YourTurn()
		{
			yield return new WaitWhile(() => m_isCardChoosed == false);
			m_isCardChoosed = false;

			Debug.Log(m_Card.name);

			string name = m_Card.name.Split('(')[0];	// Card name without (Clone)

			switch (name)
			{
				case "Color_Change_Plus_4":
					{
						Debug.Log("Plus4");
						//GameObject ColorPanel;
						Vector3 oldPos = m_colorPanel.transform.position;
						m_colorPanel.transform.position = new Vector3(0f, 0f, -6f);

						Color_Panel colorPanel = m_colorPanel.GetComponent<Color_Panel>();
						Debug.Log(colorPanel.name);
						StartCoroutine(colorPanel.SetColor(m_Card));
						Choose_Color cardColor = m_Card.GetComponent<Choose_Color>();
						//cardColor.ChangeColor(color);

						//cardColor.IsColorChanged = false;

						yield return new WaitWhile(() => cardColor.IsColorChanged == false);
						m_colorPanel.transform.position = oldPos;
						break;
					}
				case "Color_Change":
					{
						Debug.Log("Change");

						Vector3 oldPos = m_colorPanel.transform.position;
						m_colorPanel.transform.position = new Vector3(0f, 0f, -6f);

						//Color color = m_colorPanel.GetComponent<Color_Panel>().GetColor();
						StartCoroutine(m_colorPanel.GetComponent<Color_Panel>().SetColor(m_Card));
						Choose_Color cardColor = m_Card.GetComponent<Choose_Color>();
						//cardColor.ChangeColor(color);
						//cardColor.IsColorChanged = false;

						yield return new WaitWhile(() => cardColor.IsColorChanged == false);
						m_colorPanel.transform.position = oldPos;
						break;
					}
				/*case "Deck":
					{

						break;
					}*/
				default:
					break;
			}

			mPlayer.cardsInHand.Remove(m_Card);

			GameController controller = mGameProcess.GetComponent<GameController>();
			StartCoroutine(controller.AnimationMoveCardOnField(m_Card, "Player"));
			yield return new WaitWhile(() => controller.IsCardMoveDone == false);

			Debug.Log("IS YOUR TURN");
			IsYourTurn = false;
			m_Card = null;
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0) && IsYourTurn)
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					if (mAnim.mIsMoveDone == true)
					{
						GameObject choosedObject = hit.transform.gameObject;

						foreach (var card in mPlayer.cardsInHand)
						{
							if (choosedObject == card)
							{
								Card cardValues = choosedObject.GetComponent<Card>();
								GameController controller = mGameProcess.GetComponent<GameController>();
								Card upperCardValues = controller.GetUpperCardOnField().GetComponent<Card>();

								if (cardValues.color == upperCardValues.color || cardValues.value == upperCardValues.value)
								{
									m_Card = hit.transform.gameObject;
									m_isCardChoosed = true;
								}
								else if (cardValues.color == Color.Black)
								{
									m_Card = hit.transform.gameObject;
									m_isCardChoosed = true;
								}
							}
						}

						/*switch (hit.transform.gameObject.name)
						{
							case "Deck":
								{
									//StartCoroutine(AnimationGetCard());
									IsYourTurn = false;
									break;
								}
							default:
								break;
						}*/
					}
				}
			}

			/*if (mGameProcess.GetComponent<GameController>().IsGameStarted && IsYourTurn)
			{
				var upperCard = mGameProcess.GetComponent<GameController>().GetUpperCardOnField();
				if (upperCard)//EDIT!!!
				{
					Debug.Log(upperCard.name);
				}
			}*/

			//------------------------------------
			int count = mPlayer.cardsInHand.Count;

			if (OldCardCount != count)
			{
				if (count % 2 == 0 && count > 2)
				{
					mIndent -= Constants.CARD_INDENT;
				}

				if (count % 2 == 0)
				{
					Vector3 startPos = new Vector3(
						(mPlayer.spawnPoint.x - 0.56f) - (((count / 2) - 1) * mIndent),
						mPlayer.spawnPoint.y,
						mPlayer.spawnPoint.z);

					CardsPositioning(startPos);
				}
				else
				{
					Vector3 startPos = new Vector3(
						mPlayer.spawnPoint.x - (count / 2 * mIndent),
						mPlayer.spawnPoint.y,
						mPlayer.spawnPoint.z);

					CardsPositioning(startPos);
				}

				OldCardCount = mPlayer.cardsInHand.Count;
			}
		}

		private void CardsPositioning(Vector3 startPos)
		{
			foreach (var card in mPlayer.cardsInHand)
			{
				if (card.name.Contains("Color_Change_Plus_4") || card.name.Contains("Color_Change"))  // Temporarily
				{
					card.GetComponent<Choose_Color>().mIsInHand = true;
					Debug.Log("In-Hand");
				}

				StartCoroutine(mAnim.Move(card, startPos, 1f));
				startPos.x += mIndent;
				startPos.z -= 0.01f;
			}
		}
	}
}
