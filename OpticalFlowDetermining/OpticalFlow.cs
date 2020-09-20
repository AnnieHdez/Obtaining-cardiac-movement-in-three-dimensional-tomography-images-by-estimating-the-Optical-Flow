using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Filters;
using System.Threading.Tasks;

namespace OpticalFlowDetermining
{
    public interface IOpticalFlowProcess3D
    {
        float3[,,] GetFlow(byte[,,] map1, byte[,,] map2);
    }

    public static class MatrixX
    {
        public static float[,] Transpose (this float[,] mat)
        {
            int width = mat.GetLength(1);
            int height = mat.GetLength(0);

            var t = new float[width, height];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    t[j, i] = mat[i, j];
            return t;
        }

        public static float[,] Mul (this float[,] mat, float alpha)
        {
            int width = mat.GetLength(1);
            int height = mat.GetLength(0);

            var t = new float[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    t[i, j] = mat[i, j] * alpha;
            return t;
        }

        public static float[] Mul(this float[] vec, float alpha)
        {
            int size = vec.Length;

            var r = new float[size];

            for (int i = 0; i < size; i++)
                r[i] = vec[i] * alpha;
            return r;
        }



        public static float[,] getI(this int size)
        {
            float[,] mat = new float[size, size];
            for (int i = 0; i < size; i++)
                mat[i, i] = 1;
            return mat;
        }

        public static float[,] Subtract (this float[,] m1, float[,] m2)
        {
            var rows = m1.GetLength(0);
            var cols = m1.GetLength(1);
            float[,] r = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = m1[i, j] - m2[i, j];
            return r;
        }

        public static float[,] Add(this float[,] m1, float[,] m2)
        {
            var rows = m1.GetLength(0);
            var cols = m1.GetLength(1);
            float[,] r = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = m1[i, j] + m2[i, j];
            return r;
        }

        public static float[,] Mul (this float[,] mat1, float[,] mat2)
        {
            float[,] result = new float[mat1.GetLength(0), mat2.GetLength(1)];
            int n = result.GetLength(0);
            int m = result.GetLength(1);
            int l = mat1.GetLength(1);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < l; k++)
                        result[i, j] += mat1[i, k] * mat2[k, j];

            return result;
        }

        public static float[] Mul(this float[,] mat1, float[] mat2)
        {
            float[] result = new float[3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i] += mat1[i, j] * mat2[j];

            return result;
        }

        public static float Determinant2 (this float[,] mat)
        {
            if (mat.GetLength(0) != 2 || mat.GetLength(1) != 2)
                throw new ArgumentException();

            return mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
        }

        public static float Determinant3 (this float[,] m)
        {
            // computes the inverse of a matrix m
            return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) -
                         m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                         m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
        }

        public static float[,] Inverse2 (this float[,] mat)
        {
            if (mat.GetLength(0) != 2 || mat.GetLength(1) != 2)
                throw new ArgumentException();

            var det = mat.Determinant2();
            if (det == 0)
                return new float[,]
                {
                    { 1, 0 },
                    { 0, 1 }
                };

            return new float[,] { 
                { mat[1, 1], -mat[0, 1] }, 
                { -mat[1, 0], mat[0, 0] } }
            .Mul(1 / det);
        }

        public static float[,] Inverse3(this float[,] m)
        {
            if (m.GetLength(0) != 3 || m.GetLength(1) != 3)
                throw new ArgumentException();

            // computes the inverse of a matrix m
            float det = m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) -
                         m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                         m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

            if (det == 0)
                return new float[,]
                {
                    { 1, 0, 0},
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                };

            float invdet = 1 / det;

