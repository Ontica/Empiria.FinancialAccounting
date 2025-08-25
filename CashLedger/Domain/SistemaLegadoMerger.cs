/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Service provider                        *
*  Type     : SistemaLegadoMerger                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Servicio para agregar el concepto del sistema legado a objetos CashEntry.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Financial.Integration;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Servicio para agregar el concepto del sistema legado a objetos CashEntry.</summary>
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
        if (!MatchEntries(_entries[i], _movs[i])) {
          return;
        }
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


    private bool MatchEntries(CashEntry cashEntry, MovimientoSistemaLegado entrySistemaLegado) {
      if (cashEntry.VoucherId != entrySistemaLegado.IdPoliza) {
        return false;
      }
      if (cashEntry.Currency.Id != entrySistemaLegado.IdMoneda) {
        return false;
      }
      if (cashEntry.Debit == 0 && entrySistemaLegado.TipoMovimiento == 1) {
        return false;
      }
      if (cashEntry.Credit == 0 && entrySistemaLegado.TipoMovimiento == 2) {
        return false;
      }
      if (cashEntry.Amount != entrySistemaLegado.Importe) {
        return false;
      }
      if (cashEntry.SubledgerAccount.Number != entrySistemaLegado.Auxiliar) {
        return false;
      }
      if (cashEntry.LedgerAccount.Number != IntegrationLibrary.FormatAccountNumber(entrySistemaLegado.CuentaContable)) {
        return false;
      }
      return true;
    }

  }  // class SistemaLegadoMerger

} // namespace Empiria.FinancialAccounting.CashLedger
