/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                          Component : Services Layer                 *
*  Assembly : FinancialAccounting.AccountingRules.dll             Pattern   : Service class                  *
*  Type     : FinancialTransactionVoucherGenerationServices       License   : Please read LICENSE.txt file   *
*                                                                                                            *
*  Summary  : Services for financial transactions voucher generation using accounting rules.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.FinancialAccounting.Transactions;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.AccountingRules.Processor.Services {

  /// <summary>Services for financial transactions voucher generation using accounting rules.</summary>
  public class FinancialTransactionVoucherGenerationServices : Service {

    #region Constructors and parsers

    protected FinancialTransactionVoucherGenerationServices() {
      // no-op
    }

    static public FinancialTransactionVoucherGenerationServices ServicesInteractor() {
      return CreateInstance<FinancialTransactionVoucherGenerationServices>();
    }

    #endregion Constructors and parsers

    #region Services

    public int GenerateVoucher(FinancialTransaction transaction) {

      Assertion.Require(transaction, nameof(transaction));

      AccountingRuleProcessingResult processingResult = ProcessTransaction(transaction);

      if (processingResult.HasPendingItems) {
        return -1;
      }

      VoucherCreationFields voucherFields = BuildVoucherCreationFields(transaction, processingResult);

      using (var usecases = VoucherCreationUseCases.UseCaseInteractor()) {

        Voucher voucher = usecases.CreateVoucher(voucherFields);

        return voucher.Id;
      }
    }


    #endregion Services

    #region Helpers

    private VoucherCreationFields BuildVoucherCreationFields(FinancialTransaction transaction,
                                                             AccountingRuleProcessingResult processingResult) {

      var guide = AccountingGuide.ParseWithCode(transaction.TransactionType.Key);

      var area = transaction.Payload.Get<string>("orgUnitCode");

      if (area.StartsWith("2")) {
        area = area.Substring(1, 2);
      } else {
        area = "09";
      }

      return new VoucherCreationFields {
        LedgerNo = area,
        SourceSystemCode = guide.SourceSystemCode,
        OperationTypeCode = guide.OperationTypeCode,
        VoucherTypeCode = guide.VoucherTypeCode,
        AccountingDate = transaction.ApplicationDate,
        Concept = transaction.Description,
        Movements = processingResult.Movements
      };
    }


    private AccountingRuleProcessingResult ProcessTransaction(FinancialTransaction transaction) {
      switch (transaction.TransactionType.Key) {

        case "PROVISION_DE_PAGO":
          return new PaymentProvisionProcessor().Process(transaction);

        case "PAGO":
          return new PaymentTransactionProcessor().Process(transaction);

        default:
          throw Assertion.EnsureNoReachThisCode(
              $"Unsupported financial transaction type '{transaction.TransactionType.Key}'.");
      }
    }

    #endregion Helpers

  }  // class FinancialTransactionVoucherGenerationServices

}  // namespace Empiria.FinancialAccounting.AccountingRules.Processor.Services
