using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase para almacenar los datos de una expresión regular</summary>
  class RegExData
    {
    ///<summary>Expresión regular ya compilada</summary>
    public Regex ExpRe;
    ///<summary>Cadena de sustitución</summary>
    public string Sust;
    ///<summary>Indice a retornar</summary>
    public int Idx;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea al objeto con las 3 cadenas del diccionario</summary>
    public RegExData( string sRegExp, string sust, string sIndex="0" )
      {
      ExpRe = new Regex( sRegExp, RegexOptions.CultureInvariant );
      Sust  = sust;

      int.TryParse( sIndex, out Idx);
      }
    }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Clase para manejar un arreglo de expresiones regulares</summary>
  public class RegexArray
    {
    List<RegExData> Lst = new List<RegExData>();

    bool replace;         // Sustituye el pantron casado por la expresión de sustitución
    bool retIndex;        // Retorna el indice especificado en lugar del orden de la cadena

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga del diccionario de datos del idioma, el arreglo de expresiones regulares 'name'</summary>
    public bool Load( TLng lang, string name, bool replace, bool retIndex )
      {
      this.replace  = replace;
      this.retIndex = retIndex;

      try
        {
        var LngData = TrdData.GetLangData(lang);
        var ReTable = LngData.GetLangTable(name);

        for( int i=0; i<ReTable.nRow; ++i )
          {
          var sRe  = ReTable.Cell(i,0);
          var Sust = ReTable.Cell(i,1);
          var Idx  = (ReTable.nCol==3)? ReTable.Cell(i,2) : "0";

          Lst.Add( new RegExData( sRe, Sust, Idx ) );
          }

        return true;
        } catch{}

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Aplica todas las expesiones regulares de la lista sobre la cadena str</summary>
    public string Replace( string str )
      {
      if( !replace ) return str;

      foreach( var ReDat in Lst )
        str = ReDat.ExpRe.Replace( str, ReDat.Sust );

      return str;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca la primera expresión regular que machea con 's', retorna el indice y en 'len' tamaño de la cadena macheada</summary>
    public int Match( string s, ref int len )
      {
      for( int i=0; i< Lst.Count ; i++ )
        {
        var ReDat = Lst[i];
        var match = ReDat.ExpRe.Match(s);
        if( match.Success  )
          {
          len = match.Length;
          if( retIndex ) return ReDat.Idx;
          else           return i;
          }
        }

      len = 0;
      return -1;
      }

    } //++++++++++++++++++++++++++++++++++++++++++++ Fin de la Clase RegexArray +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  }   //++++++++++++++++++++++++++++++++++++++++++++ Fin del namespace           +++++++++++++++++++++++++++++++++++++++++++++++++++++++++
