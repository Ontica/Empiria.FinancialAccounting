﻿/* Empiria Financial *****************************************************************************************
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


    internal DynamicTrialBalanceEntryDto Convert(ITrialBalanceEntryDto sourceEntry) {
      if (sourceEntry is AnaliticoDeCuentasEntryDto analiticoDeCuentasDto) {
        return this.Convert(analiticoDeCuentasDto);
      }

      throw Assertion.EnsureNoReachThisCode(
          $"A converter has not been defined for trial balance entry type {sourceEntry.GetType().FullName}.");
    }


    internal DynamicTrialBalanceEntryDto Convert(AnaliticoDeCuentasEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntryDto(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("monedaNacional", sourceEntry.DomesticBalance);
      converted.SetTotalField("monedaExtranjera", sourceEntry.ForeignBalance);

      return converted;
    }


    internal FixedList<DynamicTrialBalanceEntryDto> Convert(FixedList<ITrialBalanceEntryDto> sourceEntries) {
      var convertedEntries = new List<DynamicTrialBalanceEntryDto>(sourceEntries.Count);

      foreach (var entry in sourceEntries) {
        DynamicTrialBalanceEntryDto converted = this.Convert(entry);

        convertedEntries.Add(converted);
      }

      return convertedEntries.ToFixedList();
    }

  }  // class DynamicTrialBalanceEntryConverter

}  // namespace Empiria.FinancialAccounting.FinancialReports
