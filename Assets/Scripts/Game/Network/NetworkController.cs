using System;
using System.Collections;
using System.Collections.Generic;
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

		private GameObject m_Server;
		private NetworkServer.Server m_ServerFunctions;

		//------------------------------------

		private void Start()
		{
			m_Server = GameObject.Find("Server") as GameObject;
			m_ServerFunctions = m_Server.GetComponent<NetworkServer.Server>();
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
			m_ServerFunctions.SetServerName(name);

			try
			{
				m_ServerFunctions.StartServer();
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
			m_ServerFunctions.StopServer();
			StartServerButton.SetActive(true);
			StopServerButton.SetActive(false);
		}
	}
}
