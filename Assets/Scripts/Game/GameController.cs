using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		// Public:
		public struct PlayerForLan
		{
			public int m_Number;
			public string m_Name;
		}

		public bool IsGameStarted;
		public bool IsGetCardDone;
		public bool IsCardMoveDone;
		public bool IsPlayerTurn_Lan;
		public bool IsPlayerTurnDone_Lan;
		public List<GameObject> Cards;
		public GameObject Bot;
		public GameObject Player;
		public GameObject m_TurnOrderArrows;
		public List<PlayerForLan> players_lan;
		public List<PlayerForLan> players_lan_updated;

		// Private:
		private int mPlayerNumber;
		private bool m_TurnOrder;    // if true - clockwise; if false - counterclockwise.
		//private bool mIsAbilityDone;
		private bool m_is_DeckSync;
		[HideInInspector]
		public  bool m_is_PlayersSync;
		private float mPos_Z;
		private GameObject mDeck;
		private AnimationScript mAnim;
		private List<GameObject> mCardDeck;
		private List<GameObject> m_CardsOnField { get; set; }
		private List<string> mNames;
		//private List<PlayerFunctions> mPlayers;
		private List<GameObject> mPlayersList;
		private List<GameObject> m_Discard;

		private GameObject m_ServerObj;
		private NetworkServer.Server m_Server;
		private NetworkServer.ServerFunctions m_serverFunctions;

		private GameObject m_ClientObj;
		private NetworkClient.Client m_Client;
		private GameObject m_CurrentPlayer;
		private PlayerFunctions m_CurrentPlayerFunctions;

