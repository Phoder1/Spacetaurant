using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Spacetaurant
{
    public class UpgradeButton : UiButton<UpgradeSO> 
    {
        public UpgradeSO UpgradeSO => Content;
    }
}
