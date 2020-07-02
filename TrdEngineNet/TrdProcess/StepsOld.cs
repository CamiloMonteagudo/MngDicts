using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
    //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa etapas de traducción que son consideradas viejas</summary>
  static class StepsOld
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Utiliza los comodines para lograr la conjugación y coordinación</summary>
    static public void CmdCoorCnj( Translate Trd)
      {
      var Sent = Trd.Ora;
      if( Sent.sAllTypes == "FO " ) return;

      ChangeWordType.Process( Sent.Words, Trd, "C" );

      var SaveWord = Sent.Words.ToList();                             // Guarda lista original de palabras

      for( int i=0; i<4; i++ )                                        // Repite 4 veces
        FindCmdPhrase.Process( i, Trd );                              // Busca frase por coordinación y conjugación

      Sent.Words = SaveWord;                                          // Restaura las palabras que habian originalmente

      ChangeWordType.Restore( Sent.Words );                           // Restaura los tipos cambiados durante ChangeWordType
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca el modo de los verbo según el lenguaje de diccionario</summary>
    static public void FindMood( Translate Trd)
      {
      if( Trd.Ora.sAllTypes == "FO " ) return;

      ChangeWordType.Process( Trd.Ora.Words, Trd, "C" );
      }

    }
  }
