using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrdEngine.TrdProcess
  {
  // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  // Divide una oración en palabras y le pone datos de interés para la traducción
  public class ParseWords
    {
    Translate  Trd;                                                   // Datos de la traducción
    string     Orac;                                                  // Oracion que se esta procesando
    int        Len;                                                   // Cantidad de caracteres de la oración
    TLng       Lng;                                                   // Idioma de origen de la raducción

    int nChar = 0;                                                    // Cantidad de caracteres alfanumericos encontrados
    int nNum  = 0;                                                    // Cantidad de caracteres númericos encontrados
    int nUpr  = 0;                                                    // Cantidad de letras máyusculas encontradas
    int nRom  = 0;                                                    // Cantidad de letras que forman números romanos encontradas

    public List<Word> Wrds;                                           // Lista de palabras de la oración
    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa los datos de la clase</summary>
    private ParseWords( Translate Trd, string SubOrac )
      {
      this.Trd = Trd;                                                 // Establece datos de la traducción

      Wrds = new List<Word>();                                        // Crea una lista de palabras vacias

      if( SubOrac==null )                                             // Si no es una suboración
        {
        Orac = Trd.Ora.Orig;                                          // Establece oración de la traducción como oración de trabajo
        Trd.Ora.Words = Wrds;                                         // Asocia la lista de palabras obtenidas a la traducción
        }
      else                                                            // Si no 
        Orac = SubOrac;                                               // Toma la suboración, como oración de trabajo

      Len  = Orac.Length;                                             // Establece longitud de la oración de trabajo
      Lng  = Trd.LangSrc.Lang;                                        // Establece idioma fuente de la traducción  
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa la oración actual y obtiene las palabras que la componen</summary>
    public static void Process( Translate Trd, string Orac=null )
      {
      new ParseWords(Trd,Orac).Process();                                 // Crea objeto y llama función interna del objeto
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa la oración actual y obtiene las palabras que la componen (interna para la clase)</summary>
    private void Process()
      {
      int i=0;                                                        // Indice al caracter que se esta analizando
      while( i<Len )                                                  // Mientras no se llege al final de la oración continua analizando
        {
        GetTrdWord( ref i );                                          // Trata de obterner una palabra para traducción

        if( i>=Len || char.IsLetterOrDigit(Orac[i]) )                 // Si esta al final de la cadena o el caracter actual es número o letra
          continue;                                                   // Repite el poceso desde el inicio

        var c = Orac[i];                                              // Obtiene el caracter actual
        switch( c )                                                   // Analiza caracteres que no forman parte de las palabras
          {
          case ' '  : AddSpace(ref i); break;                         // Espacio, lo adiciona a la palabra anterior
          case '\t' : AddSpace(ref i); break;                         // Tab, adiciona un espacio a la palabra anterior

          case '('  :
            if( GetExpresion(ref i) ) break;                          // Trata de ver hay una expresión matematica a partir de él parentisis
            if( GetEntre( '(',')', ref i, 1 ) ) break;                // Parentisis abierto, trata de ver si hay algo entre parentisis
            goto default;                                             // Si no, realiza acción por defecto

          case '['  :
            if( GetExpresion(ref i) ) break;                          // Trata de ver hay una expresión matematica a partir de él corchete
            if( GetEntre( '[',']', ref i, 4 ) ) break;                // Corchete abierto, trata de ver si hay algo entre corchetes abiertos
            goto default;                                             // Si no, realiza acción por defecto

          case '"'  : 
            if( GetEntre( '"','"', ref i, 2 ) ) break;                // Comilla doble, trata de ver si hay algo entre comillas doble
            goto default;                                             // Si no, realiza acción por defecto

          case '\'' :                                                 // Comilla simple
            if( GetPosesivo(ref i) ) break;                           // Trata de ver si es un posesivo
            if( GetEntre( '\'','\'', ref i, 3 ) ) break;              // Trata de ver si hay algo entre comillas simples
            goto default;                                             // Si no, realiza acción por defecto

          case TConst.IniMark:                                        // Marca inicial para no traducción
            if( GetEntre( TConst.IniMark, TConst.EndMark, ref i, 0 ) ) break;   // Forma una palabra con lo que hay entre las marcas
            goto default;                                             // Si no, realiza acción por defecto

          case '.'  :  
            if( IsPuntosSupensivos(ref i) ) break;                    // Si son puntos suspensivos
            if( IsAbreviatura(ref i)      ) break;                    // Si son parte de una abreviatura
            if( GetExpresion(ref i)       ) break;                    // Trata de ver hay una expresión matematica EJ: .8 + 25
            goto case ':';                                            // Continua abajo

          case ':'  :                                                 
          case '@'  :  
          case '_'  :                                                 
          case '/'  :  
          case '\\' : 
            if( IsUrlOrFileName(ref i) ) break;                       // Si forman parte de una Nombre de fichero, URL o EMail
            goto default;                                             // Si no, realiza acción por defecto

          case '-'  :                                                 // Puede ser una frase separada por guion
            if( GetFraseGion(ref i) ) break;                          // Trata de ver si es una frase con guiones
            goto case '+';                                            // Continua abajo por si es el digno de un número o operación resta

          case '+'  :                                                 // Pueden formar parte de una expresión númerica
            if( GetExpresion(ref i) ) break;                          // Trata de ver hay una expresión matematica a partir de él signo + ó -
            goto default;                                             // Si no, realiza acción por defecto

          case '='  :  
            var LstWrd = (Wrds.Count>0)? Wrds[Wrds.Count-1] : null;   // Toma la última palabra
            if( GetExpresion(ref i, LstWrd) ) break;                  // Trata de ver hay una expresión con igual a partir de ultima palabra
            goto default;                                             // Si no, realiza acción por defecto

          case TConst.SustMark:                                       // Es una marca de sustitución
            if( Wrds.Count > 0  )                                     // Si ya hay alguna palabra
              {
              var lWrd = Wrds[Wrds.Count-1];                          // Toma la última palabra
              lWrd.RtfFijoFin += c;                                   // Le pone la marca se sustitución detras  
              }
            else                                                      // Si no hay palabras
              Trd.Ora.RtfIni += c;                                    // Se lo pone a la oración

            ++i;
            break;

          default: AddCharWord(c,ref i); break;                       // Adiciona el caracter como una palabra y no lo traduce
          }
        }
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de obtener una palabra a partir de la posición i</summary>
    private bool GetTrdWord(ref int i)
      {
      for(; i<Len; ++i )
        {
        var c = Orac[i];                                            // Toma la letra actual
        if( !char.IsLetterOrDigit(c) ) break;                       // Si no puede formar parte de una palabra (letra o número), termina

        ++nChar;                                                    // Incrementa contador de caracteres de la palabra

        if( char.IsDigit(c) ) ++nNum;                               // Si es un digito, incrementa contador de digitos en la palabra
        if( char.IsUpper(c) ) ++nUpr;                               // Si es mayúsculas, incrementa contador de mayúsculas en la palabra

        if( "ivxlcdm".Contains(char.ToLower(c)) ) ++nRom;           // Si es un caracter romano, incrementa # de caracteres romanos en la palabra
        }

      if( nChar>0 ) AddWordLetters( ref i );                        // Si encontro palabra, la adiciona a la lista y resentea los contadores
      return (nChar>0);
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona una cadena compuesta por letras o números</summary>
    private void AddWordLetters( ref int i )
      {
      var sWrd = Orac.Substring( i-nChar, nChar );                    // Obtiene cadena con la palabra
      var fUpr = char.IsUpper( sWrd, 0 );                             // Determina si la primera letra es mayusculas

      var Wrd = new Word( sWrd );                                     // Crea una palabra nueva

      Wrd.Origlwr = (nUpr>0)? Wrd.Orig.ToLower() : sWrd;              // Crea una variante en minusculas de la palabra, si es necesario
      Wrd.Key     = Wrd.Origlwr;                                      // Toma como llave la palabra en minusculas
      Wrd.wCase   = (nUpr==nChar     )? WCaso.Upper :                 // Todos los caracteres estan en mayusculas
                    (nUpr==1 && fUpr )? WCaso.First :                 // Hay un solo caracter en mayusculas y es el primero
                    (nUpr>0          )? WCaso.Other :                 // Hay una combinación de caracteres en máyusculas y minusculas
                                        WCaso.Lower ;                 // Todos los caracteres estan en minusculas
      Wrd.Romano = ( nRom == nChar );                                 // Pone la bandera de romano

      if( nNum==0 )                                                   // Si son solo letras
        {
        Wrd.wDiWord    = DiWord.WORD;                                 // Le pone el tipo palabra
        Wrd.Concordada = false;                                       // Pone que hay que concordarla
        }
      else                                                            // Si es una combinación de letras y números
        NumExpesión( Wrd, ref i );                                    // Analiza la palabra forma parte de una expresión númerica

      Wrds.Add( Wrd );                                                // Adiciona la palabra a lista de palabras

      nChar = nNum = nUpr = nRom = 0;                                 // Borra todos los datos sobre la palabra actual
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Analiza las diferentes palabras formadas por números, o números y letras</summary>
    private void NumExpesión( Word Wrd, ref int i)
      {
      Wrd.wDiWord = DiWord.EXPRESION;                                 // Le pone el tipo numero o combinación de números
      Wrd.sTipo   = "NN";                                             // Asume el NN de forma general
      
      var nLet = nChar-nNum;                                          // Número de letras que contiene la palabra

      var   r = GetExpresion( ref i, Wrd );                           // Si es un número seguido de una expresión numerica
      r = (!r && nLet==0 && IsMedUnid(Wrd, ref i) );                  // Si es un numero más una unidad de medida
      r = (!r && nLet==2 && IsNumOrdinal(Wrd) );                      // Si es un numero ordinal
      r = (!r && nLet<=1 && IsNumDecada ( Wrd, ref i) );              // Si es un número que representa una decada

      if( !r )                                                        // Si no encontro nada
        Wrd.sTipo = (nLet==0)? "NN":"NP";                             // Si solo números "NN", mezcla de números y letras "NP" 

      Wrd.Trad       = Wrd.Orig;                                      // Pone traducción igual original
      Wrd.NoBusca    = true;                                          // Pone bandera para que no la busque en el diccionario
      Wrd.Traducida  = true;                                          // Pone bandera para que no intente traducirla
      }

    static char[] Vocales = {'a','e','i','o','u','A','E','I','O','U','á','é','í','ó','ú','Á','É','Í','Ó','Ú'};
    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Chequea si lo que viene a partir de un número es una unidad de medida</summary>
    private bool IsMedUnid(Word Wrd, ref int i )
      {
      if( i>=Len || Orac[i]!=' ' ) return false;                      // Si esta la final no hay espacio al principio, termina
      var j = i+1;                                                    // Salta el espacio

      // Salta todas las letras que no sean vocales
      while( j<Len && char.IsLetter(Orac[j]) && !Vocales.Contains(Orac[j]) ) ++j;

      if( j>i+1 && (j>=Len || Orac[j]==' ' || Orac[j]=='/' ) )        // Si encontro al menos una letra y esta seguida de espacio o slat
        {
        if( j<Len && Orac[j]=='/' )                                   // Si el útimo caracter es slat
          while( j<Len && Orac[j]!=' ' ) ++j;                         // Salta todos los caracteres hasta el proximo espacio

        var str = Orac.Substring( i, j-i );                           // Obtiene todos los caracteres analizados
        FillNoTradWord( Wrd, Wrd.Orig+str, "NN", DiWord.EXPRESION );  // Se lo adiciona a la última palabra y la pone como expresión

        i=j;                                                          // Actualiza puntero a último caracter analizado
        return true;                                                  // Retorna OK (Es una unidad de medida)
        }

      return false;                                                   // Retorna false (No es una unidad de medida)
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la comilla simple pertenece a un posesivo</summary>
    private bool GetPosesivo(ref int i)
      {
      if( Wrds.Count==0 ) return false;                               // La comilla es la primera palabra de la oración

      var LstWrd = Wrds[Wrds.Count-1];                                // Toma la última palabra
      if( LstWrd.wDiWord != DiWord.WORD ||                            // Delante no hay una palabra
          LstWrd.nSpace != 0             )  return false;             // Hay al menos un espacio detrás de la palabra

      var j = i+1;                                                    // Salta la comilla
      if( j<Len && Orac[j]==' ' )                                     // No viene s destras de la comilla
        {
        LstWrd.wDiWord = DiWord.POSESIVO_SC;                          // Posesivo sin comillas
        i = j;                                                        // Actualiza el indice el hasta el espacio 
        }
      else if( j+1<Len && (Orac[j]=='s' || Orac[j]=='S' ) && Orac[j+1]==' ' )     // Seguido de s ó S y espacio
        {
        LstWrd.wDiWord = DiWord.POSESIVO_CS;                          // Posesivo con comilla detras
        i = j+1;                                                      // Actualiza el indice hasta el espacio 
        }
      else return false;                                              // No la coma no es un posesivo

      LstWrd.sTipo    = "GX";                                         // Le pone el tipo
      LstWrd.Posesivo = true;                                         // Le pone la bandera de posesivo
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el punto de la posición actual pertenece a una abreviatura</summary>
    private bool IsAbreviatura(ref int i)
      {
      if( Wrds.Count==0  ) return false;                              // El punto esta al inicio de la oración, no es abreviatura

      var LstWrd = Wrds[Wrds.Count-1];                                // Toma la última palabra

      if( LstWrd.Orig.Length==2 && i+2<Len && Orac[i+1]=='\'' )       // Verifica si puede ser posesivo  (CC.'s)
        {
        var j = i+1; 
        if( GetPosesivo( ref j) )                                     // Si relmente es posesivo
          {
          LstWrd.Orig    += '.';                                      // Le agrega el punto a la ultima palabra
          LstWrd.Key     += '.';
          LstWrd.Origlwr += '.';
          i = j;                                                      // Actualiza el indice al ultimo caracter analizado
          return true;
          }
        }

      // Si esta detras de una palabra que empice con mayusculas y tenga menos de 6 caracteres o esta detras de una palabra en mayusculas
      if( (LstWrd.wCase==WCaso.First && LstWrd.Orig.Length<6) ||  LstWrd.wCase==WCaso.Upper  )
        {
        int j=i+1;
        while( j+2<Len && char.IsLetter(Orac[j]) && Orac[j+1]=='.' )  // Salta todos los letras+punto, Ej; E.U.
          j += 2;

        if( j<Len && Orac[j]!=' ' ) return false;                     // Si después del punto no hay un espacio, no es una abreviatura

        if( j == i+1 )                                                // Solo es el punto final
          {
          LstWrd.sTipo       = "NP";                                  // Le pone el tipo nombre propio
          LstWrd.Abreviatura = true;                                  // Le pone bandera de abreviatura
          }
        else
          {
          var sWord = LstWrd.Orig + Orac.Substring( i, j-i );         // Adiciona caracteres a la palabra original
          FillNoTradWord( LstWrd, sWord );                            // Pone los datos de la palabra para no traducir
          }

        LstWrd.wDiWord = DiWord.ABREVIATURA;                          // Pone el tipo de palabra abreviatura

        i = j;                                                        // Actualiza el indice al ultimo caracter analizado
        return true;                                                  // Retorna que encontró una abreviatura
        }

      return false;                                                   // No es una abreviatura
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si a partir de la posición actual hay puntos suspensivos</summary>
    private bool IsPuntosSupensivos(ref int i)
      {
      int j = i;                                                      // Marca el inicio del analisis
      while( j<Len && Orac[j]=='.' ) ++j;                             // Si hay punto lo salta
      if( j-i <= 1 ) return false;                                    // Si un punto o ninguno, retorna false

      var sWrd = Orac.Substring( i, j-i );                            // Obtine los puntos saltados  

      var nWord = new Word();                                         // Crea una palabra nueva
      FillNoTradWord( nWord, sWrd, "NP", DiWord.PUNTOSUSPENSIVOS );   // Llena los datos como número
      Wrds.Add( nWord );                                              // Adiciona la palabra al arreglo de palabras
      i=j;                                                            // Actualiza el indice al ultimo caracter analizado

      return true;                                                    // Retorna que fueron encontrados puntos supensivos
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    static Regex reFullURL  = new Regex(@"(?:(?:\w+:)?(?:\\|/){1,2})?(?:[^\\/*:?""<>| ]+(?:\\|/))+(?:(?:[^\\/*:?""<>| ]+\.\w+)|(?:\w+))?(?:\?\w+=\w+)?", RegexOptions.CultureInvariant);
    static Regex reFullPath = new Regex(@"\w+:(?:(?:\\|/){1,2}[^\\/*:?""<>|]+)*(?:\\|/)(?:(?:[^\\/*:?""<>|]+\.\w+)|(?:\w+))?", RegexOptions.CultureInvariant);
    static Regex reEMail    = new Regex(@"[\w_]+(?:\.[\w_]+)*@[\w_]+(?:\.[\w_]+)+", RegexOptions.CultureInvariant);
    static Regex reFile     = new Regex(@"[\w_]+(?:\.[\w_]+)+", RegexOptions.CultureInvariant);
    static Regex rePath     = new Regex(@"(?:\\|/){1,2}(?:[^\\/*:?""<>|]+(?:\\|/))*(?:(?:[^\\/*:?""<>|]+\.\w+)|(?:\w+))?", RegexOptions.CultureInvariant);

    Match ReMatch;          // Ultima expesión regular casada con MatchRegex

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si en la posicción actual hay un nombre de fichero, URL o correo</summary>
    private bool IsUrlOrFileName(ref int i)
      {
      int j = i;                                                      // Guarda la posición actual  
      var c = Orac[j--];                                              // Toma al caracter y corre indice hacia atrás
      while( j>=0 && char.IsLetterOrDigit(Orac[j]) ) --j;             // Salta todas las letras o digitos hacia atrás
      ++j;                                                            // Se pone en la última letra

      bool ret = false;
      switch(c)
        {
        case ':': ret = IsMatch( j, reFullURL, reFullPath); break;        // Trata de casar URL absoluto o camino a fichero absoluto
        case '.':
        case '_': ret = IsMatch( j, reEMail, reFullURL, reFile ); break;  // Trata de casar dirección de Coreo ó URL ó Nombre de fichero
        case '@': ret = IsMatch( j, reEMail ); break;                     // Trata de casar dirección de Coreo
        case '\\':
        case '/': ret = IsMatch( j, rePath ); break;                      // Trata de casar Nombre de fichero más extensión
        }   

      if( !ret ) return false;                                        // No caso nada, termina

      var sWrd = Orac.Substring( j, ReMatch.Length );                 // Obtiene la cadena casada anteriormente

      if( j<i )                                                       // Si el indice de inicio retrocedio
        {
        var LstWrd = Wrds[Wrds.Count-1];                              // Toma la última palabra
        FillNoTradWord( LstWrd, sWrd, "FN", DiWord.FILENAME );        // Le llena los datos de nombre propio
        }
      else                                                            // Si fue a partir de posición actual
        {
        var nWord = new Word();                                       // Crea una palabra nueva
        FillNoTradWord( nWord, sWrd, "FN", DiWord.FILENAME );         // Le llena los datos de nombre propio
        Wrds.Add( nWord );                                            // Adiciona la palabra al arreglo de palabras
        }

      i = j + ReMatch.Length;                                         // Actualiaza indice al último caracter analizado
      return true;                                                    // Termina OK
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de casar una frase con guion</summary>
    private bool GetFraseGion(ref int i)
      {
      if( i<=0 || i+1>=Len || Trd.LangSrc.Lang!=TLng.En ) return false;
        
      // Si es una frase separada por Guion
      if( char.IsLetterOrDigit(Orac[i-1]) && char.IsLetterOrDigit(Orac[i+1])  )
        {
        var nWord = new Word();                                       // Crea una palabra nueva
        FillNoTradWord( nWord, "-", "GG", DiWord.GUION );             // Llena los datos como guion de frase
        Wrds.Add( nWord );                                            // Adiciona la palabra al arreglo de palabras
        ++i;
        return true;
        }

      return false;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de casar una de las expresiones regulares que se pasan como argumentos</summary>
    private bool IsMatch( int i, params Regex[] ReList )
      {
      foreach( var Re in ReList )                                     // Recorre todas las Expresiones que se pasan como argumento
        {
        ReMatch = Re.Match( Orac, i );                                // Trata de casar la expresión a partir del caracter i
        if( ReMatch.Success && ReMatch.Index==i )                     // Si la casa, empezando en la posición i
          return true;                                                // Retrona expresión cazada y pone resultado en ReMarch
        }

      return false;                                                   // No caso ninguna expresión
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona a palabra anterior todos los espacios que haya a partir de i
    ///Nota: Al final: (#Espacios = nSpace % 50) y (#Tabuladores= nSpace / 50)  siempre que #Espacios<50 </summary>
    private void AddSpace(ref int i)
      {
      int j = i;                                                    // Marca el inicio del analisis
      int n = 0;                                                    // Número de espacios
      while( i<Len )                                                // Mientras no se llegue al final
        {
             if( Orac[i]==' '  ) n += 1;                            // Si es espacio, incremanta contador en 1
        else if( Orac[i]=='\t' ) n += 50;                           // Si es Tab, incrementa contador en 50
        else break;                                                 // Si es otro caracter, detine el analis

        ++i;                                                        // Salta espacio o tab y continua con proximo caracter
        }

      if( Wrds.Count>0  )                                           // Si hay alguna palabra en la lista
        Wrds[Wrds.Count-1].nSpace = n;                              // Coje la última palabra y la pone el numero de espacios
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra es un número ordinal</summary>
    private bool IsNumOrdinal(Word Wrd)
      {
      var sEnd = Wrd.Orig.Substring( Wrd.Orig.Length-3 );             // Obtiene los dos ultimos caracteres de la palabra

      // Verifica si es una terminación del ordinal, según el idioma del texto
      if( (Lng==TLng.Es && (sEnd=="ro" || sEnd=="do" || sEnd=="to" || sEnd=="mo" || sEnd=="vo" || sEnd=="no")) || 
          (Lng!=TLng.Es && (sEnd=="st" || sEnd=="nd" || sEnd=="rd" || sEnd=="th") ) )
        {
        Wrd.wDiWord = DiWord.ORDINAL;                                 // La palabra es un ordinal
        Wrd.sTipo   = "NU";                                           // Pone el tipo que representa un ordinal
        return true;
        }

      return false;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra es un numero que representa una decada</summary>
    private bool IsNumDecada(Word Wrd, ref int i)
      {
      if( nChar-nNum == 1 && Wrd.Orig[Wrd.Orig.Length-1]=='s' )       // Si la palabra tiene una letra y es una s al final
        {
        Wrd.wDiWord = DiWord.DECADA_S;                                // Pone tipo de palabra
        Wrd.sTipo   = "GY";                                           // Pone tipo gramatical
        return true;
        }

      if( nChar-nNum==0 && i+1<Len && Orac[i]=='\'' && Orac[i+1]=='s' )   // Si es un número y le sigue 's
        {
        Wrd.wDiWord = DiWord.DECADA_CS;                               // Pone tipo de palabra
        Wrd.sTipo   = "GY";                                           // Pone tipo gramatical
        i += 2;                                                       // Salta los dos caracteres de 's
        return true;
        }

      return false;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    const string OpBin1 = "+-*xX/=:";                                 // Operadores binarios que pueden estar separados
    const string OpBin2 = ".,'";                                      // Operadores binarios que tienen que estar junto a un número
    const string OpUni = "+-.,";                                      // Operadores unitarios
    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de casar una expresión matematica a partir de la posición i</summary>
    private bool GetExpresion( ref int i, Word Wrd=null)
      {
      string sExpd = "";
                                                                      // Cadena que se debe expandir la expresión
      for(;;)
        {                                                             // Repite el ciclo las veces que sean necesario
        int j = i;                                                    // Marca el inicio del analisis
        while( j<Len && Orac[j]==' ' ) ++j;                           // Si hay espacios los salta

        if( sExpd.Length==0 && Wrd==null )                            // Si esta al principio y no esta precedido por una palabra
          {
          if( !SkipOperando(ref j) ) break;                           // Tiene que venir un operando, si no termina
          sExpd += Orac.Substring( i, j-i );                          // Obtiene la cadena con el operador
          i = j;                                                      // Actualiza el indice a lo posicion actual

          while( j<Len && Orac[j]==' ' ) ++j;                         // Si hay espacios los salta
          }

        if( j>=Len || !OpBin1.Contains(Orac[j]) )                     // Trata de buscar un operador (se supone que delante haya un número)
          {
          if( j<1 || j+1>=Len || !OpBin2.Contains(Orac[j]) || !char.IsDigit(Orac[j-1]) || !char.IsDigit(Orac[j+1])  ) break;   // No esta entre dos números              
          }

        ++j;                                                          // Salta el signo

        while( j<Len && Orac[j]==' ' ) ++j;                           // Si hay espacios los salta

        if( !SkipOperando( ref j) ) break;                            // Si no encontro un operando, termina
          
        sExpd += Orac.Substring( i, j-i );                            // Obtiene la cadena a expandir
        i = j;                                                        // Actualiza el indice a lo pocision actual
        }                                                             // Repite el proceso

      if( sExpd.Length == 0 ) return false;                           // No encotro ninguna expresión

      if( Wrd==null )                                                 // No hay palabra anterior a la expresión
        {
        var nWord = new Word();                                       // Crea una palabra nueva
        FillNoTradWord( nWord, sExpd, "NN", DiWord.EXPRESION );       // Llena los datos como número
        Wrds.Add( nWord );                                            // Adiciona la palabra al arreglo de palabras
        }
      else                                                            // Es la continuación de una palabra
        FillNoTradWord( Wrd, Wrd.Orig+sExpd, "NN", DiWord.EXPRESION );// Llena los datos como número

      return true;                                                    // Retorna OK
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Trata de encontrar un operando de una expresión matematica a partir de la posición i</summary>
    private bool SkipOperando(ref int i)
      {
      int j=i;

      if( j<Len && (Orac[j]=='(' || Orac[j]=='[') )                   // Opcionalmente puede haber un parentisi o corchete delante del número
        {
        ++j;                              
        while( j<Len && Orac[j]==' ' ) ++j;                           // Después de ( o [ pueden haber espacios
        }

      if( j<Len && OpUni.Contains(Orac[j]) ) ++j;                     // Opcionalmente puede haber operador unitario delante del número

      int k = j;                                                      // Marca inicio del operando
      if( j<Len && char.IsLetter( Orac, j) ) ++j;                     // Opcionalmente los operandos pueden ser una letra seguida de números

      while(  j<Len && char.IsDigit( Orac, j) ) ++j;                  // Salta todos los números
      if( k==j ) return false;                                        // Si no hay numeros termina

      k = j;                                                          // Marca final del número
      while( k<Len && Orac[k]==' ' ) ++k;                             // Después de ( o [ pueden haber espacios
      if( k<Len && (Orac[k]==')' || Orac[k]==']') )                   // Opcionalmente puede haber un parentisis o corchete detras del número
        j = k + 1;                                                    // Salta los espacios y el parentisis o corchete

      if( j<Len && char.IsLetter( Orac, j) )                          // Si el caracter final es una letra, no es un operando
        return false;

      i = j;                                                          // Actualiza el indice donde se termino de leer el operando
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Intenta tomar todo el texto entre 'cIni' y 'cFin' y formar una sub-oración</summary>
    private bool GetEntre( char cIni, char cFin, ref int i, int Entre)
      {
      if( Trd.Ora.Words != Wrds ) return false;                       // Si ya estaba en un GetEntre, Retorna false (para evitar rentrancia)

      int j = i+1;                                                    // Marca el inicio del analisis
      while( j<Len && Orac[j]!=cIni && Orac[j]!=cFin ) ++j;           // Busca el caracter final

      if( j>=Len || Orac[j]!=cFin || j==i+1 ) return false;           // Si no caso el final

      ++i;                                                            // Salta caracter inicial
      var sWrd = Orac.Substring( i, j-i ).Trim();                     // Obtiene la cadena entre caracter inicial y final
      i = j+1;                                                        // Salta caracater final y acatualiza indice

      if( Entre==0 )                                                  // Caso especial para las marcas de no traducción
        {
        var nWord = new Word();                                       // Crea una palabra nueva
        FillNoTradWord( nWord, sWrd.Trim() );                         // Llena los datos de nombre propio
        Wrds.Add( nWord );                                            // Adiciona la palabra al arreglo de palabras
        return true;
        }

      if( sWrd.Contains(' ')  && sWrd.Length>0 )                      // Si hay mas de una palabra
        {
        SetSubSentence( sWrd, Entre);                                 // Pone el contenido como una sub-oración
        return true;                                                  // Termina
        }

      Word Wrd;
      var tmpParse = new ParseWords( Trd, sWrd );                     // Crea parse para la parte encerrada
      tmpParse.Process();                                             // Lo procesa
      if( tmpParse.Wrds.Count == 1 )                                  // Si como se espera encuentra una palabra
        Wrd = tmpParse.Wrds[0];                                       // Obtiene la palabra
      else                                                            // Si no se obtuvo la palabra anteriormente
        {
        Wrd = new Word();                                             // Crea una nueva palabra
        FillNoTradWord( Wrd, sWrd, "SS" );                            // Le llena los datos como nombre propio
        }

      Wrd.Entre = Entre;                                              // Le pone a la pababra entre que va
      Wrds.Add( Wrd );                                                // La adiciona a la lista de palabras
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Llena los datos de una palabra como nombre propio</summary>
    private void FillNoTradWord( Word Wrd, string sWrd, string sTipo="NP", DiWord DWord = DiWord.NOMBRE_PROPIO  )
      {
      Wrd.Trad      = sWrd;                                           // Pone traducción igual texto marcado
      Wrd.Key       = Wrd.Origlwr = Wrd.Orig = sWrd;                  // Pone palabra original y llave

      Wrd.sTipo     = sTipo;                                          // Pone el tipo gramatical como nombre propio
      Wrd.wDiWord   = DWord;                                          // Se pone el tipo de palabra especificado

      Wrd.Traducida = true;                                           // Pone marca que esta traducida
      Wrd.Buscada   = true;                                           // Pone marca que ya fue buscada
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Adiciona a la lista de palabras, una palabra de un solo caracter</summary>
    private void AddCharWord( char c, ref int i )
      {
      DiWord Dw;                                                      // Tipo de palabra asignado a la letra
      string Tipo;                                                    // Tipo gramatical asignado a la palabra
      bool   pega = false;                                            // Define si se pueden unir varias letras del mismo tipo
 
      switch( c )                                                     // De acuerdo al caracter, determina tipo gramatical y DiWord
        {
        case ':' : Tipo = "GG"; Dw = DiWord.DOSPUNTOS;     pega=true; break;
        case '.' : Tipo = "GZ"; Dw = DiWord.PUNTO;                    break;
        case ',' : Tipo = "GZ"; Dw = DiWord.COMA;                     break;
        case ';' : Tipo = "GZ"; Dw = DiWord.PUNTOYCOMA;               break;
        case '?' : Tipo = "GS"; Dw = DiWord.FININTERROGA;  pega=true; break;
        case '!' : Tipo = "GA"; Dw = DiWord.FINADMIRACION; pega=true; break;
        case '-' : Tipo = "GZ"; Dw = DiWord.MENOS;                    break;
        case '/' : Tipo = "GR"; Dw = DiWord.SLAT;                     break;
        case '<' : Tipo = "GP"; Dw = DiWord.MENORQ;                   break;
        case '>' : Tipo = "GP"; Dw = DiWord.MAYORQ;                   break;
        case '{' : Tipo = "GP"; Dw = DiWord.LLAVEA;                   break;
        case '}' : Tipo = "GP"; Dw = DiWord.LLAVEC;                   break;
        case '[' : Tipo = "GP"; Dw = DiWord.CORCHETEA;                break;
        case ']' : Tipo = "GP"; Dw = DiWord.CORCHETEC;                break;
        case '(' : Tipo = "GP"; Dw = DiWord.PARENTA;                  break;
        case ')' : Tipo = "GP"; Dw = DiWord.PARENTC;                  break;
        case '"' : Tipo = "GD"; Dw = DiWord.COMILLAD;                 break;
        case '\'': Tipo = "GC"; Dw = DiWord.COMILLAS;                 break;
        case '%' : Tipo = "GO"; Dw = DiWord.PORCIENTO;     pega=true; break;
        case '#' : Tipo = "NN"; Dw = DiWord.CHARNUMERO;        pega=true; break;
        default  : 
          if( char.GetUnicodeCategory(c) == UnicodeCategory.CurrencySymbol )    // Si es una simbolo de moneda
            Dw = DiWord.DINERO;                                       // Pone el tipo de palabra dinero
          else 
            {Dw = DiWord.CUALQUIERA;  pega=true;}                     // Es un caracter no relevante para la traducción

           Tipo = "GN";
           break;
        }

      ++i;                                                            // Avanza el indice al siguente caracter
      if( pega && Wrds.Count>0  )                                     // Si se pueden pegar los caracteres y hay al menos una palabra
        {
        var LstWrd = Wrds[Wrds.Count-1];                              // Toma la última palabra
        if( LstWrd.nSpace==0 && LstWrd.wDiWord==Dw )                  // Si es del mismo tipo y no tiene espacios detras
          {
          LstWrd.Orig += c;                                           // Le adicciona el caracter a la palabra
          LstWrd.Trad = LstWrd.Origlwr = LstWrd.Key = LstWrd.Orig;    // Pone todas las palabras iguales
          return;                                                     // Termina
          }
        }

      var Wrd = new Word();                                           // Crea una nueva palabra vacia
      FillNoTradWord( Wrd, c.ToString(), Tipo, Dw );                  // Llena los datos de la palabra de un caracter y que no se traduce
      Wrds.Add( Wrd );                                                // Adiciona la palabra a lista de palabras

      if( c == '-' &&  Trd.LangSrc.Lang==TLng.En  )                   // Caso especial para distiguir de las frases
        {
        Wrd.Orig = "--";                                              // Lo sustituye por dos guiones
        Wrd.Trad = "+-+";                                             // Le pone una traducción especial
        }
      }

    //----------------------------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la cadena sWrd como una sub-oración detrás de la última palabra o al inicio de la oración</summary>
    private void SetSubSentence(string sWrd, int EntreQue)
      {
      if( Wrds.Count==0 )
        {
        var Orac = Trd.Ora;
        if( Orac.SubOracs==null )
          Orac.SubOracs = new List<SubOrac>();

        Orac.SubOracs.Add( new SubOrac(sWrd, EntreQue) );
        }
      else
        {
        var Wrd = Wrds[Wrds.Count-1];

        if( Wrd.SubOracs==null )
          Wrd.SubOracs = new List<SubOrac>();

        Wrd.SubOracs.Add( new SubOrac(sWrd, EntreQue) );
        }
      }


    } //++++++++++++++++++++++++++++++++++++++++++++++++ Fin de la clase ParseWords ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++++++++++++++ Fin del namespace          ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
