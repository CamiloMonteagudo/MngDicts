using System;
using System.Collections.Generic;
using TrdEngine;

namespace TrdEngine.Dictionary
  {
  public class DictList : IDictTrd
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lista de diccionarios, la cual se puede usar para adicionar, insertar y borrar diccionario</summary>
    public List<DictSingle> Dicts = new List<DictSingle>();

    string path = "";               // Tipo de diccionario

    TLng  src  = TLng.NA;           // Código del idioma fuente                   
    TLng  des  = TLng.NA;           // Código del idioma destino                  
    DType type = 0;                 // Banderas que definen los atributos                  
    UInt16  flags = 0;

    public string Path  {get{return path; }}
    public TLng   Src   {get{return src;  }}
    public TLng   Des   {get{return des;  }}
    public DType  Type  {get{return type; }}
    public UInt16 Flags {
                        get{return flags;}
                        set{flags=value; }
                        }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Nombre del diccionario</summary>
    public string Name {get; set;}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Diccionario actual</summary>
    ///<remarks>Es el diccionario donde se realizo la ultima busqueda con exito</remarks>
    public int ActDict {get; set;}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Posición de la llave actual en el diccionario actual</summary>
    public int ActPos {get; set;}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Diccionario de usuario</summary>
    ///<remarks>Es el diccionario donde se realizaran los cambios</remarks>
    public int UserDict {get; set;}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Contructor por defecto</summary>
    public DictList()
      {
      Name = "";
      ActDict = 0;
      UserDict = -1;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre un diccionario y lo asocia a la lista como diccionario principal</summary>
    ///
    ///<param name="name">Nombre del archivo que contiene el diccionario principal (incluye el camino)</param>
    ///<param name="Mode">Modo en el que va a abrir el diccionario (Ver enum Modo)</param>
    ///
    ///<returns>Si la operación se realizo o no satisfactoriamente</returns>
    //----------------------------------------------------------------------------------------------------------------------
    public bool Open( string name )
      {
      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Guarda los cambio hechos al diccionario</summary>
    ///
    ///<remarks>Si se modifico alguna información en el diccionario la guarda a fichero, si hay definido un diccionario 
    ///de usuario solo guarda este diccionario, de lo contrario los guarda todos.
    ///Solo en necesario guardar los cambios de los diccionario abiertos en el modo 'Memory' ya que los que estan en modo
    ///File se guardan automaticamente</remarks>
    //------------------------------------------------------------------------------------------------------------------
    bool Save()
      {
      return false;                                   // Guarda el diccionario
      }

    //------------------------------------------------------------------------------------------------------------------


    /*******************************************************************************************************************
    ********************************** Implementación de la Interface IIdxDict *****************************************
    *******************************************************************************************************************/ 
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cantidad de registros que tiene el diccionario</summary>
    //------------------------------------------------------------------------------------------------------------------
    public int Count 
      { 
      get {
          int count = 0;
          foreach( var dic in Dicts)
            count += dic.Count;

          return count;
          } 
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
      foreach( var dic in Dicts)
        if( dic.IsKey(sKey) ) return true;

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adicciona una llave y sus datos al diccionario</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere adiccionar al diccionario </param>
    ///<param name="sData">Datos asociados a la llave que se va a agregar      </param>
    ///
    ///<returns>Retorna si la llave puedo ser agregada o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    public bool AddKey(String sKey, String sData)
      {
      //if( UserDict<0 || UserDict>=_Dicts.Count ) return false;

      //return _Dicts[UserDict].AddKey( sKey, sData );
      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Modifica los datos asociados a una llave del diccionario</summary>
    ///
    ///<param name="sKey" >Llave nueva que se quiere modificar </param>
    ///<param name="sData">Datos nuevos de para la llave dada  </param>
    ///
    ///<returns>Retorna si la llave puedo ser modificada o no</returns>
    ///<remarks>Si la llave no se encuantra en el diccionario se agrega</remarks>
    //------------------------------------------------------------------------------------------------------------------
    //public bool Modify(String sKey, String sData)
    //  {
    //  if( UserDict<0 || UserDict>=_Dicts.Count ) return false;

    //  return _Dicts[UserDict].Modify( sKey, sData );
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Borra una llave del diccionario</summary>
    ///
    ///<param name="sKey">Llave que se desea borrar del diccionario</param>
    ///
    ///<returns>Retorna si la llave se puedo borrar o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    public bool Del(String sKey)
      {
      if( UserDict<0 || UserDict>=Dicts.Count ) return false;

      return Dicts[UserDict].Del( sKey );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca una llave en el diccionario y retorna sus datos asociados</summary>
    ///
    ///<param name="sKey">Llave que se quiere buscar en el diccionario</param>
    ///
    ///<returns>Retorna datos asociados a la llave, si hay algún error retorna cadena vacia</returns>
    //------------------------------------------------------------------------------------------------------------------

    public string GetStrData(string sKey)
      {
      throw new NotImplementedException();
      }

    public Tabla GetTableData(string sKey)
      {
      throw new NotImplementedException();
      }

    public RuleData GetRuleData(string sKey)
      {
      throw new NotImplementedException();
      }

    public WordData GetWordData(string sKey)
      {
      throw new NotImplementedException();
      }

    public List<string> GetSortedKeys()
      {
      throw new NotImplementedException();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone una llave como llave actual</summary>
    ///
    ///<param name="sKey">Llave que se quiere poner como llave actual</param>
    ///
    ///<returns>Retorna si se puedo establecer la llave actual o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool SetActualKey(String sKey)
    //  {
    //  return GetStrData(sKey).Length > 0 ;
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la llave actual</summary>
    ///
    ///<returns>Retorna la llave actual, si no se puede obtener retorna cadena vacia</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public String GetActualKey()
    //  {
    //  if( ActDict<0 || ActDict>=_Dicts.Count ) return "";

    //  return _Dicts[ActDict].GetKeyAt(ActPos);
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene los datos asociados a la llave actual</summary>
    ///
    ///<returns>Retorna si la llave puedo ser borrada o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public String GetActualData()
    //  {
    //  if( ActDict<0 || ActDict>=_Dicts.Count ) return "";

    //  return _Dicts[ActDict].GetDataAt(ActPos);
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Borra la llave actual</summary>
    ///
    ///<returns>Retorna si la llave pudo ser borrada o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool DelActual()
    //  {
    //  if( ActDict!= UserDict ) return false;
    //  if( UserDict<0 || UserDict>=_Dicts.Count ) return false;

    //  return _Dicts[UserDict].DelAt(ActPos);
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Se mueve la llave actual a la PROXIMA llave del diccionario</summary>
    ///
    ///<returns>Retorna si se puedo mover la llave actual o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool Next()
    //  {
    //  SortedList<string, int> Keys = new SortedList<string, int>();

    //  string Key = GetActualKey();                        // Obtine la llave actual

    //  for( int i=0; i<_Dicts.Count; ++i)                  // Busca por todos los diccionarios
    //    {
    //    var Dict = _Dicts[i];
    //    if( Dict.GetStrData(Key).Length > 0 )                 // Si encuentra llave acyual en el diccionario
    //      Dict.Next();                                    // Se mueve a la proxima llave

    //    string NowKey = Dict.GetActualKey();              // Coje la palabra actual

    //    if (NowKey.CompareTo(Key) > 0)                    // Solo si esta despúes
    //      Keys.Add(NowKey, i);                            // Guarda en la lista ordenada
    //    }

    //  if (Keys.Count == 0) return false;                  // No encontro ninguna palabra despues de ella

    //  ActDict = Keys.Values[0];                           // De todas las encontrada la primera
    //  ActPos  = _Dicts[ActDict].GetActualPos();           // Actualiza la posiccion actual

    //  return true;
    //  }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Se mueve la llave actual a la llave ANTERIOR del diccionario</summary>
    ///
    ///<returns>Retorna si se puedo mover la llave actual o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool Previous()
    //  {
    //  SortedList<string, int> Keys = new SortedList<string, int>();

    //  string Key = GetActualKey();                        // Obtine la llave actual

    //  for( int i=0; i>_Dicts.Count; ++i )                 // Para cada uno de los diccionarios
    //    {
    //    var Dict = _Dicts[i];
    //    Dict.GetStrData(Key);                                 // Busca la llave actual

    //    if( Dict.Previous()  )                            // Si hay palabra anterior a la buscada
    //      Keys.Add( Dict.GetActualKey(), i );             // La guarda en la lista ordenada
    //    }

    //  int n = Keys.Count;                                 // Número de llaves candidatas
    //  if( n == 0 ) return false;                          // No hay llaves, es la primera palabra

    //  ActDict = Keys.Values[n-1];                         // De todas las encontrada la primera
    //  ActPos  = _Dicts[ActDict].GetActualPos();           // Actualiza la posiccion actual

    //  return true;
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Se mueve la llave actual a la PRIMERA llave del diccionario</summary>
    ///
    ///<returns>Retorna si se puedo mover la llave actual o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool First()
    //  {
    //  SortedList<string, int> Keys = new SortedList<string, int>();

    //  for( int i=0; i>_Dicts.Count; ++i )                 // Para cada uno de los diccionarios
    //    {
    //    var Dict = _Dicts[i];
    //    if( Dict.SetActualPos(0)  )                       // Se posiciona en la primera palabra
    //      Keys.Add( Dict.GetActualKey(), i );             // La guarda llave en la lista ordenada
    //    }

    //  if( Keys.Count == 0 ) return false;                 // No hay ninguna palabra en el diccionario

    //  ActDict = Keys.Values[0];                           // De todas las encontrada la primera
    //  ActPos  = 0;                                        // Obtiene la posiccion actual

    //  return true;
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Se mueve la llave actual a la ULTIMA llave del diccionario</summary>
    ///
    ///<returns>Retorna si se puedo mover la llave actual o no</returns>
    //------------------------------------------------------------------------------------------------------------------
    //public bool Last()
    //  {
    //  SortedList<string, int> Keys = new SortedList<string, int>();

    //  for( int i=0; i>_Dicts.Count; ++i )                 // Para cada uno de los diccionarios
    //    {
    //    var Dict = _Dicts[i];

    //    int last = Dict.Count - 1 ;                       // Ultimo registro del diccionario
    //    if( Dict.SetActualPos(last)  )                    // Se posiciona en la primera palabra
    //      Keys.Add( Dict.GetActualKey(), i );             // La guarda en la lista ordenada
    //    }

    //  int n = Keys.Count;                                 // Número de llaves candidatas
    //  if( n == 0 ) return false;                          // No hay ninguna palabra en el diccionario

    //  ActDict = Keys.Values[n-1];                         // De todas las encontrada la ultima
    //  ActPos  = _Dicts[ActDict].GetActualPos();           // Obtiene la posiccion actual

    //  return true;
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la llave actual es la PRIMERA del diccionario</summary>
    //------------------------------------------------------------------------------------------------------------------
    public bool IsLast()
      {
      if( ActDict<0 || ActDict>=Dicts.Count ) return false;
      return ActPos == (Dicts[ActDict].Count-1);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la llave actual es la ULTIMA del diccionario</summary>
    //------------------------------------------------------------------------------------------------------------------
    public bool IsFirst()
      {
      if( ActDict<0 || ActDict>=Dicts.Count ) return false;
      return ActPos == 0;
      }

    //------------------------------------------------------------------------------------------------------------------

    }
  }
