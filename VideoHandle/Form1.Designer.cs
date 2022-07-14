namespace VideoHandle
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblImg = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblShortV = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnClip = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblOrigin = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(30, 231);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(221, 153);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(392, 518);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 34);
            this.btnClose.TabIndex = 22;
            this.btnClose.Text = "關閉";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblImg
            // 
            this.lblImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblImg.ForeColor = System.Drawing.Color.Red;
            this.lblImg.Location = new System.Drawing.Point(30, 183);
            this.lblImg.Name = "lblImg";
            this.lblImg.Size = new System.Drawing.Size(475, 25);
            this.lblImg.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(21, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 20;
            this.label3.Text = "影片片名";
            // 
            // lblShortV
            // 
            this.lblShortV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblShortV.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblShortV.ForeColor = System.Drawing.Color.Red;
            this.lblShortV.Location = new System.Drawing.Point(30, 115);
            this.lblShortV.Name = "lblShortV";
            this.lblShortV.Size = new System.Drawing.Size(475, 25);
            this.lblShortV.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(21, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "影片編號";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(21, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 16;
            this.label5.Text = "原始影片檔路徑";
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(30, 518);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(98, 34);
            this.btnPlay.TabIndex = 24;
            this.btnPlay.Text = "播放";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnClip
            // 
            this.btnClip.Location = new System.Drawing.Point(132, 518);
            this.btnClip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClip.Name = "btnClip";
            this.btnClip.Size = new System.Drawing.Size(98, 34);
            this.btnClip.TabIndex = 25;
            this.btnClip.Text = "截取";
            this.btnClip.UseVisualStyleBackColor = true;
            this.btnClip.Click += new System.EventHandler(this.btnClip_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(234, 518);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(98, 34);
            this.btnStop.TabIndex = 26;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblOrigin
            // 
            this.lblOrigin.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrigin.Location = new System.Drawing.Point(31, 48);
            this.lblOrigin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lblOrigin.Name = "lblOrigin";
            this.lblOrigin.Size = new System.Drawing.Size(474, 30);
            this.lblOrigin.TabIndex = 27;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(277, 238);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(227, 146);
            this.pictureBox2.TabIndex = 28;
            this.pictureBox2.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(337, 518);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 609);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblOrigin);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnClip);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblImg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblShortV);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblImg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblShortV;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnClip;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox lblOrigin;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button button1;
    }
}

