using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Almacena toda la información de la oración que se esta traduciendo</summary>
  public class Sentence
    {                                               /// <summary>Oración original</summary> 
    public string Orig = "";                        /// <summary>Oración traducida</summary> 
    public string Dest = "";                        /// <summary>Marca de formato al inicio de la oracion</summary>
    public string RtfIni = "";                      /// <summary>Marca de formato al fin de la oracion</summary> 
    public string RtfFin = "";                      
                                                    /// <summary> Si toda la oración tiene la primera letra de cada palabra en mayúsculas</summary>
    public bool AllFirstUpper = false;              /// <summary>Si la oración es interrogativa</summary>
    public bool Interrogative = false;              /// <summary>Si primera palabra de la Ora. en mayúsculas</summary>
    public bool FirsUpper     = false;              /// <summary>Si toda la oracion está en mayúsculas</summary>
    public bool AllUpper      = false;              
                                                    /// <summary>Modo de la oración</summary>
    public TMod  Mode;                              /// <summary>Genero de la oración</summary>
    public TGen  Gener;                             /// <summary>Numero de la oración</summary>
    public TNum  Num;                               /// <summary>Tiempo de la oración</summary>
    public TTime Time;                              /// <summary>Persona de la oración</summary>
    public TPer  Person;                            /// <summary>Si la oracion original esta OJO ???</summary>
    public WCaso Case;                        
                                                    /// <summary>Lista de palabras de toda la oracion </summary>
    public List<Word>   Words = new List<Word>();   /// <summary>Palabras de la oración en el momento que se determina la función gramatical</summary>
    public List<string> sWords= new List<string>(); /// <summary>OJO - Igual a words ??? Palabras de la oración en el momento que se determina la función gramatical</summary>
    public List<string> sKeys = new List<string>(); /// <summary>Todos los tipos gramaticales correspondiente a las palabras de la oración</summary>
    public List<string> sTypes= new List<string>(); 
                              
    public bool EntreUnaVez   = false;              // Bandera para mercar la primera ves que se entra a GetTypeWordByWildCard
    public bool AllTypeSimple = false;              // Bandera para saber si todos los tipos gramaticales son simples

    ///<summary>Cadena de caracters con todos los tipos gramaticales de la oración</summary>
    public string sAllTypes = "";

    public List<SubOrac>  SubOracs  = null;         // Sub-Oraciones que van al inicio de la oración

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una oración con un texto y un formato</summary>
    ///<param name="s">Texto con el que se quiere inicializar el objeto</param>
    public Sentence(string s )
      { 
      Orig  = s;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Crea una oración a partir de otra oración</summary>
    ///<param name="ora">Oración de partida para crear el objeto</param>
    //public Sentence(Sentence ora)
    //  { 
    //  Orig            = ora.Orig;
    //  Dest            = ora.Dest;
    //  EntreParentesis = ora.EntreParentesis;
    //  RtfIni          = ora.RtfIni;
    //  RtfFin          = ora.RtfFin;
    //  Traducida       = ora.Traducida;
    //  Format          = ora.Format;
    //  EntreQue        = ora.EntreQue;
    //  AllUpper        = ora.AllUpper;
    //  AllFirstUpper   = ora.AllFirstUpper;
    //  Interrogative   = ora.Interrogative;
    //  FirsUpper       = ora.FirsUpper;
    //  VerIni          = ora.VerIni;
    //  VerFin          = ora.VerFin;

    //  Case   = ora.Case;
    //  Mode   = ora.Mode;
    //  Gener  = ora.Gener;
    //  Num    = ora.Num;
    //  Time   = ora.Time;
    //  Person = ora.Person; 

    //  Words.Clear();
    //  sWords.Clear();
    //  sKeys.Clear();
    //  sTypes.Clear();

    //  foreach( var word in ora.Words) Words.Add( new Word(word) );

    //  foreach( var word in ora.sWords ) sWords.Add( word );
    //  foreach( var key  in ora.sKeys  ) sKeys.Add ( key  );
    //  foreach( var type in ora.sTypes ) sTypes.Add( type );
    //  }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la marca xxxx al inicio y al final de la oración</summary>
    internal virtual bool PoneXX()
      {
      Word Wini = new Word("XXXX");
      Word Wfin = new Word("XXXX");
             
      Wini.Origlwr   = Wfin.Origlwr = Wini.Key = Wfin.Key = "xxxx";
      Wini.sTipo     = Wfin.sTipo   = "XX";
      Wini.Buscada   = true;
      Wfin.Buscada   = true;
      Wini.Traducida = true;
      Wfin.Traducida = true;
      Wini.NoBusca   = true;
      Wfin.NoBusca   = true;

      Words.Insert(0, Wini );             // Adiciona la palabra al inicio
      Words.Add( Wfin );                  // Adiciona la palabra al final;

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Quita la marca xxxx, si esta, al inicio y al final de la oración</summary>
    internal virtual void QuitaXX()
      {
      if( Words.Count==0 ) return;

      Word w = Words[0];
      if( w.sTipo=="XX" && (w.Orig=="XXXX" || w.Origlwr=="xxxx") )
        Words.RemoveAt(0);

      var iLast = Words.Count-1;
      w  = Words[iLast] ;
      if( w.sTipo=="XX" && (w.Orig=="XXXX" || w.Origlwr=="xxxx") )
        Words.RemoveAt(iLast);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna la primera palabra de la oración, no tiene en cuenta el XX</summary>
    internal Word GetFirstWord()
      {       
      if( Words.Count == 0 ) return null;

      Word w  = Words[0];
      if( w.sTipo == "XX" && Words.Count>1 )
        w = Words[1];

      return w;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna la ultima palabra de la oración, no tiene en cuenta el XX</summary>
    internal Word GetLastWord()
      {
      if( Words.Count == 0 ) return null;

      int iLast = Words.Count-1;
      Word w  = Words[iLast];
      if( w.sTipo == "XX" && iLast>=1 )
        w = Words[iLast-1];;

      return w;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna TRUE si la una palabra de la lista [s] existe a la derecha </summary>
    internal bool ExistRight( int iWrd, string s, bool tipo=false )
      {           
      if( Words.Count==0 ) return false;
      string wherefind = "," + s + ",";           // pone la cadena en que se busca entre comas
      
      for( int i=iWrd; i<Words.Count;  ++i )
        {
        var w   = Words[i];
        var tmp =  tipo? w.sTipo : w.Origlwr;

        string wfind = "," + tmp + ",";           // pone la palabra entre comas
        if( wherefind.IndexOf( wfind ) != -1)
          return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna primera palabra hacia la izquierda que no es Tipo s</summary>
    internal Word LastNotType( int iWrd, string Tipos )
      {       
      if( Tipos==null ) return null;

      for( int i=iWrd-1; i>=0; --i )
        {
        Word w = Words[i];
        if( !Tipos.Contains( w.sTipo ) )  return w;
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone valor de la palabra anterior que no sea adverbio y sea de [tipo]</summary>
    internal bool IsLastNotDDSetByType( int iWrd, string tipo, string sTrd, string AdverbialType )
      {           
      Word w = LastNotType(iWrd, AdverbialType );
      if( w==null ) return false;

      if( !tipo.Contains( w.sTipo ) ) return false;

      w.StrReserva1 = w.Trad = sTrd;
      w.Traducida = true;
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone valor de la palabra anterior que no sea adverbio y sea = word</summary>
    internal bool IsLastNotDDSetByWord( int iWrd, string word, string sTrd, string ptrAdverbialType )
      {           
      Word w = LastNotType( iWrd, ptrAdverbialType );
      if( w==null ) return false;
            
      string wfind     = ',' + w.Origlwr + ',';           // pone la palabra entre comas
      string wherefind = ',' + word      + ',';           // pone la cadena en que se busca entre comas
        
      if( !wherefind.Contains(wfind) )  return false;

      w.StrReserva1 = w.Trad = sTrd;
      w.Traducida = true;
      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la oración es interrogativa o no y actualiza la propiedad correspondiente</summary>
    internal bool CheckInterrogative()
      {           
      Interrogative = false;

      var nWrds = Words.Count;
      if( nWrds>0 )
        {
        var LastWrd = Words[nWrds-1]; 
        if( LastWrd.Orig == "?" || LastWrd.sTipo == "GS" )
          Interrogative = true;
        }

      return Interrogative;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si toda la oración esta en letras mayusculas y actualiza la propiedad correspondiente</summary>
    internal bool CheckAllWordInUpperCase()
      {
      var check = false;                                              // Bandera para indicar si se chequeo alguna palabra
      for( int i=0; i<Words.Count; ++i )                              // Recorre todas las palabras
        {
        var Wrd = Words[i];                                           // Toma la palabra actual
        if( Wrd.wDiWord != DiWord.WORD ) continue;                    // Si no es del tipo palabra de traducción la ignora

        if( Wrd.wCase!=WCaso.Upper )                                  // Si no esta en mayusculas
          return AllUpper = false;                                    // Pone banera a false y retorna false

        check = true;                                                 // Pone bandera que se chequeo al menos una palabra
        }

      return AllUpper = check;                                        // Retorna verdadero si se chequeo alguna palabra, si no retorna falso
      }     

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la primera letra de la primera palabra esta en mayusculas y actualiza la propiedad correspondiente</summary>
    internal bool CheckFirstLetterInFirstWordUpperCase()
      {
      FirsUpper = ( Words.Count>0 && Words[0].wCase == WCaso.First );
      return FirsUpper;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si todas las palabras de la oración comienzan con mayusculas y actualiza la propiedad</summary>
    internal bool CheckAllWordFirstInUpperCase()
      {
      var check = false;                                              // Bandera para indicar si se chequeo alguna palabra
      for( int i=0; i<Words.Count; ++i )
        {
        var Wrd = Words[i];

        if( Wrd.wDiWord != DiWord.WORD ) continue;

        if( Wrd.wCase!=WCaso.First && Wrd.wCase!=WCaso.Upper && Wrd.wCase!=WCaso.Other )
          {
          string novalid = ";PP;OF;TO;RD;RI;NN;FN;CC;" ;
          if( !novalid.Contains(Wrd.sTipo) )
            return AllFirstUpper = false;
          }

        check = true;
        }

      return AllFirstUpper = check;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary></summary>
    internal void PutMoodAndTimeInGramType()
      {
      foreach( var Wrd in Words )
        {
        if( Wrd.sTipo=="VV" && Wrd.wModo==TMod.Gerundio ) Wrd.sTipo = "VG";
        if( Wrd.sTipo=="VT" && Wrd.wModo==TMod.Gerundio ) Wrd.sTipo = "GT";
        if (Wrd.sTipo=="VI" && Wrd.wModo==TMod.Gerundio ) Wrd.sTipo = "GI";

        if( Wrd.sTipo=="VG" || Wrd.sTipo=="GT" || Wrd.sTipo=="GI" )
            Wrd.wModo = TMod.Gerundio ;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene todos los tipos de la oración en una cadena separados por espacio</summary>
    internal string GetAllTypes()
      {
      var lst = new StringBuilder( 3*Words.Count );                   // Reserva memoria para los tipos

      foreach( var word in Words )                                    // Recorre todas las palabra
        {                                                             
        lst.Append( word.sTipo );                                     // Agrega el tipo a la cadena
        lst.Append( ' ' );                                            // Agrega el espacio separador
        }
                                                                      
      return lst.ToString();                                          // Convierte a cadena 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si no existen diferencias entre los tipos anteriores y los nuevos</summary>
    internal bool TestIsAnyDifferent( ref string sTipos )
      {
      if( sTipos=="FO ") return true;                                 // Si esta reducida a FO, no hay nada que hacer, termina

      string sAllTypes = GetAllTypes();                               // Crea una cadena con todos los tipos

      if( sTipos==sAllTypes ) return true;                            // Si no cambio, retorna true
      else                    sTipos = sAllTypes;                     // Actualiza variable con tipos

      return false;                                                   // Son diferentes, retorna falso
      }

    //------------------------------------------------------------------------------------------------------------------
    static HashSet<string> typSimples = new  HashSet<string> {"XX","FO","GS","GP","GD","RD","RI","AA","AI","AF","AP","AE","SS","VT","VI","VR","FS","FO","FP","FI","FT","FL","FM","PP","TO","BE","VA","PT","PI","GT","GI","BN","BG","TI","NP","CC","DD","OO","OD","OI","OF","PV","PB","OC"};
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Comprueba que todos los tipos en la ora sean simples</summary>
    internal bool CheckAllTypesSimple()
      {
      foreach( var wrd in Words )                                   // Recorre todas las palabras de la oración
        if( !typSimples.Contains(wrd.sTipo) )                       // Si tipo actual no esta en la lista de tipos simples
          {
          AllTypeSimple = false;                                    // Pone bandera a falso
          return false;                                             // Retorna falso
          }

      AllTypeSimple = true;                                         // Pone bandera, todos los tipos son simples
      return true;                                                  // Retorna verdadero
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Llena un arreglo de palabras y tipos de la oración en un momento determinado</summary>
    internal void FillWordAndType()
      {
      if( Words.Count != 0) return;                                 // Si ya se lleno el arreglo no hace nada

      foreach( var wrd in Words )                                   // Recorre todos las palabras de la oración
        {
        if( (wrd.Orig.Length==0 && wrd.Key.Length==0) ||            // Si no hay palabra original o llave para buscar
             wrd.sTipo.Length==0 )                                  // o no tiene tipo gramatical
          continue;                                                 // Ignora la palabra

        if( wrd.Orig.Length>0 )                                     // Si existe palabra original
          {
          sKeys.Add ( wrd.Key  );                                   // Adiciona la llave a lista de llaves
          sWords.Add( wrd.Orig );                                   // Adiciona la palabra a lista de palabras
          }
        else                                                        // Si no hay palabra original => hay llave
          {
          sKeys.Add ( wrd.Key );                                    // Adiciona la llave a lista de llaves
          sWords.Add( wrd.Key );                                    // Adiciona la llave a lista de palabras
          }

        sTypes.Add( wrd.sTipo );                                    // Adiciona el tipo a la lista de tipos
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone un valor a la bandera NoEnComodin en todas las palabras de la oración</summary>
    internal void SetNoEnComodin( bool val )
      {
      foreach( var wrd in Words )
        wrd.NoEnComodin = val;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Convierte los verbos que tengan delante un artículo en sustantivos</summary>
    internal void ConvertVerbInNoun()
      {
      var FuerzaSust = "RI;RD;AI;AY;";
      var TipoNoSust = "VV;VT;VI;BN;BG";
      
      string TipoPrev = "    ";
      foreach( var Wrd in Words )
        {
        if( FuerzaSust.Contains(TipoPrev) && TipoNoSust.Contains( Wrd.sTipo) )
          {
          Wrd.sTipo     = "NP";
          Wrd.Trad      = Wrd.Orig;
          Wrd.Buscada   = true;
          Wrd.Traducida = true;
          }

        TipoPrev = Wrd.sTipo;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Conjuga todas las palabras de la oración que lo necesiten</summary>
    internal void ConjugateVerb( Translate Trd )
      {
      foreach( var Wrd in Words )
        Wrd.ConjugateVerb( Trd );
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Concuerda todas las palabras de la oración que lo necesiten</summary>
    internal void MakeConcordance( Translate Trd )
      {
      foreach( var Wrd in Words )                                     // Recorre todas las palabras de la oración  
        if( !Wrd.Concordada ) Wrd.MakeConcordance(Trd);               // Si no esta concordada, la manda a concordar
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone pronombre reflexivo a todas las palabras de la oración que lo necesiten</summary>
    internal void PutReflexivePronoun( Translate Trd )
      {
      foreach( var Wrd in Words )                                     // Recorre todas las palabras de la oración
        if( !Wrd.Reflexivo ) Wrd.PutReflexivePronoun( Trd );          // Si no se ha puesto el refelexivo lo manda a poner
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Forma la oración traducida</summary>
    internal void MakeTranslatedSentence( Translate Trd )
      {
      var tOra = new StringBuilder( 6*Words.Count );                  // Reserva espacio estimado para toda la oración

      if( SubOracs != null )                                          // Si hay una o más suboración al inicio de la oración
        foreach( var sbOrac in SubOracs )                             // Recorre todas las suboraciones
          tOra.Append( sbOrac.Translate(Trd) );                       // Agrega la traducción de la suboracion
	
      var TypeWithFirstLetterUp   = Trd.LangDes.FindStrData( "MAYUSCULA" );   // Obtiene tipos que requieren primera mayuscula 
      var TypeWithSpaceBeforeWord = Trd.LangDes.FindStrData( "ESPACIO1"  );   // Obtiene tipos que requieren espacio delante
      var TypeWithSpaceAfterWord  = Trd.LangDes.FindStrData( "ESPACIO2"  );   // Obtiene tipos que requieren espacio detras
   
      bool first = true;                                              // Bandera para la primera vez

      for( int i=0; i<Words.Count; ++i)                               // Recorre todas las palabras de la oración
	      {
	      Word w = Words[i];                                            // Palabra que se analiza 

		    if( w.Delete || w.Trad.Length==0 )                            // Si palabra para borrar o no hay traducción
		      {
  		    tOra.Append( w.RtfFijoIni );                                // Agrega Rtf inicial
		      tOra.Append( w.RtfFijoFin );                                // Agrega Rtf final
		      continue;                                                   // Continua con la proxima palabra
		      }

		    if( first && i<Words.Count-1 )                                // Borra artículo si la palabra no lo lleva al inicio de oración
		      {  
			    var wtmp = Words[i+1];                                      // Próxima palabra a la que se analiza 
			    if( wtmp.Articulo == 1 /*&& wtmp.ArtAtFirst*/ )             // Si se le inserto artículo y no lo lleva al inicio de oración
				    {
  		      tOra.Append( w.RtfFijoIni );                              // Agrega Rtf inicial
		        tOra.Append( w.RtfFijoFin );                              // Agrega Rtf final
			      continue;                                                 // Continua con proxima palabra
			      }
		      } 

		    if( AllUpper)                                                 // Si todas las palabras son mayusculas
		      {
		      w.Trad = w.Trad.ToUpper();                                  // Pone traducción a mayusculas
		      }
		    else
		      {    
		      if( first && FirsUpper )                                    // Si el la primera palabra y primera letra mayusculas
            w.Trad = w.Trad.FirstToUpper();                           // Pone a mayusculas primera letra de la traducción

			    first = false;                                              // Quita marcador de primera palabra
    
		      if( (TypeWithFirstLetterUp.Contains(w.sTipo) && w.wDiWord==DiWord.NOMBRE_PROPIO ) ||
              (w.wCase == WCaso.Upper && w.Origlwr.Length==1 )                            )
            w.Trad = w.Trad.FirstToUpper();
		      else if( w.wCase == WCaso.Upper && w.Origlwr.Length>1 && w.Origlwr.IndexOf(' ') == -1)
			      w.Trad = w.Trad.ToUpper();
		      }    

        tOra.Append( w.GetStrTrd(Trd) );                              // Pone la traducción 

        if( Trd.LangDes.Lang == TLng.Es )                             // Si es el idioma español    
          {
		      if( w.Trad == "?" ) tOra.Insert( 0, '¿' );                  // Es final de interrogación, pone signo inicial
		      if( w.Trad == "!" ) tOra.Insert( 0, '¡' );                  // Es final de admiración, pone signo inicial
          }
    
			  if( tOra[tOra.Length-1] == ' ' )                              // Si ya hay un espacio al final
          continue;                                                   // Continua con la proxima palabra
			  else if( TypeWithSpaceBeforeWord.Contains(w.sTipo) )          // Si el tipo no requiere un espacio al pincipio
          continue;                                                   // Continua con la proxima palabra
        else if( TypeWithSpaceAfterWord.Contains(w.sTipo) )           // Si el tipo no requiere un espacio al final
          continue;                                                   // Continua con la paxima palabra
        else if( i<Words.Count-1)                                     // Si no es la última palabra
				  tOra.Append(' ');                                           // Agrega el espacio al final

		    }                                                             // Continua con la proxima palabra

      Dest = tOra.ToString();                                         // Covierte a cadena y la poen en destino  
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE Sentence     +++++++++++++++++++++++++++++++++++++++++++++

  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ///<summary>Almacena la información de una oración que esta dentro de otra</summary>
  public class SubOrac
    {
    public int     Entre;     // 0 entre nada, 1 entre parentesis, 2 entre comillas, 3 entre comillas simples, 4 entre corchetes
    public string  Orac;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa un objeto sub-oración con el texto de la oración y entre que esta encerrado</summary>
    public SubOrac( string Orac, int EntreQue )
      {
      this.Entre = EntreQue;
      this.Orac     = Orac;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la traducción de la suboración</summary>
    public string Translate( Translate Trd )
      {
      string sTrd = "";

      sTrd += Utils.EntreIni(Entre);                                  // Le pone el caracter inicial que la encierra
      sTrd += Trd.DoSubTranslate( Orac );                             // Obtiene a agrega la traducción de la suboración
      sTrd += Utils.EntreEnd(Entre);                                  // Le pone el caracter final que la encierra

      return sTrd;                                                    // Traducción de la suboración en cerrada por los caracteres correspondientes
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE SubOrac      +++++++++++++++++++++++++++++++++++++++++++++

  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE TrdEngine  +++++++++++++++++++++++++++++++++++++++++++++
