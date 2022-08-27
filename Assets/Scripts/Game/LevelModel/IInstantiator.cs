using UnityEngine;

namespace Assets.Scripts.Game.LevelModel
{
    interface IInstantiator
    {
        public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion quaternion);

        public void DestroyObject(GameObject gameObject);
    }
}
