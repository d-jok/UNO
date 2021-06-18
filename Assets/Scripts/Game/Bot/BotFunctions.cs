using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class BotFunctions : MonoBehaviour
	{
		public bool IsYourTurn;
		public bool IsPositioningDone;
		public Bot Bot;

		private float mIndent;
		private int OldCardCount;
		private AnimationScript mAnim;
		private GameObject mGameProcess;

		private void Awake()
		{
			IsYourTurn = false;
			IsPositioningDone = false;
			Bot = new Bot();
			mIndent = 1.12f;
			OldCardCount = 0;
			mAnim = new AnimationScript();
			mGameProcess = GameObject.Find("GameController");
		}

		void Update()
		{
			

			//------------------------------------
			int count = Bot.cardsInHand.Count;

			if (OldCardCount != count)
			{
				//if (count % 2 == 0 && count > 2)
				//{
				//	mIndent -= Constants.CARD_INDENT;
				//}

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

				if (count % 2 == 0)
				{
					Vector3 startPos = new Vector3(
						(Bot.spawnPoint.x - 0.56f) - (((count / 2) - 1) * mIndent),
						Bot.spawnPoint.y,
						Bot.spawnPoint.z);

					CardsPositioning(startPos);
				}
				else
				{
					Vector3 startPos = new Vector3(
						Bot.spawnPoint.x - (count / 2 * mIndent),
						Bot.spawnPoint.y,
						Bot.spawnPoint.z);

					CardsPositioning(startPos);
				}

				OldCardCount = count;
			}
		}

		private void CardsPositioning(Vector3 startPos)
		{
			IsPositioningDone = false;

			foreach (var card in Bot.cardsInHand)
			{
				StartCoroutine(mAnim.Move(card, startPos, 1f));
				startPos.x += mIndent;
				startPos.z -= 0.01f;
			}

			IsPositioningDone = true;
		}

		public IEnumerator YourTurn()
		{
			GameObject temp = null;
			GameObject cardInHand = null;
			GameController gameController = mGameProcess.GetComponent<GameController>();

			yield return new WaitForSecondsRealtime(2f);

			Card cardOnField = gameController.GetUpperCardOnField().GetComponent<Card>();

			foreach (var card in Bot.cardsInHand)
			{
				if (card.GetComponent<Card>().color == cardOnField.color)
				{
					Debug.Log("Color"); //Edit code here!
					cardInHand = card;
					Bot.cardsInHand.Remove(cardInHand);
					break;
				}
				else if (card.GetComponent<Card>().value == cardOnField.value)
				{
					Debug.Log("Value");
					cardInHand = card;
					Bot.cardsInHand.Remove(cardInHand);
					break;
				}

				if (card.GetComponent<Card>().value == Constants.CHANGE_COLOR_PLUS_4_VALUE || 
					card.GetComponent<Card>().value == Constants.CHANGE_COLOR_VALUE)
				{
					if (temp == null)
					{
						temp = card;
					}
					else if (temp.GetComponent<Card>().value == Constants.CHANGE_COLOR_PLUS_4_VALUE && 
						card.GetComponent<Card>().value == Constants.CHANGE_COLOR_VALUE)
					{
						temp = card;
					}
					else
					{
						temp = card;
					}
				}
			}

			if (cardInHand == null && temp == null)
			{
				GameObject newCard = gameController.GetCard();
				switch (newCard.GetComponent<Card>().value)
				{
					// Color Change Plus 4.
					case Constants.CHANGE_COLOR_PLUS_4_VALUE:
						{
							newCard.GetComponent<Choose_Color>().mIsInHand = true;
							break;
						}
					// Change Color.
					case Constants.CHANGE_COLOR_VALUE:
						{
							newCard.GetComponent<Choose_Color>().mIsInHand = true;
							break;
						}
					default:
						break;
				}
				StartCoroutine(gameController.AnimationGetCard(newCard, Bot.spawnPoint));
				yield return new WaitWhile(() => gameController.IsGetCardDone == false);
				Bot.cardsInHand.Add(newCard);
			}
			else if (temp != null)
			{
				Choose_Color card = temp.GetComponent<Choose_Color>();
				Bot.cardsInHand.Remove(temp);

				// ADD choose color method!!!

				card.ChangeColor(Color.Red);
				yield return new WaitWhile(() => card.IsColorChanged == false);

				StartCoroutine(gameController.AnimationMoveCardOnField(temp, "Bot"));
				yield return new WaitWhile(() => gameController.IsCardMoveDone == false);
			}
			else
			{
				//StartCoroutine(mAnim.Rotation(cardInHand, new Vector3(0f, 0f, 180f), 0.5f));
				StartCoroutine(gameController.AnimationMoveCardOnField(cardInHand, "Bot"));
				yield return new WaitWhile(() => gameController.IsCardMoveDone == false);
			}

			temp = null;
			IsYourTurn = false;
		}
	}
}
