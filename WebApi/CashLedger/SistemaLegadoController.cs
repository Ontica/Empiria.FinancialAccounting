/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                  Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Web Api Controller                    *
*  Type     : SistemaLegadoController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web APIs para leer y limpiar información proveniente del sistema legado de flujo de efectivo.  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.CashLedger.UseCases;

namespace Empiria.FinancialAccounting.WebApi.CashLedger {

  /// <summary>Web APIs para leer y limpiar información proveniente del sistema legado de flujo de efectivo.</summary>
  public class SistemaLegadoController : WebApiController {

    #region Query web apis


    [HttpGet]
    [Route("v2/financial-accounting/cash-ledger/sistema-lgado/limpiar-transacciones")]
    public NoDataModel LimpiarTransacciones() {

      using (var usecases = SistemaLegadoUseCases.UseCaseInteractor()) {
        usecases.LimpiarTransacciones();

        return new NoDataModel(base.Request);
      }
    }

    #endregion Query web apis

  }  // class SistemaLegadoController

}  // namespace Empiria.FinancialAccounting.WebApi.CashLedger
