using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tests
  {
  // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public partial class DelNames : Form
    {
    List<string> Lines = new List<string>();
    string fName;

    ///-----------------------------------------------------------------------------------------------------------------------------------
    public DelNames()
      {
      InitializeComponent();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para seleccionar el diccionario a analizar</summary>
    private void btnPathIni_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title  = "Seleccione el diccionario para buscar nombres";
      SelFileDlg.Filter = "Diccionario con datos para mostrar (*.dcv)|*.dcv";

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
        fName = txtPathIni.Text;
        GetDiccFromDView( fName );
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL CARAR EL DICCIONARIO:\r\n*** " + ex.Message );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto diccionario a partir de un archivo de texto para mostra</summary>
    private void GetDiccFromDView(string FName)
      {
      lstIdDirsDicts.Items.Clear();

      Lines = File.ReadAllLines(FName).ToList();

      char[] sep = {'\\'};

      int nNames = 0;
      lstIdDirsDicts.BeginUpdate();
      for( int i=0; i<Lines.Count; ++i )
        {
        var parts = Lines[i].Split(sep);  
        if( parts.Length < 2 )
          {
          SetMessage( "ERROR: No se pueden separar la llave y los datos\r\nLínea: " + (i+1) + " -> "+ Lines[i]  );
          continue;
          }

        var sKey  = parts[0];
        var sData = parts[1];

        if( sData.Length>0 && sData[0]=='^' )
          sData = sData.Substring(2);

        sKey  = sKey.Trim().ToLower();
        sData = sData.Trim().ToLower();

        if( sKey == sData )
          {
          lstIdDirsDicts.Items.Add( new SelLine( Lines, i ) );
          ++nNames;
          }
        }

      SetMessage( "Se encontraron " + nNames + " Nombres" );

      lstIdDirsDicts.EndUpdate();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Selecciona todas las llaves que estan en la lista</summary>
    private void btnSelAll_Click( object sender, EventArgs e )
      {
      for( int i=0; i<lstIdDirsDicts.Items.Count; ++i )
        lstIdDirsDicts.SetItemChecked( i, true );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita todas las llave seleccionadas en la lista</summary>
    private void btnCleanAll_Click( object sender, EventArgs e )
      {
      foreach( int i in lstIdDirsDicts.CheckedIndices )
        lstIdDirsDicts.SetItemChecked( i, false );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra todas las llaves que estan seleccionadas</summary>
    private void btnDelSelKeys_Click( object sender, EventArgs e )
      {
      var SaveName = fName.Replace( ".dcv", "_DNames.dcv" );

      try
        {
        var DelItems = lstIdDirsDicts.CheckedIndices;
        for( int i = DelItems.Count-1; i >=0; i -- )
          {
          SelLine item = (SelLine)lstIdDirsDicts.Items[i];
          Lines.RemoveAt( item.Idx );
          }

        File.WriteAllLines( SaveName, Lines );

        SetMessage( "Se guardo en: '" + SaveName + "'");
        }
      catch( Exception ex)
        {
        SetMessage( "*** SE PRODUJO UN ERROR AL GUARDAR EL DICCIONARIO:\r\n*** " + ex.Message );
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

  // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  /// <summary> Guarda los datos de las lineas seleccionadas</summary>
  internal class SelLine
    {
    public int Idx;
    public string Line;

    public SelLine( List<string> lines, int i )
      {
      Line = lines[i];
      Idx = i;
      }

    public override string ToString()
      {
      return Line;
      }
    }

  }
