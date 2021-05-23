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

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds currencies rules for all accounts.</summary>
  static public class CurrenciesRulesChart {

    #region Fields

    static private FixedList<CurrencyRule> _currenciesRules;

    #endregion Fields

    #region Constructors and parsers

    static CurrenciesRulesChart() {
      _currenciesRules = AccountsChartData.GetAccountsCurrenciesRules();
    }


    #endregion Constructors and parsers


    #region Methods


    static internal FixedList<CurrencyRule> GetAccountCurrenciesRules(Account account) {
      var list = _currenciesRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.Currency.Code.CompareTo(y.Currency.Code));

      return list;
    }

    #endregion Methods

  }  // CurrenciesRulesChart

}  // namespace Empiria.FinancialAccounting
