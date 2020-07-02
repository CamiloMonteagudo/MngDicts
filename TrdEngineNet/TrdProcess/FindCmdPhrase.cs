using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Busca las frases para coordinación y conjugación (Usa una variante del algoritmo para buscar en oraciones)</summary>
  public class FindCmdPhrase
    {
    protected Translate Trd;
    List<string>        Words;        // Lista de palabras de la oración y sus posibles variaciones
    List<string>        SortKeys;    // Lista de palabras del diccionario

    int Index = 0;
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    private FindCmdPhrase( int Index, Translate Trd )
      {
      this.Trd   = Trd;
      this.Index = Index;
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Recorre la oración actual en busca de frases idiomaticas (Pueden tener comodines)</summary>
    public static void Process( int Index, Translate Trd )
      {
      var FPhrases = new FindCmdPhrase( Index, Trd );
      FPhrases.Process();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca todas la frases que haya en la oración</summary>
    protected void Process()
      {
      var Dict = Trd.GetDict( DDirId.CC );                            // Obtiene el diccionario
      if( Dict==null ) return;                                        // Si no existe no hace nada

      Trd.Ora.PoneXX();                                               // Pone XX al inicio y final de la oración

      Words    = GetWords();                                          // Busca todas las palabras de la oración (tipos)
      SortKeys = Dict.GetSortedKeys();                                // Obtiene palabas del diccionario ordenadas alfabeticamente

      var nDelWrd = 0;                                                // Tiene en cuenta las palabras borradas durante el proceso
      for( int i=0; i<Words.Count; i++ )
        {
        var sFra = FindAt(i);                                         // Busaca si a partir de la palabra i hay una frase
        if( sFra != null )                                            // Si encontró una frase
          {
          var fin = WordEnd;                                          // Obtiene palabra final de la frase
          ExecRules.ProcessCC( Trd, sFra, i-nDelWrd, fin-nDelWrd );   // Ejecuta regla de coodinación y conjugación y crea una frase

          nDelWrd += (fin-i);                                         // Calcula y acumula el número de palabras borradas
          i=fin;                                                      // Ajusta la palabra donde debe continar la busqueda
          }
        }

      Trd.Ora.QuitaXX();                                              // Quita XX del inicio y final de la oración
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene un listado de todas las palabras de la oración y sus posibles variantes para formar frases</summary>
    protected List<string> GetWords()
      {
      var Words = new List<string>();                                 // Crea lista de palabras vacia

      var Wrds = Trd.Ora.Words;                                       // Obtiene palabras de la oración
      for(int i=0; i<Wrds.Count; ++i)                                 // Recorre todas las palabras
        Words.Add( Wrds[i].sTipo.ToLower() );                         // Adiciona una sola variante, la palabra original

      return Words;                                                   // Retorna lista de palabras
      }

    //------------------------------------------------------------------------------------------------------------------
    int End  = 0;                   // Indice a la última palabra del diccionario donde buscar la frase
    public int WordEnd = 0;         // Indice a la ultima palabra de la lista de palabras analizada

    static StringComparer   StrComp = StringComparer.OrdinalIgnoreCase;       // Comparador para las cadenas
    static StringComparison CompOpt = StringComparison.OrdinalIgnoreCase;     // Opción para comparar cadenas
    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca una frase a partir de la palabra 'i' </summary>
    public string FindAt( int i )
      {
      WordEnd = i;                                                    // Inicializa inidice a ultima palabar analizada
      var Wrd = "p" + Index + ' '+ Words[i];                              // Forma primera palabra de la frase

      var idx = SortKeys.BinarySearch( Wrd, StrComp );                // Busca primera palabra en todo el diccionario
      idx = (idx<0)? ~idx : (idx+1);                                  // Si no la encuentra toma el de la palabra mas proxima

      if( FindEnd(Wrd, idx) != 0 )                                    // Busca cantidad de llaves que comiezan con la palabra actual
        return MatchNext( Wrd, i, idx );                              // Trata de casar las proximas palabras y retorna la frase

      return null;                                                    // Retorna que no encontro frase
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca la primera llave que no empieza con la palabra 'Wrd' a partir de la llave i</summary>
    public int FindEnd( string Wrd, int i )
      {
      var iniFra = Wrd + " ZZ";
      End = SortKeys.BinarySearch( i, SortKeys.Count-i, iniFra, StrComp );
      if( End<0 ) End = ~End;
      return End-i;
      }

    //------------------------------------------------------------------------------------------------------------------
    /// <summary>Crece la frase en una palabra y dertermina que hacer</summary>
    private string MatchNext( string frase, int iWrd, int Ini )
      {
      var i = iWrd+1;                                                 // Toma el indice de la proxima palabra a analizar
      if( i >= Words.Count ) return null;                             // Si llego al final, termina

      var Wrd = Words[i];                                             // Toma la palabra en la variante dada

      var frase1 = frase + ' ' + Wrd;                                 // Forma la frase
      string frase2 = "";

      var idx = SortKeys.BinarySearch( Ini, End-Ini, frase1, StrComp );  // Busca la frase dentro del rango donde puede estar en el diccionario
      if( idx >= 0 )                                                  // Encontró la frase exacta
        {
        WordEnd = i;                                                  // Guarda palabra donde termina la clase
        frase2 = MatchNext( frase1, i, ++idx );                      // Trata de buscar una frase mas larga
        return (frase2!=null)? frase2 : frase1;                       // Si encontro una mas larga la toma, sino toma la corta
        }
      else                                                            // No la encontro 
        {
        idx = ~idx;                                                   // Determina indice a la frase más cercana
        if( idx<SortKeys.Count && SortKeys[idx].StartsWith( frase1 + ' ', CompOpt ) )       // Si la más cercana empieza con la frase buscada
          {
          frase2 = MatchNext( frase1, i, idx );                      // Continua agregando la proxima palabra
          if( frase2 != null ) return frase2;                         // Si encontró la frase la retorna
          }
        
        return null;                                                  // No se encontró ninguna frase
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de FindBracketPhrase ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin de namespace         ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
