/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : ExternalProcessInvoker                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web api para ejecutar servicios de otros sistemas de Banobras desde SICOFIN (Marimba).         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Web api para ejecutar servicios de otros sistemas de Banobras desde SICOFIN (Marimba).</summary>
  public class ExternalProcessInvoker : Service {

    #region Constructors and parsers

    private ExternalProcessInvoker() {
      // no-op
    }

    static public ExternalProcessInvoker ServiceInteractor() {
      return Service.CreateInstance<ExternalProcessInvoker>();
    }

    #endregion Constructors and parsers



    public string ProcesarConciliacionSIC(ConcilacionSICExternalProcessCommand command) {
      Assertion.AssertObject(command, "command");

      try {
        ExternalProcessDataServices.ProcesarConcilacionSIC(command);

        return "El proceso de concilación con SIC fue ejecutado satisfactoriamente.";

      } catch (Exception e) {
        if (e.InnerException != null) {
          EmpiriaLog.Error(e.InnerException);
          throw e.InnerException;

        } else {
          EmpiriaLog.Error(e);
          throw;
        }
      }  // catch
    }


    public string ProcesarRentabilidad(RentabilidadExternalProcessCommand command) {
      Assertion.AssertObject(command, "command");

      try {
        ExternalProcessDataServices.ProcesarRentabilidad(command);

        return "El proceso de rentabilidad fue ejecutado satisfactoriamente.";

      } catch (Exception e) {
        if (e.InnerException != null) {
          EmpiriaLog.Error(e.InnerException);
          throw e.InnerException;

        } else {
          EmpiriaLog.Error(e);
          throw;
        }
      }  // catch
    }

  }  // class ExternalProcessInvoker

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
