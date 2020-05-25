using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creator.Selection;



namespace Creator.Mode
{
    public abstract class Mode
    {
        #region /* Creator Sub-Components */

        protected SelectionSystem Selection { get; private set; }

        #endregion



        #region === Init Methods ===

        public Mode(SelectionSystem selection)
        {
            this.Selection = selection;
        }

        #endregion


        #region === Select Methods ===

        public abstract void DefineSelectableList();

        public abstract bool AbleToEditOptions();

        #endregion
    }
}
