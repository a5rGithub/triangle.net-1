// -----------------------------------------------------------------------
// <copyright file="RingPolygon.cs" company="">
// Christian Woltering, Triangle.NET, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace MeshExplorer.Generators
{
    using TriangleNet.Geometry;

    /// <summary>
    /// Generates a ring polygon.
    /// </summary>
    public class CircleWithHole : BaseGenerator
    {
        public CircleWithHole()
        {
            name = "Circle with Hole";
            description = "";
            parameter = 2;

            descriptions[0] = "Number of points:";
            descriptions[1] = "Outer radius:";

            ranges[0] = new int[] { 100, 250 };
            ranges[1] = new int[] { 2, 15 };
        }

        public override IPolygon Generate(double param0, double param1, double param2)
        {
            // Number of points on the outer circle
            int n = GetParamValueInt(0, param0);

            double radius = GetParamValueInt(1, param1);

            // Current radius and step size
            double r, h = radius / n;

            var polygon = new Polygon(n + 1);

            // Inner cirlce (radius = 1) (hole)
            r = 1;
            var innerCircleMarker = 1;
            polygon.Add(CreateCircleContour(r, (int)(r / h), innerCircleMarker), new Point(0, 0));

            // Center cirlce
            r = (radius + 1.0) / 2.0;
            var centerCircleMarker = 2;
            polygon.Add(CreateCircleContour(r, (int)(r / h), centerCircleMarker));

            //count = input.Count;

            // Outer cirlce
            r = radius;
            var outerCircleMarker = 3;
            polygon.Add(CreateCircleContour(r, (int)(r / h), outerCircleMarker));

            // Regions: |++++++|++++++|---|
            //          r             1   0

            polygon.Regions.Add(new RegionPointer((r + 3.0) / 4.0, 0, 1));
            polygon.Regions.Add(new RegionPointer((3 * r + 1.0) / 4.0, 0, 2));

            return polygon;
        }
    }
}
