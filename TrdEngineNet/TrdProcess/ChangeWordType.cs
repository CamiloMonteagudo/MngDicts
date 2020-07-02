using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine.TrdProcess
  {
  public class ChangeWordType
    {
    Translate  Trd;                               // Todos los datos de la traducción
    List<Word> Words;                             // Lista de las palabras de la oración
    Word       Wrd = null;                        // Palabra actual
    int        iWord   = 0;                       // Indice a la palabra actual

    delegate string ChgType( int iWrd, string Tipo, string Pref );    // Delegado para cambio de tipo gramatical
    delegate string SWType();                                         // Delegado para obtención del tipo gramatical en palabras especiales

    ChgType ChageWrdType       = null;            // Función para cambio de tipo gramatical según el idioma fuente
    SWType  GetSpecialWordType = null;            // Función para obtención del tipo gramatical en palabras especiales según el idioma fuente

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa la clase para cambiar los tipos gramaticales</summary>
    public ChangeWordType( Translate Trd, List<Word> words = null )
      {
      this.Trd = Trd;                                                 // Establece todos los datos para la traducción
      Words    = (words==null)? Trd.Ora.Words : words;                // Establece la lista de palabras sobre las que se va ha trabajar
                                        
      SetDelegates( Trd.LangSrc.Lang );                        
      }

    //------------------------------------------------------------------------------------------------------------------
    // Constructor especial para inicializar los valores staticos de la clase
    private ChangeWordType(){}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone los delegados de acuerdo al idiomas de traducción</summary>
    public void SetDelegates( TLng lng )
      {
      switch( lng )                                                   // inicializa los delegados, según el idioma fuente
        {
        case TLng.Es:                                                 // Cambios de tipos y palabras especiales para el Español
          ChageWrdType       = ChageTypeEs; 
          GetSpecialWordType = GetSpecialWordTypeEs; 
          break;
        case TLng.En:                                                 // Cambios de tipos y palabras especiales para el Inglés
          ChageWrdType       = ChageTypeEn;                           
          GetSpecialWordType = GetSpecialWordTypeEn; 
          break;
        case TLng.It:                                                 // Cambios de tipos y palabras especiales para el Italiano
          ChageWrdType       = ChageTypeIt; 
          GetSpecialWordType = GetSpecialWordTypeIt; 
          break;
        case TLng.De:                                                 // Cambios de tipos y palabras especiales para el Alemán
          ChageWrdType       = ChageTypeDe; 
          GetSpecialWordType = GetSpecialWordTypeDe; 
          break;
        case TLng.Fr:                                                 // Cambios de tipos y palabras especiales para el Francés
          ChageWrdType       = ChageTypeFr; 
          GetSpecialWordType = GetSpecialWordTypeFr; 
          break;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa todas las palabras de la oración y cambia el tipo, si es necesario</summary>
    public static void Process(  List<Word> words, Translate Trd, string TypPref )
      {
      var ChgTyp = new ChangeWordType( Trd, words );
      ChgTyp.Process( TypPref );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inicializa el diccionario asociado al switch con los tipos</summary>
    public static void Inicialize( Translate Trd  )
      {
      var ChgTyp = new ChangeWordType();
      ChgTyp.SetDelegates( Trd.LangSrc.Lang );

      ChgTyp.ChageWrdType( 0, "", "" );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Restaura los tipos cambiado en el proceso de cambio de tipo</summary>
    public static void Restore( List<Word> Words )
      {
      foreach( var Wrd in Words )
        {
        if( Wrd.StrReserva5.Length>0 )
          Wrd.sTipo = Wrd.StrReserva5;
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa una palabras de la oración y cambia el tipo, si es necesario</summary>
    public string ProcessWord( int iWrd, string TypPref )
      {
      iWord   = iWrd;
      Wrd = Words[iWrd];

      return ChageWrdType( iWrd, Wrd.sTipo, TypPref );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras con tipo SW (Special Word), estas son las palabras que antes
    ///tenian lenguage de diccionario en la determinación del tipo gramatical</summary>
    public string ProcessSpecialWord( int iWrd )
      {
      iWord   = iWrd;
      Wrd = Words[iWrd];

      return GetSpecialWordType();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Procesa todas las palabras de la oración y cambia el tipo, si es necesario</summary>
    private void Process( string TpPref )
      {
      for( int i=0; i<Words.Count; i++ )
        {
        iWord    = i;
        Wrd  = Words[i];
        Wrd.StrReserva5 = Wrd.sTipo;

        bool bReflexive = Wrd.Reflexivo;

        var newTipo = ChageWrdType( i, Wrd.sTipo, TpPref );
        if( newTipo==null ) continue;

        Wrd.sTipo     = newTipo;
        Wrd.Reflexivo = bReflexive;
        }
      }

    #region "Fuciones para sumular lenguaje de diccionario"
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la proxima palabra que no es un advervio</summary>
    private Word GetNNDD()
      {
      for( int i=iWord+1; i<Words.Count; ++i )
        if( Words[i].sTipo != "DD" )
          return Words[i];

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la palabra anterior que no es un advervio</summary>
    private Word GetLNDD()
      {
      for( int i=iWord-1; i>=0; --i )
        if( Words[i].sTipo != "DD" ) 
          return Words[i];

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo de la palabra actual es el tipo dado</summary>
    private bool isTipo(string tipo) { return Wrd.sTipo==tipo;  }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tiempo de la palabra actual es igual al tiempo dado</summary>
    private bool isTime( TTime tm ) { return Wrd.wTiempo==tm; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tiempo de la palabra actual es igual al tiempo dado</summary>
    private bool isPlural() { return Wrd.wNumero==TNum.Plural; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tiempo de la palabra actual es igual al tiempo dado</summary>
    private bool isPerson( int p ) { return Wrd.wPersona==(TPer)(p-1); }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra actual es la ultima de la lista de palabras</summary>
    private bool isLastW() { return iWord==Words.Count-1; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la ultima palabra es la palabra dada</summary>
    private bool isLastW(string w) { return Words[Words.Count-1].Origlwr==w; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra actual empieza con muyusculas</summary>
    private bool isFUpper() { return Wrd.wCase == WCaso.First; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el modo de la ultima palabra es igual al modo de la palabra actual</summary>
    private bool isMode( TMod mode ) { return  Wrd.wModo==mode; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el modo de la ultima palabra es igual al modo de la palabra actual</summary>
    private bool isModeLW() { return Words[Words.Count-1].wModo == Wrd.wModo; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra 'i' posterior a la actual tiene uno de los tipos de la lista</summary>
    private bool isTN( int i, string Tipos)
      {
      if( iWord+i >= Words.Count ) return false;
      return Tipos.Contains( Words[iWord+i].sTipo );
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra 'i' anterior a la actual tiene uno de los tipos de la lista</summary>
    private bool isTL( int i, string Tipos)
      {
      if( iWord-i < 0 ) return false;
      return Tipos.Contains( Words[iWord-i].sTipo );
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo de la palabra anterior no advervio es uno de los de la lista</summary>
    private bool isTLNDD( string Tipos)
      {
      var Wrd = GetLNDD();
      return Wrd!=null && Tipos.Contains(Wrd.sTipo);
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo de la proxima palabra no advervio es uno de los de la lista</summary>
    private bool isTNNDD(string Tipos)
      {
      var Wrd = GetNNDD();
      return Wrd!=null && Tipos.Contains(Wrd.sTipo);
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra anterior no advervio es una de las de la lista</summary>
    private bool isWLNDD(string sWords)
      {
      var s = ',' + sWords + ',';
      var Wrd = GetLNDD();
      return Wrd!=null && s.Contains( ',' + Wrd.Origlwr + ',' ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra posterior no advervio es una de las de la lista</summary>
    private bool isWNNDD(string sWords)
      {
      var s = ',' + sWords + ',';
      var Wrd = GetNNDD();
      return Wrd!=null && s.Contains( ',' + Wrd.Origlwr + ',' ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra 'i' posterior a la actual es una de las palabras de la lista</summary>
    private bool isWN(int i,string sWords)
      {
      if( iWord+i >= Words.Count ) return false;

      var s = ',' + sWords + ',';
      return s.Contains( ',' + Words[iWord+i].Origlwr + ',' ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra 'i' anterior es una las palabras de la lista</summary>
    private bool isWL(int i,string sWords)
      {
      if( iWord-i < 0 ) return false;

      var s = ',' + sWords + ',';
      return s.Contains( ',' + Words[iWord-i].Origlwr + ',' ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra actual es una de las palabras de la lista</summary>
    private bool isWrd(string sWords)
      {
      var s = ',' + sWords + ',';
      return s.Contains( ',' + Wrd.Origlwr + ',' ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra actual termina con el sufijo dado</summary>
    private bool isSufix( string suff )
      {
      return Wrd.Origlwr.EndsWith(suff); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra actual termina con uno de los sufijos de la lista</summary>
    private bool isOneSufix( string suffs )
      {
      var sufijos = suffs.Split(',');
      foreach( var suf in sufijos )
        if( Wrd.Origlwr.EndsWith(suf) ) return true;
      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la proxima palabra no advervio termina con el sufijo dado</summary>
    private bool isSufixNNDD( string suff )
      {
      var wrd = GetNNDD();
      return wrd!=null && wrd.Origlwr.EndsWith(suff); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la proxima palabra no advervio termina con uno de los sufijos de la lista</summary>
    private bool isOneSufixNNDD( string suffs )
      {
      var wrd = GetNNDD();
      if( wrd==null ) return false;

      var sufijos = suffs.Split(',');
      foreach( var suf in sufijos )
        if( wrd.Origlwr.EndsWith(suf) ) return true;

      return false;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra i que sigue a la palabra actual tiene el modo dado</summary>
    private bool isModeN( int i, TMod mode )
      {
      if( iWord+i >= Words.Count ) return false;

      return ( Words[iWord+i].wModo == mode ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la prxima palabra no advervio tiene el modo dado</summary>
    private bool isModeNNDD( TMod mode )
      {
      var wrd = GetNNDD();
      return ( wrd!=null && wrd.wModo == mode ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la prxima palabra no advervio tiene el tiempo dado</summary>
    private bool isTimeNNDD( TTime tm )
      {
      var wrd = GetNNDD();
      return ( wrd!=null && wrd.wTiempo == tm ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra i que sigue a la palabra actual termina con el sufijo dado</summary>
    private bool isSufixN( int i, string suff )
      {
      if( iWord+i >= Words.Count ) return false;
      return Words[iWord+i].Origlwr.EndsWith(suff); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra i que sigue a la palabra actual termina con uno de los sufijos de la lista</summary>
    private bool isOneSufixN( int i, string suffs )
      {
      if( iWord+i >= Words.Count ) return false;

      var sufijos = suffs.Split(',');
      foreach( var suf in sufijos )
        if( Words[iWord+i].Origlwr.EndsWith(suf) ) return true;
      return false;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra dada, existe a la izquierda de la palabra actual</summary>
    private bool isLeft( string sWrd )
      {
      for( int i=iWord-1; i>=0; --i )
        if( Words[i].Origlwr == sWrd ) 
          return true;

      return false;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tipo dado, existe a la izquierda de la palabra actual</summary>
    private bool isLeftT( string Tipo )
      {
      for( int i=iWord-1; i>=0; --i )
        if( Words[i].sTipo == Tipo ) 
          return true;

      return false;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si el tiempo de la palabra i siguiente a la actual es igual al tiempo dado</summary>
    private bool isTimeN( int i, TTime tiempo)
      {
      if( iWord+i >= Words.Count ) return false;

      return ( Words[iWord+i].wTiempo == tiempo ); 
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si la palabra previa no advervio es la primera palabra de la oración</summary>
    private bool isLNDDFW()
      {
      for( int i=iWord-1; i>=0; --i )
        if( Words[i].sTipo != "DD" ) 
          return (i==0);

      return false;
      }
    #endregion

    #region "Comandos para sumular lenguaje de diccionario"
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el modo a la palabra actual</summary>
    private void Mode  (int m) { Wrd.wModo = (TMod)(m); }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el modo a la palabra i que sigue a la actual</summary>
    private void ModeN( int i, int m ) { if( iWord+i<Words.Count ) Words[iWord+i].wModo = (TMod)m; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la palabra actual como plurar</summary>
    private void Plural() { Wrd.Plural = true; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la persona a la palabra actual palabra actual</summary>
    private void Person(int p) { Wrd.wPersona = (TPer)(p-1); }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone el tiempo a la palabra actual</summary>
    private void Time  (int t) { Wrd.wTiempo  = (TTime)(t); }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta un articulo a la palabra actual</summary>
    private void InsW  (int n) { Wrd.Articulo = (short)n; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Marca para borrar la primera palabra no advervio anterior a la palabra actual</summary>
    void DelLNDD()
      {
      var wrd = GetNNDD();
      if( wrd!=null ) wrd.Delete = true;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Marca para borrar la palabra i con respecto a la palabra actual</summary>
    void Del(int i)
      {
      i = iWord + i;
      if( i>=0 && i<Words.Count ) Words[i].Delete = true;
      }
    
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta un articulo en la palabra anterior a la actual que no sea advervio</summary>
    private void InsLNDD(int n)
      {
      var wrd = GetLNDD();
      if( wrd!=null ) Wrd.Articulo = (short)n;
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta un articulo en la palabra i, anterior a la actual</summary>
    private void InsL(int i, int n)
      {
      if( iWord-i < 0 ) Words[iWord-i].Articulo = (short)n;
      }
    #endregion

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras especiales del Español</summary>
    private string GetSpecialWordTypeEs()
      {
      switch( Wrd.Orig )
        {
        case "algo" :if( isTNNDD("BE,HA,VV,VA") || isTN(1,"AA") || isTLNDD("VV")) return "OP";
                                                                                  return "DD";
        case "así"  :if( isWN( 1, "," ) )                     return "CC";
                     if( iWord==0 && isTNNDD("PP,RD,RI,AI") ) return "VT";
                     if( isWLNDD("me") )                      return "VR";
                     if( isTN(1,"SS") )                       return "AA";
                                                              return "DD";
        case "donde":if( isWN(1,"el,la,los,las,lo") )         return "PP";
                     if( isWN(1,"se,ser,estar")     )         return "OL";
                                                              return "OZ";
        case "era"  :if( isWL( 1,"la" ) )                     return "SS"; 
                                                     Time(1); return "BE";
        case "este" :if( isTLNDD("RI,RD") )                   return "SS"; 
                                                              return "OA";
        case "la"   :if( isTNNDD("VV,VT,VI,HA,BE,VD") )       return "OC"; 
                                                              return "RD";
        case "las"  :if( isTNNDD("VT,HA,BE,VI,VV,VA") )       return "OC"; 
                                                              return "RD";
        case "lo"   :if( isTNNDD("AA,AI") )                   return "RD"; 
                                                              return "OC";
        case "pero" :if( isTLNDD("RD,RI") )                   return "SS"; 
                                                              return "CC";
        case "sobre":if( isTLNDD("RD,RI") )                   return "SS"; 
                                                              return "PP";
        case "una"  :if( isWLNDD("yo,ella,él") || isTLNDD("OP") )   return "VV"; 
                                                                    return "RI";
        case "uno"  :if( isWLNDD("yo") || isTLNDD("OP") || (iWord==0 && isWNNDD("el,la,los,las,lo")) )  return "VV"; 
                                                                                                        return "OO";

        case "estamos": Person(1); Plural(); return "BE";
        case "fue"    :    Time(1); Mode(1); return "BE";
        }

      return Wrd.sTipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras especiales del Inglés</summary>
    private string GetSpecialWordTypeEn()
      {
      return Wrd.sTipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras especiales del Italiano</summary>
    private string GetSpecialWordTypeIt()
      {
      return Wrd.sTipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras especiales del Alemán</summary>
    private string GetSpecialWordTypeDe()
      {
      return Wrd.sTipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el tipo gramatical de las palabras especiales del Francés</summary>
    private string GetSpecialWordTypeFr()
      {
      return Wrd.sTipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia el tipo de la palabra para el idioma Español</summary>
    private string ChageTypeEs( int iWrd, string Tipo, string Pref )
      {
      switch( Pref + Tipo )
        {
        case "CAE":
        case "CAF":
        case "CAN": return "AI";
        case "CDF": return "DD";
        case "CGI":
        case "CGT": return "VG";
        case "CHI":
        case "CHT": return "VV";
        case "CJI":
        case "CJT": return "VD";
        case "CPI":
        case "CPT": return "VP";
        case "CVI":
        case "CVT": return "VV";
        case "TGI": return "VI";
        case "TGT": return "VT";
        case "THI": return "VI";
        case "THT": return "VT";
        case "TJI": return "VI";
        case "TJT": return "VT";
        case "TPI": return "VI";
        case "TPT": return "VT";
        case "P0VJ":
        case "P0VW":
        case "P0VX": return "VA";
        case "P3VW":
        case "P0VZ": return "VA";
        case "P3BD":
        case "P3BF":
        case "P3BH":
        case "P3BI":
        case "P3BJ":
        case "P3BK":
        case "P3BM":
        case "P3BP":
        case "P3BS": return "BE";
        case "P3HC": 
        case "P3HD": 
        case "P3HF": 
        case "P3HJ": 
        case "P3HM": 
        case "P3HP": 
        case "P3HV": return "HA";
        case "P3VB": 
        case "P3VE": 
        case "P3VF": 
        case "P3VH": 
        case "P3VJ": 
        case "P3VQ": 
        case "P3VU": 
        case "P3VX": 
        case "P3VY": return "VA";
        case "P3VZ": return "VA";
        case "P0DN": return "DN";
        case "P0WM": return "XJ";
        case "P0WL":
        case "P0WN":
        case "P0WP":
        case "P0WQ":
        case "P0WR":
        case "P0WS":
        case "P0WT":
        case "P0WV":
        case "P0WW":
        case "P0WX":
        case "P0WY":
        case "P0XB":
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTipo("FW") || isTNNDD("OF") )  return "SS";
          else if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                          return "AA";
          else                                                                                    return "VT";
        case "P0JK": Mode(6); return "VP";
        case "P0JL": Mode(6); return "VP";
        case "P1BG": Mode(5); return "BG";
        case "P1BI": Mode(1); return "BI";
        case "P1HD": Mode(1); return "HD";
        case "P1BS": Mode(4); return "BS";
        case "P1BP": Mode(3); return "BP";
        case "P1BM": Mode(2); return "BM";
        case "P1BN": Mode(6); return "BN";
        case "P1VU": Mode(6); return "VU";
        case "P1HG": Mode(5); return "HG";
        case "P1HJ": Mode(4); return "HJ";
        case "P1HM": Mode(2); return "HM";
        case "P1HN": Mode(6); return "HN";
        case "P1HP": Mode(3); return "HP";
        case "P1VJ": Mode(4); return "VJ";
        case "P1VL": Mode(1); return "VL";
        case "P1VW": Mode(2); return "VW";
        case "P1VX": Mode(1); return "VX";
        case "P1VY": Mode(5); return "VY";
        case "P1VZ": Mode(3); return "VZ";
        case "P2VK": Mode(4); return "VK";
        case "P2VL": Mode(1); return "VL";
        case "P2VM": Mode(3); return "VM";
        case "P2VN": Mode(2); return "VN";
        case "P2VP": Mode(6); return "VP";
        case "P0YH": return isTLNDD("SS")? "AA" : "BO" ; 
        case "P0YI": return isTLNDD("SS")? "AA" : "BL" ; 
        case "P0YJ": return isTLNDD("SS")? "AA" : "BX" ; 
        case "P0YK": return isTLNDD("SS")? "AA" : "BR" ; 
        case "P0YL": return isTLNDD("SS")? "AA" : "BZ" ; 
        case "P0YM": return isTLNDD("SS")? "AA" : "BY" ; 
        case "P0YN": return isTLNDD("SS")? "AA" : "VM" ; 
        case "P1SP": Plural(); return "SS"; 
        case "MVP" : Mode(6);  return Tipo;
        case "P2VG": Mode(5);  return "VG";  
        case "FVT" :      if( isMode(TMod.SubjuntivoImperativo) ) return "BZ";
                     else if( isMode(TMod.Imperativo)           ) return "VN";
                     else if( isMode(TMod.Indicativo)           ) return "VL";
                     else                                                return "VT";
        case "MBE":
          if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo") )
                                                                                        Mode(0);
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA"))  Mode(0);
          else if( isOneSufix("ado,ido"    ) && isTNNDD("RD,RI,NP") )                   Mode(1);
          else if( isOneSufix("ado,ido"    ) && isTLNDD("HA,BE") )                      Mode(6);
          else if( isOneSufix( "ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")) )    Mode(0);
          else if( isOneSufix("ando,iendo" ) && isTLNDD("BE") )                         Mode(5);
          else if( isWLNDD("a,para,por") )                                              Mode(0);
          else                                                                          Mode(1);
          break;
        case "P0QK":
        case "P0QJ":
          if( iWrd==0 && isTNNDD("OL,CC") )   return "VT";
          if( isLastW() )                     return "SS";
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          else                                return "VA";
        case "MHA":
          if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo") )
                                                                                        Mode(0);
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA") ) Mode(0);
          else if( isOneSufix("ado,ido") && isTNNDD("RD,RI,NP") )                       Mode(1);
          else if( isOneSufix("ado,ido") && isTLNDD("HA,BE"   ) )                       Mode(6);
          else if( isOneSufix("ando,iendo") && ( iWrd==0 || isWLNDD("a,para,por")) )    Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                          Mode(5);
          else if( isWLNDD("a,para,por") )                                              Mode(0);
          else                                                                          Mode(1);
          break;
        case "MVA":
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo))
            { Mode(2); InsW(0); return "VT";}
          else if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); Mode(0);
            }
          else if( isWLNDD("deber  de,tener que, deber,tener,poder" ) || isTLNDD("VA,TO") )   Mode(0);
          else if( isOneSufix("ado,ido") && isTNNDD("RD,RI,NP") )                             Mode(1);
          else if( isOneSufix("ado,ido") && isTLNDD("HA,BE"   ) )                             Mode(6);
          else if( isOneSufix("ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")) )           Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                                Mode(5);
          else if( isWLNDD("a,para,por") )                                                    Mode(0);
          else                                                                                Mode(1);
          break;
        case "MVI":
          if( (iWrd==0 || isTLNDD("GZ") && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )
            { Mode(2); InsW(0); return "VI";}
          else if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo" ) )
            { if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); else Mode(0);}
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA,TO") )  Mode(0);
          else if( isOneSufix("ado,ido"   ) && isTNNDD("RD,RI,NP") )                        Mode(1);
          else if( isOneSufix("ado,ido"   ) && isTLNDD("HA,BE"   )  )                       Mode(6);
          else if( isOneSufix("ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")) )         Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                              Mode(5);
          else if( isWLNDD("a,para,por") )                                                  Mode(0);
          else if( isWN(1,"que") || isWN(2,"que") )                                         Mode(2);
          else                                                                              Mode(1);
          break;
        case "MVR" : 
          if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo"))
           { if( iWrd==0 ) Mode(5); else Mode(0); }
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA"))      Mode(0);
          else if( isOneSufix("ado,ido"   ) && isTNNDD("RD,RI,NP") )                        Mode(1);
          else if( isOneSufix("ado,ido"   ) && isTLNDD("HA,BE"   ) )                        Mode(6);
          else if( isOneSufix("ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")))          Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                              Mode(5);
          else if( isWLNDD("a,para,por"))                                                   Mode(0);
          else                                                                              Mode(1);
          break;
        case "MVT" : 
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) 
            { Mode(2); InsW(0); return "VT";}
          if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo") ) 
            { if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); else Mode(0); }
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA,TO") )  Mode(0);
          else if( isOneSufix("ado,ido"   ) && isTNNDD("RD,RI,NP") )                        Mode(1);
          else if( isOneSufix("ado,ido"   ) && isTLNDD("HA,BE"   ) )                        Mode(6);
          else if( isOneSufix("ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")))          Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                              Mode(5);
          else if( isWLNDD("a,para,por")  )                                                 Mode(0);
          else                                                                              Mode(1);
          break;
        case "MVV" : 
          if( isOneSufix("ar,er,ir,arse,erse,irse,arme,erme,irme,arle,erle,irle,arnos,ernos,irnos,arlos,erlos,irlos,arles,erles,irles,arlo,erlo,irlo") )
            { if( iWrd==0)  Mode(5); else Mode(0); }
          else if( isWLNDD("deber  de,tener que, deber,tener,poder") || isTLNDD("VA") )     Mode(0);
          else if( isOneSufix("ado,ido"   ) && isTNNDD("RD,RI,NP") )                        Mode(1);
          else if( isOneSufix("ado,ido"   ) && isTLNDD("HA,BE") )                           Mode(6);
          else if( isOneSufix("ando,iendo") && (iWrd==0 || isWLNDD("a,para,por")) )         Mode(0);
          else if( isOneSufix("ando,iendo") && isTLNDD("BE") )                              Mode(5);
          else if( isWLNDD("a,para,por") )                                                  Mode(0);
          else                                                                              Mode(1);
          break;
        case "NVT" : 
        case "NVI" : 
          if( isMode(TMod.Potencial)  )                           return "VM";
          if( isMode(TMod.Indicativo) )                           return "VL";
          if( isOneSufix("amos,emos,imos")   )                    return "VL";
          if( isMode(TMod.Participio) )                           return "VP";
          if( isOneSufix("ado,ada,ados,adas,ido,ida,idos,idas") ) return "VP";
          if( isMode(TMod.Gerundio)   )                           return "VG";
          if( isOneSufix("ando,iendo") )                          return "VG";
          break;
        case "NVV" : 
          if( isMode(TMod.Participio) )                           return "VP";
          if( isOneSufix("ado,ada,ados,adas,ido,ida,idos,idas") ) return "VP";
          if( isMode(TMod.Gerundio) )                             return "VG";
          if( isOneSufix("ando,iendo" ) )                         return "VG";
                                                                  return "VV";
        case "P0AO": 
          if( isWLNDD("el,la,los,las,lo") || isTN(1,"VV,VT,VI,VR,HA,BE") ) return "OO";
          else                                                             return "AI";
        case "P0AS": 
          if( isFUpper() )  return "NP";
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")))  return "SS";
                            return "AA";
        case "P0HA": 
          if( isTLNDD("RD,RI,AD,AI") ) return "SS"; 
          else                         return "HA";
        case "P0IA": 
          if( isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC"; 
          if( isTNNDD("AA,AI,AO,XJ,SS,QC")       ) return "RD";
                                                   return "OC";
        case "P0IS": 
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                         return "AA";
                                                                                            return "VV";
        case "P0MU": 
        case "P0MG": 
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS")          )    return "OG";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") )    return "OG"; 
          if( isTL(1,"SS,XJ,SP") )                                     return "AF"; 
          if( isTL(2,"SS") && isTL(1,"AA,XJ") )                        return "AF";  
          if( iWrd==0 && isTN(1,"SS") )                                return "AF"; 
          if( isTNNDD("OF") && isTL(1,"SS,AA") )                     return "AF";
          if( isTLNDD("PP,TO,RD,RI,AI") && (isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "OG";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )       return "AF";
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"OG")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "OG";
                                                                       return "AF";
        case "P0MV": 
          if( isTLNDD("RD,RI,AI,AL") ) return "AN";
          return "NN";
        case "P0OD": 
               if( isTN(1,"AA")  ) return "DD"; 
          else if( isTN(1,"SS")  ) return "AA"; 
          else                     return "OO";
        case "P0OF": 
          if( isOneSufixNNDD("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,urle,urla,urlo,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") ) 
            return "TO"; else return "OF";
        case "P0OJ": 
          if( isWL(1,"¡") && isWN(1,"!") ) return "JJ";
          return "OO";
        case "P0OZ": 
          if( isTLNDD("OL") || iWrd==0 ) return "OO"; 
          if( isTN(1,"AA") )             return "DD"; 
          if( isTL(1,"SS") )             return "AA"; 
                                         return "DD";
        case "P0PP": 
          if( isOneSufixNNDD("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,urle,urla,urlo,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") ) 
            return "TO"; else return "PP";
        case "P0QA": 
          if( isOneSufix("a,e,ono,ano") && (isTL(1,"SE,OO") || (iWrd==0 || isTLNDD("SE"))) ) return "VV"; 
          if( isOneSufix("are,urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle;irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") && isTL(1,"TO,VA") ) 
                                                                  return "VV"; 
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") )  return "SS"; 
                                                                  return "DD";
        case "P0QB": 
        case "P0QC": 
        case "P0QD": 
          if( isTLNDD("HA") ) return "VP";
          return "XJ";
        case "P0QE": 
          if( isOneSufix("ato,uto") && !isTLNDD("HA") ) return "XM"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS")  )    return "XM"; 
          if( isOneSufix("ato,uto")  )                  return "VP"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )      return "VR"; 
                                                        return "VV";
        case "P0QF": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                                            return "VR"; 
                                                                                                           return "VV";
        case "P0QG": 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0QN": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                   return "AI"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AI"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") )                                return "VV"; 
          if( isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AI";
        case "P0QO": 
          if( isTLNDD("BE,SS,AA") )       return "AI"; 
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS"; 
                                          return "DD";
        case "P0QS": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )                return "VL"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                     return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                return "VL"; 
          if( isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AA";
        case "P0QT": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )                return "BY"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                     return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                return "BY"; 
          if( isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AA";
        case "P0QU": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )                return "BL"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                     return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                return "BL"; 
          if( isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AA";
        case "P0RD": 
          if( isWrd("la") && isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC"; 
                                                                  return "RD";
        case "P0SE": 
          if( iWrd==0 && isModeNNDD(TMod.Potencial) ) {Mode(1);                   return "BE";}
          if( isWN(1,"la,lo,le,las,los,les,me,nos")  && isTN(2,"VR") )            return "OC"; 
          if( isWN(1,"la,lo,le,las,los,les,me,nos") && !isTN(2,"VR") ) {Mode(1);  return "BE";}
          if( isTLNDD("OL,CC") || isTN(1,"VT,VI,VV,TX,ZB,IS,XA,XB,ZE,ZF,ZL,VA,VR,ZV,ZG,ZA,YF,ZH,SV,ZJ") || isWNNDD("debe")) {ModeN(1,6); Mode(1); return "BE";}
                                                                                  return "OR";
        case "P0SV": 
          if( isOneSufix("o,e,en"  ) && (isTL(1,"SE,OO") || iWrd==0) )                                     return "VV"; 
          if( isOneSufix("ar,er,ir") && isTL(1,"TO,VA") )                                                  return "VV"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "SS";
        case "P0TA": 
          if( isTLNDD("OO") || iWrd==0 ) return "VV"; 
          if( isTN(1,"AA") )             return "DD";
                                         return "SS";
        case "P0TO": 
          if( isOneSufixN(1,"ar,er,ir,arse,erse,irse,arme,erme,irme,arnos,ernos,irnos")) {ModeN(1,0);return "TO";} else return "PP";
        case "P0TX": 
          if( isWLNDD("me,le,les,te,nos,las") ) return "VR"; 
                                                return "VV";
        case "P0UA": 
          if( isFUpper() )  return "NP"; 
                            return "SS";
        case "P0UB": 
          if( isTLNDD("HA") ) return "VP"; 
                              return "AA";
        case "P0UC": 
          if( isTLNDD("HA") ) return "VP"; 
                              return "SS";
        case "P0UF": 
          if( iWrd==0 && isTNNDD("OL,CC") )                                                                return "VT"; 
          if( isModeLW() )                                                                                 return "SP"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP"; 
                                                                                                           return "VT";
        case "P0UG": 
          if( iWrd==0 && isTNNDD("OL,CC") )                                                                 return "VT"; 
          if (isModeLW() )                                                                                  return "SP"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SP"; 
                                                                                                            return "VV";
        case "P0UH": 
          if( isTLNDD("HA") ) return "VP"; 
                              return "AA";
        case "P0UL": 
          if( iWrd==0 && isTNNDD("OL,CC") )                                                                 return "VT"; 
          if( isModeLW() )                                                                                  return "SP"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SP"; 
                                                                                                            return "VV";
        case "P0UO": 
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") )    return "AA"; 
          if( isTLNDD("RD,RI,AI") )                     return "SS"; 
          if( iWrd==0 && isModeLW() )                   return "SS"; 
          if( iWrd==0 && isWN(1,"di") )                 return "SS"; 
          if( isOneSufix("ato,uto") && !isTLNDD("HA") ) return "AA"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )     return "AA"; 
          if( isOneSufix("ato,uto") )                   return "VP"; 
                                                        return "VV";
        case "P0UR": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )                return "VR"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                     return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                return "VV"; 
          if (isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AA";
        case "P0UW": 
          if( isTLNDD("HA") ) return "VP"; 
                              return "AA";
        case "P0UZ": 
          if( isTLNDD("HA") ) return "VP"; 
                              return "SS";
        case "P0VI": 
          if( isMode(TMod.Gerundio)   ) return "GI"; 
          if( isMode(TMod.Participio) ) return "PI"; 
                                        return "VI";
        case "P0VT": 
          if( isMode(TMod.Gerundio)   ) return "GT"; 
          if( isMode(TMod.Participio) ) return "PT"; 
                                        return "VT";
        case "P0WA": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
                                                                                                            return "HD";
        case "P0WB": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
                                                                                                            return "BS";
        case "P0WC": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
                                                                                                            return "VJ";
        case "P0WD": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
                                                                                                            return "VB";
        case "P0WE": 
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") )   return "VP"; 
                                                                          return "DD";
        case "P0WF": 
          if( isTLNDD("SS,NP") )  return "VL"; 
                                  return "DD";
        case "P0WH": 
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BL"; 
                                               return "DD";
        case "P0WJ": 
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BY"; 
                                               return "DD";
        case "P0WK": 
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") )                 return "VU"; 
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") )                              return "SS"; 
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") )                     return "SS"; 
          if( isTL(1,"SS,XJ,SP") )                                                      return "AA"; 
          if( isTL(2,"SS") && isTL(1,"AA,XJ") )                                         return "AA"; 
          if( iWrd==0 && isTN(1,"SS") )                                                 return "AA"; 
          if( isTNNDD("OF") && isTL(1,"SS,AA") )                                        return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) )    return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                        return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
                                                                                        return "AA";
        case "P0WZ": 
          if( isTL(1,"PP,TO") ) return "OC"; 
                                return "OP";
        case "P0XA": 
          if( iWrd==0 && isModeLW() )                             return "SS"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                   return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                return "VV"; 
          if( isTN(1,"AA") )                                      return "DD"; 
                                                                  return "AA";
        case "P0XC": 
          if( isOneSufix("ato,uto") && !isTLNDD("HA") ) return "XM"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )     return "XM"; 
          if( isOneSufix("ato,uto") )                   return "VP"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )      return "VR"; 
                                                        return "VV";
        case "P0XD": 
          if( isOneSufix("ato,uto") && !isTLNDD("HA") ) return "XM"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )     return "XM"; 
          if( isOneSufix("ato,uto") )                   return "VP"; 
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )      return "VR"; 
                                                        return "VV";
        case "P0XE": 
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTNNDD("OF") )  return "AA"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                               return "AA"; 
                                                                                  return "VV";
        case "P0XF": 
          if( isWLNDD("me,le,les,te,nos,las") ) return "VR"; 
                                                return "VV";
        case "P0XG": 
          if( Wrd.Entre==2 && isTL(1,"SS,AA") )                                                               return "SS"; 
          if( isTLNDD("OF,RD,RI,AI") )                                                                        return "SS"; 
          if( isOneSufix("ado,ido") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) ) return "VV"; 
          if( isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG") || isTN(1,"AA,OF") || iWrd==0 || isTNNDD("OF") )       return "SS"; 
                                                                                                              return "VV";
        case "P0XH": 
          if( isTLNDD("BE,SS,AA") )       return "AA"; 
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS"; 
                                          return "DD";
        case "P0XI": 
          if( isTLNDD("SS") ) return "AA"; 
                              return "VV";
        case "P0XJ": 
          if( iWrd==0 && isTN(1,"SS") )                                                                                           return "AA"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                                                                                     return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && (isModeLW() || isTN(1,"GZ,OF,PP,TO")) )                                                return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                                                                  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) )  return "SS"; 
                                                                                                                                  return "AA";
        case "P0XK": 
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") )  return "SS"; 
                                                                  return "DD";
        case "P0XM": 
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") )  return "AA"; 
                                                              return "DD";
        case "P0XO": 
          if( isFUpper() ) return "NP"; 
                           return "AA";
        case "P0XQ": 
          if( iWrd==0 && isModeLW() )                                                                                             return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                                                                   return "VR"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                                                                                   return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )                                                                           return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                                                                  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) )  return "SS"; 
          if( isTN(1,"RD,RI,AI") )                                                                                                return "VV"; 
          if( isTN(1,"AA") )                                                                                                      return "DD"; 
                                                                                                                                  return "AA";
        case "P0XS": 
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") )                                                                        return "SS"; 
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") )                                                               return "SS"; 
          if( isTL(1,"SS,XJ,SP") )                                                                                                return "AA"; 
          if( isTL(2,"SS") && isTL(1,"AA,XJ") )                                                                                   return "AA"; 
          if( iWrd==0 && isTN(1,"SS") )                                                                                           return "AA"; 
          if( isTNNDD("OF") && isTL(1,"SS,AA") )                                                                                return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && (isModeLW() || isTN(1,"GZ,OF,PP,TO")) )                                                return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                                                                  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) )  return "SS"; 
                                                                                                                                  return "AA";
        case "P0XW": 
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") )                                                                        return "SS"; 
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") )                                                               return "SS"; 
          if( isTL(1,"SS,XJ,SP") )                                                                                                return "AA"; 
          if( isTL(2,"SS") && isTL(1,"AA,XJ") )                                                                                   return "AA"; 
          if( iWrd==0 && isTN(1,"SS") )                                                                                           return "AA"; 
          if( isTNNDD("OF") && isTL(1,"SS,AA") )                                                                                  return "AA"; 
          if( isTLNDD("PP,TO,RD,RI,AI") && ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) )                                               return "SS"; 
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                                                                  return "AA"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) )  return "SS"; 
                                                                                                                                  return "AA";
        case "P0XY": 
          if( isTN(1,"DD,AA") )                              return "DD"; 
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") ) return "AI"; 
                                                             return "DD";
        case "P0YA": 
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VU"; 
                                                                        return "AA";
        case "P0YB": 
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                         return "AA"; 
                                                                                            return "VI";
        case "P0YC": 
          if( isTLNDD("VA,VU,VY,VJ,VX,VZ,VW") ) return "VV"; 
                                                return "AA";
        case "P0YD": 
          if( isTLNDD("SS") ) return "AA"; 
                     Mode(1); return "VV";
        case "P0YE": 
          if( isTLNDD("SS") ) return "AA"; 
                     Mode(4); return "VV";
        case "P0YF": 
          if( iWrd==0 && isTNNDD("OL,CC") )                                                                return "VT"; 
          if( isModeLW() )                                                                                 return "SS"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "VT";
        case "P0YG": 
          if( iWrd==0 && isModeLW() )                                                                      return "SS"; 
          if( (isTLNDD("RD,RI,AI,AD,OF,VT,VI,VV,TX,VG,PP") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "VR";
        case "P0YP": 
          if( iWrd==0 && isTNNDD("OL,CC") )                                                               return "VK"; 
          if( isModeLW() )                                                                                 return "SS"; 
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                          return "VK";
        case "P0YQ": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "VL";
        case "P0YR": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "VN";
        case "P0YS": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BZ";
        case "P0YT": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BL";
        case "P0YV": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BY";
        case "P0YW": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BO";
        case "P0YX": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BX";
        case "P0YZ": 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
                                                                                                           return "BR";
        case "P0ZA": 
          if( iWrd==0 && isModeLW() )                                                                                   return "SS"; 
          if( isTLNDD("OF") && isTN(1,"AA,XJ") && isTN(2,"OF") )                                                        return "SS"; 
          if( iWrd==0 && isWN(1,"la,el") )                                                                              return "VV"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,VG,VV,TX,VT,VI") && (!isTN(1,"SS") || isModeLW())) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                                                         return "VR"; 
                                                                                                                        return "VV";
        case "P0ZB": 
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") )    return "AA"; 
          if( isTLNDD("RD,RI,AI") )                     return "SS"; 
          if( iWrd==0 && isModeLW() )                   return "SS"; 
          if( iWrd==0 && isWN(1,"de") )                 return "SS"; 
          if( isOneSufix("ado,ido") && !isTLNDD("HA") ) return "AA"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )     return "AA"; 
          if( isOneSufix("ado,ido") )                   return "VP";
                                                        return "VV";
        case "P0ZE": 
          if( iWrd==0 && isModeLW() )                                                    return "SS"; 
          if( (isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                      return "AA"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                          return "VR"; 
                                                                                         return "VV";
        case "P0ZF": 
          if( isTLNDD("OF") )                                                             return "SS"; 
          if( iWrd==0 && isModeLW() )                                                     return "SS"; 
          if( (isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
                                                                                          return "VV";
        case "P0ZG": 
          if( isWNNDD("a,para,el,la,lo") )                                                                    return "VI"; 
          if( isOneSufix("ado,ido") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) ) return "VI"; 
          if( isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG") || isTN(1,"AA,OF") || iWrd==0 || isTNNDD("OF") )       return "SS"; 
                                                                                                              return "VI";
        case "P0ZH": 
          if( isWLNDD("me,le,les,te,nos,las,se") ) return "VR"; 
                                                   return "VV";
        case "P0ZI": 
          if( isOneSufix("ado,ido") && !isTLNDD("HA") ) return "AA"; 
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )     return "AA"; 
          if( isOneSufix("ado,ido") )                   return "VP"; 
          if( isWLNDD("me,le,les,te,nos,las") )         return "VR"; 
                                                        return "VV";
        case "P0ZJ": 
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO,AA") )                                                                    return "SS";
          if( isSufix("ios") )                                                                                              return "SS"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") )  return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                                                             return "VR"; 
                                                                                                                            return "VV";
        case "P0ZK": 
          if( (isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                           return "VR";
                                                                                          return "VI";
        case "P0ZV": 
          if( isWLNDD("me,le,les,te,nos,las") ) return "VR"; 
                                                return "VV";
        case "P0ZZ": 
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO") )                                                                       return "SS"; 
          if( isSufix("ios") )                                                                                              return "SS"; 
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") )  return "SS"; 
          if( isWLNDD("me,le,les,te,nos,las") )                                                                             return "VR"; 
                                                                                                                            return "VV";
        case "P1BD": 
          if( isLeft("che") ) {Mode(4); return "BS";} 
                               Mode(1); return "BI"; 
        case "P1BE": 
          if( isMode(TMod.Infinitivo) && iWrd==0 )                            {Mode(5); return "VG";} 
          if( isTLNDD("CC,OL,GZ") || iWrd==0 || (iWrd==1 && isTL(1,"DD,DN"))) {InsW(9); return "BE";}  
                                                                                        return "BE";
        case "P1BF": 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "BM";}
                                                        {Mode(1); return "BI";}
        case "P1BH": 
          if( isLeft("che"))                            {Mode(4); return "BS";}
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "BM";}  
                                                        {Mode(1); return "BI";}
        case "P1BJ": 
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "BN";}
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "BM";} 
                                                        {Mode(1); return "BI";}
        case "P1BK": 
          if( isLNDDFW() && isTNNDD("OC,OP,OO") )       {Mode(2); return "BM";}
                                                        {Mode(4); return "BS";}
        case "P1BL": 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VN";} 
                                                        {Mode(1); return "VL";}
        case "P1BO": 
          if( isLeft("che"))                            {Mode(4); return "VK";}
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VN";} 
                                                        {Mode(1); return "VD";}
        case "P1BR": 
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";} 
                                                        {Mode(1); return "VL";}
        case "P1BX": 
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";} 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VN";} 
                                                        {Mode(1); return "VL";}
        case "P1BY": 
          if( isLeft("che"))                            {Mode(4); return "VK";} 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VN";} 
                                                        {Mode(1); return "VL";}
        case "P1BZ": 
          if( isLNDDFW() && isTNNDD("OC,OP,OO"))        {Mode(2); return "VN";}
          if( isTLNDD("OC"))                            {Mode(2); return "VN";}
                                                        {Mode(4); return "VK";}
        case "P1DN": 
          if( iWrd==0 && (isOneSufixN(1,"ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse,arlo,irlo,erlo")) ) return "DN"; 
          if( (isTNNDD("VV,VI,VT,VZ") &&  !isTimeNNDD(TTime.Futuro) && isWN(0,"no")) || isTLNDD("GN") )              return "DN";
                                                                                                                      return "DN";
        case "P1HC": 
          if( isLeft("che"))                            {Mode(4); return "HJ";} 
                                                        {Mode(1); return "HD";}
        case "P1HF": 
          if( isLeft("che"))                            {Mode(4); return "HJ";} 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "HM";} 
                                                        {Mode(1); return "HD";}
        case "P1HV": 
          if( isLNDDFW() && isTNNDD("OC,OP,OO"))        {Mode(2); return "HM";} 
                                                        {Mode(4); return "HJ";}
        case "P1VB": 
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VU";} 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VW";} 
                                                        {Mode(1); return "VX";}
        case "P1VC": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") ) {InsW(7); return "VT";} 
                                                                             {InsW(7); return "VI";}  
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )                return "VT"; 
                                                                                       return "VI";
        case "P1VE": 
          if( isLeft("che"))                            {Mode(4); return "VJ";} 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VW";} 
                                                        {Mode(1); return "VX";}
        case "P1VF": 
          if( isLNDDFW() && isTNNDD("OC,OP,OO"))        {Mode(2); return "VW";} 
                                                        {Mode(4); return "VJ";}
        case "P1VH": 
          if( iWrd==0 && isTNNDD("OC,OP,OO"))           {Mode(2); return "VW";} 
                                                        {Mode(1); return "VX";}
        case "P1VO": 
          if( isLeft("che"))                            {Mode(4); return "VK";}
                                                        {Mode(1); return "VL";}
        case "P1VQ": 
          if( isLeft("che"))                            {Mode(4); return "VJ";}
                                                        {Mode(1); return "VX";}
        case "P1VS": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") ) {InsW(7); return "VT";} 
                                                                             {InsW(7); return "VI";}
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )                return "VT";
                                                                                       return "VI";
        case "P1VV": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("que,a,para") )             {InsW(7); return "VT";} 
                                                                                       {InsW(7); return "VI";}
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO,VT,VV,VI") || isWNNDD("que,a,para") ) return "VT"; 
                                                                                                 return "VI";
        case "P1XJ": 
          if( iWrd==0 && isTN(1,"SS") )                                               return "AA"; 
          if( isTNNDD("OF") && isTL(1,"SS") )                                       return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) )   return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                      return "AA";
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) )  return "SS";
                                                                                                                                  return "AA";
        case "P2BE": 
          if( isWL(1,"si,si ya,cuando") || (isWL(1,"no") && iWrd==1) || iWrd==0 ) {InsW(9); return "BE";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                    {InsW(9); return "BE";} 
          if (isTLNDD("CC") )                                                     {InsW(9); return "BE";} 
                                                                                            return "BE";
        case "P2HA": 
          if( isWL(1,"si,si ya,cuando") || isTLNDD("OL,GZ,CC") || (isWL(1,"no") && iWrd==1) || iWrd==0) {InsW(9); return "HA";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                                          {InsW(9); return "HA";} 
          if( isTLNDD("CC") )                                                                           {InsW(9); return "HA";} 
                                                                                                                  return "HA";
        case "P2SS": return "SS";
        case "P2VA": 
          if( isWL(1,"si,si ya,cuando") || (isWL(1,"no") && iWrd==1) || iWrd==0) {InsW(9); return "VA";}
          if( isTL(1,"AA") &&  isTL(2,"BE") )                                    {InsW(9); return "VA";} 
          if( isTLNDD("CC,OL,GZ") )                                              {InsW(9); return "VA";} 
                                                                                           return "VA";
        case "P2VI": 
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isMode(TMod.Participio) ) return "VP";
          if (isWLNDD("permitir,permite") )                                {InsW(7); return "VT";}
          if( isWL(1,"si,si ya,cuando") || isTLNDD("OL,GZ,CC") || iWrd==0) {InsW(9); return "VT";}
          if (isTime(TTime.Futuro) && isTL(1,"DN"))         {InsL(1,10); Mode(0); return "VI";}
          if( isTime(TTime.Futuro)  && !isTLNDD("CC") )
            {
            if( isTLNDD("BE") )                                {InsLNDD(10); Mode(6); return "VI";} 
                                                               {InsW(10);    Mode(0); return "VI";}
            }
          if( (iWrd==0 || isTL(1,"PP,OF")) && isMode(TMod.Infinitivo) && ( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse")) ) 
                                          {Mode(5); return "VI";} 
          if( iWrd==0 && isMode(TMod.Infinitivo) )  return "VI"; 
                                                    return "VI";
        case "P2VR": 
          if( isMode(TMod.Gerundio)   )             return "VG"; 
          if( isMode(TMod.Participio) )             return "VP";
          if( isWLNDD("te,me,le,lo")  ) {DelLNDD(); return "VR";} 
          if( iWrd==2 && isMode(TMod.Infinitivo) && (isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse"))) {Mode(5); return "VR";} 
                                                    return "VR";
        case "P2VT": 
          if( isMode(TMod.Gerundio)   )                                              return "VG"; 
          if( isMode(TMod.Participio) )                                              return "VP"; 
          if( isWLNDD("permitir"))                                         {InsW(7); return "VT";} 
          if( isWL(1,"si,si ya,cuando") || isTLNDD("OL,GZ,CC") || iWrd==0) {InsW(9); return "VT";} 
          if( isTime(TTime.Futuro) && isTL(1,"DN"))          { InsL(1,10); Mode(0); return "VT";} 
          if( isTime(TTime.Futuro) && !isTLNDD("CC"))
            {
            if( isTLNDD("BE") )                               {InsLNDD(10); Mode(6); return "VT";}
                                                                 {Mode(0); InsW(10); return "VT";}
            }
          if( (iWrd==0 || isTL(1,"PP,OF")) && isMode(TMod.Infinitivo) && ( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse"))) 
                                                                           {Mode(5); return "VT";} 
                                                                                     return "VT";
        case "P2VV": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO") || isWNNDD("che") )            return "VT"; 
                                                                                            return "VI";
        case "P3BE": 
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo))                     {InsW(0); Mode(2); return "BE";} 
          if( (isTime(TTime.Futuro) || (isTimeN(1,TTime.Futuro) && isModeN(1,TMod.Participio))) && isTL(1,"DN")) { InsL(1,10); Mode(0); return "BE";} 
          if( isTime(TTime.Futuro) && !isTLNDD("CC"))                                                                {InsW(10); Mode(0); return "BE";} 
                                                                                                                                          return "BE";
        case "P3BL": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT"; 
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3BO": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo))
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3BR": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3BX": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo))
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT"; 
                                                                                            return "VI";
        case "P3BY": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per"))  return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3BZ": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,a,por") )    return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )            return "BZ";
                                                                                            return "VI";
        case "P3VA": 
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VA";}
          if( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse") )                      {InsW(0); return "VA";} 
                                                                                                                      return "VA";
        case "P3VI": 
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VI";}
          if( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse,arlo,irlo,erlo") )          {InsW(0); return "VI";}
                                                                                                                      return "VI";
        case "P3VK": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3VL": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3VM": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )             return "VT";
                                                                                            return "VI";
        case "P3VN": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )             return "VT";
                                                                                            return "VI";
        case "P3VO": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "P3VT": 
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) ) {InsW(0); Mode(2); return "VT";}
          if( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse,arlo,irlo,erlo") )        {InsW(0); return "VT";} 
                                                                                                                       return "VT";
        case "P3VV": 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                            return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO") || isWNNDD("che") )            return "VT";
                                                                                            return "VI";
        case "VI":
          if( isMode(TMod.Gerundio) )                   {Mode(5); return "VG";}
          if( isOneSufix("ado,ido") || isMode(TMod.Participio) )  return "VP";
                                                                  return "VV";
        case "VR":
          if( isMode(TMod.Gerundio) )                   {Mode(5); return "VG";}
          if( isOneSufix("ado,ido") || isMode(TMod.Participio) )  return "VP";
                                                                  return "VV";
        case "VT":
          if( isMode(TMod.SubjuntivoImperativo) )                 return "BZ";
          if( isMode(TMod.Gerundio) )                   {Mode(5); return "VG";}
          if( isOneSufix("ado,ido") || isMode(TMod.Participio) )  return "VP";
                                                                  return "VV";
        default: 
          return null;    
        }

      return Tipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia el tipo de la palabra para el idioma Inglés</summary>
    private string ChageTypeEn( int iWrd, string Tipo, string Pref )
      {
      switch( Pref + Tipo )
        {
        case "CAE":
        case "CAF":
        case "CAN": return "AI";
        case "CDF": return "DD";
        case "CGI":
        case "CGT": return "VG";
        case "CHI":
        case "CHT": return "VV";
        case "CJI":
        case "CJT": return "VD";
        case "CPI":
        case "CPT": return "VP";
        case "CVI":
        case "CVT": return "VV";
        case "TGI": return "VI";
        case "TGT": return "VT";
        case "THI": return "VI";
        case "THT": return "VT";
        case "TJI": return "VI";
        case "TJT": return "VT";
        case "TPI": return "VI";
        case "TPT": return "VT";
        case "SH"  : 
        case "TSH" : 
        case "CSH" : 
        case "MSH" : return "VA";
        case "P0HE": 
        case "P0HS": return "AP";
        case "P0MO": 
        case "P0QL": return "DD";
        case "P0RP": 
        case "P0TP": return "VC";
        case "CSP" : return "SS";
        case "P0RS": return "VV";
        case "MBN" : Mode(6); break;
        case "MHI" : Mode(1); break;
        case "MHT" : Mode(1); break;
        case "MJI" : Mode(1); break;
        case "MJT" : Mode(1); break;
        case "MPI" : Mode(6); break;
        case "MPT" : Mode(6); break;
        case "NQA" : return isPlural()? "UL" : "QA";
        case "NQE" : return isPlural()? "VS" : "QE";
        case "NQF" : return isPlural()? "UL" : "QF";
        case "NQG" : return isPlural()? "VS" : "QG";
        case "NQN" : return isPlural()? "UL" : "QN";
        case "NQO" : return isPlural()? "SS" : "QO";
        case "NSS" : return isPlural()? "SP" : "SS";
        case "NSV" : return isPlural()? "UL" : "SV";
        case "NTA" : return isPlural()? "UF" : "TA";
        case "NUE" : return isPlural()? "SS" : "UE";
        case "NUH" : return isPlural()? "SS" : "UH";
        case "NUI" : return isPlural()? "SS" : "UI";
        case "NUJ" : return isPlural()? "SS" : "UJ";
        case "NUM" : return isPlural()? "SS" : "UM";
        case "NUO" : return isPlural()? "UG" : "UO";
        case "NUR" : return isPlural()? "UL" : "UR";
        case "NUT" : return isPlural()? "SS" : "UT";
        case "NUX" : return isPlural()? "SS" : "UX";
        case "NXA" : return isPlural()? "UF" : "XA";
        case "NXB" : return isPlural()? "UL" : "XB";
        case "NXC" : return isPlural()? "HI" : "XC";
        case "NXD" : return isPlural()? "HT" : "XD";
        case "NXE" : return isPlural()? "HI" : "XE";
        case "NXF" : return isPlural()? "HT" : "XF";
        case "NXG" : return isPlural()? "UG" : "XG";
        case "NXH" : return isPlural()? "SS" : "XH";
        case "NXI" : return isPlural()? "VS" : "XI";
        case "NXJ" : return isPlural()? "SS" : "XJ";
        case "NXK" : return isPlural()? "SS" : "XK";
        case "NXP" : return isPlural()? "VS" : "XP";
        case "NXQ" : return isPlural()? "UF" : "XQ";
        case "NYF" : return isPlural()? "UF" : "YF";
        case "NYG" : return isPlural()? "SS" : "YG";
        case "NZB" : return isPlural()? "UF" : "ZB";
        case "NZF" : return isPlural()? "UL" : "ZF";
        case "NZG" : return isPlural()? "VS" : "ZG";
        case "NZH" : return isPlural()? "HT" : "ZH";
        case "NZI" : return isPlural()? "HT" : "ZI";
        case "NZJ" : return isPlural()? "UF" : "ZJ";
        case "NZV" : return isPlural()? "VS" : "ZV";
        case "NVA" : return isMode(TMod.Gerundio)? "VY" : "VA";
        case "MVP" : Mode(6); return Tipo;
        case "P2VG":      
          if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,NO,FS,HS,ME") || 
              isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed") ) && !isLastW() )
                    return "GT";
                    return "GI";
        case "MBE":
          if( isWLNDD("can,must,may,should") || isTLNDD("VA") )      Mode(0);
          else if( isWrd("was,were") )                             { Mode(1); Time(1); }
          else if( isTLNDD("TO") )                                   Mode(0);
          else if( iWrd==0 && Words[Words.Count-1].Origlwr == "?" )  Mode(1);
          else if( iWrd==0 )                                         Mode(2);
          else                                                       Mode(1);
          break;

        case "P0QK":return "VA";

        case "MHA":
          if( isWrd("has") )                                         Mode(1);
          else if( iWrd==0 )                                         Mode(2);
          else if( isWLNDD("can,must,may,should") || isTLNDD("VA") ) Mode(0);
          else if( isWrd("had") && isTLNDD("HA") )                   Mode(6);
          else if( isTLNDD("TO") )                                   Mode(0);
          else                                                       Mode(1);
          break;
        case "MVA":
          if( iWrd==0 && isLastW("?") )       Mode(1);
          else if( iWrd==0 )                  Mode(2);
          else if( isTLNDD("BE,VA") )         Mode(0);
          else if( isWLNDD("CC") )            Mode(1);
          else if( Wrd.Origlwr=="could" )     Mode(3);
          else if( isTLNDD("TO,OC") )         Mode(0);
          else                                Mode(1);
          break;
        case "MVI":
          if( isTL(1,"CC") && isTL(2,"VT,VI") && isTL(3,"TO,VA") )                          Mode(0);
          else if( isTL(1,"CC") && isTL(2,"VT,VI,VV") && isTL(3,"DD") && isTL(4,"TO,VA") )  Mode(0);
          else if( iWrd==0)                                                                 Mode(2);
          else if( isTLNDD("BE,PB,PP") )                                                    Mode(0);
          else if( isWLNDD("can,must,may,should") || isTLNDD("VA") )                        Mode(0);
          else if( isTLNDD("TO,OC,PP") )                                                    Mode(0);
          else if( isPlural() )                                                             Mode(1);
          else if( isPerson(1) || isPerson(2) )                                             Mode(1);
          else                                                                              Mode(2);
          break;
        case "MBG" :
          if( iWrd==0 || isTLNDD("PP,TO,OF") || (isTL(1,"GZ") && isTL(2,"DD") && iWrd==3))  Mode(0);
                                                                                            Mode(5);
          break;
        case "MGI" :
          if( iWrd==0 || isTLNDD("PP,TO,OF,VA,PB") )                                             Mode(0);
          else if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") || iWrd==0 )
                                                                                                 Mode(0);
          else if( iWrd==0 || isTLNDD("PP,TO,OF") || (isTL(1,"GZ") && isTL(2,"DD") && iWrd==3) ) Mode(0);
          else                                                                                   Mode(5);
          break;
        case "MGT" :
          if( iWrd==0 || isTLNDD("PP,TO,OF,VA,PB") )        Mode(0);
          else if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") || iWrd==0 || (isTL(1,"CC") && isTL(2,"GT,GI,VG") && iWrd==3) )
                                                            Mode(0);
          else if( iWrd==0 || isTLNDD("PP,TO,OF") || (isTL(1,"GZ") && isTL(2,"DD") && iWrd==3) )
                                                            Mode(0);
                                                       else Mode(5);
          break;
        case "MVT" :
          if( isTL(1,"CC") && isTL(2,"VT,VI") && isTL(3,"TO,VA") )                                       Mode(0);
          else if( isTL(1,"CC") && isTL(2,"VT,VI,VV") && isTL(3,"DD") && isTL(4,"TO,VA") )               Mode(0);
          else if( iWrd==0 && !isPlural() )                                                              Mode(2);
          else if( isTLNDD("BE,PB,PP") && !isPlural() )                                                  Mode(0);
          else if( (isWLNDD("can,must,may,should") || isTLNDD("VA")) && !isPlural() )                    Mode(0);
          else if( isTLNDD("TO,OC,PP") && !isPlural() )                                                  Mode(0);
          else if( isPlural() )                                                                          Mode(1);
          else if( isPerson(1) || isPerson(2) || isWLNDD("will,do,does,did") || isTLNDD("OD,OP,SS,FS") ) Mode(1);
                                                                                                         Mode(2);
          break;
        case "MVV" :
          if( iWrd==0 )                                               Mode(2);
          else if( isTLNDD("BE,PB") )                                 Mode(0);
          else if( isWLNDD("can,must,may,should") || isTLNDD("VA") )  Mode(0);
          else if( isTLNDD("TO") )                                    Mode(0);
                                                                      Mode(1);
          break;
        case "NVI" :
          if( isPlural() )  return "HI"; 
                            return "VI";
        case "NVT" :
          if( isPlural()) {Mode(1); return "HT";} 
                                    return "VT";
        case "NVV" :
          if( isPlural() )  return "VS"; 
                            return "VV";
        case "P0AY":
          if( isTL(1,"RD,RI,DD,OF,PP") || isTN(1,"AA;SS;SP") )    return "AI"; 
                                                                  return "DD";
        case "P0BG":
          if( isTLNDD("BE") )                                     return "VG";
          if( isTL(1,"OF,PP,TO") || (iWrd==0 && !isTipo("SS")) )  return "VG";
                                                                  return "BG";
        case "P0DO":
          if( (isTL(1,"PP,TO") || iWrd==0) && isSufix("ing"))   {Mode(0); return "VG";}
          if( isTLNDD("BE") && isSufix("ing") )                           return "VG";
          if( iWrd==0 && !isLastW() )                            {Del(0); return "DO";}
          if( isTN(1,"DN") )                                     {Del(0); return "DO";}
          if( isTN(2,"DN"))                                      {Del(0); return "DO";} 
          if( isTLNDD("OP") && isTNNDD("VV,VT,VI") )                      return "DO"; 
                                                                          return "VV";
        case "P0HA":
          if( isTNNDD("VP,VC,VD,BN,PT,PI,HA,MG,UH,UB,UW,QB,QC,QD,UZ,UC,YB,JK")) {Mode(1); return "HA";} 
          if( iWrd==0 && isTN(1,"OP,YU") && isTN(2,"VC,VP,PI,PT,YB,JK") )                 return "HA"; 
                                                                                          return "VV";
        case "P0LK":
          if( isTL(1,"SS") && isModeLW()   )                                                 return "VV";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                 return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0); return "VV";}
          if( isTL(1,"SS") && isModeLW() )                                                   return "SS"; 
          if( isTL(1,"NP") && isTN(1,"VA") )                                                 return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                    return "SS";
          if( (iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to") ) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS")))) 
                                                                                             return "SS"; 
          if( iWrd==0 &&  isWN(1,"it") )                                           {Mode(2); return "VV";} 
                                                                                             return "VV";
        case "P0ME":
          if( iWrd==0 || isTN(1,"BE,HA,VV,VS,VA,QJ,YF"))  {Del(0); return "OW";} 
          if( isTLNDD("PP,TO,OF") )                                return "OW";
                                                                   return "OC";
        case "P0MG":
          if( isTNNDD("BE,HA,VV,VT,VI,VS,VA,SH") ) return "OG";
                                                   return "AF";
        case "P0MN":
          if( isTLNDD("AA,RD,RI,VP,OF") || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE") )                              return "OS";
          if( isTNNDD("BE,HA,VV,VT,VI,VS,VA") )            return "OS";
                                                           return "SS";
        case "P0MU":
          if( isTNNDD("BE,HA,VV,VT,VI,VS,VA") ) return "OG"; 
                                                return "AF";
        case "P0MV":
          if( isTN(1,"SS,AA") ) return "AN";
                                return "NN";
        case "P0NO":
          if( isTL(1,"BE") || isWL(1,"were") || (iWrd==0 &&  isWN(1,",") ) ) return "DN"; 
                                                                             return "AI";
        case "P0NR":
          if( isWLNDD("sit,sitting,sleeping,sleep") ) return "PP";
          if( isTN(1,"RD,AF,RI,AI") )                 return "PP";
          if( isTN(1,"AA,DD,VV") )                    return "DD";
          if( isTL(1,"OP,VA") )                       return "VV";
                                                      return "AA";
        case "P0OA":
          if( isTNNDD("BE,HA,VV,VT,VI,VS,VA") ) return "OD";
                                                return "AE";
        case "P0QA":
          if( isTL(1,"OC") && isTN(1,"HA,BE") )                                               return "DD";
          if( isTL(1,"BE") )                                                                  return "DD";
          if( (isTLNDD("RD,RI,VV,VT,VI,AI") || isTLNDD("AA,AE,AD")) && !isTNNDD("SS,NP,SC") ) return "SS";
          if( isTL(1,"GW,GV") )                                                               return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )                                         return "DD";
                                                                                              return "VV";
        case "P0QB":
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")))  return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )          return "AA";
          if( isTN(1,"BE") )                                                                      return "SS";
          if( isTL(1,"BE") )                                                                      return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( (isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AE,AD")) && !isTNNDD("SS,NP,SC") )          return "SS";
          if( isTL(1,"GW,GV") )                                                                   return "SS";
                                                                                                  return "VD";
        case "P0QC":
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )          return "AA";
          if( isTN(1,"BE") )                                                                      return "SS";
          if( isTL(1,"BE") )                                                                      return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( (isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AE,AD")) && !isTNNDD("SS,NP,SC") )          return "SS";
          if( isTL(1,"GW,GV") )                                                                   return "SS";
                                                                                                  return "VD";
        case "P0QD":
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )          return "AA";
          if( isTN(1,"BE") )                                                                      return "SS";
          if( isTL(1,"BE") )                                                                      return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS"; 
          if( (isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AE,AD")) && !isTNNDD("SS,NP,SC") )          return "SS";
          if( isTL(1,"GW,GV") )                                                                   return "SS"; 
                                                                                                  return "VD";
        case "P0QE":
          if( isTN(1,"XG,SS,XJ") && isTN(2,"OF,PP") )                     return "AA";
          if( (isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC"))              return "DD";
          if( isTLNDD("BE") )                                             return "DD";
          if( isTN(1,"SS,NP,SC") )                                        return "AA";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) )  return "DD";
          if (isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI") )                   return "AA"; 
                                                                          return "VV";
        case "P0QF":
          if( isTNNDD("OR") )                                                                 return "VR";
          if( isTL(1,"SS") && isModeLW() )                                                    return "VV";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VV";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if( (iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI,AF,AP") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it") )                                             {Mode(2); return "VT";} 
                                                                                              return "VV";
        case "P0QG":
          if( (isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC"))            return "DD";
          if( isTLNDD("BE"))                                            return "DD";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI"))                  return "DD";
                                                                        return "VV";
        case "P0QH":
          if( (isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC"))            return "DD";
          if( isTLNDD("BE"))                                            return "DD";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI"))                  return "DD";
                                                                        return "HT";
        case "P0QI": return "PP";
        case "P0QJ":
          if( isFUpper() && Words[Words.Count-1].Orig!="?" && iWrd==0 ) return "SS";
                                                                        return "VA";
        case "P0QM":
          if( isTLNDD("AI,RD,RI,AA") ) return "XJ";
                                       return "PP";
        case "P0QO":
          if( isTL(1,"PP") && isTN(1,"SS") )                                                                return "AI";
          if( isTN(2,"GG") )                                                                                return "SS";
          if( isTLNDD("BE") )                                                                               return "AI";
          if( isTL(1,"PP") && isSufixN(1,"ing") )                                                           return "DD";
          if( isTN(1,"SS,XG,XJ;SP") && isTLNDD("RD,RI") && !isTN(2,"SS") )                                  return "AI";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                   return "SS";
          if( isTL(1,"VV,VT,VI") && isTNNDD("RD,RI,AA,AI,PP") )                                             return "DD";
          if( isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") && isModeLW() ) return "DD";
          if( isTN(1,"SS") || (isWLNDD("more") && isWN(1,"than") ) || ( isTN(1,"XG") && isTN(1,"LW") && isTL(1,"RD") ) || ( isTN(1,"CC") && isTN(2,"XJ") && isTN(3,"SS"))) 
                                                                                                            return "AI";
          if( isTN(1,"AA"))                                                                                 return "DD";
                                                                                                            return "SS";
        case "P0QZ":
          if( isTL(1,"OP,SS") && isTN(1,"VV,VT,VI") )                                                       return "DD";
          if( isTL(1,"PP") && isTN(1,"SS") )                                                                return "AA";
          if( isTN(2,"GG") )                                                                                return "SS";
          if( isTLNDD("BE") )                                                                               return "AA";
          if( isTL(1,"PP") && isSufixN(1,"ing") )                                                           return "DD";
          if( isTN(1,"SS,XG,XJ,XH") && isTLNDD("RD,RI") && !isTN(2,"SS"))                                   return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))))                                    return "SS";
          if( isTL(1,"VV,VT,VI") && isTNNDD("RD,RI,AA,AI,PP"))                                              return "DD";
          if( isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") && isModeLW() ) return "DD";
          if( isTN(1,"SS") || (isWLNDD("more") && isWN(1,"than") ) || ( isTN(1,"XG") && isTN(1,"LW") && isTL(1,"RD") ) || ( isTN(1,"CC") && isTN(2,"XJ") && isTN(3,"SS"))) 
                                                                                                            return "AA";
          if( isTN(1,"AA"))                                                                                 return "AA";
                                                                                                            return "SS";
        case "P0SP": Plural(); return "SS";
        case "P0SV":
          if( isTL(1,"SS") && isModeLW() )                                                    return "VV";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VV";} 
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if( (iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS")))) 
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "VV";} 
                                                                                              return "VV";
        case "P0TA":
          if( isTNNDD("OR") )                                                                 return "VR";
          if( isTL(1,"SS") && isModeLW() )                                                    return "VT";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VT";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if( (iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))))
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                               {Mode(2); return "VT";} 
                                                                                              return "VT";
        case "P0TH":
          if( isTL(1,"FS,SP,SS") && isTN(1,"VT,VI,HA,HT,HI,VA,VV,BE") ) return "OL";
          if( isWLNDD("appear,appeared,appears,appearing") )            return "OL";
          if( isTL(1,"OC") && isTL(2,"VT,VI,VV") )                      return "OL";
          if( isTL(1,"SS") && isTN(1,"SS") )                            return "OL";
          if( isTLNDD("VV,VT,VI") || (isTL(2,"DN") && isTL(3,"BE")) || isTN(1,"XJ") || isWN(1,"the,a,an,this,that,these,those,it,she,he,we,i,they,you,all") || isWL(2,"the,a,an,that,this,these,those"))
                                                                        return "OL";
          if( isTN(1,"SS") )                                            return "AE";
          if( isTLNDD("BE") )                                           return "OE";
          if( isTNNDD("BE") )                                           return "OO";
          if( isTL(1,"SS") || (isTL(1,"XG") && isTL(2,"RD,RI,AA,AI")) || isTN(1,"OP,OO,DD,VA") || isWN(1,"i,you,he,she,we,it,they") || (isTN(1,"XG") && isTN(2,"RD,RI,AA,AI")) || isWN(1,"there,may,let") || isWN(1,"it") || isTL(1,"XJ") || (isTN(1,"HA") && iWrd>0) || (isTN(1,"SS,RD,RI") && isTLNDD("SS") ||  (isTN(1,"AA") && isTL(1,"AA")) || isTN(1,"RD,RI")) ) 
                                                                        return "OL";
                                                                        return "AE";
        case "P0TW":
          if( isTN(1,"BE") || (isTN(1,"WI,VA,QJ,QK") && isTN(2,"BE")) ) return "FS";
                                                                        return "DD";
        case "P0TY":
          if( isLeft("more") || isLeft("less") ) return "PP";
                                                 return "CC";
        case "P0UA":
          if( isFUpper() ) return "NP";
                           return "SS";
        case "P0UB":
          if( isTLNDD("PP") && isTNNDD("SS") )                                                                    return "AA";
          if( isTLNDD("RD,AI,RI,AE,OF,PT,PI,VP,AL") )                                                             return "AA";
          if( isTLNDD("HA") )                                                                                     return "VP";
          if( isTLNDD("BE") )                                                                                     return "AA";
          if( isTNNDD("RD,RI,AI,AL") )                                                                            return "VD";
          if( isWL(1,"there,here") )                                                                              return "VD";
          if( isTL(1,"VP") )                                                                                      return "VP";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
          if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
          if( isTL(1,"OS") )                                                                                      return "VP";
          if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
          if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
          if( isWN(1,"with") )                                                                                    return "VP";
          if( isTL(1,"CC") && isLeftT("VD") )                                                                     return "VD";
          if( isTL(1,"CC") && isLeftT("VP") )                                                                     return "VP";
          if( isTL(1,"CC") && isTL(2,"AA,AI,VP,AL") )                                                             return "VP";
          if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
          if( (isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI") )                                 return "VD";
          if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
          if( iWrd==0 && isModeLW()  )                                                                            return "VP";
          if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there"))                   return "VP";
          if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP"; 
                                                                                                                  return "VD";
        case "P0UC":
          if( isTL(1,"SS") && isModeLW() )                                                    return "VD";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VD";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "VD";} 
                                                                                              return "VD";
        case "P0UD":
          if( isTL(1,"PP,TO") || iWrd==0) {Mode(0);                return "GT";}
          if( isTLNDD("BE") )                                      return "GT";
          if( isTN(1,"SS,AA") && isTLNDD("RI,RD,BE,AI,PP,AP,AE") ) return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AP,AE") ) return "AA";
                                                                   return "GT";
        case "P0UE":
          if( isSufix("ings") )                                                               return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if((isTL(1,"PP,TO") || iWrd==0)) {Mode(0);                                          return "GT";}
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "GT";}
          if( isTLNDD("BE") )                                                                 return "GT";
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                              return "SS";
                                                                                              return "GT";
        case "P0UF":
          if( isTL(1,"SS") && isModeLW() )                                                    return "HT";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an")))  {Mode(0); return "HT";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) )
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "HT";} 
                                                                                              return "HT";
        case "P0UG":
          if( isTL(1,"SS") && isModeLW() )                                                    return "HI";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an")))  {Mode(0); return "HI";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "HI";}
                                                                                              return "HI";
        case "P0UH":
          if( isTLNDD("RD,AI,RI,AE,OF,PT,PI,VP,AL") )                                                             return "AA";
          if( isTLNDD("HA") )                                                                                     return "VP";
          if( isTLNDD("BE") )                                                                                     return "AA";
          if( isTNNDD("RD,RI,AI,AL") )                                                                            return "VD";
          if( isWL(1,"there,here") )                                                                              return "VD";
          if( isTL(1,"VP") )                                                                                      return "VP";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
          if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
          if( isTL(1,"OS") )                                                                                      return "VP";
          if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
          if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
          if( isWN(1,"with") )                                                                                    return "VP";
          if( isTL(1,"CC") && isLeftT("VD") )                                                                     return "VD";
          if( isTL(1,"CC") && isLeftT("VP") )                                                                     return "VP";
          if( isTL(1,"CC") && isTL(2,"AA,AI,VP,AL") )                                                             return "VP";
          if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
          if( (isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI") )                                 return "VD";
          if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
          if( iWrd==0 && isModeLW()  )                                                                            return "VP";
          if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there"))                   return "VP";
          if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                  return "VD";
        case "P0UI":
          if( isSufix("ings") )                                                                                   return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                                      return "SS";
          if((isTL(1,"PP,TO") || iWrd==0))                                                              {Mode(0); return "VG";}
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an")))                      {Mode(0); return "VG";}
          if( isTLNDD("BE") )                                                                                     return "VG";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                         return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                                                  return "SS";
                                                                                                                  return "VG";
        case "P0UJ":
          if( isSufix("ings") )                                                              return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                 return "SS";
          if((isTL(1,"PP,TO") || iWrd==0))                                         {Mode(0); return "GI";}
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0); return "GI";}
          if( isTLNDD("BE") )                                                                return "GI";
          if( isTL(1,"SS") && isModeLW() )                                                   return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                 return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                    return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) )
                                                                                            return "SS";
                                                                                            return "GI";
        case "P0UK":
          if( isTL(1,"PP,TO") || iWrd==0)                {Mode(0); return "VG";}
          if( isTLNDD("BE") )                                      return "VG";
          if( isTN(1,"SS,AA") && isTLNDD("RI,RD,BE,AI,PP,AP,AE") ) return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AP,AE") ) return "AA";
                                                                   return "VG";
        case "P0UL":
          if( isTL(1,"SS") && isModeLW() )                                                   return "VS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                 return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0); return "VS";}
          if( isTL(1,"SS") && isModeLW() )                                                   return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                 return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                    return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                             return "SS";
          if( iWrd==0 && isWN(1,"it"))                                             {Mode(2); return "VS";} 
                                                                                             return "VS";
        case "P0UM":
          if( isSufix("ings") )                                                                   return "SS";
          if((isTL(1,"PP,TO") || iWrd==0))                                              {Mode(0); return "VG";}
          if( isTLNDD("BE") )                                                                     return "VG";
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if((isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()))               return "SS";
          if((!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")))                  return "SS";
          if((isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )           return "AA";
                                                                                                  return "VG";
        case "P0UN":
          if( isTL(1,"PP,TO") || iWrd==0)                {Mode(0); return "GI";}
          if( isTLNDD("BE") )                                      return "GI";
          if( isTN(1,"SS,AA") && isTLNDD("RI,RD,BE,AI,PP,AP,AE") ) return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AP,AE") ) return "AA";
                                                                   return "GI";
        case "P0UO":
          if( (iWrd==0 && isWN(1,",")) || (iWrd==0 && isModeLW()) )                                                      return "SS";
          if( isTN(1,"HA,BE,VT,VV,VI,VA") )                                                                              return "SS";
          if( isTL(1,"PP") && isModeLW() || isWN(1,"?") )                                                                return "SS";
          if( isTL(1,"AI,AA") && isTN(1,"SS") )                                                                          return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                                return "SS";
          if( isTLNDD("BE") )                                                                                            return "AA";
          if(( isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AI,AL")) && (!isTNNDD("SS,NP,SC") || isWN(1,"as") || isModeLW()) ) return "SS";
          if( isTL(1,"GW,GV") )                                                                                          return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )                                                                    return "AA";
          return "VI";
        case "P0UR":
          if(  isTLNDD("RD,RI,VV,VT,VI,AI") && isTN(1,"SS") )                                 return "AI";
          if( isTL(1,"OC") && isTN(1,"HA,BE") )                                               return "DD";
          if( isTL(1,"BE") )                                                                  return "AI";
          if(( isTLNDD("RD,RI,VV,VT,VI,AI") || isTLNDD("AA,AE,AD")) && !isTNNDD("SS,NP,SC") ) return "SS";
          if( isTL(1,"GW,GV") )                                                               return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD"))
            {
            if( isTN(1,"SS,NP,SC") )                                                          return "AI";
                                                                                              return "DD"; 
            }
                                                                                              return "VV";
        case "P0UT":
          if( isSufix("ings") )                                                                   return "SS";
          if((isTL(1,"PP,TO") || iWrd==0))                                              {Mode(0); return "GT";}
          if( isTLNDD("BE") )                                                                     return "GT";
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )          return "AA";
                                                                                                  return "GT";
        case "P0UV":
          if( isFUpper() && !isTLNDD("RD,RI,AP,AI,AN") )                                                               return "NP";
          if( isTN(1,"BE,HA,VV,VT,VI,VA") )                                                                            return "SS";
          if( isTL(1,"AL,AI,AA,RD,RI") && isTN(2,"OF") )                                                               return "AA";
          if( isTL(1,"PP,TO,OF,CC,AL,AI") )                                                                            return "AA";
          if( isTLNDD("RD,RI") && !isTN(1,"AA,SS,AI,XJ") )                                                             return "SS";
          if( isTLNDD("RD,RI") && isTN(1,"SS,XJ,XG,NN,AA") )                                                           return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                              return "SS";
          if( isTL(1,"CC") && isTL(2,"AA") )                                                                           return "AA";
          if( isTLNDD("VV,VT,VI") && !isTN(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "AA";
          if( isTLNDD("BE") )                                                                                          return "AA";
          if( isTN(1,"OO,OG,OI,OC,OD,OE,OP,OR") || isTN(1,"SS,AA,XJ") || (isWLNDD("more") && isWN(1,"than")) )         return "AA";
          if((isTN(1,"XG") && isTN(1,"LW") && isTL(1,"RD")) || (isTN(1,"CC") && isTN(2,"XJ") && isTN(3,"SS,NP,SC")) || isTN(1,"XP")  && ( iWrd==0 || isTL(1,"PP,TO,OF,CC")) ) 
                                                                                                                       return "AA";
                                                                                                                       return "SS";
        case "P0UW":
          if( isTLNDD("RD,AI,RI,AE,OF,PT,PI,VP,AL") )                                                             return "AA";
          if( isTLNDD("HA") )                                                                                     return "VP";
          if( isTLNDD("BE") )                                                                                     return "AA";
          if( isTNNDD("RD,RI,AI,AL") )                                                                            return "VD";
          if( isWL(1,"there,here") )                                                                              return "VD";
          if( isTL(1,"VP") )                                                                                      return "VP";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
          if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
          if( isTL(1,"OS") )                                                                                      return "VP";
          if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
          if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
          if( isWN(1,"with") )                                                                                    return "VP";
          if( isTL(1,"CC") && isLeftT("VD") )                                                                     return "VD";
          if( isTL(1,"CC") && isLeftT("VP") )                                                                     return "VP";
          if( isTL(1,"CC") && isTL(2,"AA,AI,VP,AL") )                                                             return "VP";
          if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
          if( (isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI") )                                 return "VD";
          if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
          if( iWrd==0 && isModeLW()  )                                                                            return "VP";
          if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there") )                  return "VP";
          if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                  return "VD";
        case "P0UX":
          if( isSufix("ings") )                                                                   return "SS";
          if((isTL(1,"PP,TO") || iWrd==0))                                              {Mode(0); return "GI";}
          if( isTLNDD("BE") )                                                                     return "VI";
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") ||  isTN(1,"AA,AE,AD")) )         return "AA";
                                                                                                  return "GI";
        case "P0UY":
          if( isFUpper() && !isTLNDD("RD,RI,AP,AI,AN") )                                      return "NP";
          if( isTL(1,"SS") && isModeLW() )                                                    return "VV";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VV";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) )
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "VV";} 
                                                                                              return "VV";
        case "P0UZ":
          if( isTL(1,"SS") && isModeLW() )                                                    return "VD";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                  return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0);  return "VD";}
          if( isTL(1,"SS") && isModeLW() )                                                    return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                  return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                     return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                              return "SS";
          if( iWrd==0 && isWN(1,"it"))                                              {Mode(2); return "VD";} 
                                                                                              return "VD";
        case "P0VA":
          if( isMode(TMod.Gerundio) ) return "VY";
                                      return "VA";
        case "P0VC":
          if( isTLNDD("HA") )                                                                                     return "VP";
          if( isTLNDD("BE") )                                                                                     return "VP";
          if( isTNNDD("RD,RI,AI,AL,AF,AN,TH") )                                                                   return "VD";
          if( isWL(1,"there,here") )                                                                              return "VD";
          if( isTL(1,"VP") )                                                                                      return "VP";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
          if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP,TH") )                                                         return "VD";
          if( isTL(1,"OS") )                                                                                      return "VP";
          if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
          if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
          if( isWN(1,"with") )                                                                                    return "VP";
          if( isTL(1,"CC") && isLeftT("VD") )                                                                     return "VD";
          if( isTL(1,"CC") && isLeftT("VP") )                                                                     return "VP";
          if( isTL(1,"CC") && isTL(2,"AA,AI,VP,AL") )                                                             return "VP";
          if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
          if( (isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI") )                                 return "VD";
          if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
          if( iWrd==0 && isModeLW()  )                                                                            return "VP";
          if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there") )                  return "VP";
          if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                  return "VD";
        case "P0VY": Mode(5); return "VA";
        case "P0WO": Mode(3); return "VA";
        case "P0XA":
          if(  isTLNDD("RD,RI,VV,VT,VI,AI") && isTN(1,"SS") )                                 return "AA";
          if( isTL(1,"OC") && isTN(1,"HA,BE") )                                               return "DD";
          if( isTL(1,"BE") )                                                                  return "AA";
          if( (isTLNDD("RD,RI,VV,VT,VI,AI") || isTLNDD("AA,AE,AD")) && !isTNNDD("SS,NP,SC") ) return "SS";
          if( isTL(1,"GW,GV") )                                                               return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )
            {
            if( isTN(1,"SS,NP,SC") )                                                          return "AA";
                                                                                              return "DD";
            }
                                                                                              return "VT";
        case "P0XB":
          if( isTL(1,"GP") && isTN(1,"GP") )                                                      return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( iWrd==0 && isTN(1,"XP,VV,XG,VT,VI") && (isTN(2,"OS,AA,RI,RD,AI") || isWN(2,"it")) ) return "SS";
          if( isTNNDD("VV,VT,VI") )                                                               return "SS";
          if( isTL(1,"PP,OF") )                                                                   return "SS";
          if( isTL(1,"OS") && isTN(1,"SS") )                                                      return "AA";
          if( isTN(1,"BE,HA,VA") )                                                                return "SS";
          if( isTN(1,"AA,AE,AD") )                                                                return "AA";
          if( (isTLNDD("RD,RI,AA,VV,VT,VI")) && (!isTN(1,"SS,NP,SC") || isModeLW()) )             return "SS";
          if( (!isTN(1,"SS") && !isTN(1,"AA")) && (isTL(1,"GW") || isTL(1,"GV")) )                return "SS";
          if( (isTL(1,"RD,RI,AA,GW,GV")) && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD")) )          return "AA";
          if( isTN(1,"BE") )                                                                      return "SS";
          if( isTL(1,"BE") )                                                                      return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                         return "SS";
          if( (isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AE,AD")) && !isTNNDD("SS,NP,SC") )          return "SS";
          if( isTL(1,"GW,GV") )                                                                   return "SS";
                                                                                                  return "VV";
        case "P0XC":
          if( isTN(1,"XG,SS,XJ") && isTN(2,"OF,PP") )                    return "AA";
          if((isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC") )             return "DD";
          if( isTLNDD("BE") )                                            return "DD";
          if( isTN(1,"SS,NP,SC") )                                       return "AA";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI") )                  return "AA";
                                                                         return "VI";
        case "P0XD":
          if( isTN(1,"XG,SS,XJ") && isTN(2,"OF,PP") )                    return "AA";
          if((isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC") )             return "DD";
          if( isTLNDD("BE") )                                            return "DD";
          if( isTN(1,"SS,NP,SC") )                                       return "AA";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI") )                  return "AA";
                                                                         return "VT";
        case "P0XE":
          if( isTN(1,"GG") && isTN(2,"GG") )                              return "AA"; 
          if( isTN(1,"SS,AA,XJ") && isTLNDD("RI,RD,BE,AI,PP,AL") )        return "AA";
          if( isTN(1,"SS,AA,XJ") && isTL(1,"RI,RD,BE,AI,PP,AL") )         return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AL") )           return "AA";
                                                                          return "VI";
        case "P0XF":
          if( (isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC") )            return "DD";
          if( isTLNDD("BE") )                                            return "DD";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI") )                  return "DD";
                                                                         return "VT";
        case "P0XG":
          if( isTN(2,"GG") )                                                            return "SS";
          if( isTLNDD("SS,NP") && (isTN(1,"OS,RI,TO") || isWN(1,"it,him,her,me")) )     return "VI";
          if( isTL(1,"SS,NP") && (isTN(1,"RD,RI,SS,NP,PP,TO") || isWN(1,"a,an,the")) )  return "VI";
          if( isTN(1,"NN") )                                                            return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )               return "SS";
          if( isTLNDD("TO") )                                                           return "VI";
          if( isTL(1,"AP,AE,RD,RI") )                                                   return "SS";
          if( iWrd==0 && isTN(1,"SS") && isTN(2,"BE,VA,HA,VV,VT,VI") )                  return "SS";
          if( isTLNDD("VA,WI") )                                                        return "VI";
          if( isWN(1,"that") && isWN(2,"i,you,he,she,we,they") )                        return "VI";
          if( isTLNDD("VG") )                                                           return "SS";
          if( isTL(1,"GP") && isTN(1,"GP") )                                            return "SS";
          if( isTL(1,"OL,WI") &&  !isTN(1,"BE,HA,VV,VA,VT,VI") )                        return "VI";
          if( isTL(1,"OC") && isTL(2,"VV,VT,VI") )                                      return "VI";
          if( isTN(1,"BE,HA,VV,VA,VT,VI") || isTL(1,"BE,HA,VV,VT,VI") )                 return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                            return "SS";
          if( isTL(1,"SS") && !isTN(1,"RD,RI,PP,AI,AA") )                               return "SS";
          if( isModeLW() )                                                              return "SS";
          if( isTL(1,"SS") && isTL(3,"BE,HA,VV,VT,VI") && isTL(2,"RD,RI") )             return "SS";
          if( isTL(1,"NP") && isTL(2,"RD,RI,AI,AA") )                                   return "SS";
          if( isTL(1,"NP") && iWrd==1 &&  isTN(1,"BE,HA,VV,VT,VI") )                    return "SS";
          if( isTL(1,"NP,AP,AE,AA,RD,RI") && isTN(1,"VA,HA,BE,VC,VV,VT,VI") )           return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isWN(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP") || isTL(1,"PP,OF") || isTNNDD("OF,BE") || isTLNDD("BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) || (isTL(1,"AL") && isTL(2,"PP,OF")) ) 
                                                                                        return "SS";
          if( iWrd==0 && isWN(1,"it"))                                        {Mode(2); return "VV";} 
                                                                                        return "VI";
        case "P0XH":
          if( isTL(1,"OP,SS") && isTN(1,"VV,VT,VI") )                                                         return "DD";
          if( isTL(1,"PP") && isTN(1,"SS") )                                                                  return "AA";
          if( isTN(2,"GG") )                                                                                  return "SS";
          if( isTLNDD("BE") )                                                                                 return "AA";
          if( isTL(1,"PP") && isSufixN(1,"ing") )                                                             return "DD";
          if( isTN(1,"SS,XG,XJ,XH") && isTLNDD("RD,RI") && !isTN(2,"SS") )                                    return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                     return "SS";
          if( isTL(1,"VV,VT,VI") && isTNNDD("RD,RI,AA,AI,PP") )                                               return "DD";
          if( isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") && isModeLW() )  return "DD";
          if( isTN(1,"SS") || ( isWLNDD("more") && isWN(1,"than") ) || ( isTN(1,"XG") && isTN(1,"LW") && isTL(1,"RD") ) || ( isTN(1,"CC") && isTN(2,"XJ") && isTN(3,"SS")) ) 
                                                                                                              return "AA";
          if( isTN(1,"AA") )                                                                                  return "DD";
                                                                                                              return "SS";
        case "P0XI":
          if( isTN(1,"GG") && isTN(2,"GG") )                        return "AA";
          if( isTN(1,"SS,AA,XJ") && isTLNDD("RI,RD,BE,AI,PP,AL") )  return "AA";
          if( isTN(1,"SS,AA,XJ") && isTL(1,"RI,RD,BE,AI,PP,AL") )   return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AL") )     return "AA";
                                                                    return "VV";
        case "P0XJ":
          if( isTL(1,"AL,AI,AA,RD,RI,AF,AP") && isTN(2,"SS,AA,TI") )                                                   return "AA";
          if( isTN(1,"BE,HA,VV,VT,VI,VA") )                                                                            return "SS";
          if( isTL(1,"AL,AI,AA,RD,RI") && isTN(2,"OF") )                                                               return "AA";
          if( isTL(1,"PP,TO,OF,CC,AL,AI") )                                                                            return "AA";
          if( isTLNDD("RD,RI") && !isTN(1,"AA,SS,AI,XJ") )                                                             return "SS";
          if( isTLNDD("RD,RI") && isTN(1,"SS,XJ,XG,NN,AA") )                                                           return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                              return "SS";
          if( isTL(1,"CC") && isTL(2,"AA") )                                                                           return "AA";
          if( isTLNDD("VV,VT,VI") && !isTN(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "AA";
          if( isTLNDD("BE") )                                                                                          return "AA";
          if( isTN(1,"OO,OG,OI,OC,OD,OE,OP,OR") || isTN(1,"SS,AA,XJ") || (isWLNDD("more") && isWN(1,"than")) )         return "AA";
          if((isTN(1,"XG") && isTN(1,"LW") && isTL(1,"RD")) || (isTN(1,"CC") && isTN(2,"XJ") && isTN(3,"SS,NP,SC")) || isTN(1,"XP") && ( iWrd==0 || isTL(1,"PP,TO,OF,CC")) )
                                                                                                                       return "AA";
                                                                                                                       return "SS";
        case "P0XK":
          if( isTL(1,"RD,RI") && !isTN(1,"SS") )  return "SS";
                                                  return "DD";
        case "P0XL":
          if( isTNNDD("OR") ) return "VR";
                              return "DD";
        case "P0XM":
          if( isTL(1,"AA") )                                                                                            return "AA";
          if( isTN(1,"SS,NP,XG") && isTN(2,"PP,OF,TO") )                                                                return "AA";
          if( isTL(1,"BE") && isTN(1,"VG") )                                                                            return "DD";
          if( isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") && isTN(1,"RD,RI,PP,TO") ) return "DD";
          if( isTN(1,"SS,NP") )                                                                                         return "AA";
          if( isTL(1,"OF") && !isTN(1,"AA,AE,AD") )                                                                     return "AA";
          if( isTN(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK"))                           return "DD";
          if((isTLNDD("BE") && !isTL(2,"VA")) || isTL(1,"RD,RI,DN,DD") || isTN(1,"SS") )                                return "AA";
                                                                                                                        return "DD";
        case "P0XN":
          if( (isTL(1,"PP,TO") || iWrd==0) && isSufix("ing"))                  {Mode(0); return "VV";}
          if( isTLNDD("BE") && isSufix("ing") )                                          return "VV";
          if( isTL(1,"BE") )                                                             return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                return "SS";
          if(( isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AE,AD")) && !isTNNDD("SS,NP,SC") ) return "SS";
          if( isTL(1,"GW,GV") )                                                          return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )                                    return "AA";
                                                                                         return "VV";
        case "P0XO":
          if( isFUpper() && !isTLNDD("RD,RI,AP,AI,AN") ) return "NP";
                                                         return "AA";
        case "P0XP":
          if( isTNNDD("OR") )                                            return "VR";
          if((isWL(1,",") && isTL(2,"CC")) || isTL(1,"CC") )             return "DD";
          if( isTLNDD("BE") )                                            return "DD";
          if( isTN(1,"AA,AE,AD") || (isTN(1,"VP,VC") && isTLNDD("BE")) ) return "DD";
          if( isTL(1,"RD,RI,AA,AP,DD,DN,VV,OS,VT,VI") )                  return "DD";
                                                                         return "VV";
        case "P0XQ":
          if( isTNNDD("OR") )                                                                 return "VR";
          if( isTLNDD("RD,RI,VV,VT,VI,AI,GT,GI,VD,VC,VG") && isTN(1,"PB,AA,AF") )             return "DD";
          if( isTLNDD("RD,RI,VV,VT,VI,AI") && isTN(1,"SS") )                                  return "AA";
          if( isTL(1,"OC") && isTN(1,"HA,BE") )                                               return "DD";
          if( isTL(1,"BE") )                                                                  return "AA";
          if(( isTLNDD("RD,RI,VV,VT,VI,AI") || isTLNDD("AA,AE,AD")) && !isTNNDD("SS,NP,SC") ) return "SS";
          if( isTL(1,"GW,GV") )                                                               return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )
            {
            if( isTN(1,"SS,NP,SC") )                                                          return "AA";
                                                                                              return "DD";
            }
                                                                                              return "VT";
        case "P0XR":
          if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") || iWrd==0) 
                                                                                                                return "VG";
          if((isTLNDD("BE") && iWrd!=Words.Count-1) || (isTL(1,"PP,OF,TO") && isTN(1,"RD,RI,AA,AI,CC,PP,TO")) ) return "VG";
          if( isTLNDD("BE") && (isModeLW()) )                                                                   return "AA";
          if( isTL(1,"PP,OF") && (!isTN(1,"SS") && !isTN(1,"AA")) )                                             return "SS";
          if( isTL(1,"SS") && isTN(1,"SS") )                                                                    return "SS";
          if( isTL(1,"RD,RI") && isTN(1,"OF,PP,TO") )                                                           return "SS";
                                                                                                                return "AA";
        case "P0XS":
          if( isSufix("ings") )               return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )  return "SS";
          if( isTL(1,"CC") && isTL(2,"VG") )  return "VG";
          if( isWLNDD("avoid,begin,begins,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") || iWrd==0 || (isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") && iWrd==2) ) 
                                              return "VG";
          if( isTLNDD("BE") )                 return "VG";
          if( isTLNDD("OF,PP,TO") )           return "VG";
          if( isTLNDD("GZ") )                 return "VG";
                                              return "SS";
        case "P0XT":
          if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") ) 
                                                                              return "VG";
          if( isTLNDD("BE") && isModeLW() )                                   return "AA";
          if( isTLNDD("BE") )                                                 return "VG";
          if( isTLNDD("OF,AL") && (isTN(1,"SS,NP,SC") || isTN(1,"AA,AE,AD"))) return "AA"; 
          if( isTLNDD("SS") && isWN(1,"it") )                                 return "VG";
          if( isTLNDD("OF,PP,TO") || (iWrd==0 && !isTN(0,"SS,NP,SC")) )       return "VG";
                                                                              return "AA";
        case "P0XU":
          if( isTLNDD("HA") || isWN(1,"here,there") || isTLNDD("BE") )    return "VP";
          if( isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) )
                                                                          return "SS";
                                                                          return "VV";
        case "P0XW":
          if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") ) 
                                                                              return "VG";
          if( isTL(1,"AA") || isTL(1,"RD") || isTL(1,"RI") || isTLNDD("BE") ) return "VW";
                                                                              return "VG";
        case "P0XY":
          if( isTN(1,"SS,NP") )                                                       return "AI";
          if( isTL(1,"PP,TO,OF,CC") )                                                 return "AI";
          if( isTLNDD("PP") && isTN(1,"VP,VC") )                                      return "AI";
          if( isTL(1,"RD,RI") && isModeLW() )                                         return "AI";
          if( isTL(1,"BE") && isTN(1,"VP,VC,AA,XT") )                                 return "DD";
          if( isTN(1,"AA,AE,AD") )                                                    return "DD";
          if( isTL(1,"RD,RI") && isTN(1,"XG") )                                       return "AI";
          if((isTL(1,"BE") && !isTL(2,"VA")) || isTL(1,"DN,DD") || isTN(1,"SS,NP") )  return "AI";
                                                                                      return "DD";
        case "P0YB":
          if( isTLNDD("RD,AI,RI,AE,OF,PT,PI,VP,AL") )                                                             return "AA";
          if( isTLNDD("HA") )                                                                                     return "VP";
          if( isTLNDD("BE") )                                                                                     return "AA";
          if( isTNNDD("RD,RI,AI,AL") )                                                                            return "VD";
          if( isWL(1,"there,here") )                                                                              return "VD";
          if( isTL(1,"VP") )                                                                                      return "VP";
          if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
          if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
          if( isTL(1,"OS") )                                                                                      return "VP";
          if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
          if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
          if( isWN(1,"with") )                                                                                    return "VP";
          if((isTL(1,"CC") && isLeftT("VD")) )                                                                    return "VD";
          if((isTL(1,"CC") && isLeftT("VP")) )                                                                    return "VP";
          if((isTL(1,"CC") && isTL(2,"AA,AI,VP,AL")) )                                                            return "VP";
          if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
          if((isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI") )                                  return "VD";
          if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
          if( iWrd==0 && isModeLW()  )                                                                            return "VP";
          if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there") )                  return "VP";
          if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                  return "VD";
        case "P0YD":
          if( isTN(1,"GG") && isTN(2,"GG") )                        return "AA";
          if( isTN(1,"SS,AA,XJ") && isTLNDD("RI,RD,BE,AI,PP,AL") )  return "AA";
          if( isTN(1,"SS,AA,XJ") && isTL(1,"RI,RD,BE,AI,PP,AL") )   return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AL") )     return "AA";
                                                                    return "VV";
        case "P0YF":
          if( isTL(1,"VG,GT,GI") )                                                              return "SS";
          if( isTL(1,"OP,YU")    )                                                              return "VT"; 
          if( isTL(1,"SS") && isModeLW() )                                                      return "VT";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                    return "SS";
          if( isTLNDD("PP,FP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an"))) {Mode(0); return "VT";}
          if( isTL(1,"SS") && isModeLW() )                                                      return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                    return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                       return "SS";
          if( (iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                                return "SS";
          if( iWrd==0 && isWN(1,"it"))                                                {Mode(2); return "VT";} 
                                                                                                return "VT";
        case "P0YG":
          if( isTNNDD("OR") ) return "VR";
                              return "SS";
        case "P0YP":
          if( isTL(1,"OF,PP,TO,RD,RI,AA") && isTN(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") )    return "DD";
          if( isTL(1,"OF,PP,TO,RD,RI,AA") )                                                                                      return "AA";
          if( isTL(1,"BE,HA,VV,VC,VP,VT,VI") && isTN(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "DD";
                                                                                                                                return "PP";
        case "P0YQ":
          if( isTL(1,"SS") && isModeLW() )                                                     return "VV";
          if( isTL(1,"CC") && isTL(2,"SS") )                                                   return "SS";
          if( isTLNDD("PP,TO") && (isTNNDD("RD,RI,NP,SS") || isWNNDD("the,a,an")))   {Mode(0); return "VV";}
          if( isTL(1,"SS") && isModeLW() )                                                     return "SS";
          if( isTL(1,"NP") && isTN(1,"VA") )                                                   return "SS";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                      return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP,VT,VI") || (isTL(1,"PP") && !isWL(1,"to")) || isWN(1,"of") || isWL(1,"of") || isTLNDD("BE") || isTN(1,"BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) ) 
                                                                                               return "SS";
          if( iWrd==0 && isWN(1,"it"))                                               {Mode(2); return "VV";}
                                                                                               return "VV";
        case "P0YU":
          if( iWrd==0 || isTN(1,"BE,HA,VV,VS,VA,QJ,YF") || isTLNDD("CC,PP,TO,OF,IF") ) return "OP";
                                                                                       return "OC";
        case "P0ZB":
          if( (iWrd==0 && isWN(1,",")) || (iWrd==0 && isModeLW()) )                                                      return "SS";
          if( isTN(1,"HA,BE,VT,VV,VI,VA") )                                                                              return "SS";
          if( isTL(1,"PP") && isModeLW() || isWN(1,"?") )                                                                return "SS";
          if( isTL(1,"AI,AA") && isTN(1,"SS") )                                                                          return "AA";
          if( iWrd==0 && (isModeLW() || (isTN(1,"LW") && isTN(1,"GZ"))) )                                                return "SS";
          if( isTLNDD("BE") )                                                                                            return "AA";
          if(( isTL(1,"RD,RI,VV,VT,VI") || isTL(1,"AA,AI,AL")) && (!isTNNDD("SS,NP,SC") || isWN(1,"as") || isModeLW()) ) return "SS";
          if( isTL(1,"GW,GV") )                                                                                          return "SS";
          if( isTL(1,"RD,RI") || isTL(1,"AA,AE,AD") )                                                                    return "AA";
                                                                                                                         return "VT";
        case "P0ZF":
          if( isTNNDD("OR") )                                                       return "VR";
          if( isTLNDD("SS,NP") && (isTN(1,"OS,RI,TO") || isWN(1,"it,him,her,me")) ) return "VV";
          if( isTN(1,"NN") )                                                        return "SS";
          if( isModeLW() && iWrd==0)                                                return "SS";
          if( isTLNDD("TO") )                                                       return "VV";
          if( isTL(1,"AP,AE,RD,RI") )                                               return "SS";
          if( iWrd==0 && isTN(1,"SS") && isTN(2,"BE,VA,HA,VV,VT,VI") )              return "SS";
          if( isTLNDD("VA,WI") )                                                    return "VV";
          if( isWN(1,"that") && isWN(2,"i,you,he,she,we,they") )                    return "VV";
          if( isTLNDD("VG") )                                                       return "SS";
          if( isTL(1,"GP") && isTN(1,"GP") )                                        return "SS";
          if( isTL(1,"OL,WI") &&  !isTN(1,"BE,HA,VV,VA,VT,VI") )                    return "VV";
          if( isTL(1,"OC") && isTL(2,"VV,VT,VI") )                                  return "VV";
          if( isTN(1,"BE,HA,VV,VA,VT,VI") || isTL(1,"BE,HA,VV,VT,VI") )             return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                        return "SS";
          if( isTL(1,"SS") && !isTN(1,"RD,RI,PP,AI,AA") )                           return "SS";
          if( isModeLW() )                                                          return "SS";
          if( isTL(1,"SS") && isTL(3,"BE,HA,VV,VT,VI") && isTL(2,"RD,RI") )         return "SS";
          if( isTL(1,"NP") && isTL(2,"RD,RI,AI,AA") )                               return "SS";
          if( isTL(1,"NP") && iWrd==1 &&  isTN(1,"BE,HA,VV,VT,VI") )                return "SS";
          if( isTL(1,"NP,AP,AE,AA,RD,RI") && isTN(1,"VA,HA,BE,VC,VV,VT,VI") )       return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isWN(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP") || isTL(1,"PP,OF") || isTNNDD("OF,BE") || isTLNDD("BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) || (isTL(1,"AL") && isTL(2,"PP,OF")) ) 
                                                                                     return "SS";
          if( iWrd==0 && isWN(1,"it") )                                    {Mode(2); return "VV";} 
                                                                                     return "VV";
        case "P0ZG":
          if( isFUpper() && !isTLNDD("RD,RI,AP,AI,AN") )  return "NP";
                                                          return "VV";
        case "P0ZH":
          if( isTNNDD("OR") ) return "VR";
                              return "VT";
        case "P0ZI":
          if( isTN(1,"GG") && isTN(2,"GG") )                          return "AA";
          if( isTN(1,"SS,AA,XJ") && isTLNDD("RI,RD,BE,AI,PP,AL,AP") ) return "AA";
          if( isTN(1,"SS,AA,XJ") && isTL(1,"RI,RD,BE,AI,PP,AL") )     return "AA";
          if( isTN(1,"AA,XG") || isTLNDD("RI,RD,SS,BE,AI,AL") )       return "AA";
                                                                      return "VT";
        case "P0ZJ":
          if( isTNNDD("OR") )                                                       return "VR";
          if( isTLNDD("SS,NP") && (isTN(1,"OS,RI,TO") || isWN(1,"it,him,her,me")) ) return "VT";
          if( isTN(1,"NN") )                                                        return "SS";
          if( isModeLW() && iWrd==0)                                                return "SS";
          if( isTLNDD("TO") )                                                       return "VT";
          if( isTL(1,"AP,AE,RD,RI") )                                               return "SS";
          if( iWrd==0 && isTN(1,"SS") && isTN(2,"BE,VA,HA,VV,VT,VI") )              return "SS";
          if( isTLNDD("VA,WI") )                                                    return "VT";
          if( isWN(1,"that") && isWN(2,"i,you,he,she,we,they") )                    return "VT";
          if( isTLNDD("VG") )                                                       return "SS";
          if( isTL(1,"GP") && isTN(1,"GP") )                                        return "SS";
          if( isTL(1,"OL,WI") &&  !isTN(1,"BE,HA,VV,VA,VT,VI") )                    return "VT";
          if( isTL(1,"OC") && isTL(2,"VV,VT,VI") )                                  return "VT";
          if( isTN(1,"BE,HA,VV,VA,VT,VI") || isTL(1,"BE,HA,VV,VT,VI") )             return "SS";
          if( isTL(1,"CC") && isTL(2,"SS") )                                        return "SS";
          if( isTL(1,"SS") && !isTN(1,"RD,RI,PP,AI,AA") )                           return "SS";
          if( isModeLW() )                                                          return "SS";
          if( isTL(1,"SS") && isTL(3,"BE,HA,VV,VT,VI") && isTL(2,"RD,RI") )         return "SS";
          if( isTL(1,"NP") && isTL(2,"RD,RI,AI,AA") )                               return "SS";
          if( isTL(1,"NP") && iWrd==1 &&  isTN(1,"BE,HA,VV,VT,VI") )                return "SS";
          if( isTL(1,"NP,AP,AE,AA,RD,RI") && isTN(1,"VA,HA,BE,VC,VV,VT,VI") )       return "SS";
          if((iWrd==0 && isModeLW()) || isWL(1,"-") || isWN(1,"-") || isTL(1,"AA,AE,RD,RI,AI,OS,VP") || isTL(1,"PP,OF") || isTNNDD("OF,BE") || isTLNDD("BE") || (isTL(1,"SS") && isTL(2,"BE") && isTN(1,"PP")) || (isTN(1,"GZ") && (isTL(1,"SS"))) || (isTL(1,"AL") && isTL(2,"PP,OF")) )
                                                                                    return "SS";
          if( iWrd==0 && isWN(1,"it"))                                    {Mode(2); return "VT";} 
                                                                                    return "VT";
        case "P0ZV":
          if( isTNNDD("OR") ) return "VR";
                              return "VV";
        case "P1AL":
          if( isTN(1,"BE,HA,VV,VD,VA"))                                                                            return "OO";
          if( isTN(1,"RD,RI,DD,AA,AE,SS,XT,AI,AP,AF") || (isTL(1,"DD") && isTN(1,"VV,VT,VI")) || isTL(1,"PP,OF") ) return "AL";
                                                                                                                   return "DD";
        case "P1BE":
          if( isTLNDD("TO"))                  {Mode(1); return "BE";}
          if( isWrd("was") || isWrd("were") ) {Time(1); return "BE";} 
                                                        return "BE";
        case "P1HA":
          if( isTN(1,"OR") || isTN(2,"OR") || isTN(3,"OR")) return "HA";
                                                            return "HA";
        case "P1OD":
          if( isTLNDD("SS,NP") && isTN(1,"VA,WI,DO") )      return "OD";
          if((isTL(1,"DO,HA,BE,WI") || isWL(1,"do,does,did,had,have,has,am,is,are,was,were,will,shall,can,would,may,should")) && isLastW("?") && iWrd==1) 
                                                            return "OD";
          if(((isTLNDD("SS,NP,SC") || isTLNDD("VV,VI,VT,VP,VG,BE,HA,WI,VD,VA,BN,VR,JI,JT,PI,PT,GT,GI,HI,HT,XQ,UR,XA,QN,QF,TA,XB,ZB,UO,QA,SV,YF,XG,ZF,ZJ,ZZ,YG,XK,QE,XD,XC,XI,ZI,XE,XM,ZV,ZH,XP,XL,QG,XF,ZG,UH,UB,UW,QB,QC,QD,UZ,UC,UM,UT,UX,UI,UE,UJ,UK,UD,UN,UL,UF,UG,UY,QJ,QK,QM,DO,SH")) && !isWLNDD("month,year,day,week") || isWLNDD("one")) && !isWL(1,"when,where"))
                                                  {InsW(5); return "OD";}
          if( isTL(1,"PP,OF,TO") )                          return "OD";
          if( isTL(1,"CC") )                                return "OD";
          if( iWrd==0 || isTL(1,"GZ") )                     return "OD";
                                                            return "OD";
        case "P1OP":
          if( isTLNDD("SS,NP") && isTN(1,"VA,WI,DO") )            return "OP";
          if((isTL(1,"DO,HA,BE,WI") || isWL(1,"do,does,did,had,have,has,am,is,are,was,were,will,shall,can,would,may,should")) && isLastW("?") && iWrd==1) 
                                                                  return "OP";
          if(((isTLNDD("SS,NP,SC") || isTLNDD("VV,VI,VT,VP,VG,BE,HA,WI,VD,VA,BN,VR,JI,JT,PI,PT,GT,GI,HI,HT,XQ,UR,XA,QN,QF,TA,XB,ZB,UO,QA,SV,YF,XG,ZF,ZJ,ZZ,YG,XK,QE,XD,XC,XI,ZI,XE,XM,ZV,ZH,XP,XL,QG,XF,ZG,UH,UB,UW,QB,QC,QD,UZ,UC,UM,UT,UX,UI,UE,UJ,UK,UD,UN,UL,UF,UG,UY,QJ,QK,QM,DO,SH")) && !isWLNDD("month,year,day,week") || isWLNDD("one")) && !isWL(1,"when,where"))
                                                        {InsW(5); return "OP";}
          if( isTL(1,"PP,OF,TO") )                                return "OP";
          if( isTL(1,"CC") )                                      return "OP";
          if((iWrd==0 || isTL(1,"GZ,OL,IF")) && !isWrd("there") ) return "OP";
                                                                  return "OP";
        case "P1SS":
          if( isTL(1,"GG"))                                               {InsW(0); return "SS";}
          if( isTLNDD("OL,PP"))                                           {InsW(1); return "SS";} 
          if( isTLNDD("RD,AI,AE,AD,AF") )                                           return "SS";
          if( isTL(1,"GP") && isTN(1,"GP") )                                        return "SS";
          if((isWL(1,"from") && isWN(1,"to")) || (isWL(1,"to") && isWL(3,"from")) ) return "SS";
          if( iWrd==0 && isTN(1,"LW") && isTN(1,"NN") )                             return "SS";
          if((isTL(1,"PB,PP,VV,VT,VI,OC,CC,BE,DN,TO,GZ,OF,CO,VG,GP,OE,GT,GI") && !isTL(2,"RD,RI,AI,AA,AD,AF,AE")) || (iWrd==0 && !isLastW()) || (isTL(1,"SS") && iWrd==1) || (isTL(1,"AA") && isTL(2,"DD") ) || (isTL(1,"GZ") && isTL(2,"SS")))
                                                                          {InsW(1); return "SS";} 
                                                                                    return "SS";
        case "P1VG":
          if( isTL(1,"CC") && isTL(2,"VG"))   {Mode(5); return "VG";}
          if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify,consider"))
                                              {Mode(0); return "VG";}
          if( isWL(1,",") )                   {Mode(5); return "VG";}
          if( iWrd==0 )                       {Mode(0); return "VG";}
          if( isTLNDD("PP,TO,OF,CC") )        {Mode(0); return "VG";}
                                              {Mode(5); return "VG";}
        case "P1WI":
          if( isTNNDD("VV,VT,VI,XG,BE,HA,VA,DO")) {Del(0); return "WI";} 
                                                           return "VA";
        case "P2JK":
          //if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,NO") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW() ) 
          //                                  {Time(1); Mode(1); return "JT";}
                                            {Time(1); Mode(1); return "JT";}
        case "P2JL":
          //if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,NO") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW() )
          //                                  {Time(1); Mode(1); return "JI";}
                                            {Time(1); Mode(1); return "JI";}
        case "P2VD":
          if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,OG,XM,NO,FS,HS") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW() ) 
                                            {Time(1); Mode(1); return "JT";}
                                            {Time(1); Mode(1); return "JI";}
        case "P2VP":
          if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,NO,FS,ME") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW())
                                            {Mode(6); return "PT";}
                                            {Mode(6); return "PI";}
        case "P2VS":
          if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,OG,XM,NO,FS,HS") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW() )
                                            {Mode(1); return "HT";}
                                            {Mode(1); return "HI";}
        case "P2VV":
          if( (isTN(1,"RD,RI,AA,AI,SS,AL,SV,XG,XA,XB,ZE,ZH,XJ,XC,YF,YG,XK,,ZF,ZJ,TA,XQ,ZB,OA,AF,MG,AP,AY,NP,AN,NN,OG,XM,NO,FS,HS,ME") || isWN(1,"it,you,me,her,him,that,there,i,we") || isWLNDD("to") || isTLNDD("TO,PP") || isOneSufix("ing,ed")) && !isLastW() )
                                            return "VT";
                                            return "VI";
        case "P3AP":
          if( isTLNDD("SS,SP,NP,AE,VI"))  {InsW(5); return "AP";}
                                                    return "AP";
        case "P3JI": Time(1); return "JI";
        case "P3JK": 
        case "P3JT": Time(1); return "JT";
        case "P3OP":
          if( isTLNDD("TH,OL") )                {Del(0); return "OP";} 
          if( isTLNDD("CC") && isTNNDD("VA,BE,HA,WI") )  return "OP";
                                                         return "OP";
        case "P3VA":
          if( isWrd("would,could") ) {Del(0); return "WO";}
                                              return "VA";
        case "VI"  : 
          if( isTL(1,"BE") && isSufix("ed") )                               {Mode(6); return "VP";}
          if( isWL(1,"being") && isTL(2,"BE") && isSufix("ed"))             {Mode(6); return "VP";}
          if( isMode(TMod.Gerundio) || isSufix("ing") && !isWrd("sing") )
            {
            if( isTL(1,"CC") && isTL(2,"VG"))                               {Mode(5); return "VG";}
            if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify")) {Mode(0); return "VG";}
            if( isWL(1,",") )                                               {Mode(5); return "VG";}
            if( iWrd==0 )                                                   {Mode(0); return "VG";}
            if( isTLNDD("PP,TO,OF,CC"))                                     {Mode(0); return "VG";}
                                                                            {Mode(5); return "VG";}
            }
          if( (isOneSufix("ed") || isMode(TMod.Participio) || isWrd("known,made,abode,arose,awoke,born,borne,beaten,become,befallen,begun,beset,betaken,taken,drawn,dreamt,drunk,driven,eaten,fallen,fed,felt,fought,found,fled,flown,forgotten,frozen,got,given,gone,heard,hoven,hidden,kept,laid,led,learned,learnt,lain,lost,meant,met,mixt,mown,paid,ridden,rung,risen,rose,riven,sawn,said,seen,sodden,sold,sent,shown,shone,shod,shot,shrunk,sung,sunk,sank,sat,slain,slept,slid,slidden,slung,smelt,sown,spoken,sped,sprung,stood,stove,stolen,stuck,struck,stricken,strung,striven,swept,swollen,swum,swung,taught,torn,told,thought,thriven,throve,worn,woven,wept,won,wound,wrought,wrung,written,wrote,")) && !isWrd("need") )
            {
            if( isTLNDD("BE") )                                                                                     return "VP";
            if( isTL(1,"RD,RI,AI,AL") && isTN(1,"SS") )                                                             return "VP";
            if( isTNNDD("RD,RI,AI") )                                                                               return "VD";
            if( isWL(1,"there,here") )                                                                              return "VD";
            if( isTL(1,"VP") )                                                                                      return "VP";
            if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
            if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
            if( isTL(1,"OS") )                                                                                      return "VP";
            if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
            if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
            if( isWN(1,"with") )                                                                                    return "VP";
            if((isTL(1,"CC") && isLeftT("VD")) )                                                                    return "VD";
            if((isTL(1,"CC") && isLeftT("VP")) )                                                                    return "VP";
            if((isTL(1,"CC") && isTL(2,"AA,AI,VP,AL")) )                                                            return "VP";
            if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
            if((isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI,AL") )                               return "VD";
            if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
            if( iWrd==0 && isModeLW() )                                                                             return "VP";
            if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there"))                   return "VP";
            if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                    return "VD";
            }
                                                                                                                    return "VV";
        case "VR"  : 
          if( isWL(1,"being") && isTL(2,"BE") && isSufix("ed") ) {Mode(6); return "VP";}
          if( isMode(TMod.Gerundio) || isSufix("ing") )
            {
            if( isTL(1,"CC") && isTL(2,"VG") )  {Mode(5); return "VG";}
            if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify") )
                                                {Mode(0); return "VG";}
            if( isWL(1,",") )                   {Mode(5); return "VG";}
            if( iWrd==0 )                       {Mode(0); return "VG";}
            if( isTLNDD("PP,TO,OF,CC") )        {Mode(0); return "VG";}
                                                {Mode(5); return "VG";}
            }
          if( isSufix("ed") || isMode(TMod.Participio) )
            {
            if( isTLNDD("BE") )                                                                                     return "VP";
            if( isTL(1,"RD,RI,AI,AL") && isTN(1,"SS") )                                                             return "VP";
            if( isTNNDD("RD,RI,AI") )                                                                               return "VD";
            if( isWL(1,"there,here") )                                                                              return "VD";
            if( isTL(1,"VP") )                                                                                      return "VP";
            if( isTL(1,"SS") && isModeLW() )                                                                        return "VD";
            if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                            return "VD";
            if( isTL(1,"OS") )                                                                                      return "VP";
            if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC") )                                                            return "VP";
            if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK") ) return "VP";
            if( isWN(1,"with") )                                                                                    return "VP";
            if((isTL(1,"CC") && isLeftT("VD")) )                                                                    return "VD";
            if((isTL(1,"CC") && isLeftT("VP")) )                                                                    return "VP";
            if((isTL(1,"CC") && isTL(2,"AA,AI,VP,AL")) )                                                            return "VP";
            if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                               return "VD";
            if((isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI,AL") )                               return "VD";
            if( isWN(1,"-") || isWL(1,"-") )                                                                        return "VP";
            if( iWrd==0 && isModeLW()  )                                                                            return "VP";
            if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there") )                  return "VP";
            if( isTLNDD("BE") || isWLNDD("been") )                                                                  return "VP";
                                                                                                                    return "VD";
            }                                                                                                       
                                                                                                                    return "VV";
        case "VT"  : 
          if( isTL(1,"BE") && isSufix("ed") )                             {Mode(6); return "VP";}
          if( isWL(1,"being") && isTL(2,"BE") && isSufix("ed") )          {Mode(6); return "VP";}
          if( isMode(TMod.Gerundio) || isSufix("ing") && !isWrd("sing") )
            {
            if( isTL(1,"CC") && isTL(2,"VG") )                            {Mode(5); return "VG";}
            if( isWLNDD("avoid,begin,cease,complete,continue,decline,defer,delay,dislike,finish,forbear,hate,help,intend,like,make,postpone,prevent,propose,purpose,ronounce,repent,risk,scruple,sygnify")) 
                                                                          {Mode(0); return "VG";}
            if( isWL(1,",") )                                             {Mode(5); return "VG";}
            if( iWrd==0 )                                                 {Mode(0); return "VG";}
            if( isTLNDD("PP,TO,OF,CC"))                                   {Mode(0); return "VG";}
                                                                          {Mode(5); return "VG";}
            }
          if( isOneSufix("ed") || isMode(TMod.Participio) || isWrd("known,made,abode,arose,awoke,born,borne,beaten,become,befallen,begun,beset,betaken,taken,drawn,dreamt,drunk,driven,eaten,fallen,fed,felt,fought,found,fled,flown,forgotten,frozen,got,given,gone,heard,hoven,hidden,kept,laid,led,learned,learnt,lain,lost,meant,met,mixt,mown,paid,ridden,rung,risen,rose,riven,sawn,said,seen,sodden,sold,sent,shown,shone,shod,shot,shrunk,sung,sunk,sank,sat,slain,slept,slid,slidden,slung,smelt,sown,spoken,sped,sprung,stood,stove,stolen,stuck,struck,stricken,strung,striven,swept,swollen,swum,swung,taught,torn,told,thought,thriven,throve,worn,woven,wept,won,wound,wrought,wrung,written,wrote,") && !isWrd("need") )
            {
            if( isTLNDD("BE") )                                                                                    {Mode(6); return "VP";}
            if( isTL(1,"RD,RI,AI,AL") && isTN(1,"SS"))                                                             {Mode(6); return "VP";}
            if( isTNNDD("RD,RI,AI") )                                                                                        return "VD";
            if( isWL(1,"there,here") )                                                                                       return "VD";
            if( isTL(1,"VP"))                                                                                      {Mode(6); return "VP";}
            if( isTL(1,"SS") && isModeLW() )                                                                                 return "VD";
            if( isTL(1,"SS,NP") && isTNNDD("OS,TO,NP") )                                                                     return "VD";
            if( isTL(1,"OS")) {Mode(6);                                                                                      return "VP";}
            if( isTN(1,"PP,OF") && isTLNDD("SS,NP,SC"))                                                            {Mode(6); return "VP";}
            if( isTN(1,"PP,OF") && isTL(1,"DD,DN,DA,DB,DC,DF,DG,DI,DK,DL,DM,DO,DP,DR,DS,DT,DU,DW,DX,DY,DZ,XY,XK")) {Mode(6); return "VP";}
            if( isWN(1,"with"))                                                                                    {Mode(6); return "VP";}
            if( isTL(1,"CC") && isLeftT("VD") )                                                                              return "VD";
            if( isTL(1,"CC") && isLeftT("VP") )                                                                              return "VP";
            if( isTL(1,"CC") && isTL(2,"AA,AI,VP,AL") )                                                            {Mode(6); return "VP";}
            if( isTLNDD("NP") && isTN(1,"PP,NN,AE") )                                                                        return "VD";
            if( (isTLNDD("SS,NP,SC") || isTL(1,"GZ")) && isTN(1,"PP,NN,AE,RD,RI,AL") )                                       return "VD";
            if( isWN(1,"-") || isWL(1,"-"))                                                                        {Mode(6); return "VP";}
            if( iWrd==0 && isModeLW() )                                                                                      return "VP";
            if( isTLNDD("HA") || isTL(1,"DD,RD,RI,PP,OF,GZ,SS,NP,AL,AI") || isWN(1,"here,there") )                 {Mode(6); return"VP";}
            if( isTLNDD("BE") || isWLNDD("been") )
                                                                                                                   {Mode(6); return "VP";} 
                                                                                                                             return "VD";
            }                                                                                                      
                                                                                                                             return "VV";

        default: 
          return null;    
        }

      return Tipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia el tipo de la palabra para el idioma Italiano</summary>
    private string ChageTypeIt( int iWrd, string Tipo, string Pref )
      {
      switch( Pref + Tipo )
        {
        case "CAE":
        case "CAF":
        case "CAN": return "AI";
        case "CDF": return "DD";
        case "CGI":
        case "CGT": return "VG";
        case "CHI":
        case "CHT": return "VV";
        case "CJI":
        case "CJT": return "VD";
        case "CPI":
        case "CPT": return "VP";
        case "CVI":
        case "CVT": return "VV";
        case "TGI": return "VI";
        case "TGT": return "VT";
        case "THI": return "VI";
        case "THT": return "VT";
        case "TJI": return "VI";
        case "TJT": return "VT";
        case "TPI": return "VI";
        case "TPT": return "VT";
        case "P3BD":
        case "P3BF":
        case "P3BH":
        case "P3BI":
        case "P3BJ":
        case "P3BK":
        case "P3BM":
        case "P3BP":
        case "P3BS": return "BE";
        case "P3HC": 
        case "P3HD": 
        case "P3HF": 
        case "P3HJ": 
        case "P3HM": 
        case "P3HP": 
        case "P3HV": return "HA";
        case "P3VB": 
        case "P3VE": 
        case "P3VF": 
        case "P3VH": 
        case "P3VJ": 
        case "P3VQ": 
        case "P3VU": 
        case "P3VX": 
        case "P3VY": return "VA";
        case "P3VZ": Mode(3); return "VA";
        case "P0DN":
          if( isTN(1,"VT,VI,VA,VV,ZF,YF")  && !isTLNDD("PP,VA,BE,HA,OF,XX") ) return "VA";
                                                                              return "DN";
        case "P0WL":
        case "P0WM":
        case "P0WN":
        case "P0WP":
        case "P0WQ":
        case "P0WR":
        case "P0WS":
        case "P0WT":
        case "P0WV":
        case "P0WW":
        case "P0WX":
        case "P0WY":
        case "P0XB":
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTipo("FW") || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                              return "AA";
                                                                                                 return "VT";
        case "P0JK": Mode(6); return "VP";
        case "P0JL": Mode(6); return "VP";
        case "P1BG": Mode(5); return "BG";
        case "P1BI": Mode(1); return "BI";
        case "P1HD": Mode(1); return "HD";
        case "P1BS": Mode(4); return "BS";
        case "P1BP": Mode(3); return "BP";
        case "P1BM": Mode(2); return "BM";
        case "P1BN": Mode(6); return "BN";
        case "P1VU": Mode(6); return "VU";
        case "P1HG": Mode(5); return "HG";
        case "P1HJ": Mode(4); return "HJ";
        case "P1HM": Mode(2); return "HM";
        case "P1HN": Mode(6); return "HN";
        case "P1HP": Mode(3); return "HP";
        case "P1VJ": Mode(4); return "VJ";
        case "P1VL": Mode(1); return "VL";
        case "P1VW": Mode(2); return "VW";
        case "P1VX": Mode(1); return "VX";
        case "P1VY": Mode(5); return "VY";
        case "P1VZ": Mode(3); return "VZ";
        case "P2VK": Mode(4); return "VK";
        case "P2VL": Mode(1); return "VL";
        case "P2VM": Mode(3); return "VM";
        case "P2VN": Mode(2); return "VN";
        case "P2VP": Mode(6); return "VP";
        case "P0YH": return (isTLNDD("SS"))?"AA":"BO";
        case "P0YI": return (isTLNDD("SS"))?"AA":"BL";
        case "P0YJ": return (isTLNDD("SS"))?"AA":"BX";
        case "P0YK": return (isTLNDD("SS"))?"AA":"BR";
        case "P0YL": return (isTLNDD("SS"))?"AA":"BZ";
        case "P0YM": return (isTLNDD("SS"))?"AA":"BY";
        case "P0YN": return (isTLNDD("SS"))?"AA":"VM";
        case "P1SP": Plural(); return "SS";
        case "MVP" : Mode(6); return Tipo;
        case "P2VG": Mode(5); return "VG"; 
        case "MBE":
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
                                                                                  Mode(0);
          else if( isWLNDD("dovere,tenere,potere") || isTLNDD("VA") )             Mode(0);
          else if( isOneSufix("ato,uto") && isTNNDD("RD,RI,NP") )                 Mode(1);
          else if( isOneSufix("ato,uto") && isTLNDD("HA,BE")    )                 Mode(6);
          else if( isOneSufix("ando,endo") && ( iWrd==0 || isWLNDD("a,ad,per")) ) Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "P0QK":
        case "P0QJ":
          if( iWrd==0 && isTNNDD("OL,CC") )                                                                return "VT";
          if( isLastW() )                                                                                  return "SS";
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
                                                                                                           return "VA";
        case "MHA":
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
                                                                                  Mode(0);
          else if( isWLNDD("dovere,tenere,potere") || isTLNDD("VA") )             Mode(0);
          else if( isOneSufix("ato,uto"  ) && isTNNDD("RD,RI,NP") )               Mode(1);
          else if( isOneSufix("ato,uto"  ) && isTLNDD("HA,BE"   ) )               Mode(6);
          else if( isOneSufix("ando,endo") && ( iWrd==0 || isWLNDD("a,ad,per")) ) Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per")   )                                        Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVA":
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )
            { Mode(2); InsW(0); }
          else if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli.erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
            {    
            if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); else Mode(0);
            }
          else if( isWLNDD("devere,tenere,potere") || isTLNDD("VA,TO") )          Mode(0);
          else if( isOneSufix("ato,uto"  ) && isTNNDD("RD,RI,NP") )               Mode(1);
          else if( isOneSufix("ato,uto"  ) && isTLNDD("HA,BE"   ) )               Mode(6);
          else if( isOneSufix("ando,endo") && (iWrd==0 || isWLNDD("a,ad,per")) )  Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVI":
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )
            { Mode(2); InsW(0); return "VI";}
          else if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") ) Mode(0); else Mode(0);
            }
          else if( isWLNDD("devere,tenere,potere") || isTLNDD("VA,TO") )          Mode(0);
          else if( isOneSufix("ato,uto"  ) && isTNNDD("RD,RI,NP") )               Mode(1);
          else if( isOneSufix("ato,uto"  ) && isTLNDD("HA,BE"   ) )               Mode(6);
          else if( isOneSufix("ando,endo") && (iWrd==0 || isWLNDD("a,ad,per")) )  Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVR" :
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
            {
            if( iWrd==0 ) Mode(5); 
            else          Mode(0);
            }
          else if( isWLNDD("dovere,tenere,potere") || isTLNDD("VA") )             Mode(0);
          else if( isOneSufix("ato,uto") && isTNNDD("RD,RI,NP") )                 Mode(1);
          else if( isOneSufix("ato,uto") && isTLNDD("HA,BE") )                    Mode(6);
          else if( isOneSufix("ando,endo") && (iWrd==0 || isWLNDD("a,ad,per")) )  Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVT" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VT";}
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") )  Mode(0);
            else                              Mode(0);
            }
          else if( isWLNDD("devere,tenere,potere") || isTLNDD("VA,TO") )          Mode(0);
          else if( isOneSufix("ato,uto") && isTNNDD("RD,RI,NP") )                 Mode(1);
          else if( isOneSufix("ato,uto") && isTLNDD("HA,BE") )                    Mode(6);
          else if( isOneSufix("ando,endo") && (iWrd==0 || isWLNDD("a,ad,per")) )  Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVV" :
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") )
            {
            if( iWrd==0 )   Mode(5);
                            Mode(0);
            }
          else if( isWLNDD("devere,tenere,potere") || isTLNDD("VA") )             Mode(0);
          else if( isOneSufix("ato,uto") && isTNNDD("RD,RI,NP") )                 Mode(1);
          else if( isOneSufix("ato,uto") && isTLNDD("HA,BE") )                    Mode(6);
          else if( isOneSufix("ando,endo") && (iWrd==0 || isWLNDD("a,ad,per")) )  Mode(0);
          else if( isOneSufix("ando,endo") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("a,ad,per") )                                          Mode(0);
          else                                                                    Mode(1);
          break;
        case "NVI" :
          if( isMode(TMod.Participio) )                       return "VP";
          if( isOneSufix("ato,ata,ati,ate,uto,uta,uti,ute") ) return "VP";
          if( isMode(TMod.Gerundio) )                         return "VG";
          if( isOneSufix("ando,endo") )                       return "VG";
                                                              return "VI";
        case "NVT" :
          if( isMode(TMod.Imperativo) ) return "VM";
          if( isMode(TMod.Indicativo) ) return "VL";
          if( isMode(TMod.Participio) ) return "VP";
          if( isOneSufix("ato,ata,ati,ate,uto,uta,uti,ute") ) return "VP";
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isOneSufix("ando,endo") ) return "VG";
          return "VT";
        case "NVV" :
          if( isMode(TMod.Participio) ) return "VP";
          if( isOneSufix("ato,ata,ati,ate,uto,uta,uti,ute") ) return "VP";
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isOneSufix("ando,endo") ) return "VG";
          return "VV";
        case "P0AO" :
          if( !isWLNDD("il,la,lo,le,i,gli,un,una,uno") || isTN(1,"VV,VT,VI,VR,HA,BE") ) return "OO";
          return "AI";
        case "P0AS" :
          if( isFUpper() ) return "NP";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG,GT,GI") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0HA" :
          if( isTLNDD("RD,RI,AD,AI") ) return "SS";
          return "HA";
        case "P0HJ": Mode(4); return "HJ";
        case "P0IA" :
          if( isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC";
          if( isTNNDD("AA,AI,AO,XJ,SS,QC,UC") ) return "RD";
          return "OC";
        case "P0IS" :
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          return "VV";
        case "P0MG" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "OG";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "OG";
          if( isTL(1,"SS,XJ,SP") ) return "AF";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AF";
          if( iWrd==0 && isTN(1,"SS") ) return "AF";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AF";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "OG";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AF";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"OG")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "OG";
          return "AF";
        case "P0MU" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "OG";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "OG";
          if( isTL(1,"SS,XJ,SP") ) return "AF";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AF";
          if( iWrd==0 && isTN(1,"SS") ) return "AF";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AF";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "OG";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AF";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"OG")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "OG";
          return "AF";
        case "P0MV" :
          if( isTLNDD("RD,RI,AI,AL") ) return "AN";
          return "NN";
        case "P0OD" :
          if( isTN(1,"AA") ) return "DD";
          if( isTN(1,"SS") ) return "AA";
          return "OO";
        case "P0OF" :
          if( isSufixNNDD("urre") || isSufixNNDD("orre") || isSufixNNDD("are") || isSufixNNDD("ere") || isSufixNNDD("ire") || isSufixNNDD("arsi") || isSufixNNDD("ersi") || isSufixNNDD("irsi") || isSufixNNDD("arme") || isSufixNNDD("erme") || isSufixNNDD("irme") || isSufixNNDD("urle") || isSufixNNDD("urla") || isSufixNNDD("urlo") || isSufixNNDD("arle") || isSufixNNDD("erle") || isSufixNNDD("irle") || isSufixNNDD("arci") || isSufixNNDD("erci") || isSufixNNDD("irci") || isSufixNNDD("arli") || isSufixNNDD("erli") || isSufixNNDD("irli") || isSufixNNDD("arla") || isSufixNNDD("erla") || isSufixNNDD("irla") || isSufixNNDD("arlo") || isSufixNNDD("erlo") || isSufixNNDD("irlo") || isSufixNNDD("arle") || isSufixNNDD("erle") || isSufixNNDD("irle") || isSufixNNDD("arvi") || isSufixNNDD("ervi") || isSufixNNDD("irvi") || isSufixNNDD("arti") || isSufixNNDD("erti") || isSufixNNDD("irti") || isSufixNNDD("argli") || isSufixNNDD("ergli") || isSufixNNDD("irgli") || isSufixNNDD("arne") || isSufixNNDD("erne") || isSufixNNDD("irne") ) return "TO";
          return "OF";
        case "P0OJ" :
          if( isWN(1,"!") ) return "JJ";
          return "OO";
        case "P0OZ" :
          if( isTLNDD("OL") || iWrd==0 ) return "OO";
          if( isTN(1,"AA") ) return "DD";
          if( isTL(1,"SS") ) return "AA";
          return "DD";
        case "P0PP" :
          if( isSufixNNDD("urre") || isSufixNNDD("orre") || isSufixNNDD("are") || isSufixNNDD("ere") || isSufixNNDD("ire") || isSufixNNDD("arsi") || isSufixNNDD("ersi") || isSufixNNDD("irsi") || isSufixNNDD("arme") || isSufixNNDD("erme") || isSufixNNDD("irme") || isSufixNNDD("urle") || isSufixNNDD("urla") || isSufixNNDD("urlo") || isSufixNNDD("arle") || isSufixNNDD("erle") || isSufixNNDD("irle") || isSufixNNDD("arci") || isSufixNNDD("erci") || isSufixNNDD("irci") || isSufixNNDD("arli") || isSufixNNDD("erli") || isSufixNNDD("irli") || isSufixNNDD("arla") || isSufixNNDD("erla") || isSufixNNDD("irla") || isSufixNNDD("arlo") || isSufixNNDD("erlo") || isSufixNNDD("irlo") || isSufixNNDD("arle") || isSufixNNDD("erle") || isSufixNNDD("irle") || isSufixNNDD("arvi") || isSufixNNDD("ervi") || isSufixNNDD("irvi") || isSufixNNDD("arti") || isSufixNNDD("erti") || isSufixNNDD("irti") || isSufixNNDD("argli") || isSufixNNDD("ergli") || isSufixNNDD("irgli") || isSufixNNDD("arne") || isSufixNNDD("erne") || isSufixNNDD("irne") ) return "TO";
          return "PP";
        case "P0QA" :
          if( (isOneSufix("a,e,ono,ano")) && (isTL(1,"SE,OO") || (iWrd==0 || isTLNDD("SE") )) ) return "VV";
          if((isOneSufix("are,urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne")) && isTL(1,"TO,VA") ) return "VV";
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") ) return "SS";
          return "DD";
        case "P0QB" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "XJ";
        case "P0QC" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "XJ";
        case "P0QD" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "XJ";
        case "P0QE" :
          if( (isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "XM";
          if((isOneSufix("ato,uto")) ) return "VP";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0QF" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0QG" :
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0QI" : return "PP";
        case "P0QN" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AI";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AI";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AI";
        case "P0QO" :
          if( isTLNDD("BE,SS,AA") ) return "AI";
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS";
          return "DD";
        case "P0QS" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VL";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VL";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0QT" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "BY";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "BY";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0QU" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "BL";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "BL";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0RD" :
          if( isWrd("la") && isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC";
          return "RD";
        case "P0SE" :
          if( iWrd==0 && isModeNNDD(TMod.Potencial) )                     {Mode(1); return "BE";}
          if( isWN(1,"la,lo,le,li,le,ci,mi,vi,ti") && isTN(2,"VR") )                return "OC";
          if( isWN(1,"mi,ti,lo,la,ci,vi,li,le") && !isTN(2,"VR") )        {Mode(1); return "BE";}
          if( isTLNDD("OL,CC") || isTN(1,"VT,VI,VV,TX,ZB,IS,XA,XB,ZE,ZF,ZL,VA,VR,ZV,ZG,ZA,YF,ZH,SV,ZJ,XG") || isWNNDD("debe") ) 
                                                              {ModeN(1,6); Mode(1); return "BE";}
                                                                                    return "OR";
        case "P0SV" :
          if( (isOneSufix("a,e,ono,ano")) && (isTL(1,"SE,OO") || (iWrd==0 || isTLNDD("SE") )) )           return "VV";
          if((isOneSufix("are,urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne")) && isTL(1,"TO,VA") ) 
                                                                                                          return "VV";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "SS";
        case "P0TA" :
          if( isTLNDD("OO") || iWrd==0 ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "SS";
        case "P0TX" :
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0UA" :
          if( isFUpper() ) return "NP";
          return "SS";
        case "P0UB" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG") ) return "VP";
          return "AA";
        case "P0UC" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "SS";
        case "P0UE" :
          if( isTNNDD("BE,BD,BF,BH,BJ,BK,BA,BN,BG,BS,BI,BP,BM") ) return "VG";
          return "SS";
        case "P0UF" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VT";
          if( isModeLW() ) return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
          return "VT";
        case "P0UG" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VT";
          if( isModeLW() ) return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
          return "VV";
        case "P0UH" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "AA";
        case "P0UI" :
          if( isTNNDD("BE,BD,BF,BH,BJ,BK,BA,BN,BG,BS,BI,BP,BM") ) return "VG";
          return "SS";
        case "P0UJ" :
          if( isTNNDD("BE,BD,BF,BH,BJ,BK,BA,BN,BG,BS,BI,BP,BM") ) return "VG";
          return "SS";
        case "P0UL" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VT";
          if( isModeLW() ) return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
          return "VV";
        case "P0UO" :
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") ) return "AA";
          if( isTLNDD("RD,RI,AI") ) return "SS";
          if( iWrd==0 && isModeLW() ) return "SS";
          if( iWrd==0 && isWN(1,"di") ) return "SS";
          if((isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if((isOneSufix("ato,uto")) ) return "VP";
          return "VV";
        case "P0UR" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0UW" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "AA";
        case "P0UZ" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "SS";
        case "P0VT" :
          if( isOneSufix("are,ere,ire,urre,orre") && isWL(1,"non") && iWrd==1) {Mode(2); return "VT";}
          return "VT";
        case "P0WA" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "HD";
        case "P0WB" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BS";
        case "P0WC" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VJ";
        case "P0WD" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VB";
        case "P0WE" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "DD";
        case "P0WF" :
          if( isTLNDD("SS,NP") ) return "VL";
          return "DD";
        case "P0WH" :
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BL";
          return "DD";
        case "P0WJ" :
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BY";
          return "DD";
        case "P0WK" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VU";
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0WZ" :
          if( isTL(1,"PP,TO") ) return "OC";
                                return "OP";
        case "P0XA" :
          if( iWrd==0 && isModeLW() )                                                                                              return "SS";
          if( isTNNDD("OF") && isTL(1,"SS") )                                                                                      return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )                                                                            return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                                                                   return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") )                                                                                                 return "VV";
          if( isTN(1,"AA") )                                                                                                       return "DD";
                                                                                                                                   return "AA";
        case "P0XC" :
          if( (isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )       return "XM";
          if((isOneSufix("ato,uto")) )                    return "VP";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )        return "VR";
                                                          return "VV";
        case "P0XD" :
          if( (isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )       return "XM";
          if((isOneSufix("ato,uto")) )                    return "VP";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )        return "VR";
                                                          return "VV";
        case "P0XE" :
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTNNDD("OF") )  return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                               return "AA";
                                                                                  return "VV";
        case "P0XF" :
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") )  return "VR";
                                                    return "VV";
        case "P0XG" :
          if( Wrd.Entre==2 && isTL(1,"SS,AA") )                                                                 return "SS";
          if( isTLNDD("OF,RD,RI,AI") )                                                                          return "SS";
          if(  isOneSufix("ato,uto") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) )  return "VV";
          if(   isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG") || isTN(1,"AA,OF")   || iWrd==0 || isTNNDD("OF") )     return "SS";
                                                                                                                return "VV";
        case "P0XH" :
          if( isTLNDD("BE,SS,AA") )       return "AA";
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS";
          return "DD";
        case "P0XI" :
          if( isTLNDD("SS") ) return "AA";
                              return "VV";
        case "P0XJ" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XK" :
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") ) return "SS";
          return "DD";
        case "P0XM" :
          if( isTN(1,"DD,AA") ) return "DD";
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") ) return "AA";
          return "DD";
        case "P0XO" :
          if( isFUpper() ) return "NP";
          return "AA";
        case "P0XQ" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0XS" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XW" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XY" :
          if( isTN(1,"DD,AA") ) return "DD";
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") ) return "AI";
          return "DD";
        case "P0YA" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VU";
          return "AA";
        case "P0YB" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
          return "AA";
        case "P0YC" :
          if( isTLNDD("VA,VU,VY,VJ,VX,VZ,VW") ) return "VV";
          return "AA";
        case "P0YD" :
          if( isTLNDD("SS") ) return "AA"; else {Mode(1); return "VV";}
        case "P0YE" :
          if( isTLNDD("SS") ) return "AA"; else {Mode(4); return "VV";}
        case "P0YF" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VT";
          if( isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VT";
        case "P0YG" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,OF,VT,VI,VV,TX,VG,PP") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VR";
        case "P0YP" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VK";
          if( isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VK";
        case "P0YQ" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VL";
        case "P0YR" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VN";
        case "P0YS" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BZ";
        case "P0YT" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BL";
        case "P0YV" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BY";
        case "P0YW" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BO";
        case "P0YX" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BX";
        case "P0YZ" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BR";
        case "P0ZA" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isTLNDD("OF") && isTN(1,"AA,XJ") && isTN(2,"OF") ) return "SS";
          if( iWrd==0 && isWN(1,"la,il,i,gli,lo,un,una,uno") ) return "VV";
          if((isTLNDD("RD,RI,AI,AA,AD,VG,VV,TX,VT,VI") && (!isTN(1,"SS") || isModeLW())) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0ZB" :
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") ) return "AA";
          if( isTLNDD("RD,RI,AI") ) return "SS";
          if( iWrd==0 && isModeLW() ) return "SS";
          if( iWrd==0 && isWN(1,"di") ) return "SS";
          if((isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if((isOneSufix("ato,uto")) ) return "VP";
          return "VV";
        case "P0ZE" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0ZF" :
          if( isTLNDD("OF") ) return "SS";
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VV";
        case "P0ZG" :
          if(  isWNNDD("a,ad,per,il,la,lo") ) return "VI";
          if( isOneSufix("ato,uto") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) ) return "VI";
          if( isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG")  || isTN(1,"AA,OF") || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VI";
        case "P0ZH" :
          if( isWLNDD("mi,ti,li,gli,ci,la,si,vi") ) return "VR";
          return "VV";
        case "P0ZI" :
          if( (isOneSufix("ato,uto")) && !isTLNDD("HA") ) return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if((isOneSufix("ato,uto")) ) return "VP";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0ZJ" :
          if( isWLNDD("mi,ti,ci,si,vi,ne") || isTLNDD("SE") ) return "VR";
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO,AA") ) return "SS";
          if( isSufix("xxios") ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0ZK" :
          if( (isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VI";
        case "P0ZV" :
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P0ZZ" :
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO") ) return "SS";
          if( isSufix("xxios") ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") ) return "SS";
          if( isWLNDD("mi,ti,lo,la,ci,vi,li,le") ) return "VR";
          return "VV";
        case "P1BD" :
          if( isLeft("che")) {Mode(4); return "BS";}
          {Mode(1); return "BI";}
        case "P1BE" :
          if( isTNNDD("BN") ) return "HA";
          if( isMode(TMod.Infinitivo) && iWrd==0) {Mode(5); return "VG";}
          if( isTLNDD("CC,GZ") || iWrd==0 || (iWrd==1 && isTL(1,"DD,DN"))) {InsW(9); return "BE";}
          return "BE";
        case "P1BF" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BH" :
          if( isLeft("che")) {Mode(4); return "BS";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BJ" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "BN";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BK" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(4); return "BS";}
        case "P1BL" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BO" :
          if( isLeft("che")) {Mode(4); return "VK";}
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VD";}
        case "P1BR" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          {Mode(1); return "VL";}
        case "P1BX" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BY" :
          if( isLeft("che")) {Mode(4); return "VK";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BZ" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(4); return "VK";}
        case "P1DN" :
          if( iWrd==0 && (isOneSufix("urre") || isOneSufix("orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne")) ) 
                                                                                                        return "DN";
          if((isTNNDD("VV,VI,VT,VZ") && !isTimeNNDD(TTime.Futuro) && isWrd("non")) || isTLNDD("GN") )  return "VA";
                                                                                                        return "DN";
        case "P1HC" :
          if( isLeft("che")) {Mode(4); return "HJ";}
          {Mode(1); return "HD";}
        case "P1HF" :
          if( isLeft("che")) {Mode(4); return "HJ";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "HM";}
          {Mode(1); return "HD";}
        case "P1HV" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "HM";}
          {Mode(4); return "HJ";}
        case "P1VB" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VU";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VC" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P1VE" :
          if( isLeft("che")) {Mode(4); return "VJ";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VF" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(4); return "VJ";}
        case "P1VH" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VO" :
          if( isLeft("che")) {Mode(4); return "VK";}
          {Mode(1); return "VL";}
        case "P1VQ" :
          if( isLeft("che")) {Mode(4); return "VJ";}
          {Mode(1); return "VX";}
        case "P1VS" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P1VT" :
          if( isTN(1,"SE,OR") ) return "VR";
          return "VT";
        case "P1VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,FS,XJ") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                                    return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,AA,FS,XJ,IA,OC") || isWNNDD("che") )    return "VT";
                                                                                    return "VI";
        case "P1XJ" :
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P2BE" :
          if( isWL(1,"se,se gia,quando") || (isWL(1,"non") && iWrd==1) || iWrd==0 ) {InsW(9); return "BE";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                        {InsW(9); return "BE";}
          if( isTLNDD("CC") )                                                       {InsW(9); return "BE";}
                                                                                              return "BE";
        case "P2HA" :
          if( isWL(1,"si,quando") || isTLNDD("OL,GZ,CC") || (isWL(1,"no") && iWrd==1) || iWrd==0) {InsW(9); return "HA";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                                      {InsW(9); return "HA";}
          if( isTLNDD("CC,XX") )                                                                  {InsW(9); return "HA";}
                                                                                                            return "HA";
        case "P2SS" :
          if( isTNNDD("BE") && isTL(1,"RD") && iWrd==1 ) {Del(-1); return "SS";} 
                                                                   return "SS";
        case "P2VA" :
          if( isWL(1,"se,quando") || (isWL(1,"non") && iWrd==1) || iWrd==0) {InsW(9); return "VA";}
          if( isTL(1,"AA") && isTL(2,"BE")) {InsW(9);                                 return "VA";}
          if( isTLNDD("CC,OL,GZ")) {InsW(9);                                          return "VA";}
                                                                                      return "VA";
        case "P2VI" :
          if( isMode(TMod.Gerundio) )                                                 return "VG";
          if( isMode(TMod.Participio) )                                               return "VP";
          if( isWLNDD("permitir,permite"))                                  {InsW(7); return "VT";}
          if( isWL(1,"se,quando") || isTLNDD("GZ,CC") || iWrd==0)           {InsW(9); return "VT";}
          if( isTime(TTime.Futuro) && isTL(1,"DN"))            {InsL(1,10); Mode(0); return "VI";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )
            {
            if( isTLNDD("BE") )                                {InsLNDD(10); Mode(6); return "VI";}
                                                                   {Mode(0);InsW(10); return "VI";}
            }
          if( (iWrd==0 ||  isTL(1,"PP,OF")) && isMode(TMod.Infinitivo) &&  (isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne"))) 
                                                                            {Mode(0); return "VI";}
          if( iWrd==0 && isMode(TMod.Infinitivo) )                                    return "VI";
                                                                                      return "VI";
        case "P2VR" :
          if( isMode(TMod.Gerundio) )                   return "VG";
          if( isMode(TMod.Participio) )                 return "VP";
          if( isWLNDD("te,me,le,lo"))       {DelLNDD(); return "VR";}
          if( iWrd==2  && isMode(TMod.Infinitivo) &&  ( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne"))) 
                                              {Mode(5); return "VR";}
                                                        return "VR";
        case "P2VT" :
          if( isMode(TMod.Gerundio) )                                                   return "VG";
          if( isMode(TMod.Participio) )                                                 return "VP";
          if( isWLNDD("permitir"))                                            {InsW(7); return "VT";}
          if( isWL(1,"si,quando") || isTLNDD("GZ,CC") || iWrd==0)             {InsW(9); return "VT";}
          if( isTime(TTime.Futuro) && isTL(1,"DN"))              {InsL(1,10); Mode(0); return "VT";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )
            {
            if( isTLNDD("BE") )                                  {InsLNDD(10); Mode(6); return "VT";} 
                                                                    {InsW(10); Mode(0); return "VT";}
            }
          if( (iWrd==0 ||  isTL(1,"PP,OF") ) &&  isMode(TMod.Infinitivo) && (isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne")))
                                                                              {Mode(0); return "VT";}
          if( iWrd==0 && isMode(TMod.Infinitivo) )                                      return "VT";
                                                                                        return "VT";
        case "P2VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,FS,XJ") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                                    return "VI";
            }
          if (isTNNDD("RD,RI,AI,SS,NP,SP,ZA,AA,FS,XJ,IA,OC") || isWNNDD("che") )    return "VT";
                                                                                    return "VI";
        case "P2VZ": Mode(3); return "VA";
        case "P3BE" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )                    {InsW(0); Mode(2); return "BE";}
          if( (isTime(TTime.Futuro) || (isTimeN(1,TTime.Futuro) && isModeN(1,TMod.Participio))) && isTL(1,"DN") ) {InsL(1,10); Mode(0); return "BE";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )                                                               {InsW(10); Mode(0); return "BE";} 
                                                                                                                                          return "BE";
        case "P3BL" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";                        
                                                                              return "VI";                                                                                                            
        case "P3BO" :                                                                                                             
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";                        
                                                                              return "VI";                                                                                                            
        case "P3BR" :                                                                                                             
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";                                                                                                            
        case "P3BX" :                                                                                                             
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";                                                                                                            
        case "P3BY" :                                                                                                             
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";                        
                                                                              return "VI";                                                                                                            
        case "P3BZ" :                                                                                                             
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P3VA" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) ) {InsW(0); Mode(2); return "VA";}
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") ) 
                                                                                                             {InsW(0); return "VA";}
                                                                                                                       return "VA";
        case "P3VG" :
          if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,AA,FS,XJ") || isWNNDD("che") ) return "GT";
                                                                           return "GI";
        case "P3VI" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VI";}
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne") ) 
                                                                                                            {InsW(0); return "VI";}
                                                                                                                      return "VI";
        case "P3VK" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P3VL" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,FS") || isWNNDD("che,a,ad,per") ) return "VT";
                                                                                return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS") || isWNNDD("che") )      return "VT";
                                                                                return "VI";
        case "P3VM" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P3VN" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,FS") || isWNNDD("che,a,ad,per") )   return "VT";
                                                                                  return "VI";
            } 
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OC,IA" ) || isWNNDD("che") ) return "VT";
                                                                                  return "VI";
        case "P3VO" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("che") )       return "VT";
                                                                              return "VI";
        case "P3VT" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) ) {InsW(0); Mode(2); return "VT";}
          if( isOneSufix("urre,orre,are,ere,ire,arsi,ersi,irsi,arme,erme,irme,arle,erle,irle,arci,erci,irci,arli,erli,irli,arla,erla,irla,arlo,erlo,irlo,arle,erle,irle,arvi,ervi,irvi,arti,erti,irti,argli,ergli,irgli,arne,erne,irne")) 
                                                                                                             {InsW(0); return "VT";}
                                                                                                                       return "VT";
        case "P3VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,FS,XJ") || isWNNDD("che,a,ad,per") )  return "VT";
                                                                                    return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SP,ZA,AA,FS,XJ,IA,OC") || isWNNDD("che") )    return "VT";
                                                                                    return "VI";
        case "VI" :
          if( isMode(TMod.Gerundio) || isOneSufix("ando,endo") ) {Mode(5); return "VG";}
          if( isOneSufix("ato,uto") || isMode(TMod.Participio) )           return "VP";
                                                                           return "VV";
        case "VR" :
          if( isMode(TMod.Gerundio) || isOneSufix("ando,endo") ) {Mode(5); return "VG";}
          if( isOneSufix("ato,uto") || isMode(TMod.Participio) )           return "VP";
                                                                           return "VV";
        case "VT" :
          if( isMode(TMod.Gerundio) || isOneSufix("ando,endo") ) {Mode(5); return "VG";}
          if( isOneSufix("ato,uto") || isMode(TMod.Participio) )           return "VP";
                                                                           return "VV";
        default: 
          return null;    
        }

      return Tipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia el tipo de la palabra para el idioma Francés</summary>
    private string ChageTypeFr( int iWrd, string Tipo, string Pref )
      {
      switch( Pref + Tipo )
        {
        case "CAE":
        case "CAF":
        case "CAN": return "AI";
        case "CDF": return "DD";
        case "CGI":
        case "CGT": return "VG";
        case "CHI":
        case "CHT": return "VV";
        case "CJI":
        case "CJT": return "VD";
        case "CPI":
        case "CPT": return "VP";
        case "CVI":
        case "CVT": return "VV";
        case "TGI": return "VI";
        case "TGT": return "VT";
        case "THI": return "VI";
        case "THT": return "VT";
        case "TJI": return "VI";
        case "TJT": return "VT";
        case "TPI": return "VI";
        case "TPT": return "VT";
        case "P0VJ":
        case "P0VW":
        case "P0VX": return "VA";
        case "P3VW":
        case "P0VZ": return "VA";
        case "P3BD":
        case "P3BF":
        case "P3BH":
        case "P3BI":
        case "P3BJ":
        case "P3BK":
        case "P3BM":
        case "P3BP":
        case "P3BS": return "BE";
        case "P3HC": 
        case "P3HD": 
        case "P3HF": 
        case "P3HJ": 
        case "P3HM": 
        case "P3HP": 
        case "P3HV": return "HA";
        case "P3VB": 
        case "P3VE": 
        case "P3VF": 
        case "P3VH": 
        case "P3VJ": 
        case "P3VQ": 
        case "P3VU": 
        case "P3VX": 
        case "P3VY": return "VA";
        case "P3VZ": return "VA";
        case "P0DN": return "DN";
        case "P0WL":
        case "P0WM":
        case "P0WN":
        case "P0WP":
        case "P0WQ":
        case "P0WR":
        case "P0WS":
        case "P0WT":
        case "P0WV":
        case "P0WW":
        case "P0WX":
        case "P0WY":
        case "P0XB":
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTipo("FW") || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                                              return "AA";
                                                                                                 return "VT";
        case "P0JK": Mode(6); return "VP";
        case "P0JL": Mode(6); return "VP";
        case "P1BG": Mode(5); return "BG";
        case "P1BI": Mode(1); return "BI";
        case "P1HD": Mode(1); return "HD";
        case "P1BS": Mode(4); return "BS";
        case "P1BP": Mode(3); return "BP";
        case "P1BM": Mode(2); return "BM";
        case "P1BN": Mode(6); return "BN";
        case "P1VU": Mode(6); return "VU";
        case "P1HG": Mode(5); return "HG";
        case "P1HJ": Mode(4); return "HJ";
        case "P1HM": Mode(2); return "HM";
        case "P1HN": Mode(6); return "HN";
        case "P1HP": Mode(3); return "HP";
        case "P1VJ": Mode(4); return "VJ";
        case "P1VL": Mode(1); return "VL";
        case "P1VW": Mode(2); return "VW";
        case "P1VX": Mode(1); return "VX";
        case "P1VY": Mode(5); return "VY";
        case "P1VZ": Mode(3); return "VZ";
        case "P2VK": Mode(4); return "VK";
        case "P2VL": Mode(1); return "VL";
        case "P2VM": Mode(3); return "VM";
        case "P2VN": Mode(2); return "VN";
        case "P2VP": Mode(6); return "VP";
        case "P0YH": return (isTLNDD("SS"))?"AA":"BO";
        case "P0YI": return (isTLNDD("SS"))?"AA":"BL";
        case "P0YJ": return (isTLNDD("SS"))?"AA":"BX";
        case "P0YK": return (isTLNDD("SS"))?"AA":"BR";
        case "P0YL": return (isTLNDD("SS"))?"AA":"BZ";
        case "P0YM": return (isTLNDD("SS"))?"AA":"BY";
        case "P0YN": return (isTLNDD("SS"))?"AA":"VM";
        case "P1SP": Plural(); return "SS";
        case "MVP" : Mode(6); return Tipo;
        case "P2VG": Mode(5); return "VG";
        case "FVT" :      
          if( isMode(TMod.SubjuntivoImperativo) ) return "BZ";
          if( isMode(TMod.Imperativo) )           return "VN";
          if( isMode(TMod.Indicativo) )           return "VL";
                                                  return "VT";
        case "MBE":
                if( isOneSufix("re,er,ir") )                                      Mode(0);
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA") )           Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                     Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE")    )                     Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) )   Mode(0);
          else if( isOneSufix("ent,ant") && isTLNDD("BE") )                       Mode(5);
          else if( isWLNDD("à,pour,de") )                                         Mode(0);
          else                                                                    Mode(1);
          break;
        case "P0QK":
        case "P0QJ":
          if( iWrd==0 && isTNNDD("OL,CC") )   return "VT";
          if( isLastW() )                     return "SS";
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          else                                return "VA";
        case "MHA":
          if( isOneSufix("re,er,ir") )                                                  Mode(0);
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA") )                 Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                           Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE") )                              Mode(6);
          else if( isOneSufix("ant,ent") && ( iWord==0 || isWLNDD("à,pour,de")) )       Mode(0);
          else if( isOneSufix("ent,ant") && isTLNDD("BE") )                             Mode(5);
          else if( isWLNDD("à,pour,de") )                                               Mode(0);
          else                                                                          Mode(1);
          break;
        case "MVA":
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )
            { Mode(2); InsW(0); return "VT";}
          else if( isOneSufix("re,er,ir") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); else Mode(0);
            }
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA,TO") )        Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                     Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE"   ) )                     Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) )   Mode(0);
          else if( isOneSufix("ant,ent") && isTLNDD("BE") )                       Mode(5);
          else if( isWLNDD("à,pour,de") )                                         Mode(0);
          else                                                                    Mode(1);
          break;
        case "MVI":
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )
            { Mode(2); InsW(0); return "VI";}
          else if( isOneSufix("re,er,ir") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") ) Mode(5); else Mode(0);
            }
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA,TO") )        Mode(0);
          else if( isOneSufix("é,i"    ) && isTNNDD("RD,RI,NP") )                 Mode(1);
          else if( isOneSufix("é,i"    ) && isTLNDD("HA,BE"   )  )                Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) )   Mode(0);
          else if( isOneSufix("ant,ent") && isTLNDD("BE") )                       Mode(5);
          else if( isWLNDD("à,pour,de") )                                         Mode(0);
          else if( isWN(1,"que") || isWN(2,"que") )                               Mode(2);
          else                                                                    Mode(1);
          break;
        case "MVR" :
          if( isOneSufix("re,er,ir") )
            {
            if( iWrd==0 ) Mode(5);
            else          Mode(0);
            }
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA") )          Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                    Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE") )                       Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) )  Mode(0);
          else if( isOneSufix("ant,ent") && isTLNDD("BE") )                      Mode(5);
          else if( isWLNDD("à,pour,de") )                                        Mode(0);
          else                                                                   Mode(1);
          break;
        case "MVT" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VT";}
          if( isOneSufix("re,er,ir") )
            {
            if( iWrd==0 || isTL(1,"PP,OF") )  Mode(5);
            else                              Mode(0);
            }
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA,TO") )      Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                   Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE") )                      Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) ) Mode(0);
          else if( isOneSufix("ant,ent") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("à,pour,de") )                                       Mode(0);
          else                                                                  Mode(1);
          break;
        case "MVV" :
          if( isOneSufix("re,er,ir") )
            {
            if( iWrd==0 )     Mode(5);
            else              Mode(0);
            }
          else if( isWLNDD("devoir,pouvoir,vouloir") || isTLNDD("VA") )         Mode(0);
          else if( isOneSufix("é,i") && isTNNDD("RD,RI,NP") )                   Mode(1);
          else if( isOneSufix("é,i") && isTLNDD("HA,BE") )                      Mode(6);
          else if( isOneSufix("ant,ent") && (iWrd==0 || isWLNDD("à,pour,de")) ) Mode(0);
          else if( isOneSufix("ant,ent") && isTLNDD("BE") )                     Mode(5);
          else if( isWLNDD("à,pour,de") )                                       Mode(0);
          else                                                                  Mode(1);
          break;
        case "NVI" :
          if( isMode(TMod.Participio) ) return "VP";
          if( isOneSufix("é,ée,és,ées,i,ie,is,ies") ) return "VP";
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isOneSufix("ant,ent") ) return "VG";
          return "VI";
        case "NVT" :
          if( isMode(TMod.Participio) ) return "VP";
          if( isOneSufix("é,ée,és,ées,i,ie,is,ies") ) return "VP";
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isOneSufix("ant,ent") ) return "VG";
          return "VT";
        case "NVV" :
          if( isMode(TMod.Participio) ) return "VP";
          if( isOneSufix("é,ée,és,ées,i,ie,is,ies") ) return "VP";
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isOneSufix("ant,ent") ) return "VG";
          return "VV";
        case "P0AO" :
          if( isWLNDD("le,la,les") || isTN(1,"VV,VT,VI,VR,HA,BE") ) return "OO";
          return "AI";
        case "P0AS" :
          if( isFUpper() ) return "NP";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0HA" :
          if( isTLNDD("RD,RI,AD,AI") ) return "SS";
          return "HA";
        case "P0IA" :
          if( isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC";
          if( isTNNDD("AA,AI,AO,XJ,SS,QC") ) return "RD";
          return "OC";
        case "P0IS" :
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          return "VV";
        case "P0MG" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "OG";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "OG";
          if( isTL(1,"SS,XJ,SP") ) return "AF";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AF";
          if( iWrd==0 && isTN(1,"SS") ) return "AF";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AF";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "OG";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AF";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"OG")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "OG";
          return "AF";
        case "P0MU" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "OG";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "OG";
          if( isTL(1,"SS,XJ,SP") ) return "AF";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AF";
          if( iWrd==0 && isTN(1,"SS") ) return "AF";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AF";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "OG";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AF";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"OG")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "OG";
          return "AF";
        case "P0MV" :
          if( isTLNDD("RD,RI,AI,AL") ) return "AN";
          return "NN";
        case "P0OD" :
          if( isTN(1,"AA") ) return "DD";
          if( isTN(1,"SS") ) return "AA";
                             return "OO";
        case "P0OF" :
          if( isSufixNNDD("re") || isSufixNNDD("er") || isSufixNNDD("ir") ) return "TO";
                                                                            return "OF";
        case "P0OJ" :
          if( isWL(1,"¡") && isWN(1,"!") ) return "JJ";
                                           return "OO";
        case "P0OZ" :
          if( isTLNDD("OL") || iWrd==0 ) return "OO";
          if( isTN(1,"AA") )             return "DD";
          if( isTL(1,"SS") )             return "AA";
                                         return "DD";
        case "P0PP" :
          if( isSufixNNDD("re") || isSufixNNDD("ir") || isSufixNNDD("er") ) return "TO";
                                                                            return "PP";
        case "P0QA" :
          if( isOneSufix("e,e,ent,ent") && (isTL(1,"SE,OO") || (iWrd==0 || isTLNDD("SE"))) )  return "VV";
          if( isOneSufix("re,er,ir") && isTL(1,"TO,VA") )                                     return "VV";
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") )                              return "SS";
                                                                                              return "DD";
        case "P0QB" :
          if( isTLNDD("HA") ) return "VP";
          return "XJ";
        case "P0QC" :
          if( isTLNDD("HA") ) return "VP";
          return "XJ";
        case "P0QD" :
          if( isTLNDD("HA") ) return "VP";
          return "XJ";
        case "P0QE" :
          if( (isOneSufix("é,i")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "XM";
          if((isOneSufix("é,i")) ) return "VP";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0QF" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0QG" :
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0QN" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AI";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AI";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AI";
        case "P0QO" :
          if( isTLNDD("BE,SS,AA") ) return "AI";
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS";
          return "DD";
        case "P0QS" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VL";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VL";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0QT" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "BY";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "BY";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0QU" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "BL";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if( (isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "BL";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0RD" :
          if( isWrd("la") && isTNNDD("HA,VV,VT,VI,ZH,ZV,QG,XF") ) return "OC";
          return "RD";
        case "P0SE" :
          if( iWrd==0 && isModeNNDD(TMod.Potencial))                        {Mode(1); return "BE";}
          if( isWN(1,"me,te,le,la,se,nous,vous,les,en") && isTN(2,"VR") )             return "OC";
          if( isWN(1,"me,te,le,la,se,nous,vous,les,en") && !isTN(2,"VR") )  {Mode(1); return "BE";}
          if( isTLNDD("OL,CC") || isTN(1,"VT,VI,VV,TX,ZB,IS,XA,XB,ZE,ZF,ZL,VA,VR,ZV,ZG,ZA,YF,ZH,SV,ZJ") || isWNNDD("doit") ) 
                                                                 {ModeN(1,6);Mode(1); return "BE";} 
                                                                                      return "OR";
        case "P0SV" :
          if( (isOneSufix("e,e,en")) && (isTL(1,"SE,OO") || iWrd==0) )                                    return "VV";
          if((isOneSufix("re,er,ir")) && isTL(1,"TO,VA") )                                                return "VV";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
                                                                                                          return "SS";
        case "P0TA" :
          if( isTLNDD("OO") || iWrd==0 )  return "VV";
          if( isTN(1,"AA") )              return "DD";
                                          return "SS";
        case "P0TO" :
          if( isSufixN(1,"re") || isSufixN(1,"er") || isSufixN(1,"ir") ) {ModeN(1,0); return "TO";} 
                                                                                      return "PP";
        case "P0TX" :
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") )  return "VR";
                                                            return "VV";
        case "P0UA" :
          if( isFUpper() )  return "NP";
                            return "SS";
        case "P0UB" :
          if( isTLNDD("HA") ) return "VP";
                              return "AA";
        case "P0UC" :
          if( isTLNDD("HA") ) return "VP";
                              return "SS";
        case "P0UF" :
          if( iWrd==0 && isTNNDD("OL,CC") )                                                               return "VT";
          if( isModeLW() )                                                                                return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
                                                                                                          return "VT";
        case "P0UG" :
          if( iWrd==0 && isTNNDD("OL,CC") )                                                               return "VT";
          if( isModeLW() )                                                                                return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
                                                                                                          return "VV";
        case "P0UH" :
          if( isTLNDD("HA") ) return "VP";
                              return "AA";
        case "P0UL" :
          if( iWrd==0 && isTNNDD("OL,CC") )                                                               return "VT";
          if( isModeLW() )                                                                                return "SP";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SP";
                                                                                                          return "VV";
        case "P0UO" :
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") )  return "AA";
          if( isTLNDD("RD,RI,AI") )                   return "SS";
          if( iWrd==0 && isModeLW() )                 return "SS";
          if( iWrd==0 && isWN(1,"di") )               return "SS";
          if( isOneSufix("é,i") && !isTLNDD("HA") )   return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )   return "AA";
          if((isOneSufix("é,i")) )                    return "VP";
                                                      return "VV";
        case "P0UR" :
          if( iWrd==0 && isModeLW() )                             return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") )        return "VR";
          if( isTNNDD("OF") && isTL(1,"SS") )                     return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() )           return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )  return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") )                                return "VV";
          if( isTN(1,"AA") )                                      return "DD";
          return "AA";
        case "P0UW" :
          if( isTLNDD("HA") ) return "VP";
                              return "AA";
        case "P0UZ" :
          if( isTLNDD("HA") ) return "VP";
                              return "SS";
        case "P0VI" :
          if( isMode(TMod.Gerundio) )   return "GI";
          if( isMode(TMod.Participio) ) return "PI";
                                        return "VI";
        case "P0VT" :
          if( isMode(TMod.Gerundio) )   return "GT";
          if( isMode(TMod.Participio) ) return "PT";
                                        return "VT";
        case "P0WA" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS";
                                                                                                            return "HD";
        case "P0WB" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS";
                                                                                                            return "BS";
        case "P0WC" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS";
                                                                                                            return "VJ";
        case "P0WD" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") )  return "SS";
                                                                                                            return "VB";
        case "P0WE" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VP";
                                                                        return "DD";
        case "P0WF" :
          if( isTLNDD("SS,NP") ) return "VL";
          return "DD";
        case "P0WH" :
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BL";
          return "DD";
        case "P0WJ" :
          if( isLNDDFW() || isTLNDD("SS,NP") ) return "BY";
          return "DD";
        case "P0WK" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VU";
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0WZ" :
          if( isTL(1,"PP,TO") ) return "OC";
          return "OP";
        case "P0XA" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0XC" :
          if( (isOneSufix("é,i")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "XM";
          if((isOneSufix("é,i")) ) return "VP";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0XD" :
          if( (isOneSufix("é,i")) && !isTLNDD("HA") ) return "XM";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "XM";
          if((isOneSufix("é,i")) ) return "VP";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0XE" :
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || isTNNDD("OF") )  return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") )                               return "AA";
                                                                                  return "VV";
        case "P0XF" :
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
                                                            return "VV";
        case "P0XG" :
          if( Wrd.Entre==2 && isTL(1,"SS,AA") )                                                           return "SS";
          if( isTLNDD("OF,RD,RI,AI") )                                                                    return "SS";
          if( isOneSufix("é,i") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) ) return "VV";
          if( isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG") || isTN(1,"AA,OF")   || iWrd==0 || isTNNDD("OF") ) return "SS";
                                                                                                          return "VV";
        case "P0XH" :
          if( isTLNDD("BE,SS,AA") )       return "AA";
          if( isTLNDD("RD,RI,VG,AI,AD") ) return "SS";
                                          return "DD";
        case "P0XI" :
          if( isTLNDD("SS") ) return "AA";
                              return "VV";
        case "P0XJ" :
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XK" :
          if( isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"AA,VP,XJ") ) return "SS";
          return "DD";
        case "P0XM" :
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") ) return "AA";
          return "DD";
        case "P0XO" :
          if( isFUpper() ) return "NP";
          return "AA";
        case "P0XQ" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          if( isTNNDD("OF") && isTL(1,"SS") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") && isModeLW() ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          if( isTN(1,"RD,RI,AI") ) return "VV";
          if( isTN(1,"AA") ) return "DD";
          return "AA";
        case "P0XS" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XW" :
          if( isTLNDD("RD,RI,AI,AL,NN") && !isTN(1,"SS") ) return "SS";
          if( isTLNDD("RD,RI,AI,AL") && isTNNDD("BE,HA,VV,VT,VI") ) return "SS";
          if( isTL(1,"SS,XJ,SP") ) return "AA";
          if( isTL(2,"SS") && isTL(1,"AA,XJ") ) return "AA";
          if( iWrd==0 && isTN(1,"SS") ) return "AA";
          if( isTNNDD("OF") && isTL(1,"SS,AA") ) return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) ) return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") ) return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
          return "AA";
        case "P0XY" :
          if( isTN(1,"DD,AA") ) return "DD";
          if( isTLNDD("BE,SS,AA,RD,AI,RI") || isTN(1,"SS") ) return "AI";
          return "DD";
        case "P0YA" :
          if( isTLNDD("HA,BN,BE,HN,HG,HJ,HD,HP,HM,BN,BG,BS,BI,BP,BM") ) return "VU";
          return "AA";
        case "P0YB" :
          if( (isTLNDD("RD,RI,AI,AA,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          return "VI";
        case "P0YC" :
          if( isTLNDD("VA,VU,VY,VJ,VX,VZ,VW") ) return "VV";
          return "AA";
        case "P0YD" :
          if( isTLNDD("SS") ) return "AA"; else {Mode(1); return "VV";}
        case "P0YE" :
          if( isTLNDD("SS") ) return "AA"; else {Mode(4); return "VV";}
        case "P0YF" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VT";
          if( isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VT";
        case "P0YG" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,OF,VT,VI,VV,TX,VG,PP") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VR";
        case "P0YP" :
          if( iWrd==0 && isTNNDD("OL,CC") ) return "VK";
          if( isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VK";
        case "P0YQ" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VL";
        case "P0YR" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VN";
        case "P0YS" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BZ";
        case "P0YT" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BL";
        case "P0YV" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BY";
        case "P0YW" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BO";
        case "P0YX" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BX";
        case "P0YZ" :
          if( (isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "BR";
        case "P0ZA" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if( isTLNDD("OF") && isTN(1,"AA,XJ") && isTN(2,"OF") ) return "SS";
          if( iWrd==0 && isWN(1,"la,le") ) return "VV";
          if((isTLNDD("RD,RI,AI,AA,AD,VG,VV,TX,VT,VI") && (!isTN(1,"SS") || isModeLW())) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZB" :
          if( isTLNDD("RD,RI,AI") && isTNNDD("SS") ) return "AA";
          if( isTLNDD("RD,RI,AI") ) return "SS";
          if( iWrd==0 && isModeLW() ) return "SS";
          if( iWrd==0 && isWN(1,"de") ) return "SS";
          if((isOneSufix("é,i")) && !isTLNDD("HA") ) return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if((isOneSufix("é,i")) ) return "VP";
          return "VV";
        case "P0ZE" :
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZF" :
          if( isTLNDD("OF") ) return "SS";
          if( iWrd==0 && isModeLW() ) return "SS";
          if((isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VV";
        case "P0ZG" :
          if( isWNNDD("à,pour,le,la") ) return "VI";
          if(   isOneSufix("é,i") || isMode(TMod.Participio) && (isTL(1,"SS") || (iWrd==0 && isModeLW())) ) return "VI";
          if( isTLNDD("RD,RI,AI,OF,VT,VI,VV,TX,AD,VG")  || isTN(1,"AA,OF") || iWrd==0 || isTNNDD("OF") ) return "SS";
          return "VI";
        case "P0ZH" :
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZI" :
          if( (isOneSufix("é,i")) && !isTLNDD("HA") ) return "AA";
          if( isTLNDD("BE,SS,AA") || isTN(1,"SS") ) return "AA";
          if((isOneSufix("é,i")) ) return "VP";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZJ" :
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO,AA") ) return "SS";
          if( isSufix("ios") ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZK" :
          if( (isTLNDD("RD,RI,AI,AD,VG") && !isTN(1,"SS")) || iWrd==0 || isTNNDD("OF") ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VI";
        case "P0ZV" :
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P0ZZ" :
          if( isTL(1,"PP,OF") && isTN(1,"PP,OF,TO") ) return "SS";
          if( isSufix("ios") ) return "SS";
          if((isTLNDD("RD,RI,AI,AA,AD,OF,VT,VI,VV,TX,VG") && !isTN(1,"SS")) || isTNNDD("OF") || iWrd==0 && isTN(1,"AA") ) return "SS";
          if( isWLNDD("me,te,le,la,se,nous,vous,les,en") ) return "VR";
          return "VV";
        case "P1BD" :
          if( isLeft("que")) {Mode(4); return "BS";}
          {Mode(1); return "BI";}
        case "P1BE" :
          if( isMode(TMod.Infinitivo) && iWrd==0) {Mode(5); return "VG";}
          if( isTLNDD("CC,OL,GZ") || iWrd==0 || (iWrd==1 && isTL(1,"DD,DN"))) {InsW(9); return "BE";}
          return "BE";
        case "P1BF" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BH" :
          if( isLeft("que")) {Mode(4); return "BS";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BJ" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "BN";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(1); return "BI";}
        case "P1BK" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "BM";}
          {Mode(4); return "BS";}
        case "P1BL" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BO" :
          if( isLeft("quee")) {Mode(4); return "VK";}
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VD";}
        case "P1BR" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          {Mode(1); return "VL";}
        case "P1BX" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VP";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BY" :
          if( isLeft("que")) {Mode(4); return "VK";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VN";}
          {Mode(1); return "VL";}
        case "P1BZ" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO,GG")) {Mode(2); return "VN";}
          if( isTLNDD("OC,GG")) {Mode(2); return "VN";}
          {Mode(4); return "VK";}
        case "P1DN" :
          if( iWrd==0 && (isSufixN(1,"re") || isSufixN(1,"er")  || isSufixN(1,"ir")) ) return "DN";
          if((isTNNDD("VV,VI,VT,VZ") && !isTimeNNDD(TTime.Futuro) && isWrd("ne")) || isTLNDD("GN") ) return "DN";
          return "DN";
        case "P1HC" :
          if( isLeft("que")) {Mode(4); return "HJ";}
          {Mode(1); return "HD";}
        case "P1HF" :
          if( isLeft("que")) {Mode(4); return "HJ";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "HM";}
          {Mode(1); return "HD";}
        case "P1HV" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "HM";}
          {Mode(4); return "HJ";}
        case "P1VB" :
          if( isTLNDD("BE,BS,BD,BM,BP,BI,RD,AI,AL,HA")) {Mode(6); return "VU";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VC" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                               return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("que") )        return "VT";
                                                                               return "VI";
        case "P1VE" :
          if( isLeft("que")) {Mode(4); return "VJ";}
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VF" :
          if( isLNDDFW() && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(4); return "VJ";}
        case "P1VH" :
          if( iWrd==0 && isTNNDD("OC,OP,OO")) {Mode(2); return "VW";}
          {Mode(1); return "VX";}
        case "P1VO" :
          if( isLeft("que")) {Mode(4); return "VK";}
          {Mode(1); return "VL";}
        case "P1VQ" :
          if( isLeft("que")) {Mode(4); return "VJ";}
          {Mode(1); return "VX";}
        case "P1VS" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("que,à,pour,qui") )  return "VT";
                                                                                return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ") || isWNNDD("que") )         return "VT";
                                                                                return "VI";
        case "P1VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA") || isWNNDD("que,à,pour,qui") )                       return "VT";
                                                                                                     return "VI";
            } 
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO,VT,VV,VI") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                                     return "VI";
        case "P1XJ" :
          if( iWrd==0 && isTN(1,"SS") )                                               return "AA";
          if( isTNNDD("OF") && isTL(1,"SS") )                                         return "AA";
          if( isTLNDD("PP,TO,RD,RI,AI") &&  ( isModeLW() || isTN(1,"GZ,OF,PP,TO")) )  return "SS";
          if( isTLNDD("AA") && isTNNDD("PP,TO,BE,VA,VV,VT,VI") )                      return "AA";
          if((isTLNDD("RD,RI,AI,AA,AD,VT,OF,VT,VI,VV,TX,VG")    && !isTN(1,"SS")) || iWrd==0 || (isTNNDD("OF") && !isTL(1,"SS")) ) return "SS";
                                                                                      return "AA";
        case "P2BE" :
          if( isWL(1,"si,si dejà,quand") || (isWL(1,"ne") && iWrd==1) || iWrd==0 )  {InsW(9); return "BE";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                        {InsW(9); return "BE";}
          if( isTLNDD("CC") )                                                       {InsW(9); return "BE";}
                                                                                              return "BE";
        case "P2HA" :
          if( isWL(1,"si,si dejà,quand") || isTLNDD("OL,GZ,CC") || (isWL(1,"ne") && iWrd==1) || iWrd==0 ) {InsW(9); return "HA";}
          if( isTL(1,"AA") && isTL(2,"BE") )                                                              {InsW(9); return "HA";}
          if( isTLNDD("CC") )                                                                             {InsW(9); return "HA";}
                                                                                                                    return "HA";
        case "P2SS" : return "SS";
        case "P2VA" :
          if( isWL(1,"si,si dejà,quand") || (isWL(1,"ne") && iWrd==1) || iWrd==0) {InsW(9); return "VA";}
          if( isTL(1,"AA") && isTL(2,"BE")) {InsW(9); return "VA";}
          if( isTLNDD("CC,OL,GZ")) {InsW(9); return "VA";}
          return "VA";
        case "P2VI" :
          if( isMode(TMod.Gerundio) ) return "VG";
          if( isMode(TMod.Participio) ) return "VP";
          if( isWLNDD("permitir,permite")) {InsW(7); return "VT";}
          if( isWL(1,"si,si dejà,quand") || isTLNDD("OL,GZ,CC") || iWrd==0)            {InsW(9); return "VT";}
          if( isTime(TTime.Futuro) && isTL(1,"DN") )                       {InsL(1,10); Mode(0); return "VI";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )
            {
            if( isTLNDD("BE") ) {InsLNDD(10); Mode(6); return "VI";}
                                   {InsW(10); Mode(0); return "VI";}
            }
          if( (iWrd==0 || isTL(1,"PP,OF")) && isMode(TMod.Infinitivo) && ( isOneSufix("ar,er,ir,arnos,irnos,ernos,arme,irme,erme,arse,erse,irse")) ) 
                                             {Mode(5); return "VI";}
          if( iWrd==0 && isMode(TMod.Infinitivo) )     return "VI";
                                                       return "VI";
        case "P2VR" :
          if( isMode(TMod.Gerundio) )                                                      return "VG";
          if( isMode(TMod.Participio) )                                                    return "VP";
          if( isWLNDD("me,te,le,la") )                                         {DelLNDD(); return "VR";}
          if( iWrd==2  && isMode(TMod.Infinitivo) && ( isOneSufix("re,er,ir")) ) {Mode(5); return "VR";}
                                                                                           return "VR";
        case "P2VT" :
          if( isMode(TMod.Gerundio) )                                                                             return "VG";
          if( isMode(TMod.Participio) )                                                                           return "VP";
          if( isWLNDD("permettre")) {InsW(7);                                                                     return "VT";}
          if( isWL(1,"si,si dejà,quand") || isTLNDD("OL,GZ,CC") || iWrd==0)                             {InsW(9); return "VT";}
          if( isTime(TTime.Futuro) && isTL(1,"DN"))                                        {InsL(1,10); Mode(0); return "VT";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )
            {
            if( isTLNDD("BE") )                                                            {InsLNDD(10); Mode(6); return "VT";}
                                                                                              {InsW(10); Mode(0); return "VT";}
            }
          if( (iWrd==0 ||  isTL(1,"PP,OF") ) &&  isMode(TMod.Infinitivo) && ( isOneSufix("re,er,ir")) ) {Mode(5); return "VT";}
          if( iWrd==0 && isMode(TMod.Infinitivo) )                                                                return "VT";
                                                                                                                  return "VT";
        case "P2VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO") || isWNNDD("que") )              return "VT";
                                                                                              return "VI";
        case "P3BE" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo) )                    {InsW(0); Mode(2); return "BE";}
          if( (isTime(TTime.Futuro) || (isTimeN(1,TTime.Futuro) && isModeN(1,TMod.Participio))) && isTL(1,"DN") ) {InsL(1,10); Mode(0); return "BE";}
          if( isTime(TTime.Futuro) && !isTLNDD("CC") )                                                               {InsW(10); Mode(0); return "BE";}
                                                                                                                                          return "BE";
        case "P3BL" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                   
                                                                                              return "VI";                                                                                                                
        case "P3BO" :                                                                                                                 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                   
                                                                                              return "VI";                                                                                                                
        case "P3BR" :                                                                                                                 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                   
                                                                                              return "VI";                                                                                                                
        case "P3BX" :                                                                                                                 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                   
                                                                                              return "VI";                                                                                                                
        case "P3BY" :                                                                                                                 
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";
                                                                                              return "VI";
        case "P3BZ" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,a,por") )  return "VT";
                                                                                          return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )          return "BZ";
                                                                                          return "VI";
        case "P3VA" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VA";} 
          if( isOneSufix("re,er,ir"))                                                                       {InsW(0); return "VA";}
                                                                                                                      return "VA";
        case "P3VI" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VI";}
          if( isOneSufix("re,er,ir") )                                                                      {InsW(0); return "VI";}
                                                                                                                      return "VI";
        case "P3VK" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if(isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )               return "VT";                     
                                                                                              return "VI";                                                                                                                  
        case "P3VL" :                                                                                                                   
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                     
                                                                                              return "VI";                                                                                                                  
        case "P3VM" :                                                                                                                   
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if (isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                     
                                                                                              return "VI";                                                                                                                  
        case "P3VN" :                                                                                                                   
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if(isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )               return "VT";                     
                                                                                              return "VI";                                                                                                                  
        case "P3VO" :                                                                                                                   
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que") )              return "VT";                   
                                                                                              return "VI";
        case "P3VT" :
          if( ((iWrd==0 || isTLNDD("GZ")) && !isMode(TMod.Imperativo)) || isMode(TMod.Imperativo)) {InsW(0); Mode(2); return "VT";}
          if( isOneSufix("re,er,ir") )                                                                      {InsW(0); return "VT";}
                                                                                                                      return "VT";
        case "P3VV" :
          if( isTLNDD("BE") && isMode(TMod.Infinitivo) )
            {
            InsW(7);
            if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,FS,OP,OO") || isWNNDD("que,à,pour,qui") ) return "VT";
                                                                                              return "VI";
            }
          if( isTNNDD("RD,RI,AI,SS,NP,SS,ZA,AA,XJ,SP,FS,OO") || isWNNDD("que") )              return "VT";
                                                                                              return "VI";
        case "VI" :
          if( isMode(TMod.Gerundio)) {Mode(5); return "VG";}
          if( isOneSufix("é,i") || isMode(TMod.Participio) ) return "VP";
          return "VV";
        case "VR" :
          if( isMode(TMod.Gerundio)) {Mode(5); return "VG";}
          if( isOneSufix("é,i") || isMode(TMod.Participio) ) return "VP";
          return "VV";
        case "VT" :
          if( isMode(TMod.SubjuntivoImperativo) ) return "BZ";
          if( isMode(TMod.Gerundio)) {Mode(5); return "VG";}
          if( isOneSufix("é,i") || isMode(TMod.Participio) ) return "VP";
          return "VV";

        default: 
          return null;    
        }

      return Tipo;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia el tipo de la palabra para el idioma Alemán</summary>
    private string ChageTypeDe( int iWrd, string Tipo, string Pref )
      {
      return Tipo;
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de clase        ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin de namespace    ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                                      
