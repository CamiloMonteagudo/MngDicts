using System.Collections.Generic;
using System.Text;
using TrdEngine;
using TrdEngine.Dictionary;

namespace Tests
  {
  public class WordDataToText
    {
    static Dictionary<string,string> TypesAbrevEn = new Dictionary<string,string>
    { {"SS","Noun"},{"NP","Prop.Noun"},{"AA","Adj."},{"DD","Adv."},{"VT","Trans.V."},{"VI","Intrans.V."},{"VR","Ref.V."},{"VA","Aux.V."},{"PP","Prep."},{"PT","Trans.P."},{"PI","Intrans.P."},{"GT","Trans.G."},{"GI","Intrans.G."},{"CC","Conj."},{"JJ","Interj."},{"AI","Static Adj."} };

    static Dictionary<string,string> TypesAbrevEs = new Dictionary<string,string>
    { {"SS","Sust."},{"NP","N.Prop.  "},{"AA","Adj."},{"DD","Adv."},{"VT","V.Trans."},{"VI","V.Intrans."},{"VR","V.Ref."},{"VA","V.Aux."},{"PP","Prep."},{"PT","P.Trans."},{"PI","P.Intrans."},{"GT","G.Trans."},{"GI","G.Intrans."},{"CC","Conj."},{"JJ","Interj."},{"AI","Adj.Estat."} };

    static Dictionary<string,string> TypesAbrevFr = new Dictionary<string,string>
    { {"SS","Sust."},{"NP","N.Propre "},{"AA","Adj."},{"DD","Adv."},{"VT","V.Trans."},{"VI","V.Intrans."},{"VR","V.Ref."},{"VA","V.Aux."},{"PP","Prép."},{"PT","P.Trans."},{"PI","P.Intrans."},{"GT","G.Trans."},{"GI","G.Intrans."},{"CC","Conj."},{"JJ","Interj."},{"AI","Adj.fixe"  } };

    static Dictionary<string,string> TypesAbrevIt = new Dictionary<string,string>
    { {"SS","Sost."},{"NP","N.Proprio"},{"AA","Agg."},{"DD","Avv."},{"VT","V.Trans."},{"VI","V.Intrans."},{"VR","V.Rif."},{"VA","V.Aus."},{"PP","Prep."},{"PT","P.Trans."},{"PI","P.Intrans."},{"GT","Ger.Trans."},{"GI","G.Intrans."},{"CC","Coni."},{"JJ","Interi."},{"AI","Agg.Stat." } };

    Dictionary<string,string> TypesAbrev = new Dictionary<string,string>();

    private void SetLangSetting( TLng lng )
      {
      switch( lng )                                                       // Idioma de la Interface de usuario actual del télefono
        {
        case TLng.Es: TypesAbrev = TypesAbrevEs; break;                   // Abreviaturas para el Español
        case TLng.En: TypesAbrev = TypesAbrevEn; break;                   // Abreviaturas para el Inglés
        case TLng.It: TypesAbrev = TypesAbrevIt; break;                   // Abreviaturas para el Italiano
        case TLng.Fr: TypesAbrev = TypesAbrevFr; break;                   // Abreviaturas para el Francés
        }
      }

    public string Text;
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Formatea datos de una palabra y devuelve una cadena que los representa </summary>
    public WordDataToText( string sKey, WordData Data, TLng lng )
      { 
      SetLangSetting( lng );

      var s = new StringBuilder( 200 );
      s.Append( sKey + ":" );                                             // Pone la llave en el control

      var nMSets = Data.MeanSets.Count;                                   // Obtiene número de grupos de significados

      string sep =" ";
      for( int i=0; i<nMSets; ++i )                                       // Recorre todos los grupos de significados
        {
        var MeanSet = Data.MeanSets[i];                                   // Obtiene grupo de significado actual

        int nMeans = MeanSet.Means.Count;                                 // Obtiene número de significados del grupo actual
        if( nMeans == 0 ) continue;                                       // Si no hay significados, salta el grupo

        string sTipo = GetTipo( MeanSet.Conds, Data );                    // Trata de obtener tipo gram. que representa al grupo
        s.Append( sep + sTipo );                                         // Lo pone en el control

        for( int j=0; j<nMeans; ++j )                                     // Recorre todos los significados
          {
          var Mean = MeanSet.Means[j];                                    // Obtiene significado actual
          if( Mean.Mean.Length==0 ) continue;                             // Si el significado es vacio, lo salta

          if( Mean.Esp != "GG" )                                          // Si la especialidad del significado no es general
            s.Append( " " + Mean.Esp );                    // La pone en el control

          if( Mean.Gen == TGen.Femen )                                    // Si genero del significado es femenino
            s.Append( " f." );                            // Lo pone en el control
          else if( Mean.Gen == TGen.Neutro )                              // Si genero del significado es Neutro
            s.Append( " n." );                            // Lo pone en el control

          if( Mean.Plur  )                                                // Si el número del significado es plural
            s.Append( " pl." );                           // Lo pone en el control

          string txt = " ";
          if( Mean.Info.Length>0  )                                       // Si el significado tiene info adicional
            txt = " [" + Mean.Info + "]";                                 // La pone en la cadena de salida

          txt += Mean.Mean ;                                              // Adiciona significado a la cadena de salida

          if( j<nMeans-1 ) txt += ',' ;                                   // Si no es el ultimo significado, adiciona una coma separadora

          s.Append( txt );                                // Pon el significado en el control
          }

        sep =" |";
        }

      //Txt.Replace( "<", fmtContIni );                                   // Sustituye inicio de las palabras de contexto
      //Txt.Replace( ">", fmtContEnd );                                   // Sustituye final de las palabras de contexto

      Text = s.ToString();                                            // Retorna cadena con toda la información
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Tipo por defecto para cada uno de los tipos compuestos </summary>
    static Dictionary<string,string> TblDefTipos = new Dictionary<string,string> { 
    {"MV", "NN" }, {"QA", "VI" }, {"QC", "PT" }, {"QD", "PI" }, {"QE", "VI" }, {"QF", "VI" }, {"QG", "VI" }, {"QI", "PP" }, {"QJ", "VA" }, {"QL", "PP" }, {"QM", "VI" }, {"SV", "VI" },
    {"TA", "VT" }, {"UA", "NP" }, {"UB", "PT" }, {"UC", "PT" }, {"UD", "GT" }, {"UE", "GT" }, {"UF", "HT" }, {"UG", "HI" }, {"UH", "VI" }, {"UI", "VI" }, {"UJ", "GI" }, {"UK", "VI" },
    {"UL", "HI" }, {"UM", "VI" }, {"UN", "GI" }, {"UO", "VI" }, {"UR", "VI" }, {"UT", "GT" }, {"UV", "NP" }, {"UX", "GI" }, {"UY", "VI" }, {"UZ", "VI" }, {"VV", "VI" }, {"XA", "VT" },
    {"XB", "VI" }, {"XD", "VT" }, {"XF", "DD" }, {"XG", "VI" }, {"XH", "DD" }, {"XI", "VI" }, {"XJ", "AA" }, {"XK", "DD" }, {"XM", "DD" }, {"XO", "AA" }, {"XY", "DD" }, {"YF", "VT" },
    {"YG", "VR" }, {"ZB", "VT" }, {"ZF", "VI" }, {"ZH", "VT" }, {"ZI", "VT" }, {"ZJ", "VT" }, {"ZV", "VI" }, {"ZZ", "VI" }  };

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Trata de deducir el tipo gramatical del grupo de significados en función del las condiciones </summary>
    private string GetTipo( Conditions Conds, WordData Data )
      {
      if( Conds == null )                                                 // Si no hay ninguna caondición
        {
        if( Data.MeanSets.Count == 1 )                                    // Si hay un solo grupo de significados
          return GetTypeAbrev( Data.CompType );                           // Toma como tipo del grupo, el tipo compuesto de la palabra

        string DefTipos;
        if( TblDefTipos.TryGetValue( Data.CompType, out DefTipos ) )      // Trata de obtener el tipo por defecto, según el tipo compuesto
          return GetTypeAbrev( DefTipos );                                // Si lo puede obtener, retorna la abreviatura

        return "";
        }

      var Funs = Conds.Funcs;                                             // Obtiene funciones que conforman la condición
      if( Funs.Count==1      &&                                           // Si hay una sola función
         (Funs[0] is DFuncW) &&                                           // Si la función es sobre una propiedad de la palabra actual
         ((DFuncW)Funs[0]).iArg==0 )                                      // Si la propiedad es el tipo gramatical
        return GetTypeAbrev( ((DFuncW)Funs[0]).sTipo );                   // Toma el argumento de la función como tipo gramatical

      return "";                                                          // No se puede determinar el tipo gramatical
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene la abreviatura de un tipo gramatical conociendo su código de 2 letras </summary>
    private string GetTypeAbrev( string sTipo )
      {
      string sAbrev;
      if( TypesAbrev.TryGetValue( sTipo, out sAbrev ) )                   // Si el tipo esta en al diccionario de abraviatura
        return sAbrev;                                                    // Retorna la abrevitura

      return null;                                                        // El tipo no tiena abraviatura que lo represente
      }
    }
  }
