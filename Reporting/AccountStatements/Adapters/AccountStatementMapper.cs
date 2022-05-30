/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : AccountStatementMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map vouchers by account.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Methods used to map vouchers by account.</summary>
  static internal class AccountStatementMapper {

    #region Public mappers

    static internal AccountStatementDto Map(AccountStatement vouchers) {
      return new AccountStatementDto {
        Command = vouchers.Command,
        Columns = MapColumns(),
        Entries = MapToDto(vouchers.Entries),
        Title = vouchers.Title
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


    private static FixedList<IVouchersByAccountEntryDto> MapToDto(
                    FixedList<IVouchersByAccountEntry> list) {

      var mapped = list.Select((x) => MapToVouchersByAccount((Reporting.AccountStatementEntry) x));

      return new FixedList<IVouchersByAccountEntryDto>(mapped);
    }

    static private VouchersByAccountEntryDto MapToVouchersByAccount(
                                              Reporting.AccountStatementEntry entry) {

      var dto = new VouchersByAccountEntryDto();

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerName = entry.Ledger.Name;
      dto.LedgerNumber = entry.Ledger.Number;
      dto.CurrencyCode = entry.ItemType == TrialBalanceItemType.Entry ? entry.Currency.Code : "";
      dto.StandardAccountId = entry.StandardAccountId;
      dto.AccountName = entry.AccountName;
      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.AccountNumber = entry.AccountNumber;
      } else {
        dto.AccountNumber = entry.IsCurrentBalance ? "SALDO ACTUAL" : "SALDO INICIAL";
      }
      dto.AccountNumberForBalances = entry.AccountNumber;
      dto.SectorCode = entry.ItemType == TrialBalanceItemType.Entry ? entry.Sector.Code : "";
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber.Length > 1 ?
                                   entry.SubledgerAccountNumber : "";
      dto.VoucherNumber = entry.VoucherNumber;
      dto.ElaboratedBy = entry.ElaboratedBy.Name;

      dto.Concept = EmpiriaString.Clean(entry.Concept);

      if (entry.ItemType == TrialBalanceItemType.Entry || entry.IsCurrentBalance) {
        dto.Debit = entry.Debit;
        dto.Credit = entry.Credit;
      }
      dto.CurrentBalance = entry.CurrentBalance;
      dto.AccountingDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.AccountingDate : ExecutionServer.DateMaxValue;
      dto.RecordingDate = entry.ItemType == TrialBalanceItemType.Entry ?
                          entry.RecordingDate : ExecutionServer.DateMaxValue;
      dto.VoucherId = entry.VoucherId;

      return dto;
    }


    #endregion Private methods

  } // class AccountStatementMapper

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
