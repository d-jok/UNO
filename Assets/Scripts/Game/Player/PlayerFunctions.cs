using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerFunctions : MonoBehaviour
	{
		public bool IsYourTurn;
		public Player mPlayer;

		private float mIndent;
		private int OldCardCount;
		private AnimationScript mAnim;
		private GameObject mGameProcess;

		private void Awake()
		{
			IsYourTurn = false;
			mPlayer = new Player();
			mIndent = 1.12f;
			OldCardCount = 0;
			mAnim = new AnimationScript();
			mGameProcess = GameObject.Find("GameController");
		}

		void Start()
		{
			//mPlayer = new Player();
			//Debug.Log(mPlayer.playerName);
		}

		public IEnumerator YourTurn()
		{


			/*Destroy(mPlayer.cardsInHand[0]);
			mPlayer.cardsInHand.RemoveAt(0);*/

			yield return null;
			//yield return new WaitForSecondsRealtime(3);
			//IsYourTurn = false;
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
						foreach (var card in mPlayer.cardsInHand)
						{
							if (hit.transform.gameObject == card)
							{
								switch (hit.transform.gameObject.name)
								{
									case "Color_Change_Plus_4":
										{
											Debug.Log("Plus4");

											while(hit.transform.gameObject.GetComponent<Choose_Color>().IsColorChanged == false)
											{

											}
											break;
										}
									case "Color_Change":
										{
											Debug.Log("Change");

											while (hit.transform.gameObject.GetComponent<Choose_Color>().IsColorChanged == false)
											{

											}
											break;
										}
									case "Deck":
										{

											break;
										}
									default:
										break;
								}

								//Debug.Log(card.name + " in hand");
								IsYourTurn = false;
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
