using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Puzzle.Board
{
    public class LevelBoard
    {
        public const int SpaceSize = 2;

        private Dictionary<int, Row> Rows;

        public enum Directions
        {
            None = -1,
            Right = 0,
            UpRight = 1,
            Up = 2,
            UpLeft = 3,
            Left = 4,
            DownLeft = 5,
            Down = 6,
            DownRight = 7
        }

        public int Width  { get; private set; }
        public int Height { get; private set; }

        public GameObject Plane;

        public LevelBoard(int width, int height)
        {
            this.Width  = width;
            this.Height = height;

            this.Rows = new Dictionary<int, Row>();
        }


        #region === Piece Methods ===
        public void PlacePiece(int x, int y, Piece piece)
        {
            if (OutOfBounds(x, y))
            {
                throw new System.Exception("Piece out of bounds");
            }

            Row row;
            try
            {
                row = this.Rows[y];
            }
            catch (KeyNotFoundException)
            {
                row = new Row();
                this.Rows[y] = row;
            }

            row.PlacePiece(x, piece);
            piece.Coords = new Vector2(x, y);
        }

        public Piece RemovePiece(int x, int y)
        {
            var row = this.Rows[y];

            if (row != null)
            {
                var piece = row.RemovePiece(x);

                piece.Coords = new Vector2(-1, -1);

                return piece;
            }

            return null;
        }

        public Piece GetPiece(int x, int y)
        {
            Row row = null;
            this.Rows.TryGetValue(y, out row);

            if(row != null)
            {
                return row.GetPiece(x);
            }

            return null;
        }

        public bool MovePiece(int x, int y, Piece piece)
        {
            if(this.GetPiece(x, y) == null)
            {
                var foundPiece = this.RemovePiece((int) piece.Coords.x, (int) piece.Coords.y);

                //FIXME - You should check that what you found is what you were meant to find

                this.PlacePiece(x, y, foundPiece);

                foundPiece.Coords = new Vector2(x, y);

                return true;
            }

            return false;
        }
        #endregion


        #region === Tile Methods ===
        public void PlaceTile(int x, int y, Tile tile)
        {
            if (OutOfBounds(x, y))
            {
                throw new System.Exception("Piece out of bounds");
            }

            Row row;
            try
            {
                row = this.Rows[y];
            }
            catch (KeyNotFoundException)
            {
                row = new Row();
                this.Rows[y] = row;
            }

            row.PlaceTile(x, tile);
            tile.Coords = new Vector2(x, y);
        }

        public Tile RemoveTile(int x, int y)
        {
            var row = this.Rows[y];

            if (row != null)
            {
                var tile = row.RemoveTile(x);

                tile.Coords = new Vector2(-1, -1);

                return tile;
            }

            return null;
        }

        public Tile GetTile(int x, int y)
        {
            Row row = null;
            this.Rows.TryGetValue(y, out row);

            if (row != null)
            {
                return row.GetTile(x);
            }

            return null;
        }

        public List<Tile> GetAllTiles()
        {
            var tiles = new List<Tile>();

            var rows = new List<Row>(this.Rows.Values);

            foreach(var row in rows)
            {
                tiles.AddRange(row.GetAllTiles());
            }

            return tiles;
        }
        #endregion


        public void Draw(Transform parent, int width, int height)
        {
            // FIXME: create prefabs
            GameObject boardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boardObj.name = "Board";
            boardObj.transform.parent = parent;
            boardObj.transform.localPosition = new Vector3(width / 2, 0.5f, height / 2);
            boardObj.transform.localScale = new Vector3(width, 1, height);

            this.Plane = boardObj;
        }


        #region === Utility ===
        public bool OutOfBounds(int x, int y)
        {
            if (x < 0 || x >= this.Width) return true;
            if (y < 0 || y >= this.Height) return true;

            return false;
        }

        // Returns the coordinates of the space next to the one provided in a direction
        public Vector2 GetAdjacentCoords(Vector2 position, Directions direction)
        {
            switch (direction)
            {
                case Directions.Right:
                    position.x += 1;
                    break;

                case Directions.UpRight:
                    position.x += 1;
                    position.y -= 1;
                    break;

                case Directions.Up:
                    position.y -= 1;
                    break;

                case Directions.UpLeft:
                    position.x -= 1;
                    position.y -= 1;
                    break;

                case Directions.Left:
                    position.x -= 1;
                    break;

                case Directions.DownLeft:
                    position.x -= 1;
                    position.y += 1;
                    break;

                case Directions.Down:
                    position.y += 1;
                    break;

                case Directions.DownRight:
                    position.x += 1;
                    position.y += 1;
                    break;
            }

            if (!OutOfBounds((int)position.x, (int)position.y))
            {
                return position;
            }

            return Vector2.positiveInfinity;
        }

        public Vector2 GetAdjacentCoords(int x, int y, Directions direction)
        {
            return this.GetAdjacentCoords(new Vector2(x, y), direction);
        }

        public static Vector2 Discretize(Vector3 position)
        {
            return new Vector2(Mathf.Floor(position.z / SpaceSize), Mathf.Floor(position.x / SpaceSize));
        }

        public static Vector3 WorldCoords(Vector2 coords)
        {
            return new Vector3((coords.y + 0.5f) * SpaceSize, 1f, (coords.x + 0.5f) * SpaceSize);
        }
        #endregion
    }
}
