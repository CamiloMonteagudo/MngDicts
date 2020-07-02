using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine;

namespace Tests
  {
  //----------------------------------------------------------------------------------------------------------------------------
  /// <summary>Implementa una forma más simple de representar los datos de una palabra</summary>
  public class ViewData
    {
    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Separador entre las llaves y los datos</summary>
    public static char KeySep = '\\';

    /// <summary>Separador entre las llaves y los datos</summary>
    public static char TypeSep = '|';

    /// <summary>Separador de los indices de las palabras</summary>
    public static char IdxSep = '|'; 

    /// <summary>Separador de los valores dentro del inidice de una palabra</summary>
    public static char ValSep = ','; 

    /// <summary>Separador entre los significados</summary>
    public static char MeanSep = ';'; 

    /// <summary>Marcador del tipo gramatical</summary>
    public static char TypeMark = '^'; 

    /// <summary>Marcador de especialidad</summary>
    public static char EspMark = '~'; 

    /// <summary>Lista de los tipos gramaticales que forman los datos</summary>
    public List<ViewType> Tipos = new List<ViewType>();

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un objeto a partir de una cadena de texto</summary>
    public ViewData( string sData, bool expMrk=false )
      {
      var sTipos = sData.Split( TypeSep );
      foreach( var sTipo in sTipos )
        Tipos.Add( new ViewType(sTipo.Trim(), expMrk) );
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla los datos del tipo actual, con los datos de 'datos2'</summary>
    public void Merge( ViewData datos2 )
      {
      foreach( var tipo in datos2.Tipos )
        Tipos.Add( tipo );

      UneTipos();
      DelCommonMeans();
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Une todos los tipos gramaticales del mismo tipo</summary>
    public void UneTipos()
      {
      for( int i=0; i<Tipos.Count; i++ )
        {
        var Tipo1 = Tipos[i];
        for( int j=i+1; j<Tipos.Count; j++ )
          {
          if( Tipos[j].Tipo== Tipo1.Tipo )
            UneMeans( i, j );
          }
        }
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Une los significados del tipo 'i' con los del tipo 'j' y borra el tipo 'j'</summary>
    private void UneMeans( int idx1, int idx2 )
      {
      var m1 = Tipos[idx1];
      var m2 = Tipos[idx2].Means;

      for( int i=0; i<m2.Count; i++ )
        {
        if( !m1.MeanContain( m2[i]) )
          m1.Add( m2[i] );
        }

      Tipos.RemoveAt(idx2);
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra los significados de un tipo indefinido, que esten en otro tipo</summary>
    private void DelCommonMeans()
      {
      if( Tipos.Count < 2 ) return;

      for( int i=0; i<Tipos.Count; i++ )
        {
        if( Tipos[i].Tipo =='\0' )
          DelCommonMeans( i );

        if( Tipos[i].Means.Count==0 )
          Tipos.RemoveAt(i--);
        }
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra los significados de un tipo i, que esten en otro tipo</summary>
    private void DelCommonMeans( int iTp )
      {
      var means = Tipos[iTp].Means;
      for( int m=0; m<means.Count; m++ )
        {
        var mean = means[m];
        for( int i=0; i<Tipos.Count; i++ )
          {
          if( i==iTp ) continue;
          if( Tipos[i].MeanContain(mean) )
            {
            means.RemoveAt(m--);
            break;
            }
          }
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Muestra los datos en forma de texto</summary>
    public override string ToString()
      {
      var str = new StringBuilder(100);
      for( int i=0; i<Tipos.Count; i++ )
        {
        if( i>0 )
          {
          str.Append( ' ' );
          str.Append( ViewData.TypeSep );
          str.Append( ' ' );
          }

        str.Append( Tipos[i].ToString() );
        }

      return str.ToString();
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita caracteres opcionales en todos los significados de los datos</summary>
    internal void NormalizeMeans()
      {
      for( int i=0; i<Tipos.Count; i++ )
        {
        var means = Tipos[i].Means;
        for( int j=0; j<means.Count; j++ )
          means[j] = Normalize( means[j], false, false);
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    static char[] del     = {',',' ',';'};
    static char[] NextSpc = {'¿','¡','-','$','/'};                      // Caracteres que no llevan espacio detras
    static char[] PrevSpc = {'?','!','-',',','*','%','/','\''};         // Caracteres que no llevan espacio delante
    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita caracteres opcionales en el significado 's'</summary>
    public static string Normalize( string strIn, bool delAttr=false, bool lower=true )
      {
      var str = strIn.Trim(del);                                        // Quita caracteres, delante, detras

      if(lower) str = str.ToLowerInvariant();                           // Si es especificado, lleva a minusculas

      if( delAttr )                                                     // Si hay que quitar los atributos de los significados
        {
        str.Replace("*","");
        str.Replace("f. ","");
        str.Replace("pl. ","");
        }

      for( int now=0;;)                                                 // Busca caracteres que no necesitan estrar seguidos de espacio
        {
        var iFind = str.IndexOfAny( NextSpc, now );                     // Busca uno de los caracteres
        if( iFind==-1 || iFind>=str.Length-1 ) break;                   // Si no encontro o es el último, termina

        now = iFind + 1;                                                // Apunta al proximo caracter
        if( str[now] == ' ' ) str = str.Remove( now, 1 );               // Si es un espacio, lo elimina
        }

      for( int now=0;;)                                                 // Busca caracteres que no necesitan estrar precedidos de espacio
        {
        var iFind = str.IndexOfAny( PrevSpc, now );                     // Busca una de los caracteres
        if( iFind==-1 || iFind<1 ) break;                               // Si no lo encontro o es el primero, termina        

        now = iFind - 1;                                                // Apunta al caracter anterior
        if( str[now] == ' ' ) str = str.Remove( now, 1 );               // Si es espacio lo elimina
        else now = iFind + 1;                                           // Continua en el proximo caracter
        }

      return str;                                                       // Restaura la cadeneda 
      }

    //---------------------------------------------------------------------------------------------------------------------------
    static Dictionary<string,int> idxTypes = new Dictionary<string,int>
    { {"SS",0},{"NP",1},{"AA",2},{"DD",3},{"VT",4},{"VI",5},{"VR",6},{"VA",7},{"PP",8},{"PT",9},{"PI",10},{"GT",11},{"GI",12},{"CC",13},{"JJ",14},{"AI",15} };

    static string[]  TypCode = { "SS"    , "NP"        , "AA"  , "DD"  , "VT"      , "VI"        , "VR"    , "VA"     , "PP"   , "PT"      , "PI"        , "GT"      , "GI"        , "CC"     , "JJ"     , "AI"          };
    static string[] TypDesEs = { "Sust." , "N.Prop."   , "Adj.", "Adv.", "V.Trans.", "V.Intrans.", "V.Ref.", "V.Aux." , "Prep.", "P.Trans.", "P.Intrans.", "G.Trans.", "G.Intrans.", "Conj."  , "Interj.", "Adj.Estat."  };
    static string[] TypDesEn = { "Noun." , "Prop.Noun.", "Adj.", "Adv.", "Trans.V.", "Intrans.V.", "Ref.V.", "Aux.V." , "Prep.", "Trans.P.", "Intrans.P.", "Trans.G.", "Intrans.G.", "Conj."  , "Interj.", "Static Adj." };
    static string[] TypDesIt = { "Sost." , "N.Prop."   , "Agg.", "Avv.", "V.Trans.", "V.Intrans.", "V.Rif.", "V.Auss.", "Prep.", "P.Trans.", "P.Intrans.", "G.Trans.", "G.Intrans.", "Coniug.", "Inter." , "Agg.Stat."   };
    static string[] TypDesFr = { "Subst.", "N.Propre." , "Adj.", "Adv.", "V.Trans.", "V.Intrans.", "V.Ref.", "V.Aux." , "Prep.", "P.Trans.", "P.Intrans.", "G.Trans.", "G.Intrans.", "Conj."  , "Interj.", "Adj.Fixe."   };

    static string[][] TypDesLang = { TypDesEs, TypDesEn, TypDesIt, TypDesFr}; 

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Indice del tipo gramatical, teniendo el código de 2 letras </summary>
    public static int GetTypeIdx( string sTipo )
      {
      int iTipo;
      if( ViewData.idxTypes.TryGetValue( sTipo, out iTipo ) )           // Si el tipo esta en al diccionario de abraviatura
        return iTipo;                                                   // Retorna el indice

      return -1;                                                        // El tipo no es de los principales
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Indice del tipo gramatical, teniendo la abreviatura y el idioma </summary>
    public static int GetAbrevTypeIdx( string sTipo, int lng=-1  )
      {
      if( lng>=0 && lng<4 )                                             // Se especifico el idioma correctamente
        {
        var lngAbrv = TypDesLang[lng];                                  // Obtiene las abreviaturas del idioma

        for( int i=0; i<lngAbrv.Length; i++ )                           // Busca por todas las abreviaturas
          if( lngAbrv[i]==sTipo ) return i;
        }
      else                                                              // No se especifico el idioma
        {
        foreach( var lngAbrv in TypDesLang )                            // Busca por todos los idiomas
          {
          for( int i=0; i<lngAbrv.Length; i++ )                         // Busca por todas la abreviaturas del idioma actual
            if( lngAbrv[i]==sTipo ) return i;
          }
        }

      return -1;                                                        // No encontro un tipo con esa abreviatura
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el código del tipo gramatical, dado un caracter que representa el indice, donde la A es el 0 </summary>
    public static string GetTypeCode( char c )
      {
      int idx = c-'A';
      if( idx<0 || idx>=TypCode.Length )
        return "";

      return TypCode[idx];
      }
    
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene la abrevuatura del tipo gramatical, dado un caracter que representa el indice, donde la A es el 0 </summary>
    public static string GetTypeAbrv( char c, int lng )
      {
      int idx = c-'A';
      if( idx<0 || idx>=TypCode.Length || lng<0 || lng>=4 )
        return "";

      return TypDesLang[lng][idx];
      }
    
    //---------------------------------------------------------------------------------------------------------------------------
    static Dictionary<string,string> idxEsp = new Dictionary<string,string>
    { {"AE","01"},{"AG","02"},{"AN","03"},{"AQ","04"},{"AJ","05"},{"AU","06"},{"BI","07"},{"BT","08"},{"BO","09"},{"CC","10"},{"CL","11"},{"CO","12"},{"CP","13"},{"CS","14"},{"DE","15"},{"LA","16"},{"AB","17"},{"ED","18"},{"EN","19"},{"FA","20"},{"FE","21"},{"FG","22"},{"FL","23"},{"FI","24"},{"GO","25"},{"GR","26"},{"HI","27"},{"PR","28"},{"IN","29"},{"JE","30"},{"LI","31"},{"LO","32"},{"NA","33"},{"MA","34"},{"ME","35"},{"MT","36"},{"ML","37"},{"MN","38"},{"MI","39"},{"MU","40"},{"PE","41"},{"PO","42"},{"EC","43"},{"QM","44"},{"SO","45"},{"TP","46"},{"VE","47"},{"ZO","48"} };

    static string[] EspCod    = {"GG", "AE"        , "AG"        , "AN"       , "AQ"       , "AJ"    , "AU"        , "BI"       , "BT"           , "BO"      , "CC"       , "CL"         , "CO"        , "CP"       , "CS"        , "DE"       , "LA"       , "AB"        , "ED"        , "EN"        , "FA"        , "FE"         , "FG"      , "FL"         , "FI"        , "GO"         , "GR"        , "HI"        , "PR"       , "IN"         , "JE"       , "LI"         , "LO"       , "NA"    , "MA"         , "ME"        , "MT"         , "ML"         , "MN"        , "MI"       , "MU"       , "PE"       , "PO"         , "EC"      , "QM"        , "SO"       , "TP"      , "VE"        , "ZO"        };
    static string[] EspAbrvEs = {""  , "(Aeronáu.)", "(Agricul.)", "(Anatom.)", "(Arquit.)", "(Arte)", "(Automov.)", "(Biolog.)", "(Biotecnol.)" , "(Botán.)", "(Cocina)" , "(Coloq.)"   , "(Comerc.)" , "(Comput.)", "(Costura)" , "(Deport.)", "(Derecho)", "(Ecología)", "(Educac.)" , "(Entomol.)", "(Farmac.)" , "(Ferroc.)"  , "(Figur.)", "(Filosof.)" , "(Física)"  , "(Geograf.)" , "(Gramát.)" , "(Historia)", "(Impres.)", "(Ingenier.)", "(Jerga)"  , "(Literat.) ", "(Lógica)" , "(Mar)" , "(Matemát.)" , "(Medicina)", "(Meteorol.)", "(Militar)"  , "(Mineral.)", "(Mitol.)" , "(Música)" , "(Poesía)" , "(Política)" , "(Relig.)", "(Química)" , "(Sociol.)", "(Topog.)", "(Veterin.)", "(Zoología)"};
    static string[] EspAbrvEn = {""  , "(Aeronau.)", "(Agricul.)", "(Anatom.)", "(Archit.)", "(Art)" , "(Automob.)", "(Biolog.)", "(Biotechnol.)", "(Botan.)", "(Cooking)", "(Colloq.)"  , "(Commerc.)", "(Comput.)", "(Sewing)"  , "(Sport)"  , "(Law)"    , "(Ecology )", "(Educat.)" , "(Entomol.)", "(Pharmacy)", "(Railway)"  , "(Figur.)", "(Philoso.)" , "(Physics)" , "(Geograp.)" , "(Grammar)" , "(History)" , "(Print)"  , "(Engineer.)", "(Slang)"  , "(Literat.) ", "(Logic)"  , "(Sea)" , "(Mathemat.)", "(Medicine)", "(Meteorol.)", "(Military)" , "(Mineral.)", "(Mythol.)", "(Music)"  , "(Poetic.)", "(Politics)" , "(Relig.)", "(Chemical)", "(Sociol.)", "(Topog.)", "(Veterin.)", "(Zoology)" };
    static string[] EspAbrvIt = {""  , "(Areonau.)", "(Agricol.)", "(Anatom.)", "(Archit.)", "(Arte)", "(Automob.)", "(Biolog.)", "(Biotecnol.)" , "(Botan.)", "(Cucina)" , "(Colloq.)"  , "(Commerc.)", "(Inform.)", "(Sartoria)", "(Sport)"  , "(Diritto)", "(Ecologia)", "(Educaz.)" , "(Entomol.)", "(Farmacia)", "(Ferrov.)"  , "(Figur.)", "(Filosof.)" , "(Fisica)"  , "(Geograf.)" , "(Grammat.)", "(Storia)"  , "(Stampa)" , "(Ingenier.)", "(Tessile)", "(Letterat.)", "(Logica)" , "(Mare)", "(Matemat.)" , "(Medicina)", "(Meteorol.)", "(Militare)" , "(Mineral.)", "(Mitol.)" , "(Musica)" , "(Poesia)" , "(Politica)" , "(Relig.)", "(Chimica)" , "(Sociol.)", "(Topog.)", "(Veterin.)", "(Zoologia)"};
    static string[] EspAbrvFr = {""  , "(Aéronau.)", "(Agricul.)", "(Anatom.)", "(Archit.)", "(Art)" , "(Aumobil.)", "(Biolog.)", "(Biotecnol.)" , "(Botan.)", "(Cuisine)", "(L. parlée)", "(Commerc.)", "(Inform.)", "(Couture)" , "(Sport)"  , "(Droit)"  , "(Ecologie)", "(Enseign.)", "(Entomol.)", "(Pharmac.)", "(C. de fer)", "(Figuré)", "(Philosof.)", "(Physique)", "(Géograph.)", "(Gramma.)" , "(Histoire)", "(Impres.)", "(Génie)"    , "(Jargon)" , "(Littérat.)", "(Logique)", "(Mar)" , "(Mathémat.)", "(Médecine)", "(Metéorol.)", "(Militaire)", "(Minéral.)", "(Mythol.)", "(Musique)", "(Poesie)" , "(Politique)", "(Rélig.)", "(Chimie)"  , "(Sociol.)", "(Topog.)", "(Vétérin.)", "(Zoologie)"};

    static string[][] EspAbrvLang = { EspAbrvEs, EspAbrvEn, EspAbrvIt, EspAbrvFr}; 
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el indice correspondiente a una especialidad </summary>
    public static string GetEspIdx( string esp )
      {
      string iEsp;
      if( idxEsp.TryGetValue( esp, out iEsp ) )                       // Busca el indice de la especialidad
        return iEsp;                                                  // Retorna el indice

      Dbg.Msg("***AVISO: La especialidad no esta en la lista '" + esp + "'");
      return "";                                                      // La especialidad no esta en la lista
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el codigo de la especialidad, desde una cadena en formato DView </summary>
    public static string GetEspCode( int idx )
      {
      if( idx<0 || idx>=EspCod.Length )
        return "GG";

      return EspCod[idx];
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el codigo de la especialidad, desde una cadena en formato DView </summary>
    public static string GetEspAbrv( int idx, int lng  )
      {
      if( idx<0 || idx>=EspCod.Length || lng<0 || lng>=4 )
        return "";

      return EspAbrvLang[lng][idx];
      }

    }
  //----------------------------------------------------------------------------------------------------------------------------
  /// <summary>Implementa una forma más simple de representar los datos de un tipo gramatical</summary>
  public class ViewType
    {
    /// <summary>Codigo de una letra del tipo gramatical</summary>
    public char Tipo;
    /// <summary>Lista de los significados del tipo gramatical</summary>
    public List<string> Means = new List<string>();

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un objeto a partir de una cadena de texto</summary>
    public ViewType( string s, bool expMrk=false  )
      {
      if( expMrk ) s = GetAbrevType(s);
      else         s = GetOneByteType(s);

      var sMeans = s.Split( ViewData.MeanSep );
      foreach( var sMean in sMeans )
        Means.Add( sMean.Trim() );
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el tipo gramatical, definido por un solo byte (una letra)</summary>
    private string GetOneByteType( string s )
      {
      if( s.Length>3 && s[0]==ViewData.TypeMark && s[2]==' ' )
        {
        Tipo = s[1];
        s = s.Substring(3);
        }
      else
        Tipo = '\0';

      return s;
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el tipo gramatical, definido por su abreviatura</summary>
    private string GetAbrevType( string s )
      {
      var iFind = s.IndexOf(". ");
      if( iFind==-1 )  { Tipo = '\0'; return s; }

      var abrv = s.Substring(0,iFind+1);
      var iTp  = ViewData.GetAbrevTypeIdx( abrv );
      if( iTp == -1 ) { Tipo = '\0'; return s; }

      Tipo = (char)('A' + iTp);

      return s.Substring( iFind+2 ); 
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca si dentro del tipo gramatical esta el significado 's' </summary>
    public bool MeanContain( string s )
      {
      string ss = ViewData.Normalize( s, true );
      foreach( var item in Means )
        if( ss == ViewData.Normalize(item, true ) ) return true;

      return false;
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona el significado 'mean' al tipo gramatical</summary>
    internal void Add( string mean )
      {
      Means.Add(mean);
      }

    //----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Retorna una cadena con la representación de los datos del tipo gramatical</summary>
    public override string ToString()
      {
      var str = new StringBuilder(100);
      if( Tipo != '\0' )
        {
        str.Append( ViewData.TypeMark );
        str.Append( Tipo );
        str.Append( ' ' );
        }

      for( int i=0; i<Means.Count; i++ )
        {
        if( i>0 )
          {
          str.Append( ViewData.MeanSep );
          str.Append( ' ' );
          }

        str.Append( Means[i] );
        }

      return str.ToString();
      }
    }
  }
