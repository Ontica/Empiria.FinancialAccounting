/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherCreationUseCases                     License   : Please read LICENSE.txt file           *
*                                                                                                            *
*  Summary  : Use cases used to create vouchers and their movements.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  public class VoucherCreationUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherCreationUseCases() {
      // no-op
    }


    static public VoucherCreationUseCases UseCaseInteractor() {
      return CreateInstance<VoucherCreationUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public Voucher CreateVoucher(VoucherCreationFields fields) {
      Assertion.Require(fields, nameof(fields));

      Ledger ledger = GetLedger(fields.LedgerNo);

      VoucherFields voucherFields = MapVoucherFields(fields, ledger);

      FixedList<VoucherEntryFields> entriesFields = MapVoucherEntries(fields, ledger);

      Validate(voucherFields, entriesFields, ledger);

      var voucher = new Voucher(voucherFields, entriesFields);

      voucher.SaveAll();

      return voucher;
    }

    #endregion Use cases

    #region Mappers

    private VoucherFields MapVoucherFields(VoucherCreationFields fields, Ledger ledger) {

      return new VoucherFields {
        Concept = fields.Concept,
        AccountingDate = fields.AccountingDate,
        RecordingDate = DateTime.Today,
        LedgerUID = ledger.UID,
        TransactionTypeUID = TransactionType.ParseWithCode(fields.OperationTypeCode).UID,
        VoucherTypeUID = VoucherType.ParseWithCode(fields.VoucherTypeCode).UID,
        FunctionalAreaId = -1
      };
    }


    private FixedList<VoucherEntryFields> MapVoucherEntries(VoucherCreationFields fields,
                                                            Ledger ledger) {

      var entries = new List<VoucherEntryFields>(fields.Movements.Count);

      foreach (VoucherMovementFields movement in fields.Movements) {

        entries.Add(MapVoucherEntry(fields, movement, ledger));
      }

      return entries.ToFixedList();
    }


    private VoucherEntryFields MapVoucherEntry(VoucherCreationFields voucher,
                                               VoucherMovementFields movement,
                                               Ledger ledger) {

      LedgerAccount account = GetLedgerAccount(ledger, movement.LedgerAccountNo);

      SubledgerAccount subledger = ResolveSubledgerAccount(ledger, movement.SubledgerAccountNo);

      FunctionalArea responsibilityArea = ResolveResponsibilityArea(movement.ResponsibilityAreaCode,
                                                                    voucher.AccountingDate);

      Currency currency = ResolveCurrency(movement.CurrencyISOCode);

      return new VoucherEntryFields {
        LedgerAccountId = account.Id,
        SubledgerAccountId = subledger.Id,
        ResponsibilityAreaId = responsibilityArea.Id,

        BudgetConcept = movement.BudgetAccountCode,
        VerificationNumber = movement.BudgetControlNo,

        VoucherEntryType = GetEntryType(movement),

        Date = voucher.AccountingDate,
        Concept = movement.Concept,

        CurrencyUID = currency.UID,
        Amount = GetAmount(movement),
        ExchangeRate = movement.ExchangeRate,

        BaseCurrencyAmount = GetBaseCurrencyAmount(ledger, currency, movement)
      };
    }

    #endregion Mappers

    #region Helpers

    private decimal GetAmount(VoucherMovementFields movement) {

      return movement.DebitAmount > 0 ? movement.DebitAmount : movement.CreditAmount;
    }


    private decimal GetBaseCurrencyAmount(Ledger ledger,
                                          Currency currency,
                                          VoucherMovementFields movement) {

      decimal amount = GetAmount(movement);

      if (currency.Equals(ledger.BaseCurrency)) {
        return amount;
      }

      return Math.Round(amount * movement.ExchangeRate, 2);
    }


    private VoucherEntryType GetEntryType(VoucherMovementFields movement) {

      if (movement.DebitAmount > 0) {
        Assertion.Require(movement.CreditAmount == 0,
                          "Un movimiento no puede contener cargo y abono simultáneamente.");
        return VoucherEntryType.Debit;
      }

      Assertion.Require(movement.CreditAmount > 0, "El movimiento debe contener un cargo o un abono.");

      return VoucherEntryType.Credit;
    }


    private Ledger GetLedger(string ledgerNo) {
      return AccountsChart.IFRS.GetLedger(ledgerNo);
    }


    private LedgerAccount GetLedgerAccount(Ledger ledger,
                                       string ledgerAccountNo) {

      var standardAccount = AccountsChart.IFRS.GetStandardAccount(ledgerAccountNo);

      LedgerAccount ledgerAccount = ledger.TryGetAccount(standardAccount);

      if (ledgerAccount == null) {
        ledgerAccount = ledger.AssignAccount(standardAccount);
      }

      return ledgerAccount;
    }


    private Currency ResolveCurrency(string isoCode) {

      Currency currency = Currency.TryParseISOCode(isoCode);

      Assertion.Require(currency, $"No está registrada una moneda con el código ISO '{isoCode}'.");

      return currency;
    }


    private FunctionalArea ResolveResponsibilityArea(string areaCode, DateTime accountingDate) {

      if (string.IsNullOrWhiteSpace(areaCode)) {
        return FunctionalArea.Empty;
      }

      FunctionalArea area = FunctionalArea.ParseActive(areaCode, accountingDate);

      Assertion.Require(area, $"No existe el área de responsabilidad '{areaCode}'.");

      return area;
    }


    private SubledgerAccount ResolveSubledgerAccount(Ledger ledger, string accountNo) {

      if (string.IsNullOrWhiteSpace(accountNo)) {
        return SubledgerAccount.Empty;
      }

      SubledgerAccount account = ledger.TryGetSubledgerAccount(accountNo);

      Assertion.Require(account, $"El auxiliar '{accountNo}' no existe en la contabilidad {ledger.FullName}.");

      return account;
    }


    private void Validate(VoucherFields voucherFields,
                          FixedList<VoucherEntryFields> entries,
                          Ledger ledger) {

      voucherFields.EnsureValid();

      var validator = new VoucherValidator(ledger, voucherFields.AccountingDate);

      FixedList<string> issues = validator.Validate(entries);

      Assertion.Require(issues.Count == 0,
                        "No se puede generar la póliza debido a inconsistencias:\n\n" +
                        EmpiriaString.ToString(issues));
    }

    #endregion Helpers

  }  // class VoucherCreationUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
