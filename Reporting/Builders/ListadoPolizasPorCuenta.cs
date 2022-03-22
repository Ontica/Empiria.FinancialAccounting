/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ListadoPolizasPorCuenta                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Listado de polizas por cuenta para reporte operativo.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  ///<summary>Listado de polizas por cuenta para reporte operativo.</summary>
  internal class ListadoPolizasPorCuenta : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      ListadoPolizasPorCuentaBuilder vouchers = BuildVouchersByAccount(command);

      return MapToReportDataDto(command, vouchers);
    }


    #endregion

    #region Private methods

    private ListadoPolizasPorCuentaBuilder BuildVouchersByAccount(BuildReportCommand command) {

      var helper = new ListadoPolizasPorCuentaHelper(command);

      FixedList<AccountStatementEntry> vouchersList = helper.GetVoucherEntries();

      //TODO generar cuentas sumarias
      //FixedList<AccountStatementEntry> vouchersList = helper.GenerateSummaryEntries(entries);

      var returnedVouchers = new FixedList<IVouchersByAccountEntry>(vouchersList.Select(x => (IVouchersByAccountEntry) x));

      ListadoPolizasPorCuentaBuilder vouchers = new ListadoPolizasPorCuentaBuilder(command, returnedVouchers);

      return vouchers;
    }


    private static FixedList<DataTableColumn> GetReportColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

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


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command, 
                                                    ListadoPolizasPorCuentaBuilder vouchers) {

      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(vouchers.Entries, command)
      };
    }


    private static FixedList<IReportEntryDto> MapToReportDataEntries(
                                                FixedList<IVouchersByAccountEntry> entries,
                                                BuildReportCommand command) {
      var mappedItems = entries.Select((x) => MapToVoucherEntry((AccountStatementEntry) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private VoucherByAccountEntry MapToVoucherEntry(AccountStatementEntry voucher) {
      var voucherEntry = new VoucherByAccountEntry {
        LedgerUID = voucher.Ledger.UID,
        LedgerNumber = voucher.Ledger.Number,
        LedgerName = voucher.Ledger.Name,
        CurrencyCode = voucher.Currency.Code,
        AccountNumber = voucher.AccountNumber,
        AccountName = voucher.AccountName,
        SubledgerAccountNumber = voucher.SubledgerAccountNumber,
        SectorCode = voucher.Sector.Code,
        Debit = voucher.Debit,
        Credit = voucher.Credit,
        VoucherNumber = voucher.VoucherNumber,
        ElaboratedBy = voucher.ElaboratedBy.Name,
        Concept = voucher.Concept,
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate
      };

      return voucherEntry;
    }

    #endregion

  } // class ListadoPolizasPorCuenta

  public class VoucherByAccountEntry : IReportEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    }

    public int VoucherId {
      get; internal set;
    }

    public string LedgerUID {
      get; internal set;
    }

    public string LedgerNumber {
      get; internal set;
    }

    public string LedgerName {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public int StandardAccountId {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string AccountNumberForBalances {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public decimal? Debit {
      get; internal set;
    }

    public decimal? Credit {
      get; internal set;
    }

    public decimal CurrentBalance {
      get; internal set;
    }

    public string VoucherNumber {
      get; internal set;
    }

    public string ElaboratedBy {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public string SubledgerAccountNumber {
      get; internal set;
    }

    public DateTime AccountingDate {
      get; internal set;
    }

    public DateTime RecordingDate {
      get;
      internal set;
    }

  } // class VoucherByAccountEntry

} // namespace Empiria.FinancialAccounting.Reporting.Builders
