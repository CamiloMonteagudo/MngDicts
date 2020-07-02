using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine.Dictionary
  {
#if CMD_SUPPORT
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el soporte para leer los datos de las reglas desde ficheros CMD</summary>
  public partial class RuleData
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene todas las reglas asociadas a una cadena, desde una cadena en formato CND</summary>
    ///
    ///<param name="s">Cadena con la definición de las reglas </param>
    ///<param name="Defs">Diccionario con las definiciones de constantes para sustitución</param>
    ///
    ///<returns>Retorna objeto con los datos de las reglas, si hubo problema al parsear la cadena retorna null</returns>
    public static RuleData FromStr( string s, RulesTables Tbls  )
      {
      var RData = new RuleData();

      int i = 0;
      while( i<s.Length )
        {
        var line = s.GetLine(ref i);
        if( line.StartsWith("MATCH") )
          {
          int j = 5;
          var sArgs = line.GetBetween( ref j, '(', ')' );
          if( sArgs == null ) 
            {
            Dbg.Msg("Los parentisis del MATCH no estan balanceados");
            return null;
            }

          var sBody = s.GetBetween( ref i, '{', '}' );
          if( sBody==null )
            {
            Dbg.Msg("Las llaves del MATCH no estan balanceados");
            return null;
            }

          var RMatch = RuleMatch.FromStr( sArgs, sBody, Tbls );
          if( RMatch==null ) return null;

          RData.Matchs.Add( RMatch );
          }
        }

      return RData;
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleData ++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Mantiene todos los datos de un Match</summary>
  public partial class RuleMatch
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las condiciones y acciones asociadas a un match desde una cadena</summary>
    public static RuleMatch FromStr(string sArgs, string sBody, RulesTables Tbls )
      {
      var RMatch = new RuleMatch();

      if( !RMatch.GetConditions( sArgs, Tbls ) ) 
        return null;
    
      int idx = 0;
      if( !RMatch.GetActions( sBody, ref idx, Tbls ) )
        return null;
          
      if( idx<sBody.Length )
        {
        if( !RMatch.GetActions( sBody, ref idx, Tbls ) )
        return null;
        }
    
      return RMatch;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene todas las acciones contenidas dentro del grupo 'GrpName' desde una cadena</summary>
    private bool GetActions( string s, ref int idx, RulesTables Tbls )
      {
      while( idx+1<s.Length && s[idx]=='/' && s[idx+1]=='/' )               // Si la linea comienza con comentario
        s.GetLine(ref idx);                                                 // Salta la linea

      if( idx==s.Length ) return true;                                      // Todas la lineas estaban comentariadas

      List<MatchAction> Acts;

      int i = idx;
      var GName = s.GetName( ref i );                                       // Obtiene nombre del grupo de acciones

           if( GName=="ACTION" ) Acts = Actions;                            // Pone actual el arreglo de acciones
      else if( GName=="PHRASE" ) Acts = Phrases;                            // Pone actual el arreglo de frases
      else return Dbg.Msg("Se esperaba ACTION o PHRASE");                   // Grupo incorrecto

      var sGrp = s.GetBetween( ref i, '{', '}' );                           // Obtiene el contenido del grupo
      for( int j=0;; )                                                      // Recorre todo el contenido
        {
        if( !sGrp.SkipChar('#', ref j) )                                    // Salta el signo #
          return Dbg.Msg("Se esperaba caracter #");

        if( !char.IsDigit( sGrp[j]) )                                       // Verifica que esta seguido de un número
          return Dbg.Msg("Se esperaba un número");

        int iWord = int.Parse( sGrp.GetValue(ref j) );                      // Obtiene el número

        var sAct = sGrp.GetBetween( ref j, '{', '}' );                      // Obtiene el contenido de las acciones de la palabra
        if( sAct==null ) 
          return Dbg.Msg("Los corchetes no estan balanceados");

        for( int k=0;; )                                                    // Recorre el contenido
          {
          if( k+1<sAct.Length && sAct[k]=='/' && sAct[k+1]=='/' )           // Si esta comentariado
            sAct.GetLine(ref k);                                            // Lo salta
          else
            {
            var AName = sAct.GetName( ref k );                              // Obteiene el nombre de la acción
            if( AName==null )
              return Dbg.Msg("Error al obtener nombre de la acción");

            if( !sAct.SkipChar( '=', ref k ) )                              // Salta el signo de igual
              return Dbg.Msg("Se esperaba un signo =");

            var AValue = sAct.GetValue( ref k );                            // Obtiene el valor de la acción
            if( AValue==null )
              return Dbg.Msg("Error al obtener valor de la acción");

            AValue = AValue.Expand( Tbls.Defs, Tbls.expand );               // Sustitulle definiciones de constantes

            var Fun = CreateAction( AName, AValue, Tbls );                  // Crea función con nombre y valor
            if( Fun==null ) 
              return Dbg.Msg( "Error creando la accion '" + AName + '=' + AValue );

            Acts.Add( new MatchAction(iWord, Fun) );                        // Adiciona una acción para la palabra
            }

          if( k>=sAct.Length ) break;                                       // Llego al final de la palabra, rompe
          }
        
        if( j==sGrp.Length )                                                // Llego al final del grupo, termina
          {
          idx = i;                                                          // Actualiza el indice hasta donde llego
          return true;                                                      // Retorna OK
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las condiciones asociadas a un match desde una cadena</summary>
    private bool GetConditions( string s, RulesTables Tbls )
      {
      int i=0, iWord=1;
      char sep = '+';

      while( i<s.Length )
        {
        if( s[i]=='(' ) { sep='|'; ++i;}

        var FName = s.GetName( ref i );
        if( FName==null ) 
          return Dbg.Msg("No se pudo obtener el nombre de la condición");

        string Args = s.GetBetween( ref i, '(', ')' );
        if( Args==null ) 
          return Dbg.Msg("Los parentisis de los argumentos no estan balanceados");

        if( FName=="KWRD" )
          idxKey = iWord;
        else
          {
          Args = Args.Expand( Tbls.Defs, Tbls.expand );

          var Fun = CreateFuntion( FName, Args, Tbls ); 
          if( Fun==null )
            return Dbg.Msg( "Error creando la función '" + FName + '(' + Args + ')' );

          Conds.Add( new MatchCond(iWord, Fun) );
          }

        if( i<s.Length && s[i]==')' ) {sep='+'; ++i;};

        if( i<s.Length && !s.SkipChar(sep, ref i ) ) 
          return Dbg.Msg("Se esperaba el separador '" + sep + '\'');

        if( sep=='+' ) ++iWord;
        if( sep=='|' ) ++nAndOp;
        }

      return true;
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleMatch +++++++++++++++++++++++++++++++++++++++++++++++++++++
#endif

#if CMD_SUPPORT || XML_SUPPORT
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Codigo común para ficheros CMD y XML</summary>
  public partial class RuleMatch
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la función según el 'sName' y la asigna los argumentos</summary>
    public static IRuleFun CreateFuntion( string sName, string sArgs, RulesTables Tbls )
      {
      if( !Tbls.expand )
        return RGeneric.Create( sName + ";" + sArgs );

      switch( sName )
        {
        case "AUPR"  : return RFuncAUpr.Create  ( sArgs, Tbls );
        case "FIELD" : return RFuncField.Create ( sArgs, Tbls );
        case "FUPR"  : return RFuncFUpr.Create  ( sArgs, Tbls );  
        case "SUFFIX": return RFuncSuffix.Create( sArgs, Tbls );
        case "TYPE"  : return RFuncType.Create  ( sArgs, Tbls );  
        case "WORD"  : return RFuncWord.Create  ( sArgs, Tbls );  
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las condiciones y acciones asociadas a un match desde una cadena</summary>
    public static IRuleAct CreateAction( string aName, string aValue, RulesTables Tbls )
      {
      if( !Tbls.expand )
        return RGeneric.Create( aName + "=" + aValue );

      switch( aName )
        {
        case "CASE"       : return RActCase.Create       ( aValue, Tbls );
        case "CATEGORY"   : return RActCategory.Create   ( aValue, Tbls );
        case "COMPLEX"    : return RActComplex.Create    ( aValue, Tbls );  
        case "DELETE"     : return RActDelete.Create     ( aValue, Tbls );
        case "GENDER"     : return RActGender.Create     ( aValue, Tbls );  
        case "GRADE"      : return RActGrade.Create      ( aValue, Tbls );  
        case "GRAMTYPE"   : return RActGramType.Create   ( aValue, Tbls );  
        case "INSERT"     : return RActInsert.Create     ( aValue, Tbls );  
        case "KEY"        : return RActKey.Create        ( aValue, Tbls );  
        case "MODE"       : return RActMode.Create       ( aValue, Tbls );  
        case "NEGATIVE"   : return RActNegative.Create   ( aValue, Tbls );  
        case "NUMBER"     : return RActNumber.Create     ( aValue, Tbls );  
        case "PERSON"     : return RActPerson.Create     ( aValue, Tbls );  
        case "REFLEXIVE"  : return RActReflexive.Create  ( aValue, Tbls );  
        case "TIME"       : return RActTime.Create       ( aValue, Tbls );  
        case "TOPHRASE"   : return RActToPhrase.Create   ( aValue, Tbls );  
        case "TRANSLATE"  : return RActTranslate.Create  ( aValue, Tbls );  
        case "TRANSLATION": return RActTranslation.Create( aValue, Tbls );  
        }

      return null;
      }
    }
#endif


  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }
