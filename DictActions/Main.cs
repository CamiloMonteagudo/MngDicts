using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TrdEngine;
using TrdEngine.Data;
using System.Collections.Generic;
using Tests.Properties;
using System.Text;
using System.Threading.Tasks;

namespace Tests
  {
  public partial class Main : Form
    {
    //------------------------------------------------------------------------------------------------------------------
    public Main()
      {
      InitializeComponent();

      if( Settings.Default.wFrm > 0 ) 
        Width = Settings.Default.wFrm;

      if( Settings.Default.hFrm > 0 )
        Height = Settings.Default.hFrm;

      cbDir.Width = 140;
      }

    //------------------------------------------------------------------------------------------------------------------
    private void Form1_Load(object sender, EventArgs e)
      {
      TrdData.DicsPath = GetDictPath();                               // Obtiene le camino a los diccionarios
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene y establece el directorio de los diccionario
    private string GetDictPath()
      {
      string DicsDir = Settings.Default.DictsDirectory;

      if( Path.IsPathRooted( DicsDir ) ) return DicsDir;

      var AppDir = Assembly.GetExecutingAssembly().Location;           // Obtiene la localización del ensamblado

      AppDir = Path.GetDirectoryName(AppDir);                           // Quita el nombre de la aplicación
      AppDir = Path.GetDirectoryName(AppDir);                           // Quita el ultimo directorio

      if( string.IsNullOrEmpty(DicsDir) ) DicsDir = @"\Dictionaries\Binarios";

      DicsDir = AppDir + DicsDir;
      Settings.Default.DictsDirectory = DicsDir;
      return DicsDir;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Muestra ventana para trabajar con los ficheros de diccionarios
    private void MnuMngDiccionarios_Click(object sender, EventArgs e)
      {
      var frm = new MngDicts();
      frm.ShowDialog(this);
      frm.Dispose();
      }

    //------------------------------------------------------------------------------------------------------------------
    // Llama el dialogo para la ediccioón de diccionario
    private void MnuEditDict_Click(object sender, EventArgs e)
      {
      var frm = new EditDict();
      frm.ShowDialog();
      frm.Dispose();
      }
    
    //------------------------------------------------------------------------------------------------------------------
    // Atiene el boton Quit del menu y cierra la aplicación
    private void MnuQuit_Click(object sender, EventArgs e)
      {
      Close();
      }

    //------------------------------------------------------------------------------------------------------------------
    // Muestra el dialogo para mescar diccionarios
    private void MnuMergeDiccionarios_Click(object sender, EventArgs e)
      {
      var frm = new MergeDicts();
      frm.ShowDialog(this);
      frm.Dispose();
      }

    private void MnuDelNames_Click( object sender, EventArgs e )
      {
      var frm = new DelNames();
      frm.ShowDialog(this);
      frm.Dispose();
      }

    private void crearDiccionarioDeReducciónToolStripMenuItem_Click( object sender, EventArgs e )
      {
      var frm = new DictRed();
      frm.ShowDialog(this);
      frm.Dispose();
      }

    private void extraerVerbosDeUnDicionarioToolStripMenuItem_Click( object sender, EventArgs e )
      {
      var frm = new FindVerbs();
      frm.ShowDialog(this);
      frm.Dispose();
      }

    //------------------------------------------------------------------------------------------------------------------
    }

  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE Tests     +++++++++++++++++++++++++++++++++++++++++++++
