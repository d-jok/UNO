﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkServer
{
	public class ServerClients
	{
		// Public:
		public int Number;
		public string Name;
		public bool isLoaded = false;
		public bool isSync = false;
		public bool isCardsDisributed = false;
		public bool isTurnDone = false;

		// Card or other.
		public string Action = "";
		public string cardName = "";
		public string cardColor = "";

		public bool IsUno = false;
		public bool IsUnoPressed = false;

		// Private:
		private Socket _handler;
		private Thread _userThread;

		//-----------------------------------------------------------------------------

		public ServerClients() { }

		public ServerClients(Socket socket)
		{
			_handler = socket;
			_userThread = new Thread(listener);
			_userThread.IsBackground = true;
			_userThread.Start();
		}

		public void End()
		{
			try
			{
				_handler.Close();
				try
				{
					_userThread.Abort();
				}
				catch { } // г
			}
			catch (Exception)
			{
				//Console.WriteLine("Error with end: {0}.", exp.Message);
			}
		}

		private void listener()
		{
			while (true)
			{
				try
				{
					byte[] buffer = new byte[_handler.ReceiveBufferSize];
					int bytesRec = _handler.Receive(buffer, SocketFlags.None);

					string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
					handleCommand(data, this, buffer);
				}
				catch
				{
					ServerFunctions.EndClient(this); return;
				}
			}
		}

		private void handleCommand(string data, ServerClients client, byte[] buffer)
		{
			string hashtag = data.Split(' ')[0];

			if (hashtag == "#Name")
			{
				Name = data.Split(' ')[1];
			}
			else if (hashtag == "#Loaded")
			{
				isLoaded = true;
			}
			else if (hashtag == "#DeckIsSync")
			{
				isSync = true;
			}
			else if (hashtag == "#CardsWasDistributed")
			{
				isCardsDisributed = true;
			}
			else if (hashtag == "#TurnIsDone")
			{
				isTurnDone = true;
			}
			else if (hashtag == "#Card")
			{
				if (data.Contains("Color"))
				{
					Action = "Card";
					cardName = data.Split(' ')[1];
					cardColor = data.Split(' ')[2];
				}
				else
				{
					Action = "Card";
					cardName = data.Split(' ')[1];
				}
			}
			else if (hashtag == "#Deck")
			{
				Action = "Deck";
			}
			else if (hashtag == "#UNO_True")
			{
				string command = "#UNO_True " + client.Number;

				IsUno = true;
				IsUnoPressed = true;

				foreach (var player in ServerFunctions.Clients)
				{
					if (client.Number != player.Number)
					{
						player.Send(command);
					}
				}
			}
			else if (hashtag == "#UNO_False")
			{
				string command = "#UNO_False " + client.Number;

				IsUno = true;
				IsUnoPressed = false;

				foreach (var player in ServerFunctions.Clients)
				{
					if (client.Number != player.Number)
					{
						player.Send(command);
					}
				}
			}
		}

		public void Send(string command)
		{
			try
			{
				int bytesSent = _handler.Send(Encoding.UTF8.GetBytes(command));
				//if (bytesSent > 0) Console.WriteLine("Success");
			}
			catch (Exception)
			{
				//Console.WriteLine("Error with send command: {0}.", exp.Message);
				ServerFunctions.EndClient(this);
			}
		}
	}
}
