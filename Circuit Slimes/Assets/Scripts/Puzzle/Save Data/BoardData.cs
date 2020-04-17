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
            this.Width  = board.Width;
            this.Height = board.Height;
        }


        public LevelBoard CreateBoard(Transform parent = null)
        {
            GameObject boardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Board"));
            boardObj.transform.parent = parent;

            var inSceneWidth  = this.Width  * LevelBoard.SpaceSize;
            var inSceneHeight = this.Height * LevelBoard.SpaceSize;

            boardObj.transform.position   = new Vector3(inSceneHeight / 2, 0.5f, inSceneWidth / 2);
            boardObj.transform.localScale = new Vector3(inSceneHeight, 1, inSceneWidth);

            var board = boardObj.GetComponent<LevelBoard>();

            board.Initialize(this.Width, this.Height);

            return board;
        }

    }

}
