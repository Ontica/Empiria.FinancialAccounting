/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : CoreBalanceEntries                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains a set of CoreBlancesEntry objects.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains a set of CoreBlancesEntry objects.</summary>
  public class CoreBalanceEntries {

    #region Constructors and parsers

    private readonly TrialBalanceQuery _query;
    private readonly Lazy<FixedList<CoreBalanceEntry>> _entries;

    public CoreBalanceEntries(TrialBalanceQuery query, ExchangeRateType exchangeRateType) {
      _query = query;
      _entries = new Lazy<FixedList<CoreBalanceEntry>>(() => ReloadEntries(exchangeRateType));
    }


    public CoreBalanceEntries(ExchangeRateType exchangeRateType, TrialBalanceQuery query) {
      _query = query;
      CoreEntries = ReloadEntries(exchangeRateType);
    }

    #endregion Constructors and parsers

    #region Properties

    internal FixedList<CoreBalanceEntry> Entries {
      get {
        return _entries.Value;
      }
    }


    internal FixedList<CoreBalanceEntry> CoreEntries {
      get; set;
    }

    #endregion Properties

    #region Methods

    public FixedList<CoreBalanceEntry> GetBalancesByAccount(string accountNumber) {

      return Entries.FindAll(x => x.Account.Number.StartsWith(accountNumber))
                    .ToFixedList();
    }


    public FixedList<CoreBalanceEntry> GetCoreEntriesByAccount(string accountNumber) {

      return CoreEntries.FindAll(x => x.Account.Number.StartsWith(accountNumber))
                    .ToFixedList();
    }


    internal FixedList<CoreBalanceEntry> GetBalancesByAccountAndSector(string accountNumber,
                                                                       string sectorCode) {
      if (sectorCode == "00") {
        return Entries.FindAll(x => x.Account.Number.StartsWith(accountNumber))
                       .ToFixedList();

      } else {
        return Entries.FindAll(x => x.Account.Number.StartsWith(accountNumber) &&
                                    x.Sector.Code == sectorCode)
                      .ToFixedList();
      }
    }

    #endregion Methods

    #region Helpers

    private FixedList<CoreBalanceEntry> ReloadEntries(ExchangeRateType exchangeRateType) {

      FixedList<CoreBalanceEntry> balances = CoreBalanceEntryDataServices.GetBalanceEntries(_query);

      if (!exchangeRateType.IsEmptyInstance) {
        Valuate(balances, exchangeRateType);
      }

      return balances;
    }


    private void Valuate(FixedList<CoreBalanceEntry> entries, ExchangeRateType exchangeRateType) {

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   _query.InitialPeriod.ToDate);
      foreach (var entry in entries.FindAll(x => x.Currency.Distinct(Currency.MXN))) {

        var exchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(entry.Currency));

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} en la fecha proporcionada.");

        entry.ValuateTo(exchangeRate.Value);
      }
    }

    #endregion Helpers

  } // class CoreBalanceEntries

} // namespace Empiria.FinancialAccounting.BalanceEngine
