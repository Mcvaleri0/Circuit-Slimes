using Puzzle.Board;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Puzzle.Data
{
    /// <summary>
    /// Board's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class BoardData
    {
        public int Width;
        public int Height;


        public BoardData(LevelBoard board)
        {
            this.Width = board.Width;
            this.Height = board.Height;
        }


        public LevelBoard CreateBoard()
        {
            var board = new LevelBoard(this.Width, this.Height);

            board.Draw(null, this.Width * 2, this.Height * 2);

            return board;
        }

    }

}
