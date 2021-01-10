using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public List<GameObject> Cards;

	private List<GameObject> mCardDeck;
	private List<GameObject> mCardField;
	private List<GameObject> mPlayers;

    void Start()
    {
		CreateDeck();
		SmashDeck();

		float z = 0;
		mCardDeck = new List<GameObject>();

        foreach (var card in Cards)
		{
			mCardDeck.Add(Instantiate(card, new Vector3(0f, 0f, z), card.transform.rotation, card.transform.parent));
			z += 0.1f;
		}
    }

    void Update()
    {
        
    }

	private void AddPlayers()
	{

	}

	private void GameProcess()
	{

	}

	private void CreateDeck()
	{
		List<GameObject> list = new List<GameObject>();

		foreach (var card in Cards)
		{
			list.Add(card);

			if (!card.name.Contains("0") && !card.name.Contains("Color_Change"))
			{
				list.Add(card);
			}
			else if (card.name.Contains("Color_Change"))
			{
				list.Add(card);
				list.Add(card);
				list.Add(card);
			}
		}

		Cards = list;
	}

	private void SmashDeck()
	{
		int size = Cards.Count;

		for (int i = 0; i < size; i++)
		{
			GameObject temp = Cards[i];
			Cards.RemoveAt(i);
			Cards.Insert(Random.Range(0, size), temp);
		}
	}

	public GameObject GetCard()
	{
		int size = mCardDeck.Count;
		GameObject card = mCardDeck[size - 1];
		mCardDeck.RemoveAt(mCardDeck.Count - 1);

		return card;
	}

	private void CardsDistribution()
	{

	}
}
