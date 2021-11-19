/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : ListadoPolizasHelper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build vouchers information.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Helper methods to build vouchers information.</summary>
  internal class ListadoPolizasHelper {

    private readonly ListadoPolizasCommand _command;

    internal ListadoPolizasHelper(ListadoPolizasCommand command) {
      _command = command;
    }

    internal FixedList<PolizaEntry> GetPolizaEntries() {
      var commandExtensions = new PolizasCommandExtensions();

      PolizaCommandData commandData = commandExtensions.MapToPolizaCommandData(_command);

      FixedList<PolizaEntry> polizas = ListadoPolizasDataService.GetPolizasEntries(commandData);

      return polizas;
    }

  } // class ListadoPolizasHelper

} // namespace Empiria.FinancialAccounting.Reporting
