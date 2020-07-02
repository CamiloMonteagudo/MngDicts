using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TrdEngine.Dictionary;
using System.Reflection;

namespace TrdEngine.Data
  {
  //*******************************************************************************************************************
  ///<summary>Identificador de los diccionarios de una dirección de traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum DDirId
    {             
    ///<summary>Diccionario general de palabras</summary>
    Gen = 0,
    ///<summary>Diccionario de datos de la dirección de traducción</summary>
    Data = 1,      
    ///<summary>Reglas para la traducción de oraciones</summary>
    CT  = 2,      
    ///<summary>Reglas de traducción de oraciones interrogativas</summary>
    QCT = 3,      
    ///<summary>Reglas de traducción de oraciones imperativas</summary>
    ICT = 4,      
    ///<summary>Reglas para la traducción de frases sustantivas</summary>
    FCT = 5,      
    ///<summary>Reglas de traducción de frases verbales</summary>
    VCT = 6,      
    ///<summary>Reglas para la traducción de frases sustantivas primarias</summary>
    TCT = 7,
    ///<summary>Reglas en frases de conjugación y coordinación</summary>
    CC  = 8,     
    ///<summary>Diccionario de especialidad</summary>
    Commerce = 9,     
    ///<summary>Diccionario de especialidad</summary>
    Computer = 10,     
    ///<summary>Diccionario de especialidad</summary>
    Law = 11,     
    ///<summary>Diccionario de especialidad</summary>
    Medicine = 12,     
    ///<summary>Diccionario de especialidad</summary>
    SocialSecurity = 13
    }

  //**********************************************************************************************************************************
  ///<summary>Identificador de los diccionarios asociados a un lenguaje de traducción</summary>
  ///-----------------------------------------------------------------------------------------------------------------------------------
  public enum DLangId
    {                    
    ///<summary>Diccionario de datos del lenguaje de traducción         </summary>
    Data        = 0,    
    ///<summary>Diccionario de nombres propios                          </summary>
    PNoun       = 1,
    ///<summary>Reglas de Buscar función gramatical de Imperativas      </summary>
    IGrFun      = 2,     
    ///<summary>Reglas de Buscar nombres propios                        </summary>
    PrNoun      = 3,     
    ///<summary>Reglas de Buscar función gramatical de Interrogativas   </summary>
    QGrFun      = 4,     
    ///<summary>Reglas de Buscar función gramatical                     </summary>
    SGrFun      = 5,     
    ///<summary>Reglas de Buscar función gramatical ???                 </summary>
    TGrFun      = 6     
    }


  //*************************************************************************************************************************************
  /// <summary> Maneja todos los datos necesario para traduccir </summary>
  /// <remarks>Maneja todos los diccionarios y extruras de datos usados en la traducción, esto incluye todas las direcciones e idiomas 
  /// sorportados, pero estos solo se cargaran a memoria cuando sean solicitados</remarks>
  static public class TrdData
    {
    static string DPath = "";

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Delegado que se va a usar para abrir los diccionarios</summary>
    static public HndOpenTrdDict OpenDictDelegate = OpenDictSingle;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Datos para todas las direcciones de traducción </summary>
    static private TDirData[] Dirs  =  new TDirData[TConst.Dirs];

    ///-----------------------------------------------------------------------------------------------------------------------------------      
    /// <summary> Datos para todos los lenguajes  </summary>
    static private TLangData[] Langs =  new TLangData[TConst.Langs];

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Localización donde se encuentran los diccionario</summary>
    static public string DicsPath
      { 
      get {
          if( DPath.Length>0 ) return DPath;

          var AppDir = Assembly.GetExecutingAssembly().Location;        // Obtiene la localización del ensamblado
              AppDir = Path.GetDirectoryName(AppDir);                   // Quita el nombre del ensamblado

          while( AppDir != null )                                       // Busca en todos los directorios de camino
            {
            if( Directory.Exists( AppDir + @"\Dictionaries\" ) )        // Si existe el directorio de diccionarios
              {
              DPath = AppDir + @"\Dictionaries\";                       // Si existe lo toma y termina
              break;
              }

            AppDir = Path.GetDirectoryName(AppDir);                     // Quita el ultimo directorio
            }

          return DPath;                                                 // Retorna camino donde encontro o cadena vacia
          } 
      set { 
          DPath = value.TrimEnd('\\') + '\\';
          } 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos de traducción asocidos a una dirección</summary>
    public static TDirData GetDirData( TDir Dir )
      {
      int idx = (int)Dir;
      if( idx<0 || idx >= TConst.Dirs ) return null;

      if( Dirs[idx] == null ) 
        Dirs[idx] = new TDirData(Dir);

      return Dirs[idx];
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos de traducción asocidos a un idioma</summary>
    public static TLangData GetLangData(TLng lang)
      {
      int idx = (int)lang;
      if( idx<0 || idx >= TConst.Langs ) return null;

      if( Langs[idx] == null ) 
        Langs[idx] = new TLangData(lang);

      return Langs[idx];
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asigna un diccionario a una dirección determinada  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="DictId">Identificador del diccionario</param>
    /// <param name="src">Idioma de origen de la dirección de traducción donde se va a asignar</param>
    /// <param name="des">Idioma destino de la dirección de traducción donde se va a asignar</param>
    static public bool SetDict( IDictTrd Dict, DDirId DictId, TLng src, TLng des )
      {
      return SetDict( Dict, DictId, Utils.ToDir(src,des) );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asigna un diccionario a una dirección de traducción  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="DictId">Identificador del diccionario</param>
    /// <param name="Dir">Dirección de traducción donde se asingará el diccionario</param>
    static public bool SetDict( IDictTrd Dict, DDirId DictId, TDir Dir )
      {
      int idx = (int)DictId;
      if( idx<0 || idx >= TConst.Dirs ) return false;

      if( Dirs[idx] == null ) 
        Dirs[idx] = new TDirData(Dir);

      return Dirs[idx].SetDict( Dict, DictId );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asigna un diccionario a un idioma  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="DictId">Identificador del diccionario</param>
    /// <param name="lang">Idioma al que se le quiere asignar</param>
    static public bool SetDict( IDictTrd Dict, DLangId DictId, TLng lang )
      {
      int idx = (int)lang;
      if( idx<0 || idx >= TConst.Langs ) return false;

      if( Langs[idx] == null ) 
        Langs[idx] = new TLangData(lang);

      return Langs[idx].SetDict( Dict, DictId );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adicciona un diccionario a una dirección de traducción  </summary>
    /// <param name="Dict">Diccionario que se quiere adicionar</param>
    /// <param name="src">Idioma de origen de la dirección de traducción donde se va a asignar</param>
    /// <param name="des">Idioma destino de la dirección de traducción donde se va a asignar</param>
    /// <returns>Returna al identificador del diccionario si de pudo adicionar, de lo contrario -1</returns>
    static public DDirId AddDict( IDictTrd Dict, TLng src, TLng des )
      {
      return AddDict( Dict, Utils.ToDir(src,des) );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adicciona un diccionario a una dirección de traducción  </summary>
    /// <param name="Dict">Diccionario que se quiere adicionar</param>
    /// <param name="Dir">Dirección de traducción donde se adicionará el diccionario</param>
    /// <returns>Returna al identificador del diccionario si de pudo adicionar, de lo contrario -1</returns>
    static public DDirId AddDict( IDictTrd Dict, TDir Dir )
      {
      int idx = (int)Dir;
      if( idx<0 || idx >= TConst.Dirs ) return (DDirId)(-1);

      if( Dirs[idx] == null ) 
        Dirs[idx] = new TDirData(Dir);

      return Dirs[idx].AddDict( Dict );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un diccionario a un idioma  </summary>
    /// <param name="Dict">Diccionario que se quiere asignar</param>
    /// <param name="lang">Idioma al que se le quiere asignar</param>
    /// <returns>Returna al identificador del diccionario si de pudo adicionar, de lo contrario -1</returns>
    static public DLangId AddDict( IDictTrd Dict, TLng lang )
      {
      int idx = (int)lang;
      if( idx<0 || idx >= TConst.Langs ) return (DLangId)(-1);

      if( Langs[idx]==null ) Langs[idx] = new TLangData(lang);

      return Langs[idx].AddDict( Dict );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si una dirección de traducción esta instalada</summary>
    ///<param name="Dir">Idioma origen de la dirección de traducción</param>
    ///<returns>Verdadero si la dirección esta instalada y falso si no</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public bool IsInstalled(TDir Dir)
      {
      return File.Exists( GetDictName( DDirId.Gen, Dir ) );                                           
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si una dirección de traducción esta instalada</summary>
    ///<param name="src">Idioma origen de la dirección de traducción</param>
    ///<param name="des"  >Idioma destino de la dirección de traducción</param>
    ///<returns>Verdadero si la dirección esta instalada y falso si no</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public bool IsInstalled(TLng src, TLng des)
      {
      return File.Exists( GetDictName( DDirId.Gen, src,des ) );                                           
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre de un diccionario asociado a una dirección de traducción</summary>
    /// <param name="DictId">Identificador del diccionario</param>
    /// <param name="src">Idioma de origen de la dirección de traducción a que pertenece</param>
    /// <param name="des">Idioma destino de la dirección de traducción a que pertenece</param>
    static public string GetDictName( DDirId DictId, TLng src, TLng des )
      {
      if( (int)DictId < 9 )
        return DicsPath + src + "2" + des + DictId + ".dcb";
      else
        return DicsPath + "Sp" + src + "2" + des + DictId + ".dcb";
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre de un diccionario asociado a una dirección de traducción</summary>
    /// <param name="DictId">Identificador del diccionario solicitado</param>
    /// <param name="Dir">Dirección de traducción donde se encuentra el diccionario</param>
    static public string GetDictName( DDirId DictId, TDir Dir )
      {
      int idx = (int)Dir;
      if( idx<0 || idx >= TConst.Dirs ) return "";

      return GetDictName( DictId, Utils.Src(Dir), Utils.Des(Dir) ); 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre de un diccionario asociado a un idioma  </summary>
    /// <param name="DictId">Identificador del diccionario solicitado</param>
    /// <param name="lang">Idioma al que pertenece</param>
    static public string GetDictName( DLangId DictId, TLng lang )
      {
      return DicsPath +  lang + DictId + ".dcb";
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Libera todos los datos de una dirección de traducción</summary>
    /// <param name="src">Idioma de origen de la dirección de traducción</param>
    /// <param name="des">Idioma destino de la dirección de traducción</param>
    static public void Free( TLng src, TLng des )
      {
      Free( Utils.ToDir(src,des) );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Libera todos los datos de una dirección de traducción</summary>
    /// <param name="Dir">Dirección de traducción que se quiere liberar</param>
    static public void Free( TDir Dir )
      {
      int idx = (int)Dir;
      if( idx>=0 && idx<TConst.Dirs ) Dirs[idx] = null; 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Libera todos los datos de un idioma</summary>
    /// <param name="lang">Idioma al que se van a liberar los datos</param>
    static public void Free( TLng lang )
      {
      int idx = (int)lang;
      if( idx>=0 && idx<TConst.Langs ) 
        {
        Langs[idx] = null;
        ConjData.ReleaseLang(lang);
        RedWData.ReleaseLang(lang);
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Libera todos los datos de todos los idioma y todas las direcciones</summary>
    static public void Free()
      {
      Dirs  =  new TDirData [TConst.Dirs ];
      Langs =  new TLangData[TConst.Langs];

      ConjData.Langs = new ConjLangData[TConst.Langs];
      RedWData.Langs = new RedLangData [TConst.Langs];
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Abre los diccionarios implementados enteramente en codigo manejado (.Net)</summary>
    /// <param name="DictName">Camino completo desde donde se puede cargar el diccionario</param>
    static public IDictTrd OpenDictSingle( string DictName )
      {
      return DictSingle.Open( DictName );
      }

    } // ++++++++++++++++++++++++++++++++++++++++++++++++ Fin de la clase TrdData ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

}     // ++++++++++++++++++++++++++++++++++++++++++++++++ Fin del namespace       ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
