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

    static readonly internal string ACCOUNTS_CHART_UID = ConfigurationData.GetString("ACCOUNTS_CHART_UID");

    static readonly internal string ACCOUNTS_CHART_2021_UID = ConfigurationData.GetString("ACCOUNTS_CHART_2021_UID");

    static readonly internal int ACCOUNT_ID = ConfigurationData.Get<int>("ACCOUNT_ID");

    static readonly internal string ACCOUNT_NAME = ConfigurationData.GetString("ACCOUNT_NAME");

    static readonly internal string ACCOUNT_NUMBER = ConfigurationData.GetString("ACCOUNT_NUMBER");

    static readonly internal string SESSION_TOKEN = ConfigurationData.GetString("SESSION_TOKEN");

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