//-----------------------------------------------------------------------------

		public List<GameObject> getPlayersList()
		{
			return mPlayersList;
		}

		public bool getTurnOrder()
		{
			return m_TurnOrder;
		}

		private void Awake()
		{
			SpawnGameObject(Constants.UNO_BUTTON_PATH, new Vector3(-15f, 0f, 0f));
			SpawnGameObject(Constants.UNO_POP_UP_PATH, new Vector3(-15f, 0f, 1f));

			IsGameStarted = false;
			IsGetCardDone = false;
			IsCardMoveDone = false;
			IsPlayerTurnDone_Lan = false;
			mPlayerNumber = 0;
			m_TurnOrder = true;
			m_is_DeckSync = false;
			m_is_PlayersSync = false;
			mPos_Z = 0;
			mAnim = new AnimationScript();
			m_CardsOnField = new List<GameObject>();
			mPlayersList = new List<GameObject>();
			m_Discard = new List<GameObject>();
			players_lan = new List<PlayerForLan>();

			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
			{
				Debug.Log("Game type: AI");
				mNames = new List<string>();

				mNames.Add("Player-1");
				mNames.Add("Bot-1");
				mNames.Add("Bot-2");
				mNames.Add("Bot-3");
			}
			else if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.LOCAL_NETWORK)
			{
				Debug.Log("Game type: LOCAL-NETWORK");

				if (PlayerPrefs.GetString("PlayerRole") == MainMenu.Constants.SERVER)
				{
					m_ServerObj = GameObject.Find("Server");
					m_Server = m_ServerObj.GetComponent<NetworkServer.Server>();
					m_serverFunctions = new NetworkServer.ServerFunctions();

					CreateDeck();
					SmashDeck();
					CreateDeckGameObject();

					string command = "#StartGame";
					m_serverFunctions.SendToAll(command);
				}
				else
				{
					m_ClientObj = GameObject.Find("Client");
					m_Client = m_ClientObj.GetComponent<NetworkClient.Client>();
					m_Client.OnLoadGameScene();
					// Code here!!
				}
			}
		}

		void Start()
		{
			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
			{
				CreateDeck();
				SmashDeck();
				CreateDeckGameObject();
				SpawnPlayers();
				StartCoroutine(CardsDistribution());
				StartCoroutine(SingleGameProcess());
			}
			else
			{
				if (PlayerPrefs.GetString("PlayerRole") == MainMenu.Constants.SERVER)
				{
					int playersCount = m_serverFunctions.GetClientsCount();

					while (playersCount > 0)
					{
						foreach (var player in NetworkServer.ServerFunctions.Clients)
						{
							if (player.isLoaded)
							{
								player.Send("#SetNumber " + player.Number);	// Send player Number.
								--playersCount;
							}
						}
					}



					StartCoroutine(ServerGameProcess());
				}
				else
				{
					mNames = new List<string>();

					foreach (var player in players_lan)
					{
						mNames.Add(player.m_Name);
					}


					StartCoroutine(ClientGameProcess());
				}
			}
		}

		void Update()
		{
			// Сhecks if the player still has cards.
			//if (IsGameStarted)
			//{
			//	foreach (var player in mPlayersList)	// THERE ERRORS!!!
			//	{
			//		if (player.name.Contains("Player"))
			//		{
			//			if (player.GetComponent<PlayerFunctions>().mPlayer.cardsInHand.Count == 0)
			//			{
			//				Debug.Log(player.name + " WIN!!!"); //EDIT!!!
			//			}
			//		}
			//		else
			//		{
			//			if (player.GetComponent<BotFunctions>().Bot.cardsInHand.Count == 0)
			//			{
			//				Debug.Log(player.name + " WIN!!!"); //EDIT!!!
			//			}
			//		}
			//	}
			//}
		}

		private void SpawnGameObject(string _path, Vector3 _position)
		{
			GameObject gameObject = Resources.Load(_path) as GameObject;
			Instantiate(gameObject, _position, Quaternion.identity);
		}

		private IEnumerator ServerGameProcess()
		{
			m_Server.SendDeck(Cards);
			yield return new WaitForSeconds(4);
			Debug.Log("SYNC");

			string playersInfo = "#PlayersInfo ";
			mNames = new List<string>();
			mNames.Add(m_Server.GetServerName());
			int number = 0;
			players_lan.Add(new PlayerForLan() {
				m_Name = m_Server.GetServerName(),
				m_Number = number
			});
			playersInfo += m_Server.GetServerName() + "&" + number + " ";
			number = 1;
			foreach (var player in NetworkServer.ServerFunctions.Clients)
			{
				//player.Number = number;
				players_lan.Add(new PlayerForLan() {
					m_Name = player.Name,
					m_Number = player.Number
				});

				mNames.Add(player.Name);
				playersInfo += player.Name + "&" + player.Number + " ";
				++number;
			}

			SpawnPlayers();
			m_serverFunctions.SendToAll(playersInfo);

			// Current player initialization.
			m_CurrentPlayer = GameObject.Find(m_Server.GetServerName());
			m_CurrentPlayerFunctions = m_CurrentPlayer.GetComponent<PlayerFunctions>();
			m_CurrentPlayerFunctions.SetServer_Lan(m_serverFunctions);

			yield return StartCoroutine(CardsDistribution());

			int playersCount = m_serverFunctions.GetClientsCount();
			while (playersCount > 0)
			{
				foreach (var player in NetworkServer.ServerFunctions.Clients)
				{
					if (player.isCardsDisributed)
					{
						--playersCount;
					}
				}
			}

			//GameObject card = GetCard();
			//StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
			m_TurnOrderArrows.SetActive(true);

			while (true)
			{
				Debug.Log(mPlayerNumber);

				if (mCardDeck.Count == 0)
				{
					yield return StartCoroutine(RestoreDeck());
					//CODE HERE
				}

				NetworkServer.ServerClients CurrentPlayer = new NetworkServer.ServerClients();

				if (mPlayerNumber != 0)
				{
					// PLAYER TURN SEND. CODE
					foreach (var player in NetworkServer.ServerFunctions.Clients)
					{
						if (player.Number == mPlayerNumber)
						{
							CurrentPlayer = player;
							player.Send("#YourTurn");
							break;
						}
					}

					yield return new WaitWhile(() => CurrentPlayer.isTurnDone == false);
					CurrentPlayer.isTurnDone = false;

					Debug.Log(CurrentPlayer.Action); // DELETE.

					switch (CurrentPlayer.Action)
					{
						case "Card":
							{
								string request = "#Card ";
								GameObject player = mPlayersList[mPlayerNumber];
								PlayerFunctions func = player.GetComponent<PlayerFunctions>();
								GameObject selectedCard = new GameObject();
								int Counter = 0;
								foreach (var card in func.mPlayer.cardsInHand)
								{
									string name = card.name.Split('(')[0];
									if (name == CurrentPlayer.cardName)
									{
										selectedCard = card;
										func.mPlayer.cardsInHand.RemoveAt(Counter);
										request += CurrentPlayer.cardName + " ";
										CurrentPlayer.cardName = "";
										break;
									}
									++Counter;
								}

								if (CurrentPlayer.cardColor != "")
								{
									Choose_Color changeColor = selectedCard.GetComponent<Choose_Color>();
									Color cardColor;
									System.Enum.TryParse(CurrentPlayer.cardColor, out cardColor);
									changeColor.ChangeColor(cardColor);
									request += CurrentPlayer.cardColor;
									CurrentPlayer.cardColor = "";
								}

								yield return StartCoroutine(func.CardsPositioning());
								yield return StartCoroutine(MoveCardOnField(selectedCard, "Player")); // Change on Bot. 
																									// Becase animation must rotate the card.

								//SEND TO ALL EXCEPT the current player.
								foreach (var playerInList in NetworkServer.ServerFunctions.Clients)
								{
									if (playerInList.Number != CurrentPlayer.Number)
									{
										playerInList.Send(request + " " + CurrentPlayer.Number);
									}
								}

								break;
							}
						case "Deck":
							{
								//SEND TO ALL EXCEPT the current player.
								foreach (var playerInList in NetworkServer.ServerFunctions.Clients)
								{
									if (playerInList.Number != CurrentPlayer.Number)
									{
										playerInList.Send("#Deck " + CurrentPlayer.Number);
									}
								}

								GameObject card = GetCard();
								GameObject player = mPlayersList[mPlayerNumber];
								PlayerFunctions func = player.GetComponent<PlayerFunctions>();
								yield return StartCoroutine(func.AddCard(card));
								break;
							}
						default:
							break;
					}

					CurrentPlayer.Action = "";
				}
				else if (mPlayerNumber == 0)
				{
					yield return StartCoroutine(m_CurrentPlayerFunctions.YourTurn());
				}

				if (m_TurnOrder == true)
				{
					NextPlayerNumber();
				}
				else
				{
					PrevPlayerNumber();
				}

				Debug.Log("*****************************"); //Delete
			}
		}

		private IEnumerator ClientGameProcess()
		{
			yield return new WaitWhile(() => m_is_DeckSync == false);
			Debug.Log("SYNC");
			yield return new WaitWhile(() => m_is_PlayersSync == false);
			Debug.Log("PLayers SYNC");

			// Current player initialization.
			m_CurrentPlayer = GameObject.Find(m_Client.Name);
			m_CurrentPlayerFunctions = m_CurrentPlayer.GetComponent<PlayerFunctions>();
			m_CurrentPlayerFunctions.SetClient_Lan(m_Client);	// Set client in player functions.

			CreateDeckGameObject();
			//GameObject card = GetCard();
			//StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
			//SpawnPlayers();
			yield return StartCoroutine(CardsDistribution());
			m_Client.send("#DistributionIsDone");
			m_TurnOrderArrows.SetActive(true);

			while (true)
			{
				Debug.Log(mPlayerNumber);
				yield return new WaitWhile(() => IsPlayerTurn_Lan == false);
				yield return StartCoroutine(m_CurrentPlayerFunctions.YourTurn());
				IsPlayerTurn_Lan = false;
				m_Client.send("#TurnIsDone");

				if (m_TurnOrder == true)
				{
					NextPlayerNumber();
				}
				else
				{
					PrevPlayerNumber();
				}
			}
		}

		private IEnumerator SingleGameProcess()
		{
			yield return new WaitWhile(() => IsGameStarted == false);
			m_TurnOrderArrows.SetActive(true);

			while (true)
			{
				if (mCardDeck.Count == 0)
				{
					yield return StartCoroutine(RestoreDeck());
				}

				if (mPlayerNumber == 0)
				{
					PlayerFunctions func = mPlayersList[mPlayerNumber].GetComponent<PlayerFunctions>();
					yield return StartCoroutine(func.YourTurn());
				}
				else
				{
					BotFunctions func = mPlayersList[mPlayerNumber].GetComponent<BotFunctions>();
					yield return StartCoroutine(func.YourTurn());
				}

				if (m_TurnOrder == true)
				{
					NextPlayerNumber();
				}
				else
				{
					PrevPlayerNumber();
				}

				Debug.Log("*****************************"); //Delete
			}
		}

		public void NextPlayerNumber()
		{
			++mPlayerNumber;

			if (mNames.Count - 1 > 0 && mPlayerNumber >= mNames.Count)
			{
				mPlayerNumber = 0;
			}
			else if (mPlayerNumber >= players_lan_updated.Count)
			{
				mPlayerNumber = 0;
			}
		}

		public int GetNextPlayerNumber()
		{
			int number = mPlayerNumber;
			int playerCount = 0;
			
			if (mNames.Count - 1 > 0)
			{
				playerCount = mNames.Count;
			}
			else
			{
				playerCount = players_lan_updated.Count;
			}

			if (number + 1 >= playerCount)
			{
				return 0;
			}
			else
			{
				return number + 1;
			}
		}

		public void PrevPlayerNumber()
		{
			--mPlayerNumber;

			if (mPlayerNumber < 0)
			{
				if (mNames.Count - 1 > 0)
				{
					mPlayerNumber = mNames.Count - 1;
				}
				else
				{
					mPlayerNumber = players_lan_updated.Count - 1;
				}
			}
		}

		public int GetPrevPlayerNumber()
		{
			int number = mPlayerNumber;
			int playerNumber = 0;

			if (mNames.Count - 1 > 0)
			{
				playerNumber = mNames.Count;
			}
			else
			{
				playerNumber = players_lan_updated.Count;
			}

			if (number - 1 < 0)
			{
				return playerNumber - 1;
			}
			else
			{
				return number - 1;
			}
		}

		public int GetCurrentPlayerNumber()
		{
			return mPlayerNumber;
		}

		private void AddPlayer()
		{

		}

		public void SpawnPlayers()
		{
			int count = mNames.Count;

			if (count == 0)
			{
				count = players_lan.Count;
			}

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
				PlayerForLan playerForLanTemp = new PlayerForLan();
				List<PlayerForLan> list = new List<PlayerForLan>();

				foreach (var item in players_lan)
				{
					if (PlayerPrefs.GetString("PlayerRole") == MainMenu.Constants.SERVER)
					{
						if (item.m_Number != 0)
						{
							list.Add(item);
						}
						else
						{
							playerForLanTemp = item;
						}
					}
					else
					{
						if (item.m_Number != m_Client.Number)
						{
							list.Add(item);
						}
						else
						{
							playerForLanTemp = item;
						}
					}
				}

				//players_lan.Clear();
				//players_lan.Add(playerForLanTemp);

				//foreach (var item in list)
				//{
				//	players_lan.Add(item);
				//}

				List<PlayerForLan> SpawnList = new List<PlayerForLan>();
				SpawnList.Add(playerForLanTemp);
				foreach (var item in list)
				{
					SpawnList.Add(item);
				}

				// PROBLEM with player_lan!
				for (int i = 0; i < SpawnList.Count; ++i)
				{
					float x = Constants.RADIUS * Mathf.Cos(angle);
					float y = Constants.RADIUS * Mathf.Sin(angle);

					mPlayersList.Add(Instantiate(Player, new Vector3(x, y, 0f), Quaternion.identity, Player.transform.parent));
					mPlayersList[i].name = SpawnList[i].m_Name;
					mPlayersList[i].GetComponent<PlayerFunctions>().mPlayer.playerName = SpawnList[i].m_Name;
					mPlayersList[i].GetComponent<PlayerFunctions>().mPlayer.playerNumber = SpawnList[i].m_Number;
					mPlayersList[i].GetComponent<PlayerFunctions>().mPlayer.spawnPoint = new Vector3(x, y, 0f);

					angle += angleStep;
				}

				players_lan_updated = SpawnList;

				for (int i = 0; i < SpawnList.Count; ++i)
				{
					if (SpawnList[i].m_Number == 0)
					{
						mPlayerNumber = i;
						break;
					}
				}

				m_is_PlayersSync = true;
			}
		}

		public void DeckSync(List<string> _cardsNames)
		{
			List<GameObject> list = new List<GameObject>();

			foreach (var name in _cardsNames)
			{
				foreach (var find in Cards)
				{
					if (name == find.name)
					{
						list.Add(find);
					}
				}
			}

			Cards = list;

			Debug.Log("CLIENT - DECK IS SYNC");
			m_Client.send("#DeckIsSync");
			m_is_DeckSync = true;
		}

		private void CreateDeckGameObject()
		{
			float z = 0;
			mCardDeck = new List<GameObject>();
			mDeck = Instantiate(Player, new Vector3(-5f, 3f, z), Quaternion.identity);
			mDeck.name = "Deck";
			mDeck.GetComponent<BoxCollider>().enabled = true;
			mDeck.GetComponent<PlayerFunctions>().enabled = false;

			foreach (var card in Cards)
			{
				GameObject obj = Instantiate(card, new Vector3(-5f, 3f, z), Quaternion.Euler(new Vector3(-90f, -180f, 0f)));
				obj.transform.SetParent(mDeck.transform);
				mCardDeck.Add(obj);
				z += 0.01f;
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
			int size = m_CardsOnField.Count;
			Debug.Log(m_CardsOnField[size - 1].name);
			return m_CardsOnField[size - 1];
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

			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
			{
				for (int i = 0; i < Constants.CARD_COUNT_IN_DISTRIBUTION; ++i)
				{
					for (int j = 0; j < mPlayersList.Count; ++j)
					{
						card = GetCard();

						//if (j == 0)
						//{
						//	StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
						//}

						if (mPlayersList[j].name.Contains("Player"))
						{
							PlayerFunctions playerFunc = mPlayersList[j].GetComponent<PlayerFunctions>();
							yield return StartCoroutine(playerFunc.AddCard(card));
						}
						else
						{
							BotFunctions botFunc = mPlayersList[j].GetComponent<BotFunctions>();
							Vector3 spawnPoint = botFunc.Bot.spawnPoint;
							StartCoroutine(mAnim.Move(card, spawnPoint, 1f));
							yield return new WaitWhile(() => mAnim.mIsMoveDone == false);
							yield return StartCoroutine(botFunc.AddCard(card));
						}
					}
				}
			}
			else
			{
				int cardCount = Constants.CARD_COUNT_IN_DISTRIBUTION * mPlayersList.Count;

				for (int i = 0, k = 0; i < cardCount; ++i)
				{
					card = GetCard();

					for (int j = 0; j < mPlayersList.Count; ++j)
					{
						PlayerFunctions playerFunc = mPlayersList[j].GetComponent<PlayerFunctions>();

						if (playerFunc.mPlayer.playerNumber == players_lan[k].m_Number)
						{
							yield return StartCoroutine(playerFunc.AddCard(card));
							++k;

							if (k == players_lan.Count)
							{
								k = 0;
							}

							break;
						}
					}
				}
			}

			card = GetCard();
			StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.3f));
			StartCoroutine(mAnim.Move(card, new Vector3(0, 0, 0), 1f));
			yield return new WaitWhile(() => mAnim.mIsMoveDone == false);

			if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.LOCAL_NETWORK &&
				PlayerPrefs.GetString("PlayerRole") == MainMenu.Constants.CLIENT)
			{
				m_Client.send("#CardsWasDistributed");
			}

			m_CardsOnField.Add(card);
			IsGameStarted = true;

			yield return null;
		}

		private IEnumerator RestoreDeck()
		{
			foreach (var card in m_Discard)
			{
				yield return StartCoroutine(AnimationMoveCardInDeck(card));
				mCardDeck.Add(card);
				m_Discard.Remove(card);
			}
		}

		private IEnumerator CardsOnFieldPositioning()
		{
			int count = m_CardsOnField.Count;

			if (count >= 3)
			{
				float posZ = mPos_Z;

				for (int i = m_CardsOnField.Count - 1; i >= 0; --i)
				{
					StartCoroutine(mAnim.Move(m_CardsOnField[i], new Vector3(0f, 0f, posZ += 0.1f), 1f));
				}
			}
			else
			{
				mPos_Z -= 0.1f;
			}

			yield return null;
		}

		public IEnumerator MoveCardOnField(GameObject card, string playerType)
		{
			IsCardMoveDone = false;

			yield return StartCoroutine(CardsOnFieldPositioning());
			yield return StartCoroutine(AnimationMoveCardOnField(card, playerType));
			m_CardsOnField.Add(card);
			int count = m_CardsOnField.Count;

			if (count > 3)
			{
				Vector3 discard_pos = new Vector3(-15f, 0f, 0f);
				GameObject removedCard = m_CardsOnField[0];
				m_CardsOnField.RemoveAt(0);
				removedCard.SetActive(false);

				StartCoroutine(mAnim.Rotation(removedCard, new Vector3(0f, 0f, 180f), 0.3f));
				StartCoroutine(mAnim.Move(removedCard, new Vector3(-15f, 0f, 0f), 1f));

				m_Discard.Add(removedCard);
			}

			yield return StartCoroutine(CardsAbilities());   // HERE????

			IsCardMoveDone = true;
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
			//int number = Random.Range(0, 1);
			int number = 0;
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
						//StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z -= 0.1f), 0.5f));
						StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z), 1f));
						break;
					}
				case 1:	//EDIT!!
					{
						StartCoroutine(mAnim.Rotation(card, new Vector3(0f, 0f, 180f), 0.2f));
						//StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z -= 0.1f), 1f));
						StartCoroutine(mAnim.Move(card, new Vector3(0f, 0f, mPos_Z), 1f));
						break;
					}
				default:
					break;
			}

			yield return null;
		}

		private IEnumerator AnimationMoveCardInDeck(GameObject card)
		{
			int count = mCardDeck.Count;
			Vector3 deck_Pos = mDeck.transform.position;
			deck_Pos.z += count * 0.01f;
			yield return StartCoroutine(mAnim.Move(card, deck_Pos, 1f));
		}

		public IEnumerator CardsAbilities()
		{
			switch (m_CardsOnField[m_CardsOnField.Count - 1].GetComponent<Card>().value)
			{
				case Constants.SKIP_TURN_VALUE:
					{
						if (m_TurnOrder)
						{
							NextPlayerNumber();
						}
						else
						{
							PrevPlayerNumber();
						}
						break;
					}
				case Constants.PLUS_2_VALUE:
					{
						int playerNumber = 0;

						if (mCardDeck.Count < 2)
						{
							yield return StartCoroutine(RestoreDeck());
						}

						if (m_TurnOrder)
						{
							playerNumber = GetNextPlayerNumber();
						}
						else
						{
							playerNumber = GetPrevPlayerNumber();
						}

						if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
						{
							if (playerNumber != 0)
							{
								BotFunctions bFunc = mPlayersList[playerNumber].GetComponent<BotFunctions>();

								for (int i = 0; i < 2; ++i)
								{
									GameObject card = GetCard();
									yield return StartCoroutine(AnimationGetCard(card, bFunc.Bot.spawnPoint));
									yield return StartCoroutine(bFunc.AddCard(card));
								}
							}
							else
							{
								PlayerFunctions pFunc = mPlayersList[playerNumber].GetComponent<PlayerFunctions>();

								for (int i = 0; i < 2; ++i)
								{
									GameObject card = GetCard();
									yield return StartCoroutine(pFunc.AddCard(card));
								}
							}
						}
						else   // LOCAL NETWORK.
						{
							Debug.Log("CardAbility(Plus 2) player number:" + playerNumber);
							PlayerFunctions pFunc = mPlayersList[playerNumber].GetComponent<PlayerFunctions>();

							for (int i = 0; i < 2; ++i)
							{
								GameObject card = GetCard();
								yield return StartCoroutine(pFunc.AddCard(card));
							}

							if (PlayerPrefs.GetString("PlayerRole") == MainMenu.Constants.SERVER)
							{

							}
							else
							{

							}
						}
						break;
					}
				case Constants.CHANGE_TURN_ORDER_VALUE:
					{
						if (m_TurnOrder == true)
						{
							m_TurnOrder = false;
							m_TurnOrderArrows.transform.Rotate(0f, 180f, 0f);
						}
						else
						{
							m_TurnOrder = true;
							m_TurnOrderArrows.transform.Rotate(0f, -180f, 0f);
						}
						break;
					}
				case Constants.CHANGE_COLOR_PLUS_4_VALUE:
					{
						int playerNumber = 0;

						if (mCardDeck.Count < 4)
						{
							yield return StartCoroutine(RestoreDeck());
						}

						if (m_TurnOrder)
						{
							playerNumber = GetNextPlayerNumber();
						}
						else
						{
							playerNumber = GetPrevPlayerNumber();
						}

						if (PlayerPrefs.GetString("GameType") == MainMenu.Constants.AI)
						{
							if (playerNumber != 0)
							{
								BotFunctions bFunc = mPlayersList[playerNumber].GetComponent<BotFunctions>();
								for (int i = 0; i < 4; ++i)
								{
									GameObject card = GetCard();
									yield return StartCoroutine(AnimationGetCard(card, bFunc.Bot.spawnPoint));
									yield return StartCoroutine(bFunc.AddCard(card));
								}
								NextPlayerNumber();
							}
							else
							{
								PlayerFunctions pFunc = mPlayersList[playerNumber].GetComponent<PlayerFunctions>();
								for (int i = 0; i < 4; ++i)
								{
									GameObject card = GetCard();
									yield return StartCoroutine(pFunc.AddCard(card));
								}
								NextPlayerNumber();
							}
						}
						else    // LOCAL NETWORK
						{
							Debug.Log("CardAbility player number:" + playerNumber);
							PlayerFunctions pFunc = mPlayersList[playerNumber].GetComponent<PlayerFunctions>();
							for (int i = 0; i < 4; ++i)
							{
								GameObject card = GetCard();
								yield return StartCoroutine(pFunc.AddCard(card));
							}
							//NextPlayerNumber();
						}
						break;
					}
				default:
					break;
			}
		}
	}
}
