using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TrdEngine;
using TrdEngine.Data;

namespace Tests
  {
  public partial class FindVerbs : Form
    {
    int lngSrc = -1;
    int lngDes = -1;

    RedWord Reduc;

    public FindVerbs()
      {
      InitializeComponent();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para seleccionar el diccionario a analizar</summary>
    private void btnPathIni_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title  = "Seleccione el diccionario";
      SelFileDlg.Filter = "Diccionario Visual (terminado) (*.dcv)|*.dcv";

      if( SelFileDlg.ShowDialog(this) == DialogResult.OK )
        txtPathIni.Text = SelFileDlg.FileName;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el directorio de los diccionarios de traducción</summary>
    private string GetDictDir()
      {
      var AppDir = Assembly.GetExecutingAssembly().Location;           // Obtiene la localización del ensamblado

      AppDir = Path.GetDirectoryName(AppDir);                           // Quita el nombre de la aplicación

      return AppDir + @"\BinariesDicts";
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga el diccionario y muestra las llaves con nombres propios</summary>
    private void btnLoad_Click( object sender, EventArgs e )
      {
      var DictsDir = GetDictDir();

      RedWData.DicPath = DictsDir;
      ConjData.DicPath = DictsDir;
      TrdData.DicsPath = DictsDir;

      txtMsgBox.Text = "";
      try
        {
        var fName = txtPathIni.Text;
        if( !LangUtils.CodeLangsFromPath( fName, out lngSrc, out lngDes ) )
          {
          MessageBox.Show( "No se puedo obtener los idiomas del nombre del fichero:\r\n" + fName );
          return;
          }

        var lng1 = IntLngToTLng(lngSrc);
        var lng2 = lngSrc!=1? TLng.En : TLng.Es;

        Reduc = RedWord.GetReduc( lng1, lng2  );
        if( Reduc==null )
          {
          var Msg = "No hay reducción para el idioma "+ lng1 +"\r\n" +
                    "Diccionarios: '" + RedWData.DicPath + "'";

          MessageBox.Show( Msg );
          }

        FindVerbsInDict( fName );
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL CARAR EL DICCIONARIO:\r\n*** " + ex.Message );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee un diccionrio en formato de linea de texto y obtiene un diccionario para reducción</summary>
    TLng IntLngToTLng( int lng )
      {
      if( lng >= 3 ) ++lng;
      return (TLng) lng;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------

    StringComparer Comp = StringComparer.InvariantCultureIgnoreCase;
    static string  CodesVerbs = "VT,VI,VR,VA,PT,PI,GT,GI" ;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee un diccionrio en formato de linea de texto y obtiene un diccionario para reducción</summary>
    private void FindVerbsInDict(string FName)
      {
      var newEntries = new List<string>();

      var Lines = File.ReadAllLines(FName).ToList();

      char[] sep = {'|'};

      for( int i=0; i<Lines.Count; ++i )
        {
        var parts = Lines[i].Split( ViewData.KeySep );  
        if( parts.Length < 2 )
          {
          SetMessage( "ERROR: No se pueden separar la llave y los datos (" + (i+1) + ") -> "+ Lines[i]  );
          continue;
          }

        var sKey  = parts[0].ToLower();
        var sData = parts[1];

        if( sKey.Contains(' ') ) continue;

//        int tp = VerbType( sKey );
//        if( tp!=1 ) continue;

        var Trds = new List<string>();
        var vDat = new ViewData( sData, true );                              // Parsea los datos asociados a la llave    

        foreach( ViewType Tipo in vDat.Tipos )
          {
          var sTipo = ViewData.GetTypeCode( Tipo.Tipo );

          if( sTipo.Length==0 )
            {
            int tp = VerbType( sKey );
            if( tp==1 )
              AddMeansToList( Trds, Tipo.Means );
            }
          else if( CodesVerbs.Contains(sTipo) )
            {
            AddMeansToList( Trds, Tipo.Means );
            }
          }

        if( Trds.Count > 0) 
          newEntries.Add( sKey + ViewData.KeySep + ListToString(Trds) );
        }

      var lst = newEntries.AsParallel().OrderBy(x=>x,Comp).Select(x=>x);

      var diccPath = FName.Replace( ".", "_Verbs." );
      File.WriteAllLines( diccPath, lst );

      SetMessage( "\r\nEscribiendo el archivo: '" + diccPath + "'\r\n" );
      SetMessage( "Se encontraron " + newEntries.Count + " entradas" );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Conviente la lista de cadenas en una cadena seperadas por coma</summary>
    private string ListToString( List<string> Items )
      {
      var s = "";
      foreach( string item in Items )
        {
        if( s.Length > 0 ) s += ',';
        s += item;
        }

      return s;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona los significados de la lista, garantizando que no esten repetidos</summary>
    private void AddMeansToList( List<string> trds, List<string> means )
      {
      foreach( string mean in means )
        {
        var s = RemoveEsp(mean.ToLower());
        if( !s.Contains(' ') && !trds.Contains(s) )
          {
          if( IsVerb(s) ) trds.Add(s);
          }
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Chequea que sea un verbo en Español</summary>
    private bool IsVerb( string s )
      {
      switch( lngDes )
        {
        case 0:
          return (s.EndsWith("ar") || s.EndsWith("er") || s.EndsWith("ir") || s.EndsWith("ír"));
        case 2:
          return ( s.EndsWith("are") || s.EndsWith("ire") || s.EndsWith("ere") || s.EndsWith("rre") || s.EndsWith("rci") );
        case 3:
          return (s.EndsWith("er") || s.EndsWith("ir") || s.EndsWith("ïr") || s.EndsWith("re") );
        default:
          return true;
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Determina que tipo de verbo es la palabra 'Verb' 
    /// -1-No tiene reducción; 0-No Verbo; 1-Verbo infinitivo; 2-Conjugación </summary>
    private int VerbType( string Verb )
      {
      if( Reduc.Reduce( Verb ) )                      // Trata de reducir la palabra
        {
        var IsVerb  = false;

        foreach( var item in Reduc.Reductions )       // Recorre todas las reducciones
          {
          var verb = ( item.Tipo=="VV" || item.Tipo=="VA" || item.Tipo=="BE" || item.Tipo=="HA");

          if( verb )  IsVerb = true;                                // Al menos una reducción es verbo

          if( verb && item.Modo == TMod.Infinitivo ) return 1;      // Es una verbo en infinitivo
          }

        return IsVerb? 2 : 0;
        }

      return -1;                             // La palabra no pudo ser reducida
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra la especialidad de un significado</summary>
    private string RemoveEsp( string mean )
      {
      int ini = mean.IndexOf( '(' );
      if( ini==-1 ) return mean;

      int end = mean.IndexOf( ')', ini+1 );
      if( end==-1 ) return mean + "***";

      ++end;
      while( end<mean.Length && mean[end]==' ' ) ++end;
      
      return mean.Substring(0,ini) + mean.Substring(end); 
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

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un diccionario para cada idioma, con el contenido de todos los diccionarios que se encuentren en el directorio</summary>
    private void button1_Click( object sender, EventArgs e )
      {
      Consolidate( 0 );
      Consolidate( 1 );
      Consolidate( 2 );
      Consolidate( 3 );
      }

    Dictionary< string, lnkWords > LnkVerbs = new Dictionary< string, lnkWords >();

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Une todos los diccionarios en un solo idioma</summary>
    private void Consolidate( int lng )
      {
      LnkVerbs = new Dictionary< string, lnkWords >();

      var dir = Path.GetDirectoryName(txtPathIni.Text);
      var files = Directory.GetFiles( dir );

      for( int i=files.Length-1; i>=0; i-- )
        {
        Application.DoEvents();
        var fName = files[i];

        int Src, Des;
        if( !LangUtils.CodeLangsFromPath( fName, out Src, out Des ) ) continue;

        if( Src == lng )
          ProcesaFile( fName, Src, Des );
        }

      string Txt = DictToText( LnkVerbs, lng );

      var DicName = "TrdVerbs" + LangUtils.Abrev(lng) + ".txt";
      var diccPath = Path.Combine( dir, DicName );
      File.WriteAllText( diccPath, Txt );

      SetMessage( "Datos guardados en: '" + diccPath + "'" );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene una repesentación del diccionario 'dicc' en forma de texto</summary>
    private string DictToText( Dictionary<string, lnkWords> dicc, int lng )
      {
      var s =  new StringBuilder(200*dicc.Count);

      foreach( var Entry in dicc )
        {
        if( lng != 0 ) s.Append( Entry.Value.GetLang(0) ); else s.Append( Entry.Key );  
        s.Append( '\\' );

        if( lng != 1 ) s.Append( Entry.Value.GetLang(1) ); else s.Append( Entry.Key );  
        s.Append( '\\' );

        if( lng != 2 ) s.Append( Entry.Value.GetLang(2) ); else s.Append( Entry.Key );  
        s.Append( '\\' );

        if( lng != 3 ) s.Append( Entry.Value.GetLang(3) ); else s.Append( Entry.Key );  

        s.Append( '\r' );
        s.Append( '\n' );
        }

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa un diccionario con el idioma fuente y destido dado</summary>
    private void ProcesaFile( string fName, int src, int des )
      {
      var Lines = File.ReadAllLines(fName);
      foreach( var line in Lines )
        {
        var Parts = line.Split('\\');
        if( Parts.Length != 2 )
          {
          SetMessage( "Se salto la lines '" + line + "'");
          continue;
          }

        var Key = Parts[0].Trim();
        var Datos = Parts[1].Trim(); 

        if( LnkVerbs.ContainsKey(Key) )
          {
          LnkVerbs[Key].Merge( Datos, des );
          }
        else
          {
          var lkW = new lnkWords( Datos, des );
          LnkVerbs.Add( Key, lkW );
          }

        }
      }
    }
  }
