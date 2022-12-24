/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service Provider                        *
*  Type     : AccountsFormatter                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides validation and account numbers formatting services.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Empiria.FinancialAccounting {

  /// <summary>Provides validation and account numbers formatting services.</summary>
  internal class AccountsFormatter {

    private readonly AccountsChart _accountsChart;

    public AccountsFormatter(AccountsChart accountsChart) {
      Assertion.Require(accountsChart, nameof(accountsChart));

      _accountsChart = accountsChart;
    }


    internal string BuildParentAccountNumber(string accountNumber) {
      Assertion.Require(accountNumber, nameof(accountNumber));

      var separator = _accountsChart.MasterData.AccountNumberSeparator;

      return accountNumber.Substring(0, accountNumber.LastIndexOf(separator));
    }


    internal string FormatAccountNumber(string accountNumber) {
      string temp = EmpiriaString.TrimSpacesAndControl(accountNumber);

      Assertion.Require(temp, nameof(accountNumber));

      char separator = _accountsChart.MasterData.AccountNumberSeparator;
      string pattern = _accountsChart.MasterData.AccountsPattern;

      temp = temp.Replace(separator.ToString(), string.Empty);

      temp = temp.TrimEnd('0');

      if (temp.Length > EmpiriaString.CountOccurences(pattern, '0')) {
        Assertion.RequireFail($"Number of placeholders in pattern ({pattern}) is less than the " +
                              $"number of characters in the input string ({accountNumber}).");
      } else {
        temp = temp.PadRight(EmpiriaString.CountOccurences(pattern, '0'), '0');
      }

      for (int i = 0; i < pattern.Length; i++) {
        if (pattern[i] == separator) {
          temp = temp.Insert(i, separator.ToString());
        }
      }

      while (true) {
        if (temp.EndsWith($"{separator}0000")) {
          temp = temp.Remove(temp.Length - 5);

        } else if (temp.EndsWith($"{separator}000")) {
          temp = temp.Remove(temp.Length - 4);

        } else if (temp.EndsWith($"{separator}00")) {
          temp = temp.Remove(temp.Length - 3);

        } else if (temp.EndsWith($"{separator}0")) {
          temp = temp.Remove(temp.Length - 2);

        } else {
          break;

        }
      }

      return temp;
    }


    internal bool IsAccountNumberFormatValid(string accountNumber) {
      string formatted = FormatAccountNumber(accountNumber);

      if (this.Equals(AccountsChart.IFRS) && formatted.Contains("00")) {
        return false;
      }

      if (this.Equals(AccountsChart.IFRS) && !EmpiriaString.All(formatted, "0123456789.")) {
        return false;
      }

      if (this.Equals(AccountsChart.IFRS) && formatted.StartsWith("0")) {
        return false;
      }

      return (formatted == accountNumber);
    }

  }  // class AccountsFormatter

}  // namespace Empiria.FinancialAccounting
