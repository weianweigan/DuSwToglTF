using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SharpGLTF.Sw
{
    public class SwCustomProperty : ExtraProperties
    {
        public SwCustomProperty()
        {
            
        }

        public SwCustomProperty(string name, string value, string componentName)
        {
            Name = name;
            Value = value;
            ComponentName = componentName;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string ComponentName { get; set; }

        public List<SwCustomProperty> Children { get; set; } = new List<SwCustomProperty>();

        protected override void SerializeProperties(Utf8JsonWriter writer)
        {
            base.SerializeProperties(writer);

            SerializeProperty(writer, "name", Name);
            SerializeProperty(writer, "value", Value);
            SerializeProperty(writer, "componentName", ComponentName);
            SerializeProperty(writer, "children", Children);
        }

        protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
        {
            switch (jsonPropertyName)
            {
                case "name": Name = DeserializePropertyValue<String>(ref reader); break;
                case "value": Value = DeserializePropertyValue<String>(ref reader); break;
                case "componentName": ComponentName= DeserializePropertyValue<String>(ref reader); break;
                case "children": DeserializePropertyList<SwCustomProperty>(ref reader,Children); break;
                default: base.DeserializeProperty(jsonPropertyName, ref reader); break;
            }
        }
    }
}
