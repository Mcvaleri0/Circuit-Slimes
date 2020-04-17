using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Pieces
{
    public class Candy : Piece
    {
        public Candy(CandyTypes type)
        {
            this.Category = Categories.Candy;

            this.CandyType = type;
        }


        // Init Method
        public void Initialize(LevelBoard board, Vector2Int coords, CandyTypes type)
        {
            base.Initialize(board, coords, Categories.Candy);

            this.CandyType = type;
        }


        // Start is called before the first frame update
        void Start()
        {
            this.Category = Categories.Candy;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}