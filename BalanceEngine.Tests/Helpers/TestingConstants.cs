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

    static internal readonly string ACCOUNTS_CHART_UID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
    //"b2328e67-3f2e-45b9-b1f6-93ef6292204e";

    static internal readonly string[] BALANCE_LEDGERS_ARRAY = new string[] { };
    // "2584a757-865c-2025-8025-fa633f200c49"

    static internal readonly DateTime FROM_DATE = new DateTime(2022, 02, 01);

    static internal readonly DateTime TO_DATE = new DateTime(2022, 02, 28);

    static public bool INVOKE_USE_CASES_THROUGH_THE_WEB_API = true;

    static internal readonly string SESSION_TOKEN = ConfigurationData.GetString("Testing.SessionToken");

    static internal readonly string WEB_API_BASE_ADDRESS = "http://172.27.207.97/sicofin/api";

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
