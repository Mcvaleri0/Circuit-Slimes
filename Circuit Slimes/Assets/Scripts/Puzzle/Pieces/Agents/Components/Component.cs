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
        public void Initialize(LevelBoard board, Vector2Int coords, ComponentTypes type,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(board, coords, Categories.Component, ori, turn);

            this.ComponentType = type;
        }


        #region === Unity Events ===
        // Start is called before the first frame update
        new protected virtual void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        new protected virtual void Update()
        {
            
        }
        #endregion
    }
}

