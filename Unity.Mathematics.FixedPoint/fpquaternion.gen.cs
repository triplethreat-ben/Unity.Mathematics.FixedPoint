using System;
using System.Runtime.CompilerServices;
using static Unity.Mathematics.math;
using static Unity.Mathematics.FixedPoint.fpmath;

#pragma warning disable 0660, 0661

namespace Unity.Mathematics.FixedPoint
{
    [System.Serializable]
    public partial struct fpquaternion : IEquatable<fpquaternion>, IFormattable
    {
        public fp4 value;

        public static readonly fpquaternion identity = new fpquaternion(0, 0, 0, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fpquaternion(fp x, fp y, fp z, fp w) { value.x = x; value.y = y; value.z = z; value.w = w; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fpquaternion(fp4 value) => this.value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator fpquaternion(fp4 v) => new fpquaternion(v);

        public fpquaternion(fp3x3 m)
        {
            fp3 u = m.c0;
            fp3 v = m.c1;
            fp3 w = m.c2;

            uint u_sign = (asuint(u.x) & 0x80000000);
            fp t = v.y + asfp(asuint(w.z) ^ u_sign);
            uint4 u_mask = uint4((int)u_sign >> 31);
            uint4 t_mask = uint4((int)t >> 31);

            fp tr = ((fp) 1f) + fpmath.abs(u.x);

            uint4 sign_flips = math.uint4(0x00000000, 0x80000000, 0x80000000, 0x80000000) ^
                               (u_mask & math.uint4(0x00000000, 0x80000000, 0x00000000, 0x80000000)) ^
                               (t_mask & math.uint4(0x80000000, 0x80000000, 0x80000000, 0x00000000));

            value = fp4(tr, u.y, w.x, v.z) + asfp(fpmath.asuint(new fp4(t, v.x, u.z, w.y)) ^ sign_flips); // +---, +++-, ++-+, +-++

            value = asfp((fpmath.asuint(value) & ~u_mask) | (fpmath.asuint(value.zwxy) & u_mask));
            value = asfp((fpmath.asuint(value.wzyx) & ~t_mask) | (fpmath.asuint(value) & t_mask));
            value = fpmath.normalize(value);
        }

        public fpquaternion(fp4x4 m)
        {
            fp4 u = m.c0;
            fp4 v = m.c1;
            fp4 w = m.c2;

            uint u_sign = (asuint(u.x) & 0x80000000);
            fp t = v.y + asfp(asuint(w.z) ^ u_sign);
            uint4 u_mask = uint4((int)u_sign >> 31);
            uint4 t_mask = uint4((int)t >> 31);

            fp tr = ((fp) 1f) + abs(u.x);

            uint4 sign_flips = math.uint4(0x00000000, 0x80000000, 0x80000000, 0x80000000) ^
                               (u_mask & math.uint4(0x00000000, 0x80000000, 0x00000000, 0x80000000)) ^
                               (t_mask & math.uint4(0x80000000, 0x80000000, 0x80000000, 0x00000000));

            value = fp4(tr, u.y, w.x, v.z) + asfp(asuint(new fp4(t, v.x, u.z, w.y)) ^ sign_flips); // +---, +++-, ++-+, +-++

            value = asfp((asuint(value) & ~u_mask) | (asuint(value.zwxy) & u_mask));
            value = asfp((asuint(value.wzyx) & ~t_mask) | (asuint(value) & t_mask));
            value = fpmath.normalize(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion AxisAngle(fp3 axis, fp angle)
        {
            fp sina, cosa;
            fpmath.sincos((fp)0.5f * angle, out sina, out cosa);

            return fpmath.fpquaternion(fpmath.fp4(axis*sina, cosa));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerXYZ(fp3 xyz)
        {
            // return mul(rotateZ(xyz.z), mul(rotateY(xyz.y), rotateX(xyz.x)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpquaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fpmath.fp4(c.xyz, s.x) * fpmath.fp4(-1, 1, -1, 1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerXZY(fp3 xyz)
        {
            // return mul(rotateY(xyz.y), mul(rotateZ(xyz.z), rotateX(xyz.x)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpmath.fpquaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, 1, -1, -1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerYXZ(fp3 xyz)
        {
            // return mul(rotateZ(xyz.z), mul(rotateX(xyz.x), rotateY(xyz.y)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpquaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, 1, 1, -1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerYZX(fp3 xyz)
        {
            // return mul(rotateX(xyz.x), mul(rotateZ(xyz.z), rotateY(xyz.y)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpquaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, -1, 1, 1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerZXY(fp3 xyz)
        {
            // return mul(rotateY(xyz.y), mul(rotateX(xyz.x), rotateZ(xyz.z)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpquaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, -1, 1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerZYX(fp3 xyz)
        {
            // return mul(rotateX(xyz.x), mul(rotateY(xyz.y), rotateZ(xyz.z)));
            fp3 s, c;
            sincos((fp)0.5f * xyz, out s, out c);
            return fpquaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.x * s.z
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, 1, -1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerXYZ(fp x, fp y, fp z) => EulerXYZ(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerXZY(fp x, fp y, fp z) => EulerXZY(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerYXZ(fp x, fp y, fp z) => EulerYXZ(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerYZX(fp x, fp y, fp z) => EulerYZX(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerZXY(fp x, fp y, fp z) => EulerZXY(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion EulerZYX(fp x, fp y, fp z) => EulerZYX(fp3(x, y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion Euler(fp3 xyz, RotationOrder order = RotationOrder.ZXY)
        {
            switch (order)
            {
                case RotationOrder.XYZ:
                    return EulerXYZ(xyz);
                case RotationOrder.XZY:
                    return EulerXZY(xyz);
                case RotationOrder.YXZ:
                    return EulerYXZ(xyz);
                case RotationOrder.YZX:
                    return EulerYZX(xyz);
                case RotationOrder.ZXY:
                    return EulerZXY(xyz);
                case RotationOrder.ZYX:
                    return EulerZYX(xyz);
                default:
                    return fpquaternion.identity;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion Euler(fp x, fp y, fp z, RotationOrder order = RotationOrder.Default) => Euler(fp3(x, y, z), order);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion RotateX(fp angle)
        {
            fp sina, cosa;
            sincos((fp)0.5f * angle, out sina, out cosa);
            return fpquaternion(sina, 0, 0, cosa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion RotateY(fp angle)
        {
            fp sina, cosa;
            sincos((fp)0.5f * angle, out sina, out cosa);
            return fpquaternion(0, sina, 0, cosa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion RotateZ(fp angle)
        {
            fp sina, cosa;
            sincos((fp)0.5f * angle, out sina, out cosa);
            return fpquaternion(0, 0, sina, cosa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion LookRotation(fp3 forward, fp3 up)
        {
            fp3 t = normalize(cross(up, forward));
            return fpquaternion(fp3x3(t, fpmath.cross(forward, t), forward));
        }

        [System.Obsolete("Not supported yet", true)]
        public static fpquaternion LookRotationSafe(fp3 forward, fp3 up)
        {
            throw new NotImplementedException("Not implemented");
            fp forwardLengthSq = dot(forward, forward);
            fp upLengthSq = dot(up, up);

            forward *= rsqrt(forwardLengthSq);
            up *= rsqrt(upLengthSq);

            fp3 t = cross(up, forward);
            fp tLengthSq = dot(t, t);
            t *= rsqrt(tLengthSq);

            fp mn = min(min(forwardLengthSq, upLengthSq), tLengthSq);
            fp mx = max(max(forwardLengthSq, upLengthSq), tLengthSq);

            bool accept = mn > fp.min_value && mx < fp.max_value && isfinite(forwardLengthSq) && isfinite(upLengthSq) && isfinite(tLengthSq);
            return fpquaternion(select(fp4(0, 0, 0, 1), fpquaternion(fp3x3(t, cross(forward, t),forward)).value, accept));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(fpquaternion x) => value.x == x.value.x && value.y == x.value.y && value.z == x.value.z && value.w == x.value.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) { return x is fpquaternion converted && Equals(converted); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => (int)fpmath.hash(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) => $"fpquaternion({value.x.ToString(format, formatProvider)}, {value.y.ToString(format, formatProvider)}, {value.z.ToString(format, formatProvider)}, {value.w.ToString(format, formatProvider)})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => string.Format("quaternion({0}, {1}, {2}, {3})", value.x, value.y, value.z, value.w);

    }

    public static partial class fpmath
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion fpquaternion(fp x, fp y, fp z, fp w) => new fpquaternion(x, y, z, w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion fpquaternion(fp4 value) { return new fpquaternion(value); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion fpquaternion(fp3x3 m) => new fpquaternion(m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion fpquaternion(fp4x4 m) => new fpquaternion(m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion conjugate(fpquaternion q) => fpquaternion(q.value * fp4(-1, -1, -1, 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion inverse(fpquaternion q)
        {
            fp4 x = q.value;
            return fpquaternion(rcp(dot(x, x)) * x * fp4(-1, -1, -1, 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fpquaternion a, fpquaternion b) => dot(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp length(fpquaternion q) => sqrt(dot(q.value, q.value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lengthsq(fpquaternion q) => dot(q.value, q.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion normalize(fpquaternion q)
        {
            fp4 x = q.value;
            return fpquaternion(rsqrt(dot(x, x)) * x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion normalizesafe(fpquaternion q)
        {
            fp4 x = q.value;
            fp len = dot(x, x);
            return fpquaternion(select(Mathematics.FixedPoint.fpquaternion.identity.value, x * rsqrt(len), len > fp.min_value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion normalizesafe(fpquaternion q, fpquaternion defaultvalue)
        {
            fp4 x = q.value;
            fp len = dot(x, x);
            return fpquaternion(select(defaultvalue.value, x * rsqrt(len), len > fp.min_value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion unitexp(fpquaternion q)
        {
            fp v_rcp_len = rsqrt(dot(q.value.xyz, q.value.xyz));
            fp v_len = rcp(v_rcp_len);
            fp sin_v_len, cos_v_len;
            sincos(v_len, out sin_v_len, out cos_v_len);
            return fpquaternion(fp4(q.value.xyz * v_rcp_len * sin_v_len, cos_v_len));
        }

        // [System.Obsolete(NOT_SUPPORTED_YET, true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion exp(fpquaternion q)
        {
            // throw new System.NotImplementedException("fp doesn't support tanh");
            fp v_rcp_len = rsqrt(dot(q.value.xyz, q.value.xyz));
            fp v_len = rcp(v_rcp_len);
            fp sin_v_len, cos_v_len;
            sincos(v_len, out sin_v_len, out cos_v_len);
            return fpquaternion(fp4(q.value.xyz * v_rcp_len * sin_v_len, cos_v_len) * exp(q.value.w));
        }

        // [System.Obsolete(NOT_SUPPORTED_YET, true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion unitlog(fpquaternion q)
        {
            // throw new System.NotImplementedException("fp doesn't support tanh");
            fp w = clamp(q.value.w, -1, 1);
            fp s = acos(w) * rsqrt(1 - w*w);
            return fpquaternion(fp4(q.value.xyz * s, 0));
        }

        // [System.Obsolete(NOT_SUPPORTED_YET, true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion log(fpquaternion q)
        {
            // throw new System.NotImplementedException("fp doesn't support tanh");
            fp v_len_sq = dot(q.value.xyz, q.value.xyz);
            fp q_len_sq = v_len_sq + q.value.w*q.value.w;

            fp s = acos(clamp(q.value.w * rsqrt(q_len_sq), -1, 1)) * rsqrt(v_len_sq);
            return fpquaternion(fp4(q.value.xyz * s, (fp)0.5f * log(q_len_sq)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion mul(fpquaternion a, fpquaternion b) => fpquaternion(a.value.wwww * b.value + (a.value.xyzx * b.value.wwwx + a.value.yzxy * b.value.zxyy) * fp4(1, 1, 1, -1) - a.value.zxyz * b.value.yzxz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 mul(fpquaternion q, fp3 v)
        {
            fp3 t = 2 * cross(q.value.xyz, v);
            return v + q.value.w * t + cross(q.value.xyz, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 rotate(fpquaternion q, fp3 v)
        {
            fp3 t = 2 * cross(q.value.xyz, v);
            return v + q.value.w * t + cross(q.value.xyz, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion nlerp(fpquaternion q1, fpquaternion q2, fp t)
        {
            fp dt = dot(q1, q2);
            if(dt < 0)
            {
                q2.value = -q2.value;
            }

            return normalize(fpquaternion(lerp(q1.value, q2.value, t)));
        }

        // [System.Obsolete(NOT_SUPPORTED_YET, true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpquaternion slerp(fpquaternion q1, fpquaternion q2, fp t)
        {
            // throw new System.NotImplementedException("fp doesn't support tanh");
            fp dt = dot(q1, q2);
            if (dt < 0)
            {
                dt = -dt;
                q2.value = -q2.value;
            }

            if (dt < (fp)0.9995f)
            {
                fp angle = acos(dt);
                fp s = rsqrt(1 - dt * dt);    // 1.0f / sin(angle)
                fp w1 = sin(angle * (1 - t)) * s;
                fp w2 = sin(angle * t) * s;
                return fpquaternion(q1.value * w1 + q2.value * w2);
            }
            else
            {
                // if the angle is small, use linear interpolation
                return nlerp(q1, q2, t);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(fpquaternion q) => hash(q.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 hashwide(fpquaternion q) => hashwide(q.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 forward(fpquaternion q) => mul(q, fp3(0, 0, 1));
    }
}