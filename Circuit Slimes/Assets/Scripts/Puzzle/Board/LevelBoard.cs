using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;


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
        public bool CanPlacePiece(Vector2Int coords, Piece piece)
        {
            var footprint = piece.GetFootprintAt(coords);

            foreach(var spot in footprint)
            {
                if (this.OutOfBounds(spot)) return false;

                var pieceInSpot = this.GetPiece(spot);

                if (pieceInSpot != null && pieceInSpot != piece)
                {
                    return false;
                }
            }

            return true;
        }

        public bool PlacePiece(Vector2Int coords, Piece piece)
        {
            if (!this.CanPlacePiece(coords, piece)) return false;

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

            if(row.PlacePiece(coords.x, piece))
            {
                piece.Coords = coords;
                return true;
            }

            return false;
        }

        public Piece RemovePieceAt(Vector2Int coords)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("RemoveTile - OutOfBoundsException - " + coords);
                return null;
            }

            Row row;
            try
            {
                row = this.Rows[coords.y];

                if (row != null)
                {
                    var piece = row.RemovePiece(coords.x);

                    piece.Coords = new Vector2Int(-1, -1);

                    return piece;
                }
            }
            catch (KeyNotFoundException) { }

            return null;
        }

        public Piece RemovePiece(Piece piece)
        {
            if(this.GetPiece(piece.Coords) == piece)
            {
                return this.RemovePieceAt(piece.Coords);
            }

            return null;
        }

        public Piece GetPiece(Vector2Int coords)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("GetPiece - OutOfBoundsException - " + coords);
                return null;
            }

            this.Rows.TryGetValue(coords.y, out Row row);

            if(row != null)
            {
                return row.GetPiece(coords.x);
            }

            return null;
        }

        public bool MovePiece(Vector2Int coords, Piece piece)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("MovePiece - OutOfBoundsException - " + coords);
                return false;
            }

            // If destination Space is free
            if (this.GetPiece(coords) == null)
            {
                var foundPiece = this.GetPiece(piece.Coords);

                // If the Piece was where it should be
                if(foundPiece == piece)
                {
                    this.RemovePieceAt(foundPiece.Coords);

                    this.PlacePiece(coords, foundPiece);

                    return true;
                }
                else if(piece is Pieces.Agent agent)
                {
                    if(!agent.Active)
                    {
                        this.PlacePiece(coords, foundPiece);

                        return true;
                    }
                }
            }

            return false;
        }

        public List<Agent> GetAllAgents()
        {
            var agents = new List<Agent>();

            foreach(var entry in this.Rows)
            {
                agents.AddRange(entry.Value.GetAllAgents());
            }

            return agents;
        }
        #endregion


        #region === Tile Methods ===
        public bool PlaceTile(Vector2Int coords, Tile tile)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("PlaceTile - OutOfBoundsException - " + coords);
                return false;
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

            //visual update
            tile.UpdateTile();
            tile.UpdateCrossTiles();

            return true;
        }

        public Tile RemoveTileAt(Vector2Int coords)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("RemoveTile - OutOfBoundsException - " + coords);
                return null;
            }

            Row row;
            try
            {
                row = this.Rows[coords.y];

                if (row != null)
                {
                    var tile = row.RemoveTile(coords.x);

                    //visual update
                    tile.UpdateCrossTiles();

                    tile.Coords = new Vector2Int(-1, -1);

                    return tile;
                }
            }
            catch (KeyNotFoundException) { }

            return null;
        }

        public bool MoveTile(Vector2Int coords, Tile tile)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("MoveTile - OutOfBoundsException - " + coords);
                return false;
            }

            // If destination Space is free
            if (this.GetTile(coords) == null)
            {
                this.RemoveTileAt(tile.Coords);

                this.PlaceTile(coords, tile);

                return true;
            }

            return false;
        }

        public Tile GetTile(Vector2Int coords)
        {
            if (OutOfBounds(coords))
            {
                Debug.Log("GetTile - OutOfBoundsException - " + coords);
                return null;
            }

            this.Rows.TryGetValue(coords.y, out Row row);

            if (row != null)
            {
                return row.GetTile(coords.x);
            }

            return null;
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

        public void UpdateAllTiles()
        {
            var tiles = this.GetAllTiles();

            foreach (var tile in tiles)
            {
                tile.UpdateTile();
            }
        }

        #endregion


        #region === Utility ===
        public bool OutOfBounds(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x >= this.Width) return true;
            if (coords.y < 0 || coords.y >= this.Height) return true;

            return false;
        }

        public Vector2Int Clamp(Vector2Int coords)
        {
            int x = Mathf.Clamp(coords.x, 0, this.Width - 1);
            int y = Mathf.Clamp(coords.y, 0, this.Height - 1);

            return new Vector2Int(x, y);
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

        public Vector3 AtBoardSurface(Vector3 coords)
        {
            Vector2Int coordsBoard = Discretize(coords);
            coordsBoard = this.Clamp(coordsBoard);

            return WorldCoords(coordsBoard);
        }

        public static LevelBoard.Directions GetDirection(Vector2Int origin, Vector2Int target)
        {
            Vector2Int move = target - origin;

            if (move.sqrMagnitude == 0f) return LevelBoard.Directions.None;

            float angle = (360 - Mathf.Rad2Deg * Mathf.Atan2(move.y, move.x)) % 360;

            int intAngle = (int) (angle / 45f);

            return (LevelBoard.Directions) intAngle;
        }
        #endregion
    }
}
