using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Slimes
{
    public class ElectricSlime : Slime
    {
        public ElectricSlime() : base()
        {
            this.SlimeType = SlimeTypes.Electric;
        }

        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.SlimeType = SlimeTypes.Electric;

            this.KnownActions.Add(new Charge());
            this.KnownActions.Add(new ElectricMovement());

            this.Stats = new Statistics(10, 10, 10);
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.T) && (this.Coords.x == 0 || this.Coords.x == -1))
            {
                if(this.Active)
                {
                    this.Deactivate();
                }
                else
                {
                    this.Reactivate(this.Coords);
                }
            }
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