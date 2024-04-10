using UnityEngine;

/// <summary>
/// CriAudioManagerの使用例
/// </summary>
public class ExampleCriAudioManager : MonoBehaviour
{
    [Header("キューの名前 BGM"), SerializeField] string _cueName_BGM = default;
    [Header("キューの名前 SE"), SerializeField] string _cueName_SE = default;
    [Header("キューの名前2 SE"), SerializeField] string _cueName2_SE = default;
    
    void Start()
    {
        CriAudioManager.Instance.PlayBGM(CriAudioManager.CueSheetType.BGM, _cueName_BGM);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        { 
            CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheetType.SE, _cueName_SE);
            Debug.Log($"Play  {_cueName_SE}");
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheetType.SE, _cueName2_SE);
            Debug.Log($"Play  {_cueName2_SE}");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CriAudioManager.Instance.PauseBGM();
            Debug.Log("PauseBGM");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            CriAudioManager.Instance.ResumeBGM();
            Debug.Log("ResumeBGM");
        }
    }
}