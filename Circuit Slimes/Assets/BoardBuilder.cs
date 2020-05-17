using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBuilder
{

    private class Line
    {
        public float x0 = 0;
        public float y0 = 0;
        public float z0 = 0;
        public float x1 = 0;
        public float y1 = 0;
        public float z1 = 0;

        public Line(float x0, float z0, float x1, float z1)
        {
            this.x0 = x0;
            this.z0 = z0;
            this.x1 = x1;
            this.z1 = z1;
        }

        public Line(float x0, float y0, float z0, float x1, float y1, float z1)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
            this.x1 = x1;
            this.y1 = y1;
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

            vertices.Add(new Vector3(x0, 0, y0));
            vertices.Add(new Vector3(x1, 0, y1));
            vertices.Add(new Vector3(x2, 0, y2));

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

            vertices.Add(new Vector3(x0, 0, y1));
            vertices.Add(new Vector3(x1, 0, y1));
            vertices.Add(new Vector3(x0, 0, y0));
            vertices.Add(new Vector3(x1, 0, y0));

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


    public static void Build(GameObject boardObj, int width, int height, float unitSize)
    {
        //BOARD PROPERTIES
        float boardThickness = 0.5f;
        float bordersWidth  = 1.0f;

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
            var line = new Line(unitSize * i, h, unitSize * i, h + bordersWidth);
            topStrip.AddLine(line);

            line = new Line(unitSize * i, -bordersWidth, unitSize * i, 0);
            bottomStrip.AddLine(line);
        }
        var top = topStrip.Build(vertices, triangles);
        var bottom = bottomStrip.Build(vertices, triangles);

        //vertical
        var leftStrip = new Strip();
        var rightStrip = new Strip();
        for (var i = 0; i < height + 1; i++)
        {
            var line = new Line(0, unitSize * i, -bordersWidth, unitSize * i);
            leftStrip.AddLine(line);

            line = new Line(w + bordersWidth, unitSize * i, w, unitSize * i);
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
            if(i % 2 == 0) continue;
            if (i - 2 < top[0]) break;
            outerEdges.Add(new Vector2Int(i,i - 2));
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
            v.y -= boardThickness;
            vertices.Add(v);
            var a2 = n;

            v = vertices[edge.y];
            v.y -= boardThickness;
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
        mesh.vertices   = vertices.ToArray();
        mesh.triangles  = triangles.ToArray();
        mesh.RecalculateNormals();

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
