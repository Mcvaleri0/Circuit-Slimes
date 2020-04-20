using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class Slime : Agent
    {
        public Slime()
        {
            this.Category = Categories.Slime;
        }

        // Init Method
        public void Initialize(Puzzle puzzle, Vector2Int coords, SlimeTypes type,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(puzzle, coords, Categories.Slime, ori, turn);

            this.SlimeType = type;
        }


        #region === Unity Events ===
        // Start is called before the first frame update
        new protected virtual void Start()
        {
            base.Start();

            this.Category = Categories.Slime;
        }

        // Update is called once per frame
        new protected virtual void Update()
        {
            base.Update();
        }
        #endregion
    }
}

