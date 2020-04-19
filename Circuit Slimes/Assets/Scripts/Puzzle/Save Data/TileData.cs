using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle;
using Puzzle.Board;

namespace Puzzle.Data
{
    /// <summary>
    /// Tile's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class TileData
    {
        public Vector2Int Coords;

        public Tile.Types Type;


        public TileData(Tile tile)
        {
            this.Coords = tile.Coords;

            this.Type = tile.Type;
        }


        public Tile CreateTile(Transform parent, Puzzle puzzle)
        {
            return Tile.CreateTile(parent, puzzle, this.Coords, this.Type);
        }

    }

}
