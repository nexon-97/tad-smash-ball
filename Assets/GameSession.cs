using UnityEngine;

public enum GameState
{
	NotStarted,
	InProgress,
	Pause,
	Win,
	Defeat,
}

public class GameSession : MonoBehaviour
{
	public GameObject startGameHint;
	public GameObject pauseButton;
	public GameObject winNotification;
	public GameObject defeatNotification;
	public Transform platformsRootTransform;
	public Transform winTriggerTransform;
	public GameObject platformPrefab;
	public TMPro.TextMeshProUGUI scoreLabel;
	public int platformsCount = 10;
	public float distanceBetweenPlatforms = 1.5f;

	private GameState _gameState;
	private int _score;

	public GameState GameState
	{
		get { return _gameState; } 
	}

	public int Score
	{
		get { return _score; }
	}

	private static GameSession instance;

	public static GameSession Get()
	{
		return instance;
	}

	public void TogglePause()
	{
		_gameState = (_gameState == GameState.Pause) ? GameState.InProgress : GameState.Pause;
	}

	public void OnFinish()
	{
		_gameState = GameState.Win;

		// Show win notification
		winNotification.SetActive(true);
		pauseButton.SetActive(false);
	}

	public void AddScore(int score)
	{
		_score += score;
		scoreLabel.text = string.Format("Score: {0}", _score);
	}
	public void QuitGame()
	{
		Application.Quit();
	}

	void Start()
	{
		instance = this;
		_gameState = GameState.NotStarted;

		startGameHint.SetActive(true);
		pauseButton.SetActive(false);
		winNotification.SetActive(false);
		defeatNotification.SetActive(false);

		// Generate platforms for the game
		for (int i = 0; i < platformsCount; i++)
		{
			GameObject platform = Instantiate(platformPrefab, platformsRootTransform);
			platform.transform.position = new Vector3(0, i * -distanceBetweenPlatforms, 0);
			PlatformMesh mesh = platform.GetComponent<PlatformMesh>();
			
			int holeSize = 12;
			float rangeMax = (mesh.segments - holeSize);
			int offset = (int)(Random.value * rangeMax);
			mesh.holes = new PlatformMesh.HoleDef[] {
				new PlatformMesh.HoleDef { offset = offset, size = holeSize }
			};
		}

		// Move finish trigger below lowest platform
		winTriggerTransform.position = new Vector3(0, platformsCount * -distanceBetweenPlatforms, 0);

		_score = 0;
	}

	void Update()
	{
		// Handle start
		if (_gameState == GameState.NotStarted)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				_gameState = GameState.InProgress;

				startGameHint.SetActive(false);
				pauseButton.SetActive(true);
			}
		}
	}
}
