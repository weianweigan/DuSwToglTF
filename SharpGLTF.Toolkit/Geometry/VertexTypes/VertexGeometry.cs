﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace SharpGLTF.Geometry.VertexTypes
{
    public interface IVertexGeometry
    {
        void Validate();

        Vector3 GetPosition();
        Boolean TryGetNormal(out Vector3 normal);
        Boolean TryGetTangent(out Vector4 tangent);

        void SetPosition(Vector3 position);
        void SetNormal(Vector3 normal);
        void SetTangent(Vector4 tangent);

        void ApplyTransform(Matrix4x4 xform);

        VertexGeometryDelta Subtract(IVertexGeometry baseValue);

        void Add(VertexGeometryDelta delta);
    }

    /// <summary>
    /// Defines a Vertex attribute with a Position.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{_GetDebuggerDisplay(),nq}")]
    public struct VertexPosition : IVertexGeometry, IEquatable<VertexPosition>
    {
        #region debug

        private string _GetDebuggerDisplay() => $"𝐏:{Position}";

        #endregion

        #region constructors

        public VertexPosition(Vector3 position)
        {
            this.Position = position;
        }

        public VertexPosition(float px, float py, float pz)
        {
            this.Position = new Vector3(px, py, pz);
        }

        public VertexPosition(IVertexGeometry src)
        {
            Guard.NotNull(src, nameof(src));
            this.Position = src.GetPosition();
        }

        public static implicit operator VertexPosition(Vector3 position)
        {
            return new VertexPosition(position);
        }

        #endregion

        #region data

        [VertexAttribute("POSITION")]
        public Vector3 Position;
        public override bool Equals(object obj) { return obj is VertexPosition other && AreEqual(this, other); }
        public bool Equals(VertexPosition other) { return AreEqual(this, other); }
        public static bool operator ==(in VertexPosition a, in VertexPosition b) { return AreEqual(a, b); }
        public static bool operator !=(in VertexPosition a, in VertexPosition b) { return !AreEqual(a, b); }
        public static bool AreEqual(in VertexPosition a, in VertexPosition b)
        {
            return a.Position == b.Position;
        }

        public override int GetHashCode() { return Position.GetHashCode(); }

        #endregion

        #region API

        void IVertexGeometry.SetPosition(Vector3 position) { this.Position = position; }

        void IVertexGeometry.SetNormal(Vector3 normal) { }

        void IVertexGeometry.SetTangent(Vector4 tangent) { }

        public VertexGeometryDelta Subtract(IVertexGeometry baseValue)
        {
            return new VertexGeometryDelta((VertexPosition)baseValue, this);
        }

        public void Add(VertexGeometryDelta delta)
        {
            this.Position += delta.PositionDelta;
        }

        public Vector3 GetPosition() { return this.Position; }

        public bool TryGetNormal(out Vector3 normal) { normal = default; return false; }

        public bool TryGetTangent(out Vector4 tangent) { tangent = default; return false; }

        public void ApplyTransform(Matrix4x4 xform)
        {
            Position = Vector3.Transform(Position, xform);
        }

        public void Validate() { FragmentPreprocessors.ValidateVertexGeometry(this); }

        #endregion
    }

    /// <summary>
    /// Defines a Vertex attribute with a Position and a Normal.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{_GetDebuggerDisplay(),nq}")]
    public struct VertexPositionNormal : IVertexGeometry, IEquatable<VertexPositionNormal>
    {
        #region debug

        private string _GetDebuggerDisplay() => $"𝐏:{Position} 𝚴:{Normal}";

        #endregion

        #region constructors

        public VertexPositionNormal(Vector3 p, Vector3 n)
        {
            this.Position = p;
            this.Normal = n;
        }

        public VertexPositionNormal(float px, float py, float pz, float nx, float ny, float nz)
        {
            this.Position = new Vector3(px, py, pz);
            this.Normal = new Vector3(nx, ny, nz);
        }

        public VertexPositionNormal(IVertexGeometry src)
        {
            Guard.NotNull(src, nameof(src));

            this.Position = src.GetPosition();
            src.TryGetNormal(out this.Normal);
        }

        public static implicit operator VertexPositionNormal((Vector3 Pos, Vector3 Nrm) tuple)
        {
            return new VertexPositionNormal(tuple.Pos, tuple.Nrm);
        }

        #endregion

        #region data

        [VertexAttribute("POSITION")]
        public Vector3 Position;

        [VertexAttribute("NORMAL")]
        public Vector3 Normal;

        public override bool Equals(object obj) { return obj is VertexPositionNormal other && AreEqual(this, other); }
        public bool Equals(VertexPositionNormal other) { return AreEqual(this, other); }
        public static bool operator ==(in VertexPositionNormal a, in VertexPositionNormal b) { return AreEqual(a, b); }
        public static bool operator !=(in VertexPositionNormal a, in VertexPositionNormal b) { return !AreEqual(a, b); }
        public static bool AreEqual(in VertexPositionNormal a, in VertexPositionNormal b)
        {
            return a.Position == b.Position && a.Normal == b.Normal;
        }

        public override int GetHashCode() { return Position.GetHashCode(); }

        #endregion

        #region API

        void IVertexGeometry.SetPosition(Vector3 position) { this.Position = position; }

        void IVertexGeometry.SetNormal(Vector3 normal) { this.Normal = normal; }

        void IVertexGeometry.SetTangent(Vector4 tangent) { }

        public VertexGeometryDelta Subtract(IVertexGeometry baseValue)
        {
            return new VertexGeometryDelta((VertexPositionNormal)baseValue, this);
        }

        public void Add(VertexGeometryDelta delta)
        {
            this.Position += delta.PositionDelta;
            this.Normal += delta.NormalDelta;
        }

        public Vector3 GetPosition() { return this.Position; }

        public bool TryGetNormal(out Vector3 normal) { normal = this.Normal; return true; }

        public bool TryGetTangent(out Vector4 tangent) { tangent = default; return false; }

        public void ApplyTransform(Matrix4x4 xform)
        {
            Position = Vector3.Transform(Position, xform);
            Normal = Vector3.Normalize(Vector3.TransformNormal(Normal, xform));
        }

        public void Validate() { FragmentPreprocessors.ValidateVertexGeometry(this); }

        #endregion
    }

    /// <summary>
    /// Defines a Vertex attribute with a Position, a Normal and a Tangent.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{_GetDebuggerDisplay(),nq}")]
    public struct VertexPositionNormalTangent : IVertexGeometry, IEquatable<VertexPositionNormalTangent>
    {
        #region debug

        private string _GetDebuggerDisplay() => $"𝐏:{Position} 𝚴:{Normal} 𝚻:{Tangent}";

        #endregion

        #region constructors

        public VertexPositionNormalTangent(Vector3 p, Vector3 n, Vector4 t)
        {
            this.Position = p;
            this.Normal = n;
            this.Tangent = t;
        }

        public VertexPositionNormalTangent(IVertexGeometry src)
        {
            Guard.NotNull(src, nameof(src));

            this.Position = src.GetPosition();
            src.TryGetNormal(out this.Normal);
            src.TryGetTangent(out this.Tangent);
        }

        public static implicit operator VertexPositionNormalTangent((Vector3 Pos, Vector3 Nrm, Vector4 Tgt) tuple)
        {
            return new VertexPositionNormalTangent(tuple.Pos, tuple.Nrm, tuple.Tgt);
        }

        #endregion

        #region data

        [VertexAttribute("POSITION")]
        public Vector3 Position;

        [VertexAttribute("NORMAL")]
        public Vector3 Normal;

        [VertexAttribute("TANGENT")]
        public Vector4 Tangent;

        public override bool Equals(object obj) { return obj is VertexPositionNormalTangent other && AreEqual(this, other); }
        public bool Equals(VertexPositionNormalTangent other) { return AreEqual(this, other); }
        public static bool operator ==(in VertexPositionNormalTangent a, in VertexPositionNormalTangent b) { return AreEqual(a, b); }
        public static bool operator !=(in VertexPositionNormalTangent a, in VertexPositionNormalTangent b) { return !AreEqual(a, b); }
        public static bool AreEqual(in VertexPositionNormalTangent a, in VertexPositionNormalTangent b)
        {
            return a.Position == b.Position && a.Normal == b.Normal && a.Tangent == b.Tangent;
        }

        public override int GetHashCode() { return Position.GetHashCode(); }

        #endregion

        #region API

        void IVertexGeometry.SetPosition(Vector3 position) { this.Position = position; }

        void IVertexGeometry.SetNormal(Vector3 normal) { this.Normal = normal; }

        void IVertexGeometry.SetTangent(Vector4 tangent) { this.Tangent = tangent; }

        public VertexGeometryDelta Subtract(IVertexGeometry baseValue)
        {
            return new VertexGeometryDelta((VertexPositionNormalTangent)baseValue, this);
        }

        public void Add(VertexGeometryDelta delta)
        {
            this.Position += delta.PositionDelta;
            this.Normal += delta.NormalDelta;
            this.Tangent += new Vector4(delta.TangentDelta, 0);
        }

        public Vector3 GetPosition() { return this.Position; }

        public bool TryGetNormal(out Vector3 normal) { normal = this.Normal; return true; }

        public bool TryGetTangent(out Vector4 tangent) { tangent = this.Tangent; return true; }

        public void ApplyTransform(Matrix4x4 xform)
        {
            Position = Vector3.Transform(Position, xform);
            Normal = Vector3.Normalize(Vector3.TransformNormal(Normal, xform));

            // TODO: not sure if this is correct, must be checked. Most probably, if the xform handedness if negative, Tangent.W must be reversed.
            var txyz = Vector3.Normalize(Vector3.TransformNormal(new Vector3(Tangent.X, Tangent.Y, Tangent.Z), xform));
            Tangent = new Vector4(txyz, Tangent.W);
        }

        public void Validate() { FragmentPreprocessors.ValidateVertexGeometry(this); }

        #endregion
    }

    /// <summary>
    /// Defines a Vertex attribute with a Position, a Normal and a Tangent.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{_GetDebuggerDisplay(),nq}")]
    public struct VertexGeometryDelta : IVertexGeometry, IEquatable<VertexGeometryDelta>
    {
        #region debug

        private string _GetDebuggerDisplay() => $"Δ𝐏:{PositionDelta} Δ𝚴:{NormalDelta} Δ𝚻:{TangentDelta}";

        #endregion

        #region constructors

        public static implicit operator VertexGeometryDelta(Vector3 position)
        {
            return new VertexGeometryDelta(position, Vector3.Zero, Vector3.Zero);
        }

        public static implicit operator VertexGeometryDelta((Vector3 Pos, Vector3 Nrm) tuple)
        {
            return new VertexGeometryDelta(tuple.Pos, tuple.Nrm, Vector3.Zero);
        }

        public static implicit operator VertexGeometryDelta((Vector3 Pos, Vector3 Nrm, Vector3 tgt) tuple)
        {
            return new VertexGeometryDelta(tuple.Pos, tuple.Nrm, tuple.tgt);
        }

        public VertexGeometryDelta(IVertexGeometry src)
        {
            Guard.NotNull(src, nameof(src));

            this.PositionDelta = src.GetPosition();
            src.TryGetNormal(out this.NormalDelta);
            src.TryGetTangent(out Vector4 t);

            this.TangentDelta = new Vector3(t.X, t.Y, t.Z);
        }

        public VertexGeometryDelta(Vector3 p, Vector3 n, Vector3 t)
        {
            this.PositionDelta = p;
            this.NormalDelta = n;
            this.TangentDelta = t;
        }

        internal VertexGeometryDelta(VertexPosition rootVal, VertexPosition morphVal)
        {
            PositionDelta = morphVal.Position - rootVal.Position;
            NormalDelta = Vector3.Zero;
            TangentDelta = Vector3.Zero;
        }

        internal VertexGeometryDelta(VertexPositionNormal rootVal, VertexPositionNormal morphVal)
        {
            PositionDelta = morphVal.Position - rootVal.Position;
            NormalDelta = morphVal.Normal - rootVal.Normal;
            TangentDelta = Vector3.Zero;
        }

        internal VertexGeometryDelta(VertexPositionNormalTangent rootVal, VertexPositionNormalTangent morphVal)
        {
            PositionDelta = morphVal.Position - rootVal.Position;
            NormalDelta = morphVal.Normal - rootVal.Normal;

            var dt = morphVal.Tangent - rootVal.Tangent;
            TangentDelta = new Vector3(dt.X, dt.Y, dt.Z);
        }

        internal VertexGeometryDelta(VertexGeometryDelta rootVal, VertexGeometryDelta morphVal)
        {
            PositionDelta = morphVal.PositionDelta - rootVal.PositionDelta;
            NormalDelta = morphVal.NormalDelta - rootVal.NormalDelta;
            TangentDelta = morphVal.TangentDelta - rootVal.TangentDelta;
        }

        #endregion

        #region data

        [VertexAttribute("POSITIONDELTA")]
        public Vector3 PositionDelta;

        [VertexAttribute("NORMALDELTA")]
        public Vector3 NormalDelta;

        [VertexAttribute("TANGENTDELTA")]
        public Vector3 TangentDelta;

        public override bool Equals(object obj) { return obj is VertexGeometryDelta other && AreEqual(this, other); }
        public bool Equals(VertexGeometryDelta other) { return AreEqual(this, other); }
        public static bool operator ==(in VertexGeometryDelta a, in VertexGeometryDelta b) { return AreEqual(a, b); }
        public static bool operator !=(in VertexGeometryDelta a, in VertexGeometryDelta b) { return !AreEqual(a, b); }
        public static bool AreEqual(in VertexGeometryDelta a, in VertexGeometryDelta b)
        {
            return a.PositionDelta == b.PositionDelta && a.NormalDelta == b.NormalDelta && a.TangentDelta == b.TangentDelta;
        }

        public override int GetHashCode() { return PositionDelta.GetHashCode(); }

        #endregion

        #region API

        void IVertexGeometry.SetPosition(Vector3 position) { this.PositionDelta = position; }

        void IVertexGeometry.SetNormal(Vector3 normal) { this.NormalDelta = normal; }

        void IVertexGeometry.SetTangent(Vector4 tangent) { this.TangentDelta = new Vector3(tangent.X, tangent.Y, tangent.Z); }

        public Vector3 GetPosition() { return this.PositionDelta; }

        public bool TryGetNormal(out Vector3 normal) { normal = this.NormalDelta; return true; }

        public bool TryGetTangent(out Vector4 tangent) { tangent = new Vector4(this.TangentDelta, 0); return true; }

        public void ApplyTransform(Matrix4x4 xform) { throw new NotSupportedException(); }

        public VertexGeometryDelta Subtract(IVertexGeometry baseValue)
        {
            return new VertexGeometryDelta((VertexGeometryDelta)baseValue, this);
        }

        public void Add(VertexGeometryDelta delta)
        {
            this.PositionDelta += delta.PositionDelta;
            this.NormalDelta += delta.NormalDelta;
            this.TangentDelta += delta.TangentDelta;
        }

        public void Validate() { }

        #endregion
    }
}
