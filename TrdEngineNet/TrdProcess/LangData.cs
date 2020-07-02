using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena toda la información de la traducción asociada a un idioma (fuente o destino)</summary>
  public class LangData
    {
    public TLng Lang{ get; set; }
    public TLangData LngData = null;                    // Datos estaticos relacionados con el idioma 

    public ConjVerb Conj = null;

    public RegexArray Contractions ;
//    public RegexArray SplitInWords ;
    public RegexArray Details      ;

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicializa el objeto con los datos por defecto</summary>
    public LangData()
      {
      Inicialize();
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga uno de los diccionarios asociados al idioma de traducción</summary>
    public IDictTrd GetDict( DLangId Id ) { return LngData.GetDict( Id ); }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga los datos de la traducción asocidos a un idioma</summary>
    /// <param name="lang">Idioma del que se quieren cargar los datos</param>
    /// <param name="target">Especifica si se trata del idioma destino</param>
    public bool Load( TLng lang, bool target )
      {
      Lang    = lang;
      LngData = TrdData.GetLangData(lang);
      if( LngData==null ) return false;

      Conj = LngData.GetConj();
      if( Conj == null ) return false;


      if( GetDict( DLangId.Data )==null ) return false;

      if( target )
        {
        Details = LoadRegexArray( "Details", true , false );
        }
      else
        {
        GetDict( DLangId.PNoun );                                       // Nombres propios

        if( GetDict(DLangId.SGrFun  )==null ) return false;      
        if( GetDict(DLangId.QGrFun  )==null ) return false;      
        if( GetDict(DLangId.IGrFun  )==null ) return false;      
        if( GetDict(DLangId.TGrFun  )==null ) return false;      
        if( GetDict(DLangId.PrNoun  )==null ) return false;    
  
        Contractions = LoadRegexArray( "Contractions", true , false );
//        SplitInWords = LoadRegexArray( "SplitInWords", false, true  );
        }

      // Esto es solo a modo de prueba
      var VerbalType                      = FindStrData("VERBO");                   // Se usa en el comando ISLNDDSET del diccionario   
      var AdjectiveType                   = FindStrData("ADJETIVO");                        
      var AdverbialType                   = FindStrData("ADVERBIO");                        
      var ArticleType                     = FindStrData("ARTICULO");                        
      var TypeWithSpaceBeforeWord         = FindStrData("ESPACIO1");                        
      var TypeWithSpaceAfterWord          = FindStrData("ESPACIO2");                        
      var GerundType                      = FindStrData("GERUNDIO");                
      var TypeWithFirstLetterInUpperCase  = FindStrData("MAYUSCULA");                        
      var ParticipleType                  = FindStrData("PARTICIPIO");                       
      var PastType                        = FindStrData("PASADO");                      
      var PosesiveType                    = FindStrData("POSESIVO");                
      var PrepositionType                 = FindStrData("PREPOSICION");                   
      var ProNounType                     = FindStrData("PRONOMBRE");                       
      var NounType                        = FindStrData("SUSTANTIVO");                    
      var FemaleType                      = FindStrData("TIPOFEMENINO");            
      var PluralType                      = FindStrData("TIPOPLURAL");                 
      var TypeThatNotNeedReduction        = FindStrData("TIPOSINREDUCCION");        // Se usa en FindWordInDict        
      var FemaleTypeSiAsterisco           = FindStrData("FEMENINOSIASTERISCO");     
      var PluralTypeSiAsterisco           = FindStrData("PLURALSIASTERISCO");                 
      var TiposPermitidosEnComodin        = FindStrData("TIPOSENCOMODIN");          // Usada en FindBracketPhrase     
      var SemanticaPermitidaEnComodin     = FindStrData("SEMANTICAENCOMODIN");      


      var PastOrParticipleType            = FindStrData("PPARTICIPIO");             // Lo que esta en el diccionario es "POPARTICIPIO"             
      var GradeType                       = FindStrData("GRADE");                   // Esta cadena no esta en el diccionario                   
      var TypeNounDeclination             = FindStrData("TIPOSSDECLINACION");       // Esta cadena no esta en el diccionario         
      var TypeAdjDeclination              = FindStrData("TIPOAADECLINACION");       // Esta cadena no esta en el diccionario              

      // "ABR" esta en el diccionario pero no se usa en la traducción

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga un arreglo de expresiones regulares a memoria, si todavia no se ha cargado</summary>
    public RegexArray LoadRegexArray( string Name, bool replace, bool retIndex )
      {
      var Data = LngData.GetData( Name );                             // Trata de leerlo desde los datos del idioma
      if( Data != null )                                              // Lo pudo leer
        return (RegexArray)Data;                                      // Toma los datos y los retorna

      var REArray = new RegexArray();                                 // Crea un arreglo de expresiones nuevo
      REArray.Load( Lang, Name, replace, retIndex );                  // Carga los datos desde el diccionario
      LngData.SetData( Name, REArray );                               // Lo guarda en los datos del idioma, para uso futuro

      return REArray;                                                 // Retorna el arreglo
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Libera todos los datos asociado a un idioma</summary>
    public void Close()
      {
      Inicialize();
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicializa todos los datos del objeto</summary>
    public void Inicialize()
      {
      Lang    = TLng.NA;
      LngData = null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca en los datos del idioma una cadena de datos mediante un identificador</summary>
    public string FindStrData( string IdData ) 
      { 
      var ret = LngData.FindInLangDict( IdData ); 
      return ( ret!=null )? ret: ""; 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca si 'word' esta definida como una palabra especial en los datos del idioma</summary>
    public bool FindEspecialWord( string word ) { return (LngData.FindInLangDict( word, "SW_" )!=null); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca el patron para un tipo dado definido en los datos del idioma</summary>
    public string FindTypeAndPattern( string word ) { return LngData.FindInLangDict( word, "TP_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca lo datos asocidos al prefijo de reducción 'Pref' en los datos del idioma</summary>
    public string FindPreffixReduction( string Pref ) { return LngData.FindInLangDict( Pref, "PR_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca lo datos asocidos al sufijo de reducción 'Suff' en los datos del idioma</summary>
    public string FindSuffixReduction( string Suff ) { return LngData.FindInLangDict( Suff, "SR_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca lo datos asocidos a la reducción de la palabra 'word' en los datos del idioma</summary>
    public string FindWordReduction( string word ) { return LngData.FindInLangDict( word, "WR_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca lo datos asocidos a la palabra iregular 'Word' en los datos del idioma</summary>
    public string FindIrrWords( string Word ) { return LngData.FindInLangDict( Word, "IW_" ); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Concuerda un adjetivo en un grado determinado</summary>
    public string PutAdjectiveInGrade( string sAdj, TGrad eGrade)
      {
      return Conj.ConcordWords( sAdj, TGen.Masc, TNum.Singular, TGen.Masc, TNum.Singular, eGrade );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Pone el genero a una palabra</summary>
    public string PutWordInGender( string sWrd, TGen gender )
      {
      return Conj.ConcordWords( sWrd, TGen.Masc, TNum.Singular, gender, TNum.Singular, TGrad.Positive );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Pone el úmero a una palabra</summary>
    public string PutWordInNumber( string sWrd, TNum number )
      {
      return Conj.ConcordWords( sWrd, TGen.Masc, TNum.Singular, TGen.Masc, number, TGrad.Positive );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Le pone la declinación a un adjetivo </summary>
    public string PutAdjectiveInDeclination( string sAdj, Word Wrd )
      {
      return Conj.DeclineAdjective( sAdj,  Wrd.wGenero, Wrd.wNumero, Wrd.wDeclination, Wrd.wArticleType );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Le pone la declinación a un nombre </summary>
    public string PutNounInDeclination( string Noun, Word Wrd )
      {
      return Conj.DeclineNoun( Noun, Wrd.wGenero, Wrd.wNumero, Wrd.wDeclination );
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE LangData     +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE TrdEngine  +++++++++++++++++++++++++++++++++++++++++++++
