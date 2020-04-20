﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Components
{
    public class Battery : CircuitComponent
    {
        #region Unity Events

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Discharge());

            this.Stats = new Statistics(10, 0, -1)
            {
                Food = this.StartingCharges
            };
        }
        #endregion

        #region Component Methods

        override public Action Think()
        {
            return base.Think();
        }
        #endregion
    }
}