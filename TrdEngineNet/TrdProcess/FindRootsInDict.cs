using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine.TrdProcess
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Busca y maneja las raices de una palabra</summary>
  class FindRootsInDict
    {
    public string DirectPronoun   = null;
    public string IndirectPronoun = null;

    List<RootData> LstReds = new List<RootData>();

    Translate Trd = null;
    Word      Wrd = null;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase con los datos de la traduccón</summary>
    public FindRootsInDict (Translate Trd, Word nowWord )
      {
      this.Trd = Trd;
      this.Wrd = nowWord;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Actualiza la lista de raices para la palabra dada</summary>
    private bool UpdateList()
      {
      var RedW = Trd.DirData.GetReduc();                              // Obtiene objeto para la traducción
      if( RedW==null || !RedW.Reduce(Wrd.Key) ) return false;         // Si no crea el objeto o la palabra no se reduce, termina

      // tipos base VV,VA,BE,HA,SS,AA,AI
      int Count = RedW.Reductions.Count;                              // Obtiene la cantidad de reducciones

      for( int i=0; i<Count; i++ )                                    // Recorre todas las reducciones
        {
        var nowRed = RedW.Reductions[i];                              // Obtiene reducción actual
        var Root   = new RootData( nowRed );                          // Crea objeto RootData con datos de reducción

        if( Root.sRoot.Contains('|') )                                // Si la raiz, contiene el caracter |   ...????
          Wrd.Key = Root.sRoot;                                       // Pone la llave como la raiz

        if( !string.IsNullOrEmpty( nowRed.CDirecto ) )                // Si existe el complemento directo
          DirectPronoun =nowRed.CDirecto;                             // Lo guarda en la variable

        if( !string.IsNullOrEmpty( nowRed.CIndirecto ) )              // Si existe el complemento indirecto
          IndirectPronoun = nowRed.CIndirecto;                        // Lo guarda en una varible

        LstReds.Add( Root );                                          // Adicioma los datos a lista de reducciones
        }

      return true;                                                    // Retorna OK
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Actualiza los campos tipo y traducción de la palabra según la Raiz encontrada</summary>
    public bool UpdateWordData()
      {
      if( UpdateList() )
        {
        try
          {
          var List2 = new ListOfRoots( Trd, LstReds, Wrd );
          return List2.GetData();
          }
        catch (Exception e)
          {
          Dbg.Msg( e.Message );
          }
        }

      return false;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de la clase FindRootsInDict ++++++++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Guarda datos de las raices de las palabras, obtenidos de la reducción</summary>
  public class RootData
    {
    public string  sType;
    public TGrad   Degree;
    public TGen  Gender;
    public TNum  Number;
    public TPer Person;
    public TTime  Time;
    public TMod    Mode;
    public TNum  NounNumber;  // número del sustantivo
    public string  sRoot;
    public string  sPreffix;

    private string strItem = "";

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un objeto sin inicializar ningún dato</summary>
    public RootData(){}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un objeto a partir de los datos de la reducción</summary>
    public RootData( RedItem RetItem )
      {
      sType    = RetItem.Tipo;
      Mode     = RetItem.Modo;
      Time     = RetItem.Tiempo;
      Degree   = RetItem.Grado;
      Person   = RetItem.Persona;
      Number   = RetItem.Numero;
      Gender   = RetItem.Genero;

      sPreffix = RetItem.Prefijo;
      sRoot    = RetItem.PalReduc;

      SetString();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea un objeto a partir de una cadena</summary>
    public RootData( string sItem  )
      {
      sType = sItem.Substring(0,2);                              // Obtiene los dos primeros caracteres del tipo gramatical

      char g = sItem[3];
      Degree = (TGrad)((g=='p')?0:((g=='c')?1:2));               // Analiza caracter 3 que define el grado

      char G = sItem[5];
      Gender = (TGen)((G=='M')?0:((G=='F')?1:2));              // Analiza caracter 5 que define el genero

      char N = sItem[7];
      Number = (TNum)((N=='S')?0:1);                           // Analiza caracter 7 que define el número

      Person = (TPer)(sItem[9]-'1');                          // Analiza caracter 9 que define la persona

      char T = sItem[11];
      Time = (TTime)((T=='P')?0:((T=='p')?1:((T=='F')?2:3)));   // Analiza caracter 11 que define el tiempo

      Mode = (TMod)int.Parse( sItem.Substring(13,2) );           // Analiza caracteres 13 y 14 que define el modo

      char n = sItem[16];
      NounNumber = (TNum)((n=='S')?0:1);                       // Analiza caracter 16 que define el número del nombre
      }

    //------------------------------------------------------------------------------------------------------------------
    static string[] sMode = {" 00"," 01"," 02"," 03"," 04"," 05"," 06"," 07"," 08"," 09"," 10"," 11"," 12"," 13"," 14"," 15"," 16"};
    static string[] sTime = { " P", " p", " F", " I" };
    static string[] sNum  = { " S", " P" };
    static string[] sPers = { " 1", " 2", " 3" };
    static string[] sGen  = { " M", " F", " N" };
    static string[] sGrad = { " p", " c", " s" };

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina la cadena que representa al objeto</summary>
    public void SetString() 
      { 
      var s = new StringBuilder(20);

      s.Append( sType, 0, 2 );
      s.Append( sGrad[(int)Degree    ] );
      s.Append( sNum [(int)Number    ] );
      s.Append( sPers[(int)Person    ] );
      s.Append( sTime[(int)Time      ] );
      s.Append( sMode[(int)Mode      ] );

      strItem = s.ToString(); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una representación del objeto en forma de cadena</summary>
    public override string ToString() { return strItem; }

    } //++++++++++++++++++++++++++++++++++++ Fin de la clase FindRootsInDict ++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin del namespace               ++++++++++++++++++++++++++++++++++++++++++++++++++
