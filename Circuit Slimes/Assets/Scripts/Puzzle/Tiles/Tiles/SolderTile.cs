using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Puzzle.Tiles
{
    public class SolderTile : Tile
    {
        //Adjacent info for mesh creation (RIGHT, DOWN, LEFT, UP)
        private BitArray AdjacentInfo = new BitArray(4, false);

        //adjacent info coded into an iny (binary notation)
        private enum TilesetNum
        {
            N = 0b0000,
            R = 0b0001,
            D = 0b1000,
            L = 0b0100,
            U = 0b0010,

            RD = 0b1001,
            RL = 0b0101,
            RU = 0b0011,
            DL = 0b1100,
            DU = 0b1010,
            LU = 0b0110,

            RDL = 0b1101,
            RLU = 0b0111,
            RDU = 0b1011,
            DLU = 0b1110,

            RDLU = 0b1111
        }

        private enum TileSetDir
        {
            Right = 0,
            Down = 90,
            Left = 180,
            Up   = 270 
        }

        //Mesh stuff
        private Transform  meshTransform;
        private MeshFilter meshFilter;

        private Mesh meshCircle;
        private Mesh meshLine;
        private Mesh meshCircleLine;
        private Mesh meshBend;
        private Mesh meshCross;
        private Mesh meshTShape;


        //convert bitArray to int
        private int getIntFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new System.ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }


        //update mesh
        private void UpdateMesh()
        {

            //update adjacent info
            var adjacent_tiles = (bool[]) this.checkCrossAdjacentsTiles(this.Coords, this.Type).ToArray(typeof(bool));

            AdjacentInfo = new BitArray( adjacent_tiles);
            var adjacent = getIntFromBitArray(AdjacentInfo);

            #region set Mesh Rotation

            switch (adjacent)
            {
                //circle
                case (int) TilesetNum.N:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0);  break;

                //circle-line
                case (int)TilesetNum.R:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Left,  0); break;
                case (int)TilesetNum.D:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Up,    0); break;
                case (int)TilesetNum.L:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0); break;
                case (int)TilesetNum.U:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Down,  0); break;

                //line
                case (int)TilesetNum.RL:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0); break;
                case (int)TilesetNum.DU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Up, 0);    break;

                //bend
                case (int)TilesetNum.RD:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Down, 0); break;
                case (int)TilesetNum.DL:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Left, 0); break;
                case (int)TilesetNum.LU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Up, 0); break;
                case (int)TilesetNum.RU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0); break;
                
                //t-shape
                case (int)TilesetNum.RDL:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Left,  0); break;
                case (int)TilesetNum.DLU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Up,  0); break;
                case (int)TilesetNum.RLU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0); break;
                case (int)TilesetNum.RDU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Down,    0); break;

                //cross
                case (int)TilesetNum.RDLU:
                    meshTransform.localEulerAngles = new Vector3(0, (int)TileSetDir.Right, 0);
                    break;
                
                default:
                    break;
            }

            #endregion

            #region Load Correct Mesh

            //circle
            if (adjacent == (int)TilesetNum.N ){

                meshFilter.mesh = meshCircle;
            }

            //circleline
            else if((adjacent == (int)TilesetNum.R) ||
                (adjacent == (int)TilesetNum.D) ||
                (adjacent == (int)TilesetNum.L) ||
                (adjacent == (int)TilesetNum.U) ){

                meshFilter.mesh = meshCircleLine;
            }

            //line
            else if((adjacent == (int)TilesetNum.RL) ||
                (adjacent == (int)TilesetNum.DU) ){

                meshFilter.mesh = meshLine;
            }

            //bend
            else if((adjacent == (int)TilesetNum.RD) ||
                (adjacent == (int)TilesetNum.DL) ||
                (adjacent == (int)TilesetNum.LU) ||
                (adjacent == (int)TilesetNum.RU) ){

                meshFilter.mesh = meshBend;
            }

            //tshape
            else if((adjacent == (int)TilesetNum.RDL) ||
                (adjacent == (int)TilesetNum.DLU) ||
                (adjacent == (int)TilesetNum.RLU) ||
                (adjacent == (int)TilesetNum.RDU) ){

                meshFilter.mesh = meshTShape;
            }

            //cross
            else if (adjacent == (int)TilesetNum.RDLU ){

                meshFilter.mesh = meshCross;
            }
            #endregion

        }


        //update tile
        public override void UpdateTile()
        {
            Debug.Log("This Happened");
            this.UpdateMesh();
        }


        #region === Unity Methods ===
       
        //Load Meshes
        private void Awake()
        {
            this.Type = Types.Solder;

            this.meshTransform = transform.GetChild(0);
            this.meshFilter = gameObject.GetComponentInChildren<MeshFilter>();

            this.meshCircle = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderCircle");
            this.meshBend = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderBend");
            this.meshCircleLine = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderCircleLine");
            this.meshCross = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderCross");
            this.meshLine = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderLine");
            this.meshTShape = Resources.Load<Mesh>("Meshes/Tiles/Solder/SolderTShape");
        }
        

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        #endregion

    }
}