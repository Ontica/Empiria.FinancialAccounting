/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Type Extension methods                  *
*  Type     : ListadoPolizasQueryExtensions              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds ListadoPolizasSqlClauses objects.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Builds ListadoPolizasSqlClauses objects.</summary>
  internal class ListadoPolizasSqlClausesBuilder {

    private readonly ListadoPolizasQuery _query;

    internal ListadoPolizasSqlClausesBuilder(ListadoPolizasQuery query) {
      this._query = query;
    }

    #region Methods

    internal ListadoPolizasSqlClauses Build() {
      var commandData = new ListadoPolizasSqlClauses();

      var accountsChart = AccountsChart.Parse(_query.AccountsChartUID);

      commandData.AccountsChart = accountsChart;
      commandData.FromDate = _query.FromDate;
      commandData.ToDate = _query.ToDate;
      commandData.Ledgers = GetLedgerFilter();

      return commandData;
    }


    private string GetLedgerFilter() {
      if (_query.Ledgers == null || _query.Ledgers.Length == 0) {
        return string.Empty;
      }

      int[] ledgerIds = _query.Ledgers.Select(uid => Ledger.Parse(uid).Id)
                                        .ToArray();

      return $" AND T.ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
    }

    #endregion Methods

  } // class ListadoPolizasSqlClausesBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
