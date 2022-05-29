using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Menutee {
    public delegate void DropdownChosenHandler(DropdownManager dropdown, int index, string option);
    public class DropdownManager : UIElementManager {
        public int Id;

        public event DropdownChosenHandler DropdownChosen;

        public TextMeshProUGUI Text;
        public TMP_Dropdown Dropdown;
        public Toggle Template;

        private string[] _options;

        void Awake() {
            Dropdown.onValueChanged.AddListener(DropdownChosenInternal);
        }

        public void SetText(string newText) {
            if (Text != null) {
                Text.text = newText;
            }
        }

        public void SetOptions(string[] options, int index) {
            _options = options;

            Dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < options.Length; i++) {
                optionDatas.Add(new TMP_Dropdown.OptionData(options[i]));
            }
            Dropdown.AddOptions(optionDatas);
            Dropdown.SetValueWithoutNotify(index);
        }

        void DropdownChosenInternal(int newIndex) {
            DropdownChosen?.Invoke(this, newIndex, _options[newIndex]);
        }

        public override void SetColors(PaletteConfig config) {
            base.SetColors(config);
            config?.ApplyToSelectable(Template);
        }
    }
}