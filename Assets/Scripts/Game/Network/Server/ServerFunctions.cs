using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Client = NetworkServer.ServerClients;

namespace NetworkServer
{
	public class ServerFunctions
	{
		public static List<Client> Clients = new List<Client>();

		public static void NewClient(Socket handle)
		{
			try
			{
				Client newClient = new Client(handle);
				Clients.Add(newClient);
			}
			catch (Exception exp)
			{
				//Console.WriteLine("Error with addNewClient: {0}.", exp.Message);
			}
		}

		public static void EndClient(Client client)
		{
			try
			{
				client.End();
				Clients.Remove(client);
				//Console.WriteLine("User {0} has been disconnected.", client.UserName);
			}
			catch (Exception exp)
			{
				//Console.WriteLine("Error with endClient: {0}.", exp.Message);
			}
		}

		public int GetClientsCount()
		{
			return Clients.Count;
		}

		public void SendToAll(string _command)
		{
			try
			{
				foreach (var client in Clients)
				{
					client.Send(_command);
				}
			}
			catch (Exception)
			{

			}
		}
	}
}
