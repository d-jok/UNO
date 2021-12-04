using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkClient
{
	public class Client : MonoBehaviour
	{
		public int Number;
		public string Name;
		public Socket clientSocket;
		public Thread clientThread;

		public string Action = "";
		public string cardName = "";
		public string cardColor = "";
		//public string cardAction = "";
		public int playerPlayed = 0;

		private int m_serverPort;
		private bool m_is_StartGame = false;
		private bool m_is_DeckSync = false;
		private bool m_is_playersInfo = false;
		private bool m_is_SetNumber = false;
		private bool m_is_YourTurn = false;
		private bool m_is_Update = false;
		private List<string> m_cards_names;
		private GameObject m_gameControllerObj;
		private Game.GameController m_gameController;
		private List<string> listInfo = new List<string>();

		//-----------------------------------------------------------------------------

		private void Start()
		{
			m_serverPort = 9933;
			m_cards_names = new List<string>();
		}

		private void Update()
		{
			if (m_is_StartGame == true)
			{
				m_is_StartGame = false;
				DontDestroyOnLoad(this.gameObject);
				PlayerPrefs.SetString("GameType", MainMenu.Constants.LOCAL_NETWORK);
				PlayerPrefs.SetString("PlayerRole", MainMenu.Constants.CLIENT);
				SceneManager.LoadScene("Game");	
			}
			else if (m_is_DeckSync == true)
			{
				m_is_DeckSync = false;
				m_gameController.DeckSync(m_cards_names);
			}
			else if (m_is_playersInfo)
			{
				m_is_playersInfo = false;
				foreach (var item in listInfo)
				{
					m_gameController.players_lan.Add(new Game.GameController.PlayerForLan() {
						m_Name = item.Split('&')[0],
						m_Number = Int32.Parse(item.Split('&')[1])
					});
				}

				m_gameController.SpawnPlayers();
			}
			else if (m_is_YourTurn)
			{
				m_is_YourTurn = false;
				m_gameController.IsPlayerTurn_Lan = true;
			}
			else if (m_is_Update)
			{
				m_is_Update = false;
				StartCoroutine(GameUpdate());
			}
			//else if (m_is_SetNumber)
			//{
			//	Number = Int32.Parse(receivedCommand);
			//}
		}

		private IEnumerator GameUpdate()
		{
			List<GameObject> playersList = m_gameController.getPlayersList();
			GameObject player = new GameObject();

			for (int i = 0; i < playersList.Count; ++i)
			{
				if (m_gameController.players_lan_updated[i].m_Number == playerPlayed)
				{
					player = playersList[i];
				}
			}

			Game.PlayerFunctions func = player.GetComponent<Game.PlayerFunctions>();
			GameObject selectedCard = new GameObject();
			int Counter = 0;

			if (Action == "Card")
			{
				foreach (var card in func.mPlayer.cardsInHand)
				{
					string name = card.name.Split('(')[0];
					if (name == cardName)
					{
						selectedCard = card;
						func.mPlayer.cardsInHand.RemoveAt(Counter);
						break;
					}
					++Counter;
				}

				cardName = "";

				if (cardColor != "")
				{
					Game.Choose_Color changeColor = selectedCard.GetComponent<Game.Choose_Color>();
					Game.Color _cardColor;
					System.Enum.TryParse(cardColor, out _cardColor);
					changeColor.ChangeColor(_cardColor);
					cardColor = "";
				}

				yield return StartCoroutine(m_gameController.MoveCardOnField(selectedCard, "Player"));
				yield return StartCoroutine(func.CardsPositioning());
			}
			else if (Action == "Deck")
			{
				int CurrentPlayerNumber = m_gameController.GetCurrentPlayerNumber();
				GameObject card = m_gameController.GetCard();
				yield return StartCoroutine(func.AddCard(card));
			}

			if (m_gameController.getTurnOrder() == true)
			{
				m_gameController.NextPlayerNumber();
			}
			else
			{
				m_gameController.PrevPlayerNumber();
			}

			m_gameController.ArrowTurn();
			Action = "";
		}

		public void OnLoadGameScene()
		{
			send("#Loaded");
			m_gameControllerObj = GameObject.Find("GameController");
			m_gameController = m_gameControllerObj.GetComponent<Game.GameController>();
		}

		public void Connect(IPAddress _serverIP)
		{
			Debug.Log("Connected");
			try
			{
				IPEndPoint ipEndPoint = new IPEndPoint(_serverIP, m_serverPort);
				clientSocket = new Socket(_serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				clientSocket.Connect(ipEndPoint);

				clientThread = new Thread(listener);
				clientThread.IsBackground = true;
				clientThread.Start();
			}
			catch (Exception)
			{
				
			}
		}

		private void listener()
		{
			while (clientSocket.Connected)
			{
				byte[] buffer = new byte[8196];
				int bytesRec = clientSocket.Receive(buffer);
				string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

				if (data != null)
				{
					handleCommands(data);
				}
			}
		}

		public void send(string data)
		{

			try
			{
				byte[] buffer = Encoding.UTF8.GetBytes(data);
				int bytesSent = clientSocket.Send(buffer);
			}
			catch
			{
				
			}
		}

		private void handleCommands(string _data)
		{
			int spaceCount = _data.Split(' ').Length - 1;
			string hashtag = "";

			if (spaceCount != 0)
			{
				hashtag = _data.Split(' ')[0];
			}
			else
			{
				hashtag = _data;
			}

			if (hashtag == "#SetNumber")
			{
				Number = Int32.Parse(_data.Split(' ')[1]);
			}
			else if (hashtag == "#GetName")
			{
				send("#Name " + Name);
			}
			else if (hashtag == "#StartGame")
			{
				m_is_StartGame = true;
			}
			else if (hashtag == "#DeckAtStart")
			{
				string names = _data.Substring(hashtag.Length);
				m_cards_names = names.Split(' ').ToList();
				m_is_DeckSync = true;
			}
			else if (hashtag == "#PlayersInfo")
			{
				string info = _data.Substring(hashtag.Length + 1);
				string temp = "";

				for (int i = 0; i < info.Length; ++i)
				{
					if (info[i] != ' ')
					{
						temp += info[i];
					}
					else
					{
						listInfo.Add(temp);
						temp = "";
					}
				}
				
				//listInfo = info.Split(' ').ToList();
				m_is_playersInfo = true;
			}
			else if (hashtag == "#YourTurn")
			{
				m_is_YourTurn = true;
			}
			else if (hashtag == "#Card")
			{
				if (_data.Contains("Color"))
				{
					Action = "Card";
					cardName = _data.Split(' ')[1];
					cardColor = _data.Split(' ')[2];
					playerPlayed = Int32.Parse(_data.Split(' ')[3]);
					m_is_Update = true;
				}
				else
				{
					Action = "Card";
					cardName = _data.Split(' ')[1];
					playerPlayed = Int32.Parse(_data.Split(' ')[2]);

					//if (cardName.Contains("Block"))
					//{

					//}
					//else if (cardName.Contains("Plus_2"))
					//{

					//}
					//else if (cardName.Contains("Turn"))
					//{

					//}

					m_is_Update = true;
				}
			}
			else if (hashtag == "#Deck")
			{
				Action = "Deck";
				playerPlayed = Int32.Parse(_data.Split(' ')[1]);
				m_is_Update = true;
			}
		}
	}
}
