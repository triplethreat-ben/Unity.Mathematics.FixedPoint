using NUnit.Framework;
using static Unity.Mathematics.FixedPoint.fpmath;

namespace Unity.Mathematics.FixedPoint.Tests
{
    [TestFixture]
    public class TestFpQuaternion
    {
        static internal readonly fp3 test_angles = radians(fp3(-50, 28, 39));

        static internal readonly fp3x3 test3x3_xyz = new fp3x3( (fp)0.686179155968f, (fp)(-0.684009078513f), (fp)(-0.247567660300f),
            (fp)0.555656924414f,  (fp)0.273213475262f,  (fp)0.785238676636f,
            (fp)(-0.469471562786f), (fp)(-0.676377097075f),  (fp)0.567547772692f);

        static internal readonly fp3x3 test3x3_xzy = new fp3x3( (fp)0.686179155968f, (fp)(-0.716805468125f), (fp)(-0.123887395569f),
            (fp)0.629320391050f,  (fp)0.499539794942f,  (fp)0.595328345266f,
            (fp)(-0.364847929038f), (fp)(-0.486466765705f),  (fp)0.793874092373f);

        static internal readonly fp3x3 test3x3_yxz = new fp3x3( (fp)0.912505475649f, (fp)(-0.404519349890f), (fp)(-0.0608099701904f),
            (fp)0.276167195792f,  (fp)0.499539794942f,  (fp)0.8210917568930f,
            (fp)(-0.301770503659f), (fp)(-0.766044443119f),  (fp)0.5675477726920f);

        static internal readonly fp3x3 test3x3_yzx = new fp3x3( (fp)0.68617915596800f, (fp)(-0.629320391050f), (fp)0.364847929038f,
            (fp)(-0.00246669562435f),  (fp)0.499539794942f, (fp)0.866287428445f,
            (fp)(-0.72742840288700f), (fp)(-0.595328345266f), (fp)0.341221453011f);

        static internal readonly fp3x3 test3x3_zxy = new fp3x3( (fp)0.459852836288f, (fp)(-0.835146653037f), (fp)0.301770503659f,
            (fp)0.404519349890f,  (fp)0.499539794942f, (fp)0.766044443119f,
            (fp)(-0.790505828266f), (fp)(-0.230195701935f), (fp)0.567547772692f);

        static internal readonly fp3x3 test3x3_zyx = new fp3x3( (fp)0.686179155968f, (fp)(-0.555656924414f), (fp)0.469471562786f,
            (fp)0.125029621267f,  (fp)0.725866114623f, (fp)0.676377097075f,
            (fp)(-0.716607116711f), (fp)(-0.405418013897f), (fp)0.567547772692f);


        static internal readonly fp4x4 test4x4_xyz = new fp4x4(test3x3_xyz, new fp3(0, 0, 0));
        static internal readonly fp4x4 test4x4_xzy = new fp4x4(test3x3_xzy, new fp3(0, 0, 0));
        static internal readonly fp4x4 test4x4_yxz = new fp4x4(test3x3_yxz, new fp3(0, 0, 0));
        static internal readonly fp4x4 test4x4_yzx = new fp4x4(test3x3_yzx, new fp3(0, 0, 0));
        static internal readonly fp4x4 test4x4_zxy = new fp4x4(test3x3_zxy, new fp3(0, 0, 0));
        static internal readonly fp4x4 test4x4_zyx = new fp4x4(test3x3_zyx, new fp3(0, 0, 0));


