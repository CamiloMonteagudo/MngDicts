using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
  {
  ///===================================================================================================================================
  /// <summary> Maneja los indices a las palabras o frases encontradas, organizadas por ranking </summary>
  public class SortedIndexs
    {
    public List<EntryInfo> SortEntries;                               // Entradas al dicionario, organizadas por el ranking

    TextQuery       Query; 
    List<DictEntry> Entries;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicializa un objeto vacio</summary>
    public SortedIndexs()
      {
      Query       = null; 
      Entries     = null;
      SortEntries = new List<EntryInfo>();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Ordena los indices en el diccionario 'foundEntries', según la semejanza entre el query y la entrada correspondiente</summary>
    public SortedIndexs( Dictionary<int, List<int>> foundEntries, TextQuery query, List<DictEntry> entries )
      {
      Query   = query; 
      Entries = entries;

      SortEntries = new List<EntryInfo>();

      foreach( var item in foundEntries )
        AddEntry( item.Key, item.Value );
      }

    public int Count { get {return SortEntries.Count; }  }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adiciona una entrada y la coloca de acuerdo al ranking dado</summary>
    internal void AddEntry( int idxEntry, List<int> WrdsPos)
      {
      var rank = CalculateRank( idxEntry, WrdsPos );

      var Entry = new EntryInfo( idxEntry, rank );
      var idx = SortEntries.BinarySearch( Entry );

      if( idx < 0 ) SortEntries.Insert(~idx, Entry );
      else          SortEntries.Insert( idx, Entry );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Calcula el ranking de la entrada 'idxEntry'</summary>
    private int CalculateRank( int idxEntry, List<int> WrdsPos )
      {
      var words = Entries[idxEntry].nWrd;                             // Numero de palabra de la llave

      int found = WrdsPos.Count;                                      // Cantidad de palabras encontradas
      int del   = Query.Count  - found;                               // Cantidad de palabras que hay quitar del query
      int add   = words - found;                                      // Cantidad de palabras que hay que añadir al query

      int desp   = 0;                                                 // Cantidad de desplazamientos que hay que realizar
      int offset = 0;                                                 // Separación de la frase respecto al inicio  

      if( found>0 )                                                   // Si se encontraron palabras
        {
        offset = WrdsPos[0];                                          // Toma desplazamiento del inicio de la primera palabra
        for( int i=1; i<found; i++ )                                  // Recorre todas las demas palabras
          {
          int dt = WrdsPos[i]-(offset+i);                             // Desplazamiento respecto a la posición que le corresponde
          if( dt<0 ) dt = -dt;                                        // Si es negativo, le quita el signo

          desp += dt;                                                 // Acumula todos los desplazamientos
          }
        }

      return found*2500 - 10*(del+add) - 150*(offset) - 5*desp;      // Calcula el ranking
      }

    }

  ///===================================================================================================================================
  /// <summary> Maneja la información asocida a una entrada del diccionario </summary>
  public class EntryInfo: IComparable<EntryInfo>
    {
    public int Index;
    public int Rank;

    public EntryInfo( int idx, int rank )
      {
      Index = idx;
      Rank  = rank;
      }

    public int CompareTo( EntryInfo other )
      {
      return other.Rank-Rank;
      }
    }
  }
