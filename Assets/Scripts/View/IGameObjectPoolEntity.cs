using UnityEngine;

namespace Cytus2
{
    public interface IGameObjectPoolEntity
    {
        GameObject gameObject { get; }

        void Despawn();
    }
}