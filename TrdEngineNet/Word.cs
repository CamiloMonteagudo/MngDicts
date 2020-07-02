using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Dictionary;
using TrdEngine.TrdProcess;
using System.Text.RegularExpressions;

namespace TrdEngine
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Almacena toda la información para la traducción de una palabra</summary>
  public class Word
    {
    public WordData Data = null;            // campo data de diccionario

    public string Orig = "";                // Palabra original
    public string Origlwr = "";             // Pal original en minus la que se busca en dic
    public string Dest = "";                // Pal original del dicc en campo Destino
    public string Trad = "";                // Traduccion de la Palabra 
    public string Key = "";                 // campo key de diccionario
    public string sTipo = "";               // Tipo de la palabra según diccionario
    public string sTipo2 = "";              // Tipo auxiliar para verbos irreg, etc
    public string sComodin = "";            // Comodín que se utilizó para determinar género, etc
    public string sPrefijo = "";            // cadena con la ejecución del prefijo
    public string sSufijo = "";             // cadena con la ejecución del sufijo
    public string sKeyPrefijo = "";         // Prefijo de reducción encontrado
    public string sKeySufijo = "";          // Sufijo de reducción encontrado
    public string sInsert = "";             // palabra a insertar delante
    public string sAdd = "";                // palabra a adicionar detrás
    public string sParticle = "";           // particula separable de los verbos del alemán
    public string sSemantica = "";          // Campo semantica del diccionario
    public string RtfFijoIni = "";          // instrucciones RTF que permanecen  fijas al inicio de palabra
    public string RtfFijoFin = "";          // instrucciones RTF que permanecen  fijas al final de palabra
    public string StrReserva1 = "";         // Salva de traducción
    public string StrReserva2 = "";        
    public string StrReserva5 = "";
    public string StrSaveTrad = "";
    public int    Entre  = 0;                   // 0 entre nada, 1 entre parentesis, 2 entre comillas, 3 entre comillas simples, 4 entre corchetes
    public int    nSpace = 0;                   // Numero de espacios que siguen a la palabra

    public TMod   wModo;                        // Modo en que esta el verbo 
    public WCaso  wCase = WCaso.Lower;          // Si la pal original esta en upr, lwr o mixto
    public TGen   wGenero;                      // Genero  
    public TNum   wNumero;                      // Numero  
    public TPer   wPersona = TPer.Tercera;      // Persona 
    public TTime  wTiempo;                      // Tiempo del verbo      
    public DiWord wDiWord = DiWord.WORD;        // Tipo de la palabra según división de la oración
    public OParte wOParte;                      // Parte de la oración en que se encuentra la palabra
    public TDecl  wDeclination;                 // Declinación
    public TArtic wArticleType;                 // Tipo de articulo para la declinación de adjetivos
    public TGrad  wGrade;                       // Grado de significación del adjetivo superlativo, comparativo, superrlativo absoluto

    public Int16 Articulo;                      // 0 no lleva artículo, 1 lleva artículo, 2 lleva determinativo del otro sust,3 lleva 'of', 4 lleva 'of the', 5 lleva 'que'                      
    public Int16 Adiciona;                      // 0 no adicionar nada, 1 adiciona it      
    public char  Patron = ' ';

    // Variables booleanas
    public bool NoBusca;                    // Si la palabra se busca en dicc o no 
    public bool Buscada;                    // Si la palabra fue buscada en dicc o no 
    public bool NoEncontrada;               // Si la palabra fue buscada en dicc pero no fue encontrada
    public bool Traducida;                  // Si la palabra fue Traducida o no 
    public bool Delete;                     // Si la palabra esta borrada o no 
    public bool Plural;                     // si es plural 
    public bool Romano;                     // si puede ser # romano 
    public bool Posesivo;                   // si la palabra se encuentra entre comillas
    public bool Reflexivo;                  // si es verbo reflexivo o no
    public bool Femenino;                   // si la palbra esta en femenino por sufijo               
    public bool Abreviatura;                // si la palabra puede ser abreviatura
    public bool Negado;                     // Si es verbo y se encuentra negado
    public bool Compuesto;                  // Si el verbo se encuentra en un tiempo compuesto
    public bool NoEnComodin = false;        // Si la palabra forma parte del comodín
    public bool Concordada  = true;         // Si la palabra ya fue concordada

    public List<WordMean> AcepArray = new List<WordMean>();     // Lista de acepciones de la palabra
    public List<SubOrac>  SubOracs  = null;                     // Sub-Oraciones que van posterior a la palabra

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una palabra con con todos sus paramentros por defecto</summary>
    public Word()
      {
      }   
      
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una palabra con un texto</summary>
    ///<param name="w">Palabra con el que se quiere inicializar el objeto</param>
    public Word( string w )
      {               
      Orig = w;
      }         

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una palabra a partir de otra palabra</summary>
    ///<param name="w">Palabra de partida para crear el objeto</param>
    //public Word( Word w ) 
    //  {               
    //  Orig            = w.Orig;
    //  Origlwr         = w.Origlwr;
    //  Dest            = w.Dest;
    //  Trad            = w.Trad;
    //  Key             = w.Key;
    //  Data            = w.Data;
    //  DataC           = w.DataC;
    //  sTipo           = w.sTipo;
    //  sTipo2          = w.sTipo2;
    //  sComodin        = w.sComodin;
    //  sPrefijo        = w.sPrefijo;
    //  sSufijo         = w.sSufijo;
    //  sSufijo         = w.sKeyPrefijo;
    //  sSufijo         = w.sKeySufijo;
    //  EntreParentesis = w.EntreParentesis;
    //  sInsert         = w.sInsert;
    //  sAdd            = w.sAdd;
    //  sParticle       = w.sParticle;
    //  sSemantica      = w.sSemantica;
    //  EntreQue        = w.EntreQue;

    //  RtfFijoIni    = w.RtfFijoIni;
    //  RtfFijoFin    = w.RtfFijoFin;
    
    //  wModo        = w.wModo;
    //  wCase        = w.wCase;
    //  wGenero      = w.wGenero;
    //  wNumero      = w.wNumero;
    //  wPersona     = w.wPersona;
    //  wTiempo      = w.wTiempo;
    //  wDiWord      = w.wDiWord;
    //  wOParte      = w.wOParte;
    //  wDeclination = w.wDeclination;
    //  wArticleType = w.wArticleType;
    //  wGrade       = w.wGrade;

    //  Articulo      = w.Articulo;
    //  Adiciona      = w.Adiciona;
    //  NoBusca       = w.NoBusca;
    //  Buscada       = w.Buscada;
    //  BuscadaC      = w.BuscadaC;
    //  Traducida     = w.Traducida;
    //  Delete        = w.Delete;
    //  Plural        = w.Plural;
    //  Romano        = w.Romano;
    //  EntreComillas = w.EntreComillas;
    //  Posesivo      = w.Posesivo;
    //  TradTipo      = w.TradTipo;
    //  Reflexivo     = w.Reflexivo;
    //  LastPrepNoArt = w.LastPrepNoArt;        
    //  Femenino      = w.Femenino;
    //  Abreviatura   = w.Abreviatura;
    //  NotArtInOf    = w.NotArtInOf;
    //  ArtAtFirst    = w.ArtAtFirst;

    //  Negado    = w.Negado;
    //  Compuesto = w.Compuesto;

    //  NoEnComodin = w.NoEnComodin;
    //  Concordada  = w.Concordada;

    //  SustDerivado = w.SustDerivado;
    //  StrReserva1  = w.StrReserva1;
    //  StrReserva5  = w.StrReserva5;
    //  StrSaveTrad  = w.StrSaveTrad;
    //  StrIniBlank  = w.StrIniBlank;
    //  AcepArray    = w.AcepArray;
    //  }         

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Interpreta los datos simplificados de la palabra</summary>
    public void LengDictSimple( string sData )
      {
      var Items = sData.Split(',');

      for( int i=0; i<Items.Length; ++i )
        {
        var Item = Items[i];

        switch( Item.Substring(0,2) )                      
          {
          case "p1": wPersona = TPer.Primera; break;                  // Pone primera persona a la palabra Ej:SETPERSONA(1) => p1
          case "p2": wPersona = TPer.Segunda; break;                  // Pone segunda persona a la palabra Ej:SETPERSONA(2) => p2
          case "p3": wPersona = TPer.Tercera; break;                  // Pone tercera persona a la palabra Ej:SETPERSONA(3) => p3

          case "t0": wTiempo = TTime.Presente;  break;                  // Pone tiempo presente a la palabra  Ej:TIME(0) => t0
          case "t1": wTiempo = TTime.Pasado;    break;                  // Pone tiempo pasado a la palabra    Ej:TIME(1) => t1
          case "t2": wTiempo = TTime.Futuro;    break;                  // Pone tiempo futuro a la palabra    Ej:TIME(2) => t2
          case "t3": wTiempo = TTime.PasadoImp; break;                  // Pone tiempo pasadoImp a la palabra Ej:TIME(3) => t3

          case "m0": wModo = TMod.Infinitivo; break;                     // Pone modo infinitivo a la palabra Ej:SETMODO(0) => m0
          case "m1": wModo = TMod.Indicativo; break;                     // Pone modo indicativo a la palabra Ej:SETMODO(1) => m1
          case "m2": wModo = TMod.Imperativo; break;                     // Pone modo imperativo a la palabra Ej:SETMODO(2) => m2
          case "m3": wModo = TMod.Potencial ; break;                     // Pone modo potencial  a la palabra Ej:SETMODO(3) => m3
          case "m4": wModo = TMod.Subjuntivo; break;                     // Pone modo subjuntivo a la palabra Ej:SETMODO(4) => m4
          case "m5": wModo = TMod.Gerundio  ; break;                     // Pone modo gerundio   a la palabra Ej:SETMODO(5) => m5
          case "m6": wModo = TMod.Participio; break;                     // Pone modo participio a la palabra Ej:SETMODO(6) => m6

          case "a1": Adiciona = 1; break;                                 // Adiciona palabra del tipo 1 Ej: ADD(1) => a1
          case "a2": Adiciona = 2; break;                                 // Adiciona palabra del tipo 2 Ej: ADD(2) => a2
          case "a3": Adiciona = 3; break;                                 // Adiciona palabra del tipo 3 Ej: ADD(3) => a3

          case "T:": sTipo    = Item.Substring(2);   break;               // Pone el tipo a la palabra Ej: #TVV ó #T"VV" ....    => T:VV
          case "t:": sTipo2   = Item.Substring(2,2); break;               // Pone el tipo secundario Ej: SETTIPO2(AA) => t:AA

          case "pl": wNumero = TNum.Plural; break;                     // Pone el número plurar a la palabra Ej:SETPLURAL()  => pl
          case "fm": wGenero = TGen.Femen; break;                      // Pone genero femenino a la palabra Ej:SETFEMENINO() => fm

          case "E:":                                                      // Pone datos a la palabra Ej: #E"cerrar" => E:cerrar
            Data = WordData.CreateSingle( sTipo, Item.Substring(2) ); 
            break;   

          case "p:": sPrefijo = Item.Substring(2);   break;               // Pone prefijo a la palabra Ej: SETPREFIJO('"no @@"') => p:no @@
          case "s:": sSufijo  = Item.Substring(2);   break;               // Pone sufijo a la palabra Ej: SETSUFIJO('"más @@*"') => s:más @@*

          case "S:":  ChkSuffix( Item, ref Key,  true  ); break;          // Chequea sufijo para la llave Ej: SETKEYSUF("dría","er,ir") => S:dría-er;ir
          case "S=":  ChkSuffix( Item, ref Trad, false ); break;          // Chequea sufijo para la traducción Ej:SETTRADSUF("ty","dad") => S=ty-dad
          case "P:": ChkPreffix( Item, ref Key  ); break;                 // Chequea pefijo para la llave Ej: SETKEYPREF("inter","inter") => P:inter-inter
          case "P=": ChkPreffix( Item, ref Trad ); break;                 // Chequea pefijo para la traducción Ej: SETTRADPREF("aeto","eto") => P=aeto-eto 
         
          default:
            Dbg.Msg( "WARNING: El item data '" + Item + "' no es reconocido" ); 
            break;
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Chequea si una palabra tiene un sufijo de acuerdo a los datos de Item </summary>
    private void ChkSuffix( string Item, ref string sWrd, bool key )
      {
      var iSep = Item.IndexOf('/');                                 // Busca separdor de los argumentos
      var s1   = Item.Substring( 2, iSep-2 );                       // Obtiene el primer argumento
      var s2   = Item.Substring( iSep+1    );                       // Obtiene el segundo argumento

      if( sWrd.EndsWith(s1) )                                       // Si la palabra termina con el primer argumento
        {
        sWrd = Key.Substring( 0, Key.Length-s1.Length );            // Separa la raíz de la palabra

        if( key ) sKeySufijo = s2;                                  // Si la palabra es la llave, pone los sufijos a parte
        else      sWrd      += s2;                                  // Si no, lo pone con la raiz

        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Chequea si una palabra tiene un sufijo de acuerdo a los datos de Item </summary>
    private void ChkPreffix( string Item, ref string sWrd )
      {
      var iSep = Item.IndexOf('/');                                 // Busca separdor de los argumentos
      var s1   = Item.Substring( 2, iSep-2 );                       // Obtiene el primer argumento
      var s2   = Item.Substring( iSep+1    );                       // Obtiene el segundo argumento

      if( sWrd.StartsWith(s1) )                                     // Si la palabra empieza con el primer argumento
        sWrd = s2 + sWrd.Substring( s1.Length );                    // Sustituye inicio por segundo argumento
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna un lista con las posibles variantes de la palabra a buscar en el diccionario</summary>
    public List<string> FillWordsToFind()
      {
      var WrdsToFind = new List<string>();

      if( Posesivo )
        {
        string StrSuma = "";
             if( wDiWord == DiWord.POSESIVO_CS ) StrSuma = "'s";
        else if( wDiWord == DiWord.POSESIVO_SC ) StrSuma = "'";

        if( wDiWord == DiWord.POSESIVO_CS ) 
          WrdsToFind.Add( Orig + ((wCase==WCaso.Upper)? "'S" : "'s") );

        if( wDiWord == DiWord.POSESIVO_SC     ) WrdsToFind.Add( Orig + "'" );
        if( Origlwr != Orig                   ) WrdsToFind.Add( Origlwr + StrSuma );
        if( Key     != Origlwr && Key != Orig ) WrdsToFind.Add( Key + StrSuma );
        }

      WrdsToFind.Add( Orig );

      if( Origlwr != Orig ) WrdsToFind.Add( Origlwr );

      if( !string.IsNullOrEmpty(Key) && Key!=Origlwr && Key!=Orig )
        WrdsToFind.Add( Key );

      return WrdsToFind;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de conjugar la palabra </summary>
    internal bool ConjugateVerb( Translate Trd )
      {
      bool ret = false;
      if( NoEncontrada ) return ret;

      var ConjDes  = Trd.LangDes.Conj;
      var lngSrc   = Trd.LangSrc.Lang;
      var lngDes   = Trd.LangDes.Lang;
      var ConjTipo = Trd.LangDes.FindStrData("VERBO").Contains(sTipo);

      for( int ini =0;; )                                             // Repite el ciclo mientras halla algo que conjugar
        {
        string verboini, verbo, verbofin;

        ini = Trad.IndexOf('<', ini );                                // Busca si hay alguna palabra marcada para conjugar
        if( ini>=0 && (wModo==TMod.Infinitivo || ConjTipo) )          // Encuentra marca y el modo es infinitivo o el un tipo que se conjuga
          {
          int fin = Trad.IndexOf( '>', ini );                         // Busca la marca final
          if( fin==-1 ) return ret;                                   // Termina la conjugación                                 

          verboini = Trad.Substring( 0, ini );                        // Obtiene palabras antes de la palabra a conjugar
          verbo    = Trad.Substring( ini+1, fin-ini-1 );              // Obtene palabra a conjugar
          verbofin = Trad.Substring( fin+1  );                        // Obtiene palabras después de la palabra a conjugar
          }
        else                                                          // Si no hay ninguna palabra marcada
          {
          if( Trad.Contains(' ')  || !ConjTipo ) return ret;          // Si es una palabra compuesta o el tipo no se conjuga, termina

          verbo = Trad;                                               // Pone palabra a conjugar
          verboini = verbofin = "";                                   // La parte inicial y fina la pone vacia
          }
        
        TTime t = wTiempo;                                            // Determina el tiempo para conjugar
        if( lngDes==TLng.Fr && t==TTime.Pasado && wModo==TMod.Indicativo )
          {
          if( Key != "have" && !Compuesto )
            {
            Compuesto = true;
            t = TTime.Presente;
            }
          else
            if( t==TTime.Pasado ) t=TTime.PasadoImp;
          }
        else
          {
          if( lngSrc==TLng.En && (lngDes==TLng.It || lngDes==TLng.Es) && t == TTime.Pasado && wModo==TMod.Indicativo && (sTipo=="BE" || Reflexivo) ) 
            t = TTime.PasadoImp;
          }

        verbo = verbo.Replace("·","");                                // Quita puntos que puedan haber en la palabra
        var Conj = ConjDes.Conjugate( verbo, wModo,t, wPersona, wNumero, Negado, Compuesto );     // Conjuga la palabra

        if( Conj == "-" || Conj.Length==0 || Conj.Contains("xxxx") )  // Si no se pudo conjugar
          Conj = verbo;                                               // Toma la palabra como conjugación
        else 
          {
          ret = true;                                                 // Marca el retorno, que realizo al menos una conjugación
          if( lngDes==TLng.De )
            {
            var iFind = Conj.IndexOf('.');
            if( iFind != -1 )
              {
              sParticle = Conj.Substring( iFind+1 );
              Conj      = Conj.Substring( 0, iFind );
              }
            }
          }
        
        if( lngDes!=TLng.En && wModo==TMod.Participio )               // En inglés, si es un participio le agrega un asterisco
          Conj += "*";
        
        Trad = verboini + Conj + verbofin;                            // Forma la traducción con la palabra conjugada
        if( verbofin.Length == 0 ) return ret;                        // Si no queda nada después de la palabra termina

        ini = verboini.Length + Conj.Length;                          // Calcula posición donde se busca la proxima palabra a conjugar
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la palabras y las partes, asumiendo que 'fin' es un caracter ubicado al final de la palabra</summary>
    private string GetPartes( string sTrans, int fin, out string sInicio, out string sFinal )
      {   
      int ini = sTrans.LastIndexOf( ' ', fin );

      var sPalabra = sTrans.Substring( ini+1, fin-(ini+1) );
                
      sInicio = "";
      if( ini>=0 ) 
        sInicio = sTrans.Substring( 0, ini+1 );

      sFinal = "";
      if( fin < sTrans.Length-1 )    
        sFinal = sTrans.Substring( fin+1 );

      return sPalabra;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de concordar la palabra</summary>
    internal void MakeConcordance( Translate Trd )
      {   
      if( Concordada || NoEncontrada ) return;

      var Lng = Trd.LangDes;

      string sTrans = Trad.Replace("**", "*");

      if( (sTipo=="FS" || sTipo=="FO") && Trad.IndexOf(' ')==-1 && Trad.IndexOf('*')==-1 )
        {
        Trad = sTrans;
        Concordada = true;
        return;
        }

      if( sTipo=="NP" && sTrans.IndexOf('*') != -1 )
        sTrans = sTrans.Replace("*", "");

      var IsFemale   = Lng.FindStrData("TIPOFEMENINO"     ).Contains(sTipo);
      var IsPlural   = Lng.FindStrData("TIPOPLURAL"       ).Contains(sTipo);
      var IsGrade    = Lng.FindStrData("GRADE"            ).Contains(sTipo);
      var IsAdjDecl  = Lng.FindStrData("TIPOAADECLINACION").Contains(sTipo);
      var IsNounDecl = Lng.FindStrData("TIPOSSDECLINACION").Contains(sTipo);

         // OJO hay que preguntar si es adj o sust en español, francés e Italiano o sust en inglés
      if((  ( IsFemale   && wGenero      != TGen.Masc           )
          ||( IsPlural   && wNumero      == TNum.Plural         )
          ||( IsGrade    && wGrade       != TGrad.Positive        )
          ||( IsAdjDecl  && wDeclination != TDecl.NoDecline )
          ||( IsNounDecl && wDeclination != TDecl.NoDecline ) )
          &&( sTrans.IndexOf(' ')==-1 && sTrans.IndexOf('*')== -1) )
        sTrans += '*';

      var IsFemaleSinAster = Lng.FindStrData("FEMENINOSIASTERISCO").Contains(sTipo);
      var IsPluralSinAster = Lng.FindStrData("PLURALSIASTERISCO"  ).Contains(sTipo);

      var IsDe = (Lng.Lang == TLng.De);

      string sInicio, sFinal;
      for(;;)
        {
        int fin = sTrans.IndexOf('*');
        if( fin==-1 ) break;

        var sPalabra = GetPartes( sTrans, fin, out sInicio, out sFinal );
        var ChgWord  = (sPalabra!=sPalabra.ToLower());
    
        // Caso de concordar en Grado
        if( wGrade!=TGrad.Positive && IsGrade ) 
            sPalabra = Lng.PutAdjectiveInGrade( sPalabra, wGrade );

        // Caso de concordar en género femenino
        if( wGenero!=TGen.Masc && (IsFemale || IsFemaleSinAster) ) 
            sPalabra = Lng.PutWordInGender( sPalabra , wGenero );

        // Caso de concordar en número plural
        if( wNumero==TNum.Plural && 
           (!IsDe && (IsPlural || IsPluralSinAster) ) || (IsDe && (IsPlural && ChgWord) ) )     
            sPalabra = Lng.PutWordInNumber( sPalabra, wNumero );

        if( wDeclination!=TDecl.NoDecline )
          {
          // Caso de concordar en declinacion de adjetivos
          if( !IsDe && (IsAdjDecl || (!ChgWord && IsNounDecl )) )
              sPalabra = Lng.PutAdjectiveInDeclination( sPalabra, this );

          // Caso de concordar en declinacion de sustantivos
          if(  IsDe && (IsNounDecl && ChgWord) ) 
              sPalabra = Lng.PutNounInDeclination( sPalabra, this );
          }

        sTrans = sInicio + sPalabra + sFinal;
        }

      for(;;)
        {
        int fin = sTrans.IndexOf('^');
        if( fin == -1 ) break;

        var sPalabra = GetPartes( sTrans, fin, out sInicio, out sFinal );

        // Caso de concordar en género femenino
        if( wGenero!=TGen.Masc && IsFemale ) 
            sPalabra = Lng.PutWordInGender( sPalabra, wGenero );

        sTrans = sInicio + sPalabra + sFinal;
        }

      for(;;)
        {
        int fin = sTrans.IndexOf('~');
        if( fin == -1 ) break;

        var sPalabra = GetPartes( sTrans, fin, out sInicio, out sFinal );
        var ChgWord  = (sPalabra!=sPalabra.ToLower());

        // Caso de concordar en número plural
        if( wNumero == TNum.Plural )                          
          {
          if( (!IsDe && IsPlural) || (IsDe && IsPlural  && ChgWord) ) 
            sPalabra = Lng.PutWordInNumber( sPalabra, wNumero );
          }        

        sTrans = sInicio + sPalabra + sFinal;
        }

      Trad = sTrans;
      Concordada = true;
      }

    //------------------------------------------------------------------------------------------------------------------
    static string[] RefEs = {   "me",  "nos",   "se",   "te",  "vos" };
    static string[] RefIt = {   "mi","   ci",   "si",   "ti",   "vi" };                  
    static string[] RefFr = {   "me", "nous", "(se)",   "te"," vous" };                  
    static string[] RefDe = { "mich",  "uns", "sich", "sich",  "uns" };                  

    static Regex RefInfEs = new Regex("(ar|er|ir|ando|endo)\b");                  
    static Regex RefInfIt = new Regex("(ar|er|ir|ando|endo)e?\b");
    static Regex RefInfFr = new Regex("([^ ]+)(ar|er|ir|ando|endo)\b");
    
    static string[] SustEs1 =  new string[]{"endose","andose","endole","andole","endola","andola","endolas","andolas","endoles","andoles","endonos","andonos","endolo","andolo","endolos","andolos" };             
    static string[] SustEs2 =  new string[]{"éndose","ándose","éndole","ándole","éndola","ándola","éndolas","ándolas","éndoles","ándoles","éndonos","ándonos","éndolo","ándolo","éndolos","ándolos" }; 

    static string[] PrnsFr = new string[] { "me","te","se",    "nous","vous","se" };
    static string[] PrnsEs = new string[] { "me","te","se",    "nos" ,"vos" ,"se" };
    static string[] PrnsIt = new string[] { "mi","ti","si",    "ci"  ,"vi"  ,"si" };

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void PutReflexivePronoun( Translate Trd )
      {
      if( !Reflexivo ) return;

      var sTrd = Trad;
      var Lng = Trd.LangDes;

      string pronombre = "";

      int iPers = (int)wPersona;
      int iNum  = (int)wNumero;

      switch( Lng.Lang )
        {
        case TLng.Fr:
          if( sTrd.Contains("(se)") )
            {
            pronombre = PrnsFr[ iPers * (iNum+1) ];
            sTrd      = sTrd.Replace( "(se)", pronombre );
            }
          break;

        case TLng.Es:
          if( sTrd.Contains("(se)") )
            {
            pronombre = PrnsEs[ iPers * (iNum+1) ];
            sTrd      = sTrd.Replace( "(se)", pronombre );
            }
          break;

        case TLng.It:
          if( sTrd.Contains("(si)") )
            {
            pronombre = PrnsIt[ iPers * (iNum+1) ];
            sTrd      = sTrd.Replace( "(si)", pronombre );
            }
          break;
        }

      int i = 2;
      if( wPersona == TPer.Primera )            
          i = (wNumero == TNum.Singular )? 0 : 1;

      // Para el francés el reflexivo es diferente para cada persona no hay segunda de respeto
      if( wPersona == TPer.Segunda && Lng.Lang == TLng.Fr )  
          i = (wNumero == TNum.Singular)? 3: 4;

      var IsGerund     = Lng.FindStrData("GERUNDIO"   ).Contains(sTipo);
      var IsParticiple = Lng.FindStrData("PARTICIPIO" ).Contains(sTipo);
      var IsVerbal     = Lng.FindStrData("VERBO"      ).Contains(sTipo);

      if( IsGerund && wModo==TMod.Gerundio  && Lng.Lang==TLng.Es )
        {
        sTrd = RefInfEs.Replace( sTrd, "$1#", 1 );
        sTrd = sTrd.Replace( "#", RefEs[i] );

        for( int j=0; j<SustEs1.Length; ++j )
          {
          var s = SustEs1[j];
          if( sTrd.EndsWith(s) )
            {
            sTrd = sTrd.Substring( 0, sTrd.Length-s.Length ) + SustEs2[j];
            break;
            }
          }
        }    
      else if( wModo==TMod.Infinitivo && IsVerbal )
        {   
        if( Lng.Lang==TLng.It )
          {
          sTrd = RefInfIt.Replace( sTrd, "$1#", 1 );
          sTrd = sTrd.Replace( "#", RefIt[i] );
          }
        else if( Lng.Lang==TLng.Fr )
          {
          sTrd = RefInfFr.Replace( sTrd, "# ?1?2~", 1 );
          sTrd = sTrd.Replace( "#", RefFr[i] );
          }
        else
          {
          sTrd = RefInfEs.Replace( sTrd, "$1#", 1 );
          sTrd = sTrd.Replace( "#", RefEs[i] );
          }
        }    
      else if( wModo==TMod.Imperativo && !IsParticiple && IsVerbal )
        {   
        if( sTrd.IndexOf(' ')==-1 )   // solo lo puedo hacer si es palabra ya que hay que buscar el verbo en la frase y no lo hago todavía
          {                            // debe hacerse una función que conjugue teniendo en cuenta reflexivo y enclítico
          sTrd = " " + sTrd;

               if( Lng.Lang==TLng.It ) sTrd += RefIt[i];
          else if( Lng.Lang==TLng.Fr ) sTrd += RefFr[i];
          else                         sTrd += RefEs[i];
          }
        }    
      else if( wModo!=TMod.Gerundio && wModo!=TMod.Participio && !IsParticiple && IsVerbal )
        {   
             if( Lng.Lang==TLng.It ) sTrd = RefIt[i] + ' ' + sTrd;
        else if( Lng.Lang==TLng.De ) sTrd = sTrd + ' ' + RefDe[i];
        else if( Lng.Lang==TLng.Fr ) sTrd = RefFr[i] + ' ' + sTrd;
        else                         sTrd = RefEs[i] + ' ' + sTrd;
        }    

      Trad = sTrd.Replace("~","");
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena de traducción con todos los "Adornos" adicionales que lleva</summary>
    internal string GetStrTrd( Translate Trd )
      {
      var sTrd = new StringBuilder();
      if( RtfFijoIni.Length>0 ) sTrd.Append( RtfFijoIni );            // Si hay RTF inicio lo agrega la inicio de la traducción

      if( Entre != 0 ) sTrd.Append( Utils.EntreIni(Entre) );          // Si va encerrado entre algo, pone caracter inicial

		  if( NoEncontrada ) sTrd.Append( Trd.DUser.NfMarkIni );          // Pone marca inicial para palabras no encontradas

      sTrd.Append( Trad );                                            // Adicciona la traducción como tal

		  if( NoEncontrada ) sTrd.Append( Trd.DUser.NfMarkEnd );          // Pone marca final para palabras no encontradas
      
      if( Entre != 0 ) sTrd.Append( Utils.EntreIni(Entre) );          // Si va encerrado entre algo, pone caracter final

      if( RtfFijoFin.Length>0 ) sTrd.Append( RtfFijoFin );            // Si hay RTF final lo agrega la final de la traducción

      if( SubOracs != null )                                          // Si hay una o más suboración asociada
        {
        foreach( var sbOrac in SubOracs )                             // Recorre todas las suboraciones
          {
          sTrd.Append( ' ' );                                         // Pone un separador
          sTrd.Append( sbOrac.Translate(Trd) );                       // Le pone el caracter inicial que la encierra
          }
        }

      return sTrd.ToString();                                         // Retorna la traducción
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE Word         +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE TrdEngine  +++++++++++++++++++++++++++++++++++++++++++++
