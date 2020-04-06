using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Row
    {
        private Dictionary<int, Space> Spaces;

        public Row()
        {
            this.Spaces = new Dictionary<int, Space>();
        }

        public void PlacePiece(int x, Piece piece)
        {
            Space space;
            try
            {
                space = this.Spaces[x];
            }
            catch (KeyNotFoundException)
            {
                space = new Space();
                this.Spaces[x] = space;
            }

            space.Piece = piece;
        }

        public Piece RemovePiece(int x)
        {
            var space = this.Spaces[x];

            if (space != null)
            {
                var piece = this.Spaces[x].Piece;
                this.Spaces[x].Piece = null;

                return piece;
            }

            return null;
        }

        public Piece GetPiece(int x)
        {
            Space space = null;
            this.Spaces.TryGetValue(x, out space);

            if(space != null)
            {
                return this.Spaces[x].Piece;
            }

            return null;
        }
    }
}
