/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ListadoMovimientosPorPoliza                   License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Listado de movimientos por póliza para exportar datos a excel.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;
using System.Linq;

using Empiria.DynamicData;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

  /// <summary>Listado de movimientos por póliza para exportar datos a excel.</summary>
  internal class ListadoMovimientosPorPoliza : IReportBuilder {

    private ReportBuilderQuery _query;

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));
      _query = buildQuery;

      FixedList<IVouchersByAccountEntry> vouchersByAccount = BuildMovementsByVoucher();

      return MapToReportDataDto(buildQuery, vouchersByAccount);
    }


    #endregion Public methods

    #region Private methods

    private FixedList<IVouchersByAccountEntry> BuildMovementsByVoucher() {

      var helper = new ListadoPolizasPorCuentaHelper(_query);

      FixedList<AccountStatementEntry> voucherEntriesByGroups = GetVoucherEntriesByGroups();

      FixedList<AccountStatementEntry> voucherWithSummaryEntries =
                                        helper.GetSummaryToParentVouchers(voucherEntriesByGroups);

      FixedList<AccountStatementEntry> orderingVouchers = helper.OrderingVouchers(voucherWithSummaryEntries);

      return orderingVouchers.Select(x => (IVouchersByAccountEntry) x).ToFixedList();
    }


    private static FixedList<DataTableColumn> GetReportColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("ledgerName", "Contabilidad", "text"));
      columns.Add(new DataTableColumn("voucherId", "Id póliza", "text"));
      columns.Add(new DataTableColumn("voucherNumber", "No. Poliza", "text-nowrap"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre cuenta", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("SubledgerAccountName", "Nombre auxiliar", "text"));
      columns.Add(new DataTableColumn("verificationNumber", "No. Verif", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("debit", "Cargo", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abono", "decimal"));
      columns.Add(new DataTableColumn("accountingDate", "Afectación", "date"));
      columns.Add(new DataTableColumn("recordingDate", "Registro", "date"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text-nowrap"));
      columns.Add(new DataTableColumn("authorizedBy", "Autorizado por", "text-nowrap"));
      columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text-nowrap"));

      return columns.ToFixedList();
    }


    private FixedList<AccountStatementEntry> GetVoucherEntriesByGroups() {

      var helper = new ListadoPolizasPorCuentaHelper(_query);

      List<AccountStatementEntry> returnedVouchers = new List<AccountStatementEntry>();

      int counter = 0;
      int offset = 500;

      while (true) {
        var vouchers = _query.VoucherIds.Skip(counter).Take(offset);

        if (vouchers.Count() == 0) {
          break;
        }

        _query.VouchersIdGroup = vouchers.ToArray();
        returnedVouchers.AddRange(helper.GetVoucherEntries().ToList());

        counter += vouchers.Count();
      }
      return returnedVouchers.ToFixedList();
    }


    static private ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                    FixedList<IVouchersByAccountEntry> vouchers) {
      return new ReportDataDto {
        Query = buildQuery,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(vouchers)
      };
    }


    private static FixedList<IReportEntryDto> MapToReportDataEntries(
                                                FixedList<IVouchersByAccountEntry> entries) {

      var mappedItems = entries.Select((x) => MapToMovementEntry((AccountStatementEntry) x));
      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private VoucherByAccountEntry MapToMovementEntry(AccountStatementEntry entry) {
      var voucherMovement = new VoucherByAccountEntry();

      ClausesByItemType(voucherMovement, entry);

      voucherMovement.LedgerUID = entry.Ledger.UID;
      voucherMovement.CurrencyCode = entry.Currency.Code;
      voucherMovement.AccountName = entry.AccountName;

      if (entry.SubledgerAccountNumber != "0" && entry.SubledgerAccountNumber != null) {
        voucherMovement.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      }

      voucherMovement.Debit = entry.Debit;
      voucherMovement.Credit = entry.Credit;
      voucherMovement.ElaboratedBy = entry.ElaboratedBy.Name;
      voucherMovement.AuthorizedBy = entry.AuthorizedBy.Name;
      voucherMovement.ItemType = entry.ItemType;

      return voucherMovement;
    }


    private static void ClausesByItemType(VoucherByAccountEntry voucherMovement,
                                          AccountStatementEntry entry) {

      if (entry.ItemType == TrialBalanceItemType.Entry) {
        voucherMovement.LedgerNumber = entry.Ledger.Number;
        voucherMovement.LedgerName = entry.Ledger.Name;
        voucherMovement.AccountNumber = entry.AccountNumber;
        voucherMovement.SectorCode = entry.Sector.Code;
        voucherMovement.VoucherId = entry.VoucherId;
        voucherMovement.VoucherNumber = entry.VoucherNumber;
        voucherMovement.Concept = entry.Concept;
        voucherMovement.AccountingDate = entry.AccountingDate;
        voucherMovement.RecordingDate = entry.RecordingDate;
        voucherMovement.SubledgerAccountName = entry.SubledgerAccountName;
        voucherMovement.VerificationNumber = entry.VerificationNumber;

      } else {
        voucherMovement.AccountNumber = entry.AccountName;
        voucherMovement.AccountingDate = ExecutionServer.DateMaxValue;
        voucherMovement.RecordingDate = ExecutionServer.DateMaxValue;
      }
    }

    #endregion Private methods

  } // ListadoMovimientosPorPoliza

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
