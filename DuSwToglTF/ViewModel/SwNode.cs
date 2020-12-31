using DuSwToglTF.Extension;
using GalaSoft.MvvmLight;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DuSwToglTF.ViewModel
{
    public class SwNode : ObservableObject
    {
        private string name;

        public string Name { get => name; set => Set(ref name , value); }

        public bool IsExpanded { get; set; } = true;

        public List<SwNode> Children { get; set; } = new List<SwNode>();

        internal static SwNode Create(IModelDoc2 doc)
        {
            SwNode root;

            var type = (swDocumentTypes_e)doc.GetType();
            if (type == swDocumentTypes_e.swDocPART)
            {
                root = new PartNode(doc);
            }
            else
            {
                root = new AssemblyNode(doc);
            }
            return root;
        }
    }

    public class MeshNode : SwNode
    {
        private IBody2 body;
        private readonly Matrix4x4 location;

        public MeshNode(IBody2 body, System.Numerics.Matrix4x4 location)
        {
            this.body = body;
            this.location = location;
            Name = body.Name;
        }

        internal static SwNode Create(IBody2 body, System.Numerics.Matrix4x4 location)
        {
            return new MeshNode(body,location);
        }
    }

    public class PartNode : SwNode
    {
        private IModelDoc2 doc;

        public PartNode(IModelDoc2 doc)
        {
            Name = doc.GetTitle();
            this.doc = doc;
            var partDoc = doc as IPartDoc;
            var bodies = partDoc.GetBodiesEx();
            if (bodies != null)
            {
                foreach (var body in bodies)
                {
                    Children.Add(MeshNode.Create(body,Matrix4x4.Identity));
                }
            }
        }
    }

    public class AssemblyNode : SwNode
    {
        private IModelDoc2 doc;

        public AssemblyNode(IModelDoc2 doc)
        {
            Name = doc.GetTitle();
            this.doc = doc;

            var bodies = doc.GetBodyDataModels();

            if (bodies != null)
            {
                foreach (var body in bodies)
                {
                    Children.Add(MeshNode.Create(body.Body,body.Location));
                }
            }

        }
    }
}
