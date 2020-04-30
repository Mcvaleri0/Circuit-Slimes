using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class SeekTarget : Move
    {
        public Piece Target { get; protected set; }

        protected Piece.Caracteristics TargetCaracteristics;


        protected SeekTarget(Piece.Caracteristics caracteristics) : base()
        {
            this.TargetCaracteristics = caracteristics;
        }

        public SeekTarget(Piece target, Vector2Int targetCoords, LevelBoard.Directions direction)
        {
            this.Target = target;

            this.TargetCaracteristics = new Piece.Caracteristics(target.Caracterization.ToString());

            this.TargetCoords = targetCoords;

            this.Direction = direction;
        }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            List<Piece> foundPieces = agent.PiecesInSight(3);

            if(foundPieces.Count > 0)
            {
                foreach (Piece piece in foundPieces) {
                    if (this.TargetCaracteristics.Matches(piece.Caracterization))
                    {
                        int dX = piece.Coords.x - agent.Coords.x;
                        int dY = piece.Coords.y - agent.Coords.y;

                        Vector2Int moveCoords = piece.Coords - agent.Coords;
                        moveCoords.x = (int) Mathf.Sign(moveCoords.x) * Mathf.Min(Mathf.Abs(moveCoords.x), 1);
                        moveCoords.y = (int) Mathf.Sign(moveCoords.y) * Mathf.Min(Mathf.Abs(moveCoords.y), 1);

                        Vector2Int targetCoords = agent.Coords + moveCoords;

                        if (!agent.IsFree(targetCoords) && piece.Coords != targetCoords)
                        {
                            if (Mathf.Abs(dX) > Mathf.Abs(dY))
                            {
                                targetCoords.y = agent.Coords.y;
                            }
                            else
                            {
                                targetCoords.x = agent.Coords.x;
                            }
                        }

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