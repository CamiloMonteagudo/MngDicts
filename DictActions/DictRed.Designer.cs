namespace Tests
  {
  partial class DictRed
    {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
      {
      if( disposing && (components != null) )
        {
        components.Dispose();
        }
      base.Dispose( disposing );
      }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
      {
      this.grp1 = new System.Windows.Forms.GroupBox();
      this.lbPath1 = new System.Windows.Forms.Label();
      this.txtPathIni = new System.Windows.Forms.TextBox();
      this.btnPathIni = new System.Windows.Forms.Button();
      this.btnLoad = new System.Windows.Forms.Button();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.txtMsgBox = new System.Windows.Forms.TextBox();
      this.grp1.SuspendLayout();
      this.SuspendLayout();
      // 
      // grp1
      // 
      this.grp1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp1.Controls.Add(this.lbPath1);
      this.grp1.Controls.Add(this.txtPathIni);
      this.grp1.Controls.Add(this.btnPathIni);
      this.grp1.Location = new System.Drawing.Point(4, 11);
      this.grp1.Margin = new System.Windows.Forms.Padding(4);
      this.grp1.Name = "grp1";
      this.grp1.Padding = new System.Windows.Forms.Padding(4);
      this.grp1.Size = new System.Drawing.Size(784, 64);
      this.grp1.TabIndex = 2;
      this.grp1.TabStop = false;
      this.grp1.Text = "Diccionario a analizar: ";
      // 
      // lbPath1
      // 
      this.lbPath1.AutoSize = true;
      this.lbPath1.Location = new System.Drawing.Point(7, 27);
      this.lbPath1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbPath1.Name = "lbPath1";
      this.lbPath1.Size = new System.Drawing.Size(41, 17);
      this.lbPath1.TabIndex = 0;
      this.lbPath1.Text = "Path:";
      // 
      // txtPathIni
      // 
      this.txtPathIni.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPathIni.Location = new System.Drawing.Point(49, 25);
      this.txtPathIni.Margin = new System.Windows.Forms.Padding(4);
      this.txtPathIni.Name = "txtPathIni";
      this.txtPathIni.Size = new System.Drawing.Size(602, 22);
      this.txtPathIni.TabIndex = 1;
      // 
      // btnPathIni
      // 
      this.btnPathIni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPathIni.Location = new System.Drawing.Point(659, 23);
      this.btnPathIni.Margin = new System.Windows.Forms.Padding(4);
      this.btnPathIni.Name = "btnPathIni";
      this.btnPathIni.Size = new System.Drawing.Size(33, 28);
      this.btnPathIni.TabIndex = 2;
      this.btnPathIni.Text = "...";
      this.btnPathIni.UseVisualStyleBackColor = true;
      this.btnPathIni.Click += new System.EventHandler(this.btnPathIni_Click);
      // 
      // btnLoad
      // 
      this.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.btnLoad.Location = new System.Drawing.Point(306, 83);
      this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(127, 28);
      this.btnLoad.TabIndex = 4;
      this.btnLoad.Text = "Convertir";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // txtMsgBox
      // 
      this.txtMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMsgBox.Location = new System.Drawing.Point(4, 119);
      this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
      this.txtMsgBox.Multiline = true;
      this.txtMsgBox.Name = "txtMsgBox";
      this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtMsgBox.Size = new System.Drawing.Size(792, 438);
      this.txtMsgBox.TabIndex = 8;
      // 
      // DictRed
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(802, 570);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.txtMsgBox);
      this.Controls.Add(this.grp1);
      this.Name = "DictRed";
      this.Text = "Crea diccionario de reducción";
      this.grp1.ResumeLayout(false);
      this.grp1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion
    private System.Windows.Forms.GroupBox grp1;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Label lbPath1;
    private System.Windows.Forms.TextBox txtPathIni;
    private System.Windows.Forms.Button btnPathIni;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.TextBox txtMsgBox;
    }
  }