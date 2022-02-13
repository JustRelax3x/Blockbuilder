using UnityEngine;
namespace Assets.Scripts.Game
{
    class ParticleHandler : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _smoke;
        [SerializeField]
        private ParticleSystem _star;


        public void SetSuccessParticle(Vector3 position, Color color)
        {
            _star.gameObject.transform.localPosition = position;
            _star.startColor = color;
            _star.Play();
        }

        public void SetFailedParticle(Vector3 position)
        {
            _smoke.gameObject.transform.localPosition = position;
            _smoke.Play();
        }
    }
}
