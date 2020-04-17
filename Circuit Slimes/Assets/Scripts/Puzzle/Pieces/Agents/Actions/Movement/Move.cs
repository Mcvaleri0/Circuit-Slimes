using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;
using Puzzle.Board;

namespace Puzzle.Actions
{
    public class Move : Action
    {
        public LevelBoard.Directions Direction { get; protected set; }

        public Vector2Int TargetCoords { get; protected set; }

        public Vector3 TargetPosition { get; protected set; }


        public Move()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public Move(LevelBoard.Directions dir, Vector2Int coords, Vector3 position)
        {
            this.Direction = dir;

            this.TargetCoords = coords;

            this.TargetPosition = position;
        }


        public override Action Available(Agent agent)
        {
            return null;
        }

        public override bool Execute(Agent agent)
        {
            if(RotateAgent(agent))
            {
                return MoveAgent(agent);
            }
            
            return false;
        }


        #region === AUX Methods ===
        protected bool RotateAgent(Agent agent)
        {
            float currentAngle = agent.transform.eulerAngles.y;
            float targetAngle  = 360 - ((int) this.Direction) * 45;
            if (targetAngle == 360) targetAngle = 0;

            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 0.33f);

            if (Mathf.Abs((currentAngle % 360) - targetAngle) < 0.1f) currentAngle = targetAngle;

            agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x, currentAngle, agent.transform.eulerAngles.z);

            if(currentAngle == targetAngle)
            {
                // Update their Orientation
                agent.Orientation = this.Direction;

                return true;
            }

            return false;
        }

        protected bool MoveAgent(Agent agent)
        {
            var maxVelocity = agent.Stats.Speed / 100f;

            var currentPosition = agent.transform.position;

            // If the distance to the Target Position exceeds what can be traveled in one Step
            if (Vector3.Distance(currentPosition, this.TargetPosition) > maxVelocity)
            {
                var dX = this.TargetPosition.x - currentPosition.x; // Distance to travel (North - South)
                var dZ = this.TargetPosition.z - currentPosition.z; // Distance to travel (West  - East)
                
                var norm = (new Vector3(dX, 0, dZ)).normalized; // Normalize the distance vector;

                agent.transform.position += norm * maxVelocity; // Apply movement

                return false;
            }

            // If the Agent is within a one Step distance of the Target Position
            else
            {
                agent.transform.position = this.TargetPosition; // Set their position to the Target Position

                // Move the Piece on the Board
                agent.Board.MovePiece(this.TargetCoords, agent);

                return true;
            }
        }
        #endregion
    }
}