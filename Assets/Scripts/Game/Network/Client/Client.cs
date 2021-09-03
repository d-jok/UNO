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
		public string Name;
		public Socket clientSocket;
		public Thread clientThread;

		private int m_serverPort;
		private bool m_is_StartGame = false;
		private bool m_is_DeckSync = false;
		private List<string> m_cards_names;
		private GameObject m_gameControllerObj;
		private Game.GameController m_gameController;

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
			string hashtag;

			if (spaceCount != 0)
			{
				hashtag = _data.Split(' ')[0];
			}
			else
			{
				hashtag = _data;
			}

			if (hashtag == "#GetName")
			{
				send("#Name " + Name);
			}
			else if (hashtag == "#StartGame")
			{
				m_is_StartGame = true;
			}
			else if (hashtag == "#Deck")
			{
				string names = _data.Substring(hashtag.Length);
				m_cards_names = names.Split(' ').ToList();
				m_is_DeckSync = true;
			}
		}
	}
}
