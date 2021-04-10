/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test Helpers                            *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Testing constants                       *
*  Type     : TestingConstants                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides testing constants.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides testing constants.</summary>
  static public class TestingConstants {

    static internal decimal ACCOUNT_BALANCE = 2000;

    static internal string ACCOUNT_NAME => "Bancos Nacionales";

    static internal string ACCOUNT_NUMBER => "1000-01-01";

    static internal string SESSION_TOKEN => ConfigurationData.GetString("Testing.SessionToken");

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
