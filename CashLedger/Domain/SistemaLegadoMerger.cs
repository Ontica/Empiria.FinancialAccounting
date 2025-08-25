/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Service provider                        *
*  Type     : SistemaLegadoMerger                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Servicio para agregar el concepto del sistema legado a objetos CashTransactionEntryDto.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Servicio para agregar el concepto del sistema legado a objetos CashTransactionEntryDto.</summary>
  internal class SistemaLegadoMerger {

    private FixedList<CashEntry> _entries;
    private FixedList<MovimientoSistemaLegado> _movs;

    internal SistemaLegadoMerger(FixedList<CashEntry> entries,
                                 FixedList<MovimientoSistemaLegado> movs) {
      _entries = entries;
      _movs = movs;
    }


    internal void Merge() {
      if (_entries.Count != _movs.Count) {
        return;
      }

      for (int i = 0; i < _entries.Count; i++) {
        var entry = _entries[i];


        if (_movs[i].Disponibilidad == 2) {
          entry.SetCuentaSistemaLegado("Sin flujo");
        } else {
          entry.SetCuentaSistemaLegado(_movs[i].CuentaConcepto.ToString());
        }
      }
    }

  }  // class SistemaLegadoMerger

} // namespace Empiria.FinancialAccounting.CashLedger
