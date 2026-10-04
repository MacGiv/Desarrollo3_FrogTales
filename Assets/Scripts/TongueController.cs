namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Controls tongue rendering, raycasting, and object interaction.
    /// Maneja el renderizado de la lengua (cuerpo + punta), raycasting e interacciones.
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

        [Header("References & Visuals")]
        [SerializeField] private Transform tongueOrigin;
        [SerializeField] private Transform tongueTip; // Transform / GameObject de la punta de la lengua

        private LineRenderer lineRenderer;

        public enum TongueHitResult { None, Grapple, Grabbable }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            SetVisualsActive(false);
        }

        /// <summary>
        /// Shoots the tongue out and retracts it. Calls callback on completion.
        /// </summary>
        public IEnumerator ShootTongueRoutine(Vector2 direction, System.Action<TongueHitResult, Vector2> onComplete)
        {
            SetVisualsActive(true);

            Vector2 origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
            Vector2 targetPoint = origin + direction * maxDistance;
            TongueHitResult hitResult = TongueHitResult.None;
            Vector2 hitPosition = targetPoint;

            // Perform Raycast to check for obstructions or targets
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

            // Calcular ángulo de rotación para orientar la punta de la lengua
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Phase 1: Extend Tongue / Extensión de la lengua
            float currentDist = 0f;
            float totalDist = Vector2.Distance(origin, hitPosition);

            while (currentDist < totalDist)
            {
                currentDist += extendSpeed * Time.deltaTime;
                Vector2 currentTipPos = Vector2.MoveTowards(origin, hitPosition, currentDist);

                UpdateTongueVisuals(origin, currentTipPos, angle);

                yield return null;
                origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
            }

            // Asegurar posición exacta en el punto de impacto
            UpdateTongueVisuals(origin, hitPosition, angle);

            // Phase 2: Retract Tongue (Only if not grappling) / Retracción (si no se agarra)
            if (hitResult != TongueHitResult.Grapple)
            {
                while (currentDist > 0f)
                {
                    currentDist -= retractSpeed * Time.deltaTime;
                    Vector2 currentTipPos = Vector2.MoveTowards(origin, hitPosition, currentDist);

                    UpdateTongueVisuals(origin, currentTipPos, angle);

                    yield return null;
                    origin = tongueOrigin != null ? (Vector2)tongueOrigin.position : (Vector2)transform.position;
                }
            }

            SetVisualsActive(false);
            onComplete?.Invoke(hitResult, hitPosition);
        }

        private void UpdateTongueVisuals(Vector2 origin, Vector2 tipPos, float angle)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, tipPos);

            if (tongueTip != null)
            {
                tongueTip.position = tipPos;
                tongueTip.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void SetVisualsActive(bool active)
        {
            lineRenderer.enabled = active;
            if (tongueTip != null)
            {
                tongueTip.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Handles interaction with grabbable items or enemy shields.
        /// </summary>
        private void HandleGrabbableHit(Collider2D targetCollider)
        {
            GrabbableItem grabbable = targetCollider.GetComponent<GrabbableItem>();
            if (grabbable != null)
            {
                grabbable.Grab(transform);
            }
        }

        public void DisableLine() => SetVisualsActive(false);
    }
}