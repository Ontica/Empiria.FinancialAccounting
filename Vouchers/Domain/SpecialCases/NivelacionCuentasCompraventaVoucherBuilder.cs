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

    internal NivelacionCuentasCompraventaVoucherBuilder() {
      // no-op
    }

    internal override FixedList<string> DryRun() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      return ImplementsDryRun(entries);
    }


    internal override Voucher GenerateVoucher() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      FixedList<string> issues = this.ImplementsDryRun(entries);

      Assertion.Assert(issues.Count == 0,
          "There were one or more issues generating 'Nivelación de cuentas de compraventa' voucher: " +
          EmpiriaString.ToString(issues));

      var voucher = new Voucher(base.Fields);

      voucher.Save();

      CreateVoucherEntries(voucher, entries);

      return voucher;
    }


    private void CreateVoucherEntries(Voucher voucher, FixedList<VoucherEntryFields> entries) {
      foreach (var entryFields in entries) {

        entryFields.VoucherId = voucher.Id;

        VoucherEntry voucherEntry = voucher.AppendEntry(entryFields);

        voucherEntry.Save();
      }
    }


    private FixedList<string> ImplementsDryRun(FixedList<VoucherEntryFields> entries) {
      var validator = new VoucherValidator(Ledger.Parse(base.Fields.LedgerUID),
                                           base.Fields.AccountingDate);

      return validator.Validate(entries);
    }


    private TrialBalanceCommand BuildBalancesCommand() {
      return new TrialBalanceCommand {
        TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
        AccountsChartUID = base.Fields.AccountsChartUID,
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = true,
        InitialPeriod = new BalanceEngineCommandPeriod {
          FromDate = base.Fields.CalculationDate,
          ToDate = base.Fields.CalculationDate
        }
      };
    }


    private FixedList<TrialBalanceEntryDto> BuildTrialBalanceEntries() {
      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceCommand command = BuildBalancesCommand();

        var entries = usecases.BuildTrialBalance(command).Entries;

        return new FixedList<TrialBalanceEntryDto>(entries.Select(x => (TrialBalanceEntryDto) x));
      }
    }


    private FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<AccountsListItem> accountsListItems = GetNivelacionAccountsList();

      FixedList<TrialBalanceEntryDto> balances = BuildTrialBalanceEntries();

      FixedList<ExchangeRate> exchangeRates = GetValuationExchangeRates();

      var entries = new List<VoucherEntryFields>();

      foreach (var item in accountsListItems) {
        decimal registeredBalance = GetBalance(item.AccountNumber, balances);

        decimal revaluatedBalance = RevaluateForeignCurrencyBalances(item.TargetAccountNumber, balances, exchangeRates);

        if (registeredBalance != 0 && registeredBalance != revaluatedBalance) {
          entries.AddRange(BuildVoucherEntryFieldsList(item, registeredBalance, revaluatedBalance));
        }
      }

      Assertion.Assert(entries.Count >= 2, "Las cuentas de compraventa ya están niveladas en la fecha proporcionada.");

      return entries.ToFixedList();
    }


    private VoucherEntryFields BuildVoucherEntryFields(string accountNumber, decimal amount, bool isDebit) {
      StandardAccount stdAccount = AccountsChart.Parse(base.Fields.AccountsChartUID)
                                                .GetStandardAccount(accountNumber);

      var ledger = Ledger.Parse(base.Fields.LedgerUID);

      LedgerAccount ledgerAccount = ledger.AssignAccount(stdAccount);

      return new VoucherEntryFields {
        Amount = amount,
        BaseCurrencyAmount = amount,
        CurrencyUID = ledger.BaseCurrency.UID,
        SectorId = Sector.Empty.Id,
        SubledgerAccountNumber = String.Empty,
        StandardAccountId = stdAccount.Id,
        LedgerAccountId = ledgerAccount.Id,
        VoucherEntryType = isDebit ? VoucherEntryType.Debit : VoucherEntryType.Credit
      };
    }


    private List<VoucherEntryFields> BuildVoucherEntryFieldsList(AccountsListItem item,
                                                                 decimal registeredBalance,
                                                                 decimal revaluatedBalance) {
      var entries = new List<VoucherEntryFields>();

      decimal amount = (revaluatedBalance * -1) - registeredBalance;

      if (amount > 0) {
        entries.Add(BuildVoucherEntryFields(item.AccountNumber, amount, true));
        entries.Add(BuildVoucherEntryFields(item.ProfitAccount, amount, false));
      } else if (amount < 0) {
        entries.Add(BuildVoucherEntryFields(item.AccountNumber, Math.Abs(amount), false));
        entries.Add(BuildVoucherEntryFields(item.LossAccount, Math.Abs(amount), true));
      }

      return entries;
    }


    private decimal GetBalance(string accountNumber, FixedList<TrialBalanceEntryDto> balances) {
      var balance = balances.Find(x => x.AccountNumber == accountNumber &&
                                       x.LedgerUID == base.Fields.LedgerUID);

      if (balance == null) {
        return 0;
      }
      return balance.CurrentBalance.Value;
    }


    private FixedList<AccountsListItem> GetNivelacionAccountsList() {
      return base.SpecialCaseType.AccountsList.GetItems();
    }


    private FixedList<ExchangeRate> GetValuationExchangeRates() {
      var exchangeRateType = ExchangeRateType.ValorizacionBanxico;

      return ExchangeRate.GetList(exchangeRateType, base.Fields.CalculationDate);
    }


    private decimal RevaluateForeignCurrencyBalances(string foreignCurrencyAccountNumber,
                                                     FixedList<TrialBalanceEntryDto> balances,
                                                     FixedList<ExchangeRate> exchangeRates) {
      FixedList<TrialBalanceEntryDto> foreignBalances = balances.FindAll(x => x.AccountNumber == foreignCurrencyAccountNumber &&
                                                                              x.LedgerUID == base.Fields.LedgerUID);

      decimal revaluatedForeignBalance = 0;

      foreach (var foreignBalance in foreignBalances) {
        var currency = Currency.Parse(foreignBalance.CurrencyCode);
        var exchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(currency));

        revaluatedForeignBalance += foreignBalance.CurrentBalance.Value * exchangeRate.Value;
      }

      return Math.Round(revaluatedForeignBalance, 2);
    }


  }  // class NivelacionCuentasCompraventaVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
