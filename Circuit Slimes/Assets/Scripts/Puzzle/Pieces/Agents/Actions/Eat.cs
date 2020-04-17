using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Eat : Action
    {
        public LevelBoard.Directions Direction { get; private set; }
        
        public Vector2 TargetCoords { get; private set; }
        public Vector3 TargetPosition { get; private set; }

        public Eat()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public Eat(LevelBoard.Directions dir, Vector2 tcoords, Vector3 tposition)
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
            ArrayList freeSpots = CheckCrossAdjacents(agent);

            if(freeSpots != null) //FIXME: here we should be checking for charge left
            {
                for(var i = 0; i < 4; i++)
                {
                    var dir = ((int) agent.Orientation + 3 + i) % 4;

                    if((bool) freeSpots[dir] == true)
                    {
                        var tdir = (LevelBoard.Directions) (dir * 2);

                        var tcoords = agent.Board.GetAdjacentCoords(agent.Coords, tdir);
                        
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
    }
}