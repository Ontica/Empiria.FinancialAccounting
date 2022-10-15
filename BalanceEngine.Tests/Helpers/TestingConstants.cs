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

    static internal readonly string[] BALANCE_LEDGERS_ARRAY = new string[] { };

    static internal readonly DateTime FROM_DATE = new DateTime(2021, 12, 01);

    static internal readonly DateTime TO_DATE = new DateTime(2022, 06, 30);

    static public bool INVOKE_USE_CASES_THROUGH_THE_WEB_API = false;

    static internal readonly string WEB_API_BASE_ADDRESS = "http://172.27.207.97/sicofin/api";

    static public int WEB_API_TIMEOUT_SECONDS = 240;

  }  // class TestingConstants


}  // namespace Empiria.FinancialAccounting.Tests
