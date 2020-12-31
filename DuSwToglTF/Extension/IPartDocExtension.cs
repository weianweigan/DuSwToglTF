using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DuSwToglTF.Extension
{
    public static class IPartDocExtension
    {
        public static IEnumerable<IBody2> GetBodiesEx(this IPartDoc doc,swBodyType_e bodyType= swBodyType_e.swSolidBody,bool withInvisbleBody = false)
        {
            object[] bodies = doc.GetBodies2((int)bodyType, !withInvisbleBody) as object[];

            if (bodies == null)
            {
                yield break;
            }

            foreach (var item in bodies)
            {
                yield return item as IBody2;
            }
        }

        public static MaterialBuilder GetMaterialBuilder(this IPartDoc doc)
        {
            var matBuilder = default(MaterialBuilder);
            var mdlDoc = (IModelDoc2)doc;

            var materialValue = doc.MaterialPropertyValues as double[];
            if (materialValue == null || materialValue.Length < 7)
            {
                matBuilder = MaterialUtility.DefuatMaterial;
            }
            else
            {
                matBuilder = MaterialUtility.MaterialValueToMaterialBuilder( materialValue);
            }

            return matBuilder;
        }
    }
}
