using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant
{
    public class UpgradeInfo : InfoLoader<UpgradeSO>
    {
        [SerializeField, FoldoutGroup("Refrences")]
        private RawImage _cameraRenderer;
        [SerializeField, FoldoutGroup("Refrences")]
        private TextMeshProUGUI _name;
        [SerializeField, FoldoutGroup("Refrences")]
        private Image _icon;
        [SerializeField, FoldoutGroup("Refrences")]
        private TextMeshProUGUI _description;

        public override void Load(UpgradeSO info)
        {
            var decoration = info as DecorationSO;
            if (_cameraRenderer != null)
                _cameraRenderer.transform.parent.gameObject.SetActive(decoration != null);

            if (_name != null)
                _name.text = info.name;

            if (_icon != null)
                _icon.sprite = info.Icon;

            if (_description != null)
                _description.text = info.Description;

            base.Load(info);
        }
    }
}
