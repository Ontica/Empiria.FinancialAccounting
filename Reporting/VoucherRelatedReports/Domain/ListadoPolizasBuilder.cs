/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ListadoPolizasBuilder                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Builder de listado de polizas para reporte operativo.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Builder de Polizas para reporte operativo.</summary>
  internal class ListadoPolizasBuilder {


    internal ListadoPolizasBuilder(ListadoPolizasCommand command, FixedList<IPolizaEntry> entries) {
      Assertion.Require(command, "command");
      Assertion.Require(entries, "entries");

      Command = command;
      Entries = entries;
    }


    #region Properties

    public ListadoPolizasCommand Command {
      get;
    }


    public FixedList<IPolizaEntry> Entries {
      get;
    }

    #endregion

  } // class ListadoPolizasBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Builders
