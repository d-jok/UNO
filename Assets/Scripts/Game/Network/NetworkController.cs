using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
	public class NetworkController : MonoBehaviour
	{
		public GameObject MessagePanel;
		public GameObject InputField;
		public GameObject StartServerButton;
		public GameObject StopServerButton;
		//public GameObject ServerIP_Text;

		public GameObject ClientInputField;

		private GameObject m_ServerObj;
		private NetworkServer.Server m_Server;
		private GameObject m_ClientObj;
		private NetworkClient.Client m_Client;

		//------------------------------------

		private void Start()
		{
			m_ServerObj = GameObject.Find("Server") as GameObject;
			m_Server = m_ServerObj.GetComponent<NetworkServer.Server>();

			m_ClientObj = GameObject.Find("Client") as GameObject;
			m_Client = m_ClientObj.GetComponent<NetworkClient.Client>();
		}

		public void StartSever()
		{
			GameObject inputFieldText = InputField.transform.Find("Text").gameObject;
			string name = inputFieldText.GetComponent<Text>().text;

			if (name == "")
			{
				GameObject text = MessagePanel.transform.Find("Text").gameObject;
				text.GetComponent<Text>().text = "Input server name!";
				MessagePanel.SetActive(true);
				return;
			}

			StartServerButton.SetActive(false);
			StopServerButton.SetActive(true);
			m_Server.SetServerName(name);

			try
			{
				m_Server.StartServer();
			}
			catch (Exception ex)
			{
				GameObject text = MessagePanel.transform.Find("Text").gameObject;
				text.GetComponent<Text>().text = ex.ToString();
			}

			//ServerIP_Text.GetComponent<Text>().text += m_ServerFunctions.GetIPAddress().ToString();
		}

		public void StopServer()
		{
			m_Server.StopServer();
			StartServerButton.SetActive(true);
			StopServerButton.SetActive(false);
		}

		public void StartGame()
		{
			PlayerPrefs.SetString("PlayerRole", MainMenu.Constants.SERVER);
			m_Server.StartGame();
		}

		public void ClientConnecting()
		{
			GameObject inputFieldText = ClientInputField.transform.Find("Text").gameObject;
			string inputText = inputFieldText.GetComponent<Text>().text;
			PlayerPrefs.SetString("PlayerRole", MainMenu.Constants.CLIENT);

			if (inputText == "")
			{
				GameObject text = MessagePanel.transform.Find("Text").gameObject;
				text.GetComponent<Text>().text = "Input server IP!";
				MessagePanel.SetActive(true);
				return;
			}

			try
			{
				IPAddress ipAddress = IPAddress.Parse(inputText);
				m_Client.Connect(ipAddress);
			}
			catch (Exception ex)
			{
				GameObject text = MessagePanel.transform.Find("Text").gameObject;
				text.GetComponent<Text>().text = ex.ToString();
			}
		}
	}
}
