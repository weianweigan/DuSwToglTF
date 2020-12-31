using SharpGLTF.Materials;
using System.Numerics;

namespace DuSwToglTF.Extension
{
    public static class MaterialUtility
    {
        private static MaterialBuilder _defuatMaterial;

        public static MaterialBuilder DefuatMaterial { get => _defuatMaterial ?? ( _defuatMaterial = new MaterialBuilder()
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader()
                .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1)));}

        public static MaterialBuilder MaterialValueToMaterialBuilder(double[] materialValue,string name = "")
        {
            if (materialValue == null || materialValue.Length < 9)
            {
                return null;
            }

            var matBuilder = string.IsNullOrEmpty(name) ? new MaterialBuilder() : new MaterialBuilder(name);
            matBuilder.WithBaseColor(new Vector4(
                    (float)materialValue[0],
                    (float)materialValue[1],
                    (float)materialValue[2],
                    (float)materialValue[3]))
                .WithDoubleSide(true);

            return matBuilder;
        }
    }
}
