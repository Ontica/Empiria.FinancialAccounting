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
using System.Collections.Generic;
using System.IO;
using System.Text;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

using Empiria.FinancialAccounting.Reporting.Reconciliation.Exporters;

using Empiria.FinancialAccounting.Reporting.VouchersToHtml.Exporters;


namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services to export accounting information to PDF files.</summary>
  public class PdfExporterService {

    static private readonly Dictionary<string, string> _templates = new Dictionary<string, string>(8);

    #region Services

    public FileReportDto Export(VoucherDto voucher) {
      Assertion.Require(voucher, nameof(voucher));

      string filename = GetVoucherPdfFileName(voucher);

      if (ExistsFile(filename)) {
        return ToFileReportDto(filename);
      }

      string html = BuildVoucherHtml(voucher);

      SaveHtmlAsPdf(html, filename);

      return ToFileReportDto(filename);
    }


    public FileReportDto Export(FixedList<VoucherDto> vouchersToPrint) {
      Assertion.Require(vouchersToPrint, nameof(vouchersToPrint));

      string filename = $"{vouchersToPrint.Count}.polizas.{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss")}.pdf";

      string html = BuildVoucherHtml(vouchersToPrint);

      SaveHtmlAsPdf(html, filename);

      return ToFileReportDto(filename);
    }


    public FileReportDto Export(ReconciliationResultDto reconciliation) {
      Assertion.Require("reconciliation", nameof(reconciliation));

      string filename = $"conciliacion.derivados.{reconciliation.Command.Date.ToString("yyyy.MM.dd")}.pdf";

      if (ExistsFile(filename)) {
        RemoveFile(filename);
      }

      string html = BuildReconciliationHtml(reconciliation);

      SaveHtmlAsPdf(html, filename);

      return ToFileReportDto(filename);
    }

    #endregion Services

    #region Builders

    private string BuildReconciliationHtml(ReconciliationResultDto reconciliation) {
      string templateUID = "ReconciliationResult.HtmlTemplate";

      string htmlTemplate = GetHtmlTemplate(templateUID);

      var htmlExporter = new ReconciliationHtmlExporter(reconciliation, htmlTemplate);

      return htmlExporter.GetHtml();
    }


    private string BuildVoucherHtml(VoucherDto voucher) {
      var htmlTemplate = GetVoucherHtmlTemplate(voucher);

      var htmlExporter = new VoucherToHtmlExporter(voucher, htmlTemplate);

      string wrapper = GetVoucherWrapperHtml();

      return wrapper.Replace("{{VOUCHER.HTML.CONTENT}}", htmlExporter.GetHtml());
    }


    private string BuildVoucherHtml(FixedList<VoucherDto> vouchersToPrint) {
      var html = new StringBuilder();

      foreach (var voucher in vouchersToPrint) {
        var htmlTemplate = GetVoucherHtmlTemplate(voucher);

        var htmlExporter = new VoucherToHtmlExporter(voucher, htmlTemplate);

        if (html.Length != 0) {
          html.AppendLine("<div class=\"breakpage\"></div>");
        }
        html.Append(htmlExporter.GetHtml());
      }

      string wrapper = GetVoucherWrapperHtml();

      return wrapper.Replace("{{VOUCHER.HTML.CONTENT}}", html.ToString());
    }

    #endregion Builders

    #region Helpers

    private bool ExistsFile(string filename) {
      string fullPath = GetFileName(filename);

      return File.Exists(fullPath);
    }


    private string GetFileName(string filename) {
      return Path.Combine(FileTemplateConfig.GenerationStoragePath + "/vouchers", filename);
    }


    private string GetVoucherPdfFileName(VoucherDto voucher) {
      if (voucher.IsClosed) {
        return $"poliza.{voucher.Number}.{voucher.Ledger.UID}.pdf";
      } else {
        return $"poliza.{voucher.Id.ToString("0000000000")}.{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss")}.pdf";
      }
    }


    private string GetVoucherHtmlTemplate(VoucherDto voucher) {
      string templateUID;

      if (voucher.AllEntriesAreInBaseCurrency) {
        templateUID = $"OperationalReportTemplate.VoucherHtmlTemplateOnlyInBaseCurrency";
      } else {
        templateUID = $"OperationalReportTemplate.VoucherHtmlTemplateMultiCurrencies";
      }

      return GetHtmlTemplate(templateUID);
    }


    private string GetHtmlTemplate(string templateUID) {
      if (!_templates.ContainsKey(templateUID)) {
        var templateConfig = FileTemplateConfig.Parse(templateUID);
        var htmlTemplate = File.ReadAllText(templateConfig.TemplateFullPath);

        _templates.Add(templateUID, htmlTemplate);
      }
      return _templates[templateUID];
    }


    private string GetVoucherWrapperHtml() {
      string templateUID = $"OperationalReportTemplate.VoucherHtmlWrapperTemplate";

      return GetHtmlTemplate(templateUID);
    }


    private void RemoveFile(string filename) {
      string fullPath = GetFileName(filename);

      if (File.Exists(fullPath)) {
        File.Delete(fullPath);
      }
    }


    private void SaveHtmlAsPdf(string html, string filename) {
      string fullpath = GetFileName(filename);

      var pdfConverter = new HtmlToPdfConverter();

      var options = new PdfConverterOptions {
        BaseUri = FileTemplateConfig.TemplatesStoragePath
      };

      pdfConverter.Convert(html, fullpath, options);
    }


    private FileReportDto ToFileReportDto(string filename) {
      return new FileReportDto(FileType.Pdf, FileTemplateConfig.GeneratedFilesBaseUrl + "/vouchers/" + filename);
    }

    #endregion Helpers

  }  // class PdfExporterService

} // namespace Empiria.FinancialAccounting.Reporting
