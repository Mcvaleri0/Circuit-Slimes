using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.PlayerLoop;
using System.Diagnostics.Tracing;

namespace Puzzle.Board
{
    public class Change
    {
        public enum Operations {
            None,
            Place,
            Remove
        }

        public Operations PieceOperation { get; private set; }

        public Piece Piece { get; private set; }

        public Operations TileOperation { get; private set; }

        public Tile Tile { get; private set; }

        public Vector2Int Coords { get; private set; }


        public Change(Operations pieceOperation, Piece piece, Vector2Int coords)
        {
            this.PieceOperation = pieceOperation;

            this.Piece = piece;

            this.Coords = coords;
        }

        public Change(Operations tileOperation, Tile tile, Vector2Int coords)
        {
            this.TileOperation = tileOperation;

            this.Tile = tile;

            this.Coords = coords;
        }

        public Change(Operations pieceOperation, Piece piece, Operations tileOperation, Tile tile, Vector2Int coords)
        {
            this.PieceOperation = pieceOperation;

            this.Piece = piece;

            this.TileOperation = tileOperation;

            this.Tile = tile;

            this.Coords = coords;
        }


        public bool Update(Operations operation, Piece piece)
        {
            var conflicting = this.PieceOperation != Operations.None && this.Conflict(this.PieceOperation, operation);

            if (!conflicting)
            {
                this.PieceOperation = operation;

                this.Piece = piece;
            }

            return conflicting;
        }

        public bool Update(Operations operation, Tile tile)
        {
            var conflicting = this.TileOperation != Operations.None && this.Conflict(this.TileOperation, operation);

            if(!conflicting)
            {
                this.TileOperation = operation;

                this.Tile = tile;
            }

            return conflicting;
        }

        private bool Conflict(Operations operation1, Operations operation2)
        {
            switch(operation1)
            {
                default:
                    return false;

                case Operations.Place:
                    switch(operation2)
                    {
                        default:
                            return true;

                        case Operations.Place:
                            return false;
                    }

                case Operations.Remove:
                    switch(operation2)
                    {
                        default:
                            return true;

                        case Operations.Remove:
                            return false;
                    }
            }
        }
    }
}