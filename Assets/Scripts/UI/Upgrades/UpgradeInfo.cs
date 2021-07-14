using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spacetaurant
{
    public class UpgradeInfo : InfoLoader<UpgradeSO>
    {
        [SerializeField, RefrencesGroup]
        private RawImage _cameraRenderer;
        [SerializeField, RefrencesGroup]
        private TextMeshProUGUI _name;
        [SerializeField, RefrencesGroup]
        private Image _icon;
        [SerializeField, RefrencesGroup]
        private TextMeshProUGUI _description;
        [SerializeField, RefrencesGroup]
        private TextMeshProUGUI _price;
        [SerializeField, RefrencesGroup]
        private GameObject _buyButton;

        public override void Load(UpgradeSO info)
        {
            var decoration = info as DecorationSO;
            if (_cameraRenderer != null)
                _cameraRenderer.transform.parent.gameObject.SetActive(decoration != null);

            if (_name != null)
            {
                if (!_name.isActiveAndEnabled)
                    _name.gameObject.SetActive(info.name != null && info.name != string.Empty);

                if (_name.isActiveAndEnabled)
                    _name.text = info.Name;
            }

            if (_icon != null)
            {
                if (!_icon.isActiveAndEnabled)
                    _icon.gameObject.SetActive(info.Icon != null);

                if (_icon.isActiveAndEnabled)
                    _icon.sprite = info.Icon;
            }

            if (_description != null)
            {
                _description.gameObject.SetActive(info.Description != null && info.Description != string.Empty);

                if (_description.isActiveAndEnabled)
                    _description.text = info.Description;
            }

            if (_price != null)
            {
                _price.gameObject.SetActive(true);

                _price.text = info.Price.FormatInt();
            }
            if (_buyButton != null)
            {
                _buyButton.gameObject.SetActive(true);
            }

            base.Load(info);
        }
    }
}
