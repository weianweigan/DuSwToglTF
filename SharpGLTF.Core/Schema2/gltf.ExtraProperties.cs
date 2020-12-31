﻿using System;
using System.Collections.Generic;
using System.Linq;

// using Newtonsoft.Json;
using System.Text.Json;

using SharpGLTF.IO;

using JsonToken = System.Text.Json.JsonTokenType;

namespace SharpGLTF.Schema2
{
    /// <summary>
    /// Represents the base class for all glTF 2 Schema objects.
    /// </summary>
    /// <remarks>
    /// Defines the <see cref="Extras"/> property for every glTF object.
    /// </remarks>
    public abstract class ExtraProperties : JsonSerializable
    {
        #region data

        private readonly List<JsonSerializable> _extensions = new List<JsonSerializable>();

        private IO.JsonContent _extras;

        #endregion

        #region properties

        /// <summary>
        /// Gets a collection of <see cref="JsonSerializable"/> instances.
        /// </summary>
        public IReadOnlyCollection<JsonSerializable> Extensions => _extensions;

        /// <summary>
        /// Gets the extras of this instance, where the value can be
        /// an <see cref="IConvertible"/>,
        /// a <see cref="IReadOnlyList{Object}"/> or
        /// a <see cref="IReadOnlyDictionary{String, Object}"/>.
        /// </summary>
        public IO.JsonContent Extras
        {
            get => _extras;
            set => _extras = value.DeepClone();
        }

        #endregion

        #region API

        public T GetExtension<T>()
            where T : JsonSerializable
        {
            return _extensions.OfType<T>().FirstOrDefault();
        }

        public void SetExtension<T>(T value)
            where T : JsonSerializable
        {
            Guard.NotNull(value, nameof(value));

            var idx = _extensions.IndexOf(item => item.GetType() == typeof(T));

            if (idx >= 0) { _extensions[idx] = value; return; }

            _extensions.Add(value);
        }

        public void RemoveExtensions<T>(T value)
            where T : JsonSerializable
        {
            _extensions.RemoveAll(item => item == value);
        }

        public void RemoveExtensions<T>()
            where T : JsonSerializable
        {
            _extensions.RemoveAll(item => item.GetType() == typeof(T));
        }

        /// <summary>
        /// Gets a collection of <see cref="ExtraProperties"/> instances stored by this object.
        /// </summary>
        /// <returns>A collection of <see cref="ExtraProperties"/> instances.</returns>
        protected virtual IEnumerable<ExtraProperties> GetLogicalChildren()
        {
            return _extensions.OfType<ExtraProperties>();
        }

        protected static IEnumerable<ExtraProperties> Flatten(ExtraProperties container)
        {
            if (container == null) yield break;

            yield return container;

            foreach (var c in container.GetLogicalChildren())
            {
                var cc = Flatten(c);

                foreach (var ccc in cc) yield return ccc;
            }
        }

        #endregion

        #region validation

        protected override void OnValidateReferences(Validation.ValidationContext validate)
        {
            base.OnValidateReferences(validate);

            foreach (var lc in this.GetLogicalChildren())
            {
                lc.ValidateReferences(validate);
            }

            foreach (var ext in this.Extensions) ext.ValidateReferences(validate);

            if (this._extras.Content is JsonSerializable js) js.ValidateReferences(validate);
        }

        protected override void OnValidateContent(Validation.ValidationContext validate)
        {
            base.OnValidateContent(validate);

            foreach (var lc in this.GetLogicalChildren())
            {
                lc.ValidateContent(validate);
            }

            if (this._extras.Content is JsonSerializable js) js.ValidateContent(validate);

            if (this._extras.Content != null) validate.IsJsonSerializable("Extras", this._extras.Content);
        }

        #endregion

        #region serialization API

        /// <summary>
        /// Writes the properties of the current instance to a <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The target writer.</param>
        protected override void SerializeProperties(Utf8JsonWriter writer)
        {
            if (_extensions.Count > 0)
            {
                var dict = _ToDictionary(this, _extensions);
                SerializeProperty(writer, "extensions", dict);
            }

            // todo, only write _extras if it's a known serializable type.
            var content = _extras.Content;
            if (content == null) return;
            if (!IO.JsonContent.IsJsonSerializable(content)) return;

            SerializeProperty(writer, "extras", content);
        }

        private static IReadOnlyDictionary<string, JsonSerializable> _ToDictionary(JsonSerializable context, IEnumerable<JsonSerializable> serializables)
        {
            var dict = new Dictionary<string, JsonSerializable>();

            foreach (var val in serializables)
            {
                if (val == null) continue;

                string key = null;

                if (val is UnknownNode unk) key = unk.Name;
                else key = ExtensionsFactory.Identify(context.GetType(), val.GetType());

                if (key == null) continue;
                dict[key] = val;
            }

            return dict;
        }

        /// <summary>
        /// Reads the properties of the current instance from a <see cref="Utf8JsonReader"/>.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        /// <param name="reader">The source reader.</param>
        protected override void DeserializeProperty(string property, ref Utf8JsonReader reader)
        {
            Guard.NotNullOrEmpty(property, nameof(property));

            switch (property)
            {
                case "extensions": _DeserializeExtensions(this, ref reader, _extensions); break;

                case "extras":
                    {
                        var content = DeserializeUnknownObject(ref reader);
                        _extras = JsonContent._Wrap(content);
                        break;
                    }

                default: reader.Skip(); break;
            }
        }

        private static void _DeserializeExtensions(JsonSerializable parent, ref Utf8JsonReader reader, IList<JsonSerializable> extensions)
        {
            reader.Read();

            if (reader.TokenType == JsonToken.StartObject)
            {
                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    var key = reader.GetString();

                    var val = ExtensionsFactory.Create(parent, key);

                    if (val == null) val = new UnknownNode(key);

                    val.Deserialize(ref reader);
                    extensions.Add(val);
                    continue;
                }
            }

            reader.Skip();
        }

        #endregion
    }
}