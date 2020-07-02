using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.IO;
using TrdEngine.TrdProcess;
using TrdEngine.Data;

namespace TrdEngine
  {
  public class UserSet : UserData
    {
    UserDicType[] Dicts = {new UserDicType(),new UserDicType(),new UserDicType(),new UserDicType()};
    static string _path;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Directorio donde se encuentra toda la infomación de los UserSet</summary>
    public static string USetPath 
      {
      get {
          if( !string.IsNullOrEmpty(_path) ) return _path;

          _path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
          _path += "\\IdiomaX\\";
          if( !Directory.Exists( _path ) ) 
            Directory.CreateDirectory( _path ); 

          return _path;
          }
      set { _path = value;}
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Directorio donde se encuentra toda la infomación de los UserSet para una dirección de traducción</summary>
    public static string USetPathDir( TDir Dir ) 
      {
      var root = USetPath + Dir.ToString() + '\\';
      if( !Directory.Exists( root ) )                                          // Si el directorio no existe
        Directory.CreateDirectory( root );                                     // Lo crea

      return root;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un objeto con los datos del usuario por defecto</summary>
    /// <param name="Dir">Dirección de traducción para la que se inician los datos</param>
    public UserSet( TDir Dir ): base( Dir )
      {
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el UserSet por defecto (el ultimo utilizado)</summary>
    public static UserSet Default 
      {
      get {
          string sDir = "EnEs";
          try
            {
            var fName = File.ReadAllText( USetPath + "Default.txt" );

            var part = fName.Split('\\');
            int len  = part.GetLength(0);
            if( len>2 ) sDir = part[len-2];

            var sXml  = File.ReadAllText( fName );
            var USet  = UserSet.FromXml( sXml );

            if( USet!=null ) return USet;
            }
          catch {}

          var newUSet = new UserSet( Utils.ToDir(sDir) );
          UserSet.CheckEspDict( newUSet );
          return newUSet;
          }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Carga el contenido del UserSet desde un fichero</summary>
    /// <param name="fName">Nombre del fichero, o null para cargar el por defecto para la dirección</param>
    public bool Load( string fName=null ) 
      {
      bool ret=false;

      if(  fName==null         ) fName = "Default.uset";            // Obtiene nombre por defecto
      if( !fName.Contains(":") ) fName = USetPathDir(Dir) + fName;  // Obtiene path absoluto

      try
        {
        var sXml = File.ReadAllText( fName );
        Copy( UserSet.FromXml( sXml ) );
        ret = true;
        }
      catch {}

      CheckEspDict(this);
      File.WriteAllText( USetPath + "Default.txt", fName );
      return ret;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Guarda el contenido del UserSet a un fichero</summary>
    /// <param name="fName">Nombre del fichero, o null para guardar el por defecto para la dirección</param>
    public bool Save( string fName=null ) 
      {
      if( fName==null         ) fName = "Default.uset";
      if(!fName.Contains(":") ) fName = USetPathDir(Dir) + fName;

      try   { File.WriteAllText(fName, ToXml() ); }
      catch { return false; }

      File.WriteAllText( USetPath + "Default.txt", fName );
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Copia el contenido de otro UserSet</summary>
    ///<param name="USet">UserSet que se desea copiar</param>
    public bool Copy( UserSet USet)
      {
      if( USet==null      ) return false;
      if( Dir != USet.Dir ) return false;

      NtMarkIni = USet.NtMarkIni;
      NtMarkEnd = USet.NtMarkEnd;
      NfMarkIni = USet.NfMarkIni;
      NfMarkEnd = USet.NfMarkEnd;
      ListEsp   = USet.ListEsp  ;
      Informal  = USet.Informal ;

      for( int iTipe=0; iTipe<USet.CountDictType; ++iTipe )
        {
        var dTipe1 =      GetDictType( (DType)iTipe );
        var dTipe2 = USet.GetDictType( (DType)iTipe );

        int j=0;
        for( ; j<dTipe2.Count; ++j )
          {
          var name = dTipe2[j].Name  ;
          var file = dTipe2[j].File  ;
          var actv = dTipe2[j].Active;

          if( j < dTipe1.Count  ) 
            dTipe1.Modify( j, name, file, actv );
          else
            dTipe1.Insert( j, name, file, actv );
          }

        int n = dTipe1.Count;
        for( int i=j; i<n; ++i ) dTipe1.Remove(j);
        }

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una lista de diccionarios de un tipo determinado</summary>
    ///<param name="TypeId">Identificador del tipo de diccionario que se quiere obtener</param>
    public UserDicType GetDictType( DType TypeId )
      {
      return Dicts[ (int)TypeId ];
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el número de tipos de diccionarios que contiene el UserSet</summary>
    public int CountDictType { get{ return Dicts.GetLength(0); } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación de UserSet en formato XML</summary>
    public string ToXml()
      {
      var XUSet = new XElement( "UserSet" );

      XUSet.SetAttributeValue( "Dir", Dir  );

      XUSet.SetAttributeValue( "NtMarkIni", NtMarkIni );
      XUSet.SetAttributeValue( "NtMarkEnd", NtMarkEnd );
      XUSet.SetAttributeValue( "NfMarkIni", NfMarkIni );
      XUSet.SetAttributeValue( "NfMarkEnd", NfMarkEnd );
      XUSet.SetAttributeValue( "ListEsp"  , ListEsp   );
      XUSet.SetAttributeValue( "Informal" , Informal  );

      for( int iTipe=0; iTipe<Dicts.GetLength(0); ++iTipe )
        {
        var dTipe  = Dicts[iTipe];
        var name = "Dicts" + ((DType)iTipe).ToString();

        var XDType = new XElement( name );

        for( int j=0; j<dTipe.Count; ++j )
          {
          var XDic = new XElement("Dict", dTipe[j].File );

          XDic.SetAttributeValue( "Name"  , dTipe[j].Name   );
          XDic.SetAttributeValue( "Active", dTipe[j].Active );

          XDType.Add( XDic );
          }

        XUSet.Add( XDType );
        }

      return XUSet.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lee los datos del UserSet desde una cadena Xml.</summary>
    ///
    ///<param name="sXml">Cadena que contiene los datos del UserSet en formato Xml</param>
    //------------------------------------------------------------------------------------------------------------------
    static public UserSet FromXml( string sXml )
      {
      try
        {
        var root = XElement.Parse( sXml );                            // Analiza el elemento Raiz

        TDir Dir = Utils.ToDir( root.Attribute("Dir").Value );        // Obtiene la dirección de traducción

        var USet = new UserSet( Dir );                                // Crea un UserSet nuevo

        USet.NtMarkIni  = root.Attribute("NtMarkIni").Value;          
        USet.NtMarkEnd  = root.Attribute("NtMarkEnd").Value;
        USet.NfMarkIni  = root.Attribute("NfMarkIni").Value;
        USet.NfMarkEnd  = root.Attribute("NfMarkEnd").Value;
        USet.ListEsp    = root.Attribute("ListEsp"  ).Value;
        USet.Informal   = bool.Parse( root.Attribute("Informal" ).Value );

        for( int iTipe=0; iTipe<USet.Dicts.GetLength(0); ++iTipe )      // Recorre todos los tipos de diccionarios
          {
          var  Tipe   = (DType)iTipe;                                   // Convierte el indice a enumerador de tipo
          var typDict = USet.GetDictType( Tipe );                       // Obtiene objeto para manejar el tipo de diccionario
          var Name    = "Dicts" + Tipe.ToString();                      // Nombre del elemento XML que identifica al tipo

          var XDType = root.Elements( Name );                           // Obtiene el elemento XML con el tipo
          var Dicts  = XDType.Elements("Dict" );                        // Obtiene todos los elemetos diccionarios dentro de el
          foreach( var dic in Dicts )                                   // Recorre todos los elementos Dict
            {
            string file = dic.Value;                                    // Camino el diccionario
            string name = dic.Attribute("Name").Value;                  // Identificador del diccionario
            bool   actv = bool.Parse( dic.Attribute("Active").Value );  // Si esta activo o no 

            typDict.Add( name, file, actv );                            // Lo adicciona al UserSet
            }
          }

        CheckEspDict( USet );                                           // Chequea que los diccionarios de espacilidad existan  
        return USet;                                                    // Todo salio bien, retorna el UserSet cargado
        }
      catch
        {
        return null;                                                    // Hubo un problema retorna null
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary> Obtiene los diccionarios de especilidad disponibles para la dirección de traducción </summary>
    static public void CheckEspDict( UserSet USet )
      {
      var DicsPath = TrdData.DicsPath;                                // Directorio de los diccionanarios
      if( !Directory.Exists(DicsPath) ) return;                       // El directorio es incorrecto, termina

      TDir Dir   = USet.Dir;                                          // Dirección de traducción
      var src    = Utils.Src(Dir);                                    // Idioma fuente
      var des    = Utils.Des(Dir);                                    // Idioma destino
      var filter = "Sp" + src + '2' + des + "*.dcb";                  // Filtro para diccionarios de especialidas

      string[] files = Directory.GetFiles(DicsPath, filter);          // Busca diccionarios de especialidades en disco

      var espType = USet.GetDictType( DType.Esp );                    // Diccionarios de especialidades del UserSet

      for(int i=0; i<espType.Count; ++i )                             // Recorre todos los diccionarios
        {
        var file = espType[i].File;                                   // Camino al diccionario

        int idx = Array.FindIndex(files, s => s.ToLower()==file );    // Busca si esta en el disco
        if( idx == -1 )                                               // No esta
          espType.Remove(i--);                                        // Lo quita del UserSet
        else                                                          // Esta
          files[idx] = string.Empty;                                  // Lo marca como analizado
        }

      foreach (string file in files)                                  // Analiza todos los diccionarios encontrados
        {
        if( file == string.Empty ) continue;                          // Ya se analizó, no hace nada

        var    fName = Path.GetFileNameWithoutExtension(file);        // Toma el nombre solamente
        string dName = fName.Substring( 7 );                          // Extrae nombre de la especialidad

        espType.Add( dName, file );                                   // Lo adiciona al UserSet
        }
      }

    //----------------------------------- Fin de la clase UserSet ------------------------------------------------------
    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Agrupa y maneja todos los diccionarios del mismo tipo</summary>
  ///
  ///<remarks>Hasta el momento se soportan 4 tipo de diccionarios diferentes, estos son: Especializados, de Usuario, 
  ///de Memorias de traducción y de Palabras que no se traducen</remarks>
  public class UserDicType
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Lista de diccionarios que tiene el tipo de diccionario</summary>
    private List<UserDicData> Dicts = new List<UserDicData>();

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Indica el numero de cambios realizados después de la ultima sincronización</summary>
    public int chg = 0;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna el número de diccionario que tiene el tipo</summary>
    public int Count{ get{ return Dicts.Count;} }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene los datos del diccionario con indice Idx</summary>
    public UserDicData this [int Idx] { get{ return( (Idx<0 || Idx>=Dicts.Count)? null:Dicts[Idx] ); } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona un diccionario al tipo al final de la lista</summary>
    ///
    ///<param name="Name"  > Nombre mediate el cual el usuario identifica el diccionario      </param>
    ///<param name="Path"  > Camino donde se localiza el archivo que contieine el diccionario </param>
    ///<param name="Active"> Indica si el diccionario esta activo para la traducción o no     </param>
    public bool Add(string Name, string Path, bool Active=false )
      {
      return Insert( Dicts.Count, Name, Path, Active);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona un diccionario al tipo en una posición determinada </summary>
    ///
    ///<param name="Idx"   > Posición en la lista donde se desea insertar el diccionario      </param>
    ///<param name="Name"  > Nombre mediate el cual el usuario identifica el diccionario      </param>
    ///<param name="Path"  > Camino donde se localiza el archivo que contieine el diccionario </param>
    ///<param name="Active"> Indica si el diccionario esta activo para la traducción o no     </param>
    public bool Insert(int Idx, string Name, string Path, bool Active=false  )
      {
      var path = Path.ToLower();

      if( FindIdx( path ) != -1     ) return false;
      if( Idx<0 || Idx>Dicts.Count  ) return false;

      Dicts.Insert( Idx, new UserDicData(path,Name,Active) );
      if(Active) ++chg;
      return true;
      }


    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Modifica los datos de un diccionario</summary>
    ///
    ///<param name="Idx"   > Posición en la lista del diccionario a modificar      </param>
    ///<param name="Name"  > Nombre mediate el cual el usuario identifica el diccionario      </param>
    ///<param name="Path"  > Camino donde se localiza el archivo que contieine el diccionario </param>
    ///<param name="Active"> Indica si el diccionario esta activo para la traducción o no     </param>
    public bool Modify(int Idx, string Name, string Path, bool Active=false  )
      {
      var path = Path.ToLower();

      if( Idx<0 || Idx>Dicts.Count  ) return false;

      var dic = Dicts[Idx];

      if( dic.Name   != Name   )   dic.Name   = Name;
      if( dic.File   != path   ) { dic.File   = path;   if(Active) ++chg; }
      if( dic.Active != Active ) { dic.Active = Active; ++chg; }
      
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Quita un diccionario de la lista</summary>
    ///
    ///<param name="Idx">Posición en la lista donde se desea eliminar</param>
    public void Remove( int Idx )
      {
      if( Idx<0 || Idx>Dicts.Count  ) return;

      if( Dicts[Idx].Active ) ++chg;
      Dicts.RemoveAt( Idx );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Quita todos los diccionarios de la lista</summary>
    public void Clear()
      {
      Dicts.Clear();
      ++chg;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un diccionario dentro de la lista y retorna su posición</summary>
    ///
    ///<param name="DicPath">Localización del diccionario en disco</param>
    ///
    ///<returns>Retorna el indice al diccionario si lo encuentra, en otro caso retorna -1</returns>
    public int FindIdx( string DicPath )
      {
      var path = DicPath.ToLower();

      for( int i=0; i<Dicts.Count; ++i )
        if( Dicts[i].File==path )
          return i;

      return -1;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca un diccionario dentro de la lista y retorna sus datos</summary>
    ///
    ///<param name="DicPath">Localización del diccionario en disco</param>
    ///
    ///<returns>Retorna un objeto DictData o null si no lo puede encontar</returns>
    public UserDicData Find( string DicPath )
      {
      int idx = FindIdx( DicPath );
      if( idx==-1 ) return null;

      return Dicts[idx];
      }

    //---------------------------------------- Fin de la clase UserDicType --------------------------------------------
    }


  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Mantiene los datos asociados a un diccionario</summary>
  public class UserDicData
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Nombre mediante el cual el usuario identifica el diccionario</summary>
    public string Name  {get; internal set; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Camino completo al archivo que contiene los datos del diccionario</summary>
    public string File  {get; internal set; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Indica si el diccionario esta activo en el proceso de traducción o no</summary>
    public bool  Active{get; internal set;}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa un objeto con los datos necesarios</summary>
    ///<param name="File"> Camino completo al archivo que contiene los datos del diccionario</param>
    ///<param name="Name"> Nombre mediante el cual el usuario identifica el diccionario</param>
    ///<param name="Active">Indica si el diccionario esta activo en el proceso de traducción o no</param>
    public UserDicData( string File, string Name, bool Active = false )
      {
      this.File   = File;
      this.Name   = Name;
      this.Active = Active;
      }

    //--------------------------------------------  Fin de la clase UserDicData ----------------------------------------
    }

  }
