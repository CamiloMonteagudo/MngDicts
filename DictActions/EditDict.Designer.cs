namespace Tests
  {
  partial class EditDict
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
      this.txtDictPath = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.btnGetDict = new System.Windows.Forms.Button();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.btnLoad = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txtFindWrd = new System.Windows.Forms.TextBox();
      this.btnFind = new System.Windows.Forms.Button();
      this.lstWords = new System.Windows.Forms.ListView();
      this.colWords = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.txtTime = new System.Windows.Forms.TextBox();
      this.txtData = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // txtDictPath
      // 
      this.txtDictPath.Location = new System.Drawing.Point(141, 15);
      this.txtDictPath.Margin = new System.Windows.Forms.Padding(4);
      this.txtDictPath.Name = "txtDictPath";
      this.txtDictPath.Size = new System.Drawing.Size(669, 22);
      this.txtDictPath.TabIndex = 1;
      this.txtDictPath.Text = "C:\\_Mis_Cosas\\1___Proyectos\\TrdEngineNet\\Dictionaries\\Binarios\\Es2EnGen.dcb";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(16, 20);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(126, 17);
      this.label9.TabIndex = 0;
      this.label9.Text = "Directorio a Editar:";
      // 
      // btnGetDict
      // 
      this.btnGetDict.Location = new System.Drawing.Point(820, 12);
      this.btnGetDict.Margin = new System.Windows.Forms.Padding(4);
      this.btnGetDict.Name = "btnGetDict";
      this.btnGetDict.Size = new System.Drawing.Size(33, 28);
      this.btnGetDict.TabIndex = 2;
      this.btnGetDict.Text = "...";
      this.btnGetDict.UseVisualStyleBackColor = true;
      this.btnGetDict.Click += new System.EventHandler(this.btnGetDict_Click);
      // 
      // btnLoad
      // 
      this.btnLoad.Location = new System.Drawing.Point(861, 11);
      this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(100, 28);
      this.btnLoad.TabIndex = 3;
      this.btnLoad.Text = "Cargar";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 52);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(120, 17);
      this.label1.TabIndex = 4;
      this.label1.Text = "Palabra a buscar:";
      // 
      // txtFindWrd
      // 
      this.txtFindWrd.Location = new System.Drawing.Point(141, 47);
      this.txtFindWrd.Margin = new System.Windows.Forms.Padding(4);
      this.txtFindWrd.Name = "txtFindWrd";
      this.txtFindWrd.Size = new System.Drawing.Size(329, 22);
      this.txtFindWrd.TabIndex = 5;
      this.txtFindWrd.TextChanged += new System.EventHandler(this.txtFindWrd_TextChanged);
      // 
      // btnFind
      // 
      this.btnFind.Location = new System.Drawing.Point(480, 46);
      this.btnFind.Margin = new System.Windows.Forms.Padding(4);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(100, 28);
      this.btnFind.TabIndex = 6;
      this.btnFind.Text = "Buscar";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // lstWords
      // 
      this.lstWords.Activation = System.Windows.Forms.ItemActivation.OneClick;
      this.lstWords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lstWords.BackColor = System.Drawing.SystemColors.Window;
      this.lstWords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lstWords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWords});
      this.lstWords.FullRowSelect = true;
      this.lstWords.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.lstWords.HideSelection = false;
      this.lstWords.Location = new System.Drawing.Point(11, 81);
      this.lstWords.Margin = new System.Windows.Forms.Padding(4);
      this.lstWords.MultiSelect = false;
      this.lstWords.Name = "lstWords";
      this.lstWords.Size = new System.Drawing.Size(610, 521);
      this.lstWords.TabIndex = 8;
      this.lstWords.UseCompatibleStateImageBehavior = false;
      this.lstWords.View = System.Windows.Forms.View.Details;
      this.lstWords.VirtualMode = true;
      this.lstWords.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lstWords_RetrieveVirtualItem);
      this.lstWords.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.lstWords_SearchForVirtualItem);
      this.lstWords.SelectedIndexChanged += new System.EventHandler(this.lstWords_SelectedIndexChanged);
      // 
      // colWords
      // 
      this.colWords.Text = "";
      this.colWords.Width = 610;
      // 
      // txtTime
      // 
      this.txtTime.Location = new System.Drawing.Point(604, 46);
      this.txtTime.Margin = new System.Windows.Forms.Padding(4);
      this.txtTime.Name = "txtTime";
      this.txtTime.Size = new System.Drawing.Size(207, 22);
      this.txtTime.TabIndex = 7;
      // 
      // txtData
      // 
      this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtData.Location = new System.Drawing.Point(629, 82);
      this.txtData.Margin = new System.Windows.Forms.Padding(4);
      this.txtData.Multiline = true;
      this.txtData.Name = "txtData";
      this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtData.Size = new System.Drawing.Size(438, 520);
      this.txtData.TabIndex = 9;
      // 
      // EditDict
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1080, 649);
      this.Controls.Add(this.txtData);
      this.Controls.Add(this.lstWords);
      this.Controls.Add(this.btnFind);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.txtTime);
      this.Controls.Add(this.txtFindWrd);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtDictPath);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.btnGetDict);
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MinimizeBox = false;
      this.Name = "EditDict";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Editor de diccionarios";
      this.Load += new System.EventHandler(this.EditDict_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion

    private System.Windows.Forms.TextBox txtDictPath;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Button btnGetDict;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtFindWrd;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.ListView lstWords;
    private System.Windows.Forms.ColumnHeader colWords;
    private System.Windows.Forms.TextBox txtTime;
    private System.Windows.Forms.TextBox txtData;
    }
  }