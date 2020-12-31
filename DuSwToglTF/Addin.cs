using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands.Attributes;
using Xarial.XCad.UI.Commands.Enums;
using Xarial.XCad.UI.Commands;
using DuSwToglTF.View;
using Xarial.XCad.Base.Attributes;
using System.ComponentModel;
using System;
using System.Reflection;
using System.IO;

namespace DuSwToglTF
{
    [Title("DuSwToglTFAddin")]
    [Description("glTF Exporter for SolidWorks")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Addin:SwAddInEx
    {
        public override void OnConnect()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            System.Numerics.Vector2 v = new System.Numerics.Vector2();

            CommandManager.AddCommandGroup<SaveCommamds>().CommandClick += Addin_CommandClick; ;
        }

        private void Addin_CommandClick(SaveCommamds spec)
        {
            var window = CreatePopupWindow<ExportWindow>();
            window.Control.Init(Application.Sw.IActiveDoc2);
            window.ShowDialog();
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyPath = string.Empty;
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {
                assemblyPath = Path.Combine(AssemblyPath, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    System.Diagnostics.Debug.Print($"Assembly Load Error{assemblyPath}");
                }

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyName), ex);
            }
        }

        public string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }

    [Title("glTFExporter")]
    public enum SaveCommamds
    {
        [CommandItemInfo(true,true,WorkspaceTypes_e.Part | WorkspaceTypes_e.Assembly,true)]
        [Title("glTFExporter")]
        SaveAsglTF
    }
}
