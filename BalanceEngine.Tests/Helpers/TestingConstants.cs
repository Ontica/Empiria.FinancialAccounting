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

    static internal readonly int ACCOUNTS_CHART_ID = 1;

    static internal readonly string ACCOUNTS_CHART_UID = "b2328e67-3f2e-45b9-b1f6-93ef6292204e";

    static internal readonly string[] BALANCE_LEDGERS_ARRAY =
                                  new string[] { "3426b979-4797-4e13-4643-937361bc0fd9"}; //"2584a757-865c-2025-8025-fa633f200c49" 

    static internal readonly DateTime FROM_DATE = new DateTime(2020, 03, 01);

    static internal readonly DateTime TO_DATE = new DateTime(2020, 03, 31);

    static internal readonly string SESSION_TOKEN = ConfigurationData.GetString("Testing.SessionToken");


  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
