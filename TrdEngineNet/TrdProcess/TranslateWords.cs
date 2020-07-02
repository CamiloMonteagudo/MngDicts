using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Dictionary;

namespace TrdEngine.TrdProcess
  {
  public class TranslateWords
    {
    Translate Trd = null;                                   // Datos de la traducción
    List<Word> Words;                                       // Lista de palabras a traducir

    int ini = 0;                                            // Indide de la palabra de la lista donde comienza la traducción
    int fin = -1;                                           // Indide de la palabra de la lista donde termina la traducción

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase con los datos de la traduccón</summary>
    private TranslateWords( Translate Trd  )
      {
      this.Trd = Trd;                                                 // Pone referencia a los datos de traducción
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce la lista de palabras dadas</summary>
    public static void Process( Translate Trd, List<Word> words=null, int Ini=0, int End=-1 )
      {
      var TrdWrds = new TranslateWords( Trd );                        // Crea el objeto

      if( words==null ) words = Trd.Ora.Words;                        // Si no se pone palabras, toma la oración actual

      TrdWrds.Words = words;                                          // Asigna lista de palabras a traducir
      int MaxEnd    = words.Count-1;                                  // Determina indice de la ultima palabra de la lista
      TrdWrds.ini   = (Ini<0)? 0 : Ini;                               // Valida indice de la primera palabra a traducir
      TrdWrds.fin   = (End<0 || End>MaxEnd)? MaxEnd : End;            // Valida indice de la ultima palabra a traducir

      ChangeWordType.Process( words, Trd, "T" );                      // Cambia temporalmente tipos gramaticales
      TrdWrds.Process();                                              // Reliza proceso de traducción de palabras
      ChangeWordType.Restore( words );                                // Restaura los tipos gramaticales
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Proceso para encontrar la traducción de un grupo de palabras</summary>
    private void Process()
      {
      for( int i=ini; i<=fin; i++ )                                   // Recorre todas las palabras a traducir
        {
        var Wrd = Words[i];                                           // Toma la palabra actual
        if( Wrd.Traducida || Wrd.Trad.Length>0 || Wrd.NoBusca )       // Si ya esta traducida o no hay que traducirla
            continue;                                                 // Continua con la proxima palabra

        bool bPosesivo = true;                                        // Bandera para saber si un tipo posesivo
        
             if( Wrd.sTipo == "GV" ) Wrd.sTipo = "NP";                // Convierte los tipos posecivos
        else if( Wrd.sTipo == "GW" ) Wrd.sTipo = "SS";
        else if( Wrd.sTipo == "GB" ) Wrd.sTipo = "ST";
        else     bPosesivo = false;

        Trd.ExecLengDict( i, Words );                                 // Obtiene traducción por defecto y acepciones
          
        if( !FindSpecialtyTranslation( Wrd ) )                        // Busca traducción según la especialidad del texto
          if( !FindSemanticTranslation( Wrd )  )                      // Busca traducción según información sementica
              Wrd.StrReserva1 = Wrd.Trad ;
        
        Wrd.Traducida = true;                                         // Pone bandera de que la palabra ya esta traducida
        
        if( bPosesivo )                                               // Si esta la bandera de posesivo
          {
               if( Wrd.sTipo == "NP" ) Wrd.sTipo = "GV";              // Restaura los tipos convertidos anteriormente
          else if( Wrd.sTipo == "SS" ) Wrd.sTipo = "GW";
          else if( Wrd.sTipo == "ST" ) Wrd.sTipo = "GB";
          }
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una traducción de acuerdo a las especilidades especificadas para la traducción</summary>
    private bool FindSpecialtyTranslation( Word Wrd )
      {
      if( Trd.DUser.TxtEsps.Length == 0 ) return false;               // Si no hay especialidades definidas no hace nada

      foreach( var esp in Trd.DUser.TxtEsps)                          // Recorre todas las especialidades
        {
        foreach( var acep in Wrd.AcepArray )                          // Recorre todas las acepciones
          {
          if( acep.Esp == esp )                                       // Si acepción es de la especialidad actual
            {
            Wrd.StrReserva1 = Wrd.Trad = acep.Mean;                   // Toma significado de la acepción
            if( !Wrd.Femenino && Wrd.wGenero!=TGen.Femen )          // Si no se puso femenino anteriormente
              Wrd.wGenero   = acep.Gen;                               // Toma genero del significado

            if( !Wrd.Plural && Wrd.wNumero!=TNum.Plural )           // Si no se puso el prural anteriormente
              Wrd.wNumero   = acep.Num;                               // Toma número del significado

            Wrd.Reflexivo   = acep.Refl;                              // Toma si es reflexivo de la acepción

            return true;                                              // Retorna que se tomo traducción por la especialidad
            }
          }
        }   

      return false;                                                   // No se pudo tomar traducción por la especialidad
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una traducción de acuerdo a la semantica especificada en el diccionario</summary>
    private bool FindSemanticTranslation( Word Wrd )
      {
      foreach( var acep in Wrd.AcepArray )                            // Recorre todas las acepciones de la palabra
        {
        if( acep.Info.Length == 0 ) continue;                         // No tiene información semantica, continua con proxima acepción

        var SemWrds = acep.Info.Split(';');                           // Divide información semantica en palabras

        if( MatchSemanticWords( SemWrds ) )                           // Si alguna palabra de la oración casa con la semantica
          {
          Wrd.StrReserva1 = Wrd.Trad = acep.Mean;                     // Toma significado de la acepción
          if( !Wrd.Femenino && Wrd.wGenero!=TGen.Femen )            // Si no se puso femenino anteriormente
            Wrd.wGenero   = acep.Gen;                                 // Toma genero del significado
  
          if( !Wrd.Plural && Wrd.wNumero!=TNum.Plural )             // Si no se puso el prural anteriormente
            Wrd.wNumero   = acep.Num;                                 // Toma número del significado

          Wrd.Reflexivo   = acep.Refl;                                // Toma si es reflexivo de la acepción

          return true;                                                // Retorna que se tomo traducción por la semantica
          }
        }

      return false;                                                   // No se pudo tomar traducción por semantica
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca si alguna de las palabras de la oración contiene alguna de palabras de 'SemWrds'</summary>
    private bool MatchSemanticWords( IEnumerable<string> SemWrds )
      {
      foreach( var semWrd in SemWrds )                                // Recorre todas las palabras de la semantica
        {
        foreach( var nowWrd in Trd.Ora.Words )                        // Busca en las palabras de la oración
          if( nowWrd.Orig==semWrd || nowWrd.Origlwr==semWrd || nowWrd.Key==semWrd )
            return true;

        var sWords = Trd.Ora.sWords;                                  // Listado de palabras de la oración
        var sKeys  = Trd.Ora.sKeys;                                   // Listado de llaves de la oración
        for( int j=0; j<sWords.Count && j<sKeys.Count; j++ )          // Busca por listado de palabras y llaves de la oración
          if( sWords[j]==semWrd || sKeys[j]==semWrd )
            return true;
        }

      return false;                                                   // No encontro ninguna coincidencia
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE TranslateWords  +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE TrdEngine     +++++++++++++++++++++++++++++++++++++++++++++
