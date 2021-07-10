using DataSaving;
using Spacetaurant.Restaurant;
using UnityEngine;

namespace Spacetaurant
{
    public class GameManager : MonoSingleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();

            DataHandler.Cache<LogOutTime>();
        }
        private void OnApplicationQuit()
        {
            DataHandler.SaveAll();
        }
    }
}
