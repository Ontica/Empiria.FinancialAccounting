/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : StoredVoucherDataService                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for vouchers by account.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Provides data read methods for vouchers by account.</summary>
  static internal class AccountStatementDataService {

    static internal FixedList<AccountStatementEntry> GetVouchersByAccountEntries(
                                                        AccountStatementCommandData commandData) {

      var operation = DataOperation.Parse("@qryVouchersByAccount",
                                          commandData.AccountsChartId,
                                          CommonMethods.FormatSqlDate(commandData.FromDate),
                                          CommonMethods.FormatSqlDate(commandData.ToDate),
                                          commandData.Filters,
                                          commandData.Fields,
                                          commandData.Grouping);

      return DataReader.GetPlainObjectFixedList<AccountStatementEntry>(operation);
    }

  } // class StoredVoucherDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
