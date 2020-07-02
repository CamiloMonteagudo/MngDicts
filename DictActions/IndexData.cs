using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
  {
  ///===================================================================================================================================
  /// <summary> Maneja los datos que se guardan en los indices de cada palabra </summary>
  public class IndexData
    {
    public List< WrdIdx > Entries = new List< WrdIdx >();

    static char[] sepWrd = { ViewData.IdxSep };
    static char[] sepVal = { ViewData.ValSep };

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de los indices para una palabra, con el formato de tamaño fijo de 6 caracteres hexagecimal </summary>
    public static IndexData FromStrHex( string sData )
      {
      var idxDat = new IndexData();

      int len = sData.Length;
      int ini = 0;

      for(;;)
        {
        if( ini+6>len ) break;
        
        var sIdxOra = sData.Substring( ini, 5 );
        var cWrdPos = sData[ini+5];

        idxDat.Entries.Add( WrdIdx.FromValHex( sIdxOra, cWrdPos) );

        ini += 6;

        while( ini<len && sData[ini]=='\'' )
          {
          ++ini;
          cWrdPos = sData[ini++];
          }
        }

      return idxDat;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de los indices para una palabra, con el formato de tamaño variable de datos desimales con separadores </summary>
    public static IndexData FromStrWithSep( string sData )
      {
      var idxDat = new IndexData();

      var InOras = sData.Split( sepWrd );

      foreach( var item in InOras )
        {
        var values = item.Split( sepVal );

        idxDat.Entries.Add( WrdIdx.FromValDec( values[0], values[1]) );
        }

      return idxDat;
      }
    }

  ///===================================================================================================================================
  /// <summary> Maneja los datos de cada ocurrecia de una palabra en una oracion o frase </summary>
  public class WrdIdx
    {
    public int idxEntry;
    public int WrdPos;

    public WrdIdx( int idxOra, int WrdPos )
      {
      this.idxEntry = idxOra;
      this.WrdPos = WrdPos;
      } 

    public static WrdIdx FromValHex( string sIdxOra, char cWrdPos )
      {
      return new WrdIdx( Convert.ToInt32(sIdxOra,16), cWrdPos-'0' ); 
      } 

    public static WrdIdx FromValDec( string sIdxOra, string cWrdPos )
      {
      return new WrdIdx( int.Parse(sIdxOra), int.Parse(cWrdPos) ); 
      } 

    }
  }
