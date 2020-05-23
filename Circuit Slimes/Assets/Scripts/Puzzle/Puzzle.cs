using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;



namespace Puzzle
{
    public class Puzzle : MonoBehaviour
    {
        #region /* Attributes */

        public List<Piece> Pieces { get; private set; }

        public List<Agent> Agents { get; private set; }

        public LevelBoard Board { get; private set; }

        public GameObject PiecesObj { get; private set; }

        public GameObject TilesObj { get; private set; }

        public Dictionary<string, Resource> ResourcesAvailable { get; private set; }

        public WinCondition WinCondition { get; private set; }

        #endregion


        #region === Initialization Methods ===

        public static Puzzle CreateEmpty(int width, int height)
        {
            GameObject puzzleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Puzzle"));
            puzzleObj.name = "Puzzle";
            Puzzle puzzle = puzzleObj.GetComponent<Puzzle>();

            GameObject boardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Board"));
            boardObj.name = "Board";
            boardObj.transform.parent = puzzleObj.transform;
            LevelBoard board = boardObj.GetComponent<LevelBoard>();

            board.Initialize(width, height);
            puzzle.Initialize(board);

            return puzzle;
        }

        public void Initialize(LevelBoard board)
        {
            Initialize(board, new List<Piece>(), new List<Tile>(), new WinCondition(WinCondition.Conditions.None), new Dictionary<string, Resource>());
        }

        public void Initialize(LevelBoard board, List<Piece> pieces, List<Tile> tiles,
                        WinCondition winCondition, Dictionary<string, Resource> resources)
        {
            this.Board  = board;
            this.Pieces = pieces;
            this.WinCondition = winCondition;
            this.ResourcesAvailable = resources;

            this.Agents = new List<Agent>();

            foreach(var piece in pieces)
            {
                this.Board.PlacePiece(piece, piece.Coords);
            }

            this.Agents = this.Board.GetAllAgents();

            foreach (var tile in tiles)
            {
                this.Board.PlaceTile(tile, tile.Coords);
            }

            this.PiecesObj = this.transform.GetChild(0).gameObject;
            this.TilesObj  = this.transform.GetChild(1).gameObject;
        }

        public void Destroy()
        {
            GameObject.Destroy(this.gameObject);
        }

        #endregion


        #region === Piece Methods ===

        public Piece CreatePiece(Piece.Characteristics characterization, Vector2Int coords,
            LevelBoard.Directions ori = LevelBoard.Directions.South, int turn = 0)
        {
            var piece = Piece.CreatePiece(this, characterization, coords, ori, turn);

            if(!this.Board.PlacePiece(piece, coords))
            {
                Destroy(piece.gameObject);

                return null;
            }

            this.Pieces.Add(piece);

            if (piece is Agent agent) this.Agents.Add(agent);

            return piece;
        }

        public bool PlacePiece(Piece piece, Vector2Int coords)
        {
            if (!this.Board.PlacePiece(piece, coords)) return false;

            this.Pieces.Add(piece);

            if (piece is Agent agent) this.Agents.Add(agent);

            return true;
        }


        public bool RemovePiece(Piece piece)
        {
            // Remove the Piece from the Puzzle's Piece List
            if (!this.Pieces.Remove(piece)) return false;
            
            // If it's an Agent, remove it from the Puzzle's Agent List
            if (piece is Agent agent) this.Agents.Remove(agent);

            // Remove the Piece from the Board
            if (!this.Board.RemovePieceAt(piece.Coords)) return false;

            Destroy(piece.gameObject);

            return true;
        }


        public bool MovePiece(Vector2Int coords, Piece piece)
        {
            return this.Board.MovePiece(piece, coords);
        }


        public bool RotatePiece(Piece piece, LevelBoard.Directions orientation)
        {
            return this.Board.RotatePiece(piece, orientation);
        }


        public bool RotatePieceRight(Piece piece)
        {
            LevelBoard.Directions newDir = LevelBoard.GetNextDirection(piece.Orientation, 2);
            return this.RotatePiece(piece, newDir);
        }


        public Piece GetPiece(Vector2Int coords)
        {
            return this.Board.GetPiece(coords);
        }


        public bool CanPlacePiece(Vector2Int coords, Piece piece)
        {
            return this.Board.CanPlacePiece(coords, piece);
        }

        public bool CanPlacePiece(Vector2Int coords, LevelBoard.Directions orientation, Piece piece)
        {
            return this.Board.CanPlacePiece(coords, orientation, piece);
        }

        public bool IsFree(Vector2Int coords, Piece piece) 
        {
            return this.Board.CanPlacePiece(coords, piece);
        }

        #endregion


        #region === Tile Methods ===
 
        public Tile CreateTile(Tile.Types type, Vector2Int coords)
        {
            var tile = Tile.CreateTile(this, type, coords);

            if (!this.Board.PlaceTile(tile, coords))
            {
                Destroy(tile.gameObject);

                return null;
            }

            return tile;
        }


        public bool PlaceTile(Tile tile, Vector2Int coords)
        {
            bool success = this.Board.PlaceTile(tile, coords);

            if (tile.transform.parent == null)
                tile.transform.parent = this.TilesObj.transform;

            return success;
        }


        public bool RemoveTile(Tile tile)
        {
            return this.Board.RemoveTileAt(tile.Coords) != null;
        }


        public bool MoveTile(Vector2Int coords, Tile tile)
        {
            return this.Board.MoveTile(coords, tile);
        }


        public Tile GetTile(Vector2Int coords)
        {
            return this.Board.GetTile(coords);
        }


        public void UpdateAllTiles()
        {
            this.Board.UpdateAllTiles();
        }


        public bool IsFree(Vector2Int coords, Tile tile)
        {
            Tile tileNewPos = this.GetTile(coords);

            if (tileNewPos == null || tileNewPos == tile)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region === Resources Methods ===

        public Resource GetResource(string prefab)
        {
            try
            {
                return this.ResourcesAvailable[prefab];
            }
            catch (KeyNotFoundException)
            {
                Resource resource = new Resource(prefab);
                this.ResourcesAvailable[prefab] = resource;
                return resource;
            }
        }


        public List<string> GetAllResources()
        {
            return this.ResourcesAvailable.Keys.ToList();
        }

        #endregion


        #region === Utility ===

        public bool OutOfBounds(Vector2Int coords)
        {
            return this.Board.OutOfBounds(coords);
        }

        public Vector3 AtBoardSurface(Vector3 coords)
        {
            return this.Board.AtBoardSurface(coords);
        }

        public Vector2Int Discretize(Vector3 position)
        {
            return LevelBoard.Discretize(position);
        }

        public Vector3 WorldCoords(Vector2Int position)
        {
            return LevelBoard.WorldCoords(position);
        }

        public Vector2Int Clamp(Vector2Int coords)
        {
            int x = Mathf.Clamp(coords.x, 0, Board.Width - 1);
            int y = Mathf.Clamp(coords.y, 0, Board.Height - 1);

            return new Vector2Int(x, y);
        }

        #endregion
    }
}
