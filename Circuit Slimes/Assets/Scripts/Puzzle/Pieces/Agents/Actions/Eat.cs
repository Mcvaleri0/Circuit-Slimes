using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Eat : SeekTarget
    {
        new public Candy Target { get; protected set; }

        protected bool GonnaEat = false;

        protected bool Removed = false;

        public Eat(Candy target) : base(target) 
        {
            this.Target = target;
        }

        public Eat(Candy target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
        {
            this.Target = target;

            this.GonnaEat = target.Coords == tcoords;
        }


        #region === Action Methods ===
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
                if (!this.Removed)
                {
                    agent.Puzzle.RemovePiece(this.Target);
                    this.Removed = true;
                }

                if (agent.Rotate(this.Direction))
                {
                    if(agent.Move(this.TargetCoords))
                    {
                        GameObject.Destroy(this.Target.gameObject);

                        agent.Stats.Food++;

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
        #endregion
    }
}