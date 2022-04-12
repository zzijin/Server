
namespace Server
{
    partial class ServerMainForm
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
            this.Button_StartServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button_StartServer
            // 
            this.Button_StartServer.Location = new System.Drawing.Point(12, 12);
            this.Button_StartServer.Name = "Button_StartServer";
            this.Button_StartServer.Size = new System.Drawing.Size(75, 23);
            this.Button_StartServer.TabIndex = 0;
            this.Button_StartServer.Text = "开启服务";
            this.Button_StartServer.UseVisualStyleBackColor = true;
            this.Button_StartServer.Click += new System.EventHandler(this.Button_Start_Click);
            // 
            // ServerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Button_StartServer);
            this.Name = "ServerMainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_StartServer;
    }
}

