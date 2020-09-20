using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class Filter
    {
        static float[] Coef = new float[] {
            1, 4, 7, 4, 1,
            4, 16, 26, 16, 4,
            7, 26, 41, 26, 7,
            4, 16, 26, 16, 4,
            1, 4, 7, 4, 1};

        public static double[,,] GausCoef3D()
        {
            double[,,] Coef3D = new double[5, 5, 5];

            for (int k = 0; k < 5; k++)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        Coef3D[k, i, j] = (Coef[i * 5 + j] * Coef[k * 5 + 2] / Coef[2 * 5 + 2]) / 712.4633f;
            return Coef3D;
        }
    
        public static byte[,,] PrewittFilter(byte[,,] data)
        {
            int[,,] filterx = { { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } }, { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } }, { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } } };
            int[,,] filtery = { { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } }, { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } }, { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } } };
            int[,,] filterz = { { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } }, { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } }, { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } } };
            return ApplyFilter(data, filterx, filtery, filterz);
        }

        public static byte[,,] SobelFilter(byte[,,] data)
        {
            int[,,] filterx = { { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } }, { { -2, 0, 2 }, { -4, 0, 4 }, { -2, 0, 2 } }, { { -1, 0, 1 }, { -2, 0, 2 }, {- 1, 0, 1 } } };
            int[,,] filtery = { { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } }, { { -2, -4, -2 }, { 0, 0, 0 }, { 2, 4, 2 } }, { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } } };
            int[,,] filterz = { { { -1, -2, -1 }, { -2, -4, -2 }, { -1, -2, -1 } }, { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } }, { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } } };
            return ApplyFilter(data, filterx, filtery, filterz);
        }

        public static byte[,,] RobertsFilter(byte[,,] data)
        {
            int[,,] filterx = { { { 0, 1 }, { -1, 0} }, { { 0, 1 }, { -1, 0 } } };
            int[,,] filtery = { { { 1, 0 }, { 0, -1 } }, { { 1, 0 }, { 0, -1 } } };
            int[,,] filterz = { { { 1, 0 }, { 0, 1 } }, { { 0, -1 }, { -1, 0 } } };
            return ApplyFilter(data, filterx, filtery, filterz);
        }

        public static byte[,,] GaussianFilter(byte[,,] data, bool edge)
        {
            var Coef3D = GausCoef3D();
            return ApplyFilter(data, Coef3D, false, edge);
        }

        public static byte[,,] LoGFilter(byte[,,] data)
        {
            double[,,] Coef3D = GausCoef3D();
            double[,,] filter = { { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } }, { { 0, 1, 0 }, { 1, -6, 1 }, { 0, 1, 0 } }, { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } } };
            double[,,] Logfilter = ApplyFilter(Coef3D, filter);
            return ApplyFilter(data, Logfilter, true, false);
        }

        public static byte[,,] MedianFilter(byte[,,] data, bool edge)
        {
            int size = 6;
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);
            byte[,,] newData = new byte[vSlis, vRows, vCols];
            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        List<byte> val = new List<byte>(size * size * size);
                        for (int zd = -(size / 2); zd <= size / 2; zd++)
                            for (int yd = -(size / 2); yd <= size / 2; yd++)
                                for (int xd = -(size / 2); xd <= size / 2; xd++)
                                {
                                    var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                                    var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                                    var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));

                                    val.Add(data[zC, yC, xC]);
                                }
                        val.Sort();

                        newData[z, y, x] = Math.Min((byte)255, val[(size * size * size) / 2]);

                        if (edge)
                            newData[z, y, x] = (byte)(Math.Min(255, data[z, y, x]+Math.Max(0, 2*(data[z, y, x] - newData[z, y, x]))));
                    }
            });
            return newData;
        }

        public static byte[,,] MeanFilter(byte[,,] data, bool edge)
        {
            double[,,] filter = new double[5, 5, 5];

            for (int k = 0; k < 5; k++)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        filter[k, i, j] = 1/125f;

            return ApplyFilter(data, filter, false, edge);
        }

        public static double[,,] ApplyFilter(double[,,] data, double[,,] filter)
        {
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);
            int vSlisF = filter.GetLength(0);
            int vRowsF = filter.GetLength(1);
            int vColsF = filter.GetLength(2);
            double[,,] newData = new double[vSlis, vRows, vCols];
            double[,,] tempData = new double[vSlis, vRows, vCols];
            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        for (int zd = -(vSlisF / 2); zd <= vSlisF / 2; zd++)
                            for (int yd = -(vRowsF / 2); yd <= vRowsF / 2; yd++)
                                for (int xd = -(vColsF / 2); xd <= vColsF / 2; xd++)
                                {
                                    var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                                    var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                                    var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));

                                    tempData[z, y, x] += filter[(vSlisF / 2) + zd, (vRowsF / 2) + yd, (vColsF / 2) + xd] * data[zC, yC, xC];
                                }
                        newData[z, y, x] = tempData[z, y, x];
                    }
            });
            return newData;
        }


        public static byte[,,] ApplyFilter(byte[,,] data, double[,,] filter, bool log, bool edge)
        {
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);
            int vSlisF = filter.GetLength(0);
            int vRowsF = filter.GetLength(1);
            int vColsF = filter.GetLength(2);
            byte[,,] newData = new byte[vSlis, vRows, vCols];
            double[,,] tempData = new double[vSlis, vRows, vCols];
            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        for (int zd = -(vSlisF / 2); zd <= vSlisF / 2; zd++)
                            for (int yd = -(vRowsF / 2); yd <= vRowsF / 2; yd++)
                                for (int xd = -(vColsF / 2); xd <= vColsF / 2; xd++)
                                {
                                    var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                                    var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                                    var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));

                                    tempData[z, y, x] += filter[(vSlisF / 2) + zd, (vRowsF / 2) + yd, (vColsF / 2) + xd] * data[zC, yC, xC];
                                }

                        if (log)
                            newData[z, y, x] = (byte)(Math.Min(255, data[z, y, x] + 0.5 * tempData[z, y, x]));
                        else
                            newData[z, y, x] = (byte)(Math.Min(255, tempData[z, y, x]));
                        if(edge)
                            newData[z, y, x] = (byte)(Math.Min(255, data[z, y, x]+ Math.Max(0,4 *(data[z, y, x] - newData[z, y, x]))));
                    }
            });
            return newData;
        }

        public static byte[,,] ApplyFilter(byte[,,] data, int[,,] filterx, int[,,] filtery, int[,,] filterz)
        {
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);
            int vSlisF = filterx.GetLength(0);
            int vRowsF = filterx.GetLength(1);
            int vColsF = filterx.GetLength(2);
            byte[,,] newData = new byte[vSlis, vRows, vCols];
            float[,,] tempDatax = new float[vSlis, vRows, vCols];
            float[,,] tempDatay = new float[vSlis, vRows, vCols];
            float[,,] tempDataz = new float[vSlis, vRows, vCols];

            int addz = 0;
            if(vSlisF%2==0)
                addz += 1;

            int addy = 0;
            if (vRowsF % 2 == 0)
                addy += 1;

            int addx = 0;
            if (vColsF % 2 == 0)
                addx += 1;

            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        for (int zd = -(vSlisF / 2) +addz; zd <= vSlisF / 2; zd++)
                            for (int yd = -(vRowsF / 2) + addy; yd <= vRowsF / 2; yd++)
                                for (int xd = -(vColsF / 2) + addx; xd <= vColsF/ 2; xd++)
                                {
                                    var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                                    var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                                    var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));
                                    var posz = (vSlisF / 2) -addz + zd;
                                    var posy = (vRowsF / 2) - addy + yd;
                                    var posx = (vColsF / 2) -addx + xd;
                                    var value = data[zC, yC, xC];

                                    tempDatax[z, y, x] += filterx[posz, posy, posx] * value;
                                    tempDatay[z, y, x] += filtery[posz, posy, posx] * value;
                                    tempDataz[z, y, x] += filterz[posz, posy, posx] * value;
                                }
                        newData[z, y, x] = (byte)(Math.Min(255, Math.Sqrt(Math.Pow(tempDatax[z, y, x], 2)+ Math.Pow(tempDatay[z, y, x],2) + Math.Pow(tempDataz[z, y, x], 2))));
                        newData[z,y,x] = (byte)(Math.Min(255, data[z, y, x] + (0.1 * newData[z, y, x])));
                    }
            });
            return newData;
        }

        public static byte[,,] BilateralFilter(byte[,,] data, bool edge)
        {
            int size = 5;
            byte[,,] newData = new byte[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
            double[,,] tempData = new double[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);

            double[,,] Coef3D = GausCoef3D();

            double val = 1 / Math.Sqrt(2 * Math.PI);

            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        double weigts = 0;

                        for (int zd = -size / 2; zd <= size / 2; zd++)
                            for (int yd = -size / 2; yd <= size / 2; yd++)
                                for (int xd = -size / 2; xd <= size / 2; xd++)
                                {
                                    var xC = Math.Max(0, Math.Min(vCols - 1, x + xd));
                                    var yC = Math.Max(0, Math.Min(vRows - 1, y + yd));
                                    var zC = Math.Max(0, Math.Min(vSlis - 1, z + zd));
                                    double weight = Coef3D[(size/2)+zd, (size/2)+yd, (size/2)+xd]* val * Math.Exp(-Math.Pow(Math.Abs(data[z, y, x] - data[zC, yC, xC]), 2) / 2);
                                    tempData[z, y, x] += weight * data[zC, yC, xC];
                                    weigts += weight;
                                }
                        newData[z, y, x] = (byte)(Math.Min(255, tempData[z, y, x] / weigts));
                        if(edge)
                            newData[z, y, x] = (byte)Math.Min(255, data[z, y, x] + Math.Max(0, 2*(data[z, y, x] - newData[z, y, x])));
                    }
            });
            return newData;
        }

        public static byte[,,] CotrastSt(byte[,,] data)
        { 
            byte[,,] newData = new byte[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);
            Parallel.For(0, vSlis, z =>
            {
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)

                        newData[z, y, x] = (byte)Math.Min(255,data[z, y, x] * (255 / 180f));
            });
            return newData;
        }

        public static byte[,,] HistogramEqualization(byte[,,] data)
        {
            int vSlis = data.GetLength(0);
            int vRows = data.GetLength(1);
            int vCols = data.GetLength(2);

            byte[,,] newData = new byte[vSlis, vRows, vCols];
            int[] pixeles = new int[256];
            int k = (vSlis * vRows * vCols) / 256;

            for (int z = 0; z < vSlis; z++)
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                    {
                        pixeles[data[z, y, x]] += 1;

                    }

            int count = 0;
            int level = 0;
            int[] levels = new int[256];

            for (int i = 0; i < 256; i++)
            {
                levels[i] = level;

                if (count + pixeles[i] > (level + 1) * k)
                    level++;

                count += pixeles[i];
            }

            for (int z = 0; z < vSlis; z++)
                for (int y = 0; y < vRows; y++)
                    for (int x = 0; x < vCols; x++)
                        newData[z, y, x] = (byte)(levels[data[z, y, x]]);

            return newData;
        }
    }
}
