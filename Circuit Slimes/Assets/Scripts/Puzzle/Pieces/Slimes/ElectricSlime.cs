using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Slimes
{
    public class ElectricSlime : Slime
    {

        //
        // - Unity Events
        //

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new CardinalMove());

            this.Stats = new Statistics(10, 10, 10);
        }

        //
        // - Slime Methods
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