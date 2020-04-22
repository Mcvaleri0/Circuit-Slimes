using Puzzle;
using Puzzle.Board;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;



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

            int nPieces = puzzle.Pieces.Count;
            this.Pieces = new PieceData[nPieces];
            for (int i = 0; i < nPieces; i++)
            {
                this.Pieces[i] = new PieceData(puzzle.Pieces[i]);
            }

            var tiles = puzzle.Board.GetAllTiles();

            int nTiles = tiles.Count;
            this.Tiles = new TileData[nTiles];
            for (int i = 0; i < nTiles; i++)
            {
                this.Tiles[i] = new TileData(tiles[i]);
            }

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
            string filePath   = Path.Combine(path, name + ".json");
            byte[] jsonBytes  = File.ReadAllBytes(filePath);
            string jsonString = Encoding.ASCII.GetString(jsonBytes);
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
                    pieceList.Add(pieceData.CreatePiece(puzzle));
                }
            }
            #endregion

            #region Create Tiles
            var tileList = new List<Tile>();

            if(puzzleData.Tiles != null)
            {
                foreach (TileData tileData in puzzleData.Tiles)
                {
                    tileList.Add(tileData.CreateTile(puzzle));
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
