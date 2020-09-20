namespace OpticalFlowOtherTest
{
    partial class Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.error = new System.Windows.Forms.TextBox();
            this.density = new System.Windows.Forms.TextBox();
            this.filter = new System.Windows.Forms.ComboBox();
            this.algorithm = new System.Windows.Forms.ComboBox();
            this.evaluate = new System.Windows.Forms.Button();
            this.edgeSamples = new System.Windows.Forms.Button();
            this.clearSamples = new System.Windows.Forms.Button();
            this.spreadSamples = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.loadVelocities = new System.Windows.Forms.Button();
            this.saveVelocities = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.selector = new System.Windows.Forms.TrackBar();
            this.process = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.canvasXY = new System.Windows.Forms.PictureBox();
            this.canvasYZ = new System.Windows.Forms.PictureBox();
            this.canvasXZ = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selector)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvasXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvasYZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvasXZ)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(855, 609);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.error);
            this.panel1.Controls.Add(this.density);
            this.panel1.Controls.Add(this.filter);
            this.panel1.Controls.Add(this.algorithm);
            this.panel1.Controls.Add(this.evaluate);
            this.panel1.Controls.Add(this.edgeSamples);
            this.panel1.Controls.Add(this.clearSamples);
            this.panel1.Controls.Add(this.spreadSamples);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.loadVelocities);
            this.panel1.Controls.Add(this.saveVelocities);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.selector);
            this.panel1.Controls.Add(this.process);
            this.panel1.Controls.Add(this.load);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(134, 603);
            this.panel1.TabIndex = 0;
            // 
            // error
            // 
            this.error.Location = new System.Drawing.Point(9, 543);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(118, 20);
            this.error.TabIndex = 14;
            // 
            // density
            // 
            this.density.Location = new System.Drawing.Point(9, 517);
            this.density.Name = "density";
            this.density.Size = new System.Drawing.Size(118, 20);
            this.density.TabIndex = 13;
            // 
            // filter
            // 
            this.filter.FormattingEnabled = true;
            this.filter.Items.AddRange(new object[] {
            "Gaussian",
            "Mean",
            "Median",
            "Bilateral",
            "Prewitt",
            "Sobel",
            "Roberts",
            "LoG",
            "Gaussian Edges",
            "Mean  Edges",
            "Median  Edges",
            "Bilateral  Edges",
            "Contrast ",
            "Histogram"});
            this.filter.Location = new System.Drawing.Point(9, 126);
            this.filter.Name = "filter";
            this.filter.Size = new System.Drawing.Size(118, 21);
            this.filter.TabIndex = 12;
            this.filter.SelectedIndexChanged += new System.EventHandler(this.filter_SelectedIndexChanged);
            // 
            // algorithm
            // 
            this.algorithm.FormattingEnabled = true;
            this.algorithm.Items.AddRange(new object[] {
            "Lucas-Kanade",
            "Census Transform"});
            this.algorithm.Location = new System.Drawing.Point(9, 98);
            this.algorithm.Name = "algorithm";
            this.algorithm.Size = new System.Drawing.Size(118, 21);
            this.algorithm.TabIndex = 11;
            this.algorithm.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // evaluate
            // 
            this.evaluate.Location = new System.Drawing.Point(9, 465);
            this.evaluate.Name = "evaluate";
            this.evaluate.Size = new System.Drawing.Size(118, 46);
            this.evaluate.TabIndex = 10;
            this.evaluate.Text = "Evaluate";
            this.evaluate.UseVisualStyleBackColor = true;
            this.evaluate.Click += new System.EventHandler(this.button7_Click);
            // 
            // edgeSamples
            // 
            this.edgeSamples.Location = new System.Drawing.Point(9, 205);
            this.edgeSamples.Name = "edgeSamples";
            this.edgeSamples.Size = new System.Drawing.Size(118, 46);
            this.edgeSamples.TabIndex = 8;
            this.edgeSamples.Text = "Edge samples";
            this.edgeSamples.UseVisualStyleBackColor = true;
            this.edgeSamples.Click += new System.EventHandler(this.button5_Click);
            // 
            // clearSamples
            // 
            this.clearSamples.Location = new System.Drawing.Point(9, 309);
            this.clearSamples.Name = "clearSamples";
            this.clearSamples.Size = new System.Drawing.Size(118, 46);
            this.clearSamples.TabIndex = 6;
            this.clearSamples.Text = "Clear samples";
            this.clearSamples.UseVisualStyleBackColor = true;
            this.clearSamples.Click += new System.EventHandler(this.button2_Click);
            // 
            // spreadSamples
            // 
            this.spreadSamples.Location = new System.Drawing.Point(9, 257);
            this.spreadSamples.Name = "spreadSamples";
            this.spreadSamples.Size = new System.Drawing.Size(118, 46);
            this.spreadSamples.TabIndex = 6;
            this.spreadSamples.Text = "Spread samples";
            this.spreadSamples.UseVisualStyleBackColor = true;
            this.spreadSamples.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 306);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 306);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 5;
            // 
            // loadVelocities
            // 
            this.loadVelocities.Location = new System.Drawing.Point(9, 361);
            this.loadVelocities.Name = "loadVelocities";
            this.loadVelocities.Size = new System.Drawing.Size(118, 46);
            this.loadVelocities.TabIndex = 4;
            this.loadVelocities.Text = "Load Velocities";
            this.loadVelocities.UseVisualStyleBackColor = true;
            this.loadVelocities.Click += new System.EventHandler(this.loadVelocities_Click);
            // 
            // saveVelocities
            // 
            this.saveVelocities.Location = new System.Drawing.Point(9, 413);
            this.saveVelocities.Name = "saveVelocities";
            this.saveVelocities.Size = new System.Drawing.Size(118, 46);
            this.saveVelocities.TabIndex = 4;
            this.saveVelocities.Text = "Save Velocities";
            this.saveVelocities.UseVisualStyleBackColor = true;
            this.saveVelocities.Click += new System.EventHandler(this.saveVelocities_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(9, 569);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(118, 16);
            this.progressBar1.TabIndex = 3;
            // 
            // selector
            // 
            this.selector.Location = new System.Drawing.Point(9, 61);
            this.selector.Name = "selector";
            this.selector.Size = new System.Drawing.Size(118, 45);
            this.selector.TabIndex = 2;
            this.selector.ValueChanged += new System.EventHandler(this.selector_ValueChanged);
            // 
            // process
            // 
            this.process.Location = new System.Drawing.Point(9, 153);
            this.process.Name = "process";
            this.process.Size = new System.Drawing.Size(118, 46);
            this.process.TabIndex = 1;
            this.process.Text = "Process Optical Flow";
            this.process.UseVisualStyleBackColor = true;
            this.process.Click += new System.EventHandler(this.process_Click);
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(9, 9);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(118, 46);
            this.load.TabIndex = 0;
            this.load.Text = "Load Images";
            this.load.UseVisualStyleBackColor = true;
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.27785F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.72215F));
            this.tableLayoutPanel2.Controls.Add(this.canvasXY, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.canvasYZ, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.canvasXZ, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(143, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.34992F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.65008F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(709, 603);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // canvasXY
            // 
            this.canvasXY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasXY.Location = new System.Drawing.Point(3, 3);
            this.canvasXY.Name = "canvasXY";
            this.canvasXY.Size = new System.Drawing.Size(470, 376);
            this.canvasXY.TabIndex = 0;
            this.canvasXY.TabStop = false;
            this.canvasXY.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasXY_Paint);
            this.canvasXY.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasXY_MouseMove);
            // 
            // canvasYZ
            // 
            this.canvasYZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasYZ.Location = new System.Drawing.Point(479, 3);
            this.canvasYZ.Name = "canvasYZ";
            this.canvasYZ.Size = new System.Drawing.Size(227, 376);
            this.canvasYZ.TabIndex = 1;
            this.canvasYZ.TabStop = false;
            this.canvasYZ.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasYZ_Paint);
            this.canvasYZ.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasYZ_MouseMove);
            // 
            // canvasXZ
            // 
            this.canvasXZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasXZ.Location = new System.Drawing.Point(3, 385);
            this.canvasXZ.Name = "canvasXZ";
            this.canvasXZ.Size = new System.Drawing.Size(470, 215);
            this.canvasXZ.TabIndex = 2;
            this.canvasXZ.TabStop = false;
            this.canvasXZ.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasXZ_Paint);
            this.canvasXZ.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasXZ_MouseMove);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 609);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form";
            this.Text = "Optical flow testing";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selector)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvasXY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvasYZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvasXZ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar selector;
        private System.Windows.Forms.Button process;
        private System.Windows.Forms.Button load;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox canvasXY;
        private System.Windows.Forms.PictureBox canvasYZ;
        private System.Windows.Forms.PictureBox canvasXZ;
        private System.Windows.Forms.Button saveVelocities;
        private System.Windows.Forms.Button loadVelocities;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button spreadSamples;
        private System.Windows.Forms.Button clearSamples;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button edgeSamples;
        private System.Windows.Forms.Button evaluate;
        private System.Windows.Forms.ComboBox algorithm;
        private System.Windows.Forms.ComboBox filter;
        private System.Windows.Forms.TextBox error;
        private System.Windows.Forms.TextBox density;
    }
}

