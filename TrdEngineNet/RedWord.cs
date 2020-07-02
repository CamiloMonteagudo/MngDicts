using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;

namespace TrdEngine
  {
  public class RedWord
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Datos para la reducción en un idioma determinado</summary>
    RedLangData Data;                                               // Datos de la reducción para el idioma
    ConjVerb    Conj;                                               // Objeto para la conjugación
    IDictTrd    DictGen;                                            // Diccionario general de palabras para el idioma

    public string Palabra;                                          // Palabra a reducir
    public List<RedItem> Reductions = new List<RedItem>();      // Lista con datos de las palabras reducidas

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Esconde el constructor, para que solo pueda obtenerse una instancia a partir de 'GetConj'</summary>
    private RedWord(){}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre la reducción para un idioma dado</summary>
    ///
    ///<param name="src">Idioma de las palabras que de quieren reducir</param>
    ///<param name="des">Idioma destino para el diccionario general, si NA toma primero que encuentre con la fuente 'src'</param>
    ///<param name="path">Camino donde estan los diccionarios de reducción, si este parametro es 'null' o vacio, los 
    ///diccionarios se buscan en el directorio 'Dictionaries' relativo al directorio del ensamblado (TrdEngine.dll)</param>
    ///
    ///<returns>Retorna un objeto para la reducción, si hubo algún problema este objeto es null</returns>
    //------------------------------------------------------------------------------------------------------------------
    public static RedWord GetReduc(TLng src, TLng des=TLng.NA, string path=null)
      {
      if( !string.IsNullOrEmpty(path) )                             // Si se especifico un camino para los diccionarios
        RedWData.DicPath = path;                                    // Lo pone para los datos

      var Red = new RedWord();                                      // Crea un objeto nuevo
      Red.Data = RedWData.GetLang( src );                           // Obtiene datos para el idioma
      if( Red.Data==null ) return null;

      Red.Conj = ConjVerb.GetConj( src, path );                     // Abre la conjugación para el idioma 
      if( Red.Conj == null ) return null;                           // No lo pudo abrir, retorna error

      Red.DictGen = Red.Data.GetDictGen( des );                     // Obtiene el diccionario general
      if( Red.DictGen == null ) return null;                        // No lo pudo abrir, retorna error

      return Red;                                                   // Retorna el objeto creado
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Libera los datos de la reducción actual, para que puedan ser recolectados</summary>
    //------------------------------------------------------------------------------------------------------------------
    public void Release()
      {
      RedWData.ReleaseLang( Data.Lang );                          // Libera los datos para ese idioma
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Reduce una palabra en un idioma idioma dado</summary>
    ///
    ///<param name="Word">Palabra que se quiere reducir</param>
    //------------------------------------------------------------------------------------------------------------------
    public static RedWord Reduce( TLng lang, string word )
      {
      var Red = GetReduc(lang);
      if( Red!=null && Red.Reduce(word) ) 
        return Red;

      return null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el tipo gramatical del una palabra en el diccionario general</summary>
    public string GetWordDataType( string word ) 
      { 
      var WData = DictGen.GetWordData( word );
      if( WData==null ) return null;
      return WData.CompType; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Reduce una palabra en el idioma que se abrio la reducción</summary>
    ///
    ///<param name="Word">Palabra que se quiere reducir</param>
    //------------------------------------------------------------------------------------------------------------------
    public bool Reduce( string word )
      {
      Reductions.Clear();
      Palabra = word;

      bool reducida = false;

      reducida = ReduceSimple();
      if( reducida ) return true;

      // Buscar la pal. en el dicc. de reducción por prefijos
      var palorig = Palabra;
      while( palorig.Length > 0 )
        {
        var prefijo = Data.Find_Preffix( palorig );
        if( prefijo==null  )  break;

        var raiz = word;
        Palabra = raiz;
        reducida = ReduceSimple();

        Palabra = word;
        if( reducida )
          {
          for( int i = 0; i < Reductions.Count; i++ )
            Reductions[i].Prefijo = prefijo;

          return true;
          }

        palorig = prefijo.Substring(0, prefijo.Length - 1);
        }

      if( Data.Lang == TLng.En )   // Si es Inglés
        return false;

      // Tratar de reducir por pronombres enclíticos
      Palabra = word;
      reducida = FindRedEncliticos();

      // Reducir compuestos  
      if( Data.Lang == TLng.De )   //Si es Alemán
        reducida = ReduceCompound();

      return reducida;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool ReduceSimple()
      {
      string tipo;
      bool reducida = false;

      var datatype = GetWordDataType( Palabra );
      if( datatype!=null )
        {
        if( IsSust(datatype) )                 // Si tipo es sustantivo
          {
          reducida = true;

          var redtype = new RedItem();

          redtype.PalReduc = Palabra;
          redtype.Tipo = "SS";

          Reductions.Add(redtype);
          }

        //Si tipo es adjetivo (móvil o inmóvil)
        if( IsAdj(datatype) || IsAdjIn(datatype) || IsProNomb(datatype) || IsArtic(datatype) )
          {
          reducida = true;

          if( (IsAdj(datatype) || IsAdjIn(datatype)) && IsProNomb(datatype) )
            tipo = "AO";
          else
            {
            if( IsAdj(datatype) )     tipo = "AA";
            else
              {
              if( IsAdjIn(datatype) ) tipo = "AI";
              else                    tipo = datatype;
              }
            }

          bool red = false;
          if( Data.Lang != TLng.En )                            // Si el idioma no es Inglés se reduce el adjetivo
            {
            red = FindRedIrreg(tipo );                          // Buscar la pal. en el dicc. de reducción de sust. y adj. irreg.
            if( !red )
              red = FindRedSuffix(tipo );                       // Buscar la pal. en el dicc. de reducción por sufijos de sust. y adj.
            }

          if( !red )
            {
            var redtype = new RedItem();                      // Si no se pudo reducir es un tipo simple

            redtype.PalReduc = Palabra;
            redtype.Tipo = tipo;

            Reductions.Add(redtype);
            }
          }
        else                                                    // Aunque el tipo no sea adjetivo en el dicc. se trata de reducir
          {
          if( !FindRedIrreg("AA") )                             // Buscar la pal. en el dicc. de reducción de adjetivos
            {
            if( FindRedSuffix("AA") )                           // Buscar la pal. en el dicc. de reducción por sufijos de adjetivos
              reducida = true;
            }
          else reducida = true;
          }


        // Si tipo es verbo (auxiliar o no)
        if( IsVerb(datatype) || IsVerbAux(datatype) || IsBeHa(datatype) )
          {
          reducida = true;
          if( IsVerb(datatype) )   tipo = "VV";
          else
            if( IsVerb(datatype) ) tipo = "VA";
            else                   tipo = datatype;

          if( !FindRedIrreg(tipo) )                             // Buscar la pal. en el dicc. de reducción de verbos irreg.
            {
            if( !FindRedSuffix(tipo) )                          // Buscar la pal. en el dicc. de reducción de sufijos verbales
              {
              var redtype = new RedItem();                    // Si no se pudo reducir es un tipo simple

              redtype.PalReduc = Palabra;
              redtype.Tipo = tipo;
              redtype.Modo = (TMod)0;

              Reductions.Add(redtype);
              }
            }
          }
        else                                                    // Aunque el tipo no sea verbo en el dicc. se trata de reducir
          {
          if( !FindRedIrreg("VV") )                             // Buscar la pal. en el dicc. de reducción de verbos irreg.
            {
            if (FindRedSuffix("VV"))                            // Buscar la pal. en el dicc. de reducción de sufijos verbales
              reducida = true;
            }
          else
            reducida = true;
          }
        
        if( IsAdv(datatype) )                                   // Si tipo es adverbio
          {
          reducida = true;
          var redtype = new RedItem();

          redtype.PalReduc = Palabra;
          redtype.Tipo = "DD";

          Reductions.Add(redtype);
          }

        }
      else                                                      // Si no está en el diccionario general
        {
        if( FindRedIrreg("SS") )                                // Buscar la pal. en el dicc. de reducción de sust. y adj. irreg.  
          reducida = true;
        else
          if( FindRedSuffix("SS") )                             // Buscar la pal. en el dicc. de reducción por sufijos de sust. y adj.
            reducida = true;

        if( FindRedIrreg("AA") )                                // Buscar la pal. en el dicc. de reducción de adjetivos
          reducida = true;
        else
          if( FindRedSuffix("AA") )                             // Buscar la pal. en el dicc. de reducción por sufijos de adjetivos
            reducida = true;

        if( FindRedIrreg("VV") )                                // Buscar la pal. en el dicc. de reducción de verbos irreg.
          reducida = true;
        else
          if( FindRedSuffix("VV") )                             // Buscar la pal. en el dicc. de reducción por sufijos verbales
            reducida = true;
        }

      if( reducida && Data.Lang == TLng.En )                    // Dejar una sola reduccion en Pasado
        {
        int Past = 0;
        int i = 0;
        while( i < Reductions.Count )
          {
          if( Reductions[i].Tiempo == TTime.Pasado)
            {
            if (Past > 0)
              Reductions.RemoveAt(i);
            else
              {
              Reductions[i].Persona = TPer.Tercera;
              Reductions[i].Numero  = TNum.Singular;
              Reductions[i].Modo    = TMod.Indicativo;
              Past = 1;
              i++;
              }
            }
          else
            i++;
          }
        }

      if( reducida && Data.Lang == TLng.De )
        {
        int i = 0;
        bool inf = false;
        while( i < Reductions.Count )
          {
          if( (Reductions[i].Modo == TMod.Infinitivo) && IsVerb(Reductions[i].Tipo) )
            {
            inf = true;
            break;
            }
          else
            i++;
          }

        if( inf == true )                                       // Dejar una solo la reduccion en Infinitivo, Particip.
          {
          int ii = 0;
          while( ii < Reductions.Count )
            {
            TMod modo = Reductions[ii].Modo;
            if( modo == TMod.Indicativo || modo == TMod.Imperativo || modo == TMod.Subjuntivo )
              {
              Reductions.RemoveAt(ii);
              }
            else
              ii++;
            }
          }
        }

      return reducida;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool FindRedEncliticos()
      {
      var Encliticos = new string[]{};
      var CIndirecto = new string[]{};
      var CDirecto   = new string[]{};
      var Sufijos     = new string[]{};
      var Cambio     = new string[]{};

      switch( Data.Lang )
        {
        case TLng.Es:
          Encliticos = new string[]{"melos","melas","meles","melo","mela","mele","telos","telas","teles","telo","tela","tele","selos","selas","seles","selo","sela","sele","noslos","noslas","nosles","noslo","nosla","nosle","oslos","oslas","osles","oslo","osla","osle","los","las","lo","la","les","le","se","me","te","nos","os"};
          CIndirecto = new string[]{"me","me","me","me","me","me","te","te","te","te","te","te","se","se","se","se","se","se","nos","nos","nos","nos","nos","nos","os","os","os","os","os","os","","","","","les","le","se","me","te","nos","os"};
          CDirecto   = new string[]{"los","las","les","lo","la","le","los","las","les","lo","la","le","los","las","les","lo","la","le","los","las","les","lo","la","le","los","las","les","lo","la","le","los","las","lo","la","","","","","","",""};
          Sufijos     = new string[]{"amo","emo","imo","a","e","í"};
          Cambio     = new string[]{"amos","emos","imos","ad","ed","id"};
          break;
        case TLng.It:
          Encliticos = new string[]{ "melo","mela","meli","mele","mene","telo","tela","teli","tele","tene","celo","cela","celi","cele","cene","velo","vela","veli","vele","vene","glielo","gliela","glieli","gliele","gliene","selo","sela","seli","sele","sene","lle","mmi","tti","cci","vvi","lo","la","li","le","ne","mi","ti","ci","vi","gli","si"};
          CIndirecto = new string[]{ "mi","mi","mi","mi","mi","ti","ti","ti","ti","ti","ci","ci","ci","ci","ci","vi","vi","vi","vi","vi","gli","gli","gli","gli","gli","si","si","si","si","si","le","mi","ti","ci","vi","","","","","","mi","ti","ci","vi","gli","si"};
          CDirecto   = new string[]{ "lo","la","li","le","ne","lo","la","li","le","ne","lo","la","li","le","ne","lo","la","li","le","ne","lo","la","li","le","ne","lo","la","li","le","ne","","","","","","lo","la","li","le","ne","","","","","",""};
          Sufijos     = new string[]{ "ar","er","ir"};
          Cambio     = new string[]{ "are","ere","ire"};
          break;
        }

      var palabra = Palabra;
      int inipos  = 0;
      int pos     = 0;
      
      do
        {
        pos = FindSufInString( palabra, Encliticos, inipos );
        if( pos == -1 )  return false;

        string temp = palabra + "$";

        int pr = temp.IndexOf(Encliticos[pos]+ "$");

        var raiz = palabra.Substring(0, pr);

        Palabra = raiz;

        if( !ReduceSimple() )
          {
          if( Data.Lang == 0 )
            {
            EliminaAcento(ref raiz);
            Palabra = raiz;
            if( ReduceSimple() ) break;
            }

          int pos1 = FindSufInString( raiz, Sufijos, 0 );             // Buscar caso en que la palabra reducida no está completa

          if( pos1 != -1 )
            {
            string temp1 = raiz + "$";

            int pr1 = temp1.IndexOf(Sufijos[pos1]+ "$");

            string raiz1 = raiz.Substring(0, pr1) + Cambio[pos1];

            Palabra = raiz1;
            if( ReduceSimple() ) break;
            }

          }
        else
          break;

        inipos = pos + 1;
        }
      while( true );

      string typeVerb = "VV;VA;BE;HA";

      int i=0;
      while( i < Reductions.Count )
        {
        if( typeVerb.IndexOf(Reductions[i].Tipo) == -1 )
          Reductions.RemoveAt(i);
        else
          {
          TMod temp = Reductions[i].Modo;
          if( temp == TMod.Infinitivo || temp == TMod.Imperativo || temp == TMod.Gerundio )
            {
            Reductions[i].CDirecto = CDirecto[pos];
            Reductions[i].CIndirecto = CIndirecto[pos];
            i++;
            }
          else
            Reductions.RemoveAt(i);
          }
        }

      return ( Reductions.Count != 0 );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool ReduceCompound()
      {
      string orig = Palabra;
      int idx = orig.Length;
      List<string> Words = new List<string>();
      List<string> ReducWords = new List<string>();
      bool red = false;
      int div;
      bool checkVerb = false;
      string redW, tipo;

      // Analizar si es un verbo separable con el infijo -zu
      int pos = orig.IndexOf("zu");
      if (pos != -1)
        {
        string pref = orig.Substring(0, pos);
        string verb = pref + orig.Substring(pos + 2);

        var data = Conj.FindPreffixVerb( verb );
        if( data!=null )
          {
          pref = "." + pref;
          if (pref == data)
            {
            var redtype = new RedItem();

            redtype.PalReduc = verb;
            redtype.Tipo = "VV";

            Reductions.Add(redtype);
            return true;
            }
          }
        }

      // Analizar si es una palabra compuesta
      while (true)
        {
        var tmpPalabra = Palabra;
        var tmpDatos   = Reductions;

        var ret = FindCompElem( Palabra, orig, idx, out div, out redW, checkVerb, out tipo );

        Palabra     = tmpPalabra;
        Reductions = tmpDatos;
        if( ret )
          {
          if( div >= orig.Length )
            {
            Words.Add(orig);
            ReducWords.Add(redW);
            red = true;
            break;
            }
          else
            {
            Words.Add(orig.Substring(0, div));
            ReducWords.Add(redW);
            orig = orig.Substring(div);
            idx = orig.Length;
            }
          }
        else
          {
          if( Words.Count > 0 )
            {
            int upp = Words.Count - 1;
            string lastStr = Words[upp];
            orig = lastStr + orig;
            idx = lastStr.Length - 1;
            Words.RemoveAt(upp);
            ReducWords.RemoveAt(upp);
            }
          else
            {
            //No se pudo reducir
            //Reducir como verbo insertando -en
            if( checkVerb == false )
              {
              orig = Palabra;
              checkVerb = true;
              idx = orig.Length;
              }
            else
              break;
            }
          }
        }

      string compound;

      if( red )
        {
        compound = ReducWords[0];

        if( ReducWords.Count == 1 || compound == "s" || ReducWords[ReducWords.Count - 1] == "s" )
          return false;

        InitialToUpper(ref compound);
        for( int i = 1; i < ReducWords.Count; i++ )
          {
          string rw = ReducWords[i];
          if (rw.Length > 1)
            {
            InitialToUpper(ref rw);
            compound = compound + "|" + rw;
            }
          }

        var redtype = new RedItem();

        redtype.PalReduc = compound;
        redtype.Tipo = tipo;

        Reductions.Add(redtype);
        }
      else
        return false;

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool FindCompElem( string compuesto, string palabra, int idx, out int div, out string reducWord, bool checkVerb, out string tipo)
      {
      div = 0;
      reducWord = "";
      string prefijo = palabra.Substring(0, idx);
      bool allowCVerb = false;
      bool isNoun = false;

      //Si es una palabra al final y el compuesto comienza con minúscula puede ser un verbo conjugado
      if( palabra.Length == idx && !IsInitialUpper(compuesto) ) 
        allowCVerb = true;

      //Si es una palabra al final y el compuesto comienza con mayúscula tiene que ser un sustantivo
      if( palabra.Length == idx && IsInitialUpper(compuesto) ) 
        isNoun = true;

      tipo = "SS";
      while( prefijo.Length > 0)
        {
        if( prefijo == "s" )
          {
          div = 1;
          reducWord = prefijo;
          return true;
          }

        Reductions = new List<RedItem>();
        Palabra = prefijo;

        if( prefijo.Length>1 )
          {
          string str = prefijo; 
          if(!IsInitialUpper(str))
            {
            //Si es minúscula se lleva a mayúscula el 1er caract
            InitialToUpper(ref str);
            Palabra = str;
            }

          if( ReduceCompElem() )
            {
            tipo = "SS";
            div = str.Length;
            reducWord = str;
            return true;
            }

          if( !isNoun )
            {
            //Se lleva a minúscula el 1er caract
            InitialToLower(ref str);
            Palabra = str;

            if( ReduceCompElem() )
              {
              bool red = true;
              if( allowCVerb==false )
                {
                for( int i=0; i<Reductions.Count; i++)
                  {
                  string tipo1 = Reductions[i].Tipo;
                  //Si tipo es verbo
                  if( IsVerbAux(tipo1) || IsVerb(tipo1) || IsBeHa(tipo1) )
                    {
                    //Si verbo conj <> de verbo en infinitivo
                    if( Reductions[i].PalReduc != str )
                      {
                      red = false;
                      Reductions.Clear(); 
                      }
                    }
                  }
                }

              if( red )
                {
                div = str.Length;
                reducWord = str;
                return true;
                }
              }

            //2da pasada de Chequeo de Verbo sin -en
            if(checkVerb)
              {
              if( str[str.Length-1]=='e')
                str = str + "n";
              else
                str = str + "en";

              Palabra = str;
              if( ReduceCompElem() )
                {
                div = prefijo.Length;
                reducWord = str;
                tipo = Reductions[0].Tipo;
                return true;
                }
              }
            }
          }

        prefijo = prefijo.Substring(0, prefijo.Length-1);
        allowCVerb = false;
        isNoun = false;
        }

      div = 0;

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool ReduceCompElem()
      {
      bool red = false;

      if( ReduceSimple() )
        {
        for( int i = 0; i < Reductions.Count; i++ )
          {
          string tipo = Reductions[i].Tipo;
          if( IsSust(tipo) || IsAdj(tipo) || IsAdjIn(tipo) || IsVerb(tipo) || IsVerbAux(tipo) && IsBeHa(tipo) )
            red = true;
          }
        }

      return red;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool FindRedIrreg( string tipo)
      //tipo --> SS:Sust   AA:Adj   AI:Adj inmóvil  VV: Verbo
      {
      bool red = false;

      if( tipo == "VV" || tipo == "VA" || tipo == "BE" || tipo == "HA" )
        {
        var root = Data.FindIrregVerb( Palabra );
        if( root != null )
          {
          var datatype = GetWordDataType( root );
          if( datatype != null )
            {
            if( IsVerb(datatype) ) 
              red = VerbRedCheck(root, "VV");
            else
              {
              if( IsVerbAux(datatype) ) 
                red = VerbRedCheck(root, "VA");
              else
                if( IsBeHa(datatype) )
                  red = VerbRedCheck(root, datatype);

              }
            }
          }
        }
      else
        {
        var root = Data.FindIrregNoun( Palabra );
        if( root != null )
          {
          var datatype = GetWordDataType( root );
          if( datatype != null )
            {
            //Si tipo es sustantivo
            if( tipo == "SS" )
              {
              if( IsSust(datatype) )
                red = NounRedCheck(root);
              }
            else
              {
              if( (IsAdj(datatype) || IsAdjIn(datatype)) && IsProNomb(datatype) )
                red = AdjRedCheck(root, "AO");
              else
                {
                if( IsAdj(datatype) )
                  red = AdjRedCheck(root, "AA");
                else
                  {
                  if( IsAdj(datatype) )
                    red = AdjRedCheck(root, "AI");
                  else
                    if( IsProNomb(datatype) )
                      red = AdjRedCheck(root, datatype);
                    else
                      if( IsArtic(datatype) )
                        red = AdjRedCheck(root, datatype);
                  }
                }
              }
            }
          }
        }

      return red;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool FindRedSuffix( string tipo )
    //tipo -. 0:Sust y Adj   1: Verbos
      {
      string sufijo, palorig,datasuf,raiz,finraiz,iniraiz;
      int pos;
      bool reducida = false;

      var verb = (tipo=="VV" || tipo=="VA" || tipo=="BE" || tipo=="HA");

      palorig = Palabra;
      string inicial = palorig;

      //bool checkParticip = false;

      for(int j=0; j<2; j++ )
        {
        while( palorig.Length > 0 )
          {
          if( verb )
            sufijo = Data.Find_VerbSuffix( palorig );
          else
            sufijo = Data.Find_NounSuffix( palorig );

          if( sufijo == null )  break;

          if( verb )
            datasuf = Data.FindVerbSuffix( sufijo );
          else
            datasuf = Data.FindNounSuffix( sufijo );

          datasuf += ",";

          sufijo += "$";

          iniraiz = inicial + "$";
          
          pos = iniraiz.IndexOf(sufijo);

          if( pos==-1 ) return false;

          iniraiz = iniraiz.Substring(0, pos);

          sufijo = sufijo.Substring(0, sufijo.Length - 1);

          do
            {
            pos = datasuf.IndexOf(",");
            if(pos!=-1)
              {
              finraiz = datasuf.Substring(0, pos);
              datasuf = datasuf.Substring(pos + 1);
              }
            else
              {
              finraiz = datasuf;
              datasuf = "";
              }

            raiz = iniraiz + finraiz;

            if( raiz.Length == 1 )
              continue;

            var datatype = GetWordDataType( raiz );
            if( datatype != null )
              {
              //Si tipo es sustantivo
              if( tipo == "SS")
                {
                if( IsSust(datatype) )
                  reducida = NounRedCheck(raiz);
                }
              else
                {
                if( tipo == "VV" || tipo == "VA" || tipo == "BE" || tipo == "HA")
                  {
                  if( IsVerb(datatype) )
                    reducida = VerbRedCheck(raiz,"VV");
                  else
                    {
                    if( IsVerbAux(datatype) )
                      reducida = VerbRedCheck(raiz,"VA");
                    else
                      if( IsBeHa(datatype) )
                        reducida = VerbRedCheck(raiz,datatype);
                    }
                  }

                else
                  {
                  if( (IsAdj(datatype) || IsAdjIn(datatype)) && IsProNomb(datatype) )
                    reducida = AdjRedCheck(raiz,"AO");
                  else
                    {
                    if( IsAdj(datatype) )
                      reducida = AdjRedCheck(raiz,"AA");
                    else
                      {
                      if( IsAdj(datatype) )
                        reducida = AdjRedCheck(raiz,"AI");
                      else
                        if( IsProNomb(datatype) )
                          reducida = AdjRedCheck(raiz,datatype);
                        else
                          if( IsArtic(datatype) )
                            reducida = AdjRedCheck(raiz,datatype);
                      }
                    }
                  }
                }

              if( reducida )
                return true;
              }
            }  
          while(datasuf.Length > 0);
          palorig = sufijo.Substring(1);
          }
        //Si en Alemán no se pudo reducir la palabra
        //ver si es un participio con ge-
        if(j==0 && Data.Lang == TLng.De)
          {
          palorig = Palabra;
          int pos1 = palorig.IndexOf("ge");
          
          if(pos1 != -1)
            {
            palorig = palorig.Substring(0, pos1) + palorig.Substring(pos1+2);
            inicial = palorig;
            }
          else
            return false;
          }
        else
          return false;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    int FindSufInString(string s, string[] array, int inipos)
      {
      string temp;
      s += "$";

      for (int i = inipos; i < array.Length; i++)
        {
        temp = array[i] + "$";
        if (s.IndexOf(temp) != -1)
          return i;
        }

      return -1;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    void EliminaAcento(ref string palabra)
      {
      string vocal = "aeiou"; 
      string vocalAcent = "áéíóú";

      StringBuilder sb = new StringBuilder(palabra);
      for (int i = 0; i < sb.Length; i++)
        {
        int pos = vocalAcent.IndexOf(sb[i]);
        if( pos != -1 )
          sb[i] = vocal[pos];
        }

      palabra = sb.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    void InitialToUpper(ref string s)
      {
      s = char.ToUpper(s[0]) + s.Substring(1);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool IsInitialUpper(string s)
      {
      return char.IsUpper(s, 0);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    void InitialToLower(ref string s)
      {
      s = char.ToLower(s[0]) + s.Substring(1);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool VerbRedCheck( string root, string tipo )
      {
      bool reducida = false;

      string verbconj;
      TMod modo;
      TTime tiempo = TTime.Presente;
      TPer persona = TPer.Primera;
      TNum numero = TNum.Singular;

      int cCount = Conj.DataConjCount();
      for( int i = 0; i < cCount; i++ )
        {
        if( Conj.DataConjCompound(i) == 1 )                  // Tiempo Compuesto
          continue;

        modo = Conj.DataConjMood(i);
        if (modo == TMod.Infinitivo || modo == TMod.Gerundio || modo == TMod.Participio)
          continue;

        tiempo = Conj.DataConjTime(i);

        for( persona=TPer.Primera; persona <= TPer.Tercera; persona++ )
          for( numero=TNum.Singular; numero <= TNum.Plural; numero++ )
            {
            verbconj = Conj.Conjugate( root, modo, tiempo, persona, numero, false, false );

            if( CompareStrings(verbconj, Palabra ) )
              {
              // Se agrega la reducción a la lista m_TypeArray
              reducida = true;

              var redtype = new RedItem();

              redtype.PalReduc = root;
              redtype.Tipo = tipo;
              redtype.Modo = modo;
              redtype.Tiempo = tiempo;
              redtype.Persona = (TPer)persona;
              redtype.Numero = (TNum)numero;

              Reductions.Add(redtype);
              }
            }
        }

      List<TMod> OtherMoods = new List<TMod>();
      OtherMoods.Add(TMod.Infinitivo);
      OtherMoods.Add(TMod.Gerundio);
      OtherMoods.Add(TMod.Participio);

      for(int i=0; i < OtherMoods.Count; i++ )
        {
        verbconj = Conj.Conjugate( root, OtherMoods[i], tiempo, persona, numero, false, false );

        if( CompareStrings(verbconj, Palabra) )
          {
          // Se agrega la reducción a la lista m_TypeArray
          
          reducida = true;

          var redtype = new RedItem();

          redtype.PalReduc = root;
          redtype.Tipo = tipo;
          redtype.Modo = OtherMoods[i];

          Reductions.Add(redtype);
          }
        else
          {
          if( OtherMoods[i] == TMod.Participio && (Data.Lang != TLng.En) )
            {
            if( AdjRedCheck(verbconj, tipo) == true)
              {
              reducida = true;

              var redtype = new RedItem();
              int lastIndex;

              lastIndex = Reductions.Count - 1;
              redtype = Reductions[lastIndex];  
              redtype.PalReduc = root;
              redtype.Modo = (TMod)OtherMoods[i];
              Reductions[lastIndex] = redtype;  
              }
            }
          }
        }

      return reducida;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool NounRedCheck(string root)
      {
      string sustconc = "";
      TNum numero;
      TDecl decl;

      if( Data.Lang != TLng.De)
        {
        sustconc = Conj.ConcordWords( root,TGen.Masc, TNum.Singular, TGen.Masc, TNum.Plural, TGrad.Positive );

        if( sustconc == Palabra )
          {
          var redtype = new RedItem();                      // Se agrega la reducción a la lista m_TypeArray

          redtype.PalReduc = root;
          redtype.Tipo = "SS";
          redtype.Numero = TNum.Plural;

          Reductions.Add(redtype);
          return true;
          }
        }
      else
        {
        for( numero=TNum.Singular; numero <= TNum.Plural; numero++ )
          {
          int cwidx=0;
          int cwcount=0;
          do
            {
            if(numero==TNum.Plural)
              {
              sustconc = Conj.ConcordWordsIndex( root, TGen.Masc, TNum.Singular, TGen.Masc, TNum.Plural, TGrad.Positive, out cwcount, cwidx );
              cwidx++;
              }
            else
              sustconc = root;

            for (decl = TDecl.Nominative; decl < TDecl.Vocative; decl++)
              {
              int dnidx=0;
              int dncount=0;
              string sustconcdec;
              do
                {
                sustconcdec = Conj.DeclineNounIndex( sustconc, TGen.Masc, numero, decl, out dncount, dnidx );
                if( sustconcdec == Palabra )
                  {
                  // Se agrega la reducción a la lista m_TypeArray
                  var redtype = new RedItem();

                  redtype.PalReduc = root;
                  redtype.Tipo = "SS";
                  redtype.Numero = numero;

                  Reductions.Add(redtype);
                  return true;
                  }
                dnidx++;
                }
              while(dnidx < dncount);
              }
            }
          while(cwidx < cwcount);
          }
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool AdjRedCheck( string root, string tipo )
      {
      int genero,numero,grado,decl,artic;
      string adjconc;

      if( Data.Lang != TLng.De )
        {
        for( grado=0; grado < 3; grado++ )
          for( genero=0; genero < 2; genero++ )
            for( numero=0; numero < 2; numero++ )
              {
              // Si es Inglés no se concuerda el adj. ni en género ni en número, solo en grado
              if( (Data.Lang == TLng.En) && ( (genero>0) || (numero>0) ) ) 
                continue;

              adjconc = Conj.ConcordWords( root,TGen.Masc, TNum.Singular,(TGen)genero,(TNum)numero,(TGrad)grado );

              if( adjconc == Palabra )
                {
                // Se agrega la reducción a la lista de reducciones
                var redtype = new RedItem();

                redtype.PalReduc = root;
                redtype.Tipo = tipo;
                redtype.Grado = (TGrad)grado;
                redtype.Numero = (TNum)numero;
                redtype.Genero = (TGen)genero;

                Reductions.Add(redtype);
                return true;
                }

              }
        }
      else
        {
        for( grado=0; grado < 3; grado++ )
          for( genero=0; genero < 3; genero++ )
            for( numero=0; numero < 2; numero++ )
              for( decl=1; decl < 5; decl++ )
                for( artic=0; artic < 3; artic++ )
                  {
                  //Concordar solo en grado comp. o sup.
                  if(grado>0)
                    adjconc = Conj.ConcordWords( root, TGen.Masc, TNum.Singular, TGen.Masc, TNum.Singular, (TGrad)grado );
                  else
                    adjconc = root;

                  // Solo declinar al Plural una vez (en Masculino)
                  if( (genero>0) && (numero==1) ) 
                    continue;

                  adjconc = Conj.DeclineAdjective( adjconc, (TGen)genero, (TNum)numero, (TDecl)decl, (TArtic)artic );
                  if( adjconc == Palabra )
                    {
                    // Se agrega la reducción a la lista  de reducciones
                    var redtype = new RedItem();

                    redtype.PalReduc = root;
                    redtype.Tipo = tipo;
                    redtype.Grado = (TGrad)grado;
                    redtype.Numero = (TNum)numero;
                    redtype.Genero = (TGen)genero;

                    Reductions.Add(redtype);
                    return true;
                    }
                  }
        }
      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    bool CompareStrings(string s1, string s2)
      {
      if( s1 == s2 )  return true;

      s1 = s1.Replace("ß", "ss");
      s2 = s2.Replace("ß", "ss");

      return ( s1==s2 );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es verbo be o have</summary>
    public bool IsBeHa( string sTipo )
      {
      const string tipo_BEHA = "BE;HA";
      return tipo_BEHA.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un adverbio</summary>
    public bool IsAdv( string sTipo )
      {
      const string tipo_adverbio = "DD;XQ;UR;XA;QN;XH;QO;QA;XK;QE;XD;XC;XM;XP;XL;XY;QG;XF;QH;QI;QL;QM;QZ";
      return tipo_adverbio.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un articulo</summary>
    public bool IsArtic( string sTipo )
      {
      const string tipo_articulo = "RD;RI";
      return tipo_articulo.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un pronombre</summary>
    public bool IsProNomb( string sTipo )
      {
      const string tipo_pronombre = "OO;OD;OG;OI;OS;OL;OA;AF;MG;MU;AE;AP;AL;AD;AO";
      return tipo_pronombre.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un adjetivo imovil</summary>
    public bool IsAdjIn( string sTipo )
      {
      const string tipo_adj_in = "AI;QN;QO;XY;AO";
      return tipo_adj_in.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un verbo auxiliar</summary>
    public bool IsVerbAux( string sTipo )
      {
      const string tipo_verbo_aux = "VA;QJ;QK";
      return tipo_verbo_aux.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un verbo</summary>
    public bool IsVerb( string sTipo )
      {
      const string tipo_verbo = "VV;VR;VT;VI;VG;VP;VS;VC;PT;PI;VST;VSI;GT;GI;XQ;UR;XA;QN;QF;TA;XB;ZB;UO;QA;SV;YF;XG;ZF;ZJ;ZZ;YG;QE;XD;XC;XI;ZI;XE;ZV;ZH;XP;XL;VV;QG;XF;ZG;UH;UB;UW;QB;QC;QD;UZ;UC;UM;UT;UX;UI;UE;UJ;UK;UD;UN;UL;UF;UG;UY;QH;QI;QJ;QK;QL;QM;HT;HI;JT;JI;LK;DO";
      return tipo_verbo.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un adjetivo</summary>
    public bool IsAdj( string sTipo )
      {
      const string tipo_adj = "AA;XQ;UR;XA;QF;TA;XB;ZB;UO;XH;XJ;QE;XD;XC;XI;ZI;XE;XM;XO;PB;UH;UB;UW;QB;QC;QD;UM;UT;UX;UK;UD;UN;UV;QL;QM;QZ;MV;MU;MG";
      return tipo_adj.IndexOf(sTipo)!=-1 ;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo gramatical 'sTipo' es un sustantivo</summary>
    public bool IsSust( string sTipo )
      {
      const string tipo_sust = "SS;NP;XQ;UR;XA;QN;QF;TA;XB;ZB;UO;XH;QO;XJ;QA;SV;YF;XG;ZF;ZJ;ZZ;YG;XK;UA;QB;QC;QD;UZ;UC;UM;UT;UX;UI;UE;UJ;UL;UF;UG;UV;UY;QJ;QK;QL;QM;QZ;LK;MV";
      return tipo_sust.IndexOf(sTipo)!=-1 ;
      }


    //------------------------------------------------------------------------------------------------------------------
















    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Cantidad de reducciones producidas despues de llamar 'Reduce'</summary>
    ////------------------------------------------------------------------------------------------------------------------
    //public int RedCount()
    //  {
    //  return m_Count;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene la raiz de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public string GetRoot( int idx )
    //  {
    //  string s = RWElementRoot( m_Data, idx );
    //  return (s==null)? "" : s;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene tipo gramatical de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public string GetType( int idx )
    //  {
    //  string s = RWElementType( m_Data, idx );
    //  return (s==null)? "  " : s;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene el modo de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public Modo GetMood( int idx )
    //  {
    //  Modo i = RWElementMood( m_Data, idx );
    //  return (i==Modo.NA)? Modo.Indicativo : i;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene el tiempo de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public Tiempo GetTime( int idx )
    //  {
    //  Tiempo i = RWElementTime(m_Data, idx);
    //  return (i == Tiempo.NA) ? Tiempo.Presente : i;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene la persona de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public Persona GetPerson( int idx )
    //  {
    //  Persona i = RWElementPerson(m_Data, idx);
    //  return (i==Persona.NA)? Persona.Primera : i;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene el numero de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public bool IsSingular( int idx )
    //  {
    //  Numero i = RWElementNumber( m_Data, idx );
    //  return (i==Numero.NA || i==Numero.Singular)? true: false;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    /////<summary>Obtiene el genero de una de las reducciones obtenidas</summary>
    /////<param name="idx">Indice a la reducción que quiere conocer (entre 0 y RedCount)</param>
    ////------------------------------------------------------------------------------------------------------------------
    //public Genero GetGender( int idx )
    //  {
    //  Genero i = RWElementGender(m_Data, idx);
    //  return (i == Genero.NA) ? Genero.Masc : i;
    //  }

    ////------------------------------------------------------------------------------------------------------------------
    //// Obtiene el diccionario general usado para para buscar las palabras, si ya hay un diccionario se usa el que esta
    //// sino se trata de abrir uno según el idioma de traducción
    ////------------------------------------------------------------------------------------------------------------------
    ///*public IntPtr GetDictHandle()
    //  {
    //  if( Dict != null ) return Dict.Handle; 

    //  string name;
    //  switch( (TLng)(int.Parse(sLang)) )
    //    {
    //    case TLng.Es: name = "Es2En.dic"; break;
    //    case TLng.It: name = "It2En.dic"; break;
    //    case TLng.Fr: name = "Fr2En.dic"; break;
    //    case TLng.De: name = "De2It.dic"; break;
    //    default     : name = "En2Es.dic"; break;
    //    }

    //  Dict = new DictList();
    //  if( Dict.Open( DictPath + name, DicModo.File ) )
    //    return Dict.Handle; 

    //  Dict = null;
    //  return (IntPtr)0;
    //  }*/

    ////------------------------------------------------------------------------------------------------------------------

    ////---------------------------------------------------------------------------

    //int RWTypeCount(HandleData handle)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return -1; //
    //      }

    //    return (handle.m_TypeArray.Count);
    //    }
    //  return -1;
    //  }

    ////---------------------------------------------------------------------------

    //Persona RWElementPerson(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return Persona.NA; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return Persona.NA;
    //      }

    //    return handle.m_TypeArray[index].m_Persona;
    //    }
    //  return Persona.NA;
    //  }

    ////---------------------------------------------------------------------------

    //string RWElementRoot(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return null; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return null;
    //      }

    //    return handle.m_TypeArray[index].m_PalReduc;
    //    }

    //  return null;
    //  }

    ////---------------------------------------------------------------------------

    //string RWElementType(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return null; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return null;
    //      }

    //    return handle.m_TypeArray[index].m_Tipo;
    //    }
    //  return null;
    //  }

    ////---------------------------------------------------------------------------

    //Modo RWElementMood(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return Modo.NA; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return Modo.NA;
    //      }

    //    return handle.m_TypeArray[index].m_Modo;
    //    }
    //  return Modo.NA;
    //  }

    ////---------------------------------------------------------------------------

    //Tiempo RWElementTime(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return Tiempo.NA; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return Tiempo.NA;
    //      }

    //    return handle.m_TypeArray[index].m_Tiempo;
    //    }
    //  return Tiempo.NA;
    //  }

    ////---------------------------------------------------------------------------

    //Numero RWElementNumber(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return Numero.NA; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return Numero.NA;
    //      }

    //    return handle.m_TypeArray[index].m_Numero;
    //    }
    //  return Numero.NA;
    //  }

    ////---------------------------------------------------------------------------

    //Genero RWElementGender(HandleData handle, int index)
    //  {
    //  if (handle != null)
    //    {
    //    TLng language = handle.m_Idioma;
    //    if (language == TLng.NA)
    //      {
    //      //AfxMessageBox("El idioma no se ha cargado");
    //      return Genero.NA; //
    //      }

    //    if ((index >= handle.m_TypeArray.Count) || (index < 0))
    //      {
    //      //AfxMessageBox("Indice de arreglo de tipos incorrecto");
    //      return Genero.NA;
    //      }

    //    return handle.m_TypeArray[index].m_Genero;
    //    }
    //  return Genero.NA;
    //  }

    ////---------------------------------------------------------------------------

    }

  ///-----------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Clase que contiene los datos de una reducción</summary>
  public class RedItem
    {
    public string  PalReduc   = "";                        // Palabra reducida
    public string  Tipo       = "";                        // Tipo gramatical al que se redujo (SS,AA,AI,VV,VA)
    public TMod    Modo       = TMod.Infinitivo;          // Si es verbo, el Modo
    public TTime  Tiempo     = TTime.Presente;          // Si es verbo, el Tiempo
    public TPer Persona    = TPer.Tercera;          // Si es verbo, la Persona
    public TNum  Numero     = TNum.Singular;          // Si es verbo o sust, el Número
    public TGen  Genero     = TGen.Masc;              // Si es sust o adj, el Género
    public TGrad   Grado      = TGrad.Positive;            // Si es adjetivo, grado de comparación
    public string  CDirecto   = "";                        // Si es un verbo con enclítico, cuál es el complemento directo
    public string  CIndirecto = "";                       // Si es un verbo con enclítico, cuál es el complemento indirecto
    public string  Prefijo    = "";                        // Prefijo que se le quitó a la palabra original para llegar a la reducida
    }

  }
