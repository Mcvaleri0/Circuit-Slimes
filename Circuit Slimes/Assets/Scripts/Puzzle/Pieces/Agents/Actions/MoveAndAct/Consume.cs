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
        public Consume(Piece.Characteristics targetCharacterization) : base(targetCharacterization) { }

        public Consume(Piece.SlimeTypes slimeType) :
            base(new Piece.Characteristics(Piece.Categories.Slime, slimeType))
        { }
        public Consume(Piece.ComponentTypes componentType) :
            base(new Piece.Characteristics(Piece.Categories.Component, componentType))
        { }

        public Consume(Piece.CandyTypes candyType) :
            base(new Piece.Characteristics(Piece.Categories.Candy, candyType))
        { }

        public Consume(SeekTarget seek) : base(seek.MoveCoords, seek.Direction, seek.Target) 
        {
            this.Consuming = this.TargetCoords == this.MoveCoords;
        }
        #endregion


        #region === Action Methods ===
        override public Action Available(Agent agent)
        {
            SeekTarget baseAction = (SeekTarget) base.Available(agent);

            if(baseAction != null)
            {
                return new Consume(baseAction);
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            // If a Piece will be consumed
            if(this.Consuming)
            {
                // If the Target is still at its coordinates
                if(agent.PieceAt(this.TargetCoords) == this.Target)
                {
                    // Remove Target from the Board
                    agent.RemovePiece(this.TargetCoords);

                    // Move Agent to the Target's Board Position
                    agent.MoveInBoard(this.TargetCoords);

                    return true;
                }
            }
            else
            {
                // Confirm Move
                return base.Confirm(agent);
            }

            return false;
        }

        override public bool Execute(Agent agent)
        {
            // If a Piece will be consumed
            if (this.Consuming)
            {
                // Rotate Agent in the World
                var rotated = agent.Rotate(this.Direction);

                // Move Agent in the World
                var moved   = agent.Move(this.MoveCoords);

                // If Rotation and Movement are complete
                if (rotated && moved)
                {
                    // Hide Target
                    this.Target.Hide();

                    // Increment Food
                    agent.Stats.Food++;

                    return true;
                }
            }
            else
            {
                // Execute Movement
                return base.Execute(agent);
            }

            return false;
        }

        override public bool Undo(Agent agent)
        {
            // If a Piece was consumed
            if (this.Consuming)
            {
                // Decrement Food
                agent.Stats.Food--;

                // Reveal Target
                this.Target.Reveal();

                // Undo Movement
                base.Undo(agent);

                // Return Target to its spot on the Board
                agent.PlacePiece(this.Target, this.MoveCoords);

                return true;
            }
            else
            {
                // Undo Movement
                return base.Undo(agent);
            }
        }
        #endregion
    }
}