            float[,] minv= new float[3,3]; // inverse of matrix m
            minv[0, 0] = (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) * invdet;
            minv[0, 1] = (m[0, 2] * m[2, 1] - m[0, 1] * m[2, 2]) * invdet;
            minv[0, 2] = (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * invdet;
            minv[1, 0] = (m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2]) * invdet;
            minv[1, 1] = (m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) * invdet;
            minv[1, 2] = (m[1, 0] * m[0, 2] - m[0, 0] * m[1, 2]) * invdet;
            minv[2, 0] = (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]) * invdet;
            minv[2, 1] = (m[2, 0] * m[0, 1] - m[0, 0] * m[2, 1]) * invdet;
            minv[2, 2] = (m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1]) * invdet;

            return minv;
        }

        public static float sample(this float[,] mat, float row, float col)
        {
            row = Math.Max(0.5f, Math.Min(mat.GetLength(0) - 0.5001f, row)) - 0.5f;
            col = Math.Max(0.5f, Math.Min(mat.GetLength(1) - 0.5001f, col)) - 0.5f;

            var u = col % 1;
            var v = row % 1;

            return
                    mat[(int)row, (int)col] * (1 - u) * (1 - v) +
                    mat[(int)(row + 1), (int)col] * (1 - u) * v +
                    mat[(int)row, (int)(col + 1)] * u * (1 - v) +
                    mat[(int)(row + 1), (int)(col + 1)] * u * v;
        }

        public static float2 sample(this float2[,] mat, float row, float col)
        {
            row = Math.Max(0.5f, Math.Min(mat.GetLength(0) - 0.5001f, row)) - 0.5f;
            col = Math.Max(0.5f, Math.Min(mat.GetLength(1) - 0.5001f, col)) - 0.5f;

            var u = col % 1;
            var v = row % 1;

            return
                    mat[(int)row, (int)col] * (1 - u) * (1 - v) +
                    mat[(int)(row + 1), (int)col] * (1 - u) * v +
                    mat[(int)row, (int)(col + 1)] * u * (1 - v) +
                    mat[(int)(row + 1), (int)(col + 1)] * u * v;
        }

        public static float sample(this byte[,,] mat, int depth, int row, int col)
        {
            depth = Math.Max(0, Math.Min(mat.GetLength(0) - 1, depth));
            row = Math.Max(0, Math.Min(mat.GetLength(1) - 1, row));
            col = Math.Max(0, Math.Min(mat.GetLength(2) - 1, col));

            return mat[depth, row, col];
        }

        public static float sample(this byte[,,] mat, float depth1, float row1, float col1)
        {
           float depth = Math.Max(0.5f, Math.Min(mat.GetLength(0) - 0.5001f, depth1)) - 0.5f;
           float row = Math.Max(0.5f, Math.Min(mat.GetLength(1) - 0.5001f, row1)) - 0.5f;
           float col = Math.Max(0.5f, Math.Min(mat.GetLength(2) - 0.5001f, col1)) - 0.5f;

            var u = col % 1;
            var v = row % 1;
            var w = depth % 1;

            return
                    mat[(int)depth, (int)row, (int)col] * (1 - w) * (1 - v) * (1 - u) +
                    mat[(int)depth, (int)row, (int)col + 1] * (1 - w) * (1 - v) * (u) +
                    mat[(int)depth, (int)row + 1, (int)col] * (1 - w) * (v) * (1 - u) +
                    mat[(int)depth, (int)row + 1, (int)col + 1] * (1 - w) * (v) * (u) +

                    mat[(int)depth + 1, (int)row, (int)col] * (w) * (1 - v) * (1 - u) +
                    mat[(int)depth + 1, (int)row, (int)col + 1] * (w) * (1 - v) * (u) +
                    mat[(int)depth + 1, (int)row + 1, (int)col] * (w) * (v) * (1 - u) +
                    mat[(int)depth + 1, (int)row + 1, (int)col + 1] * (w) * (v) * (u); 
        }

        public static float3 sample(this float3[,,] mat, float3 pos)
        {
            return sample(mat, pos.z, pos.y, pos.x);
        }

        public static float3 sample(this float3[,,] mat, float depth, float row, float col)
        {
            depth = Math.Max(0.5f, Math.Min(mat.GetLength(0) - 0.5001f, depth)) - 0.5f;
            row = Math.Max(0.5f, Math.Min(mat.GetLength(1) - 0.5001f, row)) - 0.5f;
            col = Math.Max(0.5f, Math.Min(mat.GetLength(2) - 0.5001f, col)) - 0.5f;

            var u = col % 1;
            var v = row % 1;
            var w = depth % 1;

            return
                    mat[(int)depth, (int)row, (int)col] * (1 - w) * (1 - v) * (1 - u) +
                    mat[(int)depth, (int)row, (int)col + 1] * (1 - w) * (1 - v) * (u) +
                    mat[(int)depth, (int)row + 1, (int)col] * (1 - w) * (v) * (1 - u) +
                    mat[(int)depth, (int)row + 1, (int)col + 1] * (1 - w) * (v) * (u) +

                    mat[(int)depth + 1, (int)row, (int)col] * (w) * (1 - v) * (1 - u) +
                    mat[(int)depth + 1, (int)row, (int)col + 1] * (w) * (1 - v) * (u) +
                    mat[(int)depth + 1, (int)row + 1, (int)col] * (w) * (v) * (1 - u) +
                    mat[(int)depth + 1, (int)row + 1, (int)col + 1] * (w) * (v) * (u);
        }

        public static T[] forAll<T> (this int N, Func<int, T> p)
        {
            T[] result = new T[N];
            for (int i = 0; i < N; i++)
                result[i] = p(i);
            return result;
        }

        public static float[,] ToMatrix (this float2[] data)
        {
            int size = data.Length;
            float[,] result = new float[size, 2];
            for (int i=0; i<size;i++)
            {
                result[i, 0] = data[i].x;
                result[i, 1] = data[i].y;
            }
            return result;
        }

        public static float[,] ToMatrix(this float3[] data)
        {
            int size = data.Length;
            float[,] result = new float[size, 3];
            for (int i = 0; i < size; i++)
            {
                result[i, 0] = data[i].x;
                result[i, 1] = data[i].y;
                result[i, 2] = data[i].z;
            }
            return result;
        }

        public static float[,] ToMatrix(this float[] data)
        {
            int size = data.Length;
            float[,] result = new float[size, 1];
            for (int i = 0; i < size; i++)
            {
                result[i, 0] = data[i];
            }
            return result;
        }

        public static T[,] ForAll<T>(this T[,] mat, Func<int, int, T> p)
        {
            int height = mat.GetLength(0);
            int width = mat.GetLength(1);

            T[,] result = new T[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    result[i, j] = p(i, j);

            return result;
        }

        public static T[,,] ForAll<E,T>(this E[,,] mat, Func<int, int, int, T> p)
        {
            int slices = mat.GetLength(0);
            int height = mat.GetLength(1);
            int width = mat.GetLength(2);

            T[,,] result = new T[slices, height, width];

            for (int k =0; k< slices; k++)
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    result[k, i, j] = p(k, i, j);

            return result;
        }

        public static T[,] Diag <T>(this T[] data)
        {
            int size = data.Length;
            T[,] mat = new T[size, size];

            for (int i = 0; i < size; i++)
                mat[i, i] = data[i];

            return mat;
        }
    }

    public class PyrOpticalFlowProcess3D : IOpticalFlowProcess3D
    {
        // Gaussian coeficients for a 5x5 window
        float[] Coef = new float[] {
            1, 4, 7, 4, 1,
            4, 16, 26, 16, 4,
            7, 26, 41, 26, 7,
            4, 16, 26, 16, 4,
            1, 4, 7, 4, 1}.Mul(1 / 273f);

        float[] Coef3D;
        public PyrOpticalFlowProcess3D()
        {
            Coef3D = new float[25 * 5];
            int count = 0;
            for (int k = 0; k < 5; k++)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        Coef3D[count++] = Coef[i * 5 + j] * Coef[k * 5 + 2] / Coef[2 * 5 + 2];
            N = 2;
        }

        public int Radius { get; set; }

        public int N { get; set; }

        int vCols, vRows, vSlis;

        public float3[,,] GetFlow(byte[,,] map1, byte[,,] map2)
        {
            return GetPyrLKOF(map1, map2, N);
        }

        byte[,,] reduce(byte[,,] m)
        {
            byte[,,] r = new byte[m.GetLength(0) / 2, m.GetLength(1) / 2, m.GetLength(2) / 2];

            for (int i = 0; i < r.GetLength(0); i++)
                for (int j = 0; j < r.GetLength(1); j++)
                    for (int k = 0; k < r.GetLength(2); k++)
                        //r[i, j, k] =
                        //    0.25f * (m.sample(i * 2, j * 2, k * 2)) +
                        //    0.125f * (m.sample(i * 2, j * 2, k * 2 + 1) + m.sample(i * 2, j * 2, k * 2 - 1) + m.sample(i * 2, j * 2 + 1, k * 2) + m.sample(i * 2, j * 2 - 1, k * 2) + m.sample(i * 2 + 1, j * 2, k * 2) + m.sample(i * 2 - 1, j * 2, k * 2)) +
                        //    0.0625f * (m.sample(i * 2, j * 2 + 1, k * 2 + 1) + m.sample(i * 2, j * 2 + 1, k * 2 - 1) + m.sample(i * 2, j * 2 - 1, k * 2 - 1) + m.sample(i * 2, j * 2 - 1, k * 2 + 1) +
                        //    m.sample(i * 2 + 1, j * 2 + 1, k * 2) + m.sample(i * 2 + 1, j * 2 - 1, k * 2) + m.sample(i * 2 - 1, j * 2 + 1, k * 2) + m.sample(i * 2 - 1, j * 2 - 1, k * 2)
                        //    + m.sample(i * 2 + 1, j * 2, k * 2 + 1) + m.sample(i * 2 + 1, j * 2, k * 2 - 1) + m.sample(i * 2 - 1, j * 2, k * 2 + 1) + m.sample(i * 2 - 1, j * 2, k * 2 - 1)) +
                        //    0.0625f * 0.5f * (
                        //    m.sample(i * 2 + 1, j * 2 + 1, k * 2 + 1) + m.sample(i * 2 + 1, j * 2 + 1, k * 2 - 1) + m.sample(i * 2 + 1, j * 2 - 1, k * 2 - 1) + m.sample(i * 2 + 1, j * 2 - 1, k * 2 + 1) +
                        //    m.sample(i * 2 - 1, j * 2 + 1, k * 2 + 1) + m.sample(i * 2 - 1, j * 2 + 1, k * 2 - 1) + m.sample(i * 2 - 1, j * 2 - 1, k * 2 - 1) + m.sample(i * 2 - 1, j * 2 - 1, k * 2 + 1)
                        //    );
                        r[i, j, k] = (byte)((
                                        m[i * 2, j * 2, k * 2] + m[i * 2, j * 2, k * 2 + 1] + m[i * 2, j * 2 + 1, k * 2] + m[i * 2, j * 2 + 1, k * 2 + 1] +
                                        m[i * 2 + 1, j * 2, k * 2] + m[i * 2 + 1, j * 2, k * 2 + 1] + m[i * 2 + 1, j * 2 + 1, k * 2] + m[i * 2 + 1, j * 2 + 1, k * 2 + 1]
                                        ) / 8);


            //r[i, j, k] = 1 / 8 * m.sample(2 * i, 2 * j, 2 * k) +
            //    1 / 16 * (m.sample(2 * i + 1, 2 * j, 2 * k) + m.sample(2 * i - 1, 2 * j, 2 * k) + m.sample(2 * i, 2 * j + 1, 2 * k) + m.sample(2 * i, 2 * j - 1, 2 * k) + m.sample(2 * i, 2 * j, 2 * k + 1) + m.sample(2 * i, 2 * j, 2 * k - 1)) +
            //    1 / 32 * (m.sample(2 * i + 1, 2 * j + 1, 2 * k) + m.sample(2 * i - 1, 2 * j - 1, 2 * k) + m.sample(2 * i - 1, 2 * j + 1, 2 * k) + m.sample(2 * i + 1, 2 * j - 1, 2 * k) + m.sample(2 * i + 1, 2 * j, 2 * k + 1) + m.sample(2 * i + 1, 2 * j, 2 * k - 1) + m.sample(2 * i - 1, 2 * j, 2 * k + 1) + m.sample(2 * i - 1, 2 * j, 2 * k - 1) + m.sample(2 * i, 2 * j + 1, 2 * k + 1) + m.sample(2 * i, 2 * j + 1, 2 * k - 1) + m.sample(2 * i, 2 * j - 1, 2 * k + 1) + m.sample(2 * i - 1, 2 * j - 1, 2 * k - 1)) +
            //    1 / 64 * (m.sample(2 * i + 1, 2 * j + 1, 2 * k + 1) + m.sample(2 * i - 1, 2 * j - 1, 2 * k - 1) + m.sample(2 * i - 1, 2 * j + 1, 2 * k + 1) + m.sample(2 * i + 1, 2 * j - 1, 2 * k + 1) + m.sample(2 * i + 1, 2 * j + 1, 2 * k - 1) + m.sample(2 * i + 1, 2 * j - 1, 2 * k - 1) + m.sample(2 * i - 1, 2 * j - 1, 2 * k + 1) + m.sample(2 * i - 1, 2 * j + 1, 2 * k - 1));
            return r;
        }

        public float3[,,] GetPyrLKOF(byte[,,] map1, byte[,,] map2, int levels)
        {
            float3[,,] gKP1 = levels > 0 ? GetPyrLKOF(reduce(map1), reduce(map2), levels - 1) : new float3[map1.GetLength(0) / 2, map2.GetLength(1) / 2, map2.GetLength(2) / 2];

            float3[,,] g = new float3[map1.GetLength(0), map1.GetLength(1), map1.GetLength(2)];
            for (int i = 0; i < g.GetLength(0); i++)
                for (int j = 0; j < g.GetLength(1); j++)
                    for (int k = 0; k < g.GetLength(2); k++)
                    {
                        var v = gKP1.sample((i + 0.5f) / 2, (j + 0.5f) / 2, (k + 0.5f) / 2) * 2;
                        var l = float3.length(v);
                        if (l > (1 << levels))
                            v = v * ((1 << levels) / l);
                        g[i, j, k] = v;
                    }
            float3[,,] d = GetLevelFlow(map1, map2, g);

            return d;
        }

        private float3[,,] GetLevelFlow(byte[,,] map1, byte[,,] map2, float3[,,] g)
        {
            vSlis = map1.GetLength(0);
            vRows = map1.GetLength(1);
            vCols = map1.GetLength(2);

            float3[,,] result = new float3[vSlis, vRows, vCols];
            float[,,] Ix = new float[vSlis, vRows, vCols];
            float[,,] Iy = new float[vSlis, vRows, vCols];
            float[,,] Iz = new float[vSlis, vRows, vCols];
            float[,,] dI = new float[vSlis, vRows, vCols];


            for (int pz = 0; pz < vSlis; pz++)
                for (int py = 0; py < vRows; py++)
                    for (int px = 0; px < vCols; px++)
                    {
                        Ix[pz, py, px] = (map1[pz, py, Math.Min(vCols - 1, px + 1)] - map1[pz, py, Math.Max(0, px - 1)]) * 0.5f;
                        Iy[pz, py, px] = (map1[pz, Math.Min(vRows - 1, py + 1), px] - map1[pz, Math.Max(0, py - 1), px]) * 0.5f;
                        Iz[pz, py, px] = (map1[Math.Min(vSlis - 1, pz + 1), py, px] - map1[Math.Max(0, pz - 1), py, px]) * 0.5f;

                        var d = pz + 0.5f + g[pz, py, px].z;
                        var r = py + 0.5f + g[pz, py, px].y;
                        var c = px + 0.5f + g[pz, py, px].x;
                        dI[pz, py, px] = map2.sample(d, r, c) - map1[pz, py, px];
                    }

            Parallel.For(0, vSlis, z =>
            {
                //for (int z = 0; z < vSlis; z++)
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        result[z, y, x] = ComputeVectorFor(x, y, z, Ix, Iy, Iz, dI) + g[z, y, x];
                    }
            });

            return result;
        }


        private float3 ComputeVectorFor(int x, int y, int z, float[,,] Ix, float[,,] Iy, float[,,] Iz, float[,,] dI)
        {
            int i = 0;
            //float3 prevFlow = new float3(0, 0, 0);
            //float3 result = new float3(0, 0, 0);

            float[,] A = new float[125, 3];
            float[,] b = new float[125, 1];

            for (int zd = -2; zd <= 2; zd++)
                for (int yd = -2; yd <= 2; yd++)
                    for (int xd = -2; xd <= 2; xd++)
                    {
                        var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                        var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                        var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));

                        A[i, 0] = Coef3D[i] * Ix[zC, yC, xC];
                        A[i, 1] = Coef3D[i] * Iy[zC, yC, xC];
                        A[i, 2] = Coef3D[i] * Iz[zC, yC, xC];

                        b[i, 0] = -Coef3D[i] * dI[zC, yC, xC];

                        i++;
                    }

            var At = A.Transpose();

            var M = (At.Mul(A));

            var v = M.Inverse3().Mul(At).Mul(b);

            float3 e1, e2, e3;
            float L1, L2, L3;

            AnalyticalEigenSolver.EigenSol(M, out L1, out L2, out L3, out e1, out e2, out e3);
            //GetEigenVectors3(M, out L1, out L2, out L3, out e1, out e2, out e3);

            if (L2 > L1 || L3 > L1)
                throw new InvalidOperationException();


            if (L3 >= 1)
                return new float3(v[0, 0], v[1, 0], v[2, 0]);
            else
            {
                var dot = v[0, 0] * e1.x + v[1, 0] * e1.y + v[2, 0] * e1.z;
                return new float3(dot * e1.x, dot * e1.y, dot * e1.z);
            }
        }
    }

    public class NewPyrOpticalFlowProcess3D : IOpticalFlowProcess3D
    {
        // Gaussian coeficients for a 5x5 window
        float[] Coef = new float[] {
            1, 4, 7, 4, 1,
            4, 16, 26, 16, 4,
            7, 26, 41, 26, 7,
            4, 16, 26, 16, 4,
            1, 4, 7, 4, 1}.Mul(1 / 273f);

        float[,,] Coef3D;
        public NewPyrOpticalFlowProcess3D()
        {
            Coef3D = new float[5, 5, 5];
            
            for (int k = 0; k < 5; k++)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        Coef3D[k, i, j] = Coef[i * 5 + j] * Coef[k * 5 + 2] / Coef[2 * 5 + 2];
            N = 4;
        }

        public int N { get; set; }

        int vCols, vRows, vSlis;

        public float3[,,] GetFlow(byte[,,] map1, byte[,,] map2)
        {
            return GetPyrLKOF(map1, map2, N);
        }

        public float3[,,] GetPyrLKOF(byte[,,] map1, byte[,,] map2, int levels)
        {
            float3[,,] gKP1 = levels > 0 ? GetPyrLKOF(reduce(map1), reduce(map2), levels - 1) : new float3[map1.GetLength(0) / 2, map2.GetLength(1) / 2, map2.GetLength(2) / 2];

            float3[,,] g = new float3[map1.GetLength(0), map1.GetLength(1), map1.GetLength(2)];
            for (int i = 0; i < g.GetLength(0); i++)
                for (int j = 0; j < g.GetLength(1); j++)
                    for (int k = 0; k < g.GetLength(2); k++)
                    {
                        var v = gKP1.sample((i + 0.5f) / 2, (j + 0.5f) / 2, (k + 0.5f) / 2) * 2;
                        var l = float3.length(v);
                        if (l > (1 << levels))
                            v = v * ((1 << levels) / l);
                        g[i, j, k] = v;

                    }
            return GetLevelFlow(map1, map2, g);


        }

        private float3[,,] GetLevelFlow(byte[,,] map1, byte[,,] map2, float3[,,] g)
        {
            vSlis = map1.GetLength(0);
            vRows = map1.GetLength(1);
            vCols = map1.GetLength(2);

            float3[,,] result = new float3[vSlis, vRows, vCols];
            float[,,] Ix = new float[vSlis, vRows, vCols];
            float[,,] Iy = new float[vSlis, vRows, vCols];
            float[,,] Iz = new float[vSlis, vRows, vCols];
            float[,,] Ik = new float[vSlis, vRows, vCols];

            for (int pz = 0; pz < vSlis; pz++)
                for (int py = 0; py < vRows; py++)
                    for (int px = 0; px < vCols; px++)
                    {
                        Ix[pz, py, px] = (map1[pz, py, Math.Min(vCols - 1, px + 1)] - map1[pz, py, Math.Max(0, px - 1)]) * 0.5f;
                        Iy[pz, py, px] = (map1[pz, Math.Min(vRows - 1, py + 1), px] - map1[pz, Math.Max(0, py - 1), px]) * 0.5f;
                        Iz[pz, py, px] = (map1[Math.Min(vSlis - 1, pz + 1), py, px] - map1[Math.Max(0, pz - 1), py, px]) * 0.5f;
                        var d = pz + 0.5f + g[pz, py, px].z;
                        var r = py + 0.5f + g[pz, py, px].y;
                        var c = px + 0.5f + g[pz, py, px].x;
                        Ik[pz, py, px] = map2.sample(d, r, c) - map1[pz, py, px];
                    }
           
            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                       result[z, y, x] = ComputeVectorFor(x, y, z, Ix, Iy, Iz, Ik, map1, map2) + g[z, y, x];
            });
            return result;
        }

        private float3 ComputeVectorFor(int x, int y, int z, float[,,] Ix, float[,,] Iy, float[,,] Iz, float[,,] Ik, byte[,,] map1, byte[,,] map2)
        {
            float[,] M = new float[3, 3];
            float[] b = new float[3];

            for (int zd = -2; zd <= 2; zd++)
                for (int yd = -2; yd <= 2; yd++)
                    for (int xd = -2; xd <= 2; xd++)
                    {
                        var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                        var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                        var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));

                        M[0, 0] += Ix[zC, yC, xC] * Ix[zC, yC, xC]; 
                        M[0, 1] += Iy[zC, yC, xC] * Ix[zC, yC, xC];
                        M[0, 2] += Ix[zC, yC, xC] * Iz[zC, yC, xC];
                        M[1, 0] += Ix[zC, yC, xC] * Iy[zC, yC, xC];
                        M[1, 1] += Iy[zC, yC, xC] * Iy[zC, yC, xC];
                        M[1, 2] += Iy[zC, yC, xC] * Iz[zC, yC, xC];
                        M[2, 0] += Ix[zC, yC, xC] * Iz[zC, yC, xC];
                        M[2, 1] += Iy[zC, yC, xC] * Iz[zC, yC, xC];
                        M[2, 2] += Iz[zC, yC, xC] * Iz[zC, yC, xC];

                        b[0] += -Ik[zC, yC, xC] * Ix[zC, yC, xC];
                        b[1] += -Ik[zC, yC, xC] * Iy[zC, yC, xC];
                        b[2] += -Ik[zC, yC, xC] * Iz[zC, yC, xC];

                    }

            var v = M.Inverse3().Mul(b);
            var result = new float3(v[0], v[1], v[2]);

            float3 e1, e2, e3;
            float L1, L2, L3;

            AnalyticalEigenSolver.EigenSol(M, out L1, out L2, out L3, out e1, out e2, out e3);

            if (L2 > L1 || L3 > L1)
                throw new InvalidOperationException();


            if (L3 >= 1)
                return result;

            else
            {
                var dot = result.x * e1.x + result.y * e1.y + result.z * e1.z;
                return new float3(dot * e1.x, dot * e1.y, dot * e1.z);
            }

        }

        byte[,,] reduce(byte[,,] m)
        {
            byte[,,] r = new byte[m.GetLength(0) / 2, m.GetLength(1) / 2, m.GetLength(2) / 2];

            for (int i = 0; i < r.GetLength(0); i++)
                for (int j = 0; j < r.GetLength(1); j++)
                    for (int k = 0; k < r.GetLength(2); k++)
                        r[i, j, k] = (byte)Math.Min(255,
                                (m[i * 2, j * 2, k * 2] + m[i * 2, j * 2, k * 2 + 1] + m[i * 2, j * 2 + 1, k * 2] + m[i * 2, j * 2 + 1, k * 2 + 1] +
                                m[i * 2 + 1, j * 2, k * 2] + m[i * 2 + 1, j * 2, k * 2 + 1] + m[i * 2 + 1, j * 2 + 1, k * 2] + m[i * 2 + 1, j * 2 + 1, k * 2 + 1]
                                ) / 8);
            return r;
        }
    }

    public class CensusOpticalFlowProcess3D : IOpticalFlowProcess3D
    {
        public int K { get; set; }

        public int N  { get; set; }

        int vCols, vRows, vSlis;

        //bool first;

        //string[][,,] prevTrans = new string[5][,,];

        public CensusOpticalFlowProcess3D ()
        {
           // first = true;
            K = 6;
            N = 4;
        }

        byte[,,] reduce(byte[,,] m)
        {
            byte[,,] r = new byte[m.GetLength(0) / 2, m.GetLength(1) / 2, m.GetLength(2) / 2];
            for (int i = 0; i < r.GetLength(0); i++)
                for (int j = 0; j < r.GetLength(1); j++)
                    for (int k = 0; k < r.GetLength(2); k++)
                     r[i, j, k] = (byte)(
                            (m[i * 2, j * 2, k * 2] + m[i * 2, j * 2, k * 2 + 1] + m[i * 2, j * 2 + 1, k * 2] + m[i * 2, j * 2 + 1, k * 2 + 1] +
                            m[i * 2 + 1, j * 2, k * 2] + m[i * 2 + 1, j * 2, k * 2 + 1] + m[i * 2 + 1, j * 2 + 1, k * 2] + m[i * 2 + 1, j * 2 + 1, k * 2 + 1]
                            ) / 8);
            return r;
        }
        public float3[,,] GetFlow(byte[,,] map1, byte[,,] map2)
        {
            var flow = GetBlockFlow(map1, map2, N);
            //first = false;
            return flow;
        }
        public float3[,,] GetBlockFlow(byte[,,] map1, byte[,,] map2, int levels)
        {
            float3[,,] gBF = levels > 0 ? GetBlockFlow(reduce(map1), reduce(map2), levels - 1) : new float3[map1.GetLength(0) / 2, map2.GetLength(1) / 2, map2.GetLength(2) / 2];
            float3[,,] g = new float3[map1.GetLength(0), map1.GetLength(1), map1.GetLength(2)];
            for (int i = 0; i < g.GetLength(0); i++)
                for (int j = 0; j < g.GetLength(1); j++)
                    for (int k = 0; k < g.GetLength(2); k++)
                    {
                        var v = gBF.sample((i + 0.5f) / 2, (j + 0.5f) / 2, (k + 0.5f) / 2) * 2;
                        var l = float3.length(v);
                        if (l > (1 << levels))
                            v = v * ((1 << levels) / l);
                        g[i, j, k] = v;
                    }
            float3[,,] d = GetLevelFlow(map1, map2, g, levels);

            return d;
        }

        public float3[,,] GetLevelFlow(byte[,,] map1, byte[,,] map2, float3[,,] g, int Levels)
        {
            vSlis = map1.GetLength(0);
            vRows = map1.GetLength(1);
            vCols = map1.GetLength(2);
            string[,,] trans1;
            //if (first)
                trans1 = CensusTransform(map1);
           // else
               // trans1 = prevTrans[Levels];

            string[,,] trans2 = CensusTransform(map2);
           // prevTrans[Levels] = trans2;        

            float3[,,] result = new float3[vSlis, vRows, vCols];

             Parallel.For(0, vSlis, z =>
             {
           // for (int z = 0; z < vSlis; z++)
            for (int y = 0; y < vRows; y++)
                for (int x = 0; x < vCols; x++)
                {
                        var d = (int)(Math.Max(Math.Min(z + 0.5f + g[z, y, x].z, vSlis-1), 0));
                        var r = (int)(Math.Max(Math.Min(y + 0.5f + g[z, y, x].y, vRows-1), 0));
                        var c = (int)(Math.Max(Math.Min(x + 0.5f + g[z, y, x].x, vCols-1), 0));
                        result[z, y, x] = CensusGetFlow(trans1, trans2, x, y, z, c, r, d);
                }
            });

            return result;
        }

        public float3 CensusGetFlow(string[,,] map1, string[,,] map2, int x, int y, int z, int c, int r, int d)
        {
            string value = map1[z, y, x];
            string newValue = map2[d, r, c];
            if (value == newValue)
                return new float3(0, 0, 0);

            int min = HammingDistance(value, newValue);
            float3 minVect = new float3(0, 0, 0);
            float3 pos = new float3(c, r, d);

            Queue<float3> frontier = new Queue<float3>();
            frontier.Enqueue(pos);

            List<float3> visited = new List<float3>();

            while (frontier.Count != 0 && visited.Count < 7)
            {
                float3 curPos = frontier.Dequeue();
                if (!visited.Contains(curPos))
                    visited.Add(curPos);

                List<float3> succesors = new List<float3>{ new float3(curPos.x + 1, curPos.y, curPos.z) ,
                    new float3(curPos.x-1,curPos.y, curPos.z), new float3(curPos.x, curPos.y+1, curPos.z),
                    new float3(curPos.x, curPos.y-1, curPos.z), new float3(curPos.x, curPos.y, curPos.z +1),
                    new float3(curPos.x, curPos.y, curPos.z-1)};

                foreach (var newState in succesors)
                {
                    if (newState.x >= 0 && newState.y >= 0 && newState.z >= 0 && newState.x < map2.GetLength(2) && newState.y < map2.GetLength(1) && newState.z < map2.GetLength(0) && !visited.Contains(newState))
                    {
                        newValue = map2[(int)newState.z, (int)newState.y, (int)newState.x];

                        if (newValue == value)
                            return new float3(newState.x - x, newState.y - y, newState.z - z);

                        int dist = HammingDistance(value, newValue);

                        if (dist < min)
                        {
                            min = dist;
                            minVect = new float3(newState.x - x, newState.y - y, newState.z - z);
                        }
                        frontier.Enqueue(newState);
                    }
                }
            }

            return minVect;
        }
   
      
        public string[,,] CensusTransform(byte[,,] map)
        {
            string[,,] result = new string[vSlis, vRows, vCols];

            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        result[z, y, x] = Transform(map, x, y, z);
                    }
            }); 
            return result;
        }

        public string Transform(byte[,,] map, int x, int y, int z)
        {
            string result = "";
            for (int zd = -1; zd <= 1; zd++)
                for (int yd = -1; yd <= 1; yd++)
                    for (int xd = -1; xd <= 1; xd++)
                    {
                        if (xd != 0 || yd != 0 || zd != 0)
                        {
                            var xC = Math.Max(0, Math.Min(map.GetLength(2) - 1, x + xd));
                            var yC = Math.Max(0, Math.Min(map.GetLength(1) - 1, y + yd));
                            var zC = Math.Max(0, Math.Min(map.GetLength(0) - 1, z + zd));

                            int val = 1;

                            if (map[z, y, x] - map[zC, yC, xC] > K)
                                val = 0;

                            else if (map[zC, yC, xC] - map[z, y, x] > K)
                                val = 2;

                            result += val;
                        }


                    }
            return result;
        }

        public int HammingDistance(string a, string b)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    count++;
            return count;
        }
    }

    [Serializable]
    public struct float3
    {
        public float x, y, z;

        public float3 (float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float3 operator + (float3 v1, float3 v2)
        {
            return new float3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static float3 operator -(float3 v1, float3 v2)
        {
            return new float3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static float3 operator * (float3 v, float alpha)
        {
            return new float3(v.x * alpha, v.y * alpha, v.z * alpha);
        }

        public static bool operator == (float3 v1, float3 v2)
        {
            return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
        }

        public static bool operator !=(float3 v1, float3 v2)
        {
            return (v1.x != v2.x || v1.y != v2.y || v1.z != v2.z);
        }

        public override bool Equals(object v1)
        {
            return this==(float3)v1;
        }

        public override int GetHashCode()
        {
            return (int)(x + y + z);
        }

        public static float length (float3 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        public static float3 normalize(float3 v)
        {
            return v * (1 / length(v));
        }

        public static float3 cross(float3 a, float3 b)
        {
            return new float3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);
        }

        public static float dot(float3 a, float3 b)
        {
            return a.x*b.x+a.y*b.y +a.z*b.z;
        }
    }

    [Serializable]
    public struct float2
    {
        public float x, y;
        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float2 Normalized
        {
            get
            {
                float length = (float)Math.Sqrt(x * x + y * y);
                return new float2(x / length, y / length);
            }
        }

        public static float2 operator *(float2 v, float alpha) {
            return new float2(v.x * alpha, v.y * alpha);
        }

        public static float2 operator + (float2 v1, float2 v2)
        {
            return new float2(v1.x + v2.x, v1.y + v2.y);
        }

        public static float2 operator - (float2 v1, float2 v2)
        {
            return new float2(v1.x - v2.x, v1.y - v2.y);
        }

        public static float2 operator / (float2 v, float alpha)
        {
            return v * (1 / alpha);
        }

        public static float length(float2 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static float sqrLength (float2 v)
        {
            return v.x * v.x + v.y * v.y;
        }
    }
}
