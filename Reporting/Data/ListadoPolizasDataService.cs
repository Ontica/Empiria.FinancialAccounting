/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : ListadoPolizasDataService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for vouchers.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Provides data read methods for vouchers.</summary>
  static internal class ListadoPolizasDataService {

    static internal FixedList<PolizaEntry> GetPolizasEntries(PolizaCommandData command) {
      var operation = DataOperation.Parse("@qryVouchers",
                                          CommonMethods.FormatSqlDate(command.FromDate),
                                          CommonMethods.FormatSqlDate(command.ToDate),
                                          command.Ledgers,
                                          command.AccountsChart.Id);

      return DataReader.GetPlainObjectFixedList<PolizaEntry>(operation);
    }

  } // class ListadoPolizasDataService

} // namespace Empiria.FinancialAccounting.Reporting.Data
