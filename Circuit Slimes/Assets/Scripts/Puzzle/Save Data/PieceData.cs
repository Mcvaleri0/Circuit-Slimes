using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Data
{
    /// <summary>
    /// Piece's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class PieceData
    {
        public Vector2 Coords;

        public Piece.Categories Category;

        public Piece.SlimeTypes SlimeType;

        public Piece.ComponentTypes ComponentType;

        public PieceData(Piece piece)
        {
            this.Coords = piece.Coords;

            this.Category = piece.Category;

            this.SlimeType = piece.SlimeType;

            this.ComponentType = piece.ComponentType;
        }


        public Piece CreatePiece(Transform parent, LevelBoard board)
        {
            GameObject obj = null;

            switch(this.Category)
            {
                default:
                case Piece.Categories.None:
                    return null;

                case Piece.Categories.Slime:
                    obj = Piece.Instantiate(parent, this.SlimeType, this.Coords);

                    var slime = obj.GetComponent<Pieces.Slimes.Slime>();

                    slime.Initialize(board, this.Coords, this.SlimeType);

                    return slime;

                case Piece.Categories.Component:
                    obj = Piece.Instantiate(parent, this.ComponentType, this.Coords);

                    var component = obj.GetComponent<Pieces.Components.Component>();

                    component.Initialize(board, this.Coords, this.ComponentType);

                    return component;
            }
        }

    }

}
