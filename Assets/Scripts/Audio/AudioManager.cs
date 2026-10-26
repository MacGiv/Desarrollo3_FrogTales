using UnityEngine;
using AK.Wwise;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Main SoundBank")]
    [SerializeField] private AK.Wwise.Bank mainBank;

    [Header("Enemy Events")]
    [SerializeField] private AK.Wwise.Event playEnemyArrowHit;
    [SerializeField] private AK.Wwise.Event playEnemyBoogerHit;
    [SerializeField] private AK.Wwise.Event playEnemyCombatAlertFox;
    [SerializeField] private AK.Wwise.Event playEnemyCombatAlertOwl;
    [SerializeField] private AK.Wwise.Event playEnemyDamage;
    [SerializeField] private AK.Wwise.Event playEnemyDead;

    [Header("Environment Events")]
    [SerializeField] private AK.Wwise.Event playEnvDoorsOpen;
    [SerializeField] private AK.Wwise.Event playEnvFlyHealth;
    [SerializeField] private AK.Wwise.Event playEnvItemsPickup;
    [SerializeField] private AK.Wwise.Event playEnvRockDrop;
    [SerializeField] private AK.Wwise.Event playEnvRockPushButton;

    [Header("Player Events")]
    [SerializeField] private AK.Wwise.Event playPlayerBoogerHit;
    [SerializeField] private AK.Wwise.Event playPlayerBoogerShot;
    [SerializeField] private AK.Wwise.Event playPlayerBowHit;
    [SerializeField] private AK.Wwise.Event playPlayerBowShot;
    [SerializeField] private AK.Wwise.Event playPlayerDamage;
    [SerializeField] private AK.Wwise.Event playPlayerFallWater;
    [SerializeField] private AK.Wwise.Event playPlayerTongueHit;
    [SerializeField] private AK.Wwise.Event playPlayerTongueShot;

    [Header("Testing")]
    [SerializeField] private AK.Wwise.Event playTestSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (mainBank != null && mainBank.IsValid())
        {
            mainBank.Load();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this && mainBank != null && mainBank.IsValid())
        {
            mainBank.Unload();
        }
    }

    #region Generic Helper
    public void PostEvent(AK.Wwise.Event akEvent, GameObject emitter = null)
    {
        if (akEvent != null && akEvent.IsValid())
        {
            akEvent.Post(emitter != null ? emitter : gameObject);
        }
    }
    #endregion

    #region Play Methods
    public void PlayEnemyArrowHit(GameObject emitter = null) => PostEvent(playEnemyArrowHit, emitter);
    public void PlayEnemyBoogerHit(GameObject emitter = null) => PostEvent(playEnemyBoogerHit, emitter);
    public void PlayEnemyCombatAlertFox(GameObject emitter = null) => PostEvent(playEnemyCombatAlertFox, emitter);
    public void PlayEnemyCombatAlertOwl(GameObject emitter = null) => PostEvent(playEnemyCombatAlertOwl, emitter);
    public void PlayEnemyDamage(GameObject emitter = null) => PostEvent(playEnemyDamage, emitter);
    public void PlayEnemyDead(GameObject emitter = null) => PostEvent(playEnemyDead, emitter);

    public void PlayEnvDoorsOpen(GameObject emitter = null) => PostEvent(playEnvDoorsOpen, emitter);
    public void PlayEnvFlyHealth(GameObject emitter = null) => PostEvent(playEnvFlyHealth, emitter);
    public void PlayEnvItemsPickup(GameObject emitter = null) => PostEvent(playEnvItemsPickup, emitter);
    public void PlayEnvRockDrop(GameObject emitter = null) => PostEvent(playEnvRockDrop, emitter);
    public void PlayEnvRockPushButton(GameObject emitter = null) => PostEvent(playEnvRockPushButton, emitter);

    public void PlayPlayerBoogerHit(GameObject emitter = null) => PostEvent(playPlayerBoogerHit, emitter);
    public void PlayPlayerBoogerShot(GameObject emitter = null) => PostEvent(playPlayerBoogerShot, emitter);
    public void PlayPlayerBowHit(GameObject emitter = null) => PostEvent(playPlayerBowHit, emitter);
    public void PlayPlayerBowShot(GameObject emitter = null) => PostEvent(playPlayerBowShot, emitter);
    public void PlayPlayerDamage(GameObject emitter = null) => PostEvent(playPlayerDamage, emitter);
    public void PlayPlayerFallWater(GameObject emitter = null) => PostEvent(playPlayerFallWater, emitter);
    public void PlayPlayerTongueHit(GameObject emitter = null) => PostEvent(playPlayerTongueHit, emitter);
    public void PlayPlayerTongueShot(GameObject emitter = null) => PostEvent(playPlayerTongueShot, emitter);

    public void PlayTestSound(GameObject emitter = null) => PostEvent(playTestSound, emitter);
    #endregion
}