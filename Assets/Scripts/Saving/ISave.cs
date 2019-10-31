using UnityEngine;

namespace Village.Saving
{
    public interface ISave<T>
    {
        T ToSaveObj();

        void FromSaveObj(T obj);
    }
}
