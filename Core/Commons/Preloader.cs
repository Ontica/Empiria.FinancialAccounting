/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Application data preloader              *
*  Type     : Preloader                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Application data preloader for the financial accounting system.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

namespace Empiria.FinancialAccounting {

  /// <summary>Application data preloader for the financial accounting system.</summary>
  static public class Preloader {

    static private bool _alreadyExecuted = false;

    static public void Preload() {
      if (_alreadyExecuted) {
        return;
      }

      var task = new Task(() => {
        DoPreload();
      });

      task.Start();
    }


    static private void DoPreload() {

      _alreadyExecuted = true;

      try {
        EmpiriaLog.Info($"Application preloading starts at {DateTime.Now}.");
        Subledger.Preload();
        SubledgerAccount.Preload();
        AccountsChart.Preload();
        EmpiriaLog.Info($"Application preloading ends at {DateTime.Now}.");
      } catch {
        //  no-op
      }
    }

  }  // class Preloader

}  // namespace Empiria.FinancialAccounting
