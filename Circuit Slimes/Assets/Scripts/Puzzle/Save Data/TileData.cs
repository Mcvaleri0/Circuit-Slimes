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
        public Vector2 Coords;

        public Tile.Types Type;

        public TileData(Tile tile)
        {
            this.Coords = tile.Coords;

            this.Type = tile.Type;
        }


        public Tile CreateTile(Transform parent, LevelBoard board)
        {
            GameObject obj = null;

            switch(this.Type)
            {
                default:
                case Tile.Types.None:
                    return null;

                case Tile.Types.Solder:
                    obj = Tile.Instantiate(parent, this.Type, this.Coords);

                    var tile = obj.GetComponent<Tile>();

                    tile.Initialize(board, this.Coords, this.Type);

                    return tile;
            }
        }

    }

}
