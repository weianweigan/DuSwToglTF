using SharpGLTF.Schema2;
using System.Text.Json;

namespace SharpGLTF.Sw
{
    public class SolidWorks:ExtraProperties
    {
        public SwCustomProperty SwCustomProperty { get; set; }

        protected override void SerializeProperties(Utf8JsonWriter writer)
        {
            base.SerializeProperties(writer);

            if(SwCustomProperty != null)
            {
                SerializePropertyObject(writer, "SwCustomProperty", SwCustomProperty);
            }
        }

        protected override void DeserializeProperty(string property, ref Utf8JsonReader reader)
        {
            SwCustomProperty = DeserializePropertyValue<SwCustomProperty>(ref reader);
        }
    }
}
