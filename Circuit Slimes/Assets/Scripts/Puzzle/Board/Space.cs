using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Space
    {
        public Vector2 Coords { get; private set; }

        public Piece Piece;

        public Tile Tile;

        public Space(Vector2 coords)
        {
            this.Coords = coords;
        }
    }

}