using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class SeekTarget : Move
    {
        public Piece Target { get; private set; }

        public SeekTarget(Piece target) : base()
        {
            this.Target = target;
        }

        public SeekTarget(Piece target, Vector2Int targetCoords, LevelBoard.Directions direction)
        {
            this.Target = target;

            this.TargetCoords = targetCoords;

            this.TargetPosition = LevelBoard.WorldCoords(targetCoords);

            this.Direction = direction;
        }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            List<Piece> foundPieces = agent.PiecesInSight(3);

            if(foundPieces.Count > 0)
            {
                foreach (Piece piece in foundPieces) {
                    if (this.Target.TypeMatches(piece))
                    {
                        Vector2Int moveCoords = piece.Coords - agent.Coords;
                        moveCoords.x = (int) Mathf.Sign(moveCoords.x) * Mathf.Min(Mathf.Abs(moveCoords.x), 1);
                        moveCoords.y = (int) Mathf.Sign(moveCoords.y) * Mathf.Min(Mathf.Abs(moveCoords.y), 1);

                        Vector2Int targetCoords = agent.Coords + moveCoords;

                        LevelBoard.Directions dir = LevelBoard.GetDirection(agent.Coords, targetCoords);

                        return new SeekTarget(piece, targetCoords, dir);
                    }
                }
            }

            return null;
        }
        #endregion
    }
}