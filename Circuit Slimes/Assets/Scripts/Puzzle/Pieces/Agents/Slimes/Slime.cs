using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class Slime : Agent
    {
        // Init Method
        public void Initialize(LevelBoard board, Vector2Int coords, SlimeTypes type,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(board, coords, Categories.Slime, ori, turn);

            this.SlimeType = type;
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
            base.Update();
        }
        #endregion
    }
}

