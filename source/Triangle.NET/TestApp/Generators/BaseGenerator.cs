// -----------------------------------------------------------------------
// <copyright file="BaseGenerator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MeshExplorer.Generators
{
    using System;
    using System.Collections.Generic;
    using TriangleNet.Geometry;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class BaseGenerator : IGenerator
    {
        private static int MAX_PARAMS = 3;

        protected string name = "Name";
        protected string description = "Description";
        protected int parameter = 0;

        protected string[] descriptions = new string[MAX_PARAMS];
        protected int[][] ranges = new int[MAX_PARAMS][];

        public virtual string Name { get { return name; } }
        public virtual string Description { get { return description; } }
        public virtual int ParameterCount { get { return parameter; } }

        public virtual string ParameterDescription(int paramIndex)
        {
            if (descriptions[paramIndex] == null)
            {
                return String.Empty;
            }

            return descriptions[paramIndex];
        }

        public virtual string ParameterDescription(int paramIndex, double paramValue)
        {
            int[] range = ranges[paramIndex];

            if (range == null)
            {
                return String.Empty;
            }

            int num = GetParamValueInt(paramIndex, paramValue);
            return num.ToString();
        }

        public abstract IPolygon Generate(double param0, double param1, double param2);

        #region Contour helpers

        protected Contour CreateCircleContour(double r, int n, int contourmarker)
        {
            var circleVertices = CreateCircleVertices(r, n, boundarymarker: contourmarker);
            var circleContour = new Contour(circleVertices, contourmarker);
            return circleContour;
        }

        protected List<Vertex> CreateCircleVertices(double r, int n, int boundarymarker = 0)
        {
            return CreateCircleVertices(0.0, 0.0, r, n, boundarymarker);
        }

        protected List<Vertex> CreateCircleVertices(double x, double y, double r, int n, int boundarymarker = 0)
        {
            return CreateEllipseVertices(0.0, 0.0, r, 1.0, 1.0, n, boundarymarker);
        }

        protected List<Vertex> CreateEllipseVertices(double r, double a, double b, int n, int boundarymarker = 0)
        {
            return CreateEllipseVertices(0.0, 0.0, r, a, b, n, boundarymarker);
        }

        protected List<Vertex> CreateEllipseVertices(double x, double y, double r, double a, double b, int n, int boundarymarker = 0)
        {
            var contour = new List<Vertex>(n);

            double dphi = 2 * Math.PI / n;

            for (int i = 0; i < n; i++)
            {
                contour.Add(new Vertex(x + a * r * Math.Cos(i * dphi), y + b * r * Math.Sin(i * dphi), boundarymarker));
            }

            return contour;
        }

        protected List<Vertex> CreateRectangleVertices(Rectangle rect, int n, int boundarymarker = 0)
        {
            return CreateRectangleVertices(rect, n, n, boundarymarker);
        }

        protected List<Vertex> CreateRectangleVertices(Rectangle rect, int nH, int nV, int boundarymarker = 0)
        {
            var contour = new List<Vertex>(2 * nH + 2 * nV);

            // Horizontal and vertical step sizes.
            double stepH = rect.Width / nH;
            double stepV = rect.Height / nV;

            // Left box boundary points
            for (int i = 0; i < nV; i++)
            {
                contour.Add(new Vertex(rect.Left, rect.Bottom + i * stepV, 1));
            }

            // Top box boundary points
            for (int i = 0; i < nH; i++)
            {
                contour.Add(new Vertex(rect.Left + i * stepH, rect.Top, 1));
            }

            // Right box boundary points
            for (int i = 0; i < nV; i++)
            {
                contour.Add(new Vertex(rect.Right, rect.Top - i * stepV, 1));
            }

            // Bottom box boundary points
            for (int i = 0; i < nH; i++)
            {
                contour.Add(new Vertex(rect.Right - i * stepH, rect.Bottom, 1));
            }

            return contour;
        }

        #endregion

        protected int GetParamValueInt(int paramIndex, double paramOffset)
        {
            int[] range = ranges[paramIndex];

            if (range == null)
            {
                return 0;
            }

            return (int)((range[1] - range[0]) / 100.0 * paramOffset + range[0]);
        }

        protected double GetParamValueDouble(int paramIndex, double paramOffset)
        {
            int[] range = ranges[paramIndex];

            if (range == null)
            {
                return 0;
            }

            return ((range[1] - range[0]) / 100.0 * paramOffset + range[0]);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
