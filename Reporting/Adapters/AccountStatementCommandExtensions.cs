/* Empiria Financial *****************************************************************************************
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
  internal class AccountStatementCommandExtensions {

    private AccountStatementCommand _accountStatementCommand;
    internal AccountStatementCommandExtensions(AccountStatementCommand accountStatementCommand) {
      _accountStatementCommand = accountStatementCommand;
    }

    #region Public methods

    internal AccountStatementCommandData MapToVouchersByAccountCommandData() {
      var commandData = new AccountStatementCommandData();
      var accountsChart = AccountsChart.Parse(_accountStatementCommand.Command.AccountsChartUID);


      //commandData.Fields = GetFields();
      commandData.AccountsChartId = accountsChart.Id;
      commandData.FromDate = _accountStatementCommand.Command.InitialPeriod.FromDate;
      commandData.ToDate = _accountStatementCommand.Command.InitialPeriod.ToDate;
      commandData.Filters = GetFilters();
      //commandData.Grouping = GetGroupingClause();
      return commandData;
    }

    private string GetGroupingClause() {
      return string.Empty;
    }

    #endregion


    #region Private methods

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


    private string GetAccountFilter() {
      if (_accountStatementCommand.Entry.AccountNumberForBalances.Length != 0 &&
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


    private string GetLedgerFilter() {
      if (_accountStatementCommand.Entry.LedgerUID.Length > 0) {
        
        int ledgerId = Ledger.Parse(_accountStatementCommand.Entry.LedgerUID).Id;
        return $"ID_MAYOR = { ledgerId }";
      
      } else if (_accountStatementCommand.Command.Ledgers.Length > 0) {
        
        int[] ledgerIds = _accountStatementCommand.Command.Ledgers
                        .Select(uid => Ledger.Parse(uid).Id).ToArray();

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }
      return string.Empty;
    }


    static private string GetFields() {
      return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, " +
             "ID_TRANSACCION, SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
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
      if (_accountStatementCommand.Command.TrialBalanceType == 
            TrialBalanceType.SaldosPorAuxiliarConsultaRapida) {
        
        if (_accountStatementCommand.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.Entry.SubledgerAccountNumber}'";
        }

      } else {

        if (_accountStatementCommand.Entry.SubledgerAccountNumber.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.Entry.SubledgerAccountNumber}'";
        } else if (_accountStatementCommand.Command.SubledgerAccount.Length > 1) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_accountStatementCommand.Command.SubledgerAccount}'";
        }

      }
      
      return string.Empty;
    }


    #endregion



  } // class VouchersByAccountCommandExtensions

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
