using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class Tile : MonoBehaviour
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }


        public void Initialize(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
