namespace system
{
    partial class DlgRoot
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
            this.pnControllers = new System.Windows.Forms.Panel();
            this.pnControlItems = new System.Windows.Forms.Panel();
            this.chkItemComboBox = new System.Windows.Forms.CheckBox();
            this.chkItemButton = new System.Windows.Forms.CheckBox();
            this.chkItemCheckBox = new System.Windows.Forms.CheckBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.pnGLView = new System.Windows.Forms.Panel();
            this.pnControllers.SuspendLayout();
            this.pnControlItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnControllers
            // 
            this.pnControllers.Controls.Add(this.pnControlItems);
            this.pnControllers.Controls.Add(this.btnInit);
            this.pnControllers.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pnControllers.Location = new System.Drawing.Point(13, 13);
            this.pnControllers.Name = "pnControllers";
            this.pnControllers.Size = new System.Drawing.Size(200, 558);
            this.pnControllers.TabIndex = 0;
            // 
            // pnControlItems
            // 
            this.pnControlItems.Controls.Add(this.chkItemComboBox);
            this.pnControlItems.Controls.Add(this.chkItemButton);
            this.pnControlItems.Controls.Add(this.chkItemCheckBox);
            this.pnControlItems.Location = new System.Drawing.Point(4, 116);
            this.pnControlItems.Name = "pnControlItems";
            this.pnControlItems.Size = new System.Drawing.Size(193, 194);
            this.pnControlItems.TabIndex = 3;
            // 
            // chkItemComboBox
            // 
            this.chkItemComboBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkItemComboBox.Location = new System.Drawing.Point(3, 77);
            this.chkItemComboBox.Name = "chkItemComboBox";
            this.chkItemComboBox.Size = new System.Drawing.Size(91, 31);
            this.chkItemComboBox.TabIndex = 2;
            this.chkItemComboBox.Text = "ComboBox";
            this.chkItemComboBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkItemComboBox.UseVisualStyleBackColor = true;
            this.chkItemComboBox.Click += new System.EventHandler(this.chkItem_Click);
            // 
            // chkItemButton
            // 
            this.chkItemButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkItemButton.Location = new System.Drawing.Point(3, 40);
            this.chkItemButton.Name = "chkItemButton";
            this.chkItemButton.Size = new System.Drawing.Size(91, 31);
            this.chkItemButton.TabIndex = 2;
            this.chkItemButton.Text = "Button";
            this.chkItemButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkItemButton.UseVisualStyleBackColor = true;
            this.chkItemButton.Click += new System.EventHandler(this.chkItem_Click);
            // 
            // chkItemCheckBox
            // 
            this.chkItemCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkItemCheckBox.Location = new System.Drawing.Point(3, 3);
            this.chkItemCheckBox.Name = "chkItemCheckBox";
            this.chkItemCheckBox.Size = new System.Drawing.Size(91, 31);
            this.chkItemCheckBox.TabIndex = 2;
            this.chkItemCheckBox.Text = "CheckBox";
            this.chkItemCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkItemCheckBox.UseVisualStyleBackColor = true;
            this.chkItemCheckBox.Click += new System.EventHandler(this.chkItem_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(4, 4);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 0;
            this.btnInit.Text = "Init";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // pnGLView
            // 
            this.pnGLView.AutoScroll = true;
            this.pnGLView.Location = new System.Drawing.Point(220, 13);
            this.pnGLView.Name = "pnGLView";
            this.pnGLView.Size = new System.Drawing.Size(1039, 558);
            this.pnGLView.TabIndex = 1;
            // 
            // DlgRoot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1271, 594);
            this.Controls.Add(this.pnGLView);
            this.Controls.Add(this.pnControllers);
            this.Name = "DlgRoot";
            this.Text = "DlgRoot";
            this.pnControllers.ResumeLayout(false);
            this.pnControlItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnControllers;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Panel pnGLView;
        private System.Windows.Forms.CheckBox chkItemCheckBox;
        private System.Windows.Forms.Panel pnControlItems;
        private System.Windows.Forms.CheckBox chkItemButton;
        private System.Windows.Forms.CheckBox chkItemComboBox;
    }
}