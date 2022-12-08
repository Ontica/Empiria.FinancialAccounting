/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionCuentasResultadosVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels the balances of profit and loss accounts                         *
*             at a given date (cuentas de resultados).                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels the balances of profit and loss
  /// accounts (cuentas de resultados) at a given date.</summary>
  internal class CancelacionCuentasResultadosVoucherBuilder : VoucherBuilder {

    internal CancelacionCuentasResultadosVoucherBuilder() {
      // no-op
    }

    internal override FixedList<string> DryRun() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      return ImplementsDryRun(entries);
    }


    internal override Voucher GenerateVoucher() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      FixedList<string> issues = this.ImplementsDryRun(entries);

      Assertion.Require(issues.Count == 0,
        "There were one or more issues generating 'Cancelación de cuentas de resultados' voucher: " +
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


    private TrialBalanceQuery BuildBalancesQuery() {
      return new TrialBalanceQuery {
        TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
        AccountsChartUID = base.Fields.AccountsChartUID,
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = base.Fields.CalculationDate,
          ToDate = base.Fields.CalculationDate
        }
      };
    }


    private FixedList<BalanzaTradicionalEntryDto> BuildTrialBalanceEntries() {
      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceQuery query = BuildBalancesQuery();

        var entries = usecases.BuildTrialBalance(query)
                              .Entries;

        return entries.Select(x => (BalanzaTradicionalEntryDto) x)
                      .ToFixedList();
      }
    }


    private FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<AccountsListItem> accountsListItems = GetCancelacionResultadosAccountsList();

      FixedList<BalanzaTradicionalEntryDto> balances = BuildTrialBalanceEntries();

      var entries = new List<VoucherEntryFields>();

      foreach (var item in accountsListItems) {
        // Replace this: For each item in accountsListItems, GET last level TrialBalancesEntries list with subledger accounts and sectors
        decimal registeredBalance = GetBalance(item.AccountNumber, balances);

        // for each balance entry, build the voucher entries
        if (registeredBalance != 0) {
          entries.AddRange(BuildVoucherEntryFieldsList(item, registeredBalance));
        }
      }

      Assertion.Require(entries.Count >= 2,
            "No hay saldos de cuentas de resultados por cancelar a la fecha proporcionada.");

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
                                                                 decimal registeredBalance) {
      var entries = new List<VoucherEntryFields>();

      decimal amount = registeredBalance;

      if (amount > 0) {     // Check if Debtor || Creditor. Create an entry for each subledger account and/or sector if applies
        entries.Add(BuildVoucherEntryFields(item.AccountNumber, amount, true));
        entries.Add(BuildVoucherEntryFields(item.TargetAccountNumber, amount, false));
      } else if (amount < 0) {
        entries.Add(BuildVoucherEntryFields(item.AccountNumber, Math.Abs(amount), false));
        entries.Add(BuildVoucherEntryFields(item.TargetAccountNumber, Math.Abs(amount), true));
      }

      return entries;
    }


    private decimal GetBalance(string accountNumber, FixedList<BalanzaTradicionalEntryDto> balances) {
      var balance = balances.Find(x => x.AccountNumber == accountNumber &&
                                       x.LedgerUID == base.Fields.LedgerUID);

      if (balance == null) {
        return 0;
      }
      return balance.CurrentBalance.Value;
    }


    private FixedList<AccountsListItem> GetCancelacionResultadosAccountsList() {
      return base.SpecialCaseType.AccountsList.GetItems();
    }

  }  // class CancelacionCuentasResultadosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
