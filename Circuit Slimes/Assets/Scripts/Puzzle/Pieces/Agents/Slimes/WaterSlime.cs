using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Slimes
{
    public class WaterSlime : Slime
    {

        //
        // - Unity Events
        //

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Eat(new Candy(CandyTypes.Water)));

            this.Stats = new Statistics(10, 10, 10);
        }

        //
        // - Slime Methods
        //

        override public Action Think()
        {
            return base.Think();
        }
    }
}