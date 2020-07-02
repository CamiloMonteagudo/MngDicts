namespace Tests
  {
  partial class Main
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
      this.MainMenu = new System.Windows.Forms.MenuStrip();
      this.cbDir = new System.Windows.Forms.ToolStripComboBox();
      this.MnuLbDir = new System.Windows.Forms.ToolStripTextBox();
      this.MnuFichero = new System.Windows.Forms.ToolStripMenuItem();
      this.MnuQuit = new System.Windows.Forms.ToolStripMenuItem();
      this.manejoDeDatosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.MnuMngDiccionarios = new System.Windows.Forms.ToolStripMenuItem();
      this.MnuMergeDiccionarios = new System.Windows.Forms.ToolStripMenuItem();
      this.MnuDelNames = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.MnuEditDict = new System.Windows.Forms.ToolStripMenuItem();
      this.crearDiccionarioDeReducciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.btnTip = new System.Windows.Forms.ToolTip(this.components);
      this.SaveFileDlg = new System.Windows.Forms.SaveFileDialog();
      this.extraerVerbosDeUnDicionarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.MainMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainMenu
      // 
      this.MainMenu.BackColor = System.Drawing.SystemColors.ControlLight;
      this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbDir,
            this.MnuLbDir,
            this.MnuFichero,
            this.manejoDeDatosToolStripMenuItem});
      this.MainMenu.Location = new System.Drawing.Point(0, 0);
      this.MainMenu.Name = "MainMenu";
      this.MainMenu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
      this.MainMenu.Size = new System.Drawing.Size(1172, 32);
      this.MainMenu.TabIndex = 10;
      this.MainMenu.Text = "MainMenu";
      // 
      // cbDir
      // 
      this.cbDir.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.cbDir.AutoSize = false;
      this.cbDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbDir.DropDownWidth = 140;
      this.cbDir.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
      this.cbDir.Items.AddRange(new object[] {
            "Español -> Inglés",
            "Inglés    -> Español",
            "Italiano -> Inglés",
            "Inglés    -> Italiano",
            "Italiano -> Español",
            "Español -> Italiano",
            "Italiano -> Alemán",
            "Alemán -> Italiano",
            "Italiano -> Francés",
            "Francés ->Italiano",
            "Inglés    -> Francés",
            "Francés -> Inglés",
            "Español -> Francés",
            "Francés -> Español"});
      this.cbDir.Margin = new System.Windows.Forms.Padding(3, 0, 15, 0);
      this.cbDir.MaxDropDownItems = 80;
      this.cbDir.Name = "cbDir";
      this.cbDir.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
      this.cbDir.Size = new System.Drawing.Size(0, 28);
      // 
      // MnuLbDir
      // 
      this.MnuLbDir.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.MnuLbDir.BackColor = System.Drawing.SystemColors.ControlLight;
      this.MnuLbDir.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.MnuLbDir.Name = "MnuLbDir";
      this.MnuLbDir.ReadOnly = true;
      this.MnuLbDir.Size = new System.Drawing.Size(165, 28);
      this.MnuLbDir.Text = "Dirección:";
      this.MnuLbDir.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // MnuFichero
      // 
      this.MnuFichero.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuQuit});
      this.MnuFichero.Name = "MnuFichero";
      this.MnuFichero.Size = new System.Drawing.Size(69, 28);
      this.MnuFichero.Text = "Fichero";
      // 
      // MnuQuit
      // 
      this.MnuQuit.Name = "MnuQuit";
      this.MnuQuit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
      this.MnuQuit.Size = new System.Drawing.Size(166, 26);
      this.MnuQuit.Text = "Salir";
      this.MnuQuit.Click += new System.EventHandler(this.MnuQuit_Click);
      // 
      // manejoDeDatosToolStripMenuItem
      // 
      this.manejoDeDatosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuMngDiccionarios,
            this.MnuMergeDiccionarios,
            this.MnuDelNames,
            this.toolStripSeparator5,
            this.MnuEditDict,
            this.crearDiccionarioDeReducciónToolStripMenuItem,
            this.extraerVerbosDeUnDicionarioToolStripMenuItem});
      this.manejoDeDatosToolStripMenuItem.Name = "manejoDeDatosToolStripMenuItem";
      this.manejoDeDatosToolStripMenuItem.Size = new System.Drawing.Size(133, 28);
      this.manejoDeDatosToolStripMenuItem.Text = "Manejo de datos";
      // 
      // MnuMngDiccionarios
      // 
      this.MnuMngDiccionarios.Name = "MnuMngDiccionarios";
      this.MnuMngDiccionarios.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
      this.MnuMngDiccionarios.Size = new System.Drawing.Size(293, 26);
      this.MnuMngDiccionarios.Text = "Manejo de diccionarios";
      this.MnuMngDiccionarios.Click += new System.EventHandler(this.MnuMngDiccionarios_Click);
      // 
      // MnuMergeDiccionarios
      // 
      this.MnuMergeDiccionarios.Name = "MnuMergeDiccionarios";
      this.MnuMergeDiccionarios.Size = new System.Drawing.Size(293, 26);
      this.MnuMergeDiccionarios.Text = "Mesclar/Convertir diccionarios";
      this.MnuMergeDiccionarios.Click += new System.EventHandler(this.MnuMergeDiccionarios_Click);
      // 
      // MnuDelNames
      // 
      this.MnuDelNames.Name = "MnuDelNames";
      this.MnuDelNames.Size = new System.Drawing.Size(293, 26);
      this.MnuDelNames.Text = "Analizar Nombres";
      this.MnuDelNames.Click += new System.EventHandler(this.MnuDelNames_Click);
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(290, 6);
      // 
      // MnuEditDict
      // 
      this.MnuEditDict.Name = "MnuEditDict";
      this.MnuEditDict.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
      this.MnuEditDict.Size = new System.Drawing.Size(293, 26);
      this.MnuEditDict.Text = "Editar diccionario";
      this.MnuEditDict.Click += new System.EventHandler(this.MnuEditDict_Click);
      // 
      // crearDiccionarioDeReducciónToolStripMenuItem
      // 
      this.crearDiccionarioDeReducciónToolStripMenuItem.Name = "crearDiccionarioDeReducciónToolStripMenuItem";
      this.crearDiccionarioDeReducciónToolStripMenuItem.Size = new System.Drawing.Size(293, 26);
      this.crearDiccionarioDeReducciónToolStripMenuItem.Text = "Crear diccionario de reducción";
      this.crearDiccionarioDeReducciónToolStripMenuItem.Click += new System.EventHandler(this.crearDiccionarioDeReducciónToolStripMenuItem_Click);
      // 
      // extraerVerbosDeUnDicionarioToolStripMenuItem
      // 
      this.extraerVerbosDeUnDicionarioToolStripMenuItem.Name = "extraerVerbosDeUnDicionarioToolStripMenuItem";
      this.extraerVerbosDeUnDicionarioToolStripMenuItem.Size = new System.Drawing.Size(293, 26);
      this.extraerVerbosDeUnDicionarioToolStripMenuItem.Text = "Extraer Verbos de un Dicionario";
      this.extraerVerbosDeUnDicionarioToolStripMenuItem.Click += new System.EventHandler(this.extraerVerbosDeUnDicionarioToolStripMenuItem_Click);
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1172, 513);
      this.Controls.Add(this.MainMenu);
      this.KeyPreview = true;
      this.MainMenuStrip = this.MainMenu;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "Main";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Pruebas de traducción";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.MainMenu.ResumeLayout(false);
      this.MainMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion
    private System.Windows.Forms.MenuStrip MainMenu;
    private System.Windows.Forms.ToolStripMenuItem manejoDeDatosToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem MnuMngDiccionarios;
    private System.Windows.Forms.ToolStripComboBox cbDir;
    private System.Windows.Forms.ToolStripTextBox MnuLbDir;
    private System.Windows.Forms.ToolStripMenuItem MnuEditDict;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    private System.Windows.Forms.ToolStripMenuItem MnuFichero;
    private System.Windows.Forms.ToolStripMenuItem MnuQuit;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolTip btnTip;
    private System.Windows.Forms.SaveFileDialog SaveFileDlg;
    private System.Windows.Forms.ToolStripMenuItem MnuMergeDiccionarios;
    private System.Windows.Forms.ToolStripMenuItem MnuDelNames;
    private System.Windows.Forms.ToolStripMenuItem crearDiccionarioDeReducciónToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem extraerVerbosDeUnDicionarioToolStripMenuItem;
    }
  }

