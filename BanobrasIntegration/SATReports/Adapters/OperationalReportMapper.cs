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

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  /// <summary>Maps trial balances entries and accounts chart to OperationalReportDto structures.</summary>
  static public class OperationalReportMapper {
    internal static OperationalReportDto MapFromTrialBalance(
                                          OperationalReportCommand command, TrialBalanceDto trialBalance) {
      return new OperationalReportDto {
        Command = command,
        Columns = trialBalance.Columns,
        Entries = MapBalanceEntry(trialBalance.Entries)
      };
    }


    internal static OperationalReportDto MapFromAccountsChart(
                                          OperationalReportCommand command, AccountsChartDto accountsChart) {

      return new OperationalReportDto {
        Command = command,
        Columns = MapColumns(),
        Entries = MapAccountsChart(accountsChart.Accounts)
      };

      throw new NotImplementedException();
    }

    private static FixedList<IOperationalReportEntryDto> MapAccountsChart(
                                      FixedList<AccountDescriptorDto> list) {

      var mappedItems = list.Select((x) => MapAccountsToOperationalReport((AccountDescriptorDto) x));

      return new FixedList<IOperationalReportEntryDto>(mappedItems);
      
    }

    static private OperationalReportEntryDto MapAccountsToOperationalReport(AccountDescriptorDto account) {

      return new OperationalReportEntryDto {
        
      };
      
    }

    private static FixedList<DataTableColumn> MapColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("codAgrup", "CodAgrup", "text"));
      columns.Add(new DataTableColumn("numCta", "NumCta", "text"));
      columns.Add(new DataTableColumn("desc", "Desc", "text"));
      columns.Add(new DataTableColumn("subCtaDe", "SubCtaDe", "text"));
      columns.Add(new DataTableColumn("nivel", "Nivel", "text"));
      columns.Add(new DataTableColumn("natur", "Natur", "text"));

      return columns.ToFixedList();
    }

    static private FixedList<IOperationalReportEntryDto> MapBalanceEntry(FixedList<ITrialBalanceEntryDto> list) {

      var mappedItems = list.Select((x) => MapBalanceToOperationalReport((TrialBalanceEntryDto) x));

      return new FixedList<IOperationalReportEntryDto>(mappedItems);

    }


    static private OperationalReportEntryDto MapBalanceToOperationalReport(TrialBalanceEntryDto entry) {

      return new OperationalReportEntryDto {
        CurrencyCode = entry.CurrencyCode,
        AccountNumber = entry.AccountNumber,
        SectorCode = entry.SectorCode,
        AccountName = entry.AccountName,
        InitialBalance = entry.InitialBalance,
        Debit = entry.Debit,
        Credit = entry.Credit,
        CurrentBalance = entry.CurrentBalance,
        AccountLevel = entry.AccountLevel,
      };

    }

  } // class OperationalReportMapper

} // Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
