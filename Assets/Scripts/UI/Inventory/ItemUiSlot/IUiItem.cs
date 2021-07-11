namespace Spacetaurant
{
    public interface IUiItem<T>
    {
        void LoadItem(T itemSlot);
        void WasPressed();
    }
}