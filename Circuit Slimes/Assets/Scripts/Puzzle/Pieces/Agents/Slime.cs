using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class Slime : Agent
    {
        public Slime() { }


        #region === Unity Events ===
        // Start is called before the first frame update
        new protected virtual void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        new protected virtual void Update()
        {
            base.Update();
        }
        #endregion
    }
}

