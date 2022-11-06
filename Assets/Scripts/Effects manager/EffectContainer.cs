using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace ProjectCore.Effects
{
    [Serializable]
    public class EffectContainer
    {
        private Timer _timer;
        private IBonusEffect _effect;
        private Action<EffectContainer> _callback;
        public  EffectContainer(IBonusEffect bonusEffect)
        {
           
            _effect = bonusEffect;
            _timer = new Timer();
            _timer.Interval = _effect.EffectTime * 1000;
            _timer.Elapsed += TimerEndHandler;
            _timer.AutoReset = false;


        }

        public void Start(Action<EffectContainer> action)
        {
            _callback = action;
            _effect.Enable(true);
            _timer.Enabled = true;            
            Debug.Log("Start");
        }

        public void End()
        {
            _effect.Enable(false);
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = null;
            _callback.Invoke(this);
        }      

        private void TimerEndHandler(object sender, ElapsedEventArgs e)
        {
            End();
        }
        
        ~EffectContainer()
        {
            _timer.Stop();
            _timer.Elapsed -= TimerEndHandler;
            _timer.Dispose();
            _timer = null;
        }

    }
}
