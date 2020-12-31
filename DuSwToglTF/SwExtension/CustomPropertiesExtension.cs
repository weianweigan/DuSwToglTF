using SharpGLTF.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace DuSwToglTF.SwExtension
{
    /// <summary>
    /// 自定义属性扩展
    /// </summary>
    public class CustomPropertiesExtension : JsonSerializable
    {
        public struct CustomProperty
        {
            public CustomProperty(string name, string value, string nodeName)
            {
                Name = name;
                Value = value;
                NodeName = nodeName;
            }

            public string Name { get; set; }

            public string Value { get; set; }

            public string NodeName { get; set; }
        }

        public List<CustomProperty> CustomProperties { get; } = new List<CustomProperty>();

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
            SerializeProperty(writer, "SolidWorksCustomProperties", CustomProperties);
        }
    }
}
