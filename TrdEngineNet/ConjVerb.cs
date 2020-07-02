using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Dictionary;
using TrdEngine.Data;

namespace TrdEngine
  {
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ///<summary>Conjuga verbos en un idioma determinado y según el modo, el tiempo, la persona y el número</summary>
  public class ConjVerb
    {
    ConjLangData Data;

    ///<summary>Usado para identicar una columna de la tabla de cerbos</summary>
    enum ColId : byte
      {
      // Modo infinitivo
      Infinitivo = 0,                  // 0
      Gerundio,                        // 42
      ParticipioPasado,                // 43
      ParticipioPresente,              // 0

      // Modo indicativo    Tiempo presente  singular
      PresenteIndicativo1PerSing,      // 1
      PresenteIndicativo2PerSing1,     // 3
      PresenteIndicativo2PerSing2,     // 2
      PresenteIndicativo2PerSing3,     // 5
      PresenteIndicativo3PerSing,      // 3

      // Modo indicativo    Tiempo presente  plural
      PresenteIndicativo1PerPlural,    // 4
      PresenteIndicativo2PerPlural1,   // 6
      PresenteIndicativo2PerPlural2,   // 5
      PresenteIndicativo2PerPlural3,   // 5
      PresenteIndicativo3PerPlural,    // 6

      // Modo indicativo    Tiempo pasado  singular
      PasadoIndicativo1PerSing,        // 7
      PasadoIndicativo2PerSing1,       // 9
      PasadoIndicativo2PerSing2,       // 8
      PasadoIndicativo2PerSing3,       // 11
      PasadoIndicativo3PerSing,        // 9

      // Modo indicativo    Tiempo pasado  plural
      PasadoIndicativo1PerPlural,      // 10
      PasadoIndicativo2PerPlural1,     // 12
      PasadoIndicativo2PerPlural2,     // 11
      PasadoIndicativo2PerPlural3,     // 11
      PasadoIndicativo3PerPlural,      // 12


      // Modo indicativo    Tiempo pretérito imperfecto singular
      PasadoImpIndicativo1PerSing,        // 7
      PasadoImpIndicativo2PerSing1,       // 9
      PasadoImpIndicativo2PerSing2,       // 8
      PasadoImpIndicativo2PerSing3,       // 11
      PasadoImpIndicativo3PerSing,        // 9

      // Modo indicativo    Tiempo pretérito imperfecto plural
      PasadoImpIndicativo1PerPlural,      // 10
      PasadoImpIndicativo2PerPlural1,     // 12
      PasadoImpIndicativo2PerPlural2,     // 11
      PasadoImpIndicativo2PerPlural3,     // 11
      PasadoImpIndicativo3PerPlural,      // 12

      // Modo indicativo    Tiempo Futuro  singular
      FuturoIndicativo1PerSing,        // 13
      FuturoIndicativo2PerSing1,       // 15
      FuturoIndicativo2PerSing2,       // 14
      FuturoIndicativo2PerSing3,       // 17
      FuturoIndicativo3PerSing,        // 15

      // Modo indicativo    Tiempo Futuro  plural
      FuturoIndicativo1PerPlural,      // 16
      FuturoIndicativo2PerPlural1,     // 18
      FuturoIndicativo2PerPlural2,     // 17
      FuturoIndicativo2PerPlural3,     // 17
      FuturoIndicativo3PerPlural,      // 18

      // Modo Potencial singular
      Potencial1PerSing,               // 19
      Potencial2PerSing1,              // 21
      Potencial2PerSing2,              // 20
      Potencial2PerSing3,              // 23
      Potencial3PerSing,               // 21

      // Modo Potencial plural
      Potencial1PerPlural,             // 22
      Potencial2PerPlural1,            // 24
      Potencial2PerPlural2,            // 23
      Potencial2PerPlural3,            // 23
      Potencial3PerPlural,             // 24

      // Modo subjuntivo    Tiempo presente  singular
      PresenteSubjuntivo1PerSing,      // 25
      PresenteSubjuntivo2PerSing1,     // 27
      PresenteSubjuntivo2PerSing2,     // 26
      PresenteSubjuntivo2PerSing3,     // 29
      PresenteSubjuntivo3PerSing,      // 27

      // Modo Subjuntivo    Tiempo presente  plural
      PresenteSubjuntivo1PerPlural,    // 28
      PresenteSubjuntivo2PerPlural1,   // 30
      PresenteSubjuntivo2PerPlural2,   // 29
      PresenteSubjuntivo2PerPlural3,   // 29
      PresenteSubjuntivo3PerPlural,    // 30

      // Modo Subjuntivo    Tiempo pasado  singular
      PasadoSubjuntivo1PerSing,        // 31
      PasadoSubjuntivo2PerSing1,       // 33
      PasadoSubjuntivo2PerSing2,       // 32
      PasadoSubjuntivo2PerSing3,       // 35
      PasadoSubjuntivo3PerSing,        // 33

      // Modo Subjuntivo    Tiempo pasado  plural
      PasadoSubjuntivo1PerPlural,      // 34
      PasadoSubjuntivo2PerPlural1,     // 36
      PasadoSubjuntivo2PerPlural2,     // 35
      PasadoSubjuntivo2PerPlural3,     // 35
      PasadoSubjuntivo3PerPlural,      // 36

      // Modo Imperativo singular
      Imperativo2PerSing1,             // 38
      Imperativo2PerSing2,             // 37
      Imperativol2PerSing3,            // 40
      Imperativo3PerSing,              // 38

      // Modo Imperativo plural
      Imperativo1PerPlural1,           // 39
      Imperativo2PerPlural1,           // 41
      Imperativo2PerPlural2,           // 40
      Imperativo2PerPlural3,           // 40
      Imperativo3PerPlural,             // 41

      // Número de patrón de sufijo y prefijo de conjugación
      PatronSufPref
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Esconde el constructor, para que solo pueda obtenerse una instancia a partir de 'GetConj'</summary>
    private ConjVerb(){}

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Abre la conjugación para un idioma dado</summary>
    ///
    ///<param name="lang">Idioma para en el cual se quiere conjugar</param>
    ///<param name="path">Camino donde estan los diccionarios de conjugación, si este parametro es 'null' o vacio, los 
    ///diccionarios se buscan en el directorio 'Dictionaries' relativo al directorio del ensamblado (TrdEngine.dll)</param>
    ///
    ///<returns>Retorna true si se pudo abrir la conjugación en el idioma dado de lo contrario retorna false</returns>
    //------------------------------------------------------------------------------------------------------------------
    public static ConjVerb GetConj(TLng lang, string path=null)
      {
      if( !string.IsNullOrEmpty(path) )                           // Si se especifico un camino para los diccionarios
        ConjData.DicPath = path;                                  // Lo pone para los datos

      var Conj = new ConjVerb();                                  // Crea un objeto nuevo
      Conj.Data = ConjData.GetLang( lang );                       // Obtiene datos para el idioma

      if( Conj.Data==null ) return null;                          // Si no se obtubieron los datos, retorna error
      return Conj;                                                // Retorna el objeto creado
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Libera los datos de la conjugación actual, para que puedan ser recolectados</summary>
    //------------------------------------------------------------------------------------------------------------------
    public void Release()
      {
      ConjData.ReleaseLang( Data.Lang );                          // Libera los datos para ese idioma
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene la conjugación de un verbo, estableciendo todos sus parametros</summary>
    ///
    ///<param name="verb"   >Verbo que se desea conjugar                                </param>
    ///<param name="modo"   >Modo en que se quiere conjuagar el verbo (Ver enum Modo)   </param>
    ///<param name="tiempo" >Tiempo en que se quiere conjugar el verbo (Ver enum Tiempo)</param>
    ///<param name="persona">Persona del verbo conjugado                                </param>
    ///<param name="sigular">Si el verbo se conjuaga en singular o plurar               </param>
    ///<param name="comp"   >Si se conjuga en un tiempo compuesto                       </param>
    ///
    ///<returns>Retorna la conjuación del verbo sagún los parametros solicitados, si la palabra no es un verbo retorna Null</returns>
    //------------------------------------------------------------------------------------------------------------------
    public string Conjugate( string verboi, TMod modo, TTime tiempo, TPer persona, TNum numero, bool negado, bool compuesto)
      {
      string verbo = verboi.ToLower();
      int fila = -1;
      int col = -1;

      string verbprefix, rootverb, verbasic;                     // Buscar si el verbo es separable o no y hallar el prefijo

      if( FindVerbPrefix(verbo, out verbprefix, out verbasic) )
        verbo = verbasic;
      
      string prefreflex = "";                                    // Buscar si el verbo es reflexivo y hallar la raíz

      if( ReflexiveVerbRoot( verbo, out rootverb ) )
        {
        prefreflex = ReflexivePronoun( persona, numero );
        verbo = rootverb;
        }

      string verbsimp = "";

      if( (modo == TMod.Imperativo) && !string.IsNullOrEmpty(prefreflex) )    // Imperativo Reflexivo
        verbsimp = "-";

      if(verbsimp != "-")
        verbsimp = ConjVerbSimple( verbo, modo, tiempo, persona, numero, out fila, out col );

      if( string.IsNullOrEmpty(verbsimp) || verbsimp == "-")                // Hubo error o no existe la conj.
        return verbsimp;                                          
                                                                  
      string prefijo = "",sufijo = "";                            // Prefijo reflexivo según la persona
      bool verboinf   = false;                                    // Si el verbo negado está en infinitivo

      //Si se quiere conjugar en un tiempo simple
      if( (compuesto == false) || (modo == TMod.Imperativo) || (modo == TMod.Infinitivo) || (modo == TMod.Participio) || (modo == TMod.Gerundio))
        {
        Prefijo_Conjugac( fila, col, negado, ref prefijo, ref sufijo, ref verboinf );

        if( verboinf ) verbsimp = verbo;                          // El verbo con afijo va en infinitivo
                                                                  
        if( !string.IsNullOrEmpty(prefreflex) && Data.Lang != TLng.De )       
          verbsimp = prefreflex + " " + verbsimp;                 
                                                                  
        if( prefijo.Length > 0 )                                  
          {                                                       
          if( verbprefix.Length > 0 )                             
            {                                                     
            string prefix = verbprefix;                           
            if( prefix[0] == '.' )                                // Es un verbo separable
              {                                                   
              prefix = prefix.Substring(1);                       
              if(modo == TMod.Participio)                          
                verbsimp = prefix + prefijo + verbsimp;           // ausgearbeite
              else                                                
                {                                                 
                verbsimp = prefijo + prefix + verbsimp;           // ausgearbeite
                }                                                 
              }                                                   
            else                                                  // Es un verbo no separable
              {                                                   
              if(modo == TMod.Participio)                          
                verbsimp = prefix + verbsimp;                     // beratet
              else                                                
                {                                                 
                verbsimp = prefijo + prefix + verbsimp;           // ausgearbeite
                }                                                 
              }                                                   
            }                                                     
          else                                                    
            verbsimp = prefijo + verbsimp;                        
          }                                                       
        else                                                      // Si no tiene prefijo
          {                                                       
          if( verbprefix.Length > 0 )                             
            {                                                     
            string prefix = verbprefix;                           
            if( prefix[0] == '.' )                                // Es un verbo separable
              {                                                   
              prefix = prefix.Substring(1);
              if(modo == TMod.Participio || modo == TMod.Gerundio || modo == TMod.Infinitivo)  
                verbsimp = prefix + verbsimp;                     // ausarbeiten
              else                                                
                {                                                 
                verbsimp = verbsimp + verbprefix;                 // arbeite.aus
                }                                                 
              }                                                   
            else                                                  
              {                                                   
              verbsimp = verbprefix + verbsimp;                   // beraten
              }                                                   
            }                                                     
          }                                                       
                                                                  
        if( sufijo.Length > 0 )                                   
          verbsimp = verbsimp + sufijo;                           
                                                                  
        return verbsimp;                                          
        }                                                         
                                                                  
      string verbaux,verbauxinf;                                  // Si se quiere conjugar en un tiempo compuesto
      int filavaux = -1;                                          
      int colvaux = -1;                                           
                                                                  
      if(verbprefix.Length > 0)                                   // Se busca en el diccionario el verbo auxiliar para tiempos compuestos
        verbo = verboi;                                           
                                                                  
      verbauxinf = Data.FindAuxVerb(verbo);                       
      if(string.IsNullOrEmpty(verbauxinf))                        // verbo aux.
        {                                                         
        string defverb = "1default_auxverb";                      
        verbauxinf = Data.FindAuxVerb(defverb);                   
        if(string.IsNullOrEmpty(verbauxinf))                      
          return null;                                            
        }                                                         
                                                                  
      if (verbauxinf=="-")                                        // No existe verbo auxiliar para tiempos compuestos
        return verbauxinf;

      verbaux = ConjVerbSimple( verbauxinf, modo, tiempo, persona, numero, out filavaux, out colvaux );
      if( verbaux.Length == 0)                                    // Hubo error
        return null;

      Prefijo_Conjugac( filavaux, colvaux, negado, ref prefijo, ref sufijo, ref verboinf);

      if( verboinf )                                              // El verbo con afijo va en infinitivo
        verbaux = verbo;

      if(prefreflex.Length > 0)
        verbaux = prefreflex + " " + verbaux;

      if( prefijo.Length > 0 ) 
        verbaux = prefijo + verbaux;

      if( sufijo.Length > 0 ) 
        verbaux = verbaux + sufijo;

      string participio;
      int ipart = Data.TabColIndex[ (int)ColId.ParticipioPasado ];
      if( ipart > 0 )
        {
        string sinf, ter, rinf;

        if(verbprefix.Length > 0)
          verbo = verbasic;

        var tablav = Data.TableVerb;
        sinf = tablav.Cell( fila, (int)ColId.Infinitivo );    // sufijo del infinit.
        verbo += ' ';                                              // del verbo patrón
        int pos = verbo.IndexOf( sinf+' ' );                      // Se busca suf. inf. en el verbo orig.
        verbo = verbo.TrimEnd();
            
        rinf = verbo.Substring(0, pos);                            // Raíz Infinitivo
        ter = tablav.Cell(fila, ipart);                        // Terminación del verbo conjugado
        
        participio = rinf+ter;                                    // participio

        prefijo = "";
        Prefijo_Conjugac( fila, ipart, negado, ref prefijo, ref sufijo, ref verboinf);

        if( prefijo.Length > 0 )                                  // Si tiene prefijo el participio
          {
          if( verbprefix.Length > 0 ) 
            {
            string prefix = verbprefix;
            if( prefix[0] == '.' )                                // Es un verbo separable
              {
              prefix = prefix.Substring(1);
              participio = prefix + prefijo + participio;         // ausgearbeitet
              }
            else                                                  // Es un verbo no separable
              {
              participio = prefix + participio;                   // beratet (no lleva ge)
              }
            }
          else
            participio = prefijo + participio;
          }
        
        return verbaux + " " + participio;
        }

      return null;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene las conjugaciones para un modo y tiempo dato</summary>
    ///
    ///<param name="verb"   >Verbo que se desea conjugar                                </param>
    ///<param name="modo"   >Modo en que se quiere conjuagar el verbo (Ver enum Modo)   </param>
    ///<param name="tiempo" >Tiempo en que se quiere conjugar el verbo (Ver enum Tiempo)</param>
    ///
    ///<returns>Retorna una lista de 6 conjugaciones perteneciente a las 3 personas, por los 2 números, si la palabra no
    ///es un verbo las conjugaciones serán cadenas vacias</returns>
    //------------------------------------------------------------------------------------------------------------------
    public string[] ConjTime(string verb, TMod modo, TTime tiempo, bool comp)
      {
      string[] cjs = new string[6];

      cjs[0] = Conjugate( verb, modo, tiempo, TPer.Primera, TNum.Singular, false, comp );
      cjs[1] = Conjugate( verb, modo, tiempo, TPer.Segunda, TNum.Singular, false, comp );
      cjs[2] = Conjugate( verb, modo, tiempo, TPer.Tercera, TNum.Singular, false, comp );
                                 
      cjs[3] = Conjugate( verb, modo, tiempo, TPer.Primera, TNum.Plural, false, comp );
      cjs[4] = Conjugate( verb, modo, tiempo, TPer.Segunda, TNum.Plural, false, comp );
      cjs[5] = Conjugate( verb, modo, tiempo, TPer.Tercera, TNum.Plural, false, comp );

      return cjs;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene pronombres personales del idioma actual</summary>
    ///
    ///<returns>Retorna una lista de los 6 pronombres personales, correspondientes a las 3 personas, por los 2 números
    ///</returns>
    //------------------------------------------------------------------------------------------------------------------
    public string[] Pronouns()
      {
      string[] Pns = new string[6];

      Pns[0] = Pronoun(TPer.Primera, TNum.Singular);
      Pns[1] = Pronoun(TPer.Segunda, TNum.Singular);
      Pns[2] = Pronoun(TPer.Tercera, TNum.Singular);

      Pns[3] = Pronoun(TPer.Primera, TNum.Plural);
      Pns[4] = Pronoun(TPer.Segunda, TNum.Plural);
      Pns[5] = Pronoun(TPer.Tercera, TNum.Plural);

      return Pns;
      }

    //----------------------------------------------------------------------------------------------------------------------
    // Conjuga un verbo simple (no compuesto)
    string ConjVerbSimple(string verboi, TMod modo, TTime tiempo, TPer persona, TNum numero, out int nfila, out int ncol)
      {
      nfila = ncol = -1;

      if( modo==TMod.Imperativo && persona==TPer.Primera && numero==TNum.Singular ) // Primera Pers. del Imperativo o reflexivo
        return "-";

      string verbo = verboi;
      verbo = verbo.ToLower();
      int fila = -1;
      int col = -1;

      Tabla tablav = Data.TableVerb;

      var data = Data.FindIrregWord( verbo );                       // Se busca el verbo en el dicc de Pal. Irreg
      if( data!=null )                                              // está
        {                                                           
        fila = tablav.FilaIndex(data);                               // # de fila del verbo patrón en la tabla
        }                                                           
      else                                                          // Se busca el verbo en el dicc de Sufijos  
        {                                                            
        data = Data.Find_Suffix(verbo);                             
        if( data != null )                                          // está
          {                                                         
          fila = tablav.FilaIndex(data);                              // # de fila del verbo patrón en la tabla
          }                                                         
        else                                                        
          {                                                         
          return "";                                                // AfxMessageBox("La palabra no es un verbo");
          }
        }   
           
      string VerboC = "";
      if( fila > -1 )                                                // Se encontró un patrón para el verbo
        {
        string sinf, ter, rinf;
            
        sinf = tablav.Cell( fila, (int)ColId.Infinitivo );      // sufijo del infinit.
        verbo += ' ';                                                // del verbo patrón
        int pos = verbo.IndexOf(sinf + ' ');                        // Se busca suf. inf. en el verbo orig.
        verbo = verbo.TrimEnd();
                
        if (pos != -1)
          {      
          rinf = verbo.Substring(0, pos);                            //  Raíz Infinitivo
          col = IndiceCnj( modo, tiempo, persona, numero );
          ter = tablav.Cell(fila,col);                          // Terminación del verbo conjugado
          
          if( ter == "-" )                                          // Verbo defectivo. No existe esa conjugación
            {
            nfila = fila;
            ncol = col;
            return ter;
            }

          VerboC = rinf + ter;                                      // Verbo conjugado
          }
        else
          {
          return "";                                                // ERROR en tabla de conjugaciones o en diccionario
          }
        }
      else 
        {      
        return "";                                                  // ERROR en diccionarios
        }     

      nfila = fila;
      ncol = col;

      if( verbo == "be" && modo == TMod.Indicativo && Data.Lang == TLng.En && tiempo == TTime.Pasado && persona == TPer.Segunda && numero == TNum.Singular )
        VerboC = "were";

      return VerboC;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el enum correspondiente a la conjugación que se da como parámetro </summary>
    int IndiceCnj( TMod modo, TTime tiempo, TPer persona, TNum numero )
      {                       
      ColId i = ColId.Infinitivo;

      switch( modo )
        {
        case TMod.Infinitivo: i = ColId.Infinitivo;       break;
        case TMod.Participio: i = ColId.ParticipioPasado; break;
        case TMod.Gerundio:   i = ColId.Gerundio;         break;
        case TMod.Indicativo:
          {
          if (numero == TNum.Singular)
            {
            switch (tiempo)
              {
              case TTime.Presente:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PresenteIndicativo1PerSing;  break;
                  case TPer.Segunda: i = ColId.PresenteIndicativo2PerSing1; break;
                  case TPer.Tercera: i = ColId.PresenteIndicativo3PerSing;  break;
                  }
                break;
                }
              case TTime.Pasado:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoIndicativo1PerSing;  break;
                  case TPer.Segunda: i = ColId.PasadoIndicativo2PerSing1; break;
                  case TPer.Tercera: i = ColId.PasadoIndicativo3PerSing;  break;
                  }
                break;
                }
              case TTime.Futuro:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.FuturoIndicativo1PerSing;  break;
                  case TPer.Segunda: i = ColId.FuturoIndicativo2PerSing1; break;
                  case TPer.Tercera: i = ColId.FuturoIndicativo3PerSing;  break;
                  }
                break;
                }
              case TTime.PasadoImp:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoImpIndicativo1PerSing;  break;
                  case TPer.Segunda: i = ColId.PasadoImpIndicativo2PerSing1; break;
                  case TPer.Tercera: i = ColId.PasadoImpIndicativo3PerSing;  break;
                  }
                break;
                }
              }
            }
          else             // Plural del indicativo
            {
            switch (tiempo)
              {
              case TTime.Presente:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PresenteIndicativo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.PresenteIndicativo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.PresenteIndicativo3PerPlural;  break;
                  }
                break;
                }
              case TTime.Pasado:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoIndicativo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.PasadoIndicativo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.PasadoIndicativo3PerPlural;  break;
                  }
                break;
                }
              case TTime.Futuro:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.FuturoIndicativo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.FuturoIndicativo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.FuturoIndicativo3PerPlural;  break;
                  }
                break;
                }
              case TTime.PasadoImp:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoImpIndicativo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.PasadoImpIndicativo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.PasadoImpIndicativo3PerPlural;  break;
                  }
                break;
                }
              }
            }
          break;
          }
        case TMod.Imperativo:
          {
          if( numero == TNum.Singular )
            {
            switch (persona)
              {
              case TPer.Segunda: i = ColId.Imperativo2PerSing1; break;
              case TPer.Tercera: i = ColId.Imperativo3PerSing;  break;
              }
            }
          else
            {
            switch (persona)
              {
              case TPer.Primera: i = ColId.Imperativo1PerPlural1; break;
              case TPer.Segunda: i = ColId.Imperativo2PerPlural1; break;
              case TPer.Tercera: i = ColId.Imperativo3PerPlural;  break;
              }
            }
          break;
          }
        case TMod.Potencial:
          {
          if (numero == TNum.Singular)
            {
            switch (persona)
              {
              case TPer.Primera: i = ColId.Potencial1PerSing;  break;
              case TPer.Segunda: i = ColId.Potencial2PerSing1; break;
              case TPer.Tercera: i = ColId.Potencial3PerSing;  break;
              }
            }
          else
            {
            switch (persona)
              {
              case TPer.Primera: i = ColId.Potencial1PerPlural;  break;
              case TPer.Segunda: i = ColId.Potencial2PerPlural1; break;
              case TPer.Tercera: i = ColId.Potencial3PerPlural;  break;
              }
            }
          break;
          }
        case TMod.Subjuntivo:
          {
          if (numero == TNum.Singular)
            {
            switch (tiempo)
              {
              case TTime.Presente:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PresenteSubjuntivo1PerSing;  break;
                  case TPer.Segunda: i = ColId.PresenteSubjuntivo2PerSing1; break;
                  case TPer.Tercera: i = ColId.PresenteSubjuntivo3PerSing;  break;
                  }
                break;
                }
              case TTime.Pasado:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoSubjuntivo1PerSing;  break;
                  case TPer.Segunda: i = ColId.PasadoSubjuntivo2PerSing1; break;
                  case TPer.Tercera: i = ColId.PasadoSubjuntivo3PerSing;  break;
                  }
                break;
                }
              }
            }
          else             // Plural del indicativo
            {
            switch (tiempo)
              {
              case TTime.Presente:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PresenteSubjuntivo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.PresenteSubjuntivo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.PresenteSubjuntivo3PerPlural;  break;
                  }
                break;
                }
              case TTime.Pasado:
                {
                switch (persona)
                  {
                  case TPer.Primera: i = ColId.PasadoSubjuntivo1PerPlural;  break;
                  case TPer.Segunda: i = ColId.PasadoSubjuntivo2PerPlural1; break;
                  case TPer.Tercera: i = ColId.PasadoSubjuntivo3PerPlural;  break;
                  }
                break;
                }
              }
            }
          break;
          }
        }

      int ii = (int)i;
      if (ii < 0 || ii >= Data.TabColIndex.Count)
        ii = 0;
      else
        ii = (int)Data.TabColIndex[ii];

      return ii;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el pronombre personal para la persona, el número y el idioma dados como parámetros </summary>
    public string Pronoun( TPer persona, TNum numero)
      {
      int i = 0;

      switch (persona)
        {
        case TPer.Primera: i = (numero == TNum.Singular)? 0 : 3; break;
        case TPer.Segunda: i = (numero == TNum.Singular)? 1 : 4; break;
        case TPer.Tercera: i = (numero == TNum.Singular)? 2 : 5; break;
        }

      return Data.PronomPerson[i];
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca si el verbo en su conjugación afirmativa o negativa lleva prefijo o sufijo</summary>
    void Prefijo_Conjugac( int fila, int col, bool negado, ref string prefijo, ref string sufijo, ref bool verboinf)
      {
      int ipat = Data.TabColIndex[ (int)ColId.PatronSufPref ];
      if( ipat > 0 )
        {
        var cadpatron = Data.TableVerb.Cell(fila,ipat); // # del patrón de suf. y pref.
        if( cadpatron.Length > 0 )
          {
          int tipopat;
          Int32.TryParse(cadpatron, out tipopat);

          var tablap = Data.TablePatron;
          if( negado == false )  
            {
            sufijo  = tablap.Cell((4*tipopat),col);
            prefijo = tablap.Cell((4*tipopat)+1,col);
            }
          else
            {
            if( tablap.Cell((4*tipopat),ipat).Length > 0 )
              verboinf = true;

            sufijo = tablap.Cell((4*tipopat)+2,col);
            prefijo = tablap.Cell((4*tipopat)+3,col);
            }
          }
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el pronombre reflexivo para la persona, el número y el idioma dados como parámetros</summary>
    string ReflexivePronoun( TPer persona, TNum numero)
      {
      int i = 0;

      switch( persona )
        {
        case TPer.Primera:
          i = (numero == TNum.Singular)? 0 : 3;
          break;
        case TPer.Segunda:
          i = (numero == TNum.Singular)? 1 : 4;
          break;
        case TPer.Tercera:
          break;
        }

      return Data.PronomReflex[i];
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve si el verbo RefVerb es o no reflexivo. En el parámetro RootVerb devuelve el verbo raíz</summary>
    bool ReflexiveVerbRoot( string RefVerb, out string RootVerb )
      {
      string Verbo = RefVerb;

      for( int i=0; i<Data.TerminacReflex.Count; i++ )
        {
        int pos = Verbo.IndexOf( Data.TerminacReflex[i] );
        if( pos >= 0 )
          {
          RootVerb = ( Verbo.Substring(0, pos) + Data.TerminacVerbal[i] );
          return true;
          }
        }

      RootVerb = "";
      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Identifica si el verbo tiene un prefijo (separable o no separable). Devuelve el prefijo y la raíz </summary>
    bool FindVerbPrefix( string verbo, out string verbprefix, out string verbroot )
      {
      verbprefix = "";
      verbroot = "";
      string prefix;

      prefix = Data.FindPreffixVerb(verbo);
      if( prefix==null ) return false;

      verbprefix = prefix;

      if( prefix.Substring(0, 1) == "." )
        prefix = prefix.Substring(1);

      verbroot = verbo.Substring(prefix.Length); 

      return true;
      }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    #region Funciones que se usan desde la maquinaria de traducción 

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el número total de conjugaciones posibles para un idioma dado</summary>
    public int DataConjCount()
      {
      return Data.DataConjArray.Count;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el modo de la conjugación idx en el idioma dado como parámetro</summary>
    public TMod DataConjMood( int idx )
      {
      return Data.DataConjArray[idx].Modo;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve el tiempo verbal de la conjugación idx en el idioma dado como parámetro</summary>
    public TTime DataConjTime( int idx )
      {
      return Data.DataConjArray[idx].Tiempo;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Devuelve si la conjugación idx corresponde a un tiempo compuesto o no en el idioma dado como parámetro</summary>
    public int DataConjCompound( int idx )
      {
      if( idx < 0 || idx >= Data.DataConjArray.Count )
        return -1;

      return Data.DataConjArray[idx].Compuesto ? 1 : 0;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Busca un prefijo de un verbo en los datos de conjugación </summary>
    public string FindPreffixVerb( string word )
      {
      return Data.FindPreffixVerb( word );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    public string DeclineNoun( string noun, TGen gen, TNum num, TDecl decl)
      {
      string strnoun = noun;
      string declNoun = noun;

      if( Data.Lang == TLng.De )
        {
        if(num == TNum.Plural)
          {
          if(decl == TDecl.Dative)
            {
            char last = strnoun[strnoun.Length-1];
            if((last != 'n') && (last != 's'))
              declNoun += 'n';
            }
          }
        else
          {
          //Se busca el sust. en el dicc de Decl.Irreg de Sust. 
          var data = Data.FindIrregDecNoun(strnoun);
          if( data==null )
            {  
            //Se busca el sust. en el dicc de Sufijos de Decl. de sust.
            data = Data.Find_SuffixDecNoun(strnoun);
            if( data != null )
              {
              int pos;

              string suf,declList,sufElem;
              pos = data.IndexOf(",");
              suf = data.Substring(0, pos);
              declList = data.Substring(pos+1);

              sufElem = GetDeclElement(suf, declList, decl, 0);

              data = strnoun + "#";
              pos = data.IndexOf(suf+"#");
              declNoun = data.Substring(0, pos) + sufElem;
              }

            }   
          else
            {
            declNoun = GetDeclElement(strnoun, data, decl, 0);
            }
          }
        }

      return declNoun;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    public string DeclineNounIndex( string noun, TGen gen, TNum num, TDecl decl, out int decCount, int idx)
      {
      decCount = 0;
      string strnoun = noun;
      string declNoun = noun;

      if( Data.Lang == TLng.De)
        {
        if( num == TNum.Plural )
          {
          if( decl == TDecl.Dative )
            {
            char last = strnoun[strnoun.Length-1];
            if( (last != 'n') && (last != 's') )
              declNoun += 'n';
            }
          }
        else
          {
          var data = Data.FindIrregDecNoun(strnoun);                          // Se busca el sust. en el dicc de Decl.Irreg de Sust. 
          if( data==null )
            {  
            data = Data.Find_SuffixDecNoun(strnoun);                          //Se busca el sust. en el dicc de Sufijos de Decl. de sust.
            if( data!=null )
              {
              int pos;

              string suf,declList,sufElem = "";
              pos = data.IndexOf(",");
              suf = data.Substring(0, pos);
              declList = data.Substring(pos+1);

              sufElem = GetDeclElement(suf,declList,decl,idx, out decCount);

              data = strnoun + "#";
              pos = data.IndexOf(suf+"#");
              declNoun = data.Substring(0, pos) + sufElem;
              }

            }   
          else
            {
            declNoun = GetDeclElement(strnoun,data,decl,idx, out decCount);
            }
          }
        }

      return declNoun;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    string GetDeclElement(string key, string declList, TDecl decl, int index)
      {
      int declCount;
      return GetDeclElement(key, declList, decl, index, out declCount);
      }

    string GetDeclElement(string key, string declList, TDecl decl, int index, out int declCount)
      {
      var declArray = declList.Split(';');
      string sufList = "";

      switch(decl)
        {
        case TDecl.Nominative:
          {
          sufList = declArray[0];
          break;
          }
        case TDecl.Accusative:
          {
          sufList = declArray[1];
          break;
          }
        case TDecl.Dative:
          {
          sufList = declArray[2];
          break;
          }
        case TDecl.Genitive:
          {
          sufList = declArray[3];
          break;
          }
        }

      var sufArray =  declList.Split('|');
      declCount = sufArray.Length; 

      string elem = sufArray[index];

      if(elem[0] == '-')
        elem = key + elem.Substring(1);

      return elem;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Función que declina un adjetivo según el género, el número  el caso y el tipo de artículo que se le inserta </summary>
    public string DeclineAdjective( string adj, TGen gen, TNum num, TDecl decl, TArtic artic)
      {
      string stradj = adj;
      string declAdj = adj;

      if( Data.Lang == TLng.De )
        {
        var data = Data.FindIrregDecAdj( stradj );                    // Se busca el adj. en el dicc de Decl.Irreg de Adj.
        if( data==null )
          {  
          data = Data.Find_SuffixDecAdj( stradj );                    // Se busca el adj. en el dicc de Sufijos de Decl. de adj.
          if( data!=null )
            {
            int pos;
            string suf,declList,sufElem;
            pos = data.IndexOf(",");
            suf = data.Substring(0, pos);
            declList = data.Substring(pos+1);

            if(declList != "-")
              {
              declList = GetDeclArtType(data,artic);

              int idx = GetDeclIndex(gen,num);
              sufElem = GetDeclElement(suf,declList,decl,idx);

              data = stradj + "#";
              pos = data.IndexOf(suf+"#");
              declAdj = data.Substring(0, pos) + sufElem;
              }
            }

          }   
        else
          {
          if(data != "-")
            {
            string declList;
            declList = GetDeclArtType(data,artic);

            int idx = GetDeclIndex(gen,num);
            declAdj = GetDeclElement(stradj,declList,decl,idx);
            }
          }
        }

      return declAdj;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Función que concuerda un sustantivo o adjetivo en un género, número y grado determinado y un indice dado</summary>
    public string ConcordWordsIndex( string noun, TGen genfuente, TNum numfuente, TGen gendestino, TNum numdestino, TGrad adjgrado, out int decCount, int idx)
      {
      decCount = 0;
      string strnoun = noun;

      if (Data.Lang != TLng.De)
        strnoun = strnoun.ToLower();

      string adjf = strnoun;
      string adjconc = "";
      int pos;

      if( adjgrado == TGrad.Superlative )
        {
        adjconc = Data.FindIrregSup(adjf);
        if( adjconc == null )
          {  
          //Se busca el adj. en el dicc de Sufijos de Superl.
          adjconc = Data.Find_SuffixSup( adjf );
          if( adjconc!=null)
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf( s[0] + "#" );
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if( adjgrado == TGrad.Comparative )
        {
        //Se busca el adj. en el dicc de Comparat. Irreg
        adjconc = Data.FindIrregComp( adjf );
        if( adjconc ==null )
          {  
          adjconc = Data.Find_SuffixComp( adjf );
          if( adjconc != null )
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf(s[0]+"#");
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if( (genfuente == TGen.Masc) && (gendestino == TGen.Femen) )
        {
        if( !string.IsNullOrEmpty(adjconc) ) adjf = adjconc;

        //Se busca el sust. o adj. en el dicc de Fem. Irreg
        adjconc = Data.FindIrregFem( adjf );
        if( adjconc==null )
          {  
          adjconc = Data.Find_SuffixFem( adjf );
          if( adjconc != null )
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf(s[0]+"#");
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if( (numfuente == TNum.Singular) && (numdestino == TNum.Plural) )
        {
        if( !string.IsNullOrEmpty(adjconc) ) adjf = adjconc;

        //Se busca el sust. o adj. en el dicc de Plurales Irreg
        adjconc = Data.FindIrregPlural( adjf );
        if( adjconc!=null )
          {
          return GetIrregElement( adjf, adjconc, idx, out decCount );
          }
        else
          {  
          //Se busca el sust. o adj. en el dicc de Sufijos Plurales
          adjconc = Data.Find_SuffixPlural( adjf );
          if( adjconc != null )
            {
            var s  = adjconc.Split(',');
            var s2 = GetIrregElement(adjf, s[1], idx, out decCount);
            adjconc = adjf + "#";
            pos = adjconc.IndexOf(s[0]+"#");
            adjconc = adjconc.Substring(0, pos) + s2;

            return adjconc;
            }
          }   
        }

      if( string.IsNullOrEmpty(adjconc) )  return strnoun;
      else                                 return adjconc;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Función que concuerda un sustantivo o adjetivo en un género,  número y grado determinado</summary>
    public string ConcordWords( string noun, TGen genfuente, TNum numfuente, TGen gendestino, TNum numdestino, TGrad adjgrado)
      {
      string strnoun = noun;
      if( Data.Lang != TLng.De ) strnoun = strnoun.ToLower();

      string adjf = strnoun;
      string adjconc = null;
      int pos;

      if( adjgrado == TGrad.Superlative )
        {
        adjconc = Data.FindIrregSup( adjf );                        // Se busca el adj. en el dicc de Superl. Irreg
        if( adjconc==null )
          {  
          adjconc = Data.Find_SuffixSup( adjf );                    // Se busca el adj. en el dicc de Sufijos de Superl.
          if(  adjconc != null )
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf(s[0]+"#");
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if( adjgrado == TGrad.Comparative )
        {
        adjconc = Data.FindIrregComp( adjf );                       // Se busca el adj. en el dicc de Comparat. Irreg
        if( adjconc==null )
          {  
          adjconc = Data.Find_SuffixComp( adjf );                   // Se busca el adj. en el dicc de Sufijos de Comparat.
          if( adjconc != null )
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf( s[0]+"#" );
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if ((genfuente == TGen.Masc) && (gendestino == TGen.Femen))
        {
        if( !string.IsNullOrEmpty(adjconc) )  adjf = adjconc;

        adjconc = Data.FindIrregFem( adjf );                        // Se busca el sust. o adj. en el dicc de Fem. Irreg          
        if( adjconc != null )
          adjconc = GetIrregElement(adjf,adjconc,0);
        else
          {  
          adjconc = Data.Find_SuffixFem( adjf );                    // Se busca el verbo en el dicc de Sufijos Femeninos
          if( adjconc != null )
            {
            var s = adjconc.Split(',');
            adjconc = adjf + "#";
            pos = adjconc.IndexOf( s[0]+"#" );
            adjconc = adjconc.Substring(0, pos) + s[1];
            }
          }   
        }

      if( (numfuente == TNum.Singular) && (numdestino == TNum.Plural) )
        {
        if( !string.IsNullOrEmpty(adjconc) ) adjf = adjconc;

        adjconc = Data.FindIrregPlural( adjf );                     // Se busca el sust. o adj. en el dicc de Plurales Irreg
        if( adjconc!=null )
          {
          return GetIrregElement( adjf, adjconc, 0 );
          }
        else
          {  
          adjconc = Data.Find_SuffixPlural( adjf );                 // Se busca el sust. o adj. en el dicc de Sufijos Plurales
          if( adjconc!=null )
            {
            var s  = adjconc.Split(',');
            var s2 = GetIrregElement( adjf, s[1], 0 );
            adjconc = adjf + "#";
            pos = adjconc.IndexOf( s[0]+"#" );
            adjconc = adjconc.Substring(0, pos) + s2;

            return adjconc;
            }
          }   
        }

      if( string.IsNullOrEmpty(adjconc) ) return strnoun;
      else                                return adjconc;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    string GetDeclArtType(string declList, TArtic artic)
      {
      string decl = "";

      switch (artic)
        {
        case TArtic.Null:
            GetSubCampo(out decl, declList, "N", "#");
            break;
        case TArtic.Definite:
            GetSubCampo(out decl, declList, "D", "#");
            break;
        case TArtic.Indefinite:
            GetSubCampo(out decl, declList, "I", "#");
            break;
        }

      return decl;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Funcion para extraer un subcampo dentro de una cadena que  se encuentre dividida en campos </summary>
    ///
    ///<param name="subcampo">Cadena donde se retorna el contenido del subcampo</param>
    ///<param name="str">Cadena que contiene todos los campos</param>
    ///<param name="name">Nombre del campo a seleccionar</param>
    ///<param name="divisor">Elemento que se emplea como divisor #@, etc</param>
    bool GetSubCampo(out string subcampo,  string str, string name, string divisor )
      {
      bool ret = false;
      subcampo = "";

      string inter = divisor + name;
      int i = str.IndexOf(inter);
      if( i != -1 )
        {
        string tmp = str.Substring(i + inter.Length);
        int j = tmp.IndexOf(divisor);
        if( j == -1 )
          subcampo = tmp;
        else
          subcampo = tmp.Substring(0, j);

        ret = true;
        }

      return ret;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    //string GetIrregElement(string key, string elemList, int index = 0, LPWORD declCount = null)
    string GetIrregElement(string key, string elemList, int index)
      {
      int declCount;
      return GetIrregElement(key, elemList, index, out declCount);
      }

    string GetIrregElement(string key, string elemList, int index, out int declCount)
      {
      var elemArray = elemList.Split('|');
      declCount     = elemArray.Length; 

      string elem;
      elem = elemArray[index];

      if(elem[0] == '-')
        elem = key + elem.Substring(1);

      return elem;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    int GetDeclIndex(TGen gen, TNum num)
      {
      int idx = 0;

      if (num == TNum.Plural)
        idx = 3;
      else
        {
        switch (gen)
          {
          case TGen.Masc:   idx = 0; break;
          case TGen.Femen:  idx = 1; break;
          case TGen.Neutro: idx = 2; break;
          }
        }

      return idx;
      }

    #endregion
    //-----------------------------------------------------------------------------------------------------------------------------------
    }
  }
