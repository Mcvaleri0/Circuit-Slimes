using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creator.Selection;



namespace Creator.Mode
{
    public class Editor : Mode
    {
        #region === Init Methods ===

        public Editor(SelectionSystem selection) :
            base(selection) { }

        #endregion


        #region === Select Methods ===

        override public void DefineSelectableList()
        {
            this.Selection.WhiteListAllItens();
        }

        #endregion

    }
}
