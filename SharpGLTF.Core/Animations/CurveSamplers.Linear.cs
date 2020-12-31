﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SharpGLTF.Animations
{
    readonly struct SingleValueSampler<T> : ICurveSampler<T>, IConvertibleCurve<T>
    {
        #region lifecycle

        public static ICurveSampler<T> CreateForSingle(IEnumerable<(float Key, T Value)> sequence)
        {
            if (sequence.Skip(1).Any()) return null;

            return new SingleValueSampler<T>(sequence.First().Value);
        }

        public static ICurveSampler<T> CreateForSingle(IEnumerable<(float Key, (T, T, T) Value)> sequence)
        {
            if (sequence.Skip(1).Any()) return null;

            return new SingleValueSampler<T>(sequence.First().Value.Item2);
        }

        private SingleValueSampler(T value)
        {
            _Value = value;
        }

        #endregion

        #region data

        private readonly T _Value;

        #endregion

        #region API

        public int MaxDegree => 0;

        public T GetPoint(float offset) { return _Value; }

        public IReadOnlyDictionary<float, T> ToStepCurve()
        {
            return new Dictionary<float, T> { [0] = _Value };
        }

        public IReadOnlyDictionary<float, T> ToLinearCurve()
        {
            return new Dictionary<float, T> { [0] = _Value };
        }

        public IReadOnlyDictionary<float, (T TangentIn, T Value, T TangentOut)> ToSplineCurve()
        {
            return new Dictionary<float, (T TangentIn, T Value, T TangentOut)> { [0] = (default, _Value, default) };
        }

        #endregion
    }

    /// <summary>
    /// Defines a <see cref="Vector3"/> curve sampler that can be sampled with STEP or LINEAR interpolations.
    /// </summary>
    readonly struct Vector3LinearSampler : ICurveSampler<Vector3>, IConvertibleCurve<Vector3>
    {
        #region lifecycle

        public Vector3LinearSampler(IEnumerable<(float Key, Vector3 Value)> sequence, bool isLinear)
        {
            _Sequence = sequence;
            _Linear = isLinear;
        }

        #endregion

        #region data

        private readonly IEnumerable<(float Key, Vector3 Value)> _Sequence;
        private readonly Boolean _Linear;

        #endregion

        #region API

        public int MaxDegree => _Linear ? 1 : 0;

        public Vector3 GetPoint(float offset)
        {
            var (valA, valB, amount) = CurveSampler.FindRangeContainingOffset(_Sequence, offset);

            if (!_Linear) return valA;

            return Vector3.Lerp(valA, valB, amount);
        }

        public IReadOnlyDictionary<float, Vector3> ToStepCurve()
        {
            Guard.IsFalse(_Linear, nameof(MaxDegree), CurveSampler.StepCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, Vector3> ToLinearCurve()
        {
            Guard.IsTrue(_Linear, nameof(MaxDegree), CurveSampler.LinearCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, (Vector3 TangentIn, Vector3 Value, Vector3 TangentOut)> ToSplineCurve()
        {
            throw new NotSupportedException(CurveSampler.CurveError(MaxDegree));
        }

        public ICurveSampler<Vector3> ToFastSampler()
        {
            var linear = _Linear;
            return FastCurveSampler<Vector3>.CreateFrom(_Sequence, chunk => new Vector3LinearSampler(chunk, linear)) ?? this;
        }

        #endregion
    }

    /// <summary>
    /// Defines a <see cref="Quaternion"/> curve sampler that can be sampled with STEP or LINEAR interpolations.
    /// </summary>
    readonly struct QuaternionLinearSampler : ICurveSampler<Quaternion>, IConvertibleCurve<Quaternion>
    {
        #region lifecycle

        public QuaternionLinearSampler(IEnumerable<(float, Quaternion)> sequence, bool isLinear)
        {
            _Sequence = sequence;
            _Linear = isLinear;
        }

        #endregion

        #region data

        private readonly IEnumerable<(float Key, Quaternion Value)> _Sequence;
        private readonly Boolean _Linear;

        #endregion

        #region API

        public int MaxDegree => _Linear ? 1 : 0;

        public Quaternion GetPoint(float offset)
        {
            var (valA, valB, amount) = CurveSampler.FindRangeContainingOffset(_Sequence, offset);

            if (!_Linear) return valA;

            return Quaternion.Slerp(valA, valB, amount);
        }

        public IReadOnlyDictionary<float, Quaternion> ToStepCurve()
        {
            Guard.IsFalse(_Linear, nameof(MaxDegree), CurveSampler.StepCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, Quaternion> ToLinearCurve()
        {
            Guard.IsTrue(_Linear, nameof(MaxDegree), CurveSampler.LinearCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, (Quaternion TangentIn, Quaternion Value, Quaternion TangentOut)> ToSplineCurve()
        {
            throw new NotSupportedException(CurveSampler.CurveError(MaxDegree));
        }

        public ICurveSampler<Quaternion> ToFastSampler()
        {
            var linear = _Linear;
            return FastCurveSampler<Quaternion>.CreateFrom(_Sequence, chunk => new QuaternionLinearSampler(chunk, linear)) ?? this;
        }

        #endregion
    }

    /// <summary>
    /// Defines a <see cref="Transforms.SparseWeight8"/> curve sampler that can be sampled with STEP or LINEAR interpolation.
    /// </summary>
    readonly struct SparseLinearSampler : ICurveSampler<Transforms.SparseWeight8>, IConvertibleCurve<Transforms.SparseWeight8>
    {
        #region lifecycle

        public SparseLinearSampler(IEnumerable<(float Key, Transforms.SparseWeight8 Value)> sequence, bool isLinear)
        {
            _Sequence = sequence;
            _Linear = isLinear;
        }

        #endregion

        #region data

        private readonly IEnumerable<(float Key, Transforms.SparseWeight8 Value)> _Sequence;
        private readonly Boolean _Linear;

        #endregion

        #region API

        public int MaxDegree => _Linear ? 1 : 0;

        public Transforms.SparseWeight8 GetPoint(float offset)
        {
            var (valA, valB, amount) = CurveSampler.FindRangeContainingOffset(_Sequence, offset);

            if (!_Linear) return valA;

            var weights = Transforms.SparseWeight8.InterpolateLinear(valA, valB, amount);

            return weights;
        }

        public IReadOnlyDictionary<float, Transforms.SparseWeight8> ToStepCurve()
        {
            throw new NotSupportedException(CurveSampler.CurveError(MaxDegree));
        }

        public IReadOnlyDictionary<float, Transforms.SparseWeight8> ToLinearCurve()
        {
            Guard.IsTrue(_Linear, nameof(MaxDegree), CurveSampler.LinearCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, (Transforms.SparseWeight8 TangentIn, Transforms.SparseWeight8 Value, Transforms.SparseWeight8 TangentOut)> ToSplineCurve()
        {
            throw new NotSupportedException(CurveSampler.CurveError(MaxDegree));
        }

        public ICurveSampler<Transforms.SparseWeight8> ToFastSampler()
        {
            var linear = _Linear;
            return FastCurveSampler<Transforms.SparseWeight8>.CreateFrom(_Sequence, chunk => new SparseLinearSampler(chunk, linear)) ?? this;
        }

        #endregion
    }

    /// <summary>
    /// Defines a <see cref="float"/>[] curve sampler that can be sampled with STEP or LINEAR interpolations.
    /// </summary>
    readonly struct ArrayLinearSampler : ICurveSampler<float[]>, IConvertibleCurve<float[]>
    {
        #region lifecycle

        public ArrayLinearSampler(IEnumerable<(float, float[])> sequence, bool isLinear)
        {
            _Sequence = sequence;
            _Linear = isLinear;
        }

        #endregion

        #region data

        private readonly IEnumerable<(float Key, float[] Value)> _Sequence;
        private readonly Boolean _Linear;

        #endregion

        #region API

        public int MaxDegree => _Linear ? 1 : 0;

        public float[] GetPoint(float offset)
        {
            var (valA, valB, amount) = CurveSampler.FindRangeContainingOffset(_Sequence, offset);

            if (!_Linear) return valA;

            return CurveSampler.InterpolateLinear(valA, valB, amount);
        }

        public IReadOnlyDictionary<float, float[]> ToStepCurve()
        {
            Guard.IsFalse(_Linear, nameof(MaxDegree), CurveSampler.StepCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, float[]> ToLinearCurve()
        {
            Guard.IsTrue(_Linear, nameof(MaxDegree), CurveSampler.LinearCurveError);
            return _Sequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<float, (float[] TangentIn, float[] Value, float[] TangentOut)> ToSplineCurve()
        {
            throw new NotSupportedException(CurveSampler.CurveError(MaxDegree));
        }

        public ICurveSampler<float[]> ToFastSampler()
        {
            var linear = _Linear;
            return FastCurveSampler<float[]>.CreateFrom(_Sequence, chunk => new ArrayLinearSampler(chunk, linear)) ?? this;
        }

        #endregion
    }
}
