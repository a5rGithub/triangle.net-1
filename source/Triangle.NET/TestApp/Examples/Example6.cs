using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.IO;
using TriangleNet.Meshing;

namespace MeshExplorer.Examples
{
    public class Example6 : IExample
    {
        public Example6()
        {
            Name = "Parallel mesh processing";
            Description = "Loads and processes multiple meshes in parallel.";
        }
        public string Name { get; }

        public string Description { get; }

        public EventHandler InputGenerated { get; set; }

        public void Execute()
        {
            Util.Tic();
            var polygons = LoadPolygons();
            //Console.WriteLine("  Polygons: {0} (took {1}ms to load)", polygons.Count, Util.Toc());
            string loadingPolygons = $"Polygons: {polygons.Count} (took {Util.Toc()}ms to load)";
            Util.Tic();
            var sequentialInfo = RunSequential(polygons);
            //Console.WriteLine("Sequential: {0}ms", Util.Toc());
            string sequential = $"Sequential: {Util.Toc()}ms";

            Util.Tic();
            var parallelInfo = RunParallel(polygons);
            //Console.WriteLine("  Parallel: {0}ms", Util.Toc());
            string parallel = $"Parallel: {Util.Toc()}ms";
            //var dummymesh = new Mesh(new Configuration());
            var mesher = new GenericMesher(new Configuration());

            foreach (var poly in polygons)
            {
                InputGenerated(mesher.Triangulate(poly), EventArgs.Empty);
                DarkMessageBox.Show("", "");
            }
            DarkMessageBox.Show($"{Name} - {Description}", $"{loadingPolygons}\n{sequential} {sequentialInfo} \n{parallel} {parallelInfo}");

        }

        private static List<IPolygon> LoadPolygons()
        {
            string path = "../../../Data";//"c:/some/directory/with/poly/files";

            return Directory.EnumerateFiles(path, "*.poly", SearchOption.AllDirectories)
                .Select(file => FileProcessor.Read(file)).ToList();
        }



        public static string RunParallel(List<IPolygon> polygons)
        {
            var queue = new ConcurrentQueue<IPolygon>(polygons);

            int concurrencyLevel = Environment.ProcessorCount;

            var tasks = new Task<MeshResult>[concurrencyLevel];

            for (int i = 0; i < concurrencyLevel; i++)
            {
                tasks[i] =  Task.Run(() =>
                {
                    // Each task has it's own triangle pool and predicates instance.
                    var pool = new TrianglePool();
                    var predicates = new RobustPredicates();

                    var config = new Configuration();

                    // The factory methods return the above instances.
                    config.Predicates = () => predicates;
                    config.TrianglePool = () => pool.Restart();

                    IPolygon poly;

                    var mesher = new GenericMesher(config);
                    var result = new MeshResult();

                    while (queue.Count > 0)
                    {
                        if (queue.TryDequeue(out poly))
                        {
                            var mesh = mesher.Triangulate(poly);

                            ProcessMesh(mesh, result);
                        }
                    }

                    pool.Clear();

                    return result;
                });
            }

            Task.WaitAll(tasks);

            int numberOfTriangles = 0;
            int invalid = 0;

            for (int i = 0; i < concurrencyLevel; i++)
            {
                var result = tasks[i].Result;

                numberOfTriangles += result.NumberOfTriangles;
                invalid += result.Invalid;
            }
            string parallel = $"Total number of triangles processed: {numberOfTriangles}";
            //Console.WriteLine("Total number of triangles processed: {0}", numberOfTriangles);

            if (invalid > 0)
            {
                //Console.WriteLine("   Number of invalid triangulations: {0}", invalid);
                parallel += $"\tNumber of invalid triangulations: {invalid}";
            }
            return parallel;

        }

        public static string RunSequential(List<IPolygon> polygons)
        {
            var pool = new TrianglePool();
            var predicates = new RobustPredicates();

            var config = new Configuration();

            config.Predicates = () => predicates;
            config.TrianglePool = () => pool.Restart();

            var mesher = new GenericMesher(config);
            var result = new MeshResult();

            foreach (var poly in polygons)
            {
                var mesh = mesher.Triangulate(poly);

                ProcessMesh(mesh, result);
            }

            pool.Clear();

            //Console.WriteLine("Total number of triangles processed: {0}", result.NumberOfTriangles);
            var sequential = $"Total number of triangles processed: {result.NumberOfTriangles}";

            if (result.Invalid > 0)
            {
                //Console.WriteLine("   Number of invalid triangulations: {0}", result.Invalid);
                sequential += $"Number of invalid triangulations: {result.Invalid}";
            }
            return sequential;
        }

        private static void ProcessMesh(IMesh mesh, MeshResult result)
        {
            result.NumberOfTriangles += mesh.Triangles.Count;

            if (!MeshValidator.IsConsistent((Mesh)mesh))
            {
                result.Invalid += 1;
            }
        }
    }

    public class MeshResult
    {
        public int NumberOfTriangles { get; set; }
        public int Invalid { get; set; }
    }
    public static class Util
    {
        private static Stopwatch stopwatch = new Stopwatch();

        public static long Toc()
        {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static void Tic()
        {
            stopwatch.Restart();
        }
    }


}
