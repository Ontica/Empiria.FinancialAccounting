/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Use case interactor class               *
*  Type     : FinancialTransactionUseCases               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to process external financial transactions as accounting transactions.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.Financial.Transactions.Adapters;
using Empiria.Json;

namespace Empiria.FinancialAccounting.Transactions.UseCases {

  /// <summary>Use cases used to process external financial transactions as accounting transactions.</summary>
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

    public int PostTransaction(FinancialTransactionDto fields) {

      JsonObject payload = JsonObject.Parse(fields.Payload);

      EmpiriaLog.Debug($"Payload recibido de {fields.TransactionId}: {fields.Payload.ToString()}");

      return EmpiriaMath.GetRandom(1, 100);
    }

    #endregion Use cases

  } // class FinancialTransactionUseCases

} // Empiria.FinancialAccounting.Transactions.UseCases
