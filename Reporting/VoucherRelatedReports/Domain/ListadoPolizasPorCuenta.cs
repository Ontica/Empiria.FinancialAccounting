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
using Empiria.DynamicData;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

    ///<summary>Listado de polizas por cuenta para reporte operativo.</summary>
    internal class ListadoPolizasPorCuenta : IReportBuilder {

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

            if (buildQuery.ReportType == ReportTypes.ListadoDePolizasPorCuenta) {

                FixedList<AccountStatementEntry> totalsByCurrency = helper.GenerateTotalSummaryByCurrency(
                                                                        orderingVouchers);

                FixedList<AccountStatementEntry> returnedEntries = helper.CombineVouchersWithTotalByCurrency(
                                                                    orderingVouchers, totalsByCurrency);

                return returnedEntries.Select(x => (IVouchersByAccountEntry) x).ToFixedList();
            } else {

                return orderingVouchers.Select(x => (IVouchersByAccountEntry) x).ToFixedList();
            }
        }


        static private FixedList<DataTableColumn> GetColumnsForListadoDePolizasPorCuenta(
                                                    ReportBuilderQuery buildQuery) {
            var columns = new List<DataTableColumn>();
            columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
            columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
            columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
            columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
            if (buildQuery.WithSubledgerAccount) {
                columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
            }
            columns.Add(new DataTableColumn("debit", "Cargo", "decimal"));
            columns.Add(new DataTableColumn("credit", "Abono", "decimal"));
            columns.Add(new DataTableColumn("voucherNumber", "No. Poliza", "text-nowrap"));
            columns.Add(new DataTableColumn("accountingDate", "Afectación", "date"));
            columns.Add(new DataTableColumn("recordingDate", "Registro", "date"));
            columns.Add(new DataTableColumn("concept", "Concepto", "text-nowrap"));
            columns.Add(new DataTableColumn("authorizedBy", "Autorizado por", "text-nowrap"));
            columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text-nowrap"));

            return columns.ToFixedList();
        }


        private static FixedList<DataTableColumn> GetColumnsForListadoMovimientosPorPolizas(
                                                    ReportBuilderQuery buildQuery) {
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


        private static FixedList<DataTableColumn> GetReportColumns(ReportBuilderQuery buildQuery) {

            if (buildQuery.ReportType == ReportTypes.ListadoDePolizasPorCuenta) {

                return GetColumnsForListadoDePolizasPorCuenta(buildQuery);
            } else if (buildQuery.ReportType == ReportTypes.ListadoMovimientosPorPolizas) {

                return GetColumnsForListadoMovimientosPorPolizas(buildQuery);
            }
            return new FixedList<DataTableColumn>();

        }


        static private ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                        FixedList<IVouchersByAccountEntry> vouchers) {
            return new ReportDataDto {
                Query = buildQuery,
                Columns = GetReportColumns(buildQuery),
                Entries = MapToReportDataEntries(vouchers, buildQuery)
            };
        }


        private static FixedList<IReportEntryDto> MapToReportDataEntries(
                                                    FixedList<IVouchersByAccountEntry> entries,
                                                    ReportBuilderQuery buildQuery) {

            var mappedItems = entries.Select((x) => MapToVoucherEntry((AccountStatementEntry) x, buildQuery));
            return new FixedList<IReportEntryDto>(mappedItems);
        }


        static private VoucherByAccountEntry MapToVoucherEntry(AccountStatementEntry entry,
                                                               ReportBuilderQuery buildQuery) {
            var returnedVoucher = new VoucherByAccountEntry();

            ItemTypeClausesForVoucher(returnedVoucher, entry, buildQuery);

            returnedVoucher.LedgerUID = entry.Ledger.UID;
            returnedVoucher.CurrencyCode = entry.Currency.Code;
            returnedVoucher.AccountName = entry.AccountName;

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


        private static void ItemTypeClausesForVoucher(VoucherByAccountEntry returnedVoucher,
                                                      AccountStatementEntry entry,
                                                      ReportBuilderQuery buildQuery) {

            if (entry.ItemType == TrialBalanceItemType.Entry) {
                returnedVoucher.LedgerNumber = entry.Ledger.Number;
                returnedVoucher.LedgerName = entry.Ledger.Name;
                returnedVoucher.AccountNumber = entry.AccountNumber;
                returnedVoucher.SectorCode = entry.Sector.Code;
                returnedVoucher.VoucherId = entry.VoucherId;
                returnedVoucher.VoucherNumber = entry.VoucherNumber;
                returnedVoucher.Concept = entry.Concept;
                returnedVoucher.AccountingDate = entry.AccountingDate;
                returnedVoucher.RecordingDate = entry.RecordingDate;
                
                if (buildQuery.ReportType == ReportTypes.ListadoMovimientosPorPolizas) {

                    returnedVoucher.SubledgerAccountName = entry.SubledgerAccountName;
                    returnedVoucher.VerificationNumber = entry.VerificationNumber;
                }

            } else {
                returnedVoucher.AccountNumber = entry.AccountName;
                returnedVoucher.AccountingDate = ExecutionServer.DateMaxValue;
                returnedVoucher.RecordingDate = ExecutionServer.DateMaxValue;
            }

        }

        #endregion Private methods

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
        } = string.Empty;


        public string LedgerName {
            get; internal set;
        } = string.Empty;


        public string CurrencyCode {
            get; internal set;
        }


        public int StandardAccountId {
            get; internal set;
        }


        public string AccountNumber {
            get; internal set;
        } = string.Empty;


        public string AccountNumberForBalances {
            get; internal set;
        }


        public string AccountName {
            get; internal set;
        }


        public string SectorCode {
            get; internal set;
        } = string.Empty;


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
        } = string.Empty;


        public string VerificationNumber {
            get;
            internal set;
        }


        public string ElaboratedBy {
            get; internal set;
        }


        public string AuthorizedBy {
            get;
            internal set;
        }


        public string Concept {
            get; internal set;
        } = string.Empty;


        public string SubledgerAccountNumber {
            get; internal set;
        } = string.Empty;


        public string SubledgerAccountName {
            get; internal set;
        }


        public DateTime AccountingDate {
            get; internal set;
        }


        public DateTime RecordingDate {
            get; internal set;
        }


        public bool IsVoucher {
            get; internal set;
        } = false;


    } // class VoucherByAccountEntry

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
