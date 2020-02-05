namespace system
{
    partial class uiViewEditor
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveJson = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cbViewType = new System.Windows.Forms.ComboBox();
            this.chkNewView = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(578, 321);
            this.panel1.TabIndex = 0;
            // 
            // btnSaveJson
            // 
            this.btnSaveJson.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveJson.Location = new System.Drawing.Point(597, 12);
            this.btnSaveJson.Name = "btnSaveJson";
            this.btnSaveJson.Size = new System.Drawing.Size(75, 23);
            this.btnSaveJson.TabIndex = 1;
            this.btnSaveJson.Text = "Save";
            this.btnSaveJson.UseVisualStyleBackColor = true;
            this.btnSaveJson.Click += new System.EventHandler(this.btnSaveJson_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Location = new System.Drawing.Point(597, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(226, 292);
            this.panel2.TabIndex = 2;
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoad.Location = new System.Drawing.Point(678, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cbViewType
            // 
            this.cbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbViewType.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbViewType.FormattingEnabled = true;
            this.cbViewType.Location = new System.Drawing.Point(13, 340);
            this.cbViewType.Name = "cbViewType";
            this.cbViewType.Size = new System.Drawing.Size(121, 25);
            this.cbViewType.TabIndex = 3;
            // 
            // chkNewView
            // 
            this.chkNewView.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkNewView.AutoSize = true;
            this.chkNewView.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkNewView.Location = new System.Drawing.Point(141, 340);
            this.chkNewView.Name = "chkNewView";
            this.chkNewView.Size = new System.Drawing.Size(87, 27);
            this.chkNewView.TabIndex = 4;
            this.chkNewView.Text = "CreateView";
            this.chkNewView.UseVisualStyleBackColor = true;
            // 
            // uiViewEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 523);
            this.Controls.Add(this.chkNewView);
            this.Controls.Add(this.cbViewType);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSaveJson);
            this.Controls.Add(this.panel1);
            this.Name = "uiViewEditor";
            this.Text = "uiViewEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveJson;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox cbViewType;
        private System.Windows.Forms.CheckBox chkNewView;
    }
}