/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Operational report                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Mapper class                          *
*  Type     : OperationalReportMapper                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Maps trial balances entries and accounts chart to OperationalReportDto structures.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports {

  /// <summary>Maps trial balances entries and accounts chart to OperationalReportDto structures.</summary>
  static public class OperationalReportMapper {

    internal static OperationalReportDto MapFromTrialBalance(
                                          OperationalReportCommand command, TrialBalanceDto trialBalance) {
      return new OperationalReportDto {
        Command = command,
        Columns = MapBalanceColumns(),
        Entries = MapBalanceEntry(trialBalance.Entries)
      };
    }


    internal static OperationalReportDto MapFromAccountsChart(
                                         OperationalReportCommand command,
                                         FixedList<AccountDescriptorDto> accounts) {
      return new OperationalReportDto {
        Command = command,
        Columns = MapColumns(),
        Entries = MapAccountsChart(accounts)
      };
    }

    #region Helpers

    static private FixedList<IOperationalReportEntryDto> MapBalanceEntry(FixedList<ITrialBalanceEntryDto> list) {

      var mappedItems = list.Select((x) => MapBalanceToOperationalReport((TrialBalanceEntryDto) x));

      return new FixedList<IOperationalReportEntryDto>(mappedItems);

    }


    static private OperationalReportEntryDto MapBalanceToOperationalReport(TrialBalanceEntryDto entry) {

      return new OperationalReportEntryDto {
        AccountNumber = entry.AccountNumber,
        InitialBalance = entry.InitialBalance,
        Debit = entry.Debit,
        Credit = entry.Credit,
        CurrentBalance = entry.CurrentBalance
      };
    }


    static private FixedList<IOperationalReportEntryDto> MapAccountsChart(
                                      FixedList<AccountDescriptorDto> list) {

      var mappedItems = list.Select((x) => MapAccountsToOperationalReport((AccountDescriptorDto) x));

      return new FixedList<IOperationalReportEntryDto>(mappedItems);

    }

    static private OperationalReportEntryDto MapAccountsToOperationalReport(AccountDescriptorDto account) {
      string accountParent = GetParentAccountNumber(account.Number);

      return new OperationalReportEntryDto {
        GroupingCode = "000",
        AccountNumber = account.Number,
        AccountName = account.Name,
        AccountParent = accountParent,
        AccountLevel = account.Level,
        Naturaleza = account.DebtorCreditor == DebtorCreditorType.Deudora ?
                     "D" : "A"
      };

    }

    static private string GetParentAccountNumber(string accountNumber) {
      var parentAccountNumber = accountNumber.Contains("-") ? 
                                accountNumber.Substring(0, accountNumber.LastIndexOf('-')) : 
                                accountNumber;
      if (parentAccountNumber.EndsWith("-00")) {
        parentAccountNumber = parentAccountNumber.Replace("-00", "");
      }
      return parentAccountNumber;
    }

    static private FixedList<DataTableColumn> MapBalanceColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("initialBalance", "Saldo Inicial", "decimal"));
      columns.Add(new DataTableColumn("debit", "Debe", "decimal"));
      columns.Add(new DataTableColumn("credit", "Haber", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo Final", "decimal"));

      return columns.ToFixedList();
    }

    static private FixedList<DataTableColumn> MapColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("groupingCode", "CodAgrup", "text"));
      columns.Add(new DataTableColumn("accountNumber", "NumCta", "text"));
      columns.Add(new DataTableColumn("accountName", "Desc", "text"));
      columns.Add(new DataTableColumn("accountParent", "SubCtaDe", "text"));
      columns.Add(new DataTableColumn("accountLevel", "Nivel", "text"));
      columns.Add(new DataTableColumn("naturaleza", "Natur", "text"));

      return columns.ToFixedList();
    }

    #endregion


  } // class OperationalReportMapper

} // Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports
