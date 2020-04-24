using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class DropSolder : Consume
    {
        private bool Dropped = false;

        public DropSolder() : base(Piece.CandyTypes.Solder) { }

        public DropSolder(Candy target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
        { }


        #region === Action Methods

        public override Action Available(Agent agent)
        {
            var consume = (Consume) base.Available(agent);

            if (consume != null)
            {
                if (agent.Board.GetTile(consume.TargetCoords) == null)
                {
                    return new DropSolder((Candy)consume.Target, consume.TargetCoords, consume.Direction);
                }
                else
                {
                    return consume;
                }
            }
            else
            {
                return null;
            }
        }


        override public bool Execute(Agent agent)
        {
            if(base.Execute(agent))
            {
                if (agent.Stats.Food >= agent.Stats.MaxFood)
                {
                    this.Dropped = true;

                    var tile = Tile.CreateTile(agent.Puzzle, agent.Coords, Tile.Types.Solder);

                    agent.Puzzle.AddTile(tile);

                    agent.Stats.Food--;
                }

                return true;
            }

            return false;
        }


        override public bool Undo(Agent agent)
        {
            if (this.Dropped) 
            {
                this.Dropped = false;

                var tile = agent.Board.GetTile(agent.Coords);

                agent.Puzzle.RemoveTile(tile);

                GameObject.Destroy(tile.gameObject);

                agent.Stats.Food++;
            }

            return base.Undo(agent);
        }
        #endregion
    }
}