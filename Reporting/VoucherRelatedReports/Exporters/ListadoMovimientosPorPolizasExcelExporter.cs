/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ListadoPolizasPorCuentaExcelExporter         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de los movimientos por polizas en un archivo Excel.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;
using Empiria.Office;
using Empiria.Storage;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Reporting {

    /// <summary>Genera los datos de los movimientos por polizas en un archivo Excel.</summary>
    public class ListadoMovimientosPorPolizasExcelExporter : IExcelExporter {


        private readonly ReportDataDto _reportData;
        private readonly FileTemplateConfig _template;

        #region Public methods

        public ListadoMovimientosPorPolizasExcelExporter(ReportDataDto reportData,
                                                         FileTemplateConfig template) {
            Assertion.Require(reportData, "reportData");
            Assertion.Require(template, "template");

            _reportData = reportData;
            _template = template;
        }

        public FileDto CreateExcelFile() {
            var excelFile = new ExcelFile(_template);

            excelFile.Open();

            SetHeader(excelFile);

            FillOutRows(excelFile, _reportData.Entries.Select(x => (VoucherByAccountEntry) x));

            excelFile.Save();

            excelFile.Close();

            return excelFile.ToFileDto();
        }

        #endregion Public methods


        #region Private methods

        private void SetHeader(ExcelFile excelFile) {
            excelFile.SetCell($"A2", _template.Title);

            var subTitle = $"Del {_reportData.Query.FromDate.ToString("dd/MMM/yyyy")} al " +
                           $"{_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

            excelFile.SetCell($"A3", subTitle);
        }

        
        private void FillOutRows(ExcelFile excelFile, IEnumerable<VoucherByAccountEntry> vouchers) {
            int i = 5;

            foreach (var voucher in vouchers) {
                excelFile.SetCell($"A{i}", voucher.LedgerNumber);
                excelFile.SetCell($"B{i}", voucher.LedgerName);
                excelFile.SetCell($"C{i}", voucher.CurrencyCode);
                excelFile.SetCell($"D{i}", voucher.VoucherNumber);
                excelFile.SetCell($"E{i}", voucher.AccountNumber);
                excelFile.SetCell($"F{i}", voucher.AccountName);
                excelFile.SetCell($"G{i}", voucher.SectorCode);
                excelFile.SetCell($"H{i}", voucher.SubledgerAccountNumber);
                excelFile.SetCell($"I{i}", voucher.SubledgerAccountName);
                excelFile.SetCell($"J{i}", voucher.VerificationNumber);
                excelFile.SetCell($"K{i}", (decimal) voucher.Debit);
                excelFile.SetCell($"L{i}", (decimal) voucher.Credit);
                excelFile.SetCell($"M{i}", voucher.AccountingDate);
                excelFile.SetCell($"N{i}", voucher.RecordingDate);
                excelFile.SetCell($"O{i}", voucher.Concept);
                excelFile.SetCell($"P{i}", voucher.AuthorizedBy);
                excelFile.SetCell($"Q{i}", voucher.ElaboratedBy);

                //excelFile.SetRowBold(0,0);
                i++;
            }
            //excelFile.RemoveColumn("");
        }

        #endregion Private methods


    } // class ListadoMovimientosPorPolizasExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
