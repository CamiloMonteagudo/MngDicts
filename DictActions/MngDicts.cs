using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Tests.Properties;
using TrdEngine;
using TrdEngine.Data;
using TrdEngine.Dictionary;

namespace Tests
  {
  ///-----------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Maneja los diferentes formatos que pueden tener los dicionarios</summary>
  public partial class MngDicts : Form
    {
    static TextBox MsgBox;
    Dbg.DebugMsg LstFunMsg = null;

    //CultureInfo Esp = new CultureInfo("es-ES");
    //StringComparer EspComp = StringComparer.Create(Esp, true);

    //var myComp = new MyComparer("es-ES", CompareOptions.StringSort);

    StringComparer Comp = StringComparer.InvariantCultureIgnoreCase;

    MarksMng Marcas;
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary></summary>
    public MngDicts()
      {
      InitializeComponent();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary></summary>
    private void MngDicts_Load( object sender, EventArgs e )
      {
      Marcas = new MarksMng("Marcas.txt");

      string DicsDir = Settings.Default.DictsDirectory;
      txtDirsPath.Text = DicsDir;

      MsgBox = txtMsgBox;

      LstFunMsg = Dbg.FunMsg;                                         // Guarda el manipulador de mensajes que habia anteriormente
      Dbg.FunMsg = SetMessage;
      }


    #region "Botones para manejar la lista de diccionarios"
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona un diccionario a la lista</summary>
    private void btnAdd_Click( object sender, EventArgs e )
      {
      if( txtName.Text.Length > 0 )
        SearchDicts( txtName.Text );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona todos los diccionario con la extensión dada a la lista</summary>
    private void btnAddDic_Click( object sender, EventArgs e )
      {
      lstIdDirsDicts.Items.Clear();

      txtName.Text = ((Button)sender).Text;
      SearchDicts( txtName.Text );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra el diccionario seleccionado de la lista</summary>
    private void btnDel_Click( object sender, EventArgs e )
      {
      for( int i = lstIdDirsDicts.CheckedIndices.Count - 1; i >= 0; --i )
        lstIdDirsDicts.Items.RemoveAt( lstIdDirsDicts.CheckedIndices[i] );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Marca todos los diccionarios para procesar</summary>
    private void btnAll_Click( object sender, EventArgs e )
      {
      for( int i = 0; i < lstIdDirsDicts.Items.Count; ++i )
        lstIdDirsDicts.SetItemChecked( i, true );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita todos los diccionarios marcados para procesar</summary>
    private void btnClear_Click( object sender, EventArgs e )
      {
      for( int i = 0; i < lstIdDirsDicts.Items.Count; ++i )
        lstIdDirsDicts.SetItemChecked( i, false );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Cambia el directorio de los diccionarios</summary>
    private void ChangeDictsDir( string sDir )
      {
      txtDirsPath.Text = sDir;

      Settings.Default.DictsDirectory = sDir;
      Settings.Default.Save();
      }
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Muestra dialogo para seleccionar directorio donde leer los diccionarios</summary>
    private void btnGetDirData_Click( object sender, EventArgs e )
      {
      SelFolderDlg.Description = "Seleccione el directorio donde se van a leer los diccionario";
      SelFolderDlg.SelectedPath = txtDirsPath.Text;

      if( SelFolderDlg.ShowDialog() == DialogResult.OK )
        ChangeDictsDir( SelFolderDlg.SelectedPath );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Muestra dialogo para seleccionar directorio donde grardar los diccionarios</summary>
    private void btnSetDirData_Click( object sender, EventArgs e )
      {
      SelFolderDlg.SelectedPath = txtDirsPath.Text;
      SelFolderDlg.Description = "Seleccione el directorio donde se van a guardar los diccionario";

      if( SelFolderDlg.ShowDialog() == DialogResult.OK )
        txtSavePath.Text = SelFolderDlg.SelectedPath;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama al cerrar el formulario</summary>
    private void MngDicts_FormClosing( object sender, FormClosingEventArgs e )
      {
      Dbg.FunMsg = LstFunMsg;                                         // Restaura el manipulador de mensajes que habia anteriormente
      }

    int LastDir = -1;
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Recircula por los directorios que hay en el nivel directirio actual</summary>
    private void btnChgDir_Click( object sender, EventArgs e )
      {
      var sdir = Path.GetDirectoryName( txtDirsPath.Text.TrimEnd(Path.DirectorySeparatorChar) );
      if( sdir==null || !Directory.Exists( sdir ) ) return;

      var Dirs = Directory.GetDirectories( sdir );
      if( Dirs.Length == 0 ) return;

      ++LastDir;
      if( LastDir>=Dirs.Length ) LastDir = 0;

      var newDir = Path.Combine( sdir, Dirs[LastDir] ) + Path.DirectorySeparatorChar;
      if( newDir ==  txtDirsPath.Text )
        {
        LastDir = (LastDir>=Dirs.Length-1) ? 0 : ++LastDir;

        newDir = Path.Combine( sdir, Dirs[LastDir] ) + Path.DirectorySeparatorChar;
        }

      ChangeDictsDir( newDir );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita un nivel de direcctorio</summary>
    private void btnMenosDir_Click( object sender, EventArgs e )
      {
      var sdir = Path.GetDirectoryName( txtDirsPath.Text.TrimEnd(Path.DirectorySeparatorChar) );
      if( sdir==null ) return;

      ChangeDictsDir( sdir + Path.DirectorySeparatorChar );
      LastDir = 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Aumenta un nivel de direcctorio</summary>
    private void btnMasDir_Click( object sender, EventArgs e )
      {
      var Dirs = Directory.GetDirectories( txtDirsPath.Text );
      if( Dirs.Length == 0 ) return;

      ChangeDictsDir( Dirs[0] + Path.DirectorySeparatorChar );
      LastDir = 0;
      }

    private void btnDefault_Click( object sender, EventArgs e )
      {
      if( txtSavePath.Text == TrdData.DicsPath )
        txtSavePath.Text = "Result";
      else
        txtSavePath.Text = TrdData.DicsPath;
      }

    #endregion

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca todos los ficheros con una extención determinada y los agrega a la lista</summary>
    private void SearchDicts( string patron )
      {
      var loc  = TrdData.DicsPath;
      if( txtDirsPath.Text.Length > 0 )
        loc = txtDirsPath.Text;

      if( !Directory.Exists( loc ) )
        {
        MessageBox.Show( "El directorio para los diccionarios no existe" );
        return;
        }

      var files = Directory.GetFiles( loc, patron );

      for( int i = 0; i<files.Length; ++i )
        lstIdDirsDicts.Items.Add( Path.GetFileName( files[i] ) );

      SetMessage( "Fueron adicionados " +  files.Length.ToString() + " ficheros\r\n" );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone un mensaje para información del usuario</summary>
    private void SetMessage( string msg )
      {
      txtMsgBox.Text += msg;
      txtMsgBox.Focus();
      txtMsgBox.SelectionStart = txtMsgBox.Text.Length + 1;
      txtMsgBox.ScrollToCaret();
      Application.DoEvents();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el nombre del diccionario de origen de acuerdo a los datos</summary>
    private string getSrcName( string Name, string ext )
      {
      var loc  = Settings.Default.DictsDirectory;;
      if( txtDirsPath.Text.Length > 0 )
        loc = txtDirsPath.Text;

      if( string.IsNullOrWhiteSpace(ext) )
        ext = Path.GetExtension(Name);

      return Path.ChangeExtension( Path.Combine( loc, Name ), ext );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el nombre del directorio destino de acuerdo con los datos</summary>
    private string getDestName( string Name, string ext )
      {
      var loc  = Settings.Default.DictsDirectory;;
      if( txtDirsPath.Text.Length > 0 )
        loc = txtDirsPath.Text;

      string sep = "\\";
      if( txtSavePath.Text.Length > 0 )
        {
        var tmp = txtSavePath.Text.TrimStart( sep.ToCharArray() );
        if( Path.IsPathRooted( tmp ) )
          loc = tmp;
        else
          loc = Path.Combine( loc, tmp );
        }

      if( !Directory.Exists( loc ) )
        Directory.CreateDirectory( loc );

      if( string.IsNullOrWhiteSpace(ext) )
        ext = Path.GetExtension(Name);

      return Path.ChangeExtension( Path.Combine( loc, Name ), ext );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Operaciones que se pueden realizar con los diccionarios</summary>
    public enum OpDict
      {
      OrdenaDir = 0,
      WordsIndex,
      SplitSent,
      SameKeys,
      SimpleKeys,
      NormAll,
      ExpandMark,
      CrossDict,
      AddInvKeys,
      NormSimple,
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa todos los diccionarios en la lista, según el procedimiento definido por PTipo y usando las extensiones
    /// ext1 para el diccionario de entrada y ext3 para el diccionario de salida</summary>
    private void PrecessLote( OpDict op, string ext1=null, string ext2=null )
      {
      if( Application.UseWaitCursor )
        {
        MessageBox.Show( "La aplicación ya esta ocupada" );
        return;
        }

      txtMsgBox.Clear();
      Application.UseWaitCursor = true;

      var Data = new Dictionary<string,int>();
      int nRecTotal = 0;

      foreach( var item in lstIdDirsDicts.CheckedItems )
        {
        var name  = lstIdDirsDicts.GetItemText( item );

        var Name1 = getSrcName ( name, ext1 );
        var Name2 = getDestName( name, ext2 );

        SetMessage( "Procesando ...... [" + op + "] :" + Name1 + " ... \r\n" );
        int ini = Environment.TickCount;

        int nRec = 0;
        switch( op )
          {
          case OpDict.SplitSent:
            nRec = SplitSenteces( Name1, Name2 );
            break;
          case OpDict.NormSimple:
            nRec = NormalizeSimple( Name1, Name2 );
            break;
          case OpDict.NormAll:
            nRec = NormalizeAll( Name1, Name2 );
            break;
          case OpDict.OrdenaDir:
            nRec = OrdenaDView( Name1, Name2 );
            break;
          case OpDict.SameKeys:
            nRec = QuitaLlavesIguales( Name1, Name2 );
            break;
          case OpDict.AddInvKeys:
            nRec = AdicionaLlavesInvetidas( Name1, Name2 );
            break;
          case OpDict.ExpandMark:
            nRec = ExpandeMaracdores( Name1, Name2 );
            break;
          case OpDict.CrossDict:
            nRec = CruzarDictionary( Name1, Name2 );
            break;
          case OpDict.WordsIndex:
            nRec = CreateWodsIndex( Name1, Name2 );
            break;
          case OpDict.SimpleKeys:
            nRec = ExtraeSimpleKeys( Name1, Name2 );
            break;
          default:
            MessageBox.Show( "No hay operación definida para realizar" );
            break;
          }

        int end = Environment.TickCount;
        if( nRec>0 )
          {
          SetMessage( "# de registros  : " + nRec.ToString()      + "\r\n" );
          SetMessage( "Tiempo          : " + (end-ini).ToString() + " mseg\r\n" );
          }

        nRecTotal += nRec;
        }

      Application.UseWaitCursor = false;
      Cursor.Current = Cursors.Default;
      }

    #region "Funciones de procesamiento de diccionarios
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee un diccionario de oraciones en multiples idiomas y lo divide en parejas</summary>
    private int SplitSenteces( string srcDict, string desDict )
      {
      var lstEs = new List<string>();
      var lstEn = new List<string>();
      var lstIt = new List<string>();
      var lstFr = new List<string>();

      try
        {
        var lines = File.ReadAllLines( srcDict );

        for( int i=0; i<lines.Length; ++i )
          {
          var Es = lines[i++].Split('|');
          var En = lines[i++].Split('|');
          var It = lines[i++].Split('|');
          var Fr = lines[i++].Split('|');
          if( Es[0]!="Es"  || En[0]!="En"  || It[0]!="It"  || Fr[0]!="Fr" ||
              Es.Length!=2 || En.Length!=2 || It.Length!=2 || Fr.Length!=2 )
            {
            SetMessage( "Error en el grupo termina en de la linea:"+i+"\r\n");
            for( ;i<lines.Length; ++i )
              if( lines[i].StartsWith("Es|") ) { ++i; break; }
            continue;
            }

          lstEs.Add(Es[1]);
          lstEn.Add(En[1]);
          lstIt.Add(It[1]);
          lstFr.Add(Fr[1]);
          }

        List<string>[] Langs = {lstEs,lstEn,lstIt,lstFr };
        string[]       Abrvs = {"Es","En","It","Fr" };
        var desPath = Path.GetDirectoryName(desDict) + Path.DirectorySeparatorChar;
         
        for( int src=0; src<4; ++src )
          {
          var lstSrc = Langs[src];
          for( int des=0; des<4; ++des )
            {
            if( src==des ) continue;
            var file = desPath + "Senteces" + Abrvs[src] + '2' + Abrvs[des] + ".dcv";

            SaveSentecesDir( file, lstSrc, Langs[des]);
            }
          }

        return lines.Length;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL PRECESAR EL DICCIONARIO DE ORACIONES:\r\n*** " + ex.Message );
        }

      return 0;
      }
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Normaliza el uso de los espacios y otros caracteres en la llave y en los datos</summary>
    private int NormalizeAll( string srcDict, string desDict )
      {
      int i=0;
      try
        {
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo
        int src, des;
        if( !GetLanguajes( srcDict, out src, out des ) )
          {
          SetMessage( "No se puedo obtener los idiomas del nombre del fichero:\r\n" + srcDict );
          return 0;
          }

        char[] sep = { ViewData.KeySep };

        for( ; i<lines.Count; i++ )                                     // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep );                                // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          string sKey, sDat;
          
          bool Mrk1 = QuitaTarecos( parts[0], out sKey );               // Quita caracteres innecesario en la llave
          bool Mrk2 = QuitaTarecos( parts[1], out sDat );               // Quita caracteres innecesario en los datos

          if( Mrk1 || Mrk2 )                                            // Si habia marca de sustitución, en la llave o los datos
            {
            var err = Marcas.ResplaceMarks(ref sKey, ref sDat, src, des );    // Sustituye las marcas por palabras según el idioma
            if( err !=0 )
              {
              SetMessage( "Línea " + (i+1) +"-> "+ Marcas.ErrorMsg(err) + "\r\n" );
              continue;
              }
            }

          sKey = ViewData.Normalize( sKey, true, false );               // Quita todos los espacios y caracteres innecesarios
          var vDat = new ViewData( sDat );                              // Parsea los datos asociados a la llave    

          vDat.NormalizeMeans();                                        // Quita todos los espacios y caracteres innecesarios
          lines[i] = sKey + ViewData.KeySep + vDat.ToString();          // Forma la entrada nuevamente
          }

        var diccPath = desDict.Replace(".", "_nom.");
        File.WriteAllLines( diccPath, lines );
        return lines.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR NORMALIZANDO TODOS LOS DATOS LÍNEA:"+ i +"\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Normaliza el uso de los espacios y otros caracteres en la llave y en los datos, pero sin realizar las sustituciones</summary>
    private int NormalizeSimple( string srcDict, string desDict )
      {
      int i=0;
      try
        {
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo
        int src, des;
        if( !GetLanguajes( srcDict, out src, out des ) )
          {
          SetMessage( "No se puedo obtener los idiomas del nombre del fichero:\r\n" + srcDict );
          return 0;
          }

        char[] sep = { ViewData.KeySep };

        for( ; i<lines.Count; i++ )                                     // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep );                                // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          string sKey, sDat;
          
          bool Mrk1 = QuitaTarecos( parts[0], out sKey );               // Quita caracteres innecesario en la llave
          bool Mrk2 = QuitaTarecos( parts[1], out sDat );               // Quita caracteres innecesario en los datos

          if( Mrk1 || Mrk2 )                                            // Si habia marca de sustitución, en la llave o los datos
            {
            var tmpKey = sKey;
            var tmpDat = sDat;
            var err = Marcas.ResplaceMarks(ref tmpKey, ref tmpDat, src, des );    // Sustituye las marcas por palabras según el idioma
            if( err !=0 )
              {
              SetMessage( "Línea " + (i+1) +"-> "+ Marcas.ErrorMsg(err) + "\r\n" );
              continue;
              }
            }

          sKey = ViewData.Normalize( sKey, true, false );               // Quita todos los espacios y caracteres innecesarios
          var vDat = new ViewData( sDat );                              // Parsea los datos asociados a la llave    

          vDat.NormalizeMeans();                                        // Quita todos los espacios y caracteres innecesarios

          // Divide la llave en palabras
          var words = sKey.Split(wrdSep, StringSplitOptions.RemoveEmptyEntries);  // Separa en palabras

          if( words.Length>20 )                                         // Chequea el número de palabras
            SetMessage( "Línea " + (i+1) +"-> "+ words.Length + " palabras son demasiadas para una entrada\r\n" );

          lines[i] = sKey + ViewData.KeySep + vDat.ToString();          // Forma la entrada nuevamente
          }

        var ordLn = lines.AsParallel().OrderBy(x=>x,Comp);
        var LastLine = "";

        var entries = new List<string>( lines.Count );
        foreach( var line in ordLn )
          {
          if( string.Compare( line, LastLine, true)==0 ) continue;

          entries.Add( line );
          LastLine = line;
          }

        var diccPath = desDict.Replace(".", "_rev.");
        File.WriteAllLines( diccPath, entries );
        return lines.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR NORMALIZANDO TODOS LOS DATOS LÍNEA:"+ i +"\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    static char[] IniChrs = {'<','['};
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// Quita todos los caracteres, innecesarios en la cadeba 'str' y retorna si tiene marcas de sustitución o no
    private bool QuitaTarecos( string str, out string sOut )
      {
      var s = new StringBuilder(str);

      if( str.IndexOf('x') != -1 )
        {
        s.Replace( " xxxx", "" );
        s.Replace( "xxxx ", "" );
        s.Replace( "xxxx", "" );
        }

      s.Replace( "  ", " " );
      sOut = s.ToString();

      return ( str.IndexOfAny( IniChrs )!=-1 );               // Indica si puede tener marca de sustitución o no
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Ordena las entradas de un diccionario con formato DView </summary>
    private int OrdenaDView( string srcDict, string desDict )
      {
      try
        {
        var lines = File.ReadAllLines( srcDict );                                   // Lee el diccionario completo

        var Entries = new List<KeyValuePair<string,string>>( lines.Length+1 );      // Lista con todas las entradas separadas en Llava/Datos

        char[] sep = {ViewData.KeySep};
        foreach( var line in lines )                                                // Separa la llave de los datos en todas las línea
          {
          var parts = line.Split(sep);                                             // Separa la linea por el back slap
          if( parts.Length < 2 )
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          Entries.Add( new KeyValuePair<string, string>( parts[0], parts[1]) );     // La adicona a la lsita de entradas
          }

        var lst = Entries.AsParallel().OrderBy(x=>x.Key,Comp).Select(x=>x.Key+ViewData.KeySep+x.Value);

        var diccPath = desDict.Replace(".", "_ord.");
        File.WriteAllLines( diccPath, lst );

        SetMessage( "Escribiendo el archivo: '" + diccPath + "'\r\n" );
        return lines.Length;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR ORDENANDO EL DICCIONARIO:\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita las llaves que son iguales (supone que el diccionario ya esta ordenado)</summary>
    private int QuitaLlavesIguales( string srcDict, string desDict )
      {   
      string[] parts = { "","" };
      int i=0;
      try
        {
        int n = 0;
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo

        string lastKey = "";                                            // Última llave analizada
        char[] sep = { ViewData.KeySep };

        for( i=0; i<lines.Count; i++ )                                  // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          parts = line.Split( sep );                                    // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          var sKey = ViewData.Normalize( parts[0], true );              // Quita todos los espacios y caracteres innecesarios

          if( sKey == lastKey )                                         // Si la llave (Normalizada) es igual a la anterior
            {
            MergeKeys( i, lines );                                      // Mezcla la entrada 'i' con la anterior

            lines.RemoveAt(i--);                                        // Borra entrada 'i' y se queda en el lugar
            ++n;                                                        // Aumenta contador de entradas borradas
            }

          lastKey = sKey;                                               // Actualiza ultima entrada
          }

        SetMessage( "Número de líneas iguales: " + n + "\r\n" );

        var diccPath = desDict.Replace(".", "_dky.");
        File.WriteAllLines( diccPath, lines );
        return lines.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "ERROR: buscando llaves iguales (línea="+i+" llave='"+ parts[0] +"')\r\n" + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona llaves procedentes de un diccionario invertido</summary>
    private int AdicionaLlavesInvetidas( string srcDict, string desDict )
      {
      SelFileDlg.Title  = "Seleccione el diccionario al que se le van a adicionar llaves";
      SelFileDlg.Filter = "Diccionario con datos para mostrar (*.dcv)|*.dcv";

      if( SelFileDlg.ShowDialog(this) != DialogResult.OK ) return 0;

      int n = 0;
      try
        {
        var lines = File.ReadAllLines( srcDict );                                   // Lee el diccionario completo

        var Dic = new Dictionary<string,string>( Comp );                           // Diccionario con todas las llaves

        char[] sep = {ViewData.KeySep};
        foreach( var line in lines )                                                // Separa la llave de los datos en todas las línea
          {
          var parts = line.Split(sep);                                             // Separa la linea por el back slap
          if( parts.Length < 2 )
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          if( Dic.ContainsKey( parts[0] ) )
            {
            SetMessage( "Llave repetida: '" + line + "'\r\n" );
            continue;
            }

          Dic.Add( parts[0], parts[1] );
          }

        lines = File.ReadAllLines( SelFileDlg.FileName );                           // Lee el diccionario completo
        foreach( var line in lines )                                                // Separa la llave de los datos en todas las línea
          {
          var parts = line.Split(sep);                                             // Separa la linea por el back slap
          if( parts.Length < 2 )
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          if( Dic.ContainsKey( parts[0] ) ) continue;

          Dic.Add( parts[0], parts[1] );
          ++n;
          }

        var lst = Dic.AsParallel().OrderBy(x=>x.Key,Comp).Select(x=>x.Key+ViewData.KeySep+x.Value);

        var diccPath = desDict.Replace(".", "_new.");
        File.WriteAllLines( diccPath, lst );

        SetMessage( "Escribiendo el archivo: '" + diccPath + "'\r\n" );
        return n;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR ORDENANDO EL DICCIONARIO:\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Expande los marcadores de tipo gramatical y de especialidades, con su descripción según el idioma</summary>
    private int ExpandeMaracdores( string srcDict, string desDict )
      {
      try
        {
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo
        int src, des;
        if( !GetLanguajes( srcDict, out src, out des ) )
          {
          SetMessage( "No se puedo obtener los idiomas del nombre del fichero:\r\n" + srcDict );
          return 0;
          }

        char[] sep = { ViewData.KeySep };

        for( int i=0; i<lines.Count; i++ )                              // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep );                                // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          var sKey = parts[0];       
          var vDat = parts[1];

          for(;;)
            {
            var iFind = vDat.IndexOf(ViewData.TypeMark);
            if( iFind==-1 || iFind>=vDat.Length-2) break;

            var sTp = ViewData.GetTypeAbrv( vDat[iFind+1], des );

            var sResp = vDat.Substring(iFind,2);
            vDat = vDat.Replace( sResp, sTp );
            }

          for(;;)
            {
            var iFind = vDat.IndexOf(ViewData.EspMark);
            if( iFind==-1 || iFind>=vDat.Length-3) break;

            var sNum = vDat.Substring(iFind+1,2);
            var idx = -1;
            int.TryParse(sNum, out idx);

            var sEsp = ViewData.GetEspAbrv(idx, 0);

            var sResp = ViewData.EspMark + sNum;
            vDat = vDat.Replace( sResp, sEsp );
            }

          lines[i] = sKey + ViewData.KeySep + vDat;        // Forma la entrada nuevamente
          }

        var diccPath = desDict.Replace(".", "_mrk.");
        File.WriteAllLines( diccPath, lines );
        return lines.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR EXPANDIENDO MARCADORES:\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Cruza todos los datos de las llaves y forma un diccionario con los idiomas invertidos</summary>
    private int CruzarDictionary( string srcDict, string  desDict)
      {
      int i=0;
      try
        {
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo
        int src, des;
        if( !GetLanguajes( srcDict, out src, out des ) )
          {
          SetMessage( "No se puedo obtener los idiomas del nombre del fichero:\r\n" + srcDict );
          return 0;
          }

        var Dicc = new Dictionary<string,string>();

        char[] sep = { ViewData.KeySep };

        for( ; i<lines.Count; i++ )                                     // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep );                                // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          var sKey  = parts[0]; 
          var sData =  parts[1];     

          //if( sData.Contains( "asserts that" ) || sData.Contains( "needs a dozen eggs" ) )
          //  sData =  parts[1];     

          var vDat = new ViewData( sData );
          if( vDat.Tipos.Count>1 ) continue;                              // Se salta los que tienen mas de tipo

          var vTipo = vDat.Tipos[0];
          var cTp = vTipo.Tipo;
          if( cTp!='\0' &&  cTp>('A'+3) ) continue;                       // Solo acepta: Sin tipo, SS, NP, AA, DD

          var Means = vTipo.Means;
          if( Means.Count>3 ) continue;                                   // Tiene muchos significados

          var invTp  = (cTp!='\0')? (ViewData.TypeMark.ToString() + cTp + ' ') : "";
          var invDat = sKey;
          var nWrdKey = sKey.Split(' ').Length; 
          if( nWrdKey>20 )  continue;                                     // La frase es demasiado larga

          for( int k=0; k<Means.Count; k++ )
            {
            var sMean = vTipo.Means[k];

            if( sMean.IndexOf('<') != -1  ) continue;                     // Se salta significados con anotaciones
            if( sMean.IndexOf('(') != -1  ) continue;                     // Se salta significados con información

            var sEsp = "";
            if( sMean.Length>4 && sMean[0]==ViewData.EspMark &&  char.IsDigit(sMean,1) && char.IsDigit(sMean,2) && sMean[3]==' ' )
              {
              sEsp  = sMean.Substring(0,4); 
              sMean = sMean.Remove(0,4);
              }

            if( sMean.Length>2 && sMean[0]=='f' && sMean[1]=='.' && sMean[2]==' ' )
              sMean = sMean.Remove(0,3);

            if( sMean.Length>4 && sMean[0]=='p' && sMean[1]=='l' && sMean[2]=='.' && sMean[3]==' ' )
              sMean = sMean.Remove(0,4);

            var invKey = sMean.Replace("*","");
            var nWrdMean = invKey.Split(' ').Length; 

            if( nWrdMean>20           )  continue;                        // La frase es demasiado larga
            if( nWrdMean > nWrdKey*3  )  continue;                        // Si la diferencia de palabras es muy grande (puede ser una explicación)

            if( Dicc.ContainsKey(invKey) )
              {
              var dat1 = new ViewData( Dicc[invKey]  );
              var dat2 = new ViewData( invTp + sEsp + invDat );

              dat1.Merge( dat2 );

              Dicc[invKey] = dat1.ToString();
              }
            else
              Dicc[invKey] = invTp + sEsp + invDat;
            }
          }

        var lst = Dicc.AsParallel().OrderBy(x=>x.Key,Comp).Select(x=>x.Key+ViewData.KeySep+x.Value);

        var diccPath = desDict.Replace( src+"2"+ des, des+"2"+src );
        diccPath = diccPath.Replace(".", "_inv.");
        File.WriteAllLines( diccPath, lst );
        return Dicc.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR INVIRTIENDO EL DICCIONARIO LÍNEA:"+ i +"\r\n*** " + ex.Message );
        }

      return 0;
      }


    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Extrae las llaves simpes del diccionario para posteriormente cruzarlas con otro diccionario</summary>
    private int ExtraeSimpleKeys( string srcDict, string desDict )
      {
      try
        {
        var linesSimples = new List<string>();
        var lines = File.ReadAllLines( srcDict );

        for( int i=0; i<lines.Length; ++i )
          {
          var lnParts = lines[i].Split(ViewData.KeySep);

          var vDat = new ViewData( lnParts[1] );
          if( vDat.Tipos.Count==1 && vDat.Tipos[0].Means.Count<4 )
            linesSimples.Add( lines[i] ); 
          }

        var diccPath = desDict.Replace(".", "_spKeys.");
        File.WriteAllLines( diccPath, linesSimples );

        SetMessage( "Escribiendo el archivo: '" + diccPath + "'\r\n" );
        return linesSimples.Count;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR SACANDO LAS LLAVES SIMPLES:\r\n*** " + ex.Message );
        }

      return 0;
      }

    static char[] wrdSep = {' ','-','(',')','"','¿','?','!','¡',',','/','+','='};
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un diccionario con los indices de todas la palabras que forman las llaves</summary>
    private int CreateWodsIndex( string srcDict, string desDict )
      {
      var WrdDicc = new Dictionary<string,string>();

      char[] keySep = { ViewData.KeySep };

      int src; int des;
      GetLanguajes( srcDict, out src, out des );

      try
        {
        var lines = File.ReadAllLines( srcDict );

        for( int i=0; i<lines.Length; ++i )
          {
          var line = lines[i];

          var parts = line.Split( keySep );                             // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          if( parts.Length > 3 )                                        // Si hay problemas con el separador
            {
            SetMessage( "Al parecer este fichero ya fue procesado, tiene muchos campos en la linea:\r\n'" + line + "'\r\n" );
            return 0;
            }

          // Le quita los delimitadores a todos los mardadores de la llave y pone sus datos en 'MarkPos'
          string MarkPos = "";
          var sKey  = parts[0]; 
          for(;;)                                                                 // Repite el proceso mientras encuentre marcas
            {
            var iniMrk = sKey.IndexOf('{');                                       // Busca el inicio de la marca
            if( iniMrk==-1 ) break;                                               // Si no lo encuentra, termina

            var endMrk = sKey.IndexOf('}', iniMrk+1);                             // Busca final de la marca a partir de inicio 
            if( endMrk==-1 ) break;                                               // Si no le encuentra, termina

            var len = endMrk-iniMrk-1;                                            // Calcula la longitud de la marca

            var sIni = sKey.Substring(0,iniMrk);                                  // Cadena delante de la marca
            var sMid = sKey.Substring( iniMrk+1, len );                           // Cadena de la marca
            var sEnd = sKey.Substring( endMrk+1 );                                // Cadena después de la marca

            sKey = sIni + sMid + sEnd;                                            // Reconstruye la llave sin delimitadores

            var sPos = iniMrk.ToString("X2");                                     // Cadena de 2 caracteres hexagesimales con la posición de la marca
            var sLen = len.ToString("X2");                                        // Cadena de 2 caracteres hexagesimales con el tamño de la marca

            MarkPos += sPos + sLen;                                               // Cadena con datos de las marcas
            }

          // Divide la llave en palabras y las pone en el indice de palabras
          var words = sKey.Split(wrdSep, StringSplitOptions.RemoveEmptyEntries);  // Separa en palabras

          if( words.Length>30 )
            SetMessage( "Muchas palabras " + words.Length + " en la línea " + i + "\r\n");

          for( int j = 0; j < words.Length; j++ )                                 // Recorre todas las palabras
            {
            var wrd = words[j].ToLower();                                         // La lleva a minusculas

            if( wrd.Length==1 && !char.IsLetter(wrd,0) ) continue;                // Salta caracteres que no son letras
            if(char.IsNumber(wrd,0) ) continue;                                   // Salta todos los números

            QuitaAcentos( ref wrd, src );                                         // Quita los acentos a la palabra, si los tiene

            var idxEntry = i.ToString("X5");                                      // Convierte indice en cadena hexagesimal de 5 caracteres
            var WrdPos   = (char)('0' + j);                                       // Obtiene posición de la palabra en un solo caracter

            if( WrdDicc.ContainsKey(wrd) ) AddWordToEntry( WrdDicc, wrd, idxEntry, WrdPos );   // Si la entrada existe, adiciona los datos
            else                           WrdDicc[wrd]  = (idxEntry + WrdPos);                // Si no existe, crea una entrada nueva
            }

          // Crea entrada nueva con toda la información Key\datos\nWords(\MarksInfo)     
          line = sKey + ViewData.KeySep +  parts[1] + ViewData.KeySep + (char)('0' + words.Length);

          if( MarkPos.Length > 0 )                                                // Si hay marcas
            line += ViewData.KeySep + MarkPos;                                    // Pone información de la marcas al final

          lines[i] = line;                                                        // Sustituye la linea original
          }

        var lst = WrdDicc.AsParallel().OrderBy(x=>x.Key,StringComparer.InvariantCultureIgnoreCase).Select(x=>x.Key+ViewData.KeySep+x.Value);
        var SortWords = lst.ToArray();

        SetMessage( "Escribiendo el archivo: '" + desDict + "'\r\n" );
        File.WriteAllLines( desDict, SortWords );

        SetMessage( "Escribiendo el archivo: '" + srcDict + "'\r\n" );
        File.WriteAllLines( srcDict, lines );

        return SortWords.Length;
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR CREANDO EL INDICE DE LAS PALABRAS:\r\n*** " + ex.Message );
        }

      return 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    static Dictionary<char,char> EsChars = new Dictionary<char,char>{ {'á','a'},{'é','e'},{'í','i'},{'ó','o'},{'ú','u'} };
    static Dictionary<char,char> ItChars = new Dictionary<char,char>{ {'é','e'},{'à','a'},{'è','e'},{'ì','i'},{'ò','o'},{'ù','u'} };
    static Dictionary<char,char> FrChars = new Dictionary<char,char>{ {'é','e'},{'à','a'},{'è','e'},{'ù','u'},{'ë','e'},{'ï','i'},{'ö','o'},{'ü','u'},{'â','a'},{'ê','e'},{'î','i'},{'ô','o'},{'û','u'} };

    static  Dictionary<char,char>[] Chars = { EsChars, null, ItChars, FrChars};
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// Quita los acentos de las palabras según el idioma
    private void QuitaAcentos( ref string wrd, int lng )
      {
      if( lng==1 ) return;

      var FChars = Chars[lng];

      var str = new StringBuilder(wrd);
      for( int i=0; i<str.Length; ++i )
        {
        char c;
        if( FChars.TryGetValue( str[i], out c ) )
          {
          str[i] = c;
          if( lng != 3 ) break;
          }
        }

      wrd = str.ToString(); 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona una entrada 'idxEntry' a la palabra 'wrd' en el diccionario 'wrdDicc', teniendo en cuenta que esa entrada pude estar repetida</summary>
    private void AddWordToEntry( Dictionary<string, string> wrdDicc, string wrd, string idxEntry, char wrdPos )
      {
      var sData = wrdDicc[wrd];
      int len = sData.Length;
      int ini = 0;

      if( idxEntry=="316CE" )
        ini = 0;

      for(;;)
        {
        if( ini+6>len )
          {
          wrdDicc[wrd] += (idxEntry + wrdPos);
          return;
          }
        
        var sIdxEntry = sData.Substring( ini, 5 );
        ini += 6;

        if( sIdxEntry == idxEntry )
          {
          while( ini<len && sData[ini]=='\'' ) ini += 2;

          wrdDicc[wrd] = sData.Insert( ini, "'" + wrdPos );
          return;
          }

        while( ini<len && sData[ini]=='\'' ) ini += 2;
        }

      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla la entrada 'i' con la anterior en 'lines'</summary>
    private void MergeKeys( int i, List<string> lines )
      {
      char[] sep = { ViewData.KeySep };
      var parts = lines[i-1].Split(sep);

      var Datos1 = new ViewData( parts[1] );
      var Datos2 = new ViewData( lines[i].Split(sep)[1] );

      Datos1.Merge( Datos2 );

      lines[i-1] = parts[0] + ViewData.KeySep + Datos1.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene los idiomas fuente y destino a partir del nombre del fichero</summary>
    private bool GetLanguajes( string srcDict, out int src, out int des )
      {
      src=-1; des=-1;

      var lngIdx = new Dictionary<string, int>() { {"Es",0},{"En",1},{"It",2},{"Fr",3} };

      var Name = Path.GetFileNameWithoutExtension(srcDict);
      var iFind = Name.IndexOf('2');
      if( iFind<2 || iFind+2>=Name.Length ) return false;

      var sSrc = Name.Substring( iFind-2, 2 );
      var sDes = Name.Substring( iFind+1, 2 );

      if( !lngIdx.TryGetValue(sSrc, out src) ) return false;
      if( !lngIdx.TryGetValue(sDes, out des) ) return false;

      return true;
      }

    ///-------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Guarda un diccionrio de oraciones</summary>
    private void SaveSentecesDir( string file, List<string> lstSrc, List<string> lstDest )
      {
      var Oras = new StringBuilder( lstSrc.Count * 80 );

      for( int i=0; i<lstSrc.Count; ++i )
        {
        var src = lstSrc[i];
        var des = lstDest[i];

        if( string.IsNullOrWhiteSpace(src) || string.IsNullOrWhiteSpace(des) ) continue;

        Oras.Append( src + ViewData.KeySep + des + "\r\n" );
        }

      File.WriteAllText( file, Oras.ToString() );

      SetMessage( "Escribiendo el archivo: '" + file + "'\r\n" );
      }


    #endregion

    #region "Ateción a los botones"
    ///-------------------------------------------------------------------------------------------------------------------------------
    /// <summary></summary>
    private void btnDicOrder_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.OrdenaDir );
      }

    private void bntWordIndex_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.WordsIndex, null, ".dcWrd" );
      }

    private void btnSplitSenteces_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.SplitSent, ".txt" );
      }

    private void btnSamesKeys_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.SameKeys );
      }

    private void btnNormalizeAll_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.NormAll );
      }

    private void btnExpandMark_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.ExpandMark );
      }

    //--------------------------------------------------------------------------------------------------------------------------------------
    private void btnInvertDicc_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.CrossDict );
      }

    private void btnAddKeys_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.AddInvKeys );
      }

    private void btnNormalizaSimple_Click( object sender, EventArgs e )
      {
      PrecessLote( OpDict.NormSimple );
      }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------
    }

  public class MyComparer : IComparer
    {
    private CompareInfo myComp;
    private CompareOptions myOptions = CompareOptions.None;

    // Constructs a comparer using the specified CompareOptions. 
    public MyComparer( string CultName, CompareOptions options )
      {
      myComp = CompareInfo.GetCompareInfo(CultName);
      this.myOptions = options;
      }

    // Compares strings with the CompareOptions specified in the constructor. 
    public int Compare( Object a, Object b )
      {
      if( a == b ) return 0;
      if( a == null ) return -1;
      if( b == null ) return 1;

      String sa = a as String;
      String sb = b as String;
      if( sa != null && sb != null )
        return myComp.Compare( sa.ToLower(), sb.ToLower(), myOptions );
      throw new ArgumentException( "a and b should be strings." );
      }
    }

  }
