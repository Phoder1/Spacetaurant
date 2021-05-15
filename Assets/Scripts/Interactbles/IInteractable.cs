using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Interactable
{
    public interface IInteractable
    {
        void StartInteraction();
        void EndInteraction();
        Vector3 Position { get; }
        bool Interactable { get; set; }
        float InteractionTime { get; }
        Sprite ButtonIcon { get; }
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
                    if ((tempDist = Vector3.Distance(pos, x.Position)) < distance)
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
    }
}
