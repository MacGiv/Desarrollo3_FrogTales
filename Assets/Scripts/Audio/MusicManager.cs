using UnityEngine;
using AK.Wwise;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Event")]
    [SerializeField] private AK.Wwise.Event playMX;

    [Header("Location / Context States")]
    [SerializeField] private AK.Wwise.State overworldState;
    [SerializeField] private AK.Wwise.State shopState;
    [SerializeField] private AK.Wwise.State menuState;
    [SerializeField] private AK.Wwise.State dungeonState;
    [SerializeField] private AK.Wwise.State bossFightState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Play / Stop
    public void PlayMusic()
    {
        if (playMX != null && playMX.IsValid())
        {
            playMX.Post(gameObject);
        }
    }

    public void StopMusic()
    {
        if (playMX != null && playMX.IsValid())
        {
            playMX.Stop(gameObject);
        }
    }
    #endregion

    #region State Changes
    public void SetState(AK.Wwise.State targetState)
    {
        if (targetState != null && targetState.IsValid())
        {
            targetState.SetValue();
        }
    }

    public void SetOverworld() => SetState(overworldState);
    public void SetShop() => SetState(shopState);
    public void SetMenu() => SetState(menuState);
    public void SetDungeon() => SetState(dungeonState);
    public void SetBossFight() => SetState(bossFightState);
    #endregion
}