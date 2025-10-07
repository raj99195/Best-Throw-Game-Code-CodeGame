using UnityEngine;

public class ScoreReceiveVFXManeger : MonoBehaviour
{
    public static ScoreReceiveVFXManeger Instance;
    [SerializeField] GameObject ScoreReceiveVFXPrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }
   
    public void ShowNewScoreReceiveVFX(Transform target, Color ballColor, int score, int excellentNumber, int jumpNumber)
    {
        GameObject g  = Instantiate(ScoreReceiveVFXPrefab, transform);
        g.GetComponent<ScoreReceiveVFX>().ShowScoreReceiveVFX(target,ballColor,score,excellentNumber,jumpNumber);
    }
}
