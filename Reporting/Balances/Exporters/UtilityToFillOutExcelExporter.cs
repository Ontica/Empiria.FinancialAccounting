/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : UtilityToFillOutExcelExporter                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Utility methods to fill out table info for a Microsoft Excel file with balances information.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Utility methods to fill out table info for a 
  /// Microsoft Excel file with balances information.</summary>
  internal class UtilityToFillOutExcelExporter {

    private TrialBalanceQuery _query;

    private readonly DateTime MIN_LAST_CHANGE_DATE_TO_REPORT = DateTime.Parse("01/01/1970");

    public UtilityToFillOutExcelExporter(TrialBalanceQuery query) {
      _query = query;
    }


    #region Utility methods


    // TODO: CLEAN THIS CODE. ISSUE USING NEW CHART OF ACCOUNTS
    internal string GetLedgerLevelAccountNumber(string accountNumber) {
      var temp = string.Empty;

      if (accountNumber.Contains("-")) {
        temp = accountNumber.Substring(0, 4);
      } else if (accountNumber.Contains(".")) {
        temp = accountNumber.Substring(0, 1);
      }

      return temp;
    }


    // TODO: CLEAN THIS CODE. ISSUE USING NEW CHART OF ACCOUNTS
    internal string GetSubAccountNumberWithSector(string accountNumber, string sectorCode) {
      var temp = string.Empty;

      if (accountNumber.Contains("-")) {

        temp = accountNumber.Substring(4);

        temp = temp.Replace("-", String.Empty);

        temp = temp.PadRight(12, '0');

      } else if (accountNumber.Contains(".")) {

        temp = accountNumber.Substring(2);

        temp = temp.Replace(".", String.Empty);

        temp = temp.PadRight(20, '0');
      }

      return temp + sectorCode;
    }


    internal bool MustFillOutAverageBalance(decimal averageBalance, DateTime lastChangeDate) {
      if (!_query.WithAverageBalance) {
        return false;
      }
      if (averageBalance != 0) {
        return true;
      }
      if (lastChangeDate < MIN_LAST_CHANGE_DATE_TO_REPORT) {
        return false;
      }
      if (lastChangeDate == ExecutionServer.DateMaxValue) {
        return false;
      }
      return true;
    }

    #endregion Utility methods


  } // class UtilityToFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
