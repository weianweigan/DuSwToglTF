using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;

namespace DuSwToglTF.Extension
{
    public static class IComponent2Extension
    {   
        public static MaterialBuilder GetMaterialBuilder(this IComponent2 comp)
        {
            var materialValue = comp.GetModelMaterialPropertyValues("") as double[];
            return MaterialUtility.MaterialValueToMaterialBuilder(materialValue);
        }
    }
}
