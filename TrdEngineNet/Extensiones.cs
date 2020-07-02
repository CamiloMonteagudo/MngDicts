using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine
  {
  public static class Extensiones
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Machea el caracter 'cEnd' con 'cIni' teniendo en cuenta los anidamientos y empezando en 'Idx'</summary>
    ///
    ///<param name="Idx">Indice dentro de la cadena donde se inicia la busqueda</param> 
    ///<param name="cIni">Caracter inicial del macheo</param>
    ///<param name="cEnd">Caracter final del macheo</param>
    ///<param name="skip">Determina si se tienen en cuenta los caracteres entre comillas o no</param>
    ///
    ///<remarks>Los caracteres 'cIni' y 'cEnd' deben ser distintos y el caracter s[Idx] dede ser 'cIni'</remarks>
    ///<returns>Retoran el indice donde termina el macheo, o -1 si no hay macheo</returns>
    public static int MatchChar( this string s, ref int Idx, char cIni, char cEnd, bool skip=true )
      {
      int iMatch = 0 ;                                              // Define nivel de anidamiento
      bool Match = true;                                            // Define si esta entre comillas o no (se machea o no)
                                                                    
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control
                                                                    
      int i = Idx;                                                  
      if( s[i] != cIni ) return -1;                                 // El primer caracter debe coincidir con el inicial
                                                                    
      for( ; i < s.Length; ++i )                                    // Recorre todos los caracteres
        {                                                           
             if( Match && s[i] == cIni ) ++iMatch;                  // Caracter inicial, aumenta indice de anidamiento
        else if( Match && s[i] == cEnd )                            // Caracter final
               {                                                    
               --iMatch;                                            // Disminulle indice de anidamiento
               if( iMatch<=0 )  return i;                           // Anidamiento 0, termina la busqueda
               }                                                    
        else if( skip && s[i] == '"' )                              // Si encuentra una comilla
               Match = !Match;                                      // Activa/desactiva el macheo
        else if( s[i]<' ') Match = true;                            // Si caracter de control, activa macheo
        }                                                           
                                                                    
      return -1;                                                    // No se pudieron machear los dos caracteres
      }                                                             

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Machea dos caracteres 'c' a partir de 'Idx' que debe apuntar al primer caracter 'c'</summary>
    ///
    ///<param name="Idx">Indice dentro de la cadena donde se inicia la busqueda</param> 
    ///<param name="c">Caracter que se quiere machear</param>
    ///
    ///<returns>Retoran el indice donde termina el macheo, o -1 si no hay macheo</returns>
    public static int MatchChar( this string s, int Idx, char c )
      {
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control

      if( s[Idx] != c ) return -1;
      ++Idx;

      return s.IndexOf( c, Idx);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la cadena entre los caracteres 'cIni' y 'cEnd' teniendo en cuenta los anidamientos y empezando en 'Idx'</summary>
    ///
    ///<param name="Idx">Indice dentro de la cadena donde se inicia la busqueda</param> 
    ///<param name="cIni">Caracter inicial del macheo, inicialmente el caracter apuntado por Idx debe ser cIni</param>
    ///<param name="cEnd">Caracter final del macheo</param>
    ///<param name="skip">Determina si se tienen en cuenta los caracteres entre comillas o no</param>
    ///
    ///<returns>Retorna la cadena entre los caracteres o null si hay error</returns>
    public static string GetBetween( this string s, ref int Idx, char cIni, char cEnd, bool skip=true )
      {
      if( cIni==cEnd )                                              // Si los caracteres inicial y final son iguales
        return s.GetBetween( ref Idx, cIni);                        // Utiliza la versión para un solo caracter
                                                                    
      int IEnd = s.MatchChar( ref Idx, cIni, cEnd, skip );          // Machea los caracteres
      if( IEnd==-1 ) return null;                                   // No los pudo machear
                                                                    
      var sRet = s.Substring( Idx+1, IEnd-Idx-1);                   // Obtiene lo que hay entre los caracteres
                                                                    
      Idx = IEnd + 1;                                               // Actualiza el parametro de salida
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control
                                                                    
      return sRet.Trim();                                           // Retorna cadena sin espacios al principio y final
      }                                                             

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la cadena entre dos caracteres 'c' a partir de 'Idx' que debe apuntar al primer caracter 'c'</summary>
    ///
    ///<param name="Idx">Indice dentro de la cadena donde se inicia la busqueda</param> 
    ///<param name="c">Caracter que se quiere machear</param>
    ///
    ///<returns>Retorna la cadena entre los dos caracteres o null si hay error</returns>
    public static string GetBetween( this string s, ref int Idx, char c )
      {
      int IEnd = s.MatchChar( Idx, c );
      if( IEnd==-1 ) return null;

      var sRet = s.Substring( Idx+1, IEnd-Idx-1);
      Idx = IEnd+1;
      return sRet;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta en caracter 'c' partir de 'Idx' sin tener en cuenta los caracteres de control</summary>
    /// <param name="c">Caracter que se quiere saltar</param>
    /// <param name="Idx">Indice donde empieza la busqueda</param>
    /// 
    /// <returns>Verdadero si el caracter c es saltado e Idx contiene el indice del proximo caracter a partir de c que no es de control
    /// en otro caso Idx no cambia</returns>
    public static bool SkipChar( this string s, char c, ref int Idx )
      {
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control
                                                                    
      int i = Idx;                                                  
      if( s[i] != c ) return false;                                 // Comprueba si esta el caracter a saltar
                                                                    
      ++i;                                                          // Salta el caracter
      while( i<s.Length && s[i]<=' ' ) ++i;                         // Salta espacios y caracteres de control
                                                                    
      Idx = i;                                                      // Actualiza el indice de retorno
      return true;                                                  
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee hasta el final de la linea empezando en 'Idx', luego salta caracters e control incluido espacios y quita los caractres en blanco</summary>
    /// 
    /// <param name="Idx">Indice donde empieza la linea</param>
    /// 
    /// <returns>Cadena con el contenido de la linea, si no hay fin de linea lee hasta el final, </returns>
    public static string GetLine( this string s, ref int Idx )
      {
      char[] lEnds = new char[] {'\r','\n'};                        // Caracteres que definen fin de linea
                                                                    
      int i = s.IndexOfAny(lEnds, Idx );                            // Busca uno de los caracteres fin de linea
      if( i==-1 ) i = s.Length;                                     // No hay ninguno, se pone al final
                                                                    
      while( i<s.Length && (s[i]=='\r' || s[i]=='\n' )) ++i;        // Salta caracteres fin de linea
                                                                    
      var line = s.Substring( Idx, i-Idx );                         // Coje subcadena hasta el final
      Idx = i;                                                      // Actualiza el indice hasta donde llego
      return line.Trim();                                           // Quita los caracteres que sobran                            
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un nombre compuesto por letras mayusculas saltado los caractres de control a inicio y al final</summary>
    /// 
    /// <param name="Idx">Indice donde empieza buscarse el nombre</param>
    public static string GetName( this string s, ref int Idx )
      {
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control
                                                                    
      int i = Idx;                                                  
                                                                    
      while( i<s.Length && s[i]>='A' && s[i]<='Z') ++i;             
      if( i==Idx ) return null;                                     
                                                                    
      var FName = s.Substring( Idx, i-Idx );                        
                                                                    
      while( i<s.Length && s[i]==' ' ) ++i;                         // Salta espacios
      Idx = i;                                                      
      return FName;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un valor que consite en una cadena, numero o constante </summary>
    /// 
    /// <param name="Idx">Indice donde empieza buscarse el valor</param>
    public static string GetValue( this string s, ref int Idx )
      {
      int n = s.Length;
      while( Idx<n && s[Idx]==' ' ) ++Idx;                          // Salta espacios y caracteres de control

      int i = Idx;
      if( i>=n ) return null;                                       // Solo hay espacios, retorna error
        
      if( s[i]=='"' )                                               // Si empieza por comillas
        {                                                           
        ++i;                                                        // Salta la comilla inicial
        while( i<n && s[i]!='"' ) ++i;                              // Busca la proxima comilla
        if( s[i]!='"' )  return null;                               // No encontro la segunda comilla, retorna error
        ++i;                                                        // Salta la comilla final
        }
      else if( char.IsDigit(s[i]) )                                 // Si empieza con un número
        {
        while( i<n && char.IsDigit(s[i]) ) ++i;                     // Salta todos los números
        }
      else if( char.IsUpper(s[i]) )                                 // Si empieza con una letra mayuscuala
        {
        while( i<n && (char.IsLetter(s[i]) || s[i]=='?') ) ++i;     // Salta todas las letras
        }
      else return null;                                             // Comienza con otra cosa, error

      var sValue = s.Substring( Idx, i-Idx );                       // Obtiene la cadena

      while( i<n && s[i]<=' ' ) ++i;                                // Salta espacios finales
      Idx = i;                                                      // Actualiza indice de retorno
      return sValue;                                                // Retorna el valor
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Codifica los caracteres de formato XML que esten en la cadena</summary>
    public static string XmlEncode( this string  txt )
      {
      var s = new StringBuilder(txt);

      s.Replace("&" , "&amp;"  );
      s.Replace("<" , "&lt;"   );
      s.Replace(">" , "&gt;"   );
      s.Replace("\"", "&quot;" );
      s.Replace("'" , "&apos;" );

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Expande las constantes que se encuentran en la cadena, de acuerdo al diccionario de definiciones 'Defs', el proceso de
    /// expansion tambien incluye quitar las comillas</summary>
    public static string Expand(this string s, Dictionary<string, string> Defs, bool expand )
      {
      if( string.IsNullOrEmpty(s) ) return s;                       // Si es una cadena vacia o nula, no hay nada que hacer

      var sNew = new StringBuilder( "", s.Length );                 // Cadena nueva, con constantes sustituidas
      int i = 0;                                                    
                                                                    
      for(;;)                                                       // Recorre toda la cadena original
        {
        try
          {
        if( s[i]=='"' )                                             // Si encuentra comillas
          {                                                         
          sNew.Append( s.GetBetween(ref i, '"') );                  // Salta todo y quita las comillas
          }                                                         
        else                                                        
          {
          int j=i;                                                  // Guarda posición de inicio
          while( i<s.Length && s[i]!=',') ++i;                      // Salta hasta la proxima coma                         
          var def = s.Substring( j, i-j );                          // Obtiene el valor de la constante

          if( expand && Defs!=null && Defs.ContainsKey(def) )       // Si la constante se encuentra en el diccionario
            sNew.Append( Defs[def] );                               // Resplaza la constante por su valor en el diccionario
          else                                                      
            sNew.Append( def );                                     // Agrega la constante como estaba originalmente
          }                                                         
                                                                    
        if( i==s.Length ) return sNew.ToString();                   // Llego al fin, retorna la cadena
          
        if( !s.SkipChar(',', ref i ) || i==s.Length ) 
          {
          Dbg.Msg( "WARNING: hay problemas con el separador en " + s );
          return sNew.ToString();
          }
                             
          sNew.Append( ',' );                                         // Agrega una coma en la cadena nueva
          }
        catch
          {
          Dbg.Msg("Error....");
          }                                      
        }                                                           
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Divide la cadena en subcadenas separadas por 'sep' y retorna una HashSet con todas ellas</summary>
    public static HashSet<string> ToHashSet( this string s, char sep=',' )
      {
      HashSet<string> lstWords = new HashSet<string>();             // Crea el set de palabras
                                                                    
      var lst = s.Trim('"').Split(sep);                             // Quita las comillas y obtiene palabras separadas por 'sep'
                                                                    
      foreach( var subStr in lst )                                  // Copia la lista hacia el set de palabras
        lstWords.Add( subStr );                                     
                                                                    
      return lstWords;                                              // Retorna set de palabras
      }                                                             

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Toma una colección de cadenas y las une en una cadena unica, usando el separador 'sep'</summary>
    public static string ToOneString( this IEnumerable<string> set, char sep=',' )
      {
      var s = new StringBuilder( 5 * set.Count() );                 // Reserva memoria
            
      var first = true;                                             // Marcador para la primera cadena
      foreach( var ss in set )                                      // Obtiene todas las cadenas del hashset
        {                                                           
        if( !first && sep!='\0' )                                   // Si no es primera cadena y el separador no es nulo
          s.Append(sep);                                            // Agrega un separador

        s.Append(ss);                                               // Agrega la cadena
        first = false;                                              // Marca que ya no el la primera palabra
        }                                                           
                                                                    
      return s.ToString();                                          // Retorna la cadena completa
      }                                                             

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Divide una cadena en subcadenas, sin tener en cuenta los separadores entre comillas</summary>
    public static string[] SplitArg( this string s, char sep=',' )
      {
      List<string> Strs = new List<string>();

      bool Match = true;                                            // Define si esta entre comillas o no (se machea o no)
        
      int Ini = 0, end = 0;                                                      
      for( int i=0 ;; ++i )                                    // Recorre todos los caracteres
        {                                                           
        if( i==s.Length || (s[i] == sep && Match) ) 
          {
          if( i==s.Length ) end=s.Length;

          Strs.Add( s.Substring( Ini, end-Ini) );

          Ini = end = (i+1);
          if( i<s.Length ) continue; else break;
          }

        if( s[i] == '"' ) Match = !Match;

        ++end;
        }                                                           
                                                                    
      return Strs.ToArray();     
      }                                                             

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Convierte la primera letra de la cadena a mayusculas</summary>
    public static string FirstToUpper( this string s )
      {
      if( string.IsNullOrEmpty(s) ) return s;

      var Tmp = new StringBuilder(s);
      Tmp[0] = char.ToUpper(Tmp[0]); 

      return Tmp.ToString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Convierte la primera letra de la cadena a mayusculas</summary>
    public static string FirstToLower( this string s )
      {
      if( string.IsNullOrEmpty(s) ) return s;

      var Tmp = new StringBuilder(s);
      Tmp[0] = char.ToLower(Tmp[0]); 

      return Tmp.ToString();
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE Extensiones  +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE            +++++++++++++++++++++++++++++++++++++++++++++
