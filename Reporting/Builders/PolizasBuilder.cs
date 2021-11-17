/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : PolizasBuilder                                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Builder de Polizas para reporte operativo.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Builder de Polizas para reporte operativo.</summary>
  internal class PolizasBuilder {


    internal PolizasBuilder(PolizasCommand command, FixedList<IPolizaEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      Command = command;
      Entries = entries;
    }



    #region Properties

    public PolizasCommand Command {
      get;
    }


    public FixedList<IPolizaEntry> Entries {
      get;
    }

    #endregion
  } // class PolizasBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Builders
