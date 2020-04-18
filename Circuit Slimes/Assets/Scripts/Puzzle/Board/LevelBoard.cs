using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Puzzle.Board
{
    public class LevelBoard : MonoBehaviour
    {
        public const int SpaceSize = 2;

        private Dictionary<int, Row> Rows;

        public enum Directions
        {
            None      = -1,
            East      =  0,
            NorthEast =  1,
            North     =  2,
            NorthWest =  3,
            West      =  4,
            SouthWest =  5,
            South     =  6,
            SouthEast =  7
        }

        public static readonly Vector2Int[] DirectionalVectors =
        {
            Vector2Int.right,
            Vector2Int.right + Vector2Int.down,
            Vector2Int.down,
            Vector2Int.left + Vector2Int.down,
            Vector2Int.left,
            Vector2Int.left + Vector2Int.up,
            Vector2Int.up,
            Vector2Int.right + Vector2Int.up
        };

        public int Width  { get; private set; }
        public int Height { get; private set; }

        public void Initialize(int width, int height)
        {
            this.Width  = width;
            this.Height = height;

            this.Rows = new Dictionary<int, Row>();
        }


        #region === Piece Methods ===
        public void PlacePiece(Vector2Int coords, Piece piece)
        {
            if (OutOfBounds(coords))
            {
                throw new System.Exception("Piece out of bounds - ( " + coords.x + ", " + coords.y + ")");
            }

            Row row;
            try
            {
                row = this.Rows[coords.y];
            }
            catch (KeyNotFoundException)
            {
                row = new Row(coords.y);
                this.Rows[coords.y] = row;
            }

            row.PlacePiece(coords.x, piece);
            piece.Coords = coords;
        }

        public Piece RemovePiece(Vector2Int coords)
        {
            var row = this.Rows[coords.y];

            if (row != null)
            {
                var piece = row.RemovePiece(coords.x);

                piece.Coords = new Vector2Int(-1, -1);

                return piece;
            }

            return null;
        }

        public Piece GetPiece(Vector2Int coords)
        {
            this.Rows.TryGetValue(coords.y, out Row row);

            if(row != null)
            {
                return row.GetPiece(coords.x);
            }

            return null;
        }

        public bool MovePiece(Vector2Int coords, Piece piece)
        {
            if(this.GetPiece(coords) == null)
            {
                var foundPiece = this.RemovePiece(piece.Coords);

                //FIXME - You should check that what you found is what you were meant to find

                this.PlacePiece(coords, foundPiece);

                foundPiece.Coords = coords;

                return true;
            }

            return false;
        }

        public bool MovePiece(int x, int y, Piece piece)
        {
            return MovePiece(new Vector2Int(x, y), piece);
        }
        #endregion


        #region === Tile Methods ===
        public void PlaceTile(Vector2Int coords, Tile tile)
        {
            if (OutOfBounds(coords))
            {
                throw new System.Exception("Piece out of bounds");
            }

            Row row;
            try
            {
                row = this.Rows[coords.y];
            }
            catch (KeyNotFoundException)
            {
                row = new Row(coords.y);
                this.Rows[coords.y] = row;
            }

            row.PlaceTile(coords.x, tile);
            tile.Coords = coords;
        }

        public void PlaceTile(int x, int y, Tile tile)
        {
            PlaceTile(new Vector2Int(x, y), tile);
        }

        public Tile RemoveTile(Vector2Int coords)
        {
            var row = this.Rows[coords.y];

            if (row != null)
            {
                var tile = row.RemoveTile(coords.x);

                tile.Coords = new Vector2Int(-1, -1);

                return tile;
            }

            return null;
        }

        public Tile RemoveTile(int x, int y)
        {
            return RemoveTile(new Vector2Int(x, y));
        }

        public Tile GetTile(Vector2Int coords)
        {
            this.Rows.TryGetValue(coords.y, out Row row);

            if (row != null)
            {
                return row.GetTile(coords.x);
            }

            return null;
        }

        public Tile GetTile(int x, int y)
        {
            return GetTile(new Vector2Int(x, y));
        }

        public List<Tile> GetAllTiles()
        {
            var tiles = new List<Tile>();

            var rows = new List<Row>(this.Rows.Values);

            foreach(var row in rows)
            {
                tiles.AddRange(row.GetAllTiles());
            }

            return tiles;
        }
        #endregion


        #region === Utility ===
        public bool OutOfBounds(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x >= this.Width) return true;
            if (coords.y < 0 || coords.y >= this.Height) return true;

            return false;
        }

        // Returns the coordinates of the space next to the one provided in a direction
        public Vector2Int GetAdjacentCoords(Vector2Int coords, Directions direction)
        {
            Vector2Int adjCoords = new Vector2Int(coords.x, coords.y);

            // North - South
            switch (direction)
            {
                default:
                    break;

                case Directions.North:
                case Directions.NorthEast:
                case Directions.NorthWest:
                    adjCoords += Vector2Int.down;
                    break;

                case Directions.South:
                case Directions.SouthEast:
                case Directions.SouthWest:
                    adjCoords += Vector2Int.up;
                    break;
            }

            // West - East
            switch (direction)
            {
                default:
                    break;

                case Directions.East:
                case Directions.NorthEast:
                case Directions.SouthEast:
                    adjCoords += Vector2Int.right;
                    break;

                case Directions.West:
                case Directions.NorthWest:
                case Directions.SouthWest:
                    adjCoords += Vector2Int.left;
                    break;
            }

            if (!OutOfBounds(adjCoords))
            {
                return adjCoords;
            }

            return new Vector2Int(-1, -1);
        }

        public static Vector2Int Discretize(Vector3 position)
        {
            return new Vector2Int((int) Mathf.Floor(position.z / SpaceSize), (int) Mathf.Floor(position.x / SpaceSize));
        }

        public static Vector3 WorldCoords(Vector2 coords)
        {
            return new Vector3((coords.y + 0.5f) * SpaceSize, 1f, (coords.x + 0.5f) * SpaceSize);
        }

        public static LevelBoard.Directions GetDirection(Vector2Int origin, Vector2Int target)
        {
            Vector2Int move = target - origin;

            if (move.sqrMagnitude == 0f) return LevelBoard.Directions.None;

            float angle = 360 - (Mathf.Rad2Deg * Mathf.Atan(move.y / (float) move.x) - 22.5f) % 360f;

            int intAngle = (int) (angle / 45f);

            return (LevelBoard.Directions) intAngle;
        }
        #endregion
    }
}
