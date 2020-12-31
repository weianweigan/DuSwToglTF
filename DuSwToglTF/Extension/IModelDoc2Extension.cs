using DuSwToglTF.DataModel;
using DuSwToglTF.ExportContext;
using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Numerics;
using static DuSwToglTF.SwExtension.CustomPropertiesExtension;

namespace DuSwToglTF.Extension
{
    public static class IModelDoc2Extension
    {
        /// <summary>
        /// 读取实体
        /// </summary>
        public static IEnumerable<BodyDataModel> GetBodyDataModels(this IModelDoc2 doc)
        {
            var docType = (swDocumentTypes_e)doc.GetType();

            if (docType == swDocumentTypes_e.swDocPART)
            {
                var partDoc = doc as IPartDoc;

                var bodies = partDoc.GetBodiesEx();

                foreach (var body in bodies)
                {
                    yield return new BodyDataModel(body, Matrix4x4.Identity);
                }
            }
            else if (docType == swDocumentTypes_e.swDocASSEMBLY)
            {
                var assDoc = doc as IAssemblyDoc;
                var comps = assDoc.GetComponents(false) as object[];

                foreach (IComponent2 comp  in comps)
                {
                    var bodies = comp.GetBodies2((int)swBodyType_e.swSolidBody) as object[];
                    var compMaterial = comp.GetMaterialBuilder();

                    if (bodies != null)
                    {
                        var loc = comp.Transform2.GetLocation();

                        foreach (IBody2 body in bodies)
                        {
                            yield return new BodyDataModel(body, loc,compMaterial);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 读取材质
        /// </summary>
        public static MaterialBuilder GetMaterialBuilder(this IModelDoc2 doc)
        {
            var docType = (swDocumentTypes_e)doc.GetType();

            if (docType == swDocumentTypes_e.swDocPART)
            {
                var partDoc = doc as IPartDoc;
                return partDoc.GetMaterialBuilder();
            }
            else
            {
                return MaterialUtility.DefuatMaterial;
            }
        }

        public static IEnumerable<CustomProperty> GetCustomProperties(this IModelDoc2 doc)
        {
            var docType = (swDocumentTypes_e)doc.GetType();

            if (docType == swDocumentTypes_e.swDocPART)
            {
                var cusMgr = doc.Extension.CustomPropertyManager[""];

                object names = new object();
                object types = new object();
                object values = new object();

                cusMgr.GetAll(ref names, ref types, ref values);

                if (names != null)
                {
                    var nameArray = names as object[];
                    var valueArray = values as object[];
                    var typeArray = types as object[];
                    for (int i = 0; i < nameArray.Length; i++)
                    {
                        yield return new CustomProperty(nameArray[i].ToString(), valueArray[i].ToString(), doc.GetTitle());
                    }
                }
            }
            else if(docType == swDocumentTypes_e.swDocASSEMBLY)
            {

            }
        }
    }
}
