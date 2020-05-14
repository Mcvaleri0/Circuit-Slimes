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

        private Dictionary<Vector2Int, Change> Changes;

        public void Initialize(int width, int height)
        {
            this.Width  = width;
            this.Height = height;

            this.Rows = new Dictionary<int, Row>();

            this.Changes = new Dictionary<Vector2Int, Change>();
        }


        #region === Piece Methods ===
        #region Base Methods
        // These methods do not have verifications, they are not meant to
        // They are pure operations over the data structure
        // Use with caution

        private void BoardSetPiece(Vector2Int coords, Piece piece)
        {
            // Get the Piece's footprint at the intended coords
            var footprint = piece.GetFootprintAt(coords);

            // Foreach spot it will occupy
            foreach (var partCoords in footprint)
            {
                Row row;
                try
                {
                    row = this.Rows[partCoords.y];
                }
                catch (KeyNotFoundException)
                {
                    row = new Row(partCoords.y);
                    this.Rows[partCoords.y] = row;
                }

                // Set the the Space in the Row as being occupied by the Piece
                row.SetPiece(partCoords.x, piece);
            }

            // Chaneg the Pieces Coords to the new position
            piece.Coords = footprint[0];
        }

        private void BoardRemovePiece(Piece piece)
        {
            // Get the Piece's Footprint
            var footprint = piece.GetFootprint();

            // For each Space the Piece is occupying
            foreach (var partCoords in footprint)
            {
                Row row;
                try
                {
                    row = this.Rows[partCoords.y];

                    if (row != null)
                    {
                        // Remove the Piece from the Space it occupies in the Row
                        row.RemovePiece(partCoords.x);
                    }
                }
                catch (KeyNotFoundException) { }
            }

            // Update the Piece's Coords
            piece.Coords = new Vector2Int(-1, -1);
        }
        #endregion

        #region Verifications
        // Returns true if the Piece can be placed centered at the given Coords
        public bool CanPlacePiece(Vector2Int coords, Piece piece)
        {
            // Get Piece's Footprint
            var footprint = piece.GetFootprintAt(coords);

            // Foreach Space the Piece would occupy
            foreach(var partCoords in footprint)
            {
                // If the Space is Out Of Bounds return false
                if (this.OutOfBounds(partCoords)) return false;

                // Get the Piece presently at that Space (if any)
                var pieceInSpace = this.GetPiece(partCoords);

                // If the Space is occupied and not by this Piece
                if (pieceInSpace != null && pieceInSpace != piece)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanPlacePiece(Vector2Int coords, LevelBoard.Directions orientation, Piece piece)
        {
            // Get Piece's Footprint
            var footprint = piece.GetFootprintAt(coords, orientation);

            // Foreach Space the Piece would occupy
            foreach (var partCoords in footprint)
            {
                // If the Space is Out Of Bounds return false
                if (this.OutOfBounds(partCoords)) return false;

                // Get the Piece presently at that Space (if any)
                var pieceInSpace = this.GetPiece(partCoords);

                // If the Space is occupied and not by this Piece
                if (pieceInSpace != null && pieceInSpace != piece)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Getters
        // Returns the Piece at the given Coords (null if there's none)
        public Piece GetPiece(Vector2Int coords)
        {
            // If the Coords are Out Of Bounds
            if (OutOfBounds(coords))
            {
                Debug.Log("GetPiece - OutOfBoundsException - " + coords);
                return null;
            }

            // Get the Y Row of the Board (if one exists)
            this.Rows.TryGetValue(coords.y, out Row row);

            // If the Row exists
            if (row != null)
            {
                // Get the Piece at the X column of the Row (if one exists)
                return row.GetPiece(coords.x);
            }

            return null;
        }

        // Returns all the Agents on the Board
        public List<Agent> GetAllAgents()
        {
            var agents = new List<Agent>();

            foreach (var entry in this.Rows)
            {
                var agentsInRow = entry.Value.GetAllAgents();

                foreach(var agent in agentsInRow)
                {
                    if (agents.Contains(agent)) continue;

                    agents.Add(agent);
                }
            }

            return agents;
        }
        #endregion

        // Returns true if it succeeds at placing the Piece at the given Coords
        public bool PlacePiece(Piece piece, Vector2Int coords)
        {
            // If the Piece cannot be placed at the Coords return false
            if (!this.CanPlacePiece(coords, piece)) return false;

            // Set the necessary Board Spaces to hold the Piece
            this.BoardSetPiece(coords, piece);

            return true;
        }

        #region Remove
        // Returns the Piece removed from the given Coords (null if there's none)
        public Piece RemovePieceAt(Vector2Int coords)
        {
            // Get the Piece at the given Coords
            var piece = this.GetPiece(coords);

            // If there was a Piece there
            if(piece != null)
            {
                // Remove the Piece from the necessary Board Spaces
                this.BoardRemovePiece(piece);
            }

            return piece;
        }

        // Returns true if the Piece was successfully removed
        public bool RemovePiece(Piece piece)
        {
            if(this.GetPiece(piece.Coords) == piece)
            {
                if(this.RemovePieceAt(piece.Coords) == piece)
                    return true;
            }

            return false;
        }
        #endregion

        // Returns true if the Piece is successfully moved to the given Coords
        public bool MovePiece(Piece piece, Vector2Int coords)
        {
            // Get what Piece is at the given Coords
            var foundPiece = this.GetPiece(piece.Coords);

            // If the Piece was where it should be
            if(foundPiece == piece)
            {
                // If the Piece cannot fit there return false
                if (!this.CanPlacePiece(coords, piece)) return false;

                // Remove the Piece from where it is on the Board
                this.BoardRemovePiece(foundPiece);

                // Place the Piece at the new Coords
                this.BoardSetPiece(coords, foundPiece);

                return true;
            }

            // If it is not and it is an Agent
            else if(piece is Agent agent)
            {
                // If the Agent is inactive
                if(!agent.Active)
                {
                    // If the Piece cannot fit there return false
                    if (!this.CanPlacePiece(coords, piece)) return false;

                    // Place the Piece at the new Coords
                    this.BoardSetPiece(coords, foundPiece);

                    return true;
                }
            }

            return false;
        }

        // Returns true if the Piece is successfully moved to the given Coords
        public bool RotatePiece(Piece piece, Directions orientation)
        {
            // Get what Piece is at the given Coords
            var foundPiece = this.GetPiece(piece.Coords);

            // If the Piece was where it should be
            if (foundPiece == piece)
            {
                var coords = piece.Coords;

                // If the Piece cannot fit there return false
                if (!this.CanPlacePiece(coords, orientation, piece)) return false;

                // Remove the Piece from where it is on the Board
                this.BoardRemovePiece(foundPiece);

                piece.Rotate(orientation);

                // Place the Piece at the new Coords
                this.BoardSetPiece(coords, foundPiece);

                return true;
            }

            return false;
        }
        #endregion


        #region === Tile Methods ===
        public bool PlaceTile(Tile tile, Vector2Int coords)
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

                this.PlaceTile(tile, coords);

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
        public static Vector2Int GetAdjacentCoords(Vector2Int coords, Directions direction)
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

            return adjCoords;
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
