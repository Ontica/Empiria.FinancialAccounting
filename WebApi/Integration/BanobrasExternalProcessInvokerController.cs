/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : External Processes Invoker            *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : BanobrasExternalProcessInvokerController     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Invoca procesos de otros sistemas pero que se ejecutan en SICOFIN (Marimba).                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Invoca procesos de otros sistemas pero que se ejecutan en SICOFIN (Marimba).</summary>
  public class BanobrasExternalProcessInvokerController : WebApiController {

    #region Processes


    [HttpPost]
    [Route("v2/financial-accounting/integration/external-processes/procesar-conciliacion-sic")]
    public SingleObjectModel ProcesarConciliacionSIC([FromBody] ConcilacionSICExternalProcessCommand command) {

      base.RequireBody(command);

      using (var service = ExternalProcessInvoker.ServiceInteractor()) {
        string result = service.ProcesarConciliacionSIC(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/integration/external-processes/procesar-rentabilidad")]
    public SingleObjectModel ProcesarRentabilidad([FromBody] RentabilidadExternalProcessCommand command) {

      base.RequireBody(command);

      using (var service = ExternalProcessInvoker.ServiceInteractor()) {
        string result = service.ProcesarRentabilidad(command);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Processes

  }  // class BanobrasExternalProcessInvokerController

}  // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
