using SolidWorks.Interop.sldworks;
using SharpGLTF.Schema2;
using SharpGLTF.Scenes;
using DuSwToglTF.Extension;
using SharpGLTF.Materials;
using System.Numerics;
using System.IO;
using System;
using System.Windows;
using DuSwToglTF.SwExtension;
using SharpGLTF.IO;

namespace DuSwToglTF.ExportContext
{
    public class ExportOptions
    {
        public bool ImprovedQuality { get; set; } = true;
    }

    public class glTFExportContext : IExportContext
    {
        protected readonly string _savePathName;
        protected ModelRoot _model;
        protected SceneBuilder _sceneBuilder;
        protected ExportOptions _options;

        public glTFExportContext(string savePathName,ExportOptions options = null)
        {
            this._savePathName = savePathName;
            if (_options == null)
            {
                _options = new ExportOptions();
            }
        }

        public string SavePathName => _savePathName;

        public CustomPropertiesExtension CustomProperties { get; } = new CustomPropertiesExtension();

        public void Finish(bool obj,bool gltf,bool glb)
        {
           
            //_sceneBuilder.Extras = JsonContent.CreateFrom(CustomProperties.CustomProperties);
            _model = _sceneBuilder.ToGltf2();

            //_model.SetExtension<CustomPropertiesExtension>(CustomProperties);

            if(obj)
                Save(".obj",(filePathName) => _model.SaveAsWavefront(filePathName));            
            if(glb)
                Save(".glb",(filePathName) => _model.SaveGLB(filePathName));
            if (gltf)
                Save(".glTF", (filePathName) => SaveglTF(filePathName));
        }

        private void SaveglTF(string filePathName)
        {

            _model.SaveGLTF(filePathName, new WriteSettings()
            {
                JsonIndented = true,
            });
        }

        private void Save(string extension,Action<string> action)
        {
            var filePathName = _savePathName + extension;
            //判断文件是否存在
            if (File.Exists(filePathName))
            {
                var result = MessageBox.Show($"Replace {filePathName} ??", "File Exist", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            action?.Invoke(filePathName);
        }

        public void OnBodyBegin(IBody2 body,MaterialBuilder docMatBuilder,Matrix4x4 postion)
        {
            _sceneBuilder.AddRigidMesh(body.GetBodyMeshBuilder(docMatBuilder),postion);
        }

        public void OnCustomPropertyBegin(CustomPropertiesExtension.CustomProperty prop)
        {
            CustomProperties.CustomProperties.Add(prop);   
        }

        public bool Start()
        {
            _sceneBuilder = new SceneBuilder();
            return _sceneBuilder != null;
        }
    }

    public interface IExportContext
    {
        bool Start();

        void OnBodyBegin(IBody2 body, MaterialBuilder docMatBuilder, Matrix4x4 postion);

        void Finish(bool obj, bool gltf, bool glb);
    }
}
