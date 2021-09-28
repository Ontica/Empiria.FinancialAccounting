/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information Holder                   *
*  Type     : TextFileVoucherEntry                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data structure that holds information in a text file line with imported voucher entry data.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data structure that holds information in a text file line with imported voucher entry data.</summary>
  public class TextFileVoucherEntry {


    internal TextFileVoucherEntry(AccountsChartMasterData accountsChartMasterData, string textLine) {
      LoadTextLine(accountsChartMasterData.AccountsPattern, textLine);
      CleanupFields(accountsChartMasterData);
    }

    public string LedgerNumber {
      get; private set;
    }

    public string TransactionNumber {
      get; private set;
    }

    public string TransactionConcept {
      get; private set;
    }

    public string Source {
      get; private set;
    }

    public DateTime AccountingDate {
      get; private set;
    }

    public decimal Amount {
      get; private set;
    }

    public string CurrencyCode {
      get; private set;
    }

    public string SectorCode {
      get; private set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

    public string SubledgerAccount {
      get; private set;
    }

    public string AvailabilityCode {
      get; private set;
    }

    public string ResponsibilityArea {
      get; private set;
    }

    public string AccountNumber {
      get; private set;
    }

    #region Private methods

    private void CleanupFields(AccountsChartMasterData masterData) {
      this.AccountNumber = GetAccountNumber(masterData, this.AccountNumber);
      this.SubledgerAccount = GetSubledgerAccount(this.SubledgerAccount);
    }


    static private string ConvertAccountNumberToPattern(string account, string pattern) {
      Assertion.AssertObject(account, "account");
      Assertion.AssertObject(pattern, "pattern");

      pattern = pattern.Replace("0", "X");

      int patternPlaceholdersLength = pattern.Count(c => c == 'X');

      if (account.Length > patternPlaceholdersLength) {
        Assertion.AssertFail("Number of placeholders in pattern is different than " +
                             "number of characters in the input string.");
      } else {
        account = account.PadRight(patternPlaceholdersLength, '0');
      }

      var reg = new Regex(new string('N', account.Length).Replace("N", "(\\w)"));

      var regX = new Regex("X");

      for (int i = 1; i <= account.Length; i++) {
        pattern = regX.Replace(pattern, "$" + i.ToString(), 1);
      }

      return reg.Replace(account, pattern);
    }


    static private string GetAccountNumber(AccountsChartMasterData masterData, string accountNumber) {
      string temp = ConvertAccountNumberToPattern(accountNumber, masterData.AccountsPattern);

      Assertion.Assert(temp.Replace(masterData.AccountNumberSeparator.ToString(), String.Empty) == accountNumber,
                      "There was a problem in ConvertAccountNumberToPattern method.");

      temp = EmpiriaString.TrimAll(temp, $"{masterData.AccountNumberSeparator}-00", String.Empty);

      return temp;
    }


    static private string GetSubledgerAccount(string subledgerAccount) {
      return subledgerAccount.TrimStart('0');
    }


    private void LoadTextLine(string accountsPattern, string textLine) {
      int lineLengthExcess = accountsPattern.Length == 22 ? 0 : 30;

      this.Source = textLine.Substring(0, 6);
      this.LedgerNumber = textLine.Substring(6, 4);
      this.TransactionNumber = textLine.Substring(10, 12);
      this.AccountingDate = new DateTime(int.Parse(textLine.Substring(45, 4)),
                                         int.Parse(textLine.Substring(43, 2)),
                                         int.Parse(textLine.Substring(41, 2)));
      this.TransactionConcept = textLine.Substring(49, 240);

      this.AccountNumber = textLine.Substring(289, 16 + lineLengthExcess);
      this.SectorCode = textLine.Substring(305 + lineLengthExcess, 2);
      this.Amount = decimal.Parse(textLine.Substring(316 + lineLengthExcess, 13) + "." +
                                  textLine.Substring(329 + lineLengthExcess, 6));
      this.CurrencyCode = textLine.Substring(335 + lineLengthExcess, 2);
      this.ExchangeRate = decimal.Parse(textLine.Substring(337 + lineLengthExcess, 7) + "." +
                                        textLine.Substring(344 + lineLengthExcess, 8));
      this.SubledgerAccount = textLine.Substring(371 + lineLengthExcess, 20);
      this.AvailabilityCode = textLine.Substring(403 + lineLengthExcess, 1);
      this.ResponsibilityArea = textLine.Substring(419 + lineLengthExcess, 6);

    }

    #endregion Private methods

  }  // class TextFileVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
