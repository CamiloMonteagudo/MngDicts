using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tests
  {
  public partial class DictRed : Form
    {
    public DictRed()
      {
      InitializeComponent();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para seleccionar el diccionario a analizar</summary>
    private void btnPathIni_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title  = "Seleccione el diccionario";
      SelFileDlg.Filter = "Diccionario de linea de texto (*.txtdic)|*.txtdic";

      if( SelFileDlg.ShowDialog(this) == DialogResult.OK )
        txtPathIni.Text = SelFileDlg.FileName;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Carga el diccionario y muestra las llaves con nombres propios</summary>
    private void btnLoad_Click( object sender, EventArgs e )
      {
      txtMsgBox.Text = "";
      try
        {
        var fName = txtPathIni.Text;
        TextDictToRedDict( fName );
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL CARAR EL DICCIONARIO:\r\n*** " + ex.Message );
        }
      }

    static string tipo_sust      = "SS;NP;XQ;UR;XA;QN;QF;TA;XB;ZB;UO;XH;QO;XJ;QA;SV;YF;XG;ZF;ZJ;ZZ;YG;XK;UA;QB;QC;QD;UZ;UC;UM;UT;UX;UI;UE;UJ;UL;UF;UG;UV;UY;QJ;QK;QL;QM;QZ;LK;MV;SP;NM;NN;";
    static string tipo_adj       = "AA;XQ;UR;XA;QF;TA;XB;ZB;UO;XH;XJ;QE;XD;XC;XI;ZI;XE;XM;XO;PB;UH;UB;UW;QB;QC;QD;UM;UT;UX;UK;UD;UN;UV;QL;QM;QZ;MV;MU;MG;AN;";
    static string tipo_adj_in    = "AI;QN;QO;XY;AO;";
    static string tipo_verbo     = "VV;VR;VT;VI;VG;VP;VS;VC;PT;PI;VST;VSI;GT;GI;XQ;UR;XA;QN;QF;TA;XB;ZB;UO;QA;SV;YF;XG;ZF;ZJ;ZZ;YG;QE;XD;XC;XI;ZI;XE;ZV;ZH;XP;XL;VV;QG;XF;ZG;UH;UB;UW;QB;QC;QD;UZ;UC;UM;UT;UX;UI;UE;UJ;UK;UD;UN;UL;UF;UG;UY;QH;QI;QJ;QK;QL;QM;HT;HI;JT;JI;LK;DO;";
    static string tipo_verbo_aux = "VA;QJ;QK;VW;VU;VY;VX;VJ;VZ;VQ;VB;VE;VF;VH;";
    static string tipo_pronombre = "OO;OD;OG;OI;OS;OL;OA;AF;MG;MU;AE;AP;AL;AD;AO;OR;OP;";
    static string tipo_BEHA      = "BE;HA;";
    static string tipo_adverbio  = "DD;XQ;UR;XA;QN;XH;QO;QA;XK;QE;XD;XC;XM;XP;XL;XY;QG;XF;QH;QI;QL;QM;QZ;DF;";
    static string tipo_articulo  = "RD;RI;";

    const int T_VERB   = 0x01;
    const int T_VERB_A = 0x02;
    const int T_BE     = 0x04;
    const int T_ADJ    = 0x08;
    const int T_ADJ_I  = 0x10;
    const int T_PRON   = 0x20;
    const int T_ADV    = 0x40;
    const int T_ART    = 0x80;
    const int T_SUST   = 0x100;
        
    StringComparer Comp = StringComparer.InvariantCultureIgnoreCase;

    int TIPO_DICT = 2;    // 0-Largo 1-Medio 2-Pequeño

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee un diccionrio en formato de linea de texto y obtiene un diccionario para reducción</summary>
    private void TextDictToRedDict(string FName)
      {
      var MissTypes  = new Dictionary<string,int>();
      var newEntries = new List<string>();

      var Lines = File.ReadAllLines(FName).ToList();

      char[] sep = {'|'};

      for( int i=0; i<Lines.Count; ++i )
        {
        var parts = Lines[i].Split(sep);  
        if( parts.Length < 2 )
          {
          SetMessage( "ERROR: No se pueden separar la llave y los datos (" + (i+1) + ") -> "+ Lines[i]  );
          continue;
          }

        var sKey  = parts[0].ToLower();
        var sData = parts[1];

        if( !sData.StartsWith("#T") )
          {
          SetMessage( "ERROR: Los datos no comiensan con #T (" + (i+1) + ") -> "+ Lines[i]  );
          continue;
          }

        if( sKey.Contains(' ') ) continue;

        var sTipo = sData.Substring(2,2) + ';';

        int nTipos = 0;
        int Code = 0;

        switch( TIPO_DICT )
          {
          case 0:
            if( tipo_adj.Contains( sTipo )       ) { Code |= T_ADJ;    ++nTipos; }
            if( tipo_adj_in.Contains( sTipo )    ) { Code |= T_ADJ_I;  ++nTipos; }
            if( tipo_verbo.Contains( sTipo )     ) { Code |= T_VERB;   ++nTipos; }
            if( tipo_verbo_aux.Contains( sTipo ) ) { Code |= T_VERB_A; ++nTipos; }
            if( tipo_pronombre.Contains( sTipo ) ) { Code |= T_PRON;   ++nTipos; }
            if( tipo_BEHA.Contains( sTipo )      ) { Code |= T_BE;     ++nTipos; }
            if( tipo_adverbio.Contains( sTipo )  ) { Code |= T_ADV;    ++nTipos; }
            if( tipo_articulo.Contains( sTipo )  ) { Code |= T_ART;    ++nTipos; }
            if( tipo_sust.Contains( sTipo )      ) { Code |= T_SUST;   ++nTipos; }
            break;
          case 1:
            if( tipo_verbo.Contains( sTipo )     ) { Code |= T_VERB;   ++nTipos; }
            if( tipo_verbo_aux.Contains( sTipo ) ) { Code |= T_VERB_A; ++nTipos; }
            if( tipo_BEHA.Contains( sTipo )      ) { Code |= T_BE;     ++nTipos; }
            if( tipo_adj.Contains( sTipo )       ) { Code |= T_ADJ;    ++nTipos; }
            if( tipo_adj_in.Contains( sTipo )    ) { Code |= T_ADJ_I;  ++nTipos; }
            if( tipo_pronombre.Contains( sTipo ) ) { Code |= T_PRON;   ++nTipos; }
            if( tipo_adverbio.Contains( sTipo )  ) { Code |= T_ADV;    ++nTipos; }
            if( tipo_articulo.Contains( sTipo )  ) { Code |= T_ART;    ++nTipos; }
            if( tipo_sust.Contains( sTipo )      ) {                   ++nTipos; }
            break;
          case 2:
            if( tipo_verbo.Contains( sTipo )     ) { Code |= T_VERB;   ++nTipos; }
            if( tipo_verbo_aux.Contains( sTipo ) ) { Code |= T_VERB_A; ++nTipos; }
            if( tipo_BEHA.Contains( sTipo )      ) { Code |= T_BE;     ++nTipos; }
            if( tipo_adj.Contains( sTipo )       ) {                   ++nTipos; }
            if( tipo_adj_in.Contains( sTipo )    ) {                   ++nTipos; }
            if( tipo_pronombre.Contains( sTipo ) ) {                   ++nTipos; }
            if( tipo_adverbio.Contains( sTipo )  ) {                   ++nTipos; }
            if( tipo_articulo.Contains( sTipo )  ) {                   ++nTipos; }
            if( tipo_sust.Contains( sTipo )      ) {                   ++nTipos; }
            break;
          }

        if( nTipos == 0 )
          {
          if( MissTypes.ContainsKey(sTipo) ) MissTypes[sTipo] += 1;
          else                               MissTypes[sTipo]  = 1;  

          continue;
          }

        if( Code !=0 )
          {
          string[] NunFrmts = {"X3","X2","X1"};

          newEntries.Add( sKey + Code.ToString( NunFrmts[TIPO_DICT] ) );
          }
        }

      var lst = newEntries.AsParallel().OrderBy(x=>x,Comp).Select(x=>x);

      string[] Sunfixes = {"RedL.","RedM.","RedS."};
      var diccPath = FName.Replace( ".", Sunfixes[TIPO_DICT] );
      File.WriteAllLines( diccPath, lst );

      SetMessage( "\r\nEscribiendo el archivo: '" + diccPath + "'\r\n" );
      SetMessage( "Se encontraron " + newEntries.Count + " entradas" );

      if( MissTypes.Count>0 )
        {
        var lst2 = MissTypes.AsParallel().OrderBy(x=>x.Value).Select(x=>x.Key+" = "+x.Value);

        SetMessage( "\r\nSE IGNORARON LOS SIGUIENTES TIPOS:");
        foreach( var item in lst2 )
          SetMessage( item );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone un mensaje para información del usuario</summary>
    private void SetMessage( string  msg )
      {
      txtMsgBox.Text += msg + "\r\n";
      txtMsgBox.Focus();
      txtMsgBox.SelectionStart = txtMsgBox.Text.Length + 1;
      txtMsgBox.ScrollToCaret();
      Application.DoEvents();
      }

    }
  }
