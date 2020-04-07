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


        #region === Piece Methods ===
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
        #endregion


        #region === Tile Methods ===
        public void PlaceTile(int x, Tile tile)
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

            space.Tile = tile;
        }

        public Tile RemoveTile(int x)
        {
            var space = this.Spaces[x];

            if (space != null)
            {
                var tile = this.Spaces[x].Tile;
                this.Spaces[x].Tile = null;

                return tile;
            }

            return null;
        }

        public Tile GetTile(int x)
        {
            Space space = null;
            this.Spaces.TryGetValue(x, out space);

            if (space != null)
            {
                return this.Spaces[x].Tile;
            }

            return null;
        }

        public List<Tile> GetAllTiles()
        {
            var tiles = new List<Tile>();

            var spaces = new List<Space>(this.Spaces.Values);

            foreach (var space in spaces)
            {
                //if(!space.Tile.Equals(null))
                if(!object.Equals(space.Tile, null))
                {
                    tiles.Add(space.Tile);
                }
            }

            return tiles;
        }
        #endregion
    }
}
