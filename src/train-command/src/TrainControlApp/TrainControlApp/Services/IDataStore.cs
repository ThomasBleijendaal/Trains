using System.Collections.Generic;

namespace TrainControlApp.Services
{
    public interface IDataStore<T>
    {
        bool UpdateItem(T item);
        IReadOnlyList<T> GetItems(bool forceRefresh = false);
    }
}
