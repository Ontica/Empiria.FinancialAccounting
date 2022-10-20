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
using System.Collections.Generic;

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


  public class ValorizacionEntryDto : DynamicValorizacionEntryDto, ITrialBalanceEntryDto {

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


    public decimal ValuedEffects {
      get;
      internal set;
    }

    public decimal TotalValued {
      get;
      internal set;
    }

    public decimal TotalAccumulated {
      get;
      internal set;
    }


    public decimal USD {
      get;
      internal set;
    }

    public decimal YEN {
      get;
      internal set;
    }

    public decimal EUR {
      get;
      internal set;
    }

    public decimal UDI {
      get;
      internal set;
    }


    public decimal LastUSD {
      get;
      internal set;
    }


    public decimal LastYEN {
      get;
      internal set;
    }


    public decimal LastEUR {
      get;
      internal set;
    }


    public decimal LastUDI {
      get;
      internal set;
    }


    public decimal CurrentUSD {
      get;
      internal set;
    }


    public decimal CurrentYEN {
      get;
      internal set;
    }


    public decimal CurrentEUR {
      get;
      internal set;
    }


    public decimal CurrentUDI {
      get;
      internal set;
    }


    public decimal ValuedEffectUSD {
      get;
      internal set;
    }


    public decimal ValuedEffectYEN {
      get;
      internal set;
    }


    public decimal ValuedEffectEUR {
      get;
      internal set;
    }


    public decimal ValuedEffectUDI {
      get;
      internal set;
    }


    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;


    public DateTime LastChangeDate {
      get;
      internal set;
    }


    public DateTime ConsultingDate {
      get;
      private set;
    }


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


    public override IEnumerable<string> GetDynamicMemberNames() {
      List<string> members = new List<string>();

      members.Add("ItemType");
      members.Add("CurrencyCode");
      members.Add("StandardAccountId");
      members.Add("AccountNumber");
      members.Add("AccountName");
      members.Add("USD");
      members.Add("YEN");
      members.Add("EUR");
      members.Add("UDI");
      members.Add("LastUSD");
      members.Add("LastYEN");
      members.Add("LastEUR");
      members.Add("LastUDI");
      members.Add("CurrentUSD");
      members.Add("CurrentYEN");
      members.Add("CurrentEUR");
      members.Add("CurrentUDI");
      members.Add("ValuedEffectUSD");
      members.Add("ValuedEffectYEN");
      members.Add("ValuedEffectEUR");
      members.Add("ValuedEffectUDI");
      members.Add("ValuedEffects");
      members.Add("TotalValued");

      members.AddRange(base.GetDynamicMemberNames());

      members.Add("TotalAccumulated");


      return members;
    }


  } // class ValorizacionEntryDto


  public class DynamicValorizacionEntryDto : DynamicEntry {

  } // class DynamicValorizacionEntryDto


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
