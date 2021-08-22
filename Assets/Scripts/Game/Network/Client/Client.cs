using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NetworkClient
{
	public class Client : MonoBehaviour
	{
		public Socket clientSocket;
		public Thread clientThread;

		private int m_serverPort = 9933;

//-----------------------------------------------------------------------------

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
			catch (Exception ex)
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
				Debug.Log("Connected");

				if (data != null)
				{
					
				}
			}
		}
	}
}
