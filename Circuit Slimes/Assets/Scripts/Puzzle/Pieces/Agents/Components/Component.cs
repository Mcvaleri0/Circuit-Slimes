using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class Component : Agent
    {
        // Init Method
        public void Initialize(LevelBoard board, Vector2 coords, ComponentTypes type,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(board, coords, Categories.Component, ori, turn);

            this.ComponentType = type;
        }


        #region === Unity Events ===
        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        override protected void Update()
        {
            
        }
        #endregion
    }
}

