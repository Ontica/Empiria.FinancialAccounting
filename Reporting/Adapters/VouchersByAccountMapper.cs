/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Mapper class                            *
*  Type     : VouchersByAccountMapper                              License   : Please read LICENSE.txt file  *
*                                                                                                            *
*  Summary  : Methods used to map vouchers by account.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Methods used to map vouchers by account.</summary>
  static internal class VouchersByAccountMapper {


    #region Public mappers


    static internal VouchersByAccountDto Map(VouchersByAccount vouchers) {
      return new VouchersByAccountDto { 
        Command = vouchers.Command,
        Columns = MapColumns(vouchers.Command),
        Entries = MapToDto(vouchers.Entries, vouchers.Command),
        Title = vouchers.Title
      };
    }


    #endregion Public mappers


    #region Private methods

    private static FixedList<DataTableColumn> MapColumns(BalanceCommand command) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("voucherNumber", "No. Poliza", "text-nowrap"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("debit", "Cargo", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abono", "decimal"));
      columns.Add(new DataTableColumn("accountingDate", "Afectación", "date"));
      columns.Add(new DataTableColumn("recordingDate", "Registro", "date"));

      return columns.ToFixedList();
    }


    private static FixedList<IVouchersByAccountEntryDto> MapToDto(
                    FixedList<IVouchersByAccountEntry> list, BalanceCommand command) {

      var mapped = list.Select((x) => MapToVouchersByAccount((VouchersByAccountEntry) x, command));
      return new FixedList<IVouchersByAccountEntryDto>(mapped);
    }

    static private VouchersByAccountEntryDto MapToVouchersByAccount(
                                              VouchersByAccountEntry entry, BalanceCommand command) {
      
      var dto = new VouchersByAccountEntryDto();

      dto.ItemType = entry.ItemType;
      dto.LedgerName = entry.Ledger.Name;
      dto.LedgerNumber = entry.Ledger.Number;
      dto.CurrencyCode = entry.Currency.Code;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountName = entry.Account.Name;
      dto.AccountNumber = entry.Account.Number;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.VoucherNumber = entry.VoucherNumber;
      //dto.ElaboratedBy = entry.ElaboratedBy.Name;
      dto.Concept = entry.Concept;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = 0;
      dto.AccountingDate = entry.AccountingDate;
      dto.RecordingDate = entry.RecordingDate;
      dto.VoucherId = entry.VoucherId;

      return dto;
    }


    #endregion Private methods

  } // class VouchersByAccountMapper

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
