/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ListadoPolizasPorCuentaBuilder                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Builder de listado de polizas por cuenta para reporte operativo.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Builder de listado de polizas por cuenta para reporte operativo.</summary>
  internal class ListadoPolizasPorCuentaBuilder {


    internal ListadoPolizasPorCuentaBuilder(BuildReportCommand command, FixedList<IVouchersByAccountEntry> entries) {
      Assertion.Require(command, "command");
      Assertion.Require(entries, "entries");

      Command = command;
      Entries = entries;
    }


    #region Properties

    public BuildReportCommand Command {
      get;
    }

    public FixedList<IVouchersByAccountEntry> Entries {
      get;
    }
    #endregion

  } // class ListadoPolizasPorCuentaBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Builders
