using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace OpticalFlowDetermining
{
    class AnalyticalEigenSolver
    {
        public static void EigenSol(float[,] m, out float l1, out float l2, out float l3, out float3 e1, out float3 e2, out float3 e3)
        {
            //coefficients of the characteristic ecuation (x3+c2x2+c1x+c0)
            if (m[0, 0] == 0 && m[1, 0] == 0 && m[0, 1] == 0 && m[2, 0] == 0 && m[0, 2] == 0 && m[1, 2] == 0 && m[2, 1] == 0 && m[1, 1] == 0 && m[2, 2] == 0)
            {
                l1 = l2 = l3 = 0;
                e1 = e2 = e3 = new float3(0, 0, 0);
                return;
            }

            double c2 = -m[0, 0] - m[1, 1] - m[2, 2];
            double c1 = m[0, 0] * m[1, 1] + m[0, 0] * m[2, 2] + m[1, 1] * m[2, 2] - m[0, 1] * m[0, 1] - m[0, 2]
                        * m[0, 2] - m[1, 2] * m[1, 2];
            double c0 = m[0, 0] * m[1, 2] * m[1, 2] + m[1, 1] * m[0, 2] * m[0, 2] + m[2, 2] * m[0, 1] * m[0, 1]
                        - m[0, 0] * m[1, 1] * m[2, 2] - 2 * m[0, 2] * m[0, 1] * m[1, 2];

            double p = c2 * c2 - 3.0 * c1;
            double q = -27.0 / 2.0 * c0 - c2 * c2 * c2 + 9.0 / 2.0 * c2 * c1;

            if(q == 0 || p < 0)
            {
                l1 = 3;
                l2 = 2;
                l3 = 1;
                e1 = e2 = e3 = new float3(0, 0, 0);
                return;
            }
            Complex f = 1.0 / 3.0 * Complex.Atan(Complex.Sqrt(27.0 * (1.0 / 4.0 * c1 * c1 * (p - c1) + c0 * (q +
                        27.0 / 4.0 * c0))) / q);

            //Eigenvalues
            Complex v1 = Math.Sqrt(p) / 3.0 * (2.0 * Complex.Cos(f)) - 1.0 / 3.0 * c2;
            Complex v2 = Math.Sqrt(p) / 3.0 * (-Complex.Cos(f) - Math.Sqrt(3) * Complex.Sin(f)) - 1.0 / 3.0 * c2;
            Complex v3 = Math.Sqrt(p) / 3.0 * (-Complex.Cos(f) + Math.Sqrt(3) * Complex.Sin(f)) - 1.0 / 3.0 * c2;

            if (v1.Real >= v2.Real)
            {
                if (v2.Real >= v3.Real)
                {
                    l1 = (float)v1.Real;
                    l2 = (float)v2.Real;
                    l3 = (float)v3.Real;
                }

                else if (v3.Real >= v1.Real)
                {
                    l1 = (float)v3.Real;
                    l2 = (float)v1.Real;
                    l3 = (float)v2.Real;
                }

                else
                {
                    l1 = (float)v1.Real;
                    l2 = (float)v3.Real;
                    l3 = (float)v2.Real;
                }
            }

            else
            {
                if (v1.Real >= v3.Real)
                {
                    l1 = (float)v2.Real;
                    l2 = (float)v1.Real;
                    l3 = (float)v3.Real;
                }

                else if (v2.Real >= v3.Real)
                {
                    l1 = (float)v2.Real;
                    l2 = (float)v3.Real;
                    l3 = (float)v1.Real;
                }
                else
                {
                    l1 = (float)v3.Real;
                    l2 = (float)v2.Real;
                    l3 = (float)v1.Real;
                }
            }

            //Isn't a degenerate eigenvalue
            if (l1 != l2 && l2 != l3 && l1 != l3)
            {
                EigenvectorsComp(m, l1, out e1);
                EigenvectorsComp(m, l2, out e2);
                EigenvectorsComp(m, l3, out e3);
            }

            //The 3 eigenvalues are equal
            else if (l1 == l2 && l2 == l3)
            {
                EigenvectorsComp(m, l1, out e1);
                ComputeEig2(m, l1, e1, out e2);
                ComputeEig3(m, l1, e1, out e3);
            }

            //2 of the eigenvalues are equal
            else if (l1 == l2)
            {
                EigenvectorsComp(m, l1, out e1);
                ComputeEig2(m, l1, e1, out e2);
                EigenvectorsComp(m, l3, out e3);
            }

            //The other 2 eigenvalues are equal            
            else
            {
                EigenvectorsComp(m, l1, out e1);
                EigenvectorsComp(m, l2, out e2);
                ComputeEig3(m, l1, e1, out e3);
            }


            if (Math.Sqrt(e1.x * e1.x + e1.y * e1.y + e1.z * e1.z) != 0)
                e1 = float3.normalize(e1);
            if (Math.Sqrt(e2.x * e2.x + e2.y * e2.y + e2.z * e2.z) != 0)
                e2 = float3.normalize(e2);
            if (Math.Sqrt(e3.x * e3.x + e3.y * e3.y + e3.z * e3.z) != 0)
                e3 = float3.normalize(e3);
        }

        private static void EigenvectorsComp(float[,] m, double v, out float3 e)
        {
            //They are not linearly independent
            if (m[0, 1] != 0 && (m[1, 1] - v) != 0 && m[2, 1] != 0 &&
                (m[0, 0] - v) / m[0, 1] == m[1, 0] / (m[1, 1] - v) &&
                m[1, 0] / (m[1, 1] - v) == m[2, 0] / m[2, 1] &&
                (m[0, 0] - v) / m[0, 1] == m[2, 0] / m[2, 1])
            {
                double u = m[2, 0] / m[2, 1];
                e.x = (float)(1.0 / Math.Sqrt(1.0 + u * u));
                e.y = (float)(-u * (1.0 / Math.Sqrt(1 + u * u)));
                e.z = 0;
            }
            else
            {
                e.x = (float)(m[1, 0] * m[2, 1] - (m[1, 1] - v) * m[2, 0]);
                e.y = (float)(m[0, 1] * m[2, 0] - (m[0, 0] - v) * m[2, 1]);
                e.z = (float)((m[0, 0] - v) * (m[0, 0] - v) - m[0, 1] * m[1, 0]);
            }
        }

        private static void ComputeEig2(float[,] m, double v1, float3 e1, out float3 e2)
        {
            e2.x = e1.y * m[2, 0] - m[1, 0] * e1.z;
            e2.y = (float)((m[0, 0] - v1) * e1.z - e1.x * m[2, 0]);
            e2.z = (float)(e1.x * m[1, 0] - (m[0, 0] - v1) * e1.y);
        }

        private static void ComputeEig3(float[,] m, double v1, float3 e1, out float3 e3)
        {
            e3.x = (float)(e1.y * m[2, 1] - (m[1, 1] - v1) * e1.z);
            e3.y = m[0, 1] * e1.z - e1.x * m[2, 1];
            e3.z = (float)(e1.x * (m[1, 1] - v1) - m[0, 1] * e1.y);
        }
    }
}
