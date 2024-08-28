/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Converter                               *
*  Type     : AccountRangeConverter                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to convert strings to AccountRange data structures.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Provides services to convert strings to AccountRange data structures.</summary>
  static public class AccountRangeConverter {

    #region Public methods

    static public FixedList<AccountRange> GetAccountRangeToFilter(string[] accounts) {

      var token = " - ";
      var accountRangeList = new List<AccountRange>();

      foreach (var account in accounts) {
        var accountRange = new AccountRange();

        if (account.Contains(token)) {
          accountRange = GetAccountRange(account);

        } else {
          accountRange.FromAccount = account;
        }

        accountRangeList.Add(accountRange);
      }

      return accountRangeList.ToFixedList();
    }


    static public FixedList<AccountRange> GetAccountRangeToFilter(string fromAccount,
                                                                  string toAccount) {

      var accountRangeList = new List<AccountRange>();

      var range = new AccountRange {
        FromAccount = fromAccount,
        ToAccount = toAccount
      };

      accountRangeList.Add(range);
      return accountRangeList.ToFixedList();
    }

    #endregion Public methods

    #region Helpers

    static private AccountRange GetAccountRange(string accountString) {

      string[] accounts = accountString.Split(' ');
      int range = 0;
      var accountRange = new AccountRange();

      foreach (var account in accounts.Where(x => x != "-")) {

        if (String.IsNullOrWhiteSpace(account)) {
          continue;
        }

        if (accountRange.FromAccount == string.Empty && range == 0) {
          accountRange.FromAccount = $"{account.Trim().Replace(" ", "")}";
        }


        if (accountRange.FromAccount != string.Empty && range == 0) {

          foreach (var c in accountRange.FromAccount) {
            if (!char.IsNumber(c) && c != '.' && c != '-') {
              Assertion.EnsureFailed($"La cuenta '{accountRange.FromAccount} -' del rango '{accountString}' " +
                                      $"contiene espacios, letras o caracteres no permitidos.");
            }
          }

        } else if (accountRange.ToAccount == string.Empty && range == 1) {

          accountRange.ToAccount = $"{account.Trim().Replace(" ", "")}";

          foreach (var c in accountRange.ToAccount) {
            if (!char.IsNumber(c) && c != '.' && c != '-') {
              Assertion.EnsureFailed($"La cuenta '- {accountRange.ToAccount}' del rango '{accountString}' " +
                                      $"contiene espacios, letras o caracteres no permitidos.");
            }
          }

        } else {
          Assertion.EnsureFailed($"El rango '{accountString}' " +
                                  $"contiene espacios, letras o caracteres no permitidos.");
        }
        range++;
      }

      return accountRange;
    }

    #endregion Helpers

  } // class AccountRangeConverter

} // namespace Empiria.FinancialAccounting.Adapters
