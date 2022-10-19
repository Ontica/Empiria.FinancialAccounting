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

      EmpiriaLog.Info($"Application preloading starts at {DateTime.Now}.");

      _alreadyExecuted = true;

      try {
        Subledger.Preload();
      } catch (Exception e) {
        EmpiriaLog.Error(e);
      }

      try {
        SubledgerAccount.Preload();
      } catch (Exception e) {
        EmpiriaLog.Error(e);
      }

      try {
        StandardAccount.Preload();
      } catch (Exception e) {
        EmpiriaLog.Error(e);
      }

      try {
        AccountsChart.Preload();
      } catch (Exception e) {
        EmpiriaLog.Error(e);
      }

      EmpiriaLog.Info($"Application preloading ends at {DateTime.Now}.");
    }

  }  // class Preloader

}  // namespace Empiria.FinancialAccounting
