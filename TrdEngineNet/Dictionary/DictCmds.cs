using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TrdEngine.TrdProcess;
using TrdEngine.Data;

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //
  //                          IMPLEMENTACION DE TODOS LOS COMANDOS DEL DICCIONARIO
  //
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

namespace TrdEngine.Dictionary
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Define los identificadores de los comandos usados en los diccionarios</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum CmdId
    {                 ///<summary>Comando Next Ej: NEXT("at","en")                                      </summary>
    NEXT       = 1,   ///<summary>Comando para poner el modo Ej: SETMODO(3),SETMODO(W,1),SETMODO(N2,5)  </summary> 
    SETMODO    = 2,   ///<summary>Comando para poner la persona Ej: SETPERSONA(1)                       </summary>
    SETPERSONA = 3,   ///<summary>Comando para poner el tiempo Ej: TIME(1)                              </summary>
    TIME       = 4,   ///<summary>Comando si el ultimo no advervio Ej: ISLNDDSET(BE,"estar")            </summary>
    ISLNDDSET  = 5,   ///<summary>Comando poner la siguiente palabra Ej: SETFW("estar")                 </summary>   
    SETFW      = 6,   ///<summary>Comando insertar Ej: INS(LNDD,6)                                      </summary>   
    INS        = 7,   ///<summary>Comando borrar Ej: DEL(W) ó DEL()                                     </summary>   
    DEL        = 8,   ///<summary>Comando para pone la palabra en plural Ej: SETPLURAL()                </summary>   
    SETPLURAL  = 9,   ///<summary>Comando para poner una palabra Ej: SETW(N2,"than")                    </summary>   
    SETW       =10     
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Interface que deben de implementar todos los comandos</summary>
  public interface IDictCmd
    {
    ///<summary>Indentificador numerico del comando</summary>
    CmdId ID {get;}

    ///<summary>Obtiene arreglo de bytes con la representación del comando</summary>
    void ToStream( BinaryWriter Wtr );

    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    bool Exec( int iWord, List<Word> Words );
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando Next, Ej: NEXT("at","en")</summary>
  public class DCmdNext: IDictCmd
    {
    public CmdId    ID   { get { return CmdId.NEXT; } }
    public string sWord, Trd; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Next y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length != 2 ) 
        {
        Dbg.Msg("El número de argumentos de la función NEXT, no es el correcto", Dbg.Error);
        return null;
        }

      var cmd = new DCmdNext();
          cmd.sWord = args[0].Trim('"');
          cmd.Trd   = args[1].Trim('"');

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Next desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd = new DCmdNext();
          cmd.sWord = Rdr.ReadStr();
          cmd.Trd   = Rdr.ReadStr();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, sWord, Trd ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "NEXT(\"" + sWord+"\",\""+Trd+"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Si la proxima palabra no arverbio, adjetivo, sutantivo, etc. es la especificada, se asigna traducción especificada</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      for( int i=iWrd+1; i<Words.Count; ++i )
        if( !"DD,AA,AP,AI,RD,RI,SS,NP,FN,AE,AN,NN,OR,OC".Contains(Words[i].sTipo)  )
          {
          if( Words[i].Origlwr == sWord )
            {
            Words[i].Trad = Trd;
            Words[i].Traducida = true;
            return true;
            }
          break;
          }  

      return false;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdNext ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el comando para poner una palabra en plural, Ej: SETPLURAL()</summary>
  public class DCmdPlural: IDictCmd
    {
    public CmdId ID { get { return CmdId.SETPLURAL; } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETPLURAL y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length != 1 ) 
        {
        Dbg.Msg("El número de argumentos de la función SETPLURAL, no es el correcto", Dbg.Error);
        return null;
        }

      if( args[0] != "" ) 
        {
        Dbg.Msg("El valor del argumento para SETPLURAL, no es el correcto", Dbg.Error);
        return null;
        }

      return new DCmdPlural();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETPLURAL desde un stream</summary>
    public static IDictCmd FromStream()
      {
      return new DCmdPlural();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "SETPLURAL()"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone palabra actual em plural</summary>
    public bool Exec( int iWord, List<Word> Words )
      {
      Words[iWord].Plural = true;
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdPlural ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando para cambiar de Modo, Ej: SETMODO(3),SETMODO(W,1),SETMODO(N2,5)</summary>
  public class DCmdModo: IDictCmd
    {
    public CmdId    ID { get { return CmdId.SETMODO; } }
    public byte Mode;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Modo y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      var cmd = new DCmdModo();
      cmd.Mode = DcUtil.CodWordAndNum( args, "SETMODO", 15 );
      if( cmd.Mode==0xFF ) return null;

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Modo desde un stream</summary>
    internal static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd = new DCmdModo();
          cmd.Mode = Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, Mode ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringWordAndNum( "SETMODO", Mode); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Le pone el modo dado a la palabra dada o a la palabra actual</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      Word word;
      var nMode = DcUtil.DecodWordAndNum( iWrd, Words, Mode, out word );
      if( word==null ) return false;

      word.wModo = (TMod)(nMode);
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdModo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando para cambiar la persona, Ej: SETPERSONA(1)</summary>
  public class DCmdPerson: IDictCmd
    {
    public CmdId    ID { get { return CmdId.SETPERSONA; } }
    public byte Pers;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Persona y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length!=1 )
        {
        Dbg.Msg("El número de argumentos de la función SETPERSONA, no es el correcto", Dbg.Error);
        return null;
        }

      var cmd = new DCmdPerson();
      if( !byte.TryParse( args[0], out cmd.Pers ) || cmd.Pers>3 )
        {
        Dbg.Msg("Argumento de la función SETPERSONA, no es correcto", Dbg.Error);
        return null;
        }

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Persona desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd = new DCmdPerson();
          cmd.Pers = Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write((byte)ID, Pers); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "SETPERSONA("+Pers+")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la persona señalada a la palabra actual</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      Words[iWrd].wPersona = (TPer)(Pers-1);
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdPerson ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando para cambiar el tiempo, Ej: TIME(1)</summary>
  public class DCmdTime: IDictCmd
    {
    public CmdId ID { get { return CmdId.TIME; } }
    public byte Time;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Tiempo y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      var cmd = new DCmdTime();
      cmd.Time = DcUtil.CodWordAndNum( args, "TIME", 4 );
      if( cmd.Time==0xFF ) return null;

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo Tiempo desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd      = new DCmdTime();
          cmd.Time = Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return  DcUtil.ToStringWordAndNum( "TIME", Time); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write((byte)ID, Time); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      Word word;
      var nTime = DcUtil.DecodWordAndNum( iWrd, Words, Time, out word );
      if( word==null ) return false;

      word.wTiempo = (TTime)(nTime);
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdTime ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el comando para poner traducción al ultimo no advervio, Ej: ISLNDDSET(BE,"estar")</summary>
  public class DCmdDDSet: IDictCmd
    {
    public CmdId    ID   { get { return CmdId.ISLNDDSET; } }
    public string sTipo, Trd; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo ISLNDDSET y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length!=2 )
        {
        Dbg.Msg("El número de argumentos de la función ISLNDDSET, no es el correcto", Dbg.Error);
        return null;
        }

      var sTipo = args[0].Trim('"');
      if( sTipo.Length != 2  || sTipo != sTipo.ToUpper() )
        {
        Dbg.Msg("El primer argumento de ISLNDDSET, debe ser un tipo gramatical", Dbg.Error);
        return null;
        }

      var cmd       = new DCmdDDSet();
          cmd.sTipo = sTipo;
          cmd.Trd   = args[1].Trim('"');
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo ISLNDDSET desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd       = new DCmdDDSet();
          cmd.sTipo = Rdr.ReadStr();
          cmd.Trd   = Rdr.ReadStr();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, sTipo, Trd ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "ISLNDDSET("+sTipo+",\""+Trd+"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      var wrd = ConstArg.LNDD.GetWordInOra(iWrd, Words);

      if( wrd!=null && wrd.sTipo == sTipo )
        {
        wrd.StrReserva1 = wrd.Trad = Trd;
        wrd.Traducida = true;

        return true;
        }

      return false;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdDDSet +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando poner traducción a una palabra,  Ej: SETW(N2,"than")</summary>
  //------------------------------------------------------------------------------------------------------------------
  public class DCmdSetW: IDictCmd
    {
    string[] sArg = { "W","N1","N2","N3" };

    public CmdId    ID   { get { return CmdId.SETW; } }
    public ConstArg IdWord;
    public string   Trd; 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETW y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length!=2 )
        {
        Dbg.Msg("El número de argumentos de la función SETW, no es el correcto", Dbg.Error);
        return null;
        }

      var IdWord = args[0].GetWordInOraId();
      if( IdWord==ConstArg.NA ) return null;

      var cmd = new DCmdSetW();

      cmd.IdWord = IdWord;
      cmd.Trd    = args[1].Trim('"');
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETW y le asocia los argumentos</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd        = new DCmdSetW();
          cmd.IdWord = (ConstArg)Rdr.ReadByte();
          cmd.Trd    = Rdr.ReadStr();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord, Trd ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "SETW("+ IdWord + ",\""+ Trd +"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      var word = IdWord.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      word.StrReserva1 = word.Trad = Trd;
      word.Traducida = true;
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdSetW ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando borrar una palabra,  Ej: DEL(W) ó DEL()</summary>
  public class DCmdDel: IDictCmd
    {
    public CmdId ID { get { return CmdId.DEL; } }
    public ConstArg IdWord;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo DEL y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length>1 ) 
        {
        Dbg.Msg("El número de argumentos de la función DEL, no es el correcto", Dbg.Error);
        return null;
        }

      ConstArg IdWord = 0;
      if( args[0] != "" )
        {
        IdWord = args[0].GetWordInOraId();
        if( IdWord==ConstArg.NA ) return null;
        }

      var cmd = new DCmdDel();
          cmd.IdWord = IdWord;
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo DEL desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd        = new DCmdDel();
          cmd.IdWord = (ConstArg)Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, (byte)IdWord ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "DEL(" + (((int)IdWord==0)? "": IdWord.ToString()) + ")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      var Id = ((int)IdWord==0)? ConstArg.W : IdWord;
      var word = Id.GetWordInOra( iWrd, Words );
      if( word == null ) return false;

      word.Delete = true;
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdDel +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase que implementa el comando para insertar una palabra,  Ej: INS(LNDD,6)</summary>
  public class DCmdIns: IDictCmd
    {
    public CmdId ID { get { return CmdId.INS; } }
    public byte Ins;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo INS y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      var cmd = new DCmdIns();
      cmd.Ins = DcUtil.CodWordAndNum( args, "INS", 10 );
      if( cmd.Ins==0xFF ) return null;

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo INS desde un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd     = new DCmdIns();
          cmd.Ins = Rdr.ReadByte();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, Ins ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return DcUtil.ToStringWordAndNum( "INS", Ins); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWrd, List<Word> Words )
      {
      Word word;
      var nIns = DcUtil.DecodWordAndNum( iWrd, Words, Ins, out word );
      if( word==null ) return false;

      word.Articulo = (short)nIns;
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdIns +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el comando poner traducción de la proxima palabra Ej: SETFW("estar")</summary>
  public class DCmdSetFW: IDictCmd
    {
    public CmdId ID { get { return CmdId.SETFW; } }
    public string Trd;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETFW y le asocia los argumentos</summary>
    public static IDictCmd Create(string[] args) 
      { 
      if( args.Length != 1 ) 
        {
        Dbg.Msg("El número de argumentos de la función SETFW, no es el correcto", Dbg.Error);
        return null;
        }

      var cmd     = new DCmdSetFW();
          cmd.Trd = args[0].Trim('"');

      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un comando del tipo SETFW dede un stream</summary>
    public static IDictCmd FromStream(BinaryReader Rdr)
      {
      var cmd     = new DCmdSetFW();
          cmd.Trd = Rdr.ReadStr();
      return cmd; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene stream con la representación del comando</summary>
    public void ToStream( BinaryWriter Wtr ) { Wtr.Write( (byte)ID, Trd ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa el comando</summary>
    public override string ToString() { return "SETFW(\"" + Trd+"\")"; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el comando, sobre la palbra 'iWord' de la oración 'Sent'</summary>
    public bool Exec( int iWord, List<Word> Words )
      {
      var word = (Words[0].sTipo == "XX")? Words[1] : Words[0];
      if( word==null ) return false;

      word.StrReserva1 = word.Trad = Trd;
      word.Traducida = true;
      return true;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de DCmdSetFW +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  }   //++++++++++++++++++++++++++++++++++++ Fin del namespace ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
