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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailSeaRat));
            this.ShowOutput = new System.Windows.Forms.Label();
            this.ShowTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ShowOutput
            // 
            this.ShowOutput.AutoSize = true;
            this.ShowOutput.Font = new System.Drawing.Font("新細明體", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ShowOutput.Location = new System.Drawing.Point(12, 55);
            this.ShowOutput.Name = "ShowOutput";
            this.ShowOutput.Size = new System.Drawing.Size(80, 27);
            this.ShowOutput.TabIndex = 58;
            this.ShowOutput.Text = "label4";
            this.ShowOutput.Visible = false;
            // 
            // ShowTime
            // 
            this.ShowTime.AutoSize = true;
            this.ShowTime.Font = new System.Drawing.Font("新細明體", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ShowTime.Location = new System.Drawing.Point(13, 13);
            this.ShowTime.Name = "ShowTime";
            this.ShowTime.Size = new System.Drawing.Size(59, 20);
            this.ShowTime.TabIndex = 59;
            this.ShowTime.Text = "label1";
            // 
            // MailSeaRat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 207);
            this.Controls.Add(this.ShowTime);
            this.Controls.Add(this.ShowOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MailSeaRat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MailSeaRat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ShowOutput;
        private System.Windows.Forms.Label ShowTime;
    }
}

