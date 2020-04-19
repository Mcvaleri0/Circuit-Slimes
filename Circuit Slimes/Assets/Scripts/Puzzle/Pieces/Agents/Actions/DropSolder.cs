using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class DropSolder : SeekTarget
    {
        new public Candy Target { get; private set; }

        private bool GonnaEat = false;

        private bool Removed = false;

        public DropSolder(Candy target) : base(target) 
        {
            this.Target = target;
        }

        public DropSolder(Candy target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
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

                if(agent.Rotate(this.Direction))
                {
                    if(agent.Move(this.TargetCoords))
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