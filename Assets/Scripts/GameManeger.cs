using UnityEngine;

public class GameManeger : MonoBehaviour
{
    [Header("Ball and Throw setting")]
    [SerializeField] Ball _ballCs;
    [SerializeField] float _pushforce; // push force to ball
    [SerializeField] float _ratioMoveBallInTrajectory;
    Vector2 _startPoint, _endPoint, _dirction, _force;
    float _distance;
    [SerializeField] float _maxDistance, _minDistance;

    [Header("Trajectory")]
    [SerializeField] Trajectory _trajectory;
    float _colorAlphaOfTrajectory;
    Vector3 _positionOfMoveBallAtTrajectory = new Vector3(0, 0, 0);

    [SerializeField] GameObject _dragBtn; // for draging in game

    [SerializeField] CircleDragingEfect _circledragingEfectCs; // A ring that changes size when dragged.

    int _score = 0;
    int _bestScore = 0;
    bool _canPlayBestRecordEfect = true;  // for play best record efect 
    const string BEST_SCORE = "Best score";

    int _numberLevel = 0;

    [HideInInspector] public bool isDraging = false;

    Camera _mainCamera;
    CameraManeger _cameraManeger;

    public static GameManeger Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        loadPrefs();
    }

    /// <summary>
    /// Get the best score from memory
    /// </summary>
    void loadPrefs()
    {
        if (PlayerPrefs.HasKey(BEST_SCORE))
        {
            this._bestScore = PlayerPrefs.GetInt(BEST_SCORE);
        }
        else
        {
            _canPlayBestRecordEfect = false;
        }

    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _cameraManeger = _mainCamera.GetComponent<CameraManeger>();
        _mainCamera.transform.position = new Vector3(0, 0, -10);
        _ballCs.transform.position = new Vector2(LevelDesigner.Instance.firstPositionOfStation.x, 12f);
        _ballCs.SetActiveRb(false);
        _ballCs.ballTrailer.Play();

        UIManeger.instance.ShowFirstStartMenu();
        this._score = 0;
        UIManeger.instance.SetScoreText(0);

    }

    /// <summary>
    /// Called in Animation Event. To display game objects after displaying the animation of name at the beginning of the game entry
    /// </summary>
    public void StartOnDeley()
    {
        LevelDesigner.Instance.ShowTargetAndStation();
        _ballCs.SetActiveRb(true);
        AudioAndVibrationManeger.instance.play("Menu background music");
    }

    /// <summary>
    /// Loss in the game
    /// </summary>
    public void Loss()
    {   
        _cameraManeger.SetCanFollowBall(false);

        AudioAndVibrationManeger.instance.play("Loss");

        if (_numberLevel == 0)
        {
            _ballCs.ResetOptions(LevelDesigner.Instance.station.transform.position);
            _cameraManeger.SetCanFollowBall(true);
            return;
        }

        if (AdsManeger.instance.isRewardAdLoaded && AdsManeger.instance.numberOfCanShowRewardAdAfterEveryLoss > 0)     // in display the menu has a reward ads to be able to continue the game. Of course, if the value [numberOfCanShowRewardAdAfterEveryLoss] is not equal to zero
        {
            AdsManeger.instance.numberOfCanShowRewardAdAfterEveryLoss--;
          UIManeger.instance.ShowLossMenuWithContinue(_score, _bestScore);
        }
        else
        {
            //if (AdsManeger.instance.isInterstitialAdsLoaded)
            //{
            //    AdsManeger.instance.ShowInterstitalAd();
            //}
            UIManeger.instance.ShowLossMenu(_score, _bestScore);
        }
    }

    /// <summary>
    /// Pause playing
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        UIManeger.instance.ShowPauseMenu();
    }

    /// <summary>
    /// Continue playing after the pause
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
    }

    /// <summary>
    /// Continue the game by watching the reward ad after losing
    /// </summary>
    public void ContinueAfterLoss()   
    {
        _ballCs.GetComponent<Ball>().ResetOptions(LevelDesigner.Instance.station.transform.position);
        _cameraManeger.SetPositionRelativeToStationPosition(LevelDesigner.Instance.station.transform.position);
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public void Restart()
    {
        if (Time.timeScale == 0) Time.timeScale = 1;  // when click restart btn in pause menu
        if (AdsManeger.instance.isInterstitialAdsLoaded)
        {
            AdsManeger.instance.ShowInterstitalAd();
        }
        _numberLevel = 0;
        this._score = 0;
        _canPlayBestRecordEfect = true;
        UIManeger.instance.SetScoreText(0);

        LevelDesigner.Instance.ResetToStartLevel();
        LevelDesigner.Instance.ShowTargetAndStation();
        AudioAndVibrationManeger.instance.play("Menu background music");
        _cameraManeger.SetCanFollowBall(false);
        _mainCamera.transform.position = new Vector3(0, 0, -10);

        // Ads ----------------------------------------------------------
#if (UNITY_ANDROID && !UNITY_EDITOR )
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            AdsManeger adsManeger = AdsManeger.instance;
            if (adsManeger.enableRewardAd)
            {
                adsManeger.ResetNumberOfCanShowRewardVideoAfterEveryLoss();

                if (!adsManeger.isRewardAdLoaded)
                    adsManeger.RequestRewardAd();
            }
            if (adsManeger.enableInterstitial && !adsManeger.isInterstitialAdsLoaded)
                adsManeger.RequestInterstitialAd();
            if (adsManeger.enableNativeAd && !adsManeger.isNativeLoaded)
                adsManeger.RequestNativeAd();
        }
#endif
    }

    private void Update()
    {
        if (isDraging)
            onDraging();
    }

    /// <summary>
    /// When the ball returns to the station
    /// </summary>
    public void ReturnedToStation()
    {
        Debug.Log("returned to station");
        if (UIManeger.instance.isStartMenuShowed)
            AudioAndVibrationManeger.instance.play("Ball Return to station");

        setCanDraw(true);
    }

    /// <summary>
    /// The ball hits the target correctly
    /// </summary>
    /// <param name="score">score received</param>
    public void SuccessfulThrow(int score)
    {
        Debug.Log("new point is :" + score);
        this._score += score;

        if (this._score > _bestScore)
        {
            if (_canPlayBestRecordEfect)
            {
                _canPlayBestRecordEfect = false;
                UIManeger.instance.PlayNewRecordVFX();
            }
            _bestScore = this._score;
            PlayerPrefs.SetInt(BEST_SCORE, _bestScore);
            PlayerPrefs.Save();
        }

        _numberLevel++;
        UIManeger.instance.SetScoreText(this._score);
        Debug.Log("score is : " + this._score);

        LevelDesigner.Instance.GoToNextLevel(this._score);
        setCanDraw(true);
    }

    /// <summary>
    /// Activates the drag button
    /// </summary>
    /// <param name="canDraw"></param>
    void setCanDraw(bool canDraw)
    {
        _dragBtn.SetActive(canDraw);
    }

    /// <summary>
    /// By drag Button (pointer down event)
    /// </summary>
    public void OnDragStart()
    {
        if (UIManeger.instance.isStartMenuShowed)
        {
            UIManeger.instance.HideStartMenu();
            AudioAndVibrationManeger.instance.stop("Menu background music");
        }

        _ballCs.SetActiveRb(false);
        _startPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _trajectory.Show();
        _circledragingEfectCs.ShowDragingEfect(LevelDesigner.Instance.station.transform.position);
        isDraging = true;
    }


    void onDraging()
    {
        _endPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _dirction = (_startPoint - _endPoint).normalized;
        _distance = Vector2.Distance(_startPoint, _endPoint);

        if (_distance > _maxDistance)
        {
            _distance = _maxDistance;
        }

        _force = _dirction * _distance * _pushforce;

        _positionOfMoveBallAtTrajectory.x = _distance * _dirction.x * _ratioMoveBallInTrajectory;
        _positionOfMoveBallAtTrajectory.y = _distance * _dirction.y * _ratioMoveBallInTrajectory;
        _ballCs.transform.position = LevelDesigner.Instance.station.transform.position - _positionOfMoveBallAtTrajectory;

        if (_distance > _minDistance)
        {
            _colorAlphaOfTrajectory = (_distance - _minDistance) / (_maxDistance - _minDistance);
        }
        else
        {
            if (_colorAlphaOfTrajectory != 0)
                _colorAlphaOfTrajectory = 0;
        }

        _trajectory.UpdateDots(_ballCs.humanPos, _force, _colorAlphaOfTrajectory);
        _circledragingEfectCs.UpdateDragingEfect(_colorAlphaOfTrajectory);
    }

    /// <summary>
    /// By drag Button (pointer up event)
    /// </summary>
    public void OnDragEnd()
    {
        isDraging = false;
        _trajectory.Hide();
        _circledragingEfectCs.HideDragingEfect();
        if (_distance > _minDistance)
        {
            LevelDesigner.Instance.station.tag = LevelDesigner.SHOOTER_TAG;
            _ballCs.SetActiveRb(true);
            _ballCs.Add_force(_force);
            setCanDraw(false);

            AudioAndVibrationManeger.instance.play("Throwing");
        }
        else
        {
            _ballCs.canMoveToTarget = true;
        }
        _cameraManeger.SetCanFollowBall(true);
    }
}
