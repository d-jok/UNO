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
		private GameObject m_PageTitle;
		private GameObject mPowerOffButton;
		private GameObject m_ServerPanel;
		private GameObject m_ClientPanel;
		private GameObject m_NetworkController;
		private GameObject m_BackButton;
		//private NetworkServer.Server m_Server;
		//private NetworkClient.Client m_Client;
		//---------------------------------

		private bool isDone;
		private AnimationScript anim = new AnimationScript();
		private List<string> m_MenuItemsStack;

		private void Awake()
		{
			mImage = Canvas.transform.Find("Background").gameObject;
			mLogo = Canvas.transform.Find("Logo").gameObject;
			m_PageTitle = Canvas.transform.Find("PageTitle").gameObject;
			mPowerOffButton = Canvas.transform.Find("PowerButton").gameObject;
			m_ServerPanel = Canvas.transform.Find("NetworkPanel").Find("Server").gameObject;
			m_ClientPanel = Canvas.transform.Find("NetworkPanel").Find("Client").gameObject;
			m_NetworkController = GameObject.Find("NetworkController");
			m_BackButton = Canvas.transform.Find("BackButton").gameObject;
			//m_Server = m_NetworkController.GetComponent<NetworkServer.Server>();
			//m_Client = m_NetworkController.GetComponent<NetworkClient.Client>();
			//GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);

			mImage.SetActive(true);
			mLogo.SetActive(false);
			m_PageTitle.SetActive(false);
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
								m_PageTitle.GetComponent<Text>().text = Constants.NEW_GAME;
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
								m_PageTitle.GetComponent<Text>().text = Constants.SETTINGS;
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
								m_PageTitle.GetComponent<Text>().text = Constants.LOCAL_NETWORK;
								int childCount = MenuElements.transform.childCount;
								GameObject lanGame = MenuElements.transform.Find(Constants.AI).gameObject;

								for (int i = 0; i < childCount; ++i)
								{
									GameObject child = MenuElements.transform.GetChild(i).gameObject;
									if (child.name != Constants.AI && child.name != "Background")  //EDIT CHANGE!!!!
									{
										Destroy(child);
									}
								}

								StartCoroutine(AnimationLan(lanGame));
								break;
							}
						case Constants.SERVER:
							{
								m_PageTitle.GetComponent<Text>().text = Constants.LOCAL_NETWORK_SERVER;
								m_ServerPanel.SetActive(true);
								break;
							}
						case Constants.CLIENT:
							{
								m_PageTitle.GetComponent<Text>().text = Constants.LOCAL_NETWORK_CLIENT;
								m_ClientPanel.SetActive(true);
								break;
							}
						default:
							break;
					}
				}
			}
		}

		private GameObject getCanvasObj(string _name)
		{
			return Canvas.transform.Find(_name).gameObject;
		}

		IEnumerator StartIntro(int seconds)
		{
			yield return new WaitForSecondsRealtime(seconds);
			mImage.SetActive(false);
			mLogo.SetActive(true);
			m_PageTitle.SetActive(true);
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

		IEnumerator AnimationLan(GameObject _lanGame)
		{
			if (isDone == true)
			{
				isDone = false;
				Material matServer = Resources.Load<Material>("Materials/Menu Materials/ServerIcon");
				Material matClient = Resources.Load<Material>("Materials/Menu Materials/ClientIcon");

				StartCoroutine(anim.Move(_lanGame, new Vector3(0f, 1f, 0f), 1f));
				StartCoroutine(anim.Rotation(_lanGame, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				// Creates elements of lan game.
				// Server.
				GameObject Server = _lanGame;
				Server.name = Constants.SERVER;
				Server.transform.Find("Front").GetComponent<MeshRenderer>().material = matServer;
				Server.transform.Find("Text").GetComponent<TextMesh>().text = Constants.SERVER;

				// Client.
				GameObject Client = Instantiate(_lanGame, _lanGame.transform.position, _lanGame.transform.rotation, _lanGame.transform.parent);
				Client.name = Constants.CLIENT;
				Client.transform.Find("Front").GetComponent<MeshRenderer>().material = matClient;
				Client.transform.Find("Text").GetComponent<TextMesh>().text = Constants.CLIENT;
				//-----------------------------

				StartCoroutine(anim.Rotation(Server, new Vector3(0f, 0f, 180f), 0.3f));
				StartCoroutine(anim.Rotation(Client, new Vector3(0f, 0f, 180f), 0.3f));
				yield return new WaitWhile(() => anim.mIsRotateDone == false);

				StartCoroutine(anim.Move(Server, new Vector3(-1f, 1f, 0f), 1f));
				StartCoroutine(anim.Move(Client, new Vector3(1f, 1f, 0f), 1f));

				isDone = true;
			}

			m_BackButton.SetActive(true);

			yield return null;
		}

		public void Back()
		{
			m_BackButton.SetActive(false);
			GameObject server = MenuElements.transform.Find(Constants.SERVER).gameObject;

			int childCount = MenuElements.transform.childCount;

			for (int i = 0; i < childCount; ++i)
			{
				GameObject child = MenuElements.transform.GetChild(i).gameObject;
				if (child.name != Constants.SERVER && child.name != "Background")  //EDIT CHANGE!!!!
				{
					Destroy(child);
				}
			}

			StartCoroutine(AnimationNewGame(server));
			m_PageTitle.GetComponent<Text>().text = Constants.NEW_GAME;
		}
	}
}
