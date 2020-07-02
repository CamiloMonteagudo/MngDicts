using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;
using TrdEngine.Dictionary;

namespace TrdEngine.TrdProcess
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Busca las frases con comodines o sin ellos en la oración</summary>
  public class FindPhrase
    {
    protected Translate Trd;

    string TiposPermitidos;            // Arreglo con los tipos gramaticales permitidos a estar en los comodines de [] de las frases idiomaticas
    string CategoriasPermitidas;

    IDictTrd Dict = null;
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    protected FindPhrase( Translate Trd )
      {
      this.Trd = Trd;

      TiposPermitidos      = Trd.LangDes.FindStrData( "TIPOSENCOMODIN" );
      CategoriasPermitidas = Trd.LangDes.FindStrData( "SEMANTICAENCOMODIN" );

      Dict = GetDict();
      }

    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el diccionario donde buscar la frase (depende de la calse derivada)</summary>
    protected virtual IDictTrd GetDict()
      {
      return Trd.GetDict( DDirId.Gen ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Recorre la oración actual en busca de frases idiomaticas (Pueden tener comodines)</summary>
    public static void Process( Translate Trd )
      {
      var FPhrases = new FindPhrase(Trd);
      FPhrases.Process();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca todas la frases que haya en la oración</summary>
    protected void Process()
      {
      if( Dict==null ) return;
      Trd.Ora.PoneXX();                                               // Pone XX al inicio y final de la oración

      List<List<string>> Words = GetWordsAndVariants();               // Busca todas las palabras de la oración y sus varientes

      var Finder = new MatchFrase( Words, Dict.GetSortedKeys() );     // Inicializa el objeto para buscar frases

      var nDelWrd = 0;                                                // Tiene en cuenta las palabras borradas durante el proceso
      for( int i=0; i<Words.Count; i++ )
        {
        NumeraComodin( Words, i );                                    // Pone número a los comodines de relativo a la primera palabra de la frase

        var sFra = Finder.FindAt(i);                                  // Busaca si a partir de la palabra i hay una frase
        if( sFra != null )                                            // Si encontró una frase
          {
          var fin = Finder.WordEnd;                                   // Obtiene palabra final de la frase
          IfFoundPhrase( sFra, i-nDelWrd, fin-nDelWrd );              // Crea la frase y borra palabras de la oración

          nDelWrd += (fin-i);                                         // Calcula y acumula el número de palabras borradas
          i=fin;                                                      // Ajusta la palabra donde debe continar la busqueda
          }                                
        }

      Trd.Ora.QuitaXX();                                              // Quita XX del inicio y final de la oración
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un listado de todas las palabras de la oración y sus posibles variantes para formar frases</summary>
    protected virtual List<List<string>> GetWordsAndVariants()
      {
      PosComodines = new List<Tuple<int,int,int,string>>();
      var Words = new List<List<string>>();

      var Wrds = Trd.Ora.Words;
      for(int i=0; i<Wrds.Count; ++i)
        {
        var Wrd = Wrds[i];
        var Vars = new List<string>();
        Words.Add( Vars );

        Vars.Add( Wrd.Origlwr );

        if( Wrd.Posesivo )
          {
          string sEnd = "";
          if( Wrd.wDiWord == DiWord.POSESIVO_CS ) sEnd = "'s";
          if( Wrd.wDiWord == DiWord.POSESIVO_SC ) sEnd = "'" ;

          Vars.Add( Wrd.Orig + sEnd );

          if( Wrd.Key.Length>0 && Wrd.Key != Wrd.Origlwr && Wrd.Key != Wrd.Orig ) 
            Vars.Add( Wrd.Key + sEnd );
          }

        if( Wrd.Key.Length>0 && Wrd.Key != Wrd.Origlwr && Wrd.Key != Wrd.Orig ) 
          Vars.Add( Wrd.Key );

        FindComodin( Words, Wrd.sTipo, TiposPermitidos, 1 );

        if( !string.IsNullOrEmpty(Wrd.sSemantica) )
          {
          var lstCats = Wrd.sSemantica.Split( ';',',','|' );
          foreach( var Cat in lstCats)
            FindComodin( Words, Cat, CategoriasPermitidas, 2 );
          }
        }

      return Words;
      }

    //------------------------------------------------------------------------------------------------------------------
    List< Tuple<int,int,int,string> > PosComodines;   // Lista de todos los comodines y sus datos
    // Item1 => iWord Indice a la palabra dentro del arreglo de palabras
    // Item2 => iVar  Indice a la variante de la palabra dentro de la palabra iWord
    // Item3 => Tipo de comodin 1- Entre [], 2- Entre <>
    // Item4 => Texto del comodin
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca si la palabra con el tipo sTipo se puede sustituir por un comodin</summary>
    private void FindComodin( List<List<string>> Words, string sTipo, string sPermitido, int Tipo )
      {
      if(  string.IsNullOrEmpty(sTipo) ) return;
      if( !sPermitido.Contains(sTipo)  ) return;

      var iWord = Words.Count-1;
      var iVar  = Words[iWord].Count;

      Words[iWord].Add(sTipo);

      PosComodines.Add( Tuple.Create( iWord, iVar, Tipo, sTipo ) ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el número a los comodines a partir la posición 'iWrd'</summary>
    private void NumeraComodin( List<List<string>> Words, int iWrd  )
      {
      if( PosComodines==null ) return;                                // Si  no hay comodines, termina

      var LastNum = new Dictionary<string,int>();                     // Diccionario con texto del comodin y al ultimo número utilizado

      foreach( var pos in PosComodines)                               // Recorre todos los comodines
        {
        if( pos.Item1 < iWrd ) continue;                              // Si la palabra es anterior a la actual, no se tiene en cuenta
        
        int Num = 1;                                                  // Pone el número usado por defecto
        var TKey = pos.Item3 + pos.Item4;                             // Forma cadena para identificar el comodin
        if( LastNum.ContainsKey(TKey) )                               // Si ya se utilizo ese comodin
          Num = LastNum[TKey] + 1;                                    // Incrementa el ultimo número usado

        LastNum[TKey] = Num;                                          // Guarda ultimo número usado

        var sVal = pos.Item4 + Num;                                   // Crea texto del comodin con su número
        sVal = (pos.Item3==1)? '[' + sVal + ']': '<' + sVal + '>';    // Encierra el comodin según el tipo

        Words[pos.Item1][pos.Item2] = sVal;                           // Actualiza texto del comodin con el numero adecuado
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa información de la frase es encontrada en el diccionario</summary>
    protected virtual void IfFoundPhrase( string sFrase, int IniNode, int EndNode  ) 
      {
      var Words = Trd.Ora.Words;
      var Data  = Dict.GetWordData(sFrase);

      Word newWod = new Word();
      newWod.Key  = sFrase;
      newWod.Data = Data;

      if( Data != null ) 
        newWod.sTipo = Data.CompType;

      Word iniWord    = Words[IniNode];

      bool    plural   = false;
      bool    pasado   = false;
      bool    PuseModo = false;
      TMod    modo     = TMod.Infinitivo;
      TTime  tiempo   = TTime.Presente;
      TPer persona  = TPer.Primera;
      WCaso   caso     = WCaso.Lower;

      string  sTypeFirstWord="";
      string  sRtfFijoIni="";
      string  sRtfFijoFin="";

      for( int ptrTmpNode = IniNode; ptrTmpNode<=EndNode; ++ptrTmpNode )
        {
        iniWord = Words[ptrTmpNode];                        // Palabra que se analiza
        if( !plural ) plural = iniWord.Plural;

        if( string.IsNullOrEmpty(sTypeFirstWord) && (iniWord.sTipo == "VC" || iniWord.sTipo == "VP" || iniWord.sTipo == "JK" || iniWord.sTipo == "JL" || iniWord.sTipo == "VD" || iniWord.sTipo == "JT" || iniWord.sTipo == "JI") )
          sTypeFirstWord = iniWord.sTipo;

        if( ptrTmpNode-IniNode < 3 && !pasado )
          pasado = ( iniWord.wTiempo == TTime.Pasado );

        if( !PuseModo )
          {
          if( modo==TMod.Infinitivo && iniWord.wModo != TMod.Infinitivo )  modo = iniWord.wModo;

          if( iniWord.sTipo == "VT" || iniWord.sTipo == "VI" || iniWord.sTipo == "BE" || iniWord.sTipo == "VA" || iniWord.sTipo == "HA")
            PuseModo = true;
          }

        if( !string.IsNullOrEmpty( iniWord.RtfFijoIni ) )
          sRtfFijoIni += iniWord.RtfFijoIni;

        if( !string.IsNullOrEmpty( iniWord.RtfFijoFin ) )
          sRtfFijoFin += iniWord.RtfFijoFin;

        if( tiempo ==TTime.Presente && iniWord.wTiempo  != TTime.Presente ) tiempo  = iniWord.wTiempo;
        if( persona==TPer.Tercera && iniWord.wPersona != TPer.Tercera ) persona = iniWord.wPersona;

        if( ptrTmpNode == IniNode ) caso = iniWord.wCase;
        }

      if( !string.IsNullOrEmpty(sTypeFirstWord) && (newWod.sTipo == "VV" || newWod.sTipo == "VT" || newWod.sTipo == "VI") )
          newWod.sTipo = sTypeFirstWord;

      if( !string.IsNullOrEmpty(sTypeFirstWord) && (newWod.sTipo == "VA") )
        {
        newWod.sTipo = "VA";
        pasado = true;
        }

      if( (iniWord.sTipo=="GW" || iniWord.sTipo=="GV") &&  (newWod.sTipo=="SS" || newWod.sTipo=="NP") )
        newWod.sTipo = iniWord.sTipo;

      newWod.Plural      = plural;
      newWod.wTiempo     = tiempo;
      newWod.wTiempo     = (pasado)? TTime.Pasado : newWod.wTiempo;
      newWod.wModo       = modo;
      newWod.wPersona    = persona;
      newWod.wCase       = caso;
      newWod.RtfFijoFin  = sRtfFijoFin;
      newWod.RtfFijoIni  = sRtfFijoIni;

      MakePhrase(newWod, IniNode, EndNode);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una frase idiomática con lo que existe en la oración desde ini hasta fin</summary>
    protected void MakePhrase( Word ptrWord, int ini, int fin )
      {                   
      var strFrase = ptrWord.Key;

      var ComodinesTPresentes = new List<string>();    // nos da los comodines presentes en la frase [DD1], [DD3], [NP5]
      var ComodinTTranslation = new List<string>();
      var ComodinesCPresentes = new List<string>();    // nos da los comodines presentes en la frase <DD1>, <DD3>, <NP5>
      var ComodinCTranslation = new List<string>();

      if( strFrase.Contains(']') )
        {
        GetComodinList( strFrase, ComodinesTPresentes, "[","]" ) ;
        FillComodinTranslation( ComodinTTranslation, ComodinesTPresentes, ini, fin );
        }

      if( strFrase.Contains('<') )
        {
        GetComodinList( strFrase, ComodinesCPresentes, "<",">" );
        FillCComodinTranslation( ComodinCTranslation, ComodinesCPresentes, ini, fin );
        }

      var Words = Trd.Ora.Words;
      var Orig  = Words[ini].Origlwr;

      for( int i=ini+1; i<=fin; ++i )
        {
        Word tmp = Words[i];

        if( !string.IsNullOrEmpty(Orig) )
          Orig += " ";

        Orig += tmp.Origlwr;
        }
      
      Words.RemoveRange( ini+1, fin-ini );                            // Borra las palabras entre ini y fin, sin incluir la primera palabra

      for( int ic=0; ic<ComodinesTPresentes.Count && ic<ComodinTTranslation.Count; ic++ )
        {
        ptrWord.Key  = ptrWord.Key.Replace( ComodinesTPresentes[ic], ComodinTTranslation[ic] );
        ptrWord.Data = ptrWord.Data.Replace( ComodinesTPresentes[ic], ComodinTTranslation[ic] );
        ptrWord.Buscada = true;
        }

      for( int ic=0; ic<ComodinesCPresentes.Count && ic<ComodinCTranslation.Count; ic++ )
        {
        ptrWord.Key  = ptrWord.Key.Replace( ComodinesCPresentes[ic], ComodinCTranslation[ic] );
        ptrWord.Data = ptrWord.Data.Replace( ComodinesCPresentes[ic], ComodinCTranslation[ic] );
        ptrWord.Buscada = true;
        }

      Word old = Words[ini];
      old.Key              = ptrWord.Key; 
      old.Orig             = ptrWord.Key;
      old.Origlwr          = ptrWord.Key;
      old.Data             = ptrWord.Data;
      old.wGenero          = ptrWord.wGenero;
      old.Trad             = ptrWord.Trad;
      old.Traducida        = ptrWord.Traducida;
      old.Buscada          = ptrWord.Buscada = true;
      old.wModo            = ptrWord.wModo;
      old.Plural           = ptrWord.Plural;
      old.wTiempo          = ptrWord.wTiempo;
      old.Femenino         = ptrWord.Femenino;
      old.wCase            = ptrWord.wCase;
      old.Traducida        = ( !string.IsNullOrEmpty(ptrWord.Trad) && ptrWord.Traducida )? ptrWord.Traducida: false;  // caso de las frases en el diccionario de nombres p. del texto
      old.NoBusca          = false;
      old.RtfFijoFin       = ptrWord.RtfFijoFin;
      old.RtfFijoIni       = ptrWord.RtfFijoIni;
      old.sTipo            = ptrWord.sTipo;

      old.AcepArray = new List<WordMean>();
      }                   /* MakeFrase */

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un arreglo con los comodines que existen en la frase</summary>
    private void GetComodinList( string ptrFrase, List<string> CmdsPresentes, string sOpenBrace, string sCloseBrace )
      {
      string str = ptrFrase;
      
      for( int i=0; i<str.Length; )
        {
        i = str.IndexOf( sOpenBrace, i );                   // busca la frase [DD1]
        if( i == -1)   break;

        if( i+4 >= str.Length ) break;

        if( str[i+4] == sCloseBrace[0] )
          {
          var temp = str.Substring(i,5);
          CmdsPresentes.Add(temp);

          i += 5;
          }
        else
          ++i;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un arreglo con los comodines que existen en la frase</summary>
    private void FillComodinTranslation( List<string> ComodinesTranslation, List<string> ComodinesPresentes, int ini, int fin )
      {
      for( int i=0; i<ComodinesPresentes.Count; i++ )
        {
        var str = ComodinesPresentes[i];

        var tipo = str.Substring(1,2).ToUpper();
        var npos = str.Substring(3,1);

        int ipos;
        int.TryParse( npos, out ipos );

        int  pos = 1;

        var Words = Trd.Ora.Words;
        for( int nodo = ini; nodo<Words.Count && nodo<=fin ; ++nodo )
          {
          Word ptrWord = Words[nodo];

          if( ptrWord.sTipo != tipo ) continue;
          if( pos != ipos           ) { pos++; continue; }

          string Translation;
          if( tipo == "NN" || tipo == "NU" )
            {
            Translation = ptrWord.Trad;
            }
          else
            {
            if( ptrWord.Traducida || (ptrWord.Data==null && !string.IsNullOrEmpty(ptrWord.Trad)) )
              {
              Translation = ptrWord.Trad;
              }
            else if( ptrWord.Data!=null )
              {
              Trd.ExecLengDict( nodo, Words );
              Translation = ptrWord.Trad;
              }
            else
              {
              Translation = ptrWord.Orig;
              }
            }

          ptrWord.Traducida = true;
          ComodinesTranslation.Add( Translation );
          break;
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un arreglo con los comodines que existen en la frase</summary>
    private void FillCComodinTranslation( List<string> ComodinesTranslation, List<string> ComodinesPresentes, int ini, int fin )
      {
      for( int i=0; i<ComodinesPresentes.Count; i++ )
        {
        var str = ComodinesPresentes[i];

        var tipo = str.Substring(1,2).ToUpper();
        var npos = str.Substring(3);

        int ipos;
        int.TryParse( npos, out ipos );

        int pos = 1;

        var Words = Trd.Ora.Words;
        for( int nodo = ini; nodo<Words.Count && nodo<=fin ; ++nodo )
          {
          Word ptrWord = Words[nodo];

          if( !ptrWord.sSemantica.Contains(tipo) ) continue;
          if( pos != ipos                        ) {pos++; continue;}

          string Translation;
          if( tipo=="LL" || tipo=="LU" )
            {
            Translation = ptrWord.Orig;
            }
          else
            {
            if( ptrWord.Traducida || ( ptrWord.Data==null && !string.IsNullOrEmpty(ptrWord.Trad)) )
              {
              Translation = ptrWord.Trad;
              }
            else if( ptrWord.Data!=null )
              {
              Trd.ExecLengDict( nodo, Words );
              Translation = ptrWord.Trad;
              }
            else
              {
              Translation = ptrWord.Orig;
              }
            }

          ptrWord.Traducida = true;

          ComodinesTranslation.Add( Translation );
          break;
          }
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de FindBracketPhrase ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary>Implementa algoritmo para casar una frase entre una lista de palabras y un diccionario</summary>
  public class MatchFrase
    {
    List<List<string>> Words;       // Lista de palabras de la oración y sus posibles variaciones
    List<string>       SortKeys;    // Lista de palabras del diccionario

    int End  = 0;                   // Indice a la última palabra del diccionario donde buscar la frase
    public int WordEnd = 0;         // Indice a la ultima palabra de la lista de palabras analizada

    static StringComparer   StrComp = StringComparer.OrdinalIgnoreCase;       // Comparador para las cadenas
    static StringComparison CompOpt = StringComparison.OrdinalIgnoreCase;     // Opción para comparar cadenas
    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicializa lista de palabras y lista de llaves del diccionario</summary>
    public MatchFrase( List<List<string>> Words, List<string> SortKeys )
      {
      this.Words    = Words;
      this.SortKeys = SortKeys;
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca una frase a partir de la palabra 'i' </summary>
    public string FindAt( int i )
      {
      string Found = null;                                            // Inicializa que no se ha encontrado frase
      var Wrds = Words[i];                                            // Obtiene lista de variantes para la palabra i
      for( int j=0; j<Wrds.Count; ++j )                               // Recorre todas las variantes de la primera palabra
        {
        WordEnd = i;                                                  // Inicializa inidice a ultima palabar analizada
        var Wrd = Wrds[j];                                            // Toma la palabra actual

        var idx = SortKeys.BinarySearch( Wrd, StrComp );              // Busca primera palabra en todo el diccionario
        if( idx<0 )                                                   // Si no se encontro la palabra 
          idx = ~idx;                                                 // Comienza la busqueda a partir de la mas proxima
        else                                                          // Si en contro la palabra
          idx = idx+1;                                                // Comienza la busqueda a partir de la siguente

        if( idx>=SortKeys.Count ) break;                              // Si esta al final del diccionario, termina 

        if( FindEnd21(Wrd, idx) == 0 ) continue;                       // Busca cantidad de llaves que comiezan con la palabra actual

        Found = MatchNext( Wrd, i, idx, 0 );                          // Trata de casar las proximas palabras
        if( Found!=null ) break;                                      // Encontro una frase, termina
        }

      return Found;                                                   // Retorna la frase encontrada
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca la primera llave que no empieza con la palabra 'Wrd' a partir de la llave i</summary>
    /// <summary>Argoritmo incrementando el intervalos cada ves mayores a medida que se aleja de la llave actual</summary>
    public int FindEnd21( string Wrd, int i )
      {
      int len = Wrd.Length;
      int ini = i;

      var w = SortKeys[i];
      if( len>=w.Length || w[len]!=' ' ) return 0;                    // Verifica que la primera palabra sea una frase

      int EndOk = i;
      int Delta = 1;

      for(;;)
        {
        End = i + Delta;
        if( End>=SortKeys.Count )                                     // Llego al final del diccionario
          {
          if( EndOk==SortKeys.Count-1 ) { End=EndOk; break;}
          if( Delta<2 )  break;

          Delta  = 1;
          i      = EndOk;
          End    = i + 1;
          }

        w = SortKeys[End];
        if( len<w.Length && w[len]==' ' )                             // Si esta dentro de la zona de las fraces
          {
          EndOk  = End; 
          Delta *= 2;
          }
        else                                                          // Esta mas alla de la ultima frase
          {
          if( Delta<2 )  break;

          Delta = 1;
          i     = EndOk;
          }
        }

      return End - ini;
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Crece la frase en una palabra y dertermina que hacer</summary>
    private string MatchNext( string frase, int iWrd, int Ini, int iVar )
      {
      var i = iWrd+1;                                                 // Toma el indice de la proxima palabra a analizar
      if( i >= Words.Count ) return null;                             // Si llego al final, termina

      var Wrd = Words[i][iVar];                                       // Toma la palabra en la variante dada

      var frase1 = frase + ' ' + Wrd;                                 // Forma la frase
      string frase2 = "";

      var idx = SortKeys.BinarySearch( Ini, End-Ini, frase1, StrComp );  // Busca la frase dentro del rango donde puede estar en el diccionario
      if( idx >= 0 )                                                  // Encontró la frase exacta
        {
        WordEnd = i;                                                  // Guarda palabra donde termina la frase
        frase2 = MatchNext( frase1, i, ++idx, 0 );                    // Trata de buscar una frase mas larga
        if( frase2!=null )                                            // Encontro una frase mas larga          
          return frase2;                                              // La retorna  
        else                                                          // No hay frase más larga
          return SortKeys[idx-1];                                     // Retorna la frase encontrada en el diccionario
        }
      else                                                            // No la encontro 
        {
        idx = ~idx;                                                   // Determina indice a la frase más cercana
        if( idx<SortKeys.Count && SortKeys[idx].StartsWith( frase1 + ' ', CompOpt ) )       // Si la más cercana empieza con la frase buscada
          {
          frase2 = MatchNext( frase1, i, idx, 0 );                    // Continua agregando la proxima palabra
          if( frase2 != null ) return frase2;                         // Si encontró la frase la retorna
          }
        
        if( iVar < (Words[i].Count-1) )                               // Si hay mas variantes de la palabra
          return MatchNext( frase, iWrd, Ini, iVar+1 );               // Busca desde el mismo lugar con la variante nueva

        return null;                                                  // No se encontró ninguna frase
        }
      }

    } // ++++++++++++++++++++++++++++++++++++  FIN DE LA CLASE MatchFrase ++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Busca las frases en el diccicionario de nombres propios</summary>
  public class FindPhraseInProperNoun : FindPhrase
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    private FindPhraseInProperNoun( Translate Trd ) : base(Trd)
      {
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Recorre la oración actual en busca de frases idiomaticas (Pueden tener comodines)</summary>
    public static new void Process( Translate Trd )
      {
      var FPhrases = new FindPhraseInProperNoun(Trd);
      FPhrases.Process();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre el diccionario de nombres propios para buscar frases</summary>
    protected override IDictTrd GetDict()
      {
      return Trd.LangSrc.GetDict( DLangId.PNoun ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un listado de todas las palabras de la oración y sus posibles variantes para formar frases</summary>
    protected override List<List<string>> GetWordsAndVariants()
      {
      var Words = new List<List<string>>();                           // Crea lista de palabras vacia

      var Wrds = Trd.Ora.Words;                                       // Obtiene palabras de la oración
      for(int i=0; i<Wrds.Count; ++i)                                 // Recorre todas las palabras
        {
        Words.Add( new List<string>() );                              // Adiciona lista de variantes vacias
        Words[i].Add( Wrds[i].Orig );                                 // Adiciona una sola variante, la palabra original
        }

      return Words;                                                   // Retorna lista de palabras
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa información de la frase es encontrada en el diccionario</summary>
    protected override void IfFoundPhrase( string sFrase, int IniNode, int EndNode  ) 
      {
      var Words = Trd.Ora.Words;

      Word TmpWord = new Word();
      TmpWord.Key       = sFrase;
      TmpWord.Buscada   = true;
      TmpWord.Traducida = true;

      if( Words[IniNode].sTipo != "GW" )
        Words[IniNode].sTipo = "NP";

  		MakePhrase(TmpWord, IniNode, EndNode );
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de FindBracketPhrase ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin de namespace         ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
