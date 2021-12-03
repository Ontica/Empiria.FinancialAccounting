/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.Vouchers.Tests           Pattern   : Testing constants                     *
*  Type     : TestingConstants                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides testing constants.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides testing constants.</summary>
  static public class TestingConstants {

    static readonly internal string SESSION_TOKEN = ConfigurationData.GetString("SESSION_TOKEN");

    static readonly internal string LEDGER_UID = ConfigurationData.GetString("LEDGER_UID");

    static readonly internal long VOUCHER_ID = ConfigurationData.Get<long>("VOUCHER_ID");

  }  // class TestingConstants

}  // namespace Empiria.FinancialAccounting.Tests
