/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Query payload                           *
*  Type     : AccountStatementQueryExtensions            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build account statements.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.Data;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters {


  static internal class AccountStatementQueryExtensions {

    #region Public methods


    static internal string MapToFilterString(this AccountStatementQuery query) {

      string accountChart = GetAccountsChartFilter(query);
      string dateFilter = GetDateFilter(query);
      string accountRangeFilter = GetAccountFilter(query);
      string subledgerAccountFilter = GetSubledgerAccountFilter(query);
      string currencyFilter = GetCurrencyFilter(query);
      string ledgerFilter = GetLedgerFilter(query);
      string sectorFilter = GetSectorFilter(query);

      var filter = new Filter(accountChart);

      filter.AppendAnd(dateFilter);
      filter.AppendAnd(accountRangeFilter);
      filter.AppendAnd(subledgerAccountFilter);
      filter.AppendAnd(currencyFilter);
      filter.AppendAnd(ledgerFilter);
      filter.AppendAnd(sectorFilter);

      return filter.ToString().Length > 0 ? $"{filter}" : "";
    }


    static internal string MapToSortString(this AccountStatementQuery query) {

      return string.Empty;
    }


    #endregion Public methods

    #region Helpers


    static private string GetAccountFilter(AccountStatementQuery query) {
      if (query.Entry.AccountNumberForBalances.Length > 0 &&
          query.Entry.AccountNumberForBalances != "Empty") {

        if (query.Entry.ItemType == TrialBalanceItemType.Summary) {

          return $"NUMERO_CUENTA_ESTANDAR LIKE '{query.Entry.AccountNumberForBalances}%'";
        } else {

          return $"NUMERO_CUENTA_ESTANDAR = '{query.Entry.AccountNumberForBalances}'";
        }
      }
      return string.Empty;
    }


    static private string GetAccountsChartFilter(AccountStatementQuery query) {

      if (query.BalancesQuery.AccountsChartUID != string.Empty) {

        return $"ID_TIPO_CUENTAS_STD = {AccountsChart.Parse(query.BalancesQuery.AccountsChartUID).Id}";
      }
      return string.Empty;
    }


    static private string GetCurrencyFilter(AccountStatementQuery query) {
      if (query.Entry.CurrencyCode.Length == 0) {
        return string.Empty;
      }

      int currencyId = Currency.Parse(query.Entry.CurrencyCode).Id;
      return $"ID_MONEDA IN ({String.Join(", ", currencyId)})";
    }


    static private string GetDateFilter(AccountStatementQuery query) {

      return $"FECHA_AFECTACION >= " +
             $"{DataCommonMethods.FormatSqlDbDate(query.BalancesQuery.InitialPeriod.FromDate)} " +
             $"AND FECHA_AFECTACION <= " +
             $"{DataCommonMethods.FormatSqlDbDate(query.BalancesQuery.InitialPeriod.ToDate)}";
    }


    static private string GetLedgerFilter(AccountStatementQuery query) {
      if (query.Entry.LedgerUID.Length > 0) {

        int ledgerId = Ledger.Parse(query.Entry.LedgerUID).Id;
        return $"ID_MAYOR = {ledgerId}";

      } else if (query.BalancesQuery.Ledgers.Length > 0) {

        int[] ledgerIds = query.BalancesQuery.Ledgers.Select(uid => Ledger.Parse(uid).Id).ToArray();
        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }
      return string.Empty;
    }


    static private string GetSectorFilter(AccountStatementQuery query) {

      if (query.Entry.SectorCode.Length == 0 ||
          ((query.Entry.ItemType == TrialBalanceItemType.Summary ||
            query.Entry.ItemType == TrialBalanceItemType.Total) &&
           query.Entry.SectorCode == "00")) {
        return string.Empty;
      }

      int sectorId = Sector.Parse(query.Entry.SectorCode).Id;
      if (sectorId == 0 || sectorId == -1) {
        return $"ID_SECTOR IN (-1, 0)";
      }
      return $"ID_SECTOR = '{sectorId}'";
    }

    static private string GetSubledgerAccountFilter(AccountStatementQuery query) {

      if (query.BalancesQuery.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarConsultaRapida ||
          query.BalancesQuery.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarID) {

        if (query.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{query.Entry.SubledgerAccountNumber}'";
        }
      } else {
        if (query.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{query.Entry.SubledgerAccountNumber}'";
        } else if (query.BalancesQuery.SubledgerAccount.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{query.BalancesQuery.SubledgerAccount}'";
        }
      }
      return string.Empty;
    }

    #endregion Helpers


  } // class AccountStatementQueryExtensions

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters
