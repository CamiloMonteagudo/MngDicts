using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Dictionary;

namespace TrdEngine.Data
  {
  /// <summary>Datos para la traducción especificos de un idioma</summary>
  /// <remarks>Esta clase almacena todos los diccionarios y extructuras de datos necesarios en la traducción, que son especificas de un idioma determinado</remarks>
  public class TLangData
    {
    /// <summary>Arreglo de diccionarios de traducción especificos de un idioma de traducción</summary>
    private List<IDictTrd> Dicts = new List<IDictTrd>(TConst.DictsLang);

    /// <summary>Datos albitrarios que se asocian a un idioma de traducción</summary>
    private Dictionary<string, object> Datos = new Dictionary<string, object>();

    /// <summary> Idioma de traducción a que pertenecen los datos (Solo lectura) </summary>
    public TLng Lang { get; private set; } 
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Constructor del objeto para mantener los datos de un idioma de traducción</summary>
    /// <param name="lang">Idioma de traducción de los datos</param>
    public TLangData( TLng lang )
      {
      this.Lang = lang;

      for( int i=0; i<TConst.DictsLang; ++i )
        Dicts.Add(null);
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un diccionario para un Idioma determinado  </summary>
    /// <param name="DictId">Identificador del diccionario</param>
    public IDictTrd GetDict( DLangId DictId )
      {
      int Idx = (int)DictId;                                            // Convierte identicador a indice
      if( Idx >= Dicts.Count ) return null;                             // El indice esta fuera de rango, retorna null

      if( Dicts[Idx] != null ) return Dicts[Idx];                       // Si ya esta cargado, lo retorna

      if( Idx >= TConst.DictsLang ) return null;                        // No es un diccionario estandar, no sabe como cargarlo

      var DicName = TrdData.GetDictName( DictId, Lang );                // Obtiene el nombre del diccionario
      Dicts[Idx]  = TrdData.OpenDictDelegate( DicName );                // Abre el diccionario y lo guarda

      if( Dicts[Idx] == null )                                          // No se pudo cargar
        Dbg.Msg("WARING: Cargando diccionario "+DicName );              // Mensaje de avertencia (solo en modo debug) 

      return Dicts[Idx];                                                // Retorna el diccionario abierto, null sin no se pudo
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna si el diccionario 'DictId' esta cargado en memoria o no </summary>
    public bool IsLoaded( DLangId DictId )
      {
      int Idx = (int)DictId;                                            // Convierte identicador a indice
      return ( Idx<Dicts.Count && Dicts[Idx]!=null );                      
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asigna un diccionario a un Idioma determinado  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="DictId">Identificador del diccionario</param>
    public bool SetDict( IDictTrd Dict, DLangId DictId)
      {
      if( (int)DictId >= Dicts.Count ) return false;

      Dicts[(int)DictId] = Dict;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un tipo diccionario a un Idioma determinado </summary>
    /// <param name="Dict">Diccionario que se quiere adicionar</param>
    public DLangId AddDict( IDictTrd Dict)
      {
      Dicts.Add(Dict);
        
      return (DLangId) Dicts.Count-1;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un dato asociado al idioma de traducción</summary>
    /// <param name="DataId">Cadena de caracteres que identifica al dato</param>
    public object GetData( string DataId )
      {
      object Data = null;
      Datos.TryGetValue( DataId, out Data );
      return Data;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asocia un dato a un idioma de traducción </summary>
    /// <param name="DataId">Cadena de caracteres que identifica al dato</param>
    /// <param name="Value">Valor asociado al dato que se quiere asignar</param>
    public void SetData( string DataId, object Value )
      {
      if( Value==null )
        {
        Datos.Remove( DataId );
        return;
        }

      Datos[DataId] = Value;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un arreglo con el nombre de todos los datos asociados al idioma </summary>
    public string[] GetDataNames()
      {
      return Datos.Keys.ToArray();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un dato especifico el diccionario de datos del lenguaje, opcionalmente se puede usar un prefijo para diferenciar
    ///los diferentes subconjutos de datos ///</summary>
    public string FindInLangDict( string sKey, string suff=null  ) 
      { 
      if( suff != null ) sKey = suff + sKey;                          // Si se especifico un prefijo lo adiciona

      int id = (int)DLangId.Data;                                     // Obtiene identificador de datos del idioma
      if( Dicts[id]==null && GetDict(DLangId.Data)==null )            // Si el diccionario no esta cargado, lo carga
        return null;                                                  // No pudo cargarlo, retrona palabra no encontrada

      return Dicts[id].GetStrData( sKey );                            // Busca la palabra y la retorna o null si no la encuentra
      }
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene una tabla de datos de los datos del lenguaje (Nombres: Details, ProperNounGender, Contractions, SplitInWords) </summary>
    public Tabla GetLangTable( string TableName  ) 
      { 
      int id = (int)DLangId.Data;
      if( Dicts[id]==null && GetDict(DLangId.Data)==null )  
        return null;

      return Dicts[id].GetTableData( TableName ); 
      }
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene un objeto para la conjugación de verbos del idioma</summary>
    public ConjVerb GetConj() 
      { 
      ConjData.OpenDictDelegate = TrdData.OpenDictDelegate;             // Utiliza la misma función para abrir los diccionarios
      return ConjVerb.GetConj( Lang, TrdData.DicsPath );                // Usa el mismo camino para diccionarios de conjugación
      }

    } // +++++++++++++++++++++++++++++++++++++++++++++ Fin de la clase TrdLangsData ++++++++++++++++++++++++++++++++++++++++++++++++++++++

}     // ++++++++++++++++++++++++++++++++++++++++++++++++ Fin del namespace       ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
