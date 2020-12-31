using DuSwToglTF.Extension;
using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;
using System.Numerics;

namespace DuSwToglTF.DataModel
{
    public class BodyDataModel
    {
        private readonly MaterialBuilder _material;

        public IBody2 Body { get; }

        public Matrix4x4 Location { get; }

        public MaterialBuilder BodyMaterialBuilder { get => _material ?? Body.GetMaterialBuilder(); }

        public BodyDataModel(IBody2 body, Matrix4x4 location)
        {
            Body = body;
            Location = location;
        }

        public BodyDataModel(IBody2 body, Matrix4x4 location,MaterialBuilder material)
        {
            Body = body;
            Location = location;
            this._material = material;
        }
    }
}
