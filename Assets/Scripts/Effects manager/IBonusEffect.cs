using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace ProjectCore.Effects
{
    public interface IBonusEffect
    {
        void Enable(bool enable);  
        float EffectTime { get; }
    }
}
