using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrdEngine.Dictionary
  {
#if CMD_SUPPORT
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa el sorte para los diccionarios CMD</summary>
  public partial class DictSingle
    {
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee un diccionario desde un fichero CMD, el argumento 'expand' define si se expanden las contanates establecidas con
    /// DEFINE, esto solo se debe usar para cambiar de un formato a otro, manteniendo las constantes</summary>
    public static DictSingle OpenCmd( string DccName, bool expand=true )
      {
      string FText;
      try { FText = File.ReadAllText( DccName ); }                // Lee el fichero completo
      catch( Exception e)
        {
        Dbg.Msg( "Leyendo el fichero :" + e.Message, Dbg.Error );
        return null;
        }

      try 
        {
        var Dict = CreateDictObj( DccName );                        // Crea el diccionario

        Dict.Tbls.expand = expand;                                  // Difine si se expanden las cadenas o no
        if( Dict.ParseCmdText( FText ) )                            // Pasea el texto y llena el diccionario
          return Dict;
        }
      catch( Exception e)
        {
        Dbg.Msg( "Interno parseando el fichero :" + e.Message, Dbg.Error );
        }

      return null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa una linea include, tomando el nombre del fichero, cargandolo y procesandolo</summary>
    private bool ProcessInclude( string Line )
      {
      string FName = "";
      try
        {
        int i = 7;                                                        // Salta la palabra INCLUDE
        FName = Line.GetBetween(ref i, '(', ')' );                        // Obtiene el nombre del fichero

        if( FName.StartsWith("..\\") )                                    // Si es relativo (NetFramework no soporta ..)
          {
          FName = FName.Substring( 3 );                                   // Quita inicio 
          FName = Path.Combine( Path.GetDirectoryName(DicName), FName);   // Lo toma relativo al diccionario
          }

        var incLines = File.ReadAllLines( FName );                        // Lee toda las lineas del fichero
        foreach(var lin in incLines )                                     // Recorre todas las lineas
          ProcessDefine( lin );                                           // La procesa una por una (si hay error la ignora)
            
        return true;                                                      // Todo OK
        }
      catch( Exception e )
        {
        return Dbg.Msg( "Al incluir el fichero de definicines.\r\n" + e.Message, Dbg.Error );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa una linea define, tomando la constante y su valor y poniendola en 'Defs'</summary>
    private bool ProcessDefine( string Line )
      {
      if( Tbls.Defs == null )                                       // Si no se ha creado la tabla de definciones
        Tbls.Defs = new Dictionary<string,string>();                // Crea el diccionario de definiciones

      if( !Line.StartsWith("DEFINE") )                              // Si no empieza con DEFINE no la procesa
        return false;
//        return Dbg.Msg("WARNING linea ignorada: " + Line );

      int i = 6;                                                    // Salta la palabra define
      var defBody = Line.GetBetween(ref i, '(', ')' );              // Obtiene el contenido de la instrucción
      if( defBody==null )                                           // No pudo obtener el contenido
        return Dbg.Msg("Falta parentisis en define:" + Line, Dbg.Error );        

      int j=0;
      var def = defBody.GetValue( ref j );                          // Obtiene el nombre de la constante
      if( def==null ) 
        return Dbg.Msg("Nombre incorrecto en define: " + Line, Dbg.Error );

      if( !defBody.SkipChar( ',', ref j ) )                         // Salta la comilla de separación
        return Dbg.Msg("Falta separador en define: " + Line, Dbg.Error );

      var val = defBody.GetValue( ref j );                          // Obtiene el valor de la constante
      if( val==null ) 
        return Dbg.Msg("Valor incorrecto en define: " + Line, Dbg.Error );
 
      Tbls.Defs[def] = val.Trim('"');                               // Quita las comillas
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza nombre del fichero y crea objeto diccionario con esa información</summary>
    private static DictSingle CreateDictObj( string Name )
      {
      var src = TLng.Es;                                        // Origen por defecto, español
      var des = TLng.En;                                        // Destino por defecto, inglés

      try
        {
        var file = Path.GetFileName( Name );                    // Separa el nombre del fichero
        var sSrc = file.Substring( 0, 2 );                      // Obtiene las dos primeras letras
        var sDes = file.Substring( 3, 2 );                      // Obtiene las letras 4 y 5

        var tSrc = Utils.Code(sSrc);                            // Convierte de cadena a tipo TLng
        var tDes = Utils.Code(sDes);                            // Convierte de cadena a tipo TLng

        if( tSrc!=TLng.NA ) src = tSrc;                         // Si se obtuvo un idioma fuente, lo toma  
        if( tDes!=TLng.NA ) des = tDes;                         // Si se obtuvo un idioma destino, lo toma  

        }
      catch{}

      var Dict = new DictSingle( src, des, DDataType.Rule );    // Crea objeto diccionario
      Dict.DicName = Path.ChangeExtension(Name, ".dcb");        // Pone nombre de fichero, con otra extensión

      return Dict;                                              // Retorna el diccionario
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adicciona una llave al diccionario chequeando si ya existe</summary>
    private bool ParseCmdText( string Txt )
      {
      int idx = 0;
      while( idx<Txt.Length )                                       // Recorre todo el texto del fichero
        {
        var line = Txt.GetLine(ref idx);                            // Obtiene una linea de texto

        if( line.Length==0             ) continue;                  // Linea vacia, la ignora
        if( line.StartsWith("//")      ) continue;                  // Comentario, lo ignora
        if( line.StartsWith("INCLUDE") )                            // Linea Include
          {
          ProcessInclude( line );                                   // Carga el fichero y pocesa su contenido
          continue;                                                 // Continua con la proxima linea
          }
        if( line.StartsWith("DEFINE")  )                            // Una definición
          {
          ProcessDefine( line );                                    // Procesa la linea y adicciona constante a la tabla
          continue;                                                 // Continua con la proxima linea
          }
        
        if( line.StartsWith("KEYWORD") )                            // Es una palabra clave
          {
          int i = 7;                                                // Salta KEYWORD
          string sKey = line.GetBetween( ref i, '(', ')' ) ;        // Obtiene la llave

          var sData = Txt.GetBetween( ref idx, '{', '}' );          // Obtiene todos los datos de la llave
          if( sData==null ) 
            return Dbg.Msg("No casan bien las llaves en la entrada '" + sKey + "'", Dbg.Error );

          var RData = RuleData.FromStr( sData, Tbls );              // Obtiene un objeto RuleData a partir de la cadena sData

          AddRuleKey( sKey, RData );                                // Adiciona la llave al diccionario
          continue;                                                 // Continua con la proxima linea
          }

        Dbg.Msg("Ultima linea procesada: '" + line + '\'', Dbg.Error );
        break;
        }

      if( idx!=Txt.Length )                                         // No llego al final del fichero
        {
        return Dbg.Msg("El parse del fichero no llego al final", Dbg.Error );
        }
      else
        {
        if( Tbls.expand )                                           // Si ya se expandieron las definiciones
          Tbls.Defs = null;                                         // Ya no son necesarias, se ponen a null

        changed = true;                                             // Marca el diccionario como cambiado  
        Dbg.Msg(" **** EL DICCIONARIO SE CARGO CORRECTAMENTE\r\n");
        return true;                                                // Retorna diccionario
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    }
#endif

#if CMD_SUPPORT || XML_SUPPORT
  public partial class DictSingle
    {
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adicciona una regla, expandiendola y chequeado si hay que compartir datos</summary>
    private void AddRuleKey( string sKey, RuleData rData )
      {
      if( rData==null )
        {
        Dbg.Msg("WARNING: Hubo un error en el formato de los datos de la llave, " + sKey );
        return;
        }

      sKey = sKey.Expand( Tbls.Defs, Tbls.expand );             // Expande las constantes que esten en la llave
      var Keys = sKey.Split(',');                               // Divide las llaves separadas por coma

      if( Tbls.expand  && Keys.Length>1 )                       // Si hay mas de una y hay que expandir
        {
        rData.idxRule = Tbls.AddRuleData(rData) ;               // Guarda dato en tabla de datos compartidos y retorna indice

        foreach (var key in Keys)                               // Recorre todas las llaves
          AddChkKey( key, rData);                               // Adiciona cada llave con los mismos datos
        }
      else
        AddChkKey( sKey, rData);
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adicciona una llave al diccionario chequeando si ya existe</summary>
    private void AddChkKey( string Key, RuleData rData )
      {
      if( Data.ContainsKey(Key) )                                 // Si la llave ya esta en el diccionario
        {
//        Dbg.Msg( "Llave '"+ Key +"' actualizada" );
        var oldData = GetRuleData(Key);                           // Obtiene los datos de la llave

        var newData = new RuleData();                             // Crea datos de regla nuevos
        foreach( var match in oldData.Matchs )                    // Para todos los matchs de la regla vieja
          newData.Matchs.Add( match );                            // Los agrega a la nueva

        foreach( var match in rData.Matchs )                      // Para todos los match de la regla actual
          newData.Matchs.Add( match );                            // Los agrega a la nueva

        AddKey( Key, newData );                                   // Adiciona nuevos datos a la llave
        }
      else
        AddKey( Key, rData );                                     // Adiciona la llave al diccionario
      }

    //---------------------------------------------------------------------------------------------------------------------------------
    }

  ///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary>Adicciona dos datos a rule table especificos para ficheros CMD y XML</summary>
  public partial class RulesTables
    {
    internal Dictionary<string,string> Defs = null;                                  // Diccionario de definición de constantes
    internal bool expand = true;                                                     // Define si se expanden las constantes o no 
    }

#endif
  }
