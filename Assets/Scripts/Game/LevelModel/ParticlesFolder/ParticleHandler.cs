using UnityEngine;

namespace Assets.Scripts.Game.LevelModel
{
    internal class ParticleHandler
    {
        private ParticleSystem _smoke;

        private ParticleSystem _star;

        private ParticleSystem.MainModule _starModule;

        public ParticleHandler(ParticleSystem smoke, ParticleSystem star)
        {
            _smoke = smoke;
            _star = star;
            _starModule = _star.main;
        }

        public void SetSuccessParticle(Vector3 position, Color color)
        {
            _star.gameObject.transform.localPosition = position;
            if (color == Color.black) color = new Color(0.2f, 0.2f, 0.2f);
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