using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
		private AnimationScript anim = new AnimationScript();

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
								PlayerPrefs.SetString("GameType", Constants.AI);
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
			yield return new WaitForSecondsRealtime(seconds);
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
				Material matAI = Resources.Load<Material>("Materials/Menu Materials/SinglePlayerIcon");
				Material matLocalNet = Resources.Load<Material>("Materials/Menu Materials/MultiplayerIcon");

				StartCoroutine(anim.Move(newGame, new Vector3(0f, 1f, 0f), 1f));
				StartCoroutine(anim.Rotation(newGame, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				// Creates elements of new game.
				// AI
				GameObject AI = newGame;
				AI.name = Constants.AI;
				AI.transform.Find("Front").GetComponent<MeshRenderer>().material = matAI;
				AI.transform.Find("Text").GetComponent<TextMesh>().text = Constants.AI;

				// Local Network.
				GameObject localNetWork = Instantiate(newGame, newGame.transform.position, newGame.transform.rotation, newGame.transform.parent);
				localNetWork.name = Constants.LOCAL_NETWORK;
				localNetWork.transform.Find("Front").GetComponent<MeshRenderer>().material = matLocalNet;
				localNetWork.transform.Find("Text").GetComponent<TextMesh>().text = Constants.LOCAL_NETWORK;
				//-----------------------------

				StartCoroutine(anim.Rotation(AI, new Vector3(0f, 0f, 180f), 0.3f));
				StartCoroutine(anim.Rotation(localNetWork, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				StartCoroutine(anim.Move(AI, new Vector3(-1f, 1f, 0f), 1f));
				StartCoroutine(anim.Move(localNetWork, new Vector3(1f, 1f, 0f), 1f));

				isDone = true;
			}
		}

		IEnumerator AnimationSettings(GameObject settings)
		{
			StartCoroutine(anim.Scaling(settings, 20f));
			yield return new WaitWhile(() => anim.mIsScalingDone == false);

			// HERE must be a code!!!
			Destroy(settings);
		}

		IEnumerator AnimationAI(GameObject AI)
		{
			if (isDone == true)
			{
				isDone = false;

				StartCoroutine(anim.Move(AI, new Vector3(0f, 1f, 0f), 1f));
				StartCoroutine(anim.Rotation(AI, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				StartCoroutine(anim.Rotation(AI, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				StartCoroutine(anim.Rotation(AI, new Vector3(0f, 10f, 30f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				GameObject text = new GameObject();
				text.AddComponent<TextMesh>();
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
	}
}
