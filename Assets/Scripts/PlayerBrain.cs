using UnityEngine;
using FrogGame.Core;

namespace FrogGame.Gameplay
{
    /// <summary>
    /// Central context for Player FSM and components.
    /// Contexto central para la FSM y componentes del jugador.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovementHandler))]
    public class PlayerBrain : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] public Transform arrowSpawnPoint;
        public FiniteStateMachine FSM { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerMovementHandler MovementHandler { get; private set; }
        public TongueController TongueController { get; private set; }
        // States instances / Instancias de Estados
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerShootTongueState TongueState { get; private set; }
        public PlayerArrowAttackState ArrowState { get; private set; }
        public PlayerGrappleState GrappleState { get; private set; }

        public GameObject ArrowPrefab => arrowPrefab;

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            MovementHandler = GetComponent<PlayerMovementHandler>();
            TongueController = GetComponent<TongueController>();

            FSM = new FiniteStateMachine();

            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            TongueState = new PlayerShootTongueState(this);
            ArrowState = new PlayerArrowAttackState(this);
            GrappleState = new PlayerGrappleState(this);
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