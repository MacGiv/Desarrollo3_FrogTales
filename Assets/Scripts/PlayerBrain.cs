
namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Central context for Player FSM and components.
    /// Contexto central para la FSM y componentes del jugador.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovementHandler))]
    [RequireComponent(typeof(PlayerAmmoSystem))]
    public class PlayerBrain : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject stunPrefab;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private PlayerCarryHandler carryHandler;
        public FiniteStateMachine FSM { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerMovementHandler MovementHandler { get; private set; }
        public TongueController TongueController { get; private set; }
        public PlayerCarryHandler CarryHandler { get; private set; }
        public PlayerAmmoSystem AmmoSystem { get; private set; }

        // States instances / Instancias de Estados
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerShootTongueState TongueState { get; private set; }
        public PlayerArrowAttackState ArrowState { get; private set; }
        public PlayerGrappleState GrappleState { get; private set; }
        public PlayerStunAttackState StunAttackState { get; private set; }
        public Transform ArrowSpawnPoint => arrowSpawnPoint;
        public GameObject StunPrefab => stunPrefab;
        public GameObject ArrowPrefab => arrowPrefab;

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            MovementHandler = GetComponent<PlayerMovementHandler>();
            TongueController = GetComponent<TongueController>();
            CarryHandler = GetComponent<PlayerCarryHandler>();
            AmmoSystem = GetComponent<PlayerAmmoSystem>();

            FSM = new FiniteStateMachine();
            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            TongueState = new PlayerShootTongueState(this);
            ArrowState = new PlayerArrowAttackState(this);
            GrappleState = new PlayerGrappleState(this);
            StunAttackState = new PlayerStunAttackState(this);
        }

        private void Start()
        {
            FSM.Initialize(IdleState);
        }

        private void Update()
        {
            FSM.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            FSM.CurrentState.PhysicsUpdate();
        }
    }
}
