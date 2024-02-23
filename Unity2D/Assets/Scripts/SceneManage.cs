using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    #region Singleton

    private static SceneManage Instance = null;
    private SceneManage() { }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] private AudioSource EffectPlayer;
    [SerializeField] private List<AudioClip> bgmClips;

    private AudioSource BgmPlayer;
    private SceneFSM sceneFSM;

    private void Start()
    {
        BgmPlayer = GetComponent<AudioSource>();
        sceneFSM = GetComponent<SceneFSM>();
        sceneFSM.SetState(SceneFSM.EState.Ttitle);
        sceneFSM.States[SceneFSM.EState.Ttitle].OnUpdateEvent += () =>
        {
            RenderSettings.skybox.SetFloat("_Rotation", 2 * Time.time);
        };

        sceneFSM.States[SceneFSM.EState.Gaming].OnEnterEvent += () =>
        {
            SceneManager.LoadScene("Stage");
            BgmPlayer.Pause();
        };
    }

    public static void SetState(SceneFSM.EState newState) => Instance.onSetState(newState);
    public void BtnStart()
    {
        sceneFSM.SetState(SceneFSM.EState.Gaming);
        EffectPlayer.Play();
    }

    private void onSetState(SceneFSM.EState newState)
    {
        sceneFSM.SetState(newState);
    }
}
