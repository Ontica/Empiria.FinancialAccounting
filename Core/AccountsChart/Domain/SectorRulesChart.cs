/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : SectorRulesChart                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds sectors rules for all accounts.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds sectors rules for all accounts.</summary>
  static public class SectorRulesChart {

    #region Fields

    static private FixedList<SectorRule> _sectorRules;

    #endregion Fields

    #region Constructors and parsers

    static SectorRulesChart() {
      _sectorRules = AccountsChartData.GetAccountsSectorsRules();
    }


    #endregion Constructors and parsers


    #region Methods


    static internal FixedList<SectorRule> GetAccountSectorRules(Account account) {
      var list = _sectorRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.Sector.Code.CompareTo(y.Sector.Code));

      return list;
    }

    #endregion Methods

  }  // SectorRulesChart

}  // namespace Empiria.FinancialAccounting
