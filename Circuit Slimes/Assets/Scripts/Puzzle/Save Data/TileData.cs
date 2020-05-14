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
        public string PrefabName;

        public Vector2Int[] Coords;


        public TileData(Tile tile)
        {
            this.Coords = new Vector2Int[1];
            this.Coords[0] = tile.Coords;

            this.PrefabName = Tile.GetName(tile.Type);
        }


        public TileData(Tile[] tiles)
        {
            this.Coords = new Vector2Int[tiles.Length];

            var i = 0;
            foreach(var tile in tiles)
            {
                this.Coords[i++] = tile.Coords;
            }

            this.PrefabName = Tile.GetName(tiles[0].Type);
        }


        public Tile[] CreateTiles(Puzzle puzzle)
        {
            var tiles = new Tile[this.Coords.Length];

            var i = 0;
            foreach(var coords in this.Coords)
            {
                tiles[i] = Tile.CreateTile(puzzle, this.PrefabName, this.Coords[i]);
                i++;
            }

            return tiles;
        }

    }

}
