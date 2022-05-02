/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : PdfExporterService                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services to export accounting information to PDF files.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Office;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services to export accounting information to PDF files.</summary>
  public class PdfExporterService {

    public FileReportDto Export(VoucherDto voucher) {
      Assertion.AssertObject(voucher, nameof(voucher));

      string filename = GetVoucherPdfFileName(voucher);

      if (ExistsFile(filename)) {
        return ToFileReportDto(filename);
      }

      string html = GetHtml(voucher);

      SaveHtmlAsPdf(html, filename);

      return ToFileReportDto(filename);
    }


    public FileReportDto Export(ReconciliationResultDto reconciliation) {
      Assertion.AssertObject("reconciliation", nameof(reconciliation));

      string filename = GetPdfFileName(reconciliation);

      if (ExistsFile(filename)) {
        RemoveFile(filename);
      }

      string html = GetHtml(reconciliation);

      SaveHtmlAsPdf(html, filename);

      return ToFileReportDto(filename);
    }


    #region Private methods

    private bool ExistsFile(string filename) {
      string fullPath = GetFullPath(filename);

      return File.Exists(fullPath);
    }

    private string GetFullPath(string filename) {
      return Path.Combine(FileTemplateConfig.GenerationStoragePath + "/vouchers", filename);
    }


    private string GetHtml(ReconciliationResultDto reconciliation) {
      string templateUID = "ReconciliationResult.HtmlTemplate";

      var template = FileTemplateConfig.Parse(templateUID);

      string htmlTemplate = File.ReadAllText(template.TemplateFullPath);

      var htmlExporter = new ReconciliationHtmlExporter(reconciliation, htmlTemplate);

      return htmlExporter.GetHtml();
    }


    private string GetHtml(VoucherDto voucher) {
      var templateConfig = GetVoucherTemplateConfig(voucher);

      string htmlTemplate = File.ReadAllText(templateConfig.TemplateFullPath);

      var htmlExporter = new VoucherToHtmlExporter(voucher, htmlTemplate);

      return htmlExporter.GetHtml();
    }


    private string GetPdfFileName(ReconciliationResultDto reconciliation) {
      return $"conciliacion.derivados.{reconciliation.Command.Date.ToString("yyyy.MM.dd")}.pdf";
    }


    private string GetVoucherPdfFileName(VoucherDto voucher) {
      if (voucher.IsClosed) {
        return $"poliza.{voucher.Number}.{voucher.Ledger.UID}.pdf";
      } else {
        return $"poliza.{voucher.Id.ToString("0000000000")}.{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss")}.pdf";
      }
    }


    private FileTemplateConfig GetVoucherTemplateConfig(VoucherDto voucher) {
      string templateUID;

      if (voucher.AllEntriesAreInBaseCurrency) {
        templateUID = $"OperationalReportTemplate.VoucherHtmlTemplateOnlyInBaseCurrency";
      } else {
        templateUID = $"OperationalReportTemplate.VoucherHtmlTemplateMultiCurrencies";
      }

      return FileTemplateConfig.Parse(templateUID);
    }


    private bool RemoveFile(string filename) {
      string fullPath = GetFullPath(filename);

      if (File.Exists(fullPath)) {
        File.Delete(fullPath);
        return true;
      }

      return false;
    }

    private void SaveHtmlAsPdf(string html, string filename) {
      string fullpath = GetFullPath(filename);

      var pdfConverter = new HtmlToPdfConverter();

      var options = new PdfConverterOptions {
        BaseUri = FileTemplateConfig.TemplatesStoragePath
      };

      pdfConverter.Convert(html, fullpath, options);
    }


    private FileReportDto ToFileReportDto(string filename) {
      return new FileReportDto(FileType.Pdf, FileTemplateConfig.BaseUrl + "/vouchers/" + filename);
    }


    #endregion Private methods

  }  // class PdfExporterService

} // namespace Empiria.FinancialAccounting.Reporting
