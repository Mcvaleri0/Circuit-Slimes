using Puzzle;
using Puzzle.Board;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        }


        public void Save(string path, string name)
        {
            string data = JsonUtility.ToJson(this);
            System.IO.File.WriteAllText(path + "/" + name + ".json", data);
        }


        public static Puzzle Load(string path, string name)
        {
            StreamReader reader = new StreamReader(path + "/" + name + ".json");
            string jsonData = reader.ReadToEnd();
            reader.Close();

            // Load Data
            PuzzleData puzzleData = JsonUtility.FromJson<PuzzleData>(jsonData);

            // Instantiate Puzzle
            GameObject puzzleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Puzzle"));
            var puzzle = puzzleObj.GetComponent<Puzzle>();

            // Create Board
            var board = puzzleData.Board.CreateBoard(puzzleObj.transform);

            #region Create Pieces
            GameObject piecesObj = new GameObject("Pieces");
            piecesObj.transform.parent = puzzleObj.transform;

            var pieceList = new List<Piece>();

            if(puzzleData.Pieces != null)
            {
                foreach (PieceData pieceData in puzzleData.Pieces)
                {
                    pieceList.Add(pieceData.CreatePiece(piecesObj.transform, board));
                }
            }
            #endregion

            #region Create Tiles
            GameObject tilesObj = new GameObject("Tiles");
            tilesObj.transform.parent = puzzleObj.transform;

            var tileList = new List<Tile>();

            if(puzzleData.Tiles != null)
            {
                foreach (TileData tileData in puzzleData.Tiles)
                {
                    tileList.Add(tileData.CreateTile(tilesObj.transform, board));
                }
            }
            #endregion

            // Initialiaze Puzzle
            puzzle.Initialize(board, pieceList, tileList);

            return puzzle;
        }
    }

}
