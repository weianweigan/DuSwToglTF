using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DuSwToglTF.Model;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace DuSwToglTF.Controller
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;
    public class Convertor
    {
        public delegate void ProgressStatus_delegate(double Value, string Status, ErrorType error);

        public event ProgressStatus_delegate ProgressStatus_event;
        private Convertor()
        { }
        private static Convertor _DuConvert ;
        public static Convertor DuConvertor
        {
            get {
                if (_DuConvert == null)
                {
                    _DuConvert = new Convertor();
                }
                return _DuConvert;
            }
}
        public enum ErrorType
        {
            NoErros,
            NoPartOrAssemblyDocument,
            DoNotKnow
        }
        #region 公共方法
        public  SWglTFModel ConvertToglTFModel(ModelDoc2 swModel,out ErrorType errors)
        {
            SWglTFModel model = null;
            errors = ErrorType.DoNotKnow;
            ProgressStatus_event(0.01, "开始读取", ErrorType.NoErros);
            swDocumentTypes_e ModelType = (swDocumentTypes_e)swModel.GetType();
            switch (ModelType)
            {
                case swDocumentTypes_e.swDocNONE:
                    errors = ErrorType.NoPartOrAssemblyDocument;
                    return null;
                case swDocumentTypes_e.swDocPART:
                    ProgressStatus_event(0.02, "读取零件信息", ErrorType.NoErros);
                    model = ConvertPartToglTF(swModel);
                    ProgressStatus_event(0.5, "读取零件信息完成", ErrorType.NoErros);
                    break;
                case swDocumentTypes_e.swDocASSEMBLY:
                    ProgressStatus_event(0.02, "读取装配体信息", ErrorType.NoErros);
                    model = ConvertAssemblyToglTF(swModel);
                    ProgressStatus_event(0.5, "读取装配体信息完成", ErrorType.NoErros);
                    break;
                case swDocumentTypes_e.swDocDRAWING:
                    errors = ErrorType.NoPartOrAssemblyDocument;
                    return null;
                   
                case swDocumentTypes_e.swDocSDM:
                    errors = ErrorType.NoPartOrAssemblyDocument;
                    return null;
                    
                case swDocumentTypes_e.swDocLAYOUT:
                    errors = ErrorType.NoPartOrAssemblyDocument;
                    return null;
                case swDocumentTypes_e.swDocIMPORTED_PART:
                    break;
                case swDocumentTypes_e.swDocIMPORTED_ASSEMBLY:
                    break;
            }
            if (model == null)
            {
                errors = ErrorType.DoNotKnow;
                
            }
            return model;
        }

        private  SWglTFModel ConvertAssemblyToglTF(ModelDoc2 swModel)
        {
            SWglTFModel Model = new SWglTFModel();

            AssemblyDoc swAssDoc = (AssemblyDoc)swModel;
            object[] comps = swAssDoc.GetComponents(false);
            foreach (Component2 item in comps)
            {
                double[] MaterialValue = item.MaterialPropertyValues;
                if (MaterialValue == null)
                {
                    ModelDoc2 swCompModel = item.GetModelDoc2();
                    if (swCompModel!= null)
                    {
                        MaterialValue = swCompModel.MaterialPropertyValues;
                    }
                }
                object[] bodys = item.GetBodies2((int)swBodyType_e.swAllBodies);
                if (bodys != null)
                {
                    foreach (Body2 swBody in bodys)
                    {
                        var bodymodel = GetglTFBodyModel(swBody);
                        if (bodymodel.BodyMaterialValue == null && MaterialValue !=null)
                        {
                            bodymodel.BodyMaterialValue = MaterialValue;
                        }
                        bodymodel.SWMathTran = item.Transform2;
                        Model.BodyList.Add(bodymodel);
                    }
                }
            }
            return Model;
        }

        public List<string>  SaveAs(SWglTFModel Model, string Path, string Name)
        {
            var scene = new SharpGLTF.Scenes.SceneBuilder();
            
            foreach (var Body in Model.BodyList)
            {

                //创建一个网格
                var Mesh = new MeshBuilder<VERTEX>("mesh");


                var material = (Body.MaterialBuilder == null ? Model.MaterialBuilder : Body.MaterialBuilder);
                if (material == null)
                {
                    material = new MaterialBuilder()
            .WithDoubleSide(true)
            .WithMetallicRoughnessShader()
            .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1));
                }
                //确定材质属性
                var prim = Mesh.UsePrimitive(material
                    );

                foreach (var face in Body.FaceList)
                {
                    
                    foreach (var tri in face.FaceTri)
                    {
                        prim.AddTriangle(tri.a, tri.b, tri.c);
                    }
                }

                scene.AddMesh(Mesh, Body.BodyTransform);

            }


            var model = scene.ToSchema2();
            model.SaveAsWavefront(Path + "\\" + Name + ".obj");
            model.SaveGLB(Path + "\\" + Name + ".glb");
            model.SaveGLTF(Path + "\\" + Name + ".gltf");
            return new List<string>()
            {
                Path + "\\" + Name + ".obj",
                Path + "\\" + Name + ".glb",
                Path + "\\" + Name + ".gltf"
            };
        }
        public async Task<List<string>> SaveAsAsync(SWglTFModel Model, string Path, string Name)
        {
            var scene = new SharpGLTF.Scenes.SceneBuilder();

            foreach (var Body in Model.BodyList)
            {
                foreach (var face in Body.FaceList)
                {
                    var Mesh = new MeshBuilder<VERTEX>("mesh");
                    //确定材质属性
                    var prim = Mesh.UsePrimitive(
                        (Body.MaterialBuilder == null ? Model.MaterialBuilder : Body.MaterialBuilder) 
                      );


                    foreach (var tri in face.FaceTri)
                    {
                        prim.AddTriangle(tri.a, tri.b, tri.c);
                    }
                    scene.AddMesh(Mesh, Body.BodyTransform);
                }
            }
            var model = scene.ToSchema2();
            model.SaveAsWavefront(Path + "\\" + Name + ".obj");
            model.SaveGLB(Path + "\\" + Name + ".glb");
            model.SaveGLTF(Path + "\\" + Name + ".gltf");
            List<string> res = new List<string>()
            {
                Path + "\\" + Name + ".obj",
                Path + "\\" + Name + ".glb",
                Path + "\\" + Name + ".gltf"
            };
            return res;
        }
        #endregion
        #region 私有方法
        private  SWglTFModel ConvertPartToglTF(ModelDoc2 swModel)
        {
            SWglTFModel Model = new SWglTFModel();

            var MaterialValue = ((PartDoc)swModel).MaterialPropertyValues;
            if (MaterialValue != null)
            {
                Model.PartMaterialValue = MaterialValue;
            }
            object[] bodys = ((PartDoc)swModel).GetBodies((int)swBodyType_e.swAllBodies);
            foreach (Body2 swBody in bodys)
            {
                Model.BodyList.Add(GetglTFBodyModel(swBody));
            }
            return Model;
        }
        private  Model.BodyglTFModel GetglTFBodyModel(Body2 swBody)
        {
            BodyglTFModel BodyModel = new BodyglTFModel();
            if (swBody == null)
            {
                return null;
            }
            try
            {
                var BodyMaterial = (double[])swBody.MaterialPropertyValues2;
                if (BodyMaterial != null)
                {
                    BodyModel.BodyMaterialValue = BodyMaterial;
                }

                #region 网格化
                Tessellation swTessellation = swBody.GetTessellation(null);
                if (swTessellation != null)
                {
                    swTessellation.NeedFaceFacetMap = true;
                    swTessellation.NeedVertexParams = true;
                    swTessellation.NeedVertexNormal = true;
                    swTessellation.ImprovedQuality = true;
                    // How to handle matches across common edges
                    swTessellation.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
                    // Do it
                    bool bResult = swTessellation.Tessellate();
                }
                else
                {
                    return null;
                }
                #endregion
                Face2 swFace = (Face2)swBody.GetFirstFace();

                while (swFace != null)
                {
                    Model.FaceglTFModel FaceModel = new FaceglTFModel();
                    var FaceMaterial = swFace.MaterialPropertyValues;
                    if (FaceMaterial != null)
                    {
                        FaceModel.FaceMaterialValue = FaceMaterial;
                    }
                    #region 面的三角化
                    int[] aFacetIds = (int[])swTessellation.GetFaceFacets(swFace);
                    int iNumFacetIds = aFacetIds.Length;
                    for (int iFacetIdIdx = 0; iFacetIdIdx < iNumFacetIds; iFacetIdIdx++)
                    {
                        int[] aFinIds = (int[])swTessellation.GetFacetFins(aFacetIds[iFacetIdIdx]);
                        // There should always be three fins per facet
                        FaceVertexModel model = new FaceVertexModel();
                        List<double[]> points = new List<double[]>();
                        for (int iFinIdx = 0; iFinIdx < 3; iFinIdx++)
                        {
                            int[] aVertexIds = (int[])swTessellation.GetFinVertices(aFinIds[iFinIdx]);
                            // Should always be two vertices per fin
                            double[] aVertexCoords1 = (double[])swTessellation.GetVertexPoint(aVertexIds[0]);
                            double[] aVertexCoords2 = (double[])swTessellation.GetVertexPoint(aVertexIds[1]);
                            var v1 = new VERTEX(Convert.ToSingle(aVertexCoords1[0]),
                                   Convert.ToSingle(aVertexCoords1[1]),
                                   Convert.ToSingle(aVertexCoords1[2]));
                            var v2 = new VERTEX(Convert.ToSingle(aVertexCoords2[0]),
                                  Convert.ToSingle(aVertexCoords2[1]),
                                  Convert.ToSingle(aVertexCoords2[2]));
                            bool isContain = false;
                            foreach (var item in points)
                            {
                                if ((Math.Abs(item[0] - aVertexCoords1[0]) + Math.Abs(item[1] - aVertexCoords1[1]) + Math.Abs(item[2] - aVertexCoords1[2])) < 0.00001)
                                {
                                    isContain = true;
                                }
                            }
                            if (!isContain)
                            {
                                points.Add(aVertexCoords1);
                            }

                            isContain = false;
                            foreach (var item in points)
                            {
                                if ((Math.Abs(item[0] - aVertexCoords2[0]) + Math.Abs(item[1] - aVertexCoords2[1]) + Math.Abs(item[2] - aVertexCoords2[2])) < 0.00001)
                                {
                                    isContain = true;
                                }
                            }
                            if (!isContain)
                            {
                                points.Add(aVertexCoords2);
                            }
                            // Create a line
                            //swModel.CreateLine2(aVertexCoords1[0], aVertexCoords1[1], aVertexCoords1[2], aVertexCoords2[0], aVertexCoords2[1], aVertexCoords2[2]);
                        }
                        if (points.Count == 3)
                        {
                            model.a = new VERTEX(Convert.ToSingle(points[0][0]),
                                  Convert.ToSingle(points[0][1]),
                                  Convert.ToSingle(points[0][2]));
                            model.b = new VERTEX(Convert.ToSingle(points[1][0]),
                                 Convert.ToSingle(points[1][1]),
                                 Convert.ToSingle(points[1][2]));
                            model.c = new VERTEX(Convert.ToSingle(points[2][0]),
                                  Convert.ToSingle(points[2][1]),
                                  Convert.ToSingle(points[2][2]));
                            FaceModel.FaceTri.Add(model);

                        }
                    }
                    if (FaceModel.FaceTri.Count > 0)
                    {
                        BodyModel.FaceList.Add(FaceModel);
                    }
                    swFace = (Face2)swFace.GetNextFace();
                    #endregion
                }
            }
            catch (Exception ex)
            {
             
            }
            return BodyModel;
        }
        #endregion
    }
}
