using Puzzle;
using Puzzle.Board;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Puzzle.Data
{
    /// <summary>
    /// Puzzle's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class PuzzleData
    {
        public BoardData   Board;
        public PieceData[] Pieces;
        public TileData[]  Tiles;
        public string[]    Permissions;


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

                var name = piece.Caracterization.ToString();

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
        }


        public void Save(string path, string name)
        {
            string filePath = Path.Combine(path, name + ".json");

            string dataAsJson = JsonUtility.ToJson(this, true);
            byte[] jsonBytes  = Encoding.ASCII.GetBytes(dataAsJson);

            File.WriteAllBytes(filePath, jsonBytes);
        }


        public static Puzzle Load(string path, string name)
        {
            string filePath = Path.Combine(path, name + ".json");

            string jsonString;
            
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(filePath);
                while (!www.isDone) { }

                jsonString = www.text;
            }
            else
            {
                byte[] jsonBytes = File.ReadAllBytes(filePath);
                jsonString = Encoding.ASCII.GetString(jsonBytes);
            }

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

            // Initialiaze Puzzle
            puzzle.Initialize(board, pieceList, tileList, permList);

            return puzzle;
        }
    }

}
