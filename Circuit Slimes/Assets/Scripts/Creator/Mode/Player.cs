using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creator.Selection;



namespace Creator.Mode
{
    public class Player : Mode
    {
        #region === Init Methods ===

        public Player(SelectionSystem selection) :
            base(selection) { }

        #endregion


        #region === Select Methods ===

        override public void DefineSelectableList()
        {
            this.Selection.EmptyWhiteList();
        }


        public override bool AbleToEditOptions()
        {
            return false;
        }


        #endregion
    }
}
