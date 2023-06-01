using DuSwToglTF.DataModel;
using DuSwToglTF.ExportContext;
using DuSwToglTF.SwExtension;
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
                    var suppressionState = (swComponentSuppressionState_e)comp.GetSuppression();
                    if (!(suppressionState == swComponentSuppressionState_e.swComponentResolved || suppressionState == swComponentSuppressionState_e.swComponentFullyResolved))
                    {
                        continue;
                    }

                    var bodies = comp.GetBodies2((int)swBodyType_e.swSolidBody) as object[];
                    var compMaterial = comp.GetMaterialBuilder();

                    if (bodies != null)
                    {
                        var loc = comp.Transform2.GetLocation();

                        foreach (IBody2 body in bodies)
                        {
                            yield return new BodyDataModel(body, loc,compMaterial,comp.Name2);
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

        public static CustomPropertyGroup GetCustomProperties(this IModelDoc2 doc)
        {
            var docType = (swDocumentTypes_e)doc.GetType();

            var rootCompCusPropGroup = GetDocAllProperties(doc);

            if (docType == swDocumentTypes_e.swDocASSEMBLY)
            {
                var assDoc = doc as IAssemblyDoc;
                var comps = assDoc.GetComponents(false) as object[];

                HashSet<string> docs = new HashSet<string>();
                //遍历组件
                foreach (IComponent2 comp in comps)
                {
                    //Skip suppressed components
                    var suppressionState = (swComponentSuppressionState_e)comp.GetSuppression();
                    if (!(suppressionState == swComponentSuppressionState_e.swComponentResolved || 
                        suppressionState == swComponentSuppressionState_e.swComponentFullyResolved))
                    {
                        continue;
                    }

                    var compDoc = comp.GetModelDoc2() as IModelDoc2;

                    var pathName = compDoc.GetPathName();
                    if (!string.IsNullOrEmpty(pathName) && docs.Contains(pathName))
                    {
                        continue;
                    }
                    else
                    {
                        docs.Add(pathName);
                    }

                    var subGroup = GetDocAllProperties(compDoc);

                    rootCompCusPropGroup.Children.Add(subGroup);
                }
            }

            return rootCompCusPropGroup;
        }

        private static CustomPropertyGroup GetDocAllProperties(IModelDoc2 doc)
        {
            var compCusPropGroup = new CustomPropertyGroup(doc.GetTitle());

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
                    compCusPropGroup.Children.Add(new CustomProperty(nameArray[i].ToString(), valueArray[i].ToString(), doc.GetTitle()));
                }
            }

            return compCusPropGroup;
        }
    }
}
