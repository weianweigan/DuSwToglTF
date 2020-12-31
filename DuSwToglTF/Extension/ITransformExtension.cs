using SolidWorks.Interop.sldworks;
using System.Numerics;

namespace DuSwToglTF.ExportContext
{
    public static class ITransformExtension
    {
        public static Matrix4x4 GetLocation(this IMathTransform transform)
        {
            var data = transform.ArrayData as double[];

            return new Matrix4x4(
                (float)data[0], (float)data[1], (float)data[2], (float)data[13],
                (float)data[3], (float)data[4], (float)data[5], (float)data[14],
                (float)data[6], (float)data[7], (float)data[8], (float)data[15],
                (float)data[9], (float)data[10], (float)data[11], (float)data[12]
                );
        }
    }
}
