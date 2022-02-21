namespace WinformApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(173, 28);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(249, 23);
            this.txtServerUrl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "WebSocket服务器地址：";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(459, 24);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(81, 30);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "建立连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(459, 24);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(81, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "断开连接";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(21, 70);
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.Size = new System.Drawing.Size(742, 322);
            this.txtContent.TabIndex = 4;
            this.txtContent.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(21, 486);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(81, 30);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(21, 418);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(742, 43);
            this.txtInput.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 541);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtServerUrl);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RichTextBox txtContent;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtInput;
    }
}

