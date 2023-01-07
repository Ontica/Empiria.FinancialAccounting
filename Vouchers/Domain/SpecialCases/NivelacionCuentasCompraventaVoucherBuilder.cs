/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : NivelacionCuentasCompraventaVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that mantains the right valued balance for a list of accounts pairs,          *
*             typically accounts handled in different currencies at different times.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that mantains the right valued balance for a list of accounts pairs,
  /// typically accounts handled in different currencies at different times.</summary>
  internal class NivelacionCuentasCompraventaVoucherBuilder : VoucherBuilder {

    internal NivelacionCuentasCompraventaVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }

    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<AccountsListItem> accountsListItems =  base.SpecialCaseType.AccountsList.GetItems();

      FixedList<BalanzaTradicionalEntryDto> balances = GetBalances();

      FixedList<ExchangeRate> exchangeRates = GetValuationExchangeRates();

      var entries = new List<VoucherEntryFields>();

      foreach (var item in accountsListItems) {
        decimal registeredBalance = GetAccountBalance(item.AccountNumber, balances);

        decimal revaluatedBalance = RevaluateForeignCurrencyBalances(item.TargetAccountNumber, balances, exchangeRates);

        if (registeredBalance != 0 && registeredBalance != revaluatedBalance) {
          entries.AddRange(BuildVoucherEntryFieldsList(item, registeredBalance, revaluatedBalance));
        }
      }

      Assertion.Require(entries.Count >= 2, "Las cuentas de compraventa ya están niveladas en la fecha proporcionada.");

      return entries.ToFixedList();
    }

    #endregion Abstract Implements

    #region Helpers


    private VoucherEntryFields BuildVoucherEntryFields(VoucherEntryType voucherEntryType, string accountNumber, decimal amount) {
      StandardAccount stdAccount = base.AccountsChart.GetStandardAccount(accountNumber);

      LedgerAccount ledgerAccount = base.Ledger.AssignAccount(stdAccount);

      return new VoucherEntryFields {
        Amount = amount,
        BaseCurrencyAmount = amount,
        CurrencyUID = base.Ledger.BaseCurrency.UID,
        SectorId = Sector.Empty.Id,
        SubledgerAccountNumber = String.Empty,
        StandardAccountId = stdAccount.Id,
        LedgerAccountId = ledgerAccount.Id,
        VoucherEntryType = voucherEntryType
      };
    }


    private List<VoucherEntryFields> BuildVoucherEntryFieldsList(AccountsListItem item,
                                                                 decimal registeredBalance,
                                                                 decimal revaluatedBalance) {
      var entries = new List<VoucherEntryFields>();

      decimal amount = (revaluatedBalance * -1) - registeredBalance;

      if (amount > 0) {
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Debit, item.AccountNumber, amount));
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Credit, item.ProfitAccount, amount));
      } else if (amount < 0) {
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Credit, item.AccountNumber, Math.Abs(amount)));
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Debit, item.LossAccount, Math.Abs(amount)));
      }

      return entries;
    }


    private decimal GetAccountBalance(string accountNumber, FixedList<BalanzaTradicionalEntryDto> balances) {
      var balance = balances.Find(x => x.AccountNumber == accountNumber);

      if (balance == null) {
        return 0;
      }
      return balance.CurrentBalance.Value;
    }


    private FixedList<BalanzaTradicionalEntryDto> GetBalances() {
      var query = new TrialBalanceQuery {
        TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
        AccountsChartUID = base.AccountsChart.UID,
        Ledgers = new[] { base.Ledger.UID },
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = true,
        UseCache = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = base.Fields.CalculationDate,
          ToDate = base.Fields.CalculationDate
        }
      };

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        var entries = usecases.BuildTrialBalance(query)
                              .Entries;

        return entries.Select(x => (BalanzaTradicionalEntryDto) x)
                      .ToFixedList();
      }
    }


    private FixedList<ExchangeRate> GetValuationExchangeRates() {
      var exchangeRateType = ExchangeRateType.ValorizacionBanxico;

      return ExchangeRate.GetList(exchangeRateType, base.Fields.CalculationDate);
    }


    private decimal RevaluateForeignCurrencyBalances(string foreignCurrencyAccountNumber,
                                                     FixedList<BalanzaTradicionalEntryDto> balances,
                                                     FixedList<ExchangeRate> exchangeRates) {

      FixedList<BalanzaTradicionalEntryDto> foreignBalances =
                        balances.FindAll(x => x.AccountNumber == foreignCurrencyAccountNumber);

      decimal revaluatedForeignBalance = 0;

      foreach (var foreignBalance in foreignBalances) {
        var currency = Currency.Parse(foreignBalance.CurrencyCode);
        var exchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(currency));

        revaluatedForeignBalance += foreignBalance.CurrentBalance.Value * exchangeRate.Value;
      }

      return Math.Round(revaluatedForeignBalance, 2);
    }

    #endregion Helpers

  }  // class NivelacionCuentasCompraventaVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
