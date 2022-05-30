/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Type Extension methods                  *
*  Type     : ListadoPolizasQueryExtensions              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type extension methods for ListadoPolizasQuery.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Type extension methods for ListadoPolizasQuery.</summary>
  internal class ListadoPolizasQueryExtensions {

    #region Public methods

    internal PolizaCommandData MapToPolizaCommandData(ListadoPolizasQuery query) {

      var clauses = new ListadoPolizasClausesHelper(query);

      return clauses.GetPolizaCommandData();
    }


    #endregion


    private class ListadoPolizasClausesHelper {

      private readonly ListadoPolizasQuery _query;

      internal ListadoPolizasClausesHelper(ListadoPolizasQuery query) {
        this._query = query;
      }


      #region Public methods

      internal PolizaCommandData GetPolizaCommandData() {
        var commandData = new PolizaCommandData();

        var accountsChart = AccountsChart.Parse(_query.AccountsChartUID);

        commandData.AccountsChart = accountsChart;
        commandData.FromDate = _query.FromDate;
        commandData.ToDate = _query.ToDate;
        commandData.Ledgers = GetFilters();

        return commandData;
      }

      private string GetFilters() {

        return GetLedgerFilter();

      }


      private string GetLedgerFilter() {
        if (_query.Ledgers == null || _query.Ledgers.Length == 0) {
          return string.Empty;
        }

        int[] ledgerIds = _query.Ledgers.Select(uid => Ledger.Parse(uid).Id)
                                          .ToArray();

        return $" AND T.ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }

      #endregion

    } // class ListadoPolizasClausesHelper


  } // class ListadoPolizasQueryExtensions

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
