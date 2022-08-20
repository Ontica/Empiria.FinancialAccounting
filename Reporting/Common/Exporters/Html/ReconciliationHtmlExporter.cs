/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Html report                          *
*  Type     : ReconciliationHtmlExporter                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates an HTML document with the reconciliation results.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Text;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Generates an HTML document with the reconciliation results.</summary>
  internal class ReconciliationHtmlExporter {

    private readonly ReconciliationResultDto _reconciliation;
    private readonly string _htmlTemplate;

    internal ReconciliationHtmlExporter(ReconciliationResultDto reconciliation,
                                        string htmlTemplate) {
      _reconciliation = reconciliation;
      _htmlTemplate = htmlTemplate;
    }


    internal string GetHtml() {
      StringBuilder html = new StringBuilder(_htmlTemplate);

      html = SetHeaderFields(html);
      html = SetEntries(html);

      return html.ToString();
    }

    #region Build header methods

    private StringBuilder SetHeaderFields(StringBuilder html) {
      html = html.Replace("{{RECONCILIATION.DATE}}",
                          _reconciliation.Command.Date.ToString("dd/MMM/yyyy"));

      return html;
    }


    #endregion Build header methods

    #region Build entries methods

    private string GetEntriesTemplate() {
      int startIndex = _htmlTemplate.IndexOf("{{REPORT_ENTRIES.TEMPLATE.START}}");
      int endIndex = _htmlTemplate.IndexOf("{{REPORT_ENTRIES.TEMPLATE.END}}");

      var template = _htmlTemplate.Substring(startIndex, endIndex - startIndex);

      return template.Replace("{{REPORT_ENTRIES.TEMPLATE.START}}", string.Empty);
    }

    private StringBuilder ReplaceEntriesTemplate(StringBuilder html,
                                         StringBuilder entriesHtml) {
      int startIndex = html.ToString().IndexOf("{{REPORT_ENTRIES.TEMPLATE.START}}");
      int endIndex = html.ToString().IndexOf("{{REPORT_ENTRIES.TEMPLATE.END}}");

      html = html.Remove(startIndex, endIndex - startIndex);

      return html.Replace("{{REPORT_ENTRIES.TEMPLATE.END}}", entriesHtml.ToString());
    }


    private StringBuilder SetEntries(StringBuilder html) {
      string TEMPLATE = GetEntriesTemplate();

      var entriesHtml = new StringBuilder();

      foreach (var entry in _reconciliation.Entries) {
        var entryHtml = TEMPLATE.Replace("{{ACCOUNT.NUMBER}}", entry.AccountNumber);
        entryHtml = entryHtml.Replace("{{CURRENCY.CODE}}", entry.CurrencyCode);
        entryHtml = entryHtml.Replace("{{SECTOR.CODE}}", entry.SectorCode);
        entryHtml = entryHtml.Replace("{{OPERATIONAL_TOTAL}}", ToCurrencyString(entry.OperationalTotal));
        entryHtml = entryHtml.Replace("{{ACCOUNTING_TOTAL}}", ToCurrencyString(entry.AccountingTotal));
        entryHtml = entryHtml.Replace("{{DIFFERENCE}}", ToCurrencyString(entry.Difference));

        entriesHtml = entriesHtml.Append(entryHtml);
      }

      return ReplaceEntriesTemplate(html, entriesHtml);
    }

    private string ToCurrencyString(decimal amount) {
      if (amount >= 0) {
        return amount.ToString("N2");
      }
      return $"({Math.Abs(amount).ToString("N2")})";
    }

    #endregion Build entries methods

  }  // class ReconciliationHtmlExporter

}  // namespace Empiria.FinancialAccounting.Reporting
