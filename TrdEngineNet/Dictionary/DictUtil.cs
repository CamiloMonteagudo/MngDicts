using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrdEngine.Dictionary
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa una serie de fuciones utilitarias para el trabajo con los datos de los diccionario</summary>
  public static class DcUtil
    {
    static string AdvTypes = "DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una palabra de la oración de acuerdo a su identificador</summary>
    public static Word GetWordInOra( this ConstArg IdArg, int iWrd, List<Word> Ora )
      {
      switch( IdArg )
        {
        case ConstArg.W : return Ora[iWrd];                                          // Palabra actual
        case ConstArg.FW:                                                            // Primera palabra de la oración
          {
          Word w  = Ora[0];
          if( w.sTipo == "XX" && Ora.Count>1 )
            w = Ora[1];

          return w;
          }
        case ConstArg.LW:                                                            // Ultima palabra de la oración
          {
          int iLast = Ora.Count-1;
          Word w  = Ora[iLast];
          if( w.sTipo == "XX" && iLast>=1 )
            w = Ora[iLast-1];;

          return w;
          }
        case ConstArg.N1: if( iWrd+1 < Ora.Count ) return Ora[iWrd+1]; else break;   // Una palabra hacia la derecha de la actual
        case ConstArg.N2: if( iWrd+2 < Ora.Count ) return Ora[iWrd+2]; else break;   // Dos palabras hacia la derecha de la actual
        case ConstArg.N3: if( iWrd+3 < Ora.Count ) return Ora[iWrd+3]; else break;   // Tres palabras hacia la derecha de la actual
        case ConstArg.L1: if( iWrd-1 >= 0        ) return Ora[iWrd-1]; else break;   // Una palabra hacia la isquierda de la actual
        case ConstArg.L2: if( iWrd-2 >= 0        ) return Ora[iWrd-2]; else break;   // Dos palabra hacia la isquierda de la actual
        case ConstArg.L3: if( iWrd-3 >= 0        ) return Ora[iWrd-3]; else break;   // Tres palabra hacia la isquierda de la actual

        case ConstArg.LNDD:                                                          // Primera hacia la izquierda no adverbio
          for( int i=iWrd-1; i>=0; --i )
            if( !AdvTypes.Contains(Ora[i].sTipo) ) return Ora[i];
          break;

        case ConstArg.NNDD:                                                          // Primera hacia la derecha no adverbio
          for( int i=iWrd+1; i<Ora.Count; ++i )
            if( !AdvTypes.Contains(Ora[i].sTipo) ) return Ora[i];
          break;
        }
    
      return null;                                                                    // No se pudo obterener la palabra
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream dos bytes </summary>
    public static void Write( this BinaryWriter Wtr, byte ID, byte bData )
      {
      Wtr.Write( ID );                                                // Escribe identificador del comando, función o accion
      Wtr.Write( bData );                                             // Escribe dato de tipo byte
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream un byte y un entero de 16 bit </summary>
    public static void Write( this BinaryWriter Wtr, byte ID, UInt16 bData )
      {
      Wtr.Write( ID );                                                // Escribe identificador del comando, función o accion
      Wtr.Write( bData );                                             // Escribe dato de tipo UInt16
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream tres bytes </summary>
    public static void Write( this BinaryWriter Wtr, byte ID, byte bData1, byte bData2 )
      {
      Wtr.Write( ID     );                                             // Escribe identificador del comando, función o accion
      Wtr.Write( bData1 );                                             // Escribe dato de tipo byte
      Wtr.Write( bData2 );                                             // Escribe dato de tipo byte
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream un byte y una cadena</summary>
    public static void Write( this BinaryWriter Wtr, byte ID, string s )
      {
      Wtr.Write( ID );                                               // Escribe identificador del comando, función o accion

      var bytes  = Encoding.UTF8.GetBytes( s );                       // Obtiene bytes que representa la cadena en UTF8

      Wtr.Write( (byte)bytes.Length );                                // Escribe el tamaño de la cadena
      Wtr.Write( bytes );                                             // Escribe el contenido de la cadena
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream dos bytes y una cadena</summary>
    public static void Write( this BinaryWriter Wtr, byte ID, byte bData, string s )
      {
      Wtr.Write( ID );                                                // Escribe identificador del comando, función o accion
      Wtr.Write( bData );                                             // Escribe dato de tipo byte

      var bytes  = Encoding.UTF8.GetBytes( s );                       // Obtiene bytes que representa la cadena en UTF8

      Wtr.Write( (byte)bytes.Length );                                // Escribe el tamaño de la cadena
      Wtr.Write( bytes );                                             // Escribe el contenido de la cadena
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe hacia el stream un bytes y dos cadenas</summary>
    public static void Write( this BinaryWriter Wtr, byte ID, string s1, string s2 )
      {
      Wtr.Write( ID );                                                // Escribe identificador del comando, función o accion

      var bytes  = Encoding.UTF8.GetBytes( s1 );                      // Bytes que representan la primera cadena en UTF8

      Wtr.Write( (byte)bytes.Length );                                // Escribe el tamaño de la primera cadena
      Wtr.Write( bytes );                                             // Escribe el contenido de la primera cadena

      bytes  = Encoding.UTF8.GetBytes( s2 );                          // Bytes que representa la segunda cadena en UTF8

      Wtr.Write( (byte)bytes.Length );                                // Escribe el tamaño de la segunda cadena
      Wtr.Write( bytes );                                             // Escribe el contenido de la segunda cadena
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto función desde un stream</summary>
    public static IDictFunc FunFromStream(this BinaryReader Rdr)
      {
      var ID = (FuncId)Rdr.ReadByte();
      switch( ID )
        {
        case FuncId.ELEFT   : return DFuncELeft.FromStream   ( Rdr );           // Crea la función ELEFT         
        case FuncId.ERIGHT  : return DFuncERight.FromStream  ( Rdr );           // Crea la función ERIGHT         
        case FuncId.ISABR   : return DFuncIsAbr.FromStream   ( Rdr );           // Crea la función ISABR         
        case FuncId.ISINMODE: return DFuncIsInMode.FromStream( Rdr );           // Crea la función ISINMODE         
        case FuncId.ISSUFIX : return DFuncIsSufix.FromStream ( Rdr );           // Crea la función ISSUFIX         
        case FuncId.IST     : return DFuncIsT.FromStream     ( Rdr );           // Crea la función IST         
        case FuncId.ISW     : return DFuncIsW.FromStream     ( Rdr );           // Crea la función ISW         
        case FuncId.NNDD    : return DFuncNNDD.FromStream    ( Rdr );           // Crea la función NNDD         
        case FuncId.W       : return DFuncW.FromStream       ( Rdr );           // Crea la función W         
        case FuncId.L1      : return DFuncL.FromStream       ( Rdr, 0 );        // Crea la función L         
        case FuncId.L2      : return DFuncL.FromStream       ( Rdr, 1 );        // Crea la función L         
        case FuncId.L3      : return DFuncL.FromStream       ( Rdr, 2 );        // Crea la función L         
        case FuncId.N1      : return DFuncN.FromStream       ( Rdr, 0 );        // Crea la función N         
        case FuncId.N2      : return DFuncN.FromStream       ( Rdr, 1 );        // Crea la función N         
        case FuncId.N3      : return DFuncN.FromStream       ( Rdr, 2 );        // Crea la función N         
        case FuncId.LNDD    : return DFuncLNDD.FromStream    ( Rdr );           // Crea la función LNDD         
        case FuncId.FW      : return DFuncFW.FromStream      ( Rdr );           // Crea la función FW         
        case FuncId.LW      : return DFuncLW.FromStream      ( Rdr );           // Crea la función LW         
        case FuncId.INFORMAL: return DFuncInformal.FromStream();                // Crea la función INFORMAL         
        case FuncId.UPPER   : return DFuncUpper.FromStream   ();                // Crea la función UPPER         
        default:   return null;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un comando desde un stream</summary>
    public static IDictCmd CmdFromStream(this BinaryReader Rdr)
      {
      var Id = (CmdId) Rdr.ReadByte();                                // Lee identificador del comando
      switch( Id )
        {
        case CmdId.NEXT      : return DCmdNext.FromStream  ( Rdr );   // Crea un comando NEXT                  
        case CmdId.SETMODO   : return DCmdModo.FromStream  ( Rdr );   // Crea un comando SETMODO          
        case CmdId.SETPERSONA: return DCmdPerson.FromStream( Rdr );   // Crea un comando SETPERSONA       
        case CmdId.TIME      : return DCmdTime.FromStream  ( Rdr );   // Crea un comando TIME             
        case CmdId.ISLNDDSET : return DCmdDDSet.FromStream ( Rdr );   // Crea un comando ISLNDDSET        
        case CmdId.SETFW     : return DCmdSetFW.FromStream ( Rdr );   // Crea un comando SETFW            
        case CmdId.INS       : return DCmdIns.FromStream   ( Rdr );   // Crea un comando INS              
        case CmdId.DEL       : return DCmdDel.FromStream   ( Rdr );   // Crea un comando DEL              
        case CmdId.SETPLURAL : return DCmdPlural.FromStream(     );   // Crea un comando SETPLURAL        
        case CmdId.SETW      : return DCmdSetW.FromStream  ( Rdr );   // Crea un comando SETW             
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el identificador de la palabras sobre la que debe actual una función (Argumento)</summary>
    public static ConstArg GetWordInOraId( this string s )
      {
      switch( s )
        {
        case "W"   : return ConstArg.W;    
        case "NNDD": return ConstArg.NNDD;
        case "LNDD": return ConstArg.LNDD;
        case "N1"  : return ConstArg.N1;  
        case "N2"  : return ConstArg.N2;  
        case "N3"  : return ConstArg.N3;  
        case "L1"  : return ConstArg.L1;  
        case "L2"  : return ConstArg.L2;  
        case "L3"  : return ConstArg.L3;  
        case "FW"  : return ConstArg.FW;  
        case "LW"  : return ConstArg.LW;  
        }
    
      Dbg.Msg("Tipo de argumento no reconocido", Dbg.Error);
      return ConstArg.NA;                                            // Retorna set de palabras
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Codifica en un byte el argumento, consistente un número ó un identificador de palabra y un número</summary>
    public static byte CodWordAndNum( string[] args, string cmdName, int maxNum )
      {
      int nArg = args.Length;
      if( nArg!=1 && nArg!=2 ) 
        {
        Dbg.Msg("El número de argumentos del comando "+ cmdName +" no es el correcto", Dbg.Error);
        return 0xFF;
        }

      var sNum = (nArg==1)? args[0]: args[1];

      ConstArg WordId = 0;
      if( nArg==2 )
        {
        WordId = args[0].GetWordInOraId();
        if( WordId==ConstArg.NA ) 
          {
          Dbg.Msg( "El argumento de "+ cmdName +" debe ser un tipo de palabra", Dbg.Error );
          return 0xFF;
          }
        }

      int NMode;
      if( !int.TryParse( sNum, out NMode ) || NMode>maxNum )
        {
        Dbg.Msg("Argumento de la función "+ cmdName +" no es un entero");
        return 0xFF;
        }

      return (byte)(NMode<<4 | (byte)WordId);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena de caracteres desde un stream</summary>
    public static string ReadStr( this BinaryReader Rdr)
      {
      var len = Rdr.ReadByte();
      var bytes = Rdr.ReadBytes( len );

      return Encoding.UTF8.GetString( bytes );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene representación de un comando cuyo argumento puede ser un número ó un identificador de palabra y un número</summary>
    public static string ToStringWordAndNum( string cmdName, byte code )
      { 
      ConstArg WordId = (ConstArg)(code&0x0F);
      int         Num = (byte    )(code>>4  );
                       
      string sCmd = cmdName + '(';
      if( WordId!=0 ) sCmd += WordId + ","; 
      return sCmd += Num + ")"; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la palabra y el número codificado en Code</summary>
    public static int DecodWordAndNum(  int iWrd, List<Word> Words , byte Code, out Word word )
      {
      ConstArg IdWord = (ConstArg)(Code&0x0F);
      if( IdWord==0 ) IdWord = ConstArg.W;

      word = IdWord.GetWordInOra( iWrd, Words );

      return (byte)(Code>>4);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Codifica en un byte el argumento, consistente una constante del arreglo 'consts' o en número </summary>
    public static byte CodConstOrNum( this string arg, string[] consts )
      {
      for( int i=0; i<consts.Length && i<32; ++i )
        if( arg == consts[i] ) return (byte)i;

      byte nWord;
      if (!byte.TryParse(arg, out nWord) || nWord > 32)
        {
        Dbg.Msg("Valor de una acción incorrecto no es una constante valida ni un número", Dbg.Error );
        return 0xFF;
        }

      return (byte)(nWord + 32);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cadena que representa a una acción que puede tener como valor una constante o un número</summary>
    public static string ToStringConstOrNum( byte Num, string ActName, string[] consts )
      {
      if( Num<consts.Length ) return ActName + "=\"" + consts[Num] + '"';
      if( Num>=32 && Num<64 ) return ActName + "=\"wrd" + (Num-32).ToString() + '"';

      return ActName + "=\"ERROR VALUE\"";
      }

    } // +++++++++++++++++++++++++++++++++++ Fin DcUtil               ++++++++++++++++++++++++++++++++++++++++++++++++++
  }   // +++++++++++++++++++++++++++++++++++ Fin TrdEngine.Dictionary ++++++++++++++++++++++++++++++++++++++++++++++++++
