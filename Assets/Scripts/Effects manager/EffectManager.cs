using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCore.Effects
{
    public class EffectManager : MonoBehaviour
    {
        private int _lastCollidePlayerIndex;
        private List<EffectContainer> _effects = new List<EffectContainer>();

        public void TakeBonus(Transform bonus)
        {
            var bonusEffect = bonus.GetComponent<IBonusEffect>();
            if(bonusEffect != null)
                TakeBonus(bonusEffect);            
        }

        public void TakeBonus(IBonusEffect bonusEffect)
        {           
            var container = new EffectContainer(bonusEffect);
            _effects.Add(container);
            container.Start(RemoveEffect);
        }       

        public void StopEffects()
        {
            _effects.ForEach(b => b.End());
        }

        private void RemoveEffect(EffectContainer bonusEffect)
        {
            _effects.Remove(bonusEffect);
        }

        private void OnDestroy()
        {
            _effects.ForEach(x => x.End());
            _effects.Clear();
        }
    }
}
