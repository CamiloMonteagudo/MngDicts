using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using TrdEngine;

namespace TrdEngine.Dictionary
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Tipos de datos que puede contener un diccionario</summary>
  //------------------------------------------------------------------------------------------------------------------
  public enum DData:byte
    {              
    bStr = 0x00,  // Cadena de caracteres en forma binaria
    bWrd = 0x01,  // Datos de palabras en forma binaria
    bRgl = 0x02,  // Datos de reglas en forma binaria
    Bin  = 0x03,  // Datos binarias manejados por la aplicación
    bTbl = 0x04,  // Datos de tabla en forma binaria
    oStr = 0x80,  // Cadena de caracteres en forma de objeto
    oWrd = 0x81,  // Datos de palabras en forma de objeto
    oRgl = 0x82,  // Datos de reglas en forma de objeto
    oTbl = 0x84,  // Datos de tablas en forma de objeto
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Define y maneja un diccionario simple</summary>
  //------------------------------------------------------------------------------------------------------------------
  public partial class DictSingle : IDictHeader, IDictTrd, IEnumerable<string>
    {
    //------------------------------------------------------------------------------------------------------------------
    // Atributos del diccionario
    //------------------------------------------------------------------------------------------------------------------
    TLng      src     = TLng.NA;                               // Código del idioma de las llaves
    TLng      des     = TLng.NA;                               // Código del idioma de los datos
    DDataType type    = 0;                                     // Tipo de diccionario
    DFlag     flags   = 0;                                     // Banderas que definen los atributos personalizados del diccionario
    bool      changed = false;                                 // Indica que hubo cambios en el diccionario

    ///<summary>Cantidad de registros del diccionario</summary>
    public int    Count {get{return Data.Count;}}
    ///<summary>Camino al archivo que contiene el diccionario</summary>
    public string DicName {get;set;}
    ///<summary>Idioma para las llaves del diccionario</summary>
    public TLng   Src   {get{return src;  }}
    ///<summary>Idioma para los datos del diccionario</summary>
    public TLng   Des   {get{return des;  }}
    ///<summary>Clasificacion del diccionario según los datos que contiene</summary>
    public DDataType Type  {get{return type; }}
    ///<summary>Banderas personalizadas para el diccionario</summary>
    public DFlag Flags { get{return flags;} set{flags=value; }}
    ///<summary>Consulta/resetea el codigo de ultimo error producido</summary>
    public int Error { get{return 0;} set{} }
    ///<summary>Establece/Consulta si al contenido del diccionario cambio</summary>
    public bool Modified { get{ return changed; }  }

    ///<summary>Diccionario donde se almacenan las llaves y sus datos asociados</summary>
    private Dictionary<string,DicData> Data = new Dictionary<string,DicData>();
    ///<summary>Tablas de datos para los diccionarios de reglas</summary>
    private RulesTables Tbls = null;

    private List<string> SortKeys = null;

    //------------------------------------------------------------------------------------------------------------------
    //************************************************ Constructor ****************************************************/
    ///<summary>Crea un objeto DictSingle vacio</summary>
    ///
    ///<param name="Src" >Idioma que tendran las llaves</param>
    ///<param name="Des" >Idioma que tendran los datos</param>
    ///<param name="Tipo">Tipo de diccionario</param>
    //------------------------------------------------------------------------------------------------------------------
    public DictSingle( TLng Src, TLng Des, DDataType Tipo )
      {
      this.src  = Src;                                // Código del idioma de las llaves
      this.des  = Des;                                // Código del idioma de los datos
      this.type = Tipo;                               // Tipo de diccionario

      if( Tipo == DDataType.Rule )                    // Si es un diccionario de reglas
        Tbls = new RulesTables();                     // Inicializa las tablas de cadenas y datos compartidos
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre un diccionario simple desde un fichero de diccionario</summary>
    ///
    ///<param name="Path">Camino donde esta el archivo que representa el diccionario</param>
    ///
    ///<remarks>Si el diccionario no se puede abrir se retorna null</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public static DictSingle Open( string Path, DOpen Options = DOpen.None )
      {
      try
        {
        var stm   = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);   // Abre el fichero

        int count = (int)stm.Length;                              // Obtiene tamaño del fichero
        var mem = new MemoryStream(count);                        // Crea un strean en memoria del tamaño del fichero
                                                                  
        stm.CopyTo( mem, count );                                 // Copia todo el contenido del fichero a memoria
        stm.Close();                                              // Cierra el fichero
                                                                  
        mem.Position = 0;                                         // Pone puntero de memoria al inicio
        var Rdr = new BinaryReader(mem, Encoding.UTF8);           // Crea un lector binario, para leer la memoria
                                                                  
        var ID = Rdr.ReadString();                                // Lee el identificador del diccionario
        if( ID != "DictBinVer1.0" ) return null;                  // Identificador incorrecto
                                                                  
        TLng  src  = (TLng )     Rdr.ReadByte();                  // Lee código del idioma de las llaves
        TLng  des  = (TLng )     Rdr.ReadByte();                  // Lee código del idioma de los datos
        var   type = (DDataType) Rdr.ReadByte();                  // Lee tipo de diccionario
                                                                  
        var Dict = new DictSingle( src, des, type );              // Crea un objeto diccionario
                                                                  
        var flgs = (DFlag)Rdr.ReadUInt16();                       // Lee banderas del diccionario 
        if( (Options & DOpen.NoSort) != 0 )                       // Si se especifica que no se ordenen las llaves
          flgs &= (~DFlag.Sort);                                  // Quita el bit de ordenar de las banderas

        Dict.Flags   = flgs;                                      // Establece banderas del diccionario 
        Dict.DicName = Path;                                      // Establece nombre de fichero

        bool sort = false;                                        // Determina si se usa la lista de llaves ordenadas o no
        if( (flgs & DFlag.Sort)!=0 )                              // Si tiene bandera para usar llaves ordenadas
          {
          Dict.SortKeys = new List<string>();                     // Crea lista vacia de llaves ordenadas
          sort = true;                                            // Pone bandera a true
          }

        for(;;)                                                   // Lee todos los datos del diccionario
          {                                                       
          var key = Rdr.ReadString();                             // Lee la llave
          var dat = DicData.FromSteam(Rdr);                       // Lee los datos  

          Dict.Data.Add( key, dat );                              // Adiciona el par llave/datos al objeto data
          if( sort )  Dict.SortKeys.Add( key );                   // Adiciona la llave a la lista de palabras
                                                                  
          if( mem.Position == mem.Length ) break;                 // Si llego el final, termina el ciclo
          }                                                       
                                                                  
        mem.Close();                                              // Cierra el stream de memoria

        if( type == DDataType.Rule )                              // Si es un diccionario de reglas
          Dict.LoadRulesTables();                                 // Carga las tablas para las reglas

        return Dict;                                              // Retorna el diccionario
        }                                                         
      catch { return null; }                                      // Hubo un error, retorna null
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga tablas de cadenas y datos compartidos para las reglas</summary>
    private void LoadRulesTables()
      {
      Tbls = new RulesTables();                                   // Crea tabla de datos

      var lstStrs = GetStrData( "__lstStrs__" );                  // Obtiene llave con datos de las cadenas
      if( lstStrs==null ) 
        Dbg.Msg("No se encontro la tabla de cadenas", Dbg.Error);
      else
        Del( "__lstStrs__" );                                     // Borra la llave

      Tbls.CreateStringTable( lstStrs );                          // Crea tabla de cadenas

      var lstRDatas = GetBinData( "__lstRDatas__" );              // Obtiene llave con datos de duplicados
      if( lstRDatas==null ) 
        Dbg.Msg("No se encontro la tabla de datos de reglas compartidas", Dbg.Error );
      else
        Del( "__lstRDatas__" );                                   // Borra la llave

      Tbls.CreateRuleDataTable( lstRDatas );                      // Crea la tabla de datos duplicados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Guarda tablas de cadenas y datos compartidos para las reglas</summary>
    private void SaveRulesTables( BinaryWriter Wtr)
      {
      if( Tbls == null ) return;

      Wtr.Write( "__lstStrs__" );                                   // Escribe llave con todas las cadenas
      var Strs = new DicData( Tbls.GetStringTableData() );          // Obtiene datos de las cadenas
      Strs.ToStream( Wtr );                                         // Escribe los datos

      Wtr.Write(  "__lstRDatas__");                                 // Llave con todos los datos
      var Dats = new DicData( Tbls.GetRuleDataTable() );            // Obtiene stream con todos los datos
      Dats.ToStream( Wtr );                                         // Los escribe a fichero
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Guarda el contenido del diccionario hacia un fichero</summary>
    ///
    ///<param name="dicPath">Localización de se quiera guardar el diccionario, si es null usa el nombre que se uso para abrirlo</param>
    ///<param name="force"  >Fuerza a que el diccionario sea guardado, aun cuando no haya sido modificado</param>
    ///
    ///<remarks>Retorna false, si existio algún problema al guardar el diccionario</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public bool Save( string dicPath=null, bool force=false )
      {
      if( !changed && !force ) return true;                       // Si no hay nada que guardar (y no se fuerza a guardar)
      var fName = (dicPath!=null)? dicPath : DicName;             // Si no se especifica el camino coje el original

      try
        {
        var stm  = File.Create(fName);                            // Crea el fichero
        var Wtr = new BinaryWriter(stm, Encoding.UTF8 );          // Le asocia un escritor binatio

        flags |= DFlag.Sort;                                      // Save simpre guarda las llaves ordenadas

        Wtr.Write( "DictBinVer1.0" );                             // Escribe identificador del diccionario
        Wtr.Write( (byte)src       );                             // Escribe idioma para las llaves
        Wtr.Write( (byte)des       );                             // Escribe idioma para los datos
        Wtr.Write( (byte)Type      );                             // Escribe tipo de diccionario
        Wtr.Write( (UInt16)flags   );                             // Escribe banderas personalizadas

        if( SortKeys==null ) GetSortedKeys();                     // Si no hay lista de palabras ordenada la crea

        for( int i=0; i<SortKeys.Count; ++i )                     // Recorre la lista de palabras
          {
          var sKey = SortKeys[i];
          DicData Dat = null;                                     // Por defecto pone los datos nulos
          if( Data.TryGetValue( sKey, out Dat) )                  // Trata de obtener valor asociado a la llave 
            {
            Wtr.Write( sKey );                                    // Escribe la llave
            Dat.ToStream( Wtr );                                  // Escriba los datos
            }
          else                                                    // Hubo un error
            Dbg.Msg("WARNIG la llave '" + sKey +"' no pudo ser guardada, porque esta en la lista de palabras y no en los datos");
          }

        if( type == DDataType.Rule )                              // Si es un diccionario de reglas
          SaveRulesTables( Wtr );                                 // Guarda las tablas de cadenas y datos compartidos

        stm.Close();                                              // Cierra el fichero
        changed = false;                                          // Marca que ya los cambios fueron guardados
        return true;                                              // Retorna OK
        }
      catch { return false; }                                     // Hubo un error, retorna falso
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona la llave 'sKey' a la lista de llaves ordenadas</summary>
    private void AddSortKey( String sKey )
      {
      if( SortKeys==null ) return;                              // Si hay lista de llaves ordenadas

      var idx = SortKeys.BinarySearch( sKey, StringComparer.OrdinalIgnoreCase );  // Busca indice en la lista
      if( idx<0 )                                               // No la encontro
        SortKeys.Insert( ~idx, sKey );                          // La adiciona en la posición correcta
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona al diccionario, una llave y le asocia una cadena de caracteres</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="sData">Cadena de caracteres asociados a la llave</param>
    public void AddKey(String sKey, String sData)
      {
      AddSortKey( sKey );                                       // Adicciona la llave a lista de llaves ordenadas

      Data[sKey] = new DicData(sData);                          // Adiciona llave/datos al diccionario
      changed    = true;                                        // Activa la bandera de datos cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona al diccionario, una llave y le asocia los datos de una palabra</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="sData">Datos de palabra asociados a la llave que se va a agregar      </param>
    public void AddKey(String sKey, WordData wData)
      {
      AddSortKey( sKey );                                       // Adicciona la llave a lista de llaves ordenadas

      Data[sKey] = new DicData( wData );                        // Adiciona llave/datos al diccionario
      changed    = true;                                        // Activa la bandera de datos cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona al diccionario, una llave y le asocia los datos de una regla de traducción</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="rData">Datos de palabra asociados a la llave que se va a agregar      </param>
    public void AddKey(String sKey, RuleData rData)
      {
      AddSortKey( sKey );                                       // Adicciona la llave a lista de llaves ordenadas

      Data[sKey] = new DicData( rData );                        // Adiciona llave/datos al diccionario
      changed    = true;                                        // Activa la bandera de datos cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona al diccionario, una llave y le asocia los datos binarios</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="rData">Datos de palabra asociados a la llave que se va a agregar </param>
    public void AddKey(String sKey, byte[] rData)
      {
      AddSortKey( sKey );                                       // Adicciona la llave a lista de llaves ordenadas

      Data[sKey] = new DicData( rData );                        // Adiciona llave/datos al diccionario
      changed    = true;                                        // Activa la bandera de datos cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona al diccionario, una llave y le asocia una tabla</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="tData">Datos de tabla asociados a la llave que se va a agregar </param>
    public void AddKey(String sKey, Tabla tData)
      {
      AddSortKey( sKey );                                       // Adicciona la llave a lista de llaves ordenadas

      Data[sKey] = new DicData( tData );                        // Adiciona llave/datos al diccionario
      changed    = true;                                        // Activa la bandera de datos cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la llave se encuentra en el diccionario, si cambiar llave actual</summary>
    ///
    ///<param name="sKey">Llave que se quire investigar</param>
    ///
    ///<returns>Retorna si la llave se encuentra o no en el diccionario</returns>
    //------------------------------------------------------------------------------------------------------------------
    public bool IsKey(String sKey)
      {
      return Data.ContainsKey(sKey);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Borra una llave del diccionario</summary>
    ///
    ///<param name="sKey">Llave que se desea borrar del diccionario</param>
    ///
    ///<returns>Retorna si la llave se puedo borrar o no</returns>
    public bool Del(String sKey)
      {
      if( SortKeys != null )                                          // Si hay lista de llaves ordenadas
        {
        var idx = SortKeys.BinarySearch(sKey, StringComparer.OrdinalIgnoreCase ); // Busca la llave en la lista
        if( idx>=0 ) SortKeys.RemoveAt( idx );                        // Si la encontro, la borra de la lista
        }

      return Data.Remove(sKey);                                       // La borra de los datos
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca la entrada 'sKey' en el diccionario y la retorna junto a sus datos</summary>
    public KeyValuePair<string,DicData> GetKeyEntry(String sKey)
      {
      DicData Dat = null;                                             // Por defecto pone los datos nulos
      Data.TryGetValue( sKey, out Dat);                               // Trata de obtener valor asociado a la llave  

      return new KeyValuePair<string,DicData>( sKey, Dat);            // Forma el par llave dato y lo retorna
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario con datos asociados y retorna sus datos en forma binaria</summary>
    private byte[] GetBinData(string sKey)
      {
      DicData Dat;

      if( !Data.TryGetValue( sKey, out Dat) )                           // Trata de obtener valor asociado a la llave  
        return null;                                                    // No lo pudo obtener, retorna null

      if( Dat.Tipo == DData.Bin )                                       // El dato es binario
          return (byte[])Dat.Data;                                      // Retorna arreglo de bytes

      return null;                                                      // No es del dato correcto
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario con datos asociados del tipo cadena de caracteres</summary>
    public String GetStrData(String sKey)
      {
      DicData Dat;

      if( !Data.TryGetValue( sKey, out Dat) )                           // Trata de obtener valor asociado a la llave  
        return null;                                                    // No lo pudo obtener, retorna null

      if( Dat.Tipo == DData.oStr )                                      // Es un objeto cadena
          return (string)Dat.Data;                                      // Retorna la cadena directamente  

      if( Dat.Tipo == DData.bStr )                                       // Es un stream de una cadena
        {
        var bytes = (byte[])Dat.Data;                                   // Castea los datos a un arreglo de bytes  
        var s = Encoding.UTF8.GetString( bytes );                       // Codifica la cadena usando un encoder UTF8

        Dat.SetData(s);                                                 // Guarda el objeto cadena
        return s;                                                       // Retorna la cadena
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario con datos asociados del tipo datos de palabras</summary>
    public WordData GetWordData(String sKey)
      {
      DicData Dat;

      if( !Data.TryGetValue( sKey, out Dat) )                           // Trata de obtener valor asociado a la llave  
        return null;                                                    // No lo pudo obtener, retorna null

      if( Dat.Tipo==DData.oWrd )                                        // Es un objeto datos de palabra
        return (WordData)Dat.Data;                                      // Retorna el objeto directamente

      if( Dat.Tipo==DData.bWrd )                                         // Es un stream con datos de palabra
        {
        var bytes = (byte[])Dat.Data;                                   // Castea los datos a un arreglo de bytes  
        var WD = WordData.FromStream( bytes );                          // Crea objeto con los datos de la palabra
        if( WD != null )                                                // Si el formato es correcto
          {
          Dat.SetData( WD );                                            // Guarda objeto con datos de la palabra
          return WD;                                                    // Retorna datos de la palabra
          }

        Dbg.Msg("No se pudieron leer los datos de palabra en '" + sKey + '\'', Dbg.Error );
        return null;                                                     
        }

      Dbg.Msg("Fallo intento de leer como datos de palabra '" + sKey + '\'', Dbg.Error);
      return null;                                                      // Es otro tipo de dato
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario con datos asociados del tipo regla de traducción</summary>
    public RuleData GetRuleData(String sKey)
      {
      DicData Dat;

      if( !Data.TryGetValue( sKey, out Dat) )                           // Trata de obtener valor asociado a la llave  
        return null;                                                    // No lo pudo obtener, retorna null

      if( Dat.Tipo==DData.oRgl )                                        // Es un objeto de datos de reglas
        return (RuleData)Dat.Data;                                      // Retorna el objeto directamente

      if( Dat.Tipo==DData.bRgl )                                        // Es un stream con datos de reglas
        {
        var bytes = (byte[])Dat.Data;                                   // Castea los datos a un arreglo de bytes  
        var RD = RuleData.FromStream( bytes, Tbls );                    // Crea objeto con los datos de reglas
        if( RD != null )                                                // Si el formato es correcto
          {
          Dat.SetData( RD );                                            // Pone los datos en forma de objeto
          return RD;                                                    // Retorna datos de regla
          }

        Dbg.Msg( "No se pudieron leer los datos de la regla en '" + sKey + '\'', Dbg.Error);
        return null;                                                     
        }

      Dbg.Msg( "Fallo intento de leer como una regla la llave '" + sKey + '\'', Dbg.Error);
      return null;                                                     
      }


    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario con datos asociados del tipo tabla</summary>
    public Tabla GetTableData(string sKey)
      {
      DicData Dat;

      if( !Data.TryGetValue( sKey, out Dat) )                           // Trata de obtener valor asociado a la llave  
        return null;                                                    // No lo pudo obtener, retorna null

      if( Dat.Tipo==DData.oTbl )                                        // Es un objeto tabla
        return (Tabla)Dat.Data;                                         // Retorna el objeto directamente

      if( Dat.Tipo==DData.bTbl )                                        // Es un stream con datos de una tabla
        {
        var bytes = (byte[])Dat.Data;                                   // Castea los datos a un arreglo de bytes  
        var TB = Tabla.FromStream( bytes );                             // Crea objeto con los datos de la palabra
        if( TB != null )                                                // Si el formato es correcto
          {
          Dat.SetData( TB );                                            // Guarda objeto con datos de la palabra
          return TB;                                                    // Retorna datos de la palabra
          }

        Dbg.Msg("Al leer datos en forma de tabla '" + sKey + '\'', Dbg.Error );
        return null;                                                     
        }

      Dbg.Msg("No contine datos en forma de tabla '" + sKey + '\'', Dbg.Error );
      return null;                                                      // Es otro tipo de dato
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene una lista ordenada de todas las llaves del diccionario
    public List<string> GetSortedKeys()
      {
      if( SortKeys== null )
        {
//      var lst = from f in Data.AsParallel() orderby f.Key select f.Key;
        var lst = Data.AsParallel().OrderBy(x=>x.Key,StringComparer.OrdinalIgnoreCase).Select(x=>x.Key);
        SortKeys = new List<string>( lst );
        }

      return SortKeys;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Implementa la interface IEnumerable
    public IEnumerator<string> GetEnumerator()
      {
      return Data.Keys.GetEnumerator();
      }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
      return this.GetEnumerator();
      }

    //------------------------------------------------------------------------------------------------------------------
    }

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Maneja datos de un diccionario</summary>
  public partial class DicData
    {
    public DData Tipo;                               // Tipo de dato que contiene, Str-Cadena , Wrd-Palabra, Rgl- Regla
    public object Data;                              // Representación generica de los datos

    //------------------------------------------------------------------------------------------------------------------
    // Constructores de acuerdo al tipo de datos
    public DicData( byte tipo, byte[] data )                      // Stream (arreglo de bytes) 
      {
      Tipo = (DData)tipo;
      Data = data;
      }

    public DicData( string s    ) { Tipo=DData.oStr; Data=s;   }   // Con una cadena de caracteres
    public DicData( WordData wd ) { Tipo=DData.oWrd; Data=wd;  }   // Con un objeto datos de palabras
    public DicData( RuleData rd ) { Tipo=DData.oRgl; Data=rd;  }   // Con un objeto datos de reglas
    public DicData( byte[] bin  ) { Tipo=DData.Bin;  Data=bin; }   // Con un objeto binario
    public DicData(Tabla td     ) { Tipo=DData.oTbl; Data=td;  }   // Con un objeto tabla

    //------------------------------------------------------------------------------------------------------------------
    // Funciones para cambiar los datos de binario a objeto
    public void SetData( string s    ) { Tipo=DData.oStr; Data=s;   }   // Una cadena de caracteres
    public void SetData( WordData wd ) { Tipo=DData.oWrd; Data=wd;  }   // Un objeto datos de palabras
    public void SetData( RuleData rd ) { Tipo=DData.oRgl; Data=rd;  }   // Un objeto datos de reglas
    public void SetData(Tabla tb     ) { Tipo=DData.oTbl; Data=tb;  }   // Un objeto datos en forma de tabla

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lee datos desde un stream</summary>
    public static DicData FromSteam( BinaryReader Rdr )
      {
      var n    = Rdr.ReadUInt16();                            // Lee el tamaño de los datos asociados a la llave
      var tipo = Rdr.ReadByte();                              // Lee el tipo de datos
      var data = Rdr.ReadBytes( (int)(n-1) );                 // Lee los datos

      return new DicData( tipo, data );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe los datos hacia un stream</summary>
    public bool ToStream( BinaryWriter Wtr )
      {
      byte[] bytes;
      if( ((byte)Tipo & 0x80) == 0 )                                // Es un dato tipo stream
        bytes = (byte[])Data;                                       // Obtiene los bytes directamente
      else                                                          // Es un dato de un objeto especifico
        {
        switch( Tipo )                                              
          {
          case DData.oStr:                                          // Es dato es del tipo cadena
            bytes = Encoding.UTF8.GetBytes((string)Data);           // Obtiene los bytes de la cadena
            break;
          case DData.oWrd:                                          // El tipo es dato de palabra
            bytes = ((WordData)Data).ToStream();                    // Obtiene bytes representan los datos de la palabra
            break;
          case DData.oRgl:                                          // El tipo es dato de regla
            bytes = ((RuleData)Data).ToStream();                    // Obtiene bytes que representa los datos de la regla
            break;
          case DData.oTbl:                                          // El tipo es dato en forma de tabla
            bytes = ((Tabla)Data).ToStream();                       // Obtiene bytes que representa los datos de la tabla
            break;
          default:
            Dbg.Msg("Dato de diccionario no pudo ser convertido a STREAM", Dbg.Error);
            return false;
          }
        }

      byte bTipo = (byte)((byte)Tipo & 0x7f);                       // Quita el bit que indica que es un objeto

      Wtr.Write( (ushort)(bytes.Length + 1) );                      // Escribe el tamaño de los datos
      Wtr.Write( bTipo );                                           // Escribe el tipo de datos
      Wtr.Write( bytes );                                           // Escribe los datos
      return true;
      }
    }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Maneja las tablas de cadenas y de Datos que se usan en los diccionarios de reglas</summary>
  public partial class RulesTables
    {
    internal List<string>              Strs      = new List<string>();               // Lista de cadenas
    internal Dictionary<string,UInt16> StrsLook  = new Dictionary<string,UInt16>();  // Para localizar un cadena en la lista
    internal List<RuleData>            ShareKeys = new List<RuleData>();             // Lista de datos compartidos

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona una cadena a la tabla de cadenas del diccionario</summary>
    public UInt16 AddString( string str )
      {
      if( StrsLook.ContainsKey(str) )
        return StrsLook[str];
      else
        {
        StrsLook[str] = (UInt16)Strs.Count;
        Strs.Add( str );
        return (UInt16)(Strs.Count -1);
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la cadena 'idx' en el arreglo de cadenas</summary>
    public string GetString( int idx )
      {
      if( idx<0 || idx>= Strs.Count ) return null;
      return Strs[idx];
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona un dato de reglas a la tabla de datos compartidos</summary>
    public UInt16 AddRuleData( RuleData rData )
      {                                          
      ShareKeys.Add( rData );                                    // Adiciona el dato
      return (UInt16)(ShareKeys.Count -1);                       // Retorna el indice donde se adicionó el dato
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la tabla de cadenas a partir de una cadena separada por |</summary>
    public void CreateStringTable( string str )
      {
      var strs = str.Split('|');
      for( int i=0; i<strs.Length; ++i )
        AddString( strs[i] );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lee la tabla de datos de palabras desde una stream de bytes</summary>
    public void CreateRuleDataTable( byte[] bytes )
      {
      var mem = new MemoryStream( bytes );                      // Crea un strean en memoria con los datos
      var Rdr = new BinaryReader(mem, Encoding.UTF8);           // Crea un lector binario, para leer la memoria

      var NData = Rdr.ReadInt16();
      for( int i=0; i<NData; ++i )
        {
        var RData = RuleData.FromStream( Rdr, this );
        ShareKeys.Add( RData );
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea la tabla de cadenas a partir de una cadena separada por |</summary>
    public string GetStringTableData()
      {
      var s = new StringBuilder( 50*Strs.Count );
      for( int i=0; i<Strs.Count; ++i )
        {
        if( i>0 )  s.Append('|');
        s.Append( Strs[i] );
        }

      return s.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lee la tabla de datos de palabras desde una stream de bytes</summary>
    public byte[] GetRuleDataTable()
      {
      UInt16 n = (UInt16)ShareKeys.Count;

      MemoryStream MStrm = new MemoryStream( 20*n );       // Crea un stream en memoria
      var Wtr = new BinaryWriter( MStrm );                 // Le asocia un Writer binario

      Wtr.Write( n );
      for( int i=0; i<n; ++i )
        {
        var bytes = ShareKeys[i].ToStream(false);          // Obtiene bytes que representa los datos de la regla
        Wtr.Write( bytes );
        }

      return MStrm.ToArray();
      }

    //------------------------------------------------------------------------------------------------------------------
    }

  }
