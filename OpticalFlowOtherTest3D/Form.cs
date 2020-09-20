using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilDICOM;
using System.Drawing.Imaging;
using System.Numerics;
using OpticalFlowDetermining;
using Filters;
using System.Runtime.Serialization.Formatters.Binary;
using EvilDICOM.Core.Element;
using EvilDICOM.Core.Helpers;

namespace OpticalFlowOtherTest
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        List<DiVolume> volumes;

        Bitmap sliceXY;
        Bitmap sliceXZ;
        Bitmap sliceYZ;

        int[,,][] positions;
        byte[][,,] images;
        int method = 0;
        int useFilter = 14;

        int selectedImage;

        private Bitmap GetSliceXYImageFor(DiVolume image, int selectedSlice)
        {
            Bitmap bitmap = new Bitmap(image.Cols, image.Rows);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

             Parallel.For(0, image.Rows, py =>
                {
                    unsafe
                    {
                        for (int px = 0; px < image.Cols; px++)
                        {
                            var value = image.Data[selectedSlice, py, px];
                            *(((int*)data.Scan0) + py * image.Cols + px) = ColorFrom(value).ToArgb();
                        }
                    }
            });

            bitmap.UnlockBits(data);
            
            return bitmap;
        }

        private Bitmap GetSliceXZImageFor(DiVolume image, int selectedSlice)
        {
            Bitmap bitmap = new Bitmap(image.Cols, image.Slices);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Parallel.For(0, image.Slices, pz =>
                {
                    unsafe
                    {
                        for (int px = 0; px < image.Cols; px++)
                        {
                            var value = image.Data[pz, selectedSlice, px];
                            *(((int*)data.Scan0) + pz * image.Cols + px) = ColorFrom(value).ToArgb();
                        }
                    }
            });

            bitmap.UnlockBits(data);

            return bitmap;
        }

        private Bitmap GetSliceYZImageFor(DiVolume image, int selectedSlice)
        {
            GC.Collect();

            Bitmap bitmap = new Bitmap(image.Slices, image.Rows);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

           Parallel.For(0, image.Rows, py =>
            {
                unsafe
                {
                    for (int pz = 0; pz < image.Slices; pz++)
                    {
                        var value = image.Data[pz, py, selectedSlice];
                        *(((int*)data.Scan0) + py * image.Slices + pz) = ColorFrom(value).ToArgb();
                    }
                }
            });

            bitmap.UnlockBits(data);

            return bitmap;
        }

        private Bitmap UpdateSliceXYImageFor(Bitmap bitmap, DiVolume image, int selectedSlice)
        {
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Parallel.For(0, image.Rows, py =>
                {
                    unsafe
                    {
                        for (int px = 0; px < image.Cols; px++)
                        {
                            var value = image.Data[selectedSlice, py, px];
                            *(((int*)data.Scan0) + py * image.Cols + px) = ColorFrom(value).ToArgb();
                        }
                    }
                });

                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        private Bitmap UpdateSliceXZImageFor(Bitmap bitmap, DiVolume image, int selectedSlice)
        {
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Parallel.For(0, image.Slices, pz =>
                {
                    unsafe
                    {
                        for (int px = 0; px < image.Cols; px++)
                        {
                            var value = image.Data[pz, selectedSlice, px];
                            *(((int*)data.Scan0) + pz * image.Cols + px) = ColorFrom(value).ToArgb();
                        }
                    }
                });

                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        private Bitmap UpdateSliceYZImageFor(Bitmap bitmap, DiVolume image, int selectedSlice)
        {
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Parallel.For(0, image.Rows, py =>
                {
                    unsafe
                    {
                        for (int pz = 0; pz < image.Slices; pz++)
                        {
                            var value = image.Data[pz, py, selectedSlice];
                            *(((int*)data.Scan0) + py * image.Slices + pz) = ColorFrom(value).ToArgb();
                        }
                    }
                });

                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        float minValue= 0;
        float maxValue = 255;

        private Color ColorFrom(float value)
        {
            float intensity = (Math.Min(maxValue, Math.Max(minValue, value)) - minValue) / (maxValue - minValue);
            return Color.FromArgb(255, (int)(255 * intensity), (int)(255 * intensity), (int)(255 * intensity));
        }

        private void load_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.SelectedPath = Application.StartupPath;
            if (browser.ShowDialog() == DialogResult.OK)
            {
                var dirs = Directory.GetDirectories(browser.SelectedPath).OrderBy(d => int.Parse(Path.GetFileName(d))).ToArray();
                volumes = new List<DiVolume>();
                images = new byte[dirs.Count()][,,];

                int counter = 0;
                foreach (var dir in dirs)
                {
                    var data = DiVolume.LoadFrom(dir);
                    images[counter] = data;
                    byte[,,] newdata = data;
                    switch (useFilter)
                    {
                        case 0:
                            newdata = Filter.GaussianFilter(data, false);
                            break;
                        case 1:
                            newdata = Filter.MeanFilter(data, false);
                            break;
                        case 2:
                            newdata = Filter.MedianFilter(data, false);
                            break;
                        case 3:
                            newdata = Filter.BilateralFilter(data, false);
                            break;
                        case 4:
                            newdata = Filter.PrewittFilter(data);
                            break;
                        case 5:
                            newdata = Filter.SobelFilter(data);
                            break;
                        case 6:
                            newdata = Filter.RobertsFilter(data);
                            break;
                        case 7:
                            newdata = Filter.LoGFilter(data);
                            break;
                        case 8:
                            newdata = Filter.GaussianFilter(data, true);
                            break;
                        case 9:
                            newdata = Filter.MeanFilter(data, true);
                            break;
                        case 10:
                            newdata = Filter.MedianFilter(data, true);
                            break;
                        case 11:
                            newdata = Filter.BilateralFilter(data, true);
                            break;
                        case 12:
                            newdata = Filter.CotrastSt(data);
                            break;
                        case 13:
                            newdata = Filter.HistogramEqualization(data);
                            break;
                        default:
                            break;

                    }
                    var volume = new DiVolume(newdata);

                    volumes.Add(volume);

                    vWidth = volume.Cols;
                    vHeight = volume.Rows;
                    vSlices = volume.Slices;

                    SetStage("Loading...", (++counter) / (float)dirs.Length);
                }

                sliceXY = new Bitmap(vWidth, vHeight);
                sliceXZ = new Bitmap(vWidth, vSlices);
                sliceYZ = new Bitmap(vSlices, vHeight);

                SetCursorAt(vWidth / 2, vHeight / 2, vSlices / 2);

                selector.Minimum = 0;
                selector.Maximum = volumes.Count - 1;
                selector.Value = 0;

                SelectImage(0);
            }
        }

        private void SelectImage(int v)
        {
            selectedImage = v;
            SetCursorAt(selectedX, selectedY, selectedZ);
        }

        private void SetStage(string stage, float value)
        {
            progressBar1.Value = (int)(progressBar1.Maximum * value);
        }

        private void selector_ValueChanged(object sender, EventArgs e)
        {
            SelectImage(selector.Value);
        }

        void FromCoordToCell (float3 position, out int pz, out int py, out int px)
        {
            pz = Math.Max(0, Math.Min(vSlices - 1, (int)position.z));
            py = Math.Max(0, Math.Min(vHeight - 1, (int)position.y));
            px = Math.Max(0, Math.Min(vWidth - 1, (int)position.x));
        }

        private void process_Click(object sender, EventArgs e)
        {
            GC.Collect();

            positions = new int[vSlices, vHeight, vWidth][];
            float3[,,] currentPosition = new float3[vSlices, vHeight, vWidth];
            float[,,] arrivingEdgeIntensity = new float[vSlices, vHeight, vWidth];
            int[,,] arrivingFrom = new int[vSlices, vHeight, vWidth];

            for (int pz = 0; pz < vSlices; pz++)
                for (int py = 0; py < vHeight; py++)
                    for (int px = 0; px < vWidth; px++)
                    {
                        positions[pz, py, px] = new int[volumes.Count];
                        positions[pz, py, px][0] = pz * (vWidth * vHeight) + py * vWidth + px;
                        currentPosition[pz, py, px] = new float3(px + 0.5f, py + 0.5f, pz + 0.5f);
                        arrivingEdgeIntensity[pz, py, px] = byte.MinValue;
                        arrivingFrom[pz, py, px] = -1;
                    }

            IOpticalFlowProcess3D processor;
           
            if (method == 0)
                processor = new NewPyrOpticalFlowProcess3D();
            else
                processor = new CensusOpticalFlowProcess3D();

            float limit;
            var edge = Edge(volumes[0].Data, out limit);

            SetStage("Processing...", 0);

            for (int i=0;i<volumes.Count-1; i++)
            {
                GC.Collect();
                var velocities = processor.GetFlow(volumes[i].Data, volumes[i + 1].Data);


                for (int pz = 0; pz < vSlices; pz++)
                    for (int py = 0; py < vHeight; py++)
                        for (int px = 0; px < vWidth; px++)
                            if (positions[pz, py, px] != null) // particle is alive
                            {
                                //flowField[pz, py, px] = new float3(velocities[pz, py, px].x, velocities[pz, py, px].y, velocities[pz, py, px].z / (vWidth/vSlices) / 3f);
                                int cz, cy, cx;
                                FromCoordToCell(currentPosition[pz, py, px], out cz, out cy, out cx);

                                // compute next frame particle position
                                var nextPosition = currentPosition[pz, py, px] + velocities.sample(currentPosition[pz, py, px]);//[cz, cy, cx];

                                // get the next frame particle cell
                                int nz, ny, nx;
                                FromCoordToCell(nextPosition, out nz, out ny, out nx);

                                //var nextPosition2 = currentPosition[pz, py, px] + groundTruth.sample(currentPosition[pz, py, px]);//[cz, cy, cx];

                                // get the next frame particle cell
                                // int nz2, ny2, nx2;
                                // FromCoordToCell(nextPosition2, out nz2, out ny2, out nx2);

                                //int dist = Math.Abs(nz2 - nz) + Math.Abs(ny2 - ny) + Math.Abs(nx2 - nx);

                                // if it arrives there more smooth than other...
                                if (edge[pz, py, px] > arrivingEdgeIntensity[nz, ny, nx])
                                {
                                    if (arrivingFrom[nz, ny, nx] != -1) // was antoher already...
                                    { // delete path was arriving there
                                        int fz = arrivingFrom[nz, ny, nx] / (vWidth * vHeight);
                                        int fy = arrivingFrom[nz, ny, nx] % (vWidth * vHeight) / vWidth;
                                        int fx = arrivingFrom[nz, ny, nx] % vWidth;
                                        positions[fz, fy, fx] = null;
                                    }
                                    arrivingFrom[nz, ny, nx] = pz * vWidth * vHeight + py * vWidth + px;
                                    arrivingEdgeIntensity[nz, ny, nx] = edge[pz, py, px];

                                    // int truthPos = nz2 * vWidth * vHeight + ny2 * vWidth + nx2;
                                    if (positions[pz, py, px] != null)
                                        positions[pz, py, px][i + 1] = nz * vWidth * vHeight + ny * vWidth + nx;
                                    //float3 truth = cellToCoord(truthPos);
                                    //float3 est = cellToCoord(positions[pz, py, px][i + 1]);
                                    //float3 vectTruth = new float3(truth.z - currentPosition[pz, py, px].z, truth.y - currentPosition[pz, py, px].y, truth.x - currentPosition[pz, py, px].x);
                                    //float3 vectEst = new float3(est.z - currentPosition[pz, py, px].z, est.y - currentPosition[pz, py, px].y, est.x - currentPosition[pz, py, px].x);
                                    //float dist = float3.length(vectTruth);
                                    //float dist1 = float3.length(vectEst);

                                    //if (dist > 1 && dist1 > 1) // && edge[pz, py, px] > limit)
                                    //{
                                    //Console.WriteLine(vectEst.x + " " + vectEst.y + " " + vectEst.z);
                                    //Console.WriteLine(vectTruth.x + " " + vectTruth.y + " " + vectTruth.z);
                                    //Console.WriteLine();
                                    //    angularError += Math.Acos(Math.Min(1, Math.Max(-1, float3.dot(float3.normalize(vectTruth), float3.normalize(vectEst)))));
                                    //    nAE++;

                                    // double temp = Math.Sqrt(Math.Pow(vectEst.x - vectTruth.x, 2) +
                                    //      Math.Pow(vectEst.y - vectTruth.y, 2) +
                                    //    Math.Pow(vectEst.z - vectTruth.z, 2));

                                    //endPointError += temp;
                                    //nEPE++;

                                    //  endPointErrorRel += (temp / dist);
                                    //nEPER++;

                                    // }

                                    //Console.WriteLine(vecta.x + " " + vecta.y + " " + vecta.z);
                                    // Console.WriteLine(vectb.x + " " + vectb.y + " " + vectb.z);
                                    // Console.WriteLine();

                                    // float tz1 = positions[pz, py, px][i] / (vWidth * vHeight);
                                    // float ty1 = positions[pz, py, px][i] % (vWidth * vHeight) / vWidth;
                                    // float tx1 = positions[pz, py, px][i] % vWidth;
                                    // float3 prevcellCoord = new float3(tx1, ty1, tz1);
                                    // float tz = a / (vWidth * vHeight);
                                    // float ty = a % (vWidth * vHeight) / vWidth;
                                    // float tx = a % vWidth;
                                    // float3 cellCoord = new float3(tx, ty, tz);
                                    // float tz2 = positions[pz, py, px][i + 1] / (vWidth * vHeight);
                                    // float ty2 = positions[pz, py, px][i + 1] % (vWidth * vHeight) / vWidth;
                                    // float tx2 = positions[pz, py, px][i + 1] % vWidth;
                                    // float3 currentCellCoord = new float3(tx, ty, tz);



                                    //int dist = Math.Abs(nz2 - nz) + Math.Abs(ny2 - ny) + Math.Abs(nx2 - nx);
                                    // totalDist += dist;
                                }
                                else // release particle
                                {
                                    positions[pz, py, px] = null;
                                }

                                currentPosition[pz, py, px] = nextPosition;
                            }

                // endPointError /= nEPE;
                // angularError /= nAE;
                // endPointErrorRel /= nEPER;

                //Console.WriteLine(endPointError);
                //Console.WriteLine(endPointErrorRel);
                // Console.WriteLine(angularError);
                //Console.WriteLine("Manhattan : "+totalDist / (float)count);   
                ComputeError();
                ComputeDensity();
                for (int pz = 0; pz < vSlices; pz++)
                    for (int py = 0; py < vHeight; py++)
                        for (int px = 0; px < vWidth; px++)
                        {
                            arrivingFrom[pz, py, px] = -1;
                            arrivingEdgeIntensity[pz, py, px] = float.MinValue;

                            //if (positions[pz, py, px] != null)
                            //{
                            // var trayectory = myPositions[pz, py, px];
                            //float tz = trayectory[0] / (vWidth * vHeight);
                            //float ty = trayectory[0] % (vWidth * vHeight) / vWidth;
                            //float tx = trayectory[0] % vWidth;
                            //float3 cellCoord = new float3(tx, ty, tz);
                            //float tz2 = trayectory[1] / (vWidth * vHeight);
                            //float ty2 = trayectory[1] % (vWidth * vHeight) / vWidth;
                            //float tx2 = trayectory[1] % vWidth;
                            //float3 currentCellCoord = new float3(tx, ty, tz);
                            //flowField[pz, py, px] = new float3(currentCellCoord.x - cellCoord.x, currentCellCoord.y - cellCoord.y, currentCellCoord.z - cellCoord.z);

                            //}


                        }
                SetStage("Processing...", (i + 1) / (float)(volumes.Count - 1));
            }

        }

        private void FromCoordToCell(float3 position, out float fz, out float fy, out float fx)
        {
            fz = Math.Max(0, Math.Min(vSlices - 1, position.z));
            fy = Math.Max(0, Math.Min(vHeight - 1, position.y));
            fx = Math.Max(0, Math.Min(vWidth - 1,  position.x));
        }       

        int selectedX;
        int selectedY;
        int selectedZ;

        int vWidth, vHeight, vSlices;

        PointF CoordXYToCanvas (PictureBox canvas, Point coord)
        {
            return new PointF(coord.X * canvas.ClientRectangle.Width / (float)vWidth, coord.Y * canvas.ClientRectangle.Height / (float)vHeight);
        }
        PointF CoordXZToCanvas(PictureBox canvas, Point coord)
        {
            return new PointF(coord.X * canvas.ClientRectangle.Width / (float)vWidth, coord.Y * canvas.ClientRectangle.Height / (float)vSlices);
        }
        PointF CoordYZToCanvas(PictureBox canvas, Point coord)
        {
            return new PointF(coord.X * canvas.ClientRectangle.Width / (float)vSlices, coord.Y * canvas.ClientRectangle.Height / (float)vHeight);
        }
        Point CanvasToCoordXY (PictureBox canvas, Point point)
        {
            return new Point(point.X * vWidth / canvas.ClientRectangle.Width, point.Y * vHeight / canvas.ClientRectangle.Height);
        }
        Point CanvasToCoordXZ(PictureBox canvas, Point point)
        {
            return new Point(point.X * vWidth / canvas.ClientRectangle.Width, point.Y * vSlices / canvas.ClientRectangle.Height);
        }
        Point CanvasToCoordYZ(PictureBox canvas, Point point)
        {
            return new Point(point.X * vSlices / canvas.ClientRectangle.Width, point.Y * vHeight / canvas.ClientRectangle.Height);
        }
        
        private void canvasXY_MouseMove(object sender, MouseEventArgs e)
        {
            if (volumes == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                var coord = CanvasToCoordXY((PictureBox)sender, e.Location);
                SetCursorAt(coord.X, coord.Y, selectedZ);
            }
        }

        private void SetCursorAt(int x, int y, int z)
        {
            if (x < 0) x = 0;
            if (x >= vWidth) x = vWidth - 1;
            if (y < 0) y = 0;
            if (y >= vHeight) y = vHeight - 1;
            if (z < 0) z = 0;
            if (z >= vSlices) z = vSlices - 1;

            selectedX = x;
            selectedY = y;
            selectedZ = z;
           
            UpdateSliceXYImageFor(sliceXY, volumes[selectedImage], z);
            UpdateSliceXZImageFor(sliceXZ, volumes[selectedImage], y);
            UpdateSliceYZImageFor(sliceYZ, volumes[selectedImage], x);

            canvasXY.Refresh();
            canvasXZ.Refresh();
            canvasYZ.Refresh();
        }

        private void canvasYZ_MouseMove(object sender, MouseEventArgs e)
        {
            if (volumes == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                var coord = CanvasToCoordYZ((PictureBox)sender, e.Location);
                SetCursorAt(selectedX, coord.Y, coord.X);
            }
        }

        private void canvasXZ_MouseMove(object sender, MouseEventArgs e)
        {
            if (volumes == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                var coord = CanvasToCoordXZ((PictureBox)sender, e.Location);
                SetCursorAt(coord.X, selectedY, coord.Y);
            }
        }

        private void saveVelocities_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = new FileStream("velocities.vel", FileMode.Create);

            BinaryWriter bw = new BinaryWriter(file);

            bw.Write(vSlices);
            bw.Write(vHeight);
            bw.Write(vWidth);
            bw.Write(volumes.Count);

            SetStage("Saving... ", 0);

            for (int pz = 0; pz < vSlices; pz++)
            {
                for (int py = 0; py < vHeight; py++)
                    for (int px = 0; px < vWidth; px++)
                    {
                        if (positions[pz, py, px] == null)
                        { bw.Write(false); }
                        else
                        {
                            bw.Write(true);
                            for (int i = 0; i < volumes.Count; i++)
                                bw.Write(positions[pz, py, px][i]);
                        }
                    }
                SetStage("Saving... ", (pz + 1) / (float)vSlices);
            }
            file.Close();
        }

        private void loadVelocities_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                FileStream file = new FileStream("velocities.vel", FileMode.Open);
                BinaryReader br = new BinaryReader(file);

                vSlices = br.ReadInt32();
                vHeight = br.ReadInt32();
                vWidth = br.ReadInt32();

                positions = new int[vSlices, vHeight, vWidth][];

                int volumes = br.ReadInt32();

                SetStage("Loading... ", 0);

                for (int pz = 0; pz < vSlices; pz++)
                {
                    for (int py = 0; py < vHeight; py++)
                        for (int px = 0; px < vWidth; px++)
                        {
                            if (br.ReadBoolean())
                            {
                                positions[pz, py, px] = new int[volumes];

                                for (int i = 0; i < volumes; i++)
                                    positions[pz, py, px][i] = br.ReadInt32();
                            }
                        }
                    SetStage("Loading... ", (pz + 1) / (float)vSlices);
                }

                file.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }

        }

        T sample <T>(int cellIndex, T[,,] mat)
        {
            int tz = cellIndex / (vWidth * vHeight);
            int ty = cellIndex % (vWidth * vHeight) / vWidth;
            int tx = cellIndex % vWidth;
            return mat[tz, ty, tx];
        }

        float3 cellToCoord (int cellIndex)
        {
            int tz = cellIndex / (vWidth * vHeight);
            int ty = cellIndex % (vWidth * vHeight) / vWidth;
            int tx = cellIndex % vWidth;

            return new float3(tx, ty, tz);
        }

        private void ComputeError()
        {
            float totalError = 0;
            int count = 0;
            for (int pz = 0; pz < vSlices; pz++)
                for (int py = 0; py < vHeight; py++)
                    for (int px = 0; px < vWidth; px++)
                        if (positions[pz, py, px] != null)
                        {
                            var trayectory = positions[pz, py, px];

                            float currentError = 0;

                            float totalVelocity = 0;

                            float intensity = sample(trayectory[0], images[0]);

                            float3 cellCoord = cellToCoord(trayectory[0]);

                            for (int i = 1; i < trayectory.Length; i++)
                            {
                                float currentIntensity = sample(trayectory[i], images[i]);
                                currentError += Math.Abs(intensity - currentIntensity);
                                intensity = currentIntensity;

                                float3 currentCellCoord = cellToCoord(trayectory[i]);
                                totalVelocity += float3.length(currentCellCoord - cellCoord);
                                cellCoord = currentCellCoord;
                            }
                                totalError += currentError / 2000;
                                count++;
                        }

            totalError /= count;
            error.Text = "Error: "+totalError.ToString();
        }

        private void ComputeDensity()
        {
            var value = 100 * positions.Cast<int[]>().Count(x => x != null) / (float)positions.Length;
            density.Text = "Density: "+value.ToString();
        }

        List<int> trackPoints = new List<int>();
        List<Color> trackPointColors = new List<Color>();

        Random rnd = new Random(100);

        float[,,] Edge(byte[,,] mat, out float limit)
        {
            float[,,] r = new float[mat.GetLength(0), mat.GetLength(1), mat.GetLength(2)];
            int length = 16126200;
           
            List<float> num = new List<float>(length);
            for (int pz = 1; pz < r.GetLength(0) - 1; pz++)
                for (int py = 1; py < r.GetLength(1) - 1; py++)
                    for (int px = 1; px < r.GetLength(2) - 1; px++)
                    {
                        r[pz, py, px] = mat[pz, py, px] * 26
                            - mat[pz, py - 1, px - 1]
                            - mat[pz, py - 1, px]
                            - mat[pz, py - 1, px + 1]
                            - mat[pz, py, px - 1]
                            - mat[pz, py, px + 1]
                            - mat[pz, py + 1, px - 1]
                            - mat[pz, py + 1, px]
                            - mat[pz, py + 1, px + 1]
                            - mat[pz - 1, py - 1, px - 1]
                            - mat[pz - 1, py - 1, px]
                            - mat[pz - 1, py - 1, px + 1]
                            - mat[pz - 1, py, px - 1]
                            - mat[pz - 1, py, px]
                            - mat[pz - 1, py, px + 1]
                            - mat[pz - 1, py + 1, px - 1]
                            - mat[pz - 1, py + 1, px]
                            - mat[pz - 1, py + 1, px + 1]
                            - mat[pz + 1, py - 1, px - 1]
                            - mat[pz + 1, py - 1, px]
                            - mat[pz + 1, py - 1, px + 1]
                            - mat[pz + 1, py, px - 1]
                            - mat[pz + 1, py, px]
                            - mat[pz + 1, py, px + 1]
                            - mat[pz + 1, py + 1, px - 1]
                            - mat[pz + 1, py + 1, px]
                            - mat[pz + 1, py + 1, px + 1];

                        num.Add(r[pz, py, px]);

                    }
            num.Sort();
            
            limit = num[length- 1000000];
            return r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (positions == null)
                return;

            int count = 0; 
            for (int i=0; i<300; i++)
            {
                int positionX = Math.Max(0, Math.Min(vWidth - 1, selectedX + rnd.Next(-10, 10)));
                int positionY = Math.Max(0, Math.Min(vHeight - 1, selectedY + rnd.Next(-10, 10)));
                int positionZ = Math.Max(0, Math.Min(vSlices - 1, selectedZ + rnd.Next(-10, 10)));

                if (positions[positionZ, positionY, positionX] != null)
                {
                    trackPoints.Add(positionZ * vWidth * vHeight + positionY * vWidth + positionX);
                    trackPointColors.Add(Color.FromArgb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                    if (++count == 25)
                        break;
                }
            }

            SetCursorAt(selectedX, selectedY, selectedZ);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            trackPoints.Clear();
            trackPointColors.Clear();

            SetCursorAt(selectedX, selectedY, selectedZ);
        }

        private void canvasXY_Paint(object sender, PaintEventArgs e)
        {
            var canvas = (PictureBox)sender;
            if (volumes == null)
                return;

            try
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                e.Graphics.DrawImage(sliceXY, 0, 0, canvas.ClientSize.Width, canvas.ClientSize.Height);
            }
            catch { }

            for (int i = 0; i < trackPoints.Count; i++)
            {
                int cellIndex = trackPoints[i];
                int tz = cellIndex / (vWidth * vHeight);
                int ty = cellIndex % (vWidth * vHeight) / vWidth;
                int tx = cellIndex % vWidth;
                var newPositionIndex = positions[tz, ty, tx][selectedImage];
                tz = newPositionIndex / (vWidth * vHeight);
                ty = newPositionIndex % (vWidth * vHeight) / vWidth;
                tx = newPositionIndex % vWidth;

                if (tz == selectedZ)
                {
                    PointF position = CoordXYToCanvas(canvasXY, new Point(tx, ty));
                    var alpha = 100;// (int)(255 / (float)Math.Pow(2, Math.Abs(tz - selectedZ)));

                    SizeF pixelSize = new SizeF(5*canvasXY.Width / (float)vWidth,5* canvasXY.Height / (float)vHeight);

                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(alpha, trackPointColors[i])), position.X - pixelSize.Width / 2, position.Y - pixelSize.Height / 2, pixelSize.Width, pixelSize.Height);
                }
            }

            PointF selectionPoint = CoordXYToCanvas(canvas, new Point(selectedX, selectedY));
            e.Graphics.DrawLine(Pens.Yellow, 0, selectionPoint.Y, canvas.ClientRectangle.Width, selectionPoint.Y);
            e.Graphics.DrawLine(Pens.Yellow, selectionPoint.X, 0, selectionPoint.X, canvas.ClientRectangle.Height);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (positions == null)
                return;

            for (int pz = 0; pz < vSlices; pz++)
                for (int py = 0; py < vHeight; py++)
                    for (int px = 0; px < vWidth; px++)
                    {
                        if (positions[pz,py,px] != null)
                        {
                            trackPoints.Add(pz * vWidth * vHeight + py * vWidth + px);
                            trackPointColors.Add(Color.FromArgb(255, 256 * pz / vSlices, 256 * py / vHeight, 256 * px / vWidth));
                        }
                    }

            SetCursorAt(selectedX, selectedY, selectedZ);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float limit;
            float[,,] edge = Edge(volumes[0].Data, out limit);

            for (int pz = 0; pz < edge.GetLength(0); pz++)
                for (int py = 0; py < edge.GetLength(1); py++)
                    for (int px = 0; px < edge.GetLength(2); px++)
                        if (positions[pz, py, px] != null && edge[pz,py,px] > limit)
                        {
                            trackPoints.Add(pz * vWidth * vHeight + py * vWidth + px);
                            trackPointColors.Add(Color.Yellow);// Color.FromArgb(255, 255 * px / vWidth, 255 * py / vHeight, 255 * pz / vSlices));
                        }
            SetCursorAt(selectedX, selectedY, selectedZ);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ComputeError();
            ComputeDensity();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            method = algorithm.SelectedIndex;
        }


        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            useFilter = filter.SelectedIndex;
        }

        private void canvasYZ_Paint(object sender, PaintEventArgs e)
        {
            var canvas = (PictureBox)sender;
            if (volumes == null)
                return;

            try
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                e.Graphics.DrawImage(sliceYZ, 0, 0, canvas.ClientSize.Width, canvas.ClientSize.Height);
            }
            catch { }

            for (int i = 0; i < trackPoints.Count; i++)
            {
                int cellIndex = trackPoints[i];
                int tz = cellIndex / (vWidth * vHeight);
                int ty = cellIndex % (vWidth * vHeight) / vWidth;
                int tx = cellIndex % vWidth;
                var newPositionIndex = positions[tz, ty, tx][selectedImage];
                tz = newPositionIndex / (vWidth * vHeight);
                ty = newPositionIndex % (vWidth * vHeight) / vWidth;
                tx = newPositionIndex % vWidth;

                if (tx == selectedX)
                {
                    PointF position = CoordYZToCanvas(canvasYZ, new Point(tz, ty));
                    var alpha = 100;// (int)(255 / (float)Math.Pow(2, Math.Abs(tx - selectedX)));

                    SizeF pixelSize = new SizeF(5 * canvasYZ.Width / (float)vSlices, 5 * canvasYZ.Height / (float)vHeight);

                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(alpha, trackPointColors[i])), position.X - pixelSize.Width / 2, position.Y - pixelSize.Height / 2, pixelSize.Width, pixelSize.Height);
                }
            }

            PointF selectionPoint = CoordYZToCanvas(canvas, new Point(selectedZ, selectedY));
            e.Graphics.DrawLine(Pens.Yellow, 0, selectionPoint.Y, canvas.ClientRectangle.Width, selectionPoint.Y);
            e.Graphics.DrawLine(Pens.Yellow, selectionPoint.X, 0, selectionPoint.X, canvas.ClientRectangle.Height);
        }

        private void canvasXZ_Paint(object sender, PaintEventArgs e)
        {
            var canvas = (PictureBox)sender;
            if (volumes == null)
                return;

            try 
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                e.Graphics.DrawImage(sliceXZ, 0, 0, canvas.ClientSize.Width, canvas.ClientSize.Height);
            }
            catch { }

            for (int i = 0; i < trackPoints.Count; i++)
            {
                int cellIndex = trackPoints[i];
                int tz = cellIndex / (vWidth * vHeight);
                int ty = cellIndex % (vWidth * vHeight) / vWidth;
                int tx = cellIndex % vWidth;
                var newPositionIndex = positions[tz, ty, tx][selectedImage];
                tz = newPositionIndex / (vWidth * vHeight);
                ty = newPositionIndex % (vWidth * vHeight) / vWidth;
                tx = newPositionIndex % vWidth;

                if (ty == selectedY)
                {
                    PointF position = CoordXZToCanvas(canvasXZ, new Point(tx, tz));
                    var alpha = 100;// (int)(255 / (float)Math.Pow(2, Math.Abs(tx - selectedX)));

                    SizeF pixelSize = new SizeF(5 * canvasXZ.Width / (float)vWidth, 5 * canvasXZ.Height / (float)vSlices);

                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(alpha, trackPointColors[i])), position.X - pixelSize.Width / 2, position.Y - pixelSize.Height / 2, pixelSize.Width, pixelSize.Height);
                }
            }

            PointF selectionPoint = CoordXZToCanvas(canvas, new Point(selectedX, selectedZ));
            e.Graphics.DrawLine(Pens.Yellow, 0, selectionPoint.Y, canvas.ClientRectangle.Width, selectionPoint.Y);
            e.Graphics.DrawLine(Pens.Yellow, selectionPoint.X, 0, selectionPoint.X, canvas.ClientRectangle.Height);
        }
    }

    public class DiVolume
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public int Slices { get; private set; }

        public float ColSpacing { get; private set; }

        public float RowSpacing { get; private set; }

        public float DepthSpacing { get; private set; }

        public byte[,,] Data { get; private set; }

        public DiVolume(byte[,,] data)
        {
            this.Slices = data.GetLength(0);
            this.Rows = data.GetLength(1);
            this.Cols = data.GetLength(2);
            this.Data = data;
        }

        public short this[int depth, int row, int col]
        {
            get
            {
                if (depth < 0 || depth >= Slices || row < 0 || row > Rows || col < 0 || col > Cols)
                    return 0;
                return Data[depth, row, col];
            }
        }

        public static byte[,,] LoadFrom(string path)
        {
            byte[,,] data = null;
            var files = Directory.GetFiles(path, "*.dcm").OrderBy(f => new FileInfo(f).LastWriteTime).ToArray();
            int slice = 0;
            short minim = byte.MaxValue;
            short maxim = byte.MinValue;

            foreach (var f in files)
            {
                EvilDICOM.Core.DICOMObject obj = EvilDICOM.Core.DICOMObject.Read(f);
                var rows = obj.TryGetDataValue<ushort>(new EvilDICOM.Core.Element.Tag("0028", "0010"), 0).SingleValue;
                var cols = obj.TryGetDataValue<ushort>(new EvilDICOM.Core.Element.Tag("0028", "0011"), 0).SingleValue;

                if (data == null)
                    data = new byte[1 << (int)Math.Ceiling(Math.Log(files.Length, 2)), rows, cols];

                BinaryReader br = new BinaryReader(obj.PixelStream);

                for (int py = 0; py < rows; py++)
                    for (int px = 0; px < cols; px++)
                    {
                        var value = br.ReadInt16();
                        data[slice, py, px] = (byte)(Math.Min(255, (value / 4095f) * 255));
                        data[slice, py, px] = Math.Min((byte)255, data[slice, py, px]);
                        minim = Math.Min(minim, data[slice, py, px]);
                        maxim = Math.Max(maxim, data[slice, py, px]);
                    }
                slice++;
            }

           return data;
        }

        public static void SaveTo(string path, DiVolume volume)
        {
            for (int slice = 0; slice < volume.Slices; slice++)
            {
                MemoryStream ms = new MemoryStream();

                BinaryWriter writer = new BinaryWriter(ms);
                int rows = volume.Rows;
                int cols = volume.Cols;
                for (int py = 0; py < rows; py++)
                    for (int px = 0; px < cols; px++)
                        writer.Write((short)((volume[slice, py, px]/255f)*4095));


                writer.Close();

                EvilDICOM.Core.DICOMObject obj = new EvilDICOM.Core.DICOMObject(new List<EvilDICOM.Core.Interfaces.IDICOMElement>() {
                    new UnsignedShort(new Tag("0028", "0010"), (ushort)volume.Rows),
                    new UnsignedShort(new Tag("0028", "0011"), (ushort)volume.Cols),
                    new OtherByteString (TagHelper.PIXEL_DATA, ms.ToArray())
                });

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                obj.Write(path + "\\slice" + slice + ".dcm");
            }
        }

    }
}
