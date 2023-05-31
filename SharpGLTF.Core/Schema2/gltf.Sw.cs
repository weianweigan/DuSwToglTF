using SharpGLTF.Sw;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpGLTF.Schema2
{
    public sealed partial class ModelRoot
    {
        public SolidWorks SolidWorks { get; private set; }

        public void WithSwCustomProperty(SolidWorks solidWorks)
        {
            SolidWorks = solidWorks;
        }
    }
}