        [Test]
        public static void quaternion_basic_constructors()
        {
            fpquaternion q = fpquaternion(1, 2, 3, 4);
            fpquaternion q2 = fpquaternion(fp4(1, 2, 3, 4));

            TestUtils.AreEqual(q.value.x, 1);
            TestUtils.AreEqual(q.value.y, 2);
            TestUtils.AreEqual(q.value.z, 3);
            TestUtils.AreEqual(q.value.w, 4);
            TestUtils.AreEqual(q2.value.x, 1);
            TestUtils.AreEqual(q2.value.y, 2);
            TestUtils.AreEqual(q2.value.z, 3);
            TestUtils.AreEqual(q2.value.w, 4);
        }
        //
        [Test]
        public static void quaternion_construct_from_matrix()
        {
            TestUtils.AreEqual(test3x3_xyz, new fp3x3(fpquaternion(test3x3_xyz)), (fp)0.0001f);
            TestUtils.AreEqual(test4x4_xyz, new fp4x4(fpquaternion(test4x4_xyz), fp3.zero), (fp)0.0001f);

            // Make sure to hit all 4 cases
            fp3x3 m0 = fp3x3.AxisAngle(normalize(fp3(1, 2, 3)), 1);
            fp3x3 m1 = fp3x3.AxisAngle(normalize(fp3(3, 2, 1)), 3);
            fp3x3 m2 = fp3x3.AxisAngle(normalize(fp3(1, 3, 2)), 3);
            fp3x3 m3 = fp3x3.AxisAngle(normalize(fp3(1, 2, 3)), 3);
            fpquaternion q0 = fpquaternion(m0);
            fpquaternion q1 = fpquaternion(m1);
            fpquaternion q2 = fpquaternion(m2);
            fpquaternion q3 = fpquaternion(m3);
            TestUtils.AreEqual(q0, fpquaternion((fp)0.1281319f, (fp)0.2562638f, (fp)0.3843956f, (fp)0.8775827f), (fp)0.0001f);
            TestUtils.AreEqual(q1, fpquaternion((fp)0.7997754f, (fp)0.5331835f, (fp)0.2665918f, (fp)0.0707372f), (fp)0.0001f);
            TestUtils.AreEqual(q2, fpquaternion((fp)0.2665918f, (fp)0.7997754f, (fp)0.5331835f, (fp)0.0707372f), (fp)0.0001f);
            TestUtils.AreEqual(q3, fpquaternion((fp)0.2665918f, (fp)0.5331835f, (fp)0.7997754f, (fp)0.0707372f), (fp)0.0001f);
        }
        //
        //
        // [Test]
        // public static void quaternion_construct_from_matrix3x3_torture()
        // {
        //     Random rnd = new Random(0x12345678);
        //     for(int i = 0; i < 1000; i++)
        //     {
        //         fp3x3 r = fp3x3(rnd.NextQuaternionRotation());
        //         fpquaternion q = fpquaternion(r);
        //         fp3x3 t = fp3x3(q);
        //         TestUtils.AreEqual(t, r, 0.001f);
        //     }
        // }
        //
        // [Test]
        // public static void quaternion_construct_from_matrix4x4_torture()
        // {
        //     Random rnd = new Random(0x12345678);
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         float4x4 r = float4x4(rnd.NextQuaternionRotation(), fp3.zero);
        //         fpquaternion q = fpquaternion(r);
        //         float4x4 t = float4x4(q, fp3.zero);
        //         TestUtils.AreEqual(t, r, 0.001f);
        //     }
        // }
        //
        [Test]
        public static void quaternion_axis_angle()
        {
            fpquaternion q = fpquaternion.AxisAngle(normalize(fp3(1, 2, 3)), 10);

            fpquaternion r = fpquaternion((fp)(-0.2562833f), (fp)(-0.5125666f), (fp)(-0.76885f), (fp)0.2836622f);
            TestUtils.AreEqual(q, r, (fp)0.0001f);
        }
        //
        [Test]
        public static void quaternion_axis_angle_consistency()
        {
            TestUtils.AreEqual(fpquaternion.AxisAngle(fp3(1, 0, 0), (fp)1.0f), fpquaternion.RotateX(1), (fp)0.001f);
            TestUtils.AreEqual(fpquaternion.AxisAngle(fp3(0, 1, 0), (fp)1.0f), fpquaternion.RotateY(1), (fp)0.001f);
            TestUtils.AreEqual(fpquaternion.AxisAngle(fp3(0, 0, 1), (fp)1.0f), fpquaternion.RotateZ(1), (fp)0.001f);
        }
        //
        [Test]
        public static void quaternion_euler()
        {
            fpquaternion q0 = fpquaternion.Euler(test_angles);
            fpquaternion q0_xyz = fpquaternion.Euler(test_angles, math.RotationOrder.XYZ);
            fpquaternion q0_xzy = fpquaternion.Euler(test_angles, math.RotationOrder.XZY);
            fpquaternion q0_yxz = fpquaternion.Euler(test_angles, math.RotationOrder.YXZ);
            fpquaternion q0_yzx = fpquaternion.Euler(test_angles, math.RotationOrder.YZX);
            fpquaternion q0_zxy = fpquaternion.Euler(test_angles, math.RotationOrder.ZXY);
            fpquaternion q0_zyx = fpquaternion.Euler(test_angles, math.RotationOrder.ZYX);

            fpquaternion q1 = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z);
            fpquaternion q1_xyz = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.XYZ);
            fpquaternion q1_xzy = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.XZY);
            fpquaternion q1_yxz = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.YXZ);
            fpquaternion q1_yzx = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.YZX);
            fpquaternion q1_zxy = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.ZXY);
            fpquaternion q1_zyx = fpquaternion.Euler(test_angles.x, test_angles.y, test_angles.z, math.RotationOrder.ZYX);

            fp epsilon = (fp)0.0001f;
            TestUtils.AreEqual(q0, fpquaternion((fp)(-0.3133549f), (fp)0.3435619f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q0_xyz, fpquaternion((fp)(-0.4597331f), (fp)0.06979711f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q0_xzy, fpquaternion((fp)(-0.3133549f), (fp)0.06979711f, (fp)0.3899215f, (fp)0.8630749f), epsilon);
            TestUtils.AreEqual(q0_yxz, fpquaternion((fp)(-0.4597331f), (fp)0.06979711f, (fp)0.1971690f, (fp)0.8630748f), epsilon);
            TestUtils.AreEqual(q0_yzx, fpquaternion((fp)(-0.4597331f), (fp)0.34356190f, (fp)0.1971690f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q0_zxy, fpquaternion((fp)(-0.3133549f), (fp)0.34356190f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q0_zyx, fpquaternion((fp)(-0.3133549f), (fp)0.34356190f, (fp)0.1971690f, (fp)0.8630749f), epsilon);

            TestUtils.AreEqual(q1, fpquaternion((fp)(-0.3133549f), (fp)0.3435619f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q1_xyz, fpquaternion((fp)(-0.4597331f), (fp)0.06979711f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q1_xzy, fpquaternion((fp)(-0.3133549f), (fp)0.06979711f, (fp)0.3899215f, (fp)0.8630749f), epsilon);
            TestUtils.AreEqual(q1_yxz, fpquaternion((fp)(-0.4597331f), (fp)0.06979711f, (fp)0.1971690f, (fp)0.8630748f), epsilon);
            TestUtils.AreEqual(q1_yzx, fpquaternion((fp)(-0.4597331f), (fp)0.34356190f, (fp)0.1971690f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q1_zxy, fpquaternion((fp)(-0.3133549f), (fp)0.34356190f, (fp)0.3899215f, (fp)0.7948176f), epsilon);
            TestUtils.AreEqual(q1_zyx, fpquaternion((fp)(-0.3133549f), (fp)0.34356190f, (fp)0.1971690f, (fp)0.8630749f), epsilon);

            fp3x3 m0 = new fp3x3(q0);
            fp3x3 m0_xyz = new fp3x3(q0_xyz);
            fp3x3 m0_xzy = new fp3x3(q0_xzy);
            fp3x3 m0_yxz = new fp3x3(q0_yxz);
            fp3x3 m0_yzx = new fp3x3(q0_yzx);
            fp3x3 m0_zxy = new fp3x3(q0_zxy);
            fp3x3 m0_zyx = new fp3x3(q0_zyx);

            fp3x3 m1 = new fp3x3(q1);
            fp3x3 m1_xyz = new fp3x3(q1_xyz);
            fp3x3 m1_xzy = new fp3x3(q1_xzy);
            fp3x3 m1_yxz = new fp3x3(q1_yxz);
            fp3x3 m1_yzx = new fp3x3(q1_yzx);
            fp3x3 m1_zxy = new fp3x3(q1_zxy);
            fp3x3 m1_zyx = new fp3x3(q1_zyx);

            TestUtils.AreEqual(m0, test3x3_zxy, epsilon);
            TestUtils.AreEqual(m0_xyz, test3x3_xyz, epsilon);
            TestUtils.AreEqual(m0_yzx, test3x3_yzx, epsilon);
            TestUtils.AreEqual(m0_zxy, test3x3_zxy, epsilon);
            TestUtils.AreEqual(m0_xzy, test3x3_xzy, epsilon);
            TestUtils.AreEqual(m0_yxz, test3x3_yxz, epsilon);
            TestUtils.AreEqual(m0_zyx, test3x3_zyx, (fp)0.0001f);

            TestUtils.AreEqual(m1, test3x3_zxy, epsilon);
            TestUtils.AreEqual(m1_xyz, test3x3_xyz, epsilon);
            TestUtils.AreEqual(m1_yzx, test3x3_yzx, epsilon);
            TestUtils.AreEqual(m1_zxy, test3x3_zxy, epsilon);
            TestUtils.AreEqual(m1_xzy, test3x3_xzy, epsilon);
            TestUtils.AreEqual(m1_yxz, test3x3_yxz, epsilon);
            TestUtils.AreEqual(m1_zyx, test3x3_zyx, epsilon);
        }
        //
        [Test]
        public static void quaternion_rotateX()
        {
            fp angle = (fp)2.3f;
            fpquaternion q = fpquaternion.RotateX(angle);

            fpquaternion r = fpquaternion((fp)0.91276394f, (fp)0.0f, (fp)0.0f, (fp)0.40848744f);
            TestUtils.AreEqual(q, r, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_rotateY()
        {
            fp angle = (fp)2.3f;
            fpquaternion q = fpquaternion.RotateY(angle);

            fpquaternion r = fpquaternion((fp)0.0f, (fp)0.91276394f, (fp)0.0f, (fp)0.40848744f);
            TestUtils.AreEqual(q, r, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_rotateZ()
        {
            fp angle = (fp)2.3f;
            fpquaternion q = fpquaternion.RotateZ(angle);

            fpquaternion r = fpquaternion((fp)0.0f, (fp)0.0f, (fp)0.91276394f, (fp)0.40848744f);
            TestUtils.AreEqual(q, r, (fp)0.0001f);
        }
        //
        static internal readonly fpquaternion test_q0 = new fpquaternion((fp)0.3270836f, (fp)0.8449658f, (fp)(-0.1090279f), (fp)0.4088545f);
        static internal readonly fpquaternion test_q1 = new fpquaternion((fp)(-0.05623216f), (fp)0.731018f, (fp)(-0.6747859f), (fp)(-0.08434824f));
        static internal readonly fpquaternion test_q2 = new fpquaternion((fp)(-0.2316205f), (fp)(-0.6022133f), (fp)(-0.7411857f), (fp)(-0.1852964f));
        static internal readonly fpquaternion test_q3 = new fpquaternion((fp)0.3619499f, (fp)0.8352691f, (fp)(-0.1392115f), (fp)0.3897922f);
        //
        [Test]
        public static void quaternion_conjugate()
        {
            fpquaternion q = fpquaternion(1, -2, 3, -4);
            fpquaternion cq = conjugate(q);
            fpquaternion r = fpquaternion(-1, 2, -3, -4);

            TestUtils.AreEqual(cq, r);
        }

        [Test]
        public static void quaternion_inverse()
        {
            fpquaternion q = fpquaternion(1, -2, 3, -4);
            fpquaternion iq = inverse(q);
            fpquaternion qiq = mul(iq, q);

            TestUtils.AreEqual(qiq, fpquaternion.identity, (fp)0.00001f);
        }

        [Test]
        public static void quaternion_dot()
        {
            fp dot01 = dot(test_q0, test_q1);
            fp dot02 = dot(test_q0, test_q2);

            TestUtils.AreEqual(dot01,  (fp)0.6383769f, (fp)0.00001f);
            TestUtils.AreEqual(dot02, (fp)(-0.5795583f), (fp)0.00001f);
        }
        //
        [Test]
        public static void quaternion_nlerp()
        {
            fpquaternion r0 = nlerp(test_q0, test_q1, (fp)0.3f);
            fpquaternion r1 = nlerp(test_q0, test_q1, (fp)(-4.3f));
            fpquaternion r2 = nlerp(test_q0, test_q1, (fp)5.1f);

            TestUtils.AreEqual(r0, fpquaternion((fp)0.2302977f, (fp)0.8803911f, (fp)(-0.3026878f), (fp)0.2832927f), (fp)0.0001f);
            TestUtils.AreEqual(r1, fpquaternion((fp)0.4724294f, (fp)0.3192692f, (fp)0.5557517f, (fp)0.604994f), (fp)0.0001f);
            TestUtils.AreEqual(r2, fpquaternion((fp)(-0.4054004f), (fp)0.06570576f, (fp)(-0.7457358f), (fp)(-0.5246059f)), (fp)0.0001f);
        }

        [Test]
        public static void quaternion_slerp()
        {
            fpquaternion r0 = slerp(test_q0, test_q1, (fp)0.3f);
            fpquaternion r1 = slerp(test_q0, test_q1, (fp)(-4.3f));
            fpquaternion r2 = slerp(test_q0, test_q1, (fp)5.1f);

            TestUtils.AreEqual(r0, fpquaternion((fp)0.2261014f, (fp)0.8806396f, (fp)(-0.3100654f), (fp)0.2778693f), (fp)0.0001f);
            TestUtils.AreEqual(r1, fpquaternion((fp)(-0.4676181f), (fp)(-0.5321988f), (fp)(-0.3789966f), (fp)(-0.5953646f)), (fp)0.0001f);
            TestUtils.AreEqual(r2, fpquaternion((fp)0.2596942f, (fp)(-0.4369303f), (fp)0.7902023f, (fp)0.34239f), (fp)0.0001f);
        }
        //
        [Test]
        public static void quaternion_mul_vector()
        {
            fp3x3 m = test3x3_xyz;
            fpquaternion q = fpquaternion(m);

            fp3 vector = fp3((fp)1.1f, (fp)(-2.2f), (fp)3.5f);

            fp3 mvector = mul(m, vector);
            fp3 qvector = mul(q, vector);

            TestUtils.AreEqual(qvector, mvector, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_log_exp_identity()
        {
            fpquaternion q = fpquaternion((fp)1.2f, (fp)(-2.6f), (fp)3.1f, (fp)6.0f);
            fpquaternion log_q = log(q);
            fpquaternion exp_log_q = exp(log_q);
            TestUtils.AreEqual(exp_log_q, q, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_log_exp_rotation()
        {
            fpquaternion q = fpquaternion(test3x3_xyz);
            fpquaternion q3 = mul(q, mul(q, q));
            fpquaternion log_q = log(q);
            fpquaternion t = exp(fpquaternion(log_q.value * (fp)3.0f));
            TestUtils.AreEqual(t, q3, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_unitlog_unitexp_rotation()
        {
            fpquaternion q = fpquaternion(test3x3_xyz);
            fpquaternion q3 = mul(q, mul(q, q));
            fpquaternion log_q = unitlog(q);
            fpquaternion t = unitexp(fpquaternion(log_q.value * (fp)3.0f));
            TestUtils.AreEqual(t, q3, (fp)0.0001f);
        }

        [Test]
        public static void quaternion_look_rotation()
        {
            // Exercise the 4 cases
            fp3 forward0 = normalize(fp3((fp)1.0f, (fp)2.0f, (fp)3.0f));
            fp3 up0 = fp3((fp)0.0f, (fp)1.0f, (fp)0.0f);
            fpquaternion q0 = fpquaternion.LookRotation(forward0, up0);
            TestUtils.AreEqual(q0, fpquaternion((fp)(-0.274657f), (fp)0.153857f, (fp)0.044571f, (fp)0.948106f), (fp)0.001f);
            TestUtils.AreEqual(new fp3x3(q0), fp3x3.LookRotation(forward0, up0), (fp)0.001f);

            fp3 forward1 = normalize(fp3((fp)(-3.2f), (fp)2.3f, (fp)(-1.3f)));
            fp3 up1 = normalize(fp3((fp)1.0f, (fp)(-3.2f), (fp)(-1.5f)));
            fpquaternion q1 = fpquaternion.LookRotation(forward1, up1);
            TestUtils.AreEqual(q1, fpquaternion((fp)0.805418f, (fp)0.089103f, (fp)(-0.435327f), (fp)(-0.392240f)), (fp)0.001f);
            TestUtils.AreEqual(new fp3x3(q1), fp3x3.LookRotation(forward1, up1), (fp)0.001f);

            fp3 forward2 = normalize(fp3((fp)(-2.6f), (fp)(-5.2f), (fp)(-1.1f)));
            fp3 up2 = normalize(fp3((fp)(-4.2f), (fp)(-1.2f), (fp)(-4.5f)));
            fpquaternion q2 = fpquaternion.LookRotation(forward2, up2);
            TestUtils.AreEqual(q2, fpquaternion((fp)(-0.088343f), (fp)0.764951f, (fp)(-0.534144f), (fp)(-0.348907f)), (fp)0.001f);
            TestUtils.AreEqual(new fp3x3(q2), fp3x3.LookRotation(forward2, up2), (fp)0.001f);

            fp3 forward3 = normalize(fp3((fp)1.3f, (fp)2.1f, (fp)3.4f));
            fp3 up3 = normalize(fp3((fp)0.2f, (fp)(-1.0f), (fp)0.3f));
            fpquaternion q3 = fpquaternion.LookRotation(forward3, up3);
            TestUtils.AreEqual(q3, fpquaternion((fp)0.184984f, (fp)0.247484f, (fp)0.947425f, (fp)(-0.083173f)), (fp)0.001f);
            TestUtils.AreEqual(new fp3x3(q3), fp3x3.LookRotation(forward3, up3), (fp)0.001f);
        }
        //
        [Test]
        [Ignore("Not finished")]
        public static void quaternion_look_rotation_safe()
        {
            // fp3 forward0 = fp3((fp)(-3.2f), (fp)2.3f, (fp)(-1.3f)) * fp.min_value;
            // fp3 up0 = fp3((fp)1.0f, (fp)(-3.2f), (fp)(-1.5f)) * fp.max_value;
            // fpquaternion q0 = fpquaternion.LookRotationSafe(forward0, up0);
            // TestUtils.AreEqual(q0, fpquaternion((fp)0.805418f, (fp)0.089103f, (fp)(-0.435327f), (fp)(-0.392240f)), (fp)0.001f);
            //
            // fp3 forward1 = fp3((fp)(-3.2f), (fp)2.3f, (fp)(-1.3f)) * fp.min_value;
            // fp3 up1 = fp3((fp)1.0f, (fp)(-3.2f), (fp)(-1.5f));
            // fpquaternion q1 = fpquaternion.LookRotationSafe(forward1, up1);
            // TestUtils.AreEqual(q1, fpquaternion.identity, (fp)0.001f);
            //
            // fp3 forward2 = fp3((fp)(-3.2f), (fp)2.3f, (fp)(-1.3f));
            // fp3 up2 = forward2;
            // fpquaternion q2 = fpquaternion.LookRotationSafe(forward2, up2);
            // TestUtils.AreEqual(q2, fpquaternion.identity, (fp)0.001f);
        }
    }
}