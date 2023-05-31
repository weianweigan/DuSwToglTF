using GalaSoft.MvvmLight;
using SharpGLTF.IO;
using SharpGLTF.Sw;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.Json;

namespace DuSwToglTF.SwExtension
{
    public class CustomPropertyGroup : CustomProperty
    {
        public CustomPropertyGroup(string componentName)
        {
            ComponentName = componentName;
        }

        public override string ToString()
        {
            return ComponentName;
        }
    }

    public class CustomProperty:ObservableObject
    {
        private bool _isChecked = true;
        private List<CustomProperty> _children = new List<CustomProperty>();

        protected CustomProperty() { }

        public CustomProperty(string name, string value, string compName)
        {
            Name = name;
            Value = value;
            ComponentName = compName;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsChecked
        {
            get => _isChecked; set
            {
                Set(ref _isChecked ,value);
                foreach (var child in Children)
                {
                    child.IsChecked = value;
                }
            }
        }

        public string ComponentName { get; set; }

        public List<CustomProperty> Children { get => _children; set => _children = value; }

        public virtual SwCustomProperty CopyChecked()
        {
            if (!IsChecked)
            {
                return null;
            }

            if (this is CustomPropertyGroup)
            {
                var newGroup = new SwCustomProperty(Name,Value,ComponentName);
                foreach (var child in Children)
                {
                    var newChild = child.CopyChecked();
                    if (newChild != null)
                    {
                        newGroup.Children.Add(newChild); 
                    }
                }
                return newGroup;
            }
            else
            {
                return new SwCustomProperty(Name, Value, ComponentName);
            }
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }

    /// <summary>
    /// 自定义属性扩展
    /// </summary>
    public class CustomPropertiesExtension : JsonSerializable
    {
        public CustomPropertiesExtension(CustomPropertyGroup customPropertyGroup)
        {
            CustomPropertyGroup = customPropertyGroup;
        }

        public CustomPropertyGroup CustomPropertyGroup { get; set; }

        protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
        {
            switch (jsonPropertyName)
            {
                case "SolidWorksCustomProperties":
                    DeserializePropertyValue<List<CustomProperty>>(ref reader);
                    break;
                default:
                    break;
            }
        }

        protected override void SerializeProperties(Utf8JsonWriter writer)
        {
            SerializeProperty(writer, "SolidWorksCustomProperties", CustomPropertyGroup);
        }
    }
}
