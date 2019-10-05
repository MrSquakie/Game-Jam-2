namespace GameCreator.Dialogue
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Localization;

    [AddComponentMenu("")]
    public class DialogueItemChoice : IDialogueItem
    {
        public bool showOnce = false;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override IDialogueItem[] GetNextItem()
        {
            if (this.children == null || this.children.Count == 0) return null;
            return this.children.ToArray();
        }

        public override bool CanHaveParent(IDialogueItem parent)
        {
            if (parent.GetType() == typeof(DialogueItemChoiceGroup)) return true;
            return false;
        }

        public override bool CheckConditions()
        {
            if (this.showOnce && this.IsRevisit()) return false;
            return base.CheckConditions();
        }
    }
}
