/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Use case interactor class               *
*  Type     : SistemaLegadoUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Casos de uso para interactuar con transacciones de flujo de efectivo del sistema legado.       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/


using Empiria.FinancialAccounting.CashLedger.Data;

using Empiria.Services;

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

    public void ActualizarTransacciones() {

      FixedList<long> transactionsIds = SistemaLegadoData.TransaccionesSinActualizar();

      foreach (var txnId in transactionsIds) {
        CashTransaction transaction = CashLedgerData.GetTransaction(txnId);

        FixedList<CashEntry> entries = CashLedgerData.GetTransactionEntries(transaction);

        var legacyEntries = SistemaLegadoData.LeerMovimientos(txnId);
        var merger = new SistemaLegadoMerger(entries, legacyEntries);

        merger.Merge();

        SistemaLegadoData.ActualizarMovimientos(entries);
      }

    }

    #endregion Use cases

  }  // class SistemaLegadoUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
