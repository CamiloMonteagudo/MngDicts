using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace TrdEngine.Dictionary
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Guarda datos en forma de tabla</summary>
  public class Tabla
    {
    private string[,] Array;

    private List<string> NameFila = new List<string>();
    private List<string> NameCol  = new List<string>();

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Número de filas de la tabla</summary>
    public int nRow { get{return Array.GetLength(0);} }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Número de columnas de la tabla</summary>
    public int nCol { get{return Array.GetLength(1);} }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el contenido de una celda, dando la 'fila' y la columna 'col' </summary>
    public string Cell(int fila, int col)  { return Array[fila,col]; }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el indice de una fila, a partir de su nombre </summary>
    public int FilaIndex(string name) { return NameFila.IndexOf(name); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el indice de una columna, a partir de su nombre </summary>
    public int ColIndex(string name) { return NameCol.IndexOf(name); }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el nombre de una fila a partir de su indice </summary>
    public string FilaName(int idx) { return NameFila[idx]; }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el nombre de una columna a partir de su indice </summary>
    public string ColName(int idx) { return NameCol[idx]; }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto tabla desde un stream de bytes</summary>
    internal static Tabla FromStream( byte[] Bytes )
      {
      var Tbl = new Tabla();                                          // Crea una tabla vacia
      MemoryStream MStrm = new MemoryStream(Bytes);                   // Crea un stream en memoria
      var Rdr = new BinaryReader( MStrm, Encoding.UTF8 );             // Le asocia un lector (Reader) para leer datos binarios

      var nRows = (int)Rdr.ReadByte();                                // Lee cantidad el filas
      var nCols = (int)Rdr.ReadByte();                                // Lee cantidad de columnas

      Tbl.Array = new string[nRows,nCols];                            // Crea el arreglo adecuado para poner los datos
      for( int iRow=0; iRow<nRows; ++iRow )                           // Recorre todas las filas
        {
        for( int iCol=0; iCol<nCols; ++iCol )                         // Recorre todas las columnas
          {
          var n     = Rdr.ReadByte();                                 // Lee el tamaño de la cadena
          var bytes = Rdr.ReadBytes(n);                               // Lee los bytes que representan la cadena

          Tbl.Array[iRow,iCol]  = Encoding.UTF8.GetString(bytes);     // Covierte a cadena UTF8 y la agrega la tabla
          }
        }

      ReadNameList( Rdr, Tbl.NameFila );                              // Lee los nombres de las filas
      ReadNameList( Rdr, Tbl.NameCol  );                              // Lee los nombres de las columnas
      return Tbl;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene una lista de cadenas desde un stream y las coloca en lista 'lst' </summary>
    private static void ReadNameList( BinaryReader Rdr, List<string> lst )
      {
      var count = Rdr.ReadByte();                                     // Lee cantidad de cadenas en la lista

      for( int i=0; i<count; ++i )                                    // Lee cada uno de las cadenas
        {
        var n     = Rdr.ReadByte();                                   // Lee el tamaño de la cadena
        var bytes = Rdr.ReadBytes(n);                                 // Lee los bytes que representan la cadena

        lst.Add( Encoding.UTF8.GetString(bytes) );                    // Covierte a cadena UTF8 y la agrega a la lista
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna un stream de bytes con la representación del objeto Tabla </summary>
    internal byte[] ToStream()
      {
      var nRows = Array.GetLength(0);                                 // Obtiene el número de filas
      var nCols = Array.GetLength(1);                                 // Obtiene el número de columnas

      MemoryStream MStrm = new MemoryStream( 20*Array.Length );       // Crea un stream en memoria
      var Wtr = new BinaryWriter( MStrm );                            // Le asocia un Writer binario

      Wtr.Write( (byte)nRows );                                       // Escribe número de filas
      Wtr.Write( (byte)nCols );                                       // Escribe número de columnas

      for( int i=0; i<nRows; i++ )                                    // Recorre todas las filas
        {
        for( int j=0; j<nCols; j++ )                                  // Recorre todas las columnas
          {
          var s = Array[i,j];                                         // Obtiene la celda i,j
          if( s==null ) s = "";                                       // No tiene nada, toma cadena vacia

          var bytes = Encoding.UTF8.GetBytes(s);                      // Obtiene bytes que representa la celda en UTF8
          var nBytes = bytes.Length;                                  // Obteien el # de bytes
          if( nBytes>255 ) 
            {
            Dbg.Msg("WARNING: celda con más de 255 caracteres");
            nBytes=255;                                               // Si es mayor que 255 los recorta
            }

          Wtr.Write( (byte)nBytes );                                  // Escribe el tamaño de la cadena
          Wtr.Write( bytes, 0, nBytes );                              // Escribe el contenido de la cadena
          }
        }

      WriteNameList( NameFila, Wtr );                                 // Escribe nombres de las filas
      WriteNameList( NameCol,  Wtr );                                 // Escribe nombres de las columnas

      return MStrm.ToArray();                                         // Retorna arreglo de bytes
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Escribe la representación binaria de una lista de cadenas a un stream</summary>
    private static void WriteNameList( List<string> lst, BinaryWriter Wtr )
      {
      Wtr.Write( (byte)lst.Count );                                   // Escribe númer de elementos de la lista
      if( lst.Count == 0 ) return;                                    // Si la lista esta vacia, termina

      foreach( var item in lst )                                      // Recorre todos los item de la lista
        {                                                              
        var bytes = Encoding.UTF8.GetBytes( item );                   // Obtiene bytes que representa la cadena en UTF8
        Wtr.Write( (byte)bytes.Length );                              // Escribe tamaño de la cadena
        Wtr.Write( bytes );                                           // Escribe contenido de la cadena
        }
      }

#if XML_SUPPORT
    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena XML con la representación del objeto Tabla </summary>
    public string ToXml()
      {
      var nRows = Array.GetLength(0);                                                   // Obtiene número de filas
      var nCols = Array.GetLength(1);                                                   // Obtiene número de columnas

      var xml = new StringBuilder( 20*Array.Length );                                   // Reserva tamaño estimado
      xml.Append( "\r\n    <TABLE rows=\""+ nRows +"\" cols=\""+ nCols +"\">\r\n");     // Crea elemento principal

      for( int i=0; i<nRows; i++ )                                                      // Recorre todas las filas
        xml.Append( "      <ROW>" + GetRowStr( i, nCols ) + "</ROW>\r\n");              // Crea elemento fila

      if( NameFila.Count>0 )                                                            // Si existen nombres de filas
        xml.Append( "      <ROW_NAMES>" + ListToStr( NameFila ) + "</ROW_NAMES>\r\n");  // Crea elemento con nombre de filas

      if( NameCol.Count>0 )                                                             // Si existe nombres de columnas
        xml.Append( "      <COL_NAMES>" + ListToStr( NameCol ) + "</COL_NAMES>\r\n");   // Crea elementos con nombres de columnas

      xml.Append( "    </TABLE>\r\n  ");                                                // Cierra elemento principal

      return xml.ToString();                                                            // Convierte todo a cadena
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene objeto Tabla a partir de un elemento XML </summary>
    internal static Tabla FromXml(XElement el)
      {
      var TblName = GetStrAttr( "key", el );

      var xTable = el.Element("TABLE");                                 // La definición empieza a patir de elemento TABLE
      if( xTable==null )                                                // No existe hay una definición de tabla
        {
        var Msg = string.Format( "ERROR: La entrada '{0}' debe contener un elemento TABLE", TblName );
        Dbg.Msg(Msg);
        return null;
        }

      int nRows;
      if( !GetIntAttr( "rows", xTable, out nRows ) )                  // Obtiene # de filas desde el atributo
        Dbg.Msg( string.Format( "WARNIG: La tabla '{0}' no declara el número de filas", TblName ) );

      int nCols;
      if( !GetIntAttr( "cols", xTable, out nCols ) )                  // Obtiene # de columnas desde el atributo
        Dbg.Msg( string.Format( "WARNIG: La tabla '{0}' no declara el número de columnas", TblName ) );

      var xRows = xTable.Elements("ROW");                             // Obtiene todos los elementos ROW del XML
      int _Rows = xRows.Count();                                      // Determina la cantidad de filas reales

      if( _Rows != nRows )                                            // Si difiere la cantidad de filas
        {
        var Msg = string.Format( "WARNIG: La tabla '{0}' declara {1} filas y realmente tiene {2}", TblName, nRows, _Rows  );
        Dbg.Msg( Msg );
        nRows = _Rows;                                                // Toma el # de filas reales
        }

      if( nRows == 0 )                                                // Si no hay ninguna fila
        {
        Dbg.Msg( string.Format( "WARNIG: La tabla '{0}' esta vacia", TblName ) );

        var Tb = new Tabla();                                         // Se crea el objeto tabla
        Tb.Array = new string[0,0];                                   // Crea el arreglo vacio
        return Tb;                                                    // Retorna tabla vacia
        }

      if( nCols==0 )                                                  // Si no se especifico el # de columnas
        {
        nCols = xRows.First().Elements("C").Count() ;                 // Se toma el # de columnas de la primera fila
        Dbg.Msg( string.Format( "WARNIG: Se asumio que la tabla '{0}' tiene {1} columnas (Por la priemera fila)", TblName ) );
        }
 
      var Tbl = new Tabla();                                          // Se crea el objeto tabla
      Tbl.Array = new string[nRows,nCols];                            // Crea el arreglo adecuado para poner los datos

      int iRow = 0;                                                   // Empieza por la fila 0
      foreach(var xRow in xRows)                                      // Recorre todas las filas
        {                                                             
        int iCol = 0;                                                 // Pone marcador de columnas
        foreach(var xCol in xRow.Elements("C") )                      // Recorre todas las columnas (celdas)               
          {
          if( iCol<nCols )                                            // Si la columna actual es menor que el # de columnas
            Tbl.Array[ iRow, iCol] = xCol.Value;                      // Pone su valor en el arreglo
          ++iCol;                                                     // Incrementa indice de columna actual
          }

        if( iCol!=nCols )                                             // El # de columnas no coincide con el declarado
          {
          Dbg.Msg("WARNING: En la tabla '"+ TblName +"' la fila "+ iRow +" devia tener "+ nCols +" columnas y tiene "+ iCol );

          if( iCol<nCols )                                            // Si hay menos columnas de las declaradas
            while( iCol<nCols ) Tbl.Array[ iRow, iCol++] = "";        // Rellena restos de las celdas con cadena vacia
          }

        ++iRow;                                                       // Incrementa indice de fila actual
        }

      GetNamesFromXML( xTable, "ROW_NAMES", Tbl.NameFila, nRows );    // Obtiene los nombres de las filas
      GetNamesFromXML( xTable, "COL_NAMES", Tbl.NameCol , nCols );    // Obtiene los nombres de las columnas

      return Tbl;                                                     // Retorna la tabla
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una lista de nombres desde un elemnto XML </summary>
    private static void GetNamesFromXML( XElement el, string Name, List<string> lst, int nElm )
      {
      var xCNames = el.Element(Name);                                 // Obtiene elemento XML con lista de nombres
      if( xCNames == null ) return;                                   // El elemento no existe

      lst.Clear();                                                    // Limpia la lista
      foreach(var xCol in xCNames.Elements("C") )                     // Recorre los elementos (celdas)
        lst.Add( xCol.Value );                                        // Adiciona su valor

      if( lst.Count != nElm )                                         // Verifica si el # de elemento es el correcto
        Dbg.Msg("WARNIG: El # de nombres no coincide con el esperado");

      while( lst.Count < nElm ) lst.Add("");                          // Si hay menos elementos rellena con celdas vacias
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena XML con todos los item en 'lst' en etiquetas celdas </summary>
    private static string ListToStr( List<string> lst )
      {
      var sLst = new StringBuilder( 20*lst.Count );                   // Reserva memoria estimada para la cadena
      for( int i=0; i<lst.Count; i++ )                                // Recorre todos los elementos de la lista
        {
        sLst.Append("<C>");                                           // Agrega etiqueta inicio de celda
        sLst.Append( lst[i] );                                        // Agrega contenido del elemento
        sLst.Append("</C>");                                          // Agrega etiqueta final de celda
        }

      return sLst.ToString();                                         // Retorna la cadena
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena XML con todas las celdas de la fila 'iRow'</summary>
    private string GetRowStr( int iRow, int nCols )
      {
      var sRow = new StringBuilder( 20*nCols );                       // Reserva memoria estimada para la cadena

      for( int j=0; j<nCols; j++ )                                    // Recorre todas las columnas de la fila
        {
        sRow.Append("<C>");                                           // Agrega etiqueta inicio de celda
        sRow.Append( Array[iRow,j] );                                 // Agrega contenido de la columna
        sRow.Append("</C>");                                          // Agrega etiqueta final de celda
        }

      return sRow.ToString();                                         // Retorna la cadena
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el valor de un atributo entero de un elemento XML </summary>
    private static bool GetIntAttr( string AttrName, XElement el, out int Num )
      {
      Num = 0;
      var xNum = el.Attribute( AttrName );                            // Obtiene atributo con nombre solicitado
      if( xNum == null ) return false;                                // Si no existe, retorna falso

      if( !int.TryParse( xNum.Value, out Num ) )                      // Trata de convertir el atributo a entero
        return false;                                                 // No puede, retorna falso

      return true;                                                    // Retorna verdadero  y atributo entero
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene el valor de un atributo entero de un elemento XML </summary>
    private static string GetStrAttr( string AttrName, XElement el )
      {
      var xNum = el.Attribute( AttrName );                            // Obtiene atributo con nombre solicitado
      if( xNum == null ) return "";                                // Si no existe, retorna falso

      return xNum.Value;                                             // Retorna verdadero  y atributo entero
      }

#endif
    ///-----------------------------------------------------------------------------------------------------------------------------------
    }
  }
