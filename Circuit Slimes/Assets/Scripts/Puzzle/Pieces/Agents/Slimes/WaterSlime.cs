using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Slimes
{
    public class WaterSlime : Slime
    {

        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Eat(new Candy(CandyTypes.Water)));

            this.Stats = new Statistics(10, 10, 5);
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