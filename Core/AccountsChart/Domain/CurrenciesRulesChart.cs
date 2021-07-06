/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : CurrenciesRulesChart                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds currencies rules for all accounts.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds currencies rules for all accounts.</summary>
  static public class CurrenciesRulesChart {

    #region Fields

    static private EmpiriaHashTable<FixedList<CurrencyRule>> _currenciesRules =
                                                                new EmpiriaHashTable<FixedList<CurrencyRule>>();

    #endregion Fields

    #region Constructors and parsers

    static CurrenciesRulesChart() {
      var list = AccountsChartData.GetAccountsCurrenciesRules()
                                  .GroupBy(x => x.StandardAccountId);

      foreach (var item in list) {
        _currenciesRules.Insert(item.Key.ToString(),
                                item.ToList().ToFixedList());
      }
    }


    #endregion Constructors and parsers


    #region Methods


    static internal FixedList<CurrencyRule> GetAccountCurrenciesRules(Account account) {
      FixedList<CurrencyRule> list;

      _currenciesRules.TryGetValue(account.StandardAccountId.ToString(), out list);

      if (list != null) {
        return list;
      } else {
        return new FixedList<CurrencyRule>();
      }


      //var currencies = from rule in _currenciesRules
      //                 where rule.StandardAccountId == account.StandardAccountId
      //                 orderby rule.Currency.Code
      //                 select rule;

      ////var list = _currenciesRules.Select(x => x.StandardAccountId == account.StandardAccountId).;

      //// list.Sort((x, y) => x.Currency.Code.CompareTo(y.Currency.Code));

      //return new FixedList<CurrencyRule>(currencies);
    }

    #endregion Methods

  }  // CurrenciesRulesChart

}  // namespace Empiria.FinancialAccounting
