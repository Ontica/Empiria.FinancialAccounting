/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Core                  Component : Test Helpers                            *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Testing constants                       *
*  Type     : TestingConstants                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides testing constants.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides testing constants.</summary>
  static public class TestingConstants {

    static readonly internal int ACCOUNTS_CHART_COUNT = ConfigurationData.Get<int>("ACCOUNTS_CHART_COUNT");

    static readonly internal string ACCOUNTS_CHART_UID = ConfigurationData.GetString("ACCOUNTS_CHART_UID");

    static readonly internal string ACCOUNTS_CHART_2021_UID = ConfigurationData.GetString("ACCOUNTS_CHART_2021_UID");

    static readonly internal int ACCOUNT_ID = ConfigurationData.Get<int>("ACCOUNT_ID");

    static readonly internal string ACCOUNT_UID = ConfigurationData.GetString("ACCOUNT_UID");

    static readonly internal string ACCOUNT_NAME = ConfigurationData.GetString("ACCOUNT_NAME");

    static readonly internal string ACCOUNT_NUMBER = ConfigurationData.GetString("ACCOUNT_NUMBER");

    static readonly internal string LEDGER_UID = ConfigurationData.GetString("LEDGER_UID");

    static readonly internal int LEDGER_ACCOUNT_ID = ConfigurationData.Get<int>("LEDGER_ACCOUNT_ID");

    static readonly internal string SESSION_TOKEN = ConfigurationData.GetString("SESSION_TOKEN");

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
