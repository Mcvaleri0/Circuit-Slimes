using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class DropSolder : Eat
    {
        public DropSolder() : base(new Candy(Piece.CandyTypes.Solder)) { }

        public DropSolder(Candy target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
        {
            this.Target = target;

            this.GonnaEat = target.Coords == tcoords;
        }


        #region === Action Methods

        public override Action Available(Agent agent)
        {
            var eat = (Eat) base.Available(agent);

            if (eat != null)
            {
                return new DropSolder(eat.Target, eat.TargetCoords, eat.Direction);
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
                    var tile = Tile.CreateTile(null, agent.Puzzle, agent.Coords, Tile.Types.Solder);

                    agent.Puzzle.AddTile(tile);

                    agent.Stats.Food--;
                }

                return true;
            }

            return false;
        }
        #endregion
    }
}