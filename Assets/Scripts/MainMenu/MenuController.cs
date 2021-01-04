using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace MainMenu
{
	public class MenuController : MonoBehaviour
	{
		public GameObject Canvas;
		public GameObject MenuElements;

		// UI Elements
		private GameObject mImage;
		private GameObject mLogo;
		private GameObject mPageTitle;
		private GameObject mPowerOffButton;
		//---------------------------------

		private bool isDone;
		private bool mIsRotateDone;
		private bool mIsScalingDone;

		private void Awake()
		{
			mImage = Canvas.transform.Find("Background").gameObject;
			mLogo = Canvas.transform.Find("Logo").gameObject;
			mPageTitle = Canvas.transform.Find("PageTitle").gameObject;
			mPowerOffButton = Canvas.transform.Find("PowerButton").gameObject;
			//GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);

			mImage.SetActive(true);
			mLogo.SetActive(false);
			mPageTitle.SetActive(false);
			mPowerOffButton.SetActive(false);

			StartCoroutine(StartIntro(3));
		}

		void Start()
		{
			isDone = true;
			mIsRotateDone = false;
			mIsScalingDone = false;


		}

		private void FixedUpdate()
		{

		}

		private void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					switch (hit.transform.gameObject.name)
					{
						case Constants.NEW_GAME:
							{
								mPageTitle.GetComponent<Text>().text = Constants.NEW_GAME;
								int childCount = MenuElements.transform.childCount;
								GameObject newGame = MenuElements.transform.Find(Constants.NEW_GAME).gameObject;

								for (int i = 0; i < childCount; ++i)
								{
									GameObject child = MenuElements.transform.GetChild(i).gameObject;
									if (child.name != Constants.NEW_GAME && child.name != "Background")  //EDIT CHANGE!!!!
									{
										Destroy(child);
										//child.SetActive(false);
									}
								}
								//Start();

								StartCoroutine(AnimationNewGame(newGame));
								break;
							}
						case Constants.SETTINGS:
							{
								mPageTitle.GetComponent<Text>().text = Constants.SETTINGS;
								GameObject settings = MenuElements.transform.Find(Constants.SETTINGS).gameObject;
								StartCoroutine(AnimationSettings(settings));
								break;
							}
						case Constants.AI:
							{
								int childCount = MenuElements.transform.childCount;

								for (int i = 0; i < childCount; ++i)
								{
									GameObject child = MenuElements.transform.GetChild(i).gameObject;
									if (child.name != Constants.AI && child.name != "Background")   //EDIT CHANGE!!!!
									{
										Destroy(child);
									}
								}

								GameObject AI = MenuElements.transform.Find(Constants.AI).gameObject;
								StartCoroutine(AnimationAI(AI));
								break;
							}
						case Constants.LOCAL_NETWORK:
							{

								break;
							}
						default:
							break;
					}
				}
			}
		}

		IEnumerator StartIntro(int seconds)
		{
			//Image newImage = mImage.GetComponent<Image>();
			//Sprite sprite = Resources.Load<Sprite>("Menu background");

			yield return new WaitForSecondsRealtime(seconds);
			//newImage.sprite = sprite;
			mImage.SetActive(false);
			mLogo.SetActive(true);
			mPageTitle.SetActive(true);
			mPowerOffButton.SetActive(true);
		}

		public void PowerOff()
		{
			Application.Quit();
		}

		IEnumerator AnimationNewGame(GameObject newGame)
		{
			if (isDone == true)
			{
				isDone = false;
				//newGame = MenuElements.transform.Find(NEW_GAME).gameObject;
				StartCoroutine(Move(newGame, new Vector3(0f, 1f, 0f)));
				StartCoroutine(Rotation(newGame, new Vector3(0f, 0f, 180f)));

				yield return new WaitWhile(() => mIsRotateDone == false);
				mIsRotateDone = false;


				// Creates elements of new game.
				// AI
				GameObject AI = newGame;  //Edit!!!
				AI.name = Constants.AI;
				Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Menu Materials/SinglePlayerIcon.mat", typeof(Material));
				AI.transform.Find("Front").GetComponent<MeshRenderer>().material = mat;
				AI.transform.Find("Text").GetComponent<TextMesh>().text = Constants.AI;

				// Local Network.
				GameObject localNetWork = Instantiate(newGame, newGame.transform.position, newGame.transform.rotation, newGame.transform.parent);
				localNetWork.name = Constants.LOCAL_NETWORK;
				mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Menu Materials/MultiplayerIcon.mat", typeof(Material));
				localNetWork.transform.Find("Front").GetComponent<MeshRenderer>().material = mat;
				localNetWork.transform.Find("Text").GetComponent<TextMesh>().text = Constants.LOCAL_NETWORK;
				//-----------------------------


				StartCoroutine(Rotation(AI, new Vector3(0f, 0f, 180f)));
				//yield return new WaitWhile(() => mIsRotateDone == false);
				StartCoroutine(Rotation(localNetWork, new Vector3(0f, 0f, 180f)));
				yield return new WaitWhile(() => mIsRotateDone == false);
				mIsRotateDone = false;

				StartCoroutine(Move(AI, new Vector3(-1f, 1f, 0f)));
				StartCoroutine(Move(localNetWork, new Vector3(1f, 1f, 0f)));

				isDone = true;
			}
		}

		IEnumerator AnimationSettings(GameObject settings)
		{
			StartCoroutine(Scaling(settings, 20f));
			yield return new WaitWhile(() => mIsScalingDone == false);
			mIsScalingDone = false;

			Destroy(settings);
		}

		IEnumerator AnimationAI(GameObject AI)
		{
			if (isDone == true)
			{
				isDone = false;

				StartCoroutine(Move(AI, new Vector3(0f, 1f, 0f)));
				StartCoroutine(Rotation(AI, new Vector3(0f, 0f, 180f)));
				yield return new WaitWhile(() => mIsRotateDone == false);
				mIsRotateDone = false;

				StartCoroutine(Rotation(AI, new Vector3(0f, 0f, 180f)));
				yield return new WaitWhile(() => mIsRotateDone == false);
				mIsRotateDone = false;

				StartCoroutine(Rotation(AI, new Vector3(0f, 10f, 30f)));
				yield return new WaitWhile(() => mIsRotateDone == false);
				mIsRotateDone = false;

				GameObject text = new GameObject();
				text.AddComponent<TextMesh>();
				//text.AddComponent<MeshRenderer>();
				text.GetComponent<TextMesh>().text = "Игра началась...";
				text.GetComponent<TextMesh>().characterSize = 0.1f;
				text.GetComponent<TextMesh>().anchor = TextAnchor.UpperCenter;
				text.GetComponent<TextMesh>().fontSize = 18;
				// Add color HERE!!!
				text.transform.position = new Vector3(0f, 0f, 0f);

				yield return new WaitForSecondsRealtime(3);
				SceneManager.LoadScene("Game");

				isDone = true;
			}

			yield return null;
		}

		IEnumerator Move(GameObject obj, Vector3 targetPoint)
		{
			float speed = 1f;
			float step = (speed / (obj.transform.position - targetPoint).magnitude) * Time.fixedDeltaTime;
			float t = 0;
			while (t <= 1.0f)
			{
				t += step; // Goes from 0 to 1, incrementing by step each time
				obj.transform.position = Vector3.Lerp(obj.transform.position, targetPoint, t); // Move objectToMove closer to b
				yield return null;         // Leave the routine and return here in the next frame
			}
			obj.transform.position = targetPoint;

			//yield return null;
			/*float step = (speed / (obj.transform.position - targetPoint).magnitude) * Time.fixedDeltaTime;
			float t = 0;
			while (t <= 1.0f)
			{
				t += step; // Goes from 0 to 1, incrementing by step each time
				target.position = Vector3.Lerp(obj.transform.position, targetPoint, t); // Move objectToMove closer to b
				yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
			}
			target.position = targetPoint;*/
		}

		IEnumerator Rotation(GameObject obj, Vector3 rotation)
		{
			Quaternion start = obj.transform.rotation;
			Quaternion destination = start * Quaternion.Euler(rotation);
			float startTime = Time.time;
			float percentComplete = 0f;
			while (percentComplete <= 1.0f)
			{
				percentComplete = (Time.time - startTime) / 0.3f;
				obj.transform.rotation = Quaternion.Slerp(start, destination, percentComplete);
				yield return null;
			}

			obj.transform.rotation = destination;
			mIsRotateDone = true;
		}

		IEnumerator Scaling(GameObject obj, float scale)
		{
			float speed = 0.9f; // the more the value, the slower the animation.
			float endScale = obj.transform.localScale.x * scale;

			float startTime = Time.time;
			float percentComplete = 0f;

			if (endScale < obj.transform.localScale.x)
			{
				while (endScale < obj.transform.localScale.x)
				{
					percentComplete = (Time.time - startTime) / speed;
					obj.transform.localScale -= new Vector3(percentComplete, percentComplete, percentComplete);
					yield return null;
				}
			}
			else
			{
				while (endScale > obj.transform.localScale.x)
				{
					percentComplete = (Time.time - startTime) / speed;
					obj.transform.localScale += new Vector3(percentComplete, percentComplete, percentComplete);
					yield return null;
				}
			}

			mIsScalingDone = true;
		}
	}
}
