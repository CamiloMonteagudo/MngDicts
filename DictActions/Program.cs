using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace Tests
  {
  static class Program
    {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
      {
      Application.CurrentCulture = CultureInfo.InvariantCulture;
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main());
      }
    }
  }
