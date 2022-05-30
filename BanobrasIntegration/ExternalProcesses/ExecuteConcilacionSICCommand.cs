/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command payload                       *
*  Type     : ExecuteConcilacionSICCommand                License   : Please read LICENSE.txt file           *
*                                                                                                            *
*  Summary  : Command payload used to execute 'Conciliacion SIC' external process (from Marimba).            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Command payload used to execute 'Conciliacion SIC' external process (from Marimba).</summary>
  public class ExecuteConcilacionSICCommand {

    public DateTime FechaInicio {
      get; set;
    }


    public DateTime FechaFin {
      get; set;
    }

  }  // ExecuteConcilacionSICCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
