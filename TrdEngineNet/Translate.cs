using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TrdEngine.Data;
using TrdEngine.TrdProcess;
using System.Text.RegularExpressions;
using System;

namespace TrdEngine
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Contiene todos los datos y procedimientos necesarios para traducir una oración</summary>
  public class Translate
    {
    //------------------------------------------------------------------------------------------------------------------
    // Definición datos de la clase

    ///<summary>Localización donde se encuentran los diccionario</summary>
    static public string DicsPath{ get{ return TrdData.DicsPath; } set{ TrdData.DicsPath=value; }}

    ///<summary>Datos especificos para un usuario</summary>
    public UserData DUser{ get; private set;}

    ///<summary>Listado de los pasos necesarios para realizar una traducción</summary>
    public List<DoTranslateStep> Steps{ get; set;}

    ///<summary>Datos de la oración que se esta traduciendo</summary>
    public Sentence Ora{ get; private set;}

    ///<summary>Datos asociados al idioma fuente</summary>
    public LangData LangSrc{ get; private set;}

    ///<summary>Datos asociados al idioma destino</summary>
    public LangData LangDes{ get; private set;}

    ///<summary>Datos asociados a la dirección de traducción</summary>
    public TDirData DirData = null;

    ///<summary>Dirección de traducción</summary>
    public TDir Dir = TDir.NA;

    ///<summary>Datos asociados a la dirección de traducción</summary>
    public List<string> NotFoundWordList = new List<string>();

    //------------------------------------------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------------------------------------------
    public Translate( UserData UData )
      {
      DUser   = UData;
 
      Steps = new List<DoTranslateStep>() 
        {
        StepsStd.KillContractions, 
        StepsStd.ParseWordsStep,
        StepsStd.GetCaseProperty,
        StepsStd.FindWordsInDict,
        StepsStd.GetTypeOfWord,
        StepsStd.FindPhrases,
        StepsStd.FindPhrases,    
        StepsStd.GetTypeWordByWildCard,
        StepsStd.GetTypeOfWord,
        StepsStd.GetTypeWordByWildCard,
        StepsStd.FindWordsInDict,
        StepsStd.FindProperNoun,
        StepsStd.GetTypeWordByWildCard,
        StepsStd.TranslationWildCard,
        StepsStd.GetTypeWordByWildCard,
        StepsStd.TranslationWildCard,
        StepsStd.FindWordType,                                  // tipos por lenguaje de dcc
        StepsStd.TranslationWildCard,
        StepsStd.TranslationWildCard,
        StepsStd.GetTypeWordByWildCard,
        StepsStd.TranslateAllWords,
        StepsStd.TranslateBe,
        StepsOld.CmdCoorCnj,
        StepsOld.FindMood,
        StepsStd.InsertArticle,
        StepsStd.FillInsertWords,
        StepsStd.FindWordsInDict,
        StepsStd.TranslateAllWords,
        StepsStd.GenderAndNumber,
        StepsStd.TranslatePreffix,
        StepsStd.Conjugate,
        StepsStd.Ending,
        StepsStd.TranslateReflexiveVerbs,
        StepsStd.MakeTranslatedSentence,
        StepsStd.Details
        };
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga uno de los diccionarios asociados a la dirección de traducción</summary>
    public IDictTrd GetDict( DDirId Id ) { return DirData.GetDict( Id ); }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicaliza una traducción para una dirección de traducción</summary>
    /// <param name="Dir">Dirección de traducción que se quiere inicializar</param>
    /// <remarks> Esta función retorna false si la traducción no se pudo inicializar. Si ya habia una traducción 
    /// inicializada se cierra.</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public bool Open()
      {
      #if TRACE_ENGINE
      var Ini = Environment.TickCount;
      #endif

      var uDir = DUser.Dir;

      DirData = TrdData.GetDirData( uDir );
      LangSrc = new LangData();
      LangDes = new LangData();

      if( DUser.Initialize()           &&                             // Inicializa los datos del usuario  
          GetDict( DDirId.Data )!=null &&
          GetDict( DDirId.CT   )!=null &&                             // Carga comodines de traducción                            
          GetDict( DDirId.QCT  )!=null &&                             // Carga comodines de traducción de oraciones interrogativas
          GetDict( DDirId.ICT  )!=null &&                             // Carga comodines de traducción de oraciones imperativas   
          GetDict( DDirId.FCT  )!=null &&                             // Carga comodines de traducción de frases sustantivas      
          GetDict( DDirId.VCT  )!=null &&                             // Carga comodines de traducción de frases verbales         
          GetDict( DDirId.TCT  )!=null &&                             // 
          GetDict( DDirId.CC   )!=null &&                             // Carga comodines en frases de conjugación y coordinación  
          LangSrc.Load(Utils.Src(uDir), false) &&                     // Carga los datos del idioma fuente
          LangDes.Load(Utils.Des(uDir), true) )                       // Carga los datos del idioma destino
            {
            this.Dir = uDir;                                          // Establece la dirección actual

            #if TRACE_ENGINE
            Dbg.Msg( "Etapa 0 (Load "+ uDir +") = " + (Environment.TickCount-Ini), Dbg.Time);
            #endif

            ChangeWordType.Inicialize(this);                          // Inicializa los datos staticos de la clase  
            ListOfRoots.Inicialize();

            return true;                                              // Retorna OK
            }

      return false;                                                   // Retorna error
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary> Libera todos los recursos de la direccón de traducción</summary> 
    /// <remarks> Incluye tanto los recursos del objeto en si, como los compartidos como diccionarios.</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public void Free()
      {
      TrdData.Free( DUser.Dir );
      TrdData.Free( LangSrc.Lang );
      TrdData.Free( LangDes.Lang );

      Ora     = null;
      LangSrc = null;
      LangDes = null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce una oración</summary>
    ///
    ///<param name="Txt"> Oracion que se desea traducir.</param>
    ///
    ///<returns>Oracion traducida, si hay error retorna cadena vacia</returns>
    ///
    ///<remarks>Los idiomas entre los cuales se va a traducir se especifican en el objeto 'Translate' asociado este
    ///objeto, los parametros de la traducción se controlan a traves del objeto UserSet (USet) miembro de esta clase
    ///</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public string DoTranslate(string Txt)
      {
      if( Dir==TDir.NA ) return "";

      Ora = new Sentence( Txt );

      #if TRACE_ENGINE
      for( int i=0; i<Steps.Count; ++i )
        {
        var Ini = Environment.TickCount;                            // Tiempo de terminación de la etapa
        Steps[i]( this );
        var End = Environment.TickCount;

        Dbg.Msg( "Etapa " + (i+1) + "  (" + Steps[i].Method.Name + ") = " + (End-Ini), Dbg.Time);

        if( !Utils.TraceTrd( (i+1).ToString(), this  ) ) break;
        }
      #else
      for( int i=0; i<Steps.Count; ++i )
        Steps[i]( this );
      #endif

      return Ora.Dest;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce una oración en medio de la traducción de otra (Solo para uso interno del traductor)</summary>
    internal string DoSubTranslate( string Txt )
      {
      var tmpOra = Ora;
      var sTrd   = DoTranslate(Txt);
      Ora = tmpOra;

      return sTrd;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce un texto completo definido a través del 'Parse' de oraciones</summary>
    ///
    ///<param name="Ps">Objeto parse con el contenido del texto</param> 
    ///
    ///<returns>Retorna la cantidad de oraciones traducidas, y el texto traducido se pone en el campo correspondiente de 
    ///cada item traducible del parse</returns> 
    //------------------------------------------------------------------------------------------------------------------
    public int DoTranslate( Parse Ps )
      {
      if( Ps == null ) return 0;                                    // Verifica que el objeto parse es correcto

      int nOra = 0;                                                 // Contador de oraciones traducidas
      for( int i=0; i<Ps.Items.Count; ++i )                         // Recorre todos los items del texto
        {
        char c = Ps.Items[i].m_Type;                                // Coje tipo de item
        if( c=='t' || c=='T' )                                      // Si el Item es traducible
          {
          Ps.Items[i].m_Trd = DoTranslate( Ps.Items[i].m_Text );  // Lo traduce y lo pone en el campo correspondiente
          ++nOra;                                                   // Incrementa # de oraciones traducidas
          }
        }

      return nOra;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las oraciones que se pueden incluir en la memoria de traducción, a partir de una oración y su
    ///traducción </summary>
    ///
    ///<param name="Src">Oración origial a analizar</param>
    ///<param name="Trd">Traducción de 'Src'</param>
    ///
    ///<returns>Retorna un arreglo de cadenas, donde cada cadena representa dos oraciones (oración original, oración 
    ///traducida) separadas por un caracter '|', estas representan las oraciones y traduciones respectivas que se derivan
    ///de la información original</returns> 
    //------------------------------------------------------------------------------------------------------------------
    public string[] GetMemToolSenteces( string Src, string Trd )
      {
      string[] s = new string[0];                                   // Crea el arreglo que va a devolver

      return s;                                                     // Retorna el arreglo
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa el tipo de las palabras que tengan "lenguaje de diccionarios"</summary>
    internal void GetWordType()
      {
      var chgType = new ChangeWordType( this );                       // Inicializa clase para cambio de tipo

      for( int i=0; i<Ora.Words.Count; ++i )                          // Recorre todas las palabras de la oración  
        {
        var Wrd   = Ora.Words[i];                                     // Toma palabra actual
        var sType = Wrd.sTipo;                                        // Toma tipo actual

        if( sType.Length > 2 )                                        // Si el tipo tiene mas de dos caracteres
          Wrd.LengDictSimple( sType );                                // Lo procesa con lenguaje de diccionario simplificado
        else if( sType=="SW" )                                        // Si esta marcada en el diccionario como un tipo especial
          Wrd.sTipo = chgType.ProcessSpecialWord(i);                  // Obtiene el tipo mediante la clase ChangeWordType
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa los datos de la palabra 'iWrd', obtiene las acepciones adecuadas y ejecuta los comandos</summary>
    internal void ExecLengDict( int iWrd, List<Word> Words )
      {
      var Wrd  = Words[iWrd];                                         // Obtiene la palabra actual
      var Data = Wrd.Data;                                            // Obtiene los datos de la palabra
      if( Data==null ) return;                                        // Si no hay datos, mo hace nada

      for( int i=0; i<Data.MeanSets.Count; ++i )                      // Recorre todos los grupos de significados
        {
        var MSet = Data.MeanSets[i];                                  // Obtiene significado actual

        if( MSet.nSkip>1 )                                            // Es para agrupar varios significados bajo una condición
          {
          if( !MSet.Conds.Eval(iWrd, Words) )                         // Si la condición no se cumple para la palabra actual
            i += MSet.nSkip;                                          // Salta todos los MeanSet que estan bajo la condición

          continue;                                                   // Continua con el proximo, grupo de significados
          }

        if( MSet.Conds!=null && !MSet.Conds.Eval(iWrd, Words) )       // Si tiene condición y esta no se cumple
          continue;                                                   // Continua con la proxima palabra

        var acep = MSet.Means[0];                                     // Pone primer significado como traducción por defecto
        Wrd.Trad      = acep.Mean;                                    // Toma el significado (tradución)

        if( !Wrd.Femenino && Wrd.wGenero!=TGen.Femen )              // Si no se puso femenino anteriormente
          Wrd.wGenero   = acep.Gen;                                   // Toma genero del significado

        if( !Wrd.Plural && Wrd.wNumero!=TNum.Plural )               // Si no se puso el prural anteriormente
          Wrd.wNumero   = acep.Num;                                   // Toma número del significado

        Wrd.Reflexivo = acep.Refl;                                    // Toma si es reflexivo el significado

        Wrd.AcepArray = MSet.Means;                                   // Asocia lista de significados a la palabra

        foreach( var cmd in MSet.Cmds )                               // Recorre todos los comandos
          cmd.Exec( iWrd, Words );                                    // Los ejecuta

        break;                                                        // Termina procesamiento de los datos
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void WordTypeByLang()
      {
      var ChgWrd = new ChangeWordType( this, Ora.Words );

      for( int i=0; i<Ora.Words.Count; ++i )
        {
        var Wrd = Ora.Words[i];
        for( int j=0; j<4 && !Wrd.Traducida; ++j )
          {
          string TypPref = "P" + j;

          var newTipo = ChgWrd.ProcessWord( i, TypPref );
          if( newTipo != null )
            {
            Wrd.sTipo = newTipo;
            //Wrd.TradTipo = true;
            }
          }   

        string sPattern = LangSrc.FindTypeAndPattern( Wrd.sTipo );   
        if( !string.IsNullOrEmpty( sPattern ) )
            Wrd.Patron = sPattern[0];

        if( string.IsNullOrEmpty(Wrd.sTipo) )
          Wrd.sTipo = "SS";
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void TranslateTwoTranslationVerb()
      {
      for( int i=0; i<Ora.Words.Count; ++i )
        {
        var Wrd    = Ora.Words[i];
        var TmpStr = Wrd.Trad.Replace( "*", "" );

        if( LangDes.FindEspecialWord(TmpStr) )
          Ora.IsLastNotDDSetByType( i, "BE", "estar", "DD,DN,DF" );
        else
          {
          int pos = TmpStr.IndexOf(' ');
          if( pos != -1 )
            {
            TmpStr = TmpStr.Substring( 0, pos );
            if( LangDes.FindEspecialWord(TmpStr) )
              Ora.IsLastNotDDSetByType( i, "BE", "estar", "DD,DN,DF" );
            }
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void InsertOrAddWord()
      {
      for( int i=0; i<Ora.Words.Count; ++i )
        {
        var Wrd = Ora.Words[i];

             if( Wrd.sInsert.Length>0 ) InsertWord( ref i, Wrd.sInsert          , false, 0 );
        else if( Wrd.Articulo>0       ) InsertWord( ref i, "INSERT"+Wrd.Articulo, true , 0 );

             if( Wrd.sAdd.Length>0    ) InsertWord( ref i, Wrd.sAdd          , false, 1 );
        else if( Wrd.Adiciona>0       ) InsertWord( ref i, "ADD"+Wrd.Adiciona, true , 1 );
        }
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta en la posición 'Pos', la palabra 'sWord', si 'find' es true, busca la palabra en el diccionario</summary>
    private void InsertWord( ref int Pos, string sWord, bool find, int dt )
      {
      if( find ) sWord = DirData.FindTrdData( sWord );            // Si de indica, busca la palabra en el diccionario
      if( sWord==null ) return;                                   // No la encontro o era null, no hace nada
      
      var Words = sWord.Split( ';' );                             // Puede ser más de una Ej: "of the" (de el, de la)
      for( int j=0; j<Words.Length; ++j )                         // Inserta cada una de las palabras encontradas  
        {
        var newWord = new Word( Words[j] );                       // Crea una palabra nueva
                                       
        newWord.wCase = WCaso.Lower;                              // La pone en minusculas
        Ora.Words.Insert( Pos+dt, newWord );                      // La insera en la lista (dt=0 Delante, dt=1 Detras)
        ++Pos;                                                    // Corre la posición

        FindWordsInDict.FindInDic( this, newWord );               // La busca en el diccionario y toma sus datos
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void SetGenderAndNumber()
      {
      var ArticleType = LangDes.FindStrData("ARTICULO");

      for( int i=0; i<Ora.Words.Count-1; ++i )
        {
        var Wrd = Ora.Words[i];
        if( !ArticleType.Contains( Wrd.sTipo) ) continue;

        var NextWrd = Ora.Words[i+1];
                
        Wrd.wGenero = NextWrd.wGenero;
        Wrd.wNumero = NextWrd.wNumero;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene traducción de un sufijo de acuerdo a los argumentos del comando SETSUFIJO</summary>
    private string GetSufijoTrd( string SufArg, string sTrad )
      {                
      var sData = SufArg.Replace( "@@", sTrad );                      // Pone la traducción en el lugar marcado con @@

      int ini = sData.IndexOf('[');                                   // Busca inicio de lista de posibles sufijos
      int fin = sData.IndexOf(']');                                   // Busca final de lista de posibles sufijos
                        
      if( ini==-1 || fin ==-1 ) return sData;                         // Si no hay lista, retorna la cadena formada

      string Left     = sData.Substring( 0, ini );                    // Obtiene cadena a la izquierda de la lista
      string Right    = sData.Substring( fin+1  );                    // Obtiene cadena a la derecha de la lista
      string Corchete = sData.Substring( ini+1, fin-(ini+1) );        // Obtiene cadena entre corchetes

      var Sufs = Corchete.Split( '/' , '|' );                         // Separa cadenas delimitadas por / ó |

      for( int i=0; i<Sufs.Length-1; i+=2 )                           // Reorre cadenas de dos en dos   
        {
        var sufSrc = Sufs[i  ];                                       // Terminación que será resplazada
        var sufTrd = Sufs[i+1];                                       // Sufijo traducido para esa terminación

        if( Left.EndsWith(sufSrc) )                                   // Si terminación coincide con la actual
          {     
          int n = Left.Length - sufSrc.Length;                        // Calucula tamaño nuevo de la parte derecha
          return( Left.Substring( 0, n ) + sufTrd + Right);           // Forma la traducción y termina
          }
        }

      return sData;                                                   // Si ni encontor ninguna terminación, retorna cadena inicial
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void TranslatePreffix()
      {                                                    
      for( int i=0; i<Ora.Words.Count; ++i )                          // Recorre todas las palabras de la oración
        {
        var Wrd = Ora.Words[i];                                       // Coje la palabra actual
        if( Wrd.sTipo == "FS" && !string.IsNullOrEmpty(Wrd.sTipo2) )  // Si es una frase y tiene tipo2
            Wrd.sTipo = Wrd.sTipo2;                                   // Toma el tipo2 como tipo principal

        if( !string.IsNullOrEmpty(Wrd.sKeySufijo) )                   // Si la palabra tiene sufijo de reducción
          {
          string sufijo = '$' + Wrd.sKeySufijo + '$';                 // Encierra el sufijo entre $
          var sData = DirData.FindTrdSuffix( sufijo );                // Lo busca en el diccionario de sufijos de traducción

          if( sData != null )                                         // Si lo encuentra
            {  
            var idx = sData.IndexOf('#');                             // Busca separador entre datos asocidos al tipo o a la palabra
            sData = sData.Substring( idx+1 );                         // Obtiene solo los datos asociados a la palabra
                
            Wrd.LengDictSimple( sData );                              // Interpreta datos de la palabra con lenguaje simplificado

            if( Wrd.sSufijo.Length>0 )                                // Si se lleno la cadena sSufijo (habia SETSUFIJO)
              Wrd.Trad = GetSufijoTrd( Wrd.sSufijo, Wrd.Trad );       // Obtiene la traducción del sufijo para esa palabra
            }
          }

        if( string.IsNullOrEmpty(Wrd.sKeyPrefijo) ) continue;         // Si no tiene prefijo de reducción, continua con la proxima

        string prefijo = '$' + Wrd.sKeyPrefijo + '$';                 // Encierra el prefijo entre $

        var prefData = DirData.FindTrdPreffix( prefijo );             // Lo busca en el diccionario de sufijos de traducción
        if( prefData == null ) continue;                              // Si no lo encuentra continua con proxima palabra

        var iSep = prefData.IndexOf('#');                             // Busca separador entre datos asocidos al tipo o a la palabra
        prefData = prefData.Substring( iSep+1 );                      // Obtiene solo los datos asociados a la palabra
                
        Wrd.LengDictSimple( prefData );                               // Interpreta datos de la palabra con lenguaje simplificado

        if( Wrd.sPrefijo.Length>0 && Wrd.Traducida )                  // Si se lleno sPrefijo (habia SETPREFIJO)
          Wrd.Trad = Wrd.sPrefijo.Replace("@@",Wrd.Trad);             // Obtiene la traducción de refijo
        }
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE Translate    +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE TrdEngine  +++++++++++++++++++++++++++++++++++++++++++++
