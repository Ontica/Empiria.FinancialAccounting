/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Root Types                              *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Application data preloader              *
*  Type     : Preloader                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Application data preloader for the financial accounting system.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Application data preloader for the financial accounting system.</summary>
  static public class Preloader {

    #region Public methods

    static public void Preload() {
      try {
        EmpiriaLog.Info($"Application preloading starts at {DateTime.Now}.");
        StandardAccount.Preload();
        AccountsChart.Preload();
        SubsidiaryLedger.Preload();
        SubsidiaryAccount.Preload();
        EmpiriaLog.Info($"Application preloading ends at {DateTime.Now}.");
      } catch (Exception e) {
        EmpiriaLog.Error(e);
      }
    }

    #endregion Public methods

  }  // class Preloader

}  // namespace Empiria.FinancialAccounting
