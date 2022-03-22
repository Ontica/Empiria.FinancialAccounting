/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : ListadoPolizasPorCuentaDataService         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for vouchers by account.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary></summary>
  static class ListadoPolizasPorCuentaDataService {


    static internal FixedList<AccountStatementEntry> GetVouchersByAccountEntries(
                                                        PolizaCommandData commandData) {

      var operation = DataOperation.Parse("@qryVouchersByAccount",
                                          commandData.AccountsChart.Id,
                                          CommonMethods.FormatSqlDate(commandData.FromDate),
                                          CommonMethods.FormatSqlDate(commandData.ToDate),
                                          commandData.Filters);

      return DataReader.GetPlainObjectFixedList<AccountStatementEntry>(operation);
    }


  } // class ListadoPolizasPorCuentaDataService

} // namespace Empiria.FinancialAccounting.Reporting.Data
