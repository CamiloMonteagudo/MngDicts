using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;

namespace TrdEngine.TrdProcess
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa etapas estandar del proceso de traducción</summary>
  static class StepsStd
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Elimina las contracciones de las palabras de la oración</summary>
    static public void KillContractions( Translate Trd )
      {
      var Sent = Trd.Ora;                                             // Obtiene objeto con datos de la oración
      var Orig = Sent.Orig;                                           // Obtiene oración original

      int Ini = 0;
      while( Ini<Orig.Length && Orig[Ini]=='×'  ) ++Ini;              // Salta marcas de sustitución al inicio de la oración

      int Fin = Orig.Length-1;
      while( Fin>Ini && Orig[Fin]=='×' ) --Fin;                       // Salta marcas de sustitución al final de la oración

      if( Fin != Orig.Length-1 )                                      // Si hubo marcas al final de la oración
        {
        Sent.RtfFin = Orig.Substring( Fin );                          // Las guarda en variable RtfFin
        Orig        = Orig.Substring(0,Fin);                          // Quita las marcas del final
        }

      if( Ini != 0 )                                                  // Si hubo marcas al inicio de la oración
        {
        Sent.RtfIni = Orig.Substring( 0, Ini );                       // Las guada en la varible RtfIni
        Orig        = Orig.Substring(Ini);                            // Quita las marcas del inicio
        }

      Orig = ChangeNTMarks( Orig, Trd.DUser.NtMarkIni, Trd.DUser.NtMarkEnd );   // Cambia las marcas de no traducción

      var ConstREs = Trd.LangSrc.Contractions;                        // Obtiene arreglo de ReExp para contracciones

      Sent.Orig = ConstREs.Replace( Orig );                           // Quita contracciones en toda la oración
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Cambia las marcas de no traducción del usuario por las marcas internas de Idiomax</summary>
    static private string ChangeNTMarks( string Ora, string Mark1, string Mark2 )
      {
      for( int start=0;;)                                             // Repite el proceso la veces que sean necesario
        {
        string Inicio, Medio, Final; 

        var Ini = Ora.IndexOf( Mark1, start );                        // Busca marca inicial de no traducción
        if( Ini == -1 ) return Ora;                                   // No la encontró termina

        var Fin = Ora.IndexOf( Mark2, Ini + Mark1.Length );           // Busca la marca final de no traducción
        if( Fin != -1 )                                               // Si la encontró
          {
          Inicio = Ora.Substring( 0, Ini );                           // Obtiene parte de la oración hasta la primera marca
          Ini   += Mark1.Length;                                      // Se salta la marca inicial
          Medio  = Ora.Substring( Ini, (Fin-Ini) );                   // Obtiene parte de la oración marcada
          Final  = Ora.Substring( Fin + Mark2.Length );               // Obtiene parte de la oración después de la marca final
          }
        else
          {
          Inicio = Ora.Substring( 0, Ini );                           // Obtiene parte de la oración hasta la primera marca
          Medio  = Ora.Substring( Ini + Mark2.Length );               // Considera parte marcada hasta el final de la oración
          Final  = "";                                                // Pone parte final vacia
          }

        Ora = Inicio + TConst.IniMark + Medio + TConst.EndMark + Final;   // Crea la oración con las marcas nuevas
        if( Final.Length==0 ) return Ora;                             // Si no hay parte final termina

        start = Ora.Length - Final.Length;                            // Continua buscando en la parte final
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Divide la oración en palabras</summary>
    static public void ParseWordsStep( Translate Trd)
      {
      ParseWords.Process( Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las propiedades de la oración, en relación a las mayusculas y minusculas</summary>
    static public void GetCaseProperty( Translate Trd)
      {
      Trd.Ora.CheckInterrogative();                                   // Chequea si la oracion es interrogativa
      Trd.Ora.CheckAllWordInUpperCase();                              // Chequea si todas las palabras estan en mayusculas
      Trd.Ora.CheckFirstLetterInFirstWordUpperCase();                 // Chequea si la primeara palabra de la oración es mayusculas
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca las palabras de la oración en el diccionario</summary>
    static public void FindWordsInDict( Translate Trd)
      {
      var Find = new FindWordsInDict( Trd );
      Find.Process();

      if( Find.WordCount > 0 )
        {
        int porciento = (Find.NotFoundCount*100)/Find.WordCount;
        if( (porciento >50 && Find.WordCount>6 ) || 
            (porciento>=60 && Find.WordCount>=5) || 
            (porciento>=65 && Find.WordCount<6 && Find.WordCount>= 3) || 
            (porciento==100) )
          {
          // La oracion esta en un idioma que no es el original
          Trd.Ora.Dest = Trd.Ora.Orig;
          return;
          }
        }

      Trd.Ora.CheckAllWordFirstInUpperCase(); 

      if( Trd.LangSrc.Lang == TLng.De ) Find.FindVerboSeparableDe();

      Trd.Ora.PutMoodAndTimeInGramType();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene tipo gramatical si existe lenguaje de diccionario en el tipo gramatical de la palabra en el diccionario</summary>
    static public void GetTypeOfWord( Translate Trd)
      {
      if( Trd.Ora.sAllTypes != "FO " ) Trd.GetWordType();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca frases idiomáticas dentro de la oración</summary>
    static public void FindPhrases( Translate Trd)
      {
      FindPhrase.Process(Trd);

      ChangeWordType.Process( Trd.Ora.Words, Trd, "F" );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca el tipo gramatical de la palabra mediante comodines</summary>
    static public void GetTypeWordByWildCard( Translate Trd)
      {
      var Sent = Trd.Ora;
      if( Sent.sAllTypes == "FO " ) return;

      if( Sent.EntreUnaVez )
        {
        if( Sent.AllTypeSimple         ) return;
        if( Sent.CheckAllTypesSimple() ) return;
        }

      Sent.EntreUnaVez = true;

      Sent.PutMoodAndTimeInGramType();

      if( Sent.Interrogative )
        {
        for(int i = 0; i < 3; i++)
          {
          ExecRules.Process( Trd, DLangId.QGrFun );
          if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
          }
        }

      for(int i = 0; i < 3; i++)
        {
        ExecRules.Process( Trd, DLangId.SGrFun, true,  false );
        ExecRules.Process( Trd, DLangId.SGrFun, false, true  );

        if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
        }

      Sent.FillWordAndType();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca nombres propios mediante comodines y frases idiomáticas dentro del dcc de nombres propios</summary>
    static public void FindProperNoun( Translate Trd)
      {
      FindPhraseInProperNoun.Process( Trd );

      if( Trd.LangSrc.Lang != TLng.En )
        Trd.Ora.ConvertVerbInNoun();

      ExecRules.Process( Trd, DLangId.PrNoun, true,  false );
      ExecRules.Process( Trd, DLangId.PrNoun, false, true  );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce mediante el uso de comodines</summary>
    static public void TranslationWildCard( Translate Trd)
      {
      var Sent = Trd.Ora;
      if( Sent.sAllTypes == "FO " ) return;

      ChangeWordType.Process( Trd.Ora.Words, Trd, "F" );

	    // Contiene las reglas que deben pasar primero con Sustantivo + numero y las frases con guion
	    for( int i=0; i<4; ++i )
		    {
        ExecRules.Process( Trd, DDirId.ICT );
        if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
		    }

      // Traducción de frases sustantivas
	    for( int i=0; i<4; ++i )
		    {
        ExecRules.Process( Trd, DDirId.FCT );
        if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
        }

	    for( int i=0; i<4; ++i )
		    {
        ExecRules.Process( Trd, DDirId.VCT );
        if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
        }

      if( Trd.Ora.Interrogative )
        {
        // Reglas de traducción para oraciones interrogativas
	      for( int i=0; i<2; ++i )
		      {
          ExecRules.Process( Trd, DDirId.QCT );
          if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
          }
        }

	    for( int i=0; i<4; ++i )    
		    {
        ExecRules.Process( Trd, DDirId.CT );
        if( Sent.TestIsAnyDifferent( ref Sent.sAllTypes ) ) break;
        }

      ExecRules.Process( Trd, DLangId.SGrFun );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca el tipo gramatical utilizando el lenguaje de diccionario</summary>
    static public void FindWordType( Translate Trd)
      {
      var Sent = Trd.Ora;
      Sent.sAllTypes = Sent.GetAllTypes(); 

      Trd.WordTypeByLang();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce todas las palabras de la oración</summary>
    static public void TranslateAllWords( Translate Trd)
      {
      TranslateWords.Process( Trd ); 
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traduce los verbos que tiene dos formas tales como ser o estar, essere o avere, etc</summary>
    static public void TranslateBe( Translate Trd)
      {
      Trd.TranslateTwoTranslationVerb();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Inserta palabras dentro de la oración</summary>
    static public void InsertArticle( Translate Trd)
      {
      Trd.InsertOrAddWord();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca las palabras insertadas en el diccionario y llena sus datos</summary>
    static public void FillInsertWords( Translate Trd)
      {
      // OJO - No hace nada en la versión de Fran
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Concuerda en genero y número las palabras insertadas en la oración</summary>
    static public void GenderAndNumber( Translate Trd)
      {
      Trd.SetGenderAndNumber();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Traducción de prefijos</summary>
    static public void TranslatePreffix( Translate Trd)
      {
      Trd.TranslatePreffix();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Conjuga los verbos que no pudieron ser conjugados mediante comodines</summary>
    static public void Conjugate( Translate Trd)
      {
      Trd.Ora.ConjugateVerb( Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Concuerda en genero y número las palabras que no pudieron ser traducidas mediante comodines</summary>
    static public void Ending( Translate Trd)
      {
      Trd.Ora.MakeConcordance( Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la terminación a los verbos refléxivos</summary>
    static public void TranslateReflexiveVerbs( Translate Trd)
      {
      Trd.Ora.PutReflexivePronoun( Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Construye la oración traducida</summary>
    static public void MakeTranslatedSentence( Translate Trd)
      {
      Trd.Ora.MakeTranslatedSentence( Trd );
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone detalles del idioma tales como contracciones, etc. en la oración traducida</summary>
    static public void Details( Translate Trd)
      {
      var Sent = Trd.Ora;
      var sTrd = Trd.LangDes.Details.Replace( Sent.Dest );

      Sent.Dest = Sent.RtfIni + sTrd + Sent.RtfFin;

	    if( Sent.AllUpper )
        Sent.Dest = Sent.Dest.ToUpper();
      }

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE StepsStd     +++++++++++++++++++++++++++++++++++++++++++++
  }   // ++++++++++++++++++++++++++++++++++++    FIN DEL NAMESPACE            +++++++++++++++++++++++++++++++++++++++++++++
