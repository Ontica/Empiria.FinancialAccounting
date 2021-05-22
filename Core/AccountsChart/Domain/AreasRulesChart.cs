/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : AreasRulesChart                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds responsibility areas rules for all accounts.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds responsibility areas rules for all accounts.</summary>
  static public class AreasRulesChart {

    #region Fields

    static private FixedList<AreaRule> _areasRules;

    #endregion Fields

    #region Constructors and parsers

    static AreasRulesChart() {
      _areasRules = AccountsChartData.GetAccountsAreasRules();
    }


    #endregion Constructors and parsers


    #region Methods


    static internal FixedList<AreaRule> GetAccountAreasRules(Account account) {
      var list = _areasRules.FindAll(x => x.AccountId == account.StandardAccountId);

      list.Sort((x, y) => x.AccountId.CompareTo(y.AccountId));

      return list;
    }

    #endregion Methods

  }  // AreasRulesChart

}  // namespace Empiria.FinancialAccounting
