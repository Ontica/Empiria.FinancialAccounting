/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command payload                       *
*  Type     : ExecuteRentabilidadCommand                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to execute 'Rentabilidad' external process (from Marimba).                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Command payload used to execute 'Rentabilidad' external process (from Marimba).</summary>
  public class ExecuteRentabilidadCommand {

    public int Anio {
      get; set;
    }

    public int Mes {
      get; set;
    }

    public int Metodologia {
      get; set;
    }

  }  // class ExecuteRentabilidadCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
