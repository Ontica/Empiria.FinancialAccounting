/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Use cases                            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Use case interactor class            *
*  Type     : TransactionSlipUseCases                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases used for retrive information about Banobras operating systems' transaction slips.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.UseCases {

  /// <summary>Use cases used for retrive information about Banobras operating
  /// systems' transaction slips.</summary>
  public class TransactionSlipUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionSlipUseCases() {
      // no-op
    }

    static public TransactionSlipUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionSlipUseCases>();
    }

    #endregion Constructors and parsers


    #region Importers

    public TransactionSlipDto GetTransactionSlip(string transactionSlipUID) {
      Assertion.AssertObject(transactionSlipUID, "transactionSlipUID");

      TransactionSlip transactionSlip = TransactionSlip.Parse(transactionSlipUID);

      return TransactionSlipMapper.Map(transactionSlip);
    }


    public FixedList<TransactionSlipDto> GetTransactionSlipsList(SearchTransactionSlipsCommand command) {
      Assertion.AssertObject(command, "command");

      FixedList<TransactionSlip> transactionSlipsList = TransactionSlipSearcher.Search(command);

      return TransactionSlipMapper.Map(transactionSlipsList, true);
    }


    public FixedList<TransactionSlipDescriptorDto> SearchTransactionSlips(SearchTransactionSlipsCommand command) {
      Assertion.AssertObject(command, "command");

      FixedList<TransactionSlip> transactionSlipsList = TransactionSlipSearcher.Search(command);

      return TransactionSlipMapper.MapToDescriptors(transactionSlipsList);
    }


    #endregion Importers

  }  // class TransactionSlipUseCases

}  // Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases
