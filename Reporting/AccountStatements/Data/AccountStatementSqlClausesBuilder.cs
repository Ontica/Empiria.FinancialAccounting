/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Type Extension methods                  *
*  Type     : AccountStatementSqlClausesBuilder          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type extension methods for AccountStatementQueryExtensions.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Type extension methods for AccountStatementQueryExtensions.</summary>
  internal class AccountStatementSqlClausesBuilder {

    private readonly AccountStatementQuery _accountStatementQry;

    internal AccountStatementSqlClausesBuilder(AccountStatementQuery accountStatementQuery) {
      _accountStatementQry = accountStatementQuery;
    }

    #region Methods

    internal AccountStatementSqlClauses BuildSqlClauses() {
      var commandData = new AccountStatementSqlClauses();

      var accountsChart = AccountsChart.Parse(_accountStatementQry.BalancesQuery.AccountsChartUID);

      commandData.AccountsChartId = accountsChart.Id;
      commandData.FromDate = _accountStatementQry.BalancesQuery.InitialPeriod.FromDate;
      commandData.ToDate = _accountStatementQry.BalancesQuery.InitialPeriod.ToDate;
      commandData.Filters = GetFilters();
      commandData.Fields = GetFields();
      commandData.Grouping = GetGroupingClause();

      return commandData;
    }

    #endregion Methods

    #region Helpers

    private string GetAccountFilter() {
      if (_accountStatementQry.Entry.AccountNumberForBalances.Length > 0 &&
          _accountStatementQry.Entry.AccountNumberForBalances != "Empty") {
        if (_accountStatementQry.Entry.ItemType == TrialBalanceItemType.Summary) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_accountStatementQry.Entry.AccountNumberForBalances}%'";
        } else {
          return $"NUMERO_CUENTA_ESTANDAR = '{_accountStatementQry.Entry.AccountNumberForBalances}'";
        }

      } else {
        return string.Empty;
      }
    }


    private string GetCurrencyFilter() {
      if (_accountStatementQry.Entry.CurrencyCode.Length == 0) {
        return string.Empty;
      }

      int currencyId = Currency.Parse(_accountStatementQry.Entry.CurrencyCode).Id;

      return $"ID_MONEDA IN ({String.Join(", ", currencyId)})";
    }


    static private string GetFields() {
      return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
             "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
             "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
             "CONCEPTO_TRANSACCION, SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
    }


    private string GetFilters() {
      string accountRangeFilter = GetAccountFilter();
      string subledgerAccountFilter = GetSubledgerAccountFilter();
      string currencyFilter = GetCurrencyFilter();
      string ledgerFilter = GetLedgerFilter();
      string sectorFilter = GetSectorFilter();

      var filter = new Filter(accountRangeFilter);

      filter.AppendAnd(subledgerAccountFilter);
      filter.AppendAnd(currencyFilter);
      filter.AppendAnd(ledgerFilter);
      filter.AppendAnd(sectorFilter);

      return filter.ToString().Length > 0 ? $"AND {filter}" : "";
    }

    private string GetGroupingClause() {
      return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
             "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
             "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
             "CONCEPTO_TRANSACCION";
    }


    private string GetLedgerFilter() {
      if (_accountStatementQry.Entry.LedgerUID.Length > 0) {

        int ledgerId = Ledger.Parse(_accountStatementQry.Entry.LedgerUID).Id;
        return $"ID_MAYOR = { ledgerId }";

      } else if (_accountStatementQry.BalancesQuery.Ledgers.Length > 0) {

        int[] ledgerIds = _accountStatementQry.BalancesQuery.Ledgers
                        .Select(uid => Ledger.Parse(uid).Id).ToArray();

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }
      return string.Empty;
    }


    private string GetSectorFilter() {
      if (_accountStatementQry.Entry.SectorCode.Length == 0 ||
          ((_accountStatementQry.Entry.ItemType == TrialBalanceItemType.Summary ||
            _accountStatementQry.Entry.ItemType == TrialBalanceItemType.Total) &&
           _accountStatementQry.Entry.SectorCode == "00")) {
        return string.Empty;
      }

      int sectorId = Sector.Parse(_accountStatementQry.Entry.SectorCode).Id;
      if (sectorId == 0 || sectorId == -1) {
        return $"ID_SECTOR IN (-1, 0)";
      }
      return $"ID_SECTOR = '{sectorId}'";
    }


    private string GetSubledgerAccountFilter() {
      if (_accountStatementQry.BalancesQuery.TrialBalanceType ==
            TrialBalanceType.SaldosPorAuxiliarConsultaRapida ||
            _accountStatementQry.BalancesQuery.TrialBalanceType ==
            TrialBalanceType.SaldosPorAuxiliarID) {

        if (_accountStatementQry.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementQry.Entry.SubledgerAccountNumber}'";
        }

      } else {

        if (_accountStatementQry.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementQry.Entry.SubledgerAccountNumber}'";
        } else if (_accountStatementQry.BalancesQuery.SubledgerAccount.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementQry.BalancesQuery.SubledgerAccount}'";
        }

      }

      return string.Empty;
    }

    #endregion Helpers

  } // class AccountStatementSqlClausesBuilder

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements
