using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Consume : SeekTarget
    {
        protected bool Consuming = false;

        protected bool Removed = false;


        #region === Constructors ===
        public Consume(Piece target) : base(target) { }

        public Consume(Piece.SlimeTypes slimeType) :
            base(new Piece.Caracteristics(Piece.Categories.Slime, slimeType))
        { }
        public Consume(Piece.ComponentTypes componentType) :
            base(new Piece.Caracteristics(Piece.Categories.Component, componentType))
        { }

        public Consume(Piece.CandyTypes candyType) :
            base(new Piece.Caracteristics(Piece.Categories.Candy, candyType))
        { }

        public Consume(Piece target, Vector2Int tcoords, LevelBoard.Directions dir) : base(target, tcoords, dir) 
        {
            this.Target = target;

            this.Consuming = target.Coords == tcoords;
        }
        #endregion


        #region === Action Methods ===
        override public Action Available(Agent agent)
        {
            SeekTarget baseAction = (SeekTarget) base.Available(agent);

            if(baseAction != null)
            {
                return new Consume(baseAction.Target, baseAction.TargetCoords, baseAction.Direction);
            }

            return null;
        }

        override public bool Execute(Agent agent)
        {
            if(this.Consuming)
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

        override public bool Undo(Agent agent)
        {
            if (this.Consuming)
            {
                base.Undo(agent);

                if (this.Removed)
                {
                    this.Target = this.RecreateTarget(agent.Puzzle);
                    agent.Puzzle.AddPiece(this.Target);
                    this.Removed = false;
                }

                agent.Stats.Food--;

                return true;
            }
            else
            {
                return base.Undo(agent);
            }
        }
        #endregion


        #region === Aux Methods ===
        protected Piece RecreateTarget(Puzzle puzzle)
        {
            var target = Piece.CreatePiece(puzzle, this.TargetCoords, this.TargetCaracteristics.ToString());

            return target;
        }
        #endregion
    }
}