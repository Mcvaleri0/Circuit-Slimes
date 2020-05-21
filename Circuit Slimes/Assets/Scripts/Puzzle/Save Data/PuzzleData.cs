using Puzzle;
using Puzzle.Board;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using Level;



namespace Puzzle.Data
{
    /// <summary>
    /// Puzzle's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class PuzzleData
    {
        public BoardData    Board;
        public PieceData[]  Pieces;
        public TileData[]   Tiles;
        public string[]     Permissions;
        public ResourceData[] ResourcesAvailable;
        public WinCondition.Conditions Condition;

        public PuzzleData(Puzzle puzzle)
        {
            this.Board = new BoardData(puzzle.Board);

            #region Pieces
            List<string> pieceTypesFound = new List<string>();

            int nPieces = puzzle.Pieces.Count;
            var piecesData = new List<PieceData>();

            for (int i = 0; i < nPieces; i++)
            {
                var piece = puzzle.Pieces[i];

                var name = piece.Characterization.ToString();

                if (!pieceTypesFound.Contains(name))
                {
                    var piecesOfType = new List<Piece>()
                    {
                        piece
                    };

                    for(int j = i + 1; j < nPieces; j++)
                    {
                        var otherPiece = puzzle.Pieces[j];

                        if (otherPiece.TypeMatches(piece))
                            piecesOfType.Add(otherPiece);
                    }

                    piecesData.Add(new PieceData(piecesOfType.ToArray()));
                    pieceTypesFound.Add(name);
                }
            }

            this.Pieces = piecesData.ToArray();
            #endregion

            #region Tiles
            var tiles = puzzle.Board.GetAllTiles();

            var tileTypesFound = new List<string>(); 

            int nTiles = tiles.Count;
            var tilesData = new List<TileData>();

            for (int i = 0; i < nTiles; i++)
            {
                var tile = tiles[i];

                var name = Tile.GetName(tile.Type);

                if(!tileTypesFound.Contains(name))
                {
                    var tilesOfTypeFound = new List<Tile>()
                    {
                        tile
                    };

                    for(var j = i + 1; j < nTiles; j++)
                    {
                        var otherTile = tiles[j];

                        if (otherTile.Type == tile.Type)
                            tilesOfTypeFound.Add(otherTile);
                    }

                    tilesData.Add(new TileData(tilesOfTypeFound.ToArray()));
                    tileTypesFound.Add(name);
                }
            }

            this.Tiles = tilesData.ToArray();
            #endregion

            this.Permissions = puzzle.Permissions.ToArray();

            #region Resources
            List<Resource> resources = puzzle.ResourcesAvailable.Values.Where(r => r.WorthSaving()).ToList();
            int nResources = resources.Count;

            this.ResourcesAvailable = new ResourceData[nResources];

            for (int i = 0; i < nResources; i++)
            {
                this.ResourcesAvailable[i] = new ResourceData(resources[i]);
            }
            #endregion

            this.Condition = puzzle.WinCondition.Condition;
        }


        public void Save(string name)
        {
            string dataAsJson = JsonUtility.ToJson(this, true);

            FileHelper.WriteLevel(dataAsJson, name);
        }


        public static Puzzle Load(string name)
        {
            string jsonString = FileHelper.LoadLevel(name);

            PuzzleData puzzleData = JsonUtility.FromJson<PuzzleData>(jsonString);

            // Instantiate Puzzle
            GameObject puzzleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Puzzle"));
            puzzleObj.name = "Puzzle";
            var puzzle = puzzleObj.GetComponent<Puzzle>();

            // Create Board
            var board = puzzleData.Board.CreateBoard(puzzleObj.transform);

            puzzle.Initialize(board);

            #region Create Pieces
            var pieceList = new List<Piece>();

            if(puzzleData.Pieces != null)
            {
                foreach (PieceData pieceData in puzzleData.Pieces)
                {
                    pieceList.AddRange(pieceData.CreatePieces(puzzle));
                }
            }
            #endregion

            #region Create Tiles
            var tileList = new List<Tile>();

            if(puzzleData.Tiles != null)
            {
                foreach (TileData tileData in puzzleData.Tiles)
                {
                    tileList.AddRange(tileData.CreateTiles(puzzle));
                }
            }
            #endregion

            #region Create Permissions
            List<string> permList = new List<string>();

            if (puzzleData.Permissions != null)
            {
                foreach (string perm in puzzleData.Permissions)
                {
                    permList.Add(perm);
                }
            }
            #endregion

            #region Resources
            Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

            if (puzzleData.ResourcesAvailable != null)
            {
                foreach (ResourceData resource in puzzleData.ResourcesAvailable)
                {
                    resources.Add(resource.Name, resource.CreateResource());
                }
            }
            #endregion

            // Initialiaze Puzzle
            puzzle.Initialize(board, pieceList, tileList, permList, new WinCondition(puzzleData.Condition), resources);

            return puzzle;
        }
    }

}
