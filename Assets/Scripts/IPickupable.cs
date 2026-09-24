namespace FrogGame.Core
{
    using UnityEngine;

    /// <summary>
    /// Interface for items that can be collected either by walking over them or by tongue retrieval.
    /// </summary>
    public interface IPickupable
    {
        public bool Collect(GameObject collector);
    }
}
