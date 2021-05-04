/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test Helpers                            *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Testing constants                       *
*  Type     : TestingConstants                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides testing constants.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides testing constants.</summary>
  static public class TestingConstants {

    static internal decimal ACCOUNT_BALANCE = 2000;

    static internal string ACCOUNT_NAME => "Billetes";

    static internal string ACCOUNT_NUMBER => "1101-01";

    static internal string SESSION_TOKEN => ConfigurationData.GetString("Testing.SessionToken");

    static internal int LedgerId = 9;

    static internal int AccountCatalogueId = 1;

    static internal string AccountNumber = "";
    
    static internal string AccountName = "";

    static internal string Fields = "";

    static internal string Condition = "";

    static internal string Grouping = "";

    static internal string Having = "";

    static internal string Ordering = "";

    static internal DateTime InitialDate = new DateTime(2020, 03, 01);

    static internal DateTime FinalDate = new DateTime(2020, 03, 31);

    static internal int BalanceGroupId = 1;

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
