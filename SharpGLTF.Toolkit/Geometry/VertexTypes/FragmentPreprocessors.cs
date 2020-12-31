﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SharpGLTF.Geometry.VertexTypes
{
    /// <summary>
    /// Defines a set of vertex fragment preprocessors to be used with <see cref="VertexPreprocessor{TvG, TvM, TvS}"/>
    /// </summary>
    static class FragmentPreprocessors
    {
        /// <summary>
        /// validates a vertex geometry, throwing exceptions if found invalid
        /// </summary>
        /// <typeparam name="TvG">
        /// The vertex fragment type with Position, Normal and Tangent.
        /// Valid types are:
        /// <see cref="VertexPosition"/>,
        /// <see cref="VertexPositionNormal"/>,
        /// <see cref="VertexPositionNormalTangent"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvG"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvG"/> vertex, or null if sanitization failed.</returns>
        /// <exception cref="ArgumentException">When the vertex is invalid.</exception>
        public static TvG? ValidateVertexGeometry<TvG>(TvG vertex)
            where TvG : struct, IVertexGeometry
        {
            var p = vertex.GetPosition();
            Guard.IsTrue(p._IsFinite(), "Position", "Values are not finite.");

            if (vertex.TryGetNormal(out Vector3 n))
            {
                Guard.IsTrue(n._IsFinite(), "Normal", "Values are not finite.");
                Guard.MustBeBetweenOrEqualTo(n.Length(), 0.99f, 1.01f, "Normal.Length");
            }

            if (vertex.TryGetTangent(out Vector4 t))
            {
                Guard.IsTrue(t._IsFinite(), "Tangent", "Values are not finite.");
                Guard.IsTrue(t.W == 1 || t.W == -1, "Tangent.W", "Invalid value");
                Guard.MustBeBetweenOrEqualTo(new Vector3(t.X, t.Y, t.Z).Length(), 0.99f, 1.01f, "Tangent.XYZ.Length");
            }

            return vertex;
        }

        /// <summary>
        /// Sanitizes a vertex material with a best effort approach
        /// </summary>
        /// <typeparam name="TvM">
        /// The vertex fragment type with Colors and Texture Coordinates.
        /// Valid types are:
        /// <see cref="VertexEmpty"/>,
        /// <see cref="VertexColor1"/>,
        /// <see cref="VertexTexture1"/>,
        /// <see cref="VertexColor1Texture1"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvM"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvM"/> vertex, or null if sanitization failed.</returns>
        public static TvM? ValidateVertexMaterial<TvM>(TvM vertex)
            where TvM : struct, IVertexMaterial
        {
            for (int i = 0; i < vertex.MaxColors; ++i)
            {
                var c = vertex.GetColor(i);
                Guard.IsTrue(c._IsFinite(), $"Color{i}", "Values are not finite.");
                Guard.MustBeBetweenOrEqualTo(c.X, 0, 1, $"Color{i}.R");
                Guard.MustBeBetweenOrEqualTo(c.Y, 0, 1, $"Color{i}.G");
                Guard.MustBeBetweenOrEqualTo(c.Z, 0, 1, $"Color{i}.B");
                Guard.MustBeBetweenOrEqualTo(c.W, 0, 1, $"Color{i}.A");
            }

            for (int i = 0; i < vertex.MaxTextCoords; ++i)
            {
                var t = vertex.GetTexCoord(i);
                Guard.IsTrue(t._IsFinite(), $"TexCoord{i}", "Values are not finite.");
            }

            return vertex;
        }

        /// <summary>
        /// Sanitizes a vertex skinning with a best effort approach
        /// </summary>
        /// <typeparam name="TvS">
        /// The vertex fragment type with Skin Joint Weights.
        /// Valid types are:
        /// <see cref="VertexEmpty"/>,
        /// <see cref="VertexJoints4"/>,
        /// <see cref="VertexJoints8"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvS"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvS"/> vertex, or null if sanitization failed.</returns>
        public static TvS? ValidateVertexSkinning<TvS>(TvS vertex)
            where TvS : struct, IVertexSkinning
        {
            // validation must ensure that:
            // - every joint is unique
            // - every joint and weight is 0 or positive value
            // - 0 weight joints point to joint 0
            // - sum of weights is 1

            if (vertex.MaxBindings == 0) return vertex;

            // Apparently the consensus is that weights are required to be normalized.
            // More here: https://github.com/KhronosGroup/glTF/issues/1213

            float weightsSum = 0;

            for (int i = 0; i < vertex.MaxBindings; ++i)
            {
                var (index, weight) = vertex.GetJointBinding(i);

                Guard.MustBeGreaterThanOrEqualTo(index, 0, $"Joint{i}");
                Guard.IsTrue(weight._IsFinite(), $"Weight{i}", "Values are not finite.");
                if (weight == 0) Guard.IsTrue(index == 0, "joints with weight zero must be set to zero");

                weightsSum += weight;
            }

            // TODO: check that joints are unique

            Guard.MustBeBetweenOrEqualTo(weightsSum, 0.99f, 1.01f, "Weights SUM");

            return vertex;
        }

        /// <summary>
        /// Sanitizes a vertex geometry with a best effort approach
        /// </summary>
        /// <typeparam name="TvG">
        /// The vertex fragment type with Position, Normal and Tangent.
        /// Valid types are:
        /// <see cref="VertexPosition"/>,
        /// <see cref="VertexPositionNormal"/>,
        /// <see cref="VertexPositionNormalTangent"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvG"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvG"/> vertex, or null if sanitization failed.</returns>
        public static TvG? SanitizeVertexGeometry<TvG>(TvG vertex)
            where TvG : struct, IVertexGeometry
        {
            var p = vertex.GetPosition();

            if (!p._IsFinite()) return null;

            if (vertex.TryGetNormal(out Vector3 n))
            {
                if (!n._IsFinite()) n = p;
                if (n == Vector3.Zero) n = p;
                if (n == Vector3.Zero) return null;

                var l = n.Length();
                if (l < 0.99f || l > 0.01f) vertex.SetNormal(Vector3.Normalize(n));
            }

            if (vertex.TryGetTangent(out Vector4 tw))
            {
                if (!tw._IsFinite()) return null;

                var t = new Vector3(tw.X, tw.Y, tw.Z);
                if (t == Vector3.Zero) return null;

                if (tw.W > 0) tw.W = 1;
                if (tw.W < 0) tw.W = -1;

                var l = t.Length();
                if (l < 0.99f || l > 0.01f) t = Vector3.Normalize(t);

                vertex.SetTangent(new Vector4(t, tw.W));
            }

            return vertex;
        }

        /// <summary>
        /// Sanitizes a vertex material with a best effort approach
        /// </summary>
        /// <typeparam name="TvM">
        /// The vertex fragment type with Colors and Texture Coordinates.
        /// Valid types are:
        /// <see cref="VertexEmpty"/>,
        /// <see cref="VertexColor1"/>,
        /// <see cref="VertexTexture1"/>,
        /// <see cref="VertexColor1Texture1"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvM"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvM"/> vertex, or null if sanitization failed.</returns>
        public static TvM? SanitizeVertexMaterial<TvM>(TvM vertex)
            where TvM : struct, IVertexMaterial
        {
            for (int i = 0; i < vertex.MaxColors; ++i)
            {
                var c = vertex.GetColor(i);
                if (!c._IsFinite()) c = Vector4.Zero;
                c = Vector4.Min(Vector4.One, c);
                c = Vector4.Max(Vector4.Zero, c);
                vertex.SetColor(i, c);
            }

            for (int i = 0; i < vertex.MaxTextCoords; ++i)
            {
                var t = vertex.GetTexCoord(i);
                if (!t._IsFinite()) vertex.SetTexCoord(i, Vector2.Zero);
            }

            return vertex;
        }

        /// <summary>
        /// Sanitizes a vertex skinning with a best effort approach
        /// </summary>
        /// <typeparam name="TvS">
        /// The vertex fragment type with Skin Joint Weights.
        /// Valid types are:
        /// <see cref="VertexEmpty"/>,
        /// <see cref="VertexJoints4"/>,
        /// <see cref="VertexJoints8"/>.
        /// </typeparam>
        /// <param name="vertex">the source <typeparamref name="TvS"/> vertex.</param>
        /// <returns>A sanitized <typeparamref name="TvS"/> vertex, or null if sanitization failed.</returns>
        public static TvS? SanitizeVertexSkinning<TvS>(TvS vertex)
            where TvS : struct, IVertexSkinning
        {
            if (vertex.MaxBindings == 0) return vertex;

            // Apparently the consensus is that weights are required to be normalized.
            // More here: https://github.com/KhronosGroup/glTF/issues/1213

            var sparse = Transforms.SparseWeight8.OrderedByWeight(vertex.GetWeights());

            var sum = sparse.WeightSum;
            if (sum == 0) return default(TvS);

            sparse = Transforms.SparseWeight8.Multiply(sparse, 1.0f / sum);

            vertex.SetWeights(sparse);

            return vertex;
        }
    }
}
