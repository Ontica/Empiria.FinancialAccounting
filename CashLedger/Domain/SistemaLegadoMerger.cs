/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Service provider                        *
*  Type     : SistemaLegadoMerger                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Servicio para agregar el concepto del sistema legado a objetos CashEntry.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.Financial.Integration;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Servicio para agregar el concepto del sistema legado a objetos CashEntry.</summary>
  internal class SistemaLegadoMerger {

    private FixedList<CashEntry> _entries;
    private FixedList<MovimientoSistemaLegado> _legacyEntries;

    internal SistemaLegadoMerger(FixedList<CashEntry> entries,
                                 FixedList<MovimientoSistemaLegado> legacyEntries) {
      _entries = entries;
      _legacyEntries = legacyEntries;
    }


    internal void Merge() {
      if (_entries.Count != _legacyEntries.Count && _legacyEntries.Count == 0) {
        return;
      } else if (_entries.Count != _legacyEntries.Count && _legacyEntries.Count != 0) {
        Assertion.RequireFail($"En el sistema legado la póliza {_entries[0].VoucherId} tiene " +
                              $"{_legacyEntries.Count} movimientos. " +
                              $"En SICOFIN esa misma póliza tiene {_entries.Count}.");
      }

      for (int i = 0; i < _entries.Count; i++) {
        CleanUp(_legacyEntries[i]);

        if (!EqualEntries(_entries[i], _legacyEntries[i])) {
          ReorderLegacyEntries();
          break;
        }
      }


      for (int i = 0; i < _entries.Count; i++) {
        var entry = _entries[i];

        if (_legacyEntries[i].Disponibilidad == 2) {
          entry.SetCuentaSistemaLegado("Sin flujo");
        } else {
          entry.SetCuentaSistemaLegado(_legacyEntries[i].CuentaConcepto.ToString());
        }
      }
    }


    private void CleanUp(MovimientoSistemaLegado legacyEntry) {
      legacyEntry.CuentaContable = IntegrationLibrary.FormatAccountNumber(legacyEntry.CuentaContable);
    }


    private bool EqualEntries(CashEntry cashEntry, MovimientoSistemaLegado legacyEntry) {

      if (cashEntry.Currency.Id != legacyEntry.IdMoneda) {
        return false;
      }
      if (cashEntry.Amount != legacyEntry.Importe) {
        return false;
      }
      if (cashEntry.Debit == 0 && legacyEntry.TipoMovimiento == 1) {
        return false;
      }
      if (cashEntry.Credit == 0 && legacyEntry.TipoMovimiento == 2) {
        return false;
      }
      if (cashEntry.LedgerAccount.Number != legacyEntry.CuentaContable) {
        return false;
      }
      if (cashEntry.HasSubledgerAccount && cashEntry.SubledgerAccount.Number != legacyEntry.Auxiliar) {
        return false;
      }
      if (!cashEntry.HasSubledgerAccount && legacyEntry.Auxiliar.Length != 0) {
        return false;
      }
      return true;
    }


    private void ReorderLegacyEntries() {
      var reordered = new List<MovimientoSistemaLegado>(_entries.Count);

      foreach (CashEntry entry in _entries) {
        var legacyEntry = _legacyEntries.Find(x => !reordered.Contains(x) &&
                                                   ((x.TipoMovimiento == 1 && x.Importe == entry.Debit) ||
                                                    (x.TipoMovimiento == 2 && x.Importe == entry.Credit)) &&
                                                   (x.IdMoneda == entry.Currency.Id && x.CuentaContable == entry.LedgerAccount.Number) &&
                                                   ((entry.HasSubledgerAccount && x.Auxiliar == entry.SubledgerAccount.Number) ||
                                                    (!entry.HasSubledgerAccount && x.Auxiliar.Length == 0)));
        if (legacyEntry != null) {
          reordered.Add(legacyEntry);
        } else {
          Assertion.RequireFail($"En el sistema legado no se encuentra el movimiento {entry.Id} " +
                                $"correspondiente a la póliza {entry.VoucherId} de SICOFIN.");
        }
      }

      _legacyEntries = reordered.ToFixedList();
    }

  }  // class SistemaLegadoMerger

} // namespace Empiria.FinancialAccounting.CashLedger
