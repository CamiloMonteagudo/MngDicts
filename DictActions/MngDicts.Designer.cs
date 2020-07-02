namespace Tests
  {
  partial class MngDicts
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
      this.components = new System.ComponentModel.Container();
      this.txtDirsPath = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.btnGetDirData = new System.Windows.Forms.Button();
      this.btnSetDirData = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.txtSavePath = new System.Windows.Forms.TextBox();
      this.SelFolderDlg = new System.Windows.Forms.FolderBrowserDialog();
      this.txtMsgBox = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txtName = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.btnAddDict1 = new System.Windows.Forms.Button();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnAll = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.lstIdDirsDicts = new System.Windows.Forms.CheckedListBox();
      this.label2 = new System.Windows.Forms.Label();
      this.btnDicOrder = new System.Windows.Forms.Button();
      this.btnChgDir = new System.Windows.Forms.Button();
      this.btnMenosDir = new System.Windows.Forms.Button();
      this.btnMasDir = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.btnDefault = new System.Windows.Forms.Button();
      this.bntWordIndex = new System.Windows.Forms.Button();
      this.btnSplitSenteces = new System.Windows.Forms.Button();
      this.btnSamesKeys = new System.Windows.Forms.Button();
      this.btnNormalizeAll = new System.Windows.Forms.Button();
      this.btnExpandMark = new System.Windows.Forms.Button();
      this.btnInvertDicc = new System.Windows.Forms.Button();
      this.btnAddKeys = new System.Windows.Forms.Button();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.btnNormalizaSimple = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtDirsPath
      // 
      this.txtDirsPath.Location = new System.Drawing.Point(168, 6);
      this.txtDirsPath.Margin = new System.Windows.Forms.Padding(4);
      this.txtDirsPath.Name = "txtDirsPath";
      this.txtDirsPath.Size = new System.Drawing.Size(512, 22);
      this.txtDirsPath.TabIndex = 5;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(16, 11);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(134, 17);
      this.label9.TabIndex = 4;
      this.label9.Text = "Directorio para leer:";
      // 
      // btnGetDirData
      // 
      this.btnGetDirData.Location = new System.Drawing.Point(763, 4);
      this.btnGetDirData.Margin = new System.Windows.Forms.Padding(4);
      this.btnGetDirData.Name = "btnGetDirData";
      this.btnGetDirData.Size = new System.Drawing.Size(33, 28);
      this.btnGetDirData.TabIndex = 6;
      this.btnGetDirData.Text = "...";
      this.toolTip1.SetToolTip(this.btnGetDirData, "Selección del directorio para leer los diccionarios");
      this.btnGetDirData.UseVisualStyleBackColor = true;
      this.btnGetDirData.Click += new System.EventHandler(this.btnGetDirData_Click);
      // 
      // btnSetDirData
      // 
      this.btnSetDirData.Location = new System.Drawing.Point(685, 36);
      this.btnSetDirData.Margin = new System.Windows.Forms.Padding(4);
      this.btnSetDirData.Name = "btnSetDirData";
      this.btnSetDirData.Size = new System.Drawing.Size(33, 28);
      this.btnSetDirData.TabIndex = 6;
      this.btnSetDirData.Text = "...";
      this.toolTip1.SetToolTip(this.btnSetDirData, "Selecciona directorio para guardar diccionarios procesados");
      this.btnSetDirData.UseVisualStyleBackColor = true;
      this.btnSetDirData.Click += new System.EventHandler(this.btnSetDirData_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 43);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(157, 17);
      this.label3.TabIndex = 4;
      this.label3.Text = "Directorio para Escribir:";
      // 
      // txtSavePath
      // 
      this.txtSavePath.Location = new System.Drawing.Point(168, 38);
      this.txtSavePath.Margin = new System.Windows.Forms.Padding(4);
      this.txtSavePath.Name = "txtSavePath";
      this.txtSavePath.Size = new System.Drawing.Size(512, 22);
      this.txtSavePath.TabIndex = 5;
      // 
      // txtMsgBox
      // 
      this.txtMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMsgBox.Location = new System.Drawing.Point(8, 438);
      this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
      this.txtMsgBox.Multiline = true;
      this.txtMsgBox.Name = "txtMsgBox";
      this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtMsgBox.Size = new System.Drawing.Size(800, 272);
      this.txtMsgBox.TabIndex = 8;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.txtName);
      this.groupBox1.Controls.Add(this.button2);
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.btnAddDict1);
      this.groupBox1.Controls.Add(this.btnDel);
      this.groupBox1.Controls.Add(this.btnAll);
      this.groupBox1.Controls.Add(this.btnAdd);
      this.groupBox1.Controls.Add(this.btnClear);
      this.groupBox1.Controls.Add(this.lstIdDirsDicts);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Location = new System.Drawing.Point(8, 70);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
      this.groupBox1.Size = new System.Drawing.Size(486, 361);
      this.groupBox1.TabIndex = 9;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Diccionarios";
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(8, 326);
      this.txtName.Margin = new System.Windows.Forms.Padding(4);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(249, 22);
      this.txtName.TabIndex = 6;
      this.txtName.Text = "En2Es.xml";
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(13, 247);
      this.button2.Margin = new System.Windows.Forms.Padding(4);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(104, 28);
      this.button2.TabIndex = 5;
      this.button2.Text = "*.txt";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.btnAddDic_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(13, 211);
      this.button1.Margin = new System.Windows.Forms.Padding(4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(104, 28);
      this.button1.TabIndex = 5;
      this.button1.Text = "*_ord.dcv";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.btnAddDic_Click);
      // 
      // btnAddDict1
      // 
      this.btnAddDict1.Location = new System.Drawing.Point(13, 175);
      this.btnAddDict1.Margin = new System.Windows.Forms.Padding(4);
      this.btnAddDict1.Name = "btnAddDict1";
      this.btnAddDict1.Size = new System.Drawing.Size(104, 28);
      this.btnAddDict1.TabIndex = 5;
      this.btnAddDict1.Text = "*.dcv";
      this.toolTip1.SetToolTip(this.btnAddDict1, "Agrega a la lista todos los ficheros con extesion dic");
      this.btnAddDict1.UseVisualStyleBackColor = true;
      this.btnAddDict1.Click += new System.EventHandler(this.btnAddDic_Click);
      // 
      // btnDel
      // 
      this.btnDel.Location = new System.Drawing.Point(13, 106);
      this.btnDel.Margin = new System.Windows.Forms.Padding(4);
      this.btnDel.Name = "btnDel";
      this.btnDel.Size = new System.Drawing.Size(104, 28);
      this.btnDel.TabIndex = 5;
      this.btnDel.Text = "Borrar";
      this.toolTip1.SetToolTip(this.btnDel, "Quita de la lista todos los diccionarios marcados");
      this.btnDel.UseVisualStyleBackColor = true;
      this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
      // 
      // btnAll
      // 
      this.btnAll.Location = new System.Drawing.Point(13, 21);
      this.btnAll.Margin = new System.Windows.Forms.Padding(4);
      this.btnAll.Name = "btnAll";
      this.btnAll.Size = new System.Drawing.Size(104, 28);
      this.btnAll.TabIndex = 5;
      this.btnAll.Text = "Check All";
      this.toolTip1.SetToolTip(this.btnAll, "Marca todos los ficheros para procesar o borrar");
      this.btnAll.UseVisualStyleBackColor = true;
      this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(263, 326);
      this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(85, 28);
      this.btnAdd.TabIndex = 5;
      this.btnAdd.Text = "Adicionar";
      this.toolTip1.SetToolTip(this.btnAdd, "Adiciona a la lista todos los diccionarios que cumplan el patron");
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnClear
      // 
      this.btnClear.Location = new System.Drawing.Point(13, 52);
      this.btnClear.Margin = new System.Windows.Forms.Padding(4);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(104, 28);
      this.btnClear.TabIndex = 5;
      this.btnClear.Text = "Uncheck All";
      this.toolTip1.SetToolTip(this.btnClear, "Quita la maraca a todos los diccionarios de la lista");
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // lstIdDirsDicts
      // 
      this.lstIdDirsDicts.CheckOnClick = true;
      this.lstIdDirsDicts.FormattingEnabled = true;
      this.lstIdDirsDicts.Location = new System.Drawing.Point(127, 21);
      this.lstIdDirsDicts.Margin = new System.Windows.Forms.Padding(4);
      this.lstIdDirsDicts.Name = "lstIdDirsDicts";
      this.lstIdDirsDicts.Size = new System.Drawing.Size(351, 276);
      this.lstIdDirsDicts.TabIndex = 4;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 308);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(254, 17);
      this.label2.TabIndex = 2;
      this.label2.Text = "Patron de los nombres de diccionarios:";
      // 
      // btnDicOrder
      // 
      this.btnDicOrder.Location = new System.Drawing.Point(502, 186);
      this.btnDicOrder.Margin = new System.Windows.Forms.Padding(4);
      this.btnDicOrder.Name = "btnDicOrder";
      this.btnDicOrder.Size = new System.Drawing.Size(175, 28);
      this.btnDicOrder.TabIndex = 12;
      this.btnDicOrder.Text = "Ordenar diccionarios";
      this.btnDicOrder.UseVisualStyleBackColor = true;
      this.btnDicOrder.Click += new System.EventHandler(this.btnDicOrder_Click);
      // 
      // btnChgDir
      // 
      this.btnChgDir.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btnChgDir.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
      this.btnChgDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnChgDir.Location = new System.Drawing.Point(679, 5);
      this.btnChgDir.Margin = new System.Windows.Forms.Padding(4);
      this.btnChgDir.Name = "btnChgDir";
      this.btnChgDir.Size = new System.Drawing.Size(27, 27);
      this.btnChgDir.TabIndex = 6;
      this.btnChgDir.Text = "˅";
      this.toolTip1.SetToolTip(this.btnChgDir, "Recicla directorios al mismo nivel");
      this.btnChgDir.UseVisualStyleBackColor = true;
      this.btnChgDir.Click += new System.EventHandler(this.btnChgDir_Click);
      // 
      // btnMenosDir
      // 
      this.btnMenosDir.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btnMenosDir.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
      this.btnMenosDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMenosDir.Location = new System.Drawing.Point(704, 5);
      this.btnMenosDir.Margin = new System.Windows.Forms.Padding(4);
      this.btnMenosDir.Name = "btnMenosDir";
      this.btnMenosDir.Size = new System.Drawing.Size(27, 27);
      this.btnMenosDir.TabIndex = 6;
      this.btnMenosDir.Text = "-";
      this.toolTip1.SetToolTip(this.btnMenosDir, "Decremente nivel de directorio");
      this.btnMenosDir.UseVisualStyleBackColor = true;
      this.btnMenosDir.Click += new System.EventHandler(this.btnMenosDir_Click);
      // 
      // btnMasDir
      // 
      this.btnMasDir.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btnMasDir.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
      this.btnMasDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMasDir.Location = new System.Drawing.Point(729, 5);
      this.btnMasDir.Margin = new System.Windows.Forms.Padding(0);
      this.btnMasDir.Name = "btnMasDir";
      this.btnMasDir.Size = new System.Drawing.Size(27, 27);
      this.btnMasDir.TabIndex = 6;
      this.btnMasDir.Text = "+";
      this.toolTip1.SetToolTip(this.btnMasDir, "Incrementa nivel de direcctorio");
      this.btnMasDir.UseVisualStyleBackColor = true;
      this.btnMasDir.Click += new System.EventHandler(this.btnMasDir_Click);
      // 
      // btnDefault
      // 
      this.btnDefault.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btnDefault.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
      this.btnDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDefault.Location = new System.Drawing.Point(723, 37);
      this.btnDefault.Margin = new System.Windows.Forms.Padding(0);
      this.btnDefault.Name = "btnDefault";
      this.btnDefault.Size = new System.Drawing.Size(27, 27);
      this.btnDefault.TabIndex = 6;
      this.btnDefault.Text = "*";
      this.toolTip1.SetToolTip(this.btnDefault, "Pone como direcctorio de salida el que se esta utilizando por la maquinaria de tr" +
        "aducción");
      this.btnDefault.UseVisualStyleBackColor = true;
      this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
      // 
      // bntWordIndex
      // 
      this.bntWordIndex.Location = new System.Drawing.Point(502, 339);
      this.bntWordIndex.Margin = new System.Windows.Forms.Padding(4);
      this.bntWordIndex.Name = "bntWordIndex";
      this.bntWordIndex.Size = new System.Drawing.Size(175, 28);
      this.bntWordIndex.TabIndex = 12;
      this.bntWordIndex.Text = "Indice de palabras";
      this.bntWordIndex.UseVisualStyleBackColor = true;
      this.bntWordIndex.Click += new System.EventHandler(this.bntWordIndex_Click);
      // 
      // btnSplitSenteces
      // 
      this.btnSplitSenteces.Location = new System.Drawing.Point(684, 72);
      this.btnSplitSenteces.Margin = new System.Windows.Forms.Padding(4);
      this.btnSplitSenteces.Name = "btnSplitSenteces";
      this.btnSplitSenteces.Size = new System.Drawing.Size(122, 28);
      this.btnSplitSenteces.TabIndex = 12;
      this.btnSplitSenteces.Text = "Divide Senteces";
      this.btnSplitSenteces.UseVisualStyleBackColor = true;
      this.btnSplitSenteces.Click += new System.EventHandler(this.btnSplitSenteces_Click);
      // 
      // btnSamesKeys
      // 
      this.btnSamesKeys.Location = new System.Drawing.Point(502, 222);
      this.btnSamesKeys.Margin = new System.Windows.Forms.Padding(4);
      this.btnSamesKeys.Name = "btnSamesKeys";
      this.btnSamesKeys.Size = new System.Drawing.Size(175, 28);
      this.btnSamesKeys.TabIndex = 12;
      this.btnSamesKeys.Text = "Llaves Iguales";
      this.btnSamesKeys.UseVisualStyleBackColor = true;
      this.btnSamesKeys.Click += new System.EventHandler(this.btnSamesKeys_Click);
      // 
      // btnNormalizeAll
      // 
      this.btnNormalizeAll.Location = new System.Drawing.Point(502, 150);
      this.btnNormalizeAll.Margin = new System.Windows.Forms.Padding(4);
      this.btnNormalizeAll.Name = "btnNormalizeAll";
      this.btnNormalizeAll.Size = new System.Drawing.Size(175, 28);
      this.btnNormalizeAll.TabIndex = 12;
      this.btnNormalizeAll.Text = "Normalizar datos";
      this.btnNormalizeAll.UseVisualStyleBackColor = true;
      this.btnNormalizeAll.Click += new System.EventHandler(this.btnNormalizeAll_Click);
      // 
      // btnExpandMark
      // 
      this.btnExpandMark.Location = new System.Drawing.Point(502, 294);
      this.btnExpandMark.Margin = new System.Windows.Forms.Padding(4);
      this.btnExpandMark.Name = "btnExpandMark";
      this.btnExpandMark.Size = new System.Drawing.Size(175, 28);
      this.btnExpandMark.TabIndex = 12;
      this.btnExpandMark.Text = "Expande Marcadores";
      this.btnExpandMark.UseVisualStyleBackColor = true;
      this.btnExpandMark.Click += new System.EventHandler(this.btnExpandMark_Click);
      // 
      // btnInvertDicc
      // 
      this.btnInvertDicc.Location = new System.Drawing.Point(684, 223);
      this.btnInvertDicc.Margin = new System.Windows.Forms.Padding(4);
      this.btnInvertDicc.Name = "btnInvertDicc";
      this.btnInvertDicc.Size = new System.Drawing.Size(124, 28);
      this.btnInvertDicc.TabIndex = 12;
      this.btnInvertDicc.Text = "Invertir Dic.";
      this.btnInvertDicc.UseVisualStyleBackColor = true;
      this.btnInvertDicc.Click += new System.EventHandler(this.btnInvertDicc_Click);
      // 
      // btnAddKeys
      // 
      this.btnAddKeys.Location = new System.Drawing.Point(502, 258);
      this.btnAddKeys.Margin = new System.Windows.Forms.Padding(4);
      this.btnAddKeys.Name = "btnAddKeys";
      this.btnAddKeys.Size = new System.Drawing.Size(175, 28);
      this.btnAddKeys.TabIndex = 12;
      this.btnAddKeys.Text = "Adicionar Llaves";
      this.btnAddKeys.UseVisualStyleBackColor = true;
      this.btnAddKeys.Click += new System.EventHandler(this.btnAddKeys_Click);
      // 
      // btnNormalizaSimple
      // 
      this.btnNormalizaSimple.Location = new System.Drawing.Point(502, 72);
      this.btnNormalizaSimple.Margin = new System.Windows.Forms.Padding(4);
      this.btnNormalizaSimple.Name = "btnNormalizaSimple";
      this.btnNormalizaSimple.Size = new System.Drawing.Size(175, 28);
      this.btnNormalizaSimple.TabIndex = 12;
      this.btnNormalizaSimple.Text = "Normaliza Simple";
      this.btnNormalizaSimple.UseVisualStyleBackColor = true;
      this.btnNormalizaSimple.Click += new System.EventHandler(this.btnNormalizaSimple_Click);
      // 
      // MngDicts
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(815, 716);
      this.Controls.Add(this.btnDefault);
      this.Controls.Add(this.btnMasDir);
      this.Controls.Add(this.btnMenosDir);
      this.Controls.Add(this.btnChgDir);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btnInvertDicc);
      this.Controls.Add(this.bntWordIndex);
      this.Controls.Add(this.btnSplitSenteces);
      this.Controls.Add(this.btnNormalizaSimple);
      this.Controls.Add(this.btnNormalizeAll);
      this.Controls.Add(this.btnExpandMark);
      this.Controls.Add(this.btnAddKeys);
      this.Controls.Add(this.btnSamesKeys);
      this.Controls.Add(this.btnDicOrder);
      this.Controls.Add(this.txtMsgBox);
      this.Controls.Add(this.txtSavePath);
      this.Controls.Add(this.txtDirsPath);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.btnSetDirData);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.btnGetDirData);
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MngDicts";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Manejo de diccionarios";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MngDicts_FormClosing);
      this.Load += new System.EventHandler(this.MngDicts_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion

    private System.Windows.Forms.TextBox txtDirsPath;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Button btnGetDirData;
    private System.Windows.Forms.Button btnSetDirData;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtSavePath;
    private System.Windows.Forms.FolderBrowserDialog SelFolderDlg;
    private System.Windows.Forms.TextBox txtMsgBox;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Button btnAddDict1;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnAll;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.CheckedListBox lstIdDirsDicts;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnDicOrder;
    private System.Windows.Forms.Button btnChgDir;
    private System.Windows.Forms.Button btnMenosDir;
    private System.Windows.Forms.Button btnMasDir;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button btnDefault;
    private System.Windows.Forms.Button bntWordIndex;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button btnSplitSenteces;
    private System.Windows.Forms.Button btnSamesKeys;
    private System.Windows.Forms.Button btnNormalizeAll;
    private System.Windows.Forms.Button btnExpandMark;
    private System.Windows.Forms.Button btnInvertDicc;
    private System.Windows.Forms.Button btnAddKeys;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.Button btnNormalizaSimple;
    }
  }