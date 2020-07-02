using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;
using TrdEngine.Dictionary;

namespace TrdEngine.Data
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Contiene todos los datos necesario para la conjugación</summary>
  static public class ConjData
    {
    static string DPath = "";

    ///-----------------------------------------------------------------------------------------------------------------------------------      
    /// <summary> Datos para las conjugaciones en todos los idioma </summary>
    static public ConjLangData[] Langs =  new ConjLangData[TConst.Langs];

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Localización donde se encuentran los diccionario</summary>
    static public string DicPath{ get{return DPath;} set{ DPath = value.TrimEnd('\\') + '\\';} }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Delegado que se va a usar para abrir los diccionarios</summary>
    static public HndOpenTrdDict OpenDictDelegate = OpenDictSingle;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre del diccionario de conjugación asociado a un idioma  </summary>
    /// <param name="lang">Idioma para el cual se obtiene el nombre</param>
    static public string GetDictName( TLng lang )
      {
      return DicPath + lang + "ConjVerb.dcb";
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Constructor estatico que incializa los datos la primera vez que se usa la clase</summary>
    static ConjData()
      {
      var Loc = Assembly.GetExecutingAssembly().Location;           // Obtiene la localización del ensamblado
      DicPath = Path.GetDirectoryName(Loc) + "\\Dictionaries";      // Toma directorio 'Dictionaries' respecto al ensamblado
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Manipulador por defecto para abrir el diccionario (Diccionario binario)</summary>
    /// <param name="Lang">Idioma del diccionario que se quiere abrir</param>
    static public IDictTrd OpenDictSingle( string DictName )
      {
      return DictSingle.Open( DictName );

//      return DictSingle.OpenXml( Path.ChangeExtension( GetDictName(LangId),"xml") );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos para la conjugación en un idioma </summary>
    /// <param name="lang">Idioma de conjugación del que se quieren obtener los datos</param>
    static public ConjLangData GetLang( TLng lang )
      {
      int idx = (int)lang;                                               // Castea el idioma a un entero
      if( idx<0 || idx >= TConst.Langs ) return null;                    // Chequea que este dentro del rango

      if( Langs[idx] == null )                                           // Si no se ha cargado el idioma
        Langs[idx] = ConjLangData.Load( lang );                          // Lo carga

      return Langs[idx];                                                 // Retorna datos del idioma
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Libera los datos de conjugación para un idioma dado, para que puedan ser recolectados </summary>
    ///<param name="lang">Idioma de conjugación al que se le quieren liberar los datos</param>
    public static void ReleaseLang(TLng lang)
      {
      int idx = (int)lang;                                               // Castea el idioma a un entero
      if( idx>=0 || idx < TConst.Langs )                                 // Chequea que este dentro del rango
        Langs[idx] = null;
      }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Contiene información sobre una conjugación</summary>
  public class ConjInfo
    {
    public string Nombre;      // Nombre de la conjugación
    public TMod   Modo;        // Modo
    public TTime Tiempo;      // Tiempo
    public bool   Compuesto;  // Si es un tiempo compuesto
    }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Contiene todos los datos necesario para la conjugación en un idioma especifico</summary>
  public class ConjLangData
    {
    IDictTrd  Dict;                                       // Diccionario con todos los datos (Solo de uso interno)

    ///<summary>Idioma de la conjugación</summary>
    public TLng Lang {get;set;}                           

    ///<summary>Nombre de las columnas</summary>   // No se usa
    //public List<string>  TabColName = new List<string>(85);
    ///<summary>Indice a la columna</summary>
    public List<ushort>  TabColIndex = new List<ushort>(85);
    ///<summary>Pronombres personales del idioma</summary>
    public List<string>  PronomPerson = new List<string>(6);            
    ///<summary>Pronombres reflexivos del idioma</summary>
    public List<string>  PronomReflex = new List<string>(6);               
    ///<summary>Terminaciones de verbos reflexivos</summary>
    public List<string>  TerminacReflex = new List<string>(3);          
    ///<summary>Terminaciones de verbos que se sustituyen por los reflexivos</summary>
    public List<string>  TerminacVerbal = new List<string>(3);            
    ///<summary>Lista con los datos de las conjugaciones válidas para un idioma</summary>
    public List<ConjInfo> DataConjArray = new List<ConjInfo>(17); 
    ///<summary>Tabla de verbos</summary>
    public Tabla TableVerb = new Tabla();
    ///<summary>Tabla de patrones de verbos</summary>
    public Tabla TablePatron = new Tabla();

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga todos los datos necesario para conjugar verbos en un idioma dado</summary>
    /// <param name="lang">Idioma de los verbos que se quieren conjugar</param>
    public static ConjLangData Load( TLng lang )
      {
      var Name = ConjData.GetDictName( lang );                         // Nombre del diccionarios de conjugación para el idioma
      var dict = ConjData.OpenDictDelegate( Name );                    // Carga el diccionario para el idioma
      if( dict==null ) return null;                                    // No lo pudo abrir, retorna error

      var Data = new ConjLangData();                                   // Crea objeto con datos de conjugación
      Data.Lang = lang;                                                // Le asigna el idioma
      Data.Dict = dict;                                                // Le asigna el diccionario

      if( !Data.GetNameIndex() ) return null;                          // Tabla con Nombres de columnas e indices
      if( !Data.GetTerminac()  ) return null;                          // Tabla con terminaciones verbales y reflexivas
      if( !Data.GetPronom()    ) return null;                          // Tabla con pronombres personales y reflexivos
      if( !Data.GetDataConj()  ) return null;                          // Tabla con datos de conjugaciones aceptadas

      if( !Data.GetTbVerb()    ) return null;                          // Tabla de verbos
      if( !Data.GetTbPatron()  ) return null;                          // Tabla de patrones

      return Data;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga la tabla de Indices y nombres de columnas desde el diccionario</summary>
    public bool GetNameIndex()
      {
      var Tbl = Dict.GetTableData("ColName");
      if( Tbl==null ) 
        {
        Dbg.Msg("No pudo cargar la tabla Indices a las columnas", Dbg.Error);
        return false;
        }

      for( int i=0; i<Tbl.nRow; i++)
        TabColIndex.Add( ushort.Parse(Tbl.Cell(i,1)) );

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Carga la tabla de terminaciones verbales y reflexivas del diccionario</summary>
    public bool GetTerminac()
      {
      var Tbl = Dict.GetTableData("TerminacReflex");
      if( Tbl==null )
        {
        Dbg.Msg("No pudo cargar la tabla de terminaciones", Dbg.Error);
        return false;
        }

      for( int i=0; i<Tbl.nRow; i++)
        {
        TerminacReflex.Add( Tbl.Cell(i,0) );
        TerminacVerbal.Add( Tbl.Cell(i,1) );
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Carga la tabla de pronombres de conjugación del diccionario </summary>
    public bool GetPronom()
      {
      var Tbl = Dict.GetTableData("Pronoms");
      if( Tbl==null ) 
        {
        Dbg.Msg("No pudo cargar la tabla de pronombres", Dbg.Error);
        return false;
        }

      for( int i=0; i<Tbl.nRow; i++)
        {
        PronomPerson.Add( Tbl.Cell(i,0) );
        PronomReflex.Add( Tbl.Cell(i,1) );
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga la tabla con los datos de las conjugaciones válidas para un idioma</summary>
    public bool GetDataConj()
      {
      var Tbl = Dict.GetTableData("DataConj");
      if( Tbl==null ) 
        {
        Dbg.Msg("No pudo cargar los datos de las conjugaciones", Dbg.Error);
        return false;
        }

      for( int i=0; i<Tbl.nRow; i++)
        {
        var CInfo = new ConjInfo();
        CInfo.Nombre    = Tbl.Cell(i,0);
        CInfo.Modo      = (TMod)(int.Parse( Tbl.Cell(i,1) ));
        CInfo.Tiempo    = (TTime)(int.Parse( Tbl.Cell(i,2) ));
        CInfo.Compuesto = ( Tbl.Cell(i,3) != "0" );

        DataConjArray.Add( CInfo );
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga la tabla de verbos</summary>
    public bool GetTbVerb()
      {
      TableVerb = Dict.GetTableData("Verbos");
      if( TableVerb==null ) 
        {
        Dbg.Msg("No pudo cargar la tabla de verbos", Dbg.Error);
        return false;
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga la tabla de patrones</summary>
    public bool GetTbPatron()
      {
      TablePatron = Dict.GetTableData("Patron");
      if( TablePatron==null ) 
        {
        Dbg.Msg("No pudo cargar la tabla de patrones", Dbg.Error);
        return false;
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca una palabra irregular en los datos de conjugación  </summary>
    public string FindIrregWord( string word ) { return Dict.GetStrData( "IW_" + word ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un verbo auxiliar en los datos de conjugación  </summary>
    public string FindAuxVerb( string verb ) { return Dict.GetStrData( "AV_" + verb ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca una forma irregular femenina en los datos de conjugación  </summary>
    public string FindIrregFem( string IregF ) { return Dict.GetStrData( "IF_" + IregF ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un verbo irregular plural en los datos de conjugación  </summary>
    public string FindIrregPlural( string verb ) { return Dict.GetStrData( "IP_" + verb ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un adjetivo irregular superlativo en los datos de conjugación  </summary>
    public string FindIrregSup( string adj ) { return Dict.GetStrData( "IS_" + adj ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un adjetivo comparativo irregular en los datos de conjugación  </summary>
    public string FindIrregComp( string adj ) { return Dict.GetStrData( "IC_" + adj ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un prefijo de un verbo en los datos de conjugación  </summary>
    public string FindPreffixVerb( string verb ) { return Dict.GetStrData( "PV_" + verb ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un nombre declarativo irregular en los datos de conjugación  </summary>
    public string FindIrregDecNoun( string noun ) { return Dict.GetStrData( "IN_" + noun ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un adjetivo decalarativo irregular en los datos de conjugación  </summary>
    public string FindIrregDecAdj( string adj ) { return Dict.GetStrData( "IA_" + adj ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un adjetivo sufijo superlativo en los datos de conjugación  </summary>
    public string Find_SuffixSup( string adj ) { return FindSuffix( adj, "SS_" );  }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un adjetivo sufijo comparativo en los datos de conjugación  </summary>
    public string Find_SuffixComp( string adj ) { return FindSuffix( adj, "SC_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo femenino en los datos de conjugación  </summary>
    public string Find_SuffixFem( string suf ) { return FindSuffix( suf, "SF_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo plural en los datos de conjugación  </summary>
    public string Find_SuffixPlural( string suf ) { return FindSuffix( suf, "SP_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo de nombre decalrativo en los datos de conjugación  </summary>
    public string Find_SuffixDecNoun( string noun ) { return FindSuffix( noun, "SN_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo declarativo del adjetivo en los datos de conjugación  </summary>
    public string Find_SuffixDecAdj( string adj ) { return FindSuffix( adj, "SA_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo en los datos de conjugación  </summary>
    public string Find_Suffix( string suf ) { return FindSuffix( suf, "Sx_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un sufijo de la palabra 'word'</summary>
    private string FindSuffix( string word, string FId )
      {
      for( int i=0; i<word.Length; ++i )
        {
        var Suff = Dict.GetStrData( FId + word.Substring(i) );
        if( Suff != null ) return Suff;
        }

      return null;
      }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
  }
