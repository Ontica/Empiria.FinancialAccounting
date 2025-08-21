/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Use case interactor class               *
*  Type     : SistemaLegadoUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Casos de uso para interactuar con transacciones de flujo de efectivo del sistema legado.       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.Financial.Integration.CashLedger;

using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.UseCases {

  /// <summary>Casos de uso para interactuar con transacciones de flujo de efectivo del sistema legado.</summary>
  public class SistemaLegadoUseCases : UseCase {

    #region Constructors and parsers

    protected SistemaLegadoUseCases() {
      // no-op
    }

    static public SistemaLegadoUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SistemaLegadoUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<MovimientoSistemaLegado> LeerMovimientos(long idPoliza) {
      return SistemaLegadoData.LeerMovimientos(idPoliza);
    }


    public void LimpiarTransacciones() {
      SistemaLegadoData.LimpiarTransacciones();
    }

    #endregion Use cases

  }  // class SistemaLegadoUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
