﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Type Extension methods                  *
*  Type     : VouchersByAccountCommandExtensions         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type extension methods for VouchersByAccountCommand.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Data;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Type extension methods for VouchersByAccountCommand.</summary>
  internal class AccountStatementQueryExtensions {

    private readonly AccountStatementQuery _accountStatementCommand;

    internal AccountStatementQueryExtensions(AccountStatementQuery accountStatementCommand) {
      _accountStatementCommand = accountStatementCommand;
    }

    #region Methods

    internal AccountStatementCommandData MapToVouchersByAccountCommandData() {
      var commandData = new AccountStatementCommandData();

      var accountsChart = AccountsChart.Parse(_accountStatementCommand.BalancesQuery.AccountsChartUID);

      commandData.AccountsChartId = accountsChart.Id;
      commandData.FromDate = _accountStatementCommand.BalancesQuery.InitialPeriod.FromDate;
      commandData.ToDate = _accountStatementCommand.BalancesQuery.InitialPeriod.ToDate;
      commandData.Filters = GetFilters();
      commandData.Fields = GetFields();
      commandData.Grouping = GetGroupingClause();

      return commandData;
    }

    #endregion Methods

    #region Helpers

    private string GetAccountFilter() {
      if (_accountStatementCommand.Entry.AccountNumberForBalances.Length > 0 &&
          _accountStatementCommand.Entry.AccountNumberForBalances != "Empty") {
        if (_accountStatementCommand.Entry.ItemType == TrialBalanceItemType.Summary) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_accountStatementCommand.Entry.AccountNumberForBalances}%'";
        } else {
          return $"NUMERO_CUENTA_ESTANDAR = '{_accountStatementCommand.Entry.AccountNumberForBalances}'";
        }

      } else {
        return string.Empty;
      }
    }


    private string GetCurrencyFilter() {
      if (_accountStatementCommand.Entry.CurrencyCode.Length == 0) {
        return string.Empty;
      }

      int currencyId = Currency.Parse(_accountStatementCommand.Entry.CurrencyCode).Id;
      return $"ID_MONEDA IN ({String.Join(", ", currencyId)})";
    }


    static private string GetFields() {
      return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
             "-1 AS ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
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
             "ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, NUMERO_CUENTA_AUXILIAR, " +
             "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, CONCEPTO_TRANSACCION";
    }


    private string GetLedgerFilter() {
      if (_accountStatementCommand.Entry.LedgerUID.Length > 0) {

        int ledgerId = Ledger.Parse(_accountStatementCommand.Entry.LedgerUID).Id;
        return $"ID_MAYOR = { ledgerId }";

      } else if (_accountStatementCommand.BalancesQuery.Ledgers.Length > 0) {

        int[] ledgerIds = _accountStatementCommand.BalancesQuery.Ledgers
                        .Select(uid => Ledger.Parse(uid).Id).ToArray();

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }
      return string.Empty;
    }


    private string GetSectorFilter() {
      if (_accountStatementCommand.Entry.SectorCode.Length == 0 ||
          ((_accountStatementCommand.Entry.ItemType == TrialBalanceItemType.Summary ||
            _accountStatementCommand.Entry.ItemType == TrialBalanceItemType.Total) &&
           _accountStatementCommand.Entry.SectorCode == "00")) {
        return string.Empty;
      }

      int sectorId = Sector.Parse(_accountStatementCommand.Entry.SectorCode).Id;
      if (sectorId == 0 || sectorId == -1) {
        return $"ID_SECTOR IN (-1, 0)";
      }
      return $"ID_SECTOR = '{sectorId}'";
    }


    private string GetSubledgerAccountFilter() {
      if (_accountStatementCommand.BalancesQuery.TrialBalanceType ==
            TrialBalanceType.SaldosPorAuxiliarConsultaRapida) {

        if (_accountStatementCommand.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.Entry.SubledgerAccountNumber}'";
        }

      } else {

        if (_accountStatementCommand.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.Entry.SubledgerAccountNumber}'";
        } else if (_accountStatementCommand.BalancesQuery.SubledgerAccount.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.BalancesQuery.SubledgerAccount}'";
        }

      }

      return string.Empty;
    }

    #endregion Helpers

  } // class VouchersByAccountCommandExtensions

} // namespace Empiria.FinancialAccounting.Reporting.Adapters