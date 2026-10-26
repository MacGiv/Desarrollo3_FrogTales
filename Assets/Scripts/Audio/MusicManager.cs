using UnityEngine;
using AK.Wwise;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Event")]
    [SerializeField] private AK.Wwise.Event playMX;

    [Header("Location / Context States")]
    [SerializeField] private AK.Wwise.State gameplayState;
    [SerializeField] private AK.Wwise.State shopState;
    [SerializeField] private AK.Wwise.State menuState;
    [SerializeField] private AK.Wwise.State dungeonState;
    [SerializeField] private AK.Wwise.State bossFightState;

    // ID numérico de reproducción (0 representa una instancia no válida o inactiva)
    private uint musicPlayingId = 0u;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayMusic();

        if (gameplayState != null && gameplayState.IsValid())
        {
            SetGameplay();
        }
    }

    #region Play / Stop
    public void PlayMusic()
    {
        // Si la música ya tiene un ID de reproducción activo (> 0), evitamos volver a dispararla
        if (musicPlayingId != 0u)
        {
            Debug.LogWarning($"[MusicManager] Se intentó disparar PlayMusic(), pero la música ya está activa (ID: {musicPlayingId}).", gameObject);
            return;
        }

        if (playMX != null && playMX.IsValid())
        {
            // Post() devuelve un uint > 0 si el evento se ejecutó correctamente
            musicPlayingId = playMX.Post(gameObject);

            if (musicPlayingId != 0u)
            {
                Debug.Log($"[MusicManager] Música iniciada con Playing ID: {musicPlayingId}");
            }
        }
    }

    public void StopMusic()
    {
        if (playMX != null && playMX.IsValid())
        {
            playMX.Stop(gameObject);
            musicPlayingId = 0u;
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

    public void SetGameplay() => SetState(gameplayState);
    public void SetShop() => SetState(shopState);
    public void SetMenu() => SetState(menuState);
    public void SetDungeon() => SetState(dungeonState);
    public void SetBossFight() => SetState(bossFightState);
    #endregion
}