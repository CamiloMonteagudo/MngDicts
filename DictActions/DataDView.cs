using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine;
using TrdEngine.Dictionary;

namespace Tests
  {
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Extenciones para manejar los datos del tipo DictView, estos son datos de tipo texto optimizados para ser mostrados
    /// eficientemente en IPhone, Mac y otras plataformas</summary>
  static public class DataDView
    {
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene un objeto con los datos de una palabra, desde una cadena en formato DView </summary>
    public static bool FromDView( this WordData dt, string sData )
      {
      var sTipos = sData.Split( ViewData.TypeSep );

      for( int i=0; i<sTipos.Length; ++i )
        {
        var sMean = sTipos[i];
        if( string.IsNullOrWhiteSpace(sMean) ) continue;

        int j = 0;
        var tipo = TipoFromDView( sMean, ref j );

        Conditions cond = null;
        if( !string.IsNullOrWhiteSpace(tipo) ) cond = MeanSet.GetCondition( "W=" + tipo );

        var Means = MeanSetFromDView( sMean.Substring(j), cond );

        dt.MeanSets.Add( Means );                                            // Adiciona grupos de significados a la palabra
        }

//      dt.cType = xType.Value;                                            // La asocia el tipo gramatical
      return true;
      }  

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el tipo gramatical, desde una cadena en formato DView </summary>
    private static string TipoFromDView( string sTipo, ref int i )
      {
      string tCode="";
      while( sTipo[i]<=' ' ) ++i;

      if( sTipo[i]==ViewData.TypeMark )
        {
        tCode = ViewData.GetTypeCode( sTipo[++i] );
        ++i;
        }

      return tCode;
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene los significados, desde una cadena en formato DView </summary>
    public static MeanSet MeanSetFromDView( string s, Conditions Conds )
      {
      var Means = new MeanSet( Conds );

      var sMeans = s.Split(';');                                        // Separa todos los significados
      for( int i=0; i<sMeans.Length; ++i )                              // Recorre todos los significados
        {
        var str = sMeans[i].Trim();                                     // Toma el significado y quita los espacios
        if( str.Length == 0 ) continue;

        WordMean WM = new WordMean();                                   // Crea objeto para guardar significado

        int j = 0;
        WM.Esp = EspFromDView( str, ref j );                            // Toma la especialidad si la hay

        if( j+3<str.Length && str.Substring(j,2)=="f." )                // Si se define el genero
          {
          WM.Gen = TGen.Femen;                                          // Pone el genero al significado
          j += 2;                                                       // Salta las letras
          }

        while( j<str.Length && str[j]<=' ' ) ++j;                       // Salta los espacios
        if( j+4<str.Length && str.Substring(j,3)=="pl." )               // Si se define el numero
          {
          WM.Plur = true;                                               // Pone el número al significado
          j += 3;                                                       // Salta las letras
          }

        while( j<str.Length && str[j]<=' ' ) ++j;                       // Salta los espacios
        WM.Mean = str.Substring(j);                                     // El resto del texto lo toma como significado

        Means.Means.Add( WM );                                          // Adiciona significado al MeanSet
        }

      return Means;                                                     // Retorna objeto MeanSet con todos los significados
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Extrae la informacion contextual de un significado de la cadena str si la hay </summary>
    private static string InfoFromDView( string str, ref int j )
      {
      int i = j;
      if( str[i++] != '[' ) return "";                                  // Si no empieza con [ no es información contextual      

      int ii = i;                                                       // Guarda el inicio de la información
      int n = 0;
      while( i<str.Length && str[i++]!=']' ) ++n;                       // Busca hasta el cierre del corchete y cuenta los caracteres

      if( i>=str.Length ) return "";                                    // Si no termina con ], ignora la informacion

      while( str[i]<=' ' ) ++i;                                         // Salta los espacios

      j = i;                                                            // Retorna el indice donde termina el analisis
      return str.Substring( ii, n );                                    // Retorna la información contextual
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna los datos de una palabra en formato DView </summary>
    public static string ToDView( this WordData Data, string sKey )
      { 
      sKey = sKey.Trim();

      var s = new StringBuilder( 200 );
      s.Append( sKey + ViewData.KeySep );                                 // Pone la llave en el control mas el separador

      var nMSets = Data.MeanSets.Count;                                   // Obtiene número de grupos de significados

      string sep ="";
      for( int i=0; i<nMSets; ++i )                                       // Recorre todos los grupos de significados
        {
        s.Append( sep );                                                  // Pone el separador de tipos, si hay mas de uno
        var MeanSet = Data.MeanSets[i];                                   // Obtiene grupo de significado actual

        int nMeans = MeanSet.Means.Count;                                 // Obtiene número de significados del grupo actual
        if( nMeans == 0 ) continue;                                       // Si no hay significados, salta el grupo

        int iTipo = GetIdxTipo( MeanSet.Conds, Data );                    // Trata de obtener tipo gram. que representa al grupo
        if( iTipo >= 0)                                                   // Si obtuvo el tipo gramatical
          {
          s.Append( ViewData.TypeMark );                                  // Agrega marcador de tipo gramatical
          s.Append( (char)('A'+iTipo) );                                  // Agrega el indice el tipo gramatical
          s.Append( ' ');                                                 // Agrega un espacio de separación
          }

        for( int j=0; j<nMeans; ++j )                                     // Recorre todos los significados
          {
          var Mean = MeanSet.Means[j];                                    // Obtiene significado actual
          if( Mean.Mean.Length==0 ) continue;                             // Si el significado es vacio, lo salta

          if( j>0 )                                                       // Si no es el primer significado
            {
            s.Append( ViewData.MeanSep);                                  // Agrega un separador de significados
            s.Append( ' ' );                                              // Agrega un espacio de sepación
            }

          if( Mean.Esp != "GG" )                                          // Si la especialidad del significado no es general
            {
            var iEsp = ViewData.GetEspIdx(Mean.Esp);                      // Si al especialidad esta en la lista estandar
            if( iEsp.Length>0 ) s.Append( ViewData.EspMark + iEsp + ' ' );             // Pone el indice a la especialidad
            }

          if( Mean.Gen==TGen.Femen ) s.Append( "f. " );                   // Si genero del significado es femenino, lo pone
          if( Mean.Plur  )           s.Append( "pl. " );                  // Si el número del significado es plural, lo pone

          if( Mean.Info.Length>0  )                                       // Si el significado tiene info adicional
            {
            var sInfo = Mean.Info.Replace(";", ", ");                     // Sustituye los punto y como, por comas
            s.Append( "<" + sInfo + "> ");                                // La pone en la cadena de salida
            }

          s.Append( Mean.Mean );                                          // Adiciona significado a la cadena de salida
          }

        sep =" " + ViewData.TypeSep + ' ';                                // Separacion entre tipos gramaticales (acepciones)
        }

      return s.ToString();                                                // Retorna cadena con toda la información
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
    ///<summary> Retorna el indice del tipo gramatical representado por las condiciones 'Conds' </summary>
    static int GetIdxTipo( Conditions Conds, WordData Data )
      {
      var code = GetCodTipo( Conds, Data );
      if( code == null ) return -1;

      return ViewData.GetTypeIdx( code );                     
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna el codigo del tipo gramatical representado por las condiciones 'Conds' </summary>
    static string GetCodTipo( Conditions Conds, WordData Data )
      {
      if( Conds == null )                                                 // Si no hay ninguna caondición
        {
        if( Data.MeanSets.Count == 1 )                                    // Si hay un solo grupo de significados
          {
          if( string.IsNullOrWhiteSpace(Data.CompType) )                  // Si no existe un tipo compuesto 
            return null;                                                  // No hay tipo

          return Data.CompType;                                           // Toma como tipo del grupo, el tipo compuesto de la palabra
          }

        string DefTipos;
        if( TblDefTipos.TryGetValue( Data.CompType, out DefTipos ) )      // Trata de obtener el tipo por defecto, según el tipo compuesto
          return DefTipos;                                                // Si lo puede obtener, retorna la abreviatura

        return null;
        }

      var Funs = Conds.Funcs;                                             // Obtiene funciones que conforman la condición
      if( Funs.Count==1      &&                                           // Si hay una sola función
         (Funs[0] is DFuncW) &&                                           // Si la función es sobre una propiedad de la palabra actual
         ((DFuncW)Funs[0]).iArg==0 )                                      // Si la propiedad es el tipo gramatical
        return ((DFuncW)Funs[0]).sTipo;                                   // Toma el argumento de la función como tipo gramatical
                                                                                
      return null;                                                        // No se puede determinar el tipo gramatical
      }

    //---------------------------------------------------------------------------------------------------------------------------
    private static string EspFromDView( string str, ref int j )
      {
      int i = j;                                                        // Copia indice donde empieza el analisis
      while( str[i]<=' ' ) ++i;                                         // Salta los espcios al principio

      if( str[i++] != ViewData.EspMark ) return "GG";                   // Si encuentra el marcador lo salta, sino termina

      var d1 = str[i++]-'0';                                            // Obtiene primer digito
      var d2 = str[i++]-'0';                                            // Obtiene segundo digito
      if( d1<0 || d1>9 || d2<0 || d2>9 )  return "GG";                  // Si algún digito no es numerico, termina

      int idx = 10*d1 + d2;                                             // Calcula el indice                          

      while( str[i]<=' ' ) ++i;                                         // Salta los espacios después de la especialidad

      j = i;                                                            // Actualiza el indice donde termino el analisis
      return ViewData.GetEspCode(idx);                                  // Retorna la especialidad
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Mezcla dos objetos WordData </summary>
    public static void Merge( this WordData WData1, WordData WData2 )
      {
      WData1.SetSinglesTypes();                                       // Pones todas la condiciones a tipos simples
      WData2.SetSinglesTypes();

      foreach( var item in WData2.MeanSets )                          // Tomo todos los significado del segundo grupo
        WData1.MeanSets.Add( item );                                  // Los adiciona al primer grupo

      WData1.UneMeansSet();                                           // Une todos los grupos de significados del mismo tipo
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Pone todos los tipos gramaticales de manera explicita </summary>
    public static void SetSinglesTypes( this WordData Data )
      {
      var nMSets = Data.MeanSets.Count;                                   // Obtiene número de grupos de significados

      for( int i=0; i<nMSets; ++i )                                       // Recorre todos los grupos de significados
        {
        var MeanSet   = Data.MeanSets[i];                                 // Obtiene grupo de significado actual
        var codTipo   = GetCodTipo( MeanSet.Conds, Data );                // Trata de obtener tipo gram. que representa al grupo
        if( codTipo != null )                                             // Si el grupo, puede representar un tipo simple
          MeanSet.Conds = MeanSet.GetCondition( "W=" + codTipo );         // Lo pone como condición
        else                                                              // Si no
          MeanSet.Conds = null;                                           // No pone ninguna condición
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Une todos los conjuntos de significados del mismo tipo </summary>
    public static void UneMeansSet( this WordData Data )
      {
      int IdxNull = -1;                                                 // Grupo sin condiciones
      for( int i=0; i<Data.MeanSets.Count; ++i )                        // Recorre todos los grupos de significados
        {
        var MSet1 = Data.MeanSets[i];                                   // Obtiene grupo de significado actual
        var Cond1 = MSet1.strConds();                                   // Obtiene representación en texto de las condiciones

        for( int j=i+1; j<Data.MeanSets.Count; ++j )                    // Recorre los grupos de significados restantes
          {
          var MSet2 = Data.MeanSets[j];                                 // Obtiene grupo de significado actual
          var Cond2 = MSet2.strConds();                                 // Obtiene representación en texto de las condiciones

          if( Cond1 == Cond2 )                                          // Si las condiciones son iguales al grupo actual
            {
            MSet1.Merge( MSet2 );                                       // Mezcla este grupo con el grupo actual
            Data.MeanSets.RemoveAt(j);                                  // Remueve el grupo de significados
            --j;                                                        // Se queda analizando en el mismo lugar
            }
          }

        if( Cond1=="" )  IdxNull = i;                                   // Guarda el grupo sin condiciones, para luego analizar
        }

      if( IdxNull != -1 )                                               // Si habia un grupo de significados sin condición
        {
        var MSet = Data.MeanSets[IdxNull];                              // Obtiene grupo de significado, que no tiene tipo
        Data.GetCommon( MSet );                                         // Quita los significados comunes a otros tipos

        if( MSet.Means.Count==0 )                                       // Si no queda ningún significado
          Data.MeanSets.RemoveAt(IdxNull);                              // Remueve el grupo de los significados
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca los significados de 'mSet' que esten en otros grupos de significados y los borra </summary>
    public static void GetCommon( this WordData Data, MeanSet mSet )
      {
      for( int i=0; i<mSet.Means.Count; i++ )
        {
        var mean = mSet.Means[i].Mean;
        if( Data.ContainMean( mean, mSet ) )
          mSet.Means.RemoveAt( i-- );
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Determina si los datos de la palabra contienen el significado 'mean', la busqueda no incluye el grupo 'NoSet'</summary>
    public static bool ContainMean( this WordData Data, string mean, MeanSet NoSet )
      {
      for( int i=0; i<Data.MeanSets.Count; ++i )                        // Recorre todos los grupos de significados
        {
        var MSet = Data.MeanSets[i];                                    // Obtiene grupo de significado actual
        if( MSet == NoSet ) continue;                                   // Si es el que hay que excluir, lo salta

        if( MSet.Contain( mean ) ) return true;                         // Busca si el significado esta dentro del grupo
        }

      return false;
      }


    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene una cadena que representa las condiciones del grupo de significados </summary>
    public static string strConds( this MeanSet MSet )
      {
      var cond = MSet.Conds;
      if( cond==null ) return "";
      return cond.ToString();
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Mezcla dos grupos de significados </summary>
    public static void Merge( this MeanSet MSet1, MeanSet MSet2)
      {
      foreach( var item in MSet2.Means )                              // Agrega todos los significados al grupo actual   
        MSet1.Means.Add( item );

      MSet1.UneMeans();                                               // Une todos los significados iguales
      }

    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Quita los significados que sean iguales </summary>
    public static void UneMeans( this MeanSet MSet)
      {
      for( int i=0; i<MSet.Means.Count; ++i )                           // Recorre todos los significados del grupo
        {
        var Mean1  = MSet.Means[i];                                     // Obtiene el objeto del significado actual
        var sMean1 = Mean1.Mean;                                        // Obtiene el significado actual

        for( int j=i+1; j<MSet.Means.Count; ++j )                       // Recorre los ignificados restantes
          {
          var Mean2  = MSet.Means[j];                                   // Obtiene el objeto del significado a analizar
          var sMean2 = Mean2.Mean;                                      // Obtiene el significado a analizar

          if( sMean1 == sMean2 )                                        // Los dos significados son iguales
            {
            MSet.Means.RemoveAt(j);                                     // Remueve el significados
            --j;                                                        // Se queda analizando en el mismo lugar
            }
          }
        }
      }
    //---------------------------------------------------------------------------------------------------------------------------
    ///<summary> Determina si en el grupo de significados esta 'mean' </summary>
    public static bool Contain(  this MeanSet MSet, string mean )
      {
      for( int i=0; i<MSet.Means.Count; ++i )                           // Recorre todos los significados del grupo
        {
        var Mean  = MSet.Means[i];                                      // Obtiene el objeto del significado actual
        if( Mean.Mean == mean ) return true;                            // Si son iguales retorna verdadero
        }

      return false;
      }
    }
  }
