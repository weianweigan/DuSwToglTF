using DuSwToglTF.Extension;
using SolidWorks.Interop.sldworks;
using System;
using System.Linq;
using static DuSwToglTF.SwExtension.CustomPropertiesExtension;

namespace DuSwToglTF.ExportContext
{
    public static class ExporterUtility
    {
        public static void ExportData(IModelDoc2 doc,glTFExportContext context,Action<int,string> progressAction = null,bool hasObj = false,bool hasglTF = true,bool hasglb = true)
        {
            try
            {
                context.Start();

                //写入实体网格
                var bodies = doc.GetBodyDataModels().ToList();
                progressAction?.Invoke(10, "Read SolidWorks's SolidBody...");
                var material = doc.GetMaterialBuilder();
                progressAction?.Invoke(10, "Read SolidWorks Doc's Material...");

                int count = bodies.Count;
                int i = 1;

                foreach (var item in bodies)
                {
                    int progressValue = (80 / count) * (i++) + 10;
                    progressAction?.Invoke(progressValue, $"Progress SolidBody {item.Body.Name}...");

                    context.OnBodyBegin(item.Body, item.BodyMaterialBuilder ?? material, item.Location);

                    progressAction?.Invoke(progressValue, $"Finish SolidBody {item.Body.Name}...");
                }

                //写入自定义属性
                //var properties = doc.GetCustomProperties();
                //foreach (var prop in properties)
                //{
                //    context.OnCustomPropertyBegin(prop);
                //}

                progressAction?.Invoke(90, "Saving");

                context.Finish(hasObj, hasglTF, hasglb);

                progressAction?.Invoke(100, $"Suceess, FileDirectory:{context.SavePathName}.*");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
