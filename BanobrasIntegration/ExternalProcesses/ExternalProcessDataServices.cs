/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Data Service                          *
*  Type     : ExternalProcessDataServices                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Invokes Banobras' external processes through stored procedures calls (Marimba).                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Invokes Banobras' external processes through stored procedures calls (Marimba).</summary>
  static internal class ExternalProcessDataServices {

    static internal void ProcesarRentabilidad(RentabilidadExternalProcessCommand command) {
      var dataOperation = DataOperation.Parse("do_procesar_rentabilidad",
                                              command.Anio, command.Mes, command.Metodologia);
      DataWriter.Execute(dataOperation);
    }

  }  // class ExternalProcessDataServices

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
