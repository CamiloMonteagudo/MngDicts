using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace TrdEngine.Dictionary
  {
#if XML_SUPPORT
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa el sorte XML para los diccionarios</summary>
  public partial class DictSingle
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre un diccionario simple desde un fichero xml</summary>
    ///
    ///<param name="Path">Camino donde esta el archivo que representa el diccionario</param>
    //------------------------------------------------------------------------------------------------------------------
    public static DictSingle OpenXml( string DName )
      {
      try
        {
        var doc  = XDocument.Load( DName );                                   // Carga fichero XML con el diccionario
        var Dict = CreateFromHrd( doc.Root );                                 // Inicializa diccionario, con datos en el primer elemento

        Dict.DicName = Path.ChangeExtension(DName, ".dcb");                   // Pone nombre de fichero, con otra extensión
        Dict.GetIncludeDefs( doc.Root );                                      // Si tiene include carga las definiciones

        var elms = doc.Root.Elements("KEYWORD");                              // Obtiene todos los elemento KEYWORD
        foreach( var el in elms )                                             // Recorre por cada elemento
          {
          var xkey = el.Attribute("key");                                     // Obtiene atributo de nombre de la llave
          if( xkey==null )
            {
            Dbg.Msg("WARNING: Llave ignorada, KEYWORD sin atributo 'key'");
            continue;
            }

          var sKey = xkey.Value;                                              // Obtiene valor de la llave
          if( el.Value == null )                                              // Verifica si hay datos asociados a la llave
            {
            Dbg.Msg("WARNING: Llave ignorada, los datos no pueden ser nulos");
            continue;
            }

          DicData dData;
          var typ = el.Attribute("type");                                     // Obtiene atributo de tipo de dato
          string styp = (typ==null)? "Str" : typ.Value;                       // Si no hay atributo, toma cadena por defecto
          switch( styp )
            {
            case "Str":                                                       // Para datos del tipo cadena
              dData = new DicData( el.Value );                                // Obtiene datos como cadena
              break;
            case "Wrd":                                                       // Para datos del tipo palabra de traducción
              dData = new DicData( WordData.FromXml(el) );                    // Obtiene el objeto
              break;
            case "Tbl":                                                       // Para datos del tipo tabla
              dData = new DicData( Tabla.FromXml(el) );                       // Obtiene el objeto
              break;
            case "Rgl":                                                       // Para datos del tipo regla
              var rData = RuleData.FromXml( el, Dict.Tbls );                  // Obtiene un objeto regla
              Dict.AddRuleKey( sKey, rData );                                 // Adiciona la llave al diccionario
              continue;
            //case "Bin":
            //  break;
            default:
              Dbg.Msg("WARNING: Tipo de dato no soportado para lectura XML, " + sKey );
              continue;
            }

          if( dData.Data != null )                                            // Comprueba lo dato, por si error
            {
            if( Dict.IsKey(sKey) )                                            // Cheque que la llave no este en el diccionario
              Dbg.Msg("WARNING: llave duplicada, " + sKey  );
            else
              Dict.Data[sKey] = dData;                                        // Agrega llave al diccionario
            }
          else
            Dbg.Msg("WARNING: Hubo un error en el formato de los datos de la llave, " + sKey );
          }

        if( Dict.Tbls != null ) Dict.Tbls.Defs = null;                        // Anula tabla de definiciones (ya se tuvo en cuenta)
        Dict.changed   = true;                                                // Marca el diccionario como cambiado  

        Dbg.Msg( "INFO: Total de llaves adiccionadas: " + Dict.Count );
        return Dict;
        } 
      catch (Exception e )                                                    // Hubo algun error no previsto
        {
        Dbg.Msg("Error interno, " + e.Message, Dbg.Error );
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Guarda el contenido de un diccionario en un fichero xml</summary>
    ///
    ///<param name="Path">Camino del archivo donde se quiere guardar el diccionario</param>
    //------------------------------------------------------------------------------------------------------------------
    public bool SaveXml( string DName )
      {
      try
        {
        var xml = new StringBuilder( 100 * Count );

        xml.Append( GetDictXmlRoot( DName ) );

        foreach( var key in Data )
          xml.Append( GetKeyXml(key) );

        xml.Append( "</DICTIONARY>\r\n" );

        var s = xml.ToString();
        File.WriteAllText( DName, s ); 

        return true;
        }
      catch (Exception e )
        {
        Dbg.Msg("Error interno, " + e.Message, Dbg.Error );
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la representación en XML de una entrada en el diccionario</summary>
    public string GetKeyXml( KeyValuePair<string,DicData> KeyEntry )
      {
      string sTipo = "Str";
      string sKey = KeyEntry.Key;

      string xRData = null;
      switch( (KeyEntry.Value.Tipo) )
        {
        case DData.bStr:
        case DData.oStr: xRData = GetStrData(sKey);           sTipo="Str"; break;
        case DData.bWrd:
        case DData.oWrd: xRData = GetWordData(sKey).ToXml();  sTipo="Wrd"; break;
        case DData.bRgl:
        case DData.oRgl: xRData = GetRuleData(sKey).ToXml();  sTipo="Rgl"; break;
        case DData.bTbl:
        case DData.oTbl: xRData = GetTableData(sKey).ToXml(); sTipo="Tbl"; break;
      //case DData.Bin: break;
        default:
          Dbg.Msg("WARNING: Tipo de dato no soportado para escritura XML, " + sKey ); break;
        }

      if( xRData!=null )
        return( "  <KEYWORD key=\"" + sKey +"\" type=\""+ sTipo +"\" >" +  xRData +  "</KEYWORD>\r\n" );

      Dbg.Msg("WARNING: no se pudieron leer los datos de la llave " + sKey );
      return "";
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea elmento XML principal, con información general del diccionario</summary>
    private string GetDictXmlRoot( string DName )
      {
      string sInc = "";
      if( Type==DDataType.Rule && Tbls.Defs!=null )
       {
       var xml = new StringBuilder( 100 * Tbls.Defs.Count );
       xml.Append( "<DEFINES>\r\n" );

       foreach( var pair in Tbls.Defs)
         xml.Append( "  <DEFINE const=\""+ pair.Key +"\" value=\""+ pair.Value +"\" />\r\n" );

       xml.Append( "</DEFINES>\r\n" );

       var IName = Path.ChangeExtension( DName, "xDef" );
       File.WriteAllText( IName,  xml.ToString() ); 

       sInc = " include=\""+ Path.GetFileName(IName) + "\"";
       }

      return "<DICTIONARY src=\""+ Src +"\" des=\""+ Des +"\" type=\""+ Type +"\""+ sInc +" >\r\n";
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un diccionario con la información del elemento raiz</summary>
    private static DictSingle CreateFromHrd( XElement root )
      {
      TLng src=TLng.En, des=TLng.Es;                              // Asume por defecto Englés-Español

      var sSrc = root.Attribute("src");                           // Obtiene atributo de idioma fuente
      if( sSrc != null )                                          // Si existe
        src = Utils.Code(sSrc.Value);                             // Obtiene el código del idioma
      else
        Dbg.Msg("WARNING: Se asumio idioma fuente 'En'");

      var sDes = root.Attribute("des");                           // Obtiene atributo de idioma destino
      if( sDes != null )                                          // Si existe
        des = Utils.Code(sDes.Value);                             // Obtiene código del idioma
      else
        Dbg.Msg("WARNING: Se asumio idioma destino 'Es'");

      DDataType type = type = DDataType.Str;                      // Asume que el diccionario es del tipo general
      var stype = root.Attribute("type");                         // Lee atributo de tipo de diccionario
      if( stype != null )                                         // Si existe
        {
        switch( stype.Value )                                     // Según el valor obtiene tipo
          {
          case "Rule": type = DDataType.Rule; break;
          case "Word": type = DDataType.Word; break;
          case "Str" : type = DDataType.Str; break;
          default: 
            Dbg.Msg("WARNING: El tipo de diccionario no es correcto");
            break;
          }
        }
      else
        Dbg.Msg("WARNING: Se asumio que el tipo de datos del diccionario como STR");

      var Dict  = new DictSingle( src, des, type );               // Crea diccionario con los paramentros determinados

      return Dict;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca si en el elemento raiz, hay un include, lo carga y procesa todas las entradas</summary>
    //------------------------------------------------------------------------------------------------------------------
    private void GetIncludeDefs(XElement root)
      {
      var xInc = root.Attribute("include");                       // Obtiene atributo de inclución de fichero
      if( xInc == null ) return;                                  // No existe el atributo, no hace nada

      var sInc  = xInc.Value;                                     // Obtiene le valor del atributo include
      var dPath = Path.GetDirectoryName( DicName) ;               // Obtiene el direcctorio del diccionario
      var FIncName = Path.Combine( dPath, sInc );                 // Forma camino del fichero a incluir

      try
        {
        var doc  = XDocument.Load( FIncName );                    // Carga el fichero de definiciones
        var elms = doc.Root.Elements("DEFINE");                   // Obtiene todos los elementos DEFINE

        if( elms.Count<XElement>() > 0 )                          // Si hay al menos un elemento
          Tbls.Defs = new Dictionary<string,string>();            // Crea el diccionario de definiciones

        foreach( var el in elms )                                 // Recorre todos los elementos de definiciones
          {
          var xConst = el.Attribute("const");                     // Obtiene nombre de la constante
          var xValue = el.Attribute("value");                     // Obtiene valor de la constante

          if( xConst==null || xValue==null )                      // Si falta valor o constante
            {
            Dbg.Msg("WARNING: defnición ignorada, por falta de atributo");
            continue;
            }

          var sConst = xConst.Value;                              // Obtiene valor de la constante
          if( Tbls.Defs.ContainsKey(sConst) )                     // Verifica que no este definida
            {
            Dbg.Msg("WARNING: definición de constante repetida " + sConst );
            continue;
            }

          Tbls.Defs[sConst] = xValue.Value;                       // Adicciona a tabla de definiciones
          }

        Dbg.Msg("INFO: El numero de constantes adicionadas fueron: " + Tbls.Defs.Count );
        } 
      catch (Exception e )                                        // Hubo un problema imprevisto
        {
        Dbg.Msg("No se cargaron las definiciones de constantes, " + e.Message, Dbg.Error );
        }

      }

    //------------------------------------------------------------------------------------------------------------------
    }

#endif
  }
