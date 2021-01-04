using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	private List<GameObject> mGameObjects = new List<GameObject>();

	public void AddGameObject(ref GameObject obj)
	{
		mGameObjects.Add(obj);
	}

	public GameObject GetGameObject(ref Vector3 position)
	{
		foreach (GameObject obj in mGameObjects)
		{
			if (!obj.activeSelf)
			{
				obj.transform.position = position;
				obj.SetActive(true);
				return obj;
			}
		}

		return Instantiate(mGameObjects[0], position, Quaternion.identity);
	}
}
