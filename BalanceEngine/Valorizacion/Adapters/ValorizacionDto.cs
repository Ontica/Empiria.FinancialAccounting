/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : ValorizacionDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a valorization report                                 *
*             with foreign currencies totals and UDIS                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return the entries of a valorization report
  /// with foreign currencies totals and UDIS</summary>
  public class ValorizacionDto {

    
    public TrialBalanceQuery Query {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<ValorizacionEntryDto> Entries {
      get; internal set;
    }


  } // class ValorizacionDto


  public class ValorizacionEntryDto : ITrialBalanceEntryDto {

    public string CurrencyCode {
      get; internal set;
    }

    public int StandardAccountId {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public decimal InitialBalance {
      get;
      internal set;
    }

    public decimal CurrentBalance {
      get;
      internal set;
    }

    public decimal ValuedEffects {
      get;
      internal set;
    }

    public decimal TotalValued {
      get;
      internal set;
    }

    public decimal TotalBalance {
      get;
      internal set;
    }


    public decimal DollarBalance {
      get;
      internal set;
    }

    public decimal YenBalance {
      get;
      internal set;
    }

    public decimal EuroBalance {
      get;
      internal set;
    }

    public decimal UdisBalance {
      get;
      internal set;
    }


    public decimal ExchangeRate {
      get;
      internal set;
    }

    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;

    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    string ITrialBalanceEntryDto.SubledgerAccountNumber {
      get {
        return String.Empty;
      }
    }

  } // class ValorizacionEntryDto


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
