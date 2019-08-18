using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace MeshExplorer.Examples
{
    public class Example11 : IExample
    {
        public Example11()
        {
            Name = "Usage of labels";
            Description = "Can I use point labels to distinguish segments";
        }
        public string Name { get; }
        public string Description { get; }

        public EventHandler InputGenerated { get; set; }

        public void Execute()
        {
            var rectContourVertices = new List<Vertex>();
            rectContourVertices.Add(new Vertex(0.0, 0.0, 1));
            rectContourVertices.Add(new Vertex(10.0, 0.0, 2));
            rectContourVertices.Add(new Vertex(10.0, 10.0, 3));
            rectContourVertices.Add(new Vertex(0.0, 10.0, 4));
            var rectangularContour = new Contour(rectContourVertices, true);

            var innerrectContourVertices = new List<Vertex>();
            innerrectContourVertices.Add(new Vertex(2.0, 4.0, 1));
            innerrectContourVertices.Add(new Vertex(8.0, 4.0, 2));
            innerrectContourVertices.Add(new Vertex(6.5, 2.2, 2));
            innerrectContourVertices.Add(new Vertex(5.2, 1.5, 3));
            innerrectContourVertices.Add(new Vertex(4.8, 1.5, 4));
            innerrectContourVertices.Add(new Vertex(3.5, 2.2, 2));

            var innerContour = new Contour(innerrectContourVertices, true);
            var polygon = new Polygon(5, true);
            var face = Example2.Circle(5, new Point(5, 5), 0.5, 1);
            polygon.Add(face, hole: false, regionlabel: 10);

            var segmentLabels = polygon.Segments.Select(s => s.Label);
            polygon.Add(rectangularContour, hole: false, regionlabel: 1);
            polygon.Add(innerContour, hole: false, regionlabel: 45);
            var lefteye = Example2.Circle(0.5, new Point(3, 7), 0.5, 100);
            polygon.Add(lefteye, hole: false, regionlabel: 100);
            var righteye = Example2.Circle(0.5, new Point(7, 7), 0.5, 100);
            polygon.Add(righteye, hole: false, regionlabel: 100);
            var mesh = polygon.Triangulate(new QualityOptions() { MinimumAngle = 25.0, VariableArea = true });

            //var mesh = polygon.Triangulate(new QualityOptions() { MinimumAngle = 30.0, MaximumArea = 0.3, VariableArea = true});
            InputGenerated(mesh, EventArgs.Empty);
        }
    }
}
