using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Components
{
    public class LED : CircuitComponent
    {
        private LedLight Light;


        #region  === Unity Events ===
        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.Light = this.GetComponent<LedLight>();
        }
        #endregion


        #region === Component Methods ===
        override protected void TurnOn()
        {
            base.TurnOn();
            this.Light.TurnOn();
        }

        override protected void TurnOff()
        {
            base.TurnOff();
            this.Light.TurnOff();
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