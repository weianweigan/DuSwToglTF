using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using MaterialDesignColors;
using MaterialDesignThemes;

namespace DuSwToglTF
{
    /// <summary>
    /// ConvertPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ConvertPanel : UserControl
    {
        private ISldWorks swApp;
        public ConvertPanelViewModel viewmodel;
        public ConvertPanel(SolidWorks.Interop.sldworks.ISldWorks App)
        {
            //加载MaterialDesign 程序集
            MaterialDesignColors.SwatchesProvider SP = new SwatchesProvider();

            InitializeComponent();
           
            swApp = App;

            viewmodel = new ConvertPanelViewModel(swApp);

            DataContext = viewmodel;
        }
        public static string RootPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static void PreloadDlls()
        {

            var assemblyList = new string[]
            {
                "MaterialDesignColors.dll",
                "MaterialDesignThemes.Wpf.dll"
            };

            foreach (var assembly in assemblyList)
            {

                var assemblyPath = (RootPath + "\\" + assembly);
                if (File.Exists(assemblyPath))
                    Assembly.LoadFrom(assemblyPath);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        //    List<FaceVertexModel> faceVertexList = new List<FaceVertexModel>();

        //    ModelDoc2 swModel = swApp.ActiveDoc;
        //    if (swModel == null)
        //    {
        //        swApp.SendMsgToUser("当前未打开文件");
        //    }
        //    object[] bodys = ((PartDoc)swModel).GetBodies((int)swBodyType_e.swAllBodies);
        //    foreach (Body2 swBody in bodys)
        //    {
                
        //       Tessellation swTessellation = swBody.GetTessellation(null);
        //        if (swTessellation != null)
        //        {
        //            // Set up the Tessellation object

        //            swTessellation.NeedFaceFacetMap = true;

        //            swTessellation.NeedVertexParams = true;

        //            swTessellation.NeedVertexNormal = true;

        //            swTessellation.ImprovedQuality = true;

        //            Debug.Print("Tessellation is configured for higher-quality data output.");

        //            // How to handle matches across common edges

        //            swTessellation.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;

        //            // Do it

        //            bool bResult = swTessellation.Tessellate();

        //        }
        //        else
        //        {
        //            return;
        //        }
        //        Face2 swFace = (Face2)swBody.GetFirstFace();

        //        while (swFace != null)

        //        {

        //            int[]  aFacetIds = (int[])swTessellation.GetFaceFacets(swFace);

        //            int iNumFacetIds = aFacetIds.Length;

        //            for (int iFacetIdIdx = 0; iFacetIdIdx < iNumFacetIds; iFacetIdIdx++)

        //            {

        //                int[] aFinIds = (int[])swTessellation.GetFacetFins(aFacetIds[iFacetIdIdx]);

        //                // There should always be three fins per facet
        //                FaceVertexModel model = new FaceVertexModel();
        //                List<double[]> points = new List<double[]>();
        //                for (int iFinIdx = 0; iFinIdx < 3; iFinIdx++)

        //                {

        //                    int[] aVertexIds = (int[])swTessellation.GetFinVertices(aFinIds[iFinIdx]);

        //                    // Should always be two vertices per fin

        //                    double[] aVertexCoords1 = (double[])swTessellation.GetVertexPoint(aVertexIds[0]);

        //                    double[]  aVertexCoords2 = (double[])swTessellation.GetVertexPoint(aVertexIds[1]);
                            
        //                    var v1= new VERTEX(Convert.ToSingle(aVertexCoords1[0] ),
        //                           Convert.ToSingle(aVertexCoords1[1]),
        //                           Convert.ToSingle(aVertexCoords1[2] ));
        //                    var v2 = new VERTEX(Convert.ToSingle(aVertexCoords2[0]),
        //                          Convert.ToSingle(aVertexCoords2[1]),
        //                          Convert.ToSingle(aVertexCoords2[2]));

        //                    bool isContain = false;
        //                    foreach (var item in points)
        //                    {
        //                        if ((Math.Abs(item[0]-aVertexCoords1[0])+ Math.Abs(item[1] - aVertexCoords1[1])+ Math.Abs(item[2] - aVertexCoords1[2])) < 0.00001)
        //                        {
        //                            isContain = true;
        //                        }
        //                    }
        //                    if (!isContain)
        //                    {
        //                        points.Add(aVertexCoords1);
        //                    }

        //                    isContain = false;
        //                    foreach (var item in points)
        //                    {
        //                        if ((Math.Abs(item[0] - aVertexCoords2[0]) + Math.Abs(item[1] - aVertexCoords2[1]) + Math.Abs(item[2] - aVertexCoords2[2])) < 0.00001)
        //                        {
        //                            isContain = true;
        //                        }
        //                    }
        //                    if (!isContain)
        //                    {
        //                        points.Add(aVertexCoords2);
        //                    }
        //                    // Create a line

        //                    //swModel.CreateLine2(aVertexCoords1[0], aVertexCoords1[1], aVertexCoords1[2], aVertexCoords2[0], aVertexCoords2[1], aVertexCoords2[2]);

        //                }
        //                if (points.Count  == 3)
        //                {
        //                    model.a = new VERTEX(Convert.ToSingle(points[0][0]),
        //                          Convert.ToSingle(points[0][1]),
        //                          Convert.ToSingle(points[0][2]));
        //                    model.b = new VERTEX(Convert.ToSingle(points[1][0]),
        //                         Convert.ToSingle(points[1][1]),
        //                         Convert.ToSingle(points[1][2]));
        //                    model.c = new VERTEX(Convert.ToSingle(points[2][0]),
        //                          Convert.ToSingle(points[2][1]),
        //                          Convert.ToSingle(points[2][2]));
        //                    faceVertexList.Add(model);

        //                }
        //            }

        //            swFace = (Face2)swFace.GetNextFace();

        //        }


        //    }
        //    SaveAs(faceVertexList,Path.GetDirectoryName(swModel.GetPathName()));
        //}

        //public void SaveAs( List<FaceVertexModel> vertexs,string Path)
        //{
            //// create two materials

            //var material1 = new MaterialBuilder()
            //    .WithDoubleSide(true)
            //    .WithMetallicRoughnessShader()
            //    .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 1));

            //var material2 = new MaterialBuilder()
            //    .WithDoubleSide(true)
            //    .WithMetallicRoughnessShader()
            //    .WithChannelParam("BaseColor", new Vector4(1, 0, 1, 1));

            //// create a mesh with two primitives, one for each material

            //var mesh = new MeshBuilder<VERTEX>("mesh");

            //var prim = mesh.UsePrimitive(material1);

            //foreach (var item in vertexs)
            //{
            //    prim.AddTriangle(item.a,item.b,item.c);
                
            //}
            ////prim.AddTriangle(new VERTEX(-10, 0, 0), new VERTEX(10, 0, 0), new VERTEX(0, 10, 0));
            ////prim.AddTriangle(new VERTEX(10, 0, 0), new VERTEX(-10, 0, 0), new VERTEX(0, -10, 0));

            ////prim = mesh.UsePrimitive(material2);
            ////prim.AddQuadrangle(new VERTEX(-5, 0, 3), new VERTEX(0, -5, 3), new VERTEX(5, 0, 3), new VERTEX(0, 5, 3));

            //// create a scene

            //var scene = new SharpGLTF.Scenes.SceneBuilder();

            //scene.AddMesh(mesh, Matrix4x4.Identity);

            //// save the model in different formats

            //var model = scene.ToSchema2();
            //model.SaveAsWavefront(Path+"\\"+"mesh.obj");
            //model.SaveGLB(Path + "\\" + "mesh.glb");
            //model.SaveGLTF(Path + "\\" + "mesh.gltf");
        }
    }
   
}
