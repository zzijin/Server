
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
            this.connPoolGridView = new System.Windows.Forms.DataGridView();
            this.ConnIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connUsedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connUsedTimes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connReceiveBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connSendBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connParseMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connExecutemsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connSendMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.check_onlyActivity = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.connPoolGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_StartServer
            // 
            this.Button_StartServer.Location = new System.Drawing.Point(12, 13);
            this.Button_StartServer.Margin = new System.Windows.Forms.Padding(4);
            this.Button_StartServer.Name = "Button_StartServer";
            this.Button_StartServer.Size = new System.Drawing.Size(96, 31);
            this.Button_StartServer.TabIndex = 0;
            this.Button_StartServer.Text = "开启服务";
            this.Button_StartServer.UseVisualStyleBackColor = true;
            this.Button_StartServer.Click += new System.EventHandler(this.Button_Start_Click);
            // 
            // connPoolGridView
            // 
            this.connPoolGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.connPoolGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ConnIndex,
            this.ConnState,
            this.connUsedTime,
            this.connUsedTimes,
            this.connReceiveBytes,
            this.connSendBytes,
            this.connParseMsg,
            this.connExecutemsg,
            this.connSendMsg});
            this.connPoolGridView.Location = new System.Drawing.Point(12, 88);
            this.connPoolGridView.Name = "connPoolGridView";
            this.connPoolGridView.RowHeadersWidth = 51;
            this.connPoolGridView.RowTemplate.Height = 29;
            this.connPoolGridView.Size = new System.Drawing.Size(1177, 409);
            this.connPoolGridView.TabIndex = 1;
            // 
            // ConnIndex
            // 
            this.ConnIndex.HeaderText = "链接序号";
            this.ConnIndex.MinimumWidth = 6;
            this.ConnIndex.Name = "ConnIndex";
            this.ConnIndex.ReadOnly = true;
            this.ConnIndex.Width = 125;
            // 
            // ConnState
            // 
            this.ConnState.HeaderText = "链接状态";
            this.ConnState.MinimumWidth = 6;
            this.ConnState.Name = "ConnState";
            this.ConnState.ReadOnly = true;
            this.ConnState.Width = 125;
            // 
            // connUsedTime
            // 
            this.connUsedTime.HeaderText = "链接使用时长";
            this.connUsedTime.MinimumWidth = 6;
            this.connUsedTime.Name = "connUsedTime";
            this.connUsedTime.ReadOnly = true;
            this.connUsedTime.Width = 125;
            // 
            // connUsedTimes
            // 
            this.connUsedTimes.HeaderText = "链接使用次数";
            this.connUsedTimes.MinimumWidth = 6;
            this.connUsedTimes.Name = "connUsedTimes";
            this.connUsedTimes.ReadOnly = true;
            this.connUsedTimes.Width = 125;
            // 
            // connReceiveBytes
            // 
            this.connReceiveBytes.HeaderText = "链接接收字节数";
            this.connReceiveBytes.MinimumWidth = 6;
            this.connReceiveBytes.Name = "connReceiveBytes";
            this.connReceiveBytes.ReadOnly = true;
            this.connReceiveBytes.Width = 125;
            // 
            // connSendBytes
            // 
            this.connSendBytes.HeaderText = "链接发送字节数";
            this.connSendBytes.MinimumWidth = 6;
            this.connSendBytes.Name = "connSendBytes";
            this.connSendBytes.ReadOnly = true;
            this.connSendBytes.Width = 125;
            // 
            // connParseMsg
            // 
            this.connParseMsg.HeaderText = "链接解析信息数";
            this.connParseMsg.MinimumWidth = 6;
            this.connParseMsg.Name = "connParseMsg";
            this.connParseMsg.ReadOnly = true;
            this.connParseMsg.Width = 125;
            // 
            // connExecutemsg
            // 
            this.connExecutemsg.HeaderText = "链接执行消息数";
            this.connExecutemsg.MinimumWidth = 6;
            this.connExecutemsg.Name = "connExecutemsg";
            this.connExecutemsg.ReadOnly = true;
            this.connExecutemsg.Width = 125;
            // 
            // connSendMsg
            // 
            this.connSendMsg.HeaderText = "链接发送消息数";
            this.connSendMsg.MinimumWidth = 6;
            this.connSendMsg.Name = "connSendMsg";
            this.connSendMsg.ReadOnly = true;
            this.connSendMsg.Width = 125;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "链接池信息统计";
            // 
            // check_onlyActivity
            // 
            this.check_onlyActivity.AutoSize = true;
            this.check_onlyActivity.Location = new System.Drawing.Point(189, 62);
            this.check_onlyActivity.Name = "check_onlyActivity";
            this.check_onlyActivity.Size = new System.Drawing.Size(136, 24);
            this.check_onlyActivity.TabIndex = 3;
            this.check_onlyActivity.Text = "仅已使用的链接";
            this.check_onlyActivity.UseVisualStyleBackColor = true;
            this.check_onlyActivity.CheckedChanged += new System.EventHandler(this.check_onlyActivity_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 513);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "内存缓存信息统计";
            // 
            // ServerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 764);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.check_onlyActivity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connPoolGridView);
            this.Controls.Add(this.Button_StartServer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ServerMainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.connPoolGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_StartServer;
        private System.Windows.Forms.DataGridView connPoolGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn connUsedTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn connUsedTimes;
        private System.Windows.Forms.DataGridViewTextBoxColumn connReceiveBytes;
        private System.Windows.Forms.DataGridViewTextBoxColumn connSendBytes;
        private System.Windows.Forms.DataGridViewTextBoxColumn connParseMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn connExecutemsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn connSendMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox check_onlyActivity;
        private System.Windows.Forms.Label label2;
    }
}

