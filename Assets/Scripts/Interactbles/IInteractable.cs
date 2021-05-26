using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Interactable
{
    public interface IInteractable
    {
        void StartInteraction();
        void CancelInteraction();
        Vector3 Position { get; }
        bool IsInteractable { get; set; }
        float InteractionTime { get; }
        Sprite ButtonIcon { get; }
        InteractType InteractType { get; }
        #region Events
        UnityEventForRefrence OnInteraction { get; }
        UnityEventForRefrence OnInteractionStart { get; }
        UnityEventForRefrence OnInteractionFinish { get; }
        UnityEventForRefrence OnInteractionCancel { get; }
        UnityEventForRefrence OnInteractable { get; }
        UnityEventForRefrence OnUninteractable { get; }
        #endregion
    }
    public static class Interactables
    {
        public static List<IInteractable> available = new List<IInteractable>();
        public static InteractableHit GetClosest(Vector3 pos)
        {
            IInteractable closest = default;
            float distance = Mathf.Infinity;

            available.ForEach(
                (x)
                =>
                {
                    float tempDist;
                    if (x.IsInteractable && (tempDist = Vector3.Distance(pos, x.Position)) < distance)
                    {
                        closest = x;
                        distance = tempDist;
                    }
                }
                );
            return new InteractableHit(closest, distance);
        }
    }
    public class InteractableHit
    {
        public IInteractable interactable;
        public float distance;

        public InteractableHit(IInteractable interactable, float distance)
        {
            this.interactable = interactable;
            this.distance = distance;
        }
        public void UpdateDistance(Vector3 pos)
            => distance = Vector3.Distance(pos, interactable.Position);

        public static InteractableHit Clean => new InteractableHit(null, Mathf.Infinity);
    }
}
