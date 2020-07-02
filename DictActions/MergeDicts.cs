using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tests;
using TrdEngine;
using TrdEngine.Data;
using TrdEngine.Dictionary;

namespace Tests
  {
  // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public partial class MergeDicts : Form  
    {
    string[] FrmId  = { "txtdic", "dcb", "xml", "dcv" };
    string[] FrmDes = { "Cada llave en una línea, con un separador entre las llaves y los datos","Binario para maquinaria Net framework","Xml con representación de todos los datos","Datos en texto similar a como se muestran"};

    int FrmDic1 = -1;
    int FrmDic2 = -1;

    DictSingle Dic = null;
    public MergeDicts()
      {
      InitializeComponent();

      Dbg.FunMsg = SetMessage;
      SelFileDlg.FilterIndex = 4;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para seleccionar el diccionario inicial</summary>
    private void btnPathIni_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title  = "Seleccione el diccionario al que se le van a adicionar llaves";
      SelFileDlg.Filter = "Diccionario por línea de texto (*.txtdic)|*.txtdic|Diccionario binario (*.dcb)|*.dcb|Diccionario completo en Xml (*.xml)|*.xml|Diccionario con datos para mostrar (*.dcv)|*.dcv";

      if( SelFileDlg.ShowDialog(this) == DialogResult.OK )
        {
        txtPathIni.Text = SelFileDlg.FileName;

        FrmDic1 = SelFileDlg.FilterIndex-1;

        txtFrmIni.Text = FrmDes[FrmDic1] + " (" + FrmId[FrmDic1] + ")";
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para seleccionar el diccionario a mezclar</summary>
    private void btnPathMezcla_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title = "Seleccione el diccionario al que se le van a adicionar llaves";
      SelFileDlg.Filter = "Diccionario por línea de texto (*.txtdic)|*.txtdic|Diccionario binario (*.dcb)|*.dcb|Diccionario completo en Xml (*.xml)|*.xml|Diccionario con datos para mostrar (*.dcv)|*.dcv";

      if( SelFileDlg.ShowDialog(this) == DialogResult.OK )
        {
        txtPathMezcla.Text = SelFileDlg.FileName;

        FrmDic2 = SelFileDlg.FilterIndex-1;

        txtFrmMezcla.Text = FrmDes[FrmDic2] + " (" + FrmId[FrmDic2] + ")";
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atienede el boton para seleccionar el directorio de salva</summary>
    private void btnPathSave_Click( object sender, EventArgs e )
      {
      SelFolderDlg.SelectedPath = txtPathIni.Text;
      SelFolderDlg.Description = "Seleccione el directorio donde se van a guardar los diccionario";

      if (SelFolderDlg.ShowDialog() == DialogResult.OK)
        txtPathSave.Text = SelFolderDlg.SelectedPath;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga el diccionario inicial en memoria</summary>
    private void btnLoad_Click( object sender, EventArgs e )
      {
      if( !IniWait() ) return;

      txtMsgBox.Clear();
      int tm = LoadMainDict( txtPathIni.Text, FrmDic1 );
      if( tm== 0 ) return;

      SetMessage( "Tiempo total de carga: " + tm + " mseg" );

      EndWait();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga el diccionario principal a memoria</summary>
    private int LoadMainDict( string fullName, int tipo )
      {
      try
        {
        int ini = Environment.TickCount;

        add=0; mzc=0; ign=0;
        SetMessage( "Cargando: " + fullName );
        Dic = GetDiccFrom( fullName, tipo);

        return  Environment.TickCount-ini;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL CARAR EL DICCIONARIO:\r\n*** " + ex.Message );
        return 0;
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un diccionario nuevo y vacio</summary>
    private void btnNuevo_Click( object sender, EventArgs e )
      {
      txtMsgBox.Clear();
      Dic = new DictSingle(TLng.En, TLng.Es, DDataType.Word);
      SetMessage( "UN DICCIONARIO VACIO HA SIDO CREADO\r\n" );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla el diccionario indicado con el que esta en memoria</summary>
    private void btnMezclar_Click( object sender, EventArgs e )
      {
      if( !IniWait() ) return;

      int tm = LoadAndMergeDict( txtPathMezcla.Text, FrmDic2 );
      if( tm== 0 ) return;

      SetMessage( "Tiempo total de carga: " + tm + " mseg" );

      EndWait();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla el diccionario indicado con el que esta en memoria</summary>
    private int LoadAndMergeDict( string DictName, int tipo )
      {
      try
        {
        int ini = Environment.TickCount;

        add=0; mzc=0; ign=0;
        SetMessage( "\r\nCargando: " + DictName );
        DictSingle dic2 = GetDiccFrom( DictName, tipo );

        MezclaDicts( dic2 );

        return Environment.TickCount-ini;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR MEZCLANDO LOS DICCIONARIOS:\r\n*** " + ex.Message );
        return 0;
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Guarda el diccionario que se encuentra en memoria</summary>
    private void btnSave_Click( object sender, EventArgs e )
      {
      var NewDictName =  getDestName();
      if( NewDictName == null ) return;

      var MainDict = txtPathIni.Text;
      if( string.Compare(MainDict, NewDictName, true) == 0 )
        {
        SetMessage( "\r\nNo se puede sobrescribir el directorio principal");
        }
      
      if( !IniWait() ) return;

      try
        {
        int ini = Environment.TickCount;

        SetMessage( "\r\nGuardando: " + NewDictName );

        SaveDic( Dic, NewDictName );

        int end = Environment.TickCount;
        SetMessage( "Tiempo total de guardado: " + (end-ini).ToString() + " mseg" );
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR GUARDANDO EL DICCIONARIO:\r\n*** " + ex.Message );
        }

      EndWait();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicia el modo de espera, si ya esta en espera retorna false</summary>
    private bool IniWait()
      {
      if( Application.UseWaitCursor )
        {
        MessageBox.Show("La aplicación ya esta ocupada");
        return false;
        }

      Application.UseWaitCursor = true;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Finaliza el modo de espera</summary>
    private void EndWait()
      {
      Application.UseWaitCursor = false;
      Cursor.Current = Cursors.Default;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Guarda los datos del diccionario, según el formato actual </summary>
    private void SaveDic( DictSingle dic, string DictName )
      {
           if( rbTexto1.Checked ) SaveAsDView( dic, DictName );
      else if( rbBinary.Checked ) dic.Save( DictName, true );
      else if( rbXml.Checked    ) dic.SaveXml( DictName );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Guarda los datos del diccionario, en el formato de llave y datos en una linea</summary>
    private void SaveAsDView( DictSingle dic, string DictName )
      {
      var txt = new StringBuilder( 80*dic.Count);
      foreach( var sKey in dic )
        {
        var data = dic.GetWordData(sKey);
        var text = data.ToDView( sKey );

        txt.AppendLine( text );
        }

      File.WriteAllText(DictName, txt.ToString());  
      SetMessage( "Diccionario guardado ...." );
      }

    // Numero de llaves adicionadas, mezcladas o ignoradas
    int add=0, mzc=0, ign=0;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla el diccionario dado, con el diccionario actual</summary>
    private bool MezclaDicts( DictSingle dic2 )
      {
      if( Dic==null )
        {
        SetMessage( "Primero seleccione y cargue el diccionario inicial" );
        return false;
        }

      SetMessage( "\r\nMezclando los datos ...");

      add=0; mzc=0; ign=0;
      foreach( var sKey in dic2 )
        {
        WordData Data2 = dic2.GetWordData(sKey);
        if( Data2==null )
          {
          SetMessage( "ERROR: no se pudieron obtener los datos de la palabra '" + sKey + '\'' );
          ++ign;
          continue;
          }

        AddOrMergeKey( Dic, sKey, Data2  );
        }

      SetMessage( "Entradas Adiccionadas:"+add+" Mezcladas:"+mzc+" Ignoradas:"+ign );
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona la llave 'sKey' y sus datos al diccionario 'dic', si ya la llave existe la mezcla</summary>
    private void AddOrMergeKey( DictSingle dic, string sKey, WordData Data  )
      {
      if( !dic.IsKey( sKey ) )
        {
        dic.AddKey( sKey, Data);
        ++add;
        return;
        }

      WordData Data1 = dic.GetWordData(sKey);
      if( Data1==null )
        {
        SetMessage( "ERROR: no se pudieron obtener los datos de la palabra '" + sKey + '\'' );
        ++ign;
        return;
        }

      Data1.Merge( Data );
      ++mzc;
      }
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto diccionario a partir de un archivo de lineas de texto y el tipo de diccionario</summary>
    private DictSingle GetDiccFrom( string FName, int tipo )
      {
      if( tipo==0 ) return GetDiccFromTxtDic(FName);
      if( tipo==1 ) return DictSingle.Open(FName);
      if( tipo==2 ) return DictSingle.OpenXml(FName);
      if( tipo==3 ) return GetDiccFromDView(FName);

      return null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto diccionario a partir de un archivo de texto para mostra</summary>
    private DictSingle GetDiccFromDView(string FName)
      {
      var dic = new DictSingle(TLng.En, TLng.Es, DDataType.Word);
      var lines = File.ReadAllLines(FName);

      char[] sep = {'\\'};
      for( int i=0; i<lines.Length; ++i )
        {
        var parts = lines[i].Split(sep);  
        if( parts.Length < 2 )
          {
          SetMessage( "ERROR: No se pueden separar la llave y los datos\r\nLínea: " + (i+1) + " -> "+ lines[i]  );
          ++ign;
          continue;
          }

        var sKey  = parts[0];
        var sData = parts[1];
        //if( sKey=="any" )
        //  {
        //  sData = parts[1];
        //  }

        var data = new WordData();
        try
          {
          data.FromDView( sData );
          }
        catch( Exception ex)
          {
          SetMessage( "ERROR: Hubo una excepción al analizar los datos("+ex.Message+")\r\nLínea: " + (i+1) + " -> "+ sKey  );
          ++ign;
          continue;
          }

        AddOrMergeKey( dic, sKey, data );
        }
      
      SetMessage( "Entradas Adiccionadas:"+add+" Mezcladas:"+mzc+" Ignoradas:"+ign );
      return dic;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto diccionario a partir de un archivo de lineas de texto</summary>
    private DictSingle GetDiccFromTxtDic(string FName)
      {
      var dic = new DictSingle(TLng.En, TLng.Es, DDataType.Word);
      var lines = File.ReadAllLines(FName);

      char[] sep = {'|'};
      for( int i=0; i<lines.Length; ++i )
        {
        var parts = lines[i].Split(sep,2);
        if( parts.Length < 2 )
          {
          SetMessage( "ERROR: No se pueden separar la llave y los datos\r\nLínea: " + (i+1) + " -> "+ lines[i]  );
          ++ign;
          continue;
          }

        WordData data;
        try
          {
          data = WordData.FromStr( parts[1] );

          if( data==null )
            {
            SetMessage( "ERROR: El formato de los datos no es correcto\r\nLínea: " + (i+1) + " -> "+ parts[0]  );
            ++ign;
            continue;
            }
          }
        catch( Exception ex)
          {
          SetMessage( "ERROR: Hubo una excepción al analizar los datos("+ex.Message+")\r\nLínea: " + (i+1) + " -> "+ parts[0]  );
          ++ign;
          continue;
          }

        AddOrMergeKey( dic, parts[0], data );
        }
      
      SetMessage( "Entradas Adiccionadas:"+add+" Mezcladas:"+mzc+" Ignoradas:"+ign );
      return dic;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla todos los diccionarios que estan en el mismo directorio del diccionario principal</summary>
    private void btnMergeAll_Click( object sender, EventArgs e )
      {
      var MainDict = txtPathIni.Text;
      if(string.IsNullOrWhiteSpace(MainDict) )
        {
        MessageBox.Show( "Debe seleccionar el directorio principal" );
        return;
        }

      var MainPath = Path.GetDirectoryName( MainDict );

      if( !Directory.Exists( MainPath ) )
        {
        MessageBox.Show( "El directorio para buscar los diccionarios no existe" );
        return;
        }

      if( !IniWait() ) return;

      txtMsgBox.Clear();
      int tm = LoadMainDict( MainDict, FrmDic1 );
      if( tm== 0 ) return;

      SetMessage( "Tiempo total de carga: " + tm + " mseg" );

      var patron = "*" + Path.GetExtension( MainDict );
      var files = Directory.GetFiles( MainPath, patron );

      for( int i = 0; i<files.Length; ++i )
        {
        var dict = files[i];
        if( string.Compare( dict, MainDict, true ) != 0 )
          {
          tm = LoadAndMergeDict( files[i], FrmDic1 );
          SetMessage( "Tiempo total de carga: " + tm + " mseg" );
          }
        }

      EndWait();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el nombre del directorio destino de acuerdo con los datos</summary>
    private string getDestName()
      {
      if( Dic==null )
        {
        MessageBox.Show("No hay ningún diccionario para guardar");
        return null;
        }

      if( string.IsNullOrEmpty( txtPathSave.Text ) )
        {
        MessageBox.Show("Debe de especificarse un camino para gardar el diccionario resultante");
        return null;
        }

      var refPath = txtPathIni.Text;
      if( string.IsNullOrWhiteSpace(refPath) )
        {
        refPath = txtPathMezcla.Text;
        if( string.IsNullOrWhiteSpace(refPath) )
          {
          MessageBox.Show("No se puede obtener el camino para guardar al diccionario");
          return null;
          }
        }

      string loc  = Path.GetDirectoryName(refPath);
      string Name = Path.GetFileNameWithoutExtension(refPath);
      string ext  = Path.GetExtension(refPath);

           if( rbTexto1.Checked ) ext = "dcv";
      else if( rbBinary.Checked ) ext = "dcb";
      else if( rbXml.Checked    ) ext = "xml";

      string sep = "\\";
      if( txtPathSave.Text.Length > 0 )
        {
        var tmp = txtPathSave.Text.TrimStart( sep.ToCharArray() );
        if( Path.IsPathRooted(tmp) )
          loc = tmp;
        else
          loc = Path.Combine(loc, tmp);
        }

      if( !Directory.Exists(loc) )
        Directory.CreateDirectory(loc);  

      return Path.ChangeExtension( Path.Combine(loc, Name), ext );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone un mensaje para información del usuario</summary>
    private void SetMessage( string  msg )
      {
      txtMsgBox.Text += msg + "\r\n";
      txtMsgBox.Focus();
      txtMsgBox.SelectionStart = txtMsgBox.Text.Length + 1;
      txtMsgBox.ScrollToCaret();
      Application.DoEvents();
      }

    } // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }
