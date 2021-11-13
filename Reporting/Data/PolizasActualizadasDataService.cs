/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : AccountBalanceDataService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for trial balances.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting.Data {
  static internal class PolizasActualizadasDataService {

    static internal FixedList<PolizaActualizadaEntry> GetPolizasEntries(PolizasCommand command) {
      var operation = DataOperation.Parse("@");

      EmpiriaLog.Debug(operation.AsText());

      return DataReader.GetPlainObjectFixedList<PolizaActualizadaEntry>(operation);
    }

  }
}
