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
      EmpiriaLog.Info($"Application preloading starts at {DateTime.Now}.");
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

    #endregion Public methods

  }  // class Preloader

}  // namespace Empiria.FinancialAccounting
