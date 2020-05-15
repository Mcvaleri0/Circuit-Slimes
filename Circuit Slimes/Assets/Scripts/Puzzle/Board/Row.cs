using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;

namespace Puzzle.Board
{
    public class Row
    {
        private int Id;

        private Dictionary<int, Space> Spaces;

        public Row(int id)
        {
            this.Id = id;

            this.Spaces = new Dictionary<int, Space>();
        }


        #region === Piece Methods ===
        public bool SetPiece(int x, Piece piece)
        {
            Space space;
            try
            {
                space = this.Spaces[x];

                if(space.Piece == null)
                {
                    space.Piece = piece;
                    return true;
                }
            }
            catch (KeyNotFoundException)
            {
                space = new Space(new Vector2(x, this.Id));
                this.Spaces[x] = space;
                space.Piece = piece;
                return true;
            }

            return false;
        }

        public Piece RemovePiece(int x)
        {
            Space space;
            try
            {
                space = this.Spaces[x];

                if (space != null)
                {
                    var piece = this.Spaces[x].Piece;
                    this.Spaces[x].Piece = null;

                    return piece;
                }
            }
            catch (KeyNotFoundException) { }

            return null;
        }

        public Piece GetPiece(int x)
        {
            this.Spaces.TryGetValue(x, out Space space);

            if(space != null)
            {
                return this.Spaces[x].Piece;
            }

            return null;
        }

        public List<Agent> GetAllAgents()
        {
            var agents = new List<Agent>();

            foreach(var entry in this.Spaces)
            {
                var piece = entry.Value.Piece;

                if(piece != null && piece is Agent agent)
                {
                    if (agents.Contains(agent)) continue;

                    agents.Add(agent);
                }
            }

            return agents;
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
                space = new Space(new Vector2(x, this.Id));
                this.Spaces[x] = space;
            }

            space.Tile = tile;
        }

        public Tile RemoveTile(int x)
        {
            Space space;
            try
            {
                space = this.Spaces[x];

                if (space != null)
                {
                    var tile = this.Spaces[x].Tile;
                    this.Spaces[x].Tile = null;

                    return tile;
                }
            }
            catch (KeyNotFoundException) { }

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
