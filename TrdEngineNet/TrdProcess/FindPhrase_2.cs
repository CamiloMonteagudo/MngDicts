using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Busca las frases con comodines o sin ellos en la oración</summary>
  public class FindPhrase
    {
    protected Translate Trd;

    string TiposPermitidos;            // Arreglo con los tipos gramaticales permitidos a estar en los comodines de [] de las frases idiomaticas
    string CategoriasPermitidas;

    List<string> TiposArray    = new List<string>();      // Arreglo con los tipos de las palabras
    List<int>    iTiposArray   = new List<int>();         // Arreglo con la posición de los tipos de las palabras
    List<string> CategoryArray = new List<string>();      // Arreglo con las categorias de las palabras
    List<int>    iCategoryArray= new List<int>();         // Arreglo con la posición de las categorias de las palabras
                    // este arreglo debe estar en los datos de la dirección de traducción ya que para cada idioma pueden existir tipos diferentes
                    // por ejemplo para el ingles XY (Adj o Adv) puede estar aqui al igual AA ya que no se concuerdan los Adj

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
    ///<summary></summary>
    protected void Process()
      {
      if( Dict==null ) return;

      var Wrds = Trd.Ora.Words;
      for( int i=0; i<Wrds.Count; i++ )
        {
        Word nowWord = Wrds[i];

        string sLastPhrase ="";                                   // Última frase formada para buscar en el diccionario
        string sNewPhrase  ="";                                   // Nueva frase formada para buscarla en el diccionario
        int EndPhrase      = -1;                                  // nodo posible fin de frase 

        // Llena el arreglo con las partes de la palabra que se desea buscar en el diccionario
        ResetCounter();
        var FraseOld = FillWordsToFindArray( nowWord );

        for( int j=0; j<FraseOld.Count; j++ )
          {
          string FoundFrase = "";

          sNewPhrase = FraseOld[j];
          if( string.IsNullOrEmpty(sNewPhrase) || sNewPhrase==sLastPhrase )
              continue;

          sLastPhrase = sNewPhrase;
          for( int k=i+1;  k<Wrds.Count; ++k )
            {   
            string LastFrase = "";
            bool  entrefrase = false;

            nowWord = Wrds[k];

            var FraseOld1 = FillWordsToFindArray( nowWord );

            for( int kk=0; kk<FraseOld1.Count; kk++ )
              {
              sNewPhrase = FraseOld[j];
              sNewPhrase += ' ';
              sNewPhrase += FraseOld1[kk];

              if( sNewPhrase == LastFrase ) continue;
              if( !Dict.IsKey(sNewPhrase) ) continue;

              entrefrase       = true;
              EndPhrase        = k;
              FraseOld[j]      = sNewPhrase;
              FoundFrase = sNewPhrase;
              break;
              }

            if( !entrefrase ) break;
            }

          if( FoundFrase.Length==0 ) continue;

          IfFoundPhrase( FoundFrase, i, EndPhrase );
          break;
          }
        }

      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    private void ResetCounter()
      {
      TiposArray.Clear();
      iTiposArray.Clear();
      CategoryArray.Clear();
      iCategoryArray.Clear();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    private void AddItemToFindArray( List<string> aWordsToFind, string sTipo, string sPermitido, List<string> strArray, List<int> wArray, string strOpenBrace, string strCloseBrace )
      {
      if(  string.IsNullOrEmpty(sTipo) ) return;
      if( !sPermitido.Contains(sTipo)  ) return;

      int k = -1;
      for( int i=0; i<strArray.Count; i++ )
        if( strArray[i] == sTipo )
          {
          k = wArray[i];
          wArray[i] = (++k);
          }

      if( k==-1 )
        {
        k = 1;
        strArray.Add( sTipo );
        wArray.Add( k );
        }

      var tmp = strOpenBrace + sTipo + k + strCloseBrace; 
      aWordsToFind.Add( tmp );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Llena una lsita con las posibles variantes de la palabra a buscar en el diccionario</summary>
    protected virtual List<string> FillWordsToFindArray( Word nowWord )
      {
      var WrdsToFind = nowWord.FillWordsToFind(); 

      AddItemToFindArray( WrdsToFind, nowWord.sTipo, TiposPermitidos, TiposArray, iTiposArray, "[","]" );
      if( !string.IsNullOrEmpty(nowWord.sSemantica) )
        {
        var lstCats = nowWord.sSemantica.Split( ';',',','|' );
        for( int i=0; i<lstCats.Length; ++i )
          {
          AddItemToFindArray( WrdsToFind, lstCats[i], CategoriasPermitidas, CategoryArray, iCategoryArray, "<", ">" );
          }
        }

      return WrdsToFind;
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
      Modo    modo     = Modo.Infinitivo;
      Tiempo  tiempo   = Tiempo.Presente;
      Persona persona  = Persona.Primera;
      WCaso   caso     = WCaso.Lower;

      string  sTypeFirstWord="";
      string  sRtfFijoIni="";
      string  sRtfFijoFin="";
      string  sStrIniBlank="";

      for( int ptrTmpNode = IniNode; ptrTmpNode<=EndNode; ++ptrTmpNode )
        {
        iniWord = Words[ptrTmpNode];                        // Palabra que se analiza
        if( !plural ) plural = iniWord.Plural;

        if( string.IsNullOrEmpty(sTypeFirstWord) && (iniWord.sTipo == "VC" || iniWord.sTipo == "VP" || iniWord.sTipo == "JK" || iniWord.sTipo == "JL" || iniWord.sTipo == "VD" || iniWord.sTipo == "JT" || iniWord.sTipo == "JI") )
          sTypeFirstWord = iniWord.sTipo;

        if( ptrTmpNode-IniNode < 3 && !pasado )
          pasado = ( iniWord.wTiempo == Tiempo.Pasado );

        if( !PuseModo )
          {
          if( modo==Modo.Infinitivo && iniWord.wModo != Modo.Infinitivo )  modo = iniWord.wModo;

          if( iniWord.sTipo == "VT" || iniWord.sTipo == "VI" || iniWord.sTipo == "BE" || iniWord.sTipo == "VA" || iniWord.sTipo == "HA")
            PuseModo = true;
          }

        if( !string.IsNullOrEmpty( iniWord.RtfFijoIni ) )
          sRtfFijoIni += iniWord.RtfFijoIni;

        if( !string.IsNullOrEmpty( iniWord.RtfFijoFin ) )
          sRtfFijoFin += iniWord.RtfFijoFin;

        if( !string.IsNullOrEmpty( iniWord.StrIniBlank ) &&  string.IsNullOrEmpty( sStrIniBlank ) )
          sStrIniBlank += iniWord.StrIniBlank;

        if( tiempo ==Tiempo.Presente && iniWord.wTiempo  != Tiempo.Presente ) tiempo  = iniWord.wTiempo;
        if( persona==Persona.Tercera && iniWord.wPersona != Persona.Tercera ) persona = iniWord.wPersona;

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
      newWod.wTiempo     = (pasado)? Tiempo.Pasado : newWod.wTiempo;
      newWod.wModo       = modo;
      newWod.wPersona    = persona;
      newWod.wCase       = caso;
      newWod.RtfFijoFin  = sRtfFijoFin;
      newWod.RtfFijoIni  = sRtfFijoIni;
      newWod.StrIniBlank = sStrIniBlank;

      MakePhrase(newWod, IniNode, EndNode);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una frase idiomática con lo que existe en la oración desde ini hasta fin</summary>
    protected void MakePhrase( Word ptrWord, int ini, int fin )
      {                   
      int entreque = 0;
      string entrep = "";

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

        if( !string.IsNullOrEmpty(tmp.EntreParentesis) )
          {
          entreque = tmp.EntreQue;
          entrep   = tmp.EntreParentesis;
          }

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
      old.EntreParentesis  = entrep;
      old.EntreQue         = entreque;
      old.RtfFijoFin       = ptrWord.RtfFijoFin;
      old.RtfFijoIni       = ptrWord.RtfFijoIni;
      old.StrIniBlank      = ptrWord.StrIniBlank;
      old.sTipo            = ptrWord.sTipo;

      old.AcepArray.Clear();
      }                   /* MakeFrase */

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un arreglo con los comodines que existen en la frase</summary>
    private void GetComodinList( string ptrFrase, List<string> CmdsPresentes, string sOpenBrace, string sCloseBrace )
      {
      string str = ptrFrase;
      
      int ini=0;
      for( int i=0; i<str.Length; )
        {
        i = str.LastIndexOf( sOpenBrace, ini );                // busca la frase [DD1]
        if( i == -1)   break;

        if( i+4 >= str.Length ) break;

        if( str[i+4] == sCloseBrace[0] )
          {
          var temp = str.Substring(i,5);
          CmdsPresentes.Add(temp);

          ini = i+5;
          }
        else
          ++ini;
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
        var npos = str.Substring(3);

        int ipos;
        int.TryParse( npos, out ipos );

        int  pos = 1;

        var Words = Trd.Ora.Words;
        for( int nodo = ini; nodo<Words.Count && nodo<fin ; ++nodo )
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
              Trd.ExecLengDict( nodo, Words, true );
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
        for( int nodo = ini; nodo<Words.Count && nodo<fin ; ++nodo )
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
              Trd.ExecLengDict( nodo, Words, true );
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

  ///<summary>Busca las frases con comodines o sin ellos en la oración</summary>
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
    ///<summary>Llena una lista con las posibles variantes de la palabra a buscar en el diccionario</summary>
    protected override List<string> FillWordsToFindArray( Word nowWord )
      {
      var Words = new List<string>(1);
      Words.Add( nowWord.Orig );
      return Words;
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
