using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TrdEngine.Dictionary
  {
#if XML_SUPPORT
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa el soporte XML para los datos de traducción de una palabra</summary>
  //------------------------------------------------------------------------------------------------------------------
  public partial class WordData
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación de los datos de la palabra en formato XML</summary>
    public string ToXml()
      {
      string sInfo = (string.IsNullOrEmpty(M)? "" : " Info=\""+ M +"\"") ;  // Obtiene información adicional

      var xml = new StringBuilder( 80*MeanSets.Count );                     // Reserva tamaño estimado

      xml.Append( "\r\n    <WORDDATA type=\""+ cType +"\""+ sInfo +">\r\n");    // Crea elemento principal

      for( int i=0; i<MeanSets.Count; ++i )                                 // Recorre todos los MeanSet
        {
        var MSet = MeanSets[i];                                             // Obtiene MeanSet actual    

        if( MSet.nSkip == 1 )                                               // No salto en orden de ejecución
          MSet.ToXml( xml, ""  );                                           // Adicciona la representación de cada MeanSet
        else                                                                // Hay un salto
          {
          string sConds = "cond='"+ MSet.Conds.ToString() +"'";
 
          xml.Append( "      <GROUP "+ sConds + ">\r\n");                   // Adicciona inicio de grupo

          var iEnd = i + 1 + MSet.nSkip;                                    // Cálcula elemento final del grupo
          for( int j=i+1; j<iEnd; ++j )                                     // Recorre todos los MeanSet del grupo
            MeanSets[j].ToXml( xml, "  " );                                 // Adicciona la representación 

          i = i + MSet.nSkip;
          xml.Append( "      </GROUP>\r\n");                                // Elemento que cierra el grupo
          }
        }

      xml.Append( "    </WORDDATA>\r\n  ");                                 // Cierra elemento principal

      return xml.ToString();                                                // Convierte todo a cadena
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto WordData a partir de un elemento XML que contiene los datos</summary>
    public static WordData FromXml( XElement el )
      {
      var xWData = el.Element("WORDDATA");                                  // Busca inicio de definicion de datos de palabra
      if( xWData==null )                                                    // No existe una definición de datos de palabra
        {
        Dbg.Msg("ERROR: Los datos deben contener con un elemento WORDDATA");
        return null;
        }

      var xType = xWData.Attribute("type");                                 // Obtiene atributo de tipo asociado a la palabra
      if( xType == null )                                                   // No contiene atibuto de tipo
        {
        Dbg.Msg("ERROR: no aparece el atributo 'type' en la palabra");
        return null;                                
        }

      var WData = new WordData();                                           // Crea objeto datos de palabra
      WData.cType = xType.Value;                                            // La asocia el tipo gramatical

      var xInfo = xWData.Attribute("Info");                                 // Obtiene atributo de información adicional
      if( xInfo != null )                                                   // Existe información adicional
        WData.M =  xInfo.Value;                                             // Le asigna valor al objeto

      foreach( var elem in xWData.Elements() )                              // Recorre todos los elementos dentro datos de palabra
        {
        if( elem.Name == "MEANSET" )                                        // Es un grupo de significados
          {
          MeanSet MSet = MeanSet.FromXml( elem );                           // Obtiene el objeto que los representa
          if( MSet==null ) return null;                                     // Hubo un error al obtener el grupo de significados

          WData.MeanSets.Add( MSet );                                       // Adiciona grupos de significados a la palabra
          continue;
          }

        if( elem.Name == "GROUP" )                                          // Es un elemento de agrupación de grupo de significados
          {
          var ret = MeanSet.GrpFromXml( elem, WData.MeanSets );             // Obtiene todo los grupos de significados dentro del grupo
          if( !ret ) return null;                                           // Hubo un error al obtener grupo de significados
          }
        }

      return WData;                                                         // Retorna objeto con datos de la palabra
      }

    //------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    WordType                                     *********************************/
///<summary>Almacena y procesa las condiciones asociadas a un grupo de significados</summary>
//----------------------------------------------------------------------------------------------------------------------------------
  public partial class Conditions
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa las condiciones</summary>
    public override string ToString()
      {
      if( Funcs.Count==1 )                                            // Si hay una sola función                              
        return Funcs[0].ToString();                                   // Retorna representación de la función

      string[] pila = new string[10];                                   
      int    ptr  = -1;                                             

      char[] sep = {'|', '&'}; 
      foreach( var Id in Orden)                                     
        {
        if( Id < opNone )                                           
          pila[++ptr] = Funcs[Id].ToString();
        else 
          {
          var Op2 = pila[ptr];
          if( pila[ptr].IndexOfAny(sep)!=-1 )
            Op2 = '(' +pila[ptr] + ')';

          if( Id==opOr && ptr>=1 )                               
            {
            pila[ptr-1] = ( pila[ptr-1] + " OR " + Op2 );                
            --ptr;                                                    
            }
          else if( Id==opAnd && ptr>=1  )                             
            {
            pila[ptr-1] = ( pila[ptr-1] + " AND " + Op2 );                
            --ptr;                                                    
            }
          }
        }

      return pila[0];                                               // Retorna resultado final
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    MeanSet                                      *********************************/
///<summary>Almacena la información correspondiente a un grupo de significados</summary>
  public partial class MeanSet  
    {
    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación del MeanSet en formato XML</summary>
    public void ToXml(StringBuilder xml, string sTab )
      {
      string sConds = "";
      if( Conds!=null )
        sConds = " cond='"+ Conds.ToString() +"'";
 
      xml.Append( sTab + "      <MEANSET"+ sConds + ">\r\n");

      foreach( var Mean in Means)
        Mean.ToXml( xml, sTab );

      foreach( var Cmd in Cmds)
        xml.Append( sTab + "        <COMAND>"+ Cmd + "</COMAND>\r\n");
  
      xml.Append( sTab + "      </MEANSET>\r\n");
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación de un grupo de significados desde un elemento XML</summary>
    public static MeanSet FromXml(XElement xMSet)
      {
      Conditions Conds = null;                                              // Por defecto, asume que no hay condiciones

      var xConds = xMSet.Attribute("cond");                                 // Obtiene atributo con la condición 
      if( xConds != null )                                                  // Se pudo obtener el atributo 
        {
        Conds = GetCondition( xConds.Value );                               // Crea objeto con los datos del atributo
        if( Conds==null ) return null;                                      // No se pudo obtener la condición
        }

      var MSet = new MeanSet( Conds );                                      // Crea significado vacio con la condición     

      foreach( var elem in xMSet.Elements() )                               // Recorre todos los elementos dentros de MEANSET
        {
        if( elem.Name == "MEAN" )                                           // El elemento es un significado
          {
          WordMean Mean = WordMean.FromXml( elem );                         // Crea un objeto significado a partir del elemento  
          if( Mean==null ) return null;                                     

          MSet.Means.Add( Mean );                                           // Adiciona el objeto significado
          continue;
          }

        if( elem.Name == "COMAND" )                                         // Si el elemento es un comando 
          {
          int Idx = 0;
          var Cmd = CmdFormStr( elem.Value, ref Idx );                      // Obtiene datos del comando desde el elemento
          if( Cmd == null ) return null;                                    // No lo pudo obtener, el comando

          MSet.Cmds.Add( Cmd );                                             // Adicciona el comando a la lista
          }
        }

      return MSet;                                                         
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un grupo de MeanSet desde un elemento XML</summary>
    public static bool GrpFromXml(XElement xGrp, List<MeanSet> MeanSets)
      {
      var xConds = xGrp.Attribute("cond");                                  // Obtiene atributo con condiciones    
      if( xConds == null )                                                  // El atributo no se encuentra
        return Dbg.Msg( "ERROR: falta la condición en le etiqueta GROUP");

      var Conds = GetCondition( xConds.Value );                             // Crea objeto con los datos de la condición
      if( Conds==null ) return false;                                       // No se pudo obtener la condición

      var MSet = new MeanSet( Conds );                                      // Crea MeanSet vacio

      var Elems = xGrp.Elements("MEANSET");                                 // Obtiene todos los MeanSet dentro del grupo

      MSet.nSkip = Elems.Count();                                           // Obtiene cantidad de elementos
      if( MSet.nSkip< 2 )                                                   // La canidad de elementos es incorrecta
        return Dbg.Msg( "ERROR: un grupo debe tener el menos 2 elementos");

      MeanSets.Add( MSet );                                                 // Adiciona MeanSet con salto a la lista
      foreach( var elem in Elems )                                          // Recorre todos MeanSet dentro del grupo
        {
        MSet = MeanSet.FromXml( elem );                                     // Obtiene el objeto que los representa
        if( MSet==null ) return false;                                      // Hubo un error al obtener el grupo de significados

        MeanSets.Add( MSet );                                               // Adiciona grupos de significados a la palabra
        }

      return true;                                                          // Todo OK
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto condición desde una cadena de un documento XML</summary>
    public static Conditions GetCondition( string sCond )
      {
      var s = new StringBuilder( sCond );
      s.Replace( " OR " , "|" );                                            // Sustituye OR para compatibilidad con cmd
      s.Replace( " AND ", "&" );                                            // Sustituye AND para compatibilidad con cmd
                                                             
      var Conds = new Conditions();                                         // Crea objeto condiciones
      if( !Conds.CondFromStr( s.ToString() ) )                              // Obtiene datos desde la cadena sCond
        return null;                                                        // Hubo error

      return Conds;                                                         // Retorna OK
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    WordMean                                     *********************************/
///<summary>Almacena la información correspondiente a un significado de una palabra</summary>
  public partial class WordMean  
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación del significado en formato XML</summary>
    public void ToXml(StringBuilder xml, string sTab)
      {
      string sAttr = "";

      if( esp!= "GG"         ) sAttr += " esp=\"" + esp + '"' ;
      if( gen==TGen.Femen  ) sAttr += " gen=\"f\"" ;
      if( gen==TGen.Neutro ) sAttr += " gen=\"n\"" ;
      if( plur               ) sAttr += " num=\"p\"" ;
      if( refl               ) sAttr += " ref=\"true\"" ;

      string sInfo = (info!="")? "["+ info +"] ":"";

      xml.Append( sTab + "        <MEAN"+ sAttr +">"+ sInfo + mean.XmlEncode() + "</MEAN>\r\n");
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto WordMean a partir de un elemento XML</summary>
    public static WordMean FromXml(XElement xMean)
      {
      var Mean = new WordMean();                                            // Crea objeto significado vacio

      var xEsp = xMean.Attribute("esp");                                    // Obtiene atributo de especialidad
      if( xEsp != null )  
        {
        if( xEsp.Value.Length != 2 )                                        // Comprueba que el valor sea aceptable
          {
          Dbg.Msg( "ERROR: el código de la especialidad debe ser de 2 caracteres" );
          return null;
          }

        Mean.Esp = xEsp.Value;                                              // Toma el valor de la especialidad
        }

      var xGen = xMean.Attribute("gen");                                    // Obtiene atributo de genero
      if( xGen != null )                                                    // Si existe
        {
        if( xGen.Value=="f" ) Mean.Gen = TGen.Femen;                      // Si el valor es femenino, lo toma
        if( xGen.Value=="n" ) Mean.Gen = TGen.Neutro;                     // Si el valor es masculino, lo toma
        }

      var xNum = xMean.Attribute("num");                                    // Obtiene atributo de número
      Mean.Plur = ( xNum != null && xNum.Value=="p" );                      // Si existe y es plural, lo toma

      var xRef  = xMean.Attribute("ref");                                   // Obtiene atributo de reflexivo
      Mean.Refl = ( xRef != null && xRef.Value=="true" );                   // Si existe y es verdadero, lo toma

      var val = xMean.Value;                                                // Obtiene el contenido del elemento
      if( val.Length>0 && val[0] == '[' )                                   // Si comienza con corchete
        {
        int idx = 0;
        Mean.Info = val.GetBetween( ref idx, '[', ']' );                    // Toma contenido del corchete como info
        Mean.Mean = val.Substring( idx );                                   // El resto lo toma como significado
        }
      else
        Mean.Mean = val;                                                    // Toma todo como significado

      return Mean;                                                          // Retorna el objeto
      }

    //------------------------------------------------------------------------------------------------------------------
    }
#endif
  }
