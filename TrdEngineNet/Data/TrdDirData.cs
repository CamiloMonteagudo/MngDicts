using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine;

namespace TrdEngine.Data
  {
  ///-------------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Datos para la traducción especificos de una dirección  de traducción</summary>
  /// <remarks>Esta clase almacena todos los diccionarios y extructuras de datos necesarios en la traducción que son especificas de una dirección de traducción determinada</remarks>
  public class TDirData
    {
    /// <summary>Arreglo de diccionarios de traducción especificos de una dirección de traducción</summary>
    public List<IDictTrd> Dicts = new List<IDictTrd>(TConst.DictsDir);

    /// <summary>Datos albitrarios que se asocian a una dirección de traducción</summary>
    public Dictionary<string, object> Datos = new Dictionary<string, object>();

    /// <summary> Dirección de traducción a que pertenecen los datos (Solo lectura) </summary>
    public TDir Dir { get; private set; } 

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Constructor del objeto para mantener los datos de una dirección de traducción</summary>
    /// <param name="Dir">Dirección de traducción de los datos</param>
    public TDirData( TDir Dir )
      {
      this.Dir = Dir;                                                   // Establece la dirección

      for( int i=0; i<TConst.DictsDir; ++i )                            // Inicializa todos los diccionarios en null
        Dicts.Add(null);
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un diccionario para una dirección determinada  </summary>
    /// <param name="DictId">Identificador del diccionario</param>
    public IDictTrd GetDict( DDirId DictId )
      {
      int Idx = (int)DictId;                                            // Convierte identificador a indice
      if( Idx >= Dicts.Count ) return null;                             // No es un indice valido, retorna null

      if( Dicts[Idx] != null ) return Dicts[Idx];                       // Si ya esta cargado, lo devuelve

      if( Idx >= TConst.DictsDir ) return null;                         // No es un diccionario estandar, no sabe como cargarlo

      var DicName = TrdData.GetDictName( DictId, Dir );                 // Obtiene el nombre del diccionario
      Dicts[Idx]  = TrdData.OpenDictDelegate( DicName );                // Abre el diccionario y lo guarda

      if( Dicts[Idx] == null )                                          // No se pudo cargar
        Dbg.Msg("WARING: Cargando diccionario "+DicName );              // Mensaje de avertencia (solo en modo debug) 

      return Dicts[Idx];                                                // Retorna al diccionario cargado o null si no se pudo
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Determina si el diccionario 'dDirId' esta cargado en memoria o no  </summary>
    public bool IsLoaded(DDirId dDirId)
      {
      int Idx = (int)dDirId;                                            // Convierte identificador a indice
      return ( Idx<Dicts.Count && Dicts[Idx]!=null );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asigna un diccionario a una dirección determinada  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="DictId">Identificador del diccionario</param>
    public bool SetDict( IDictTrd Dict, DDirId DictId)
      {
      if( (int)DictId >= Dicts.Count ) return false;

      Dicts[(int)DictId] = Dict;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un tipo diccionario a un Idioma determinado </summary>
    /// <param name="Dict">Diccionario que se quiere adicionar</param>
    public DDirId AddDict( IDictTrd Dict)
      {
      Dicts.Add(Dict);
        
      return (DDirId) Dicts.Count-1;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un dato asociado a la dirección de traducción</summary>
    /// <param name="DataId">Cadena de caracteres que identifica al dato</param>
    public object GetData( string DataId )
      {
      object Data = null;
      Datos.TryGetValue( DataId, out Data );
      return Data;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asocia un dato a una dirección de traducción </summary>
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
    /// <summary> Obtiene un arreglo con el nombre de todos los datos asociados la dirección </summary>
    public string[] GetDataNames()
      {
      return Datos.Keys.ToArray();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto para la reducción de palabras para la dirección</summary>
    public RedWord GetReduc() 
      { 
      var Src = Utils.Src(Dir);
      var Des = Utils.Des(Dir);

      RedWData.OpenDictDelegate = TrdData.OpenDictDelegate;           // Utiliza la misma función para abrir los diccionarios
      return RedWord.GetReduc( Src, Des, TrdData.DicsPath );          // Usa el mismo camino para diccionarios de reducción
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un dato especifico el diccionario de datos de la dirección de traducción, opcionalmente se puede usar un prefijo 
    ///para diferenciar los diferentes subconjutos de datos ///</summary>
    public string FindInDirDict( string sKey, string suff=null  ) 
      { 
      if( suff != null ) sKey = suff + sKey;                          // Si se especifico un prefijo lo adiciona

      int id = (int)DDirId.Data;                                      // Obtiene identificador de datos de la dirección
      if( Dicts[id]==null && GetDict(DDirId.Data)==null )             // Si el diccionario no esta cargado, lo carga
        return null;                                                  // No pudo cargarlo, retrona palabra no encontrada

      return Dicts[id].GetStrData( sKey );                            // Busca la palabra y la retorna o null si no la encuentra
      }
    
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una palabra en el diccionario de sufijos de traducción</summary>
    public string FindTrdSuffix( string suff ) { return FindInDirDict( suff, "SF_" ); }
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una palabra en el diccionario de prefijos de traducción</summary>
    public string FindTrdPreffix(string pref) { return FindInDirDict( pref, "PF_" ); }
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca en los datos de traducción palabras para insertar o adicionar</summary>
    public string FindTrdData(string data) { return FindInDirDict( data, "DT_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un sufijo de la palabra 'Key' en el diccionario de sufijos de traducción</summary>
    public string FindTrd_Suffix(string key)
      {
      for( int i=0; i<key.Length; ++i )
        {
        var Suff = key.Substring(i);
        if( FindInDirDict( Suff, "SF_" ) != null ) return Suff;
        }

      return null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un prefijo de la palabra 'Key' en el diccionario de prefijos de traducción</summary>
    public string FindTrd_Preffix(string key)
      {
      int n = key.Length;
      for( int i=n; i>0; --i )
        {
        var Preff = key.Substring( 0, n-i );
        if( FindInDirDict( Preff, "PF_" ) != null ) return Preff;
        }

      return null;
      }

    } // +++++++++++++++++++++++++++++++++++++++++++++ Fin de la clase TrdDirsData +++++++++++++++++++++++++++++++++++++++++++++++++++++++

}     // +++++++++++++++++++++++++++++++++++++++++++++ Fin del namespace           +++++++++++++++++++++++++++++++++++++++++++++++++++++++
