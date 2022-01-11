/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : TransactionSlipDto                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO for transaction slips.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Output DTO for transaction slips.</summary>
  public class TransactionSlipDto {

    internal TransactionSlipDto() {
      // no-op
    }


    public TransactionSlipDescriptorDto Header {
      get;
      internal set;
    }


    public FixedList<TransactionSlipEntryDto> Entries {
      get;
      internal set;
    }


    public FixedList<TransactionSlipIssueDto> Issues {
      get;
      internal set;
    }


    public NamedEntityDto Voucher {
      get;
      internal set;
    }


  }  // class TransactionSlipDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
