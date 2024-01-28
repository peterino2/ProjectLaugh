using UnityEngine;

namespace ScriptableObjectArchitecture.Examples
{
    public class DamageDealerWithEvent : DamageDealer
    {
        [SerializeField]
        private GameEvent _onDamagedEvent = default(GameEvent);

        [SerializeField]
        private AudioClipGameEvent _onDamagedAudioEvent = default(AudioClipGameEvent);

        protected override void DealDamage(UnitHealth target)
        {
            base.DealDamage(target);

            _onDamagedEvent.Raise();

            _onDamagedAudioEvent.Raise();
        }
    }
}