﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TrdEngine.TrdProcess;
using System.Xml.Linq;

namespace TrdEngine.Dictionary
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena la información correspondiente a los datos de traducción de una palabra</summary>
  //------------------------------------------------------------------------------------------------------------------
  public partial class WordData
    {
    //------------------------------------------------------------------------------------------------------------------
    // Datos de la clase
    //------------------------------------------------------------------------------------------------------------------
    public List<MeanSet> MeanSets = new List<MeanSet>(5);                   // Información para los tipos gramaticales
    string cType = "";                                                      // Tipo compuesto de la palabra
    public string M = "";                                                          // Información que viene con #M

    ///<summary>Tipo compuesto o principal de la palabra</summary>
    public string CompType { get{return cType;}} 

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea datos de palabras con un solo significado</summary>
    public static WordData CreateSingle( string cType, string Mean )
      {
      var WData = new WordData();                                     // Crea objeto datos de palabra

      WData.cType = cType;                                            // Le pone el tipo

      var WMean  = new WordMean();                                    // Crea objeto significado
      WMean.Mean = Mean;                                              // Le pone el significado

      var Means = new MeanSet(null);                                  // Crea objeto grupo de significados
      Means.Means.Add( WMean );                                       // Le agrega el significado

      WData.MeanSets.Add(Means);                                      // Adiciona grupo de significados a datos de palabra

      return WData;                                                   // Retorna datos de palabra
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un Stream que contiene los datos de una palabra</summary>
    public byte[] ToStream()
      {
      MemoryStream ms = new MemoryStream(600);                        // Crea un stream en memoria
      var Wtr = new BinaryWriter(ms);                                 // Le asocia un Writer binario

      Wtr.Write( cType[0] );                                          // Escribe primera letra del tipo gramatical
      Wtr.Write( cType[1] );                                          // Escribe segunda letra del tipo gramatical

      if( M.Length==0 )
        Wtr.Write( (byte)0 );                                         // Marca que no hay datos adiccionales
      else
        {
        var bytes = Encoding.UTF8.GetBytes(M);                        // Obtiene bytes que representa la cadena en UTF8
        Wtr.Write( (byte)bytes.Length );                              // Escribe el tamaño de la cadena
        Wtr.Write( bytes );                                           // Escribe el contenido de la cadena
        }

      Wtr.Write( (byte)MeanSets.Count );                              // Escribe el número de grupos de significados
      foreach( var NSet in MeanSets )                                 // Recorre todos los grupos de significados
        NSet.ToStream( Wtr );                                         // Obtiene su representación en stream

       return ms.ToArray();                                            // Retorna un arreglo de bytes
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto WordData a partir un stream de bytes</summary>
    //------------------------------------------------------------------------------------------------------------------
    public static WordData FromStream( byte[] Bytes )
      {
      var wd = new WordData();
      MemoryStream ms = new MemoryStream(Bytes);                      // Crea un stream en memoria con el arreglo de bytes
      var Rdr = new BinaryReader( ms, Encoding.UTF8 );                // Le asocia un lector (Reader) para leer datos binarios

      char c1 = Rdr.ReadChar();                                       // Lee el primer caracter del tipo
      char c2 = Rdr.ReadChar();                                       // Lee el segundo caracter del tipo
      if( !char.IsUpper(c1) || !char.IsUpper(c2) )                    // Verifica que ambos sean letras mayusculas
        return null;

      wd.cType = new string( new char[]{c1,c2} );                     // Covierte caracteres a cadena

      byte MLen = Rdr.ReadByte();                                     // Obtiene tamaño de la información adicional
      if( MLen>0 )                                                    // Mayor que 0, implica hay informción adicional
        {
        var bytes = Rdr.ReadBytes( MLen );                            // Lee el número de bytes de información
        wd.M = Encoding.UTF8.GetString( bytes );                      // Convierte los bytes en cadena UTF8
        }

      byte nMSet = Rdr.ReadByte();                                    // Obtiene número de grupos de significados
      for( int i=0; i<nMSet; ++i  )                                   // Recorre todos los significados
        {
        var MSet = MeanSet.FromStream(Rdr);                           // Obtiene grupo de significados desde el stream
        if( MSet==null ) return null;

        wd.MeanSets.Add( MSet );                                      // Adiciona el grupo de significados
        }

      return wd;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un dato nuevo producto de la mezcla de la información de dos tipos</summary>
    public static WordData MergeData( WordData WData1, WordData WData2 )
      {
      if( WData1.MeanSets.Count < WData2.MeanSets.Count )             // Si el segundo dato tiene mas meanset que el primero
        {var tmp=WData1; WData1=WData2; WData2=tmp; }                 // Intercambia los datos

      var WData   = new WordData();                                   // Crea objeto nuevo de datos de palabra vacio
      WData.cType = WData1.cType;                                     // Le asigna el tipo del primer dato
      WData.M     = WData1.M;                                         // Le asigna información semantica del primer dato

      foreach( var set in WData1.MeanSets )                           // Recorre todos meanset del primer dato
        WData.MeanSets.Add( set );                                    // Los adiciona a los datos nuevos

      for( int i=0; i<WData2.MeanSets.Count; ++i )                    // Recorre todos los meanset de segundo dato
        {
        var MSet2 = WData2.MeanSets[i];                               // Toma meanset actual
        if( MSet2.Conds==null && i>0 ) break;                         // Si no tiene condiciones y no es el primero lo ignora

        if( MSet2.Conds==null )                                       // Si no tiene condiciones (hay un solo meanset)
          MSet2 = MSet2.Duplicate( WData2.CompType );                 // Crea un duplicado con el tipo como condición

        var sCond2 = MSet2.Conds.ToString();                          // Toma la cadena que representa a las condiciones

        bool Add = true;                                              // Por defecto hay que adicionarla
        for( int j=0; j<WData1.MeanSets.Count; ++j )                  // Recorre significados del primer dato
          {
          var MSet = WData1.MeanSets[j];                              // Obtiene significado actual
          if( MSet.Conds==null && j>0 ) break;                        // Si no tiene condiciones y no es el primero termina

          string sCond;
          if( MSet.Conds==null )                                      // Si no tiene condiciones (hay un solo meanset)
            sCond = "W=" + WData1.CompType;                           // Crea reprersentación de la condición con el tipo gramatical
          else                                                        // Si hay condiciones
            sCond = MSet.Conds.ToString();                            // Obtiene la cadena que representa a la condición

          if( sCond==sCond2 ) { Add=false; break; }                   // Condiciones iguales, existe en los dos, no hay que adicionarla
          }

        if( Add )                                                     // Si hay que adicionarla
          WData.MeanSets.Insert( WData.MeanSets.Count-1, MSet2 );     // Inserta el dato nuevo, antes del ultimo dato
        }

      return WData;                                                   // Retorna datos nuevos, mezcaldos
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Resplaza la subcadena 'oldStr' por 'newStr' en todas los significados de la palabra</summary>
    public WordData Replace( string oldStr, string newStr )
      {
      var WData   = new WordData();                                   // Crea objeto datos de palabra vacio
      WData.cType = cType;                                            // Copia el tipo principal a datos nuevos
      WData.M     = M;                                                // Copia información semantica a datos nuevos

      foreach( var set in MeanSets )                                  // Copia todos los significados
        {
        var MSet = new MeanSet( set.Conds );                          // Crea un conjunto de significados nuevo
        WData.MeanSets.Add( MSet );                                   // Lo Adiciona a los datos nuevos

        MSet.Cmds  = set.Cmds;                                        // Copia los comandos
        MSet.nSkip = set.nSkip;                                       // Copia número de grupos de significados a saltar si la condición es falsa

        foreach( var mean in set.Means )                              // Recorre todos los significados para copiarlos
          {
          var Mean = new WordMean();                                  // Crea significado nuevo
          MSet.Means.Add( Mean );                                     // La adiciona a grupos de significados nuevo
          
          Mean.Gen  = mean.Gen;                                       // Copia el genero al significado nuevo
          Mean.Plur = mean.Plur;                                      // Copia si es plurar al significado nuevo
          Mean.Refl = mean.Refl;                                      // Copia si es reflexivo al significado nuevo
          Mean.Esp  = mean.Esp;                                       // Copia si especialidad al significado nuevo
          Mean.Mean = mean.Mean.Replace( oldStr, newStr );            // Copia significado con la subcadena sustituida
          Mean.Info = mean.Info;                                      // Copia cadena de información al significado nuevo
          }
        }

      return WData;                                                   // Retorna objeto con valores cambiados
      }

    //------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    WordType                                     *********************************/
///<summary>Almacena y procesa las condiciones asociadas a un grupo de significados</summary>
//----------------------------------------------------------------------------------------------------------------------------------
  public partial class Conditions
    {
    public const int opNone = 200;                                  // Código para indicar que no hay operador
    public const int opOr   = 201;                                  // Código de la operación 'OR' en el arreglo 'Orden'
    public const int opAnd  = 202;                                  // Código de la operación 'AND' en el arreglo 'Orden'
                                                                    
    public List<IDictFunc> Funcs = new List<IDictFunc>();           // Lista de funciones que forman la condición
    public List<int> Orden = new List<int>();                       // Establece al orden de ejacución de funciones y operadores en notación polaca
                                                                    // inversa, valor menores de 200 son indice a funciones, mayores son operadores

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna una cadena de caracteres que representa las condiciones</summary>
    //public override string ToString()
    //  {
    //  if( Funcs.Count==1 )                                            // Si hay una sola función                              
    //    return Funcs[0].ToString();                                   // Retorna representación de la función

    //  string[] pila = new string[10];                                   
    //  int    ptr  = -1;                                             

    //  char[] sep = {'|', '&'}; 
    //  foreach( var Id in Orden)                                     
    //    {
    //    if( Id < opNone )                                           
    //      pila[++ptr] = Funcs[Id].ToString();
    //    else 
    //      {
    //      var Op2 = pila[ptr];
    //      if( pila[ptr].IndexOfAny(sep)!=-1 )
    //        Op2 = '(' +pila[ptr] + ')';

    //      if( Id==opOr && ptr>=1 )                               
    //        {
    //        pila[ptr-1] = ( pila[ptr-1] + " OR " + Op2 );                
    //        --ptr;                                                    
    //        }
    //      else if( Id==opAnd && ptr>=1  )                             
    //        {
    //        pila[ptr-1] = ( pila[ptr-1] + " AND " + Op2 );                
    //        --ptr;                                                    
    //        }
    //      }
    //    }

    //  return pila[0];                                               // Retorna resultado final
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evalua la condición de la expresión, y determina si se cumple o no, según los datos de la palabra actual
    /// y/o la oración  que se esta analizando</summary>
    public bool Eval( int iWord, List<Word> Words ) 
      { 
      if( Funcs.Count==1 )                                          // La condición solo tiene un función
        return Funcs[0].Eval( iWord, Words );                       // La evalua y retorna sin tener en cuenta el orden

      bool[] pila = new bool[10];                                   // Pila para guardar los resultados de las operaciones
      int    ptr  = -1;                                             // Puntero a la ultima operación en la pila

      foreach( var Id in Orden)                                     // Combina funciones y operadores según la secuencia establecida 
        {
        if( Id < opNone )                                           // Id es un indice a una función
          pila[++ptr] = Funcs[Id].Eval( iWord, Words );             // Ejecuta la función, pone resultado en la pila y avaza puntero
        else if( Id==opOr && ptr>=1 )                               // Id es un operador OR, tienen que haber al menos 2 resultados
          {
          pila[ptr-1] = ( pila[ptr-1] || pila[ptr]);                // Toma los dos ultimos resultados de la pila, ejecuta el OR, pone
          --ptr;                                                    // el resultado en el primero y retraza el puntero a la pila
          }
        else if( Id==opAnd && ptr>=1  )                             // Id es un operador AND, tienen que haber al menos 2 resultados
          {
          pila[ptr-1] = ( pila[ptr-1] && pila[ptr]);                // Toma los dos ultimos resultados de la pila, ejecuta el AND, pone
          --ptr;                                                    // el resultado en el primero y retraza el puntero a la pila
          }
        else
          throw new Exception("Operador incorrecto, en evaluación de condición");
        }

      if( ptr!=0 )                                                  // El resultado final siempre tiene que estar en inicio de la pila
          throw new Exception("Desvalance se operadores, en evaluación de condición");

      return pila[0];                                               // Retorna resultado final
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un stream que representa las condiciones</summary>
    public void ToStream(BinaryWriter Wtr)
      {
      Wtr.Write( (byte)Funcs.Count );                                 // Escribe candidad de funciones de la condición
      foreach( var fun in Funcs )                                     // Recorre todas las funciones
        fun.ToStream( Wtr );                                          // Escribe su stream que lo representa

      if( Funcs.Count==1 ) return;                                    // Si hay una sola función, termina

      Wtr.Write( (byte)Orden.Count );                                 // Escribe cantidad operando y operadores
      foreach( var ord in Orden )                                     // Recorre orden de ejecución
        Wtr.Write( (byte)ord );                                       // Escribe indice al operando ú operador
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene objeto Conditions a partir de un stream</summary>
    public static Conditions FromStream(BinaryReader Rdr, byte nCond)
      {
      var conds = new Conditions();                                   // Crea un objeto nuevo

      for( int i=0; i<nCond; ++i )                                    // Recorre todas las condiciones
        {
        var Fun = Rdr.FunFromStream();                                // Obtiene la función
        if( Fun == null ) return null;                                // No pudo leer la función

        conds.Funcs.Add( Fun );                                       // La agrega a la lista de funciones
        }
      
      if( nCond==1 ) return conds;                                    // Si hay una sola candición termina

      var nOrd = Rdr.ReadByte();                                      // Lee cantidad de elementos del orden de ejecución
      if( nOrd<nCond || nOrd>2*nCond ) return null;                   // El número de elementos es incorrecto

      for( int i=0; i<nOrd; ++i )                                     // Lee cada una de los elementos del orden de ejecución
        conds.Orden.Add( Rdr.ReadByte() );                            // Lee indice a función ú operador logico

      return conds;                                                   // Retorna objeto con las condiciones
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    }

/**********************************************************************************************************************************/
/********************************                    MeanSet                                      *********************************/
///<summary>Almacena la información correspondiente a un grupo de significados</summary>
  public partial class MeanSet  
    {
    ///<summary>Lista de los significados asociados al tipo gramatical</summary>
    public List<WordMean> Means = new List<WordMean>(6);

    ///<summary>Lista de los comandos asociados al tipo gramatical</summary>
    public List<IDictCmd> Cmds = new List<IDictCmd>();

    ///<summary>Codiciones que deben cumplirse para asumir el MeanSet</summary>
    public Conditions Conds = null;

    ///<summary>Número de MeanSets que hay que saltarse en el caso que la condición sea falsa</summary>
    public int nSkip = 1;

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea MeanSet vacio con unas condiciones dadas</summary>
    public MeanSet( Conditions conds )
      {
      Conds = conds;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un stream que representa el grupo de significados</summary>
    public void ToStream(BinaryWriter Wtr)
      {
      if( Conds==null )                                               // Si no hay condiciones
        Wtr.Write( (byte)0 );                                         // Escribe un 0 para marcar 
      else
        Conds.ToStream( Wtr );                                        // Escribe las condiciones

      Wtr.Write( (byte) nSkip );                                      // Escribe cantidad de MeanSet a saltar
      if( nSkip>1 ) return;                                           // Es un grupo, que no tiene significados 

      Wtr.Write( (byte)Means.Count );                                 // Escribe cantidad de grupos de significados
      foreach( var mean in Means )                                    // Recorre todos los significados
        mean.ToStream( Wtr );                                         // Escribe datos del significado

      Wtr.Write( (byte)Cmds.Count );                                  // Escribe cantidad de comandos
      foreach( var cmd in Cmds )                                      // Recorre todos los comandos
        cmd.ToStream( Wtr );                                          // Escribe datos del comando
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un objeto MeanSet desde un stream</summary>
    public static MeanSet FromStream(BinaryReader Rdr)
      {
      Conditions Conds = null;
      var nCond = Rdr.ReadByte();                                     // Lee cantidad de condiciones
      if( nCond >0 )                                                  // Hay condiciones
        {
        Conds = Conditions.FromStream( Rdr, nCond );                  // Obtiene un objeto con las condiciones desde el stream 
        if( Conds == null )  return null;
        }

      var MSet = new MeanSet( Conds );                                // Crea un MeanSet con las condicones asociadas
      var nSkip = Rdr.ReadByte();                                     // Lee cantidad de MeanSet a saltar
      if( nSkip>1 )                                                   // Es un grupo, no tiene significados
        {
        MSet.nSkip = nSkip;                                           // Pone valor a saltar
        return MSet;                                                  // Retorna MeanSet vacio
        }

      var nMean = Rdr.ReadByte();                                     // Lee cantidad de significados en el MeanSet
      for( int i=0; i<nMean; ++i )                                    // Recorre todos los grupos de significados     
        {
        var mean = WordMean.FromStream(Rdr);                          // Obtiene un grupo de significado desde el stream
        if( mean==null ) return null;                                 // No lo pudo obtener, retorna
        
        MSet.Means.Add( mean );                                       // Adiciona el significado a la lista
        }

      var nCmd = Rdr.ReadByte();                                      // Lee cantidad de significados en el MeanSet
      for( int i=0; i<nCmd; ++i )                                     // Recorre todos los grupos de significados     
        {
        IDictCmd cmd = Rdr.CmdFromStream();                           // Obtiene un grupo de significado desde el stream
        if( cmd==null ) return null;                                  // No lo pudo obtener, retorna
        
        MSet.Cmds.Add( cmd );                                         // Adiciona el significado a la lista
        }

      return MSet;                                                    // Retorna el MeanSet creado
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Duplica el Meanset pero poniendole 'sTipo' como condición</summary>
    internal MeanSet Duplicate( string sTipo )
      {
      var conds = new Conditions();                                   // Crea objeto para manejar condiciones

      var tFunc = new DFuncW();                                       // Crea una función del tipo W=...
      tFunc.iArg = 0;                                                 // Especifica que el argumento es un tipo gramatical
      tFunc.sTipo = sTipo;                                            // Pone el tipo gramatical

      conds.Funcs.Add( tFunc );                                       // Adiciona la función a las condiciones

      var MSet = new MeanSet( conds );                                // Crea nuevo meanset con nuevas condiciones
      MSet.Means = Means;                                             // Le asocia los mismos significados
      MSet.Cmds  = Cmds;                                              // Le asocia los mismos comando
      MSet.nSkip = nSkip;

      return MSet;                                                    // Retorna nuevo meanset
      }
    }

/**********************************************************************************************************************************/
/********************************                    WordMean                                     *********************************/
///<summary>Almacena la información correspondiente a un significado de una palabra</summary>
  public partial class WordMean  
    {
    TGen gen  = 0;
    bool   plur = false;
    bool   refl = false;
    string esp  = "GG";
    string mean = "";
    string info = "";

    public TGen Gen  { get{return gen; } set{gen =value;} }
    public bool   Plur { get{return plur;} set{plur=value;} }
    public bool   Refl { get{return refl;} set{refl=value;} }
    public string Esp  { get{return esp; } set{esp =value;} }
    public string Mean { get{return mean;} set{mean=value;} }
    public string Info { get{return info;} set{info=value;} }

    public TNum Num  { get{return (plur)? TNum.Plural : TNum.Singular; } 
                         set{ plur= (value==TNum.Plural);                  } 
                       }

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
    public static WordMean FromStream( BinaryReader Rdr)
      {
      var WMean = new WordMean();

      byte Attr = Rdr.ReadByte();                                   // Obtine de bits con los atributos del significado
      if( (Attr&0xC0)!=0 )  return null;                            // No es valor correcto, los dos ultimos bit deben ser 0

      if( (Attr&0x01)!=0 ) WMean.gen = TGen.Femen  ;                // Bit 1- Femenino/masculino
      if( (Attr&0x02)!=0 ) WMean.gen = TGen.Neutro ;                // Bit 2- Neutro
      if( (Attr&0x04)!=0 ) WMean.plur = true;                       // Bit 3- Plurar/singular
      if( (Attr&0x08)!=0 ) WMean.refl = true;                       // Bit 4- Reflexivo
  
      var mLen   = Rdr.ReadByte();                                  // Lee tamaño del significado
      var bytes  = Rdr.ReadBytes( mLen );                           // Lee bytes del significado
      WMean.mean = Encoding.UTF8.GetString(bytes);                  // Obtiene cadena UTF8

      if( (Attr&0x10) != 0 )                                        // Si hay especialidad
        {
        char[] cEsp = {  Rdr.ReadChar(),  Rdr.ReadChar() };         // Lee los 2 caracteres de la especilidad
        WMean.esp   = new string( cEsp );                           // Covierte caracteres a cadena
        }

      if( (Attr&0x20) != 0 )                                        // Si hay información adicional
        {
        var mInfo  = Rdr.ReadByte();                                // Lee tamaño de la información adicional
        bytes      = Rdr.ReadBytes( mInfo );                        // Lee bytes del tamaño adicional
        WMean.Info = Encoding.UTF8.GetString(bytes);                // Obtiene cadena UTF8
        }

      return WMean;                                                 // Retorna el objeto con el significado
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Escribe en el stream 'Wtr' una representación del significado</summary>
    //------------------------------------------------------------------------------------------------------------------
    public void ToStream( BinaryWriter Wtr )
      {
      byte Attr = 0;                                                // Campo de bits con los atributos del significado
      if( gen==TGen.Femen  ) Attr |= 0x01;                          // Bit 1- Femenino/masculino
      if( gen==TGen.Neutro ) Attr |= 0x02;                          // Bit 2- Neutro
      if( plur             ) Attr |= 0x04;                          // Bit 3- Plurar/singular
      if( refl             ) Attr |= 0x08;                          // Bit 4- Reflexivo
      if( esp != "GG"      ) Attr |= 0x10;                          // Bit 5- Tiene especialidad
      if( info.Length > 0  ) Attr |= 0x20;                          // Bit 6- Tiene información adiccional

      Wtr.Write( Attr );                                            // Escribe los atributos
  
      var bytes = Encoding.UTF8.GetBytes(mean);                     // Obtiene bytes de mean en formato UTF8

      Wtr.Write( (byte)bytes.Length );                              // Escribe tamaño del significado en UTF8
      Wtr.Write( bytes );                                           // Escribe el significado en UTF8

      if( (Attr&0x10) != 0 )                                        // Si hay especialidad
        {
        Wtr.Write( (byte)esp[0] );                                  // Escribe primera letra de la especilidad
        Wtr.Write( (byte)esp[1] );                                  // Escribe segunda letra de la especilidad
        }

      if( (Attr&0x20) != 0 )                                        // Si hay información adicional
        {
        bytes = Encoding.UTF8.GetBytes(info);                       // Obtiene bytes de info en formato UTF8

        Wtr.Write( (byte)bytes.Length );                            // Escribe tamaño de info en UTF8
        Wtr.Write( bytes );                                         // Escribe el contenido de info en UTF8
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    }

  }
