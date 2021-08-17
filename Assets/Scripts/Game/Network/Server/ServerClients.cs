using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkServer
{
	public class ServerClients
	{
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
			catch (Exception exp)
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

		}
	}
}
