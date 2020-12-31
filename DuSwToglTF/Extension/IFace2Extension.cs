using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuSwToglTF.Extension
{
    public static class IFace2Extension
    {
        public static MaterialBuilder GetMaterialBuilder(this IFace2 face)
        {
            if (!face.HasMaterialPropertyValues())
            {
                return null;
            }

            var materialValue = face.MaterialPropertyValues as double[];
            return MaterialUtility.MaterialValueToMaterialBuilder( materialValue);
        }
    }
}
