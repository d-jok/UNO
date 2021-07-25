using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerFunctions : MonoBehaviour
	{
		// Public:
		public bool IsYourTurn;
		public Player mPlayer;

		// Private:
		private bool m_isCardChoosed;
		private int OldCardCount;
		private float mIndent;
		private string m_choosedObjectName;
		private AnimationScript mAnim;
		private GameObject m_GameProcess;
		private GameObject m_Card;
		private GameObject m_colorPanel;
		private GameObject m_UnoButton;
		private GameObject m_Uno_PopUp;

		private void Awake()
		{
			IsYourTurn = false;
			mPlayer = new Player();

			m_isCardChoosed = false;
			OldCardCount = 0;
			mIndent = 1.12f;
			m_choosedObjectName = "";
			mAnim = new AnimationScript();
			m_GameProcess = GameObject.Find("GameController");
			m_Card = null;
			m_colorPanel = GameObject.Find("Color_Choose");
		}

		void Start()
		{
			m_UnoButton = GameObject.Find("Uno_Button(Clone)");
			m_Uno_PopUp = GameObject.Find("Uno_PopUp(Clone)");
		}

		public IEnumerator AddCard(GameObject _card)
		{
			yield return StartCoroutine(AnimationAddingCard(_card));
			mPlayer.cardsInHand.Add(_card);
			yield return StartCoroutine(CardsPositioning());
		}

		private IEnumerator AnimationAddingCard(GameObject _card)
		{
			Vector3 cardPos = _card.transform.position;
			cardPos.z -= 0.5f;
			StartCoroutine(mAnim.Move(_card, cardPos, 0.3f));
			yield return StartCoroutine(mAnim.Rotation(_card, new Vector3(0f, 0f, 180f), 0.3f));
			yield return new WaitWhile(() => mAnim.mIsMoveDone == false);
		}

		public IEnumerator YourTurn()
		{
			IsYourTurn = true;
			GameController gameController = m_GameProcess.GetComponent<GameController>();

			yield return new WaitWhile(() => m_isCardChoosed == false);
			m_isCardChoosed = false;
			string name = "";

			if (m_choosedObjectName.Length == 0)
			{
				name = m_Card.name.Split('(')[0];    // Card name without (Clone)
			}
			else
			{
				name = m_choosedObjectName;
			}

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
						yield return StartCoroutine(colorPanel.SetColor(m_Card));
						Choose_Color cardColor = m_Card.GetComponent<Choose_Color>();
						//cardColor.ChangeColor(color);

						//cardColor.IsColorChanged = false;

						//yield return new WaitWhile(() => cardColor.IsColorChanged == false);
						m_colorPanel.transform.position = oldPos;
						break;
					}
				case "Color_Change":
					{
						Debug.Log("Change");

						Vector3 oldPos = m_colorPanel.transform.position;
						m_colorPanel.transform.position = new Vector3(0f, 0f, -6f);

						//Color color = m_colorPanel.GetComponent<Color_Panel>().GetColor();
						yield return StartCoroutine(m_colorPanel.GetComponent<Color_Panel>().SetColor(m_Card));
						Choose_Color cardColor = m_Card.GetComponent<Choose_Color>();
						//cardColor.ChangeColor(color);
						//cardColor.IsColorChanged = false;

						//yield return new WaitWhile(() => cardColor.IsColorChanged == false);
						m_colorPanel.transform.position = oldPos;
						break;
					}
				case "Deck":
					{
						m_Card = null;
						GameObject card = gameController.GetCard();
						yield return StartCoroutine(AddCard(card));
						break;
					}
				default:
					break;
			}

			if (m_Card != null)
			{
				mPlayer.cardsInHand.Remove(m_Card);

				GameController controller = m_GameProcess.GetComponent<GameController>();
				yield return StartCoroutine(controller.MoveCardOnField(m_Card, "Player"));
			}

			// UNO Button
			if (mPlayer.cardsInHand.Count == 1)
			//if (true)
			{
				Vector3 oldPos = m_UnoButton.transform.position;
				m_UnoButton.transform.position = new Vector3(6f, -4.5f, 0f);
				UnoButton buttonFunctions = m_UnoButton.GetComponent<UnoButton>();

				buttonFunctions.ResetObject();
				StartCoroutine(buttonFunctions.ButtonPress());
				yield return new WaitWhile(() => buttonFunctions.isDone == false);
				m_UnoButton.transform.position = oldPos;

				if (buttonFunctions.isPressed)
				{
					Vector3 Old_Position = m_Uno_PopUp.transform.position;
					m_Uno_PopUp.transform.position = new Vector3(0f, 0f, -0.5f);
					yield return new WaitForSeconds(1);

					m_Uno_PopUp.transform.position = Old_Position;
				}
				else
				{
					const int Card_Count = 3;
					m_Card = null;

					for (int i = 0; i < Card_Count; ++i)
					{
						GameObject card = gameController.GetCard();
						yield return StartCoroutine(AddCard(card));
					}
				}
			}

			if (m_Card != null)
			{
				yield return StartCoroutine(CardsPositioning());
			}		
			m_Card = null;
			m_choosedObjectName = "";
			IsYourTurn = false;
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

						if (choosedObject.name == "Deck")
						{
							m_choosedObjectName = choosedObject.name;
							m_isCardChoosed = true;
						}
						else
						{
							foreach (var card in mPlayer.cardsInHand)
							{
								if (choosedObject == card)
								{
									Card cardValues = choosedObject.GetComponent<Card>();
									GameController controller = m_GameProcess.GetComponent<GameController>();
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

									break;
								}
							}
						}
					}
				}
			}
		}

		private IEnumerator CardsPositioning()
		{
			int count = mPlayer.cardsInHand.Count;

			if (count % 2 == 0 && count > 2)
			{
				if (OldCardCount < count)
				{
					mIndent -= Constants.CARD_INDENT;
				}
				else if (mIndent + Constants.CARD_INDENT <= mIndent)
				{
					mIndent += Constants.CARD_INDENT;
				}
				else
				{
					mIndent = 1.12f;
				}
			}

			Vector3 startPos;

			if (count % 2 == 0)
			{
				startPos = new Vector3(
					(mPlayer.spawnPoint.x - 0.56f) - (((count / 2) - 1) * mIndent),
					mPlayer.spawnPoint.y,
					mPlayer.spawnPoint.z);
			}
			else
			{
				startPos = new Vector3(
					mPlayer.spawnPoint.x - (count / 2 * mIndent),
					mPlayer.spawnPoint.y,
					mPlayer.spawnPoint.z);
			}

			OldCardCount = mPlayer.cardsInHand.Count;



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

			yield return new WaitWhile(() => mAnim.mIsMoveDone == false);
		}
	}
}
