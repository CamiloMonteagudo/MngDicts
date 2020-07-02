using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
  {
  ///===================================================================================================================================
  /// <summary> Maneja los datos sobre la consulta que se hacen a los datos del diccionario </summary>
  public class TextQuery
    {
    public string[] Words;                                  // Lista de palabras que forman el query
    public int Count;                                       // Cantidad de palabras del query

    static char[] wrdSep = {' ','-','(',')','"','¿','?','!','¡','$',',','/','+','*','='};

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un objeto, con la cadena de busqueda</summary>
    public TextQuery( string sQuery )
      {
      // Lleva a minusculas y separa las palabras
      Words = sQuery.ToLower().Split( wrdSep, StringSplitOptions.RemoveEmptyEntries );
      Count = Words.Length;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca palabras del query usando el diccionario de indices 'dictIndexs'</summary>
    internal Dictionary<int, List<int>> FindWords( Dictionary<string, IndexData> dictIndexs )
      {
      var EntrysPos = new Dictionary<int, List<int>>();

      for( int i=0; i<Count; ++i )                              // Recorre todas las palabras
        {
        var wrd = Words[i];

        if( dictIndexs.ContainsKey( wrd ) )                     // Si esta en el diccionario de indices de palabras
          {
          var EntrysData = dictIndexs[wrd];                     // Obtiene todas las entradas donde esta la palabra

          foreach( var item in EntrysData.Entries )             // Recorre las entradas donde esta la palabra
            {
            var idx = item.idxEntry;                            // Obtiene el indice de la entrada donde esta la palabra

            List<int> WrdsPos;
            if( !EntrysPos.TryGetValue( idx, out WrdsPos ) )    // Si no hay ninguna palabra en esa entrada
              {
              WrdsPos = new List<int>();                        // Crea una nueva lista de posiciones de palabras
              EntrysPos[idx] = WrdsPos;                         // Adiciona la lista a diccionario de entradas
              }

            WrdsPos.Add( item.WrdPos );                         // Adiciona la posición a la lista posiciones de entrada idx
            }
          }
        }

      return EntrysPos;                                         // Retorna lista de entradas, con las posiciones de las palabras encontradas
      }
    }
  }
