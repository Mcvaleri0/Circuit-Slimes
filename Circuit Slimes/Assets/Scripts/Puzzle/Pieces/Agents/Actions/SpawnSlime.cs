using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class SpawnSlime : Action
    {
        public LevelBoard.Directions Direction { get; private set; }
        
        public Vector2 TargetCoords { get; private set; }
        public Vector3 TargetPosition { get; private set; }

        public SpawnSlime()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public SpawnSlime(LevelBoard.Directions dir, Vector2 tcoords, Vector3 tposition)
        {
            this.Direction = dir;

            this.TargetCoords = tcoords;

            this.TargetPosition = tposition;
        }

        //
        // - Action Methods
        //

        public override Action Available(Agent agent)
        {
            ArrayList freeSpots = CheckAdjacents(agent);

            if(freeSpots != null) //FIXME: here we should be checking for charge left
            {
                for(var i = 0; i < 4; i++)
                {
                    var dir = ((int) agent.Orientation + 3 + i) % 4;

                    if((bool) freeSpots[dir] == true)
                    {
                        var tdir = (LevelBoard.Directions) (dir * 2);

                        var tcoords = agent.Board.GetAdjacentCoords((int)agent.Coords.x, (int)agent.Coords.y, tdir);
                        
                        var tposition = LevelBoard.WorldCoords(tcoords);

                        return new SpawnSlime(tdir, tcoords, tposition);
                    }
                }
            }

            return null;
        }

        public override bool Execute(Agent agent)
        {
            //Piece.Instantiate(Puzzle.Pieces, Piece.SlimeTypes.Electric, new Vector2(this.TargetPosition.x, this.TargetPosition.z));

            return false;
        }

        //
        // - Aux Methods
        //

        private static ArrayList CheckAdjacents(Agent agent)
        {
            ArrayList adjacents = new ArrayList();

            var x = (int)agent.Coords.x;
            var y = (int)agent.Coords.y;

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