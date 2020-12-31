using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpGLTF.Materials;
using SolidWorks.Interop.sldworks;
/* TODO：材料属性的光照度等
 * TODO:网格角度
 */


namespace DuSwToglTF.Model
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public class SWglTFModel
    {
        /// <summary>
        /// SolidWorks材料属性
        /// </summary>
        public double[] PartMaterialValue;
        /// <summary>
        /// Solidworks变换矩阵
        /// </summary>
        public MathTransform SWMathTran;
        /// <summary>
        /// 默认材料属性
        /// </summary>
        private MaterialBuilder InitMaterial = new MaterialBuilder()
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader()
                .WithChannelParam(KnownChannels.MetallicRoughness, new Vector4(1, 0, 0, 1));
        /// <summary>
        /// 初始原点矩阵
        /// </summary>
        public  Matrix4x4 InitPartPos = new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0,1
            );

        /// <summary>
        /// 零件矩阵
        /// </summary>
        public Matrix4x4 PartTransform
        {
            get {
                if (SWMathTran == null)
                {
                    return InitPartPos;
                }
                else
                {
                    double[] Tran = SWMathTran.ArrayData;
                    return new Matrix4x4(
                        Convert.ToSingle(Tran[0]), Convert.ToSingle(Tran[1]), Convert.ToSingle(Tran[2]), 0,
                        Convert.ToSingle(Tran[3]), Convert.ToSingle(Tran[4]), Convert.ToSingle(Tran[5]), 0,
                        Convert.ToSingle(Tran[6]), Convert.ToSingle(Tran[7]), Convert.ToSingle(Tran[8]), 0,
                        Convert.ToSingle(Tran[9]), Convert.ToSingle(Tran[10]), Convert.ToSingle(Tran[11]), 0
                        );
                }
            }
        }
     
        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]
        /// <summary>
        /// 材料属性
        /// </summary>
        public MaterialBuilder MaterialBuilder
        {
            get
            {

                if (PartMaterialValue != null)
                {
                    //if (PartMaterialValue[0] == 1 && PartMaterialValue[1] == 1 && PartMaterialValue[2] == 1)
                    //{
                    //    return InitMaterial;
                    //}
                    //else
                    //{
                    _MaterialBuilder = new MaterialBuilder()
              .WithDoubleSide(true)
              .WithChannelParam(KnownChannels.BaseColor, new System.Numerics.Vector4(
                 Convert.ToSingle(PartMaterialValue[0]),
                 Convert.ToSingle(PartMaterialValue[1]),
                 Convert.ToSingle(PartMaterialValue[2]),
                 1));
                    // }
                    
                }
                if (_MaterialBuilder == null)
                {
                    _MaterialBuilder = InitMaterial;
                }
                return _MaterialBuilder;
            }
        }
        /// <summary>
        /// 所有的实体
        /// </summary>
        public List<BodyglTFModel> BodyList = new List<BodyglTFModel>();
        #region 私有字段
        private Matrix4x4 _PartTransform;
        private MaterialBuilder _MaterialBuilder = null;
        #endregion
    }
    /// <summary>
    /// 实体模型属性
    /// </summary>
    public class BodyglTFModel
    {
        /// <summary>
        /// 初始原点矩阵
        /// </summary>
        public Matrix4x4 InitPartPos = new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
            );
        public MathTransform SWMathTran;

        /// <summary>
        /// 零件矩阵
        /// </summary>
        public Matrix4x4 BodyTransform
        {
            get
            {
                if (SWMathTran == null)
                {
                    return InitPartPos;
                }
                else
                {
                    double[] Tran = SWMathTran.ArrayData;
                    return new Matrix4x4(
                        Convert.ToSingle(Tran[0]), Convert.ToSingle(Tran[1]), Convert.ToSingle(Tran[2]), 0,
                        Convert.ToSingle(Tran[3]), Convert.ToSingle(Tran[4]), Convert.ToSingle(Tran[5]), 0,
                        Convert.ToSingle(Tran[6]), Convert.ToSingle(Tran[7]), Convert.ToSingle(Tran[8]), 0,
                        Convert.ToSingle(Tran[9]), Convert.ToSingle(Tran[10]), Convert.ToSingle(Tran[11]), 0
                        );
                }
            }
        }

        /// <summary>
        /// 实体材料属性
        /// </summary>
        public double[] BodyMaterialValue;

        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]
        public MaterialBuilder MaterialBuilder
        {
            get
            {
             
                    if (BodyMaterialValue != null)
                    {
                        _MaterialBuilder = new MaterialBuilder()
                        .WithDoubleSide(true)
                        .WithChannelParam(KnownChannels.Normal, new System.Numerics.Vector4(
                           Convert.ToSingle(BodyMaterialValue[0]),
                           Convert.ToSingle(BodyMaterialValue[1]),
                           Convert.ToSingle(BodyMaterialValue[2]),
                           1));
                    }
              
                    return _MaterialBuilder;
                

            }
        }
        /// <summary>
        /// 实体中所有的面
        /// </summary>
        public List<FaceglTFModel> FaceList = new List<FaceglTFModel>();

        private MaterialBuilder _MaterialBuilder = null;

    }

    public class FaceglTFModel
    {
        /// <summary>
        /// 面材质属性
        /// </summary>
        public double[] FaceMaterialValue;

        private MaterialBuilder _MaterialBuilder = null;
        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]
        public MaterialBuilder MaterialBuilder
        {
            get
            {
                
                    if (FaceMaterialValue != null)
                    {
                        _MaterialBuilder = new MaterialBuilder()
                        .WithDoubleSide(true)
                        .WithChannelParam(KnownChannels.Normal, new System.Numerics.Vector4(
                           Convert.ToSingle(FaceMaterialValue[0]),
                           Convert.ToSingle(FaceMaterialValue[1]),
                           Convert.ToSingle(FaceMaterialValue[2]),
                           1));
                    }
                
                return _MaterialBuilder;
            }
        }
        /// <summary>
        /// 面中所有的三角面
        /// </summary>
        public List<FaceVertexModel> FaceTri = new List<FaceVertexModel>();
    }
    /// <summary>
    /// 一个三角形面的三个顶点
    /// </summary>
    public class FaceVertexModel
    {
        public VERTEX a;
        public VERTEX b;
        public VERTEX c;
    }
}
