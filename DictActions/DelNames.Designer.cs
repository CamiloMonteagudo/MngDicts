namespace Tests
  {
  partial class DelNames
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
      this.btnLoad = new System.Windows.Forms.Button();
      this.lbPath1 = new System.Windows.Forms.Label();
      this.txtPathIni = new System.Windows.Forms.TextBox();
      this.btnPathIni = new System.Windows.Forms.Button();
      this.lstIdDirsDicts = new System.Windows.Forms.CheckedListBox();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.btnCleanAll = new System.Windows.Forms.Button();
      this.btnSelAll = new System.Windows.Forms.Button();
      this.btnDelSelKeys = new System.Windows.Forms.Button();
      this.txtMsgBox = new System.Windows.Forms.TextBox();
      this.grp1.SuspendLayout();
      this.SuspendLayout();
      // 
      // grp1
      // 
      this.grp1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp1.Controls.Add(this.btnLoad);
      this.grp1.Controls.Add(this.lbPath1);
      this.grp1.Controls.Add(this.txtPathIni);
      this.grp1.Controls.Add(this.btnPathIni);
      this.grp1.Location = new System.Drawing.Point(13, 13);
      this.grp1.Margin = new System.Windows.Forms.Padding(4);
      this.grp1.Name = "grp1";
      this.grp1.Padding = new System.Windows.Forms.Padding(4);
      this.grp1.Size = new System.Drawing.Size(794, 64);
      this.grp1.TabIndex = 1;
      this.grp1.TabStop = false;
      this.grp1.Text = "Diccionario a analizar: ";
      // 
      // btnLoad
      // 
      this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoad.Location = new System.Drawing.Point(710, 23);
      this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(76, 28);
      this.btnLoad.TabIndex = 4;
      this.btnLoad.Text = "Cargar";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
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
      this.txtPathIni.Size = new System.Drawing.Size(612, 22);
      this.txtPathIni.TabIndex = 1;
      // 
      // btnPathIni
      // 
      this.btnPathIni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPathIni.Location = new System.Drawing.Point(669, 23);
      this.btnPathIni.Margin = new System.Windows.Forms.Padding(4);
      this.btnPathIni.Name = "btnPathIni";
      this.btnPathIni.Size = new System.Drawing.Size(33, 28);
      this.btnPathIni.TabIndex = 2;
      this.btnPathIni.Text = "...";
      this.btnPathIni.UseVisualStyleBackColor = true;
      this.btnPathIni.Click += new System.EventHandler(this.btnPathIni_Click);
      // 
      // lstIdDirsDicts
      // 
      this.lstIdDirsDicts.CheckOnClick = true;
      this.lstIdDirsDicts.FormattingEnabled = true;
      this.lstIdDirsDicts.Location = new System.Drawing.Point(13, 85);
      this.lstIdDirsDicts.Margin = new System.Windows.Forms.Padding(4);
      this.lstIdDirsDicts.Name = "lstIdDirsDicts";
      this.lstIdDirsDicts.Size = new System.Drawing.Size(786, 565);
      this.lstIdDirsDicts.TabIndex = 5;
      // 
      // btnCleanAll
      // 
      this.btnCleanAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCleanAll.Location = new System.Drawing.Point(162, 655);
      this.btnCleanAll.Margin = new System.Windows.Forms.Padding(4);
      this.btnCleanAll.Name = "btnCleanAll";
      this.btnCleanAll.Size = new System.Drawing.Size(162, 28);
      this.btnCleanAll.TabIndex = 6;
      this.btnCleanAll.Text = "Deseleccionar todas";
      this.btnCleanAll.UseVisualStyleBackColor = true;
      this.btnCleanAll.Click += new System.EventHandler(this.btnCleanAll_Click);
      // 
      // btnSelAll
      // 
      this.btnSelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelAll.Location = new System.Drawing.Point(13, 655);
      this.btnSelAll.Margin = new System.Windows.Forms.Padding(4);
      this.btnSelAll.Name = "btnSelAll";
      this.btnSelAll.Size = new System.Drawing.Size(136, 28);
      this.btnSelAll.TabIndex = 6;
      this.btnSelAll.Text = "Seleccionar todas";
      this.btnSelAll.UseVisualStyleBackColor = true;
      this.btnSelAll.Click += new System.EventHandler(this.btnSelAll_Click);
      // 
      // btnDelSelKeys
      // 
      this.btnDelSelKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDelSelKeys.Location = new System.Drawing.Point(560, 655);
      this.btnDelSelKeys.Margin = new System.Windows.Forms.Padding(4);
      this.btnDelSelKeys.Name = "btnDelSelKeys";
      this.btnDelSelKeys.Size = new System.Drawing.Size(239, 28);
      this.btnDelSelKeys.TabIndex = 6;
      this.btnDelSelKeys.Text = "Borrar llaves seleccionadas";
      this.btnDelSelKeys.UseVisualStyleBackColor = true;
      this.btnDelSelKeys.Click += new System.EventHandler(this.btnDelSelKeys_Click);
      // 
      // txtMsgBox
      // 
      this.txtMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMsgBox.Location = new System.Drawing.Point(13, 690);
      this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
      this.txtMsgBox.Multiline = true;
      this.txtMsgBox.Name = "txtMsgBox";
      this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtMsgBox.Size = new System.Drawing.Size(784, 66);
      this.txtMsgBox.TabIndex = 7;
      // 
      // DelNames
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(820, 759);
      this.Controls.Add(this.txtMsgBox);
      this.Controls.Add(this.btnSelAll);
      this.Controls.Add(this.btnDelSelKeys);
      this.Controls.Add(this.btnCleanAll);
      this.Controls.Add(this.lstIdDirsDicts);
      this.Controls.Add(this.grp1);
      this.Name = "DelNames";
      this.Text = "Analiza los nombres propios de un diccionario";
      this.grp1.ResumeLayout(false);
      this.grp1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion
    private System.Windows.Forms.GroupBox grp1;
    private System.Windows.Forms.Label lbPath1;
    private System.Windows.Forms.TextBox txtPathIni;
    private System.Windows.Forms.Button btnPathIni;
    private System.Windows.Forms.CheckedListBox lstIdDirsDicts;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.Button btnCleanAll;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Button btnSelAll;
    private System.Windows.Forms.Button btnDelSelKeys;
    private System.Windows.Forms.TextBox txtMsgBox;
    }
  }