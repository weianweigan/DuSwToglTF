using NUnit.Framework;
using DuSwToglTFTests.ExportContextTests;
using SolidWorks.Interop.sldworks;
using DuSwToglTF;
using DuSwToglTF.ExportContext;

namespace DuSwToglTFTests.ExportContextTests
{
    [TestFixture()]
    public class AssemblyDocExportContextTests:  ExportTestBase
    {
        public AssemblyDocExportContextTests() 
        {
        }

        [Test()]
        public void AssemblyDocExportContextTest(ISldWorks app)
        {
            Export(app,".SLDASM","Assembly",(diretory) => new AssemblyDocExportContext(diretory));

        }
    }
}