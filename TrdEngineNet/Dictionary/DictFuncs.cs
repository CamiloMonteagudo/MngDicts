using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TrdEngine.TrdProcess;
using System.IO;


namespace TrdEngine.Dictionary
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //
  //                   IMPLEMENTACIÓN DE LAS FUNCIONES DEL LENGUAJE DE DICCIONARIO     
  //
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Define los identificadores de las funciones usadas en los diccionarios</summary>
  public enum FuncId
    {
    ELEFT    = 1 ,  // Existe left
    ERIGHT   = 2 ,
    ISABR    = 3 ,
    ISINMODE = 4 ,  
    ISSUFIX  = 5 ,
    IST      = 6 ,
    ISW      = 7 ,
    NNDD     = 8 ,
    W        = 9 ,
    L1       = 11,
    L2       = 12,
    L3       = 13,
    N1       = 14,
    N2       = 15,
    N3       = 16,
    LNDD     = 17,
    FW       = 18,
    LW       = 19,
    INFORMAL = 20,
    UPPER    = 21
    }

  ///<summary>Listado de argumentos constantes</summary>
  public enum ConstArg
    {                                     
    W          = 1,                     // Palabra actual 
    LNDD       = 2,                     // Ultimo no advervio
    NNDD       = 3,                     // Proximo no advervio
    N1         = 4,                     // Primera palabra después de la actual
    N2         = 5,                     // Segunda palabra despues de la actual
    N3         = 6,                     // Tercera palabra despues de la actual
    L1         = 7,                     // Primera palabra antes de la actual
    L2         = 8,                     // Segunda palabra antes de la actual
    L3         = 9,                     // Tercera palabra antes de la actual
    FW         = 10,                    // Primera palabra de la oración
    LW         = 11,                    // Ultima palabra de la oración
    NA         = -1                     // Se usa para indicar que no es un ConstArg
    }

  
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Interface que deben de implementar todos las funciones</summary>
  public interface IDictFunc
    {
    ///<summary>Indentificador numerico la función o propiedad</summary>
    FuncId ID {get;}

    ///<summary>Obtiene arreglo de bytes con la representación la función o propiedad</summary>
    void ToStream(  BinaryWriter Wtr );

    bool Eval( int iWrd, List<Word> Words );
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ELEFT("[String][,...]") </summary>
  public class DFuncELeft: IDictFunc
    {
    public FuncId ID { get { return FuncId.ELEFT; } }
    public HashSet<string> lstWords; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ELEFT y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var cmd = new DFuncELeft();                                     // Crea el objeto función
      cmd.lstWords = args.ToHashSet();                                // Obtiene el conjunto de palabras del argumento
      return cmd;                                                     // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ELEFT a partir de un stream</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var cmd = new DFuncELeft();                                     // Crea el objeto función
      cmd.lstWords = Rdr.ReadStr().ToHashSet();                        // Obtiene el conjunto de palabras del stream
      return cmd;                                                     // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, lstWords.ToOneString() ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ELEFT(\""+ lstWords.ToOneString()  +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si alguna de las palabras de 'lstWords' existe en la oración a la izquierda de la palabra 'iWord'</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      for( int i=iWrd-1; i>=0;  --i )
        if( lstWords.Contains( Words[i].Origlwr ) )
          return true;

      return false;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncELeft ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ERIGHT("[String][,...]")</summary>
  public class DFuncERight: IDictFunc
    {
    public FuncId ID { get { return FuncId.ERIGHT; } }
    public HashSet<string> lstWords; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ERIGHT y le asocia los argumentos</summary>
    public static IDictFunc Create( string args ) 
      { 
      var cmd = new DFuncERight();                                    // Crea el objeto
      cmd.lstWords = args.ToHashSet();                                // Obtiene el conjunto de palabras del argumento
      return cmd;                                                     // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ERIGHT desde un stream</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var cmd = new DFuncERight();                                    // Crea el objeto
      cmd.lstWords = Rdr.ReadStr().ToHashSet();                        // Obtiene el conjunto de palabras desde el stream
      return cmd;                                                     // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, lstWords.ToOneString() ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ERIGHT(\""+ lstWords.ToOneString()  +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si alguna de las palabras de 'lstWords' existe en la oración a la derecha de la palabra 'iWord'</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      for( int i=iWrd+1; i<Words.Count;  ++i )
        if( lstWords.Contains( Words[i].sTipo ) )
          return true;

      return false;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncERight +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ISABR(W)</summary>
  public class DFuncIsAbr: IDictFunc
    {
    public FuncId ID { get { return FuncId.ISABR; } }
    public ConstArg IdWord; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISABR y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var Id = args.GetWordInOraId();
      if( Id==ConstArg.NA ) 
        {
        Dbg.Msg( "El argumento de ISABR debe ser un tipo de palabra", Dbg.Error );
        return null;
        }

      var cmd = new DFuncIsAbr();
      cmd.IdWord = Id;

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISABR desde un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr)
      {
      var cmd = new DFuncIsAbr();
      cmd.IdWord = (ConstArg)Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ISABR("+ IdWord +")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra especificada es una abreviatura</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;
      return word.Abreviatura; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncIsAbr ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ISINMODE(W,MPOTENCIAL)</summary>
  public class DFuncIsInMode: IDictFunc
    {
    public FuncId ID { get { return FuncId.ISINMODE; } }

    static string[] Modes = {"MINFINITIVO","MINDICATIVO","MIMPERATIVO","MPOTENCIAL","MSUBJUNTIVO","MGERUNDIO","MPARTICIPIO"};
    int Mode;
    public ConstArg IdWord; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISINMODE y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var lstArg = args.SplitArg();                             // Separa los argumentos por la coma
      if( lstArg.Length != 2 )
        {
        Dbg.Msg( "La función ISINMODE necesita 2 argumentos", Dbg.Error );
        return null;
        }

      var fun = new DFuncIsInMode();

      fun.IdWord = lstArg[0].GetWordInOraId();
      if( fun.IdWord==ConstArg.NA ) 
        {
        Dbg.Msg( "El primer argumento de ISINMODE debe ser un tipo de palabra", Dbg.Error );
        return null;
        }

      for( int i=0; i<DFuncIsInMode.Modes.Length; i++)
        if( DFuncIsInMode.Modes[i] == lstArg[1] )
          { fun.Mode = i;  return fun; }

      Dbg.Msg( "El segundo argumento de ISINMODE debe ser un modo", Dbg.Error );
      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISINMODE desde un stream</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var fun    = new DFuncIsInMode();
      fun.IdWord = (ConstArg)Rdr.ReadByte();
      fun.Mode   = Rdr.ReadByte();
      return fun;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord, (byte)Mode ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ISINMODE("+ IdWord + ',' + Modes[Mode] + ')'; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua la palabra especificada esta en el modo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;
      return( word.wModo == (TMod)Mode ); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncIsInMode +++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ISSUFIX(NNDD|N[n]|W,"String")</summary>
  public class DFuncIsSufix: IDictFunc
    {
    public FuncId ID { get { return FuncId.ISSUFIX; } }
    public ConstArg IdWord; 
    string suff;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISSUFIX y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var lstArg = args.SplitArg();                             // Separa los argumentos por la coma
      if( lstArg.Length != 2 )
        {
        Dbg.Msg( "La función ISSUFIX necesita 2 argumentos", Dbg.Error );
        return null;
        }

      var fun = new DFuncIsSufix();

      fun.IdWord = lstArg[0].GetWordInOraId();
      if( fun.IdWord==ConstArg.NA ) 
        {
        Dbg.Msg( "El primer argumento de ISSUFIX debe ser un tipo de palabra", Dbg.Error );
        return null;
        }

      fun.suff = lstArg[1].Trim('"');
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISSUFIX a partir de un stream</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var fun    = new DFuncIsSufix();
      fun.IdWord = (ConstArg)Rdr.ReadByte();
      fun.suff   = Rdr.ReadStr();
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord,  suff ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ISSUFIX("+ IdWord + ",\"" + suff +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua la palabra especificada tiene el sufijo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;
      return word.Origlwr.EndsWith(suff); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncIsSufix ++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función IST(LNDD|NNDD|L[n]|N[n]|W  ,"String,...")</summary>
  public class DFuncIsT: IDictFunc
    {
    public FuncId ID { get { return FuncId.IST; } }
    public ConstArg IdWord; 
    string sTipos;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo IST y le asocia los argumentos</summary>
    public static IDictFunc Create( string args) 
      { 
      var lstArg = args.SplitArg();                             // Separa los argumentos por la coma
      if( lstArg.Length != 2 )
        {
        Dbg.Msg( "La función IST necesita 2 argumentos", Dbg.Error );
        return null;
        }

      var fun = new DFuncIsT();

      fun.IdWord = lstArg[0].GetWordInOraId();
      if( fun.IdWord==ConstArg.NA ) 
        {
        Dbg.Msg( "El primer argumento de IST debe ser un tipo de palabra", Dbg.Error );
        return null;
        }

      fun.sTipos = lstArg[1].Trim('"');
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo IST a partir de un stream</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var fun    = new DFuncIsT();
      fun.IdWord = (ConstArg)Rdr.ReadByte();
      fun.sTipos = Rdr.ReadStr();
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord, sTipos ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "IST("+ IdWord + ",\"" + sTipos +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si el tipo de la palabra especificada es de uno de los tipos de lista especificada</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      return sTipos.Contains( word.sTipo ); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncIsT ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función ISW(LNDD|NNDD|L[n]|N[n]|W  ,"String,...")</summary>
  public class DFuncIsW: IDictFunc
    {
    public FuncId ID { get { return FuncId.ISW; } }
    public ConstArg IdWord; 
    string sWord;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISW y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var lstArg = args.SplitArg();                             // Separa los argumentos por la coma
      if( lstArg.Length != 2 )
        {
        Dbg.Msg( "La función ISW necesita 2 argumentos", Dbg.Error );
        return null;
        }

      var fun = new DFuncIsW();

      fun.IdWord = lstArg[0].GetWordInOraId();
      if( fun.IdWord==ConstArg.NA ) 
        {
        Dbg.Msg( "El primer argumento de ISW debe ser un tipo de palabra", Dbg.Error );
        return null;
        }

      fun.sWord = lstArg[1].Trim('"');
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo ISW desde un streeam</summary>
    public static IDictFunc FromStream(BinaryReader Rdr)
      {
      var fun    = new DFuncIsW();
      fun.IdWord = (ConstArg)Rdr.ReadByte();
      fun.sWord  = Rdr.ReadStr();
      return fun; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord, sWord ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "ISW("+ IdWord + ",\"" + sWord +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra especificada es una de lista de palabras especificadas</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      var s = ',' + sWord + ',';
      return s.Contains( ',' + word.Origlwr + ',' ); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncIsW ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad NNDD=AS|BE|MGERUNDIO</summary>
  public class DFuncNNDD: IDictFunc
    {
    public FuncId ID { get { return FuncId.NNDD; } }

    static string[] sArg = {"AS","BE","MGERUNDIO"};
    public int iTipo; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo NNDD y le asocia su valor</summary>
    public static IDictFunc Create(string args) 
      { 
      var prop = new DFuncNNDD();
      for( int i=0; i<DFuncNNDD.sArg.Length; i++)
        if( DFuncNNDD.sArg[i] == args )
          { prop.iTipo = i;  return prop; }

      Dbg.Msg("El valor de la propiedad NNDD no es reconocido", Dbg.Error );
      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo NNDD desde un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr)
      {
      var prop = new DFuncNNDD();
      prop.iTipo = Rdr.ReadByte();
      return prop;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)iTipo ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la propiedad</summary>
    public override string ToString() { return "NNDD=" + sArg[iTipo]; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la proxima palabra no adverbio es del tipo o modo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = ConstArg.NNDD.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      if( iTipo==2 ) return (word.wModo == TMod.Gerundio);
      else           return (word.sTipo  == sArg[iTipo]  );
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncNNDD +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad LNDD=AS|BE|MGERUNDIO</summary>
  public class DFuncLNDD: IDictFunc
    {
    public FuncId ID { get { return FuncId.LNDD; } }

    static string[] sArg = {"AS","BE","MGERUNDIO"};
    public int iTipo; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo LNDD y le asocia su valor</summary>
    public static IDictFunc Create(string args) 
      { 
      var prop = new DFuncLNDD();
      for( int i=0; i<DFuncLNDD.sArg.Length; i++)
        if( DFuncLNDD.sArg[i] == args )
          { prop.iTipo = i;  return prop; }

      Dbg.Msg("El valor de la propiedad LNDD no es reconocido", Dbg.Error );
      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo LNDD a partir de un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr)
      {
      var prop = new DFuncLNDD();
      prop.iTipo = Rdr.ReadByte();
      return prop;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)iTipo ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la propiedad</summary>
    public override string ToString() { return "LNDD=" + sArg[iTipo]; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra previa no adverbio es del tipo o modo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = ConstArg.LNDD.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      if( iTipo==2 ) return (word.wModo == TMod.Gerundio);
      else           return (word.sTipo  == sArg[iTipo]  );
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncLNDD +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad W=[Tipo]|PRIMERAP|SEGUNDAP|TERCERAP|SINGULAR|PLURAL </summary>
  public class DFuncW: IDictFunc
    {
    public FuncId ID { get { return FuncId.W; } }

    static string[] sArg = {"","PRIMERAP","SEGUNDAP","TERCERAP","SINGULAR","PLURAL"};
    public int iArg; 
    public string sTipo="";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo W y le asocia su valor</summary>
    public static IDictFunc Create(string args) 
      { 
      var prop = new DFuncW();
      if( args.Length==2 )
        {
        prop.iArg = 0;
        prop.sTipo = args;
        return prop;
        }
      else
        {
        for( int i=1; i<DFuncW.sArg.Length; i++)
          if( DFuncW.sArg[i] == args )
            { prop.iArg = i; return prop; }
        }

      Dbg.Msg( "El valor de la propiedad W no es reconocido", Dbg.Error );
      return null; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo W desde un stream</summary>
    public static IDictFunc FromStream( BinaryReader Rdr )
      {
      var prop   = new DFuncW();
      prop.iArg  = Rdr.ReadByte();
      prop.sTipo = Rdr.ReadStr();
      return prop;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)iArg, sTipo ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la propiedad</summary>
    public override string ToString() { return "W=" + ((iArg==0)? sTipo : sArg[iArg] ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra actual es del tipo, número o persona especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = Words[iWrd]; 
      
      if( iArg==0 ) return (word.sTipo==sTipo);
      if( iArg<=3 ) return (word.wPersona == (TPer)(iArg-1));
      if( iArg<=5 ) return (word.wNumero  == (TNum)(iArg-4));
                   
      return false; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncW  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad L[n]="String"|PRIMERAP|SEGUNDAP|TERCERAP|SINGULAR|PLURAL|AA|NN|OF|TO</summary>
  public class DFuncL: IDictFunc
    {
    int n = 0;
    public FuncId ID { get { return (FuncId.L1 + n); } }

    static string[] sArg = {"", "PRIMERAP","SEGUNDAP","TERCERAP","SINGULAR","PLURAL","AA","NN","OF","TO"};
    public int iArg; 
    public string sWord = "";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo L y le asocia su valor</summary>
    public static IDictFunc Create( string args, int num ) 
      { 
      var prop = new DFuncL();
      prop.n = num;

      if( args.Length > 0 )                                             // Si hay un valor
        {
        if( args[0]=='"' )                                              // Es un valor de una cadena
          {                                                             // Pone el indice a 0
          prop.iArg  = 0;                                               // Almacena la cadena, sin comillas
          prop.sWord = args.Trim('"');                                  // Retorna el objeto
          return prop;
          }
        else
          {                                                             // Busca en la lista de posibles valores
          for( int i=1; i<DFuncL.sArg.Length ;  i++)
            if( DFuncL.sArg[i] == args )
              { prop.iArg = i; return prop; }
          }
        }

      Dbg.Msg( "El valor de la propiedad L no es reconocido", Dbg.Error );
      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo L desde un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr, int n)
      {
      var prop = new DFuncL();
      prop.n = n;
      prop.iArg = Rdr.ReadByte();
      prop.sWord = Rdr.ReadStr();
      return prop;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)iArg, sWord ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la propiedad</summary>
    public override string ToString() { return ID + "=" + ((iArg==0)? "\""+sWord+"\"" : sArg[iArg]); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra anterior especificada, es la palabra, número, persona o tipo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = (ConstArg.L1+n).GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      if( iArg==0 ) return (word.Origlwr   == sWord );
      if( iArg<=3 ) return (word.wPersona == (TPer)(iArg-1));
      if( iArg<=5 ) return (word.wNumero  == (TNum)(iArg-4));
      if( iArg<=9 ) return (word.sTipo     == sArg[iArg]);
                   
      return false; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncL  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad N[n]="String"|PRIMERAP|SEGUNDAP|TERCERAP|SINGULAR|PLURAL|AA|NN|OF|TO</summary>
  public class DFuncN: IDictFunc
    {
    int n = 0;
    public FuncId ID { get { return (FuncId.N1 + n); } }

    static string[] sArg = {"","PRIMERAP","SEGUNDAP","TERCERAP","SINGULAR","PLURAL","AA","NN","OF","TO"};
    public int iArg; 
    public string sWord="";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo N y le asocia su valor</summary>
    public static IDictFunc Create( string args, int num ) 
      { 
      var prop = new DFuncN();                                          // Crea un objeto
      prop.n = num;                                                     // Pone el numero de la palabra

      if( args.Length > 0 )                                             // Si hay un valor
        {
        if( args[0]=='"' )                                              // Es un valor de una cadena
          {                                                             // Pone el indice a 0
          prop.iArg  = 0;                                               // Almacena la cadena, sin comillas
          prop.sWord = args.Trim('"');                                  // Retorna el objeto
          return prop;
          }
        else
          {                                                             // Busca en la lista de posibles valores
          for( int i=1; i<DFuncN.sArg.Length ;  i++)                    // Si lo encontro
            if( DFuncN.sArg[i] == args )                                // Almacena el indice y retorna
              { prop.iArg = i; return prop;}
          }
        }

      Dbg.Msg( "El valor de la propiedad N no es reconocido", Dbg.Error );
      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo N a partir de un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr, int n)
      {
      var prop = new DFuncN();                                          // Crea un objeto
      prop.n = n;                                                       // Pone el numero de la palabra
      prop.iArg = Rdr.ReadByte();
      prop.sWord = Rdr.ReadStr();
      return prop;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)iArg, sWord ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la pripiedad</summary>
    public override string ToString() { return ID + "=" + ((iArg==0)? "\""+sWord+"\"" : sArg[iArg]); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra posterior especificada, es la palabra, número, persona o tipo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = (ConstArg.N1+n).GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      if( iArg==0 ) return (word.Origlwr   == sWord );
      if( iArg<=3 ) return (word.wPersona == (TPer)(iArg-1));
      if( iArg<=5 ) return (word.wNumero  == (TNum)(iArg-4));
      if( iArg<=9 ) return (word.sTipo     == sArg[iArg]);
                   
      return false; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncN  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad FW=[TT]</summary>
  public class DFuncFW: IDictFunc
    {
    public FuncId ID { get { return FuncId.FW; } }
    public string sTipo; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo FW y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      if( args.Length != 2 )
        {
        Dbg.Msg("El valor de la propiedad FW tiene que ser un tipo", Dbg.Error );
        return null;
        }

      var prop = new DFuncFW();
      prop.sTipo = args;

      return prop; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo FW a partir de un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr)
      {
      var prop = new DFuncFW();
      prop.sTipo = Rdr.ReadStr();
      return prop; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, sTipo ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "FW=" + sTipo; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la primera palabra de la oración es del tipo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = ConstArg.FW.GetWordInOra( iWrd, Words );
      return (word.sTipo == sTipo);
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncFW  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la propiedad LW="String"|GS</summary>
  public class DFuncLW: IDictFunc
    {
    public FuncId ID { get { return FuncId.LW; } }
    public string sVal; 
    bool   isStr;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo LW y le asocia los argumentos</summary>
    public static IDictFunc Create(string args) 
      { 
      var prop = new DFuncLW();
      prop.isStr = args.StartsWith("\"");
      prop.sVal = args.Trim('"');
      return prop; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una propiedad del tipo LW a partir de un stream</summary>
    internal static IDictFunc FromStream(BinaryReader Rdr)
      {
      var prop   = new DFuncLW();
      prop.isStr = (Rdr.ReadByte()==1);
      prop.sVal  = Rdr.ReadStr();
      return prop; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la propiedad</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)(isStr?1:0), sVal ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la propiedad</summary>
    public override string ToString() { return "LW=" + (isStr? ('"'+ sVal + '"') : sVal); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la ultima palabra de la oración es del tipo especificado</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      var word = ConstArg.LW.GetWordInOra( iWrd, Words );

      if( isStr ) return (word.Origlwr == sVal);
      else        return (word.sTipo   == sVal);
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncLW  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función INFORMAL</summary>
  public class DFuncInformal: IDictFunc
    {
    public FuncId ID { get { return FuncId.INFORMAL; } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo INFORMAL</summary>
    public static IDictFunc Create() 
      { 
      return new DFuncInformal();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo INFORMAL</summary>
    public static IDictFunc FromStream()
      {
      return new DFuncInformal();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación de la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "INFORMAL"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si se esta trabajando en mod informal o no</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      Dbg.Msg("WARING: la función INFORMAL no esta implementada");
      return false; 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncInformal +++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa la función UPPER</summary>
  public class DFuncUpper: IDictFunc
    {
    public FuncId ID { get { return FuncId.UPPER; } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo UPPER</summary>
    public static IDictFunc Create() 
      { 
      return new DFuncUpper();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función del tipo UPPER</summary>
    public static IDictFunc FromStream()
      {
      return new DFuncUpper();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación la función</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa la función</summary>
    public override string ToString() { return "UPPER"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua si la palabra actual esta en mayusculas</summary>
    public bool Eval( int iWrd, List<Word> Words ) 
      { 
      return (Words[iWrd].wCase == WCaso.Upper); 
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DFuncUpper ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  }   //++++++++++++++++++++++++++++++++++++ Fin de namespace  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
