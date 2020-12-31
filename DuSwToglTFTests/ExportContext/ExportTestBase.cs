using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using DuSwToglTF.ExportContext;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DuSwToglTFTests.ExportContextTests
{
    public class ExportTestBase
    {
        public ExportTestBase()
        {
            
        }

        protected void Export(ISldWorks app, string fileExtension,string childFolder,Func<string,glTFExportContext > contextFunc)
        {
            var dirInfo = new DirectoryInfo(Path.GetDirectoryName(typeof(PartDocExportContextTests).Assembly.Location));
            
            var diretory = Path.Combine(dirInfo.Parent.FullName, "DuSwToglTFTests", "SolidWorksModel", childFolder);

            var files = Directory.GetFiles(diretory).Where(p => p.ToUpper().EndsWith(fileExtension));

            foreach (var item in files)
            {
                int errors = 0;
                var doc = app.OpenDoc2(item, (int)swDocumentTypes_e.swDocPART,true,false,true,ref errors) as IModelDoc2;

                var fileName = Path.GetFileNameWithoutExtension(item);

                ExporterUtility.ExportData(doc, contextFunc.Invoke(Path.Combine(diretory,fileName)));

                Assert.IsTrue(File.Exists(diretory + ".gltf"));
                Assert.IsTrue(File.Exists(diretory + ".glb"));

            }
        }
    }
}
