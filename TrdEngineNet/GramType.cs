using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrdEngine
  {
//---------------------------------------------------------------------------------------
// Estructura para definir las caracteristicas de cada tipo soportado  
//---------------------------------------------------------------------------------------
  class InfoType
    {
    public string   Type;
    public string[] SubTypes;

    public InfoType( string FType, string[] STypes )
      {
      Type     = FType;
      SubTypes = STypes;
      }
    }

  public class GramType
    {
    static string[,] Names = 
      {// Español                 Inglés                    Italiano                  Aleman                    Francés
      {"Sustantivo"             ,"Sustantivo"             ,"Sustantivo"             ,"Sustantivo"             ,"Sustantivo"             }, 
      {"Adjetivo"               ,"Adjetivo"               ,"Adjetivo"               ,"Adjetivo"               ,"Adjetivo"               }, 
      {"Adverbio"               ,"Adverbio"               ,"Adverbio"               ,"Adverbio"               ,"Adverbio"               }, 
      {"Verbo Transitivo"       ,"Verbo Transitivo"       ,"Verbo Transitivo"       ,"Verbo Transitivo"       ,"Verbo Transitivo"       }, 
      {"Nombre Propio"          ,"Nombre Propio"          ,"Nombre Propio"          ,"Nombre Propio"          ,"Nombre Propio"          }, 
      {"Preposición"            ,"Preposición"            ,"Preposición"            ,"Preposición"            ,"Preposición"            }, 
      {"Participio Transitivo"  ,"Participio Transitivo"  ,"Participio Transitivo"  ,"Participio Transitivo"  ,"Participio Transitivo"  }, 
      {"Participio Intransitivo","Participio Intransitivo","Participio Intransitivo","Participio Intransitivo","Participio Intransitivo"}, 
      {"Gerundio Transitivo"    ,"Gerundio Transitivo"    ,"Gerundio Transitivo"    ,"Gerundio Transitivo"    ,"Gerundio Transitivo"    }, 
      {"Conjunción"             ,"Conjunción"             ,"Conjunción"             ,"Conjunción"             ,"Conjunción"             }, 
      {"Interjección"           ,"Interjección"           ,"Interjección"           ,"Interjección"           ,"Interjección"           }, 
      {"Adjetivo Estático"      ,"Adjetivo Estático"      ,"Adjetivo Estático"      ,"Adjetivo Estático"      ,"Adjetivo Estático"      }, 
      {"Verbo Intransitivo"     ,"Verbo Intransitivo"     ,"Verbo Intransitivo"     ,"Verbo Intransitivo"     ,"Verbo Intransitivo"     }, 
      {"Verbo Reflexivo"        ,"Verbo Reflexivo"        ,"Verbo Reflexivo"        ,"Verbo Reflexivo"        ,"Verbo Reflexivo"        }, 
      {"Verbo Auxiliar"         ,"Verbo Auxiliar"         ,"Verbo Auxiliar"         ,"Verbo Auxiliar"         ,"Verbo Auxiliar"         }, 
      {"Verbo 3ra Transitivo"   ,"Verbo 3ra Transitivo"   ,"Verbo 3ra Transitivo"   ,"Verbo 3ra Transitivo"   ,"Verbo 3ra Transitivo"   }, 
      {"Verbo 3ra Intransitivo" ,"Verbo 3ra Intransitivo" ,"Verbo 3ra Intransitivo" ,"Verbo 3ra Intransitivo" ,"Verbo 3ra Intransitivo" }, 
      {"Gerundio Intransitivo"  ,"Gerundio Intransitivo"  ,"Gerundio Intransitivo"  ,"Gerundio Intransitivo"  ,"Gerundio Intransitivo"  }, 
      {"Tipo compuesto"         ,"Tipo compuesto"         ,"Tipo compuesto"         ,"Tipo compuesto"         ,"Tipo compuesto"         }, 
      {"Tipo desconocido"       ,"Tipo desconocido"       ,"Tipo desconocido"       ,"Tipo desconocido"       ,"Tipo desconocido"       }, 
      };

    static string[,] Abrev = 
      {// Español      Inglés        Italiano      Aleman        Francés
      {"Sust."      ,"Sust."      ,"Sust."      ,"Sust."      ,"Sust."      }, 
      {"Adj."       ,"Adj."       ,"Adj."       ,"Adj."       ,"Adj."       }, 
      {"Adv."       ,"Adv."       ,"Adv."       ,"Adv."       ,"Adv."       }, 
      {"V.Tran."    ,"V.Tran."    ,"V.Tran."    ,"V.Tran."    ,"V.Tran."    }, 
      {"Nomb."      ,"Nomb."      ,"Nomb."      ,"Nomb."      ,"Nomb."      }, 
      {"Prep."      ,"Prep."      ,"Prep."      ,"Prep."      ,"Prep."      }, 
      {"P.Tran."    ,"P.Tran."    ,"P.Tran."    ,"P.Tran."    ,"P.Tran."    }, 
      {"P.Intran."  ,"P.Intran."  ,"P.Intran."  ,"P.Intran."  ,"P.Intran."  }, 
      {"G.Tran."    ,"G.Tran."    ,"G.Tran."    ,"G.Tran."    ,"G.Tran."    }, 
      {"Conj."      ,"Conj."      ,"Conj."      ,"Conj."      ,"Conj."      }, 
      {"Inter."     ,"Inter."     ,"Inter."     ,"Inter."     ,"Inter."     }, 
      {"Adj.Est."   ,"Adj.Est."   ,"Adj.Est."   ,"Adj.Est."   ,"Adj.Est."   }, 
      {"V.Intran."  ,"V.Intran."  ,"V.Intran."  ,"V.Intran."  ,"V.Intran."  }, 
      {"V.Refl."    ,"V.Refl."    ,"V.Refl."    ,"V.Refl."    ,"V.Refl."    }, 
      {"V.Aux."     ,"V.Aux."     ,"V.Aux."     ,"V.Aux."     ,"V.Aux."     }, 
      {"V.3.Tran."  ,"V.3.Tran."  ,"V.3.Tran."  ,"V.3.Tran."  ,"V.3.Tran."  }, 
      {"V.3.Intran.","V.3.Intran.","V.3.Intran.","V.3.Intran.","V.3.Intran."}, 
      {"G.Intran."  ,"G.Intran."  ,"G.Intran."  ,"G.Intran."  ,"G.Intran."  }, 
      {"T.Comp."    ,"T.Comp."    ,"T.Comp."    ,"T.Comp."    ,"T.Comp."    }, 
      {"T.Desc."    ,"T.Desc."    ,"T.Desc."    ,"T.Desc."    ,"T.Desc."    } 
      };

    static InfoType[] Info = 
      {
      new InfoType( "FI", new string[]{"FI"}),     // - Frase ??
      new InfoType( "FL", new string[]{"FL"}),     // - Frase de lugar
      new InfoType( "FO", new string[]{"FO"}),     // - Frase ??
      new InfoType( "FP", new string[]{"FP"}),     // - Frase preposicional
      new InfoType( "FS", new string[]{"FS"}),     // - Frase Sustantiva
      new InfoType( "FT", new string[]{"FT"}),     // - Frase de tiempo
      new InfoType( "RD", new string[]{"RD"}),     // Articulo
      new InfoType( "RI", new string[]{"RI"}),     // Articulo Invariable
      new InfoType( "TI", new string[]{"TI"}),     // ??
      new InfoType( "AD", new string[]{"AD"}),     // dtADETERMINATIVO
      new InfoType( "DN", new string[]{"DN"}),     // dtDNEGACION 
      new InfoType( "GZ", new string[]{"GZ"}),     // dtCOMA
      new InfoType( "XX", new string[]{"XX"}),     // pasado o ppasado o sust

      new InfoType( "AA", new string[]{"AA"}),     // Adjetivo en grado positivo
      new InfoType( "AC", new string[]{"AC"}),     // Adjetivo en grado comparativo
      new InfoType( "AE", new string[]{"AE"}),     // Adjetivo demostrativo
      new InfoType( "AF", new string[]{"AF"}),     // Adjetivo indefinido
      new InfoType( "AI", new string[]{"AI"}),     // Adjetivo inmovil en grado positivo
      new InfoType( "AN", new string[]{"AN"}),     // Adjetivo numeral
      new InfoType( "AP", new string[]{"AP"}),     // Adjetivo posesivo
      new InfoType( "AQ", new string[]{"AQ"}),     // Adjetivo interrogativo
      new InfoType( "AS", new string[]{"AS"}),     // Adjetivo en grado superlativo
      new InfoType( "XW", new string[]{"XW"}),     // Adjetivo en grado comparativo o sustantivo
      new InfoType( "XS", new string[]{"XS"}),     // Adjetivo en grado superlativo o sustantivo
      new InfoType( "MG", new string[]{"MG"}),     // Adjetivo indefinido o pronombre indefinido
      new InfoType( "MU", new string[]{"MU"}),     // Adjetivo indefinido o pronombre indefinido o adverbio
//      new InfoType( "MV", new string[]{"MV"}),     // Adjetivo numeral y sustantivo numeral AN o NN
      new InfoType( "PB", new string[]{"PB"}),     // Adjetivo preposicional  
      new InfoType( "PV", new string[]{"PV"}),     // Adjetivo con preposición TO
      new InfoType( "OO", new string[]{"OO"}),     // Pronombre genérico
      new InfoType( "OC", new string[]{"OC"}),     // Pronombre personal en el complemento directo
      new InfoType( "OH", new string[]{"OH"}),     // Pronombre personal en el complemento indirecto
      new InfoType( "OD", new string[]{"OD"}),     // Pronombre demostrativo
      new InfoType( "OE", new string[]{"OE"}),     // Pronombre enfático
      new InfoType( "OG", new string[]{"OG"}),     // Pronombre indefinido
      new InfoType( "OI", new string[]{"OI"}),     // Pronombre interrrogativo
      new InfoType( "OP", new string[]{"OP"}),     // Pronombre personal
      new InfoType( "OR", new string[]{"OR"}),     // Pronombre reflexivo
      new InfoType( "OS", new string[]{"OS"}),     // Pronombre posesivo
      new InfoType( "OL", new string[]{"OL"}),     // Pronombre relativo
      new InfoType( "OW", new string[]{"OW"}),     // Pronombre personal detrás de preposición ej: to me, with me
      new InfoType( "OA", new string[]{"OA"}),     // Pronombre demostrativo o adjetivo demostrativo
      new InfoType( "DD", new string[]{"DD"}),     // Adverbio
      new InfoType( "DF", new string[]{"DF"}),     // adverbio de frecuencia
      new InfoType( "DQ", new string[]{"DQ"}),     // Adverbio interrogativo
      new InfoType( "DI", new string[]{"DI"}),     // Adverbio intensificador
      new InfoType( "SS", new string[]{"SS"}),     // Sustantivo
      new InfoType( "NN", new string[]{"NN"}),     // Sustantivo numeral o número
      new InfoType( "NM", new string[]{"NM"}),     // Nombre de un mes del año
      new InfoType( "NP", new string[]{"NP"}),     // Nombre Propio
      new InfoType( "JJ", new string[]{"JJ"}),     // Interjección
      new InfoType( "PP", new string[]{"PP"}),     // Preposición
      new InfoType( "CC", new string[]{"CC"}),     // Conjunción
      new InfoType( "CO", new string[]{"CO"}),     // Conjunción de oracion
      new InfoType( "VT", new string[]{"VT"}),     // Verbo transitivo
      new InfoType( "VI", new string[]{"VI"}),     // Verbo intransitivo
      new InfoType( "VG", new string[]{"VG"}),     // Verbo gerundio transitivo o intransitivo
      new InfoType( "GT", new string[]{"GT"}),     // Gerundio transitivo
      new InfoType( "GI", new string[]{"GI"}),     // Gerundio intransitivo
      new InfoType( "VP", new string[]{"VP"}),     // Participio transitivo o intransitivo
      new InfoType( "PT", new string[]{"PT"}),     // Participio transitivo
      new InfoType( "PI", new string[]{"PI"}),     // Participio intransitivo
      new InfoType( "VS", new string[]{"VS"}),     // Verbo tercera persona transitivo o intransitivo
      new InfoType( "HT", new string[]{"HT"}),     // VST  verbo tercera persona transitivo
      new InfoType( "HI", new string[]{"HI"}),     // VSI  verbo tercera persona intransitivo
      new InfoType( "VC", new string[]{"VC"}),     // Verbo pasado o participio
      new InfoType( "JK", new string[]{"JK"}),     // Verbo pasado o participio transitivo
      new InfoType( "JL", new string[]{"JL"}),     // Verbo pasado o participio intransitivo
      new InfoType( "VD", new string[]{"VD"}),     // Verbo pasado transitivo o intransitivo
      new InfoType( "JT", new string[]{"JT"}),     // Verbo pasado transitivo
      new InfoType( "JI", new string[]{"JI"}),     // Verbo pasado intransitivo
      new InfoType( "VR", new string[]{"VR"}),     // Verbo reflexivo
      new InfoType( "BE", new string[]{"BE"}),     // Palabra be o ser o essere en infinitivo o indicativo (debe cambiarse a BI)
      new InfoType( "BN", new string[]{"BN"}),     // Palabra be o ser o essere en participio
      new InfoType( "BG", new string[]{"BG"}),     // Palabra be o ser o essere en gerundio
      new InfoType( "BS", new string[]{"BS"}),     // Palabra be o ser o essere en subjuntivo
      new InfoType( "BI", new string[]{"BI"}),     // Palabra be o ser o essere en indicativo
      new InfoType( "BP", new string[]{"BP"}),     // Palabra be o ser o essere en potencial
      new InfoType( "BM", new string[]{"BM"}),     // Palabra be o ser o essere en imperativo
      new InfoType( "BD", new string[]{"BD"}),     // Palabra be o ser o essere en indicativo o subjuntivo
      new InfoType( "BF", new string[]{"BF"}),     // Palabra be o ser o essere en indicativo o imperativo
      new InfoType( "BH", new string[]{"BH"}),     // Palabra be o ser o essere en indicativo o imperativo o subjuntivo
      new InfoType( "BJ", new string[]{"BJ"}),     // Palabra be o ser o essere en indicativo o imperativo o participio
      new InfoType( "BK", new string[]{"BK"}),     // Palabra be o ser o essere en subjuntivo o imperativo
      new InfoType( "BA", new string[]{"BA"}),     // 
      new InfoType( "HA", new string[]{"HA"}),     // Palabra have, haber, avere
      new InfoType( "HN", new string[]{"HN"}),     // Palabra have, haber, avere en participio
      new InfoType( "HG", new string[]{"HG"}),     // Palabra have, haber, avere en gerundio
      new InfoType( "HJ", new string[]{"HJ"}),     // Palabra have, haber, avere en subjuntivo
      new InfoType( "HD", new string[]{"HD"}),     // Palabra have, haber, avere en indicativo
      new InfoType( "HP", new string[]{"HP"}),     // Palabra have, haber, avere en potencial
      new InfoType( "HM", new string[]{"HM"}),     // Palabra have, haber, avere en imperativo
      new InfoType( "HC", new string[]{"HC"}),     // Palabra have, haber, avere en indicativo o subjuntivo
      new InfoType( "HF", new string[]{"HF"}),     // Palabra have, haber, avere en indicativo o imperativo o subjuntivo
      new InfoType( "HV", new string[]{"HV"}),     // Palabra have, haber, avere en subjuntivo o imperativo
      new InfoType( "VA", new string[]{"VA"}),     // Verbo auxiliar en infinitivo o indicativo
      new InfoType( "VU", new string[]{"VU"}),     // Verbo auxiliar en participio
      new InfoType( "VY", new string[]{"VY"}),     // Verbo auxiliar en gerundio
      new InfoType( "VJ", new string[]{"VJ"}),     // Verbo auxiliar en subjuntivo
      new InfoType( "VX", new string[]{"VX"}),     // Verbo auxiliar en indicativo
      new InfoType( "VZ", new string[]{"VZ"}),     // Verbo auxiliar en potencial
      new InfoType( "VW", new string[]{"VW"}),     // Verbo auxiliar en imperativo
      new InfoType( "VQ", new string[]{"VQ"}),     // Verbo auxiliar en indicativo o subjuntivo
      new InfoType( "VB", new string[]{"VB"}),     // Verbo auxiliar en indicativo o imperativo o participio
      new InfoType( "VE", new string[]{"VE"}),     // Verbo auxiliar en indicativo o imperativo o subjuntivo
      new InfoType( "VF", new string[]{"VF"}),     // Verbo auxiliar en subjuntivo o imperativo
      new InfoType( "VH", new string[]{"VH"}),     // Verbo auxiliar en indicativo o imperativo
      new InfoType( "VK", new string[]{"VK"}),     // Verbo transitivo e intransitivo en subjuntivo
      new InfoType( "VL", new string[]{"VL"}),     // Verbo transitivo e intransitivo en indicativo
      new InfoType( "VM", new string[]{"VM"}),     // Verbo transitivo e intransitivo en potencial
      new InfoType( "VN", new string[]{"VN"}),     // Verbo transitivo e intransitivo en imperativo
      new InfoType( "VO", new string[]{"VO"}),     // Verbo transitivo e intransitivo en indicativo o subjuntivo
      new InfoType( "BX", new string[]{"BX"}),     // Verbo transitivo e intransitivo en indicativo o imperativo o participio
      new InfoType( "BY", new string[]{"BY"}),     // Verbo transitivo e intransitivo en indicativo o imperativo o subjuntivo
      new InfoType( "BZ", new string[]{"BZ"}),     // Verbo transitivo e intransitivo en subjuntivo o imperativo
      new InfoType( "BL", new string[]{"BL"}),     // Verbo transitivo e intransitivo en indicativo o imperativo
      new InfoType( "BR", new string[]{"BR"}),     // Verbo transitivo e intransitivo en indicativo o participio
      new InfoType( "BO", new string[]{"BO"}),     // Verbo transitivo e intransitivo en indicativo o imperativo o subjuntivo o participio
      new InfoType( "BU", new string[]{"BU"}),     // Verbo transitivo e intransitivo en indicativo o potencial
      new InfoType( "BV", new string[]{"BV"}),     // Verbo transitivo e intransitivo en subjuntivo o participio
//      new InfoType( "AS", new string[]{"AS"}),     // Palabra "as"
      new InfoType( "YU", new string[]{"YU"}),     // Palabra "you","it"
      new InfoType( "HE", new string[]{"HE"}),     // Palabra "her"
      new InfoType( "TO", new string[]{"TO"}),     // Palabra "to"
      new InfoType( "OF", new string[]{"OF"}),     // Palabra "of"
      new InfoType( "MN", new string[]{"MN"}),     // Palabra "mine"
      new InfoType( "ME", new string[]{"ME"}),     // Palabras "me", "us"
      new InfoType( "HS", new string[]{"HS"}),     // Palabra  "his"
      new InfoType( "AL", new string[]{"AL"}),     // Predeterminativo "All"
      new InfoType( "TH", new string[]{"TH"}),     // Palabra "that"
      new InfoType( "NO", new string[]{"NO"}),     // Palabra  "no"
      new InfoType( "EI", new string[]{"EI"}),     // Palabra  "either"
      new InfoType( "NI", new string[]{"NI"}),     // Palabra  "nor" y "neither"
      new InfoType( "WI", new string[]{"WI"}),     // Palabra  "will" auxiliar de futuro
      new InfoType( "WO", new string[]{"WO"}),     // Palabra  "would" auxiliar de potencial
      new InfoType( "MO", new string[]{"MO"}),     // Palabra "more"
      new InfoType( "NE", new string[]{"NE"}),     // Palabra "one"
      new InfoType( "DO", new string[]{"DO"}),     // Verbo DO, DOES
      new InfoType( "DW", new string[]{"DW"}),     // Palabra "down"
      new InfoType( "LK", new string[]{"LK"}),     // Palabra  "like"
      new InfoType( "NR", new string[]{"NR"}),     // Palabra  "near"
      new InfoType( "AY", new string[]{"AY"}),     // Palabra "any"
      new InfoType( "TY", new string[]{"TY"}),     // Palabra "than"
      new InfoType( "TW", new string[]{"TW"}),     // Palabra "there"
      new InfoType( "QZ", new string[]{"QZ"}),     // Palabras pueden ser SS AA DD pero delante de adjetivos no son abverbios como "good"
      new InfoType( "SP", new string[]{"SP"}),     // Sustantivo plural
      new InfoType( "SM", new string[]{"SM"}),     // Sustantivo nombre de día de la semana Lunes, martes, etc.
      new InfoType( "GW", new string[]{"GW"}),     // Posesivo 's de un sustantivo en minusculas
      new InfoType( "GV", new string[]{"GV"}),     // Posesivo 's de un sustantivo en mayúsculas o nombre propio
      new InfoType( "SE", new string[]{"SE"}),     // Palabra "se"
      new InfoType( "IA", new string[]{"IA"}),     // Pronombre en complemento directo o articulo (palabra lo y la)
      new InfoType( "WZ", new string[]{"WZ"}),     // Pronombre en complemento directo o pronombre personal
      new InfoType( "YA", new string[]{"YA"}),     // Adjetivo verbo auxiliar en participio
      new InfoType( "YC", new string[]{"YC"}),     // Adjetivo verbo transitivo o intransitivo en infinitivo
      new InfoType( "YD", new string[]{"YD"}),     // Adjetivo verbo transitivo o intransitivo en indicativo
      new InfoType( "YE", new string[]{"YE"}),     // Adjetivo verbo transitivo o intransitivo en subjuntivo
      new InfoType( "YH", new string[]{"YH"}),     // Adjetivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO o INDICATIVO o PARTICIPIO
      new InfoType( "YJ", new string[]{"YJ"}),     // Adjetivo verbo transitivo o intransitivo en IMPERATIVO o PARTICIPIO o INDICATIVO
      new InfoType( "YI", new string[]{"YI"}),     // Adjetivo verbo transitivo o intransitivo en IMPERATIVO o INDICATIVO
      new InfoType( "YK", new string[]{"YK"}),     // Adjetivo verbo transitivo o intransitivo en PARTICIPIO o INDICATIVO
      new InfoType( "YL", new string[]{"YL"}),     // Adjetivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO
      new InfoType( "YN", new string[]{"YN"}),     // Adjetivo verbo transitivo o intransitivo en POTENCIAL
      new InfoType( "YM", new string[]{"YM"}),     // Adjetivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO o INDICATIVO
      new InfoType( "YP", new string[]{"YP"}),     // Sustantivo verbo transitivo o intransitivo en SUBJUNTIVO
      new InfoType( "YQ", new string[]{"YQ"}),     // Sustantivo verbo transitivo o intransitivo en INDICATIVO
      new InfoType( "YR", new string[]{"YR"}),     // Sustantivo verbo transitivo o intransitivo en IMPERATIVO
      new InfoType( "YS", new string[]{"YS"}),     // Sustantivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO
      new InfoType( "YT", new string[]{"YT"}),     // Sustantivo verbo transitivo o intransitivo en IMPERATIVO o INDICATIVO
      new InfoType( "YV", new string[]{"YV"}),     // Sustantivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO o INDICATIVO
      new InfoType( "YW", new string[]{"YW"}),     // Sustantivo verbo transitivo o intransitivo en SUBJUNTIVO o IMPERATIVO o INDICATIVO o PARTICIP
      new InfoType( "YX", new string[]{"YX"}),     // Sustantivo verbo transitivo o intransitivo en IMPERATIVO o PARTICIPIO o INDICATIVO
      new InfoType( "YZ", new string[]{"YZ"}),     // Sustantivo verbo transitivo o intransitivo en PARTICIPIO o INDICATIVO
      new InfoType( "QQ", new string[]{"QQ"}),     // Sustantivo o verbo transitivo o intransitivo en SUBJUNTIVO o INDICATIVO
      new InfoType( "QR", new string[]{"QR"}),     // Sustantivo o verbo transitivo o intransitivo en potencial
      new InfoType( "WA", new string[]{"WA"}),     // Sustantivo verbo have, avere o haber en INDICATIVO
      new InfoType( "WB", new string[]{"WB"}),     // Sustantivo verbo be, essere, ser en subjuntivo
      new InfoType( "WC", new string[]{"WC"}),     // Sustantivo verbo auxiliar en Subjuntivo
      new InfoType( "WD", new string[]{"WD"}),     // Sustantivo verbo auxiliar en Indicativo o participio o imperativo
      new InfoType( "WE", new string[]{"WE"}),     // Verbo en participio o Adverbio
      new InfoType( "WF", new string[]{"WF"}),     // Verbo en Indicativo o Adverbio
      new InfoType( "WG", new string[]{"WG"}),     // Verbo en subjuntivo o en imperativo o Adverbio
      new InfoType( "WH", new string[]{"WH"}),     // Verbo en Indicativo o en imperativo o Adverbio
      new InfoType( "WJ", new string[]{"WJ"}),     // Verbo en Indicativo o en subjuntivo o en imperativo o Adverbio
      new InfoType( "WK", new string[]{"WK"}),     // Sustantivo o adjetivo o verbo auxiliar en participio
      new InfoType( "WL", new string[]{"WL"}),     // Sustantivo o adjetivo o verbo en subjuntivo
      new InfoType( "WM", new string[]{"WM"}),     // Sustantivo o adjetivo o verbo en indicativo
      new InfoType( "WN", new string[]{"WN"}),     // Sustantivo o adjetivo o verbo en potencial
      new InfoType( "WP", new string[]{"WP"}),     // Sustantivo o adjetivo o verbo en participio o en indicativo
      new InfoType( "WQ", new string[]{"WQ"}),     // Sustantivo o adjetivo o verbo en imperativo o en participio o en indicativo
      new InfoType( "WR", new string[]{"WR"}),     // Sustantivo o adjetivo o verbo en subjuntivo o en imperativo o en participio o en indicativo
      new InfoType( "WS", new string[]{"WS"}),     // Sustantivo o adjetivo o verbo en subjuntivo o en imperativo o en indicativo
      new InfoType( "WT", new string[]{"WT"}),     // Sustantivo o adjetivo o verbo en participio o en indicativo
      new InfoType( "WV", new string[]{"WV"}),     // Sustantivo o adjetivo o verbo en imperativo o en indicativo
      new InfoType( "WW", new string[]{"WW"}),     // Sustantivo o adjetivo o verbo en subjuntivo o en imperativo
      new InfoType( "WX", new string[]{"WX"}),     // Sustantivo o adverbio o verbo en subjuntivo o en imperativo o en indicativo
      new InfoType( "WY", new string[]{"WY"}),     // Sustantivo o adverbio o verbo en imperativo o en indicativo
      new InfoType( "QS", new string[]{"QS"}),     // Sustantivo y Adjetivo y verbo (indicativo) y adverbio
      new InfoType( "QT", new string[]{"QT"}),     // Sustantivo y Adjetivo y verbo (indicativo o subjuntivo o imperativo) y adverbio
      new InfoType( "QU", new string[]{"QU"}),     // Sustantivo y Adjetivo y verbo (indicativo o imperativo) y adverbio
      new InfoType( "AO", new string[]{"AO"}),     // Pronombre o adjetivo inmóvil
      new InfoType( "CN", new string[]{"CN"}),     // Monedas euro,dollar, yen
      new InfoType( "NS", new string[]{"NS"}),     // Dia de la semana lunes,martes, etc
      new InfoType( "LL", new string[]{"LL"}),     // Una letra en minusculas
      new InfoType( "LU", new string[]{"LU"}),     // Una letra en mayusculas
      new InfoType( "PA", new string[]{"PA"}),     // 
      new InfoType( "SA", new string[]{"SA"}),     // Sustantivo que se forma de palabras con guion y fuerza que la proxima palabra es sust o adj

      new InfoType( "XJ", new string[]{"SS","AA"                    }), 
      new InfoType( "ZB", new string[]{"SS","AA","VT"               }), 
      new InfoType( "XB", new string[]{"SS","AA","VT","VI"          }), 
      new InfoType( "TA", new string[]{"SS","AA","VT","VR"          }), 
      new InfoType( "UR", new string[]{"SS","AA","VT","VI","DD"     }), 
      new InfoType( "QF", new string[]{"SS","AA","VT","VI","VR"     }), 
      new InfoType( "QM", new string[]{"SS","AA","VT","VI","DD","PP"}), 
      new InfoType( "UO", new string[]{"SS","AA","VI"               }), 
      new InfoType( "XH", new string[]{"SS","AA","DD"               }), 
      new InfoType( "XA", new string[]{"SS","AA","DD","VT"          }), 
      new InfoType( "UV", new string[]{"SS","AA","NP"               }), 
      new InfoType( "XQ", new string[]{"SS","AA","VR","DD","VT"     }), 
      new InfoType( "UT", new string[]{"SS","AA","GT"               }), 
      new InfoType( "UX", new string[]{"SS","AA","GI"               }), 
      new InfoType( "QC", new string[]{"SS","AA","PT"               }), 
      new InfoType( "QD", new string[]{"SS","AA","PI"               }), 
      new InfoType( "QL", new string[]{"SS","AA","DD","PP"          }), 
//      new InfoType( "UM", new string[]{"SS","AA","GT","GI"          }), 
      new InfoType( "UM", new string[]{"SS","AA","VT","VI"          }), 
      new InfoType( "QB", new string[]{"SS","AA","PT","PI"          }), 
      new InfoType( "YF", new string[]{"SS","VT"                    }), 
      new InfoType( "SV", new string[]{"SS","VT","VI"               }), 
      new InfoType( "QA", new string[]{"SS","VT","VI","DD"          }), 
      new InfoType( "QN", new string[]{"SS","VT","VI","DD","AI"     }), 
      new InfoType( "ZJ", new string[]{"SS","VT","VR"               }), 
      new InfoType( "ZF", new string[]{"SS","VT","VI","VR"          }), 
      new InfoType( "UY", new string[]{"SS","VT","VI","NP"          }), 
      new InfoType( "QK", new string[]{"SS","VT","VA"               }), 
      new InfoType( "XG", new string[]{"SS","VI"                    }), 
      new InfoType( "ZZ", new string[]{"SS","VI","VR"               }),
//      new InfoType( "UI", new string[]{"SS","GT","GI"               }), 
      new InfoType( "UI", new string[]{"SS","VT","VI"               }), 
//      new InfoType( "UZ", new string[]{"SS","PT","PI"               }), 
      new InfoType( "UZ", new string[]{"SS","VT","VI"               }), 
//      new InfoType( "UL", new string[]{"SS","HT","HI"               }), 
      new InfoType( "UL", new string[]{"SS","VT","HI"               }), 
      new InfoType( "YG", new string[]{"SS","VR"                    }), 
      new InfoType( "XK", new string[]{"SS","DD"                    }), 
      new InfoType( "QJ", new string[]{"SS","VA"                    }), 
      new InfoType( "QO", new string[]{"SS","AI","DD"               }), 
      new InfoType( "UA", new string[]{"SS","NP"                    }), 
      new InfoType( "UC", new string[]{"SS","PT"                    }), 
      new InfoType( "UE", new string[]{"SS","GT"                    }), 
      new InfoType( "UJ", new string[]{"SS","GI"                    }), 
      new InfoType( "UF", new string[]{"SS","HT"                    }), 
      new InfoType( "UG", new string[]{"SS","HI"                    }), 
      new InfoType( "ZI", new string[]{"AA","VT"                    }), 
      new InfoType( "XI", new string[]{"AA","VT","VI"               }), 
      new InfoType( "QE", new string[]{"AA","VT","VI","DD"          }), 
      new InfoType( "XD", new string[]{"AA","VT","DD"               }), 
      new InfoType( "XC", new string[]{"AA","VI","DD"               }), 
      new InfoType( "XE", new string[]{"AA","VI"                    }), 
      new InfoType( "XM", new string[]{"AA","DD"                    }), 
      new InfoType( "XO", new string[]{"AA","NP"                    }), 
//      new InfoType( "UK", new string[]{"AA","GT","GI"               }), 
      new InfoType( "UK", new string[]{"AA","VT","VI"               }), 
      new InfoType( "UD", new string[]{"AA","GT"                    }), 
      new InfoType( "UN", new string[]{"AA","GI"                    }), 
//      new InfoType( "UH", new string[]{"AA","PT","PI"               }), 
      new InfoType( "UH", new string[]{"AA","VT","VI"               }), 
      new InfoType( "UB", new string[]{"AA","PT"                    }), 
      new InfoType( "UW", new string[]{"AA","PI"                    }), 
      new InfoType( "VV", new string[]{"VT","VI"                    }), 
      new InfoType( "XP", new string[]{"VT","VI","VR","DD"          }), 
      new InfoType( "QG", new string[]{"VT","VI","DD"               }), 
      new InfoType( "ZG", new string[]{"VT","VI","NP"               }), 
      new InfoType( "ZV", new string[]{"VT","VI","VR"               }), 
      new InfoType( "XF", new string[]{"VT","DD"                    }), 
      new InfoType( "ZH", new string[]{"VT","VR"                    }), 

      new InfoType( "XY", new string[]{"DD","AI"                    }), 
      new InfoType( "XL", new string[]{"DD","VR"                    }), 
      new InfoType( "QI", new string[]{"DD","PP"                    }), 
      new InfoType( "QH", new string[]{"DD","HT"                    }), 
      new InfoType( "MV", new string[]{"NN","AN"                    }) 
      };



    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el nombre de un tipo gramatical en un idioma solicitado</summary>
    ///<param name="Type">Código númerico del tipo gramatical que se quiere obtener el nombre</param>
    ///<param name="Lng" >Código númerico del idioma para el nombre</param>
    ///<param name="Abrv" >Indica que lo que se quiere obtener es una abreviatura del nombre</param>
    //------------------------------------------------------------------------------------------------------------------
    static public string Name( int Type, TLng Lng, bool Abrv )
      {
      int nTypes = Names.GetLength(0) - 2;                          // Número de nombres de tipos simples definidos
      int nLangs = Names.GetLength(1);                              // Número de idiomas definido para cada tipo

      if( Type >= nTypes  )                                         // Es mayor que el número de tipos simples
        {
        if( Type < Info.Length  )                                   // Esta dentro del rango de tipos con información
          Type = nTypes;                                            // Es un tipo compuesto
        else                                                        // Si no
          Type = nTypes + 1;                                        // Es un tipo desconocido
        }

      if( Type<0 ) Type = nTypes + 1;                               // Es negativo, tipo desconocido

      if( Lng<0 || (int)Lng>=nLangs ) Lng = TLng.En;                // Idioma fuera de rango, toma Inglés por defecto

      if( Abrv )
        return Abrev[ Type, (int)Lng ];                             // Retorna abreviatura del nombre del tipo
      else
        return Names[ Type, (int)Lng ];                             // Retorna nombre completo del tipo
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el nombre de un tipo gramatical en un idioma solicitado</summary>
    ///<param name="sType">Código de dos letras del tipo gramatical que se quiere obtener el nombre</param>
    ///<param name="Lng"  >Código númerico del idioma para el nombre</param>
    ///<param name="Abrv" >Indica que lo que se quiere obtener es una abreviatura del nombre</param>
    //------------------------------------------------------------------------------------------------------------------
    static public string Name( string sType, TLng Lng, bool Abrv )
      {
      return Name( Code(sType), Lng, Abrv);                         // Obtiene indice del tipo y retorna el nombre
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el código númerico de un tipo representado por su código de dos letras</summary>
    ///<param name="sType">Código de dos letras del tipo gramatical que se quiere obtener</param>
    ///<returns>Retorna código númerico del tipo, si 'sType' no existe retorna -1</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public int Code( string sType )
      {
      for( int i=0; i<Info.Length; ++i )                            // Busca por info para todos los tipos
        if( Info[i].Type == sType )                                 // Si el tipo
          return i;                                                 // Retorna el indice

      return -1;                                                    // Retorna el indice
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el código dos letras de un tipo gramatical conociendo su código númerico</summary>
    ///<param name="Type">Código númerico del tipo gramatical que se quiere obtener</param>
    ///<returns>Retorna código de dos letras del tipo, si no se puedo obtener retorna codigo en blanco</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public string Code(int Type )
      {
      if( Type<0 || Type >= Info.Length  )                          // Esta fuera del rango de tipos con información
        return "  ";                                                // Retorna tipo en blanco  

      return Info[Type].Type;                                       // Retorna código del tipo
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si un tipo es simple o no</summary>
    ///<param name="sType">Código de dos letras del tipo gramatical</param>
    ///<returns>Retorna si el tipo es simple o no (compuesto)</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public bool IsSingle(string sType )
      {
      for( int i=0; i<Info.Length; ++i )                            // Busca por info para todos los tipos
        {
        if( Info[i].SubTypes.Length > 1 ) return false;             // No es un tipo simple (mas de un subtipo)
        if( Info[i].Type == sType       ) return true;              // Si el tipo solicitado, retorna OK
        }

      return false;                                                 // Retorna false, (Nunca pasa por aqui)
      }
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Con una lista de tipos simples, obtiene el tipo compuesto que los representa</summary>
    ///<param name="Types">Lista de tipos simples que forman un tipo compuesto</param>
    ///<returns>Retorna código de dos letras del tipo compuesto, si no se puedo obtener retorna codigo en blanco</returns>
    //------------------------------------------------------------------------------------------------------------------
    static public string GetCompoundType( List<string> Types )
      {  
      if( Types.Count ==1 ) return Types[0];                        // Es un tipo simple

      for( int i=0; i<Info.Length; ++i )                            // Busca en la lista de info, de tipos
        if( CompareType( i, Types ) )                               // Compara todos los subtipos 
          return Info[i].Type;                                      // Retorna tipo complejo

      return "  ";                                                  // No lo encontro
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Dada una lista de tipos y un tipo simple, determina si este es valido o no</summary>
    ///<param name="type" >Tipo simple que se quiere investigar</param>
    ///<param name="types">Lista de tipos que ya tiene la palabra</param>
    ///<returns>Si el tipo es valido o no</returns> 
    //------------------------------------------------------------------------------------------------------------------
    static public bool IsValidType( string type, List<string> types ) 
      {
      for( int i=0; i<Info.Length; ++i )                            // Para todos los tipos en la información de tipos
        {
        if( Info[i].SubTypes.Length <= types.Count )                // Tiene menos subtipos que lista de entrada
          continue;                                                 // No lo analiza

        List<string> lst = Remainder( i, types );                   // Obtiene subtipos que no estan en lista de entrada

        foreach( string t in lst )                                  // Para cada subtipos encontrados
          if( t == type ) return true;                              // Si el tipo investigado, return OK,
        }
                                                                    
      return false;                                                 // Recorrio todos los tipos y no encontro ninguno
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Dada una lista de tipos, obtiene un listado de los posibles tipos que pueden adiconarse a ella</summary>
    ///<param name="types">Lista de tipos de entrada</param>
    ///<returns>Lista de los posibles tipos que puedena adicionarse a la lista en entrada</returns> 
    //------------------------------------------------------------------------------------------------------------------
    static public List<string> GetValidTypes( List<string> types ) 
      {
      List<string> ret = new List<string>();

      for( int i=0; i<Info.Length; ++i )                            // Para todos los tipos en la información de tipos
        {
        if( Info[i].SubTypes.Length <= types.Count )                // Tiene menos subtipos que lista de entrada
          continue;                                                 // No lo analiza

        List<string> lst = Remainder( i, types );                   // Obtiene subtipos que no estan en types

        foreach( string t in lst )                                  // Para cada subtipos en la lista
          if( !ret.Contains(t) )                                    // No esta en la lista de salida
            ret.Add(t);                                             // Lo adicciona a la lista de salida
        }
                                                                    
      return ret;                                                   // Lista de tipos validos para adicionar a 'types'
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Determina si un tipo simple es un subtipo de un tipo compuesto</summary>
    ///<param name="cType">Tipo compuesto</param>
    ///<param name="sType">Tipo simple</param>
    ///<returns>Retorna si 'sType' es un subtipo de cType o no</returns> 
    //------------------------------------------------------------------------------------------------------------------
    static public bool IsSubType( string cType, string sType ) 
      {
      int Idx = Code(cType);
      if( Idx == -1 ) return false;

      return Info[Idx].SubTypes.Contains(sType);
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Dado un tipo compuesto y una lista de tipos, obtiene el tipo simple que falta</summary>
    ///<param name="cType">Tipo compuesto</param>
    ///<param name="types">Lista de tipos que forman parcialmente el tipo 'cType'</param>
    ///<returns>Si types contiene todos los subtipos de cTypes menos uno, retorna el que falta, de lo contrario retorna
    ///cadena vacia</returns> 
    //------------------------------------------------------------------------------------------------------------------
    static public string GetDeafultType( string cType, IList<string> types ) 
      {
      List<string> lst = Remainder( Code(cType), types );           // Obtiene subtipos que no estan en types
                                                                    
      return (lst.Count==1)? lst[0] : "";                           // Si hay uno solo lo retorna, si no, retorna vacio
      }

    //------------------------------------------------------------------------------------------------------------------
    //Compara todos los subtipos del tipo 'i' en la información de tipos, con la lista de tipos 'Types' y si ambos 
    //estan formados por los mismos tipos retorna true, de lo contrario retorna false
    //------------------------------------------------------------------------------------------------------------------
    static bool CompareType( int i, IList<string> Types )
      { 
      if( Info[i].SubTypes.Length != Types.Count )                  // No tiene la misma cantidad de subtipos
        return false;                                               // No son iguales

      foreach( string t1 in Types )                                 // Para cada subtipos de la lista de entrada
        if( !Types.Contains(t1) )                                   // Si el subtipo esta dentro de la lista      
          return false;                                             // No son iguales

      return true;                                                  // Encontro todos los subtitulos, retorna OK
      }

    //------------------------------------------------------------------------------------------------------------------
    // Dado una lista de tipos simples 'Lst' y el indice a un tipo de la lista de información de tipos 'Idx' determina
    // cuales son los tipos que faltan
    //------------------------------------------------------------------------------------------------------------------
    static List<string> Remainder( int idx, IList<string> Lst1 )
      {
      List<string> Lst2 = new List<string>();
      
      foreach( string t in Lst1 )                                   // Para todos los tipos de la lista de entrada
        if( !Info[idx].SubTypes.Contains(t) )                       // El tipo no esta contenido en la lista de subtipos
          return Lst2;                                              // Retorna lista vacia (Lst no es un posible tipo idx)

      foreach( string t in Info[idx].SubTypes )                     // Para todos los subtipos de idx
        if( !Lst1.Contains(t) )                                     // Subtipo no contenido en la lista de entrada
          Lst2.Add(t);                                              // Lo adiciona a la lista de salida

      return Lst2;                                                  // Retorna lista de salida
      }

    //------------------------------------------------------------------------------------------------------------------
    }
  }
