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

        public DropSolder(Agent agent, Candy target) : base(agent, target) 
        { }


        #region === Action Methods

        override public Action Available(Agent agent)
        {
            var consume = (Consume) base.Available(agent);

            if (consume != null)
            {
                if (agent.TileAt(consume.MoveCoords) == null)
                {
                    return new DropSolder(agent, (Candy)consume.Target);
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

                    agent.CreateTile(Tile.Types.Solder, agent.Coords);

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

                var tile = agent.TileAt(agent.Coords);

                agent.RemoveTile(agent.Coords);

                GameObject.Destroy(tile.gameObject);

                agent.Stats.Food++;
            }

            return base.Undo(agent);
        }
        #endregion
    }
}