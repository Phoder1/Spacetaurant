// --- PBGamesStudio - PhantomBeasts ---
// Alon Talmi(alon.talmi@gmail.com)
// 7/5/2021 2:44:27 PM
// ----------------------

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Spacetaurant;
using UnityEngine;

namespace Spacetaurant
{
    public class ColoredFoldoutGroupAttributeDrawer : OdinGroupDrawer<ColoredFoldoutGroupAttribute>
    {
        private LocalPersistentContext<bool> isExpanded;

        protected override void Initialize()
        {
            this.isExpanded = this.GetPersistentValue<bool>(
                "ColoredFoldoutGroupAttributeDrawer.isExpanded",
                GeneralDrawerConfig.Instance.ExpandFoldoutByDefault && this.Attribute.Expanded);

            //if (!this.Attribute.Expanded)
            //    this.isExpanded.Value = false;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushColor(new Color(this.Attribute.R, this.Attribute.G, this.Attribute.B, this.Attribute.A));
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            GUIHelper.PopColor();
            this.isExpanded.Value = SirenixEditorGUI.Foldout(this.isExpanded.Value, label);
            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(this, this.isExpanded.Value))
            {
                for (int i = 0; i < this.Property.Children.Count; i++)
                {
                    this.Property.Children[i].Draw();
                }
            }

            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}
