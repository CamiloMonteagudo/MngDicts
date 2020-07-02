using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TrdEngine;
using TrdEngine.Dictionary;

namespace Tests
  {
  //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public partial class EditDict : Form
    {
    List<DictEntry>              Entries    = new List<DictEntry>();                  // Lista de todas la entradas al diccionario
    Dictionary<string,IndexData> DictIndexs = new Dictionary<string, IndexData>();    // Todas la palabras usadas y sus indices

    SortedIndexs SortEntries = new SortedIndexs();                                    // Lista de entradas al diccionario organizadas por ranking

    static char[] wrdSep = {' ','-','(',')','"','¿','?','!','¡','$',',','/','+','*','='};

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Constructor de la clase </summary>
    public EditDict()
      {
      InitializeComponent();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Se llama al inicializarse el formulario </summary>
    private void EditDict_Load(object sender, EventArgs e)
      {
      Application.CurrentCulture = CultureInfo.InvariantCulture;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee el diccionario 'srcDict'</summary>
    private bool LoadDictionary( string srcDict )
      {
      int i=0;
      Entries = new List<DictEntry>();
      try
        {
        var lines = File.ReadAllLines(srcDict).ToList();                // Lee el diccionario completo

        char[] sep = { ViewData.KeySep };

        for( ; i<lines.Count; i++ )                                     // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep  );                               // Separa la linea por el back slap
          if( parts.Length < 3 )                                       // Si hay problemas con el separador
            {
            MessageBox.Show( "Separador incorrecto: '" + line + "'\r\n" );
            return false;
            }

          var sKey  = parts[0]; 
          var sData = parts[1];     
          var nWrds = parts[2][0]-'0';

          Entries.Add( new DictEntry( sKey, sData, nWrds ) );
          }

        return true;
        }
      catch( Exception ex)
        {
        MessageBox.Show( "*** SE PRODUJO UN ERROR CARGANDO EL DICCIONARIO LÍNEA:"+ i +"\r\n*** " + ex.Message );
        }

      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Lee los indices para el diccionario 'srcDict'</summary>
    private bool LoadDictIndexes( string srcDict )
      {
      var nameIndexs = srcDict.Replace(".dcv", ".dcWrd");
      DictIndexs = new Dictionary<string, IndexData>();                 // Lista de datos del diccionario (inicialmente vacia)

      int i=0;
      try
        {
        var lines = File.ReadAllLines(nameIndexs).ToList();             // Lee el diccionario completo

        char[] sep = { ViewData.KeySep };

        for( ; i<lines.Count; i++ )                                     // Recorre todas las entradas
          {
          var line = lines[i];                                          // Obtiene la entrada actual
          var parts = line.Split( sep ,2 );                             // Separa la linea por el back slap
          if( parts.Length < 2 )                                        // Si hay problemas con el separador
            {
            MessageBox.Show( "Separador incorrecto: '" + line + "'\r\n" );
            continue;
            }

          var sKey  = parts[0]; 
          var sData = parts[1]; 
          
//          DictIndexs[sKey] = IndexData.FromStrWithSep( sData );
          DictIndexs[sKey] = IndexData.FromStrHex( sData );
          }

        return true;
        }
      catch( Exception ex)
        {
        MessageBox.Show( "*** SE PRODUJO UN ERROR Al CARGAR LOS INDICES:"+ i +"\r\n*** " + ex.Message );
        }

      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Selecciona el diccionario a editar a traves de un dialogo de selección de fichero </summary>
    private void btnGetDict_Click(object sender, EventArgs e)
      {
      SelFileDlg.Title  = "Seleccione el diccionario que desea editar";
      SelFileDlg.Filter = "Diccionario para mostrar (*.dcv)|*.dcv";

      if( SelFileDlg.ShowDialog(this) == DialogResult.OK )
        {
        txtDictPath.Text = SelFileDlg.FileName;
        btnLoad_Click( btnLoad, null);
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Intenta cargar el diccionario cuya localización aparece en txtDictPath</summary>
    private void btnLoad_Click(object sender, EventArgs e)
      {
      var crono = Stopwatch.StartNew();

      if( !LoadDictionary( txtDictPath.Text )  ||
          !LoadDictIndexes( txtDictPath.Text ) )
        return;

      txtTime.Text = "t1=" + crono.ElapsedMilliseconds;

      lstWords.BackColor = SystemColors.Window;
      lstWords.VirtualListSize = SortEntries.Count;
      lstWords.Refresh();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone las palabras según el indice en el diccionario en la lista de palabras</summary>
    private void lstWords_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
      {
      var Entry  = SortEntries.SortEntries[e.ItemIndex];
      var idxKey = Entry.Index;
      var Rank   = Entry.Rank;

      e.Item = new ListViewItem( "{" + Rank + "} " + Entries[idxKey].Key );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama cada vez que se cambia la palabra a buscar</summary>
    private void txtFindWrd_TextChanged(object sender, EventArgs e)
      {
      btnFind_Click( null, null );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el botón buscar</summary>
    private void btnFind_Click(object sender, EventArgs e)
      {
      var crono = Stopwatch.StartNew();

      var Query        = new TextQuery( txtFindWrd.Text );
      var FoundEntries = Query.FindWords( DictIndexs );

      SortEntries = new SortedIndexs( FoundEntries, Query, Entries );

      txtTime.Text = "t1=" + crono.ElapsedMilliseconds;

      lstWords.VirtualListSize = SortEntries.Count;
      lstWords.Refresh();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama cuando es necesario localizar un item en el lisview</summary>
    private void lstWords_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
      {
      //var idx = DictKeys.BinarySearch( e.Text, StrComp );
      //if( idx < 0 )
      //  idx = ~idx;

      //e.Index = idx;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama cuando se selecciona una palabra en la lista</summary>
    private void lstWords_SelectedIndexChanged(object sender, EventArgs e)
      {
      var sel = lstWords.SelectedIndices;
      if( sel.Count == 0  ) return;

      var idxfrase = SortEntries.SortEntries[ sel[0] ].Index;
      txtData.Text = Entries[ idxfrase ].Data;
      }


    //------------------------------------------------------------------------------------------------------------------

    } // ++++++++++++++++++++++++++++++++++++    FIN DE LA CLASE EdictDict     +++++++++++++++++++++++++++++++++++++++++++++

  public class DictEntry
    {
    public string Key;                  // Palabra, frase u oración que representa a la entrada
    public string Data;                 // Datos asocidos a la llave
    public int    nWrd;                 // Número de palabras de la llave

    public DictEntry( string sKey, string sData, int nWrds )
      {
      Key  = sKey;
      Data = sData;
      nWrd = nWrds;
      }
    }

  }   // ++++++++++++++++++++++++++++++++++++  FIN DEL NAMESPACE TrdEngine +++++++++++++++++++++++++++++++++++++++++++++
