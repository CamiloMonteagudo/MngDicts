using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
  {
  public class lnkWords
    {
    List<string>[] Lngs = new List<string>[4];

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un objeto nuevo con los datos 'datos', perteneciente al idioma 'lng'</summary>
    public lnkWords( string datos, int lng )
      {
      Lngs[0] = new List<string>();
      Lngs[1] = new List<string>();
      Lngs[2] = new List<string>();
      Lngs[3] = new List<string>();

      Lngs[lng] = DatosToList( datos );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene las palabras pertenecientes al idioma solicitadp</summary>
    internal string GetLang( int lng )
      {
      var lst = Lngs[lng];
      var s = new StringBuilder( 15*lst.Count);

      for( int i=0; i<lst.Count; i++ )
        {
        if( i>0 ) s.Append(',');
        s.Append( lst[i] );
        }

      return s.ToString();
      }

    char[] sep = { ',' , ' '};

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Conviete una cadena de texto, con palabras separadas por coma, en una lista de palabras</summary>
    private List<string> DatosToList( string datos )
      {
      var lst = datos.Split( sep, StringSplitOptions.RemoveEmptyEntries );

      return new List<string>(lst);
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Meclas los datos 'datos' en el idioma 'lng' con los que ya existen</summary>
    internal void Merge( string datos, int lng )
      {
      Lngs[lng] = MergeList( Lngs[lng], DatosToList(datos) );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Meclas dos lista de palabras, adicionando al final de lista 1, las palabras nuevas de la lista 2</summary>
    private List<string> MergeList( List<string> list1, List<string> list2 )
      {
      foreach( var word in list2 )
        {
        if( !list1.Contains(word) )
          list1.Add( word );
        }

      return list1;
      }

    }
  }