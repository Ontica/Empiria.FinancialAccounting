/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : DynamicTrialBalanceEntryConverter          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Converts trial balance entries to DynamicTrialBalanceEntry objects with dynamic fields.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Converts trial balance entries to DynamicTrialBalanceEntry objects with dynamic fields.</summary>
  internal class DynamicTrialBalanceEntryConverter {

    internal DynamicTrialBalanceEntryConverter() {
      // ToDo: Use convertion rules for dynamic fields names
    }


    internal FixedList<DynamicTrialBalanceEntry> Convert(FixedList<ITrialBalanceEntryDto> sourceEntries) {
      var convertedEntries = new List<DynamicTrialBalanceEntry>(sourceEntries.Count);

      foreach (var entry in sourceEntries) {
        DynamicTrialBalanceEntry converted = Convert(entry);

        convertedEntries.Add(converted);
      }

      return convertedEntries.ToFixedList();
    }

    #region Helpers

    private DynamicTrialBalanceEntry Convert(ITrialBalanceEntryDto sourceEntry) {
      if (sourceEntry is AnaliticoDeCuentasEntryDto analiticoDeCuentasEntryDto) {
        return Convert(analiticoDeCuentasEntryDto);
      }

      if (sourceEntry is BalanzaColumnasMonedaEntryDto balanzaColumnasMonedaEntryDto) {
        return Convert(balanzaColumnasMonedaEntryDto);
      }

      if (sourceEntry is BalanzaTradicionalEntryDto balanzaTradicionalEntryDto) {
        return Convert(balanzaTradicionalEntryDto);
      }

      throw Assertion.EnsureNoReachThisCode(
          $"A converter has not been defined for trial balance entry type {sourceEntry.GetType().FullName}.");
    }


    private DynamicTrialBalanceEntry Convert(AnaliticoDeCuentasEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("monedaNacional", sourceEntry.DomesticBalance);
      converted.SetTotalField("monedaExtranjera", sourceEntry.ForeignBalance);

      return converted;
    }


    private DynamicTrialBalanceEntry Convert(BalanzaColumnasMonedaEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("pesosTotal", sourceEntry.DomesticBalance);

      converted.SetTotalField("dollarTotal", sourceEntry.DollarBalance);

      converted.SetTotalField("yenTotal", sourceEntry.YenBalance);

      converted.SetTotalField("euroTotal", sourceEntry.EuroBalance);

      converted.SetTotalField("udisTotal", sourceEntry.UdisBalance);

      return converted;
    }


    private DynamicTrialBalanceEntry Convert(BalanzaTradicionalEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor == DebtorCreditorType.Deudora.ToString() ?
                                              DebtorCreditorType.Deudora : DebtorCreditorType.Acreedora;

      converted.SetTotalField("saldoActual", sourceEntry.CurrentBalanceForBalances);

      return converted;
    }

    #endregion Helpers

  }  // class DynamicTrialBalanceEntryConverter

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
