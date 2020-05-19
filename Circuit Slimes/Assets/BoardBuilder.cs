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

    private static readonly int Subspaces = 3;
    private static readonly Color LineColor = new Color32(11, 222, 162, 255);
    private static readonly float LineWidth = 0.2f;


    private class Edge
    {
        public float x0 = 0;
        public float y0 = YLevel;
        public float z0 = 0;

        public float x1 = 0;
        public float y1 = YLevel;
        public float z1 = 0;

        public Edge(float x0, float z0, float x1, float z1)
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
        public List<Edge> edges = new List<Edge>();

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }

        public int[] Build(List<Vector3> vertices, List<int> triangles)
        {
            if (edges.Count <= 1) return null;

            //verts is the id of the first and last vertex
            var verts = new int[2];
            verts[0] = vertices.Count;

            //first line
            var edge = edges[0];
            vertices.Add(new Vector3(edge.x0, edge.y0, edge.z0));
            vertices.Add(new Vector3(edge.x1, edge.y1, edge.z1));

            for (var i = 1; i < edges.Count; i++)
            {
                edge = edges[i];
                vertices.Add(new Vector3(edge.x0, edge.y0, edge.z0));
                vertices.Add(new Vector3(edge.x1, edge.y1, edge.z1));

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
            var edge = new Edge(unitSize * i, h, unitSize * i, h + BordersWidth);
            topStrip.AddEdge(edge);

            edge = new Edge(unitSize * i, -BordersWidth, unitSize * i, 0);
            bottomStrip.AddEdge(edge);
        }
        var top = topStrip.Build(vertices, triangles);
        var bottom = bottomStrip.Build(vertices, triangles);

        //vertical
        var leftStrip = new Strip();
        var rightStrip = new Strip();
        for (var i = 0; i < height + 1; i++)
        {
            var edge = new Edge(0, unitSize * i, -BordersWidth, unitSize * i);
            leftStrip.AddEdge(edge);

            edge = new Edge(w + BordersWidth, unitSize * i, w, unitSize * i);
            rightStrip.AddEdge(edge);
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



    private class Line
    {
        private List<Vector2Int> Nodes;
        public int id = -1;
        public int target = -1;

        public Line(Vector2Int startnode, int id, int target)
        {
            this.Nodes = new List<Vector2Int>
            {
                startnode
            };
            this.id = id;
            this.target = target;
        }

        public int GetSize()
        {
            return this.Nodes.Count;
        }

        public Vector2Int GetNode(int ind)
        {
            return this.Nodes[ind];
        }

        public void AddNode(Vector2Int node)
        {
            Nodes.Add(node);
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public void DrawDebug(int w, int h, float realw, float realh)
        {
            var col = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
            );

            for (var i = 0; i < this.Nodes.Count-1; i++)
            {
                var n0 = this.Nodes[i];
                var n1 = this.Nodes[i+1];
                
                var pos1 = new Vector3(n0.x * (realw/w), YLevel, n0.y * (realh/h));
                var pos2 = new Vector3(n1.x * (realw/w), YLevel, n1.y * (realh/h));

                Debug.DrawLine(pos1, pos2, col, 5000, true);
            }
        }

        public void Build(GameObject obj, Material mat, int w, int h, float realw, float realh)
        {
            var numNodes = this.Nodes.Count;

            var lineobj = new GameObject();
            lineobj.transform.parent = obj.transform;
            lineobj.transform.localEulerAngles = new Vector3(90, 0, 0);

            var renderer = lineobj.AddComponent<LineRenderer>();
            renderer.alignment = LineAlignment.TransformZ;
            renderer.positionCount = numNodes;
            renderer.startWidth = LineWidth;
            renderer.material = mat;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            List<Vector3> nodePos = new List<Vector3>();
            for (var i = 0; i < numNodes; i++)
            {
                var n = this.Nodes[i];

                var pos = new Vector3(n.x * (realw / (w-1)) , YLevel + 0.005f, n.y * (realh / (h-1)));
                nodePos.Add(pos);
            }
            renderer.SetPositions(nodePos.ToArray());

            
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
            { Direction.W,  new Vector2Int(-1,  0) },
            { Direction.NW, new Vector2Int(-1,  1) },
            { Direction.N,  new Vector2Int( 0,  1) },
            { Direction.NE, new Vector2Int( 1,  1) },
            { Direction.E,  new Vector2Int( 1,  0) },
            { Direction.SE, new Vector2Int( 1, -1) },
            { Direction.S,  new Vector2Int( 0, -1) },
            { Direction.SW, new Vector2Int(-1, -1) },
        };

        private int[,] grid;
        private Dictionary<Vector2Int, Vector2Int> parents;

        public int w { get; private set; }
        public int h { get; private set; }
        public float realw{ get; private set; }
        public float realh { get; private set; }

        public List<Line> Lines;


        public LineGrid(int w, int h, float realw, float realh)
        {
            this.w = w;
            this.h = h;
            this.realw = realw;
            this.realh = realh;

            this.grid = new int[w,h];
            this.parents = new Dictionary<Vector2Int, Vector2Int>();

            this.Lines = new List<Line>();
        }


        public int Get(Vector2Int pos)
        {
            var i = pos.x;
            var j = pos.y;
            return grid[i, j];
        }

        public void Set(Vector2Int pos, int val)
        {
            var i = pos.x;
            var j = pos.y;
            grid[i, j] = val;
        }

        public void SetParent(Vector2Int pos, Vector2Int parent)
        {
            parents[pos] = parent;
        }

        public Vector2Int GetParent(Vector2Int pos)
        {
            return parents[pos];
        }


        public bool IsOutOfBounds(Vector2Int pos)
        {
            var i = pos.x;
            var j = pos.y;

            if (i < 0 || i >= w || j < 0 || j >= h)
            {
                return true; 
            }
            return false;
        }

        public bool IsFree(Vector2Int pos)
        {
            if(IsOutOfBounds(pos))
            {
                return false;
            }
            return (Get(pos) == 0);
        }

        public Vector2Int GetAdjacentFree(Vector2Int pos, Direction dir)
        {
            var ti = pos.x + Directions[dir].x;
            var tj = pos.y + Directions[dir].y;
            var tpos = new Vector2Int(ti, tj);

            if (!IsOutOfBounds(tpos) && IsFree(tpos))
            {
                return tpos; 
            }
            return new Vector2Int(-1,-1);
        }

        public List<Vector2Int> GetAllAdjacentFree(Vector2Int pos, Vector2Int targetpos)
        {
            List<Vector2Int> res = new List<Vector2Int>();

            var bad = new Vector2Int(-1, -1);
            
            var test = (Direction[]) System.Enum.GetValues(typeof(Direction));
            List<Direction> possibleDirections = new List<Direction>(test);

            //if in border only allow 1 direction
            if (pos.x == 0)     { possibleDirections = new List<Direction>() { Direction.E }; } 
            if (pos.x == w - 1) { possibleDirections = new List<Direction>() { Direction.W }; }

            if (pos.y == 0)     { possibleDirections = new List<Direction>() { Direction.N }; }
            if (pos.y == h - 1) { possibleDirections = new List<Direction>() { Direction.S }; }

            //if near border don't allow diagonals towards it
            if (pos.x == 1)
            {
                possibleDirections.Remove(Direction.NW);
                possibleDirections.Remove(Direction.SW);
            }
            if (pos.x == w - 2)
            {
                possibleDirections.Remove(Direction.NE);
                possibleDirections.Remove(Direction.SE);
            }
            if (pos.y == 1)
            {
                possibleDirections.Remove(Direction.SE);
                possibleDirections.Remove(Direction.SW);
            }
            if (pos.y == h - 2)
            {
                possibleDirections.Remove(Direction.NE);
                possibleDirections.Remove(Direction.NW);
            }

            foreach (var d in possibleDirections)
            {
                var diags = true;

                switch (d)
                {
                    case Direction.SE:
                        diags &= GetAdjacentFree(pos, Direction.S) != bad;
                        diags &= GetAdjacentFree(pos, Direction.E) != bad;
                        break;

                    case Direction.NE:
                        diags &= GetAdjacentFree(pos, Direction.N) != bad;
                        diags &= GetAdjacentFree(pos, Direction.E) != bad;
                        break;

                    case Direction.NW:
                        diags &= GetAdjacentFree(pos, Direction.N) != bad;
                        diags &= GetAdjacentFree(pos, Direction.W) != bad;
                        break;

                    case Direction.SW:
                        diags &= GetAdjacentFree(pos, Direction.S) != bad;
                        diags &= GetAdjacentFree(pos, Direction.W) != bad;
                        break;
                }

                var adj = GetAdjacentFree(pos, (Direction)d);
                if (diags && adj != bad)
                {
                    if (adj.x == 0 || adj.x == w - 1 || adj.y == 0 || adj.y == h - 1)
                    {
                        if (adj == targetpos)
                        {
                            res.Add(adj);
                        }
                    }
                    else
                    {
                        res.Add(adj);
                    }
                }
            }
            return res;
        }

        public Vector2Int GetBest(List<Vector2Int> list, Vector2Int target)
        {
            var minpos = new Vector2Int();
            var mindist = float.MaxValue;

            foreach (var p in list)
            {
                var dist = Vector2Int.Distance(p, target);
                if(dist < mindist)
                {
                    mindist = dist;
                    minpos = p;
                }
            }
            return minpos;
        }


        public void AddLine(Line line)
        {
            if (this.IsFree(line.GetNode(0)))
            {
                this.Lines.Add(line);
            }
        }

        public void TraceBackwards(Line line, Vector2Int finalpos)
        {
            
            var startpos = line.GetNode(0);
            var pos = finalpos;

            line.Clear();

            while (pos != startpos)
            {
                Set(pos, line.id);
                line.AddNode(pos);

                pos = GetParent(pos);
            }
            Set(pos, line.id);
            line.AddNode(pos);
        }

        public void DrawDebug()
        {
            foreach (var line in this.Lines)
            {
                line.DrawDebug(this.w, this.h, this.realw, this.realh);
            }
        }

        public void BuildLines(GameObject obj, Material mat)
        {
            foreach (var line in this.Lines)
            {
                line.Build(obj, mat, this.w, this.h, this.realw, this.realh);
            }
        }

        public Line ChooseTarget(Line line)
        {
            foreach (var l in this.Lines)
            {
                //check if it is a good candidate
                if (l.id == line.id || l.target != line.target)
                {
                    continue;
                }

                //check if target is valid
                if(l.GetSize() == 1 && IsFree(l.GetNode(0)))
                {
                    return l;
                }
            }
            //no target found
            return null;
        }

        public void Print()
        {
            var res = "";

            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    res += grid[i, j] + "\t";
                }
                res += "\n";
            }
            Debug.Log(res);            
        }
    }



    //Generate 'count' random numbers between 'min' and 'max' (max not inclusive)
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

    //Generate ports to be used in the borders of the board
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

    //Populate the line grid with line points from the ports
    private static void PopulatePorts(LineGrid grid, int subspaces, List<int>[] allPorts, int numLines)
    {
        var id = 1;

        //LEFT
        var target = 0;
        var ports = allPorts[0];
        foreach(var i in ports)
        {
            for(var j = 0; j < subspaces; j++)
            {
                var line = new Line(new Vector2Int(0, i * subspaces + j), id++, target);
                grid.AddLine(line);

                target = (target + 1) % numLines;
            }
        }

        //TOP
        target = 0;
        ports = allPorts[1];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                var line = new Line(new Vector2Int(i * subspaces + j, grid.h - 1), id++, target);
                grid.AddLine(line);

                target = (target + 1) % numLines;
            }
        }

        //RIGHT
        target = 0;
        ports = allPorts[2];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                var line = new Line(new Vector2Int(grid.w - 1, i * subspaces + j), id++, target);
                grid.AddLine(line);

                target = (target + 1) % numLines;
            }
        }

        //BOTTOM
        target = 0;
        ports = allPorts[3];
        foreach (var i in ports)
        {
            for (var j = 0; j < subspaces; j++)
            {
                var line = new Line(new Vector2Int(i * subspaces + j, 0), id++, target);
                grid.AddLine(line);

                target = (target + 1) % numLines;
            }
        }
    }


    //pathfind to connect individual Line
    private static void PathFindLine(LineGrid grid, Line line, Line target)
    {
        //target position
        var targetpos = target.GetNode(0);

        //list of nodes opened
        List<Vector2Int> open = new List<Vector2Int>{ line.GetNode(0) };

        //list of nodes closed
        List<Vector2Int> closed = new List<Vector2Int>();

        //pathfind
        do {
            var currentpos = grid.GetBest(open, targetpos);
            if(currentpos == targetpos)
            {
                //found a full path!
                grid.TraceBackwards(line, targetpos);
                return;
            }

            var allpossible = grid.GetAllAdjacentFree(currentpos, targetpos);
            foreach(var pos in allpossible)
            {
                if(!open.Contains(pos) && !closed.Contains(pos))
                {
                    open.Add(pos);
                    grid.SetParent(pos, currentpos);
                }
            }
            
            open.Remove(currentpos);
            closed.Add(currentpos);
        }
        while (open.Count != 0);

        /*
        if(closed.Count >= 3 && Random.Range(0, 5) == 1)
        {
            grid.TraceBackwards(line, closed[Random.Range(closed.Count/2, closed.Count)]);
        }
        */
        return;
    }


    //pathfind to connect the lines 
    private static void PathFindLines(LineGrid grid)
    {

        foreach(var line in grid.Lines)
        {
            //if line desnt exist ignore
            if (!grid.IsFree(line.GetNode(0))) continue;

            //chose target and if none can be chosen ignore
            var target = grid.ChooseTarget(line);
            if (target == null) continue;

            PathFindLine(grid, line, target);

            grid.Print();
        } 
    }



    //Build All
    public static void Build(GameObject boardObj, int width, int height, float unitSize)
    {
        /*
        //board ports
        var allPorts = GeneratePorts(width, height);

        //grid
        var gridw = width  * Subspaces;
        var gridh = height * Subspaces;
        LineGrid grid = new LineGrid(gridw, gridh, width * unitSize, height * unitSize);

        //populate grid with port points
        var numLines = (int) ((allPorts[0].Count + allPorts[1].Count + allPorts[2].Count + allPorts[3].Count) * Subspaces)/4;
        PopulatePorts(grid, Subspaces, allPorts, numLines);

        //create lines from ports to other ports
        PathFindLines(grid);
        */

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

        var mats = new List<Material>();
        parentRenderer.GetMaterials(mats);
        var mainMaterial = mats[0];
        var lineMaterial = mats[1];
        gameObject.GetComponent<MeshRenderer>().material = mainMaterial;

        //add outline to board
        var outln = gameObject.AddComponent<QuickOutline>();
        outln.OutlineMode = QuickOutline.Mode.OutlineVisible;
        outln.OutlineColor = new Color32(7, 80, 73, 255);
        outln.OutlineWidth = 5;

        //grid.BuildLines(gameObject, lineMaterial);

        return;
    }
}
