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
      Assertion.Require(transactionSlipUID, nameof(transactionSlipUID));

      TransactionSlip slips = TransactionSlip.Parse(transactionSlipUID);

      return TransactionSlipMapper.Map(slips);
    }


    public FixedList<TransactionSlipDto> GetTransactionSlipsList(TransactionSlipsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<TransactionSlip> slips = TransactionSlipSearcher.Search(query);

      return TransactionSlipMapper.Map(slips, true);
    }


    public FixedList<TransactionSlipDescriptorDto> SearchTransactionSlips(TransactionSlipsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<TransactionSlip> slips = TransactionSlipSearcher.Search(query);

      return TransactionSlipMapper.MapToDescriptors(slips);
    }


    #endregion Importers

  }  // class TransactionSlipUseCases

}  // Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases
