using UnityEngine;
namespace Assets.Scripts.Game.LevelModel
{
    class ParticleHandler : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _smoke;
        [SerializeField]
        private ParticleSystem _star;

        private ParticleSystem.MainModule _starModule;

        private void OnEnable()
        {
            _starModule = _star.main;
        }
        public void SetSuccessParticle(Vector3 position, Color color)
        {
            _star.gameObject.transform.localPosition = position;
            if (color == Color.black) color = new Color(0.2f,0.2f,0.2f);
            _starModule.startColor = color;
            _star.Play();
        }

        public void SetFailedParticle(Vector3 position)
        {
            _smoke.gameObject.transform.localPosition = position;
            _smoke.Play();
        }
    }
}
