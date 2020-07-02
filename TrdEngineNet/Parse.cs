using System;
using System.Collections.Generic;
using System.Text;

namespace TrdEngine
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Implementa extración de las oraciones de textos en diferentes formato</summary>
  ///
  ///<remarks>Hasta ahora los textos suportados pueden esta en 3 formatos diferentes (TXT, RTF y HTML), sin embargo
  ///a traves de filtors de conversión se puede trabajar con textos PDF, Word, Docx, Wordperfect, etc.
  ///
  ///Tambien el texto puede ser suministrado por diferentes vias como, memoria (string), fichero, Url y portapales de
  ///Windows</remarks>
  //------------------------------------------------------------------------------------------------------------------
  public class Parse
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Define el formato de documento a analizar como Texto</summary>
    //------------------------------------------------------------------------------------------------------------------
    public const int F_TEXT = 0;
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Define el formato de documento a analizar como RTF</summary>
    //------------------------------------------------------------------------------------------------------------------
    public const int F_RTF  = 1;
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Define el formato de documento a analizar como HTML</summary>
    //------------------------------------------------------------------------------------------------------------------
    public const int F_HTML = 2;

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Marca inicial para definir el texto que no se debe traducir</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string NtMarkIni{ set{m_NTIni=value;} get{return m_NTIni;} }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Marca final para definir el texto que no se debe traducir</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string NtMarkEnd{ set{m_NTEnd=value;} get{return m_NTEnd;} }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene/establece el formato del texto que se esta analizando.</summary>
    ///
    ///<remarks>Los posibles formatos son los siguientes.
    ///              0 - Formato de texto
    ///              1 - Formato RTF
    ///              2 - Formato HTML
    ///Si se intenta establecer un formato diferente será ignorado.
    ///
    ///Esta propiedad debe ser consulatda/establecida despues de haber llamado a <c>SetText</c>, 
    ///<c>SetTextFromFile</c>, <c>SetTextFromUrl</c> ó <c>SetTextFromClipboard</c>.</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public int Format
      {
      get{ return m_Formato; }
      set{ if( value==F_TEXT || value==F_RTF || value==F_HTML ) m_Formato = value; }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Arreglo con los elementos(Item) que forman el texto que se esta analizando</summary>
    //------------------------------------------------------------------------------------------------------------------
    public List<CItem> Items = new List<CItem>();       // Contiene todos los items analizados

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Establece el texto que se quiere separar en oraciones</summary>
    ///
    ///<param name="Txt"> Texto a analizar.</param>
    ///<remarks> Si el texto corresponde al formato de un fichero RTF o HTM el formato se determina automaticamente, en 
    ///otro caso se asume que el que es formato de texto.
    ///       
    ///Nota: Para conocer o modificar el formato del texto se puede usar la propiedad <c>Format</c>.
    ///</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public void SetText(string Txt)
      {
      m_Txt = Txt;                                            // Guarda el texto para uso posterior
      m_len = Txt.Length;                                      // Longitud del texto

      int i = 0;

      // Determinar el formato del texto
      while( m_Txt[i]<=' ' && i < m_len ) ++i;                // Salta los espacios iniciales

      if (i >= m_len)                                          // Se llego al final del texto (nunca debiera ocurrir)
        m_Formato = F_TEXT;                                   // Asumir como texto
      else if( m_Txt[i]=='<' )                                // Es un texto HTML
        m_Formato = F_HTML;
      else if( m_Txt.Substring(i,5).ToLower() == "{\\rtf" )   // Es un texto RTF
        m_Formato = F_RTF;
      else
        {
        if (m_Txt[i+1] == '<')                                // chequear el siguiente caracter
          m_Formato = F_HTML;
        else
          m_Formato = F_TEXT;                                 // Es texto (Si no HTML ni RTF)
        }

      Clear();                                                // Libera todos los items que habia
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Optiene el texto que se quiere separar en oraciones desde un fichero</summary>
    ///
    ///<param name="FileName">Nombre del fichero que se quiere separar en oraciones, Una ves leido el texto del fichero 
    ///automaticamente se determina el formato.</param>
    ///       
    ///<returns><b>Retorno:</b>Verdadero si se pudo leer el fichero, false en otro caso.</returns>
    ///
    ///<remarks>Los tipo de ficheros que se admiten son texto, RTF, PDF, Ms-Word y Docx, estos dos ultimos necesitan
    ///una dll de conversión que debe esta en el mismo directorio que el parse.
    ///
    ///<b>Nota:</b> Para conocer o modificar el formato del texto se puede usar la propiedad <c>TextFormat</c>.
    ///</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public bool SetTextFromFile(string FileName)
      {
      return false;
      //// Determina el formato y usa filtro si es necesario
      //TRY
      //  CStdioFile File1( File, MODE1 );                          // Abre en modo binario
      
      //  BYTE hdr[10]={0};
      //  File1.Read( hdr, 8 );                                     // Lee encabezamiento
      //  File1.Close();                                            // Cierra el fichero
        
      //  if( hdr[0]=='%' && hdr[1]=='P' &&                         // Es un documento PDF
      //      hdr[2]=='D' && hdr[3]=='F' )
      //    return SetTextAndFilter(FileName, L"PDF2RTF.dll", Ret); // Usa filtro
      
      //  if( hdr[0]=='P' && hdr[1]=='K' &&                         // Es un documento Docx
      //      hdr[2]==3 && hdr[3]==4 )
      //    return SetTextAndFilter(FileName, L"DocxFilter.dll", Ret); // Usa filtro

      //  if( hdr[0]==0xD0 && hdr[1]==0xCF && hdr[2]==0x11 &&       // Es un documento word
      //      hdr[3]==0xE0 && hdr[4]==0xA1 && hdr[5]==0xB1 && 
      //      hdr[6]==0x1A && hdr[7]==0xE1 )
      //    return SetTextAndFilter(FileName, L"CV01.DLL", Ret);    // Usa filtro
      //CATCH_ALL(e)
      //END_CATCH_ALL

      //// Carga contenido del fichero y lo carga en el parse
      //*Ret = VARIANT_FALSE;                                       // Retorno por defecto
      //TRY
      //  CStdioFile File2( File, MODE2 );                          // Abre en modo texto

      //  DWORD nBytes = (DWORD)File2.GetLength();                  // Tamaño del fichero
      //  LPSTR pTxt   = m_Txtcpy.GetBuffer( nBytes );              // Reserva memoria 
      //  UINT  cb     = File2.Read( pTxt, nBytes );                // Lee el contenido

      //  m_Txtcpy.ReleaseBuffer( cb );                             // Libera el buffer

      //  File2.Close();                                            // Cierra el fichero

      //  m_Parse.SetText( m_Txtcpy, File.MakeLower(), m_iInputEncoding, &m_iOutputEncoding);            // Pone texto en el parse

      //  *Ret = VARIANT_TRUE;                                      // Retorna OK
      //CATCH_ALL(e)
      //END_CATCH_ALL

      //return S_OK;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Optiene el texto que se quiere separar en oraciones desde un fichero, utilizando un filtro, para convertir 
    ///el fichero a uno de los formatos soportados</summary>
    ///
    ///<param name="FileName">Nombre del fichero que se quiere separar en oraciones, una ves leido el texto del fichero 
    ///automáticamente se determina el formato.</param>
    ///       
    ///<param name="FilterName">Nombre del filtro que se usará para convertir el fichero de su formato original a uno de 
    ///los soportdados.</param>
    ///       
    ///<returns><b>Retorno:</b> Verdadero si se pudo leer el fichero, false en otro caso.</returns>
    ///
    ///<remarks>El filtro presupone la conversion del texto a uno de los 3 formatos soportados (txt,rtf y html), y se 
    ///pueden usar 3 tipos de filtros; Fitros de MS-WORD, Filtro PDF y filtro Docx.</remarks>
    //------------------------------------------------------------------------------------------------------------------

    public bool SetTextAndFilter(string FileName, string FilterName)
      {
      return false;
      //CString nFile  ( FileName   );                      // Lleva a string
      //CString nFilter( FilterName );                      // Lleva a string

      //if( nFilter.Find(':')==-1 )                         // Si no es un Path absoluto
      //  nFilter = GetDiretory(DIR_MODULE) + nFilter;      // Adiciona Path de TrdObjs

      //*Ret = VARIANT_FALSE;                               // Retorno por defecto

      //nFilter.MakeUpper();
      //if( nFilter.Find("PDF") != -1 )
      //  {  // es el filtro del conversor de PDF
      //  HINSTANCE hLib = LoadLibrary( nFilter );    // Carga la DLL de conversión
      //  DWORD n = GetLastError();                
      //  if (hLib < (HINSTANCE)HINSTANCE_ERROR)      // No se puedo cargar
      //    return S_OK;                              // No hace nada

      //  // Carga las funciones de conversión
      //  LPINITPROCESS _InitProcess = (LPINITPROCESS)GetProcAddress(hLib, "InitProcess");
      //  LPLOADIMAGES  _LoadImages  = (LPLOADIMAGES) GetProcAddress(hLib, "LoadImages");
      //  LPPDF2RTF     _Pdf2Rtf     = (LPPDF2RTF)    GetProcAddress(hLib, "Pdf2Rtf"    );
      //  LPFREERTF     _FreeRtf     = (LPFREERTF)    GetProcAddress(hLib, "FreeRtf"    );
      //  LPENDPROCESS  _EndProcess  = (LPENDPROCESS) GetProcAddress(hLib, "EndProcess" );

      //  // Verifica que se cargen todas las funciones
      //  if( _InitProcess==NULL || _LoadImages==NULL || _Pdf2Rtf==NULL || _FreeRtf==NULL || _EndProcess==NULL )
      //    {
      //    FreeLibrary( hLib );                          // Descarga la DLL del filtro
      //    return S_OK;                                  // No hace nada, Dll incorrecta
      //    }

      //  long idProc = _InitProcess();                   // Inicia el proceso
      //  _LoadImages( idProc, false, false, false, false);  // Paramenros para las imagenes

      //  char *sRtf;
      //  long sLen;
      //  if( _Pdf2Rtf(idProc, nFile, &sRtf, &sLen)==0 )  // Realiza la conversion
      //    {
      //    m_Txtcpy = CString(sRtf,sLen);                // Copia el resultado

      //    m_Parse.SetText( m_Txtcpy, nFile.MakeLower(), m_iInputEncoding); // Introduce texto en el parse
      //    *Ret = VARIANT_TRUE;                            // Se realizo la conversion
      //    }

      //  _FreeRtf(sRtf);                                 // Libera buffer de conversión
      //  _EndProcess(idProc);                            // Libera recursos de conversión
      //  FreeLibrary( hLib );                            // Descarga la DLL del filtro
      //  }
      //else if( nFilter.Find("DOCXFILTER") != -1 )
      //  {  // es el filtro del conversor de Docx
      //  HINSTANCE hLib = LoadLibrary( nFilter );    // Carga la DLL de conversión
      //  DWORD n = GetLastError();                
      //  if (hLib < (HINSTANCE)HINSTANCE_ERROR)      // No se puedo cargar
      //    return S_OK;                              // No hace nada

      //  typedef int (CALLBACK *LP_DOCX_TO_RTF)(LPCSTR sDocxFilePath, CStringA** sOutRtf);

      //  typedef int (CALLBACK *LP_DOCX_FREE_RTF)(CStringA* sRtf);

      //  // Carga las funciones de conversión
      //  LP_DOCX_TO_RTF   _DocxToRtf = (LP_DOCX_TO_RTF)GetProcAddress(hLib, "DocxToRtf");
      //  LP_DOCX_FREE_RTF _FreeRtf = (LP_DOCX_FREE_RTF)GetProcAddress(hLib, "FreeRtf");

      //  // Verifica que se cargen todas las funciones
      //  if( _DocxToRtf==NULL || _FreeRtf==NULL)
      //    {
      //    FreeLibrary( hLib );                          // Descarga la DLL del filtro
      //    return S_OK;                                  // No hace nada, Dll incorrecta
      //    }

      //  CStringA* sRtf = NULL;
      //  if( _DocxToRtf(nFile, &sRtf) >= 0 )              // Realiza la conversion
      //    {
      //    m_Txtcpy = sRtf->GetString();

      //    m_Parse.SetText( m_Txtcpy, nFile.MakeLower(), m_iInputEncoding);  // Introduce texto en el parse
      //    *Ret = VARIANT_TRUE;                          // Se realizo la conversion
      //    }

      //  _FreeRtf(sRtf);                                  // Libera buffer de conversión

      //  FreeLibrary( hLib );                            // Descarga la DLL del filtro
      //  }
      //else
      //  {  // son los filtros normales
      //  CCnv_Word Filter(nFilter);
      //  if( Filter.FileToRtf( nFile, m_Txtcpy) == CNV_OK )  // Carga y convierte el fichero
      //    {
      //    m_Parse.SetText( m_Txtcpy, nFile.MakeLower(), m_iInputEncoding);   // Introduce texto en el parse

      //    *Ret = VARIANT_TRUE;                              // Se realizo la conversion
      //    }
      //  }

      //return S_OK;                                         
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Optiene el texto que se quiere separar en oraciones desde una URL</summary>
    ///
    ///<param name="PageUrl">URL de la página web que se quiere separar en oraciones</param>
    ///       
    ///<returns>Verdadero si se pudo descargar la página, false en otro caso.</returns>
    ///
    ///<remarks>El formato se determina automaticamente, en este caso casi siempre será HTML pero en algunos casos puede 
    ///ser TXT.
    ///
    ///<b>Nota:</b> Para conocer o modificar el formato del texto se puede usar la propiedad <c>TextFormat</c>.</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public bool SetTextFromUrl(string PageUrl)
      {
      return false;
      //#define PROXY_CFG_KEY _T("Software\\IdiomaX\\PROXY_CFG")
      //m_UrlFailCause = URL_OK;
      //CString sURL(PageUrl);                              // Obtiene URL desde BSTR

      //DWORD         Type;
      //CString       Server, PathPage;
      //INTERNET_PORT nPort;

      //AfxParseURL( sURL, Type, Server, PathPage, nPort);  // Separa URL en componentes

      //*Ret = VARIANT_FALSE;                               // Retorno por defecto
      //if( Type == AFX_INET_SERVICE_FILE )                 // Si es local en la máquina
      //  {
      //  CorrectPath( PathPage );                            // Para poderlo leer como fichero
      //  SetTextFromFile( PathPage.AllocSysString(), Ret );  // Lee texto desde fichero
      //  }
      //else                                                // Otro servicio de Internet
      //  {
      //  TRY
      //    {
      //    CString sProxyName;
      //    CString sProxyBypass;
      //    DWORD dwAuthentication = 0;
      //    CString sProxyUser;
      //    CString sProxyPassword;
      //    DWORD dwAccessType = INTERNET_OPEN_TYPE_PRECONFIG;  // usar config. del IE por defecto

      //    if (m_ProxyType == PROXY_NONE)
      //      dwAccessType = INTERNET_OPEN_TYPE_DIRECT;     // Conexion sin proxy
      //    else if (m_ProxyType == PROXY_MANUAL)
      //      {
      //      dwAccessType = INTERNET_OPEN_TYPE_PROXY;

      //      CRegKey reg;
      //      long nError = reg.Create(HKEY_CURRENT_USER, PROXY_CFG_KEY);

      //      if (nError != ERROR_SUCCESS)
      //        // si hubo error no usar proxy
      //        dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
      //      else
      //        {
      //        // cargar la configuracion del proxy del registro
      //        ULONG len;
      //        CString sProxyServer;
      //        CString sProxyPort;
      //        DWORD dwUseProxy = 0;

      //        // leer el uso de proxy
      //        nError = reg.QueryDWORDValue(_T("UseProxy"), dwUseProxy);

      //        if (dwUseProxy == 1)        // si se usa un proxy, leer sus datos
      //          {
      //          DWORD dwBypassLocalDirs = 0;
      //          CString sBypassDirList;

      //          // leer el servidor proxy
      //          len = 200;
      //          nError = reg.QueryStringValue(_T("Server"), sProxyServer.GetBuffer(len), &len);
      //          sProxyServer.ReleaseBuffer();

      //          // leer el numero del puerto
      //          len = 200;
      //          nError = reg.QueryStringValue(_T("Port"), sProxyPort.GetBuffer(len), &len);
      //          sProxyPort.ReleaseBuffer();

      //          // leer no usar proxy para direcciones locales
      //          nError = reg.QueryDWORDValue(_T("BypassLocalDirs"), dwBypassLocalDirs);

      //          // leer no usar proxy para un listado de direcciones
      //          len = 200;
      //          nError = reg.QueryStringValue(_T("BypassDirList"), sBypassDirList.GetBuffer(len), &len);
      //          sBypassDirList.ReleaseBuffer();

      //          // Formar el listado de direcciones que no pasan por el proxy
      //          if (dwBypassLocalDirs == 1)
      //            sProxyBypass = _T("<local> ");
      //          sProxyBypass += sBypassDirList;

      //          // leer el uso de autenticacion
      //          nError = reg.QueryDWORDValue(_T("Authentication"), dwAuthentication);

      //          if (dwAuthentication == 1)
      //            {
      //            // leer el nombre de usuario
      //            len = 200;
      //            nError = reg.QueryStringValue(_T("User"), sProxyUser.GetBuffer(len), &len);
      //            sProxyUser.ReleaseBuffer();

      //            // leer el password
      //            len = 200;
      //            nError = reg.QueryStringValue(_T("Password"), sProxyPassword.GetBuffer(len), &len);
      //            sProxyPassword.ReleaseBuffer();
      //            }
      //          }

      //        // verificar que halla servidor y puerto
      //        if (dwUseProxy == 1 && !sProxyServer.IsEmpty() && !sProxyPort.IsEmpty())
      //          //sProxyName.Format("http=http://%s:%s", sProxyServer, sProxyPort);
      //          sProxyName.Format("http://%s:%s", sProxyServer, sProxyPort);
      //        else
      //          // si no, no usar proxy
      //          dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
      //        }
      //      }

      //    // Inicializa la sesión
      //    LPCSTR szProxyName = (sProxyName.IsEmpty()) ? NULL : sProxyName;
      //    LPCSTR szProxyBypass = (sProxyBypass.IsEmpty()) ? NULL : sProxyBypass;
          
      //    CInternetSession ISession("trdObjs", 1, dwAccessType, szProxyName, szProxyBypass);

      //    CStdioFile* IFile = ISession.OpenURL( sURL );   // Abre URL como fichero
      //    if( IFile==NULL )                               // No se pudo abrir
      //      {
      //      m_UrlFailCause = URL_BAD;      // El url no se pudo cargar
      //      return S_OK;
      //      }

      //    // Manejar la auntentificacion del proxy si es necesaria

      //    // Obtener el CHttpFile
      //    CHttpFile* oHttpFile = NULL;
      //    if (IFile->IsKindOf(RUNTIME_CLASS(CHttpFile)))
      //      oHttpFile = (CHttpFile*)IFile;

      //    // Pedir el status code
      //    BOOL bAuthenticated = TRUE;
      //    BOOL bRes = TRUE;
      //    DWORD dwStatusCode = 0;
      //    if (oHttpFile)
      //      bRes = oHttpFile->QueryInfoStatusCode(dwStatusCode);

      //    //DWORD dwError = -1;
      //    //if (bRes == FALSE)
      //    //  dwError = GetLastError();
      //    if (oHttpFile && bRes)
      //      { 
      //      // tenemos el CHttpFile y el status code
      //      if (dwStatusCode == HTTP_STATUS_PROXY_AUTH_REQ)
      //        { 
      //        // el proxy requiere autenticacion
      //        if (dwAuthentication == 1)
      //          {
      //          // poner el usuario y password
      //          bRes = oHttpFile->SetOption(INTERNET_OPTION_PROXY_USERNAME, (LPVOID)(LPCTSTR)sProxyUser, sProxyUser.GetLength()+1);
      //          if (bRes)
      //            bRes = oHttpFile->SetOption(INTERNET_OPTION_PROXY_PASSWORD, (LPVOID)(LPCTSTR)sProxyPassword, sProxyPassword.GetLength()+1);

      //          // volver a pedir el documento
      //          if (bRes)
      //            bRes = oHttpFile->SendRequest();

      //          // pedir de nuevo el status code
      //          if (bRes)
      //            bRes = oHttpFile->QueryInfoStatusCode(dwStatusCode);

      //          if (bRes && dwStatusCode == HTTP_STATUS_PROXY_AUTH_REQ)
      //            {
      //            // el proxy rechazo las credenciales, por tanto el usuario o password son incorrectos
      //            m_UrlFailCause = URL_AUTH_FAILED;
      //            return S_OK;
      //            }
      //          else if (!bRes)
      //            {
      //            // si ocurrio algun otro error, retornar.
      //            m_UrlFailCause = URL_ERROR;
      //            return S_OK;
      //            }
      //          }
      //        else
      //          {
      //          // el proxy requiere autenticacion pero no esta definida en el registro
      //          m_UrlFailCause = URL_AUTH_NEEDED;
      //          return S_OK;
      //          }
      //        }
      //      }

      //    DWORD nSize = 65535;                            // Tamaño del bloque a leer
      //    DWORD nRead = 0;                                // Número de bytes leidos

      //    for(;;)                                           // Empieza a leer bloques
      //      {
      //      LPSTR pHtml = m_Txtcpy.GetBuffer( nRead + nSize );  // Obtiene Buffer adecuado

      //      DWORD cb = IFile->Read( pHtml + nRead, nSize ); // Lee un bloque

      //      nRead += cb;                                  // Actualiza cantidad bytes leidos

      //      m_Txtcpy.ReleaseBuffer( nRead );              // Libera el buffer

      //      if( cb < nSize )                              // Si leyo menos de un bloque
      //        break;                                      // Termina de leer
      //      }

      //    IFile->Close();                                 // Cierra fichero
      //    delete IFile;                                   // Libera fichero
      //    ISession.Close();                               // Cierra sesión

      //    m_Parse.SetText( m_Txtcpy, sURL.MakeLower(),    // Introduce texto en el parse
      //      TEXT_INPUT_ENCODING_AUTO,                     // Detectar el encoding
      //      &m_iOutputEncoding);
      //    m_Url = sURL;                                   // Guarda URL para uso futuro

      //    *Ret = VARIANT_TRUE;                            // Retorna OK
      //    }
      //  CATCH_ALL(e)
      //  END_CATCH_ALL
      //  }

      //return S_OK;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Optiene el texto que se quiere separar en oraciones desde el porta-papeles.</summary>
    ///       
    ///<returns>Verdadero si se pudo obtener texto del porta-papeles, false en otro caso.</returns>
    ///
    ///<remarks>En esta implementación solo se leen los datos con formato texto que hay en el portapapeles, en el futuro 
    ///pudieran soportarse otros tipos de datos.
    ///
    ///Para conocer o modificar el formato del texto se puede usar la propiedad <c>TextFormat</c>.</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public bool SetTextFromClipboard()
      {
      if( System.Windows.Forms.Clipboard.ContainsText())
        {
        SetText( System.Windows.Forms.Clipboard.GetText() );
        return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el texto que se esta analizando.</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string GetSrcText()
      {
      return m_Txt;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Analiza el texto establecido anteriormente y lo separa en oraciones.</summary>
    ///
    ///<remarks>Esta función debe ser llamada despues de <c>SetText</c>, <c>SetTextFromFile</c>, <c>SetTextFromUrl</c> 
    ///ó <c>SetTextFromClipboard</c> que son las que establecen el texto a analizar y determinan automaticamente el formato, 
    ///si se conoce el formato del texto, se puede utilizar <c>Format</c> para garantizar que el formato sea el adecuado.
    ///</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public void DoParse()
      {
      Clear();                                        // Libera todos los items que habia

      for( int i=0; i<m_len; )                        // Recorre todos los caracteres del texto
        {
        SkipNoText( ref i );                          // Obtiene todo hasta principio de oracion

        int _i = i;
        GetTextOra( ref i );                          // Obtiene todo hasta fin de oración

        if( i<m_len && _i == i )                      // No se puedo obtener texto
          AddItem( 'c', m_Txt.Substring( i++, 1) );   // Pone el caracter en la cascara para que 
                                                      // no caiga en un ciclo infinito
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el texto traducido completo.</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string GetTrdText()
      {
      var          Text = new StringBuilder( "", 50*Items.Count );    // Texto traducido completo
      List<string> Sust = new List<string>();                         // Cadenas para sustitución
      List<string> Atrb = new List<string>();                         // Cadenas de sust. atributos

      for( int i=0; i<Items.Count; ++i )                              // Recorre todos los items
        {
        CItem Item = Items[i];                                        // Toma el Item actual

        if( Item.m_Type == 'c' || Item.m_Type == 's' )                // Si el item no se traduce
          {
          string Tmp = Item.m_Text;                                   // Obtiene la traduccion
          string Txt = "";                                            

          for( int j=0; j<Atrb.Count; ++j)                            // Para todas las sustituciones
            {
            int idx = Tmp.IndexOf(SUST_MARK);                         // Busca marca de sustitucion
            if( idx == -1 )                                           // No encontro la marca 
              break;

            Txt += Tmp.Substring(0,idx);                              // Adiciona 1ra mitad al texto
            Txt += Atrb[j];                                           // Sustituye la cadena

            Tmp = Tmp.Substring(idx+1);                               // Continua con 2da mitad
            }

          Txt += Tmp;                                                 // Adiciona el resto al texto
          Atrb.Clear();                                               // Limpia arreglo de sustitución

          if( Item.m_Type == 'c' )                                    // Es un item de cascara
            Text.Append( Txt );                                       // Adiciona item al texto
          else
            Sust.Add(Txt);                                            // Lo adiciona al arreglo

          continue;                                                   // Coje el proximo item
          }

        if( Item.m_Type == 'T' )                                      // Es un item de sustitución
          {
          Atrb.Add(Item.m_Trd);                                       // Lo adiciona al arreglo
          continue;                                                   // Coje el proximo item
          }

        if( Item.m_Type == 't' )                                      // Si es un item que se traduce
          {
          string Tmp = Item.m_Trd;                                    // Obtiene la traduccion

          if( string.IsNullOrEmpty(Tmp) )                             // Si no hay texto traducido
            Tmp = Item.m_Text;                                        // Coje el texto original

          for( int j=0; j<Sust.Count; ++j)                            // Para todas las sustituciones
            {
            int idx = Tmp.IndexOf(SUST_MARK);                         // Busca marca de sustitucion
            if( idx == -1 )                                           // No encontro la marca 
              break;

            Text.Append( Tmp.Substring(0,idx ) );                     // Adiciona 1ra mitad al texto
            Text.Append( Sust[j] );                                   // Sustituye la cadena

            Tmp = Tmp.Substring(idx+1);                               // Continua con 2da mitad
            }
                                       
          Text.Append( Tmp );                                         // Adiciona el resto al texto
          Sust.Clear();                                               // Limpia arreglo de sustitución
          }
        }

      return Text.ToString();
    }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el indice del proximo item a traducir a partir del indice <c>Idx</c>.</summary>
    ///
    ///<param name="Idx">Indice a partir del cual se quiere localizar el proximo item de traducción</param>
    ///
    ///<returns>Si no se localiza ningún item de traducción retorna false</returns>
    //------------------------------------------------------------------------------------------------------------------
    public bool FindNextTrdItem( ref int idx)
      {
      for( ; idx<Items.Count; ++idx )
        {
        char c = Items[idx].m_Type;
        if( c=='t' || c=='T' ) return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Elimina todos los datos obtenidos en un parse anterior.</summary>
    //------------------------------------------------------------------------------------------------------------------
    public void Clear() 
      {
      Items.Clear();                                // Limpia el arreglo de Items
      m_nNoTrd = 0;                                 // Resetea contador de marca de no traducción
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Pone todos los item del parse en forma de texto</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string ToText() 
      {
      var s = new StringBuilder( Items.Count * 40 );
      foreach( CItem Item in Items )
        {
        char c = Item.m_Type;

        s.Append( c ); 
        s.Append(':');
        AddText( s, Item.m_Text );

        if( (c=='t' || c=='T') && Item.m_Trd != null )
          {
          s.Append(".:");
          AddText( s, Item.m_Trd );
          }
        }

      return s.ToString();
      }

  //--------------------------------------------------------------------------------------------------------------------------
  ///<summary>Adiciona la cadena de texto</summary>
  //--------------------------------------------------------------------------------------------------------------------------
  private void AddText( StringBuilder str, string Txt )
    {
    int i = str.Length;
    int n = Txt.Length;

    str.Append( Txt ); 
    str.Replace( "\\", "\\\\", i, n );
    str.Replace( "\n", "\\n" , i, n );
    str.Replace( "\r", "\\r" , i, n );
    str.Replace( "\t", "\\t" , i, n );
    str.Replace( "\"", "\\\"", i, n );
    str.Append( "\r\n" ); 
    }


    //------------------------------------------------------------------------------------------------------------------
    // Definición de las características de cada caracter ANSI
    //------------------------------------------------------------------------------------------------------------------
    const Byte INI     = 1;
    const Byte END     = 2;
    const Byte NUM     = 4;
    const Byte UP      = 8;
    const Byte LOW     = 16;
    const Byte ALFA    = (LOW |UP );
    const Byte ALFANUM = (ALFA|NUM);

    static Byte[] Keys= {// Ini , End , Num , Up  , Low
                          ( 0   | END | 0   | 0   | 0   ),// 0  NUL
                          ( 0   | END | 0   | 0   | 0   ),// 1  SOH
                          ( 0   | END | 0   | 0   | 0   ),// 2  STX
                          ( 0   | END | 0   | 0   | 0   ),// 3  ETX
                          ( 0   | END | 0   | 0   | 0   ),// 4  EOT
                          ( 0   | END | 0   | 0   | 0   ),// 5  ENQ
                          ( 0   | END | 0   | 0   | 0   ),// 6  ACK
                          ( 0   | END | 0   | 0   | 0   ),// 7  BEL
                          ( 0   | END | 0   | 0   | 0   ),// 8  BS
                          ( 0   | END | 0   | 0   | 0   ),// 9  HT
                          ( 0   | END | 0   | 0   | 0   ),// 10 LF
                          ( 0   | END | 0   | 0   | 0   ),// 11 VT
                          ( 0   | END | 0   | 0   | 0   ),// 12 FF
                          ( 0   | END | 0   | 0   | 0   ),// 13 CR
                          ( 0   | END | 0   | 0   | 0   ),// 14 SO
                          ( 0   | END | 0   | 0   | 0   ),// 15 SI
                          ( 0   | END | 0   | 0   | 0   ),// 16 SLE
                          ( 0   | END | 0   | 0   | 0   ),// 17 CS1
                          ( 0   | END | 0   | 0   | 0   ),// 18 DC2
                          ( 0   | END | 0   | 0   | 0   ),// 19 DC3
                          ( 0   | END | 0   | 0   | 0   ),// 20 DC4
                          ( 0   | END | 0   | 0   | 0   ),// 21 NAK
                          ( 0   | END | 0   | 0   | 0   ),// 22 SYN
                          ( 0   | END | 0   | 0   | 0   ),// 23 ETB
                          ( 0   | END | 0   | 0   | 0   ),// 24 CAN
                          ( 0   | END | 0   | 0   | 0   ),// 25 EM
                          ( 0   | END | 0   | 0   | 0   ),// 26 SIB
                          ( 0   | END | 0   | 0   | 0   ),// 27 ESC
                          ( 0   | END | 0   | 0   | 0   ),// 28 FS
                          ( 0   | END | 0   | 0   | 0   ),// 29 GS
                          ( 0   | END | 0   | 0   | 0   ),// 30 RS
                          ( 0   | END | 0   | 0   | 0   ),// 31 US
                          ( 0   | 0   | 0   | 0   | 0   ),// 32 (space)
                          ( INI | END | 0   | 0   | 0   ),// 33 !
                          ( INI | END | 0   | 0   | 0   ),// 34 "
                          ( 0   | 0   | 0   | 0   | 0   ),// 35 #
                          ( 0   | 0   | 0   | 0   | 0   ),// 36 $
                          ( 0   | 0   | 0   | 0   | 0   ),// 37 %
                          ( 0   | 0   | 0   | 0   | 0   ),// 38 &
                          ( INI | 0   | 0   | 0   | 0   ),// 39 '
                          ( INI | 0   | 0   | 0   | 0   ),// 40 (
                          ( 0   | 0   | 0   | 0   | 0   ),// 41 )
                          ( 0   | 0   | 0   | 0   | 0   ),// 42 *
                          ( 0   | 0   | 0   | 0   | 0   ),// 43 +
                          ( 0   | 0   | 0   | 0   | 0   ),// 44 ,
                          ( 0   | 0   | 0   | 0   | 0   ),// 45 -
                          ( 0   | END | 0   | 0   | 0   ),// 46 .
                          ( 0   | 0   | 0   | 0   | 0   ),// 47 /
                          ( INI | 0   | NUM | 0   | 0   ),// 48 0
                          ( INI | 0   | NUM | 0   | 0   ),// 49 1
                          ( INI | 0   | NUM | 0   | 0   ),// 50 2
                          ( INI | 0   | NUM | 0   | 0   ),// 51 3
                          ( INI | 0   | NUM | 0   | 0   ),// 52 4
                          ( INI | 0   | NUM | 0   | 0   ),// 53 5
                          ( INI | 0   | NUM | 0   | 0   ),// 54 6
                          ( INI | 0   | NUM | 0   | 0   ),// 55 7
                          ( INI | 0   | NUM | 0   | 0   ),// 56 8
                          ( INI | 0   | NUM | 0   | 0   ),// 57 9
                          ( 0   | END | 0   | 0   | 0   ),// 58 :
                          ( 0   | END | 0   | 0   | 0   ),// 59 ;
                          ( 0   | 0   | 0   | 0   | 0   ),// 60 <
                          ( 0   | 0   | 0   | 0   | 0   ),// 61 =
                          ( 0   | 0   | 0   | 0   | 0   ),// 62 >
                          ( 0   | END | 0   | 0   | 0   ),// 63 ?
                          ( 0   | 0   | 0   | 0   | 0   ),// 64 @
                          ( INI | 0   | 0   | UP  | 0   ),// 65 A
                          ( INI | 0   | 0   | UP  | 0   ),// 66 B
                          ( INI | 0   | 0   | UP  | 0   ),// 67 C
                          ( INI | 0   | 0   | UP  | 0   ),// 68 D
                          ( INI | 0   | 0   | UP  | 0   ),// 69 E
                          ( INI | 0   | 0   | UP  | 0   ),// 70 F
                          ( INI | 0   | 0   | UP  | 0   ),// 71 G
                          ( INI | 0   | 0   | UP  | 0   ),// 72 H
                          ( INI | 0   | 0   | UP  | 0   ),// 73 I
                          ( INI | 0   | 0   | UP  | 0   ),// 74 J
                          ( INI | 0   | 0   | UP  | 0   ),// 75 K
                          ( INI | 0   | 0   | UP  | 0   ),// 76 L
                          ( INI | 0   | 0   | UP  | 0   ),// 77 M
                          ( INI | 0   | 0   | UP  | 0   ),// 78 N
                          ( INI | 0   | 0   | UP  | 0   ),// 79 O
                          ( INI | 0   | 0   | UP  | 0   ),// 80 P
                          ( INI | 0   | 0   | UP  | 0   ),// 81 Q
                          ( INI | 0   | 0   | UP  | 0   ),// 82 R
                          ( INI | 0   | 0   | UP  | 0   ),// 83 S
                          ( INI | 0   | 0   | UP  | 0   ),// 84 T
                          ( INI | 0   | 0   | UP  | 0   ),// 85 U
                          ( INI | 0   | 0   | UP  | 0   ),// 86 V
                          ( INI | 0   | 0   | UP  | 0   ),// 87 W
                          ( INI | 0   | 0   | UP  | 0   ),// 88 X
                          ( INI | 0   | 0   | UP  | 0   ),// 89 Y
                          ( INI | 0   | 0   | UP  | 0   ),// 90 Z
                          ( 0   | END | 0   | 0   | 0   ),// 91 [
                          ( 0   | END | 0   | 0   | 0   ),// 92 backslat
                          ( 0   | END | 0   | 0   | 0   ),// 93 ]
                          ( 0   | END | 0   | 0   | 0   ),// 94 ^
                          ( 0   | 0   | 0   | 0   | 0   ),// 95 _
                          ( 0   | 0   | 0   | 0   | 0   ),// 96 `
                          ( INI | 0   | 0   | 0   | LOW ),// 97 a
                          ( INI | 0   | 0   | 0   | LOW ),// 98 b
                          ( INI | 0   | 0   | 0   | LOW ),// 99 c
                          ( INI | 0   | 0   | 0   | LOW ),// 100 d
                          ( INI | 0   | 0   | 0   | LOW ),// 101 e
                          ( INI | 0   | 0   | 0   | LOW ),// 102 f
                          ( INI | 0   | 0   | 0   | LOW ),// 103 g
                          ( INI | 0   | 0   | 0   | LOW ),// 104 h
                          ( INI | 0   | 0   | 0   | LOW ),// 105 i
                          ( INI | 0   | 0   | 0   | LOW ),// 106 j
                          ( INI | 0   | 0   | 0   | LOW ),// 107 k
                          ( INI | 0   | 0   | 0   | LOW ),// 108 l
                          ( INI | 0   | 0   | 0   | LOW ),// 109 m
                          ( INI | 0   | 0   | 0   | LOW ),// 110 n
                          ( INI | 0   | 0   | 0   | LOW ),// 111 o
                          ( INI | 0   | 0   | 0   | LOW ),// 112 p
                          ( INI | 0   | 0   | 0   | LOW ),// 113 q
                          ( INI | 0   | 0   | 0   | LOW ),// 114 r
                          ( INI | 0   | 0   | 0   | LOW ),// 115 s
                          ( INI | 0   | 0   | 0   | LOW ),// 116 t
                          ( INI | 0   | 0   | 0   | LOW ),// 117 u
                          ( INI | 0   | 0   | 0   | LOW ),// 118 v
                          ( INI | 0   | 0   | 0   | LOW ),// 119 w
                          ( INI | 0   | 0   | 0   | LOW ),// 120 x
                          ( INI | 0   | 0   | 0   | LOW ),// 121 y
                          ( INI | 0   | 0   | 0   | LOW ),// 122 z
                          ( 0   | END | 0   | 0   | 0   ),// 123 {
                          ( 0   | END | 0   | 0   | 0   ),// 124 |
                          ( 0   | END | 0   | 0   | 0   ),// 125 }
                          ( 0   | END | 0   | 0   | 0   ),// 126 ~
                          ( 0   | 0   | 0   | 0   | 0   ),// 127
                          ( INI | 0   | 0   | UP  | 0   ),// 128 €
                          ( 0   | 0   | 0   | 0   | 0   ),// 129
                          ( 0   | END | 0   | 0   | 0   ),// 130 ‚
                          ( 0   | 0   | 0   | 0   | 0   ),// 131 
                          ( 0   | END | 0   | 0   | 0   ),// 132 „
                          ( 0   | END | 0   | 0   | 0   ),// 133 …
                          ( 0   | END | 0   | 0   | 0   ),// 134 †
                          ( 0   | END | 0   | 0   | 0   ),// 135 ‡
                          ( 0   | 0   | 0   | 0   | 0   ),// 136
                          ( 0   | END | 0   | 0   | 0   ),// 137 ‰
                          ( INI | 0   | 0   | UP  | 0   ),// 138 Š
                          ( INI | 0   | 0   | 0   | 0   ),// 139 ‹
                          ( INI | 0   | 0   | UP  | 0   ),// 140 S
                          ( INI | 0   | 0   | UP  | 0   ),// 141 T
                          ( INI | 0   | 0   | UP  | 0   ),// 142 Ž
                          ( INI | 0   | 0   | UP  | 0   ),// 143 Z
                          ( 0   | 0   | 0   | 0   | 0   ),// 144 
                          ( 0   | 0   | 0   | 0   | 0   ),// 145 ‘
                          ( 0   | 0   | 0   | 0   | 0   ),// 146 ’
                          ( 0   | 0   | 0   | 0   | 0   ),// 147 “
                          ( 0   | END | 0   | 0   | 0   ),// 148 ”
                          ( 0   | 0   | 0   | 0   | 0   ),// 149 •
                          ( 0   | 0   | 0   | 0   | 0   ),// 150 –
                          ( 0   | 0   | 0   | 0   | 0   ),// 151 — 
                          ( 0   | 0   | 0   | 0   | 0   ),// 152
                          ( 0   | END | 0   | 0   | 0   ),// 153 ™
                          ( INI | 0   | 0   | 0   | LOW ),// 154 š
                          ( 0   | 0   | 0   | 0   | 0   ),// 155 ›
                          ( INI | 0   | 0   | 0   | LOW ),// 156 s
                          ( INI | 0   | 0   | 0   | LOW ),// 157 t
                          ( INI | 0   | 0   | 0   | LOW ),// 158 ž
                          ( INI | 0   | 0   | 0   | LOW ),// 159 z
                          ( 0   | 0   | 0   | 0   | 0   ),// 160   
                          ( INI | 0   | 0   | 0   | 0   ),// 161 ¡
                          ( 0   | 0   | 0   | 0   | 0   ),// 162 ¢
                          ( 0   | 0   | 0   | 0   | 0   ),// 163 £
                          ( 0   | 0   | 0   | 0   | 0   ),// 164 ¤
                          ( 0   | 0   | 0   | 0   | 0   ),// 165 ¥
                          ( 0   | END | 0   | 0   | 0   ),// 166 ¦
                          ( 0   | 0   | 0   | 0   | 0   ),// 167 §
                          ( 0   | END | 0   | 0   | 0   ),// 168 ¨
                          ( 0   | 0   | 0   | 0   | 0   ),// 169 ©
                          ( 0   | END | 0   | 0   | 0   ),// 170 ª
                          ( INI | 0   | 0   | 0   | 0   ),// 171 «
                          ( 0   | END | 0   | 0   | 0   ),// 172 ¬
                          ( 0   | END | 0   | 0   | 0   ),// 173 ­
                          ( 0   | 0   | 0   | 0   | 0   ),// 174 ®
                          ( 0   | END | 0   | 0   | 0   ),// 175 ¯
                          ( 0   | 0   | 0   | 0   | 0   ),// 176 °
                          ( 0   | 0   | 0   | 0   | 0   ),// 177 ±
                          ( 0   | 0   | 0   | 0   | 0   ),// 178 ²
                          ( 0   | 0   | 0   | 0   | 0   ),// 179 ³
                          ( 0   | 0   | 0   | 0   | 0   ),// 180 ´
                          ( 0   | 0   | 0   | 0   | 0   ),// 181 µ
                          ( 0   | 0   | 0   | 0   | 0   ),// 182 ¶
                          ( 0   | END | 0   | 0   | 0   ),// 183 ·
                          ( 0   | END | 0   | 0   | 0   ),// 184 ¸
                          ( 0   | END | 0   | 0   | 0   ),// 185 ¹
                          ( 0   | 0   | 0   | 0   | 0   ),// 186 º
                          ( 0   | 0   | 0   | 0   | 0   ),// 187 »
                          ( INI | 0   | 0   | 0   | 0   ),// 188 ¼
                          ( INI | 0   | 0   | 0   | 0   ),// 189 ½
                          ( INI | 0   | 0   | 0   | 0   ),// 190 ¾
                          ( INI | END | 0   | 0   | 0   ),// 191 ¿
                          ( INI | 0   | 0   | UP  | 0   ),// 192 À
                          ( INI | 0   | 0   | UP  | 0   ),// 193 Á
                          ( INI | 0   | 0   | UP  | 0   ),// 194 Â
                          ( INI | 0   | 0   | UP  | 0   ),// 195 Ã
                          ( INI | 0   | 0   | UP  | 0   ),// 196 Ä
                          ( INI | 0   | 0   | UP  | 0   ),// 197 Å
                          ( 0   | 0   | 0   | 0   | 0   ),// 198 Æ
                          ( 0   | 0   | 0   | UP  | 0   ),// 199 Ç
                          ( INI | 0   | 0   | UP  | 0   ),// 200 È
                          ( INI | 0   | 0   | UP  | 0   ),// 201 É
                          ( INI | 0   | 0   | UP  | 0   ),// 202 Ê
                          ( INI | 0   | 0   | UP  | 0   ),// 203 Ë
                          ( INI | 0   | 0   | UP  | 0   ),// 204 Ì
                          ( INI | 0   | 0   | UP  | 0   ),// 205 Í
                          ( INI | 0   | 0   | UP  | 0   ),// 206 Î
                          ( INI | 0   | 0   | UP  | 0   ),// 207 Ï
                          ( INI | 0   | 0   | UP  | 0   ),// 208 Ð
                          ( INI | 0   | 0   | UP  | 0   ),// 209 Ñ
                          ( INI | 0   | 0   | UP  | 0   ),// 210 Ò
                          ( INI | 0   | 0   | UP  | 0   ),// 211 Ó
                          ( INI | 0   | 0   | UP  | 0   ),// 212 Ô
                          ( INI | 0   | 0   | UP  | 0   ),// 213 Õ
                          ( INI | 0   | 0   | UP  | 0   ),// 214 Ö
                          ( 0   | 0   | 0   | 0   | 0   ),// 215 ×
                          ( 0   | 0   | 0   | UP  | 0   ),// 216 Ø
                          ( INI | 0   | 0   | UP  | 0   ),// 217 Ù
                          ( INI | 0   | 0   | UP  | 0   ),// 218 Ú
                          ( INI | 0   | 0   | UP  | 0   ),// 219 Û
                          ( INI | 0   | 0   | UP  | 0   ),// 220 Ü
                          ( INI | 0   | 0   | UP  | 0   ),// 221 Ý
                          ( INI | 0   | 0   | 0   | 0   ),// 222 Þ
                          ( INI | 0   | 0   | 0   | 0   ),// 223 ß
                          ( INI | 0   | 0   | 0   | LOW ),// 224 à
                          ( INI | 0   | 0   | 0   | LOW ),// 225 á
                          ( INI | 0   | 0   | 0   | LOW ),// 226 â
                          ( INI | 0   | 0   | 0   | LOW ),// 227 ã
                          ( INI | 0   | 0   | 0   | LOW ),// 228 ä
                          ( INI | 0   | 0   | 0   | LOW ),// 229 å
                          ( INI | 0   | 0   | 0   | LOW ),// 230 æ
                          ( INI | 0   | 0   | 0   | LOW ),// 231 ç
                          ( INI | 0   | 0   | 0   | LOW ),// 232 è
                          ( INI | 0   | 0   | 0   | LOW ),// 233 é
                          ( INI | 0   | 0   | 0   | LOW ),// 234 ê
                          ( INI | 0   | 0   | 0   | LOW ),// 235 ë
                          ( INI | 0   | 0   | 0   | LOW ),// 236 ì
                          ( INI | 0   | 0   | 0   | LOW ),// 237 í
                          ( INI | 0   | 0   | 0   | LOW ),// 238 î
                          ( INI | 0   | 0   | 0   | LOW ),// 239 ï
                          ( INI | 0   | 0   | 0   | LOW ),// 240 ð
                          ( INI | 0   | 0   | 0   | LOW ),// 241 ñ
                          ( INI | 0   | 0   | 0   | LOW ),// 242 ò
                          ( INI | 0   | 0   | 0   | LOW ),// 243 ó
                          ( INI | 0   | 0   | 0   | LOW ),// 244 ô
                          ( INI | 0   | 0   | 0   | LOW ),// 245 õ
                          ( INI | 0   | 0   | 0   | LOW ),// 246 ö
                          ( 0   | 0   | 0   | 0   | 0   ),// 247 ÷
                          ( 0   | 0   | 0   | 0   | LOW ),// 248 ø
                          ( INI | 0   | 0   | 0   | LOW ),// 249 ù
                          ( INI | 0   | 0   | 0   | LOW ),// 250 ú
                          ( INI | 0   | 0   | 0   | LOW ),// 251 û
                          ( INI | 0   | 0   | 0   | LOW ),// 252 ü
                          ( INI | 0   | 0   | 0   | LOW ),// 253 ý
                          ( INI | 0   | 0   | 0   | LOW ),// 254 þ
                          ( INI | 0   | 0   | 0   | LOW ),// 255 ÿ
                         };      
    
    //------------------------------------------------------------------------------------------------------------------
    // Macro para saber las características de cada letra
    //------------------------------------------------------------------------------------------------------------------
    bool isc_ini(char c)      { return(Keys[c] & INI     )!= 0; }
    bool isc_end(char c)      { return(Keys[c] & END     )!= 0; }
    bool isc_num(char c)      { return(Keys[c] & NUM     )!= 0; }
    bool isc_up(char c)       { return(Keys[c] & UP      )!= 0; }
    bool isc_low(char c)      { return(Keys[c] & LOW     )!= 0; }
    bool isc_alfa(char c)     { return(Keys[c] & ALFA    )!= 0; }
    bool isc_alfanum(char c)  { return(Keys[c] & ALFANUM )!= 0; }
    
    //------------------------------------------------------------------------------------------------------------------
    // Definiciones para los formatos de los textos a parsiar
    //------------------------------------------------------------------------------------------------------------------
    int m_Formato = F_TEXT;                         // Formato del texto

    bool IsText() {return (m_Formato==F_TEXT);}
    bool IsRtf()  {return (m_Formato==F_RTF );}
    bool IsHtml() {return (m_Formato==F_HTML);}

    //------------------------------------------------------------------------------------------------------------------
    // Definción para los tipos de caracteres manejados por GetChar
    //------------------------------------------------------------------------------------------------------------------
    const int C_ANSI         = 0;       // Caracter ANSI normal 
    const int C_RTF           = 1;      // Caracter definido según RTF
    const int C_HTML         = 2;       // Caracter definido según HTML
    const int C_HTML_UNICODE = 3;       // Caracter unicode definido según HTML

    int m_TypeChar = C_ANSI;            // Tipo de caracter, C_NORMAL, C_HTML, C_RTF

    bool IsCharText() {return (m_TypeChar==C_ANSI);}
    bool IsCharRtf()  {return (m_TypeChar==C_RTF); }
    bool IsCharHtml() {return (m_TypeChar==C_HTML);}

    //------------------------------------------------------------------------------------------------------------------
    // Definción del caracter utilizado como marca, para sustitución de los caracteres de
    // definición de formato (HTML o RTF) que se encuentran en el medio de la oración.
    //------------------------------------------------------------------------------------------------------------------
    const char SUST_MARK = '×';

    string  m_Txt;                                 // Texto a analizar
    int     m_len;                                 // Cantidad de caracteres del texto

    int     m_nNoTrd;       // Contador de macheo de las marcas de no traduccion

    string m_NTIni="[";        // Caracter(es) para marcar el inicio de las palabras que no se traducen
    string m_NTEnd="]";        // Caracter(es) para marcar el final de las palabras que no se traducen

    // Variables actualizadas por GetHtmlTag
    string  m_TagName;      // Nombre del ultimo tag analizado.
    bool    m_TagEnd;       // Si el ultimo tag analizado es un terminador o no.
    bool    m_SkipAll;      // Si el ultimo tag analizado salto hasta el terminador o no.

    // Variables actualizadas por GetChar
    int    m_ic;           // Indice al inicio del caracter analizado
    string m_sChar;        // Representación del caracter especial
    int    m_nSp;          // Numero de &nbsp; seguidos
    int    m_iSp;          // Indice donde comienza el primer &nbsp;
    int    m_icmd;         // Indice del ultimo comando RTF analizado

//    IHtmlModify* i_Modify;  // Interface para la modificación de TAGs HTML


    ////------------------------------------------------------------------------------------------------------------------
    //// Esta función convierte un texto Unicode a ANSI
    ////------------------------------------------------------------------------------------------------------------------
    //private string ConvertFormUnicode( string Str )
    //  {
      
    //  byte[] uBytes = Encoding.Unicode.GetBytes(Str);         // Obtiene un arreglo de bytes de la cadena unicode

    //  // Convierte los Bytes de unicode a Ansi
    //  byte[] aBytes = Encoding.Convert( Encoding.Unicode, Encoding.Default, uBytes );

    //  return Encoding.Default.GetString( aBytes );            // Obtiene una cadena a partir del arreglo de bytes
    //  }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza el texto 'Text' a partir del caracter 'i', obteniendo todos los caracteres no texto y avanzando 'i' hasta 
    // el comienzo de la proxima oración. Si 'i' lleva hasta 'len' retorna false y termina.
    //------------------------------------------------------------------------------------------------------------------
    private bool SkipNoText( ref int _i)
      {
      string Cascara = "";
      int i;
      for( i=_i; i<m_len; ++i )                       // Para todos los caracters i    
        {                                      
        string dumy="";                        
        CheckNoTrdMark( ref i, ref dumy);             // Marca de no Traducción, la salta
                                               
        if( m_Txt[i]=='<' )                           // Inicio de comando HTML
          {                                    
          string Tag="";                       
          int j = i;                                  // Prserva le apuntador
          if( GetHtmlTag(ref j, ref Tag ))            // Obtiene un TagHTML
            {                                  
            if( m_TagName == "IDX" )                  // Es un comando XML para el traductor
              break;                                  // Inicia la oración
                                               
            if( IsHtml() )                            // Si el texto es HTML
              {                                
              Cascara += Tag;                         // Incorpora Tag a la cascara
              i = j;                                  // Avanza el apuntador
              continue;                               // Continua buscando inicio de oración
              }                                
            }                                  
          }                                    
                                               
        char c = GetChar( ref i);                     // Obtine el caracter i 
                                                
        if( (c=='\\' || c=='{' )   &&                 // Caracter de inicio de comando RTF
            IsRtf() && IsCharText() )                 // El texto es RTF y c no caracter especial 
          {                                    
          if( GetRtfTag( ref i, ref Cascara ))        // Obtiene un TagRTF
            continue;                                 // Continua con el resto de la oración
          }                                    
                                               
        if( c<255 && isc_ini(c)  )                    // Si el caracter puede inicial oración
          {
          if( IsBullet( m_ic, ref i, ref Cascara) )   // Si empieza con bullet
            continue;                                 // Contunua analizando
          else                                
            {                                 
            i = m_ic;                                 // Idx donde comienza el caracter actual
            break;                                    // Termina el analasis
            }                                 
          }                                   
                                              
        if( IsCharText() )                            // Es un caracter normal
          Cascara += c;                               // Guarda el caracter
        else                                          // Es un caracter especial
          Cascara += m_sChar;                         // Lo guarda como estaba
        }                                     
                                              
      // Encontro el inicio de oración        
      AddItem( 'c', Cascara );                        // Adiciona los caracteres a la lista
      _i = i;                                         // Pone el puntero al inicio de oracion

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza el texto 'Text' a partir del caracter 'i', obteniendo todos los caracteres que forman parte de una oración 
    // y avanzando 'i' hasta el final de la oración. Si 'i' lleva hasta 'len' retorna false y termina.
    //------------------------------------------------------------------------------------------------------------------
    private bool GetTextOra( ref int _i )
      {
      string Ora="";                                        // Contenido de la oración
      var    ITags = new CInnerTags();                      // Arreglo de TAGs internos a la oracion  
                                                  
      bool NtIni = ( m_nNoTrd > 0 );                        // Si esta abierta la marca de no traducción
      m_nSp = 0;                                            // Resetea contador de &nbsp; seguidos
                                                  
      int i=_i;                                   
      for( ; i<m_len; ++i )                                 // Para todos los caracters i    
        {                                         
        CheckNoTrdMark( ref i, ref Ora);                    // Verifica marcas para no traducir
                                                  
        if( m_Txt[i]=='<' )                                 // Inicio de comando HTML
          {                                       
          if( IsHtml()  )                                   // El texto es HTML y no caracter especial
            {                                     
            string sTag="";                       
            int n=0, ii = i;                                // Posicion donde comienza el Tag
            if( GetInnerHtmlTags( ref i, ref sTag, ref n ) )  // Todos los tag internos a la oración
              {
              int Len = Ora.Length;                         // Posicion donde se va a poner la marca
              ITags.Add( sTag, Len, ii );                   // Guarda info del TAGs interno
              Ora += SUST_MARK;                             // Pone un marcador a la oración
              continue;                                     // Continua con el resto de la oración
              }                                  
            else                                 
              {                                  
              if( n>0 ) goto Termina;                       // Encontro un TAG terminador
              }
            }

          string s=""; int k=i;
          if( GetHtmlTag( ref k, ref s ) && m_TagName=="IDX" ) // Es un TAG interno de a la oración
            {
            Ora += s;                                       // Adiciona el TAG a la oración
            i = k;                                          // Actualiza la posición 
            continue;                                       // Continua con el resto de la oración
            }                                     
          }                                       
                                                  
        char c = GetChar(ref i);                            // Obtine el caracter i 
        if( c== '–'  ) c='-';
        if( c>=255 ) continue;
                                                  
        if( c=='\0' && m_TypeChar != C_ANSI)                // Hay que ignorar el caracter
          continue;                               
                                                  
        if( m_nSp>1 )                                       // Mas de un &nbsp; seguido
          {                                       
          i = m_iSp;                                        // Restaura la posicion del primer &nbsp;
          goto Termina;                                     // Corta la oracion
          }

        if (m_TypeChar == C_HTML_UNICODE)
          {
          int Len = Ora.Length;                             // Posicion de la marca
          ITags.Add( m_sChar, Len, i - m_sChar.Length + 1); // Guarda info de la cascara
          Ora += SUST_MARK;                                 // Pone un marcador a la oración
          //MessageBox(NULL, Ora, m_sChar, 0);
          continue;
          }

        if( isc_end(c)  )                                   // Si el caracter puede ser fin de oración
          {
          switch( c )
            {
            case '\r': case '\n':                           // Cambio de linea.
              if( IsText() )                                // Si es formato de texto
                {                                         
                int j = i+1;                                // Indice temporal
                if( c=='\r' && m_Txt[j]=='\n' )             // Car return, seguido de New line
                  ++j;                                      // Salta el new line
                                                          
                if( c=='\n' && m_Txt[j]=='\r' )             // New line, seguido de Car return
                  ++j;                                      // Salta el Car return
                                                          
                for(; j<m_len && !isc_ini(m_Txt[j]); ++j )  // Salta los caracteres que no inician oración
                  {                                       
                  if( m_Txt[j]=='\n' || m_Txt[j]=='\r' )    // Linea vacia
                    goto Termina;                           // Corta
                  }                                       
                                                          
                if( j>=m_len || isc_up(m_Txt[j]) )          // Proxima letra mayuscula o fin del texto
                  goto Termina;                             // Corta
                                                          
                string s="";                              
                if( IsBullet(j, ref j, ref s) )             // Es un Bullet
                  goto Termina;                             // Corta
                                                          
                s = m_Txt.Substring( i, j-i );              // Coje la cascara
                Ora += ' ';                                 // Espacio para separar palabras

                int Len = Ora.Length;                       // Posicion de la marca
                ITags.Add( s, Len, i );                     // Guarda info de la cascara
                Ora += SUST_MARK;                           // Pone un marcador a la oración
                                                     
                i = j-1;                                    // Actaliza pocición actual
                continue;                                   // Continua con resto de la oración
                }                                    
              else                                         
                {                                    
                if( IsHtml() )                              // Si es formato HTML
                  {                                  
                  int len = Ora.Length;                     // Longitud de la cadena
                  if( len>0 && Ora[len-1] != ' ' )          // Si el ultimo no es espacio
                    Ora += ' ';                             // Agreaga un espacio
                  }                                  
                continue;                                   // Los ignora
                }                                    
                                                            
            case '\t':                                      // Tab
              {                                      
              if( IsText() )                                // Si es formato de texto
                goto Termina;                               // Siempre corta la oración
                                                     
              if( IsHtml() )                                // Si es formato de Html
                break;                                      // Siempre lo toma (Html lo ignora)
                                                     
              int j=i+1;                                    // Salta el tab
              while( j<m_len && m_Txt[j]<=' ')              // Salta espacios tercos
                ++j;                                
                                                    
              if( isc_up(m_Txt[j]) )                        // Sigue un caracter en mayusculas
                goto Termina;                               // Corta la oración
              }                                     
              break;                                        // Lo toma
                                                    
            case ':':                               
              if( isFile( ref i, ref Ora) )                 // Nombre de fichero o URL
                continue ;                                  // Lo toma
              goto Termina;                                 // Corta
                                                    
            case '.':                               
              {                                     
              if( isc_up(m_Txt[i-1]) &&                     // Antecedido de mayuscula
                  isc_up(m_Txt[i+1])  )                     // Seguido de mayuscula
                break;                                      // Lo toma       
                                                    
              if( IsExt(i+1) || IsAbr(i-1) )                // Si es una extension o abreviatura
                break;                                      // Lo toma
                                                    
              int j = i+1 ;                         
              for( ; m_Txt[j]==' '; ++j );                  // Salta espacios
                                                    
              if( isc_up(m_Txt[j])     ||                   // Le sigue mayuscula
                 !isc_alfanum(m_Txt[j]) )                   // Le sigue no alfanumerico
                goto Termina;                               // Rompe.
                                                    
              break;                                        // En otro caso lo toma
              }                                     
                                                    
            case '!':                               
              if( Ora.Length==0  )                          // Es el primer caracter
                break;                                      // Siempre lo toma

              Ora += m_Txt[i++];                            // Lo toma y despues termina
              goto Termina;                             
                                                        
            case '?':                                   
              Ora += m_Txt[i++];                            // Lo toma y despues termina
              goto Termina;                             
                                                        
            case '¿':                                   
              if( Ora.Length==0  )                          // Es el primer caracter
                break;                                      // Lo toma
                                                        
              i = m_ic;                                     // Indice del ultimo caracter analizado
              goto Termina;                                 // En otro caso, corta
                                                        
            case '”':                                   
            case '"':                                   
              {                                         
              if( Ora.Length==0  )                          // Es el primer caracter
                break;                                      // Siempre lo toma
                                                        
              int j = i+1;                              
              while( j<m_len && m_Txt[j] == ' ') ++j;       // Salta espacios
              if( !isc_alfanum(m_Txt[j]) )                  // Seguido de no alfanumerico
                {
                Ora += m_Txt[i++];                          // Lo toma y despues termina
                goto Termina;                               
                }                                  
              break;                                        // Lo toma y sigue
              }                                    
                                                   
            case '`':                              
              c = '\'';                                     // Sustituye el tipo de comilla
              break;                                        // Y la toma siempre
                                                   
            case '\\':                                      // Inicio de comando RTF
            case '{':                                       // Inicio de bloque de comandos RTF
            case '}':                                       // Fin de bloque de comandos RTF
              if( IsRtf() && IsCharText() )                 // Texto RTF y no caracter especial?
                {                                  
                string sTag="";                    
                int ii = i;                                 // Posicion donde comienza el Tag
                if( GetInnerRtfCmd( ref i, ref sTag )>0 )   // Todos los tag internos a la oración
                  {
                  int Len = Ora.Length;                     // Posicion donde se va a poner la marca
                  ITags.Add( sTag, Len, ii );               // Guarda info del TAGs interno
                  Ora += SUST_MARK;                         // Pone un marcador a la oración
                                                  
                  --i;                                      // Compensa incremento del for(...)
                  continue;                                 // Continua con el resto de la oración
                  }                               
                }                                 
              goto Termina;                                 // Corta
            default:                                        // Siempre es fin de oración
              i = m_ic;                                     // Indice del ultimo caracter analizado
              goto Termina;                                 // Corta
            } // end switch                       
          } // end if                             
                                                  
        Ora += c;                                           // Agrega caracter a la oración
        } // end for

    Termina:;

      FixApostrofes(ref Ora, ref ITags);                    // Arregla los apotrofes

      bool NtEnd = (m_nNoTrd > 0);                          // Continua abierta la marca de no traducción

      if( NtIni || NtEnd  )                                 // Inicio, final o toda la oracion no se traducen
        {
        SlitByNoTrdMark(ref Ora, NtIni, NtEnd, ref ITags ); // Pica la oración según el caso
        }
      else
        {
        i = DelEndSustMark( ref i, ref Ora, ref ITags);     // Borra Marcas al final de la oración

        if( ITags.Len()>0 && IsRtf() )                      // Si hay tags internos y es RTF
          GetBullet2( ref Ora, ref ITags);                  // Obtiene bullet al inicio de la oración 

        DelIniSustMarks( ref Ora, ref ITags );              // Quita marcas del inicio
        JoinSustMarks( ref Ora, ref ITags );                // Une las marcas que estan cosecutivas

        for(int j=0; j<ITags.Len(); ++j )                   // Guarda cascara interior de la oracion
          AddItem( 's', ITags.GetTxt(j) );                  // Adiciona cadena de sustitución

        AddItem( 't', Ora );                                // Adiciona oración a la lista de items
        }

      _i = i;                                               // Pone el puntero al final de la oracion

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza el texto 'Text' a partir del caracter 'i', determina si es un numerado con
    // uno de los siguientes formatos:
    //    Romano  - Ej: I- II- III- ó I. II. III. ó I) II) III) ó I)- II)- III)- 
    //              Ej: i- ii- iii- ó i. ii. iii. ó i) ii) iii) ó i)- ii)- iii)- 
    //    Numero  - Ej: 1- 2- 3- ó 1. 2. 3. ó 1) 2) 3) ó 1)- 2)- 3)- 
    //    Letra   - Ej: a- b- c- ó a. b. c. ó a) b) c) ó a)- b)- c)- 
    //
    // Si es este el caso, pone los caracteres en la cadena 'Cascara', avanza i y retorna
    // true, en otro caso retorna false sin hacer nada.
    //------------------------------------------------------------------------------------------------------------------
    private bool IsBullet( int ini, ref int i, ref string Cascara )
      {
      bool num = false;
      int   j = ini;

      while( m_Txt[j]=='I' || m_Txt[j]=='V' || m_Txt[j]=='X' )      // Romanos mayusculas
        ++j;

      if( j == ini)                                                 // No ha encontrado nada
        while( m_Txt[j]=='i' || m_Txt[j]=='v' || m_Txt[j]=='x' )    // Romanos minusculas
          ++j;

      if( j == ini)                                                 // No ha encontrado nada
        {
        int k = j;                                                  // Si 2.1... o A.1...
        if( isc_alfanum(m_Txt[k]) )                                 // Si es un número o una letra
          { 
          ++k;                                                      // Salta primer caracter
          while( isc_num(m_Txt[k]) ) ++k;                           // Si es un número de mas de un digito

          while( m_Txt[k]=='.' && isc_alfanum(m_Txt[k+1]) )         // Separador seguido de un numero o letra
            { 
            k+=2;                                                   // Salta separador y el digito que le sigue
            while( isc_num(m_Txt[k]) ) ++k;                         // Si es un número de mas de un digito
            num = true;                                             // Bandera de bullet numerico
            j = k;                                                  // Salta todos los caracteres analizados
            } 
          }
        }

      if( j == ini)                                                 // No ha encontrado nada        
        j = i + 1;                                                  // Salta caracter actual
      
      int    skip = 0;
      string sTag = "";       

      if( IsRtf() )                                                 // Salta comandos RTF
        if( GetInnerRtfCmd(ref j, ref sTag )== -1 )                 // Hay un fin de parrafos
          skip = m_icmd - j;                                        // Lo toma como un bullet

      if( skip==0 && (m_Txt[j]=='-' || m_Txt[j]=='.' ||             // Seguido de - ó .
                    m_Txt[j]==')' || m_Txt[j]==':') )               // Seguido de ) ó :
        {
        ++j;

        if( m_Txt[j]=='-' ) ++j;                                    // Opcionalmente - Ej a)- i.- a:-    

        if( m_Txt[j]==' ' )                                         // Seguido de espacio
          skip = 1;
        }

      if( skip==0 && IsRtf() )                                      // Salta comandos RTF
        if( GetInnerRtfCmd( ref j, ref sTag )== -1 )                // Hay un fin de parrafos
          skip = m_icmd - j;                                        // Lo toma como un bullet

      if( skip==0 && num && m_Txt[j]==' ' )                         // Numero seguido de espacio
        skip = 1;

      if( skip==0 && m_Txt[j]=='\t' )                               // Seguido de tab normal
        skip = 1;

      if( skip!=0 )                                                 // Es un bullet o numerando
        {
        j += skip;
        Cascara += m_Txt.Substring( ini, j-ini);                    // Lo mete en la cascara

        i = j-1;                                                    // Actualiza el puntero
        return true;
        }

      return false;                                                 // No complio con el formato
      }

    //------------------------------------------------------------------------------------------------------------------
    // Determina si es un nombre de fichero o un URL
    //------------------------------------------------------------------------------------------------------------------
    private bool isFile( ref int _i, ref string Ora )
      {
      int i = _i;
      if( (i>2 && !isc_alfanum(m_Txt[i-2]) && isc_alfa(m_Txt[i-1]) ) ||      // A:\ ...
          (m_len>4 && m_Txt.Substring( i-4, 4).ToLower()=="file"   ) ||      // file: ...
          (m_len>4 && m_Txt.Substring( i-4, 4).ToLower()=="http"   ) ||      // http: ...
          (m_len>3 && m_Txt.Substring( i-3, 3).ToLower()=="ftp"    )  )      //  ftp: ...
        {
        ++i;                         // Salta el : 

        // Solo seguido de numero, letra o slat o backslat
        if( !isc_alfanum(m_Txt[i]) && m_Txt[i]!='\\' && m_Txt[i]!='/' )
          return false;
      
        // Toma el resto del nombre del fichero
        for( ;i<m_len; ++i )
          {
          if( isc_alfanum(m_Txt[i]) )
            continue;

          if( m_Txt[i]=='\\' || m_Txt[i]=='/' )
            continue;

          // El punto solo si esta entre letras o numeros
          if( m_Txt[i]=='.' && isc_alfanum(m_Txt[i-1]) && isc_alfanum(m_Txt[i+1]) )
            continue;

          break;
          }

        Ora += m_Txt.Substring( _i, i-_i );    // Coje el nombre de fichero
        _i = i-1;
        return true;
        }

      return false;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Chequea si a partir del caracter 'i' viene un marca de no traducción
    //------------------------------------------------------------------------------------------------------------------
    private void CheckNoTrdMark( ref int i, ref string Ora )
      {
      int len1 = m_NTIni.Length;
      int len2 = m_NTEnd.Length;

      // Chequea si viene una marca de inicio de no traducción
      if( len1>0 && i+len1<m_len && m_Txt.Substring(i, len1) == m_NTIni )
        {
        i   += len1;                          // La salta la marca
        Ora += m_NTIni;                       // Incorpora la marca al texto

        if( m_nNoTrd>0 && m_NTIni==m_NTEnd )  // Si son iguales las marcas y ya hay una
          --m_nNoTrd;                         // La toma como final (Decrementa contador)
        else
          ++m_nNoTrd;                         // Incrementa contador
        }

      // Chequea si viene una marca de final de no traducción
      if( len2>0 && i+len2<m_len && m_Txt.Substring(i, len2) == m_NTEnd )
        {
        if( m_nNoTrd>0 )                      // Si no esta macheada la ignora
          {
          i   += len2;                        // La salta
          Ora += m_NTEnd;                     // Incorpora la marca al texto

          --m_nNoTrd;                         // Decrementa contador
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene un tag HTML, y lo pone en la cadena 'Txt', para que esta función trabaje 'i' debe apuntar al caracter '<' 
    // si se puede obtener el TAB devuelve 'true' e 'i' apunta al ultimo caracter del Tab en otro caso devuelve 'false' 
    // y no se modifica ni 'i' ni 'Txt'.
    // Nota: Esta función tambien actualiza las siguientes variables.
    //        m_TagName     - Nombre del tag analizado.                 
    //        m_TagEnd      - Si el tag es un terminador.
    //        m_SkipAll     - Si se salto todo hasta el tag terminador.
    //------------------------------------------------------------------------------------------------------------------
    private bool GetHtmlTag( ref int _i, ref string Text )
      {
      string Skip = "<!--><COMMENT><EMBED><OBJECT><SCRIPT><STYLE>";

      m_TagName="";

      int i = _i;                                       // Peserva donde se inicia el analisis
      if( m_Txt[i] != '<' )                             // Solo si se esta al inicio del TAG
        return false;

      ++i;                                              // Salta el primer caracter
      while( m_Txt[i] == ' ' ) ++i;                     // Salta los espacios en blanco

      if( m_Txt[i] == '/' )                             // Es un TAG terminador
        {
        m_TagEnd = true;                                // Si es terminador

        ++i;                                            // Salta backslat
        while( m_Txt[i] == ' ' ) ++i;                   // Salta los espacios en blanco
        }
      else
        m_TagEnd = false;                               // No es terminador
      
      int j = i;
      for(; m_Txt[j]>' ' && m_Txt[j]!='>'; ++j );       // Salta el nombre

      if( j>=m_len || j==i )                            // No se obtuvo el TAG correctamente
        return false;

      m_TagName = m_Txt.Substring(i, j-i);              // Copia el nombre del TAG
      m_TagName.ToUpper();                              // Lo convierte a mayusculas

      i = j;                                            // Apunta al final del nombre

      m_SkipAll = false;                                // En principio no saltar hasta terminador

      if( m_TagEnd == false )                           // Si no es un TAG terminador
        {
        string fTag = '<'+m_TagName+'>';                // Pone un seperador para buscar

        if( Skip.Contains(fTag) )                       // Debe saltar hasta el TAG terminador?
          {
          string sEnd = "</"+m_TagName+'>';             // Forma nombre del terminador

          if( sEnd == "</!-->")                         // Caso especial para comentarios
            sEnd = "-->";

          int idx = m_Txt.IndexOf( sEnd, i);            // Busca el tag terminador
          if( idx>=0 )                                  // Lo encontro
            {
            i = idx + sEnd.Length;                      // Apunta al final del TAG
            m_SkipAll = true;                           // Es una Tag que salta todo
            }
          }
        }
        
      bool en = false;
      while( i<m_len )                                  // Salta hasta el final del tag
        {
        if( m_Txt[i] == '"' )                           // Machea comillas
          en = !en;

        if( m_Txt[i] == '>' && !en )                    // Terminador fuera de comillas
          break;

        ++i;                                            // Salta al proximo caracter
        }
        
      if( m_Txt[i] != '>' )                             // No encontro el final
        return false;

      ++i;                                              // Salta fin de TAG

      string sTag = m_Txt.Substring(_i, i-_i );         // Coje el contenido completo del TAG

      //if( i_Modify != NULL )                            // Si hay interface de modificacion
      //  {
      //  BSTR tmp1 = m_TagName.AllocSysString();         // Convierte TagName a BSTR
      //  BSTR tmp2 = sTag.AllocSysString();              // Convierte Tag a BSTR 
      //  i_Modify->ModifyTag( tmp1, &tmp2, m_TagEnd );   // LLama función de conversion

      //  sTag = tmp2;                                    // Actualiza Tag modificado
      //  }

      _i = i-1;                                           // Actualiza el puntero

      Text += sTag;                                       // Adicina el TAG a la informacion acumulada

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene todos los tags consecutivos que son internos a una oración
    //------------------------------------------------------------------------------------------------------------------
    private bool GetInnerHtmlTags( ref int _i, ref string iTags, ref int nTags )
      {
      string nowTag = "";
      int         i = _i;                       // Posición por donde se esta analizando
      int      iEnd = _i;                       // Posición hasta donde llegan los tags validos

      nTags = 0;                            // Pone # de tags a 0
      while( GetHtmlTag(ref i, ref nowTag ) )     // Obtiene el TagHTML
        {  
        ++i;                                // Salta final del TAG

        if( m_TagName=="IDX" )              // Tab especial para IdiomaX
          break;

        ++nTags;                            // Contador de # de tags
        if( !IsInnerParagraf(m_TagName) )   // Si no puede estar interno a la oración
          return false;                     // Falla la función

        iTags += nowTag;                    // Agrega Tag a la lista de Tags OK
        nowTag = "";                     // Limpia Tag actual
        iEnd = i;                           // Ultima posición valida

        while( i<m_len && m_Txt[i]<=' ' )   // Salta posibles tarecos 
          nowTag += m_Txt[i++];             // Agrega los posibles tarecos al tag
        }

      if( iTags.Length >0 )            // Obtuvo algun TAG?
        {
        _i = iEnd-1;                        // Actuliza ultima pocicion analizada
        return true;                        // Return OK
        }

      return false;                         // No TAGs, Falla la función
      }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza el caracter i y determina según el formato que se este usando si es un caracter especial o no, si es un 
    // caracter normal devuelve el caracter tal como es, si es un caracter especial lo decodifica y devuelve su equivalente 
    // ANSI ademas adelanta el apuntador _i hasta el final de la definicion del caracter.
    // Nota: Esta función tambien actualiza las siguientes varibles.
    //        m_TypeChar - Tipo de caracter 0 normal, 1 - Especial RTF, 2- Especial HTML
    //        m_ic       - Apuntador donde comenzo la definición del caracter
    //        m_sChar    - Definición original del caracter
    //
    //------------------------------------------------------------------------------------------------------------------
    private char GetChar( ref int _i )
      {
      m_TypeChar = C_ANSI;                            // Por defecto el caracter es ANSI
      m_ic       = _i;                                // Solo un caracter
      char     c =  m_Txt[_i];                        // Toma caracter a analizar

      if( IsHtml() && c=='&' )                        // Texto HTML e inicio de definición de caracter
        c = GetCharHtml(ref _i);                      // Procesa caracteres HTML especiales
      else
        {
        if( IsRtf() && c=='\\' )                      // Texto RTF e inicio de definición de caracter
          {
          int j = _i;
          c = GetCharRtf(ref _i);                     // Procesa caracteres RTF especiales
          if( _i > j)
            {
            m_sChar = m_Txt.Substring(j, _i-j+1 );    // Obtiene cadena que representa al caracter
            m_TypeChar = C_RTF;                       // Declara tipo de caracter
            }
          }
        }

      if( c == '×' ) c = 'x';                          // Para evitar que hayan caracteres de sustitucion en el texto

      if( c!=' ' ) m_nSp = 0;                         // Resetea contador de &nbsp; seguidos

      return c;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene el codigo ANSI de un caracter definido según sintaxis de HTML, '_i' apunta al ultimo caracter de la 
    // definición del caracter.
    //------------------------------------------------------------------------------------------------------------------
    private char GetCharHtml( ref int _i )
      {
      string Names =  "&copy;    &die;     &laquo;   &not;     &ordf;    &sect;    &uml;     &Aacute;  " +
                      "&aacute;  &Acirc;   &acirc;   &acute;   &AElig;   &aelig;   &Agrave;  &agrave;  " +
                      "&amp;     &Aring;   &aring;   &Atilde;  &atilde;  &Auml;    &auml;    &brkbar;  " +
                      "&brvbar;  &Ccedil;  &ccedil;  &cedil;   &cent;    &curren;  &deg;     &divide;  " +
                      "&Eacute;  &eacute;  &Ecirc;   &ecirc;   &Egrave;  &egrave;  &ETH;     &eth;     " +
                      "&Euml;    &euml;    &frac12;  &frac14;  &frac34;  &gt;      &hibar;   &Iacute;  " +
                      "&iacute;  &Icirc;   &icirc;   &iexcl;   &Igrave;  &igrave;  &iquest;  &Iuml;    " +
                      "&iuml;    &lt;      &macr;    &micro;   &middot;  &nbsp;    &Ntilde;  &ntilde;  " +
                      "&Oacute;  &oacute;  &Ocirc;   &ocirc;   &Ograve;  &ograve;  &ordm;    &Oslash;  " +
                      "&oslash;  &Otilde;  &otilde;  &Ouml;    &ouml;    &para;    &plusmn;  &pound;   " +
                      "&quot;    &raquo;   &reg;     &shy;     &sup1;    &sup2;    &sup3;    &szlig;   " +
                      "&THORN;   &thorn;   &times;   &Uacute;  &uacute;  &Ucirc;   &ucirc;   &Ugrave;  " +
                      "&ugrave;  &Uuml;    &uuml;    &Yacute;  &yacute;  &yen;     &yuml;    &#8216;   " +
                      "&#8217;   &#8230;   &#8220;   &#8221;   &lsquo;   &rsquo;   ";

      string Letras = "©¨«¬ª§¨Á" +
                      "áÂâ´ÆæÀà" +
                      "&ÅåÃãÄä¦" +
                      "¦Çç¸¢¤°÷" +
                      "ÉéÊêÈèÐð" +
                      "Ëë½¼¾>¯Í" +
                      "íÎî¡Ìì¿Ï" +
                      "ï<¯µ· Ññ" +
                      "ÓóÔôÒòºØ" +
                      "øÕõÖö¶±£" +
                      "\"»®­¹²³ß" +
                      "Þþ×ÚúÛûÙ" +
                      "ùÜüÝý¥ÿ‘" +
                      "’…“”‘’";   

      int i = _i;                                               // Preserva el valor del puntero
                                                  
      if( m_Txt[i] == '&' )                                     // Es la definición de un caracter
        {                                         
        ++i;                                                    // Salta el caracter &
                                                  
        while( i<m_len && m_Txt[i]!=';' )                       // Salta definición del caracter
          ++i;                                    
                                                  
        if( i-_i<10 && m_Txt[i]==';' )                          // Encontro definición de un caracter        
          {                                       
          ++i;                                                  // Salta el terminador (;)
                                                  
          m_sChar = m_Txt.Substring( _i, i-_i );                // Obtiene cadena que representa al caracter
                                                  
          if( m_sChar=="&nbsp;" )                               // Para contador de &nbsp; 
            {                                     
            if( m_nSp==0 )                                      // Es el primer espacio
              m_iSp = _i;                                       // Guarda donde empiezan
                                                  
            ++m_nSp;                                            // Incrementa el numero de espacios
            }                                     
                                                  
          int idx = Names.IndexOf( m_sChar );                   // Busca en nombres definidos
                                                  
          if( idx >= 0 )                                        // Lo encontro
            {                                     
            _i = i-1;                                           // Actualiza el puntero
            m_TypeChar = C_HTML;                                // Declara tipo de caracter
            return Letras[ idx/10 ];                            // Retorna codigo ANSI correspondiente
            }

          if( m_sChar[1] == '#' )                               // Letra definida por un numero
            {
            if( m_sChar[2] == 'x' ) // Letra definida por un numero hexadecimal
              {
              try
                {
                string sNum = m_sChar.Substring(3);             // Resto de la cadena
                int iCode = Convert.ToInt32( sNum, 16 );        // Obtiene el numero
                if( iCode>0 && iCode <= 255)                    // Es un codigo ANSI
                  {
                  _i = i-1;                                     // Actualiza el puntero
                  m_TypeChar = C_HTML;                          // Declara tipo de caracter
                  return (char)iCode;                           // Retorna codigo ANSI
                  }
                }
              catch{}
              }
            else                    // Letra definida por un numero decimal
              {
              int n;
              bool ret = int.TryParse( m_sChar.Substring(2), out n );  // Obtiene el numero
              if( ret && n <= 255 )                             // Es un codigo ANSI
                {
                _i = i-1;                                       // Actualiza el puntero
                m_TypeChar = C_HTML;                            // Declara tipo de caracter
                return (char)n;                                 // Retorna codigo ANSI
                }
              }
            }

          // Es un nombre de caracter no reconocido, debe ser unicode
          m_TypeChar = C_HTML_UNICODE;
          _i = i-1;                                             // Actualiza el puntero
          return '*';
          }
        }

      return m_Txt[i];                                          // Retorna el caracter normal
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene un tag (comando) RTF, y lo pone en la cadena 'Txt', para que esta función trabaje 'i' debe apuntar a un 
    // caracter '{' o '\', si se puede obtener el TAG devuelve 'true' e 'i' apunta al ultimo caracter del Tag, en otro 
    // caso devuelve 'false' y no se modifica ni 'i' ni 'Txt'.
    // Nota: Esta función tambien actualiza las siguientes variables.
    //        m_TagName  - Nombre del tag (comando) analizado.
    //        m_TagpAll  - Si se salto toda la información que contenia el comando.
    //------------------------------------------------------------------------------------------------------------------
    private bool GetRtfTag( ref int _i, ref string Text )
      {
      string Skip = "<pict><info><fonttbl><stylesheet><colortbl><objdata><objclass><generator><xe>";

      int i = _i;                               // Peserva donde se inicia el analisis
      if( m_Txt[i]!='{' && m_Txt[i]!='\\' )     // Solo si se esta al inicio del TAG
        return false;

      if( i>0 && m_Txt[i-1]=='\\' )             /* Pecedidos de '\' no son TAG RTF        */
        return false;

      int key = -1;                             // Marcador de incio de llave abierta (grupo)
      if( m_Txt[i]=='{' )                       // Salta la llave
        {
        key = i++;                              // Indice de inicio de llave
        while( i<m_len && m_Txt[i]<=' ')        // Salta tarecos
          ++i;

        if( m_Txt[i]!='\\' )                    // Despues de la llave tiene que venir '\'
          return false;
        }

      m_SkipAll = false;                        // Inicializa bandera de saltar comando
      ++i;                                      // Salta el '\'                           
      if( m_Txt[i]=='*' )                       // \*\ indica que el tag se puede ignorar 
        {
        ++i;                                    // Salta el * 
        while( i<m_len && m_Txt[i]<=' ')        // Salta posibles tarecos
          ++i;

        if( m_Txt[i]!='\\' )                    // Tiene que ser '\*\'       
          return false;

        ++i;                                    // Salta el '\'                           
        m_SkipAll = (key!=-1);                  // Pone bandera solo si es un grupo
        }        

      int j=i; 
      for(; isc_alfa(m_Txt[j]); ++j );          // Salta nombre del comando (letras)
      if( j==i )                                // Nombre de comando vacio ?
        return false;                           // Retorna

      m_TagName = m_Txt.Substring(i, j-i);      // Copia el nombre del comando
      i = j;                                    // Apunta al final del nombre

      if( m_Txt[i]=='-' && isc_num(m_Txt[i+1]) )// Signo negativo del parametro
        ++i;                                    // lo salta

      while( isc_num(m_Txt[i]) ) ++i;           // Salta los numeros (Parametro)

      if( m_Txt[i]==' ' )                       // Termino con un espacio
        ++i;                                    // Salta el espacio

      if( !m_SkipAll && key!=-1 )               // Hay que determinar si se salta al final?
        {
        string fTag = '<'+m_TagName+'>';        // Pone un separador para buscar

        m_SkipAll = Skip.Contains(fTag);        // Verdadero si esta en la lista
        }

      if( m_SkipAll  )                          // Hay que saltar hasta final del grupo?
        FindMatch( ref i, '{', '}' );           // Busca fin del grupo
        
      Text += m_Txt.Substring(_i, i-_i );       // Adicina el TAG a la informacion acumulada

      _i = i-1;                                 // Actualiza el puntero al ultimo caracter

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Encuentra el caracter 'cEnd' que corresponde un caracter 'cIni' que esta delante de 'i', teniendo en cuenta que 
    // la pareja de caracteres pueden estar anidados
    //------------------------------------------------------------------------------------------------------------------
    private bool FindMatch( ref int _i, char cIni, char cEnd )
      {
      int Match = 1;                                    // Supone que ya se paso por un 'cIni'
      for( int i=_i; i<m_len; ++i )                     // Recorre todos los caracteres despues de _i
        {                                    
        if( m_Txt[i] == '\\'  )                         // Si es una palabra de control
          {
          ++i;
          if( i+3<m_len && m_Txt[i]=='b' && m_Txt[i+1]=='i' && m_Txt[i+3]=='n' )  // Es bin
            {
            StringBuilder sNum = new StringBuilder();
            for( i+=3; isc_num(m_Txt[i]); ++i )         // Obtine el numero de bytes
              sNum.Append( m_Txt[i] );

            if( sNum.Length>0 )                         // Si se obtuvo el numero de bytes
              i += int.Parse(sNum.ToString());          // Salta todos los bytes
            }
          }

        if( m_Txt[i] == cIni )                         // Caracter incial
          ++Match;                                     // Aumenta contador
                                             
        if( m_Txt[i] == cEnd )                         // Caracter final
          --Match;                                     // Disminulle contador
                                             
        if( Match==0 )                                 // Igual la cantidad de cIni y cEnd
          {                                  
          _i = i+1;                                    // Actualiza apuntador
          return true;                                 // Retorna OK
          }                                  
        }                                    
                                             
      return false;                                    // No encontro macheo  
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene el codigo ANSI de un caracter definido según sintaxis de RTF, '_i' apunta al ultimo caracter de la 
    // definición del caracter.
    //------------------------------------------------------------------------------------------------------------------
    private char GetCharRtf( ref int _i )
      {
      if( m_Txt[_i]=='\\' )                                   // Definición de un caracter especial
        {                                                    
        if( m_Txt[_i+1]=='{'  ||                              // Para { -> '\{'
            m_Txt[_i+1]=='}'  ||                              // Para } -> '\}'
            m_Txt[_i+1]=='\\' )                               // Para \ -> '\\'
          {
          ++_i;
          return m_Txt[_i];
          }

        // Busca caracteres especiales en RTF
        string cmdName = "\\rquote   \\lquote   \\rdblquote\\ldblquote\\emdash   \\endash   \\~        \\emspace  \\enspace  \\-        \\_        \\";
        char[] cmdChar = {'\''        ,'\''      ,'\"'      ,'\"'      ,'-'       ,'-'       ,' '       ,' '       ,' '       ,'\0'      ,'\0'      ,};

        // Obtine comando RTF
        int i = _i + 1;                                       // Salta backslat

        // Comandos especiales de un solo caracter
        if( m_Txt[i]=='~' || m_Txt[i]=='-' || m_Txt[i]=='_')
          ++i;
        else
          while( m_Txt[i]>' ' && m_Txt[i]!='\\' )             // Salta caracteres del comando
            ++i;                                  
                                                  
        string cmd = m_Txt.Substring(_i, i-_i );              // Obtiene comando
                                                  
        int idx = cmdName.IndexOf(cmd);                       // Busca si es nombre de caracter
        if( idx>=0 )                                          // Si es un nombre de caracter
          {                                       
          if( m_Txt[i]==' ' ) ++i;                            // Salta espacio al final
          _i = i-1;                                           // Retorna ultimo caracter leido
                                                  
          return cmdChar[idx/10];                             // Retorna caracter equivalente
          }                                       
                                                  
        if( m_Txt[_i+1]=='\'' )                               // Codigo del caracter en hexagesimal
          {
          try
            {
            char[] hNum = { m_Txt[_i+2], m_Txt[_i+3] };       // Forma el número
            string sNum = new string(hNum);                   // Resto de la cadena
            int n = Convert.ToInt32( sNum, 16 );              // Obtiene el numero
            if( n>0 && n <= 255)                              // Es un codigo ANSI
              {                                  
              _i += 3;                                        // Apunta al final
              return (char)n;                                 // Devuelve el codigo del caracter
              }                                  
            }                                    
          catch{}                                
          }
        }

      return m_Txt[_i];                           // Retorna el caracter actual
      }

    //------------------------------------------------------------------------------------------------------------------
    // Obtiene todos los tags consecutivos que son internos a una oración
    //------------------------------------------------------------------------------------------------------------------
    private int GetInnerRtfCmd( ref int _i, ref string iCmd )
      {
      string Cmds = "";
      bool    tab = false;                      // Bandera para si hay tabuladores
      int       i = _i;                         // Guarda posicion temporalmente

      m_icmd = _i;                              // Indice donde termina el comando

      for(;;)                                   // Para todos los comandos consecutivos
        {     
        if( m_Txt[i]=='}' )                     // Fin de bloque
          {
          Cmds += m_Txt[i++];                   // Guarda la llave y salta

          while( i<m_len && m_Txt[i]<' ' )      // Salta posibles tarecos 
            Cmds += m_Txt[i++];                 // Guarda los tarecos
          }

        int k = i;                              // Apuntador temporar a caracter leido
        GetCharRtf(ref k);                      // Procesa caracteres RTF especiales
        if( k>i )                               // Es una caracter especial
          {
          int len = Cmds.Length;                // Longitud de los comandos
          if( len!=0 && Cmds[len-1] != ' ')     // El comando no termina en espacio
            Cmds += ' ';                        // Se lo agrega

          break;                                // Termina..
          }

        if( !GetRtfTag(ref i, ref Cmds ) )      // Obtiene un comando RTF
          break;                                // No puedo obternelo termina

        m_icmd = ++i;                           // Indice donde termina el comando
        if( m_TagName=="tab" )                  // Encontro un TAB
          tab = true;                           // No decide hasta el final
        else
          if( !IsInnerParagraf(m_TagName) )     // Si no puede estar interno a la oración
            return -1;                          // No todos son internos

        while( i<m_len && m_Txt[i]< ' ' )       // Salta posibles tarecos 
          Cmds += m_Txt[i++];                   // Guarda los tarecos
        }

      int len2 = Cmds.Length;                   // Longitud de los cmd analizados
      if( len2 >0 )                             // Obtuvo algun Comando?
        {
        if( tab )                               // Si hubo un tab intermedio
          {
          int j=i;                              // Utiliza indice temporar
          while( j<m_len && m_Txt[j]<=' ' )     // Salta posibles tarecos y espacio
            ++j;

          if( isc_up(m_Txt[j]) )                // Sigue un caracter en mayusculas
            return -1;                          // El tab termina la oración
          }

        _i = i;                                 // Actuliza ultima pocicion analizada
        iCmd = Cmds;                            // Lista de comandos obtenidos
        }

      return len2;                              // Longitud de los comandos internos
      }

    //------------------------------------------------------------------------------------------------------------------
    // Determina si la palabra que sigue al punto es una de las exteniones mas conocidas para nombres de ficheros
    //------------------------------------------------------------------------------------------------------------------
    private bool IsExt(int i)
      {
      int j=i;
      for(; isc_alfa(m_Txt[j]); ++j );                    // Salta caracteres
      if( j==i )                                          // No encontor ninguno
        return false;                                     // Retorna no extension

      string Exts = "|net|com|gob|doc|txt|es|mx|htm|html|exe|dll|xml|rtf|bmp|jpe|zip||psd|pdf|rar|reg|mp|avi|dat|ttf|hlp" +
                    "|gid|sys|cfg|gif|pnp|wmf|wmv|wma|asf|mid|dwg|pdf|mht|ini|bad|log|tmp|drv|ocx|inf|";

      string Ext = m_Txt.Substring(i, j-i).ToLower();  ;  // Forma palabra para buscar
      
      return Exts.Contains( '|' + Ext + '|' );            // Return true si esta en el listado de extensiones
      }

    //------------------------------------------------------------------------------------------------------------------
    // Trata de determinar si la palabra que antecede al punto es una abreviatura, o no.
    //------------------------------------------------------------------------------------------------------------------
    private bool IsAbr(int i)
      {
      int j=i;
      for( ; j>0 && isc_alfa(m_Txt[j]); --j );            // Salta caracteres hacia atras
      if( j==i )                                          // No encontro nada
        return false;                                     // Retorna no abreviatura

      if( j==0 )                                          // Es una sola palabra
        return true;                                      // No corta

      if( j+1 == i && m_Txt[j]=='.' )                     // Si es X.X.X. ó x.x.x
        return true;                                      // Retorna no extension

      int n = i-j;
      if( isc_up(m_Txt[j+1]) && n>1 && n<5 )              // Comienza en mayuscula y tiene 2,3 0 4 letras
        {                                                 // Ej: Xxx.
        for( int k=j+2; k<i; ++k )                        // Para los demas caracteres
          if( isc_up(m_Txt[k]) )                          // Es mayuscula
            return false;                                 // No es abreviatura

        return true;                                      // Es abreviatura
        }

      while( j>0 && m_Txt[j] <= ' ')                     // Salta tarecos hacia atras
        --j;

      if( m_Txt[j]=='(' || m_Txt[j]=='{'  || m_Txt[j]=='[' || // Caracter de agrupamiento
          m_Txt[j]=='"' || m_Txt[j]=='“'  )                   // Ej: ( xxxx. ó "xxxxx.
        return true;                                          // Retorna si abreviatura

      return false;                                           // Retorna no abreviatura
      }

    //------------------------------------------------------------------------------------------------------------------
    // Adiciona un item a la lista
    //------------------------------------------------------------------------------------------------------------------
    private bool AddItem(char Type, string Text)
      {
      if( string.IsNullOrEmpty(Text) ) return false;

      string Txt = Text;
      if( IsHtml() && (Type=='c' || Type=='s') )                    // Busca atributos traducibles
        {
        int    init=0;                                              // Inicio de la busqueda
        string aVal="";                                             // Valor del atributo
        while( SustHtmlAttrVal( ref init, ref Txt, ref aVal ) )     // Sust. Atrib. por una marca
          {
          if( aVal.Length > 0 )                                     // Solo si tiene valor
            Items.Add( new CItem(aVal,'T') );                       // Guarda valor como Item
          }
        }

      Items.Add( new CItem(Txt,Type) );

      return true;
      }

    //------------------------------------------------------------------------------------------------------------------
    // Determina si el TAG 'Tag' puede estar dentro de un parrafo o no.
    //------------------------------------------------------------------------------------------------------------------
    private bool IsInnerParagraf(string Tag)
      {
      // Lista de comandos HTML que pueden estar dentro de una oración
      string InHTM = "<A><ADRESS><AREA><B><BASE><BASEFONT><BGSOUND><BIG><CITE><EM><FONT><I><KBD><LINK><LISTING><NOBR><S>"+
                    "<SAMP><SMALL><STRIKE><STRONG><SUB><SUP><U><VAR><XMP><SPAN>";         

      // Lista de comandos RTF que seaparan las oraciones
      string InRTF = "<par><cell><row><tab><nestcell><nestrow><sect><pard><tcelld><trowd>";

      string sTag = '<'+Tag+'>';              // Pone seperadores para buscar

      bool ret; 
      if( IsHtml() )                          // Si el texto es HTML
        ret = InHTM.Contains(sTag);           // Busca en lista de comandos HTML
      else                                    // Si el texto es RTF
        ret = InRTF.Contains(sTag);           // Busca en lista de comandos RTF

      return ret;                             // Si lo encuentra retorna true
      }

    //------------------------------------------------------------------------------------------------------------------
    // Busca un atributo traducible, comenzando por el caracter 'i' de 'Text' y lo sustitulle por una marca de sustitución 
    // y devuelve el valor del atributo en 'aVal'
    //------------------------------------------------------------------------------------------------------------------
    private bool SustHtmlAttrVal( ref int i, ref string Text, ref string aVal )
      {
      aVal="";                                  // Inicializa a vacio

      int len = Text.Length;                  // Obtiene longitud de la cadena

      int idx = Text.IndexOf( "alt", i );
      if( idx < 0 )                              // No lo encontro
        return false;

      idx += 3;                                   // Salta el atributo
      while( idx<len && Text[idx]<=' ')           // Salta todos los espacios
        ++idx;

      if( Text[idx] != '=' )                    // Si no esta seguido de igual
        {                                       // No es un atributo
        i = idx;                                 // Actualiza puntero
        return true;                            // Retorna true sin cambiar nada
        }

      ++idx;                                      // Salta el igual
      while( idx<len && Text[idx]==' ')                   // Salta todos los espacios
        ++idx;

      char c = ' ';                                       // Caracter final
      if( Text[idx] == '"' || Text[idx] == '\'' )         // Si esta encerrado en " ó '
        c = Text[idx++];                                  // Lo toma como caracter fianal

      int pIni = idx;                           // Puntero a inicio del valor
      while( Text[idx] != c )                           // Busca hasta el final
        {
        if( Text[idx] == '\0' )                          // Si llega al final de la cadena
          return false;                         // Retorna que no lo encontro

        aVal += Text[idx];                      // Toma el valor
        ++idx;                                    // Pasa a la otra letra
        }

      if( aVal.Length > 0 )                       // Si obtuvo un valor
        Text = Text.Substring(0,pIni)             // Parte inicial de la cadena
             + SUST_MARK                          // Marca de sustitución
             + Text.Substring(idx);               // Parte final de la cadena

      i = pIni + 1;                             // Actualiza puntero
      return true;                              // Retorna OK
      }

    //------------------------------------------------------------------------------------------------------------------
    // Pica la oracion según la pocision de la marca de no traducción, tambien se tienen en cuenta las marcas de 
    // sustitución que estan en la parte que no se va ha traducir.
    //------------------------------------------------------------------------------------------------------------------
    private void SlitByNoTrdMark( ref string Ora, bool NtIni, bool NtEnd, ref CInnerTags ITags )
      {
      int SpIni = -1;
      int SpEnd = Ora.Length;
      if( NtIni )                                   // Comienza con una marca
        {                                
        SpIni = Ora.IndexOf( m_NTEnd );             // Busca la marca final
        if( SpIni == -1 )                           // No la encuentra (Toda la oración)
          SpIni = Ora.Length;                       // Logitud de la oracion
        }                                
                                         
      if( NtEnd )                                   // Termina con una marca
        {                                
        SpEnd = 0;                                  // Asume toda la oración
        for( int i=0;;)
          {
          int iFind = Ora.IndexOf( m_NTIni, i );    // Busca la marca final
          if( iFind == -1 )                         // No la encuentra (Toda la oración)
            break;

          SpEnd = iFind;                            // Ultima marca encontrada
          i     = iFind + m_NTIni.Length;
          }
        }

      int iNew = 0;                                 // Nuevo indice en el arreglo
      int dt = 0;                                   // Incremento del tamaño de la oración
      string s1,s2;
      for( int i=0; i<ITags.Len(); ++i )            // Recorre todas las marcas
        {
        int Idx = ITags.GetiOra(i) + dt;            // Recalcula el indice a la marca
        if( Idx<SpIni || Idx>=SpEnd )               // Marca dentro del segmento inicial o final
          {                                         // Sustitulye la marca
          s1 = Ora.Substring(0,Idx);                // Obtiene primera mitad
          s2 = Ora.Substring(Idx+1);                // Obtiene segunda mitad

          Ora = s1 + ITags.GetTxt(i) + s2;          // Inserta el texto donde esta la marca
          int len = ITags.GetTxt(i).Length-1;       // Numero que se incrementa la oración

          dt += len;                                // Suma lo que crece la oración
                                                  
          if( Idx<SpIni ) SpIni += len;             // Recalcula segmento inicial 
          if( Idx<SpEnd ) SpEnd += len;             // Recalcula segmento final      
          }                                       
        else                                      
          ITags.Copy( i, iNew++ );                  // Indices no sustituidos
        }                                         
                                                  
      ITags.SetLen(iNew);                           // Redimensiona arreglo de TAGs internos
                                                  
      if( SpIni >= SpEnd )                          // Toda la oración no se traduce
        {                                         
        AddItem( 'c', Ora );                        // Adiciona oración a la lista de items
        return;                                   
        }                                         
                                                  
      // Analiza Segmento inicial si esxiste      
      if( SpIni > 0 )                               // Procesa parte inicial
        {                                         
        s1 = Ora.Substring(0,SpIni);                // Separa primer segmento
        AddItem( 'c', s1 );                         // Adiciona segmento a la lista de items
        }                                         
                                                  
      // Analiza parte intermedia                 
      if( SpIni<0 )                               
        SpIni = 0;                                
      else                                        
        SpIni += m_NTEnd.Length;                    // Salta la marca
                                                  
      s1 = Ora.Substring( SpIni, SpEnd-SpIni );     // Toma parte intermedia
                                                  
      for( int i=0; i<ITags.Len(); ++i )            // Recorre items no sutituidos
        AddItem( 's', ITags.GetTxt(i) );            // Adiciona texto a la lista de items
                                                  
      AddItem( 't', s1 );                           // Adiciona oración a la lista de items
                                                  
      SpEnd += m_NTIni.Length;                      // Salta la marca     
                                                  
      // Analiza segmento final si existe         
      if( SpEnd < Ora.Length )                      // Procesa parte final ?
        {                                         
        s1 = Ora.Substring(SpEnd);                  // Separa segmento final
        AddItem( 'c', s1 );                         // Adiciona segmento a la lista de items
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    // Determina si al final de la oración hay una marca de sustitución que se puede quitar
    //------------------------------------------------------------------------------------------------------------------
    private int DelEndSustMark( ref int i, ref string Ora, ref CInnerTags ITags)
      {
      int n = ITags.Len()-1;              // Indice al ultimo TAG interno
      int k = Ora.Length-1;               // Indice al ultimo caracter de 'Ora'

      for( ;k>=0 && n>=0; --k )           // Analiza 'Ora' desde atras
        {
        if( Ora[k]<=' ' )                 // Caracteres sin importancia al final
          continue;                       // Continua analizando

        if( Ora[k] == SUST_MARK )         // Encuentra marca de sustitución
          {
          i = ITags.GetiTxt(n--);         // Obtiene inicio del TAG en el texto
          continue;                       // Continua analizando
          }

        break;                            // Termina el analisis
        }

      if( n < ITags.Len()-1 )             // Si encontro alguna marca de sustitución
        {
        Ora = Ora.Substring(0,k+1);       // Quita caracteres analizados (del final)
        ITags.SetLen( n+1 );              // Quita TAGs analizados del arreglo
        }

      return i;                           // Retorna caracter para seguir analisis
      }

    //------------------------------------------------------------------------------------------------------------------
    // Analiza la oración obtenida para ver si todavia permanece comenzando con un bullet, tiene en cuanta los siguientes 
    // formatos:
    //    Romano  - Ej: I- II- III- ó I. II. III. ó I) II) III) ó I)- II)- III)- 
    //              Ej: i- ii- iii- ó i. ii. iii. ó i) ii) iii) ó i)- ii)- iii)- 
    //    Numero  - Ej: 1- 2- 3- ó 1. 2. 3. ó 1) 2) 3) ó 1)- 2)- 3)- 
    //    Letra   - Ej: a- b- c- ó a. b. c. ó a) b) c) ó a)- b)- c)- 
    //------------------------------------------------------------------------------------------------------------------
    private bool GetBullet2( ref string Ora, ref CInnerTags ITags )
      {
      bool num = false;
      int    j = 0;

      while( Ora[j]=='I' || Ora[j]=='V' || Ora[j]=='X' )      // Romanos mayusculas
        ++j;

      if( j == 0 )
        while( Ora[j]=='i' || Ora[j]=='v' || Ora[j]=='x' )    // Romanos minusculas
          ++j;

      if( j == 0 )
        while( isc_num(Ora[j]) )                              // Numeros 
          { 
          ++j; 
          if( Ora[j]=='.' && isc_num(Ora[j+1]) )              // Si 2.1 ...
            { j+=2; num = true; }                             // Salta el punto y num.
          }

      if( j == 0 && isc_low(Ora[0]))                          // Un caracter en minuscula                         
        j = 1;                                                // Lo toma
      
      if( j!=0 &&  (Ora[j]=='-' || Ora=="." ||                // Seguido de - ó .
          num || Ora[j]==')' || Ora==":") )                   // Seguido de ) ó :
        {
        if( !num ) ++j;                                       // Salta caracter adicional

        if( Ora[j]=='-' ) ++j;                                // Opcionalmente - Ej a)- i.- a:-  

        int nMark = 0;
        bool sp   = false;                                    // Bandera para espacio

        for(;;)                                               // Busca caracteres que le siguen
          {
          if( Ora[j] == SUST_MARK )                           // Marca de sustitución
            ++nMark;
          else if( Ora[j] == ' ' || Ora[j] == '\t')           // Espacio o tabulador
            sp = true;
          else
            break;                                            // Termina la busqueda

          ++j;                                                // Salta el caracter
          }

        if( sp )                                              // Encontro espacio, es un bullet
          {
          string sIni = Ora.Substring(0,j);                   // Toma el bullet mas la cascara
          Ora = Ora.Substring(j);                             // Lo quita de la oración

          for( int n = nMark-1; n>=0; --n )                   // Busca todas las marcas que habia
            {
            int i = ITags.GetiOra(n);                         // Posición de la marca en la oración

            sIni = sIni.Substring(0,i) + ITags.GetTxt(n) + sIni.Substring(i+1);  // Sustituye la marca

            ITags.DeleteAt(n);                                // Borra la marca
            }

          AddItem( 'c', sIni );                               // Guarda la cascara

          return true;                                        // Retorna OK
          }
        }

      return false;                                           // No complio con el formato
      }

    //------------------------------------------------------------------------------------------------------------------
    // Quita caracteres especiales delante de los apotrofes, para garantizar palabras con contracción
    //------------------------------------------------------------------------------------------------------------------
    private void FixApostrofes( ref string Ora, ref CInnerTags ITags )
      {
      char[] cOra = Ora.ToCharArray();
      for(int i=0; i<ITags.Len(); ++i )                     // Busca por todas las marcas
        {
        int j = ITags.GetiOra(i);                           // Posición de la marca

        if( j==0 || j>=cOra.Length-1 )                      // La marca esta en un extremo
          continue;                                         // No hace nada

        if( (isc_alfa(cOra[j-1]) || cOra[j-1]=='\'' ) &&    // Si la marca esta entre
            (isc_alfa(cOra[j+1]) || cOra[j+1]=='\'' ) )     // letras o apostrofe
          {
          int k;

          // Busca el inicio de la palabra y corre los caracteres     
          for( k=j-1; k>=0 && (isc_alfa(cOra[k]) || cOra[k]=='\''); --k ) 
            cOra[k+1] = cOra[k];                      

          cOra[k+1] = SUST_MARK;                            // Pone la marca el principio

          Ora = new string(cOra);

          ITags.SetiOra( i, k+1 );                          // Rectifica posicion de la marca
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    // Une las marcas de sustitución que vienen juntas o separadas por espacio dentro de
    // dentro de la oración la oracion
    //------------------------------------------------------------------------------------------------------------------
    private void JoinSustMarks( ref string Ora, ref CInnerTags ITags )
      {
      for(int i=0; i<ITags.Len(); ++i )                             // Busca por todas las marcas
        {
        int nMark = 0;
        int j = ITags.GetiOra(i);                                   // Posición de la marca

        for( int k=j+1; ; ++k )                                     // Busca marcas seguidas 
          {
          if( k >= Ora.Length ) break;                              // Llego al final de la oración, termina
          if( Ora[k] == SUST_MARK )                                 // Es un caracter marca de sustitución
            ++nMark;                                                // Incrementa contador de marcas seguidas
          else if( Ora[k] > ' ' ) break;                            // No es espacio o caracter de control, termina
          }

        if( nMark > 0 )                                             // Encontro una o mas marcas seguidas
          {
          string s = ITags.GetTxt(i);                              // Coje texto de la primera marca
          for( int n=0; n<nMark; ++n )                              // Para todas las marcas consecutivas
            {
            s += ITags.GetTxt(i+1);                                 // Adiciona texto de marca actual a la anterior

            int p = ITags.GetiOra(i+1) - n;                         // Posición de la marca actual en la oración
            Ora.Remove(p);                                          // Borra caracter de la oración

            ITags.DeleteAt(i+1);                                    // Borra la marca de lista de marcas
            }

          ITags.SetTxt( i, s );                                     // Incorpora texto de toda las marcas a la inicial

          for(int k=i+1; k<ITags.Len(); ++k )                       // Para todas las marcas posteriores a la actual
            ITags.SetiOra( k, ITags.GetiOra(k) - nMark );           // Arregla posición de acuerdo a las marcas borradas
          }
        }
      }

    //------------------------------------------------------------------------------------------------------------------
    // Quita caracteres especiales que aparecen al inicio de la oración
    //------------------------------------------------------------------------------------------------------------------
    private void DelIniSustMarks( ref string Ora, ref CInnerTags ITags )
      {
      int nMark = 0;

      int j=0;
      for(;j<Ora.Length; ++j )                                      // Busca caracteres que le siguen
        {
        if( Ora[j] == SUST_MARK )                                   // Marca de sustitución
          ++nMark;
        else if( Ora[j] > ' ')                                      // Si no espacio o caracter de control
          break;                                                    // Termina la busqueda
        }

      if( j>0 )                                                     // Encontro espacio, es un bullet
        {
        string sIni = Ora.Substring(0,j);                           // Toma el bullet mas la cascara
        Ora = Ora.Substring(j);                                     // Lo quita de la oración

        for( int n = nMark-1; n>=0; --n )                           // Busca todas las marcas que habia
          {
          int i = ITags.GetiOra(n);                                 // Posición de la marca en la oración

          sIni = sIni.Substring(0,i) + ITags.GetTxt(n) + sIni.Substring(i+1);  // Sustituye la marca

          ITags.DeleteAt(n);                                        // Borra la marca
          }

        AddItem( 'c', sIni );                                       // Guarda la cascara
        }
      }

    } /************************************** FIN DE CLASE (Parse) *****************************************************/
      
    //------------------------------------------------------------------------------------------------------------------
    ///<summary> Define un elemento (Item) del texto.</summary>
    ///<remarks> Los item se caracterizan por su tipo, por el texto que lo representa, y por su traducción en el caso 
    ///que sea un item traducible</remarks>
    //------------------------------------------------------------------------------------------------------------------
    public class CItem
    { 
    //------------------------------------------------------------------------------------------------------------------
    ///<summary> Tipo de item.</summary>
    ///<remarks> En la actualidad se soportan los siguientes tipos:
    ///         <c>'c'</c> - Caracteres no traducibles fuera de la oración
    ///         <c>'s'</c> - Caracteres no traducibles dentro de la oración
    ///         <c>'t'</c> - Caracteres traducibles normales
    ///         <c>'T'</c> - Caracteres traducibles dentro de atributos
    ///</remarks>                
    //------------------------------------------------------------------------------------------------------------------
    public char   m_Type = 't';
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Texto del item original</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string m_Text;
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Texto del item traducido</summary>
    //------------------------------------------------------------------------------------------------------------------
    public string m_Trd;
        
    //------------------------------------------------------------------------------------------------------------------
    public CItem( string s, char t ) { m_Text = s; m_Type=t; }   

    } /************************************** FIN DE CLASE (CItem) *****************************************************/

    //------------------------------------------------------------------------------------------------------------------
    // Define los datos de los tags que estan en el interior de una oración
    //------------------------------------------------------------------------------------------------------------------

    class CInnerTags
    {
      const int MAX_INNTAGS = 30;

      int     _n;                                   // Número de tags actuales
      string[] _Txt = new string[MAX_INNTAGS] ;     // Texto de los tags internos
      int[]   _iOra = new int[MAX_INNTAGS];         // Posición de los tag en la oración
      int[]   _iTxt = new int[MAX_INNTAGS];         // Posicion de los tag en el texto

      public void Add( string Txt, int iOra, int iTxt )
        { if( _n < MAX_INNTAGS )
            {
            _Txt [_n] = Txt;
            _iOra[_n] = iOra;
            _iTxt[_n] = iTxt;

            ++_n;
            }
        }

      public void Copy( int nOld, int nNew )
        {
        _Txt [nOld] = _Txt [nNew];
        _iOra[nOld] = _iOra[nNew];
        _iTxt[nOld] = _iTxt[nNew];
        }

      public void DeleteAt( int n )
        {
        for( int i=n; i < _n-1; ++i )
          Copy( i, i+1 );

        --_n;
        }

      public string GetTxt (int n) { return _Txt[n]; }
      public int    GetiOra(int n) { return _iOra[n];}
      public int    GetiTxt(int n) { return _iTxt[n];}
      public int    Len()          { return _n;      }

      public void SetTxt (int n, string s ) {_Txt[n]  = s;   }
      public void SetiOra(int n, int iOra ) {_iOra[n] = iOra;}
      public void SetiTxt(int n, int iTxt ) {_iTxt[n] = iTxt;}
      public void SetLen ( int nNew       ) {_n = nNew;      }

    } /*********************************** FIN DE CLASE (CInnerTags) ***************************************************/
  }
