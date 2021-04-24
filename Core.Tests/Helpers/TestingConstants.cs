/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Core                  Component : Test Helpers                            *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Testing constants                       *
*  Type     : TestingConstants                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides testing constants.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides testing constants.</summary>
  static public class TestingConstants {

    static internal decimal ACCOUNT_BALANCE = 2000;

    static internal int ACCOUNT_ID => 4;

    static internal string ACCOUNT_NAME => "Billetes";

    static internal string ACCOUNT_NUMBER => "1101-01";

    static internal string SESSION_TOKEN => ConfigurationData.GetString("Testing.SessionToken");

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
