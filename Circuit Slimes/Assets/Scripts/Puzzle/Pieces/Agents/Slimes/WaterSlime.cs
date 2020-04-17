using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;

namespace Puzzle.Pieces.Slimes
{
    public class WaterSlime : Slime
    {

        //
        // - Unity Events
        //

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new SeekTarget(new Candy(CandyTypes.Water)));

            this.Stats = new Statistics(10, 10, 10);
        }

        override protected void Update()
        {
            base.Update();

            if(Input.GetKeyUp(KeyCode.T))
            {
                List<Piece> inSight = this.PiecesInSight(3);

                foreach(Piece piece in inSight)
                {
                    Debug.Log(piece.Coords);
                }
            }
        }

        //
        // - Slime Methods
        //

        override public Action Think()
        {
            foreach(var action in this.KnownActions)
            {
                return action.Available(this);
            }

            return null;
        }
    }
}