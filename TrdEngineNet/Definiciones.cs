#if MERCADO                     // Si se compila para mercado
  #define NO_DBG_MSG            // No se tienen en cuenta los mensajes para debugguer
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TrdEngine.Dictionary;

namespace TrdEngine
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Delegado para implementar una etapa de traducción, se usa para agreagar a Tranlate.Steps</summary>
  public delegate void DoTranslateStep(Translate trd);
  
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Delegado para resivir notificación de terminación de una etapa de traducción, se usa con Utils.SetEndStepHandle
  ///</summary>
  public delegate bool HndTraceTrd( string StepId, Translate trd );
  public delegate void HndTraceRgl( string RglId, List<string> rglData, Translate trd );

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Delegado para implementar la carga de un diccionarios de traduccion</summary>
  public delegate IDictTrd HndOpenTrdDict( string DictName );

  #region "Definición de enumeradores"

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define cantidades usadas por la maquinaria de traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  public static class TConst
    {                                     ///<summary>Número de direcciones de traducción soportadas</summary>
    public const int Dirs         = 14;   ///<summary>Número de diccionarios usados por dirección de traducción</summary>
    public const int DictsDir     = 14;   ///<summary>Número de diccionarios usados por idioma de traducción</summary>
    public const int DictsLang    = 7;    ///<summary>Número de diccionarios personalizables usados en la traducción</summary>
    public const int DictsCustom  = 4;    ///<summary>Número de listas de diccionarios usados en la traducción</summary>
    public const int DictsList    = 3;    ///<summary>Número de idiomas soportados</summary>
    public static int Langs       = Utils.Codes.Length;
                                          ///<summary>Marca inicial para no traducir (Interna del traductor)</summary>
    public const char IniMark  = '⌠';     ///<summary>Marca final para no traducir (Interna del traductor)</summary>
    public const char EndMark  = '⌡';     ///<summary>Marca de sustitución, para información de formato interno a la oración</summary>
    public const char SustMark = '×';     
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define los idiomas usados por idiomax, tanto para la interfaz de usuario como para la traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TLng:sbyte
    {         ///<summary>Idioma Español</summary>
    Es = 0,   ///<summary>Idioma Inglés</summary> 
    En = 1,   ///<summary>Idioma Italiano</summary>
    It = 2,   ///<summary>Idioma Alemán</summary>
    De = 3,   ///<summary>Idioma Francés</summary>
    Fr = 4,   ///<summary>Idioma Portugués</summary>
    Pt = 5,   ///<summary>Idioma no disponible</summary>
    NA = -1 
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define las direcciones de traducción usadas por idiomax</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TDir:sbyte
    {           ///<summary>Dirección Español-Inglés   </summary>
    EsEn = 0,   ///<summary>Dirección Inglés-Español   </summary> 
    EnEs = 1,   ///<summary>Dirección Italiano-Inglés  </summary>
    ItEn = 2,   ///<summary>Dirección Inglés-Italiano  </summary>
    EnIt = 3,   ///<summary>Dirección Italiano-Español </summary>
    ItEs = 4,   ///<summary>Dirección Español-Italiano </summary>
    EsIt = 5,   ///<summary>Dirección Italiano-Alemán  </summary>
    ItDe = 6,   ///<summary>Dirección Alemán-Italiano  </summary> 
    DeIt = 7,   ///<summary>Dirección Italiano-Francés </summary>
    ItFr = 8,   ///<summary>Dirección Francés-Italiano </summary>
    FrIt = 9,   ///<summary>Dirección Inglés-Francés   </summary>
    EnFr = 10,  ///<summary>Dirección Francés-Inglés   </summary>
    FrEn = 11,  ///<summary>Dirección Español-Francés  </summary>
    EsFr = 12,  ///<summary>Dirección Francés-Español  </summary> 
    FrEs = 13,  
    NA = -1 
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de diccionarios personalizados por los usuarios</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum DType:byte
    {             ///<summary>Dicionario general ó principal</summary>
    Gen   = 0,    ///<summary>Dicionario especializado</summary>
    Esp   = 1,    ///<summary>Dicionario de usuario</summary>
    User  = 2,    ///<summary>Dicionario de memorias de traducción</summary>
    MTool = 3,    ///<summary>Dicionario de palabras a no traducir</summary>
    WNT   = 4,    ///<summary>Dicionario de reglas de traducción</summary>
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de listas de diccionarios que se usan en la traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum DLType
    {             ///<summary>Dicionario principal de palabras y frases, formado por: el de usuario, los de especialidades y el general</summary>
    Main    = 0,  ///<summary>Diccionarios utilizados para las memorias de traducción</summary> 
    MemTool = 1,  ///<summary>Diccionarios de palabras que no se deben traducir</summary>
    NTrdWrd = 2
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Opciones utilizadas para abrir los diccionarios con 'Open' de 'DictSingle'</summary>
  //------------------------------------------------------------------------------------------------------------------
  [Flags]public enum DOpen
    {                   ///<summary>No se tiene en cuenta ninguna opción en especial para abrir el diccionario</summary>
    None     = 0x0000,  ///<summary>Abre el diccionario para solo lectura</summary>
    ReadOnly = 0x0001,  ///<summary>No tiene en cuenta la lista de llaves ordenadas</summary>
    NoSort   = 0x0002,  ///<summary>Abre el diccionario para solo lectura y sin llaves ordenadas</summary>
    NSrtRO   = 0x0003  
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Banderas utilizadas para los diccionarios</summary>
  //------------------------------------------------------------------------------------------------------------------
  [Flags]public enum DFlag : ushort
    {                     ///<summary>Indica que las llaves estan ordenadas</summary>
    Sort     = 0x0001,    ///<summary>Indica que los datos estan codificados</summary>
    HideData = 0x0002,    
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Modos usados para las conjugaciones verbales</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TMod : byte
    {                  
    Infinitivo = 0,   
    Indicativo = 1,   
    Imperativo = 2,
    Potencial  = 3,
    Subjuntivo = 4,
    Gerundio   = 5,
    Participio = 6,
    SubjuntivoImperativo           = 7,
    SubjuntivoImperativoIndicativo = 8,
    ImperativoParticipioIndicativo = 9,
    SubjuntivoImperativoIndicativoParticipio = 10,
    ParticipioIndicativo = 11,
    ImperativoIndicativo = 12,
    ImperativoInfinitivo = 13,
    SubjuntivoIndicativo = 14,
    PotencialIndicativo  = 15,
    SubjuntivoParticipio = 16,
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tiempos verbales</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TTime:byte { Presente, Pasado, Futuro, PasadoImp }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Números usadas para la cordinación de palabras</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TNum:byte { Singular, Plural };

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Personas usadas para la conjugación de verbos</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TPer:byte { Primera, Segunda, Tercera };

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define el genero de una significado</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum TGen:byte { Masc=0, Femen=1, Neutro=2 }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Grado de los adjetivos</summary>
  public enum TGrad { Positive, Comparative, Superlative,  NA = -1  };

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de declinaciones</summary>
  public enum TDecl { NoDecline, Nominative, Accusative, Dative, Genitive, Vocative }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de articulo para realizar la declinación de adjetivos del alemán</summary>
  public enum TArtic{ Null, Definite, Indefinite };

    //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define los posibles formatos de una oración</summary>
  public enum OraFmt { TXT=0, RTF=1 }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Define los casos de los caracteres en una oración o palabra</summary>
  public enum WCaso { First=0, Upper=1, Lower=2, Other=3 }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Partes de una oración</summary>
  public enum OParte { opSUJETO, opVERBO, opCD, opCI, opCCT, opCCM, opCCL }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de palabras que reconoce el parsing de palabras </summary>
  public enum DiWord
    {
    WORD              =  0,   // Palabra formada por una o mas letras (Mayusculas o minusculas)
    POSESIVO_CS       =  1,   // Representa una palabra con una comilla y una s detras para indicar posesivo
    POSESIVO_SC       =  2,   // Representa una palabra y una comilla detras (sin s) para indicar posesivo

    NOMBRE_PROPIO     = 10,   // Combinación de letras, y/o números que no se traduce
    NUM_SOLO          = 11,   // Palabra formada solo por números
    NUM_LETRA         = 12,   // Palabra formada comienza por número y termina en letras
    LETRA_NUM         = 13,   // Palabra que comienza con letra y termina en número
    DECADA_CS         = 15,   // La palabra es un número seguido de 's que representa una decada
    DECADA_S          = 16,   // La palabra es un número seguido de s que representa una decada (no lleva comilla)
    EXPRESION         = 17,   // Es cualquier combinación de números y signos que forman una expresión
    ORDINAL           = 18,   // La palabra es un número ordinal Ej: 8vo ó 3ro
    FILENAME          = 19,   // La palabar es un nombre de fichero, URL o correo electronico
    ABREVIATURA       = 20,   // La palabra es una abreviatura Ej: Cant. ó E.U.

    CHARNUMERO        = 50,   // La palabra es un caracter #
    DOSPUNTOS         = 51,   // La palabra es un caracter :
    PUNTOYCOMA        = 52,   // La palabra es un caracter ;
    PUNTOSUSPENSIVOS  = 53,   // La palabra son tres puntos seguidos
    PUNTO             = 54,   // La palabra es un punto
    COMA              = 55,   // La palabra es una coma
    FININTERROGA      = 56,   // La palabra es caracter ?
    FINADMIRACION     = 57,   // La palabra es un caracter !
    DINERO            = 58,   // La palabra es un caracter para representar una moneda Ej: $
    MENOS             = 59,   // La palabra es un signo menos -
    GUION             = 60,   // La palabra es un guión que separa a una frase Ej: Play-Off
    SLAT              = 61,   // La palabra es un caracter /
    LLAVEA            = 62,   // La palabra es un caracter {
    LLAVEC            = 63,   // La palabra es un caracter }
    CORCHETEA         = 64,   // La palabra es un caracter [
    CORCHETEC         = 65,   // La palabra es un caracter ]
    MENORQ            = 66,   // La palabra es un caracter <
    MAYORQ            = 67,   // La palabra es un caracter >
    PARENTA           = 68,   // La palabra es un caracter (
    PARENTC           = 69,   // La palabra es un caracter )
    COMILLAD          = 70,   // La palabra es un caracter "
    COMILLAS          = 71,   // La palabra es un caracter '
    PORCIENTO         = 72,   // La palabra es un caracter %
    CUALQUIERA        = 73,   // La palabra es un caracter no especificado
    }
  #endregion

  #region "Definición de Interfaces"
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Interface que define las funcionalidades basicas que debe implementar un diccionario para que pueda ser
  ///usado en la traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  public interface IDictTrd
    {
    bool   IsKey(String sKey);

    String   GetStrData(String sKey);
    Tabla    GetTableData(String sKey);
    RuleData GetRuleData(String sKey);
    WordData GetWordData(String sKey);
    List<string> GetSortedKeys();
    }
  
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Interface que define los datos basicos que debe suministrar un diccionario</summary>
  //------------------------------------------------------------------------------------------------------------------
  public interface IDictHeader
    {
    ///<summary>Número de registros que tiene el diccionario</summary>
    int Count {get;}

    ///<summary>Camino del archivo que guarda los datos del diccionario</summary>
    string DicName {get;}

    ///<summary>Idioma de las llaves del diccionario</summary>
    TLng   Src {get;}
    ///<summary>Idioma de los datos del diccionario</summary>
    TLng   Des  {get;}

    ///<summary>Tipo de diccionario</summary>
    DDataType  Type  {get;}

    ///<summary>Banderas para identificación del diccionario</summary>
    DFlag  Flags  {get;set;}
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de diccionario según sus datos</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum DDataType:byte
    {              
    Str  = 0x02,  // De forma general sus datos son cadenas de caracteres
    Rule = 0x00,  // De forma general sus datos son reglas de traducción
    Word = 0x01   // De forma general sus datos son de traducción de palabras
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Utilitario para mostrar mensajes en modo debug</summary>
  //------------------------------------------------------------------------------------------------------------------
  public static class Dbg
    {
    public delegate void DebugMsg( string  msg );
    public static DebugMsg FunMsg = null;
    public static int Filter = 0;

    public const int Time    = 0x0001;              // Para clasificar mensajes para mostar tiempo
    public const int Warning = 0x0002;              // Para clasificar mensajes para mostrar arvertencias
    public const int Error   = 0x0004;              // Para clasificar mensajes para mostrar errores

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone mensaje para debuger y retorna un boolen</summary>
    //[ConditionalAttribute("NO_DBG_MSG")]
    public static bool Msg( string sMsg, int msgTipo=0 )
      {
      #if !MERCADO
      if( (Filter & msgTipo) != 0 ) return false;

      if( msgTipo == Warning ) sMsg = "WARNING: " + sMsg;
      if( msgTipo == Error   ) sMsg = "ERROR: " + sMsg;

      if( FunMsg!=null ) FunMsg( sMsg + "\r\n" );
      else               Debug.Write( sMsg + "\r\n" ); 
      #endif
      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone función para ignorar los mensajes</summary>
    public  static void SetNoMsg() { FunMsg = NoMsg; }
    private static void NoMsg( string  msg ) {}
    }


//------------------------------------------------------------------------------------------------------------------
  #endregion
  }
