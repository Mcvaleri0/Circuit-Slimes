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
        public BoardData Board;
        public PieceData[] Pieces;


        public PuzzleData(Puzzle puzzle)
        {
            this.Board = new BoardData(puzzle.Board);

            int nPieces = puzzle.Pieces.Count;
            this.Pieces = new PieceData[nPieces];
            for (int i = 0; i < nPieces; i++)
            {
                this.Pieces[i] = new PieceData(puzzle.Pieces[i]);
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

            PuzzleData puzzleData = JsonUtility.FromJson<PuzzleData>(jsonData);

            LevelBoard board = puzzleData.Board.CreateBoard();
            
            GameObject puzzleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Puzzle"));
            var puzzle = puzzleObj.GetComponent<Puzzle>();

            GameObject piecesObj = new GameObject("Pieces");
            piecesObj.transform.parent = puzzleObj.transform;

            var pieceList = new List<Piece>();

            foreach (PieceData pieceData in puzzleData.Pieces)
            {
                pieceList.Add(pieceData.CreatePiece(piecesObj.transform, board));
            }

            puzzle.Initialize(board, pieceList);

            return puzzle;
        }
    }

}
