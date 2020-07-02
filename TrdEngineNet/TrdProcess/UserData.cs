using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;
using TrdEngine.Dictionary;

namespace TrdEngine.TrdProcess
  {
  public class UserData
    {
    public DictList[] UserDicts = new DictList[TConst.DictsList];

    ///<summary>Dirección de traducción a la corresponden los datos</summary>
    public TDir Dir { get; private set;}

    ///<summary>Marca inicial para definir el texto que no se debe traducir</summary>
    public string NtMarkIni{ get; set;}

    ///<summary>Marca final para definir el texto que no se debe traducir</summary>
    public string NtMarkEnd{ get; set;}

    ///<summary>Marca inicial para las palabras no encontradas durante la traducción</summary>
    public string NfMarkIni{ get; set;}

    ///<summary>Marca final para las palabras no encontradas durante la traducción</summary>
    public string NfMarkEnd{ get; set;}

    ///<summary>Utilizar lenguaje informal en la traducción</summary>
    public bool Informal{ get; set;}

    ///<summary>Especialidades activas para la traducción</summary>
    public string ListEsp{ get{ return TxtEsps.ToOneString(); } 
                           set{ 
                              if( !string.IsNullOrEmpty(value) ) TxtEsps = value.Split( ','); 
                              else                               TxtEsps = new string[0];
                             } 
                         }

    internal string[] TxtEsps = new string[0];
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un objeto con los datos del usuario por defecto</summary>
    /// <param name="Dir">Dirección de traducción para la que se inician los datos</param>
    public UserData( TDir Dir )
      {
      this.Dir = Dir;

      NtMarkIni = "[";
      NtMarkEnd = "]";
      NfMarkIni = "";
      NfMarkEnd = "";
      ListEsp   = "";
      Informal  = false;

      for( int i=0; i<UserDicts.Length; ++i )                         // Inicializa todas las listas de diccionarios
        UserDicts[i] = null;                                                  
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa los datos de usuario para una dirección de traducción</summary>
    public bool Initialize()
      {
      var DirData = TrdData.GetDirData(Dir);
      var MainDict = DirData.GetDict( DDirId.Gen) as DictSingle;              // Obtine el diccionario general
      if( MainDict == null ) return false;

      var Dict = new DictList();                                              // Crea una lista nueva
          Dict.Dicts.Add( MainDict );                                         // Adiciona el diccionario general al principio

      UserDicts[ (int)DLType.Main ] = Dict;                                   // Pone esa lista como la principal

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una lista de diccionario para la traducción</summary>
    /// <param name="DictId">Identificador del la lista de diccionario que se quiere obtener</param>
    public IDictTrd GetDict( DLType DictId )
      {
      return UserDicts[(int)DictId];
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una lista de diccionario para su manipulación</summary>
    /// <param name="DictId">Identificador del la lista de diccionario que se quiere obtener</param>
    public DictList GetDictList( DLType DictId )
      {
      return UserDicts[(int)DictId];
      }

    //------------------------------------------------------------------------------------------------------------------
    }

  }
