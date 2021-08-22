using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;

namespace NetworkServer
{
	public class Server : MonoBehaviour
	{
		private string m_ServerName;
		private IPAddress m_IP_Address;
		private const int m_Port = 9933;
		private Thread m_SocketThread;
		private Socket m_Listener;
		//private Socket m_Handler;

//-----------------------------------------------------------------------------

		public void SetServerName(string _name)
		{
			m_ServerName = _name;
		}

		public string GetServerName()
		{
			return m_ServerName;
		}

		public IPAddress GetIPAddress()
		{
			return m_IP_Address;
		}

		public void StartServer()
		{
			Application.runInBackground = true;
			m_SocketThread = new Thread(networkCode);
			m_SocketThread.IsBackground = true;
			m_SocketThread.Start();
		}

		public void StopServer()
		{
			m_Listener.Close();
			Debug.Log("Disconnected!");

			if (m_SocketThread != null)
			{
				m_SocketThread.Abort();
			}
		}

		private void networkCode()
		{
			string localIP = "";
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
				}
			}

			IPAddress[] ipArray = Dns.GetHostAddresses(localIP);
			m_IP_Address = ipArray[0];
			IPEndPoint ipEndPoint = new IPEndPoint(m_IP_Address, m_Port);
			m_Listener = new Socket(m_IP_Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			m_Listener.Bind(ipEndPoint);
			m_Listener.Listen(1000);
			Debug.Log("Sever IP: " + ipEndPoint);

			while (true)
			{
				try
				{
					Socket user = m_Listener.Accept();
					ServerFunctions.NewClient(user);
				}
				catch (Exception ex)
				{
					Debug.Log(ex.ToString());
				}
			}
		}
	}
}
