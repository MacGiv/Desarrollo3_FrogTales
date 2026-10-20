namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Destroys the visual effect GameObject automatically once its Animator animation finishes.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AutoDestroyFX : MonoBehaviour
    {
        private void Start()
        {
            Animator animator = GetComponent<Animator>();

            animator.Update(0f);
            float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

            Destroy(gameObject, animLength);
        }
    }
}