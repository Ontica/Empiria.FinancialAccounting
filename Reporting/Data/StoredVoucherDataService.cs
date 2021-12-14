/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Data Service                            *
*  Type     : StoredVoucherDataService                  License   : Please read LICENSE.txt file             *
*                                                                                                            *
*  Summary  : Provides data read methods for vouchers by account.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Provides data read methods for vouchers by account.</summary>
  static internal class StoredVoucherDataService {

    static internal FixedList<VouchersByAccountEntry> GetVouchersByAccountEntries(
                                                        VouchersByAccountCommandData commandData) {

      var operation = DataOperation.Parse("@qryVouchersByAccount",
                                          commandData.AccountsChartId,
                                          CommonMethods.FormatSqlDate(commandData.FromDate),
                                          CommonMethods.FormatSqlDate(commandData.ToDate),
                                          commandData.Filters);

      return DataReader.GetPlainObjectFixedList<VouchersByAccountEntry>(operation);
    }

  } // class StoredVoucherDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
