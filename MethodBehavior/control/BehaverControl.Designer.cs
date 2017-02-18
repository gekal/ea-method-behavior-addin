namespace MethodBehavior.control
{
    partial class BehaverControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaverControl));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.enableAutoReflesh = new System.Windows.Forms.ToolStripButton();
            this.saveBehaver = new System.Windows.Forms.ToolStripButton();
            this.viewChange = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.richTextBox = new MethodBehavior.gekale.GekaleRichTextBox();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableAutoReflesh,
            this.saveBehaver,
            this.viewChange});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(700, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "ツールバー";
            // 
            // enableAutoReflesh
            // 
            this.enableAutoReflesh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.enableAutoReflesh.Image = ((System.Drawing.Image)(resources.GetObject("enableAutoReflesh.Image")));
            this.enableAutoReflesh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.enableAutoReflesh.Name = "enableAutoReflesh";
            this.enableAutoReflesh.Size = new System.Drawing.Size(23, 22);
            this.enableAutoReflesh.Text = "操作と自動リンクする。";
            this.enableAutoReflesh.Click += new System.EventHandler(this.enableAutoReflesh_Click);
            // 
            // saveBehaver
            // 
            this.saveBehaver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveBehaver.Image = ((System.Drawing.Image)(resources.GetObject("saveBehaver.Image")));
            this.saveBehaver.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveBehaver.Name = "saveBehaver";
            this.saveBehaver.Size = new System.Drawing.Size(23, 22);
            this.saveBehaver.Text = "振舞いを保存する。";
            this.saveBehaver.Click += new System.EventHandler(this.saveBehaver_Click);
            // 
            // viewChange
            // 
            this.viewChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewChange.Image = ((System.Drawing.Image)(resources.GetObject("viewChange.Image")));
            this.viewChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewChange.Name = "viewChange";
            this.viewChange.Size = new System.Drawing.Size(23, 22);
            this.viewChange.Text = "編集点を確認する。";
            this.viewChange.ToolTipText = "編集点を確認する。";
            this.viewChange.Click += new System.EventHandler(this.viewChange_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 578);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(700, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "ステータスバー";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.DoubleClickEnabled = true;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(140, 17);
            this.toolStripStatus.Text = "操作とのリンスはありません。";
            this.toolStripStatus.DoubleClick += new System.EventHandler(this.toolStripStatus_DoubleClick);
            // 
            // richTextBox
            // 
            this.richTextBox.AcceptsTab = true;
            this.richTextBox.Location = new System.Drawing.Point(3, 28);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(694, 547);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            // 
            // BehaverControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Name = "BehaverControl";
            this.Size = new System.Drawing.Size(700, 600);
            this.Resize += new System.EventHandler(this.BehaverControl_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.ToolStripButton enableAutoReflesh;
        private gekale.GekaleRichTextBox richTextBox;
        private System.Windows.Forms.ToolStripButton saveBehaver;
        private System.Windows.Forms.ToolStripButton viewChange;
    }
}
