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

    //private List<RealTransaction> RealTrasactions {
    //  get; set;
    //} = new List<RealTransaction>();


    #endregion Properties

    #region Methods

    public void GroupTransactions() {

      var accountingOperations = AccountingOperation.GetList().FindAll(x => x.DebitAccount == "1.05.04.02");

      foreach (var accountingOperation in accountingOperations) {
        var debitAccounts = FindDebitAccounts(accountingOperation);

        foreach (var debitAccount in debitAccounts) {
          Group(debitAccount, accountingOperation);
        }

      }
    }

    #endregion Methods

    #region Helpers

    private void Group(VoucherEntryDescriptorDto cuenta, AccountingOperation operation) {

      //var cuenta = FindDebitAccount(operation);
      //if (cuenta == null)
      //  return;

      var contraCuenta = FindCreditAccount(operation, cuenta);
      if (contraCuenta == null)
        return;

      var transactionsGroup = new List<VoucherEntryDescriptorDto> {
            cuenta, contraCuenta
      };

      var IdMoneda = Convert.ToInt32(cuenta.Currency.Substring(0, 2));

      if (IdMoneda == 44) {

        var cuentaNueveUDISDebe = FindAccount("9.01.03", cuenta, isDebit: true);
        if (cuentaNueveUDISDebe == null) {
          var transactions1 = GenerateTransaction(transactionsGroup, operation);
          UpdateTransaction(transactions1);
          return;
        }


        var cuentaNueveUDISHaber = FindAccount("9.01.04", cuenta, isDebit: false);
        if (cuentaNueveUDISHaber == null)
          return;

        transactionsGroup.Add(cuentaNueveUDISDebe);
        transactionsGroup.Add(cuentaNueveUDISHaber);
      } else {

        var cuentaNueveDebe = FindAccount("9.01.01", cuenta, isDebit: true);
        if (cuentaNueveDebe == null) {
          var transactions2 = GenerateTransaction(transactionsGroup, operation);
          UpdateTransaction(transactions2);
          return;
        }

        var cuentaNueveHaber = FindAccount("9.01.02", cuenta, isDebit: false);
        if (cuentaNueveHaber == null)
          return;

        transactionsGroup.Add(cuentaNueveDebe);
        transactionsGroup.Add(cuentaNueveHaber);
      }


      var transactions = GenerateTransaction(transactionsGroup, operation);
      UpdateTransaction(transactions);


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

    private FixedList<VoucherEntryDescriptorDto> FindDebitAccounts(AccountingOperation operation) {
      return _transactions.FindAll(x => x.AccountNumber == operation.DebitAccount);
    }


    private List<RealTransaction> GenerateTransaction(List<VoucherEntryDescriptorDto> transactionsGroup, AccountingOperation operation) {
      string uid = Guid.NewGuid().ToString();
      var realTrasactions = new List<RealTransaction>();


      foreach (var transaction in transactionsGroup) {
        var realTransaction = new RealTransaction(uid, operation, transaction);
        realTrasactions.Add(realTransaction);

        _transactions.Remove(transaction);
      }

      realTrasactions[0].IdMonedaReal = Convert.ToInt32(realTrasactions[0].Transaction.Currency.Substring(0, 2));
      realTrasactions[0].MontoReal = realTrasactions[0].Transaction.Debit;

      realTrasactions[1].IdMonedaReal = Convert.ToInt32(realTrasactions[0].Transaction.Currency.Substring(0, 2));
      realTrasactions[1].MontoReal = realTrasactions[0].Transaction.Debit;

      return realTrasactions;
    }


    private void UpdateTransaction(List<RealTransaction> realTransactions) {
      foreach (var transaction in realTransactions) {
        AccountingOperationDataService.UpdateTransaction(transaction);
      }

    }

    #endregion Helperes

  } // class TransactionReclasificator

} // namespace Empiria.FinancialAccounting.Reclassification
