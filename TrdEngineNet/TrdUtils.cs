using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrdEngine
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Funciones y contantes usadas en la traducción</summary>
  //------------------------------------------------------------------------------------------------------------------
  static public class Utils
    {
#region "Poner e invocar eventos en las etapas de traducción"
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evento que se genera cada vez que se casa una regla</summary>
    static private event HndTraceRgl TraceRglEvent;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Establece un manipulador para atender el evento que se produce al casar una regla</summary>
    static public void SetTraceRgl( HndTraceRgl Hnd ) { TraceRglEvent = Hnd; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Invoca el evento para trazar los datos de una regla</summary>
    static public void TraceRgl( string RglId, List<string> rglData, Translate Trd )
      { if( TraceRglEvent!=null ) TraceRglEvent( RglId, rglData, Trd ); }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna si se establecio un evento para atender la traza de las reglas o no</summary>
    public static bool IsTraceRule { get{ return TraceRglEvent!=null; } }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Evento que se genera al terminar cada etapa de tratucción</summary>
    static private event HndTraceTrd TraceTrdEvent;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Invoca evento de terminación de etapa de traducción</summary>
    static public void SetTraceTrd( HndTraceTrd Hnd )  { TraceTrdEvent = Hnd; }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Invoca evento de terminación de etapa de traducción</summary>
    static public bool TraceTrd( string StepId, Translate Trd )
      {
      if( TraceTrdEvent == null ) return true;

      return TraceTrdEvent( StepId, Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Retorna si se establecio un evento para atender la traza de la traducción o no</summary>
    public static bool IsTraceTrd { get{ return TraceTrdEvent!=null; } }

    //------------------------------------------------------------------------------------------------------------------
#endregion

#region "Manejo de direcciones de traducción"
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una dirección a partir del idioma fuente y el idioma destino</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public TDir ToDir(TLng src, TLng des )
      {
      TDir[,] Lngs  = {//Español  , Ingles   , Italiano , Aleman   , Frances       
      /* Español */   {  TDir.NA  , TDir.EsEn, TDir.EsIt, TDir.NA  , TDir.EsFr },  
      /* Ingles  */   {  TDir.EnEs, TDir.NA  , TDir.EnIt, TDir.NA  , TDir.EnFr },  
      /* Italiano*/   {  TDir.ItEs, TDir.ItEn, TDir.NA  , TDir.ItDe, TDir.ItFr },  
      /* Aleman  */   {  TDir.NA  , TDir.NA  , TDir.DeIt, TDir.NA  , TDir.NA   },  
      /* Frances */   {  TDir.FrEs, TDir.FrEn, TDir.FrIt, TDir.NA  , TDir.NA   } };

      return (src>=0 && (int)src<TConst.Langs && des>=0 && (int)des<TConst.Langs) ? Lngs[ (int)src, (int)des ] : TDir.NA;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una dirección a partir una cadena de caracteres</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public TDir ToDir( string sDir )
      {
      for( int i=0; i<TConst.Dirs; ++i )
        {
        var Dir = (TDir)i;
        if( sDir==Dir.ToString() ) return Dir;
        }

      return TDir.NA;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el idioma origen de una dirección de traducción</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public TLng Src(TDir Dir )
      {
      TLng[] lstSrc = { TLng.Es, TLng.En, TLng.It, TLng.En, TLng.It, TLng.Es, TLng.It, TLng.De, TLng.It, TLng.Fr, TLng.En, TLng.Fr, TLng.Es, TLng.Fr };

      return (Dir>=0 && (int)Dir<TConst.Dirs) ? lstSrc[(int)Dir] : TLng.NA;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el idioma destino de una dirección de traducción</summary>
    //------------------------------------------------------------------------------------------------------------------
    static public TLng Des(TDir Dir )
      {
      TLng[] lstDes = { TLng.En, TLng.Es, TLng.En, TLng.It, TLng.Es, TLng.It, TLng.De, TLng.It, TLng.Fr, TLng.It, TLng.Fr, TLng.En, TLng.Fr, TLng.Es };

      return (Dir>=0 && (int)Dir<TConst.Dirs) ? lstDes[(int)Dir] : TLng.NA;
      }
    //------------------------------------------------------------------------------------------------------------------
#endregion

#region "Manejo de idiomas"
    //------------------------------------------------------------------------------------------------------------------
    static internal string[]  Codes = {"Es","En","It","De","Fr"};
    static internal string[,] Names = {
                                        {"Español" , "Inglés"  , "Italiano"   , "Alemán"  , "Francés"     },
                                        {"Spanish" , "English" , "Italian"    , "German"  , "French"      },
                                        {"Spagnolo", "Inglese" , "Italiano"   , "Tedesco" , "Francese"    },
                                        {"Spanisch", "Englisch", "Italienisch", "Deustch" , "Französisch" },
                                        {"Espagnol", "Anglais" , "Italien"    , "Allemand", "Français"    },
                                      };

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Código ISO de dos letras del idioma</summary>
    ///<param name="L">Código númerico del idioma que se quiere obtener el código ISO</param>
    //------------------------------------------------------------------------------------------------------------------
    static public string Iso( TLng L )
      { 
      return (L>=0 && (int)L<Codes.Length)? Codes[(int)L]: "Xx"; 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Nombre completo del idioma en un idioma de interface</summary>
    ///<param name="Lang" >Código númerico del idioma a obtener</param>
    ///<param name="iUser">Código númerico del idioma de interface para el nombre</param>
    //------------------------------------------------------------------------------------------------------------------
    static public string Name( TLng Lang, TLng iUser )
      {
      int L = (int)Lang;
      int U = (int)iUser;
      return( L>=0 && L<Names.GetLength(0) && U>=0 && U<Names.GetLength(1) )? Names[U,L]: "Unknown";
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Código númerico del idioma según su código ISO de 2 letras</summary>
    ///<param name="Iso">Código Iso de dos letras del idioma que se quiere obtener</param>
    //------------------------------------------------------------------------------------------------------------------
    static public TLng Code( string Iso )
      {
      if( Iso.Length ==2 )                              // El código tiene que ser de dos letras
        {
        char c1 = Iso.ToUpper()[0];                     // Lleva primera letra a mayuscula
        char c2 = Iso.ToLower()[1];                     // Lleva segunda letra a minuscula

        for( int i=0; i<Codes.Length; i++)              // Recorre todos los códigos
          if( c1==Codes[i][0] && c2==Codes[i][1] )      // Es el código solicitado
            return (TLng)i;                             // Retorna el idioma
        }

      return TLng.NA;                                   // Retorna idioma no disponible
      }
#endregion

#region "Caracteres para encerrar suboraciones"
    //------------------------------------------------------------------------------------------------------------------
    static char[] cEntreQueIni = {' ', '(','"','\'','[' };          // Caracteres iniciales para encerrar oraciones
    static char[] cEntreQueEnd = {' ', ')','"','\'',']' };          // Caracteres finales para encerrar oraciones
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene caracter que hay que poner delante según el valor de entre</summary>
    static public char EntreIni( int entre )  { return cEntreQueIni[entre]; }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene caracter que hay que poner detras según el valor de entre</summary>
    static public char EntreEnd( int entre )  { return cEntreQueEnd[entre]; }

#endregion

    //------------------------------------------------------------------------------------------------------------------
    }
  }
