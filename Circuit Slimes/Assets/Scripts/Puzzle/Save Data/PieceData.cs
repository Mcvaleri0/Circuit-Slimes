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
        public Vector2Int Coords;

        public Piece.Categories Category;

        public Piece.SlimeTypes SlimeType;

        public Piece.ComponentTypes ComponentType;

        public Piece.CandyTypes CandyType;

        public PieceData(Piece piece)
        {
            this.Coords = piece.Coords;

            this.Category = piece.Category;

            this.SlimeType = piece.SlimeType;

            this.ComponentType = piece.ComponentType;

            this.CandyType = piece.CandyType;
        }


        public Piece CreatePiece(Transform parent, LevelBoard board)
        {
            GameObject obj;

            switch(this.Category)
            {
                default:
                case Piece.Categories.None:
                    return null;

                case Piece.Categories.Slime:
                    obj = Piece.Instantiate(parent, this.SlimeType, this.Coords);

                    var slime = obj.GetComponent<Pieces.Slimes.Slime>();

                    if(slime != null)
                    {
                        slime.Initialize(board, this.Coords, this.SlimeType);
                    }
                    
                    return slime;

                case Piece.Categories.Component:
                    obj = Piece.Instantiate(parent, this.ComponentType, this.Coords);

                    var component = obj.GetComponent<Pieces.Components.Component>();

                    if(component != null)
                    {
                        component.Initialize(board, this.Coords, this.ComponentType);
                    }
                    
                    return component;

                case Piece.Categories.Candy:
                    obj = Piece.Instantiate(parent, this.CandyType, this.Coords);

                    var candy = obj.GetComponent<Pieces.Candy>();

                    if(candy != null)
                    {
                        candy.Initialize(board, this.Coords, this.CandyType);
                    }

                    return candy;
            }
        }

    }

}
