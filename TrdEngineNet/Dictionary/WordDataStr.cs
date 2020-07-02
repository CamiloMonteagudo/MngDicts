using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrdEngine.Dictionary
  {
#if DIC_OLD_SUPPORT
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa soporte para leer los datos de una palabra desde una cadena de caracteres</summary>
  //------------------------------------------------------------------------------------------------------------------
  public partial class WordData
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto datos de palabras, a partir de una cadena</summary>
    public static WordData FromStr( string s )
      {
      if( s.Length<6 )                                              // Tamaño minimo para el formato de los datos
        {
        Dbg.Msg( "ERROR: El formato de los datos es incorrecto"); 
        return null;
        }

      var Wd = new WordData();                                      // Crea una palabra nueva

      int i = 0;
      var cType = GetMarker( s, 'T', ref i );                       // Obtiene contenido de #T
      if( cType==null || cType.Length !=2 )
        {
        Dbg.Msg( "ERROR: los datos deben comenzar con #T y dos letras"); 
        return null;
        }
      Wd.cType = cType;

      var mStr = GetMarker( s, 'M', ref i );                        // Obtiene contenido de #M
      if( mStr != null )                                            // Si lo pudo obtener
        Wd.M = mStr;                                                // Asigna información de M

      var sDat = GetMarker( s, 'E', ref i );                        // Obtiene contenido de #E
      if( sDat==null )
        {
        Dbg.Msg( "ERROR: Se esperaba #E"); 
        return null;
        }

      var iIni = sDat.IndexOf('{');                                 // Busca llave abierta
      if( iIni == -1 )                                              // No hay lenguaje de diccionario
        {
        var Ms = MeanSet.FromStr( sDat, null );                     // Obtiene los significados (sin condición)
        if( Ms == null ) return null;                               // Hubo un error al obtener los significados

        Wd.MeanSets.Add(Ms);                                        // Adiciona significados a la lista
        return Wd;                                                  // Retorna el objeto
        }

      if( !Wd.LengDictFromStr(sDat) )                               // Analiza los datos con lenguaje de diccionario
        return null;                                                // Hubo un error en el lenguje de diccionario

      return Wd;                                                    // Retorna la palabra
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene información asociada a un marcador, consistente en el caracter # y una letra</summary>
    public static string GetMarker( string s, char mark, ref int i )
      {
      int j=i;
      if( !s.SkipChar( '#', ref j) ) return null;                   // Busca inicio de la marca
      if( s[j] != mark ) return null;                               // Busca caracter de la marca

      ++j;                                                          // Salta el caracter
      var iEnd = s.IndexOf( '#', j );                               // Busca el proximo marcador
      if( iEnd==-1 )
        iEnd = s.Length;

      var sData = s.Substring( j, iEnd-j );                         // Obtiene datos entre los dos marcadores
      i = iEnd;                                                     // Actualiza puntero, hasta donde leyo
      
      return sData;                                                 // Retorna los datos
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa una cadena de lenguaje de diccionario y obtiene sus datos</summary>
    private bool LengDictFromStr( string s )
      {
      int Idx = 0;
      var sCond = s.GetBetween( ref Idx, '(', ')', false );         // Obtiene la condición, entre parentisis
      if( sCond==null ) 
        return Dbg.Msg( "ERROR: se esperaba una condición entre parentisis" );

      var Conds = new Conditions();
      if( !Conds.CondFromStr( sCond ) )                             // Interpreta y almacena la condición
        return false;                                               // Hubo un error procesando la condición

      if( !s.SkipChar('?', ref Idx) )                               // Se salta el caracter ?
        return Dbg.Msg( "ERROR: se esperaba un caracter '?' después de la condición" );

      if( !PartFromStr( s, ref Idx, Conds ) )                       // Analiza la parte verdadera y asocia condiciones
        return false;

      if( !s.SkipChar( ':', ref Idx ) )                             // Las dos partes tienen que estar separadas por :
        return Dbg.Msg( "ERROR: se esperaba un caracter ':' entre dos partes" );

      if( !PartFromStr( s, ref Idx, null ) )                        // Analiza la parte falsa que no lleva condiciones
        return false;

      if( Idx<s.Length )                                            // No se analizaron los datos hata el final
        Dbg.Msg( "WARNING: no se analizaron los datos hata el final, quedo: " + s.Substring(Idx) );

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una de las partes de la expresión desde una cadeena</summary>
    public bool PartFromStr( string s, ref int Idx, Conditions Conds )
      {
      if( s[Idx] == '{' )                                           // Sigue llave (Son significados)
        {
        var sMeans = s.GetBetween( ref Idx, '{', '}' );             // Obtiene los datos entre llaves
        if( sMeans == null ) 
          return Dbg.Msg( "ERROR: los significados deben estar entre llaves");

        var Ms = MeanSet.FromStr( sMeans, Conds );                  // Obtiene objeto MeanSet desde una cadena
        if( Ms == null ) return false;

        MeanSets.Add( Ms );                                         // Lo adiciona a la lista de MeanSet de la palabra
        return true;                                                // Retorna después de }
        }
      else if( s[Idx] == '(' )                                      // Sigue parentisis (Es una expresión o un grupo)
        {
        int tmp = Idx;
        var sData = s.GetBetween( ref Idx, '(', ')' );              // Obtiene los datos entre parentisis
        if( sData==null )
          return Dbg.Msg( "ERROR: No se puede cazar parentisis de cierre");

        if( s[Idx] == '?' )                                         // Es una expresión
          {
          sData = s.Substring( tmp );                               // Coge el resto de la cadena, desde (
          Idx  = s.Length;                                          // Pone posición al final

          return LengDictFromStr( sData );                          // Procesa la cadena hasta el final
          }
        else if( Idx==s.Length || s[Idx] == ':' )                   // Es un grupo
          {
          var Ms = new MeanSet( Conds );                            // Crea un MeanSet con las condiciones
          MeanSets.Add( Ms );                                       // Adiciona a la lista un MeanSet vacio
          var nMs = MeanSets.Count;                                 // Guarda # de MeanSet hasta ese momento

          if( !LengDictFromStr(sData) )                             // Analiza el grupo completo
            return false;

          Ms.nSkip = MeanSets.Count-nMs;                            // Pone a MeanSet vacio, cantidad de MeanSet a saltar
          return true;
          }
        else
          return Dbg.Msg("ERROR: se esperaba ? o :");               // Error, solo pueden estar ? ó :
        }
      else                                                          // Después de ? solo puede haber llave o parentisis
        return Dbg.Msg("ERROR: se esperaba { o (");
      }

    //------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    WordType                                     *********************************/
///<summary>Implementa lectura de las condiciones asociadas a un grupo de significados desde una cadena</summary>
//----------------------------------------------------------------------------------------------------------------------------------
  public partial class Conditions
    {
    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las condiciones desde una cadena</summary>
    public bool CondFromStr(string s)
      {
      int i=0;
      int logOp = opNone;

      for(;;)                                                       // Recorre todos los caracteres
        {
        if( s[i]=='(' )                                             // Es una agrupación de funciones
          {
          var sGrp = s.GetBetween( ref i, '(',')' );                // Obtiene los datos entre parentisis
          if( sGrp==null ) 
            {
            Dbg.Msg("ERROR: No estan casados los parentisis para agrupar funciones" );
            return false;                                       
            }

          if( !CondFromStr(sGrp) ) return false;                    // Procesa el grupo como una condición a parte
          }
        else
          {
          var Fun = FuncFromStr( s, ref i );                        // Obtiene una función a partir de la i
          if( Fun == null ) 
            {
            Dbg.Msg("ERROR: Se esperaba una definición de función o propiedad" );
            return false;                                       
            }
                     
          int IdxFun = Funcs.Count;                                 // Recupera el indice en la lista de funciones
          Funcs.Add( Fun );                                         // Adiciona la funnción a la lista
                                                                    
          Orden.Add( IdxFun );                                      // Pone la función en el orden de ejecución
          }
                                                            
        if( logOp != opNone )                                       // Si hay un operador definido
          {                                                 
          Orden.Add( logOp );                                       // Lo adiciona al orden de ejecución
          logOp = opNone;                                           // Y lo indefine
          }                                                 
                         
        if( i>=s.Length ) break;                                    // Ya llego al final

             if( s.SkipChar('|',ref i) ) logOp = opOr;              // Hay que hacer un 'OR'  con el proximo operando
        else if( s.SkipChar('&',ref i) ) logOp = opAnd;             // Hay que hacer un 'And' con el proximo operando
        else break;                                                 // Si no hay operación logica, termina la condición 
        }

      return( i==s.Length && logOp==opNone );                       // Solo retorna verdadero si esta al final
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una función a partir de la cadena 's' y avanza la i hasta el final de la definición de la función</summary>
    private static IDictFunc FuncFromStr( string s, ref int i )
      {
      int iIni = i;
      while( i<s.Length && s[i]>='A' && s[i]<='Z' ) ++i;          // Salta caracteres del nombre de la función

      string funcName = s.Substring( iIni, i-iIni  );             // Nombre de la función
      string Args     = "";                                       // Argumentos

      if( i<s.Length && s[i]>='1' && s[i]<='9' )                  // Si el nombre esta seguido de un número
        funcName += s[i++];                                       // Agrega el número y avanza la i

      if( i==s.Length )                                           // La función no tiene, ni parametros, ni valor
        return CreateFunc( funcName, Args );                      // Crea una función con la información obtenida

      if( s[i] == '(' )                                           // Es una función con argumentos
        {                                                        
        int IEnd = s.MatchChar( ref i, '(', ')' );                // Busca el parentisis final
        if( IEnd==-1 ) return null;                               // No lo encontro, error
                                                                   
        ++i;                                                      // Se salta primer parentisis
        Args = s.Substring( i, IEnd-i );                          // Coje contenido del parentisis (Argumentos)
                                                                   
        i = IEnd + 1;                                             // Se pone detras del parentisis
        }                                                        
      else if( s[i] == '=' )                                      // Es una propiedad (PName=Valor)
        {                                                        
        ++i;                                                      // Salta el signo de igual
        if( s[i]=='"' )                                           // Es una cadena
          {
          int j = i+1;                                            // Salta la primera comilla      
          while( j<s.Length && s[j]!='"' ) ++j;                   // Salta hasta la comilla de cierre

          Args = s.Substring( i, j-i+1 );                         // Coje cadena con comillas y todo
          i = j + 1;                                              // Se pone detras de la comilla
          }                                                       // Es un valor
        else
          {
          int j = i;                                              // Salta la primera comilla      
          while( j<s.Length && s[j]>='A' && s[j]<='Z' ) ++j;

          Args = s.Substring( i, j-i );                           // Coje contenido de la cadena
          i = j ;                                                 // Se pone al final del valor
          }
        }     
        
      return CreateFunc( funcName, Args );                        // Crea una función con la información obtenida
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una función de lenguaje de diccionario, a partir de su nombre y los argumentos</summary>
    public static IDictFunc CreateFunc( string Name, string args )
      {
      switch( Name )                                                    // Según el nombre de la función
        {
        case "ELEFT"   : return DFuncELeft.Create   ( args );           // Crea la función ELEFT
        case "ERIGHT"  : return DFuncERight.Create  ( args );           // Crea la función ERIGHT
        case "ISABR"   : return DFuncIsAbr.Create   ( args );           // Crea la función ISABR
        case "ISINMODE": return DFuncIsInMode.Create( args );           // Crea la función ISINMODE
        case "ISSUFIX" : return DFuncIsSufix.Create ( args );           // Crea la función ISSUFIX
        case "IST"     : return DFuncIsT.Create     ( args );           // Crea la función IST
        case "ISW"     : return DFuncIsW.Create     ( args );           // Crea la función ISW
        case "NNDD"    : return DFuncNNDD.Create    ( args );           // Crea la función NNDD
        case "W"       : return DFuncW.Create       ( args );           // Crea la función W
        case "L1"      : return DFuncL.Create       ( args, 0 );        // Crea la función L
        case "L2"      : return DFuncL.Create       ( args, 1 );        // Crea la función L
        case "L3"      : return DFuncL.Create       ( args, 2 );        // Crea la función L
        case "N1"      : return DFuncN.Create       ( args, 0 );        // Crea la función N
        case "N2"      : return DFuncN.Create       ( args, 1 );        // Crea la función N
        case "N3"      : return DFuncN.Create       ( args, 2 );        // Crea la función N
        case "LNDD"    : return DFuncLNDD.Create    ( args );           // Crea la función LNDD
        case "FW"      : return DFuncFW.Create      ( args );           // Crea la función FW
        case "LW"      : return DFuncLW.Create      ( args );           // Crea la función LW
        case "INFORMAL": return DFuncInformal.Create();                 // Crea la función INFORMAL
        case "UPPER"   : return DFuncUpper.Create   ();                 // Crea la función UPPER
        }

      Dbg.Msg("Error: función no reconocida '" + Name + '\'');
      return null;                                                      // No es una función conocida
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    MeanSet                                      *********************************/
///<summary>Almacena la información correspondiente a un grupo de significados</summary>
  public partial class MeanSet  
    {
    static Regex reMean = new Regex("(@((?<Esp>[A-Z]{2})|(\\[(?<Info>[^\\]]+)\\])))?\"(?<mean>[^\"]*)\"(?<Att>[$*!-]){0,3} *", RegexOptions.CultureInvariant);

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un MeanSet desde una cadena  y le asocia las condiciones 'Conds'</summary>
    public static MeanSet FromStr( string s, Conditions Conds )
      {
      var Means = new MeanSet( Conds );

      for(int Idx=0;;)                                                  // Recorre todos los significados
        {
        WordMean WM = new WordMean();                                   // Crea objeto para guardar significado

        Match m = reMean.Match( s, Idx );                               // Machea formato para un singnificado
        if( m.Success && m.Index==Idx  )                                // Macheo un significado en el incio de la cadena
          {
          WM.Mean = m.Groups["mean"].Value.Trim();                      // Pone significado en los datos (no es opcional)

          if( m.Groups["Esp"].Success )                                 // Se especifico la espacialidad
            WM.Esp = m.Groups["Esp"].Value;                             // La pone en los datos

          if( m.Groups["Info"].Success )                                // Se especifico información adicional
            WM.Info = m.Groups["Info"].Value;                           // La pone en los datos

          for( int i=0; i<m.Groups["Att"].Captures.Count; ++i )         // Recorre todos los atributos capturados
            {
            char c = m.Groups["Att"].Captures[i].Value[0];              // Obtiene primer caracter

                 if( c == '*' ) WM.Gen  = TGen.Femen;                 // Se especifico genero femenino
            else if( c == '-' ) WM.Gen  = TGen.Neutro;                // Se especifico genero neutro
            else if( c == '$' ) WM.Plur = true;                         // Se especifico número prural
            else if( c == '!' ) WM.Refl = true;                         // Se especifico que es una referencia
            }

          Means.Means.Add( WM );                                        // Adiciona significado al MeanSet

          Idx += m.Length;                                              // Salta la parte analizada
          }
        else
          {
          var Cmd = CmdFormStr( s, ref Idx );                           // Trata de obtener un comando
          if( Cmd == null )                                             // No lo pudo obtener, el comando
            {
            Dbg.Msg( "ERROR: no se encontro significados ni comandos" );
            return null;                                              
            }

          Means.Cmds.Add( Cmd );                                        // Adicciona el comando a la lista
          }

        if( s.Length == Idx )                                           // Analizo hasta el final de la cadena
          return Means;                                                 // Retorna OK

        if( s[Idx++] != ',' )                                           // Separador entre signicados o comandos
          {
          Dbg.Msg( "ERROR: se esperaba una , para separar los significados" );
          return null;                                                  // No existe separador entre significados
          }
        }
      }

    static Regex reCmd = new Regex("(?<name>[A-Z]+)\\((?<args>[^\\)]*)\\) *", RegexOptions.CultureInvariant);

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de obtener un comando a partir de la posición idx de la cadena str</summary>
    public static IDictCmd CmdFormStr( string str, ref int idx)
      {
      Match m = reCmd.Match( str, idx );                                // Machea formato para un comando
      if( !m.Success || m.Index>idx ) return null;                      // No es un comando, error de formato
                                                                        
      var Name = m.Groups["name"].Value;                                // Coje el nombre del comando

      var Args = "";
      if( m.Groups["args"].Success )                                    // Si macheo algún argumento
        Args = m.Groups["args"].Value;                                  // Obtiene obtiene valor de los argumentos
                                                                        
      var aArgs = Args.Split(',');                                      // Obtiene arreglo de argumentos

      IDictCmd Cmd = null;
      switch( Name )                                                    // Según el nombre del comando
        {
        case "NEXT"      : Cmd = DCmdNext.Create  ( aArgs ); break;     // Crea un comando NEXT     
        case "SETMODO"   : Cmd = DCmdModo.Create  ( aArgs ); break;     // Crea un comando SETMODO
        case "SETPERSONA": Cmd = DCmdPerson.Create( aArgs ); break;     // Crea un comando SETPERSONA
        case "TIME"      : Cmd = DCmdTime.Create  ( aArgs ); break;     // Crea un comando TIME
        case "ISLNDDSET" : Cmd = DCmdDDSet.Create ( aArgs ); break;     // Crea un comando ISLNDDSET
        case "SETFW"     : Cmd = DCmdSetFW.Create ( aArgs ); break;     // Crea un comando SETFW
        case "INS"       : Cmd = DCmdIns.Create   ( aArgs ); break;     // Crea un comando INS
        case "DEL"       : Cmd = DCmdDel.Create   ( aArgs ); break;     // Crea un comando DEL
        case "SETPLURAL" : Cmd = DCmdPlural.Create( aArgs ); break;     // Crea un comando SETPLURAL
        case "SETW"      : Cmd = DCmdSetW.Create  ( aArgs ); break;     // Crea un comando SETW
        default: 
          Dbg.Msg( "ERROR: Comando no reconocido '" + str + '\'' );     // El nombre del comando no es reconocido
          return null;
        }
                                                                        
      if( Cmd != null )                                                 // Si lo pudo crear el comando correctamente
        idx += m.Length;                                                // Salta la parte analizada

      return Cmd;                                                       // Retorna el comando creado
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    }
#endif
  }
