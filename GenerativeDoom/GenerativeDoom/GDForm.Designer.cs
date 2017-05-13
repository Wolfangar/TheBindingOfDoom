namespace GenerativeDoom
{
    partial class GDForm
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
            this.BtnClose = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.upDownWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.upDownHeight = new System.Windows.Forms.NumericUpDown();
            this.picVisual = new System.Windows.Forms.PictureBox();
            this.pnlCon = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.upDownRoom = new System.Windows.Forms.NumericUpDown();
            this.upDownX = new System.Windows.Forms.NumericUpDown();
            this.upDownY = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnDoom = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.upDownKeyEnd = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.upDownLoop = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.upDownKey = new System.Windows.Forms.NumericUpDown();
            this.labelError = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.upDownBoss = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.upDownDiff = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.upDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).BeginInit();
            this.pnlCon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownKeyEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownLoop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownBoss)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownDiff)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnClose.Location = new System.Drawing.Point(485, 232);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(2);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(60, 26);
            this.BtnClose.TabIndex = 0;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(15, 218);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(1);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(79, 33);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Generation";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // upDownWidth
            // 
            this.upDownWidth.Location = new System.Drawing.Point(50, 11);
            this.upDownWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownWidth.Name = "upDownWidth";
            this.upDownWidth.Size = new System.Drawing.Size(35, 19);
            this.upDownWidth.TabIndex = 5;
            this.upDownWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Height";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Width";
            // 
            // upDownHeight
            // 
            this.upDownHeight.Location = new System.Drawing.Point(50, 29);
            this.upDownHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownHeight.Name = "upDownHeight";
            this.upDownHeight.Size = new System.Drawing.Size(35, 19);
            this.upDownHeight.TabIndex = 12;
            this.upDownHeight.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // picVisual
            // 
            this.picVisual.Location = new System.Drawing.Point(53, 36);
            this.picVisual.Name = "picVisual";
            this.picVisual.Size = new System.Drawing.Size(246, 142);
            this.picVisual.TabIndex = 1;
            this.picVisual.TabStop = false;
            // 
            // pnlCon
            // 
            this.pnlCon.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlCon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCon.Controls.Add(this.picVisual);
            this.pnlCon.Location = new System.Drawing.Point(198, 12);
            this.pnlCon.Name = "pnlCon";
            this.pnlCon.Size = new System.Drawing.Size(347, 215);
            this.pnlCon.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(-2, -3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 14);
            this.label3.TabIndex = 14;
            this.label3.Text = "Area size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(-2, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 14);
            this.label4.TabIndex = 15;
            this.label4.Text = "Start position";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Max room";
            // 
            // upDownRoom
            // 
            this.upDownRoom.Location = new System.Drawing.Point(147, 11);
            this.upDownRoom.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.upDownRoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.upDownRoom.Name = "upDownRoom";
            this.upDownRoom.Size = new System.Drawing.Size(35, 19);
            this.upDownRoom.TabIndex = 17;
            this.upDownRoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // upDownX
            // 
            this.upDownX.Location = new System.Drawing.Point(28, 67);
            this.upDownX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.upDownX.Name = "upDownX";
            this.upDownX.Size = new System.Drawing.Size(35, 19);
            this.upDownX.TabIndex = 18;
            this.upDownX.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // upDownY
            // 
            this.upDownY.Location = new System.Drawing.Point(85, 67);
            this.upDownY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.upDownY.Name = "upDownY";
            this.upDownY.Size = new System.Drawing.Size(35, 19);
            this.upDownY.TabIndex = 19;
            this.upDownY.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 15);
            this.label6.TabIndex = 20;
            this.label6.Text = "x";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(69, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "y";
            // 
            // btnDoom
            // 
            this.btnDoom.Enabled = false;
            this.btnDoom.Location = new System.Drawing.Point(103, 218);
            this.btnDoom.Margin = new System.Windows.Forms.Padding(1);
            this.btnDoom.Name = "btnDoom";
            this.btnDoom.Size = new System.Drawing.Size(79, 33);
            this.btnDoom.TabIndex = 22;
            this.btnDoom.Text = "Doomify";
            this.btnDoom.UseVisualStyleBackColor = true;
            this.btnDoom.Click += new System.EventHandler(this.btnDoom_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(63, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 15);
            this.label8.TabIndex = 23;
            this.label8.Text = "Key End Proba";
            // 
            // upDownKeyEnd
            // 
            this.upDownKeyEnd.DecimalPlaces = 2;
            this.upDownKeyEnd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.upDownKeyEnd.Location = new System.Drawing.Point(138, 121);
            this.upDownKeyEnd.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownKeyEnd.Name = "upDownKeyEnd";
            this.upDownKeyEnd.Size = new System.Drawing.Size(44, 19);
            this.upDownKeyEnd.TabIndex = 24;
            this.upDownKeyEnd.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(78, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 15);
            this.label9.TabIndex = 25;
            this.label9.Text = "Loop Proba";
            // 
            // upDownLoop
            // 
            this.upDownLoop.DecimalPlaces = 2;
            this.upDownLoop.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.upDownLoop.Location = new System.Drawing.Point(138, 144);
            this.upDownLoop.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownLoop.Name = "upDownLoop";
            this.upDownLoop.Size = new System.Drawing.Size(44, 19);
            this.upDownLoop.TabIndex = 26;
            this.upDownLoop.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(54, 187);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 15);
            this.label10.TabIndex = 27;
            this.label10.Text = "Max Normal Key";
            // 
            // upDownKey
            // 
            this.upDownKey.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.upDownKey.Location = new System.Drawing.Point(138, 187);
            this.upDownKey.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.upDownKey.Name = "upDownKey";
            this.upDownKey.Size = new System.Drawing.Size(44, 19);
            this.upDownKey.TabIndex = 28;
            this.upDownKey.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(195, 236);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(0, 15);
            this.labelError.TabIndex = 29;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(78, 166);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 15);
            this.label11.TabIndex = 30;
            this.label11.Text = "Boss Proba";
            // 
            // upDownBoss
            // 
            this.upDownBoss.DecimalPlaces = 2;
            this.upDownBoss.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.upDownBoss.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.upDownBoss.Location = new System.Drawing.Point(138, 166);
            this.upDownBoss.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownBoss.Name = "upDownBoss";
            this.upDownBoss.Size = new System.Drawing.Size(44, 19);
            this.upDownBoss.TabIndex = 31;
            this.upDownBoss.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(92, 99);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 15);
            this.label12.TabIndex = 32;
            this.label12.Text = "Difficulty";
            // 
            // upDownDiff
            // 
            this.upDownDiff.DecimalPlaces = 2;
            this.upDownDiff.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.upDownDiff.Location = new System.Drawing.Point(139, 99);
            this.upDownDiff.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownDiff.Name = "upDownDiff";
            this.upDownDiff.Size = new System.Drawing.Size(44, 19);
            this.upDownDiff.TabIndex = 33;
            this.upDownDiff.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            // 
            // GDForm
            // 
            this.AcceptButton = this.BtnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.BtnClose;
            this.ClientSize = new System.Drawing.Size(556, 261);
            this.Controls.Add(this.upDownDiff);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.upDownBoss);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.upDownKey);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.upDownLoop);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.upDownKeyEnd);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnDoom);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.upDownY);
            this.Controls.Add(this.upDownX);
            this.Controls.Add(this.upDownRoom);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pnlCon);
            this.Controls.Add(this.upDownHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.upDownWidth);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.BtnClose);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GDForm";
            this.Text = "Generative Doom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GDForm_FormClosing);
            this.Load += new System.EventHandler(this.GDForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.upDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).EndInit();
            this.pnlCon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.upDownRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownKeyEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownLoop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownBoss)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownDiff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown upDownWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown upDownHeight;
        private System.Windows.Forms.PictureBox picVisual;
        private System.Windows.Forms.Panel pnlCon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown upDownRoom;
        private System.Windows.Forms.NumericUpDown upDownX;
        private System.Windows.Forms.NumericUpDown upDownY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnDoom;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown upDownKeyEnd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown upDownLoop;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown upDownKey;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown upDownBoss;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown upDownDiff;
    }
}