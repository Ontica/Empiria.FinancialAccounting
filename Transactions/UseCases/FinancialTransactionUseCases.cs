/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Use case interactor class               *
*  Type     : FinancialTransactionUseCases               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for post and manage financial transactions sent from external systems.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.Financial.Transactions.Adapters;

namespace Empiria.FinancialAccounting.Transactions.UseCases {

  /// <summary>Use cases for post and manage financial transactions sent from external systems.</summary>
  public class FinancialTransactionUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialTransactionUseCases() {
      // no-op
    }


    static public FinancialTransactionUseCases UseCaseInteractor() {
      return CreateInstance<FinancialTransactionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public int PostTransaction(FinancialTransactionFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var txn = FinancialTransaction.Create(fields);

      txn.Save();

      return txn.Id;
    }

    #endregion Use cases

  } // class FinancialTransactionUseCases

} // Empiria.FinancialAccounting.Transactions.UseCases
