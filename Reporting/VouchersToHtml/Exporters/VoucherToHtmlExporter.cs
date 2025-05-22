/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : VoucherToHtmlExporter                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates an HTML document with voucher data.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Text;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Reporting.VouchersToHtml.Exporters {

  /// <summary>Generates an HTML document with voucher data.</summary>
  internal class VoucherToHtmlExporter {

    private readonly VoucherDto _voucher;
    private readonly string _htmlTemplate;

    internal VoucherToHtmlExporter(VoucherDto voucher, string htmlTemplate) {
      _voucher = voucher;
      _htmlTemplate = htmlTemplate;
    }


    internal string GetHtml() {
      StringBuilder html = new StringBuilder(_htmlTemplate);

      html = SetHeaderFields(html);
      html = SetEntries(html);
      html = SetTotals(html);

      return html.ToString();
    }

    #region Build header methods

    private StringBuilder SetHeaderFields(StringBuilder html) {
      html.Replace("{{VOUCHER.ID}}", _voucher.Id.ToString());
      html.Replace("{{VOUCHER.NUMBER}}", _voucher.Number);
      html.Replace("{{VOUCHER.CONCEPT}}", _voucher.Concept);
      html.Replace("{{ACCOUNTS_CHART.NAME}}", _voucher.AccountsChart.Name);
      html.Replace("{{LEDGER.NAME}}", _voucher.Ledger.Name);
      html.Replace("{{VOUCHER_TYPE.NAME}}", _voucher.VoucherType.Name);
      html.Replace("{{TRANSACTION_TYPE.NAME}}", _voucher.TransactionType.Name);
      html.Replace("{{FUNCTIONAL_AREA.NAME}}", _voucher.FunctionalArea.Name);
      html.Replace("{{ACCOUNTING_DATE}}", _voucher.AccountingDate.ToString("dd/MMM/yyyy"));
      html.Replace("{{ELABORATED_BY}}", _voucher.ElaboratedBy);
      html.Replace("{{RECORDING_DATE}}", _voucher.RecordingDate.ToString("dd/MMM/yyyy"));
      html.Replace("{{AUTHORIZED_BY}}", _voucher.AuthorizedBy);
      html.Replace("{{CLOSED_BY}}", _voucher.IsClosed ? _voucher.ClosedBy : _voucher.Status);

      return html;
    }


    #endregion Build header methods

    #region Build entries methods

    private string GetEntriesTemplate() {
      int startIndex = _htmlTemplate.IndexOf("{{VOUCHER_ENTRY.TEMPLATE.START}}");
      int endIndex = _htmlTemplate.IndexOf("{{VOUCHER_ENTRY.TEMPLATE.END}}");

      var template = _htmlTemplate.Substring(startIndex, endIndex - startIndex);

      return template.Replace("{{VOUCHER_ENTRY.TEMPLATE.START}}", string.Empty);
    }


    private StringBuilder ReplaceEntriesTemplate(StringBuilder html,
                                                 StringBuilder entriesHtml) {
      int startIndex = html.ToString().IndexOf("{{VOUCHER_ENTRY.TEMPLATE.START}}");
      int endIndex = html.ToString().IndexOf("{{VOUCHER_ENTRY.TEMPLATE.END}}");

      html.Remove(startIndex, endIndex - startIndex);

      return html.Replace("{{VOUCHER_ENTRY.TEMPLATE.END}}", entriesHtml.ToString());
    }


    private StringBuilder SetEntries(StringBuilder html) {
      string TEMPLATE = GetEntriesTemplate();

      var entries = _voucher.Entries.FindAll(x => x.ItemType == VoucherEntryItemType.AccountEntry);

      var entriesHtml = new StringBuilder();

      foreach (var entry in entries) {
        var entryHtml = new StringBuilder(TEMPLATE.Replace("{{ACCOUNT.NUMBER}}", entry.AccountNumber));

        entryHtml.Replace("{{ACCOUNT.NAME}}", entry.AccountName);
        entryHtml.Replace("{{SECTOR.CODE}}", entry.Sector);

        entryHtml.Replace("{{SUBLEDGER_ACCOUNT.NUMBER}}", entry.SubledgerAccountNumber);
        entryHtml.Replace("{{SUBLEDGER_ACCOUNT.NAME}}", entry.SubledgerAccountName);

        entryHtml.Replace("{{VERIFICATION_NUMBER}}", entry.VerificationNumber);
        entryHtml.Replace("{{RESPONSIBILITY_AREA.CODE}}", entry.ResponsibilityArea);

        entryHtml.Replace("{{CURRENCY.CODE}}", entry.Currency);

        entryHtml.Replace("{{EXCHANGE.RATE}}", entry.ExchangeRate != 1 ?
                                               entry.ExchangeRate.ToString("C6") : string.Empty);

        entryHtml.Replace("{{DEBIT.AMOUNT}}", entry.Debit.ToString("C2"));
        entryHtml.Replace("{{CREDIT.AMOUNT}}", entry.Credit.ToString("C2"));

        entriesHtml.Append(entryHtml);
      }

      return ReplaceEntriesTemplate(html, entriesHtml);
    }

    #endregion Build entries methods

    #region Build totals methods

    private string GetTotalsTemplate() {
      int startIndex = _htmlTemplate.IndexOf("{{VOUCHER_TOTALS.TEMPLATE.START}}");
      int endIndex = _htmlTemplate.IndexOf("{{VOUCHER_TOTALS.TEMPLATE.END}}");

      var template = _htmlTemplate.Substring(startIndex, endIndex - startIndex);

      return template.Replace("{{VOUCHER_TOTALS.TEMPLATE.START}}", string.Empty);
    }


    private StringBuilder ReplaceTotalsTemplate(StringBuilder html, StringBuilder totalsHtml) {
      int startIndex = html.ToString().IndexOf("{{VOUCHER_TOTALS.TEMPLATE.START}}");
      int endIndex = html.ToString().IndexOf("{{VOUCHER_TOTALS.TEMPLATE.END}}");

      html.Remove(startIndex, endIndex - startIndex);

      return html.Replace("{{VOUCHER_TOTALS.TEMPLATE.END}}", totalsHtml.ToString());
    }


    private StringBuilder SetTotals(StringBuilder html) {
      string TEMPLATE = GetTotalsTemplate();

      var totalEntries = _voucher.Entries.FindAll(x => x.ItemType == VoucherEntryItemType.TotalsEntry);

      var totalsHtml = new StringBuilder();

      foreach (var entry in totalEntries) {
        var totalHtml = new StringBuilder(TEMPLATE.Replace("{{TOTAL.TITLE}}", entry.AccountName));

        totalHtml.Replace("{{DEBIT.TOTAL}}", entry.Debit.ToString("C2"));
        totalHtml.Replace("{{CREDIT.TOTAL}}", entry.Credit.ToString("C2"));

        totalsHtml.Append(totalHtml);
      }

      return ReplaceTotalsTemplate(html, totalsHtml);
    }

    #endregion Build totals methods

  }  // class VoucherToHtmlExporter

}  // namespace Empiria.FinancialAccounting.Reporting.VouchersToHtml.Exporters
