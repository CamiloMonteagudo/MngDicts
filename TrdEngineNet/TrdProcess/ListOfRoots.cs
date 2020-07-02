using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrdEngine.Data;
using TrdEngine.Dictionary;

namespace TrdEngine.TrdProcess
  {
  public class ListOfRoots
    {
    static Dictionary<string,string> TblRoots = new Dictionary<string,string>
    { 
    // SP Sustantivo P
    /*DD*/ {"DD p S 3 P 00", "DD p M S 3 P 00 S" },
    /*OI*/ {"OI p S 3 P 00", "OI p M S 3 P 00 S" },
    /*OO*/ {"OO p S 3 P 00", "OO p M S 3 P 00 S" },
//  /*OO*/ {"OO p S 3 P 00", "OO p F S 3 P 00 S" },
    /*OO*/ {"OO p P 3 P 00", "OO p M P 3 P 00 S" },
//  /*OO*/ {"OO p P 3 P 00", "OO p F P 3 P 00 S" },
//  /*OO*/ {"OO p S 3 P 00", "OO p N S 3 P 00 S" },
//  /*OO*/ {"OO p P 3 P 00", "OO p N P 3 P 00 S" },
    /*AA*/ {"AA p S 3 P 00", "AA p N S 3 P 00 S" },
//  /*AA*/ {"AA p S 3 P 00", "AA p M S 3 P 00 S" },
//  /*AA*/ {"AA p S 3 P 00", "AA p F S 3 P 00 S" },
    /*AA*/ {"AA p P 3 P 00", "AA p N P 3 P 00 P" },
//  /*AA*/ {"AA p P 3 P 00", "AA p M P 3 P 00 P" },
//  /*AA*/ {"AA p P 3 P 00", "AA p F P 3 P 00 P" },
    /*AA*/ {"AI p S 3 P 00", "AI p N S 3 P 00 S" },
//  /*AI*/ {"AI p S 3 P 00", "AI p M S 3 P 00 S" },
//  /*AI*/ {"AI p S 3 P 00", "AI p F S 3 P 00 S" },
    /*AI*/ {"AI p P 3 P 00", "AI p N P 3 P 00 P" },
//  /*AI*/ {"AI p P 3 P 00", "AI p M P 3 P 00 P" },
//  /*AI*/ {"AI p P 3 P 00", "AI p F P 3 P 00 P" },
    /*AL*/ {"AL p S 3 P 00", "AL p M S 3 P 00 S" },
//  /*AL*/ {"AL p S 3 P 00", "AL p F S 3 P 00 S" },
    /*AL*/ {"AL p P 3 P 00", "AL p N P 3 P 00 P" },
//  /*AL*/ {"AL p P 3 P 00", "AL p M P 3 P 00 P" },
//  /*AL*/ {"AL p P 3 P 00", "AL p F P 3 P 00 P" },
    /*AO*/ {"AO p S 3 P 00", "AO p M S 3 P 00 S" },
//  /*AO*/ {"AO p S 3 P 00", "AO p F S 3 P 00 S" },
    /*AO*/ {"AO p P 3 P 00", "AO p M P 3 P 00 P" },
//  /*AO*/ {"AO p P 3 P 00", "AO p F P 3 P 00 P" },
//  /*AO*/ {"AO p S 3 P 00", "AO p N S 3 P 00 S" },
//  /*AO*/ {"AO p P 3 P 00", "AO p N P 3 P 00 P" },
    /*RD*/ {"RD p S 3 P 00", "RD p N S 3 P 00 S" },
//  /*RD*/ {"RD p S 3 P 00", "RD p M S 3 P 00 S" },
//  /*RD*/ {"RD p S 3 P 00", "RD p F S 3 P 00 S" },
    /*RD*/ {"RD p P 3 P 00", "RD p N P 3 P 00 P" },
//  /*RD*/ {"RD p P 3 P 00", "RD p M P 3 P 00 P" },
//  /*RD*/ {"RD p P 3 P 00", "RD p F P 3 P 00 P" },
    /*RI*/ {"RI p S 3 P 00", "RI p N S 3 P 00 S" },
//  /*RI*/ {"RI p S 3 P 00", "RI p M S 3 P 00 S" },
//  /*RI*/ {"RI p S 3 P 00", "RI p F S 3 P 00 S" },
    /*RI*/ {"RI p P 3 P 00", "RI p N P 3 P 00 P" },
//  /*RI*/ {"RI p P 3 P 00", "RI p M P 3 P 00 P" },
//  /*RI*/ {"RI p P 3 P 00", "RI p F P 3 P 00 P" },
    /*AC*/ {"AA c S 3 P 00", "AC p N S 3 P 00 S" },
//  /*AC*/ {"AA c S 3 P 00", "AC p M S 3 P 00 S" },
//  /*AC*/ {"AA c S 3 P 00", "AC p F S 3 P 00 S" },
    /*AC*/ {"AA c P 3 P 00", "AC p N P 3 P 00 P" },
//  /*AC*/ {"AA c P 3 P 00", "AC p M P 3 P 00 P" },
//  /*AC*/ {"AA c P 3 P 00", "AC p F P 3 P 00 P" },
    /*AC*/ {"AI c S 3 P 00", "AC p M S 3 P 00 S" },
//  /*AC*/ {"AI c S 3 P 00", "AC p F S 3 P 00 S" },
    /*AC*/ {"AI c P 3 P 00", "AC p M P 3 P 00 P" },
//  /*AC*/ {"AI c P 3 P 00", "AC p F P 3 P 00 P" },
    /*AS*/ {"AA s S 3 P 00", "AS p N S 3 P 00 S" },
//  /*AS*/ {"AA s S 3 P 00", "AS p M S 3 P 00 S" },
//  /*AS*/ {"AA s S 3 P 00", "AS p F S 3 P 00 S" },
    /*AS*/ {"AA s P 3 P 00", "AS p N P 3 P 00 P" },
//  /*AS*/ {"AA s P 3 P 00", "AS p M P 3 P 00 P" },
//  /*AS*/ {"AA s P 3 P 00", "AS p F P 3 P 00 P" },
    /*AS*/ {"AI s S 3 P 00", "AS p N S 3 P 00 S" },
//  /*AS*/ {"AI s S 3 P 00", "AS p M S 3 P 00 S" },
//  /*AS*/ {"AI s S 3 P 00", "AS p F S 3 P 00 S" },
    /*AS*/ {"AI s P 3 P 00", "AS p N P 3 P 00 P" },
//  /*AS*/ {"AI s P 3 P 00", "AS p M P 3 P 00 P" },
//  /*AS*/ {"AI s P 3 P 00", "AS p F P 3 P 00 P" },
                                                 
    /*SP*/ {"SS p P 3 P 00", "SP p M P 3 P 00 P" },
//  /*SP*/ {"SS p P 3 P 00", "SP p M P 3 P 00 P" },
    /*SS*/ {"SS p S 3 P 00", "SS p M S 3 P 00 S" },
//  /*SS*/ {"SS p S 3 P 00", "SS p F S 3 P 00 S" },

    // para el verbo be , ser, essere - OJO falta por convertir el modo indicativo a BI y tener en cuenta la unión del inglés
    /*BE*/ {"BE p S 3 P 00", "BE p M S 3 P 00 S" },
    /*BE*/ {"BE p S 1 P 01", "BE p M S 1 P 01 S" },
    /*BE*/ {"BE p S 2 P 01", "BE p M S 2 P 01 S" },
    /*BE*/ {"BE p S 3 P 01", "BE p M S 3 P 01 S" },
    /*BE*/ {"BE p P 1 P 01", "BE p M P 1 P 01 S" },
    /*BE*/ {"BE p P 2 P 01", "BE p M P 2 P 01 S" },
    /*BE*/ {"BE p P 3 P 01", "BE p M P 3 P 01 S" },
    /*BE*/ {"BE p S 1 p 01", "BE p M S 1 p 01 S" },
    /*BE*/ {"BE p S 2 p 01", "BE p M S 2 p 01 S" },
    /*BE*/ {"BE p S 3 p 01", "BE p M S 3 p 01 S" },
    /*BE*/ {"BE p P 1 p 01", "BE p M P 1 p 01 S" },
    /*BE*/ {"BE p P 2 p 01", "BE p M P 2 p 01 S" },
    /*BE*/ {"BE p P 3 p 01", "BE p M P 3 p 01 S" },
    /*BE*/ {"BE p S 1 I 01", "BE p M S 1 I 01 S" },
    /*BE*/ {"BE p S 2 I 01", "BE p M S 2 I 01 S" },
    /*BE*/ {"BE p S 3 I 01", "BE p M S 3 I 01 S" },
    /*BE*/ {"BE p P 1 I 01", "BE p M P 1 I 01 S" },
    /*BE*/ {"BE p P 2 I 01", "BE p M P 2 I 01 S" },
    /*BE*/ {"BE p P 3 I 01", "BE p M P 3 I 01 S" },
    /*BE*/ {"BE p S 1 F 01", "BE p M S 1 F 01 S" },
    /*BE*/ {"BE p S 2 F 01", "BE p M S 2 F 01 S" },
    /*BE*/ {"BE p S 3 F 01", "BE p M S 3 F 01 S" },
    /*BE*/ {"BE p P 1 F 01", "BE p M P 1 F 01 S" },
    /*BE*/ {"BE p P 2 F 01", "BE p M P 2 F 01 S" },
    /*BE*/ {"BE p P 3 F 01", "BE p M P 3 F 01 S" },
    /*BS*/ {"BE p S 1 P 04", "BS p M S 1 P 04 S" },
    /*BS*/ {"BE p S 2 P 04", "BS p M S 2 P 04 S" },
    /*BS*/ {"BE p S 3 P 04", "BS p M S 3 P 04 S" },
    /*BS*/ {"BE p P 1 P 04", "BS p M P 1 P 04 S" },
    /*BS*/ {"BE p P 2 P 04", "BS p M P 2 P 04 S" },
    /*BS*/ {"BE p P 3 P 04", "BS p M P 3 P 04 S" },
    /*BS*/ {"BE p S 1 p 04", "BS p M S 1 p 04 S" },
    /*BS*/ {"BE p S 2 p 04", "BS p M S 2 p 04 S" },
    /*BS*/ {"BE p S 3 p 04", "BS p M S 3 p 04 S" },
    /*BS*/ {"BE p P 1 p 04", "BS p M P 1 p 04 S" },
    /*BS*/ {"BE p P 2 p 04", "BS p M P 2 p 04 S" },
    /*BS*/ {"BE p P 3 p 04", "BS p M P 3 p 04 S" },
    /*BM*/ {"BE p S 1 P 02", "BM p M S 1 P 02 S" },
    /*BM*/ {"BE p S 2 P 02", "BM p M S 2 P 02 S" },
    /*BM*/ {"BE p S 3 P 02", "BM p M S 3 P 02 S" },
    /*BM*/ {"BE p P 1 P 02", "BM p M P 1 P 02 S" },
    /*BM*/ {"BE p P 2 P 02", "BM p M P 2 P 02 S" },
    /*BM*/ {"BE p P 3 P 02", "BM p M P 3 P 02 S" },
                                                 
    /*BP*/ {"BE p S 1 P 03", "BP p M S 1 P 03 S" },
    /*BP*/ {"BE p S 2 P 03", "BP p M S 2 P 03 S" },
    /*BP*/ {"BE p S 3 P 03", "BP p M S 3 P 03 S" },
    /*BP*/ {"BE p P 1 P 03", "BP p M P 1 P 03 S" },
    /*BP*/ {"BE p P 2 P 03", "BP p M P 2 P 03 S" },
    /*BP*/ {"BE p P 3 P 03", "BP p M P 3 P 03 S" },
                                                 
    /*BP*/ {"BE p S 1 p 03", "BP p M S 1 p 03 S" },
    /*BP*/ {"BE p S 2 p 03", "BP p M S 2 p 03 S" },
    /*BP*/ {"BE p S 3 p 03", "BP p M S 3 p 03 S" },
    /*BP*/ {"BE p P 1 p 03", "BP p M P 1 p 03 S" },
    /*BP*/ {"BE p P 2 p 03", "BP p M P 2 p 03 S" },
    /*BP*/ {"BE p P 3 p 03", "BP p M P 3 p 03 S" },
                                                 
    /*BD*/ {"BE p P 2 p 14", "BD p M P 2 p 14 S" },
    /*BF*/ {"BE p S 3 P 12", "BF p M S 3 P 12 S" },
    /*BH*/ {"BE p P 1 P 08", "BH p M P 1 P 08 S" },
    /*BJ*/ {"BE p P 2 P 09", "BJ p M P 2 P 09 S" },
    /*BK*/ {"BE p S 3 P 07", "BK p M S 3 P 07 S" },
    /*BK*/ {"BE p P 2 P 07", "BK p M P 2 P 07 S" },
    /*BK*/ {"BE p P 3 P 07", "BK p M P 3 P 07 S" },
    /*BN*/ {"BE p S 3 P 06", "BN p M S 3 P 06 S" },
    /*BN*/ {"BE p P 3 P 06", "BN p M P 3 P 06 S" },
//  /*BN*/ {"BE p S 3 P 06", "BN p F S 3 P 06 S" },
//  /*BN*/ {"BE p P 3 P 06", "BN p F P 3 P 06 S" },
    /*BG*/ {"BE p S 3 P 05", "BG p M S 3 P 05 S" },
                                                     
    // para el verbo haver, haber, avere             
    /*HA*/ {"HA p S 3 P 00", "HA p M S 3 P 00 S" },
    /*HA*/ {"HA p S 1 P 01", "HA p M S 1 P 01 S" },
    /*HA*/ {"HA p S 2 P 01", "HA p M S 2 P 01 S" },
    /*HA*/ {"HA p S 3 P 01", "HA p M S 3 P 01 S" },
    /*HA*/ {"HA p P 1 P 01", "HA p M P 1 P 01 S" },
    /*HA*/ {"HA p P 2 P 01", "HA p M P 2 P 01 S" },
    /*HA*/ {"HA p P 3 P 01", "HA p M P 3 P 01 S" },
    /*HA*/ {"HA p S 1 p 01", "HA p M S 1 p 01 S" },
    /*HA*/ {"HA p S 2 p 01", "HA p M S 2 p 01 S" },
    /*HA*/ {"HA p S 3 p 01", "HA p M S 3 p 01 S" },
    /*HA*/ {"HA p P 1 p 01", "HA p M P 1 p 01 S" },
    /*HA*/ {"HA p P 2 p 01", "HA p M P 2 p 01 S" },
    /*HA*/ {"HA p P 3 p 01", "HA p M P 3 p 01 S" },
    /*HA*/ {"HA p S 1 I 01", "HA p M S 1 I 01 S" },
    /*HA*/ {"HA p S 2 I 01", "HA p M S 2 I 01 S" },
    /*HA*/ {"HA p S 3 I 01", "HA p M S 3 I 01 S" },
    /*HA*/ {"HA p P 1 I 01", "HA p M P 1 I 01 S" },
    /*HA*/ {"HA p P 2 I 01", "HA p M P 2 I 01 S" },
    /*HA*/ {"HA p P 3 I 01", "HA p M P 3 I 01 S" },
    /*HA*/ {"HA p S 1 F 01", "HA p M S 1 F 01 S" },
    /*HA*/ {"HA p S 2 F 01", "HA p M S 2 F 01 S" },
    /*HA*/ {"HA p S 3 F 01", "HA p M S 3 F 01 S" },
    /*HA*/ {"HA p P 1 F 01", "HA p M P 1 F 01 S" },
    /*HA*/ {"HA p P 2 F 01", "HA p M P 2 F 01 S" },
    /*HA*/ {"HA p P 3 F 01", "HA p M P 3 F 01 S" },
    /*VC*/ {"HA p S 3 P 06", "VC p M S 1 P 06 S" },
    /*VP*/ {"HA p P 3 P 06", "VP p M P 1 P 06 S" },
//  /*VP*/ {"HA p S 3 P 06", "VP p F S 1 P 06 S" },
//  /*VP*/ {"HA p P 3 P 06", "VP p F P 1 P 06 S" },
    /*VG*/ {"HA p S 3 P 05", "VG p M S 1 P 05 S" },
    /*HM*/ {"HA p S 1 P 02", "HM p M S 1 P 02 S" },
    /*HM*/ {"HA p S 2 P 02", "HM p M S 2 P 02 S" },
    /*HM*/ {"HA p S 3 P 02", "HM p M S 3 P 02 S" },
    /*HM*/ {"HA p P 1 P 02", "HM p M P 1 P 02 S" },
    /*HM*/ {"HA p P 2 P 02", "HM p M P 2 P 02 S" },
    /*HM*/ {"HA p P 3 P 02", "HM p M P 3 P 02 S" },
                                                 
    /*HJ*/ {"HA p S 1 P 04", "HJ p M S 1 P 04 S" },
    /*HJ*/ {"HA p S 2 P 04", "HJ p M S 2 P 04 S" },
    /*HJ*/ {"HA p S 3 P 04", "HJ p M S 3 P 04 S" },
    /*HJ*/ {"HA p P 1 P 04", "HJ p M P 1 P 04 S" },
    /*HJ*/ {"HA p P 2 P 04", "HJ p M P 2 P 04 S" },
    /*HJ*/ {"HA p P 3 P 04", "HJ p M P 3 P 04 S" },
    /*HJ*/ {"HA p S 1 p 04", "HJ p M S 1 p 04 S" },
    /*HJ*/ {"HA p S 2 p 04", "HJ p M S 2 p 04 S" },
    /*HJ*/ {"HA p S 3 p 04", "HJ p M S 3 p 04 S" },
    /*HJ*/ {"HA p P 1 p 04", "HJ p M P 1 p 04 S" },
    /*HJ*/ {"HA p P 2 p 04", "HJ p M P 2 p 04 S" },
    /*HJ*/ {"HA p P 3 p 04", "HJ p M P 3 p 04 S" },
    /*HP*/ {"HA p S 1 P 03", "HP p M S 1 P 03 S" },
    /*HP*/ {"HA p S 2 P 03", "HP p M S 2 P 03 S" },
    /*HP*/ {"HA p S 3 P 03", "HP p M S 3 P 03 S" },
    /*HP*/ {"HA p P 1 P 03", "HP p M P 1 P 03 S" },
    /*HP*/ {"HA p P 2 P 03", "HP p M P 2 P 03 S" },
    /*HP*/ {"HA p P 3 P 03", "HP p M P 3 P 03 S" },
                                                 
    /*HC*/ {"HA p P 2 p 14", "HC p M P 2 p 14 S" },
    /*HF*/ {"HA p P 1 P 08", "HF p M P 1 P 08 S" },
    /*HV*/ {"HA p S 3 P 07", "HV p M S 3 P 07 S" },
    /*HV*/ {"HA p P 2 P 07", "HV p M P 2 P 07 S" },
    /*HV*/ {"HA p P 3 P 07", "HV p M P 3 P 07 S" },
                                                     
    // para verbos auxiliares                        
    /*VA*/ {"VA p S 3 P 00", "VA p M S 3 P 00 S" },
    /*VA*/ {"VA p S 1 P 01", "VA p M S 1 P 01 S" },
    /*VA*/ {"VA p S 2 P 01", "VA p M S 2 P 01 S" },
    /*VA*/ {"VA p S 3 P 01", "VA p M S 3 P 01 S" },
    /*VA*/ {"VA p P 1 P 01", "VA p M P 1 P 01 S" },
    /*VA*/ {"VA p P 2 P 01", "VA p M P 2 P 01 S" },
    /*VA*/ {"VA p P 3 P 01", "VA p M P 3 P 01 S" },
    /*VA*/ {"VA p S 1 p 01", "VA p M S 1 p 01 S" },
    /*VA*/ {"VA p S 2 p 01", "VA p M S 2 p 01 S" },
    /*VA*/ {"VA p S 3 p 01", "VA p M S 3 p 01 S" },
    /*VA*/ {"VA p P 1 p 01", "VA p M P 1 p 01 S" },
    /*VA*/ {"VA p P 2 p 01", "VA p M P 2 p 01 S" },
    /*VA*/ {"VA p P 3 p 01", "VA p M P 3 p 01 S" },
    /*VA*/ {"VA p S 1 I 01", "VA p M S 1 I 01 S" },
    /*VA*/ {"VA p S 2 I 01", "VA p M S 2 I 01 S" },
    /*VA*/ {"VA p S 3 I 01", "VA p M S 3 I 01 S" },
    /*VA*/ {"VA p P 1 I 01", "VA p M P 1 I 01 S" },
    /*VA*/ {"VA p P 2 I 01", "VA p M P 2 I 01 S" },
    /*VA*/ {"VA p P 3 I 01", "VA p M P 3 I 01 S" },
    /*VA*/ {"VA p S 1 F 01", "VA p M S 1 F 01 S" },
    /*VA*/ {"VA p S 2 F 01", "VA p M S 2 F 01 S" },
    /*VA*/ {"VA p S 3 F 01", "VA p M S 3 F 01 S" },
    /*VA*/ {"VA p P 1 F 01", "VA p M P 1 F 01 S" },
    /*VA*/ {"VA p P 2 F 01", "VA p M P 2 F 01 S" },
    /*VA*/ {"VA p P 3 F 01", "VA p M P 3 F 01 S" },
    /*VC*/ {"VA p S 3 P 06", "VC p M S 1 P 06 S" },
    /*VU*/ {"VA p P 3 P 06", "VU p M P 1 P 06 S" },
//  /*VU*/ {"VA p S 3 P 06", "VU p F S 1 P 06 S" },
//  /*VU*/ {"VA p P 3 P 06", "VU p F P 1 P 06 S" },
    /*VY*/ {"VA p S 3 P 05", "VY p M S 1 P 05 S" },
    /*VW*/ {"VA p S 1 P 02", "VW p M S 1 P 02 S" },
    /*VW*/ {"VA p S 2 P 02", "VW p M S 2 P 02 S" },
    /*VW*/ {"VA p S 3 P 02", "VW p M S 3 P 02 S" },
    /*VW*/ {"VA p P 1 P 02", "VW p M P 1 P 02 S" },
    /*VW*/ {"VA p P 2 P 02", "VW p M P 2 P 02 S" },
    /*VW*/ {"VA p P 3 P 02", "VW p M P 3 P 02 S" },
    /*VB*/ {"VA p S 2 p 11", "VB p M S 2 p 11 S" },
                                                 
    /*VJ*/ {"VA p S 1 P 04", "VJ p M S 1 P 04 S" },
    /*VJ*/ {"VA p S 2 P 04", "VJ p M S 2 P 04 S" },
    /*VJ*/ {"VA p S 3 P 04", "VJ p M S 3 P 04 S" },
    /*VJ*/ {"VA p P 1 P 04", "VJ p M P 1 P 04 S" },
    /*VJ*/ {"VA p P 2 P 04", "VJ p M P 2 P 04 S" },
    /*VJ*/ {"VA p P 3 P 04", "VJ p M P 3 P 04 S" },
    /*VJ*/ {"VA p S 1 p 04", "VJ p M S 1 p 04 S" },
    /*VJ*/ {"VA p S 2 p 04", "VJ p M S 2 p 04 S" },
    /*VJ*/ {"VA p S 3 p 04", "VJ p M S 3 p 04 S" },
    /*VJ*/ {"VA p P 1 p 04", "VJ p M P 1 p 04 S" },
    /*VJ*/ {"VA p P 2 p 04", "VJ p M P 2 p 04 S" },
    /*VJ*/ {"VA p P 3 p 04", "VJ p M P 3 p 04 S" },
    /*VZ*/ {"VA p S 1 P 03", "VZ p M S 1 P 03 S" },
    /*VZ*/ {"VA p S 2 P 03", "VZ p M S 2 P 03 S" },
    /*VZ*/ {"VA p S 3 P 03", "VZ p M S 3 P 03 S" },
    /*VZ*/ {"VA p P 1 P 03", "VZ p M P 1 P 03 S" },
    /*VZ*/ {"VA p P 2 P 03", "VZ p M P 2 P 03 S" },
    /*VZ*/ {"VA p P 3 P 03", "VZ p M P 3 P 03 S" },
    /*VQ*/ {"VA p P 1 P 14", "VQ p M P 1 P 14 S" },
    /*VQ*/ {"VA p S 2 p 14", "VQ p M S 2 p 14 S" },
    /*VQ*/ {"VA p P 2 p 14", "VQ p M P 2 p 14 S" },
    /*VQ*/ {"VA p S 1 P 14", "VQ p M S 1 P 14 S" },
    /*VQ*/ {"VA p S 2 P 14", "VQ p M S 2 P 14 S" },
    /*VQ*/ {"VA p S 3 P 14", "VQ p M S 3 P 14 S" },
    /*VQ*/ {"VA p S 3 p 14", "VQ p M S 3 p 14 S" },
    /*VQ*/ {"VA p P 2 P 14", "VQ p M P 2 P 14 S" },
    /*VQ*/ {"VA p P 2 I 14", "VQ p M P 2 I 14 S" },
    /*VQ*/ {"VA p P 3 P 14", "VQ p M P 3 P 14 S" },
    /*VQ*/ {"VA p P 3 p 14", "VQ p M P 3 p 14 S" },
    /*VQ*/ {"VA p P 1 P 12", "VQ p M P 1 P 12 S" },
    /*VQ*/ {"VA p P 2 P 12", "VQ p M P 2 P 12 S" },
    /*VB*/ {"VA p P 3 P 09", "VB p F P 3 P 09 S" },
    /*VE*/ {"VA p S 3 P 08", "VE p M S 3 P 08 S" },
    /*VE*/ {"VA p S 1 P 08", "VE p M S 1 P 08 S" },
                                                 
    /*VE*/ {"VA p P 1 P 08", "VE p M P 1 P 08 S" },
    /*VF*/ {"VA p P 3 P 07", "VF p M P 3 P 07 S" },
    /*VH*/ {"VA p S 3 P 12", "VH p M S 3 P 12 S" },
                                                     
    // para verbos transitivos e intransitivos       
    /*VV*/ {"VV p S 3 P 00", "VV p M S 3 P 00 S" },
    /*VV*/ {"VV p S 1 P 01", "VV p M S 1 P 01 S" },
    /*VV*/ {"VV p S 2 P 01", "VV p M S 2 P 01 S" },
    /*VV*/ {"VV p S 3 P 01", "VV p M S 3 P 01 S" },
    /*VV*/ {"VV p P 1 P 01", "VV p M P 1 P 01 S" },
    /*VV*/ {"VV p P 2 P 01", "VV p M P 2 P 01 S" },
    /*VV*/ {"VV p P 3 P 01", "VV p M P 3 P 01 S" },
    /*VV*/ {"VV p S 1 p 01", "VV p M S 1 p 01 S" },
    /*VV*/ {"VV p S 2 p 01", "VV p M S 2 p 01 S" },
    /*VV*/ {"VV p S 3 p 01", "VV p M S 3 p 01 S" },
    /*VV*/ {"VV p P 1 p 01", "VV p M P 1 p 01 S" },
    /*VV*/ {"VV p P 2 p 01", "VV p M P 2 p 01 S" },
    /*VV*/ {"VV p P 3 p 01", "VV p M P 3 p 01 S" },
    /*VV*/ {"VV p S 1 I 01", "VV p M S 1 I 01 S" },
    /*VV*/ {"VV p S 2 I 01", "VV p M S 2 I 01 S" },
    /*VV*/ {"VV p S 3 I 01", "VV p M S 3 I 01 S" },
    /*VV*/ {"VV p P 1 I 01", "VV p M P 1 I 01 S" },
    /*VV*/ {"VV p P 2 I 01", "VV p M P 2 I 01 S" },
    /*VV*/ {"VV p P 3 I 01", "VV p M P 3 I 01 S" },
    /*VV*/ {"VV p S 1 F 01", "VV p M S 1 F 01 S" },
    /*VV*/ {"VV p S 2 F 01", "VV p M S 2 F 01 S" },
    /*VV*/ {"VV p S 3 F 01", "VV p M S 3 F 01 S" },
    /*VV*/ {"VV p P 1 F 01", "VV p M P 1 F 01 S" },
    /*VV*/ {"VV p P 2 F 01", "VV p M P 2 F 01 S" },
    /*VV*/ {"VV p P 3 F 01", "VV p M P 3 F 01 S" },
    /*VP*/ {"VV p S 3 P 06", "VP p M S 3 P 06 S" },
//  /*VP*/ {"VV p S 3 P 06", "VP p F S 1 P 06 S" },
    /*VP*/ {"VV p P 3 P 06", "VP p M P 1 P 06 S" },
//  /*VP*/ {"VV p P 3 P 06", "VP p F P 1 P 06 S" },
    /*VG*/ {"VV p S 3 P 05", "VG p M S 1 P 05 S" },
    /*VN*/ {"VV p S 1 P 02", "VN p M S 1 P 02 S" },
    /*VN*/ {"VV p S 2 P 02", "VN p M S 2 P 02 S" },
    /*VN*/ {"VV p S 3 P 02", "VN p M S 3 P 02 S" },
    /*VN*/ {"VV p P 1 P 02", "VN p M P 1 P 02 S" },
    /*VN*/ {"VV p P 2 P 02", "VN p M P 2 P 02 S" },
    /*VN*/ {"VV p P 3 P 02", "VN p M P 3 P 02 S" },
                                                 
    /*VK*/ {"VV p S 1 P 04", "VK p M S 1 P 04 S" },
    /*VK*/ {"VV p S 2 P 04", "VK p M S 2 P 04 S" },
    /*VK*/ {"VV p S 3 P 04", "VK p M S 3 P 04 S" },
    /*VK*/ {"VV p P 1 P 04", "VK p M P 1 P 04 S" },
    /*VK*/ {"VV p P 2 P 04", "VK p M P 2 P 04 S" },
    /*VK*/ {"VV p P 3 P 04", "VK p M P 3 P 04 S" },
    /*VK*/ {"VV p S 1 p 04", "VK p M S 1 p 04 S" },
    /*VK*/ {"VV p S 2 p 04", "VK p M S 2 p 04 S" },
    /*VK*/ {"VV p S 3 p 04", "VK p M S 3 p 04 S" },
    /*VK*/ {"VV p P 1 p 04", "VK p M P 1 p 04 S" },
    /*VK*/ {"VV p P 2 p 04", "VK p M P 2 p 04 S" },
    /*VK*/ {"VV p P 3 p 04", "VK p M P 3 p 04 S" },
    /*VM*/ {"VV p S 1 P 03", "VM p M S 1 P 03 S" },
    /*VM*/ {"VV p S 2 P 03", "VM p M S 2 P 03 S" },
    /*VM*/ {"VV p S 3 P 03", "VM p M S 3 P 03 S" },
    /*VM*/ {"VV p P 1 P 03", "VM p M P 1 P 03 S" },
    /*VM*/ {"VV p P 2 P 03", "VM p M P 2 P 03 S" },
    /*VM*/ {"VV p P 3 P 03", "VM p M P 3 P 03 S" },
    /*VO*/ {"VV p P 1 P 14", "VO p M P 1 P 14 S" },
    /*VO*/ {"VV p P 3 P 14", "VO p M P 3 P 14 S" },
    /*VO*/ {"VV p S 1 P 14", "VO p M S 1 P 14 S" },
    /*VO*/ {"VV p S 2 P 14", "VO p M S 2 P 14 S" },
    /*VO*/ {"VV p S 3 P 14", "VO p M S 3 P 14 S" },
    /*VO*/ {"VV p P 2 I 14", "VO p M P 2 I 14 S" },
                                                 
    /*VO*/ {"VV p S 3 p 14", "VO p M S 3 p 14 S" },
    /*VO*/ {"VV p P 2 p 14", "VO p M P 2 p 14 S" },
    /*VO*/ {"VV p P 2 P 14", "VO p M P 2 P 14 S" },
    /*VO*/ {"VV p S 2 p 14", "VO p M S 2 p 14 S" },
    /*VO*/ {"VV p P 3 p 14", "VO p M P 3 p 14 S" },
                                                 
    /*BU*/ {"VV p S 2 F 15", "BU p M S 2 F 15 S" },
    /*BO*/ {"VV p S 2 P 08", "BO p M S 2 P 08 S" },
    /*BX*/ {"VV p S 2 P 09", "BX p M S 2 P 09 S" },
                                                 
    /*BL*/ {"VV p P 1 P 12", "BL p M P 1 P 12 S" },          
    /*BL*/ {"VV p P 2 P 12", "BL p M P 2 P 12 S" },
    /*BL*/ {"VV p S 2 P 12", "BL p M S 2 P 12 S" },
    /*BL*/ {"VV p S 2 P 13", "BL p M S 2 P 13 S" },
//  /*BL*/ {"VV p S 2 P 12", "BL p M S 2 P 12 S" },
    /*BL*/ {"VV p S 3 I 12", "BL p M S 3 I 12 S" },
    /*BL*/ {"VV p S 3 P 12", "BL p M S 3 P 12 S" },
                                                 
    /*BZ*/ {"VV p S 2 P 07", "BZ p M S 2 P 07 S" },
    /*BZ*/ {"VV p S 3 P 07", "BZ p M S 3 P 07 S" },
    /*BZ*/ {"VV p P 1 P 07", "BZ p M P 1 P 07 S" },
    /*BZ*/ {"VV p P 2 P 07", "BZ p M P 2 P 07 S" },
    /*BZ*/ {"VV p P 3 P 07", "BZ p M P 3 P 07 S" },
    /*BO*/ {"VV p P 2 P 10", "BO p M P 2 P 10 S" },
    /*BO*/ {"VV p S 3 P 10", "BO p M S 3 P 10 S" },
    /*BO*/ {"VV p S 2 P 10", "BO p M S 2 P 10 S" },
    /*BO*/ {"VV p P 3 p 10", "BO p M P 3 p 10 S" },
    /*BO*/ {"VV p S 3 p 10", "BO p M S 3 p 10 S" },
    /*BO*/ {"VV p P 2 P 08", "BO p M P 2 P 08 S" },
    /*BO*/ {"VV p P 3 P 08", "BO p M P 3 P 08 S" },
                                                 
    /*BR*/ {"VV p S 1 p 11", "BR p M S 1 p 11 S" },
//  /*BR*/ {"VV p S 1 p 11", "BR p M S 1 p 11 S" },
    /*BR*/ {"VV p S 3 p 11", "BR p M S 3 p 11 S" },
    /*BR*/ {"VV p S 3 P 11", "BR p M S 3 P 11 S" },
    /*BR*/ {"VV p P 2 P 11", "BR p M P 2 P 11 S" },
    /*BX*/ {"VV p P 3 P 09", "BX p F P 3 P 09 S" },
                                                 
    /*BV*/ {"VV p S 1 P 16", "BV p M S 1 P 16 S" },
                                                 
    /*BY*/ {"VV p P 1 P 08", "BY p M P 1 P 08 S" },
    /*BY*/ {"VV p S 3 P 08", "BY p M S 3 P 08 S" },
    /*BY*/ {"VV p S 1 P 08", "BY p M S 1 P 08 S" },
    /*BY*/ {"VV p P 3 p 08", "BY p M P 3 p 08 S" },

    // Tipo de dos
    /*BE*/ {"BE p S 1 I 01|BE p S 3 I 01", "BE p M S 3 I 01 S" },
    /*BE*/ {"BE p S 3 P 01|BE p S 2 P 02", "BE p M S 3 P 01 S" },
    /*SS*/ {"SS p S 3 P 00|BE p P 2 P 02", "SS p M S 3 P 00 S" },
                                                               
    /*YR*/ {"SS p S 3 P 00|VV p P 2 P 02", "YR p M S 2 P 02 S" },
    /*QQ*/ {"SS p S 3 P 00|VV p P 3 P 14", "QQ p M P 3 P 14 S" },            
    /*QQ*/ {"SS p P 3 P 00|VV p S 2 P 14", "QQ p M S 2 P 14 P" },
    /*YT*/ {"SS p S 3 P 00|VV p P 1 P 12", "YT p M P 1 P 12 S" },            
                                                               
    /*YZ*/ {"SS p S 3 P 00|VV p S 3 P 11", "YZ p M S 3 P 11 S" }, 
    /*YW*/ {"SS p P 3 P 00|VV p S 2 P 10", "YW p M S 2 P 10 P" },
    /*QR*/ {"SS p P 3 P 00|VV p S 1 P 03", "QR p M S 1 P 03 P" },
    /*YT*/ {"SS p P 3 P 00|VV p P 1 P 12", "YT p M P 1 P 12 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 1 I 01", "YQ p M S 1 I 01 P" },
    /*QQ*/ {"SS p P 3 P 00|VV p P 1 I 14", "QQ p M P 1 I 14 P" },
    /*QQ*/ {"SS p S 3 P 00|VV p S 2 P 14", "QQ p M S 2 P 14 S" },
    /*YP*/ {"SS p S 3 P 00|VV p S 1 P 04", "YP p M S 1 P 04 S" },
                                                               
    /*YP*/ {"SS p S 3 P 00|VV p S 1 p 04", "YP p M S 1 p 04 S" },
    /*YP*/ {"SS p S 3 P 00|VV p S 2 P 04", "YP p M S 2 P 04 S" },
                                                               
    /*YP*/ {"SS p P 3 P 00|VV p P 1 P 04", "YP p M P 1 P 04 P" },
    /*YP*/ {"SS p P 3 P 00|VV p S 2 P 04", "YP p M S 2 P 04 P" },
    /*YP*/ {"SS p P 3 P 00|VV p S 2 p 04", "YP p M S 2 p 04 P" },
    /*YP*/ {"SS p P 3 P 00|VV p P 1 p 04", "YP p M P 1 p 04 P" },
                                                               
    /*XJ*/ {"SS p S 3 P 00|AA p S 3 P 00", "XJ p M S 3 P 00 S" },
//  /*XJ*/ {"SS p S 3 P 00|AA p S 3 P 00", "XJ p M S 3 P 00 S" },
    /*XJ*/ {"SS p S 3 P 00|AA p P 3 P 00", "XJ p M S 3 P 00 S" },
                                                               
    /*XJ*/ {"SS p P 3 P 00|AA p P 3 P 00", "XJ p M P 3 P 00 S" },
//  /*XJ*/ {"SS p S 3 P 00|AA p P 3 P 00", "XJ p M S 3 P 00 S" },
//  /*XJ*/ {"SS p P 3 P 00|AA p P 3 P 00", "XJ p M P 3 P 00 P" },
//  /*XJ*/ {"SS p P 3 P 00|AA p P 3 P 00", "XJ p M P 3 P 00 P" },
    /*XJ*/ {"SS p P 3 P 00|AA p S 3 P 00", "XJ p M S 3 P 00 P" },
    /*XJ*/ {"SS p S 3 P 00|AA s S 3 P 00", "XJ p M S 3 P 00 S" },
    /*XJ*/ {"SS p P 3 P 00|AA s P 3 P 00", "XJ p M P 3 P 00 P" },
//  /*XJ*/ {"SS p S 3 P 00|AA s S 3 P 00", "XJ p M S 3 P 00 S" },
//  /*XJ*/ {"SS p P 3 P 00|AA s P 3 P 00", "XJ p M P 3 P 00 P" },
                                                               
    /*YA*/ {"AA p S 3 P 00|VA p S 3 P 06", "YA p M S 3 P 06 S" },
    /*YA*/ {"AA p P 3 P 00|VA p P 3 P 06", "YA p M P 3 P 06 P" },
//  /*YA*/ {"AA p S 3 P 00|VA p S 3 P 06", "YA p F S 3 P 06 S" },            
//  /*YA*/ {"AA p P 3 P 00|VA p P 3 P 06", "YA p F P 3 P 06 P" },
    /*WD*/ {"SS p S 3 P 00|VA p S 2 p 11", "WD p M S 2 p 11 S" },
                                                               
    /*UH*/ {"AA p S 3 P 00|VV p S 3 P 06", "UH p M S 3 P 06 S" },
    /*UH*/ {"AA p P 3 P 00|VV p P 3 P 06", "UH p M P 3 P 06 P" },
//  /*UH*/ {"AA p S 3 P 00|VV p S 3 P 06", "UH p F S 3 P 06 S" },
//  /*UH*/ {"AA p P 3 P 00|VV p P 3 P 06", "UH p F P 3 P 06 P" },
                                                               
    /*UH*/ {"AA p S 3 P 00|VV p S 3 p 11", "UH p M S 3 p 06 S" },
    /*UH*/ {"AA p P 3 P 00|VV p S 3 p 11", "UH p M P 3 P 06 P" },
//  /*UH*/ {"AA p S 3 P 00|VV p S 3 p 11", "UH p F S 3 P 06 S" },
//  /*UH*/ {"AA p P 3 P 00|VV p S 3 p 11", "UH p F P 3 P 06 P" },
                                                               
    /*UK*/ {"AA p S 3 P 00|VV p S 3 P 05", "UK p M S 3 P 05 S" },
                                                               
    /*XI*/ {"AA p S 3 P 00|VV p S 3 P 00", "XI p M S 3 P 00 S" },
    /*XI*/ {"AA p P 3 P 00|VV p S 3 P 00", "XI p M S 3 P 00 P" },
//  /*XI*/ {"AA p S 3 P 00|VV p S 3 P 00", "XI p F S 3 P 00 S" },
//  /*XI*/ {"AA p P 3 P 00|VV p S 3 P 00", "XI p F S 3 P 00 P" },
                                                               
    /*YD*/ {"AA p S 3 P 00|VV p S 3 P 01", "YD p M S 3 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 3 P 01", "YD p M S 3 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 3 P 01", "YD p F S 3 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 3 P 01", "YD p F S 3 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 2 P 01", "YD p M S 2 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 2 P 01", "YD p M S 2 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 2 P 01", "YD p F S 2 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 2 P 01", "YD p F S 1 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 1 P 01", "YD p M S 1 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 1 P 01", "YD p M S 1 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 1 P 01", "YD p F S 1 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 1 P 01", "YD p F S 1 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 3 P 01", "YD p M P 3 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 3 P 01", "YD p M P 3 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 3 P 01", "YD p F P 3 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 3 P 01", "YD p F P 3 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 2 P 01", "YD p M P 2 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 2 P 01", "YD p M P 2 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 2 P 01", "YD p F P 2 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 2 P 01", "YD p F P 2 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 1 P 01", "YD p M P 1 P 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 1 P 01", "YD p M P 1 P 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 1 P 01", "YD p F P 1 P 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 1 P 01", "YD p F P 1 P 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 3 p 01", "YD p M S 3 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 3 p 01", "YD p M S 3 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 3 p 01", "YD p F S 3 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 3 p 01", "YD p F S 3 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 2 p 01", "YD p M S 2 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 2 p 01", "YD p M S 2 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 2 p 01", "YD p F S 2 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 2 p 01", "YD p F S 2 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 1 p 01", "YD p M S 1 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 1 p 01", "YD p M S 1 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 1 p 01", "YD p F S 1 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 1 p 01", "YD p F S 1 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 3 p 01", "YD p M P 3 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 3 p 01", "YD p M P 3 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 3 p 01", "YD p F P 3 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 3 p 01", "YD p F P 3 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 2 p 01", "YD p M P 2 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 2 p 01", "YD p M P 2 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 2 p 01", "YD p F P 2 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 2 p 01", "YD p F P 2 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 1 p 01", "YD p M P 1 p 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 1 p 01", "YD p M P 1 p 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 1 p 01", "YD p F P 1 p 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 1 p 01", "YD p F P 1 p 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 3 I 01", "YD p M S 3 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 3 I 01", "YD p M S 3 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 3 I 01", "YD p F S 3 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 3 I 01", "YD p F S 3 I 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 2 I 01", "YD p M S 2 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 2 I 01", "YD p M S 2 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 2 I 01", "YD p F S 2 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 2 I 01", "YD p F S 2 I 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p S 1 I 01", "YD p M S 1 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p S 1 I 01", "YD p M S 1 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p S 1 I 01", "YD p F S 1 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p S 1 I 01", "YD p F S 1 I 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 3 I 01", "YD p M P 3 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 3 I 01", "YD p M P 3 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 3 I 01", "YD p F P 3 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 3 I 01", "YD p F P 3 I 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 2 I 01", "YD p M P 2 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 2 I 01", "YD p M P 2 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 2 I 01", "YD p F P 2 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 2 I 01", "YD p F P 2 I 01 P" },
    /*YD*/ {"AA p S 3 P 00|VV p P 1 I 01", "YD p M P 1 I 01 S" },
    /*YD*/ {"AA p P 3 P 00|VV p P 1 I 01", "YD p M P 1 I 01 P" },
//  /*YD*/ {"AA p S 3 P 00|VV p P 1 I 01", "YD p F P 1 I 01 S" },
//  /*YD*/ {"AA p P 3 P 00|VV p P 1 I 01", "YD p F P 1 I 01 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 3 P 04", "YE p M S 3 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 3 P 04", "YE p M S 3 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 3 P 04", "YE p F S 3 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 3 P 04", "YE p F S 3 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 2 P 04", "YE p M S 2 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 2 P 04", "YE p M S 2 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 2 P 04", "YE p F S 2 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 2 P 04", "YE p F S 2 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 1 P 04", "YE p M S 1 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 1 P 04", "YE p M S 1 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 1 P 04", "YE p F S 1 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 1 P 04", "YE p F S 1 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 3 P 04", "YE p M P 3 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 3 P 04", "YE p M P 3 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 3 P 04", "YE p F P 3 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 3 P 04", "YE p F P 3 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 2 P 04", "YE p M P 2 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 2 P 04", "YE p M P 2 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 2 P 04", "YE p F P 2 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 2 P 04", "YE p F P 2 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 1 P 04", "YE p M P 1 P 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 1 P 04", "YE p M P 1 P 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 1 P 04", "YE p F P 1 P 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 1 P 04", "YE p F P 1 P 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 3 p 04", "YE p M S 3 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 3 p 04", "YE p M S 3 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 3 p 04", "YE p F S 3 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 3 p 04", "YE p F S 3 p 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 2 p 04", "YE p M S 2 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 2 p 04", "YE p M S 2 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 2 p 04", "YE p F S 2 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 2 p 04", "YE p F S 2 p 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p S 1 p 04", "YE p M S 1 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p S 1 p 04", "YE p M S 1 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p S 1 p 04", "YE p F S 1 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p S 1 p 04", "YE p F S 1 p 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 3 p 04", "YE p M P 3 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 3 p 04", "YE p M P 3 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 3 p 04", "YE p F P 3 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 3 p 04", "YE p F P 3 p 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 2 p 04", "YE p M P 2 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 2 p 04", "YE p M P 2 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 2 p 04", "YE p F P 2 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 2 p 04", "YE p F P 2 p 04 P" },
    /*YE*/ {"AA p S 3 P 00|VV p P 1 p 04", "YE p M P 1 p 04 S" },
    /*YE*/ {"AA p P 3 P 00|VV p P 1 p 04", "YE p M P 1 p 04 P" },
//  /*YE*/ {"AA p S 3 P 00|VV p P 1 p 04", "YE p F P 1 p 04 S" },
//  /*YE*/ {"AA p P 3 P 00|VV p P 1 p 04", "YE p F P 1 p 04 P" },
    /*YE*/ {"AA s S 3 P 00|VV p P 1 p 04", "YE p M P 1 p 04 S" },
                                                              
    /*YH*/ {"AA p P 3 P 00|VV p P 2 P 10", "YH p F P 2 P 10 P" },
    /*YH*/ {"AA p S 3 P 00|VV p P 3 p 10", "YH p M P 2 p 10 S" },
    /*YH*/ {"AA p S 3 P 00|VV p S 3 P 10", "YH p M S 3 P 10 S" },
    /*YH*/ {"AA p S 3 P 00|VV p P 3 p 14", "YH p M P 3 p 14 S" },
                                                              
    /*YI*/ {"AA p S 3 P 00|VV p S 3 P 12", "YI p F S 3 P 12 S" },
    /*YI*/ {"AA p P 3 P 00|VV p P 2 P 12", "YI p F P 2 P 12 P" },
    /*YI*/ {"AA p P 3 P 00|VV p S 2 P 12", "YI p M S 2 P 12 P" },
    /*YJ*/ {"AA p P 3 P 00|VV p P 3 P 09", "YJ p M P 3 P 09 P" },
    /*YK*/ {"AA p P 3 P 00|VV p S 1 p 11", "YK p F S 1 p 11 P" },
//  /*YK*/ {"AA p P 3 P 00|VV p S 1 p 11", "YK p M S 1 p 11 P" },
    /*YL*/ {"AA p S 3 P 00|VV p P 3 P 07", "YL p M P 3 P 07 S" },
    /*YL*/ {"AA p S 3 P 00|VV p S 3 P 07", "YL p F S 3 P 07 S" },
    /*YM*/ {"AA p P 3 P 00|VV p S 3 P 08", "YM p M S 3 P 08 P" },
    /*YM*/ {"AA p S 3 P 00|VV p S 2 P 08", "YM p M S 2 P 08 S" },
    /*YM*/ {"AA p S 3 P 00|VV p S 3 P 08", "YM p M S 3 P 08 P" },
    /*YM*/ {"AA p S 3 P 00|VV p S 3 p 14", "YM p M S 3 p 14 S" },
    /*YM*/ {"AA p S 3 P 00|VV p S 1 P 08", "YM p F S 1 P 08 S" },
    /*YH*/ {"AA p S 3 P 00|VV p S 1 P 16", "YH p M S 1 P 16 S" },
    /*YN*/ {"AA p P 3 P 00|VV p S 1 P 03", "YN p M S 1 P 03 S" },
                                                              
    /*XK*/ {"SS p S 3 P 00|DD p S 3 P 00", "XK p M S 3 P 00 S" },
    /*XK*/ {"SS p P 3 P 00|DD p S 3 P 00", "XK p M S 3 P 00 P" },
    /*XM*/ {"AA p S 3 P 00|DD p S 3 P 00", "XM p M S 3 P 00 S" },
    /*XM*/ {"AA p P 3 P 00|DD p S 3 P 00", "XM p M P 3 P 00 P" },
//  /*XM*/ {"AA p S 3 P 00|DD p S 3 P 00", "XM p F S 3 P 00 S" },
//  /*XM*/ {"AA p P 3 P 00|DD p S 3 P 00", "XM p F P 3 P 00 P" },
    /*XW*/ {"SS p S 3 P 00|AC c S 3 P 00", "XW c M S 3 P 00 S" },
    /*XV*/ {"SS p S 3 P 00|AV s S 3 P 00", "XV s M S 3 P 00 S" },
                                                               
//  /*XI*/ {"AA p S 3 P 00|VV p S 3 P 00", "XI p M S 3 P 00 S" },
//  /*UK*/ {"AA p S 3 P 00|VV p S 3 P 05", "UK p M S 3 P 05 S" },
    /*SV*/ {"SS p S 3 P 00|VV p S 3 P 00", "SV p M S 3 P 00 S" },
    /*UL*/ {"SS p P 3 P 00|VV p S 3 P 01", "UL p M S 3 P 01 P" },
    /*UI*/ {"SS p S 3 P 00|VV p S 3 P 05", "UI p M S 3 P 05 S" },
                                                              
    /*UZ*/ {"SS p S 3 P 00|VV p S 3 P 06", "UZ p M S 3 P 06 S" },
    /*UZ*/ {"SS p P 3 P 00|VV p S 3 P 06", "UZ p M S 3 P 06 P" },
    /*UZ*/ {"SS p P 3 P 00|VV p P 3 P 06", "UZ p M S 3 P 06 S" },
//  /*UZ*/ {"SS p P 3 P 00|VV p P 3 P 06", "UZ p M S 3 P 06 P" },
//  /*UZ*/ {"SS p S 3 P 00|VV p S 3 P 06", "UZ p F S 3 P 06 S" },
//  /*UZ*/ {"SS p P 3 P 00|VV p S 3 P 06", "UZ p F S 3 P 06 P" },
//  /*UZ*/ {"SS p S 3 P 00|VV p S 3 P 06", "UZ p M S 3 P 06 S" },
                                                              
    /*YP*/ {"SS p S 3 P 00|VV p S 3 P 04", "YP p M S 3 P 04 S" },
    /*YP*/ {"SS p P 3 P 00|VV p P 2 P 04", "YP p M P 2 P 04 P" },
    /*YP*/ {"SS p S 3 P 00|VV p P 1 p 04", "YP p M P 1 p 04 S" },
    /*YP*/ {"SS p S 3 P 00|VV p S 3 p 04", "YP p M S 3 p 04 S" },
    /*YP*/ {"SS p P 3 P 00|VV p S 1 p 04", "YP p M S 1 p 04 P" },
    /*YP*/ {"SS p P 3 P 00|VV p S 3 p 04", "YP p M S 3 p 04 P" },
                                                              
    /*YQ*/ {"SS p S 3 P 00|VV p S 1 P 01", "YQ p M S 1 P 01 S" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 2 P 01", "YQ p M S 2 P 01 P" },
//  /*YQ*/ {"SS p P 3 P 00|VV p S 3 P 01", "YQ p M S 3 P 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p P 2 P 01", "YQ p M P 2 P 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 1 p 01", "YQ p M S 1 p 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 2 p 01", "YQ p M S 2 p 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 2 F 01", "YQ p M S 2 F 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p S 3 p 01", "YQ p M S 3 p 01 P" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 1 p 01", "YQ p M S 1 p 01 S" },            
                                                              
    /*YQ*/ {"SS p P 3 P 00|VV p S 2 I 01", "YQ p M S 2 I 01 P" },
    /*YQ*/ {"SS p S 3 P 00|VV p P 3 P 01", "YQ p M P 3 P 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 3 P 01", "YQ p M S 3 P 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 1 F 01", "YQ p M S 1 F 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 3 p 01", "YQ p M S 3 p 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 3 I 01", "YQ p M S 3 I 01 S" },
                                                              
    /*YQ*/ {"SS p P 3 P 00|VV p P 1 P 01", "YQ p M P 1 P 01 P" },
    /*YQ*/ {"SS p S 3 P 00|VV p P 2 P 01", "YQ p M P 2 P 01 P" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 2 P 01", "YQ p M S 2 P 01 S" },            
    /*YQ*/ {"SS p P 3 P 00|VV p P 1 I 01", "YQ p M P 1 I 01 P" },
//  /*YQ*/ {"SS p S 3 P 00|VV p S 1 p 01", "YQ p M S 1 p 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 2 p 01", "YQ p M S 2 p 01 S" },
//  /*YQ*/ {"SS p S 3 P 00|VV p S 3 p 01", "YQ p M S 3 p 01 S" },
    /*YQ*/ {"SS p P 3 P 00|VV p P 1 p 01", "YQ p M P 1 p 01 P" },
    /*YQ*/ {"SS p P 3 P 00|VV p P 1 F 01", "YQ p M P 1 F 01 P" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 3 F 01", "YQ p M S 3 F 01 S" },
    /*YQ*/ {"SS p S 3 P 00|VV p S 1 I 01", "YQ p M S 1 I 01 S" },
                                                              
    /*YR*/ {"SS p P 3 P 00|VV p S 2 P 02", "YR p M S 2 P 02 P" },
    /*YS*/ {"SS p S 3 P 00|VV p S 3 P 07", "YS p M S 3 P 07 S" },
    /*YS*/ {"SS p S 3 P 00|VV p P 3 P 07", "YS p M P 3 P 07 S" },
    /*YS*/ {"SS p P 3 P 00|VV p S 3 P 07", "YS p M S 3 P 07 P" },
    /*YT*/ {"SS p S 3 P 00|VV p S 3 P 12", "YT p M S 3 P 12 S" },
    /*YT*/ {"SS p S 3 P 00|VV p S 2 P 12", "YT p M S 2 P 12 S" },
    /*YT*/ {"SS p P 3 P 00|VV p P 2 P 12", "YT p M P 2 P 12 P" },
    /*YT*/ {"SS p P 3 P 00|VV p S 3 P 12", "YT p M S 3 P 12 P" },
    /*YT*/ {"SS p P 3 P 00|VV p S 2 P 12", "YT p M S 2 P 12 P" },
    /*YT*/ {"SS p S 3 P 00|VV p P 2 P 12", "YT p M P 2 P 12 S" },            
    /*YV*/ {"SS p P 3 P 00|VV p S 3 P 08", "YV p M S 3 P 08 P" },
    /*YV*/ {"SS p S 3 P 00|VV p S 3 P 08", "YV p M S 3 P 08 S" },
    /*YV*/ {"SS p S 3 P 00|VV p S 1 P 08", "YV p M S 1 P 08 S" },
                                                              
    /*YW*/ {"SS p P 3 P 00|VV p P 2 P 10", "YW p M P 2 P 10 P" },
    /*YX*/ {"SS p P 3 P 00|VV p P 3 P 09", "YX p F P 3 P 09 P" },
    /*YX*/ {"SS p S 3 P 00|VV p P 3 P 09", "YX p F P 3 P 09 S" },
    /*YZ*/ {"SS p P 3 P 00|VV p S 1 p 11", "YZ p M S 1 p 11 P" },
//  /*YZ*/ {"SS p P 3 P 00|VV p S 1 p 11", "YZ p M S 1 p 11 P" },
    /*WA*/ {"SS p P 3 P 00|HA p S 1 p 01", "WA p M S 1 p 01 P" },
    /*WB*/ {"SS p P 3 P 00|BE p S 3 p 04", "WB p M S 3 p 04 P" },
    /*WB*/ {"SS p P 3 P 00|BE p S 1 p 04", "WB p M S 1 p 04 P" },
    /*WC*/ {"SS p S 3 P 00|VA p S 3 P 04", "WC p M S 3 P 04 S" },            
    /*WD*/ {"SS p P 3 P 00|VA p P 3 P 09", "WC p F P 3 P 09 P" },
    /*WE*/ {"VV p S 3 P 06|DD p S 3 P 00", "WE p F S 3 P 06 S" },
//  /*WE*/ {"VV p S 3 P 06|DD p S 3 P 00", "WE p F S 3 P 00 S" },
    /*WF*/ {"VV p S 1 P 01|DD p S 3 P 00", "WF p M S 1 P 01 S" },
    /*WF*/ {"VV p S 3 P 01|DD p S 3 P 00", "WF p M S 3 P 01 S" },
    /*WF*/ {"VV p S 2 I 01|DD p S 3 P 00", "WF p M S 2 I 01 S" },
    /*WG*/ {"VV p P 3 P 07|DD p S 3 P 00", "WG p M P 3 P 07 P" },
    /*WG*/ {"VV p S 3 P 12|DD p S 3 P 00", "WG p M S 3 P 12 S" },
    /*WG*/ {"VV p S 3 P 08|DD p S 3 P 00", "WG p M S 3 P 08 S" },
    /*WG*/ {"VV p P 3 P 08|DD p S 3 P 00", "WG p M S 3 P 08 P" },
    /*WG*/ {"VV p P 1 P 12|DD p S 3 P 00", "WG p M P 1 P 12 S" },
    /*QG*/ {"VV p S 3 P 00|DD p S 3 P 00", "QG p M S 3 P 00 S" },
                                                              
    /*YK*/ {"AA p S 3 P 00|VV p S 3 P 11", "YK p M S 3 P 11 S" },
    /*YM*/ {"AA p S 3 P 00|VV p P 3 P 14", "YM p M P 3 P 14 S" },
    /*YM*/ {"AA p P 3 P 00|VV p S 2 P 14", "YM p M S 2 P 14 S" },
//  /*YM*/ {"AA p P 3 P 00|VV p S 2 P 14", "YM p M S 2 P 14 S" },
    /*YH*/ {"AA p P 3 P 00|VV p S 2 P 10", "YH p M S 2 P 10 S" },
    /*YK*/ {"AA p P 3 P 00|VV p P 2 P 11", "YK p M P 2 P 11 S" },
    /*YK*/ {"AA p S 3 P 00|VV p S 1 p 11", "YK p M S 1 p 11 S" },
    /*YJ*/ {"AA p S 3 P 00|VV p S 2 P 09", "YJ p M S 2 P 09 S" },
    /*YJ*/ {"AA p P 3 P 00|VV p S 2 P 09", "YJ p M S 2 P 09 S" },
//  /*YM*/ {"AA p S 3 P 00|VV p S 3 P 08", "YM p M S 3 P 08 S" },
                                                              
    /*VC*/ {"VV p S 3 p 01|VV p S 3 P 06", "VC p M S 3 P 00 S" },
    /*VK*/ {"VV p S 1 P 04|VV p S 3 P 04", "VK p M S 3 P 04 S" },
    /*VL*/ {"VV p S 1 p 01|VV p S 3 p 01", "VL p M S 3 p 01 S" },
    /*YZ*/ {"SS p S 3 P 00|VV p S 1 p 11", "YZ p M S 1 p 11 S" },
    /*YW*/ {"SS p S 3 P 00|VV p S 1 P 16", "YW p M S 1 P 16 S" },
    /*YV*/ {"SS p S 3 P 00|VV p S 2 P 08", "YV p M S 2 P 08 S" },
    /*YX*/ {"SS p S 3 P 00|VV p S 2 P 09", "YX p M S 2 P 09 S" },
    /*YX*/ {"SS p P 3 P 00|VV p S 2 P 09", "YX p M S 2 P 09 P" },
                                                              
    /*AO*/ {"OO p S 3 P 00|AI p S 3 P 00", "AO p M S 3 P 00 S" },
//  /*AO*/ {"OO p S 3 P 00|AI p S 3 P 00", "AO p F S 3 P 00 S" },
    /*AO*/ {"OO p P 3 P 00|AI p P 3 P 00", "AO p M P 3 P 00 P" },
//  /*AO*/ {"OO p P 3 P 00|AI p P 3 P 00", "AO p F P 3 P 00 P" },

    // Tipo de tres
    /*ZB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 00", "ZB p M S 3 P 00 S" },
    /*ZB*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 00", "ZB p M S 3 P 00 S" },
                                                                             
    /*WK*/ {"SS p P 3 P 00|AA p P 3 P 00|VA p P 3 P 06", "WK p M P 3 P 06 P" },
    /*WK*/ {"SS p S 3 P 00|AA p S 3 P 00|VA p S 3 P 06", "WK p F S 3 P 06 S" },
//  /*WK*/ {"SS p S 3 P 00|AA p S 3 P 00|VA p S 3 P 06", "WK p M S 3 P 06 S" },
                                                                             
    /*QB*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 3 P 06", "QB p M P 3 P 06 P" },
//  /*QB*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 3 P 06", "QB p F P 3 P 06 P" },
//  /*QB*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 3 P 06", "QB p F P 3 P 06 P" },
    /*QB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 06", "QB p F S 3 P 06 S" },
//  /*QB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 06", "QB p M S 3 P 06 S" },
    /*UM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 05", "UM p M S 3 P 05 S" },
    /*QE*/ {"AA p S 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QE p M S 3 P 12 S" },
    /*QE*/ {"AA p S 3 P 00|VV p S 3 P 06|DD p S 3 P 00", "QE p M S 3 P 06 S" },
    /*QE*/ {"AA p S 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QE p M S 3 P 01 S" },
//  /*QE*/ {"AA p S 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QE p M S 3 P 12 S" },
    /*QE*/ {"AA p P 3 P 00|VV p S 2 P 02|DD p S 3 P 00", "QE p M S 3 P 02 P" },
    /*QE*/ {"AA p P 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QE p M S 3 P 08 P" },
    /*QE*/ {"AA p S 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QE p M S 3 P 08 S" },
//  /*QE*/ {"AA p S 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QE p F S 3 P 08 S" },
//  /*QE*/ {"AA p P 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QE p M S 3 P 08 P" },
//  /*QE*/ {"AA p P 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QE p F S 3 P 08 P" },
                                                                             
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 I 01", "WM p M S 2 I 01 P" },
    /*XB*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 3 P 00", "XB p M S 3 P 00 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 3 P 01", "WM p M S 3 P 01 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 p 01", "WM p M S 2 p 01 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 1 F 01", "WM p M P 1 F 01 P" },
    /*XB*/ {"SS p P 3 P 00|AA p S 3 P 00|VV p S 3 P 00", "XB p M S 3 P 00 P" },
    /*UR*/ {"AA p S 3 P 00|VV p S 3 P 01|DD p S 3 P 00", "UR p M S 3 P 01 S" },
    /*WL*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 p 04", "WL p M S 2 p 04 P" },
    /*WL*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 2 P 04", "WL p M P 2 P 04 P" },
    /*WL*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 p 04", "WL p M S 1 p 04 S" },
    /*WL*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 p 04", "WL p M S 1 p 04 P" },
    /*WL*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p P 2 P 04", "WL p M P 2 P 04 S" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 p 01", "WM p M S 1 p 01 P" },
    /*WN*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 P 03", "WN p M S 1 P 03 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 3 p 01", "WM p M S 3 p 01 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 2 I 01", "WM p M P 2 I 01 P" },
    /*WM*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 P 01", "WM p M S 2 P 01 P" },
    /*WP*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 p 11", "WP p M S 1 p 11 P" },
    /*WQ*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 3 P 09", "WQ p F P 3 P 09 P" },
//  /*WQ*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 3 P 09", "WQ p F P 3 P 09 P" },
    /*WR*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 2 P 10", "WR p M P 2 P 10 P" },
    /*WS*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 3 P 08", "WS p M S 3 P 08 P" },
//  /*WT*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 p 11", "WT p M S 1 p 11 P" },
//  /*WT*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 1 p 11", "WT p M S 1 p 11 P" },
    /*WT*/ {"SS p P 3 P 00|AA p S 3 P 00|VV p S 1 p 11", "WT p M S 1 p 11 P" },
    /*WV*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 P 12", "WV p M S 2 P 12 P" },
    /*WV*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 2 P 12", "WV p M P 2 P 12 P" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 I 01", "WM p M S 1 I 01 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 P 01", "WM p M S 1 P 01 S" },
//  /*XB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 00", "XB p M S 3 P 00 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p P 3 P 01", "WM p M P 3 P 01 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 01", "WM p M S 3 P 01 S" },
//  /*XB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 00", "XB p M S 3 P 00 S" },
    /*WM*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 01", "WM p M S 3 P 01 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p P 3 p 01", "WM p M P 3 p 01 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 I 01", "WM p M S 3 I 01 S" },
    /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 p 01", "WM p M S 3 p 01 S" },
//  /*WM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 01", "WM p M S 3 P 01 S" },
    /*WQ*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p P 3 P 09", "WQ p F P 3 P 09 S" },
    /*WW*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 07", "WW p M S 3 P 07 S" },
    /*WS*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 08", "WS p M S 3 P 08 S" },
    /*WV*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 12", "WV p M S 3 P 12 S" },
//  /*WV*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 12", "WV p M S 3 P 12 S" },
    /*WV*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 2 P 12", "WV p M S 2 P 12 S" },
    /*WW*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p P 3 P 07", "WW p M P 3 P 07 S" },
    /*QA*/ {"SS p S 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QA p M S 1 P 01 S" },
    /*WX*/ {"SS p S 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "WX p M S 3 P 08 S" },
    /*WY*/ {"SS p S 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "WY p M S 3 P 12 S" },
                                                                             
    /*XH*/ {"SS p S 3 P 00|AA p S 3 P 00|DD p S 3 P 00", "XH p M S 3 P 00 S" },
//  /*XH*/ {"SS p S 3 P 00|AA p S 3 P 00|DD p S 3 P 00", "XH p M S 3 P 00 S" },
    /*WV*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 1 P 12", "WV p M P 1 P 12 P" },
    /*WS*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 P 14", "WS p M S 2 P 14 P" },
    /*WT*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p P 2 P 11", "WT p M P 2 P 11 P" },
//  /*WS*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 P 14", "WS p M S 2 P 14 P" },
    /*WS*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 2 P 14", "WS p M S 2 P 14 S" },
    /*WL*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 P 04", "WL p M S 1 P 04 S" },
    /*WS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p P 3 P 14", "WS p M P 3 P 14 S" },
//  /*WS*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 2 P 14", "WS p M S 2 P 14 S" },
    /*WP*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 11", "WP p M S 3 P 11 S" },
    /*QS*/ {"SS p S 3 P 00|VV p S 3 P 11|DD p S 3 P 00", "QS p M S 3 P 11 S" },
    /*QS*/ {"SS p S 3 P 00|VV p S 1 p 11|DD p S 3 P 00", "QS p M S 1 p 11 S" },
                                                                             
    /*WP*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 p 11", "WP p M S 1 p 11 S" },
    /*WS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 2 P 08", "WS p M S 2 P 08 S" },
                                                                             
    /*UH*/ {"AA p S 3 P 00|VV p S 3 p 01|VV p S 3 P 06", "UH p M S 3 P 00 S" },
    /*UZ*/ {"SS p S 3 P 00|VV p S 3 p 01|VV p S 3 P 06", "UZ p M S 3 P 00 S" },
//  /*UM*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 05", "UM p M S 3 P 00 S" },
    /*WQ*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 2 P 09", "WQ p M S 2 P 09 S" },
    /*WQ*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 2 P 09", "WQ p M S 2 P 09 S" },
    /*WQ*/ {"SS p P 3 P 00|AA p P 3 P 00|VV p S 2 P 09", "WQ p M S 2 P 09 P" },
    /*QT*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 08", "QT p M S 3 P 08 S" },
//  /*QT*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 08", "QT p M S 3 P 08 S" },
    /*YK*/ {"AA p P 3 P 00|VV p P 2 P 01|VV p P 3 P 06", "YK   M   2   11 S" },

    // Tipo de cuatro
    /*QB*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 p 01|VV p S 3 P 06", "QB p M S 3 P 00 S" },
    /*QS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QS p M S 3 P 01 S" },
    /*QS*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QS p M S 3 P 01 S" },
//  /*QS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QS p M S 3 P 01 S" },
//  /*QS*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 1 P 01|DD p S 3 P 00", "QS p M S 3 P 01 S" },
    /*QS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 01|DD p S 3 P 00", "QS p M S 3 P 01 S" },
    /*QT*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QT p M S 3 P 08 S" },
    /*QT*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QT p M S 3 P 08 S" },
//  /*QT*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QT p M S 3 P 08 S" },
//  /*QT*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 08|DD p S 3 P 00", "QT p M S 3 P 08 S" },
    /*QU*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QU p M S 3 P 12 S" },
    /*QU*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QU p M S 3 P 12 S" },
//  /*QU*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QU p M S 3 P 12 S" },
//  /*QU*/ {"SS p S 3 P 00|AA p P 3 P 00|VV p S 3 P 12|DD p S 3 P 00", "QU p M S 3 P 12 S" },
    /*XH*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 05|DD p S 3 P 00", "XH p M S 3 P 00 S" },
    /*UR*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 3 P 00|DD p S 3 P 00", "UR p M S 3 P 00 S" },
    /*QS*/ {"SS p S 3 P 00|AA p S 3 P 00|VV p S 1 p 11|DD p S 3 P 00", "QS p M S 1 p 11 S" },
    
    // Tipo de cinco
           {"VV p S 3 P 00|VV p S 3 P 00|VV p S 3 P 00|VV p S 3 P 00|VV p S 3 P 00", "VV p M S 3 P 00 S" },
    }; 

    static Dictionary<string,string> TblRedc = new Dictionary<string,string>
    { // De 7
      {"VV p M S 2 P 01 S|VV p M P 2 P 01 S|VV p M S 3 P 01 S|VV p M P 2 P 02 S|VV p M S 2 P 04 S|VV p M P 2 P 04 S|VV p M S 3 P 06 S", "VV p M S 3 P 10 S" },

      // De 6
      {"VV p S 2 P 01|VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02|VV p S 2 P 04|VV p P 2 P 04", "VV p M S 3 P 08 S" },
      {"VV p S 1 P 01|VV p S 2 P 01|VV p S 1 p 01|VV p S 2 p 01|VV p S 2 P 02|VV p P 3 P 06", "VV p M S 2 P 09 S" },
      {"VA p S 2 P 01|VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02|VA p S 2 P 04|VA p P 2 P 04", "VA p M S 3 P 08 S" },

      // De 5
      {"VV p S 1 P 01|VV p S 3 P 01|VV p S 1 P 04|VV p S 3 P 04|VV p S 2 P 02", "VV p M S 3 P 08 S" },
      {"VV p S 1 P 01|VV p S 2 P 01|VV p S 1 p 01|VV p S 2 p 01|VV p S 2 P 02", "VV p M S 2 P 12 S" },
      {"VV p S 2 P 01|VV p S 1 P 04|VV p S 2 P 04|VV p S 3 P 04|VV p S 3 P 02", "VV p M S 3 P 08 S" },
      {"VV p P 1 p 01|VV p P 3 p 01|VV p P 1 p 04|VV p P 3 p 04|VV p S 3 P 06", "VV p M P 3 p 10 S" },
      {"VV p S 1 p 01|VV p S 3 p 01|VV p S 1 p 04|VV p S 3 p 04|VV p S 3 P 06", "VV p M S 3 p 10 S" },
      {"VV p S 1 P 01|VV p S 2 P 02|VV p S 3 P 02|VV p S 1 P 04|VV p S 3 P 04", "VV p M S 3 P 08 S" },
      {"VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02|VV p P 2 P 04|VV p S 3 P 06", "VV p M P 3 p 10 S" },
//    {"VV p P 1 p 01|VV p P 3 p 01|VV p P 1 p 04|VV p P 3 p 04|VV p S 3 P 06", "VV p M P 3 p 10 S" },
      {"VA p P 1 p 01|VA p P 3 p 01|VA p P 1 p 04|VA p P 3 p 04|VA p S 3 P 06", "VA p M P 3 p 14 S" },
      {"VA p S 1 P 01|VA p S 3 P 01|VA p S 1 P 04|VA p S 3 P 04|VA p S 2 P 02", "VA p M S 3 P 08 S" },
      {"VA p S 2 P 01|VA p S 1 P 04|VA p S 2 P 04|VA p S 3 P 04|VA p S 3 P 02", "VA p M S 3 P 08 S" },
      {"VA p S 1 P 01|VA p S 2 P 02|VA p S 3 P 02|VA p S 1 P 04|VA p S 3 P 04", "VA p M S 3 P 08 S" },
//    {"VA p S 1 P 01|VA p S 2 P 02|VA p S 3 P 02|VA p S 1 P 04|VA p S 3 P 04", "VA p M P 3 p 08 S" },
      {"VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02|VA p P 2 P 04|VA p S 3 P 06", "VA p M P 3 p 10 S" },
//    {"VA p P 1 p 01|VA p P 3 p 01|VA p P 1 p 04|VA p P 3 p 04|VA p S 3 P 06", "VA p M P 3 p 10 S" },
      {"VA p S 1 P 01|VA p S 3 P 01|VA p S 2 P 02|VA p S 3 P 02|VA p S 3 P 06", "VA p M S 3 P 01 S" },
//    {"VA p S 1 P 01|VA p S 3 P 01|VA p S 1 P 04|VA p S 3 P 04|VA p S 2 P 02", "VA p M S 1 P 08 S" },
      {"HA p S 1 P 01|HA p S 2 P 02|HA p S 3 P 02|HA p S 1 P 04|HA p S 3 P 04", "HA p M S 1 P 01 S" },

      // De 4
      {"VV p S 1 P 01|VV p S 3 P 01|VV p S 1 P 04|VV p S 3 P 02", "VV p M S 3 P 08 S" },
      {"VV p S 1 P 04|VV p S 2 P 04|VV p S 3 P 04|VV p S 2 P 02", "VV p M S 3 P 07 S" },
      {"VV p S 1 P 04|VV p S 2 P 04|VV p S 3 P 04|VV p S 3 P 02", "VV p M S 3 P 07 S" },
      {"VV p P 2 P 01|VV p P 2 P 04|VV p P 2 P 02|VV p P 3 P 06", "VV p M P 2 P 10 S" },
      {"VV p P 1 p 01|VV p P 3 p 01|VV p P 1 p 04|VV p P 3 p 04", "VV p M P 3 p 14 S" },
      {"VV p S 2 P 01|VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 1 P 01|VV p S 3 P 01|VV p S 2 P 02|VV p S 3 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 1 P 01|VV p S 3 P 02|VV p S 1 P 04|VV p S 3 P 04", "VV p M S 3 P 08 S" },
      {"VV p S 1 P 01|VV p S 2 P 02|VV p S 3 P 02|VV p S 1 P 04", "VV p M S 3 P 08 S" },
      {"VV p S 1 P 01|VV p S 2 P 01|VV p S 2 P 02|VV p S 3 P 06", "VV p M S 2 P 08 S" },
      {"VV p S 1 p 01|VV p S 3 p 01|VV p S 1 p 04|VV p S 3 p 04", "VV p M S 3 p 14 S" },

      {"VV p P 1 P 02|VV p P 3 P 02|VV p P 1 P 04|VV p P 3 P 04", "VV p M S 3 P 07 S" },
      {"VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02|VV p P 2 P 04", "VV p M S 3 P 08 S" },
      {"VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02|VV p S 3 P 06", "VV p M S 3 P 10 S" },

      {"VA p S 1 P 04|VA p S 2 P 04|VA p S 3 P 04|VA p S 3 P 02", "VA p M S 3 P 07 S" },
      {"VA p S 2 P 01|VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02", "VA p M S 3 P 12 S" },
      {"VA p S 1 P 01|VA p S 3 P 01|VA p S 2 P 02|VA p S 3 P 02", "VA p M S 3 P 12 S" },
      {"VA p S 1 P 01|VA p S 3 P 02|VA p S 1 P 04|VA p S 3 P 04", "VA p M S 3 P 08 S" },
      {"VA p S 1 P 01|VA p S 2 P 02|VA p S 3 P 02|VA p S 1 P 04", "VA p M S 3 P 08 S" },
      {"VA p P 1 P 02|VA p P 3 P 02|VA p P 1 P 04|VA p P 3 P 04", "VA p M S 3 P 07 S" },
      {"VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02|VA p P 2 P 04", "VA p M S 3 P 08 S" },
      {"VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02|VA p S 3 P 06", "VA p M S 3 P 10 S" },
      {"VA p S 1 p 01|VA p S 3 p 01|VA p S 1 p 04|VA p S 3 p 04", "VA p M S 3 p 14 S" },
      {"VA p P 1 p 01|VA p P 3 p 01|VA p P 1 p 04|VA p P 3 p 04", "VA p M P 3 p 14 S" },
      {"BE p S 1 P 04|BE p S 2 P 04|BE p S 3 P 04|BE p S 3 P 02", "BE p M S 3 P 07 S" },
      {"BE p P 1 P 02|BE p P 3 P 02|BE p P 1 P 04|BE p P 3 P 04", "BE p M S 3 P 07 S" },
      {"BE p P 2 P 01|BE p S 3 P 01|BE p P 2 P 02|BE p P 2 P 04", "BE p M S 3 P 08 S" },
      {"BE p P 2 P 01|BE p S 3 P 01|BE p P 2 P 02|BE p S 3 P 06", "BE p M S 3 P 10 S" },
      {"HA p S 1 P 04|HA p S 2 P 04|HA p S 3 P 04|HA p S 3 P 02", "HA p M S 3 P 07 S" },

      // De 3
      {"VV p P 1 I 01|VV p P 1 P 04|VV p P 1 p 04", "VV p M P 1 P 14 S" },
      {"VV p P 3 P 01|VV p P 3 P 04|VV p P 3 p 04", "VV p M P 3 P 14 S" },
      {"VV p S 1 p 01|VV p S 2 p 01|VV p P 3 P 06", "VV p M S 1 p 11 S" },
      {"VV p S 1 p 01|VV p S 2 p 01|VV p S 3 P 06", "VV p M S 1 p 11 S" },
      {"VV p S 3 P 01|VV p S 3 p 01|VV p S 3 P 06", "VV p M S 3 P 11 S" },
      {"VV p S 1 P 04|VV p S 3 P 04|VV p S 3 P 06", "VV p M S 1 P 16 S" },
      {"VV p P 2 I 01|VV p P 2 P 04|VV p P 2 p 04", "VV p M P 2 I 14 S" },
//    {"VV p P 1 I 01|VV p P 1 P 04|VV p P 1 p 04", "VV p M P 1 I 14 S" },
      {"VV p S 2 F 01|VV p S 1 P 03|VV p S 2 P 03", "VV p M S 2 F 15 S" },
      {"VV p S 1 P 04|VV p S 3 P 04|VV p S 1 p 04", "VV p M S 1 P 04 S" },
      {"VV p S 1 P 01|VV p S 2 P 01|VV p S 3 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 1 P 01|VV p S 2 P 01|VV p S 2 P 02", "VV p M S 2 P 12 S" },
      {"VV p S 1 P 04|VV p S 3 P 04|VV p S 3 P 02", "VV p M S 3 P 07 S" },
      {"VV p S 1 P 04|VV p S 3 P 04|VV p S 2 P 02", "VV p M S 2 P 07 S" },
      {"VV p P 1 P 01|VV p P 1 P 04|VV p P 1 P 02", "VV p M P 1 P 08 S" },
      {"VV p P 2 P 01|VV p P 2 P 04|VV p P 2 P 02", "VV p M P 2 P 08 S" },
      {"VV p S 1 P 04|VV p S 2 P 04|VV p S 3 P 04", "VV p M S 3 P 04 S" },
      {"VV p P 1 p 01|VV p P 3 p 01|VV p S 3 P 06", "VV p M S 3 p 11 S" },
      {"VV p P 2 P 01|VV p P 2 P 02|VV p P 3 P 06", "VV p F P 3 P 09 S" },
      {"VV p S 2 P 01|VV p S 1 p 01|VV p S 2 P 02", "VV p M S 2 P 12 S" }, 
      {"VV p S 2 P 01|VV p S 3 P 01|VV p S 3 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 2 P 01|VV p S 3 P 01|VV p S 2 P 02", "VV p M S 3 P 12 S" },
      {"VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 1 P 01|VV p S 3 P 01|VV p S 3 P 02", "VV p M S 3 P 12 S" },
      {"VV p S 1 P 01|VV p S 2 P 02|VV p S 3 P 02", "VV p M S 3 P 12 S" },
      {"VV p P 2 P 01|VV p S 3 P 01|VV p P 2 P 04", "VV p M S 3 P 14 S" },
      {"VV p S 1 P 01|VV p S 1 P 04|VV p S 3 P 04", "VV p M S 3 P 14 S" },
      {"VV p P 2 P 01|VV p P 2 P 02|VV p P 2 P 04", "VV p M P 2 P 08 S" },
      {"VV p P 1 p 04|VV p P 3 p 04|VV p S 3 P 06", "VV p M P 2 P 10 S" },
      {"VV p S 2 P 02|VV p S 1 P 04|VV p S 3 P 04", "VV p M S 3 P 07 S" },
      {"VA p S 1 P 01|VA p S 1 P 04|VA p S 1 P 02", "VA p M S 1 P 08 S" },
      {"VA p P 1 P 01|VA p P 1 P 04|VA p P 1 P 02", "VA p M P 1 P 08 S" },
      {"VA p S 1 P 04|VA p S 2 P 04|VA p S 3 P 04", "VA p M S 3 P 04 S" },
      {"VA p P 2 P 01|VA p P 2 P 02|VA p P 3 P 06", "VA p F P 3 P 09 S" },
      {"VA p S 2 P 01|VA p S 3 P 04|VA p S 3 P 02", "VA p M S 3 P 08 S" },
//    {"VA p P 1 P 01|VA p P 1 P 04|VA p P 1 P 02", "VA p M P 1 P 08 S" }, 
      {"VA p S 2 P 01|VA p S 3 P 01|VA p S 3 P 02", "VA p M S 3 P 12 S" },
      {"VA p S 2 P 01|VA p S 3 P 01|VA p S 2 P 02", "VA p M S 3 P 12 S" },
      {"VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 02", "VA p M S 3 P 12 S" },
      {"VA p S 1 P 01|VA p S 3 P 01|VA p S 3 P 02", "VA p M S 3 P 12 S" },
      {"VA p S 1 P 01|VA p S 2 P 02|VA p S 3 P 02", "VA p M S 3 P 12 S" },
      {"VA p P 2 P 01|VA p S 3 P 01|VA p P 2 P 04", "VA p M S 3 P 14 S" },
      {"VA p S 1 P 01|VA p S 1 P 04|VA p S 3 P 04", "VA p M S 3 P 14 S" },
      {"VA p P 2 P 01|VA p P 2 P 02|VA p P 2 P 04", "VA p M P 2 P 08 S" },
      {"VA p P 1 p 04|VA p P 3 p 04|VA p S 3 P 06", "VA p M P 2 P 10 S" },
      {"VA p P 1 P 01|VA p P 3 P 01|VA p S 3 P 00", "VA p M S 3 P 00 S" },
      {"HA p P 1 P 01|HA p P 1 P 04|HA p P 1 P 02", "HA p M P 1 P 08 S" },
      {"BE p P 1 P 01|BE p P 1 P 04|BE p P 1 P 02", "BE p M P 1 P 08 S" },
      {"BE p P 2 P 01|BE p P 2 P 02|BE p P 3 P 06", "BE p M P 2 P 09 S" }, 

      // De 2
      {"VV p S 1 P 04|VV p S 3 P 04", "VV p M S 1 P 04 S" },
      {"VV p S 1 P 04|VV p S 3 P 02", "VV p M S 2 P 07 S" },
      {"VV p S 3 P 01|VV p S 3 P 04", "VV p M S 3 P 14 S" },
      {"VV p S 3 p 01|VV p S 3 p 04", "VV p M S 3 p 14 S" },
      {"VV p S 2 P 04|VV p S 2 p 04", "VV p M S 2 P 04 S" },
      {"VV p P 2 P 04|VV p P 2 P 02", "VV p M P 2 P 07 S" },
      {"VV p P 3 P 04|VV p P 3 P 02", "VV p M P 3 P 07 S" },
      {"VV p S 1 P 04|VV p S 2 P 02", "VV p M S 2 P 07 S" },
      {"VV p S 1 p 01|VV p P 3 P 06", "VV p M S 1 p 11 S" },
      {"VV p P 2 P 01|VV p P 3 P 06", "VV p M P 2 P 11 S" },
      {"VV p S 3 p 01|VV p P 3 P 06", "VV p M S 1 p 11 S" },
      {"VV p S 3 P 01|VV p S 3 P 06", "VV p M S 3 P 11 S" },
      {"VV p S 1 P 01|VV p S 2 P 01", "VV p M S 1 P 01 S" },
      {"VV p S 3 P 01|VV p S 2 P 02", "VV p M S 3 P 12 S" },  
      {"VV p S 2 P 01|VV p S 2 P 02", "VV p M S 2 P 12 S" },
      {"VV p P 1 P 01|VV p P 1 P 02", "VV p M P 1 P 12 S" },
      {"VV p P 2 P 01|VV p P 2 P 02", "VV p M P 2 P 12 S" },
      {"VV p S 1 p 04|VV p S 2 p 04", "VV p M S 1 p 04 S" }, 
      {"VV p S 3 P 01|VV p S 3 p 01", "VV p M S 3 P 01 S" },
      {"VV p S 2 P 02|VV p S 3 P 00", "VV p M S 2 P 13 S" },
      {"VV p P 1 P 01|VV p P 1 P 04", "VV p M P 1 P 14 S" }, 
      {"VV p P 2 I 01|VV p P 2 P 04", "VV p M P 2 I 14 S" },
      {"VV p P 3 P 01|VV p P 3 P 04", "VV p M P 3 P 14 S" },
//    {"VV p P 2 I 01|VV p P 2 P 04", "VV p M P 2 I 14 S" },
      {"VV p P 1 P 04|VV p P 1 P 02", "VV p M P 1 P 07 S" },
      {"VV p P 1 I 01|VV p P 1 P 04", "VV p M P 1 I 14 S" },
      {"VV p S 2 P 04|VV p P 3 P 06", "VV p M S 2 P 10 S" },
      {"VV p S 3 I 01|VV p S 3 P 02", "VV p M S 3 I 12 S" },
      {"VV p P 1 p 01|VV p P 3 p 01", "VV p M P 3 p 01 S" },
      {"VV p P 1 p 04|VV p P 3 p 04", "VV p M P 3 p 04 S" },
      {"VV p P 1 P 01|VV p P 3 P 01", "VV p M P 3 P 01 S" },
      {"VV p S 1 p 01|VV p S 2 p 01", "VV p M S 1 p 01 S" },
      {"VV p P 1 P 04|VV p P 3 P 04", "VV p M P 3 P 04 S" },
      {"VV p S 1 p 04|VV p S 3 p 04", "VV p M S 1 p 04 S" },
      {"VV p P 2 p 01|VV p P 2 p 04", "VV p M P 2 p 14 S" },
      {"VV p P 2 P 01|VV p P 2 P 04", "VV p M P 2 P 14 S" },
      {"VV p S 2 p 01|VV p S 2 p 04", "VV p M S 2 p 14 S" },
//    {"VV p P 2 P 01|VV p P 2 P 02", "VV p M P 2 P 12 S" },
      {"VV p S 1 P 01|VV p S 3 P 01", "VV p M S 3 P 01 S" },
      {"VV p S 2 p 01|VV p P 2 p 01", "VV p M P 2 p 01 S" },
      {"VV p S 2 P 02|VV p S 3 P 02", "VV p M S 3 P 02 S" },
      {"VV p S 2 P 01|VV p S 2 p 01", "VV p M S 2 P 01 S" },
      {"VV p S 2 P 01|VV p S 2 P 04", "VV p M S 2 P 14 S" },
      {"VV p S 2 P 01|VV p S 3 P 01", "VV p M S 3 P 01 S" },
      {"VV p S 3 P 00|VV p S 3 P 06", "VV p M S 3 P 00 S" },
      {"VV p S 3 P 05|DD p S 3 P 00", "VV p M S 3 P 05 S" },
      {"VV p S 1 P 03|VV p S 2 P 03", "VV p M S 1 P 03 S" },
      {"VV p S 1 I 01|VV p S 2 I 01", "VV p M S 1 I 01 S" },
      {"HA p P 1 p 01|HA p P 3 p 01", "HA p M P 3 p 01 S" },
      {"HA p P 2 p 01|HA p P 2 p 04", "HA p M P 2 p 14 S" },
      {"HA p S 1 p 04|HA p S 2 p 04", "HA p M S 1 p 04 S" }, 
      {"HA p P 1 p 04|HA p P 3 p 04", "HA p M P 3 p 04 S" },
      {"HA p S 1 p 04|HA p S 3 p 04", "HA p M S 3 p 04 S" },
      {"HA p P 2 P 04|HA p P 2 P 02", "HA p M P 2 P 07 S" },
      {"HA p P 3 P 04|HA p P 3 P 02", "HA p M P 3 P 07 S" },
      {"HA p P 2 P 01|HA p P 2 P 02", "HA p M P 2 P 01 S" },
      {"HA p S 1 p 01|HA p S 3 p 01", "HA p M S 3 p 01 S" },
      {"VA p P 2 p 01|VA p P 2 p 04", "VA p M P 2 p 14 S" },
      {"VA p S 1 p 04|VA p S 2 p 04", "VA p M S 1 p 04 S" }, 
      {"VA p S 3 P 01|VA p S 2 P 02", "VA p M S 3 P 12 S" },
      {"VA p P 2 P 01|VA p P 2 P 02", "VA p M P 2 P 12 S" }, 
      {"VA p P 3 P 04|VA p P 3 P 02", "VA p M P 3 P 07 S" },
      {"VA p P 1 P 01|VA p P 1 P 04", "VA p M P 1 P 14 S" },

      {"VA p P 1 P 01|VA p P 1 P 02", "VA p M P 1 P 12 S" },
      {"VA p S 1 P 04|VA p S 3 P 04", "VA p M S 3 P 04 S" },
      {"VA p S 1 P 01|VA p S 3 P 01", "VA p M S 3 P 01 S" },
      {"VA p S 1 p 04|VA p S 3 p 04", "VA p M S 3 p 04 S" },
      {"VA p S 1 p 01|VA p S 3 p 01", "VA p M S 3 p 01 S" },
      {"VA p P 2 P 01|VA p P 2 P 04", "VA p M P 2 P 14 S" },
      {"VA p P 1 p 04|VA p P 3 p 04", "VA p M P 3 p 04 S" },
      {"VA p P 1 p 01|VA p P 3 p 01", "VA p M P 3 p 01 S" },
      {"VA p S 1 P 03|VA p S 2 P 03", "VA p M S 1 P 03 S" },
      {"VA p S 1 P 01|VA p S 2 P 01", "VA p M S 1 P 01 S" },
      {"VA p S 1 I 01|VA p S 2 I 01", "VA p M S 1 I 01 S" },
      {"VA p P 2 I 01|VA p P 2 P 04", "VA p M P 2 I 14 S" },
      {"VA p S 2 p 01|VA p P 3 P 06", "VA p M S 2 p 11 S" },
      {"VA p P 3 P 01|VA p P 3 P 04", "VA p M P 3 P 14 S" },

      {"BE p P 2 p 01|BE p P 2 p 04", "BE p M P 2 p 14 S" },
      {"BE p P 1 P 04|BE p P 1 P 02", "BE p M P 1 P 07 S" },
      {"BE p P 2 P 04|BE p P 2 P 02", "BE p M P 2 P 07 S" },
      {"BE p P 3 P 04|BE p P 3 P 02", "BE p M P 3 P 07 S" },

      {"BE p S 1 P 01|BE p P 3 P 01", "BE p M S 1 P 01 S" },
//    {"BE p S 3 P 01|BE p S 2 P 02", "BE p M S 3 P 12 S" }, 
      {"BE p S 1 p 04|BE p S 2 p 04", "BE p M S 1 p 04 S" }, 
//    {"BE p S 1 p 04|BE p S 2 p 04", "BE p M S 1 p 04 S" }, 
//    {"BE p P 3 P 04|BE p P 3 P 02", "BE p M P 3 P 04 S" }, 
      {"BE p P 1 P 01|BE p P 3 P 01", "BE p M P 1 P 01 S" },
      {"BE p S 1 p 01|BE p S 3 p 01", "BE p M S 3 p 01 S" },
      {"BE p P 1 p 01|BE p P 3 p 01", "BE p M P 3 p 01 S" },
      {"BE p S 1 p 04|BE p S 3 p 04", "BE p M S 3 p 04 S" },
      {"BE p P 1 p 04|BE p P 3 p 04", "BE p M S 3 p 04 S" },
      {"BE p P 2 P 01|BE p P 2 P 02", "BE p M P 2 P 01 S" },
      {"BE p S 1 P 03|BE p S 2 P 03", "BE p M S 1 P 03 S" },
      {"BE p S 1 I 01|BE p S 2 I 01", "BE p M S 1 I 01 S" },
      {"BE p S 1 p 01|BE p S 2 p 01", "BE p M S 1 p 01 S" },
      {"VA p S 2 p 01|VA p S 2 p 04", "VA p M S 2 p 14 S" },
    };

    List<RootData> RootArray = new List<RootData>();

    Translate Trd = null;
    Word      Wrd = null;

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor, solo se llama para cargar los datos estaticos</summary>
    static public void Inicialize()
      {
      TblRoots.ContainsKey("");
      TblRedc.ContainsKey("");
      }


    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Constructor de la clase</summary>
    public ListOfRoots( Translate Trd, List<RootData> ArrayItem, Word nowWord )
      {
      this.Trd       = Trd;
      this.RootArray = ArrayItem;
      this.Wrd       = nowWord;

      CompactList();
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Si la palabra actual casa con una de la lista le pone sus datos</summary>
    public bool GetData()
      {
      var sRoots = GetString(0);                                      // Obtiene cadena representando todos los datos de la lista

      var RootItem = FindInList( TblRoots, sRoots );                  // Busca si la cadena esta en el diccionario de listas de raices
      if( RootItem == null ) return false;                            // No esta retorna falso

      Wrd.sTipo     = RootItem.sType;                                 // Copia tipo de la raiz hacia la palabra
      Wrd.wGenero  = RootItem.Gender;                                 // Copia genero de la raiz hacia la palabra
      Wrd.wNumero  = RootItem.Number;                                 // Copia numero de la raiz hacia la palabra
      Wrd.wPersona = RootItem.Person;                                 // Copia persona de la raiz hacia la palabra
      Wrd.wTiempo  = RootItem.Time;                                   // Copia tiempo de la raiz hacia la palabra
      Wrd.wModo    = RootItem.Mode;                                   // Copia modo de la raiz hacia la palabra
      Wrd.Plural    = (RootItem.NounNumber==TNum.Plural);           // Copia número del nombre de la raiz hacia la palabra

      if( Wrd.sTipo.Length == 2 )                                     // Si el tipo es de 2 letras ++OJO - Siempre lo es
        return GetTranslation();

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene una cadena que representa el arreglo de raices a partir de elemento 'ini'</summary>
    public string GetString( int ini ) 
      { 
      var n = RootArray.Count;
      if( n-ini <= 0 ) return "";                                     // Si no hay raices retorna cadena vacia

      var s = new StringBuilder( 18*(n-ini) );                        // Reserva memoria estimada para la cadena resultante
      s.Append( RootArray[ini].ToString() );                          // Agrega la primera raiz

      for( int i=ini+1; i<n; i++ )                                    // Recorre las raices restantes
        {
        s.Append('|');                                                // Agrega un separador
        s.Append( RootArray[i].ToString() );                          // Agrega la raiz correspondiente
        }

      return s.ToString();                                            // Retorna la cadena con todo
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Busca lista de raices en el diccionario Roots, si la encuentra retorna objeto, si no retorna null</summary>
    public RootData FindInList( Dictionary<string,string> Roots, string sRootsArray )
      {
      string sItem;
      if( !Roots.TryGetValue(sRootsArray, out sItem) )                // Busca si la cadena esta en diccionario de lista de raices
        return null;                                                  // No esta termina

      return new RootData( sItem );                                   // Crea objeto con datos de la lista que casa y lo retorna
      }

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Compacta la lista combinado dos mas item en uno</summary>
    private void CompactList()
      {
      bool Found = true;                                              // Bandera para saber si se encontro alguna raiz
      while( Found )                                                  // Se repite el ciclo hasta que no se encuentre ninguna
        {
        Found = false;                                                // Marca no encontrada el inicio del ciclo
        for( int i=0; i<RootArray.Count-1; i++ )                      // Palabra donde se empieza, desde la primera a la penultima
          {
          var sRoots = GetString(i);                                  // Obtiene una reprecentación a partir de la palabra i de las raices

          var newItem = FindInList( TblRedc, sRoots );                // Busca si la cadena esta en el diccionario de listas de raices para reducción
          if( newItem == null ) continue;                             // No esta, continua a partir de la proxima palabra

          newItem.sRoot = RootArray[i+1].sRoot;                       // Al item encontrado le pone la raiz de proxima (???) palabra de la lista
          newItem.SetString();                                        // Establece la representación del item como cadena

          RootArray.RemoveRange( i, RootArray.Count-i );              // Quita todos los items de la lista a partir de la raiz i
          RootArray.Add( newItem );                                   // Adiciona item nuevo a la lista

          Found = true;                                               // Marca como encontrada
          break;                                                      // Comienza otro ciclo desde la primera palabra
          }
        }
      }

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene arreglo de raices unicas a las que se redujo la palabra original</summary>
    private List<string> GetRoots()
      {
      var roots = new HashSet<string>();

      foreach( var root in RootArray )
        if( !roots.Contains(root.sRoot) )
          roots.Add( root.sRoot );
       
      return roots.ToList();
      }

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Pone la traducción de la palabra, de acuerdo a los datos de las raices </summary>
    private bool GetTranslation()
      {
      var GenDict = Trd.GetDict( DDirId.Gen );
      var array   = GetRoots();

      if( array.Count==0 ) return false;

      Wrd.Key = array[0];
      Wrd.Data = GenDict.GetWordData( Wrd.Key );

      for( int i=1; i<array.Count; ++i ) 
        {
        string sKey = array[i];
        var WData = GenDict.GetWordData( sKey );

        if( WData != null )
          Wrd.Data = WordData.MergeData( Wrd.Data, WData );
        }

      ConvertVerbs();
      return true;
      }

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Tipos que son solo verbo transitivo</summary>
    static string sVerboT = "VT,JT,PT,GT,HT,XQ,XA,TA,ZB,YF,XG,ZJ,YG,XK,XD,ZI,XM,ZH,XL,XF,UB,QC,UC,UT,UE,UD,UF,QK";
    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Tipos que son solo verbo intransitivo</summary>
    static string sVerboI = "VI,JI,PI,GI,HI,UO,XG,ZZ,YG,XK,XC,XE,XM,XL,UW,QD,UX,UJ,UN,UG,UY";

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Convierte los verbos transitivos e intransitivos a solo transitivo o solo intransitivo de ser posible</summary>
    private void ConvertVerbs()
      {
      var GenDict = Trd.GetDict( DDirId.Gen );
      var array   = GetRoots();

      bool VT = false;
      bool VI = false;

      for( int i=0; i<array.Count; i++ )
        {
        var sKey = array[i];
        var Data = GenDict.GetWordData( sKey );
        if( Data==null ) continue;

        if( sVerboT.Contains( Data.CompType ) ) VT = true;
        if( sVerboI.Contains( Data.CompType ) ) VI = true;
        }

           if( VT && !VI ) FindVerbConvertion( true );
      else if(!VT &&  VI ) FindVerbConvertion( false );
      }

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Tabla de convesión verbos</summary>
    static string[] CnvVV={"VV","VP","VS","VD","VG","XQ","UR","QF","XB","SV","ZF","QE","XI","ZV","QG","UH","QB","UZ","UM","UI","UK","UL","VC"};
    static string[] CnvVT={"VT","PT","HT","JT","GT","XQ","XA","TA","ZB","YF","ZJ","XD","ZI","ZH","XF","UB","QC","UC","UT","UE","UD","UF","JK"};
    static string[] CnvVI={"VI","PI","HI","JI","GI","XQ","UR","QF","UO","XG","ZZ","XC","XE","ZV","QG","UW","QD","UZ","UX","UJ","UN","UG","JL"};

    //-----------------------------------------------------------------------------------------------------------------
    ///<summary>Busca en la tabla y realiza la conversión del tipo de verbo</summary>
    private void FindVerbConvertion( bool bTransitive )
      {
      for( int i=0; i<CnvVV.Length; i++ )
        {
        if( Wrd.sTipo != CnvVV[i] ) continue;

        if( Wrd.sTipo=="VV" && Wrd.wModo==TMod.Indicativo && Wrd.wPersona==TPer.Tercera )
          Wrd.sTipo = (bTransitive)? "HT" : "HI";
        else
          Wrd.sTipo = (bTransitive)? CnvVT[i] : CnvVI[i];

        break;
        }
      }

    } //++++++++++++++++++++++++++++++++++++ Fin de ListOfRoots ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  }   //++++++++++++++++++++++++++++++++++++ Fin de namespace   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
