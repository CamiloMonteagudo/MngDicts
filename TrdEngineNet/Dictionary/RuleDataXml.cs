using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TrdEngine.Dictionary
  {
#if XML_SUPPORT
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el manejo de los datos de reglas en formato XML</summary>
  public partial class RuleData
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto RuleData a partir de un elemento XML</summary>
    public static RuleData FromXml( XElement xData,  RulesTables rTbls )
      {
      var Matchs = xData.Elements("MATCH");
      if( Matchs==null  )
        {
        Dbg.Msg("No se encuentra ningún Match en los datos", Dbg.Error);
        return null;
        }
        
      var RData = new RuleData();
      foreach( var match in Matchs )
        {
        var RMatch = RuleMatch.FromXml( match, rTbls );
        if( RMatch==null ) return null;

        RData.Matchs.Add( RMatch );
        }

      return RData;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de los datos de la regla en formato xml</summary>
    public string ToXml()
      {
      string xml = "\r\n";
      foreach (var item in Matchs)
        xml += item.ToXml();

      xml += "  ";
      return xml;
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleData ++++++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Implementa el manejo de los match en formato XML</summary>
  public partial class RuleMatch
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto RuleMatch a partir de un elemento XML</summary>
    public static RuleMatch FromXml( XElement xMatch,  RulesTables rTbls  )
      {
      var RMatch = new RuleMatch();

      var xWrd = xMatch.Attribute("kwrd");                          // Obtiene número de palabra clave
      if( xWrd != null )                                            // Si existe
        {
        if( !int.TryParse( xWrd.Value, out RMatch.idxKey ))
          {
          Dbg.Msg("Indice a palabra clave incorrecto", Dbg.Error);
          return null;
          }
        }

      if( !RMatch.GetConditions( xMatch, rTbls ) )
        return null;

      if( !RMatch.GetActions( "ACTIONS", xMatch, rTbls ) )
        return null;
          
      if( !RMatch.GetActions( "PHRASE", xMatch, rTbls ) )
        return null;
    
      return RMatch;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la condición de un match a partir de un elemento XML</summary>
    private bool GetConditions(XElement xMatch, RulesTables rTbls )
      {
      var conds = xMatch.Elements("COND");
      if( conds==null  )
        {
        Dbg.Msg("No hay ninguna condición dentro del Match", Dbg.Error);
        return false;
        }
        
      int LastWrd = -1;
      foreach( var cond in conds )
        {
        int iWord;
        var xWrd = cond.Attribute("wrd");                           // Obtiene atributo de número de palabra
        if( xWrd != null )                                          // Si existe
          {
          if( !int.TryParse( xWrd.Value, out iWord ) || iWord>32 )
            {
            Dbg.Msg("Condición con número de palabra incorrecto", Dbg.Error);
            return false;
            }
          }
        else
          {
          Dbg.Msg("Condición sin número de palabra", Dbg.Error);
          return false;
          }

        var sFun = cond.Value;
        int    i = 0;
        var FName = sFun.GetName( ref i );
        if( FName==null ) 
          return Dbg.Msg("No se pudo obtener el nombre de la condición", Dbg.Error);

        string Args = sFun.GetBetween( ref i, '(', ')' );
        if( Args==null ) 
          return Dbg.Msg("Los parentisis de los argumentos no estan balanceados", Dbg.Error);

        Args = Args.Expand( rTbls.Defs, rTbls.expand );

        var Fun = CreateFuntion( FName, Args, rTbls ); 
        if( Fun==null )
          return Dbg.Msg( "Creando la función '" + FName + '(' + Args + ')', Dbg.Error );

        Conds.Add( new MatchCond(iWord, Fun) );
        if( iWord==LastWrd ) ++nAndOp; else iWord=LastWrd;
        }

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene todas las acciones contenidas dentro del grupo 'GrpName' desde un elemento XML</summary>
    private bool GetActions( string GrpName, XElement xMatch, RulesTables rTbls)
      {
      List<MatchAction> Acts;

      var xGrp = xMatch.Element(GrpName);                                   // Obtiene elemento que representa al grupo
      if( xGrp == null ) return true;                                       // No existe el grupo, (es permitido)

           if( GrpName=="ACTIONS" ) Acts = Actions;                         // Pone actual el arreglo de acciones
      else if( GrpName=="PHRASE"  ) Acts = Phrases;                         // Pone actual el arreglo de frases
      else return Dbg.Msg("Se esperaba ACTIONS o PHRASE");                  // Grupo incorrecto

      var xActs = xGrp.Elements("ACTION");                                  // Filtra las acciones que hay dentro del grupo
      if( xActs==null  )
        return Dbg.Msg("No hay ninguna acción dentro de " + GrpName, Dbg.Error);
        
      foreach( var xAct in xActs )                                          // Recorre todas las acciones
        {
        int iWord;                                                          // Número de la palabra
        var xWrd = xAct.Attribute("wrd");                                   // Obtiene atrbuto de # de palabra
        if( xWrd == null )                                                  // No encontro atributo
          return Dbg.Msg("No atributo de # de palabra", Dbg.Error );

        if( !int.TryParse( xWrd.Value, out iWord ) && iWord<32 )            // Obtiene el número de palabra
          return Dbg.Msg("Valor de # de palabra incorrecto", Dbg.Error );

        foreach( var xAttr in xAct.Attributes() )                           // Recorre todos los atributos
          {
          var sName = xAttr.Name.ToString();                                // Obtiene nombre del atributo
          if( sName=="wrd" ) continue;                                      // Salta atributo de # de palabra

          var sVal = xAttr.Value;                                           // Obtiene valor del atributo
          if( sVal.StartsWith("wrd") )                                      // Es un numero de palabra
            sVal = sVal.Substring(3);                                       // Coje solo el número
          else if( sName=="GRAMTYPE" )                                      // Si la acción es tipo gramatical
            sVal = sVal.Expand( rTbls.Defs, rTbls.expand );                 // Trata de expandir las constantes

          if( sVal==null )
            return Dbg.Msg( "Valor de la accion incorrecto " + xAttr, Dbg.Error );

          var Fun = CreateAction( sName, sVal, rTbls );                     // Crea función con nombre y valor
          if( Fun==null ) 
            return Dbg.Msg( "Creando la accion '" + sName + '=' + sVal, Dbg.Error );

          Acts.Add( new MatchAction(iWord, Fun) );                          // Adiciona una acción al grupo
          }
        }

      return true;                                                          // Llego al final, todo OK
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de Match en formato xml</summary>
    public string ToXml()
      {
      var xml = new StringBuilder();
      xml.Append("    <MATCH kwrd=\"" + idxKey + "\" >\r\n");

      foreach( var cond in Conds )
        xml.Append( cond.ToXml() );
      
      if( Actions.Count>0 )
        AtionsToXml( "ACTIONS", Actions, xml );

      if( Phrases.Count>0 )
        AtionsToXml( "PHRASE", Phrases, xml );

      xml.Append("    </MATCH>\r\n");
      return xml.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de Match en formato xml</summary>
    public override string ToString()
      {
      return ToXml();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una representación de un grupo de acciones en formato xml</summary>
    public void AtionsToXml( string GrpName, List<MatchAction> Lst, StringBuilder xml )
      {
      int iWrd = -1;

      xml.Append( "      <" + GrpName + ">\r\n" );
      for( int i=0; i<Lst.Count; ++i )
        {
        var act = Lst[i];

        if( iWrd != act.iWord )
          {
          if( i>0 )
            xml.Append( "/>\r\n" );

          xml.Append( "        <ACTION wrd=\"" + act.iWord + "\" " );

          iWrd = act.iWord;
          }

        xml.Append( act.ToString().XmlEncode() + ' ' );
        }

      xml.Append( "/>\r\n      </" + GrpName + ">\r\n" );
      }
    } // +++++++++++++++++++++++++++++++++++++++++ FIN DE RuleMatch +++++++++++++++++++++++++++++++++++++++++++++++++++++

#endif
  }   // ++++++++++++++++++++++++++++++++++++++++ FIN DEL namespace +++++++++++++++++++++++++++++++++++++++++++++++++++++
