namespace Tests
  {
  partial class MergeDicts
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
      this.txtMsgBox = new System.Windows.Forms.TextBox();
      this.txtPathSave = new System.Windows.Forms.TextBox();
      this.txtPathIni = new System.Windows.Forms.TextBox();
      this.btnPathSave = new System.Windows.Forms.Button();
      this.lbPath1 = new System.Windows.Forms.Label();
      this.btnPathIni = new System.Windows.Forms.Button();
      this.SelFolderDlg = new System.Windows.Forms.FolderBrowserDialog();
      this.lbFrm1 = new System.Windows.Forms.Label();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.txtFrmIni = new System.Windows.Forms.Label();
      this.txtFrmMezcla = new System.Windows.Forms.Label();
      this.rbXml = new System.Windows.Forms.RadioButton();
      this.rbBinary = new System.Windows.Forms.RadioButton();
      this.rbTexto1 = new System.Windows.Forms.RadioButton();
      this.grp1 = new System.Windows.Forms.GroupBox();
      this.btnNuevo = new System.Windows.Forms.Button();
      this.btnLoad = new System.Windows.Forms.Button();
      this.grp2 = new System.Windows.Forms.GroupBox();
      this.lbPath2 = new System.Windows.Forms.Label();
      this.txtPathMezcla = new System.Windows.Forms.TextBox();
      this.lbFrm2 = new System.Windows.Forms.Label();
      this.btnPathMezcla = new System.Windows.Forms.Button();
      this.btnMezclar = new System.Windows.Forms.Button();
      this.grp3 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.lbPath3 = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnMergeAll = new System.Windows.Forms.Button();
      this.grp1.SuspendLayout();
      this.grp2.SuspendLayout();
      this.grp3.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtMsgBox
      // 
      this.txtMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMsgBox.Location = new System.Drawing.Point(14, 296);
      this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
      this.txtMsgBox.Multiline = true;
      this.txtMsgBox.Name = "txtMsgBox";
      this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtMsgBox.Size = new System.Drawing.Size(784, 291);
      this.txtMsgBox.TabIndex = 3;
      // 
      // txtPathSave
      // 
      this.txtPathSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPathSave.Location = new System.Drawing.Point(49, 43);
      this.txtPathSave.Margin = new System.Windows.Forms.Padding(4);
      this.txtPathSave.Name = "txtPathSave";
      this.txtPathSave.Size = new System.Drawing.Size(579, 22);
      this.txtPathSave.TabIndex = 5;
      this.txtPathSave.Text = "Result";
      // 
      // txtPathIni
      // 
      this.txtPathIni.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPathIni.Location = new System.Drawing.Point(49, 25);
      this.txtPathIni.Margin = new System.Windows.Forms.Padding(4);
      this.txtPathIni.Name = "txtPathIni";
      this.txtPathIni.Size = new System.Drawing.Size(533, 22);
      this.txtPathIni.TabIndex = 1;
      // 
      // btnPathSave
      // 
      this.btnPathSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPathSave.Location = new System.Drawing.Point(633, 41);
      this.btnPathSave.Margin = new System.Windows.Forms.Padding(4);
      this.btnPathSave.Name = "btnPathSave";
      this.btnPathSave.Size = new System.Drawing.Size(33, 28);
      this.btnPathSave.TabIndex = 6;
      this.btnPathSave.Text = "...";
      this.btnPathSave.UseVisualStyleBackColor = true;
      this.btnPathSave.Click += new System.EventHandler(this.btnPathSave_Click);
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
      // btnPathIni
      // 
      this.btnPathIni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPathIni.Location = new System.Drawing.Point(589, 23);
      this.btnPathIni.Margin = new System.Windows.Forms.Padding(4);
      this.btnPathIni.Name = "btnPathIni";
      this.btnPathIni.Size = new System.Drawing.Size(33, 28);
      this.btnPathIni.TabIndex = 2;
      this.btnPathIni.Text = "...";
      this.btnPathIni.UseVisualStyleBackColor = true;
      this.btnPathIni.Click += new System.EventHandler(this.btnPathIni_Click);
      // 
      // lbFrm1
      // 
      this.lbFrm1.AutoSize = true;
      this.lbFrm1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFrm1.Location = new System.Drawing.Point(43, 51);
      this.lbFrm1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbFrm1.Name = "lbFrm1";
      this.lbFrm1.Size = new System.Drawing.Size(56, 15);
      this.lbFrm1.TabIndex = 4;
      this.lbFrm1.Text = "Formato:";
      // 
      // txtFrmIni
      // 
      this.txtFrmIni.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtFrmIni.Location = new System.Drawing.Point(102, 52);
      this.txtFrmIni.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.txtFrmIni.Name = "txtFrmIni";
      this.txtFrmIni.Size = new System.Drawing.Size(480, 14);
      this.txtFrmIni.TabIndex = 5;
      // 
      // txtFrmMezcla
      // 
      this.txtFrmMezcla.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtFrmMezcla.Location = new System.Drawing.Point(102, 51);
      this.txtFrmMezcla.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.txtFrmMezcla.Name = "txtFrmMezcla";
      this.txtFrmMezcla.Size = new System.Drawing.Size(514, 16);
      this.txtFrmMezcla.TabIndex = 5;
      // 
      // rbXml
      // 
      this.rbXml.AutoSize = true;
      this.rbXml.Location = new System.Drawing.Point(247, 17);
      this.rbXml.Margin = new System.Windows.Forms.Padding(4);
      this.rbXml.Name = "rbXml";
      this.rbXml.Size = new System.Drawing.Size(57, 21);
      this.rbXml.TabIndex = 3;
      this.rbXml.Text = "XML";
      this.rbXml.UseVisualStyleBackColor = true;
      // 
      // rbBinary
      // 
      this.rbBinary.AutoSize = true;
      this.rbBinary.Location = new System.Drawing.Point(170, 17);
      this.rbBinary.Margin = new System.Windows.Forms.Padding(4);
      this.rbBinary.Name = "rbBinary";
      this.rbBinary.Size = new System.Drawing.Size(73, 21);
      this.rbBinary.TabIndex = 2;
      this.rbBinary.Text = "Binario";
      this.rbBinary.UseVisualStyleBackColor = true;
      // 
      // rbTexto1
      // 
      this.rbTexto1.AutoSize = true;
      this.rbTexto1.Checked = true;
      this.rbTexto1.Location = new System.Drawing.Point(75, 17);
      this.rbTexto1.Margin = new System.Windows.Forms.Padding(4);
      this.rbTexto1.Name = "rbTexto1";
      this.rbTexto1.Size = new System.Drawing.Size(89, 21);
      this.rbTexto1.TabIndex = 1;
      this.rbTexto1.TabStop = true;
      this.rbTexto1.Text = "Text View";
      this.rbTexto1.UseVisualStyleBackColor = true;
      // 
      // grp1
      // 
      this.grp1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp1.Controls.Add(this.txtFrmIni);
      this.grp1.Controls.Add(this.lbPath1);
      this.grp1.Controls.Add(this.txtPathIni);
      this.grp1.Controls.Add(this.lbFrm1);
      this.grp1.Controls.Add(this.btnPathIni);
      this.grp1.Controls.Add(this.btnNuevo);
      this.grp1.Controls.Add(this.btnMergeAll);
      this.grp1.Controls.Add(this.btnLoad);
      this.grp1.Location = new System.Drawing.Point(13, 3);
      this.grp1.Margin = new System.Windows.Forms.Padding(4);
      this.grp1.Name = "grp1";
      this.grp1.Padding = new System.Windows.Forms.Padding(4);
      this.grp1.Size = new System.Drawing.Size(784, 112);
      this.grp1.TabIndex = 0;
      this.grp1.TabStop = false;
      this.grp1.Text = "Diccionario Inicial";
      // 
      // btnNuevo
      // 
      this.btnNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNuevo.Location = new System.Drawing.Point(711, 23);
      this.btnNuevo.Margin = new System.Windows.Forms.Padding(4);
      this.btnNuevo.Name = "btnNuevo";
      this.btnNuevo.Size = new System.Drawing.Size(66, 28);
      this.btnNuevo.TabIndex = 3;
      this.btnNuevo.Text = "Nuevo";
      this.btnNuevo.UseVisualStyleBackColor = true;
      this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
      // 
      // btnLoad
      // 
      this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoad.Location = new System.Drawing.Point(627, 23);
      this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(76, 28);
      this.btnLoad.TabIndex = 3;
      this.btnLoad.Text = "Cargar";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // grp2
      // 
      this.grp2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp2.Controls.Add(this.txtFrmMezcla);
      this.grp2.Controls.Add(this.lbPath2);
      this.grp2.Controls.Add(this.txtPathMezcla);
      this.grp2.Controls.Add(this.lbFrm2);
      this.grp2.Controls.Add(this.btnPathMezcla);
      this.grp2.Controls.Add(this.btnMezclar);
      this.grp2.Location = new System.Drawing.Point(13, 125);
      this.grp2.Margin = new System.Windows.Forms.Padding(4);
      this.grp2.Name = "grp2";
      this.grp2.Padding = new System.Windows.Forms.Padding(4);
      this.grp2.Size = new System.Drawing.Size(784, 75);
      this.grp2.TabIndex = 1;
      this.grp2.TabStop = false;
      this.grp2.Text = "Diccionario a mezclar";
      // 
      // lbPath2
      // 
      this.lbPath2.AutoSize = true;
      this.lbPath2.Location = new System.Drawing.Point(7, 27);
      this.lbPath2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbPath2.Name = "lbPath2";
      this.lbPath2.Size = new System.Drawing.Size(41, 17);
      this.lbPath2.TabIndex = 0;
      this.lbPath2.Text = "Path:";
      // 
      // txtPathMezcla
      // 
      this.txtPathMezcla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPathMezcla.Location = new System.Drawing.Point(49, 25);
      this.txtPathMezcla.Margin = new System.Windows.Forms.Padding(4);
      this.txtPathMezcla.Name = "txtPathMezcla";
      this.txtPathMezcla.Size = new System.Drawing.Size(574, 22);
      this.txtPathMezcla.TabIndex = 1;
      // 
      // lbFrm2
      // 
      this.lbFrm2.AutoSize = true;
      this.lbFrm2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFrm2.Location = new System.Drawing.Point(43, 51);
      this.lbFrm2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbFrm2.Name = "lbFrm2";
      this.lbFrm2.Size = new System.Drawing.Size(56, 15);
      this.lbFrm2.TabIndex = 4;
      this.lbFrm2.Text = "Formato:";
      // 
      // btnPathMezcla
      // 
      this.btnPathMezcla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPathMezcla.Location = new System.Drawing.Point(628, 23);
      this.btnPathMezcla.Margin = new System.Windows.Forms.Padding(4);
      this.btnPathMezcla.Name = "btnPathMezcla";
      this.btnPathMezcla.Size = new System.Drawing.Size(33, 28);
      this.btnPathMezcla.TabIndex = 2;
      this.btnPathMezcla.Text = "...";
      this.btnPathMezcla.UseVisualStyleBackColor = true;
      this.btnPathMezcla.Click += new System.EventHandler(this.btnPathMezcla_Click);
      // 
      // btnMezclar
      // 
      this.btnMezclar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMezclar.Location = new System.Drawing.Point(688, 23);
      this.btnMezclar.Margin = new System.Windows.Forms.Padding(4);
      this.btnMezclar.Name = "btnMezclar";
      this.btnMezclar.Size = new System.Drawing.Size(88, 28);
      this.btnMezclar.TabIndex = 3;
      this.btnMezclar.Text = "Mezclar";
      this.btnMezclar.UseVisualStyleBackColor = true;
      this.btnMezclar.Click += new System.EventHandler(this.btnMezclar_Click);
      // 
      // grp3
      // 
      this.grp3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp3.Controls.Add(this.rbXml);
      this.grp3.Controls.Add(this.label4);
      this.grp3.Controls.Add(this.rbBinary);
      this.grp3.Controls.Add(this.lbPath3);
      this.grp3.Controls.Add(this.rbTexto1);
      this.grp3.Controls.Add(this.btnSave);
      this.grp3.Controls.Add(this.txtPathSave);
      this.grp3.Controls.Add(this.btnPathSave);
      this.grp3.Location = new System.Drawing.Point(14, 211);
      this.grp3.Margin = new System.Windows.Forms.Padding(4);
      this.grp3.Name = "grp3";
      this.grp3.Padding = new System.Windows.Forms.Padding(4);
      this.grp3.Size = new System.Drawing.Size(784, 77);
      this.grp3.TabIndex = 2;
      this.grp3.TabStop = false;
      this.grp3.Text = "Guardar diccionario";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(8, 19);
      this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(64, 17);
      this.label4.TabIndex = 0;
      this.label4.Text = "Formato:";
      // 
      // lbPath3
      // 
      this.lbPath3.AutoSize = true;
      this.lbPath3.Location = new System.Drawing.Point(7, 45);
      this.lbPath3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbPath3.Name = "lbPath3";
      this.lbPath3.Size = new System.Drawing.Size(30, 17);
      this.lbPath3.TabIndex = 4;
      this.lbPath3.Text = "Dir:";
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Location = new System.Drawing.Point(688, 40);
      this.btnSave.Margin = new System.Windows.Forms.Padding(4);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(88, 28);
      this.btnSave.TabIndex = 7;
      this.btnSave.Text = "Guardar";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnMergeAll
      // 
      this.btnMergeAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMergeAll.Location = new System.Drawing.Point(167, 73);
      this.btnMergeAll.Margin = new System.Windows.Forms.Padding(4);
      this.btnMergeAll.Name = "btnMergeAll";
      this.btnMergeAll.Size = new System.Drawing.Size(450, 28);
      this.btnMergeAll.TabIndex = 3;
      this.btnMergeAll.Text = "Mezclar todos los diccionarios en el mismo directorio";
      this.btnMergeAll.UseVisualStyleBackColor = true;
      this.btnMergeAll.Click += new System.EventHandler(this.btnMergeAll_Click);
      // 
      // MergeDicts
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(810, 603);
      this.Controls.Add(this.grp3);
      this.Controls.Add(this.grp2);
      this.Controls.Add(this.grp1);
      this.Controls.Add(this.txtMsgBox);
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MergeDicts";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Mescla y/o conversión de diccionarios";
      this.grp1.ResumeLayout(false);
      this.grp1.PerformLayout();
      this.grp2.ResumeLayout(false);
      this.grp2.PerformLayout();
      this.grp3.ResumeLayout(false);
      this.grp3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion
    private System.Windows.Forms.TextBox txtMsgBox;
    private System.Windows.Forms.TextBox txtPathSave;
    private System.Windows.Forms.TextBox txtPathIni;
    private System.Windows.Forms.Button btnPathSave;
    private System.Windows.Forms.Label lbPath1;
    private System.Windows.Forms.Button btnPathIni;
    private System.Windows.Forms.FolderBrowserDialog SelFolderDlg;
    private System.Windows.Forms.Label lbFrm1;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.Label txtFrmIni;
    private System.Windows.Forms.Label txtFrmMezcla;
    private System.Windows.Forms.RadioButton rbXml;
    private System.Windows.Forms.RadioButton rbBinary;
    private System.Windows.Forms.RadioButton rbTexto1;
    private System.Windows.Forms.GroupBox grp1;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.GroupBox grp2;
    private System.Windows.Forms.Label lbPath2;
    private System.Windows.Forms.TextBox txtPathMezcla;
    private System.Windows.Forms.Label lbFrm2;
    private System.Windows.Forms.Button btnPathMezcla;
    private System.Windows.Forms.Button btnMezclar;
    private System.Windows.Forms.GroupBox grp3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lbPath3;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnNuevo;
    private System.Windows.Forms.Button btnMergeAll;
    }
  }