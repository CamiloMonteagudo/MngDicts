using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.TrdProcess;
using System.IO;

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //
  //            IMPLEMENTACIÓN DE LAS FUNCIONES Y COMAMDOS PARA LAS REGLAS DE TRADUCCIÓN
  //
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

namespace TrdEngine.Dictionary
  {
  ///<summary>Define los identificadores de las funciones y acciones usadas en las reglas</summary>
  public enum RuleId
    {
    // Funciones
    GENERIC = 0 ,  
    AUPR    = 1 ,             // Si toda la palabra esta en mayúsculas
    FIELD   = 2 ,             // Si casa el campo categoria semantica del diccionario
    FUPR    = 3 ,             // Si primera letra en mayúsculas
    SUFFIX  = 4 ,             // Si tiene el sufijo
    TYPE    = 5 ,             // Si es un tipo gramatical de la lista el tipo gramatical
    KWRD    = 6 ,             // Para marcar palabra clave (No se usa en esta vesión)
    WORD    = 7 ,             // Si es una palabra de la lista

    // Acciones
    CASE        = 20,
    CATEGORY    = 22,
    COMPLEX     = 23,
    DELETE      = 24,
    GENDER      = 25,
    GRADE       = 26,
    GRAMTYPE    = 27,
    INSERT      = 28,
    KEY         = 29,
    MODE        = 30,
    NEGATIVE    = 31,
    NUMBER      = 33,
    PERSON      = 34,
    REFLEXIVE   = 35,
    TIME        = 36,
    TOPHRASE    = 37,
    TRANSLATE   = 38,
    TRANSLATION = 39
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Interface que deben de implementar todos las funciones que se usan en las reglas</summary>
  public interface IRuleFun
    {
    ///<summary>Indentificador numerico del comando</summary>
    RuleId ID {get;}

    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    void ToStream( BinaryWriter Wtr );

    ///<summary>Evalua la función para la palabra 'iWord' de la oración 'Ora'</summary>
    bool Eval( Word wrd );
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Interface que deben de implementar todos las acciones que se usan en las reglas</summary>
  public interface IRuleAct
    {
    ///<summary>Indentificador numerico del comando</summary>
    RuleId ID {get;}

    ///<summary>Escribe representación de la acción al flujo de bytes 'Wtr'</summary>
    void ToStream( BinaryWriter Wtr) ;

    ///<summary>Ejecuta la acción para la palabra 'iWord' de la lista de palabra 'Words', 'iBase' es la primera palabra casada</summary>
    void Exec( int iWord, List<Word> Words, int iBase );
    }

#if CMD_SUPPORT || XML_SUPPORT
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Acción/Función generica, que se usa para trabajar sin sustituir las constantes y sirve para todas las 
  ///acciones y funciones</summary>
  public class RGeneric : IRuleAct, IRuleFun
    {
    public RuleId ID { get { return RuleId.GENERIC; } }
    string Name;                                            // Nombre de la acción                                     
    string Valor;                                           // Valor de la acción
    bool Fun;                                               // Define si es una acción o una funcion  

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando/función generico para almacenar el nombre y el argumento</summary>
    public static RGeneric Create(string args)
      {
      var fun = new RGeneric();                           // Crea el objeto función
      int i = args.IndexOfAny(new char[] { ';', '=' });   // Separa el argumento

      fun.Fun = (args[i] == ';');                         // Nombre;Valor => funcion, Nombre=Valor => accion

      fun.Name = args.Substring(0, i);                    // Obtiene la parte del nombre
      var Value = args.Substring(i + 1);                  // Obtiene la parte del valor

      if( !fun.Fun              &&                        // Si es una propiedad
          Value.Length>0        &&                        // Si el valor no es vacio
          char.IsDigit(Value, 0) )                        // Si el valor es numerico
        fun.Valor = "wrd" + Value;                        // Representa al número de una palabra
      else
        fun.Valor = Value;                                // Toma el valor tal y como viene

      return fun;                                         // Retorna el objeto
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString()
      {
      if (Fun)
        return Name + "(" + Valor + ')';
      else
        return Name + "=\"" + Valor + '\"';
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Implemta funciones de las interfaces (No hacen nada)</summary>
    public bool Eval( Word wrd ) { return false; }
    public void Exec( int iWord, List<Word> Words, int iBase ) { return; }
    public void ToStream(BinaryWriter Wtr) 
      {
      Dbg.Msg("ERROR: se esta usando una función generica en modo binario");
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RGeneric ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endif
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //                                    IMPLEMENTACIÓN DE LAS FUNCIONES
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función TYPE ej: TYPE("PT,PI"), determina si la palabra actual tiene uno de los tipo de la lista</summary>
  public class RFuncType: IRuleFun
    {
    public RuleId ID { get { return RuleId.TYPE; } }
    public HashSet<string> Types; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo TYPE y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncType();                          // Crea el objeto función TYPE
      fun.Types = args.ToHashSet();                       // Separa todos los tipos y los pone en un HashSet
      fun.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return fun;                                         // Retorna el objeto
      }
#endif
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo TYPE desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncType();                          // Crea el objeto función TYPE

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      fun.Types = Str.ToHashSet();                        // Separa todos los tipos y los pone en un HashSet
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "TYPE(" + Types.ToOneString() + ')';  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      { 
      return Types.Contains( wrd.sTipo ) ; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncType +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función AUPR ej: AUPR("NP,SS"), detemina si la palabra actual esta en mayusculas </summary>
  public class RFuncAUpr: IRuleFun
    {
    public RuleId ID { get { return RuleId.AUPR; } }
    public HashSet<string> Types = null; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un función del tipo AUPR y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncAUpr();                          // Crea el objeto función AUPR
      if( args.Length> 0 )                                // Si el argumento tiene contenido
        fun.Types = args.ToHashSet();                     // Separa todos los tipos y los pone en un HashSet

      fun.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return fun;                                         // Retorna el objeto
      }
#endif
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo AUPR desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncAUpr();                          // Crea el objeto función AUPR

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      if( Str.Length>0 )                                  // Si la cadena tiene contenido
        fun.Types = Str.ToHashSet();                      // Separa todos los tipos y los pone en un HashSet
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "AUPR(" + ((Types!=null)? Types.ToOneString(): "") + ')'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      {                           
      if( Types!=null && !Types.Contains( wrd.sTipo ) )  return false;
      return ( wrd.wCase == WCaso.Upper ); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncAUpr +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función FIELD ej: FIELD("CO"), detemina si el palabra tiene la categoria sementica del argumento</summary>
  public class RFuncField: IRuleFun
    {
    public RuleId ID { get { return RuleId.FIELD; } }
    public string Type; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo FIELD y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncField();                           // Crea el objeto función FIELD
      fun.Type = args;                                      // Guarda el argumento
      fun.idxStr = Tbls.AddString(args);                    // Almacena el argumento en la tabla de cadenas
      return fun;                                           // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo FIELD desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncField();                         // Crea el objeto función FIELD

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      fun.Type = Str;                                     // Guarda la cadena en la función
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "FIELD(" + Type + ')'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      { 
      return (wrd.sSemantica == Type) ; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncField ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función FUPR ej:FUPR("GV,GW"), determina si la palabra empieza con mayusculas</summary>
  public class RFuncFUpr: IRuleFun
    {
    public RuleId ID { get { return RuleId.FUPR; } }
    public HashSet<string> Types = null; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo FUPR y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncFUpr();                          // Crea el objeto función FUPR
      if( args.Length>0 )                                 // Si el argumento tiene contenido
        fun.Types = args.ToHashSet();                     // Separa todos los tipos y los pone en un HashSet

      fun.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return fun;                                         // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo FUPR desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncFUpr();                          // Crea el objeto función FUPR

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      if( Str.Length>0 )                                  // Si la cadena tiene contenido
        fun.Types = Str.ToHashSet();                      // Separa todos los tipos y los pone en un HashSet
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "FUPR(" + ((Types!=null)? Types.ToOneString(): "") + ')';  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      { 
      if( Types!=null && !Types.Contains( wrd.sTipo ) )  return false;
      return ( wrd.wCase == WCaso.Upper || wrd.wCase == WCaso.First ); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncFUpr +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función SUFFIX ej:SUFFIX("ar,er,ir"), determina si la palabra tiene uno de los sufijos de la lista</summary>
  public class RFuncSuffix: IRuleFun
    {
    public RuleId ID { get { return RuleId.SUFFIX; } }
    public string[] Suffs; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo SUFFIX y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncSuffix();                        // Crea el objeto función SUFFIX
      fun.Suffs = args.Split(',');                        // Separa todos los sufijos y los pone en un HashSet
      fun.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return fun;                                         // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo SUFFIX desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncSuffix();                        // Crea el objeto función SUFFIX

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      fun.Suffs = Str.Split(',');                         // Separa todos los tipos y los pone en un HashSet
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "SUFFIX(" + Suffs.ToOneString() + ')'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      {
      var word = wrd.Origlwr;
      foreach( var suff in Suffs )
        if( word.EndsWith(suff) ) 
          return true;

      return false; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncSuffix +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Función WORD ej: WORD("to,into"), determina si la palabra actual es una de las de la lista</summary>
  public class RFuncWord: IRuleFun
    {
    public RuleId ID { get { return RuleId.WORD; } }
    public HashSet<string> Words; 
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo WORD y le asocia los argumentos</summary>
    public static IRuleFun Create(string args, RulesTables Tbls)
      {
      var fun = new RFuncWord();                          // Crea el objeto función WORD
      fun.Words = args.ToHashSet();                       // Separa todos las palabras y las pone en un HashSet
      fun.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return fun;                                         // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo WORD desde un stream, usando la tabla de cadenas en rTbls</summary>
    public static IRuleFun FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var fun = new RFuncWord();                          // Crea el objeto función WORD

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null

      fun.idxStr = idx;                                   // Asigna el valor del indice
      fun.Words  = Str.ToHashSet();                       // Separa todos los tipos y los pone en un HashSet
      return fun;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "WORD(" + Words.ToOneString() + ')'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la función al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua los datos y retorna si verdadero o falso</summary>
    public bool Eval( Word wrd ) 
      { 
      if( wrd.Orig.Length == 0 ) return false;
      return Words.Contains(wrd.Origlwr) || Words.Contains(wrd.Orig) || Words.Contains(wrd.Key); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RFuncWord +++++++++++++++++++++++++++++++++++++++++++++++++++++++++


  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //                                    IMPLEMENTACIÓN DE LAS ACCIONES
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción GRAMTYPE</summary>
  public class RActGramType: IRuleAct
    {
    public RuleId ID { get { return RuleId.GRAMTYPE; } }
    string Tipo;
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GRAMTYPE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActGramType();                         // Crea el objeto función
      Act.Tipo = args;
      Act.idxStr = Tbls.AddString(args);                    // Almacena el argumento en la tabla de cadenas
      return Act;                                           // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GRAMTYPE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActGramType();                       // Crea el objeto acción GRAMTYPE

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null
                                  
      Act.idxStr = idx;                                   // Establece indice a la cadena
      Act.Tipo   = Str;                                   // Establece la cadena
      return Act;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "GRAMTYPE=\"" + Tipo + '"'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el tipo grantical dado a la palabra 'iWrd' de la lista 'Words'</summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].sTipo = Tipo;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActGramType ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción CASE</summary>
  public class RActCase: IRuleAct
    {
    static string[] Values = {"MIX","MIXTO","UPPER","LOWER","OTHER"};
    public RuleId ID { get { return RuleId.CASE; } }
    byte Case;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción CASE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActCase();                             // Crea el objeto función
      Act.Case = args.CodConstOrNum( Values );
      if( Act.Case == 0xFF ) return null;
      return Act;                                           // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción CASE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActCase();                             // Crea el objeto acción CASE
      Act.Case = Rdr.ReadByte();                            // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Case, "CASE", Values ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Case );  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el case dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Case<32 )                                                   // Es una asignación directa
        Words[iWrd].wCase = (WCaso)Case;                              // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Case-32);                                  // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wCase = Words[idx].wCase;                       // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActCase ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción CATEGORY</summary>
  public class RActCategory: IRuleAct
    {
    public RuleId ID { get { return RuleId.CATEGORY; } }
    string Categ;
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción CATEGORY y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActCategory();                       // Crea el objeto función
      Act.Categ = args;
      Act.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return Act;                                         // Retorna el objeto
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción CATEGORY desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActCategory();                       // Crea el objeto acción CATEGORY

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null
                                  
      Act.idxStr = idx;                                   // Establece indice a la cadena
      Act.Categ  = Str;                                   // Establece la cadena
      return Act;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "CATEGORY=\"" + Categ + '"'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la categoria semantica dada a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].sSemantica = Categ;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActCategory ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción COMPLEX</summary>
  public class RActComplex: IRuleAct
    {
    public RuleId ID { get { return RuleId.COMPLEX; } }

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción COMPLEX y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      if (args != "TRUE") return null;
      return new RActComplex();                            // Crea el objeto función y lo retorna
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción COMPLEX desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      return new RActComplex();                            // Crea el objeto acción COMPLEX y lo retorna
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "COMPLEX=\"TRUE\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la palabra 'iWrd' de la lista 'Words' como compuesta </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].Compuesto = true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActComplex +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción DELETE</summary>
  public class RActDelete: IRuleAct
    {
    public RuleId ID { get { return RuleId.DELETE; } }

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción DELETE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      if (args != "TRUE") return null;
      return new RActDelete();                             // Crea el objeto función y lo retorna
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción DELETE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      return new RActDelete();                            // Crea el objeto acción DELETE y lo retorna
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "DELETE=\"TRUE\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la palabra 'iWrd' de la lista 'Words' para ser borrada</summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].Delete = true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActDelete ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción GENDER</summary>
  public class RActGender: IRuleAct
    {
    public RuleId ID { get { return RuleId.GENDER; } }
    static string[] Generos = { "MALE", "FEMALE", "NEUTRO"};
    byte Gen;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GENDER y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActGender();                             // Crea el objeto función y lo retorna    
      Act.Gen = args.CodConstOrNum( Generos );
      if( Act.Gen == 0xFF ) return null;
      return Act;
      }
#endif
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GENDER desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActGender();                           // Crea el objeto acción GENDER
      Act.Gen = Rdr.ReadByte();                             // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Gen, "GENDER", Generos ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Gen ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el genero dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Gen<32 )                                                    // Es una asignación directa
        Words[iWrd].wGenero = (TGen)Gen;                            // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Gen-32);                                   // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wGenero = Words[idx].wGenero;                   // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActGender ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción GRADE</summary>
  public class RActGrade: IRuleAct
    {
    static string[] Values = {"POSITIVE","COMPARATIVE","SUPERLATIVE"};
    public RuleId ID { get { return RuleId.GRADE; } }
    byte grado;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GRADE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActGrade();                             // Crea el objeto función y lo retorna          
      Act.grado = args.CodConstOrNum( Values );
      if( Act.grado == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción GRADE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActGrade();                            // Crea el objeto acción GRADE
      Act.grado = Rdr.ReadByte();                           // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( grado, "GRADE", Values ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, grado ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el grado dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( grado<32 )                                                  // Es una asignación directa
        Words[iWrd].wGrade = (TGrad)grado;                            // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (grado-32);                                 // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wGrade = Words[idx].wGrade;                     // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActGrade ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción INSERT</summary>
  public class RActInsert: IRuleAct
    {
    public RuleId ID { get { return RuleId.INSERT; } }
    string sWord;                                           // Cadena que se quiere insertar
    public UInt16 iWrdStr;                                  // Indice a una palabra o a una cadena del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción INSERT y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActInsert();                           // Crea el objeto función y lo retorna          

      if (!UInt16.TryParse(args, out Act.iWrdStr))          // Trata de obtener indice a la palabra    
        {
        Act.sWord = args;                                   // Asigna la palabra
        Act.iWrdStr = (UInt16)(Tbls.AddString(args) + 32);  // Asigna indice a la cadena (desplazada 32)
        }

      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción INSERT desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActInsert();                           // Crea el objeto acción INSERT

      var idx     = Rdr.ReadUInt16();                       // Lee numero del steam
      Act.iWrdStr = idx;                                    // Asigna el número leido
      if( idx > 32 )                                        // El número es un indice a cadena
        {
        var Str = rTbls.GetString( idx-32 );                // Obtiene la cadena de la tabla
        if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null
                                  
        Act.sWord = Str;                                    // Establece la cadena
        }

      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() 
      { 
      string sNum = sWord;
      if( iWrdStr<32 )
        sNum = iWrdStr.ToString();

      return "INSERT=\"" + sNum + "\""; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)iWrdStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta un articulo o una palabra delante de la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( iWrdStr<32 ) Words[iWrd].Articulo = (short)iWrdStr;
      else             Words[iWrd].sInsert  = sWord; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActInsert ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción KEY</summary>
  public class RActKey: IRuleAct
    {
    public RuleId ID { get { return RuleId.KEY; } }
    string sKey;
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción KEY y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActKey();                             // Crea el objeto función y lo retorna          
      Act.sKey = args;
      Act.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción KEY desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActKey();                            // Crea el objeto acción KEY

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null
                                  
      Act.idxStr = idx;                                   // Establece indice a la cadena
      Act.sKey   = Str;                                   // Establece la cadena
      return Act;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "KEY=\"" + sKey + "\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia la llave de la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].Key       = sKey;
      Words[iWrd].Origlwr   = sKey;
      Words[iWrd].Orig      = sKey;
      Words[iWrd].Traducida = false;
      Words[iWrd].Buscada   = false;
      Words[iWrd].NoBusca   = false;
      Words[iWrd].wDiWord   = DiWord.WORD;
      Words[iWrd].Trad      = "";
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActKey +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción MODE</summary>
  public class RActMode: IRuleAct
    {
    public RuleId ID { get { return RuleId.MODE; } }

    static string[] Modes = {"INFINITIVE"   ,"INDICATIVE"   ,"IMPERATIVE"   ,"POTENCIAL"   ,"SUBJUNTIVE"   ,"GERUND"     ,"PARTICIPLE"    ,"INFINITIVE?INDICATIVE","INFINITIVE?IMPERATIVE","INFINITIVE?SUBJUNTIVE","INFINITIVE?POTENCIAL"};
    static TMod[]  AModes = {TMod.Indicativo,TMod.Indicativo,TMod.Imperativo,TMod.Potencial,TMod.Subjuntivo,TMod.Gerundio,TMod.Participio , TMod.Infinitivo       , TMod.Infinitivo       , TMod.Infinitivo       , TMod.Infinitivo      };
    static TMod[]  NModes = {TMod.Indicativo,TMod.Indicativo,TMod.Imperativo,TMod.Potencial,TMod.Subjuntivo,TMod.Gerundio,TMod.Participio , TMod.Indicativo       , TMod.Imperativo       , TMod.Subjuntivo       , TMod.Potencial       };

    byte Mode;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción MODE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActMode();                             // Crea el objeto función y lo retorna          
      Act.Mode = args.CodConstOrNum( Modes );
      if( Act.Mode == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción MODE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActMode();                            // Crea el objeto acción MODE
      Act.Mode = Rdr.ReadByte();                            // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Mode, "MODE", Modes ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Mode ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el modo dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      var Wrd = Words[iWrd];
      
      if( Mode<32 )                                                   // Es una asignación directa del modo
        {
        if( Mode<7 )                                                  // Es un modo normal
          Wrd.wModo = (TMod)Mode;                                     // Toma el valor
        else                                                          // Es un modo alternativo
          {
          if( Wrd.wModo == AModes[Mode]             ||                // Si ya esta en el modo normal
              Wrd.wModo < TMod.SubjuntivoImperativo ||                // o es un modo normal
              Wrd.wModo > TMod.SubjuntivoIndicativo )                 // o es uno de los ultimos modos
            Wrd.wModo = NModes[Mode];                                 // Asigna modo alternativo
          }
        }
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Mode-32);                                  // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wModo = Words[idx].wModo;                       // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActMode ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción NEGATIVE</summary>
  public class RActNegative: IRuleAct
    {
    static string[] Values = {"FALSE","TRUE"};
    public RuleId ID { get { return RuleId.NEGATIVE; } }
    byte Neg;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción NEGATIVE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActNegative();                             // Crea el objeto función y lo retorna          
      Act.Neg = args.CodConstOrNum( Values );
      if( Act.Neg == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción NEGATIVE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActNegative();                        // Crea el objeto acción NEGATIVE
      Act.Neg = Rdr.ReadByte();                             // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Neg, "NEGATIVE", Values ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Neg ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el modo negativo o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Neg<32 )                                                    // Es una asignación directa
        Words[iWrd].Negado = (Neg==1);                                // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Neg-32);                                   // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].Negado = Words[idx].Negado;                     // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActNegative ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción NUMBER</summary>
  public class RActNumber: IRuleAct
    {
    public RuleId ID { get { return RuleId.NUMBER; } }
    static string[] Numeros = { "SINGULAR", "PLURAL"};
    byte Num;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción NUMBER y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActNumber();                             // Crea el objeto función y lo retorna          
      Act.Num = args.CodConstOrNum( Numeros );
      if( Act.Num == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción NUMBER desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActNumber();                          // Crea el objeto acción NUMBER
      Act.Num = Rdr.ReadByte();                             // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Num, "NUMBER", Numeros ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Num ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el Número dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Num<32 )                                                    // Es una asignación directa
        Words[iWrd].wNumero = (TNum)Num;                            // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Num-32);                                   // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wNumero = Words[idx].wNumero;                   // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActNumber ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción PERSON</summary>
  public class RActPerson: IRuleAct
    {
    public RuleId ID { get { return RuleId.PERSON; } }
    static string[] Personas = { "FIRST", "SECOND", "THIRD"};
    byte Pers;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción PERSON y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActPerson();                             // Crea el objeto función y lo retorna          
      Act.Pers = args.CodConstOrNum( Personas );
      if( Act.Pers == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción PERSON desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActPerson();                          // Crea el objeto acción PERSON
      Act.Pers = Rdr.ReadByte();                            // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Pers, "PERSON", Personas ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Pers ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la Persona dada o la de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Pers<32 )                                                   // Es una asignación directa
        Words[iWrd].wPersona = (TPer)Pers;                         // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Pers-32);                                  // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wPersona = Words[idx].wPersona;                 // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActPerson ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción REFLEXIVE</summary>
  public class RActReflexive: IRuleAct
    {
    static string[] Values = {"FALSE","TRUE"};
    public RuleId ID { get { return RuleId.REFLEXIVE; } }
    byte Refl;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción REFLEXIVE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActReflexive();                             // Crea el objeto función y lo retorna 
      Act.Refl = args.CodConstOrNum( Values );
      if( Act.Refl == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción REFLEXIVE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActReflexive();                       // Crea el objeto acción REFLEXIVE
      Act.Refl = Rdr.ReadByte();                            // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum( Refl, "REFLEXIVE", Values ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Refl ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone Reflexivo o el reflaxivo de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Refl<32 )                                                   // Es una asignación directa
        Words[iWrd].Reflexivo = (Refl!=0);                            // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Refl-32);                                  // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].Reflexivo = Words[idx].Reflexivo;               // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActReflexive +++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción TIME</summary>
  public class RActTime: IRuleAct
    {
    public RuleId ID { get { return RuleId.TIME; } }
    static string[] Tiempos = { "PRESENT", "PAST", "FUTURE", "IPAST"};
    byte Time;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TIME y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActTime();                             // Crea el objeto función y lo retorna    
      Act.Time = args.CodConstOrNum(Tiempos);
      if( Act.Time == 0xFF ) return null;
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TIME desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act  = new RActTime();                            // Crea el objeto acción TIME
      Act.Time = Rdr.ReadByte();                            // Lee el valor del case desde el stream
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringConstOrNum(Time,"TIME",Tiempos); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, Time ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el Tiempo dado o el de la palabra con indice dado a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      if( Time<32 )                                                   // Es una asignación directa
        Words[iWrd].wTiempo = (TTime)Time;                           // Asingna el valor dado
      else                                                            // Es el indice a otra palabra
        {
        var idx = iBase + (Time-32);                                  // Obtiene indice a la palabra
        if( idx<Words.Count )                                         // Si el indice esta dentro del rango
          Words[iWrd].wTiempo = Words[idx].wTiempo;                   // Copia su valor para la palabra actual
        else
          Dbg.Msg("WARNIG: indice de palabra incorrecto, en: " + ToString());    // Pone mensaje de advertencia
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActTime ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción TOPHRASE</summary>
  public class RActToPhrase: IRuleAct
    {
    public RuleId ID { get { return RuleId.TOPHRASE; } }
    bool frase;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TOPHRASE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActToPhrase();                             // Crea el objeto función y lo retorna          
           if( args=="FALSE" ) Act.frase = false;
      else if( args=="TRUE"  ) Act.frase = true; 
      else
        {
        Dbg.Msg("ERROR: valor incorrecto para TOPHRASE");
        return null;
        }
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TOPHRASE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActToPhrase();                         // Crea el objeto acción TOPHRASE
      var val = Rdr.ReadByte();                             // Lee el valor del TOPHRASE desde el stream
      Act.frase = (val==1);                                 // Lo convierte a verdadero o falso

      if( (int)val>1 ) return null;                         // Es un valor incorrecto
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "TOPHRASE=\"" + ((frase)? "TRUE" : "FALSE") + "\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (byte)(frase? 1:0) ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Marca la palabra 'iWrd' de la lista 'Words' para que no forme parte de la frase casada</summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].NoEnComodin = !frase;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActToPhrase ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción TRANSLATE</summary>
  public class RActTranslate: IRuleAct
    {
    public RuleId ID { get { return RuleId.TRANSLATE; } }
    bool trd;

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TRANSLATE y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActTranslate();                             // Crea el objeto función y lo retorna          
           if( args=="FALSE" ) Act.trd = false;
      else if( args=="TRUE"  ) Act.trd = true; 
      else
        {
        Dbg.Msg("ERROR: valor incorrecto para TRANSLATE");
        return null;
        }
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TRANSLATE desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActTranslate();                        // Crea el objeto acción TRANSLATE
      var val = Rdr.ReadByte();                             // Lee el valor del TRANSLATE desde el stream
      Act.trd = (val==1);                                   // Lo convierte a verdadero o falso

      if( (int)val>1 ) return null;                         // Es un valor incorrecto
      return Act;                                           // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "TRANSLATE=\"" + ((trd)? "TRUE" : "FALSE") + "\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (byte)(trd? 1: 0) ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la palabra 'iWrd' de la lista 'Words' como traducida </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      var Wrd = Words[iWrd];
      if( trd == false )
        {
        if( string.IsNullOrEmpty(Wrd.StrSaveTrad)  )
          Wrd.StrSaveTrad = Wrd.Trad;

             if( !string.IsNullOrEmpty(Wrd.Orig   ) ) Wrd.Trad = Wrd.Orig;
        else if( !string.IsNullOrEmpty(Wrd.Origlwr) ) Wrd.Trad = Wrd.Origlwr;
        else if( !string.IsNullOrEmpty(Wrd.Key    ) ) Wrd.Trad = Wrd.Key;
        }
      else
        Wrd.Trad = Wrd.StrSaveTrad;

      Wrd.Traducida = (!trd);
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActTranslate +++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa la acción TRANSLATION</summary>
  public class RActTranslation: IRuleAct
    {
    public RuleId ID { get { return RuleId.TRANSLATION; } }
    string trans;
    public UInt16 idxStr;                                 // Indice en el arreglo de cadenas del diccionario

#if CMD_SUPPORT || XML_SUPPORT
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TRANSLATION y le asocia el valor</summary>
    public static IRuleAct Create(string args, RulesTables Tbls)
      {
      var Act = new RActTranslation();                             // Crea el objeto función y lo retorna          
      Act.trans = args;
      Act.idxStr = Tbls.AddString(args);                  // Almacena el argumento en la tabla de cadenas
      return Act;
      } 
#endif

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la acción TRANSLATION desde un stream y le asocia el valor</summary>
    public static IRuleAct FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var Act = new RActTranslation();                    // Crea el objeto acción TRANSLATION

      var idx = Rdr.ReadUInt16();                         // Lee indice de la cadena del stream
      var Str = rTbls.GetString( idx );                   // Obtiene la cadena de la tabla
      if( Str==null ) return null;                        // Si no pudo obtener la cadena retorna null
                                  
      Act.idxStr = idx;                                   // Establece indice a la cadena
      Act.trans  = Str;                                   // Establece la cadena
      return Act;                                         // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "TRANSLATION=\"" + trans + "\""; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe representación de la accion al flujo de bytes 'Wtr'</summary>
    public void ToStream( BinaryWriter Wtr) { Wtr.Write( (byte)ID, (UInt16)idxStr ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la traducción dada a la palabra 'iWrd' de la lista 'Words' </summary>
    public void Exec( int iWrd, List<Word> Words, int iBase )
      {
      Words[iWrd].Trad      = trans;
      Words[iWrd].Traducida = true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de RActTranslation +++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin del namespace      +++++++++++++++++++++++++++++++++++++++++++++++++++++
