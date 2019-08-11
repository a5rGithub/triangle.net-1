using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshExplorer.Examples
{
    interface IExample
    {
        void Execute();
        string Name { get; }
        string Description { get; }
        EventHandler InputGenerated { get; set; }
    }
}
