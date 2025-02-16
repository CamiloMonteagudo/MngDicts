﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using TrdEngine.Dictionary;

namespace TrdEngine.Data
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Contiene todos los datos necesario para la Reducción</summary>
  static public class RedWData
    {
    static string DPath = "";

    ///-----------------------------------------------------------------------------------------------------------------------------------      
    /// <summary> Datos para las reducciones en todos los idioma </summary>
    static public RedLangData[] Langs =  new RedLangData[TConst.Langs];

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Localización donde se encuentran los diccionario</summary>
    static public string DicPath{ get{return DPath;} set{ DPath = value.TrimEnd('\\') + '\\';} }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Delegado que se va a usar para abrir los diccionarios</summary>
    static public HndOpenTrdDict OpenDictDelegate = OpenDictSingle;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Constructor estatico que incializa los datos la primera vez que se usa la clase</summary>
    static RedWData()
      {
      var Loc = Assembly.GetExecutingAssembly().Location;           // Obtiene la localización del ensamblado
      DicPath = Path.GetDirectoryName(Loc) + "\\Dictionaries";      // Toma directorio 'Dictionaries' respecto al ensamblado
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre del diccionario de reducción asociado a un idioma  </summary>
    /// <param name="lang">Idioma para el cual se obtiene el nombre</param>
    static public string GetDictName( TLng lang )
      {
      return DicPath + lang + "RedWord.dcb";
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Manipulador por defecto para abrir el diccionario (Diccionario binario)</summary>
    /// <param name="DictName">Camino y nombre completo del diccionario</param>
    static public IDictTrd OpenDictSingle( string DictName )
      {
      return DictSingle.Open( DictName );

//      return DictSingle.OpenXml( Path.ChangeExtension( GetDictName(LangId),"xml") );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos para la reducción en un idioma </summary>
    /// <param name="lang">Idioma de reducción del que se quieren obtener los datos</param>
    static public RedLangData GetLang( TLng lang )
      {
      int idx = (int)lang;                                               // Castea el idioma a un entero
      if( idx<0 || idx >= TConst.Langs ) return null;                    // Chequea que este dentro del rango

      if( Langs[idx] == null )                                           // Si no se ha cargado el idioma
        Langs[idx] = RedLangData.Load( lang );                          // Lo carga

      return Langs[idx];                                                 // Retorna datos del idioma
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Libera los datos de reducción para un idioma dado, para que puedan ser recolectados </summary>
    ///<param name="lang">Idioma de reducción al que se le quieren liberar los datos</param>
    public static void ReleaseLang(TLng lang)
      {
      int idx = (int)lang;                                               // Castea el idioma a un entero
      if( idx>=0 || idx < TConst.Langs )                                 // Chequea que este dentro del rango
        Langs[idx] = null;
      }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Contiene todos los datos necesario para la reducción en un idioma especifico</summary>
  public class RedLangData
    {
    IDictTrd  Dict;                                       // Diccionario con todos los datos de reducción (Solo de uso interno)

    ///<summary>Idioma de la reducción</summary>
    public TLng Lang {get;set;}                           

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga todos los datos necesario para conjugar verbos en un idioma dado</summary>
    /// <param name="lang">Idioma de los verbos que se quieren conjugar</param>
    public static RedLangData Load( TLng lang )
      {
      var DictName = RedWData.GetDictName( lang );                        // Obtiene el nombre del diccionario
      var dict = RedWData.OpenDictDelegate( DictName );                   // Carga el diccionario para el idioma
      if( dict==null ) return null;                                       // No lo pudo abrir, retorna error

      var Data = new RedLangData();                                       // Crea objeto de datos de reducción para el idioma
      Data.Lang = lang;                                                   // Le pone el idioma
      Data.Dict = dict;                                                   // Le pone el diccionario de reducción

      return Data;                                                        // Retorna datos de reducción para el idioma
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un diccionario general, según el idioma de reducción</summary>
    public IDictTrd GetDictGen( TLng des )
      {
      if( des != TLng.NA )
        {
        var DData = TrdData.GetDirData( Utils.ToDir(Lang,des) );          // Obtiene los datos para la dirección de traducción
        var Dict  = DData.GetDict( DDirId.Gen );
        if( Dict!=null ) return Dict;
        }
      else
        {
        for( int i=0; i<TConst.Langs; ++i )                               // Busca para todos los idiomas destinos
          {
          des = (TLng)i;
          if( des==Lang ) continue;                                       // Si los dos idiomas son iguales, lo salta

          var DData = TrdData.GetDirData( Utils.ToDir(Lang,des) );        // Obtiene los datos para la dirección de traducción
          if( DData != null )                                             // Si pudo obtener la dirección
            {
            var Dict  = DData.GetDict( DDirId.Gen );                      // Abre el diccionario general
            if( Dict!=null ) return Dict;                                 // Si lo encontro, lo retrona
            }
          }
        }

      return null;                                                        // Con todos los idiomas como destino, no se pudo
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sust. o adj. irregular en los datos de reducción</summary>
    public string FindIrregNoun( string word ) { return Dict.GetStrData( "SI_" + word ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un verbo irregular en los datos de reducción  </summary>
    public string FindIrregVerb( string verb ) { return Dict.GetStrData( "VI_" + verb ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo verbal en los datos de reducción  </summary>
    public string FindVerbSuffix( string suff ) { return Dict.GetStrData( "SV_" + suff ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo de sustantivo o adjetivo en los datos de reducción  </summary>
    public string FindNounSuffix( string sust ) { return Dict.GetStrData( "SS_" + sust ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un prefijo con traducción en los datos de reducción  </summary>
    public string FindPreffix( string pref ) { return Dict.GetStrData( "PD_" + pref ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un prefijo para la palabra word, si lo encuentra retorna null </summary>
    public string Find_Preffix( string word ) { return FindPreffix( word, "PD_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un sufijo para un verbo, si lo encuentra retorna null </summary>
    public string Find_VerbSuffix( string verb ) { return FindSuffix( verb, "SV_" ); }

    ///<summary> Busca un sufijo para sustantivo o adjetivo, si lo encuentra retorna null </summary>
    public string Find_NounSuffix( string sust ) { return FindSuffix( sust, "SS_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un sufijo de la palabra 'word'</summary>
    private string FindSuffix( string word, string FId )
      {
      for( int i=0; i<word.Length; ++i )
        {
        var Suff = word.Substring(i);
        if( Dict.IsKey( FId + Suff ) ) return Suff;
        }

      return null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un prefijo de la palabra 'word', 'FId' el identificador del tipo de diccionario</summary>
    private string FindPreffix( string word, string FId )
      {
      int n = word.Length;
      for( int i=n; i>0; --i )
        {
        var Preff = word.Substring( 0, n-i );
        if( Dict.IsKey( FId + Preff ) ) return Preff;
        }

      return null;
      }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    }
  }
