/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : DynamicTrialBalanceEntryConverter          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to convert trial balance entries to their dynamical representation.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides services to convert trial balance entries to their dynamical representation.</summary>
  internal class DynamicTrialBalanceEntryConverter {


    internal DynamicTrialBalanceEntryConverter() {
      // ToDo: Use convertion rules for dynamic fields names
    }


    internal FixedList<DynamicTrialBalanceEntryDto> Convert(FixedList<ITrialBalanceEntryDto> sourceEntries) {
      var convertedEntries = new List<DynamicTrialBalanceEntryDto>(sourceEntries.Count);

      foreach (var entry in sourceEntries) {
        DynamicTrialBalanceEntryDto converted = Convert(entry);

        convertedEntries.Add(converted);
      }

      return convertedEntries.ToFixedList();
    }

    #region Helpers

    private DynamicTrialBalanceEntryDto Convert(ITrialBalanceEntryDto sourceEntry) {
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


    private DynamicTrialBalanceEntryDto Convert(AnaliticoDeCuentasEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntryDto(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("monedaNacional", sourceEntry.DomesticBalance);
      converted.SetTotalField("monedaExtranjera", sourceEntry.ForeignBalance);

      return converted;
    }


    private DynamicTrialBalanceEntryDto Convert(BalanzaColumnasMonedaEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntryDto(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("pesosTotal", sourceEntry.DomesticBalance);

      converted.SetTotalField("dollarTotal", sourceEntry.DollarBalance);

      converted.SetTotalField("yenTotal", sourceEntry.YenBalance);

      converted.SetTotalField("euroTotal", sourceEntry.EuroBalance);

      converted.SetTotalField("udisTotal", sourceEntry.UdisBalance);

      return converted;
    }


    private DynamicTrialBalanceEntryDto Convert(BalanzaTradicionalEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntryDto(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor == DebtorCreditorType.Deudora.ToString() ?
                                              DebtorCreditorType.Deudora : DebtorCreditorType.Acreedora;

      converted.SetTotalField("saldoActual", sourceEntry.CurrentBalanceForBalances);

      return converted;
    }

    #endregion Helpers

  }  // class DynamicTrialBalanceEntryConverter

}  // namespace Empiria.FinancialAccounting.FinancialReports
