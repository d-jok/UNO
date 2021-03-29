using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot
{
	public string playerName;
	public Vector3 spawnPoint;
	public List<GameObject> cardsInHand;

	public Bot()
	{
		playerName = "default";
		cardsInHand = new List<GameObject>();
	}

	public Bot(string name)
	{
		playerName = name;
		cardsInHand = new List<GameObject>();
	}

	public Bot(string name, Vector3 point, List<GameObject> cards)
	{
		playerName = name;
		spawnPoint = point;
		cardsInHand = cards;
	}
}
