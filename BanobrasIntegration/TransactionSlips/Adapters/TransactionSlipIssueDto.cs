/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : TransactionSlipIssueDto                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO for transaction slips issues.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Output DTO for transaction slips issues.</summary>
  public class TransactionSlipIssueDto {

    internal TransactionSlipIssueDto() {
      // no-op
    }

    public string Description {
      get; internal set;
    }

  }  // class TransactionSlipIssueDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
