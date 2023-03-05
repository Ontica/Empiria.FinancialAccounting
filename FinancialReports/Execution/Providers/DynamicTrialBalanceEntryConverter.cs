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

    private readonly FinancialReportType _financialReportType;
    private readonly ExchangeRatesProvider _exchangeRatesProvider;

    internal DynamicTrialBalanceEntryConverter(FinancialReportType financialReportType,
                                               ExchangeRatesProvider exchangeRatesProvider) {
      _financialReportType = financialReportType;
      _exchangeRatesProvider = exchangeRatesProvider;
    }


    internal FixedList<DynamicTrialBalanceEntry> Convert(FixedList<ITrialBalanceEntryDto> sourceEntries) {
      var convertedEntries = new List<DynamicTrialBalanceEntry>(sourceEntries.Count);

      FixedList<string> baseFields =
                _financialReportType.DataColumns.FindAll(x => x.Type == "decimal" && !x.IsCalculated)
                                                .Select(y => y.Field)
                                                .ToFixedList();

      foreach (var entry in sourceEntries) {
        DynamicTrialBalanceEntry converted = Convert(entry, baseFields);

        convertedEntries.Add(converted);
      }

      return convertedEntries.ToFixedList();
    }

    #region Helpers

    private DynamicTrialBalanceEntry Convert(ITrialBalanceEntryDto sourceEntry,
                                             FixedList<string> fields) {

      if (sourceEntry is AnaliticoDeCuentasEntryDto analiticoDeCuentasEntryDto) {
        return Convert(analiticoDeCuentasEntryDto);
      }

      if (sourceEntry is BalanzaColumnasMonedaEntryDto balanzaColumnasMonedaEntryDto) {
        return Convert(balanzaColumnasMonedaEntryDto, fields);
      }

      if (sourceEntry is BalanzaTradicionalEntryDto balanzaTradicionalEntryDto) {
        return Convert(balanzaTradicionalEntryDto);
      }

      if (sourceEntry is ValorizacionEntryDto estimacionPreventivaEntryDto) {
        return Convert(estimacionPreventivaEntryDto);
      }

      throw Assertion.EnsureNoReachThisCode(
          $"A converter has not been defined for trial balance entry type {sourceEntry.GetType().FullName}.");
    }


    private DynamicTrialBalanceEntry Convert(AnaliticoDeCuentasEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("monedaNacional",   sourceEntry.DomesticBalance);
      converted.SetTotalField("monedaExtranjera", sourceEntry.ForeignBalance);

      return converted;
    }


    private DynamicTrialBalanceEntry Convert(BalanzaColumnasMonedaEntryDto sourceEntry,
                                             FixedList<string> fields) {

      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("pesosTotal",   sourceEntry.DomesticBalance);
      converted.SetTotalField("dollarTotal",  sourceEntry.DollarBalance);
      converted.SetTotalField("yenTotal",     sourceEntry.YenBalance);
      converted.SetTotalField("euroTotal",    sourceEntry.EuroBalance);
      converted.SetTotalField("udisTotal",    sourceEntry.UdisBalance);

      if (fields.Contains("dollarMXNTotal")) {
        converted.SetTotalField("dollarMXNTotal",
                                _exchangeRatesProvider.Convert_USD_To_MXN(sourceEntry.DollarBalance, 2));
      }

      if (fields.Contains("yenMXNTotal")) {
        converted.SetTotalField("yenMXNTotal",
                              _exchangeRatesProvider.Convert_YEN_To_MXN(sourceEntry.YenBalance, 2));
      }

      if (fields.Contains("euroMXNTotal")) {
        converted.SetTotalField("euroMXNTotal",
                              _exchangeRatesProvider.Convert_EUR_To_MXN(sourceEntry.EuroBalance, 2));
      }

      if (fields.Contains("udisMXNTotal")) {
        converted.SetTotalField("udisMXNTotal",
                              _exchangeRatesProvider.Convert_UDI_To_MXN(sourceEntry.UdisBalance, 2));
      }

      if (fields.Contains("yenUSDTotal")) {
        converted.SetTotalField("yenUSDTotal",
                              _exchangeRatesProvider.Convert_YEN_To_USD(sourceEntry.YenBalance, 2));
      }

      if (fields.Contains("euroUSDTotal")) {
        converted.SetTotalField("euroUSDTotal",
                              _exchangeRatesProvider.Convert_EUR_To_USD(sourceEntry.EuroBalance, 2));
      }

      return converted;
    }


    private DynamicTrialBalanceEntry Convert(BalanzaTradicionalEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor == DebtorCreditorType.Deudora.ToString() ?
                                              DebtorCreditorType.Deudora : DebtorCreditorType.Acreedora;

      converted.SetTotalField("saldoActual", sourceEntry.CurrentBalanceForBalances);

      return converted;
    }


    private DynamicTrialBalanceEntry Convert(ValorizacionEntryDto sourceEntry) {
      var converted = new DynamicTrialBalanceEntry(sourceEntry);

      converted.DebtorCreditor = sourceEntry.DebtorCreditor;

      converted.SetTotalField("cargosPesos", sourceEntry.MXNDebit);
      converted.SetTotalField("abonosPesos", sourceEntry.MXNCredit);

      converted.SetTotalField("cargosMonExtVal",
            sourceEntry.ValuedUSDDebit + sourceEntry.ValuedEURDebit + sourceEntry.ValuedYENDebit);

      converted.SetTotalField("abonosMonExtVal",
            sourceEntry.ValuedUSDCredit + sourceEntry.ValuedEURCredit + sourceEntry.ValuedYENCredit);

      converted.SetTotalField("cargosUDIsVal", sourceEntry.ValuedUDIDebit);
      converted.SetTotalField("abonosUDIsVal", sourceEntry.ValuedUDICredit);


      converted.SetTotalField("cargosEfectosValuacionMonExt",
            sourceEntry.ValuedEffectUSD + sourceEntry.ValuedEffectEUR + sourceEntry.ValuedEffectYEN);

      converted.SetTotalField("abonosEfectosValuacionMonExt",
            (sourceEntry.ValuedEffectUSD + sourceEntry.ValuedEffectEUR + sourceEntry.ValuedEffectYEN) * -1m);

      converted.SetTotalField("cargosEfectosValuacionUDIs", sourceEntry.ValuedEffectUDI);

      converted.SetTotalField("abonosEfectosValuacionUDIs", sourceEntry.ValuedEffectUDI * -1m);

      return converted;
    }

    #endregion Helpers

  }  // class DynamicTrialBalanceEntryConverter

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
