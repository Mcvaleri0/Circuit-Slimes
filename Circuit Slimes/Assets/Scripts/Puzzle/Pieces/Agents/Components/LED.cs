using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Components
{
    public class LED : CircuitComponent
    {

        //private int ChargeCount = 5;

        //
        // - Unity Events
        //

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Discharge());
        }

        //
        // - Component Methods
        //

        override public Action Think()
        {
            foreach(var action in this.KnownActions)
            {
                return action.Available(this);
            }

            return null;
        }
    }
}