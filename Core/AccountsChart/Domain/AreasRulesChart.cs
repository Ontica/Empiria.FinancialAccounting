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

    static private FixedList<AreaRule> _areasRules = AccountsChartData.GetAccountsAreasRules();

    #endregion Fields

    #region Methods

    static internal FixedList<AreaRule> GetAccountAreasRules(Account account) {
      var list = _areasRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.StandardAccountId.CompareTo(y.StandardAccountId));

      return list;
    }

    #endregion Methods

  }  // AreasRulesChart

}  // namespace Empiria.FinancialAccounting
