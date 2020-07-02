using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;
using TrdEngine.Dictionary;
using System.Text.RegularExpressions;

namespace TrdEngine.TrdProcess
  {
  public class FindWordsInDict
    {
    public int WordCount = 0;       // Cantidad de palabras buscadas
    public int NotFoundCount = 0;   // Cantidad de palabras no encontradas

    Translate Trd = null;           // Datos de la traducción
    IDictTrd  GenDict;              // Diccionario general
    List<Word> Words;               // Lista de palabras a buscar

    int Ini;                        // Primera palabra de la lista a buscar
    int End;                        // Ultima palabra de la lista a buscar

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase pata buscar palabras en el diccionario</summary>
    public FindWordsInDict( Translate Trd, List<Word> Wrds=null, int Ini=0, int End=-1 )
      {
      this.Trd = Trd;

      Words = (Wrds==null)? Trd.Ora.Words: Wrds;                      // Valida lista de palabras a buscar

      int MaxEnd = Words.Count-1;                                     // Determina indice de la ultima palabra de la lista

      this.Ini = (Ini<0)? 0 : Ini;                                    // Valida indice de la primera palabra a buscar
      this.End = (End<0 || End>MaxEnd)? MaxEnd : End;                 // Valida indice de la ultima palabra a buscar

      GenDict = Trd.GetDict( DDirId.Gen );                            // Obtiene el diccionario general de palabras
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca palabras de una lista en el diccionario y actualiza sus datos</summary>
    static public void Process( Translate Trd, List<Word> Wrds=null, int Ini=0, int End=-1 )
      {
      var FWrd = new FindWordsInDict( Trd, Wrds, Ini, End );
      FWrd.Process();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca palabras de una lista en el diccionario y actualiza sus datos</summary>
    public void Process()
      {
      if( GenDict==null ) return;

      var IsDe = (Trd.LangSrc.Lang==TLng.De);                         // Si el idioma es Aleman

      for( int i=Ini; i<=End; ++i )                                   // Recorre todas las palabras
        {
        var Wrd = Words[i];                                           // Coge palabra actual
        if( Wrd.Buscada || Wrd.NoBusca ) continue;                    // Si no hay que buscarla, continua con la proxima

        WordCount++;                                                  // Cuenta la palabra que se buscan

        bool    inDic = false;                                        // si se encontro la palabra en dicc
        string buffer = Wrd.Origlwr;                                  // salva de Origlwr

        // Si es aleman y la 1ra palabra de la oración busco primero si verbo
        if( IsDe && Wrd==GetFirstWord() )
          {
          Wrd.Key = Wrd.Origlwr;
          inDic = FindInDic( Wrd );
          }
        else
          {
          if( Wrd.Posesivo )                                          // Si es posesivo
            {
             Wrd.Key = Wrd.Orig + '\'';                               // Le agrega la comilla
            if( Wrd.wDiWord== DiWord.POSESIVO_CS )                    // Si es del tipo con s
              Wrd.Key = Wrd.Key + ((Wrd.wCase==WCaso.Upper)? 'S':'s');  // Le agraga la s según el case

            if( !FindInDic(Wrd) )                                     // Si no encuentra la palabra en el diccionario
              {
              Wrd.Key.ToLower();                                      // La pone en minusculas
              if( !FindInDic(Wrd) ) Wrd.Posesivo = false;             // Si tampoco la encuentra, no es posesivo
              }

            inDic = Wrd.Posesivo;                                     // Si continua posesivo, la encontro
            }

          // búsqueda de cualquier palabra
          if( inDic==false )
            {
            Wrd.Key = Wrd.Orig;
            inDic = FindInDic( Wrd );
            }
          }

        // palabras que son nombres propios pero que no llevan traducción
        // ya que es el mismo del original
        if( inDic==true && Wrd.sTipo == "TT" )
          {
          Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;
          Wrd.sTipo       = "NP";
          Wrd.Traducida   = true;
          Wrd.Buscada     = true;

          continue;
          }

        // Parte para los acronimos que dan tantos bateos
        if( Trd.Ora.AllUpper == false )
          {
          Regex grep = new Regex( "^[A-Z][A-Z]+s$", RegexOptions.CultureInvariant );

          if( (Wrd.wCase == WCaso.Upper || grep.IsMatch( Wrd.Orig ) ) && Wrd.Orig.Length < 9 && Wrd.Orig.Length > 1 )
            {
            if( Wrd.Orig == Wrd.Key )
              {
              if( grep.IsMatch( Wrd.Orig ) )
                {
                Wrd.Orig     = Wrd.Orig.Substring( 0, Wrd.Orig.Length-1 );
                Wrd.wNumero = TNum.Plural;
                Wrd.Key      = Wrd.Orig;
                }

              inDic = FindInDic( Wrd );
              if( inDic==false )
                {
                Wrd.Trad      = Wrd.Orig;
                Wrd.Key       = Wrd.Orig;
                Wrd.Trad      = Wrd.Orig;
                Wrd.sTipo     = "NP";
                Wrd.Traducida = true;
                Wrd.Buscada   = true;

                inDic = true;
                continue;
                }
              }
            }
          }

        var NoRedTipos = Trd.LangSrc.FindStrData( "TIPOSINREDUCCION" );

        if( inDic && Wrd.wCase!=WCaso.Upper && ( !IsDe || Wrd!=GetFirstWord() ) )
          {
          if( Wrd.wDiWord == DiWord.ABREVIATURA )
            Wrd.wCase = WCaso.Lower;    

          if( inDic && (Wrd.Key == Wrd.Origlwr && (IsDe && Wrd.wCase==WCaso.Lower) && !NoRedTipos.Contains(Wrd.sTipo) ) )
            inDic = FindRedWord( i, Wrd );

          Wrd.Key = Wrd.Orig;
          }
        else if( !inDic  && !IsDe )
          {
          Wrd.Key = Wrd.Origlwr;
          inDic = FindInDic( Wrd );
          }

        if( inDic && (Wrd.wCase == WCaso.Upper || Wrd.sTipo.Length != 2 || IsDe || Wrd.Key.Contains('$')) )
            {}
        else if( !Wrd.Key.Contains(' ') && (!inDic || (inDic && Wrd.Key == Wrd.Origlwr && !NoRedTipos.Contains(Wrd.sTipo) ) || 
                    (inDic && (Wrd.wCase != WCaso.First &&  Wrd.wCase != WCaso.Other ) && !NoRedTipos.Contains(Wrd.sTipo) ) ) )
          {
          if( inDic) FindRedWord( i, Wrd );
          else       inDic = FindRedWord( i, Wrd );
          }

        if( !inDic && (Wrd.wCase==WCaso.Other || Wrd.sTipo=="NP" || (Wrd.Orig.Contains(' ') && Wrd.wCase==WCaso.First) ) )   // si el caso es mixto es Nombre Propio
          {
          Wrd.sTipo = "NP";
          Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;     
          Wrd.Traducida = true;
          NotFoundCount++;

          Trd.NotFoundWordList.Add( Wrd.Orig );
          }
        else
          {
          // Parte para que las letras independientes se cojan dentro de los comodines de frases idiomaticas como <LL1> o <LU1> segun mayusculas o minusculas
          if( Wrd.Key.Length == 1)
            {
            if( !string.IsNullOrEmpty(Wrd.sSemantica) )
              Wrd.sSemantica += ";";

            if(Wrd.Orig == Wrd.Origlwr)
              Wrd.sSemantica += "LL";
            else
              Wrd.sSemantica += "LU";

            if( !inDic )
              {
              Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;
              Wrd.sTipo = "SS";
              Wrd.Traducida = true;
              Wrd.Buscada = true;
              inDic = true;
              }
            }

          if( !inDic ) inDic = FindIrrgVerb(Wrd);                   // Prueba primero si es un verbo irregular
          if( !inDic ) inDic = FindKeySufijo( Wrd );                // Prueba si tiene sufijo
          if( !inDic ) inDic = FindKeyPrefijo( Wrd, false );        // Prueba si tiene prefijo

          if( !inDic && Wrd.wCase == WCaso.Upper && !Wrd.Key.Contains(' ') && Wrd.Key.Length<6 && !Trd.Ora.AllUpper && !Wrd.Romano )
            {
            Wrd.sTipo = "NP";
            Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;     
            Wrd.Traducida = true;

            Trd.NotFoundWordList.Add( Wrd.Orig );
            NotFoundCount++;
            }

          string issimo = "issimo";
          string issima = "issima";

          if( ( Wrd.Origlwr.EndsWith(issimo) || Wrd.Origlwr.EndsWith(issima) ) &&  Trd.LangSrc.Lang == TLng.It )
            {
            string Raiz = Wrd.Origlwr.Substring( 0, Wrd.Origlwr.Length-issimo.Length );

            string buscar;
            buscar = Raiz +  'o';
            Wrd.Key = buscar;

            inDic = FindInDic( Wrd );

            if( !inDic )
              {
              buscar = Raiz + 'a';
              Wrd.Key = buscar;
              inDic = FindInDic(Wrd);
              }

            if( !inDic )
              {
              buscar = Raiz +  'e';
              Wrd.Key = buscar;
              inDic = FindInDic(Wrd);
              }

            if( inDic )
              {
              Wrd.sTipo = "AA";
              Wrd.sPrefijo = "\"very @@\"";
              Wrd.Buscada = true;
              Wrd.Trad = "";
              }
            }

        if( !inDic && !Wrd.Traducida )
          {
          if( Wrd.Romano )
            {
            inDic = true;
            Wrd.StrReserva1 = Wrd.Trad = Wrd.Key;
            Wrd.sTipo = "NN";
            Wrd.Traducida = true;

            Trd.NotFoundWordList.Add( Wrd.Orig );
            NotFoundCount++;
            }                      
          else if( Wrd.wCase == WCaso.First )
            {
            Wrd.sTipo = "NP";
            Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;     
            Wrd.Traducida = true;

            Trd.NotFoundWordList.Add( Wrd.Orig );
            NotFoundCount++;
            }
          else if( Wrd.wCase == WCaso.Upper )
            {    
            Wrd.Key = Wrd.Origlwr.FirstToUpper();
            inDic = FindInDic(Wrd);
            if( inDic )  // caso en que la palabra esta en mayusculas y lo que se encuentra en el dcc es un NP con 1ra mayusculas
              {
              Wrd.Buscada = true;
              Wrd.Traducida = false;
              inDic = true;
              Wrd.Trad = "";
              }
            else
              {
              Wrd.StrReserva1 = Wrd.Trad = Wrd.Orig;     // por ahora no voy a ponerle marca de no encontrada a acrónimos
              Wrd.Traducida = true;
              Wrd.sTipo = "SS";

              Trd.NotFoundWordList.Add( Wrd.Orig );
              NotFoundCount++;
              }
            }
          else
            {    
            Wrd.sTipo        = "SS";
            Wrd.Trad         = Wrd.Orig;
            Wrd.StrReserva1  = Wrd.Trad;
            Wrd.Traducida    = true;
            Wrd.NoEncontrada = true;

            Trd.NotFoundWordList.Add( Wrd.Orig );
            NotFoundCount++;
            }
          }
        }

        if( Wrd.Plural                      ) Wrd.wNumero = TNum.Plural;
        if( Wrd.Femenino                    ) Wrd.wGenero = TGen.Femen;
        if( string.IsNullOrEmpty(Wrd.sTipo) ) Wrd.sTipo   = "SS";

        if( Wrd.Posesivo )
          {
          string PuedeSerSust = "XQ,UR,XA,QF,TA,XB,ZB,UO,XH,QO,QN,XJ,QA,SV,YF,XG,ZF,ZJ,ZZ,YG,XK,UA,QB,QC,QD,UZ,UC,UM,UT,UX,UI,UE,UJ,QJ,QK,QL,QM,SS";

               if( Wrd.sTipo == "NP"                             ) Wrd.sTipo = "GV";
          else if( PuedeSerSust.Contains(Wrd.sTipo)              ) Wrd.sTipo = "GW";
          else if( Wrd.Orig!=Wrd.Key && Wrd.wCase == WCaso.First )
            {
            Wrd.sTipo     = "GV";
            Wrd.Trad      = Wrd.Orig;
            Wrd.Traducida = true;
            }
          else if( Wrd.sTipo == "ST"                             ) Wrd.sTipo = "GB";
          else                                                     Wrd.sTipo = "GW";
          }

        Wrd.Buscada = true;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna la primera palabra de la oración, no tiene en cuenta el XX</summary>
    public Word GetFirstWord()
      {       
      if( Words.Count == 0 ) return null;

      Word w  = Words[0];
      if( w.sTipo == "XX" && Words.Count>1 )
        w = Words[1];

      return w;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    private bool FindRedWord( int Pos, Word Wrd) // Llama Función de Patricia
      {
      var find = new FindRootsInDict( Trd, Wrd );

      if( !find.UpdateWordData() ) return false;

      var palabra = find.DirectPronoun;
      if( !string.IsNullOrEmpty( palabra ) )
        {
        var newWord = new Word( palabra );
                                   
        newWord.wCase = Wrd.wCase;

        Words.Insert( Pos, newWord );
        ++Pos;
        ++End;
        }

      palabra = find.IndirectPronoun;
      if( !string.IsNullOrEmpty( palabra ) )
        {
        var newWord = new Word( palabra );
                                   
        newWord.wCase = Wrd.wCase;
        Words.Insert( Pos, newWord );
        ++End;
        }

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una palabra en el diccionario</summary>
    public static bool FindInDic( Translate Trd, Word Wrd )
      {
      var obj = new FindWordsInDict( Trd );
      return obj.FindInDic( Wrd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una palabra en el diccionario</summary>
    private bool FindInDic( Word Wrd, bool getType = true )
      {          
      if( GenDict==null ) return false;

      var WData = GenDict.GetWordData( Wrd.Key );
      if( WData == null ) return false;

      Wrd.Data       = WData;
      Wrd.sSemantica = WData.M;

      if( getType ) Wrd.sTipo = WData.CompType;
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>De los datos simplificados de la palabra extrae el tipo y retorna los datos</summary>
    private string SplitData( string sWData, Word Wrd ) 
      {  
      var idx = sWData.IndexOf('#');
      if( idx>0 )  
        Wrd.sTipo = sWData.Substring( 0, idx );

      return sWData.Substring( idx+1 );
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    private bool FindIrrgVerb( Word Wrd) 
      {      
      var sData = Trd.LangSrc.FindIrrWords( Wrd.Key );
      if( sData ==null ) return false;

      sData = SplitData( sData, Wrd );
      Wrd.LengDictSimple( sData );

      if( Wrd.Data == null ) return false;

      Wrd.Key = Wrd.Data.MeanSets[0].Means[0].Mean;

      return FindInDic( Wrd, false );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    private bool FindTradSufijo( Word Wrd ) 
      {      
      string key   = Wrd.Key;
      string sData = null;

      for( int i=0; i<key.Length; ++i )                                     // Acorta la llave hasta penultimo caracter
        {
        string suffix = key.Substring(i);                                   // Toma sufijo a partir del caracter i

        sData = Trd.DirData.FindTrdSuffix( suffix );
        if( sData == null ) continue;           

        sData = SplitData( sData, Wrd );
        Wrd.LengDictSimple( sData );

        Wrd.Traducida = true;
        return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    private bool FindTradPrefijo( Word Wrd ) 
      {      
      string key  = Wrd.Key;
      string sData = null;
    
      for( int i=key.Length-1; i>0; --i )
        {
        var pref = key.Substring( 0, i );
        sData = Trd.DirData.FindTrdPreffix( pref );

        if( sData==null ) continue;

        sData = SplitData( sData, Wrd );
        Wrd.LengDictSimple( sData );

        Wrd.Traducida = true;
        return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca el sufijo de una palabra (optimizada por mi)</summary>
    private bool FindKeySufijo( Word Wrd ) 
      {      
      string Key = Wrd.Key;                                           // Preserva la llave original

      for( int i=0; i<Key.Length; ++i )                               // Acorta la llave hasta penultimo caracter
        {
        string suffix = Key.Substring(i);                             // Toma sufijo a partir del caracter i

        var sData = Trd.LangSrc.FindSuffixReduction( suffix );        // Lo busca en el diccionario de sufijos de reducción
        if( sData == null ) continue;                                 // No la encontro, termina

        sData = SplitData( sData, Wrd );                              // Separa el campo tipo del campo datos
        Wrd.LengDictSimple( sData );                                  // Intepreta campo dato, con leguaje simplificado

        var raiz  = Wrd.Key;                                          // LengDict pone raiz en Key
        var suffs = Wrd.sKeySufijo.Split('|');                        // LengDict pone posibles sufijos en sKeySufijo, separados por |

        foreach( var suf in suffs)                                        // Recorre todos los posibles sufijo
          {
          Wrd.Key = raiz + suf;                                       // Forma la llave con la raíz y sufijo actual
          if( FindInDic( Wrd, false ) )                               // La busca en el diccionario (sin cambiar el tipo)
            {
            Wrd.sKeySufijo = Key.Substring(raiz.Length) ;             // Actualiza sufijo encontrado
            return true;                                              // Retorna que encontró un sufijo
            }
          }

        Wrd.Key = Key;                                                // Restaura los datos de la llave
        }

      Wrd.sKeySufijo = "";                                            // Ignora sufijo
      Wrd.sTipo      = "";                                            // Ignora el tipo encontado

      return false;                                                   // Retorna sufijo no encontrado
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca el prefijo de una palabra</summary>
    private bool FindKeyPrefijo( Word Wrd, bool reduce) 
      {      
      string key   = Wrd.Key;                                         // Guarda el valor original de la llave
      string sData = null;                                            // Inicializa datos de la llave (no encontrados)
      string Pref  = null;                                            // Inicializa valor del prefijo (no encontrado)
    
      for( int i=key.Length-1; i>0; --i )                             // Recorre caracteres desde el ultimo hasta el primero
        {
        Pref  = key.Substring( 0, i );                                // Toma prefijo como los primeros i caracteres

        sData = Trd.LangSrc.FindPreffixReduction( Pref );             // Lo busca en diccionario de prefijos para el idioma
        if( sData==null ) continue;                                   // No lo encontró, continua la busqueda

        Wrd.sKeyPrefijo = Pref;                                       // Guarda el valor del prefijo encontrado

        sData = SplitData( sData, Wrd );                              // Separa información de tipo del resto de los datos
        Wrd.LengDictSimple( sData );                                  // Ejecuta comandos para resto de los datos y modifica la llave

        if( FindInDic(Wrd, false) ) return true;                      // Si la llave resultante existe en el diccionario, termina OK
        if( FindIrrgVerb( Wrd )   ) return true;                      // Si la llave resultante es un verbo irregular, termina OK
        if( FindKeySufijo( Wrd )  ) return true;                      // Si la llave resultante tiene un sifijo, termina OK

        if( !reduce ) continue;                                       // Si no viene de reducción continua buscando
        if( Wrd.wCase!=WCaso.Upper || Wrd.Abreviatura )               // Si la palabra es mayusculas ó abreviatura
          {
          Wrd.Trad = Wrd.Key;                                         // Pone traducción igual a la llave
          if( FindTradSufijo(Wrd) )                                   // Si encuentra sufijo de traducción
            {   
            FindTradPrefijo(Wrd);                                     // Busca prefijo de traducción

            Wrd.Trad = "";                                            // Borra la traducción colocada anteriormente
            return true;                                              // Retorna OK
            }
          }   
        }                                                             // Intenta buscar otro prefijo

      Wrd.Key = key;                                                  // Restaura todos los valores modificados
      Wrd.sPrefijo = "";
      Wrd.sKeyPrefijo = "";
      Wrd.sTipo = "";
      Wrd.Data = null;

      return false;                                                   // Retorna que no encontró prefijo
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    public void FindVerboSeparableDe()
      {
      for( int i=Ini; i<=End; ++i )
        {
        var Wrd = Words[i];

        string sParticulas = ",ab,acht,an,aneinander,auf,aufeinander,aufrecht,aufwärts,aus,auseinander,bei,beieinander,beisammen,bekannt,bereit,besser,bestehen,bevor,blau,bleiben,bloß,bloss,brach,breit,da,dabei,dagegen,daher,dahin,dahinter,dar,daran,darunter,darüber,davon,dazu,dazwischen,dicht,dran,drauflos,drein,durch,durcheinander,ein,eis,empor,entgegen,entlang,fallen,fehl,fern,fertig,fest,flach,flott,flöten,fort,frei,fremd,gefangen,gegen,gegenüber,geheim,gehen,gerade,gering,glatt,gleich,groß,gut,haften,halt,hand,haus,heilig,heim,heimlich,heiß,hell,her,herab,heran,herauf,heraus,herbei,herein,herum,herunter,hervor,herüber,hier,hierher,hin,hinauf,hinaus,hindurch,hinein,hinter,hintereinander,hinterher,hinunter,hinweg,hinzu,hinüber,hoch,hof,hohn,hops,hängen,höher,ineinander,inne,irre,kahl,kalt,kaputt,kehrt,kennen,klar,kleben,klein,knapp,kopf,krank,krumm,kund,kurz,kürzer,lahm,lang,leicht,lieb,liegen,locker,los,mal,maschine,maß,mit,nach,nackt,nahe,nieder,not,näher,ob,offen,preis,rad,ran,raus,rein,richtig,ruhig,rund,rüber,rück,sauber,sausen,scharf,schief,schlapp,schlecht,schwarz,schwer,schön,seil,sein,selig,sicher,sitzen,spazieren,spitz,stand,statt,staub,stecken,stehen,steif,stil,still,stramm,teil,tief,tot,trocken,um,umher,umhin,unter,untereinander,urauf,verloren,verschütt,vol,voll,vor,voran,voraus,vorbei,vorher,vorweg,vorwärts,vorüber,wach,wahr,warm,weg,weh,weis,weiter,weiß,wett,wider,wieder,wohl,wund,zu,zufrieden,zurecht,zurück,zusammen,zuvor,zuwider,übel,über,überein,übereinander,überhand,übrig,";
        string sFinalType  = ",XX,CC,GS,GZ,GA,";
        string sKey        = ',' + Wrd.Key + ',';

        int NextNode = i+1;
        
        Word NxtWord = (NextNode<=End)? Words[NextNode] : null ;

        if ( NxtWord==null || !sParticulas.Contains(sKey) || !sFinalType.Contains( NxtWord.sTipo ) || Wrd.Delete )
          continue;

        string sNoType = ",SS,DD,RD,RI,AI,AA,FP,FT,FL,FI,FS,NP,XX,PP,XM,XJ,AO,OP,VP,PT,PI,AE,AF,AP,GP,GD,GG";

        for( int j=i-1; j>=0; )
          {
          var posWord = Words[j];

          if( sNoType.Contains( posWord.sTipo) )
              continue;

          string sTemp = posWord.Key;
          posWord.Key = Wrd.Key + posWord.Key;

          bool bFound = FindInDic( posWord );
          if( bFound == false)
            posWord.Key = sTemp;
          else
            {
            Wrd.Delete = true;
            Wrd.sTipo = "XX";
            break;
            }
          }
        }
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE FindWordsInDict  +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE                +++++++++++++++++++++++++++++++++++++++++++++
