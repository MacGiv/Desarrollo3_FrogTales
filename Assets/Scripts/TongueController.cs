namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Controls tongue rendering, raycasting, and object interaction.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class TongueController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxDistance = 5.0f;
        [SerializeField] private float extendSpeed = 25.0f;
        [SerializeField] private float retractSpeed = 30.0f;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LayerMask grabbableLayer;
        [SerializeField] private LayerMask wallLayer;

        [Header("References")]
        [SerializeField] private Transform tongueOrigin;

        private LineRenderer lineRenderer;

        public enum TongueHitResult { None, Grapple, Grabbable }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        /// <summary>
        /// Shoots the tongue out and retracts it. Calls callback on completion.
        /// </summary>
        public IEnumerator ShootTongueRoutine(Vector2 direction, System.Action<TongueHitResult, Vector2> onComplete)
        {
            lineRenderer.enabled = true;

            Vector2 origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
            Vector2 targetPoint = origin + direction * maxDistance;
            TongueHitResult hitResult = TongueHitResult.None;
            Vector2 hitPosition = targetPoint;

            // Perform Raycast to check for obstructions or targets
            // Realizar Raycast para detectar impactos
            LayerMask combinedMask = grappleLayer | grabbableLayer | wallLayer;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, combinedMask);

            if (hit.collider != null)
            {
                hitPosition = hit.point;
                int hitLayer = 1 << hit.collider.gameObject.layer;

                if ((hitLayer & grappleLayer) != 0)
                {
                    hitResult = TongueHitResult.Grapple;
                }
                else if ((hitLayer & grabbableLayer) != 0)
                {
                    hitResult = TongueHitResult.Grabbable;
                    HandleGrabbableHit(hit.collider);
                }
            }

            // Phase 1: Extend Tongue / Extensión de la lengua
            float currentDist = 0f;
            float totalDist = Vector2.Distance(origin, hitPosition);

            while (currentDist < totalDist)
            {
                currentDist += extendSpeed * Time.deltaTime;
                Vector2 currentTipPos = Vector2.MoveTowards(origin, hitPosition, currentDist);

                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, currentTipPos);

                yield return null;
                origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
            }

            // Phase 2: Retract Tongue (Only if not grappling) / Retracción (si no se agarra)
            if (hitResult != TongueHitResult.Grapple)
            {
                while (currentDist > 0f)
                {
                    currentDist -= retractSpeed * Time.deltaTime;
                    Vector2 currentTipPos = Vector2.MoveTowards(origin, hitPosition, currentDist);

                    lineRenderer.SetPosition(0, origin);
                    lineRenderer.SetPosition(1, currentTipPos);

                    yield return null;
                    origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
                }
            }

            lineRenderer.enabled = false;
            onComplete?.Invoke(hitResult, hitPosition);
        }

        /// <summary>
        /// Handles interaction with grabbable items or enemy shields.
        /// Interacción con ítems atraíbles o escudos.
        /// </summary>
        private void HandleGrabbableHit(Collider2D targetCollider)
        {
            // 1. Check for enemy shield / Chequear si es un escudo
            GrabbableItem grabbable = targetCollider.GetComponent<GrabbableItem>();
            if (grabbable != null)
            {
                grabbable.Grab(transform);
                return;
            }

            // 2. Check for generic enemy shield script if independent
            // EnemyShield shield = targetCollider.GetComponent<EnemyShield>();
            // if (shield != null) { shield.DetachShield(); }
        }

        public void DisableLine() => lineRenderer.enabled = false;
    }
}