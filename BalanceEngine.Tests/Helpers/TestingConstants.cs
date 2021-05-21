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

    static internal readonly decimal ACCOUNT_BALANCE = 2000;

    static internal readonly string ACCOUNT_NAME = "Billetes";

    static internal readonly string ACCOUNT_NUMBER = "1101-01";

    static internal readonly string[] Ledgers = { "a70281d1-c9f0-4121-afc1-fe32026feb0d",
                                                  "82cb925c-1752-4a2b-b0c5-5ae87576cae3" };

    static internal readonly int AccountCatalogueId = 1;

    static internal readonly string AccountNumber = "";

    static internal readonly string AccountName = "";

    static internal readonly string Fields = "";

    static internal readonly string Condition = "";

    static internal readonly string Grouping = "";

    static internal readonly string Having = "";

    static internal readonly string Ordering = "";

    static internal readonly string[] Sectors = { "0", "5", "7", "11" };

    static internal readonly DateTime FromDate = new DateTime(2019, 01, 01);

    static internal readonly DateTime ToDate = new DateTime(2020, 12, 31);

    static internal readonly int BalanceGroupId = 1;

    static internal readonly string SESSION_TOKEN = ConfigurationData.GetString("Testing.SessionToken");


  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
