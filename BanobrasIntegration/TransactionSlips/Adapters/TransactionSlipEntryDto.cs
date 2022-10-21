/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : TransactionSlipEntryDto                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO for a transaction slip entry (movimiento en volante).                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Output DTO for a transaction slip entry (movimiento en volante).</summary>
  public class TransactionSlipEntryDto {

    internal TransactionSlipEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public long EntryNumber {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public string SubledgerAccount {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string FunctionalArea {
      get; internal set;
    }


    public string VerificationNumber {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }

    public FixedList<TransactionSlipIssueDto> Issues {
      get; internal set;
    } = new FixedList<TransactionSlipIssueDto>();


  }  // class TransactionSlipEntryDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
