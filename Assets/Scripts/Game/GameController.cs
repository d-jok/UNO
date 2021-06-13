using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		public bool IsGameStarted;
		public bool IsGetCardDone;
		public bool IsCardMoveDone;
		public List<GameObject> Cards;
		public GameObject Bot;
		public GameObject Player;

		private int mPlayerNumber;
		private bool mTurnOrder;    // if true - clockwise; if false - counterclockwise.
		private bool mIsAbilityDone;
		private float mPos_Z;
		private GameObject mDeck;
		private AnimationScript mAnim;
		private List<GameObject> mCardDeck;
		private List<GameObject> mCardsOnField { get; set; }
		private List<string> mNames;
		//private List<PlayerFunctions> mPlayers;
		private List<GameObject> mPlayersList;

		private void Awake()
		{
			IsGameStarted = false;
			IsGetCardDone = false;
			IsCardMoveDone = false;
			mPlayerNumber = 0;
			mTurnOrder = true;
			mIsAbilityDone = false;
			mPos_Z = 0;
			mAnim = new AnimationScript();
			mCardsOnField = new List<GameObject>();
			mPlayersList = new List<GameObject>();

			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
			{
				mNames = new List<string>();

				mNames.Add("Player-1");
				mNames.Add("Bot-1");
				mNames.Add("Bot-2");
				mNames.Add("Bot-3");
			}
			else
			{

			}
		}

		void Start()
		{
			CreateDeck();
			SmashDeck();

			float z = 0;
			mCardDeck = new List<GameObject>();
			mDeck = Instantiate(Player, new Vector3(-5f, 3f, z), Quaternion.identity);
			mDeck.name = "Deck";

			foreach (var card in Cards)
			{
				GameObject obj = Instantiate(card, new Vector3(-5f, 3f, z), Quaternion.Euler(new Vector3(-90f, -180f, 0f)));
				obj.transform.SetParent(mDeck.transform);
				mCardDeck.Add(obj);
				//mCardDeck.Add(Instantiate(card, new Vector3(0f, 0f, z), Quaternion.Euler(new Vector3(-90f, -180f, 0f))));
				z += 0.01f;
			}

			SpawnPlayers();
			StartCoroutine(CardsDistribution());

			//---------------------------------------------
			StartCoroutine(SingleGameProcess());
		}

		void Update()
		{
			// Сhecks if the player still has cards.
			if (IsGameStarted)
			{
				foreach (var player in mPlayersList)	// THERE ERRORS!!!
				{
					if (player.name.Contains("Player"))
					{
						if (player.GetComponent<PlayerFunctions>().mPlayer.cardsInHand.Count == 0)
						{
							Debug.Log(player.name + " WIN!!!"); //EDIT!!!
						}
					}
					else
					{
						if (player.GetComponent<BotFunctions>().Bot.cardsInHand.Count == 0)
						{
							Debug.Log(player.name + " WIN!!!"); //EDIT!!!
						}
					}
				}
			}
		}

		private void GameProcess()
		{

		}

		private IEnumerator SingleGameProcess()
		{
			//int number = 0;
			Debug.Log("In");
			yield return new WaitWhile(() => IsGameStarted == false);

			while (true)
			{
				if (mPlayerNumber == 0)
				{
					PlayerFunctions func = mPlayersList[mPlayerNumber].GetComponent<PlayerFunctions>();
					func.IsYourTurn = true;
					StartCoroutine(func.YourTurn());
					yield return new WaitWhile(() => func.IsYourTurn == true);
					//++mPlayerNumber;
				}
				else
				{
					BotFunctions func = mPlayersList[mPlayerNumber].GetComponent<BotFunctions>();
					func.IsYourTurn = true;
					StartCoroutine(func.YourTurn());
					yield return new WaitWhile(() => func.IsYourTurn == true);
					//++mPlayerNumber;
				}

				if (mTurnOrder == true)
				{
					//++mPlayerNumber;
					NextPlayerNumber();
				}
				else
				{
					PrevPlayerNumber();
				}

				//if (mPlayerNumber == 4)
				//{
				//	mPlayerNumber = 0;
				//}

				Debug.Log("*****************************"); //Delete
			}
		}

		private void NextPlayerNumber()
		{
			++mPlayerNumber;

			if (mPlayerNumber == 4)
			{
				mPlayerNumber = 0;
			}
		}

		private int GetNextPlayerNumber()
		{
			int num = mPlayerNumber;
			return num + 1 == 4 ? mPlayerNumber = 0 : ++num;
		}

		private void PrevPlayerNumber()
		{
			--mPlayerNumber;

			if (mPlayerNumber <= 0)
			{
				mPlayerNumber = 3;
			}
		}

		private int GetPrevPlayerNumber()
		{
			int num = mPlayerNumber;
			return num - 1 <= 0 ? mPlayerNumber = 3 : --num;
		}

		private void NetWorkGameProcess()
		{
			//Code here!!!
		}

		private void AddPlayer()
		{

		}

		private void SpawnPlayers()
		{
			int count = mNames.Count;
			float angleStep = (360 / count) * (Mathf.PI / 180);     // Convert to radians.
			float angle = Constants.START_SPAWN_ANGLE * (Mathf.PI / 180);   // Convert to radians.

			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
			{
				int botNumber = 1;
				int playerNumber = 1;

				for (int i = 0; i < count; ++i)
				{
					float x = Constants.RADIUS * Mathf.Cos(angle);
					float y = Constants.RADIUS * Mathf.Sin(angle);

					// Create Player or Bot.
					if (mNames[i].Contains("Player"))
					{
						mPlayersList.Add(Instantiate(Player, new Vector3(x, y, 0f), Quaternion.identity, Player.transform.parent));
						mPlayersList[i].name = "Player-" + playerNumber;
						mPlayersList[i].GetComponent<PlayerFunctions>().mPlayer.playerName = mNames[i];
						mPlayersList[i].GetComponent<PlayerFunctions>().mPlayer.spawnPoint = new Vector3(x, y, 0f);
						++playerNumber;
					}
					else
					{
						mPlayersList.Add(Instantiate(Bot, new Vector3(x, y, 0f), Quaternion.identity, Player.transform.parent));
						mPlayersList[i].name = "Bot-" + botNumber;
						mPlayersList[i].GetComponent<BotFunctions>().Bot.playerName = mNames[i];
						mPlayersList[i].GetComponent<BotFunctions>().Bot.spawnPoint = new Vector3(x, y, 0f);
						++botNumber;
					}

					angle += angleStep;
				}
			}
			else
			{
				// Code here!!!
			}
		}

		private void CreateDeck()
		{
			List<GameObject> list = new List<GameObject>();

			foreach (var card in Cards)
			{
				list.Add(card);

				if (!card.name.Contains("0") && !card.name.Contains("Color_Change"))
				{
					list.Add(card);
				}
				else if (card.name.Contains("Color_Change"))
				{
					list.Add(card);
					list.Add(card);
					list.Add(card);
				}
			}

			Cards = list;
		}

		private void SmashDeck()
		{
			int size = Cards.Count;

			for (int i = 0; i < size; i++)
			{
				GameObject temp = Cards[i];
				Cards.RemoveAt(i);
				Cards.Insert(Random.Range(0, size), temp);
			}
		}

		public GameObject GetUpperCardOnField()
		{
			int size = mCardsOnField.Count;
			Debug.Log(mCardsOnField[size - 1].name);
			return mCardsOnField[size - 1];
		}

		public GameObject GetCard()
		{
			GameObject card = mCardDeck[0];
			mCardDeck.RemoveAt(0);
			return card;
		}

		private IEnumerator CardsDistribution()
		{
			//float d = 0f;
			//float angle = 30;
			//angle = angle * (Mathf.PI / 180);

			GameObject card;

			for (int i = 0; i < Constants.CARD_COUNT_IN_DISTRIBUTION; ++i)
			{
				for (int j = 0; j < mPlayersList.Count; ++j)
				{
					card = GetCard();

					if (j == 0)
					{
						StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
					}

					if (mPlayersList[j].name.Contains("Player"))
					{
						Player player = mPlayersList[j].GetComponent<PlayerFunctions>().mPlayer;
						StartCoroutine(mAnim.Move(card, player.spawnPoint, 1f));
						yield return new WaitWhile(() => mAnim.mIsMoveDone == false);
						player.cardsInHand.Add(card);
					}
					else
					{
						Bot bot = mPlayersList[j].GetComponent<BotFunctions>().Bot;
						StartCoroutine(mAnim.Move(card, bot.spawnPoint, 1f));
						yield return new WaitWhile(() => mAnim.mIsMoveDone == false);
						bot.cardsInHand.Add(card);
					}
				}


				

				/*for (int j = 0; j < mPlayers.Count; ++j)
				{
					card = GetCard();

					if (j == 0)
					{
						StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
					}

					//StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, -30f), 0.3f));
					/*Vector3 v = new Vector3(mPlayers[j].spawnPoint.x + d * Mathf.Cos(angle),
						mPlayers[j].spawnPoint.y + d * Mathf.Sin(angle),
						mPlayers[j].spawnPoint.z + d * Mathf.Sin(angle));

					d += 0.2f;

					StartCoroutine(mAnim.Move(card, mPlayers[j].spawnPoint, 1f));
					yield return new WaitWhile(() => mAnim.mIsMoveDone == false);

					mPlayers[j].cardsInHand.Add(card);
				}*/
			}

			card = GetCard();
			StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
			StartCoroutine(mAnim.Move(card, new Vector3(0, 0, 0), 1f));
			yield return new WaitWhile(() => mAnim.mIsMoveDone == false);

			mCardsOnField.Add(card);
			IsGameStarted = true;

			yield return null;
		}

		public IEnumerator AnimationGetCard(GameObject card, Vector3 position)	// CHANGE !!!!
		{
			IsGetCardDone = false;
			//StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));

			StartCoroutine(mAnim.Move(card, position, 1f));
			yield return new WaitWhile(() => mAnim.mIsMoveDone == false);

			IsGetCardDone = true;

			//yield return null;
		}

		public IEnumerator AnimationMoveCardOnField(GameObject card, string playerType)	//EDIT!!!
		{
			IsCardMoveDone = false;

			int number = Random.Range(0, 1);
			int angle = Random.Range(-10, 10);

			switch (number)
			{
				case 0:
					{
						if (playerType == "Bot")
						{
							card.transform.Rotate(0f, 0f, 180f, Space.Self);
						}
						StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 180f + angle, 0f), 0.3f));
						StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z -= 0.1f), 0.5f));
						break;
					}
				case 1:	//EDIT!!
					{
						StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.2f));
						StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z -= 0.1f), 1f));
						break;
					}
				default:
					break;
			}

			mCardsOnField.Add(card);
			StartCoroutine(CardsAbilities());   // HERE????
			yield return new WaitWhile(() => mIsAbilityDone == false);
			IsCardMoveDone = true;

			yield return null;
		}

		private IEnumerator CardsAbilities()
		{
			mIsAbilityDone = false;

			switch (mCardsOnField[mCardsOnField.Count - 1].GetComponent<Card>().value)
			{
				case Constants.SKIP_TURN_VALUE:
					{
						NextPlayerNumber();
						break;
					}
				case Constants.PLUS_2_VALUE:
					{
						if (GetNextPlayerNumber() != 0)
						{
							BotFunctions bFunc = mPlayersList[mPlayerNumber + 1].GetComponent<BotFunctions>();
							for (int i = 0; i < 2; ++i)
							{
								GameObject card = GetCard();
								StartCoroutine(AnimationGetCard(card, bFunc.Bot.spawnPoint));
								yield return new WaitWhile(() => IsGetCardDone == false);

								bFunc.Bot.cardsInHand.Add(card);    //Maybe smth wrong.
								yield return new WaitWhile(() => bFunc.IsPositioningDone == false);
							}
						}
						else
						{
							PlayerFunctions pFunc = mPlayersList[mPlayerNumber].GetComponent<PlayerFunctions>();
							for (int i = 0; i < 2; ++i)
							{
								GameObject card = GetCard();
								StartCoroutine(AnimationGetCard(card, pFunc.mPlayer.spawnPoint));

								StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
								yield return new WaitWhile(() => mAnim.mIsRotateDone == false);

								pFunc.mPlayer.cardsInHand.Add(card);
								// Need to add WaitWhile.
							}
						}
						break;
					}
				case Constants.CHANGE_TURN_ORDER_VALUE:
					{
						if (mTurnOrder == true)
						{
							mTurnOrder = false;
						}
						else
						{
							mTurnOrder = true;
						}
						break;
					}
				case Constants.CHANGE_COLOR_PLUS_4_VALUE:
					{
						if (GetNextPlayerNumber() != 0)
						{
							BotFunctions bFunc = mPlayersList[mPlayerNumber + 1].GetComponent<BotFunctions>();
							for (int i = 0; i < 4; ++i)
							{
								GameObject card = GetCard();
								StartCoroutine(AnimationGetCard(card, bFunc.Bot.spawnPoint));
								yield return new WaitWhile(() => IsGetCardDone == false);

								bFunc.Bot.cardsInHand.Add(card);    //Maybe smth wrong.
								yield return new WaitWhile(() => bFunc.IsPositioningDone == false);
							}
							NextPlayerNumber();
						}
						else
						{
							PlayerFunctions pFunc = mPlayersList[mPlayerNumber].GetComponent<PlayerFunctions>();
							for (int i = 0; i < 4; ++i)
							{
								GameObject card = GetCard();
								StartCoroutine(AnimationGetCard(card, pFunc.mPlayer.spawnPoint));

								StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
								yield return new WaitWhile(() => mAnim.mIsRotateDone == false);

								pFunc.mPlayer.cardsInHand.Add(card);
								// Need to add WaitWhile.
							}
							NextPlayerNumber();
						}
						break;
					}
				default:
					break;
			}

			mIsAbilityDone = true;
		}
	}
}
