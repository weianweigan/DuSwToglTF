using NUnit.Framework;
using DuSwToglTF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuSwToglTFTests.ExportContextTests;
using SolidWorks.Interop.sldworks;
using System.IO;
using SolidWorks.Interop.swconst;
using DuSwToglTF.ExportContext;

namespace DuSwToglTFTests.ExportContextTests
{
    [TestFixture()]
    public class PartDocExportContextTests : ExportTestBase
    {
        public PartDocExportContextTests() 
        {
        }

        [Test()]
        public void PartDocExportContextTest(ISldWorks app)
        {
            Export(app,".SLDPRT","Part",(diretory) => new PartDocExportContext(diretory));
        }
    }
}