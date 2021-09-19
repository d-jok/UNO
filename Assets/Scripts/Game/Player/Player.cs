using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Player
	{
		public string playerName;
		public int playerNumber;
		public Vector3 spawnPoint;
		public List<GameObject> cardsInHand;

		public Player()
		{
			playerName = "default";
			cardsInHand = new List<GameObject>();
		}

		public Player(string name)
		{
			playerName = name;
			cardsInHand = new List<GameObject>();
		}

		public Player(string name, Vector3 point, List<GameObject> cards)
		{
			playerName = name;
			spawnPoint = point;
			cardsInHand = cards;
		}
	}
}
