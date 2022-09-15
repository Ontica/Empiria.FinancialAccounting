/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : AccountStatementMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for account statements (estados de cuenta).                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters {

  /// <summary>Mapping methods for account statements (estados de cuenta).</summary>
  static internal class AccountStatementMapper {

    #region Public mappers

    static internal AccountStatementDto Map(AccountStatement accountStatement) {
      return new AccountStatementDto {
        Query = accountStatement.Query,
        Columns = MapColumns(),
        Entries = MapToDto(accountStatement.Entries),
        Title = accountStatement.Title
      };
    }

    #endregion Public mappers

    #region Private methods

    private static FixedList<DataTableColumn> MapColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("voucherNumber", "No. Poliza", "text-nowrap"));
      columns.Add(new DataTableColumn("debit", "Cargo", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abono", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("accountingDate", "Afectación", "date"));
      columns.Add(new DataTableColumn("recordingDate", "Registro", "date"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text-nowrap"));
      columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text-nowrap"));

      return columns.ToFixedList();
    }


    private static FixedList<IVouchersByAccountEntryDto> MapToDto (
                    FixedList<IVouchersByAccountEntry> list) {

      var mapped = list.Select((x) => MapToVouchersByAccount((AccountStatementEntry) x));

      return new FixedList<IVouchersByAccountEntryDto>(mapped);
    }

    static private VouchersByAccountEntryDto MapToVouchersByAccount(
                                              AccountStatementEntry entry) {

      var dto = new VouchersByAccountEntryDto();

      ItemTypeClausesForDto(dto, entry);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerName = entry.Ledger.Name;
      dto.LedgerNumber = entry.Ledger.Number;
      dto.StandardAccountId = entry.StandardAccountId;
      dto.AccountName = entry.AccountName;
      dto.AccountNumberForBalances = entry.AccountNumber;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber.Length > 1 ?
                                   entry.SubledgerAccountNumber : "";
      dto.VoucherNumber = entry.VoucherNumber;
      dto.ElaboratedBy = entry.ElaboratedBy.Name;
      dto.Concept = EmpiriaString.Clean(entry.Concept);
      dto.CurrentBalance = entry.CurrentBalance;
      dto.VoucherId = entry.VoucherId;

      return dto;
    }

    private static void ItemTypeClausesForDto(VouchersByAccountEntryDto dto,
                                             AccountStatementEntry entry) {

      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.AccountNumber = entry.AccountNumber;
        dto.AccountingDate = entry.AccountingDate;
        dto.RecordingDate = entry.RecordingDate;
        dto.CurrencyCode = entry.Currency.Code;
        dto.SectorCode = entry.Sector.Code;
      } else {
        dto.AccountNumber = entry.IsCurrentBalance ? "SALDO ACTUAL" : "SALDO INICIAL";
        dto.AccountingDate = ExecutionServer.DateMaxValue;
        dto.RecordingDate = ExecutionServer.DateMaxValue;
      }

      if (entry.ItemType == TrialBalanceItemType.Entry || entry.IsCurrentBalance) {
        dto.Debit = entry.Debit;
        dto.Credit = entry.Credit;
      }

    }


    #endregion Private methods

  } // class AccountStatementMapper

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters
