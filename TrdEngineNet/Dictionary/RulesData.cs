using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrdEngine.Dictionary
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Mantiene todos los datos de reglas de una llave</summary>
  public partial class RuleData
    {
    public UInt16 idxRule = UInt16.MaxValue;
    public List<RuleMatch> Matchs = new List<RuleMatch>();

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si en los datos de las reglas hay alguno que case con la palabra 'iNow'de la oración 'Sent'</summary>
    public int Match( int iNow, Sentence Sent )
      {
      var ora = Sent.Words;
      for( int i=0; i<Matchs.Count; ++i )                   // Recorre todos los match
        {
        var match = Matchs[i];
        int ini = iNow - match.idxKey + 1;                  // Determina primera palabra de la oración a casar
        int end = ini + match.Conds.Count - match.nAndOp;   // Determina ultima palabra de la oración a casar
        
        if( ini<0 || end>=ora.Count ) continue;             // Rango de macheo no cae dentro de la oración

        bool ret = true;
        foreach( var cond in match.Conds )                  // Recorre todas las condiciones para ese match
          {
          var iWrd = ini + (cond.iWord-1);                  // Calcula indice de la palabra para la condición

          ret = cond.Funtion.Eval( ora[iWrd] );             // Evalua la condición para la palabra
          if( ret==false ) break;                           // No se cumple la condición, termina el ciclo  
          }

        if( ret==false ) continue;                          // No se cumplieron todas las condiciones, continua
        return i;                                           // Retorna el Macth
        }

      return -1;                                           // No se cumplio ningún match
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de los datos de la regla en forma de un fujo de bytes</summary>
    public byte[] ToStream( bool share=true )
      {
      int n = Matchs.Count;                                // Obtiene el numero de Match
      MemoryStream MStrm = new MemoryStream( 20*n );       // Crea un stream en memoria
      var Wtr = new BinaryWriter( MStrm );                 // Le asocia un Writer binario

      if( idxRule==UInt16.MaxValue || !share )             // No tiene datos compartidos
        {
        Wtr.Write( (Byte)(n) );                            // Escribe número de matchs
        for( int i=0; i<n; ++i )                           // Recorre todos los match
          Matchs[i].ToStream( Wtr );                       // Escribe contenido del match 
        }
      else                                                 // Es un dato compartido 
        {
        Wtr.Write( (Byte)(Byte.MaxValue) );                // Escribe un byte para marcar
        Wtr.Write( (UInt16)(idxRule)     );                // Escribe indice del dato en la tabla de datos compartidos
        }

      return MStrm.ToArray();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna objeto RuleData si los datos en Rdr se ajustan a la representación del tipo de objeto</summary>
    public static RuleData FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      byte nMatch = Rdr.ReadByte();
      if( nMatch == (Byte.MaxValue) )                       // Es una llave duplicada
        {
        UInt16 idxRule = Rdr.ReadUInt16();                  // Lee el indice de la llave
        return rTbls.ShareKeys[idxRule];                    // Retorna desde la tabla de dato duplicados
        }
      else
        {
        var RData = new RuleData();
        for( int i=0; i<nMatch; ++i )
          {
          var match = RuleMatch.FromStream( Rdr, rTbls );
          if( match==null ) return null;

          RData.Matchs.Add( match );
          }

        return RData;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna objeto RuleData si el flujo de bytes se ajusta a la representación del tipo de objeto</summary>
    public static RuleData FromStream( byte[] Bytes, RulesTables rTbls)
      {
      var mem = new MemoryStream( Bytes );                      // Crea un strean en memoria con los datos
      var Rdr = new BinaryReader(mem, Encoding.UTF8);           // Crea un lector binario, para leer la memoria

      return FromStream( Rdr, rTbls );                          // Crea objeto con los datos
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleData ++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Mantiene todos los datos de un Match</summary>
  public partial class RuleMatch
    {
    public int idxKey = 1;                                      // Indice dentro del match de la palabra que tiene la llave
    public int nAndOp = 0;                                      // Número de operaciones AND (condiciones que no agreagan palabras) 

    public List<MatchCond> Conds = new List<MatchCond>();           // Todas las condiciones del match

    public List<MatchAction> Actions = new List<MatchAction>();     // Todas las acciones agrupadas en ACTION
    public List<MatchAction> Phrases = new List<MatchAction>();     // Todas las acciones agrupadas en PHRASE
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Indice a la primera palabra a machear, estando en 'iNow'</summary>
    public int IniWord( int iNow ) { return iNow - (idxKey-1); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Indice a la ultima palabra a machear, estando en 'iNow'</summary>
    public int EndWord( int iNow ) { return iNow-(idxKey-1) + (Conds.Count-nAndOp); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta las acciones agrupadas en ACTION para las palabras de la oración</summary>
    public void RunActions( int iNow, List<Word> OraWords )
      {
      int ini = iNow - idxKey;
      for( int i=0; i<Actions.Count; ++i )
        {
        var act  = Actions[i];
        var iWrd = ini + act.iWord;
        if( iWrd < OraWords.Count )
          act.Action.Exec( iWrd, OraWords, ini );
        else
          Dbg.Msg("WARNIG: indice de palabra en una acción icorrecto");
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta las acciones agrupadas en PHRASE para el grupo de palabras macheadas</summary>
    public void RunPhrase( List<Word> matchWords )
      {
      for( int i=0; i<Phrases.Count; ++i )
        {
        var act  = Phrases[i];
        var iWrd = act.iWord;
        if( iWrd < matchWords.Count )
          act.Action.Exec( iWrd, matchWords, 0 );
        else
          Dbg.Msg("WARNIG: indice de palabra en una acción icorrecto");
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna objeto RuleMatch si los datos en Rdr se ajustan a la representación ese objeto</summary>
    internal static RuleMatch FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      var RMatch = new RuleMatch();                                 // Crea el objeto

      var comb = Rdr.ReadByte();                                    // Lee byte que combina información
      RMatch.idxKey = comb & 0x0F;                                  // Posición de la palabra clave (4 bit bajos)
      RMatch.nAndOp = comb >> 4;                                    // # de palabras con mas de una condición (4 bit altos)

      byte nCond = Rdr.ReadByte();                                  // Lee la cantidad de condiciones
      for( int i=0; i<nCond; ++i )                                  // Para cada condición
        {
        var Cond = MatchCond.FromStream( Rdr, rTbls );              // Lee la condición
        if( Cond == null ) return null;                             // No la pudo leer

        RMatch.Conds.Add( Cond );                                   // Adicciona la condición al match
        }

      byte nAct = Rdr.ReadByte();                                   // Lee la cantidad de acciones
      for( int i=0; i<nAct; ++i )                                   // Para cada acción
        {
        var Act = MatchAction.FromStream( Rdr, rTbls );             // Lee la acción
        if( Act == null ) return null;                              // No la pudo leer

        RMatch.Actions.Add( Act );                                  // Adicciona la accción
        }

      byte nfra = Rdr.ReadByte();                                   // Lee la cantidad de frases
      for( int i=0; i<nfra; ++i )                                   // Para cada frases
        {
        var Fra = MatchAction.FromStream( Rdr, rTbls );             // Lee la acción para la frase
        if( Fra == null ) return null;                              // No la pudo leer

        RMatch.Phrases.Add( Fra );                                  // Adicciona la acción a la frase
        }

      return RMatch;                                                // Retorna el Match
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe los datos del Match al flujo de bytes 'Wtr'</summary>
    internal void ToStream(BinaryWriter Wtr)
      {
      byte comb = (byte)((nAndOp<<4) | (idxKey));                   // Combina # de operaciones, con indice de la palabra

      Wtr.Write( (byte)comb );                                      // Escribe en un byte información conbinada

      Wtr.Write( (byte)Conds.Count );                               // Escribe número de condiciones
      for( int i=0; i<Conds.Count; ++i )                            // Recorre todas las condiciones
        Conds[i].ToStream( Wtr );                                   // Escribe la condición al stream

      Wtr.Write( (byte)Actions.Count );                             // Escribe # de acciones dentro de ACTION
      for( int i=0; i<Actions.Count; ++i )                          // Recorre todas las acciones
        Actions[i].ToStream( Wtr );                                 // Escribe el stream al stream

      Wtr.Write( (byte)Phrases.Count );                             // Escribe # de acciones dentro de PHRASE
      for( int i=0; i<Phrases.Count; ++i )                          // Recorre todas las acciones
        Phrases[i].ToStream( Wtr );                                 // Escribe el stream al stream
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleMatch +++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Mantiene los datos de una condición</summary>
  public class MatchCond
    {
    public int iWord;                                               // Indice a la palabra que se aplica la condición
    public IRuleFun Funtion;                                        // Función que establece la condición

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de una condición, con la palabra y la función a aplicar</summary>
    public MatchCond( int iWord, IRuleFun Funtion)                  
      {
      this.iWord = iWord;
      this.Funtion = Funtion;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe los datos de la condición al flujo de bytes 'Wtr'</summary>
    internal void ToStream(BinaryWriter Wtr)
      {
      Wtr.Write( (byte)iWord );
      Funtion.ToStream( Wtr );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna objeto MatchCond si los datos en Rdr se ajustan a la representación del tipo de objeto</summary>
    internal static MatchCond FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      int    iWord = Rdr.ReadByte();
      RuleId FunId = (RuleId)Rdr.ReadByte();

      var Func = FunFromStream( FunId, Rdr, rTbls );
      if( Func == null ) return null;

      return new MatchCond( iWord, Func );
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de la condición en formato xml</summary>
    internal string ToXml()
      {
       return "      <COND wrd=\""+ iWord +"\"> " + Funtion.ToString() + " </COND>\r\n";
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Dato el ID de una función la crea desde un stream</summary>
    internal static IRuleFun FunFromStream( RuleId FunId, BinaryReader Rdr, RulesTables Tbls )
      {
      switch( FunId )
        {
        case RuleId.AUPR  : return RFuncAUpr.FromStream  ( Rdr, Tbls );
        case RuleId.FIELD : return RFuncField.FromStream ( Rdr, Tbls );
        case RuleId.FUPR  : return RFuncFUpr.FromStream  ( Rdr, Tbls );  
        case RuleId.SUFFIX: return RFuncSuffix.FromStream( Rdr, Tbls );
        case RuleId.TYPE  : return RFuncType.FromStream  ( Rdr, Tbls );  
        case RuleId.WORD  : return RFuncWord.FromStream  ( Rdr, Tbls );  
        }

      return null;
      }
    } // ++++++++++++++++++++++++++++++++++++++++ FIN DE MatchCond ++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Mantiene los datos de una acción</summary>
  public class MatchAction
    {
    public int iWord;
    public IRuleAct Action;

    public MatchAction( int iWord, IRuleAct Action)
      {
      this.iWord = iWord;
      this.Action = Action;
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la accion</summary>
    public override string ToString() { return Action.ToString(); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe los datos de la acción al flujo de bytes 'Wtr'</summary>
    internal void ToStream(BinaryWriter Wtr)
      {
      Wtr.Write( (byte)iWord );
      Action.ToStream( Wtr );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna objeto MatchAction si los datos en Rdr se ajustan a la representación del tipo de objeto</summary>
    internal static MatchAction FromStream(BinaryReader Rdr, RulesTables rTbls)
      {
      int    iWord = Rdr.ReadByte();
      RuleId ActId = (RuleId)Rdr.ReadByte();

      var Act = ActFromStream( ActId, Rdr, rTbls );
      if( Act == null ) return null;

      return new MatchAction( iWord, Act );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Dado el ID de una acción la crea desde un stream</summary>
    internal static IRuleAct ActFromStream( RuleId ActId, BinaryReader Rdr, RulesTables rTbls )
      {
      switch( ActId )
        {
        case RuleId.CASE       : return RActCase.FromStream       ( Rdr, rTbls );
        case RuleId.CATEGORY   : return RActCategory.FromStream   ( Rdr, rTbls );
        case RuleId.COMPLEX    : return RActComplex.FromStream    ( Rdr, rTbls );  
        case RuleId.DELETE     : return RActDelete.FromStream     ( Rdr, rTbls );
        case RuleId.GENDER     : return RActGender.FromStream     ( Rdr, rTbls );  
        case RuleId.GRADE      : return RActGrade.FromStream      ( Rdr, rTbls );  
        case RuleId.GRAMTYPE   : return RActGramType.FromStream   ( Rdr, rTbls );  
        case RuleId.INSERT     : return RActInsert.FromStream     ( Rdr, rTbls );  
        case RuleId.KEY        : return RActKey.FromStream        ( Rdr, rTbls );  
        case RuleId.MODE       : return RActMode.FromStream       ( Rdr, rTbls );  
        case RuleId.NEGATIVE   : return RActNegative.FromStream   ( Rdr, rTbls );  
        case RuleId.NUMBER     : return RActNumber.FromStream     ( Rdr, rTbls );  
        case RuleId.PERSON     : return RActPerson.FromStream     ( Rdr, rTbls );  
        case RuleId.REFLEXIVE  : return RActReflexive.FromStream  ( Rdr, rTbls );  
        case RuleId.TIME       : return RActTime.FromStream       ( Rdr, rTbls );  
        case RuleId.TOPHRASE   : return RActToPhrase.FromStream   ( Rdr, rTbls );  
        case RuleId.TRANSLATE  : return RActTranslate.FromStream  ( Rdr, rTbls );  
        case RuleId.TRANSLATION: return RActTranslation.FromStream( Rdr, rTbls );  
        }

      return null;
      }
    } // ++++++++++++++++++++++++++++++++++++++++ FIN DE MatchAction ++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++++++ FIN DEL namespace +++++++++++++++++++++++++++++++++++++++++++++++++++++
