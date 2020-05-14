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
        public Consume(Piece.Caracteristics targetCaracterization) : base(targetCaracterization) { }

        public Consume(Piece.SlimeTypes slimeType) :
            base(new Piece.Caracteristics(Piece.Categories.Slime, slimeType))
        { }
        public Consume(Piece.ComponentTypes componentType) :
            base(new Piece.Caracteristics(Piece.Categories.Component, componentType))
        { }

        public Consume(Piece.CandyTypes candyType) :
            base(new Piece.Caracteristics(Piece.Categories.Candy, candyType))
        { }

        public Consume(Agent agent, Piece target) : base(agent, target) 
        {
            this.Target = target;

            this.Consuming = this.TargetCoords == this.MoveCoords;
        }
        #endregion


        #region === Action Methods ===
        override public Action Available(Agent agent)
        {
            SeekTarget baseAction = (SeekTarget) base.Available(agent);

            if(baseAction != null)
            {
                return new Consume(agent, baseAction.Target);
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if(this.Consuming)
            {
                if(agent.PieceAt(this.TargetCoords) == this.Target)
                {
                    return true;
                }
            }
            else
            {
                return base.Confirm(agent);
            }

            return false;
        }

        override public bool Execute(Agent agent)
        {
            if(this.Consuming)
            {
                if (!this.Removed)
                {
                    agent.RemovePiece(this.MoveCoords);
                    this.Removed = true;
                }

                var rotated = agent.Rotate(this.Direction);

                var moved = agent.Move(this.MoveCoords);

                if (rotated && moved)
                {
                    var pos = this.Target.transform.position;

                    pos.y = -2f;

                    this.Target.transform.position = pos;

                    agent.Stats.Food++;

                    return true;
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
                    var pos = this.Target.transform.position;

                    pos.y = 1f;

                    agent.PlacePiece(this.Target, this.MoveCoords);

                    this.Target.transform.position = pos;

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
    }
}