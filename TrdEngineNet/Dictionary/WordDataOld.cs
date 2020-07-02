using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace TrdEngine.Dictionary
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena la información correspondiente a los datos de traducción de una palabra</summary>
  //------------------------------------------------------------------------------------------------------------------
  public class WordData_
    {
    //------------------------------------------------------------------------------------------------------------------
    // Datos de la clase
    //------------------------------------------------------------------------------------------------------------------
    List<MeanSet_> types = new List<MeanSet_>(5);                   // Información para los tipos gramaticales
    string cType = "";                                              // Tipo compuesto de la palabra
    string Info  = "";

    List<TermInfo> Term = new List<TermInfo>();                     // Información sobre los terminos

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Listado de todos los tipos gramaticales que forman la palabra</summary>
    public List<MeanSet_> Types{ get{return types;} }
    ///<summary>Tipo compuesto o principal de la palabra</summary>
    public string CompType { get{return cType;}} 

    //------------------------------------------------------------------------------------------------------------------
    // Expersiones regulares para obtener la informacion de las palabras
    //------------------------------------------------------------------------------------------------------------------
    static Regex reCType = new Regex(@"^#T(?<Tc>[A-Z]{2})(?<inf>#[A-Z;]{2,})?(?<Term>;[A-Z]{2}:[A-Z]{2})*#E", RegexOptions.Compiled);
    static Regex reSType = new Regex(@"\(W=(?<Ts>[A-Z]{2})\) *\?", RegexOptions.Compiled);
    static Regex reMeans = new Regex(@"\{(?<means>[^}]+)\}"      , RegexOptions.Compiled);

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto WordData a partir una cadena de caracteres</summary>
    ///
    ///<param name="s">Cadena con la información de una palabra, generalmente se obtine de un diccionario</param>
    ///
    ///<returns>Retorna un objeto con los datos de la palabra, si la cadena no tiene el fromato correcto retorna null</returns>
    //------------------------------------------------------------------------------------------------------------------
    public static WordData_ FormStr( string s )
      {
      Match m1 = reCType.Match(s);                                  // Machea el encabezamiento (Tipo compuesto)
      if( !m1.Success ) return null;                                // No se corresponde, formato incorrecto
        
      var wd = new WordData_();
      wd.cType = m1.Groups["Tc"  ].Value;                           // Obtiene el tipo compuesto
      wd.Info  = m1.Groups["inf" ].Value;                           // Obtiene información adicional

      wd.ParseTermino( m1 );                                        // Si se puede comportar como un termino, en una especialidad
      if( m1.Groups["Term"].Captures.Count>0 )
        Dbg.Msg( "TERMINO: " + s );

      int Idx = m1.Length;                                          // Salta elementos analizados
      
      for(;;)                                                       // Repite para todos los tipos gramaticales
        {
        if( !wd.ParseTypeSingle( s, ref Idx ) )                     // Analiza el tipo actual
          return null;                                              // Hubo error al obtener el tipo

        if( Idx >= s.Length ) return wd;                            // Estamos al final de la cadena, retorna OK
        if( s[Idx++] != ':' ) return null;                          // No hay separador para los tipos, error de formato
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto WordData a partir un stream de bytes</summary>
    ///
    ///<param name="bytes">Arreglo de bytes con la información de una palabra</param>
    ///
    ///<returns>Retorna un objeto con los datos de la palabra, si el stream no se corresponde con los datos de una palabra retorna null</returns>
    //------------------------------------------------------------------------------------------------------------------
    public static WordData_ FromStream( byte[] bytes )
      {
      var wd = new WordData_();
      MemoryStream ms = new MemoryStream(bytes);                    // Crea un stream en memoria
      var buff = new BinaryReader( ms, Encoding.UTF8 );             // Le asocia un lector (Reader) para leer datos binarios

      var TipoData = (DData)buff.ReadByte();                        // Lee el primer byte que define el tipo de dato
      if( TipoData!=DData.bWrd ) return null;                       // Si no el tipo adecuado retorna error

      int nTipos = buff.ReadByte();                                 // Lee el primer byte que define el número de tipos 
      if( nTipos<=0 || nTipos>6 )                                   // Verifica que el número de tipos sea correcto
        return null;                                                // Retorna error

      if( nTipos>1 )                                                // Es un tipo compuesto
        wd.cType = new string( buff.ReadChars(2) );                 // Lee el tipo compuesto

      for( int i=0; i<nTipos; ++i )                                 // Lee todos los tipos gramaticales
        {
        if( i < nTipos-1)
          buff.ReadUInt16();                                        // Se salta el tamaño del primer tipo gramatical

        MeanSet_ Wt = MeanSet_.FromStream( buff );                  // Obtiene la información de un tipo simple
        if( Wt==null ) return null;                                 // Hubo error el obtener el tipo gramatical
                           
        wd.Types.Add( Wt );                                         // Adiciona el tipo a la lista
        }

      if( nTipos==1 )                                               // Es un tipo simple
        wd.cType = wd.Types[0].sType;                               // Pone tipo complejo igual a primer tipo simple

      return wd;                                                    // Retorna el objeto con todos sus datos
      }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza la información de un tipo simple a partir del idice 'Idx' de la cadena 's'
    //------------------------------------------------------------------------------------------------------------------
    private bool ParseTypeSingle( string s, ref int Idx )
      {
      string sType = null, Means;

      if( s[Idx] == '(' )                                           // Viene especificación del tipo simple
        {                                                   
        Match m2 = reSType.Match( s, Idx ) ;                        // Machea formato para especificar tipo simple
        if( !m2.Success || m2.Index>Idx ) return false;             // No se corresponde, formato incorrecto 
                                                            
        sType = m2.Groups["Ts"].Value;                              // Obtiene valor del tipos simple
        Idx  += m2.Length;                                          // Salta elementos analizados
        }                                                   
                                                            
      if( s[Idx] == '{' )                                           // Viene definición de los significados
        {                                                           
        Match m3 = reMeans.Match( s, Idx );                         // Machea formato para los significados
        if( !m3.Success || m3.Index>Idx ) return false;             // No se corresponde, formato incorrecto
                                                            
        Means = m3.Groups["means"].Value;                           // Obtiene significados
        Idx  += m3.Length;                                          // Salta elementos analizados
        }                                                   
      else                                                  
        {                                                   
        if( types.Count != 0 )                                      // Ya hay otros tipos analizados
          return false;                                             // Formatos incorrecto, solo es valido para un tipo simple
                                                            
        Means = s.Substring(Idx);                                   // Toma el resto de la cadena, como significados
        Idx   = s.Length;                                   
        }                                                   
                                                            
      bool End = (Idx>=s.Length);                                   // Indica si se analizo hasta la final la cadena o no
      if( !CheckType( ref sType, End ) )                            // Verifica o corrige el tipo simple
        return false;                                       
                                                            
      var Wt = new MeanSet_();                                      // Crea un objeto para guardar información de tipo
      if( !Wt.Parse( sType, Means ) )                               // No pudo adiciona tipo y significados
        return false;                                               // Formato de los significados incorrecto
                                                            
      types.Add(Wt);                                                // Adiciona información de tipo a los datos
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Si 'sType' es null trata de inferirlo, de lo contrario chequea que sea valido para el tipo compuesto actual
    //------------------------------------------------------------------------------------------------------------------
    private bool CheckType( ref string sType, bool End )
      {
      if( sType == null  )                                          // No se ha obtenido ningún tipo simple
        {
        if( !End ) return false;                                    // No esta al final de la cadena, error

        if( types.Count == 0 )                                      // No hay mas tipo en la palabra
          {
          if( !GramType.IsSingle(cType) ) return false;             // No es un tipo simple. error
          sType = cType;                                            // Tipo simple y compuesto son iguales
          }
        else
          {
          var Typs = GetTypeList();                                 // Obtiene listado de tipos
          sType = GramType.GetDeafultType( cType, Typs );           // Obtiene el subtipo que le falta al tipo compuesto
          if( sType.Length < 2 )                                    // No lo pudo obtener
            return false;                                           // Retorna Error
          }
        }
      else
        if( !GramType.IsSubType( cType, sType ) )                   // El tipo simple forma parte del tipo compuesto
          return false;                                             

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene información de los terminos, si existe
    //------------------------------------------------------------------------------------------------------------------
    private void ParseTermino( Match m )
      {
      for( int i=0; i<m.Groups["Term"].Captures.Count; ++i )        // Recorre todos los terminos capturados
        {
        string s   = m.Groups["Term"].Captures[i].Value;            // Obtiene cadena capturada
        string esp = s.Substring(1,2);                              // Extrae la especilidad
        string typ = s.Substring(4,2);                              // Extrae el tipo gramatical
      
        Term.Add( new TermInfo(esp,typ) );                          // Guarda en la lista de información de terminos
        }
      }                                      

    //------------------------------------------------------------------------------------------------------------------
    // Convierte la información de terminos del objeto, en una cadena que lo represente
    //------------------------------------------------------------------------------------------------------------------
    private string TermToStr()
      {
      string sTerm = "";

      foreach( TermInfo ti in Term )                                // Para todos los terminos almacenados
        sTerm += string.Format( ";{0}:{1}", ti.Esp, ti.Typ );       // Agrega una cadena con el formato correcto

      return sTerm;                                                 // Retorna
      }                                      

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un listado con el código de dos letras de todos los tipos gramaticales de la palabra</summary>
    //------------------------------------------------------------------------------------------------------------------
    public List<string> GetTypeList()
      {
      List<string> lst = new List<string>();
      foreach( MeanSet_ wt in Types )
        lst.Add( wt.sType );

      return lst;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena que contiene los datos de una palabra</summary>
    //------------------------------------------------------------------------------------------------------------------
    public override string ToString()
      {
      var Typs = GetTypeList();                                     // Obtiene listado de tipos simples
      string cType = GramType.GetCompoundType(Typs) ;               // Obtiene el tipo compuesto a partir de tipos simples

      var s = new StringBuilder( "#T" + cType );                    // Crea encabezamiento con el tipo compuesto

      if( Info.Length>0 ) s.Append(Info);                           // Si hay info adiconal, la agrega
      if( Term.Count >0 ) s.Append(TermToStr());                    // Si hay información de tipo, la grega

      s.Append("#E");                                               // Agrega marcador de inicio de datos

      int n = types.Count;                                          // Obtiene número de tipos simples
      for( int i=0; i<n; ++i )                                      // Recorre todos los tipos simples
        {
        if( i != 0 ) s.Append(':');                                 // Si no es el primero agrega separador

        if( i<n-1 )                                                 // Si no es el ultimo
          s.Append("(W=" + types[i].sType + ")?");                  // Agrega información de tipo fijo

        string sType = types[i].ToString();                         // Obtiene representación del tipo

        if( n>1 ) s.Append( '{' + sType + '}' );                    // Si hay mas de uno agrega significados entre llaves
        else      s.Append( sType );                                // Si hay solo uno agrega significados sin llaves
        }
      
 	    return s.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un Stream que contiene los datos de una palabra</summary>
    //------------------------------------------------------------------------------------------------------------------
    public byte[] ToStream()
      {
      MemoryStream ms = new MemoryStream(600);                      // Crea un stream en memoria
      var buff = new BinaryWriter(ms);                              // Le asocia un Writer binario

      var Typs = GetTypeList();                                     // Obtiene listado de tipos simples
      string cType = GramType.GetCompoundType(Typs) ;               // Obtiene el tipo compuesto a partir de tipos simples

      int n = types.Count;                                          // Obtiene número de tipos simples
      buff.Write( (byte)n );                                        // Escribe el número de tipos simples

      if( n > 1 )
        {
        buff.Write(cType[0]);                                       // Escribe primara letra del tipo gramatical compuesto
        buff.Write(cType[1]);                                       // Escribe segunda letra del tipo gramatical compuesto

        for( int i=0; i<n; ++i )                                    // Recorre todos los tipos simples
          {
          byte[] tBytes = types[i].ToStream();                      // Obtiene el stream del tipo gramatical

          if( i<n-1 )                                               // Si no es el ultimo
            buff.Write( (ushort)(tBytes.Length) );                  // Escribe la longitud del arreaglo

          buff.Write( tBytes );                                     // Escribe el arreaglo
          }
        }
      else
        buff.Write( types[0].ToStream() );                          // Escribe el arreaglo para el unico tipo

 	    return ms.ToArray();                                          // Retorna un arreglo de bytes
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena en formato JSON que contiene los datos de una palabra</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string ToJSON( string Key, int Idx )
      {
      var s = new StringBuilder( 500 );                             // Reserva espacio para 500 caracteres

      s.Append("{Error:0,");                                        // Inicia objeto con datos de la palabra
      s.Append( "Key:\"" + Key.Replace( @"""", @"\""") + "\"," );   // Agrega la palabra
      s.Append( "Idx:"   + Idx + ','   );                           // Agrega indice a la palabra

      s.Append("Types:[");                                          // Inicia arreglo de tipos simples
      for( int i=0; i<types.Count; ++i )                            // Recorre todos los tipos simples
        {
        if( i != 0 ) s.Append(',');                                 // Si no es el primero agrega separador
        s.Append( types[i].ToJSON() );                              // Agrega el tipo gramatical simple
        }
      s.Append(']');                                                // Termina arreglo de tipos simples

      s.Append('}');                                                // Termina de definición de objeto
 	    return s.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Chequea la validez de todos los datos de la palabra</summary>
    ///
    ///<returns>Cero si los datos son correctos o un número que idica el código del error encontrado</returns>
    ///<remarks>Los código de errores son los siguientes:
    ///             1 -> Ningún tipo compuesto tiene los tipos actuales
    ///             2 -> Tipo gramatical sin significado
    ///             3 -> Caracteres no validos en el significado
    ///             4 -> Genero no valido
    ///             5 -> Código de especilidad incorrecto
    ///</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public int CheckData()
      {
      // Chequeo de valides de los tipos simples
      var Typs = GetTypeList();                                     // Obtiene listado de tipos simples
      string cType = GramType.GetCompoundType(Typs) ;               // Obtiene el tipo compuesto a partir de tipos simples
      if( cType[0]==' ' )  return 1;                                // Error: Ningún tipo compuesto tiene los tipos actuales

      char[] InvCh = "{}[]\"".ToCharArray();
      foreach( MeanSet_ wt in Types )                               // Recorre todos los tipos
        {
        // Chequeo de al menos un significado por tipo
        if( wt.Means.Count == 0 ) return 2;                         // Error: Tipo gramatical sin significado

        foreach( WordMean_ wm in wt.Means )
          {
          // Chequeo de caracteres ilegales dentro de los significados
          if( wm.Mean.IndexOfAny(InvCh) != -1 ) return 3;           // Error: Caracteres no validos en el significado

          // Chequeo que el genero sea valido
          if( wm.Gen != Genero.Femen && 
              wm.Gen != Genero.Masc  &&
              wm.Gen != Genero.Neutro  ) return 4;                  // Error: Genero no valido

          // Chequeo de código de especialidad
          if( wm.Esp.Length != 2  ) return 5;                       // Error: Código de especilidad incorrecto
          }
        }

      return 0;
      }

    //------------------------------------------------------------------------------------------------------------------

    internal static WordData_ FromXml(System.Xml.Linq.XElement el)
      {
      throw new NotImplementedException();
      }

    internal string ToXml()
      {
      throw new NotImplementedException();
      }
    }

/**********************************************************************************************************************************/
/********************************                    WordType                                     *********************************/
/**********************************************************************************************************************************/

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena la información correspondiente a un tipo gramatical</summary>
  //------------------------------------------------------------------------------------------------------------------
  public class MeanSet_  
    {
    ///<summary>Código del tipo gramatical al que estan asociado los datos</summary>
    public string sType = "";

    ///<summary>Lista de los significados asociados al tipo gramatical</summary>
    public List<WordMean_> Means = new List<WordMean_>(6);

    ///<summary>Lista de los comandos asociados al tipo gramatical</summary>
    public List<IDictCmd> Cmds = new List<IDictCmd>();

    static Regex reMean = new Regex("(@((?<Esp>[A-Z]{2})|(\\[(?<Info>[^\\]]+)\\])))?\"(?<mean>[^\"]+)\"(?<Att>[$*!-]){0,3}", RegexOptions.Compiled);

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Analiza una cadena con los datos de un significado de una palabra y extrae su infomación</summary>
    ///
    ///<param name="sType">Tipo simple al que pertenecen los datos  </param>
    ///<param name="s"    >Cadena con la información del significado</param>
    ///
    ///<returns>Retorna true si la cadena que representa la información de un significado pudo ser analizada completamente, de
    ///lo contrario retorna false</returns>
    //------------------------------------------------------------------------------------------------------------------
    public bool Parse( string sType, string s )
      {
      this.sType = sType;                                           // Pone codigo del tipo simple

      for(int Idx=0;;)                                              // Recorre todos los significados
        {
        Match m = reMean.Match( s, Idx );                           // Machea formato para un singnificado
        if( !m.Success || m.Index>Idx  )                            // No macheo un significado
          return MatchCmds( s, Idx );                               // Trata de matchear comandos

        WordMean_ WM = new WordMean_();                               // Crea objeto para guardar significado

        WM.Mean = m.Groups["mean"].Value;                           // Pone significado en los datos (no es opcional)

        if( m.Groups["Esp"].Success )                               // Se especifico la espacialidad
          WM.Esp = m.Groups["Esp"].Value;                           // La pone en los datos

        if( m.Groups["Info"].Success )                              // Se especifico información adicional
          WM.Info = m.Groups["Info"].Value;                         // La pone en los datos

        for( int i=0; i<m.Groups["Att"].Captures.Count; ++i )       // Recorre todos los atributos capturados
          {
          char c = m.Groups["Att"].Captures[i].Value[0];            // Obtiene primer caracter

               if( c == '$' ) WM.Gen  = Genero.Femen;               // Se especifico genero femenino
          else if( c == '-' ) WM.Gen  = Genero.Neutro;              // Se especifico genero neutro
          else if( c == '*' ) WM.Plur = true;                       // Se especifico número prural
          else if( c == '!' ) WM.Refl = true;                       // Se especifico que es una referencia
          }

        Means.Add( WM );                                            // Adiciona significado a la lista de significados del tipo

        Idx += m.Length;                                            // Salta la parte analizada
        if( s.Length == Idx )                                       // Analizo hasta el final de la cadena
          return true;                                              // Retorna OK

        if( s[Idx++] != ',' ) return false;                         // No existe separador entre significados
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene los datos del tipo gramatical a traves de un stream</summary>
    //------------------------------------------------------------------------------------------------------------------
    public static MeanSet_ FromStream(BinaryReader buff)
      {
      var wt = new MeanSet_();                                      // Crea objeto para datos del tipo

      wt.sType = new string( buff.ReadChars(2) );                   // Lee el tipo simple

      int nMeans = buff.ReadByte();                                 // Lee el número de significados
      if( nMeans<=0 || nMeans>100 ) return null;                    // Verifica que el número de significados sea correcto
            
      int nCmds = buff.ReadByte();                                  // Lee el número de comandos

      for( int i=0; i<nMeans; ++i )                                 // Lee todos los tipos significados
        {
        WordMean_ wm = WordMean_.FromStream( buff );                  // Obtiene la información del significado
        if( wm==null ) return null;                                 // Hubo error el obtener el significado
            
        wt.Means.Add( wm );                                         // Adiciona significado a la lista
        }

      for( int i=0 ;i<nCmds; ++i )                                  // Lee el número de comandos
        {
        //IDictCmd cd = MngDictCmds.FromStream( buff );               // Continua leyendo los comandos
        //if( cd == null ) return null;                               // Hubo error el obtener el comando

        //wt.Cmds.Add( cd );                                          // Adiciona comando a la lista
        }

      return wt;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena que contiene los datos de un tipo gramatical</summary>
    //------------------------------------------------------------------------------------------------------------------
    public override string ToString()
      {
      int n = Means.Count;
      var s = new StringBuilder( 15*n );                            // Reserva memoria para significados

      for( int i=0; i<n; ++i )                                      // Recorre todos los significados
        {
        s.Append( Means[i].ToString(i==0) );                        // Agrega significados (diferenciando el primero)

        if( i<n-1 ) s.Append(',');                                  // Si no es el ultimo agrega un separador
        }

      foreach (var cmd in Cmds )                                    // Recorre todos los comados
        s.Append( ',' + cmd.ToString() );                           // Los agrega al final

 	    return s.ToString();                                          // Retorna la cadena
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un stream que contiene los datos de un tipo gramatical</summary>
    //------------------------------------------------------------------------------------------------------------------
    internal byte[] ToStream()
      {
      int n1 = Means.Count;
      int n2 = Cmds.Count;

      MemoryStream ms = new MemoryStream( 15*(n1+n2) );
      var buff = new BinaryWriter(ms);

      buff.Write(sType[0]);                                         // Escribe primara letra del tipo gramatical simple
      buff.Write(sType[1]);                                         // Escribe segunda letra del tipo gramatical simple

      buff.Write( (byte)(n1) );                                     // Escribe el número de significados
      buff.Write( (byte)(n2) );                                     // Escribe el número de comandos

      for( int i=0; i<n1; ++i )                                     // Recorre todos los significados
        buff.Write( Means[i].ToStream() );                          // Obtiene stream de bytes con los significados y los escribe

      for( int i=0; i<n2; ++i )                                     // Recorre todos los comandos
        buff.Write( Cmds[i].ToStream() );                           // Convierte comando a arreglo de bytes y lo escribe

 	    return ms.ToArray();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena JSON que contiene los datos de un tipo gramatical</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string ToJSON()
      {
      int n = Means.Count;
      var s = new StringBuilder( 15*n );                            // Reserva memoria para significados

      s.Append('{');                                                // Inicio de objeto tipo gramatical simple
      s.Append( "t:" + GramType.Code(sType) + "," );                // Pone el tipo gramatical simple

      s.Append( "c:\"" );                                           // Agrega comandos asosiados al tipo
      for( int i=0; i<Cmds.Count; ++i )                             // Recorre todos los comandos
        {
        if( i>0 ) s.Append( ',' );                                  // Si no es el primero, agrega separador

        s.Append( Cmds[i].ToString() );                             // Agrega comando
        }
                                                                    // Cierra la cadena de comandos
      s.Append( "\"," );


      s.Append("Means:[");                                          // Inicio de arreglos de significados
      for( int i=0; i<n; ++i )                                      // Recorre todos los significados
        {
        if( i != 0 ) s.Append(',');                                 // Agrega separador, si no es el primero
        s.Append( Means[i].ToJSON() );                              // Agrega significado
        }
      s.Append(']');                                                // Fin de arreglo de significados

      s.Append('}');                                                // Fin de objeto tipo gramatical simple
 	    return s.ToString();                                          // Retorna la cadena
      }


    //------------------------------------------------------------------------------------------------------------------
    // Trata de buscar todos los comando de que se espcifiquen en 's', si en 's' existe otra cosa que no sea un comando
    // retorna false en otro caso retorna true
    //------------------------------------------------------------------------------------------------------------------
    private bool MatchCmds( string s, int Idx )
      {
      for(;;)                                                       // Recorre todos los comandos
        {
        //var Cmd = MngDictCmds.FormStr( s, ref Idx );                // Trata de obtener un comando
        //if( Cmd == null ) return false;                             // No lo pudo obtener, retorna falso

        //Cmds.Add( Cmd );                                            // Adicciona el comando a la lista

        //if( s.Length == Idx )                                       // Analizo hasta el final de la cadena
          return true;                                              // Retorna OK

        //if( s[Idx++] != ',' ) return false;                         // No es un separador de comando, error
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    WordMean                                     *********************************/
/**********************************************************************************************************************************/

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena la información correspondiente a un significado de una palabra</summary>
  //------------------------------------------------------------------------------------------------------------------
  public class WordMean_  
    {
    Genero    gen  = 0;
    bool   plur = false;
    bool   refl = false;
    string esp  = "GG";
    string mean = "";
    string info = "";

    public Genero Gen  { get{return gen; } set{gen =value;} }
    public bool   Plur { get{return plur;} set{plur=value;} }
    public bool   Refl { get{return refl;} set{refl=value;} }
    public string Esp  { get{return esp; } set{esp =value;} }
    public string Mean { get{return mean;} set{mean=value;} }
    public string Info { get{return info;} set{info=value;} }

    static string Esps = "GG,AE,AG,AN,AQ,AJ,AU,BI,BT,BO,CC,CL,CO,CP,CS,DE,LA,AB,ED,EN,FA,FE,FG,FL,FI,GO,GR,HI,PR,IN,JE,LI,LO,NA,MA,ME,MT,ML,MN,MI,MU,PE,PO,EC,QM,SO,TP,VE,ZO";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el indice correspondiente a la espacialidad 'esp'</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public int GetEspIdx( string esp )
      {
      int Idx = Esps.IndexOf(esp);
      if( Idx<0 ) Idx = 0;
      return (Idx/3);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el código de la especialidad conociendo su indice 'Idx'</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public string GetEspCode( int Idx )            
      {
      int i = 3*Idx;
      if( i<0 || i>= (Esps.Length-2) ) return "GG";

      return Esps.Substring(i,2);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto con los datos de un significado a partir de un stream</summary>
    //------------------------------------------------------------------------------------------------------------------
    public static WordMean_ FromStream(BinaryReader buff)
      {
      int nBytes = buff.ReadByte();                                       // Lee el tamaño del significado
      if( nBytes<=0  ) return null;                                       // Verifica que el tamaño sea correcto

      byte Attr = buff.ReadByte();                                        // Lee los atributos del significado

      byte[] bytes = buff.ReadBytes( nBytes-1 );                          // Lee el significado
      var sMean    = Encoding.UTF8.GetString( bytes, 0, nBytes-1 );       // Lo convierte a cadena UTF8

      var wm = new WordMean_();

      if( (Attr & 0x01) !=0 ) wm.Gen = Genero.Femen;                      // Bit 1- Femenino/masculino
      if( (Attr & 0x02) !=0 ) wm.Gen = Genero.Neutro;                     // Bit 2- Neutro
      if( (Attr & 0x04) !=0 ) wm.Plur = true;                             // Bit 3- Plurar/singular
      if( (Attr & 0x08) !=0 ) wm.Refl = true;                             // Bit 4- Reflexivo
      if( (Attr & 0x10) !=0 )                                             // Bit 5- Tiene especialidad
        {
        wm.Esp  = sMean.Substring(0,2);                                   // Extrae dos letras de especilidad
        wm.Mean = sMean.Substring(2);                                     // Extrae la parte del significado
        }
      else
        {
        wm.Esp  = "GG";                                                   // Toma especialidad general
        wm.Mean = sMean;                                                  // Toma significado
        }

      return wm;                                                          // Retorna el objeto con el significado
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena que representa los datos del significado</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string ToString( bool firt )
      {
      var sMean = new StringBuilder("");

      if( !firt )                                                       // El primero no lleva especilidad ni Info
        {
        sMean.Append("@");                                              // Agrega marcador

        if( info.Length>0 ) sMean.Append( '[' + info + ']' );           // Si hay información la agrega
        else                sMean.Append( esp );                        // Agrega especialidad
        }

      sMean.Append( '"' + mean + '"' );                                 // Agrega significado

      if( plur     ) sMean.Append( "*" );                               // Agrega número si no es singular
      if( gen != 0 ) sMean.Append( (gen==Genero.Femen)? "$" : "-" );    // Agrega genero, si no es masculino
      if( refl     ) sMean.Append( "!" );                               // Agraga si es verbo reflexivo

 	    return sMean.ToString();
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una stream que representa los datos del significado</summary>
    //------------------------------------------------------------------------------------------------------------------
    internal byte[] ToStream()
      {
      MemoryStream ms = new MemoryStream( mean.Length+2 );      // Crea stream en memoria
      var buff = new BinaryWriter(ms);                          // Crea writer binario para escribir

      bool   Esp = esp != "GG";                                 // Indica si hay especialidad

      string dat = "";                                          // Datos
      if( Esp ) dat = esp;                                      // Si hay especilidad la agrega
      dat +=  mean;                                             // Agrega el significado

      byte Attr = 0;                                            // Campo de bits con los atributos del significado
      if( gen==Genero.Femen  ) Attr |= 0x01;                    // Bit 1- Femenino/masculino
      if( gen==Genero.Neutro ) Attr |= 0x02;                    // Bit 2- Neutro
      if( plur               ) Attr |= 0x04;                    // Bit 3- Plurar/singular
      if( refl               ) Attr |= 0x08;                    // Bit 4- Reflexivo
      if( Esp                ) Attr |= 0x10;                    // Bit 5- Tiene especialidad

      var bytes = Encoding.UTF8.GetBytes(dat);                  // Covierte datos a bytes UTF8

      buff.Write( (byte)(bytes.Length + 1) );                   // Escribe el tamaño de los datos
      buff.Write( Attr );                                       // Escribe los atributos

      buff.Write( bytes );                                      // Escribe los datos

 	    return ms.ToArray();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena JSON que representa los datos del significado</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string ToJSON()
      {
      var s = new StringBuilder(50);

      s.Append('{');                                              // Inicio de objeto significado
      s.Append( "m:\"" + mean + "\"," );                          // Agrega significado

      s.Append( "e:" + GetEspIdx(esp) + ',' );                    // Agrega especialidad
      s.Append( "g:" + ((int)gen).ToString() + ',' );             // Agrega genero, si no es masculino
      s.Append( "n:" + (plur? "1":"0") + ',' );                   // Agrega número si no es singular
      s.Append( "r:" + (refl? "1":"0") + ','  );                  // Agraga si es verbo reflexivo

      s.Append( "i:\"" + info + '"' );                            // Agreaga información adicional del significado

      s.Append('}');                                              // Fin de objeto significado
 	    return s.ToString();
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena que representa los datos del significado</summary>
    //------------------------------------------------------------------------------------------------------------------
    public override string ToString()
      {
 	    return ToString(false);
      }

    //------------------------------------------------------------------------------------------------------------------

    }

  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Guarda información sobre los terminos</summary>
  //------------------------------------------------------------------------------------------------------------------
  public class TermInfo
    {
    public string Esp = "";                           // Especialidad en que la palabra es un termino
    public string Typ = "";                           // Tipo gramatical que se asume para el termino

    public TermInfo( string Esp, string Typ )
      {
      this.Esp = Esp;
      this.Typ = Typ;
      }
    //------------------------------------------------------------------------------------------------------------------
    }
  }
