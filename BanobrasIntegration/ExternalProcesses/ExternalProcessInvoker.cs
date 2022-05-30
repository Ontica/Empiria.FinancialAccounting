/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : ExternalProcessInvoker                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Ejecutor de servicios de otros sistemas de Banobras desde SICOFIN (Marimba).                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.UseCases;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Ejecutor de servicios de otros sistemas de Banobras desde SICOFIN (Marimba).</summary>
  public class ExternalProcessInvoker : Service {

    #region Constructors and parsers

    protected ExternalProcessInvoker() {
      // no-op
    }

    static public ExternalProcessInvoker ServiceInteractor() {
      return Service.CreateInstance<ExternalProcessInvoker>();
    }

    #endregion Constructors and parsers


    public string ProcesarConciliacionSIC(ExecuteConcilacionSICCommand command) {
      Assertion.Require(command, "command");

      ExportarSaldosSIC(command.FechaInicio, command.FechaFin);

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


    public string ProcesarRentabilidad(ExecuteRentabilidadCommand command) {
      Assertion.Require(command, "command");

      EmpiriaLog.Info("Iniciando la exportación de saldos para el proceso de rentabilidad.");

      ExportarSaldosRentabilidad(command);

      EmpiriaLog.Info("Terminó la exportación de saldos del proceso de rentabilidad.");

      try {
        ExternalProcessDataServices.ProcesarRentabilidad(command);

        EmpiriaLog.Info("El proceso de rentabilidad fue ejecutado satisfactoriamente.");

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


    #region Helper methods


    private void ExportarSaldosRentabilidad(ExecuteRentabilidadCommand command) {
      var balancesCommand = new ExportBalancesCommand {
        AccountsChartId = command.Metodologia,
        BreakdownLedgers = true,
        FromDate = new DateTime(command.Anio, command.Mes, 1),
        ToDate = new DateTime(command.Anio, command.Mes,
                              DateTime.DaysInMonth(command.Anio, command.Mes)),
        StoreInto = StoreBalancesInto.SaldosRentabilidad
      };

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {
        usecases.Export(balancesCommand);
      }
    }


    private void ExportarSaldosSIC(DateTime fechaInicio, DateTime fechaFin) {
      var command = new ExportBalancesCommand {
        AccountsChartId = AccountsChart.IFRS.Id,
        BreakdownLedgers = true,
        FromDate = fechaInicio,
        ToDate = fechaFin,
        StoreInto = StoreBalancesInto.SaldosSIC
      };

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {
        usecases.Export(command);
      }
    }

    #endregion Helper methods

  }  // class ExternalProcessInvoker

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
