using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tests
  {
  //===================================================================================================================================
  /// <summary>Maneja todas las marcas utilizadas en los diccionarios</summary>
  public class MarksMng
    {
    /// <summary>Diccionario para obtener la información con el código de la marca y </summary>
    public Dictionary<string/*code*/, MarkInfo> Marks = new Dictionary<string, MarkInfo>();

    /// <summary>Diccionario para obtener el código de la marca, con la cadena de sustitución en Español</summary>
    public Dictionary<string , string/*code*/> EsSust = new Dictionary<string, string>();

    /// <summary>Diccionario para obtener el código de la marca, con la cadena de sustitución en Inglés</summary>
    public Dictionary<string , string/*code*/> EnSust = new Dictionary<string, string>();

    /// <summary>Diccionario para obtener el código de la marca, con la cadena de sustitución en Italiano</summary>
    public Dictionary<string , string/*code*/> ItSust = new Dictionary<string, string>();

    /// <summary>Diccionario para obtener el código de la marca, con la cadena de sustitución en Francés</summary>
    public Dictionary<string , string/*code*/> FrSust = new Dictionary<string, string>();

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea el objeto y carga las marcas desde un fichero</summary>
    public MarksMng( string FName )
      {
      var Lines = File.ReadAllLines("Marcas.txt");

      for( int i=0; i<Lines.Length; ++i )
        {
        var ln = Lines[i].Trim();
        if( ln.Length == 0 ) continue;
        if( ln[0]=='/' ) continue;

        var Datos = ln.Split('|');
        if( Datos.Length != 6 )
          {
          MessageBox.Show( "ERROR: (" + i + ") La cantidad de datos es incorrecta" );
          continue;
          }

        var code = Datos[0].Trim();
        if( Marks.ContainsKey(code) )
          {
          MessageBox.Show( "ERROR: (" + i + ") Ya hay una marca con ese código" );
          continue;
          }

        AddMark( code, Datos );
        }
      }

    public MarksMng() : base()
      {
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona una marca con el código y toda la información</summary>
    private void AddMark( string code, string[] datos )
      {
      var info = new MarkInfo( datos );

      Marks[ code ] = info;

      AddSustMark( info.Es, EsSust, code );
      AddSustMark( info.En, EnSust, code );
      AddSustMark( info.It, ItSust, code );
      AddSustMark( info.Fr, FrSust, code );
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona una marca para un idioma especifico</summary>
    private bool AddSustMark( string sLng, Dictionary<string, string> LngSust, string code )
      {
      if( LngSust.ContainsKey(sLng) )
        {
        MessageBox.Show( "ERROR: La marca '" + sLng + "' esta repetida" );
        return false;
        }

      LngSust[ sLng ] = code;
      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Determina si existe una marca con el codigo dado</summary>
    public bool Exist( string code )
      {
      return Marks.ContainsKey(code);
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene informacion asociada a la marca con el codigo dado</summary>
    public MarkInfo Info( string code )
      {
      return Marks[code];
      }

    //-----------------------------------------------------------------------------------------------------------------------------------
    static Regex reMark = new Regex( @"((?<m><)|\[)([A-Z][A-Z][a-z]?[1-9])?(?(m)>|\])", RegexOptions.Compiled );

    private string LastMark = "";
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Sustituye las marcas por palabras en el idioma de la cadena</summary>
    public int ResplaceMarks( ref string sKey, ref string sDat, int src, int des )
      {
      var KeyMarks = new HashSet<string>();                             // Marcas que hay en la llave

      var matches = reMark.Matches( sKey);                              // Busca todas las marcas en la llave
      foreach( Match match in matches )                                 // Recorre todas las marcas encontradas
        {
        LastMark = match.Groups[2].Value;                               // Obtiene el codigo de la marca

        if( !Marks.ContainsKey( LastMark ) ) return 1;                  // ERROR: La marca no esta registrada
        if( KeyMarks.Contains( LastMark )   ) return 2;                 // ERROR: La marca aparece duplicada
        
        KeyMarks.Add( LastMark );                                       // Adiciona la marca al conjunto
        }

      sKey = ResplaceMarks( sKey, src, matches );                       // Sustituye todas las marcas, por la palabra correspondiente

      var s = new StringBuilder( sDat.Length + KeyMarks.Count*15 );     // Cadena para guardar los datos con las marcas sustituidas

      var Tipos = sDat.Split('|');                                      // Separa los distintos tipos gramaticales de la entrada
      for( int i=0; i<Tipos.Length; ++i )                               // Recorre todos los tipos gramaticales
        {
        if( i>0 ) s.Append('|');                                        // Si no es primer tipo, pone separador

        var Means = Tipos[i].Split(';');                                // Separa los distintos significados del tipo
        for( int j=0; j<Means.Length; ++j )                             // Recorre todos los significados
          {
          matches = reMark.Matches( Means[j] );                         // Busca todas las marcas en el significado
          if( matches.Count != KeyMarks.Count ) return 5;               // ERROR: Un significado no tiene la misma cantidad de marcas

          var MeanMarks = new HashSet<string>();                        // Marcas que hay en el significado
          foreach( Match match in matches )                             // Recorre todas las marcas del significado
            {
            LastMark = match.Groups[2].Value;                           // Obtiene el codigo de la marca

            if( !KeyMarks.Contains( LastMark ) ) return 3;              // ERROR: La marca no aparece en la llave
            if( MeanMarks.Contains( LastMark ) ) return 4;              // ERROR: La marca aparece duplicada en el significado
        
            MeanMarks.Add( LastMark );                                  // Adiciona la marca al conjunto
            }

          if( j>0 ) s.Append(';');                                      // Si no es primer significado, pone separador

          s.Append( ResplaceMarks( Means[j], des, matches ) );          // Adiciona el significado con las marcas sustituidas
          }
        }

      sDat = s.ToString();                                              // Restaura la cadena de los datos
      return 0;                                                         // Retorna 'No error'
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Sustituye las marcas localizadas con 'matches' en la cadenada 'str' según el idioma 'str'</summary>
    private string ResplaceMarks( string str, int lng, MatchCollection matches )
      {
      var s = new StringBuilder( str );                                 // Convierte a StringBuilder (para eficiencia)

      for( int i = matches.Count-1; i>=0; i-- )                         // Sustituyes las marcas, desde atras hacia adelante                         
        {
        Match m = matches[i];                                           // Obtiene los datos de la marca encontrada

        var Mark  =  m.Groups[2].Value;                                 // Obtiene el código de la marca
        var Info  = Marks[Mark];                                        // Obtiene la informacion de la marca
        var sMark = Info.ForLang(lng);                                  // Obtiene cadena de sustitución de la marca en el idioma dado

        s.Replace( m.Value, sMark, m.Index, m.Length );                 // Sustituye la marca
        }

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene una cadena la la descripcion del ultimo error producido</summary>
    public string ErrorMsg( int code )
      {
      if( code==0 ) return "No error";

      switch( code )
        {
        case 1: return "La marca ["+ LastMark +"] no esta registrada";
        case 2: return "La marca ["+ LastMark +"] aparece duplicada en la llave";
        case 3: return "La marca ["+ LastMark +"] no aparece en la llave de la entrada";
        case 4: return "La marca ["+ LastMark +"] aparece duplicada en un significado";
        case 5: return "Un significado no tiene la misma cantidad de marcas";
        }

      return "Error desconocido";
      }
    }

  //===================================================================================================================================
  /// <summary>Mantiene la información relacionada con una marca</summary>
  public class MarkInfo
    {
    public string Desc;                     // Descripción de la marca (siempre en el idioma español)

    public string Es;                       // Cadena para mostrar la marca cuando los datos son en Español
    public string En;                       // Cadena para mostrar la marca cuando los datos son en Inglés
    public string It;                       // Cadena para mostrar la marca cuando los datos son en Italiano
    public string Fr;                       // Cadena para mostrar la marca cuando los datos son en Francés

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea informacion de la marca con un arreglo de cadenas</summary>
    public MarkInfo( string[] datos )
      {
      Desc = datos[1].Trim();

      Es = datos[2].Trim();
      En = datos[3].Trim();
      It = datos[4].Trim();
      Fr = datos[5].Trim();
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene la cadena de sustitución para un idioma dado</summary>
    public string ForLang( int lng )
      {
      switch( lng )
        {
        case 0: return Es;
        case 1: return En;
        case 2: return It;
        case 3: return Fr;
        }

      return "";
      }
    }
  }
