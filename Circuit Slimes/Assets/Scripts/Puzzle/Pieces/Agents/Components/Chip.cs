using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Components
{
    public class Chip : CircuitComponent
    {

        //private int ChargeCount = 5;

        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Discharge());
        }
        #endregion

        #region === Agent Methods ===

        override public Action Think()
        {
            return base.Think();
        }
        #endregion
    }
}