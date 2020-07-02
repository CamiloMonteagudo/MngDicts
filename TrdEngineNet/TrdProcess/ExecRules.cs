#if MERCADO
  #undef TRACE_ENGINE
#endif
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TrdEngine.Data;
using TrdEngine.Dictionary;
using System.Windows.Forms;
using System.Diagnostics;

namespace TrdEngine.TrdProcess
  {
  public class ExecRules
    {
    Translate  Trd;                               // Todos los datos de la traducción
    Sentence   Orac;                              // Oración de trabajo

    bool AddWord;                                 // pregunta si se adicionan las palabras al arreglo de busqueda en el dcc
    bool AddType;                                 // pregunta si se adicionan los tipos entre $ al arreglo de busqueda en el dcc

    IDictTrd Dict = null;                         // Diccionario donde se buscan las reglas
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Contructor del objeto</summary>
    private ExecRules( Translate Trd, bool AddWord=true, bool AddType=true )
      {
      this.Trd = Trd;                                                 // Pone datos de traducción al objeto
      Orac = Trd.Ora;                                                 // Pone oración de trabajo

      this.AddWord = AddWord;                                         // Bandera para buscar reglas para palabras
      this.AddType = AddType;                                         // Bandera para buscar reglas para tipos
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el proceso de aplicar las reglas sobre la oración, para diccionario asocido a una dirección</summary>
    public static int Process( Translate Trd, DDirId DictId, bool AddWord=true, bool AddType=true )
      {
      var ExeRule = new ExecRules( Trd, AddWord, AddType );           // Crea objeto

      ExeRule.Dict = Trd.DirData.GetDict( DictId );                   // Carga diccionario asociado a dirección actual
      return ExeRule.Process( DictId.ToString() );                    // Busca reglas para toda la oración
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Ejecuta el proceso de aplicar las reglas sobre la oración, para diccionario asocido a un idioma</summary>
    public static int Process( Translate Trd, DLangId DictId, bool AddWord=true, bool AddType=true )
      {
      var ExeRule = new ExecRules( Trd, AddWord, AddType );           // Crea objeto

      ExeRule.Dict = Trd.LangSrc.GetDict( DictId );                   // Carga diccionario asociado al idioma fuente
      return ExeRule.Process( DictId.ToString() );                    // Busca reglas para toda la oración
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    private int Process( string DId )
      {
      if( Dict == null ) return 0;                                    // Si no hay diccionario, no hace nada

      Orac.PoneXX();                                                  // Pone XX al inicio y final de la oración
      Orac.SetNoEnComodin( false );                                   // Pone bandera SetNoEnComodin a falso en todas las palabras

      int n = 0;                                                      // Contador de número de frases casadas en la oración
      for( int i=0; i<Orac.Words.Count; i++ )                         // Recorre todas las palabras de la oración
        {
        var aFindInDcc = GetFindWords( Orac.Words[i] );               // Obtiene variantes para la palabra actual

        foreach( var sKey in aFindInDcc )                             // Recorre todas las variantes de la palabra actual
          {
          var RuleDat = Dict.GetRuleData( sKey );                     // La busca en el diccionario
          if( RuleDat == null ) continue;                             // No encuentra reglas, continua con la proxima variante

          var nMatch = RuleDat.Match( i, Orac );                      // Casa reglas para la palabra, en la oración
          if( nMatch < 0 ) continue;                                  // No casa ninguna, continua con proxima variante
          var Match = RuleDat.Matchs[nMatch];                         // Toma el Match que caso

          RuleInfoIni( i, sKey, nMatch, Match, DId );                 // Recupera información de regla casada

          Match.RunActions( i, Orac.Words );                          // Ejecuta las acciones del grupo Actions
          RuleInfoAtions( i, Match );                                 // Recupera información de estado de la palabras después del ACTION

          ProcessPhrases( i, Match );                                 // Procesa las acciones del grupo Phrases

          RuleInfoEnd();                                              // Termina recuperación y genenera mensaje para que se muestre

          ++nMatch;                                                   // Incrementa contador de reglas casadas
          break;                                                      // Pasa a la proxima palabra
          }
        }

      Trd.Ora.QuitaXX();                                              // Quita XX de inicio y final de la oración
      return n;                                                       // Retorna número de frases casadas
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa acciones en el grupo Phrases y la formación de la frase</summary>
    private void ProcessPhrases( int iWrd, RuleMatch Match )
      {
      if( Match.Phrases.Count == 0 ) return;                          // Si no hay acciones en grupo Phrases, termina

      var Ini  = Match.IniWord(iWrd);                                 // Obtiene indice a la primera palabra de la frase
      var End  = Match.EndWord(iWrd);                                 // Obtiene indice a la última palabra de la frase

      FindWordsInDict.Process( Trd, Trd.Ora.Words, Ini, End );        // Vuelve a buscar palabras de la frase en el diccionario
      TranslateWords .Process( Trd, Trd.Ora.Words, Ini, End );        // Traduce las palabras de la frase

      var Wrds = MakeWordFrase( Ini, End );                           // Obtiene palabras de la frase y las quita de la oración

      Match.RunPhrase( Wrds );                                        // Ejecuta acciones del grupo Phrases de la regla casada
      RuleInfoPhrase( Wrds );                                         // Recupera información de estado de la palabras después de PHARSE

      Concordance( Wrds, true );                                      // Concuerda palabras de la frase  
      ReplaceDollar( Wrds  );                                         // Sustituye marcadores $ en traduccón de la frase

      if( Trd.LangDes.Lang == TLng.De )                               // Solo para el alemán
        ReplaceParticle( Wrds );                                      // Sustituye las particulas dentro de la frase

      var FrWrd = Wrds[0];                                            // Palabra que representa la frase
      FrWrd.Buscada   = true;                                         // Pone que la palabra ya fue buscada
      FrWrd.Traducida = true;                                         // Pone que la palabra ya fue traducida
      FrWrd.wDiWord   = DiWord.WORD;                                  // Pone que es una palabra normal

      if( Trd.LangDes.Lang == TLng.En )                               // Solo para las frase guion hacia el Inglés
        FrWrd.Trad = FrWrd.Trad.Replace( " - ", "-" );                // Quita espacio con los guiones en la traducción

      RuleInfoPhrase2( Wrds );                                        // Recupera información de estado de la palabras después de PHARSE  
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una lista con las palabras de frase casada y las borra de la oración</summary>
    public List<Word> MakeWordFrase( int Ini, int End )
      {
      var Words = Orac.Words;                                         // Obtiene palabras de la oración

      int pos = 0;                                                    // Posicion dentro de la frase, 0-Inicio, 1-Intermedio, 2-Final
      int end = End;
      for( int i=Ini; i<=end; ++i )                                   // Recorre todas las palabras de la frase
        {
        if( Words[i].NoEnComodin )                                    // Si tiene bandera que no debe formar la frase
          {
          Words[i].NoEnComodin = false;                               // Restaura la bandera  
          if( pos==1 ) {End=i-1; pos=2;}                              // Es la posicion intermedia, redefine final y cambia a posición final
          }
        else                                                          // Debe formar parte de la frase
          if( pos==0 ) {Ini=i; pos=1;}                                // Es al inicio, refefine inicio y cambia a posición intermedia
        }

      var FraWrds = new List<Word>( (End-Ini) + 1 );                  // Crea lista de palabras de la frase

      var newWord = new Word();                                       // Crea una palabra nueva
      newWord.sTipo = "DD";                                           // Le pone el tipo por defecto adverbio

      FraWrds.Add( newWord );                                         // La agrega a lista como primera

      for( int i=Ini; i<=End; ++i )                                   // Recorre todas las palabras, incluidas la inicial y la final
        FraWrds.Add( Words[i] );                                      // La adicciona a lista de palabras de la frase

      Words.RemoveRange( Ini+1, End-Ini );                            // Quita todas las palabras de la lista menos la primera
      Words[Ini] = newWord;                                           // Pone la palabra nueva en lugar de la primera

      return FraWrds;                                                 // Retoran la lista de palabras
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las posibles variantes de la llave a buscar en el diccionario de comodines</summary>
    private List<string> GetFindWords( Word Wrd )
      {
      var Wrds = new List<string>();
 
      if( AddWord && Wrd.Orig.Length>0 && Wrd.Orig!="XXXX" )
        {
        Wrds.Add( Wrd.Origlwr );

        if( Wrd.Orig != Wrd.Origlwr                        )  Wrds.Add( Wrd.Orig );
        if( Wrd.Key  != Wrd.Origlwr && Wrd.Key != Wrd.Orig )  Wrds.Add( Wrd.Key   );
        }

      if( AddType )
        Wrds.Add( '$' +Wrd.sTipo + '$' );

      return Wrds;
      }

    static string TipoConjugar = "VV,VT,VI,VG,GT,GI,BE,HA,PT,PI,VP,HT,HI,VS,VA,JT,JI,DO,VR";        
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Sustituye los $[#] por la traducción de la palabra correspondiente al número dado dentro del comodín</summary>
    private void ReplaceDollar( List<Word> FrWrds )
      {
      var sTrad = FrWrds[0].Trad;

      var TFemale = Trd.LangDes.FindStrData("TIPOFEMENINO");       // Obtiene tipos femeninos desde el diccionario datos
      var TPlural = Trd.LangDes.FindStrData("TIPOPLURAL");         // Obtiene tipos plurales desde el diccionario datos

      var RtfIni = "";                                                // Almacena los RTF al inicio que no se tienen en cuenta
      var RtfFin = "";                                                // Almacena los RTF al final que no se tienen en cuenta
      for( int i = FrWrds.Count-1; i>0; --i )                         // Recorre las palabras de atras hacia alante
        {
        Word ws = FrWrds[i];                                          // Coge la palabra actual        

        string sNum = "$" + i;                                                // Forma el $[#] para esa palabra     
        if( !sTrad.Contains( sNum ) )                                         // Si no se encuentra $[#] en la cadena de traducción
          {
          if( ws.RtfFijoIni.Length>0 ) RtfIni = ws.RtfFijoIni + RtfIni;       // Acumula RTF inicial
          if( ws.RtfFijoFin.Length>0 ) RtfFin = ws.RtfFijoFin + RtfFin;       // Acumula RTF final
          continue;                                                           // Continua con proxima palabra de la frase
          }
          
        if( TipoConjugar.Contains( ws.sTipo ) )                       // Si el tipo es uno de los que hay que conjugar
          {
          ws.ConjugateVerb( Trd );                                    // Conjuga la palabra
          ws.PutReflexivePronoun( Trd );                              // Le pone el reflexivo si hace falta
          }

        var IsTipo  = TFemale.Contains(ws.sTipo) || TPlural.Contains(ws.sTipo);   // Si es un tipo que hay que concordar
        var HasChar = ws.Trad.IndexOfAny("*~^".ToCharArray())!=-1;                // Traducción tiene un marcador que indica concordar

        if( ws.sTipo!="NC" && (IsTipo || HasChar ) )                  // Si es un tipo o tiene un marcador para concordar
          {
          if( HasChar ) ws.Concordada = false;                        // Si tiene un marcador, fuerza concordar

          ws.MakeConcordance( Trd );                                  // Concuerda la palabra
          }

        sTrad = sTrad.Replace( sNum, ws.GetStrTrd(Trd) );             // Sustituye el $[#] por la traducción de la palabra   
        }

      FrWrds[0].Trad      = RtfIni + sTrad + RtfFin;                  // Pone traducción mas RTFs acumulados
      FrWrds[0].Traducida = true;                                     // Pone marca de traducida
      FrWrds[0].Buscada   = true;                                     // Pone marca de buscada
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Concuerda palabras de la frase</summary>
    private void Concordance( List<Word> FrWrds, bool bMakeCorcondance)
      {
      // pone el género y el número de los adjetivos
      for( int i=1; i<FrWrds.Count; i++ )
        {
        Word w = FrWrds[i];

        if( w.sTipo == "NC" )      // no coordinar
          {
          w.sTipo = "PT";
          if( w.wModo == TMod.Participio ) 
              w.ConjugateVerb( Trd );

          w.sTipo = "NC";
          if( !w.Trad.Contains(' ') && !w.Trad.Contains('*') )
            w.Trad += '*';
          }

        if( w.sTipo == "NG" && !w.Trad.Contains('<') && !w.Trad.Contains(' ') )
          w.Trad = "<" + w.Trad + ">";

        if( w.sTipo != "NC" && w.sTipo != "XX" && bMakeCorcondance )
          w.MakeConcordance( Trd );
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Sustituye cada ocurrencia de !número por la particula separable de la palabra correspondiente al número dado dentro del comodín</summary>
    private void ReplaceParticle( List<Word> FrWrds )
	    {
      var sTrad = FrWrds[0].Trad;
	    if( string.IsNullOrEmpty(sTrad) ) return;

		  for( int i=1; i<FrWrds.Count; ++i )
			  {
			  string tmp = "!"+(i+1);

        if( !sTrad.Contains( tmp ) ) continue;

				Word ws = FrWrds[i];

        string sTemp = " " + ws.sParticle;  // le pone un espacio delante
				FrWrds[0].Trad = sTrad.Replace( tmp, sTemp );
			  }
	    }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Aplica regla de coordinación y conjugación a una parte de la oración</summary>
    public static void ProcessCC( Translate Trd,  string sKey, int Ini, int End )
      {
      var Dict = Trd.GetDict( DDirId.CC );                          // Carga diccionario de conjugación y coordinación
      if( Dict == null ) return;                                    // Si no hay diccionario, no hace nada

      var Rl = new ExecRules( Trd, false, false );                  // Crea objeto

      var RuleDat = Dict.GetRuleData( sKey );                       // Busca en el diccionario y obtiene datos de la regla
      if( RuleDat == null ) return;                                 // No encuentra reglas (Siempre la debe encontar)

      var Match = RuleDat.Matchs[0];                                // Toma el primer Match (No hay que casar)

      Rl.RuleInfoIni( Ini, sKey, 0, Match, "CC" );                  // Recupera información de regla casada

      var Wrds = Rl.MakeWordFrase( Ini, End );                      // Obtiene palabras de la frase y las quita de la oración
      Match.RunPhrase( Wrds );                                      // Ejecuta acciones del grupo Phrases de la regla casada

      Rl.RuleInfoEnd( Wrds );                                       // Termina recuperación y genenera mensaje para que se muestre

      var FrWrd       = Wrds[0];                                    // Palabra que representa la frase
      FrWrd.Buscada   = true;                                       // Pone que la palabra ya fue buscada
      FrWrd.Traducida = true;                                       // Pone que la palabra ya fue traducida
      FrWrd.wDiWord   = DiWord.WORD;                                // Pone que es una palabra normal
      }

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Funciones para recuperar infomación sobre las reglas que casan, solo se usan para tracear las reglas
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#if TRACE_ENGINE   
    List<string> sDbgRgl = null;
    string DictId = "";
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Recupera la información sobre la regla casada y estado inicial de las palabras de la regla</summary>
    private void RuleInfoIni(int iWrd, string sKey, int nMatch, RuleMatch Match, string Id )
      {
      if( !Utils.IsTraceRule ) return;

      DictId  = Id;
      sDbgRgl = new List<string>(50);
      sDbgRgl.Add( string.Format("TIPO: {0}           LLAVE: {1}             MATCH: {2}", Id, sKey, nMatch+1 ) );

      int Ini, End, Key;
      if( Id != "CC")
        {
        Ini  = Match.IniWord(iWrd);                                 // Obtiene indice a la primera palabra de la frase
        End  = Match.EndWord(iWrd);                                 // Obtiene indice a la última palabra de la frase
        Key  = Ini + Match.idxKey - 1;
        }
      else
        {
        Ini = iWrd;
        End = iWrd + (sKey.Length-2)/3 - 1 ;
        Key  = Ini;
        }

      MatchWrdsToStr( Key, Ini, End );                                // Obtiene cadena con palabras que forman la frase

      bool key = false;
      foreach( var cond in Match.Conds )
        {
        var iw = cond.iWord;
        if( iw>Match.idxKey && !key )
          {
          sDbgRgl.Add( string.Format("> W{0} {1}", Match.idxKey, sKey ) );
          key = true;
          }

        sDbgRgl.Add( string.Format("  W{0} {1}", iw, cond.Funtion ) );
        }

      if( !key && Id!="CC" ) sDbgRgl.Add( string.Format("> W{0} {1}", Match.idxKey, sKey ) );
      
      if( Match.Actions.Count>0 )
        AtionsToStr( "-ACTIONES", Match.Actions );

      if( Match.Phrases.Count>0 )
        AtionsToStr( "-FRASE", Match.Phrases );

      sDbgRgl.Add( "_0" );
      int n = 0;
      for( int i=Ini; i<=End; ++i )
        GetWordInfo( ++n, Orac.Words[i] );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una cadena con una representación de las palabras de la oración que caso la regla</summary>
    private void MatchWrdsToStr(int Key, int Ini, int End)
      {
      var str = "-";                                                  // Marcador para que ponga linea resaltada
      for( int i=Ini; i<=End; ++i )
        {
        var sWrd = Orac.Words[i].Orig;
        if( sWrd.Length==0 ) sWrd = Orac.Words[i].Trad;

        if( i==Key ) sWrd = '[' + sWrd + ']';

        str +=  sWrd.Replace( ' ', '.' ) + ' ';
        }

      sDbgRgl.Add( str );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de un grupo de acciones en formato de cadena</summary>
    public void AtionsToStr( string GrpName, List<MatchAction> Lst )
      {
      int iWrd = -1;

      sDbgRgl.Add( GrpName );
      string str = "";
      for( int i=0; i<Lst.Count; ++i )
        {
        var act = Lst[i];

        if( iWrd != act.iWord )
          {
          if( i>0 )
            {
            sDbgRgl.Add( str );
            str = "";
            }

          str += "  W" + act.iWord + ' ';
          iWrd = act.iWord;
          }

        str += act.ToString() + ' ';
        }

      if( str.Length>0 ) sDbgRgl.Add( str );
      }

    //------------------------------------------------------------------------------------------------------------------
    static string[] sTime   = { "Pres", "Pas", "Fut", "PasI" };
    static string[] sNumero = { "S", "P" };
    static string[] sPerson = { "1", "2", "3" };
    static string[] sGenero = { "M", "F", "N" };
    static string[] sGrado  = { "Pos", "Com", "Sup" };
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena con la información de la palabra y la adiciona a la información sobre las reglas</summary>
    private void GetWordInfo(int n, Word Wrd)
      {
      var sWrd = new StringBuilder(200);

      sWrd.Append( n.ToString()               + '|' );
      sWrd.Append( Wrd.Orig                   + '|' );
      sWrd.Append( Wrd.Trad                   + '|' );
      sWrd.Append( Wrd.sTipo                  + '|' );
      sWrd.Append( Wrd.wModo.ToString()       + '|' );
      sWrd.Append( sGenero[(int)Wrd.wGenero ] + '|' );
      sWrd.Append( sNumero[(int)Wrd.wNumero ] + '|' );
      sWrd.Append( sPerson[(int)Wrd.wPersona] + '|' );
      sWrd.Append( sTime  [(int)Wrd.wTiempo ] + '|' );
      sWrd.Append( sGrado [(int)Wrd.wGrade  ] + '|' );
      sWrd.Append( Wrd.wCase.ToString()       + '|' );
      sWrd.Append( Wrd.sSemantica             + '|' );
      sWrd.Append( Wrd.Articulo.ToString()    + '|' );
      sWrd.Append( Wrd.sInsert                + '|' );
      sWrd.Append( Wrd.NoEnComodin? "No|" : "Si|" );
      sWrd.Append( Wrd.Reflexivo?   "Si|" : "No|" );
      sWrd.Append( Wrd.Compuesto?   "Si|" : "No|" );
      sWrd.Append( Wrd.Delete?      "Si|" : "No|" );
      sWrd.Append( Wrd.Negado?      "Si"  : "No" );

      sDbgRgl.Add( sWrd.ToString() );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Recupera la información sobre las palabras que forman la regla después de ACTION</summary>
    private void RuleInfoAtions(int iWrd, RuleMatch Match)
      {
      if( !Utils.IsTraceRule ) return;

      sDbgRgl.Add( "_1" );

      int n = 0;
      var Ini  = Match.IniWord(iWrd);                                 // Obtiene indice a la primera palabra de la frase
      var End  = Match.EndWord(iWrd);                                 // Obtiene indice a la última palabra de la frase

      for( int i=Ini; i<=End; ++i )
        GetWordInfo( ++n, Orac.Words[i] );
      }

    ///<summary>Recupera la información sobre las palabras que forman la regla después de PHRASE</summary>
    private void RuleInfoPhrase(List<Word> Wrds)
      {
      if( !Utils.IsTraceRule ) return;

      sDbgRgl.Add( "_2" );

      for( int i=0; i<Wrds.Count; ++i )
        GetWordInfo( i, Wrds[i] );
      }

    ///<summary>Recupera la información sobre las palabras al terminar de crear la frase</summary>
    private void RuleInfoPhrase2(List<Word> Wrds)
      {
      if( !Utils.IsTraceRule ) return;

      sDbgRgl.Add( "_3" );

      for( int i=0; i<Wrds.Count; ++i )
        GetWordInfo( i, Wrds[i] );
      }

    ///<summary>Recupera la información después PHRASE y llama al evento para mostrar los resultados</summary>
    private void RuleInfoEnd( List<Word> Wrds=null )
      {
      if( Wrds!=null ) RuleInfoPhrase(Wrds);
      Utils.TraceRgl( DictId, sDbgRgl, Trd );
      }
#else
    [ConditionalAttribute("TRACE_ENGINE")] private void RuleInfoIni(int iWrd, string sKey, int nMatch, RuleMatch Match, string Id ) {}
    [ConditionalAttribute("TRACE_ENGINE")] private void RuleInfoAtions(int iWrd, RuleMatch Match) {}
    [ConditionalAttribute("TRACE_ENGINE")] private void RuleInfoPhrase(List<Word> Wrds) {}
    [ConditionalAttribute("TRACE_ENGINE")] private void RuleInfoPhrase2(List<Word> Wrds) {}
    [ConditionalAttribute("TRACE_ENGINE")] private void RuleInfoEnd( List<Word> Wrds=null ) {}
#endif
    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE ExecRule     +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE   ++++++++++++++++++++++++++++++++++++++++++++++++++++++
