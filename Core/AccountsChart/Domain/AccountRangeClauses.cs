/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : AccountRangeClauses                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Manages account range.                                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting {


  /// <summary>Manages account range.</summary>
  public class AccountRangeClauses {


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


    static public FixedList<AccountRange> GetAccountRangeToFilter(string fromAccount, string toAccount) {

      var accountRangeList = new List<AccountRange>();

      var range = new AccountRange {
        FromAccount = fromAccount,
        ToAccount = toAccount
      };

      accountRangeList.Add(range);
      return accountRangeList.ToFixedList();
    }


    #endregion Public methods


    #region Private methods


    static private AccountRange GetAccountRange(string accountString) {

      string[] accounts = accountString.Split(' ');
      int range = 0;
      var accountRange = new AccountRange();

      foreach (var account in accounts.Where(x => x != "-")) {

        if (account != string.Empty) {

          accountRange.FromAccount = accountRange.FromAccount == string.Empty && range == 0
                        ? $"{account.Trim().Replace(" ", "")}"
                        : accountRange.FromAccount;

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
      }

      return accountRange;
    }

    
    #endregion Private methods


  } // class AccountRangeClauses

} // namespace Empiria.FinancialAccounting
