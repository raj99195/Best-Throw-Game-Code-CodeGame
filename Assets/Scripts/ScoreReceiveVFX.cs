using UnityEngine;
using UnityEngine.UI;

public class ScoreReceiveVFX : MonoBehaviour
{
    [SerializeField] Text _totalReceiveScoreTxt;
    [SerializeField] Text _excellentTxt;
    [SerializeField] Text _jumpTxt;
    [SerializeField] Image _ringVfxImg;
    Canvas _scoreVFXCanvas;

    Transform _target;
    RectTransform _rectTransform;
    Animator _animator;
    Camera _cam;

    bool _canShowingScoreVFX;
    Vector2 _uiScreenSize;
    Vector2 _posInUi;

    private void Awake()
    {
        _scoreVFXCanvas = transform.parent.GetComponent<Canvas>();
        _animator = GetComponent<Animator>();
        _rectTransform = GetComponent<RectTransform>();
        _cam = Camera.main;
    }

    private void Start()
    {
        _uiScreenSize = _scoreVFXCanvas.GetComponent<RectTransform>().sizeDelta;
    }

    private void FixedUpdate()
    {
        if (_canShowingScoreVFX)
            setPosition();
    }

    /// <summary>
    /// Displays notifications and VFXs when the ball entry the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ballColor"></param>
    /// <param name="score"></param>
    /// <param name="excellentNumber"></param>
    /// <param name="jumpNumber"></param>
    public void ShowScoreReceiveVFX(Transform target, Color ballColor, int score, int excellentNumber, int jumpNumber)
    {
        this._target = target;
        setPosition();
        _canShowingScoreVFX = true;

        _ringVfxImg.color = ballColor;

        _totalReceiveScoreTxt.text = "+" + score;
        string triggerName = "";

        if (score == 1)
        {
            triggerName = "Totall score receive (show)";

        }
        else if (score > 1)
        {
            if (excellentNumber == 1)
                _excellentTxt.text = "Excellent!";
            else if (excellentNumber > 1)
                _excellentTxt.text = "Excellent × " + excellentNumber;

            if (jumpNumber == 1)
                _jumpTxt.text = "Jump!";
            else if (jumpNumber > 1)
                _jumpTxt.text = "Jump × " + jumpNumber;


            if (excellentNumber > 0 && jumpNumber > 0)
            {
                triggerName = "Totall score receive + Excellent  + Jump (show)";
            }
            else if (excellentNumber > 0 && jumpNumber == 0)
            {
                triggerName = "Totall score receive + Excellent (show)";
            }
            else if (excellentNumber == 0 && jumpNumber > 0)
            {
                triggerName = "Totall score receive + Jump (show)";
            }
        }

        _animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// Set the position of the "Score Receive VFX gameobject" relative to the target position in the UI
    /// </summary>
    void setPosition()
    {
        _posInUi = _cam.WorldToViewportPoint(_target.transform.position);
        _posInUi *= _uiScreenSize;
        _rectTransform.anchoredPosition = _posInUi;
    }

    // Called in animation events -------------------------------------
    void StopShowingScoreVFX()
    {
        Destroy(gameObject);
    }

    void PlayTotalVFXSound()
    {
        AudioAndVibrationManeger.instance.play("Total VFX Score Receive");
    }

    void PlayExcellentVFXSound()
    {
        AudioAndVibrationManeger.instance.play("Excellent VFX Score Receive");
    }

    void PlayJumpVFXSound()
    {
        AudioAndVibrationManeger.instance.play("jump VFX Score Receive");
    }
    // ------------------------------------------------------------------
}
