/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : MovimientosNumeroVerificacion                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Listado de polizas por número de verificación para reporte operativo.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.DynamicData;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

  /// <summary>Listado de polizas por número de verificación para reporte operativo.</summary>
  internal class MovimientosNumeroVerificacion : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      FixedList<IVouchersByAccountEntry> vouchersByAccount = BuildVouchersByAccount(buildQuery);

      return MapToReportDataDto(buildQuery, vouchersByAccount);
    }

    #endregion Public methods

    #region Private methods

    private FixedList<IVouchersByAccountEntry> BuildVouchersByAccount(ReportBuilderQuery buildQuery) {

      var helper = new ListadoPolizasPorCuentaHelper(buildQuery);

      FixedList<AccountStatementEntry> vouchersList = helper.GetVoucherEntries();

      vouchersList = helper.GetSummaryToParentVouchers(vouchersList);

      FixedList<AccountStatementEntry> orderingVouchers = helper.OrderingVouchers(vouchersList);

      return orderingVouchers.Select(x => (IVouchersByAccountEntry) x)
                            .ToFixedList();
    }


    private static FixedList<DataTableColumn> GetReportColumns(ReportBuilderQuery buildQuery) {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("verificationNumber", "No. Verif", "text"));
      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("voucherNumber", "No. Poliza", "text-nowrap"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("debit", "Cargo", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abono", "decimal"));
      columns.Add(new DataTableColumn("accountingDate", "Afectación", "date"));
      columns.Add(new DataTableColumn("recordingDate", "Registro", "date"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text-nowrap"));
      columns.Add(new DataTableColumn("authorizedBy", "Autorizado por", "text-nowrap"));
      columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text-nowrap"));

      return columns.ToFixedList();
    }


    static private ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                    FixedList<IVouchersByAccountEntry> vouchers) {
      return new ReportDataDto {
        Query = buildQuery,
        Columns = GetReportColumns(buildQuery),
        Entries = MapToReportDataEntries(vouchers)
      };
    }


    private static FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<IVouchersByAccountEntry> entries) {
      var mappedItems = entries.Select((x) => MapToVoucherEntry((AccountStatementEntry) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private VoucherByAccountEntry MapToVoucherEntry(AccountStatementEntry entry) {
      var returnedVoucher = new VoucherByAccountEntry();

      returnedVoucher.VerificationNumber = entry.VerificationNumber;
      returnedVoucher.LedgerUID = entry.Ledger.UID;
      returnedVoucher.CurrencyCode = entry.Currency.Code;
      returnedVoucher.AccountName = entry.AccountName;
      
      returnedVoucher.LedgerNumber = entry.Ledger.Number;
      returnedVoucher.LedgerName = entry.Ledger.Name;
      returnedVoucher.AccountNumber = entry.AccountNumber;
      returnedVoucher.SectorCode = entry.Sector.Code;
      returnedVoucher.VoucherNumber = entry.VoucherNumber;
      returnedVoucher.Concept = entry.Concept;
      returnedVoucher.AccountingDate = entry.AccountingDate;
      returnedVoucher.RecordingDate = entry.RecordingDate;

      if (entry.SubledgerAccountNumber != "0" && entry.SubledgerAccountNumber != null) {
        returnedVoucher.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      }

      returnedVoucher.Debit = entry.Debit;
      returnedVoucher.Credit = entry.Credit;
      returnedVoucher.ElaboratedBy = entry.ElaboratedBy.Name;
      returnedVoucher.AuthorizedBy = entry.AuthorizedBy.Name;
      returnedVoucher.ItemType = entry.ItemType;

      return returnedVoucher;
    }

    #endregion Private methods

  } // class MovimientosNumeroVerificacion

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
