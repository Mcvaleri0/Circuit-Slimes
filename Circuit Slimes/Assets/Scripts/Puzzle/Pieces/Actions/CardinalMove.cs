using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class CardinalMove : Action
    {
        public LevelBoard.Directions Direction { get; private set; }
        
        public Vector2 TargetCoords { get; private set; }
        public Vector3 TargetPosition { get; private set; }

        public CardinalMove()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public CardinalMove(LevelBoard.Directions dir, Vector2 coords, Vector3 position)
        {
            this.Direction = dir;

            this.TargetCoords = coords;

            this.TargetPosition = position;
        }

        //
        // - Action Methods
        //

        public override Action Available(Agent agent)
        {
            ArrayList freeSpots = CheckAdjacents(agent);

            if(freeSpots != null)
            {
                var start = ((int) agent.Orientation) / 2;

                for (var i = 0; i < 4; i++)
                {
                    var ind = (start + i + 3) % 4;

                    if((bool) freeSpots[ind] == true)
                    {
                        var dir = (LevelBoard.Directions) (ind * 2);

                        var tcoords = agent.Board.GetAdjacentCoords(agent.Coords, dir);

                        if (tcoords.sqrMagnitude > 1000f) continue;
                        
                        var tpos = LevelBoard.WorldCoords(tcoords);

                        Debug.Log(Random.value + " - Chosen Direction: " + dir + " | Target Coords: " + tcoords + " | Target Position: " + tpos);

                        return new CardinalMove(dir, tcoords, tpos);
                    }
                }
            }

            return null;
        }

        public override bool Execute(Agent agent)
        {
            var current = agent.transform.position;

            var angle = ((int) this.Direction) * -45;

            agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x, angle, agent.transform.eulerAngles.z);

            if (Vector3.Distance(current, this.TargetPosition) > agent.Stats.Speed / 100f)
            {
                var dX = this.TargetPosition.x - current.x;
                var dZ = this.TargetPosition.z - current.z;

                var norm = (new Vector2(dX, dZ)).normalized;

                current.x += norm.x * (agent.Stats.Speed / 100f);
                current.z += norm.y * (agent.Stats.Speed / 100f);
            }
            else
            {
                current.x = this.TargetPosition.x;
                current.z = this.TargetPosition.y;

                agent.Board.MovePiece((int) this.TargetCoords.x, (int) this.TargetCoords.y, agent);

                agent.Orientation = this.Direction;

                return true;
            }

            agent.transform.position = current;

            return false;
        }

        //
        // - Aux Methods
        //

        private static ArrayList CheckAdjacents(Agent agent)
        {
            ArrayList adjacents = new ArrayList();

            var x = (int) agent.Coords.x;
            var y = (int) agent.Coords.y;

            // Right
            if (agent.IsFree(x + 1, y))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Up
            if (agent.IsFree(x, y - 1))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Down
            if (agent.IsFree(x, y + 1))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Left
            if (agent.IsFree(x - 1, y))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            if (adjacents.Count == 0) return null;

            return adjacents;
        }
    }
}