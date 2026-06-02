/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : AccountingOperation                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to reclassify transactions.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/


using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.Reclassification.Data;
using Empiria.FinancialAccounting.Vouchers.Adapters;


namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Used to reclassify transactions.</summary>
  public class TransactionReclasificator {

    #region Constructors and Parsers

    private TransactionReclasificator() {
      // Required by Empiria Framework
    }

    internal TransactionReclasificator(VoucherDto voucherDto) {
      _transactions = voucherDto.Entries;
    }

    #endregion Constructors and Parsers

    #region Properties
    private FixedList<VoucherEntryDescriptorDto> _transactions {
      get; set;
    }

    private List<RealTransaction> RealTrasactions {
      get; set;
    } = new List<RealTransaction>();


    #endregion Properties

    #region Methods

    public Boolean GroupTransactions() {

      AccountingOperation accountingOperation = AccountingOperation.GetList().Find(x => x.DebitAccount == "1.05.04.02");
      Group(accountingOperation);
      UpdateTransaction();


      return false;
    }

    #endregion Methods

    #region Helpers

    private void Group(AccountingOperation operation) {

      var cuenta = FindDebitAccount(operation);
      if (cuenta == null)
        return;

      var contraCuenta = FindCreditAccount(operation, cuenta);
      if (contraCuenta == null)
        return;

      var transactionsGroup = new List<VoucherEntryDescriptorDto> {
            cuenta, contraCuenta
      };


      var cuentaNueveDebe = FindAccount("9.01.01", cuenta, isDebit: true);
      if (cuentaNueveDebe == null) {
        GenerateTransaction(transactionsGroup, operation);
        return;
      }


      var cuentaNueveHaber = FindAccount("9.01.02", cuenta, isDebit: false);
      if (cuentaNueveHaber == null)
        return;

      transactionsGroup.Add(cuentaNueveDebe);
      transactionsGroup.Add(cuentaNueveHaber);

      GenerateTransaction(transactionsGroup, operation);
    }


    private bool AmountsMatch(decimal a, decimal b) {
      return Math.Round(a, 2, MidpointRounding.AwayFromZero) ==
             Math.Round(b, 2, MidpointRounding.AwayFromZero);
    }


    private VoucherEntryDescriptorDto FindAccount(string accountNumber,
                                                  VoucherEntryDescriptorDto cuenta,
                                                  bool isDebit) {
      return _transactions.Find(x =>
          x.AccountNumber == accountNumber &&
          AmountsMatch(
              isDebit ? x.Debit * x.ExchangeRate : x.Credit * x.ExchangeRate,
              cuenta.ExchangeRate * cuenta.Debit
          ));
    }


    private VoucherEntryDescriptorDto FindCreditAccount(AccountingOperation operation,
                                                       VoucherEntryDescriptorDto cuenta) {
      return _transactions.Find(x =>
          x.AccountNumber == operation.CreditAccount &&
          x.SubledgerAccountNumber == cuenta.SubledgerAccountNumber &&
          AmountsMatch(x.Credit * x.ExchangeRate, cuenta.ExchangeRate * cuenta.Debit));
    }


    private VoucherEntryDescriptorDto FindDebitAccount(AccountingOperation operation) {
      return _transactions.Find(x => x.AccountNumber == operation.DebitAccount);
    }


    private void GenerateTransaction(List<VoucherEntryDescriptorDto> transactionsGroup, AccountingOperation operation) {
      string uid = Guid.NewGuid().ToString();

      foreach (var transaction in transactionsGroup) {
        var realTransaction = new RealTransaction(uid, operation, transaction);
        RealTrasactions.Add(realTransaction);

        _transactions.Remove(transaction);
      }

      RealTrasactions[0].IdMonedaReal = Convert.ToInt32(RealTrasactions[0].Transaction.Currency.Substring(0, 2));
      RealTrasactions[0].MontoReal = RealTrasactions[0].Transaction.Debit;

      RealTrasactions[1].IdMonedaReal = Convert.ToInt32(RealTrasactions[0].Transaction.Currency.Substring(0, 2));
      RealTrasactions[1].MontoReal = RealTrasactions[0].Transaction.Debit;
    }


    private void UpdateTransaction() {
      foreach (var transaction in RealTrasactions) {
        AccountingOperationDataService.UpdateTransaction(transaction);
      }

    }

    #endregion Helperes

  } // class TransactionReclasificator

} // namespace Empiria.FinancialAccounting.Reclassification
