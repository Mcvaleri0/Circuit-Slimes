using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardBuilder
{

    //BOARD PROPERTIES
    private static readonly float BoardThickness = 0.5f;
    private static readonly float BordersWidth = 0.5f;
    private static readonly float YLevel = 1;

    private class Line
    {
        public float x0 = 0;
        public float y0 = YLevel;
        public float z0 = 0;

        public float x1 = 0;
        public float y1 = YLevel;
        public float z1 = 0;

        public Line(float x0, float z0, float x1, float z1)
        {
            this.x0 = x0;
            this.z0 = z0;
            this.x1 = x1;
            this.z1 = z1;
        }
    }

    private class Triangle
    {
        public float x0 = 0;
        public float y0 = 0;
        public float x1 = 0;
        public float y1 = 0;
        public float x2 = 0;
        public float y2 = 0;

        public Triangle(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2
;
        }

        public void Build(List<Vector3> vertices, List<int> triangles)
        {
            var n = vertices.Count;

            vertices.Add(new Vector3(x0, YLevel, y0));
            vertices.Add(new Vector3(x1, YLevel, y1));
            vertices.Add(new Vector3(x2, YLevel, y2));

            triangles.Add(n + 0);
            triangles.Add(n + 1);
            triangles.Add(n + 2);
        }
    }

    private class RectangleQuad{

        public float x0 = 0;
        public float y0 = 0;
        public float y1 = 0;
        public float x1 = 0;

        public RectangleQuad(float x0, float y0, float x1, float y1)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
        }

        public void Build(List<Vector3> vertices, List<int> triangles)
        {
            var n = vertices.Count;

            vertices.Add(new Vector3(x0, YLevel, y1));
            vertices.Add(new Vector3(x1, YLevel, y1));
            vertices.Add(new Vector3(x0, YLevel, y0));
            vertices.Add(new Vector3(x1, YLevel, y0));

            triangles.Add(n + 0);
            triangles.Add(n + 1);
            triangles.Add(n + 2);
            triangles.Add(n + 2);
            triangles.Add(n + 1);
            triangles.Add(n + 3);
        }
    }

    private class Strip
    {
        public List<Line> lines = new List<Line>();

        public void AddLine(Line line)
        {
            lines.Add(line);
        }

        public int[] Build(List<Vector3> vertices, List<int> triangles)
        {
            if (lines.Count <= 1) return null;

            //verts is the id of the first and last vertex
            var verts = new int[2];
            verts[0] = vertices.Count;

            //first line
            var line = lines[0];
            vertices.Add(new Vector3(line.x0, line.y0, line.z0));
            vertices.Add(new Vector3(line.x1, line.y1, line.z1));

            for (var i = 1; i < lines.Count; i++)
            {
                line = lines[i];
                vertices.Add(new Vector3(line.x0, line.y0, line.z0));
                vertices.Add(new Vector3(line.x1, line.y1, line.z1));

                var n = vertices.Count;

                triangles.Add(n - 4);
                triangles.Add(n - 3);
                triangles.Add(n - 2);
                triangles.Add(n - 2);
                triangles.Add(n - 3);
                triangles.Add(n - 1);
            }

            verts[1] = vertices.Count -1;
            return verts;
        }
    }


    private class LineGrid
    {
        public enum Border
        {
            Left,
            Top,
            Right,
            Bottom
        }

        public enum Direction
        {
            W, NW, N, NE, E, SE, S, SW
        }

        public Dictionary<Direction, Vector2Int> Directions = new Dictionary<Direction, Vector2Int>{
            { Direction.W,  new Vector2Int( 0, -1) },
            { Direction.NW, new Vector2Int(-1, -1) },
            { Direction.N,  new Vector2Int(-1,  0) },
            { Direction.NE, new Vector2Int(-1,  1) },
            { Direction.E,  new Vector2Int( 0,  1) },
            { Direction.SE, new Vector2Int( 1,  1) },
            { Direction.S,  new Vector2Int( 1,  0) },
            { Direction.SW, new Vector2Int( 1, -1) },
        };

        private int[,] grid;
        public int h { get; private set; }
        public int w { get; private set; }

        public LineGrid(int h, int w)
        {
            this.h = h;
            this.w = w;
            this.grid = new int[h, w];
        }

        public int Get(int i, int j)
        {
            return grid[i, j];
        }

        public void Set(int i, int j, int val)
        {
            grid[i, j] = val;
        }

        public bool IsOutOfBounds(int i, int j)
        {
            if(i < 0 || i >= h || j < 0 || j >= w)
            {
                return true; 
            }
            return false;
        }

        public bool IsFree(int i, int j)
        {
            if(IsOutOfBounds(i, j))
            {
                return false;
            }
            return (Get(i, j) == 0);
        }

        public int GetAdjacent(int i, int j, Direction dir)
        {
            var ti = 0 + Directions[dir].x;
            var tj = 0 + Directions[dir].y; 

            if (ti != -1 && tj != -1 && !IsOutOfBounds(ti, tj))
            {
                return grid[i, j]; 
            }
            return -1;
        }

        public List<float> ScoreDirs(List<Direction> dirs, Vector2 v)
        {
            var res = new List<float>();
            foreach (var d in dirs)
            {
                var dv = Directions[d];
                float angle = Mathf.Atan2(dv.y - v.x, dv.x - v.y);
                res.Add(angle);
            }

            return res;
        }

        public void Print()
        {
            var res = "";

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    res += grid[i, j] + "\t";
                }
                res += "\n";
            }

            Debug.Log(res);
        }
    }


 
    public static List<int> GenerateRandom(int count, int min, int max)
    {
        if (max <= min || count < 0 || (count > max - min && max - min > 0))
        {
            return null;
        }

        // generate count random values.
        HashSet<int> candidates = new HashSet<int>();

        // start count values before max, and end at max
        for (int top = max - count; top < max; top++)
        {
            // May strike a duplicate.
            // Need to add +1 to make inclusive generator
            // +1 is safe even for MaxVal max value because top < max
            if (!candidates.Add((int) Random.Range(min, top + 1)))
            {
                // collision, add inclusive max.
                // which could not possibly have been added before.
                candidates.Add(top);
            }
        }

        // load them in to a list, to sort
        List<int> result = candidates.ToList();
        result.Sort();

        return result;
    }


    private static List<int>[] GeneratePorts(int width, int height) {

        //generate ports randomly for all borders
        var minPortsH = 1;
        var maxPortsH = width / 2;
        var minPortsV = 1;
        var maxPortsV = height / 2;

        var numLeftPorts = (int)Random.Range(minPortsV, maxPortsV);
        var numTopPorts = (int)Random.Range(minPortsH, maxPortsH);
        var numRightPorts = (int)Random.Range(minPortsV, maxPortsV);
        var numBottomPorts = (int)Random.Range(minPortsH, maxPortsH);

        var leftPorts = GenerateRandom(numLeftPorts, 1, height-1);
        var topPorts = GenerateRandom(numTopPorts, 1, width-1);
        var rightPorts = GenerateRandom(numRightPorts, 1, height-1);
        var bottomPorts = GenerateRandom(numBottomPorts, 1, width-1);

        return new List<int>[4]{leftPorts, topPorts, rightPorts, bottomPorts};
    }


    private static void PopulatePorts(LineGrid grid, int subspaces, List<int>[] allPorts, int numLines)
    {
        //LEFT
        var ports = allPorts[0];
        foreach(var i in ports)
        {
            for(var j = 0; j < subspaces; j++)
            {
                grid.Set(i*subspaces + j, 0, 1);
            }
        }

        //TOP
        ports = allPorts[1];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                grid.Set(grid.h-1, i * subspaces + j, 1);
            }
        }

        //RIGHT
        ports = allPorts[2];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                grid.Set(i * subspaces + j, grid.w-1, 1);
            }
        }

        //BOTTOM
        ports = allPorts[3];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                grid.Set(0, i * subspaces + j, 1);
            }
        }
    }


    //Build the mesh for the base
    private static Mesh CreateBoardBaseMesh(int width, int height, float unitSize)
    {
        //Create the mesh
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //Create base
        var h = height * unitSize;
        var w = width * unitSize;
        var quad = new RectangleQuad(0, 0, w, h);
        quad.Build(vertices, triangles);

        #region  Create Borders

        //horizontal
        var topStrip = new Strip();
        var bottomStrip = new Strip();
        for (var i = 0; i < width + 1; i++)
        {
            var line = new Line(unitSize * i, h, unitSize * i, h + BordersWidth);
            topStrip.AddLine(line);

            line = new Line(unitSize * i, -BordersWidth, unitSize * i, 0);
            bottomStrip.AddLine(line);
        }
        var top = topStrip.Build(vertices, triangles);
        var bottom = bottomStrip.Build(vertices, triangles);

        //vertical
        var leftStrip = new Strip();
        var rightStrip = new Strip();
        for (var i = 0; i < height + 1; i++)
        {
            var line = new Line(0, unitSize * i, -BordersWidth, unitSize * i);
            leftStrip.AddLine(line);

            line = new Line(w + BordersWidth, unitSize * i, w, unitSize * i);
            rightStrip.AddLine(line);
        }
        var left = leftStrip.Build(vertices, triangles);
        var right = rightStrip.Build(vertices, triangles);

        #endregion

        #region Create Corners

        //left-top corner
        var v1 = left[1] - 1;
        var v2 = left[1];
        var v3 = top[0] + 1;
        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);

        //top-right corner
        v1 = top[1] - 1;
        v2 = top[1];
        v3 = right[1] - 1;
        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);

        //right-bottom corner
        v1 = right[0];
        v2 = bottom[1] - 1;
        v3 = right[0] + 1;
        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);

        //bottom-left corner
        v1 = bottom[0];
        v2 = left[0] + 1;
        v3 = bottom[0] + 1;
        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);

        #endregion

        #region Extrude

        //find outer edges
        var outerEdges = new List<Vector2Int>();

        //left
        for (var i = left[1]; i >= left[0]; i--)
        {
            if (i % 2 == 0) continue;
            if (i - 2 < left[0]) break;
            outerEdges.Add(new Vector2Int(i, i - 2));
        }

        //top
        for (var i = top[1]; i >= top[0]; i--)
        {
            if (i % 2 == 0) continue;
            if (i - 2 < top[0]) break;
            outerEdges.Add(new Vector2Int(i, i - 2));
        }

        //right
        for (var i = right[0]; i <= right[1]; i++)
        {
            if (i % 2 != 0) continue;
            if (i + 2 > right[1]) break;
            outerEdges.Add(new Vector2Int(i, i + 2));
        }

        //bottom
        for (var i = bottom[0]; i <= bottom[1]; i++)
        {
            if (i % 2 != 0) continue;
            if (i + 2 > bottom[1]) break;
            outerEdges.Add(new Vector2Int(i, i + 2));
        }

        //corners
        outerEdges.Add(new Vector2Int(top[0] + 1, left[1]));
        outerEdges.Add(new Vector2Int(right[1] - 1, top[1]));
        outerEdges.Add(new Vector2Int(bottom[1] - 1, right[0]));
        outerEdges.Add(new Vector2Int(left[0] + 1, bottom[0]));
        
        //extrude
        for (var i = 0; i < outerEdges.Count; i++)
        {
            var n = vertices.Count;

            var edge = outerEdges[i];
            var a0 = edge.x;
            var a1 = edge.y;

            //duplicate edge 
            var v = vertices[edge.x];
            v.y -= BoardThickness;
            vertices.Add(v);
            var a2 = n;

            v = vertices[edge.y];
            v.y -= BoardThickness;
            vertices.Add(v);
            var a3 = n + 1;

            triangles.Add(a0);
            triangles.Add(a1);
            triangles.Add(a2);
            triangles.Add(a2);
            triangles.Add(a1);
            triangles.Add(a3);
        }

        #endregion

        //set mesh properties
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    //Build All
    public static void Build(GameObject boardObj, int width, int height, float unitSize)
    {
        //board ports
        var allPorts = GeneratePorts(height, width);

        //grid
        var subspaces = 3;
        var gridh = height * subspaces;
        var gridw = width * subspaces;
        LineGrid grid = new LineGrid(gridh, gridw);

        //populate grid with port points
        var numLines = (int) ((allPorts[0].Count + allPorts[1].Count + allPorts[2].Count + allPorts[3].Count) * subspaces)/4;
        PopulatePorts(grid, subspaces, allPorts, numLines);
        grid.Print();


        //Create base mesh
        Mesh mesh = CreateBoardBaseMesh(width, height, unitSize);

        //parent transf
        var parent = boardObj.transform;
        var parentRenderer = parent.GetComponent<MeshRenderer>();
        parentRenderer.enabled = false;

        //Create the gameobject
        GameObject gameObject = new GameObject("MeshTest", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.transform.parent = parent;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        var mainMaterial = parentRenderer.material;
        gameObject.GetComponent<MeshRenderer>().material = mainMaterial;

        //add outline to board
        var outln = gameObject.AddComponent<QuickOutline>();
        outln.OutlineMode = QuickOutline.Mode.OutlineVisible;
        outln.OutlineColor = new Color32(7, 80, 73, 255);
        outln.OutlineWidth = 5;

        return;
    }
}
