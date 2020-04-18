using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Eat : SeekTarget
    {
        new public Candy Target { get; private set; }

        private bool GonnaEat = false;

        private bool Removed = false;

        public Eat(Candy target) : base(target) 
        {
            this.Target = target;
        }

        public Eat(Candy target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
        {
            this.Target = target;

            this.GonnaEat = target.Coords == tcoords;
        }

        //
        // - Action Methods
        //

        override public Action Available(Agent agent)
        {
            SeekTarget baseAction = (SeekTarget) base.Available(agent);

            if(baseAction != null)
            {
                return new Eat((Candy) baseAction.Target, baseAction.TargetCoords, baseAction.Direction);
            }

            return null;
        }

        override public bool Execute(Agent agent)
        {
            if(this.GonnaEat)
            {
                if(!this.Removed)
                {
                    this.Removed = agent.Board.RemovePiece(this.Target.Coords);
                }

                if(this.RotateAgent(agent))
                {
                    if(this.MoveAgent(agent))
                    {
                        GameObject.Destroy(this.Target.gameObject);

                        return true;
                    }
                }
            }
            else
            {
                return base.Execute(agent);
            }

            return false;
        }
    }
}