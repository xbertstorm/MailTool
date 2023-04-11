namespace MailSeaRat
{
    partial class MailSeaRat
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailSeaRat));
            this.ShowOutput = new System.Windows.Forms.Label();
            this.ShowTime = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ShowCount = new System.Windows.Forms.Label();
            this.Reboot = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ShowOutput
            // 
            this.ShowOutput.AutoSize = true;
            this.ShowOutput.Font = new System.Drawing.Font("新細明體", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ShowOutput.Location = new System.Drawing.Point(7, 146);
            this.ShowOutput.Name = "ShowOutput";
            this.ShowOutput.Size = new System.Drawing.Size(80, 27);
            this.ShowOutput.TabIndex = 58;
            this.ShowOutput.Text = "label4";
            // 
            // ShowTime
            // 
            this.ShowTime.AutoSize = true;
            this.ShowTime.Font = new System.Drawing.Font("新細明體", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ShowTime.Location = new System.Drawing.Point(12, 64);
            this.ShowTime.Name = "ShowTime";
            this.ShowTime.Size = new System.Drawing.Size(59, 20);
            this.ShowTime.TabIndex = 59;
            this.ShowTime.Text = "label1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(95, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked);
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // ShowCount
            // 
            this.ShowCount.AutoSize = true;
            this.ShowCount.Font = new System.Drawing.Font("新細明體", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ShowCount.Location = new System.Drawing.Point(12, 107);
            this.ShowCount.Name = "ShowCount";
            this.ShowCount.Size = new System.Drawing.Size(59, 20);
            this.ShowCount.TabIndex = 61;
            this.ShowCount.Text = "label1";
            // 
            // Reboot
            // 
            this.Reboot.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Reboot.Location = new System.Drawing.Point(12, 12);
            this.Reboot.Name = "Reboot";
            this.Reboot.Size = new System.Drawing.Size(285, 39);
            this.Reboot.TabIndex = 62;
            this.Reboot.Text = "Reboot";
            this.Reboot.UseVisualStyleBackColor = true;
            this.Reboot.Click += new System.EventHandler(this.Reboot_Click);
            // 
            // MailSeaRat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 193);
            this.Controls.Add(this.Reboot);
            this.Controls.Add(this.ShowCount);
            this.Controls.Add(this.ShowTime);
            this.Controls.Add(this.ShowOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MailSeaRat";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MailSeaRat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MailSeaRat_FormClosing);
            this.Load += new System.EventHandler(this.MailSeaRat_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ShowOutput;
        private System.Windows.Forms.Label ShowTime;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label ShowCount;
        private System.Windows.Forms.Button Reboot;
    }
}

